using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.GLSL.Language
{
	class Lexer<TokenClassificationType>
	{
		static readonly Parser<string> NumberWithTrailingDigit = Parse.Number.Then(result => Parse.Char('.').Select(dot => result + dot));
		static readonly Parser<string> ParserNumber = Parse.DecimalInvariant.Or(NumberWithTrailingDigit);
		static readonly Parser<string> ParserComment = new CommentParser().AnyComment;
		static readonly Parser<string> ParserPreprocessor = Parse.Char('#').Once().Concat(Parse.AnyChar.Until(Parse.String(Environment.NewLine))).Text();
		static readonly Parser<string> ParserIdentifier = Parse.Identifier(Parse.Letter.Or(Parse.Char('_')), Parse.LetterOrDigit.Or(Parse.Char('_')));
		static readonly Parser<char> ParserOperator = Parse.Chars(".;,+-*/()[]{}<>=&$!\"%?:|");
		private readonly Parser<IEnumerable<PositionAware<TokenClassificationType>>> tokenParser;

		public Lexer(ITokenTypes<TokenClassificationType> tokenTypes)
		{
			var comment = ParserComment.Select(value => tokenTypes.Comment);
			var preprocessor = ParserPreprocessor.Select(value => tokenTypes.PreprocessorKeyword);

			TokenClassificationType CheckGlslSpecifics(string word)
			{
				if (GlslSpecification.IsKeyword(word)) return tokenTypes.Keyword;
				if (GlslSpecification.IsFunction(word)) return tokenTypes.Function;
				if (GlslSpecification.IsVariable(word)) return tokenTypes.Variable;
				if (GlslSpecification.IsUserKeyWord(word)) return tokenTypes.UserKeyWord;
				return tokenTypes.Identifier;
			}

			var number = ParserNumber.Select(value => tokenTypes.Number);
			var identifier = ParserIdentifier.Select(value => CheckGlslSpecifics(value));
			var op = ParserOperator.Select(value => tokenTypes.Operator);
			var token = comment.Or(preprocessor).Or(number).Or(identifier).Or(op);
			tokenParser = token.WithPosition().Token().XMany();
		}

		public IEnumerable<(int start, int length, TokenClassificationType type)> Tokenize(string text)
		{
			if (0 == text.Trim().Length) yield break;
			var tokens = tokenParser.TryParse(text);
			if (tokens.WasSuccessful)
			{
				foreach (var posToken in tokens.Value)
				{
					yield return (posToken.Start.Pos, posToken.Length, posToken.Value);
				}
			}
		}
	}
}
