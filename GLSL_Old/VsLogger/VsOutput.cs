using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using System;

namespace DMS.GLSL.VsLogger
{
    internal static class VsOutput
    {
        internal static void StatusBar(string message)
        {
            _ = ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await StatusBarAsync(message).ConfigureAwait(true);
            });
        }

        internal static async System.Threading.Tasks.Task StatusBarAsync(string message)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var statusBar = (IVsStatusbar)Package.GetGlobalService(typeof(SVsStatusbar));

            if (statusBar is null) return;

            statusBar.IsFrozen(out var frozen);
            if (frozen != 0) statusBar.FreezeOutput(0);

            ErrorHandler.Succeeded(statusBar.SetText(message));
        }

        internal static async System.Threading.Tasks.Task WindowPaneAsync(string message)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var outputWindowPane = (IVsOutputWindowPane)Package.GetGlobalService(typeof(SVsGeneralOutputWindowPane));

            if (outputWindowPane != null)
            {
                ErrorHandler.Succeeded(outputWindowPane.OutputStringThreadSafe(message + Environment.NewLine));
            }
        }

        internal static void WindowPane(string message)
        {
            _ = ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
              {
                  await WindowPaneAsync(message).ConfigureAwait(true);
              });
        }
    }
}
