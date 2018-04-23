using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DMS.GLSL.Errors
{
	class SquiggleTagger : ITagger<IErrorTag>
	{
		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		internal SquiggleTagger(ITextBuffer buffer)
		{
			this.buffer = buffer;
			if (buffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument document))
			{
				filePath = document.FilePath;
			}
		}

		public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection inputSpans)
		{
			if (errors.Count == 0) yield break;
			//TODO: parse error.message for offending words to narrow down span
			//error.Message.
			foreach (var inputSpan in inputSpans)
			{
				foreach (var error in errors)
				{
					var lineNumber = inputSpan.Start.GetContainingLine().LineNumber + 1;
					if (error.LineNumber == lineNumber)
					{
						var tag = new ErrorTag(ConvertErrorType(error.Type), error.Message);
						var span = new TagSpan<IErrorTag>(inputSpan, tag);
						yield return span;
					}
				}
			}
		}

		private string ConvertErrorType(string type)
		{
			//Debug.WriteLine(type);
			if (type.Contains("ERROR"))
			{
				return PredefinedErrorTypeNames.CompilerError;
			}
			else if (type.Contains("WARNING"))
			{
				return PredefinedErrorTypeNames.Warning;
			}
			return PredefinedErrorTypeNames.OtherError;
		}

		public void UpdateErrors(List<ShaderLogLine> errorLog)
		{
			errors = errorLog;
			ErrorList.GetInstance().Clear();
			foreach (var error in errors)
			{
				ErrorList.GetInstance().Write(error.Message, error.LineNumber - 1, filePath);
			}
			var span = new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length);
			TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
		}

		private List<ShaderLogLine> errors = new List<ShaderLogLine>();
		private ITextBuffer buffer;
		private string filePath;
	}
}
