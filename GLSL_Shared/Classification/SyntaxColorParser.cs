using DMS.GLSL.Contracts;
using GLSLhelper;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;

namespace DMS.GLSL.Classification
{
	internal class SyntaxColorParser : ISyntaxColorParser
	{
		public SyntaxColorParser(IClassificationTypeRegistryService classificationTypeRegistry, IUserKeywords userKeywords)
		{
			if (classificationTypeRegistry is null)
			{
				throw new System.ArgumentNullException(nameof(classificationTypeRegistry));
			}

			if (userKeywords is null)
			{
				throw new System.ArgumentNullException(nameof(userKeywords));
			}

			Comment = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Comment);
			Identifier = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Identifier);
			Number = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Number);
			Operator = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Operator);
			QuotedString = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.String);
			PreprocessorKeyword = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.PreprocessorKeyword);

			Function = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.Function);
			Keyword = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.Keyword);
			UserKeyword1 = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.UserKeyword1);
			UserKeyword2 = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.UserKeyword2);
			Variable = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.Variable);
			parser = new GlslParser();
			userKeywords.PropertyChanged += (s, a) =>
			{
				ResetUserKeywords(userKeywords);
				Changed?.Invoke(this);
			};
			ResetUserKeywords(userKeywords);
		}

		public delegate void ChangedEventHandler(object sender);
		public event ChangedEventHandler Changed;

		public IList<ClassificationSpan> CalculateSpans(SnapshotSpan snapshotSpan)
		{
			var output = new List<ClassificationSpan>();
			var text = snapshotSpan.GetText();
			foreach (var token in parser.Tokenize(text))
			{
				var lineSpan = new SnapshotSpan(snapshotSpan.Snapshot, token.Start, token.Length);
				output.Add(new ClassificationSpan(lineSpan, Convert(token)));
			}
			return output;
		}

		private readonly GlslParser parser;
		private readonly Dictionary<string, IClassificationType> userKeywords = new Dictionary<string, IClassificationType>();

		private IClassificationType Comment { get; }
		private IClassificationType Identifier { get; }
		private IClassificationType Number { get; }
		private IClassificationType Operator { get; }
		private IClassificationType QuotedString { get; }
		private IClassificationType PreprocessorKeyword { get; }
		private IClassificationType Function { get; }
		private IClassificationType Keyword { get; }
		private IClassificationType UserKeyword1 { get; }
		private IClassificationType UserKeyword2 { get; }
		private IClassificationType Variable { get; }

		private void ResetUserKeywords(IUserKeywords userKeywords)
		{
			this.userKeywords.Clear();
			foreach (var word in userKeywords.UserKeywordArray1) this.userKeywords[word] = UserKeyword1;
			foreach (var word in userKeywords.UserKeywordArray2) this.userKeywords[word] = UserKeyword2;
		}

		private IClassificationType Convert(IToken token)
		{
			IClassificationType CheckUserDefined(IToken currentToken, IClassificationType defaultType)
			{
				return userKeywords.TryGetValue(currentToken.Value, out var type) ? type : defaultType;
			}

			switch (token.Type)
			{
				case TokenType.Comment: return Comment;
				case TokenType.Function: return CheckUserDefined(token, Function);
				case TokenType.Keyword: return CheckUserDefined(token, Keyword);
				case TokenType.Number: return Number;
				case TokenType.Operator: return Operator;
				case TokenType.Preprocessor: return PreprocessorKeyword;
				case TokenType.Variable: return CheckUserDefined(token, Variable);
				case TokenType.Identifier: return CheckUserDefined(token, Identifier);
				case TokenType.QuotedString: return QuotedString;
				default:
					return Identifier;
			}
		}
	}
}
