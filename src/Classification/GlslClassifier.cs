using DMS.GLSL.Errors;
using DMS.GLSL.Language;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;

namespace DMS.GLSL.Classification
{
	[Export(typeof(IClassifierProvider))]
	[ContentType("glslShader")]
	[TagType(typeof(ClassificationTag))]
	internal class GlslClassifierProvider : IClassifierProvider, IPartImportsSatisfiedNotification
	{
		public void OnImportsSatisfied()
		{
			var tokenTypes = new TokenTypes(classificationTypeRegistry);
			lexer = new GlslLexer<IClassificationType>(tokenTypes);
		}

		public IClassifier GetClassifier(ITextBuffer textBuffer)
		{
			return textBuffer.Properties.GetOrCreateSingletonProperty(() => new GlslClassifier(textBuffer, lexer)); //per buffer classifier
		}

		[Import]
		internal IClassificationTypeRegistryService classificationTypeRegistry = null;
		private GlslLexer<IClassificationType> lexer;
	}

	internal sealed class GlslClassifier : IClassifier
	{
		internal GlslClassifier(ITextBuffer textBuffer, GlslLexer<IClassificationType> lexer)
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
				var spans = CalculateSpans(lexer, snapshotSpan);
#if DEBUG
				OutMessage.OutputWindowPane($"[{DateTime.Now:hh:mm:ss:mmm}] {time.ElapsedTicks * 1e3f / Stopwatch.Frequency}ms : tokens={spans.Count}");
#endif
				this.spans = spans;
				ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(snapshotSpan));
			}

			observableSnapshot
				.Throttle(TimeSpan.FromSeconds(0.3f))
				.Subscribe(snapshot => UpdateSpans());
			Lexer = lexer;

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

		public GlslLexer<IClassificationType> Lexer { get; }

		private static IList<ClassificationSpan> CalculateSpans(GlslLexer<IClassificationType> lexer, SnapshotSpan snapshotSpan)
		{
			var output = new List<ClassificationSpan>();
			var text = snapshotSpan.GetText();
			foreach (var (start, length, type) in lexer.Tokenize(text))
			{
				var lineSpan = new SnapshotSpan(snapshotSpan.Snapshot, start, length);
				output.Add(new ClassificationSpan(lineSpan, type));
//				if (type == lexer.TokenTypes.Identifier)
//				{
//#if DEBUG
//					OutMessage.OutputWindowPane(text.Substring(start, length));
//#endif
//				}
			}
			return output;
		}
	}
}