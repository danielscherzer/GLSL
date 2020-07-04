using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DMS.GLSL.Language.Tests
{
	[TestClass()]
	public class GlslTokenizerTests
	{
		[DataTestMethod()]
		[DynamicData(nameof(GetSingleTokenData), DynamicDataSourceType.Method)]
		public void TokenizeSingleTest(string text, TokenTypes expectedType)
		{
			var tokenizer = new GlslTokenizer();
			var token = tokenizer.Tokenize(text).First();
			Assert.AreEqual(expectedType, token.Type);
			Assert.AreEqual(text, text.Substring(token.Start, token.Length));
		}

		[DataTestMethod()]
		[DynamicData(nameof(GetTokensData), DynamicDataSourceType.Method)]
		public void TokenizeTest(string text, TokenTypes[] expectedTypes)
		{
			var tokenizer = new GlslTokenizer();
			var tokens = tokenizer.Tokenize(text).ToArray();
			for (int i = 0; i < tokens.Length; ++i)
			{
				Assert.AreEqual(expectedTypes[i], tokens[i].Type);
			}
		}

		private static IEnumerable<object[]> GetSingleTokenData()
		{
			yield return new object[] { "// comment stuff", TokenTypes.Comment };
			yield return new object[] { "/* comment stuff\n\n # preprocessor */", TokenTypes.Comment };
			yield return new object[] { "max", TokenTypes.Function };
			yield return new object[] { "a", TokenTypes.Identifier };
			yield return new object[] { "_x123", TokenTypes.Identifier };
			yield return new object[] { "vec4", TokenTypes.Keyword };
			yield return new object[] { "1", TokenTypes.Number };
			yield return new object[] { ".1", TokenTypes.Number };
			yield return new object[] { "1.", TokenTypes.Number };
			yield return new object[] { "*", TokenTypes.Operator };
			yield return new object[] { "# pre processor stuff", TokenTypes.Preprocessor };
			yield return new object[] { "gl_FragCoord", TokenTypes.Variable };
		}

		private static IEnumerable<object[]> GetTokensData()
		{
			yield return new object[] { "// comment stuff\n# prepor", new TokenTypes[] { TokenTypes.Comment, TokenTypes.Preprocessor } };
			yield return new object[] { "# prepor\r\n/* comment stuff uniform float;\n\n*/", new TokenTypes[] { TokenTypes.Preprocessor, TokenTypes.Comment } };
			yield return new object[] { "# version\n uniform float test", new TokenTypes[] { TokenTypes.Preprocessor, TokenTypes.Keyword, TokenTypes.Keyword, TokenTypes.Identifier } };
			yield return new object[] { "gl_FragCoord = vec4(1.)", new TokenTypes[] { TokenTypes.Variable, TokenTypes.Operator, TokenTypes.Keyword, TokenTypes.Operator, TokenTypes.Number, TokenTypes.Operator } };
		}
	}
}