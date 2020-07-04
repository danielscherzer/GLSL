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
				output.Add(new ClassificationSpan(lineSpan, Convert(token)));
			}
			return output;
		}

		private IClassificationType Convert(IToken token)
		{
			switch(token.Type)
			{
				case TokenTypes.Comment: return Comment;
				case TokenTypes.Function: return Function;
				case TokenTypes.Keyword: return Keyword;
				case TokenTypes.Number: return Number;
				case TokenTypes.Operator: return Operator;
				case TokenTypes.Preprocessor: return PreprocessorKeyword;
				case TokenTypes.Variable: return Variable;
				case TokenTypes.Identifier:
					switch (UserKeyWords.GetDefinedWordType(token.Value))
					{
						case UserKeyWords.DefinedWordType.UserKeyword1: return UserKeyWord1;
						case UserKeyWords.DefinedWordType.UserKeyword2: return UserKeyWord2;
						default: return Identifier;
					}
				default:
					return Identifier;
			}
		}
	}
}
