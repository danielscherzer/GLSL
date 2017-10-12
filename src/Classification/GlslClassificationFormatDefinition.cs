using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace DMS.GLSL.Classification
{
	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = GlslClassificationTypes.Keyword)]
	[Name(nameof(GlslReservedWordClassificationFormatDefinition))]
	//this should be visible to the end user
	[UserVisible(true)]
	//set the priority to be after the default classifiers
	[Order(Before = Priority.Default)]
	internal sealed class GlslReservedWordClassificationFormatDefinition : ClassificationFormatDefinition
	{
		/// <summary>
		/// Defines the visual format for the "exclamation" classification type
		/// </summary>
		public GlslReservedWordClassificationFormatDefinition()
		{
			DisplayName = "GLSL keyword"; //human readable version of the name
			ForegroundColor = Colors.CornflowerBlue;
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = GlslClassificationTypes.Function)]
	[Name(nameof(GlslFunctionClassificationFormatDefinition))]
	//this should be visible to the end user
	[UserVisible(true)]
	//set the priority to be after the default classifiers
	[Order(Before = Priority.Default)]
	internal sealed class GlslFunctionClassificationFormatDefinition : ClassificationFormatDefinition
	{
		/// <summary>
		/// Defines the visual format for the "exclamation" classification type
		/// </summary>
		public GlslFunctionClassificationFormatDefinition()
		{
			DisplayName = "GLSL function"; //human readable version of the name
			ForegroundColor = Colors.Orange;
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = GlslClassificationTypes.Variable)]
	[Name(nameof(GlslVariableClassificationFormatDefinition))]
	//this should be visible to the end user
	[UserVisible(true)]
	//set the priority to be after the default classifiers
	[Order(Before = Priority.Default)]
	internal sealed class GlslVariableClassificationFormatDefinition : ClassificationFormatDefinition
	{
		/// <summary>
		/// Defines the visual format for the "exclamation" classification type
		/// </summary>
		public GlslVariableClassificationFormatDefinition()
		{
			DisplayName = "GLSL variable"; //human readable version of the name
			ForegroundColor = Colors.LawnGreen;
		}
	}
}
