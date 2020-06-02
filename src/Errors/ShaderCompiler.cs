using DMS.GLSL.Classification;
using DMS.GLSL.Options;
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

		private static string AutoDetectShaderContentType(string shaderCode)
		{
			if (shaderCode.Contains("EmitVertex")) return ShaderContentTypesGenerated.Geometry;
			if (shaderCode.Contains("local_size_")) return ShaderContentTypesGenerated.Compute;
			if (shaderCode.Contains(") out;")) return ShaderContentTypesGenerated.TessellationControl;
			if (shaderCode.Contains("gl_TessLevel")) return ShaderContentTypesGenerated.TessellationControl;
			if (shaderCode.Contains("gl_TessCoord")) return ShaderContentTypesGenerated.TessellationEvaluation;
			if (shaderCode.Contains("gl_Position")) return ShaderContentTypesGenerated.Vertex;
			if (shaderCode.Contains(") in;")) return ShaderContentTypesGenerated.TessellationEvaluation;
			return ShaderContentTypesGenerated.Fragment;
		}

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
				var log = Compile(expandedCode, compileData.ShaderType);
				var errorLog = new ShaderLogParser(log);
				if (!string.IsNullOrWhiteSpace(log) && OptionsPagePackage.Options.PrintCompilationResult)
				{
					OutMessage.OutputWindowPane(log);
				}
				compileData.CompilationFinished?.Invoke(errorLog.Lines);
			}
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

		private static string Compile(string shaderCode, string shaderContentType)
		{
			if(ShaderContentTypesGenerated.AutoDetect == shaderContentType)
			{
				shaderContentType = AutoDetectShaderContentType(shaderCode);
				OutMessage.PaneAndBar($"{DateTime.Now:HH.mm.ss.fff} Auto detecting shader type to '{shaderContentType}'");
			}
			var externalCompiler = OptionsPagePackage.Options.ExternalCompilerExeFilePath;
			if (File.Exists(externalCompiler))
			{
				return CompileExternal(shaderCode, shaderContentType);
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(externalCompiler))
				{
					OutMessage.PaneAndBar($"External compiler '{externalCompiler}' not found using GPU");
				}
				return CompileOnGPU(shaderCode, shaderContentType);
			}
		}

		private static string CompileExternal(string shaderCode, string shaderContentType)
		{
			var options = OptionsPagePackage.Options;
			//create temp shader file for external compiler
			var tempPath = Path.GetTempPath();
			var shaderFileName = Path.Combine(tempPath, $"shader{ShaderContentTypesGenerated.DefaultFileExtension(shaderContentType)}");
			try
			{
				File.WriteAllText(shaderFileName, shaderCode);
				using (var process = new Process())
				{
					process.StartInfo.FileName = options.ExternalCompilerExeFilePath;
					var arguments = Environment.ExpandEnvironmentVariables(options.ExternalCompilerArguments);
					process.StartInfo.Arguments = $"{arguments} {shaderFileName}"; //arguments
					process.StartInfo.WorkingDirectory = tempPath;
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
					process.StartInfo.CreateNoWindow = true; //do not display a windows
					OutMessage.PaneAndBar($"Using external compiler '{Path.GetFileNameWithoutExtension(options.ExternalCompilerExeFilePath)}' with arguments '{options.ExternalCompilerArguments}' on temporal shader file '{shaderFileName}'");
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
				OutMessage.PaneAndBar(message);
				return string.Empty;
			}
#pragma warning restore CA1031 // Do not catch general exception types
		}

		[HandleProcessCorruptedStateExceptions]
		private static string CompileOnGPU(string shaderCode, string shaderType)
		{
			// detect shader type
			if (!mappingContentTypeToShaderType.TryGetValue(shaderType, out ShaderType glShaderType))
			{
				OutMessage.PaneAndBar($"Unsupported shader type '{shaderType}' by OpenTK shader compiler. Use an external compiler");
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
			[ShaderContentTypesGenerated.Fragment] = ShaderType.FragmentShader,
			[ShaderContentTypesGenerated.Vertex] = ShaderType.VertexShader,
			[ShaderContentTypesGenerated.Geometry] = ShaderType.GeometryShader,
			[ShaderContentTypesGenerated.TessellationControl] = ShaderType.TessControlShader,
			[ShaderContentTypesGenerated.TessellationEvaluation] = ShaderType.TessEvaluationShader,
			[ShaderContentTypesGenerated.Compute] = ShaderType.ComputeShader,
		};
	}
}