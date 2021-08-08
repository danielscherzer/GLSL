using DMS.GLSL.Contracts;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace DMS.GLSL.Outlining
{
	internal sealed class OutliningTagger : ITagger<IOutliningRegionTag>
	{
		public OutliningTagger(ITextBuffer textBuffer, IClassifier classifier, ILogger logger)
		{
			var observableSnapshot = Observable.Return(textBuffer.CurrentSnapshot).Concat(
				Observable.FromEventPattern<ClassificationChangedEventArgs>(h => classifier.ClassificationChanged += h, h => classifier.ClassificationChanged -= h)
				.Select(e => e.EventArgs.ChangeSpan.Snapshot));

			void UpdateRegionSpans()
			{
				var snapshot = textBuffer.CurrentSnapshot;
				var snapshotSpan = new SnapshotSpan(snapshot, 0, snapshot.Length);
				regionSpans = CalculateRegionSpans(classifier, snapshotSpan, logger);
				TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(snapshotSpan));
			}

			observableSnapshot
				.Throttle(TimeSpan.FromSeconds(0.3f))
				.Subscribe(snapshot => UpdateRegionSpans());
		}

		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			if (0 == spans.Count) yield break;
			var currentRegionSpans = regionSpans;
			if (0 == currentRegionSpans.Count) yield break;
			SnapshotSpan entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(currentRegionSpans[0].Snapshot, SpanTrackingMode.EdgeInclusive);

			foreach (var region in currentRegionSpans)
			{
				if (entire.OverlapsWith(region))
				{
					yield return new TagSpan<IOutliningRegionTag>(region, new OutliningRegionTag(false, false, ellipsis, region.GetText()));
				}
			}
		}

		private const char startHide = '{';     //the characters that start the outlining region
		private const char endHide = '}';       //the characters that end the outlining region
		private const string ellipsis = "...";    //the characters that are displayed when the region is collapsed
		private IList<SnapshotSpan> regionSpans = new List<SnapshotSpan>();

		private static IList<SnapshotSpan> CalculateRegionSpans(IClassifier classifier, SnapshotSpan snapshotSpan, ILogger logger)
		{
			var classificationSpans = classifier.GetClassificationSpans(snapshotSpan);
#if DEBUG
			logger.Log($"CalculateRegionSpans on {classificationSpans.Count} spans");
#endif

			var points = new Stack<SnapshotPoint>();
			var output = new List<SnapshotSpan>();

			foreach (var classificationSpan in classificationSpans)
			{
				if (classificationSpan.ClassificationType.IsOfType(PredefinedClassificationTypeNames.Operator))
				{
					var text = classificationSpan.Span.GetText();
					for (int i = 0; i < text.Length; ++i)
					{
						if (startHide == text[i])
						{
							points.Push(classificationSpan.Span.Start);
						}
						else if (endHide == text[i])
						{
							if (0 == points.Count) continue;
							var start = points.Pop();
							var end = classificationSpan.Span.Start + i;
							if (start.GetContainingLine().LineNumber != end.GetContainingLine().LineNumber)
							{
								var span = new SnapshotSpan(start, end + 1);
								output.Add(span);
							}
						}
					}
				}
			}
			return output;
		}
	}
}
