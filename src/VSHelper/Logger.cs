using DMS.GLSL.Contracts;
using System.ComponentModel.Composition;

namespace DMS.GLSL.VSHelper
{
	[Export(typeof(ILogger))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class Logger : ILogger
	{
		public void Log(string message, bool highPriority = false)
		{
			VsOutput.WindowPane(message);
			if (highPriority)
			{
				VsOutput.StatusBar(message);
			}
		}
	}
}
