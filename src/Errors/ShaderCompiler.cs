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
		internal delegate void OnCompilationFinished(List<ShaderLogLine> errorLog);
		internal event OnCompilationFinished CompilationFinished;

		internal ShaderCompiler()
		{
			//start up gl task for doing shader compilations in background
			taskGL = Task.Factory.StartNew(TaskGlAction);
		}

		internal void RequestCompile(string shaderCode, string shaderType)
		{
			if (!Enum.TryParse(shaderType, true, out ShaderContentType contentType)) contentType = ShaderContentType.GlslFragmentShader;
			RequestCompile(shaderCode, (ShaderType)contentType);
		}

		private void RequestCompile(string shaderCode, ShaderType shaderType)
		{
			CompileData data;
			while (compileInput.TryTake(out data)) ; //remove pending compiles
			data.shaderCode = shaderCode;
			data.shaderType = shaderType;
			compileInput.TryAdd(data); //add new
		}

		private struct CompileData
		{
			public string shaderCode;
			public ShaderType shaderType;
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
		private BlockingCollection<CompileData> compileInput = new BlockingCollection<CompileData>();

		private void TaskGlAction()
		{
			//create Gamewindow for rendering context, until run is called it is invisible so no problem
			Console.WriteLine("new OpenTK GameWindow");
			var context = new GameWindow(1, 1);
			while (!compileInput.IsAddingCompleted)
			{
				var compileData = compileInput.Take(); //block until compile requested
				var log = Compile(compileData.shaderCode, compileData.shaderType);
				var errorLog = ShaderLog.Parse(log);
				CompilationFinished?.Invoke(errorLog);
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