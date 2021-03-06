﻿namespace DMS.GLSL.Contracts
{
	public interface ICompilerSettings
	{
		int CompileDelay { get; }
		string ExternalCompilerArguments { get; }
		string ExternalCompilerExeFilePath { get; }
		bool LiveCompiling { get; }
		bool PrintShaderCompilerLog { get; }
	}
}