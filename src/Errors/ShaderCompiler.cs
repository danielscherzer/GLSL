using ContextGL;
using DMS.GLSL.Classification;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace DMS.GLSL.Errors
{
	[Export]
	[PartCreationPolicy(CreationPolicy.Shared)] //default singleton behavior
	internal class ShaderCompiler
	{
		internal delegate void OnCompilationFinished(List<ShaderLogLine> errorLog);

		internal void RequestCompile(string shaderCode, string sShaderType, OnCompilationFinished compilationFinishedHandler)
		{
			StartGlThreadOnce();
			//conversion
			if (!mappingContentTypeToShaderType.TryGetValue(sShaderType, out ShaderType shaderType)) shaderType = ShaderType.FragmentShader;
			
			CompileData data;
			while (compileRequests.TryTake(out data)) ; //remove pending compiles
			data.shaderCode = shaderCode;
			data.shaderType = shaderType;
			data.CompilationFinished = compilationFinishedHandler;
			compileRequests.TryAdd(data); //put compile on request list
		}

		private struct CompileData
		{
			public string shaderCode;
			public ShaderType shaderType;
			public OnCompilationFinished CompilationFinished;
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
		//[Import(AllowDefault = true)] SharedContextGL context = null;

		private void StartGlThreadOnce()
		{
			if (!(taskGL is null)) return;
			//start up gl task for doing shader compilations in background
			taskGL = Task.Factory.StartNew(TaskGlAction);
		}

		private void TaskGlAction()
		{
			//create Gamewindow for rendering context, until run is called it is invisible so no problem
			var context = new GameWindow(1, 1);
			while (!compileRequests.IsAddingCompleted)
			{
				var compileData = compileRequests.Take(); //block until compile requested
				var log = Compile(compileData.shaderCode, compileData.shaderType);
				var errorLog = ShaderLog.Parse(log);
				compileData.CompilationFinished?.Invoke(errorLog);
			}
		}

		[HandleProcessCorruptedStateExceptions]
		private string Compile(string shaderCode, ShaderType shaderType)
		{
			try
			{
				//context.MakeCurrent();
				int shaderObject = GL.CreateShader(shaderType);
				if (0 == shaderObject) throw new SystemException("Could not create " + shaderType.ToString() + " object");
				// Compile vertex shader
				GL.ShaderSource(shaderObject, shaderCode);
				GL.CompileShader(shaderObject);
				GL.GetShader(shaderObject, ShaderParameter.CompileStatus, out int status_code);
				string log = string.Empty;
				if (1 != status_code)
				{
					log = GL.GetShaderInfoLog(shaderObject);
				}
				GL.DeleteShader(shaderObject);
				return log;
			}
			catch (AccessViolationException)
			{
				return $"(1 1):ERROR: OpenGL shader compiler has crashed";
			}
		}
	}
}