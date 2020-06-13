using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.GLSL.Language
{
	public static partial class GlslSpecification
	{
		public enum DefinedWordType { Identifier, Keyword, Function, Variable, UserKeyword1, UserKeyword2 };

		private static readonly Dictionary<string, DefinedWordType> s_definedWords = Initialize();

		public static IEnumerable<KeyValuePair<string, DefinedWordType>> DefinedWords => s_definedWords;
		
		public static DefinedWordType GetDefinedWordType(string word)
		{
			if (s_definedWords.TryGetValue(word, out var type)) return type;
			return DefinedWordType.Identifier;
		}

		public static bool IsIdentifierChar(char c) => char.IsLetterOrDigit(c) || '_' == c;

		public static bool IsIdentifierStartChar(char c) => char.IsLetter(c) || '_' == c;

		public static void ResetType(DefinedWordType type, string userKeyWords)
		{
			s_definedWords.RemoveType(type);
			s_definedWords.AddRange(ParseTokens(userKeyWords), type);
		}

		private static void AddRange(this Dictionary<string, DefinedWordType> result, IEnumerable<string> words, DefinedWordType type)
		{
			foreach (var word in words)
			{
				result[word] = type;
			}
		}

		private static HashSet<string> ParseTokens(string tokens)
		{
			char[] blanks = { ' ' };
			return new HashSet<string>(tokens.Split(blanks, StringSplitOptions.RemoveEmptyEntries));
		}

		private static void RemoveType(this Dictionary<string, DefinedWordType> result, DefinedWordType type)
		{
			var keysToDelete = result.Where(t => type == t.Value).Select(t => t.Key).ToArray();
			foreach (var key in keysToDelete) result.Remove(key);
		}
	}
}