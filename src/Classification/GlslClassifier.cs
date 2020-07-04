using DMS.GLSL.Contracts;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;

namespace DMS.GLSL.Classification
{
	internal sealed class GlslClassifier : IClassifier
	{
		internal GlslClassifier(ITextBuffer textBuffer, SyntaxColorParser parser, ILogger logger)
		{
			var observableSnapshot = Observable.Return(textBuffer.CurrentSnapshot).Concat(
				Observable.FromEventPattern<TextContentChangedEventArgs>(h => textBuffer.Changed += h, h => textBuffer.Changed -= h)
				.Select(e => e.EventArgs.After));

			void UpdateSpans()
			{
#if DEBUG
				var time = Stopwatch.StartNew();
#endif
				var snapshotSpan = new SnapshotSpan(textBuffer.CurrentSnapshot, 0, textBuffer.CurrentSnapshot.Length);
				var spans = parser.CalculateSpans(snapshotSpan);
#if DEBUG
				logger.Log($"[{DateTime.Now:hh:mm:ss:mmm}] {time.ElapsedTicks * 1e3f / Stopwatch.Frequency}ms : tokens={spans.Count}");
#endif
				this.spans = spans;
				ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(snapshotSpan));
			}

			observableSnapshot
				.Throttle(TimeSpan.FromSeconds(0.3f))
				.Subscribe(snapshot => UpdateSpans());

			//UpdateSpans(new SnapshotSpan(textBuffer.CurrentSnapshot, 0, textBuffer.CurrentSnapshot.Length));
			//textBuffer.Changed += (s, a) =>
			//{
			//	// ignore not up-to-date versions
			//	if (a.After != textBuffer.CurrentSnapshot) return;


			//	var start = a.Changes.Min(change => Math.Min(change.OldPosition, change.NewPosition));
			//	var end = a.Changes.Max(change => Math.Max(change.OldEnd, change.NewEnd));
			//	var length = Math.Min(end - start, textBuffer.CurrentSnapshot.Length);
			//	var changeSpan = new SnapshotSpan(textBuffer.CurrentSnapshot, start, length);
			//	var changeText = changeSpan.GetText();
			//	//if(changeText.Contains('{') || changeText.Contains('}') || changeText.Contains('*') || )
			//	//UpdateSpans(changeSpan);
			//	UpdateSpans(new SnapshotSpan(textBuffer.CurrentSnapshot, 0, textBuffer.CurrentSnapshot.Length));
			//};
		}

		public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

		public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan inputSpan)
		{
			var output = new List<ClassificationSpan>();
			var currentSpans = spans; // if UpdateSpans runs during execution we want to avoid any exceptions
			if (0 == currentSpans.Count) return output;
			var translatedInput = inputSpan.TranslateTo(currentSpans[0].Span.Snapshot, SpanTrackingMode.EdgeInclusive);

			foreach (var span in currentSpans)
			{
				if (translatedInput.OverlapsWith(span.Span))
				{
					output.Add(span);
				}
			}
			return output;
		}

		private IList<ClassificationSpan> spans = new List<ClassificationSpan>();
	}
}