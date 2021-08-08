namespace DMS.GLSL.Contracts
{
	internal interface ILogger
	{
		void Log(string message, bool highPriority = false);
	}
}
