using Sprache;
using System.Collections.Generic;
using System.Linq;

namespace DMS.GLSL.Language
{
	class Lexer<TokenClassificationType>
	{
		static readonly Parser<string> NumberWithTrailingDigit = from number in Parse.Number
																 from trailingDot in Parse.Char('.')
																 select number + trailingDot;
		static readonly Parser<string> ParserNumber = Parse.DecimalInvariant.Or(NumberWithTrailingDigit);
		static readonly Parser<string> ParserComment = new CommentParser().AnyComment;
		static readonly Parser<string> ParserPreprocessor = from first in Parse.Char('#') 
															from rest in Parse.CharExcept('\n').Many().Text() 
															select rest;
		static readonly Parser<char> ParserUnderscore = Parse.Char('_');
		static readonly Parser<string> ParserIdentifier = Parse.Identifier(Parse.Letter.Or(ParserUnderscore), Parse.LetterOrDigit.Or(ParserUnderscore));
		static readonly Parser<string> ParserFunction = from i in ParserIdentifier
														from w in Parse.WhiteSpace.Optional()
														from op in Parse.Char('(')
														select i;
		static readonly Parser<char> ParserOperator = Parse.Chars(".;,+-*/()[]{}<>=&$!\"%?:|^");
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
