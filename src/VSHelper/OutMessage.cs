using DMS.GLSL.Contracts;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Composition;

namespace DMS.GLSL.VSHelper
{
	[Export(typeof(ILogger))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class OutMessage : ILogger
	{
		private static void PaneAndBar(string message)
		{
			StatusBar(message);
			OutputWindowPane(message);
		}

		private static void StatusBar(string message)
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

		private static void OutputWindowPane(string message)
		{
			ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
			{
				await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

				var outputWindowPane = (IVsOutputWindowPane)Package.GetGlobalService(typeof(SVsGeneralOutputWindowPane));

				if (outputWindowPane is null) return;

				outputWindowPane.OutputStringThreadSafe(message + Environment.NewLine);
			});
		}

		public void Log(string message, bool highPriority = false)
		{
			if(highPriority)
			{
				PaneAndBar(message);
			}
			else
			{
				OutputWindowPane(message);
			}
		}
	}
}
