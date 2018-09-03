using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace DMS.GLSL.Classification
{
	[Export(typeof(IClassifierProvider))]
	[ContentType("glslShader")]
	[TagType(typeof(ClassificationTag))]
	internal class GlslTaggerProvider : IClassifierProvider, IPartImportsSatisfiedNotification
	{
		public void OnImportsSatisfied()
		{
			//var contentTypes = from element in contentTypeRegistryService.ContentTypes
			//				   select new { name = element.DisplayName, ances = element.BaseTypes };

			glslTokenTypeClassifications[GlslTokenTypes.Function] = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.Function);
			glslTokenTypeClassifications[GlslTokenTypes.Keyword] = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.Keyword);
			glslTokenTypeClassifications[GlslTokenTypes.Variable] = classificationTypeRegistry.GetClassificationType(GlslClassificationTypes.Variable);
			glslTokenTypeClassifications[GlslTokenTypes.Identifier] = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Identifier);
			glslTokenTypeClassifications[GlslTokenTypes.Comment] = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Comment);
			glslTokenTypeClassifications[GlslTokenTypes.Number] = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Number);
			glslTokenTypeClassifications[GlslTokenTypes.Operator] = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Operator);
			glslTokenTypeClassifications[GlslTokenTypes.PreprocessorKeyword] = classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.PreprocessorKeyword);
		}

		public IClassifier GetClassifier(ITextBuffer textBuffer)
		{
			ITagAggregator<GlslTokenTag> tagAggregator = aggregatorFactory.CreateTagAggregator<GlslTokenTag>(textBuffer);
			return new GlslClassifier(tagAggregator, glslTokenTypeClassifications);
		}

		[Import]
		internal IClassificationTypeRegistryService classificationTypeRegistry = null;

		[Import]
		internal IBufferTagAggregatorFactoryService aggregatorFactory = null;

		//[Import] internal IContentTypeRegistryService contentTypeRegistryService = null;

		IDictionary<GlslTokenTypes, IClassificationType> glslTokenTypeClassifications = new Dictionary<GlslTokenTypes, IClassificationType>();
	}

	internal sealed class GlslClassifier : IClassifier
	{
		ITagAggregator<GlslTokenTag> aggregator;
		IDictionary<GlslTokenTypes, IClassificationType> glslTypes;

		internal GlslClassifier(ITagAggregator<GlslTokenTag> glslTagAggregator, 
			IDictionary<GlslTokenTypes, IClassificationType> glslTokenTypeClassifications)
		{
			aggregator = glslTagAggregator;
			glslTypes = glslTokenTypeClassifications;
		}

		public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged { add { } remove { } }

		public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan inputSpan)
		{
			var output = new List<ClassificationSpan>();
			foreach (var tagSpan in aggregator.GetTags(inputSpan))
			{
				var tagSpans = tagSpan.Span.GetSpans(inputSpan.Snapshot);
				var type = glslTypes[tagSpan.Tag.Type];
				foreach (var span in tagSpans)
				{
					output.Add(new ClassificationSpan(span, type));
				}
			}
			return output;
		}
	}
}