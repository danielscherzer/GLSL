using DMS.GLSL.Language;

namespace GLSLTests.Language
{
	class TestTokenTypes : ITokenTypes<string>
	{
		public string Comment => nameof(Comment);

		public string Function => nameof(Function);

		public string Identifier => nameof(Identifier);

		public string Keyword => nameof(Keyword);

		public string Number => nameof(Number);

		public string Operator => nameof(Operator);

		public string PreprocessorKeyword => nameof(PreprocessorKeyword);

		public string UserKeyWord1 => nameof(UserKeyWord1);

		public string UserKeyWord2 => nameof(UserKeyWord2);

		public string Variable => nameof(Variable);

	}
}
