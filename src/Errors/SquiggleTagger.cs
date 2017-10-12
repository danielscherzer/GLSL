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
			// Make sure we are only tagging the top buffer
			if (buffer != textView.TextBuffer) return null;

			var tagger = new SquiggleTagger(buffer);
			var typeName = buffer.ContentType.TypeName;

			tagger.RequestCompileShader(buffer.CurrentSnapshot.GetText(), typeName); //initial compile
			
			buffer.Changed += (s, e) => tagger.RequestCompileShader(e.After.GetText(), typeName); //compile on text change. can be very often!
			return tagger as ITagger<T>;
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
			//todo: parse error.message for offending words to narrow down span
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

		internal void RequestCompileShader(string shaderCode, string shaderType)
		{
			//if not currently compiling then compile shader from changed text otherwise add to the "to be compiled" list
			if (!Enum.TryParse(shaderType, true, out ShaderCompiler.ShaderContentType contentType)) contentType = ShaderCompiler.ShaderContentType.GlslFragmentShader;
			shaderCompiler.CompilationFinished += ShaderCompiler_CompilationFinished;
			shaderCompiler.RequestCompile(shaderCode, contentType);
		}

		private List<ShaderLogLine> errors = new List<ShaderLogLine>();
		private static ShaderCompiler shaderCompiler = new ShaderCompiler();
		private ITextBuffer buffer;
		private string filePath;

		private void ShaderCompiler_CompilationFinished(List<ShaderLogLine> errorLog)
		{
			shaderCompiler.CompilationFinished -= ShaderCompiler_CompilationFinished;
			errors = errorLog;
			ErrorList.GetInstance().Clear();
			foreach (var error in errors)
			{
				ErrorList.GetInstance().Write(error.Message, error.LineNumber - 1, filePath);
			}
			//todo: use PredefinedErrorTypeNames
			var span = new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length);
			TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
		}
	}
}
