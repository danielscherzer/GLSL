using DMS.GLSL.Language;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;

namespace DMS.GLSL.Classification
{
	class TokenTypes : ITokenTypes<IClassificationType>
	{
		public TokenTypes(IClassificationTypeRegistryService classificationTypeRegistry)
		{
			Comment = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Comment);
			Function = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.Function);
			Identifier = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Identifier);
			Keyword = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.Keyword);
			Number = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Number);
			Operator = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Operator);
			PreprocessorKeyword = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.PreprocessorKeyword);
			UserKeyWord = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.UserKeyWord);
			Variable = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.Variable);
		}

		public IClassificationType Comment { get; }
		public IClassificationType Function { get; }
		public IClassificationType Identifier { get; }
		public IClassificationType Keyword { get; }
		public IClassificationType Number { get; }
		public IClassificationType Operator { get; }
		public IClassificationType PreprocessorKeyword { get; }
		public IClassificationType UserKeyWord { get; }
		public IClassificationType Variable { get; }
	}
}