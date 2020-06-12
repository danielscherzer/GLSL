namespace DMS.GLSL.Language
{
	public interface ITokenTypes<TYPE>
	{
		TYPE Comment { get; }
		TYPE Function { get; }
		TYPE Identifier { get; }
		TYPE Keyword { get; }
		TYPE Number { get; }
		TYPE Operator { get; }
		TYPE PreprocessorKeyword { get; }
		TYPE UserKeyWord1 { get; }
		TYPE UserKeyWord2 { get; }
		TYPE Variable { get; }
	}
}