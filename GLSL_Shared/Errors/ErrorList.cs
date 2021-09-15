using GLSLhelper;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace DMS.GLSL.Errors
{
	public sealed class ErrorList : IServiceProvider
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

		internal void Write(string message, int lineNumber, string filePath, MessageType type)
		{
			ErrorTask task = CreateTask(message, lineNumber, filePath, Convert(type));
			task.Navigate += Task_Navigate;
			try
			{
				provider.Tasks.Add(task);
				//provider.BringToFront();
			}
			catch (OperationCanceledException)
			{ }
		}

		private static ErrorTask CreateTask(string text, int line, string document, TaskErrorCategory errorCategory)
		{
			return new ErrorTask
			{
				Category = TaskCategory.BuildCompile,
				Text = text,
				Line = line,
				Column = 1,
				Document = document,
				ErrorCategory = errorCategory,
			};
		}

		private void Task_Navigate(object sender, EventArgs e)
		{
			if (sender is ErrorTask task)
			{
				var temp = CreateTask(task.Text, task.Line + 1, task.Document, task.ErrorCategory);
				provider.Navigate(temp, new Guid(LogicalViewID.Code));
			}
		}

		private static readonly ErrorList instance = new ErrorList();
		private readonly ErrorListProvider provider;

		private static TaskErrorCategory Convert(MessageType type)
		{
			switch (type)
			{
				case MessageType.Error: return TaskErrorCategory.Error;
				case MessageType.Warning: return TaskErrorCategory.Warning;
				default: return TaskErrorCategory.Message;
			}
		}
	}
}
