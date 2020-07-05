using DMS.GLSL.Contracts;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace DMS.GLSL.Outlining
{
	[Export(typeof(ITaggerProvider))]
	[TagType(typeof(IOutliningRegionTag))]
	[ContentType("glslShader")]
	internal sealed class OutliningTaggerProvider : ITaggerProvider
	{
		[ImportingConstructor]
		public OutliningTaggerProvider(IClassifierAggregatorService classifierAggregatorService, ILogger logger)
		{
			this.classifierAggregatorService = classifierAggregatorService ?? throw new ArgumentNullException(nameof(classifierAggregatorService));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
		{
			if (buffer == null)
			{
				throw new ArgumentNullException(nameof(buffer));
			}
			//create a single tagger for each buffer.
			ITagger<T> sc() { return new OutliningTagger(buffer, classifierAggregatorService.GetClassifier(buffer), logger) as ITagger<T>; }
			return buffer.Properties.GetOrCreateSingletonProperty(sc);
		}

		private readonly IClassifierAggregatorService classifierAggregatorService;
		private readonly ILogger logger;
	}
}
