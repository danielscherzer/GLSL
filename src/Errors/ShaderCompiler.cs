using DMS.GLSL.Contracts;
using GLSLhelper;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace DMS.GLSL.Errors
{
	[Export]
	[PartCreationPolicy(CreationPolicy.Shared)] //default singleton behavior
	internal class ShaderCompiler
	{
		[ImportingConstructor]
		public ShaderCompiler(ICompilerSettings settings, ILogger logger)
		{
			this.settings = settings;
			this.logger = logger;
		}

		internal delegate void OnCompilationFinished(IEnumerable<ShaderLogLine> errorLog);

		internal void RequestCompile(string shaderCode, string sShaderType, OnCompilationFinished compilationFinishedHandler, string documentDir)
		{
			StartGlThreadOnce();
			while (compileRequests.TryTake(out _)) ; //remove pending compiles
			var data = new CompileData
			{
				ShaderCode = shaderCode,
				ShaderType = sShaderType,
				DocumentDir = documentDir,
				CompilationFinished = compilationFinishedHandler
			};
			compileRequests.TryAdd(data); //put compile on request list
		}

		private struct CompileData
		{
			public string ShaderCode { get; set; }
			public string ShaderType { get; set; }
			public OnCompilationFinished CompilationFinished { get; set; }
			public string DocumentDir { get; set; }
		}

		private Task taskGL;
		private readonly BlockingCollection<CompileData> compileRequests = new BlockingCollection<CompileData>();

		private void StartGlThreadOnce()
		{
			if (!(taskGL is null)) return;
			//start up GL task for doing shader compilations in background
			taskGL = Task.Factory.StartNew(TaskGlAction, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
		}

		private void TaskGlAction()
		{
			//create a game window for rendering context, until run is called it is invisible so no problem
			var context = new GameWindow(1, 1);
			while (!compileRequests.IsAddingCompleted)
			{
				var compileData = compileRequests.Take(); //block until compile requested
				var expandedCode = ExpandedCode(compileData.ShaderCode, compileData.DocumentDir);
				var log = Compile(expandedCode, compileData.ShaderType, logger, settings);
				var errorLog = new ShaderLogParser(log);
				if (!string.IsNullOrWhiteSpace(log) && settings.PrintCompilationResult)
				{
					logger.Log(log, false);
				}
				compileData.CompilationFinished?.Invoke(errorLog.Lines);
			}
		}

		private static string AutoDetectShaderContentType(string shaderCode)
		{
			if (shaderCode.Contains("EmitVertex")) return ShaderContentTypes.Geometry;
			if (shaderCode.Contains("local_size_")) return ShaderContentTypes.Compute;
			if (shaderCode.Contains(") out;")) return ShaderContentTypes.TessellationControl;
			if (shaderCode.Contains("gl_TessLevel")) return ShaderContentTypes.TessellationControl;
			if (shaderCode.Contains("gl_TessCoord")) return ShaderContentTypes.TessellationEvaluation;
			if (shaderCode.Contains("gl_Position")) return ShaderContentTypes.Vertex;
			if (shaderCode.Contains(") in;")) return ShaderContentTypes.TessellationEvaluation;
			return ShaderContentTypes.Fragment;
		}

		private static string ExpandedCode(string shaderCode, string shaderFileDir, HashSet<string> includedFiles = null)
		{
			if (includedFiles is null)
			{
				includedFiles = new HashSet<string>();
			}

			string SpecialCommentReplacement(string code, string specialComment)
			{
				var lines = code.Split(new[] { '\n' }, StringSplitOptions.None); //if UNIX style line endings still working so do not use Envirnoment.NewLine
				for (int i = 0; i < lines.Length; ++i)
				{
					var index = lines[i].IndexOf(specialComment); // search for special comment
					if (-1 != index)
					{
						lines[i] = lines[i].Substring(index + specialComment.Length); // remove everything before special comment
					}
				}
				return string.Join("\n", lines);
			}

			string GetIncludeCode(string includeName)
			{
				var includeFileName = Path.GetFullPath(Path.Combine(shaderFileDir, includeName));

				if (File.Exists(includeFileName))
				{
					var includeCode = File.ReadAllText(includeFileName);

					if (includedFiles.Contains(includeFileName))
					{
						return includeCode;
					}
					includedFiles.Add(includeFileName);

					return ExpandedCode(includeCode, Path.GetDirectoryName(includeFileName), includedFiles: includedFiles);
				}
				return $"#error include file '{includeName}' not found";
			}

			shaderCode = SpecialCommentReplacement(shaderCode, "//!");
			if (includedFiles.Count == 0)
			{
				shaderCode = SpecialCommentReplacement(shaderCode, "//?");
			}
			return Transformations.ExpandIncludes(shaderCode, GetIncludeCode);
		}

		private static string Compile(string shaderCode, string shaderContentType, ILogger logger, ICompilerSettings settings)
		{
			if(ShaderContentTypes.AutoDetect == shaderContentType)
			{
				shaderContentType = AutoDetectShaderContentType(shaderCode);
				logger.Log($"{DateTime.Now:HH.mm.ss.fff} Auto detecting shader type to '{shaderContentType}'", true);
			}
			if (string.IsNullOrWhiteSpace(settings.ExternalCompilerExeFilePath))
			{
				return CompileOnGPU(shaderCode, shaderContentType, logger);
			}
			else
			{
				return CompileExternal(shaderCode, shaderContentType, logger, settings);
			}
		}

		private static string CompileExternal(string shaderCode, string shaderContentType, ILogger logger, ICompilerSettings settings)
		{
			//create temp shader file for external compiler
			var tempPath = Path.GetTempPath();
			var shaderFileName = Path.Combine(tempPath, $"shader{ShaderContentTypes.DefaultFileExtension(shaderContentType)}");
			try
			{
				File.WriteAllText(shaderFileName, shaderCode);
				using (var process = new Process())
				{
					process.StartInfo.FileName = settings.ExternalCompilerExeFilePath;
					var arguments = VsExpand.EnvironmentVariables(settings.ExternalCompilerArguments);
					process.StartInfo.Arguments = $"{arguments} {shaderFileName}"; //arguments
					process.StartInfo.WorkingDirectory = tempPath;
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
					process.StartInfo.CreateNoWindow = true; //do not display a windows
					logger.Log($"Using external compiler '{settings.ExternalCompilerExeFilePath}' with arguments '{arguments}' on temporal shader file '{shaderFileName}'", true);
					process.Start();
					process.WaitForExit(10000);
					var output = process.StandardOutput.ReadToEnd(); //The output result
					return output.Replace(shaderFileName, string.Empty); //HACK: glslLangValidator produces inconsistent error message format when using Vulkan vs GLSL compilation
				}
			}
#pragma warning disable CA1031 // Do not catch general exception types
			catch (Exception e)
			{
				var message = "Error executing external compiler with message\n" + e.ToString();
				logger.Log(message, true);
				return string.Empty;
			}
#pragma warning restore CA1031 // Do not catch general exception types
		}

		[HandleProcessCorruptedStateExceptions]
		private static string CompileOnGPU(string shaderCode, string shaderType, ILogger logger)
		{
			// detect shader type
			if (!mappingContentTypeToShaderType.TryGetValue(shaderType, out ShaderType glShaderType))
			{
				logger.Log($"Unsupported shader type '{shaderType}' by OpenTK shader compiler. Use an external compiler", true);
			}
			try
			{
				var id = GL.CreateShader(glShaderType);
				if (0 == id) throw new Exception($"Could not create {shaderType} instance.");
				GL.ShaderSource(id, shaderCode);
				GL.CompileShader(id);
				GL.GetShader(id, ShaderParameter.CompileStatus, out int status_code);
				string log = string.Empty;
				if (1 != status_code)
				{
					log = GL.GetShaderInfoLog(id);
				}
				GL.DeleteShader(id);
				return log;
			}
#pragma warning disable CA1031 // Do not catch general exception types
			catch (AccessViolationException)
			{
				return $"(1 1):ERROR: OpenGL shader compiler has crashed";
			}
#pragma warning restore CA1031 // Do not catch general exception types
		}

		private static readonly IReadOnlyDictionary<string, ShaderType> mappingContentTypeToShaderType = new Dictionary<string, ShaderType>()
		{
			[ShaderContentTypes.Fragment] = ShaderType.FragmentShader,
			[ShaderContentTypes.Vertex] = ShaderType.VertexShader,
			[ShaderContentTypes.Geometry] = ShaderType.GeometryShader,
			[ShaderContentTypes.TessellationControl] = ShaderType.TessControlShader,
			[ShaderContentTypes.TessellationEvaluation] = ShaderType.TessEvaluationShader,
			[ShaderContentTypes.Compute] = ShaderType.ComputeShader,
		};

		private readonly ICompilerSettings settings;
		private readonly ILogger logger;
	}
}