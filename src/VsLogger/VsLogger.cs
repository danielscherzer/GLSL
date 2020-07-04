using DMS.GLSL.Contracts;
using System.ComponentModel.Composition;

namespace DMS.GLSL.VsLogger
{
	[Export(typeof(ILogger))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class VsLogger : ILogger
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
