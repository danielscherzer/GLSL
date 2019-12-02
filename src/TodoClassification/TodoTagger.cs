using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace DMS.GLSL.TodoClassification
{
	[Export(typeof(ITaggerProvider))]
	[ContentType("code")]
	[TagType(typeof(TodoTag))]
	internal class TodoTaggerProvider : ITaggerProvider
	{
		[Import] private IClassifierAggregatorService classifierAggregatorService = null;

		public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			return new TodoTagger(classifierAggregatorService.GetClassifier(buffer)) as ITagger<T>;
		}
	}

	internal class TodoTag : IGlyphTag
	{
	}

	internal class TodoTagger : ITagger<TodoTag>
	{
		public event EventHandler<SnapshotSpanEventArgs> TagsChanged { add { } remove { } }

		private IClassifier classifier;
		private const string m_searchText = "todo";

		internal TodoTagger(IClassifier classifier)
		{
			this.classifier = classifier;
		}

		IEnumerable<ITagSpan<TodoTag>> ITagger<TodoTag>.GetTags(NormalizedSnapshotSpanCollection spans)
		{
			foreach (SnapshotSpan span in spans)
			{
				//look at each classification span
				foreach (ClassificationSpan classification in classifier.GetClassificationSpans(span))
				{
					//if the classification is a comment
					if (classification.ClassificationType.IsOfType(PredefinedClassificationTypeNames.Comment))
					{
						//if the word "todo" is in the comment,
						//create a new TodoTag TagSpan
						int index = classification.Span.GetText().ToLower().IndexOf(m_searchText);
						if (index != -1)
						{
							yield return new TagSpan<TodoTag>(new SnapshotSpan(classification.Span.Start + index, m_searchText.Length), new TodoTag());
						}
					}
				}
			}
		}
	}
}
