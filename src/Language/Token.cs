using Sprache;

namespace DMS.GLSL.Language
{
	internal class Token : IPositionAware<Token>, IToken
	{
		public Token(TokenType type, string value)
		{
			Type = type;
			Value = value;
		}

		public int Length { get; private set; }
		public int Start { get; private set; }
		public TokenType Type { get; private set; }
		public string Value { get; private set; }

		public Token SetPos(Position startPos, int length)
		{
			Start = startPos.Pos;
			Length = length;
			return this;
		}
	}
}