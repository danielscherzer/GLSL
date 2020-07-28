using GLSLhelper;
using Microsoft.VisualStudio.Shell;
using System;

namespace DMS.GLSL.Errors
{
	public class ErrorList : IServiceProvider
	{
		//public enum Type { Warning, Error}
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

		internal void Write(string message, int lineNumber, string filePath, MessageType type)
		{
			var task = new ErrorTask
			{
				Category = TaskCategory.BuildCompile,
				Text = message,
				Line = lineNumber,
				Document = filePath,
				ErrorCategory = Convert(type)
			};
			try
			{
				provider.Tasks.Add(task);
			}
			catch (OperationCanceledException)
			{ }
		}

		private static readonly ErrorList instance = new ErrorList();
		private readonly ErrorListProvider provider;

		private TaskErrorCategory Convert(MessageType type)
		{
			switch(type)
			{
				case MessageType.Error: return TaskErrorCategory.Error;
				case MessageType.Warning: return TaskErrorCategory.Warning;
				default: return TaskErrorCategory.Message;
			}
		}
	}
}
