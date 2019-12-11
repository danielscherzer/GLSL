using DMS.GLSL.Language;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace DMS.GLSL.Classification
{
	[Export(typeof(IClassifierProvider))]
	[ContentType("glslShader")]
	[TagType(typeof(ClassificationTag))]
	internal class GlslClassifierProvider : IClassifierProvider, IPartImportsSatisfiedNotification
	{
		public void OnImportsSatisfied()
		{
			var tokenTypes = new TokenTypes(classificationTypeRegistry);
			var lexer = new Lexer<IClassificationType>(tokenTypes);
			classifier = new GlslClassifier(lexer);
		}

		public IClassifier GetClassifier(ITextBuffer textBuffer) => classifier;
	 // buffer.Properties.GetOrCreateSingletonProperty(() => new GlslClassifier(buffer, lexer)); //per buffer classifier

		[Import]
		internal IClassificationTypeRegistryService classificationTypeRegistry = null;

		private GlslClassifier classifier;
	}

	internal sealed class GlslClassifier : IClassifier
	{
		private readonly Lexer<IClassificationType> lexer;

		internal GlslClassifier(Lexer<IClassificationType> lexer)
		{
			this.lexer = lexer;
		}

		public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged { add { } remove { } }

		public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan inputSpan)
		{
			var output = new List<ClassificationSpan>();
			var text = inputSpan.GetText();
			//var log = new StringBuilder();
			//var time = Stopwatch.StartNew();
			foreach (var (start, length, type) in lexer.Tokenize(text))
			{
				var lineSpan = new SnapshotSpan(inputSpan.Snapshot, inputSpan.Start + start, length);
				output.Add(new ClassificationSpan(lineSpan, type));
				//log.Append($"{lineSpan.Start.Position}:{lineSpan.Length}={type}; ");
			}
			//OutMessage.OutputWindowPane(time.ElapsedTicks * 1e3f / Stopwatch.Frequency + ":" + log.ToString());
			return output;
		}
	}
}