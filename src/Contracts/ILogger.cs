namespace DMS.GLSL.Contracts
{
	interface ILogger
	{
		void Log(string message, bool highPriority = false);
	}
}
