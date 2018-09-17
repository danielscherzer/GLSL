using System;
using System.Collections.Generic;

namespace DMS.GLSL
{
	public enum GlslTokenTypes
	{
		Keyword, Function, Variable, Identifier, Comment, Operator, Number, PreprocessorKeyword
	}

	public static class GlslSpecification
	{
		public static HashSet<string> Keywords { get; private set; } = ParseTokens(Resources.glslKeywords);
		public static HashSet<string> Functions { get; private set; } = ParseTokens(Resources.glslFunctions);
		public static HashSet<string> Variables { get; private set; } = ParseTokens(Resources.glslVariables);

		private static HashSet<string> ParseTokens(string tokens)
		{
			char[] blanks = { ' ', '\n', '\r' };
			return new HashSet<string>(tokens.Split(blanks, StringSplitOptions.RemoveEmptyEntries));
		}

		public static GlslTokenTypes AssignType(string word)
		{
			if (Keywords.Contains(word)) return GlslTokenTypes.Keyword;
			if (Functions.Contains(word)) return GlslTokenTypes.Function;
			if (Variables.Contains(word)) return GlslTokenTypes.Variable;
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