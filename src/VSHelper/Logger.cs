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
			VsServiceHelper.OutputWindowPane(message);
			if (highPriority)
			{
				VsServiceHelper.StatusBar(message);
			}
		}
	}
}
