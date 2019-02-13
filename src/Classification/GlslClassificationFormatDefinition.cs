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
		public GlslVariableClassificationFormatDefinition()
		{
			DisplayName = "GLSL variable"; //human readable version of the name
			ForegroundColor = Colors.LawnGreen;
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = GlslClassificationTypes.UserKeyWord)]
	[Name(nameof(GlslUserKeyWordClassificationFormatDefinition))]
	//this should be visible to the end user
	[UserVisible(true)]
	//set the priority to be after the default classifiers
	[Order(Before = Priority.Default)]
	internal sealed class GlslUserKeyWordClassificationFormatDefinition : ClassificationFormatDefinition
	{
		public GlslUserKeyWordClassificationFormatDefinition()
		{
			DisplayName = "GLSL user key word"; //human readable version of the name
			ForegroundColor = Colors.DarkOrange;
		}
	}
}
