using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DMS.GLSL.Errors
{
	public static class VsExpand
	{
		public static async Task<string> EnvironmentVariablesAsync(string text)
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

		public static string EnvironmentVariables(string text)
		{
			var joinableTaskFactory = ThreadHelper.JoinableTaskFactory;
			return joinableTaskFactory.Run(() => EnvironmentVariablesAsync(text));
		}

		private static readonly object _syncRoot = new object();
	}
}
