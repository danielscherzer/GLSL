using GLSLhelper;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;

namespace DMS.GLSL.CodeCompletion
{
	[Export(typeof(ICompletionSourceProvider))]
	[ContentType("glslShader")]
	[Name("glslCompletion")]
	internal class GlslCompletionSourceProvider : ICompletionSourceProvider
	{
		[ImportingConstructor]
		public GlslCompletionSourceProvider(IClassifierAggregatorService classifierAggregatorService, IGlyphService glyphService)
		{
			if (glyphService is null)
			{
				throw new ArgumentNullException(nameof(glyphService));
			}

			this.classifierAggregatorService = classifierAggregatorService ?? throw new ArgumentNullException(nameof(classifierAggregatorService));
			identifier = glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupVariable, StandardGlyphItem.GlyphItemFriend);

			var keyword = glyphService.GetGlyph(StandardGlyphGroup.GlyphKeyword, StandardGlyphItem.GlyphItemPublic);
			var function = glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupMethod, StandardGlyphItem.GlyphItemPublic);
			var variable = glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupVariable, StandardGlyphItem.GlyphItemPublic);
			ImageSource ConvertReservedType(TokenType type)
			{
				switch (type)
				{
					case TokenType.Keyword: return keyword;
					case TokenType.Function: return function;
					case TokenType.Variable: return variable;
					default: return identifier;
				}
			}
			foreach (var var in GlslSpecification.ReservedWords)
			{
				staticCompletions.Add(GlslCompletionSource.NewCompletion(var.Key, ConvertReservedType(var.Value)));
			}
			//ImageSource ConvertUser(UserKeyWords.DefinedWordType type)
			//{
			//	switch (type)
			//	{
			//		case UserKeyWords.DefinedWordType.UserKeyword1:
			//		case UserKeyWords.DefinedWordType.UserKeyword2:
			//		default: return identifier;
			//	}
			//}
			//foreach (var var in UserKeyWords.DefinedWords)
			//{
			//	staticCompletions.Add(GlslCompletionSource.NewCompletion(var.Key, ConvertUser(var.Value)));
			//}
			staticCompletions.Sort((a, b) => a.DisplayText.CompareTo(b.DisplayText));
		}

		public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
		{
			var classifier = classifierAggregatorService.GetClassifier(textBuffer);
			return new GlslCompletionSource(textBuffer, staticCompletions, identifier, classifier);
		}

		private readonly IClassifierAggregatorService classifierAggregatorService;
		private readonly ImageSource identifier;
		private readonly List<Completion> staticCompletions = new List<Completion>();
	}

	internal class GlslCompletionSource : ICompletionSource
	{
		private readonly ITextBuffer currentBuffer;
		private bool _disposed = false;
		private readonly IEnumerable<Completion> staticCompletions;
		private readonly Func<SnapshotSpan, IEnumerable<string>> queryIdentifiers;
		private readonly ImageSource imgIdentifier;

		public GlslCompletionSource(ITextBuffer buffer, IEnumerable<Completion> staticCompletions, ImageSource identifier, IClassifier classifier)
		{
			currentBuffer = buffer;
			this.staticCompletions = staticCompletions;
			imgIdentifier = identifier;

			queryIdentifiers = (snapshotSpan) =>
			{
				var tokens = classifier.GetClassificationSpans(snapshotSpan);
				//only those tokens that are identifiers and do not overlap the input position because we do not want to add char that started session to completions
				var filtered = from token in tokens
							   where token.ClassificationType.IsOfType(PredefinedClassificationTypeNames.Identifier)
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
