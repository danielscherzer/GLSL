namespace DMS.GLSL.Language
{
	public interface IToken
	{
		int Length { get; }
		int Start { get; }
		TokenType Type { get; }
		string Value { get; }
	}
}