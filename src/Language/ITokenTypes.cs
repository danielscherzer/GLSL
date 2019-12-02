namespace DMS.GLSL.Language
{
	interface ITokenTypes<TYPE>
	{
		TYPE Comment { get; }
		TYPE Function { get; }
		TYPE Identifier { get; }
		TYPE Keyword { get; }
		TYPE Number { get; }
		TYPE Operator { get; }
		TYPE PreprocessorKeyword { get; }
		TYPE UserKeyWord { get; }
		TYPE Variable { get; }
	}
}