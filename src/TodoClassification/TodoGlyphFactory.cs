using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

using System.ComponentModel.Composition;
using System.Windows;

namespace DMS.GLSL.TodoClassification
{
    internal class TodoGlyphFactory : IGlyphFactory
    {
        //TODO: Use or Remove
        //const double m_glyphSize = 14.0;

        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            // Ensure we can draw a glyph for this marker.
            if (tag == null || !(tag is TodoTag))
            {
                return null;
            }
            return new TodoGlyph();
        }
    }

    [Export(typeof(IGlyphFactoryProvider))]
    [Name("TodoGlyph")]
    [Order(Before = "VsTextMarker")]
    [ContentType("code")]
    [TagType(typeof(TodoTag))]
    internal sealed class TodoGlyphFactoryProvider : IGlyphFactoryProvider
    {
        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin)
        {
            return new TodoGlyphFactory();
        }
    }
}
