using DMS.GLSL.Classification;
using DMS.GLSL.Options;
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
using Zenseless.HLGL;
using Zenseless.OpenGL;
using Zenseless.Patterns;
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
			IReadOnlyDictionary<string, ShaderType> mappingContentTypeToShaderType = new Dictionary<string, ShaderType>()
			{
				[ContentTypesGlsl.FragmentShader] = ShaderType.FragmentShader,
				[ContentTypesGlsl.VertexShader] = ShaderType.VertexShader,
				[ContentTypesGlsl.GeometryShader] = ShaderType.GeometryShader,
				[ContentTypesGlsl.TessControlShader] = ShaderType.TessControlShader,
				[ContentTypesGlsl.TessEvaluationShader] = ShaderType.TessEvaluationShader,
				[ContentTypesGlsl.ComputeShader] = ShaderType.ComputeShader,
			};

			StartGlThreadOnce();
			//conversion
			if (!mappingContentTypeToShaderType.TryGetValue(sShaderType, out ShaderType shaderType)) shaderType = AutoDetectShaderType(shaderCode);
			
			while (compileRequests.TryTake(out CompileData dataOld)) ; //remove pending compiles
			var data = new CompileData
			{
				ShaderCode = shaderCode,
				ShaderType = shaderType,
				DocumentDir = documentDir,
				CompilationFinished = compilationFinishedHandler
			};
			compileRequests.TryAdd(data); //put compile on request list
		}

		private struct CompileData
		{
			public string ShaderCode { get; set; }
			public ShaderType ShaderType { get; set; }
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
				var errorLog = new ShaderLog(log);
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
				return lines.Combine("\n");
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
			return ShaderLoader.ResolveIncludes(shaderCode, GetIncludeCode);
		}

		private static string Compile(string shaderCode, ShaderType shaderType)
		{
			var options = OptionsPagePackage.Options;
			if(!string.IsNullOrWhiteSpace(options.ExternalCompilerExeFilePath))
			{
				//create temp shader file for external compiler
				var shaderFileName = GetShaderFileName(shaderType);
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
						VsStatusBar.SetText($"Using external compiler '{Path.GetFileNameWithoutExtension(options.ExternalCompilerExeFilePath)}' with arguments '{options.ExternalCompilerArguments}'");
						process.Start();
						process.WaitForExit(10000);
						var output = process.StandardOutput.ReadToEnd(); //The output result
						return output.Replace(shaderFileName, "0"); //HACK: glslLangValidator produces inconsistent error message format when using vulkan vs glsl compilation
					}
				}
				catch(Exception e)
				{
					var message = "Error executing external compiler with message\n" + e.ToString();
					VsStatusBar.SetText(message);
				}
			}
			VsStatusBar.SetText("Using driver compiler");
			return CompileOnGPU(shaderCode, shaderType);
		}

		private static string GetShaderFileName(ShaderType shaderType)
		{
			var extension = FileExtension(shaderType);
			return Path.Combine(Path.GetTempPath(), $"shader.{extension}");
		}

		private static string FileExtension(ShaderType shaderType)
		{
			switch(shaderType)
			{
				case ShaderType.ComputeShader: return "comp";
				case ShaderType.FragmentShader: return "frag";
				case ShaderType.GeometryShader: return "geom";
				case ShaderType.TessControlShader: return "tesc";
				case ShaderType.TessEvaluationShader: return "tese";
				case ShaderType.VertexShader: return "vert";
			}
			return "frag";
		}

		[HandleProcessCorruptedStateExceptions]
		private static string CompileOnGPU(string shaderCode, ShaderType shaderType)
		{
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
	}
}