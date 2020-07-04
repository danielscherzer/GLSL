using Sprache;
using System.Collections.Generic;
using System.Linq;

namespace DMS.GLSL.Language
{
	public partial class GlslTokenizer
	{
		static readonly Parser<string> NumberWithTrailingDigit = from number in Parse.Number
																 from trailingDot in Parse.Char('.')
																 select number + trailingDot;
		static readonly Parser<string> ParserNumber = Parse.DecimalInvariant.Or(NumberWithTrailingDigit);
		static readonly Parser<string> ParserComment = new CommentParser().AnyComment;
		static readonly Parser<string> ParserPreprocessor = from first in Parse.Char('#')
															from rest in Parse.CharExcept('\n').Many().Text()
															select rest;
		static readonly Parser<string> ParserIdentifier = Parse.Identifier(Parse.Char(GlslSpecification.IsIdentifierStartChar, "Identifier start"),
																			Parse.Char(GlslSpecification.IsIdentifierChar, "Identifier character"));
		//static readonly Parser<string> ParserFunction = from i in ParserIdentifier
		//												from w in Parse.WhiteSpace.Optional()
		//												from op in Parse.Char('(')
		//												select i;
		static readonly Parser<char> ParserOperator = Parse.Chars(".;,+-*/()[]{}<>=&$!\"%?:|^");
		private readonly Parser<IEnumerable<Token>> tokenParser;

		public GlslTokenizer()
		{
			var comment = ParserComment.Select(value => new Token(TokenType.Comment, value));
			var preprocessor = ParserPreprocessor.Select(value => new Token(TokenType.Preprocessor, value));
			var number = ParserNumber.Select(value => new Token(TokenType.Number, value));
			var identifier = ParserIdentifier.Select(value => new Token(GlslSpecification.GetReservedWordType(value), value));
			var op = ParserOperator.Select(value => new Token(TokenType.Operator, value.ToString()));
			var token = comment.Or(preprocessor).Or(number).Or(identifier).Or(op);
			tokenParser = token.Positioned().Token().XMany();
		}

		public IEnumerable<IToken> Tokenize(string text)
		{
			if (0 == text.Trim().Length) yield break;
			var tokens = tokenParser.TryParse(text);
			if (tokens.WasSuccessful)
			{
				foreach (var token in tokens.Value)
				{
					yield return token;
				}
			}
		}
	}
}
