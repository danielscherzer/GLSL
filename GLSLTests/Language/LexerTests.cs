using DMS.GLSL;
using DMS.GLSL.Language;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace GLSLTests.Language
{
	[TestClass()]
	public class LexerTests
	{
		private static readonly TestTokenTypes tokenTypes = new TestTokenTypes();
		private static readonly GlslLexer<string> lexer = CreateLexer();

		[DataTestMethod()]
		[DynamicData(nameof(GetSingleTokenData), DynamicDataSourceType.Method)]
		public void TokenizeSingleTest(string text, string expectedType)
		{
			var (start, length, type) = lexer.Tokenize(text).First();
			Assert.AreEqual(expectedType, type);
			Assert.AreEqual(text, text.Substring(start, length));
		}

		[DataTestMethod()]
		[DynamicData(nameof(GetTokensData), DynamicDataSourceType.Method)]
		public void TokenizeTest(string text, string[] expectedTypes)
		{
			var tokens = lexer.Tokenize(text).ToArray();
			for(int i = 0; i < tokens.Length; ++i)
			{
				var (_, _, type) = tokens[i];
				Assert.AreEqual(expectedTypes[i], type);
			}
		}

		private static GlslLexer<string> CreateLexer()
		{
			GlslSpecification.ResetType(GlslSpecification.ReservedWordType.UserKeyword1, "boss sepp heinz");
			return new GlslLexer<string>(tokenTypes);
		}

		private static IEnumerable<object[]> GetSingleTokenData()
		{
			yield return new object[] { "// comment stuff", tokenTypes.Comment };
			yield return new object[] { "/* comment stuff\n\n # preprocessor */", tokenTypes.Comment };
			yield return new object[] { "max", tokenTypes.Function };
			yield return new object[] { "a", tokenTypes.Identifier };
			yield return new object[] { "_x123", tokenTypes.Identifier };
			yield return new object[] { "vec4", tokenTypes.Keyword };
			yield return new object[] { "1", tokenTypes.Number };
			yield return new object[] { ".1", tokenTypes.Number };
			yield return new object[] { "1.", tokenTypes.Number };
			yield return new object[] { "*", tokenTypes.Operator };
			yield return new object[] { "# pre processor stuff", tokenTypes.PreprocessorKeyword };
			yield return new object[] { "boss", tokenTypes.UserKeyWord1 };
			yield return new object[] { "gl_FragCoord", tokenTypes.Variable };
		}

		private static IEnumerable<object[]> GetTokensData()
		{
			yield return new object[] { "// comment stuff\n# prepor", new string[] { tokenTypes.Comment, tokenTypes.PreprocessorKeyword } };
			yield return new object[] { "# prepor\r\n/* comment stuff uniform float;\n\n*/", new string[] { tokenTypes.PreprocessorKeyword, tokenTypes.Comment } };
			yield return new object[] { "# version\n uniform float test", new string[] { tokenTypes.PreprocessorKeyword, tokenTypes.Keyword, tokenTypes.Keyword, tokenTypes.Identifier } };
			yield return new object[] { "gl_FragCoord = vec4(1.)", new string[] { tokenTypes.Variable, tokenTypes.Operator, tokenTypes.Keyword, tokenTypes.Operator, tokenTypes.Number, tokenTypes.Operator } };
		}
	}
}