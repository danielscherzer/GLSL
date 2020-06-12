using Sprache;
using System.Collections.Generic;
using System.Linq;

namespace DMS.GLSL.Language
{
	public class GlslLexer<TokenClassificationType>
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
		private readonly Parser<IEnumerable<PositionAware<TokenClassificationType>>> tokenParser;

		public GlslLexer(ITokenTypes<TokenClassificationType> tokenTypes)
		{
			var comment = ParserComment.Select(value => tokenTypes.Comment);
			var preprocessor = ParserPreprocessor.Select(value => tokenTypes.PreprocessorKeyword);

			TokenClassificationType Convert(string word)
			{
				switch(GlslSpecification.GetDefinedWordType(word))
				{
					case GlslSpecification.DefinedWordType.Keyword: return tokenTypes.Keyword;
					case GlslSpecification.DefinedWordType.Function: return tokenTypes.Function;
					case GlslSpecification.DefinedWordType.Variable: return tokenTypes.Variable;
					case GlslSpecification.DefinedWordType.UserKeyword1: return tokenTypes.UserKeyWord1;
					case GlslSpecification.DefinedWordType.UserKeyword2: return tokenTypes.UserKeyWord2;
					default: return tokenTypes.Identifier;
				}
			}

			var number = ParserNumber.Select(value => tokenTypes.Number);
			var identifier = ParserIdentifier.Select(value => Convert(value));
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
