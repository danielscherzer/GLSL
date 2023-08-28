using System.IO;

namespace Glslang;

public class Glslang
{
	public static string Compile(string shaderCode, string shaderContentType)
	{
		//create temp shader file for external compiler
		var tempPath = Path.GetTempPath();
		//var shaderFileName = Path.Combine(tempPath, $"shader{ShaderContentTypes.DefaultFileExtension(shaderContentType)}");
		//try
		//{
		//	File.WriteAllText(shaderFileName, shaderCode);
		//	using (var process = new Process())
		//	{
		//		process.StartInfo.FileName = VsExpand.EnvironmentVariables(settings.ExternalCompilerExeFilePath);
		//		var arguments = VsExpand.EnvironmentVariables(settings.ExternalCompilerArguments);
		//		process.StartInfo.Arguments = $"{arguments} \"{shaderFileName}\""; //arguments
		//		process.StartInfo.WorkingDirectory = tempPath;
		//		process.StartInfo.UseShellExecute = false;
		//		process.StartInfo.RedirectStandardOutput = true;
		//		process.StartInfo.RedirectStandardError = true;
		//		process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		//		process.StartInfo.CreateNoWindow = true; //do not display a windows
		//		logger.Log($"Using external compiler '{settings.ExternalCompilerExeFilePath}' with arguments '{arguments}' on temporal shader file '{shaderFileName}'", true);
		//		process.Start();
		//		if (!process.WaitForExit(10000))
		//		{
		//			logger.Log("External compiler did take more than 10 seconds to finish. Aborting!", true);
		//		}
		//		return process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd(); //The output result
		//	}
		//}
		//catch (Exception e)
		//{
		//	var message = "Error executing external compiler with message\n" + e.ToString();
		//	logger.Log(message, true);
		//	return string.Empty;
		//}
		return string.Empty;
	}
}
