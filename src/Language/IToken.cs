namespace DMS.GLSL.Language
{
	public interface IToken
	{
		int Length { get; }
		int Start { get; }
		TokenTypes Type { get; }
		string Value { get; }
	}
}