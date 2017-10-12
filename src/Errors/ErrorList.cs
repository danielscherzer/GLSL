using Microsoft.VisualStudio.Shell;
using System;

namespace DMS.GLSL.Errors
{
	public class ErrorList : System.IServiceProvider
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

		public void Write(string message, int line, string filePath)
		{
			var task = new ErrorTask
			{
				Category = TaskCategory.BuildCompile,
				Text = message,
				Line = line,
				Document = filePath
			};
			provider.Tasks.Add(task);
		}

		private static ErrorList instance = new ErrorList();
		private ErrorListProvider provider;
	}
}
