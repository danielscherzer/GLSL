using DMS.GLSL.Classification;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Zenseless.HLGL;
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
			//conversion
			if (!mappingContentTypeToShaderType.TryGetValue(sShaderType, out ShaderType shaderType)) shaderType = ShaderType.FragmentShader;
			
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

		private readonly Dictionary<string, ShaderType> mappingContentTypeToShaderType = new Dictionary<string, ShaderType>()
		{
			[ContentTypesGlsl.FragmentShader] = ShaderType.FragmentShader,
			[ContentTypesGlsl.VertexShader] = ShaderType.VertexShader,
			[ContentTypesGlsl.GeometryShader] = ShaderType.GeometryShader,
			[ContentTypesGlsl.TessControlShader] = ShaderType.TessControlShader,
			[ContentTypesGlsl.TessEvaluationShader] = ShaderType.TessEvaluationShader,
			[ContentTypesGlsl.ComputeShader] = ShaderType.ComputeShader,
		};

		private Task taskGL;
		private BlockingCollection<CompileData> compileRequests = new BlockingCollection<CompileData>();

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
				var log = Compile(compileData.ShaderCode, compileData.ShaderType, compileData.DocumentDir);
				var errorLog = new ShaderLog(log);
				compileData.CompilationFinished?.Invoke(errorLog.Lines);
			}
		}

		[HandleProcessCorruptedStateExceptions]
		private static string Compile(string shaderCode, ShaderType shaderType, string shaderFileDir)
		{
			string GetIncludeCode(string includeName)
			{
				var includeFileName = Path.Combine(shaderFileDir, includeName);
				if (File.Exists(includeFileName))
				{
					return File.ReadAllText(includeFileName);
				}
				return $"#error include file '{includeName}' not found";
			}
			try
			{
				using (var shader = new ShaderGL(shaderType))
				{
					var expandedCode = ShaderLoader.ResolveIncludes(shaderCode, GetIncludeCode);
					shader.Compile(expandedCode);
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