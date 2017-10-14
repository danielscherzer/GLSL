using ContextGL;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace DMS.GLSL.Errors
{
	[Export]
	[PartCreationPolicy(CreationPolicy.Shared)] //default singleton behavior
	internal class ShaderCompiler
	{
		internal delegate void OnCompilationFinished(List<ShaderLogLine> errorLog);

		internal ShaderCompiler()
		{
			//start up gl task for doing shader compilations in background
			taskGL = Task.Factory.StartNew(TaskGlAction);
		}

		internal void RequestCompile(string shaderCode, string sShaderType, OnCompilationFinished compilationFinishedHandler)
		{
			//conversion
			if (!Enum.TryParse(sShaderType, true, out ShaderContentType contentType)) contentType = ShaderContentType.GlslFragmentShader;
			var shaderType = (ShaderType)contentType;
			
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

		private enum ShaderContentType
		{
			GlslFragmentShader = ShaderType.FragmentShader,
			GlslVertexShader = ShaderType.VertexShader,
			GlslGeometryShader = ShaderType.GeometryShader,
			GlslTessEvaluationShader = ShaderType.TessEvaluationShader,
			GlslTessControlShader = ShaderType.TessControlShader,
			GlslComputeShader = ShaderType.ComputeShader,
		}

		private Task taskGL;
		private BlockingCollection<CompileData> compileRequests = new BlockingCollection<CompileData>();
		//[Import] SharedContextGL context = null;

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
		
		private static string Compile(string shaderCode, ShaderType shaderType)
		{
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
	}
}