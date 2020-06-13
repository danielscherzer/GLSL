using Microsoft.VisualStudio.Shell;
using System;

namespace DMS.GLSL.Errors
{
	public class ErrorList : IServiceProvider
	{
		private ErrorList()
		{
			provider = new ErrorListProvider(this);
		}

		public static ErrorList GetInstance()
		{
			return instance;
		}

		public object GetService(Type serviceType)
		{
			return Package.GetGlobalService(serviceType);
		}

		public void Clear()
		{
			provider.Tasks.Clear();
		}

		public void Write(string message, int line, string filePath, bool isWarning = false)
		{
			var task = new ErrorTask
			{
				Category = TaskCategory.BuildCompile,
				Text = message,
				Line = line,
				Document = filePath,
				ErrorCategory = isWarning ? TaskErrorCategory.Warning : TaskErrorCategory.Error,
			};
			
			provider.Tasks.Add(task);
		}

		private static readonly ErrorList instance = new ErrorList();
		private readonly ErrorListProvider provider;
	}
}
