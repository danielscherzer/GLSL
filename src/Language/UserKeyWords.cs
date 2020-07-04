using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.GLSL.Language
{
	public static class UserKeyWords
	{
		public enum DefinedWordType 
		{
			None,
			UserKeyword1,
			UserKeyword2
		};

		private static readonly Dictionary<string, DefinedWordType> s_definedWords = new Dictionary<string, DefinedWordType>();

		public static IEnumerable<KeyValuePair<string, DefinedWordType>> DefinedWords => s_definedWords;

		public static DefinedWordType GetDefinedWordType(string word)
		{
			if (s_definedWords.TryGetValue(word, out var type)) return type;
			return DefinedWordType.None;
		}

		public static void ResetType(DefinedWordType type, string userKeyWords)
		{
			s_definedWords.RemoveType(type);
			s_definedWords.AddRange(ParseWords(userKeyWords), type);
		}

		private static void AddRange(this Dictionary<string, DefinedWordType> result, IEnumerable<string> words, DefinedWordType type)
		{
			foreach (var word in words)
			{
				result[word] = type;
			}
		}

		private static HashSet<string> ParseWords(string words)
		{
			char[] blanks = { ' ' };
			return new HashSet<string>(words.Split(blanks, StringSplitOptions.RemoveEmptyEntries));
		}

		private static void RemoveType(this Dictionary<string, DefinedWordType> result, DefinedWordType type)
		{
			var keysToDelete = result.Where(t => type == t.Value).Select(t => t.Key).ToArray();
			foreach (var key in keysToDelete) result.Remove(key);
		}
	}
}
