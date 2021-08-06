
namespace DMS.GLSL.Classification
{
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    using System.ComponentModel.Composition;

    internal static class GlslClassificationTypes
    {
        public const string Function = nameof(glslFunction); public const string Keyword = nameof(glslKeyword); public const string Variable = nameof(glslVariable); public const string UserKeyword1 = nameof(glslUserKeyword1); public const string UserKeyword2 = nameof(glslUserKeyword2);
#pragma warning disable 169 //never used warning

        [Export]
        [Name(Function)]
        [BaseDefinition(PredefinedClassificationTypeNames.Keyword)]
        private static readonly ClassificationTypeDefinition glslFunction;

        [Export]
        [Name(Keyword)]
        [BaseDefinition(PredefinedClassificationTypeNames.Keyword)]
        private static readonly ClassificationTypeDefinition glslKeyword;

        [Export]
        [Name(Variable)]
        [BaseDefinition(PredefinedClassificationTypeNames.Keyword)]
        private static readonly ClassificationTypeDefinition glslVariable;

        [Export]
        [Name(UserKeyword1)]
        [BaseDefinition(PredefinedClassificationTypeNames.Identifier)]
        private static readonly ClassificationTypeDefinition glslUserKeyword1;

        [Export]
        [Name(UserKeyword2)]
        [BaseDefinition(PredefinedClassificationTypeNames.Identifier)]
        private static readonly ClassificationTypeDefinition glslUserKeyword2;
#pragma warning restore 169 //never used warning
    }
}
