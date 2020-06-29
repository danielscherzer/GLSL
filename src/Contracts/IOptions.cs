namespace DMS.GLSL.Contracts
{
	public interface IOptions
	{
		int CompileDelay { get; }
		string ExternalCompilerArguments { get; }
		string ExternalCompilerExeFilePath { get; }
		bool LiveCompiling { get; }
		bool PrintCompilationResult { get; }
		string UserKeyWords1 { get; }
		string UserKeyWords2 { get; }
	}
}