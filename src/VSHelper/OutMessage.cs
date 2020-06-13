using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace DMS.GLSL.VSHelper
{
	public static class OutMessage
	{
		public static void PaneAndBar(string message)
		{
			StatusBar(message);
			OutputWindowPane(message);
		}

		public static void StatusBar(string message)
		{
			ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
			{
				await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

				var statusBar = (IVsStatusbar)Package.GetGlobalService(typeof(SVsStatusbar));

				if (statusBar is null) return;

				statusBar.IsFrozen(out var frozen);
				if (0 != frozen) statusBar.FreezeOutput(0);

				statusBar.SetText(message);
			});
		}

		public static void OutputWindowPane(string message)
		{
			ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
			{
				await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

				var outputWindowPane = (IVsOutputWindowPane)Package.GetGlobalService(typeof(SVsGeneralOutputWindowPane));

				if (outputWindowPane is null) return;

				outputWindowPane.OutputStringThreadSafe(message + Environment.NewLine);
			});
		}
	}
}
