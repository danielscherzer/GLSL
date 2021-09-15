using System.Threading.Tasks;

namespace DMS.GLSL.Contracts
{
	internal interface ILogger
	{
		void Log(string message, bool highPriority = false);
		Task LogAsync(string message, bool highPriority = false);
	}
}
