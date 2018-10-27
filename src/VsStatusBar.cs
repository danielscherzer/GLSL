using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace DMS.GLSL
{
	public static class VsStatusBar
	{
		private static IVsStatusbar statusBar = (IVsStatusbar)Package.GetGlobalService(typeof(SVsStatusbar));

		public static void SetText(string message)
		{
			if (statusBar is null) return;

			statusBar.IsFrozen(out var frozen);
			if (0 != frozen) statusBar.FreezeOutput(0);

			statusBar.SetText(message);
		}
	}
}
