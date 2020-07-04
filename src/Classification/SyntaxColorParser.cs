using DMS.GLSL.Language;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;

namespace DMS.GLSL.Classification
{
	class SyntaxColorParser
	{
		private readonly GlslTokenizer tokenizer;

		public SyntaxColorParser(IClassificationTypeRegistryService classificationTypeRegistry)
		{
			Comment = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Comment);
			Identifier = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Identifier);
			Number = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Number);
			Operator = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Operator);
			PreprocessorKeyword = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.PreprocessorKeyword);

			Function = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.Function);
			Keyword = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.Keyword);
			UserKeyWord1 = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.UserKeyWord1);
			UserKeyWord2 = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.UserKeyWord2);
			Variable = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.Variable);
			tokenizer = new GlslTokenizer();
		}

		private IClassificationType Comment { get; }
		private IClassificationType Identifier { get; }
		private IClassificationType Number { get; }
		private IClassificationType Operator { get; }
		private IClassificationType PreprocessorKeyword { get; }
		private IClassificationType Function { get; }
		private IClassificationType Keyword { get; }
		private IClassificationType UserKeyWord1 { get; }
		private IClassificationType UserKeyWord2 { get; }
		private IClassificationType Variable { get; }

		public IList<ClassificationSpan> CalculateSpans(SnapshotSpan snapshotSpan)
		{
			var output = new List<ClassificationSpan>();
			var text = snapshotSpan.GetText();
			foreach (var token in tokenizer.Tokenize(text))
			{
				var lineSpan = new SnapshotSpan(snapshotSpan.Snapshot, token.Start, token.Length);
				output.Add(new ClassificationSpan(lineSpan, Convert(token.Type)));
			}
			return output;
		}

		private IClassificationType Convert(Language.TokenTypes type)
		{
			switch(type)
			{
				case Language.TokenTypes.Comment: return Comment;
				case Language.TokenTypes.Function: return Function;
				case Language.TokenTypes.Keyword: return Keyword;
				case Language.TokenTypes.Number: return Number;
				case Language.TokenTypes.Operator: return Operator;
				case Language.TokenTypes.Preprocessor: return PreprocessorKeyword;
				case Language.TokenTypes.Variable: return Variable;
				default:
					return Identifier;
			}
		}
	}
}
