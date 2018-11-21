using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace DMS.GLSL
{
	public static class VsStatusBar
	{
		public static void SetText(string message)
		{
			var joinableTaskFactory = ThreadHelper.JoinableTaskFactory;
			joinableTaskFactory.Run(async delegate
			{
				await joinableTaskFactory.SwitchToMainThreadAsync();

				var statusBar = (IVsStatusbar)Package.GetGlobalService(typeof(SVsStatusbar));

				if (statusBar is null) return;

				statusBar.IsFrozen(out var frozen);
				if (0 != frozen) statusBar.FreezeOutput(0);

				statusBar.SetText(message);
			});
		}
	}
}
