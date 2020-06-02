using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace DMS.GLSL.Classification
{
	class GlslClassificationTypes
	{
		public const string Function = nameof(glslFunction); //must be unique name!
		public const string Keyword = nameof(glslKeyword); //must be unique name!
		public const string Variable = nameof(glslVariable); //must be unique name!
		public const string UserKeyWord = nameof(glslUserKeyWord); //must be unique name!

#pragma warning disable 169 //"never used" warning
		[Export]
		[Name(Function)]
		[BaseDefinition("code")]
		private static readonly ClassificationTypeDefinition glslFunction;

		[Export]
		[Name(Keyword)]
		[BaseDefinition("code")]
		private static readonly ClassificationTypeDefinition glslKeyword;

		[Export]
		[Name(Variable)]
		[BaseDefinition("code")]
		private static readonly ClassificationTypeDefinition glslVariable;

		[Export]
		[Name(UserKeyWord)]
		[BaseDefinition("code")]
		private static readonly ClassificationTypeDefinition glslUserKeyWord;
#pragma warning restore 169
	}
}
