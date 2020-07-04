using System.Collections.Generic;

namespace DMS.GLSL.Language
{
	public static partial class GlslSpecification
	{
		public enum ReservedWordType { None, Keyword, Function, Variable };
		//public interface ITypeBuilder { void OnKeyword(string word); }

		private static readonly Dictionary<string, ReservedWordType> s_reservedWords = Initialize();

		public static IEnumerable<KeyValuePair<string, ReservedWordType>> ReservedWords => s_reservedWords;
		
		//public static void ForEachReservedWord(ITypeBuilder builder)
		//{
		//	foreach(var pair in s_reservedWords)
		//	{

		//	}
		//}

		public static ReservedWordType GetReservedWordType(string word)
		{
			if (s_reservedWords.TryGetValue(word, out var type)) return type;
			return ReservedWordType.None;
		}

		public static bool IsIdentifierChar(char c) => char.IsLetterOrDigit(c) || '_' == c;

		public static bool IsIdentifierStartChar(char c) => char.IsLetter(c) || '_' == c;

		private static void AddRange(this Dictionary<string, ReservedWordType> result, IEnumerable<string> words, ReservedWordType type)
		{
			foreach (var word in words)
			{
				result[word] = type;
			}
		}
	}
}