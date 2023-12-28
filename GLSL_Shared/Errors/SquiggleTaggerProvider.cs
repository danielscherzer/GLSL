using DMS.GLSL.Contracts;
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
		[ImportingConstructor]
		public SquiggleTaggerProvider(ShaderCompiler shaderCompiler, ICompilerSettings settings)
		{
			this.shaderCompiler = shaderCompiler ?? throw new ArgumentNullException(nameof(shaderCompiler));
			this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
		}

		public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
		{
			//Debug.WriteLine($"CreateTagger: textView={textView}, buffer={buffer}");

			// Make sure we are only tagging the top buffer
			if (!ReferenceEquals(buffer, textView.TextBuffer))
				return null;

			return buffer.Properties.GetOrCreateSingletonProperty(() =>
			{
				var tagger = new SquiggleTagger(buffer);

				var shaderType = buffer.ContentType.TypeName;

				var observableSourceCode = Observable.Return(buffer.CurrentSnapshot.GetText()).Concat(
						Observable.FromEventPattern<TextContentChangedEventArgs>(h => buffer.Changed += h, h => buffer.Changed -= h)
						.Select(e => e.EventArgs.After.GetText()));

				void RequestCompileShader(string shaderCode)
				{
					//if not currently compiling then compile shader from changed text otherwise add to the "to be compiled" list
					if (settings.LiveCompiling)
					{
						var filePath = GetFilePath(buffer);
						shaderCompiler.RequestCompile(shaderCode, shaderType, tagger.UpdateErrors, Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
					}
					else
					{
						tagger.UpdateErrors(new List<ShaderLogLine>());
					}
				}

				observableSourceCode
					.Throttle(TimeSpan.FromSeconds(settings.CompileDelay * 0.001f))
					.Subscribe(sourceCode => RequestCompileShader(sourceCode));

				return tagger;

			}) as ITagger<T>;
		}

		private readonly ShaderCompiler shaderCompiler;
		private readonly ICompilerSettings settings;

		private static string GetFilePath(ITextBuffer textBuffer)
		{
			foreach (var prop in textBuffer.Properties.PropertyList)
			{
				if (!(prop.Value is ITextDocument doc)) continue;
				return doc.FilePath;
			}
			return string.Empty;
		}
	}
}
