using System;
using System.Collections.Generic;

namespace DMS.GLSL.Language
{
	public static class UserKeyWords
	{
		public enum DefinedWordType 
		{ 
			None = GlslSpecification.ReservedWordType.None,
			Keyword = GlslSpecification.ReservedWordType.Keyword,
			Function = GlslSpecification.ReservedWordType.Function,
			Variable = GlslSpecification.ReservedWordType.Variable,
			UserKeyword1,
			UserKeyword2
		};

		private static readonly Dictionary<string, DefinedWordType> s_definedWords = new Dictionary<string, DefinedWordType>();

		public static IEnumerable<KeyValuePair<string, DefinedWordType>> DefinedWords => Initialize();

		//public static DefinedWordType GetDefinedWordType(string word)
		//{
		//	var type = GlslSpecification.GetReservedWordType(word);
		//	if(GlslSpecification.ReservedWordType.None == type)
		//	{
		//		if (s_definedWords.TryGetValue(word, out var type)) return type;
		//	}
		//	return ReservedWordType.None;
		//}

		private static IEnumerable<KeyValuePair<string, DefinedWordType>> Initialize()
		{
			return null;
		}
	}
}
