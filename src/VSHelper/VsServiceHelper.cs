using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DMS.GLSL.VSHelper
{
	internal static class VsServiceHelper
	{
		internal static async Task<string> ExpandEnvironmentVariablesAsync(string text)
		{
			const string solutionDirVar = "$(SolutionDir)";
			if (text.Contains(solutionDirVar))
			{
				var joinableTaskFactory = ThreadHelper.JoinableTaskFactory;
				await joinableTaskFactory.SwitchToMainThreadAsync();
				lock (_syncRoot)
				{
					var dTE = Package.GetGlobalService(typeof(DTE)) as DTE;
					string solutiondir = File.Exists(dTE.Solution.FileName) ? Path.GetDirectoryName(dTE.Solution.FileName) : string.Empty; // the value of $(SolutionDir)
					text = text.Replace(solutionDirVar, solutiondir);
					//todo: use dTE.Events.SolutionEvents.Opened
				}
			}
			return Environment.ExpandEnvironmentVariables(text);
		}

		internal static string ExpandEnvironmentVariables(string text)
		{
			var joinableTaskFactory = ThreadHelper.JoinableTaskFactory;
			return joinableTaskFactory.Run(() => ExpandEnvironmentVariablesAsync(text));
		}

		internal static void StatusBar(string message)
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

		internal static void OutputWindowPane(string message)
		{
			ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
			{
				await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

				var outputWindowPane = (IVsOutputWindowPane)Package.GetGlobalService(typeof(SVsGeneralOutputWindowPane));

				if (outputWindowPane is null) return;

				outputWindowPane.OutputStringThreadSafe(message + Environment.NewLine);
			});
		}

		private static readonly object _syncRoot = new object();
	}
}
