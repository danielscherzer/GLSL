using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace DMS.GLSL.Errors
{
	[Export(typeof(IViewTaggerProvider))]
	[ContentType("glslShader")]
	[TagType(typeof(ErrorTag))]
	internal class SquiggleTaggerProvider : IViewTaggerProvider
	{
		public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
		{
			if (textView is null) return null;
			// Make sure we are only tagging the top buffer
			if (!ReferenceEquals(buffer, textView.TextBuffer)) return null;
			//make sure only one tagger for a textbuffer is created all views should share
			if (!taggers.ContainsKey(buffer))
			{
				var tagger = new SquiggleTagger(buffer);
				taggers[buffer] = tagger;
				var typeName = buffer.ContentType.TypeName;
				buffer.Changed += (s, e) => RequestCompileShader(tagger, e.After.GetText(), typeName); //compile on text change. can be very often!
				RequestCompileShader(tagger, buffer.CurrentSnapshot.GetText(), typeName); //initial compile
			}
			return taggers[buffer] as ITagger<T>;
		}

		[Import] private ShaderCompiler shaderCompiler = null;
		private Dictionary<ITextBuffer, SquiggleTagger> taggers = new Dictionary<ITextBuffer, SquiggleTagger>();

		private void RequestCompileShader(SquiggleTagger tagger, string shaderCode, string shaderType)
		{
			if (shaderCompiler is null) return;
			//if not currently compiling then compile shader from changed text otherwise add to the "to be compiled" list
			shaderCompiler.RequestCompile(shaderCode, shaderType, tagger.UpdateErrors);
		}
	}

	class SquiggleTagger : ITagger<IErrorTag>
	{
		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		internal SquiggleTagger(ITextBuffer buffer)
		{
			this.buffer = buffer;
			if (buffer.Properties.TryGetProperty<ITextDocument>(typeof(ITextDocument), out ITextDocument document))
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
						var tag = new ErrorTag(error.Type, error.Message);
						var span = new TagSpan<IErrorTag>(inputSpan, tag);
						yield return span;
					}
				}
			}
		}

		public void UpdateErrors(List<ShaderLogLine> errorLog)
		{
			errors = errorLog;
			ErrorList.GetInstance().Clear();
			foreach (var error in errors)
			{
				ErrorList.GetInstance().Write(error.Message, error.LineNumber - 1, filePath);
			}
			//TODO: use PredefinedErrorTypeNames
			var span = new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length);
			TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
		}

		private List<ShaderLogLine> errors = new List<ShaderLogLine>();
		private ITextBuffer buffer;
		private string filePath;
	}
}
