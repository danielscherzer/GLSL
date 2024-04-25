using DMS.GLSL.Contracts;
using GLSLhelper;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;

namespace DMS.GLSL.Errors
{
	internal class SquiggleTagger : ITagger<IErrorTag>
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

		public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection inputSpans) => _tags;

		public void UpdateErrors(IEnumerable<ShaderLogLine> errorLog)
		{
			_tags.Clear();
			ErrorList.GetInstance().Clear();

			foreach (var error in errorLog)
			{
				var lineNumber = error.LineNumber.HasValue ? error.LineNumber.Value - 1 : 0;
				ErrorList.GetInstance().Write(error.Message, lineNumber, filePath, error.Type);

				var lineSpan = buffer.CurrentSnapshot.GetLineFromLineNumber(lineNumber).Extent; //TODO: parse error.message for offending words to trim down span
				var tag = new ErrorTag(ConvertErrorType(error.Type), error.Message);
				_tags.Add(new TagSpan<IErrorTag>(lineSpan, tag));
			}
			var span = new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length);
			TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
		}

		private readonly List<ITagSpan<IErrorTag>> _tags = new List<ITagSpan<IErrorTag>>();
		private readonly ITextBuffer buffer;
		private readonly string filePath;

		private static string ConvertErrorType(MessageType type)
		{
			switch (type)
			{
				case MessageType.Error: return PredefinedErrorTypeNames.SyntaxError;
				case MessageType.Warning: return PredefinedErrorTypeNames.Warning;
				default: return PredefinedErrorTypeNames.Suggestion;
			}
		}
	}
}
