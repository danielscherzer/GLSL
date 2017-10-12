namespace DMS.GLSL
{
	public enum GlslTokenTypes
	{
		Keyword, Function, Variable, Identifier, Comment, Operator, Number, PreprocessorKeyword
	}

	public static class GlslSpecification
	{
		public static GlslTypeInstances Keywords { get; private set; } = new GlslTypeInstances(Resources.glslKeywords);
		public static GlslTypeInstances Functions { get; private set; } = new GlslTypeInstances(Resources.glslFunctions);
		public static GlslTypeInstances Variables { get; private set; } = new GlslTypeInstances(Resources.glslVariables);

		public static GlslTokenTypes AssignType(string predefinedWord)
		{
			if (Keywords.IsInstance(predefinedWord))
			{
				return GlslTokenTypes.Keyword;
			}
			else if (Functions.IsInstance(predefinedWord))
			{
				return GlslTokenTypes.Function;
			}
			else if (Variables.IsInstance(predefinedWord))
			{
				return GlslTokenTypes.Variable;
			}
			return GlslTokenTypes.Identifier;
		}

		public static bool IsIdentifierChar(char c)
		{
			return char.IsLetterOrDigit(c) || '_' == c;
		}
		public static bool IsIdentifierStartChar(char c)
		{
			return char.IsLetter(c) || '_' == c;
		}
	}
}