using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.GLSL.Errors
{
	internal class ShaderCompiler
	{
		internal enum ShaderContentType {
			GlslFragmentShader = ShaderType.FragmentShader,
			GlslVertexShader = ShaderType.VertexShader,
			GlslGeometryShader = ShaderType.GeometryShader,
			GlslTessEvaluationShader = ShaderType.TessEvaluationShader,
			GlslTessControlShader = ShaderType.TessControlShader,
			GlslComputeShader = ShaderType.ComputeShader,
		}

		internal delegate void OnCompilationFinished(List<ShaderLogLine> errorLog);
		internal event OnCompilationFinished CompilationFinished;

		internal ShaderCompiler()
		{
			//start up gl task for doing shader compilations in background
			taskGL = Task.Factory.StartNew(TaskGlAction);
		}

		internal void RequestCompile(string shaderCode, ShaderContentType contentType)
		{
			CompileData data;
			while (compileInput.TryTake(out data)) ; //remove pending compiles
			data.shaderCode = shaderCode;
			data.contentType = contentType;
			compileInput.TryAdd(data); //add new
		}

		private struct CompileData
		{
			public string shaderCode;
			public ShaderContentType contentType;
		}

		private Task taskGL;
		private BlockingCollection<CompileData> compileInput = new BlockingCollection<CompileData>();

		private void TaskGlAction()
		{
			//create Gamewindow for rendering context, until run is called it is invisible so no problem
			Console.WriteLine("new OpenTK GameWindow");
			var context = new GameWindow(1, 1);
			while (!compileInput.IsAddingCompleted)
			{
				var compileData = compileInput.Take(); //block until compile requested
				var log = Compile(compileData.shaderCode, compileData.contentType);
				var errorLog = ShaderLog.Parse(log);
				CompilationFinished?.Invoke(errorLog);
			}
		}
		
		private string Compile(string shaderCode, ShaderContentType contentType)
		{
			var shaderType = (ShaderType)contentType;
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