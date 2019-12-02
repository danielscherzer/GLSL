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
			tokenTypes = new TokenTypes(classificationTypeRegistry);
		}

		public IClassifier GetClassifier(ITextBuffer textBuffer)
		{
			return new GlslClassifier(tokenTypes);
		}

		[Import]
		internal IClassificationTypeRegistryService classificationTypeRegistry = null;

		private TokenTypes tokenTypes;
	}

	internal sealed class GlslClassifier : IClassifier
	{
		private readonly TokenTypes tokenTypes;
		private readonly Lexer<IClassificationType> lexer;

		internal GlslClassifier(TokenTypes tokenTypes)
		{
			this.tokenTypes = tokenTypes;
			lexer = new Lexer<IClassificationType>(tokenTypes);
		}

		public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged { add { } remove { } }

		public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan inputSpan)
		{
			var output = new List<ClassificationSpan>();
			var text = inputSpan.GetText();
			//var log = new StringBuilder();
			foreach (var (start, length, type) in lexer.Tokenize(text))
			{
				var lineSpan = new SnapshotSpan(inputSpan.Snapshot, inputSpan.Start + start, length);
				output.Add(new ClassificationSpan(lineSpan, type));
				//log.AppendLine($"{lineSpan.Start.Position}:{lineSpan.Length}={type}");
			}
			//OutMessage.OutputWindowPane(log.ToString());
			return output;
		}
	}
}