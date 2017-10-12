using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;

namespace DMS.GLSL
{
	[Export(typeof(ICompletionSourceProvider))]
	[ContentType("glslShader")]
	[Name("glslCompletion")]
	class GlslCompletionSourceProvider : ICompletionSourceProvider, IPartImportsSatisfiedNotification
	{
		public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
		{
			var tagger = glslTokenTaggerProvider.CreateTagger<GlslTokenTag>(textBuffer);
			return new GlslCompletionSource(textBuffer, staticCompletions, identifier, tagger);
		}

		public void OnImportsSatisfied()
		{
			var keyword = glyphService.GetGlyph(StandardGlyphGroup.GlyphKeyword, StandardGlyphItem.GlyphItemPublic);
			var function = glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupMethod, StandardGlyphItem.GlyphItemPublic);
			var variable = glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupVariable, StandardGlyphItem.GlyphItemPublic);
			identifier = glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupVariable, StandardGlyphItem.GlyphItemFriend);

			foreach (var var in GlslSpecification.Keywords) staticCompletions.Add(GlslCompletionSource.NewCompletion(var, keyword));
			foreach (var var in GlslSpecification.Functions) staticCompletions.Add(GlslCompletionSource.NewCompletion(var, function));
			foreach (var var in GlslSpecification.Variables) staticCompletions.Add(GlslCompletionSource.NewCompletion(var, variable));
			staticCompletions.Sort((a, b) => a.DisplayText.CompareTo(b.DisplayText));
		}

		[Import]
		internal GlslTokenTaggerProvider glslTokenTaggerProvider = null;
		[Import]
		private IGlyphService glyphService = null;

		private List<Completion> staticCompletions = new List<Completion>();
		private ImageSource identifier;
	}

	class GlslCompletionSource : ICompletionSource
	{
		private ITextBuffer currentBuffer;
		private bool _disposed = false;
		private IEnumerable<Completion> staticCompletions = new List<Completion>();
		private Func<SnapshotSpan, IEnumerable<string>> queryIdentifiers;
		private ImageSource imgIdentifier;

		public GlslCompletionSource(ITextBuffer buffer, IEnumerable<Completion> staticCompletions, 
			ImageSource identifier, ITagger<GlslTokenTag> tagger)
		{
			currentBuffer = buffer;
			this.staticCompletions = staticCompletions;
			imgIdentifier = identifier;

			queryIdentifiers = (snapshotSpan) =>
			{
				var tokens = tagger.GetTags(new NormalizedSnapshotSpanCollection(snapshotSpan));
				//only those tokens that are identifiers and do not overlap the input position because we do not want to add char that started session to completions
				var filtered = from token in tokens
							   where token.Tag.type == GlslTokenTypes.Identifier
								&& !token.Span.Contains(snapshotSpan.End - 1)
							   let text = token.Span.GetText()
							   orderby text
							   select text;
				return filtered.Distinct();
			};
		}

		public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
		{
			if (_disposed) throw new ObjectDisposedException(nameof(GlslCompletionSource));

			var snapshot = currentBuffer.CurrentSnapshot;
			var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(snapshot);

			if (triggerPoint == null) return;

			var completions = new List<Completion>();
			var startToPosition = new Span(0, triggerPoint.Position);
			foreach (var identifier in queryIdentifiers.Invoke(new SnapshotSpan(snapshot, startToPosition)))
			{
				completions.Add(NewCompletion(identifier, imgIdentifier));
			}
			completions.AddRange(staticCompletions);

			var start = NonIdentifierPositionBefore(triggerPoint);
			var applicableTo = snapshot.CreateTrackingSpan(new SnapshotSpan(start, triggerPoint), SpanTrackingMode.EdgeInclusive);
			completionSets.Add(new CompletionSet("All", "All", applicableTo, completions, Enumerable.Empty<Completion>()));
		}

		public static Completion NewCompletion(string text, ImageSource image)
		{
			return new Completion(text, text, null, image, null);
		}

		public void Dispose()
		{
			_disposed = true;
		}

		private static SnapshotPoint NonIdentifierPositionBefore(SnapshotPoint triggerPoint)
		{
			var line = triggerPoint.GetContainingLine();

			SnapshotPoint start = triggerPoint;

			while (start > line.Start && GlslSpecification.IsIdentifierChar((start - 1).GetChar()))
			{
				start -= 1;
			}

			return start;
		}
	}
}
