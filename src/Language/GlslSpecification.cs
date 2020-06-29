using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.GLSL.Language
{
	public static partial class GlslSpecification
	{
		public enum ReservedWordType { None, Keyword, Function, Variable, UserKeyword1, UserKeyword2 };
		public interface ITypeBuilder { void OnKeyword(string word); }

		private static readonly Dictionary<string, ReservedWordType> s_reservedWords = Initialize();

		public static IEnumerable<KeyValuePair<string, ReservedWordType>> ReservedWords => s_reservedWords;
		
		public static void ForEachReservedWord(ITypeBuilder builder)
		{
			foreach(var pair in s_reservedWords)
			{

			}
		}
		public static ReservedWordType GetReservedWordType(string word)
		{
			if (s_reservedWords.TryGetValue(word, out var type)) return type;
			return ReservedWordType.None;
		}

		public static bool IsIdentifierChar(char c) => char.IsLetterOrDigit(c) || '_' == c;

		public static bool IsIdentifierStartChar(char c) => char.IsLetter(c) || '_' == c;

		public static void ResetType(ReservedWordType type, string userKeyWords)
		{
			s_reservedWords.RemoveType(type);
			s_reservedWords.AddRange(ParseTokens(userKeyWords), type);
		}

		private static void AddRange(this Dictionary<string, ReservedWordType> result, IEnumerable<string> words, ReservedWordType type)
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

		private static void RemoveType(this Dictionary<string, ReservedWordType> result, ReservedWordType type)
		{
			var keysToDelete = result.Where(t => type == t.Value).Select(t => t.Key).ToArray();
			foreach (var key in keysToDelete) result.Remove(key);
		}
	}
}