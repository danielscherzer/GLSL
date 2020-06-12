
namespace DMS.GLSL.Classification
{
	using Microsoft.VisualStudio.Text.Classification;
	using Microsoft.VisualStudio.Utilities;
	using System.ComponentModel.Composition;

	internal class GlslClassificationTypes
	{
		public const string Function = nameof(glslFunction);
		public const string Keyword = nameof(glslKeyword);
		public const string Variable = nameof(glslVariable);
		public const string UserKeyWord1 = nameof(glslUserKeyWord1);
		public const string UserKeyWord2 = nameof(glslUserKeyWord2);

#pragma warning disable 169 //never used warning
		
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
		[Name(UserKeyWord1)]
		[BaseDefinition("code")]
		private static readonly ClassificationTypeDefinition glslUserKeyWord1;

		[Export]
		[Name(UserKeyWord2)]
		[BaseDefinition("code")]
		private static readonly ClassificationTypeDefinition glslUserKeyWord2;

#pragma warning restore 169 //never used warning
	}
}
