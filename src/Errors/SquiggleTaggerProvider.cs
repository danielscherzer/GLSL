using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using DMS.GLSL.Classification;

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
			var options = OptionsPagePackage.Options;
			if (options is null) return;
			if (!options.LiveCompiling)
			{
				tagger.UpdateErrors(new List<ShaderLogLine>());
				return;
			}
			shaderCompiler.RequestCompile(shaderCode, shaderType, tagger.UpdateErrors);
		}
	}
}
