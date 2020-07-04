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
		public GlslClassifierProvider(IClassificationTypeRegistryService classificationTypeRegistry, ILogger logger)
		{
			this.logger = logger;
			parser = new SyntaxColorParser(classificationTypeRegistry);
		}

		public IClassifier GetClassifier(ITextBuffer textBuffer)
		{
			return textBuffer.Properties.GetOrCreateSingletonProperty(() => new GlslClassifier(textBuffer, parser, logger)); //per buffer classifier
		}

		private readonly ILogger logger;
		private readonly SyntaxColorParser parser;
	}
}