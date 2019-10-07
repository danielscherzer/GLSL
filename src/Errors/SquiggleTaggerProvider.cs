using DMS.GLSL.Classification;
using DMS.GLSL.Options;
using GLSLhelper;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;

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

				// TODO: Move all this stuff into the SquiggleTagger constructor
				var typeName = buffer.ContentType.TypeName;
				string shaderCode = "";

				System.Timers.Timer timer = new System.Timers.Timer();
				timer.AutoReset = false;
				timer.Elapsed += (o, e) =>
				{
					RequestCompileShader(tagger, shaderCode, typeName, GetDocumentDir(buffer));
				};

				buffer.Changed += (s, e) => {
					timer.Stop();
					int ms = OptionsPagePackage.Options.CompileDelay;
					if (ms > 0)
					{
						shaderCode = e.After.GetText();
						timer.Interval = ms;
						timer.Start();
					}
					else
					{
						RequestCompileShader(tagger, e.After.GetText(), typeName, GetDocumentDir(buffer)); //compile on text change. can be very often!
					}
				};
				RequestCompileShader(tagger, buffer.CurrentSnapshot.GetText(), typeName, GetDocumentDir(buffer)); //initial compile

				return tagger;

			}) as ITagger<T>;
		}

		[Import] private ShaderCompiler shaderCompiler = null;

		private string GetDocumentDir(ITextBuffer textBuffer)
		{
			foreach (var prop in textBuffer.Properties.PropertyList)
			{
				var doc = prop.Value as ITextDocument;
				if (doc is null) continue;
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
