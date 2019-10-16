using DMS.GLSL.Classification;
using DMS.GLSL.Options;
using GLSLhelper;
using OpenTK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Zenseless.OpenGL;
using ShaderType = Zenseless.HLGL.ShaderType;

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
			
			while (compileRequests.TryTake(out CompileData dataOld)) ; //remove pending compiles
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
		private BlockingCollection<CompileData> compileRequests = new BlockingCollection<CompileData>();

		private static ShaderType AutoDetectShaderType(string shaderCode)
		{
			if(shaderCode.Contains("EmitVertex")) return ShaderType.GeometryShader;
			if (shaderCode.Contains("local_size_")) return ShaderType.ComputeShader;
			if (shaderCode.Contains(") out;")) return ShaderType.TessControlShader;
			if (shaderCode.Contains("gl_TessLevel")) return ShaderType.TessControlShader;
			if (shaderCode.Contains("gl_TessCoord")) return ShaderType.TessEvaluationShader;
			if (shaderCode.Contains("gl_Position")) return ShaderType.VertexShader;
			if (shaderCode.Contains(") in;")) return ShaderType.TessEvaluationShader;
			return ShaderType.FragmentShader;
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
				if (OptionsPagePackage.Options.PrintCompilationResult)
				{
					OutMessage.OutputWindowPane(log);
				}
				var errorLog = new ShaderLogParser(log);
				compileData.CompilationFinished?.Invoke(errorLog.Lines);
			}
		}

		private static string ExpandedCode(string shaderCode, string shaderFileDir)
		{
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
				var includeFileName = Path.Combine(shaderFileDir, includeName);
				if (File.Exists(includeFileName))
				{
					var includeCode = File.ReadAllText(includeFileName);
					includeCode = SpecialCommentReplacement(includeCode, "//!");
					return includeCode;
				}
				return $"#error include file '{includeName}' not found";
			}

			shaderCode = SpecialCommentReplacement(shaderCode, "//!");
			shaderCode = SpecialCommentReplacement(shaderCode, "//?");
			return Transformations.ExpandIncludes(shaderCode, GetIncludeCode);
		}

		private static string Compile(string shaderCode, string shaderType)
		{
			var options = OptionsPagePackage.Options;
			if(!string.IsNullOrWhiteSpace(options.ExternalCompilerExeFilePath))
			{
				//create temp shader file for external compiler
				var shaderFileName = Path.Combine(Path.GetTempPath(), $"shader{ShaderContentTypes.DefaultFileExtension(shaderType)}");
				try
				{
					File.WriteAllText(shaderFileName, shaderCode);
					using (var process = new Process())
					{
						process.StartInfo.FileName = options.ExternalCompilerExeFilePath;
						process.StartInfo.Arguments = $"{options.ExternalCompilerArguments} {shaderFileName}"; //arguments
						process.StartInfo.UseShellExecute = false;
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
						process.StartInfo.CreateNoWindow = true; //do not display a windows
						OutMessage.StatusBar($"Using external compiler '{Path.GetFileNameWithoutExtension(options.ExternalCompilerExeFilePath)}' with arguments '{options.ExternalCompilerArguments}' on temporal shader file '{shaderFileName}'");
						process.Start();
						process.WaitForExit(10000);
						var output = process.StandardOutput.ReadToEnd(); //The output result
						return output.Replace(shaderFileName, "0"); //HACK: glslLangValidator produces inconsistent error message format when using vulkan vs glsl compilation
					}
				}
				catch(Exception e)
				{
					var message = "Error executing external compiler with message\n" + e.ToString();
					OutMessage.StatusBar(message);
				}
			}
			OutMessage.StatusBar("Using driver compiler");
			return CompileOnGPU(shaderCode, shaderType);
		}

		[HandleProcessCorruptedStateExceptions]
		private static string CompileOnGPU(string shaderCode, string sShaderType)
		{
			if (!mappingContentTypeToShaderType.TryGetValue(sShaderType, out ShaderType shaderType))
			{
				var time = DateTime.Now.ToString("HH.mm.ss.fff");
				if (ShaderContentTypes.AutoDetect == sShaderType)
				{
					shaderType = AutoDetectShaderType(shaderCode);
					OutMessage.StatusBar($"{time} Auto detecting shader type to '{shaderType}'");
				}
				else
				{
					OutMessage.StatusBar($"{time} Unsupported shader type '{sShaderType}' by OpenTK shader compiler. Use an external compiler");
				}
			}
			try
			{
				using (var shader = new ShaderGL(shaderType))
				{
					shader.Compile(shaderCode);
					return shader.Log;
				}
			}
			catch (AccessViolationException)
			{
				return $"(1 1):ERROR: OpenGL shader compiler has crashed";
			}
		}

		private static IReadOnlyDictionary<string, ShaderType> mappingContentTypeToShaderType = new Dictionary<string, ShaderType>()
		{
			[ShaderContentTypes.Fragment] = ShaderType.FragmentShader,
			[ShaderContentTypes.Vertex] = ShaderType.VertexShader,
			[ShaderContentTypes.Geometry] = ShaderType.GeometryShader,
			[ShaderContentTypes.TessellationControl] = ShaderType.TessControlShader,
			[ShaderContentTypes.TessellationEvaluation] = ShaderType.TessEvaluationShader,
			[ShaderContentTypes.Compute] = ShaderType.ComputeShader,
		};
	}
}