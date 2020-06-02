using DMS.GLSL.Options;
using GLSLhelper;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reactive.Linq;

namespace DMS.GLSL.Errors
{
	[Export(typeof(IViewTaggerProvider))]
	[ContentType("glslShader")]
	[TagType(typeof(ErrorTag))]
	internal class SquiggleTaggerProvider : IViewTaggerProvider
	{
		public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
		{
			//Debug.WriteLine($"CreateTagger: textView={textView}, buffer={buffer}");

			// Make sure we are only tagging the top buffer
			if (!ReferenceEquals(buffer, textView.TextBuffer))
				return null;

			return buffer.Properties.GetOrCreateSingletonProperty(() =>
			{
				var tagger = new SquiggleTagger(buffer);

				var typeName = buffer.ContentType.TypeName;

				var observableSourceCode = Observable.Return(buffer.CurrentSnapshot.GetText()).Concat(
						Observable.FromEventPattern<TextContentChangedEventArgs>(h => buffer.Changed += h, h => buffer.Changed -= h)
						.Select(e => e.EventArgs.After.GetText()));
				observableSourceCode
					.Throttle(TimeSpan.FromSeconds(OptionsPagePackage.Options.CompileDelay * 0.001f))
					.Subscribe(sourceCode => RequestCompileShader(tagger, sourceCode, typeName, GetDocumentDir(buffer)));

				return tagger;

			}) as ITagger<T>;
		}

		[Import] private readonly ShaderCompiler shaderCompiler = null;

		private string GetDocumentDir(ITextBuffer textBuffer)
		{
			foreach (var prop in textBuffer.Properties.PropertyList)
			{
				if (!(prop.Value is ITextDocument doc)) continue;
				return Path.GetDirectoryName(doc.FilePath);
			}
			return string.Empty;
		}

		private void RequestCompileShader(SquiggleTagger tagger, string shaderCode, string shaderType, string documentDir)
		{
			if (shaderCompiler is null) return;
			//if not currently compiling then compile shader from changed text otherwise add to the "to be compiled" list
			var options = OptionsPagePackage.Options;
			if (!options.LiveCompiling)
			{
				tagger.UpdateErrors(new List<ShaderLogLine>());
				return;
			}
			shaderCompiler.RequestCompile(shaderCode, shaderType, tagger.UpdateErrors, documentDir);
		}
	}
}
