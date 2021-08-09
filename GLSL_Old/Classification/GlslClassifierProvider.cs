using DMS.GLSL.Contracts;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace DMS.GLSL.Classification
{
	[Export(typeof(IClassifierProvider))]
	[ContentType("glslShader")]
	[TagType(typeof(ClassificationTag))]
	internal class GlslClassifierProvider : IClassifierProvider
	{
		[ImportingConstructor]
		public GlslClassifierProvider(IClassificationTypeRegistryService classificationTypeRegistry, ILogger logger, IUserKeywords userKeywords)
		{
			if (classificationTypeRegistry is null)
			{
				throw new System.ArgumentNullException(nameof(classificationTypeRegistry));
			}

			if (userKeywords is null)
			{
				throw new System.ArgumentNullException(nameof(userKeywords));
			}

			this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
			parser = new SyntaxColorParser(classificationTypeRegistry, userKeywords);
		}

		public IClassifier GetClassifier(ITextBuffer textBuffer)
		{
			return textBuffer.Properties.GetOrCreateSingletonProperty(() => new GlslClassifier(textBuffer, parser, logger)); //per buffer classifier
		}

		private readonly ILogger logger;
		private readonly SyntaxColorParser parser;
	}
}