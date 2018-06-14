using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace DMS.GLSL
{
	public class GlslTokenTag : ITag
	{
		public GlslTokenTypes Type { get; private set; }

		public GlslTokenTag(GlslTokenTypes type)
		{
			this.Type = type;
		}
	}

	[Export(typeof(ITaggerProvider))]
	[Export(typeof(GlslTokenTaggerProvider))]
	[PartCreationPolicy(CreationPolicy.Shared)] //default singleton behavior
	[ContentType("glslShader")]
	[TagType(typeof(GlslTokenTag))]
	internal class GlslTokenTaggerProvider : ITaggerProvider
	{
		public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
		{
			if (cppClassifierProvider is null) throw new ArgumentNullException(nameof(cppClassifierProvider));
			if (buffer is null) throw new ArgumentNullException(nameof(buffer));
			if (!m_bufferTagger.TryGetValue(buffer, out GlslTokenTagger tagger))
			{
				if(cppClassifierProvider is null)
				{
					return null;
				}
				var cppClassifier = cppClassifierProvider.GetClassifier(buffer);
				tagger = new GlslTokenTagger(cppClassifier);
				buffer.Changed += tagger.TextChanged;
				m_bufferTagger[buffer] = tagger;
			}
			return tagger as ITagger<T>;
		}

		[Import(typeof(CppClassifier))]
		private IClassifierProvider cppClassifierProvider = null;

		private Dictionary<ITextBuffer, GlslTokenTagger> m_bufferTagger = new Dictionary<ITextBuffer, GlslTokenTagger>();
	}

	internal class GlslTokenTagger : ITagger<GlslTokenTag>
	{
		public event EventHandler<SnapshotSpanEventArgs> TagsChanged { add { } remove { } }

		internal GlslTokenTagger(IClassifier cppClassifier)
		{
			m_cppClassifier = cppClassifier;
		}

		IEnumerable<ITagSpan<GlslTokenTag>> ITagger<GlslTokenTag>.GetTags(NormalizedSnapshotSpanCollection inputSpans)
		{
			foreach (SnapshotSpan inputSpan in inputSpans)
			{
				foreach (var classification in m_cppClassifier.GetClassificationSpans(inputSpan))
				{
					var span = classification.Span;
					var containingLine = span.Start.GetContainingLine();
					var lineText = containingLine.GetText().TrimStart(new char[] { ' ', '\t' });
					//preprocessor lines
					if (lineText.StartsWith("#"))
					{
						var tag = new GlslTokenTag(GlslTokenTypes.PreprocessorKeyword);
						//whole line as a span; this could lead to overlapping spans if m_cppClassifier finds multiple spans in line
						var lineSpan = new SnapshotSpan(span.Snapshot, containingLine.Start, containingLine.Length);
						var tagSpan = new TagSpan<GlslTokenTag>(lineSpan, tag);
						yield return tagSpan;
					}
					else
					{
						//everything else
						var spanText = span.GetText();
						var type = ClassificationToTokenType(classification.ClassificationType.Classification, spanText);
						var tag = new GlslTokenTag(type);
						//if(GlslTokenTypes.Comment == type && spanText.StartsWith("/*"))
						//{
						//	var start = span.Start;
						//	var text = span.Snapshot.GetText().Substring(start);
						//	var indexEnd = text.IndexOf("*/");
						//	if (-1 == indexEnd) indexEnd = text.Length;
						//	span = new SnapshotSpan(span.Snapshot, start, indexEnd);
						//}
						var tagSpan = new TagSpan<GlslTokenTag>(span, tag);
						yield return tagSpan;
					}
				}
			}
		}

		internal void TextChanged(object sender, TextContentChangedEventArgs e)
		{
			foreach(var change in e.Changes)
			{
			}
		}

		private IClassifier m_cppClassifier;

		private static GlslTokenTypes ClassificationToTokenType(string classification, string word)
		{
			if (classification == PredefinedClassificationTypeNames.Comment) return GlslTokenTypes.Comment;
			if (classification == PredefinedClassificationTypeNames.Number) return GlslTokenTypes.Number;
			if (classification == PredefinedClassificationTypeNames.Operator) return GlslTokenTypes.Operator;
			return GlslSpecification.AssignType(word);
		}
	}
}