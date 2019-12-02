using System;
using System.Collections.Generic;

namespace DMS.GLSL
{
	public static class GlslSpecification
	{
		private static readonly HashSet<string> s_keywords = ParseTokens(Resources.glslKeywords);
		private static readonly HashSet<string> s_functions = ParseTokens(Resources.glslFunctions);
		private static readonly HashSet<string> s_variables = ParseTokens(Resources.glslVariables);
		private static HashSet<string> s_userKeyWords = new HashSet<string>();

		public static IEnumerable<string> Keywords => s_keywords;
		public static IEnumerable<string> Functions => s_functions;
		public static IEnumerable<string> Variables => s_variables;
		public static IEnumerable<string> UserKeyWords => s_userKeyWords;

		public static bool IsKeyword(string word) => s_keywords.Contains(word);
		public static bool IsVariable(string word) => s_variables.Contains(word);
		public static bool IsFunction(string word) => s_functions.Contains(word);
		public static bool IsUserKeyWord(string word) => s_userKeyWords.Contains(word);

		public static bool IsIdentifierChar(char c)
		{
			return char.IsLetterOrDigit(c) || '_' == c;
		}

		public static bool IsIdentifierStartChar(char c)
		{
			return char.IsLetter(c) || '_' == c;
		}

		public static void SetUserKeyWords(string userKeyWords) => s_userKeyWords = ParseTokens(userKeyWords);

		private static HashSet<string> ParseTokens(string tokens)
		{
			char[] blanks = { ' ', '\n', '\r' };
			return new HashSet<string>(tokens.Split(blanks, StringSplitOptions.RemoveEmptyEntries));
		}
	}
}