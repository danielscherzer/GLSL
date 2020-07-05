
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace DMS.GLSL.Classification
{
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
			DisplayName = "GLSL Function"; //human readable version of the name
			ForegroundColor = Colors.Orange;
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = GlslClassificationTypes.Keyword)]
	[Name(nameof(GlslKeywordClassificationFormatDefinition))]
	//this should be visible to the end user
	[UserVisible(true)]
	//set the priority to be after the default classifiers
	[Order(Before = Priority.Default)]
	internal sealed class GlslKeywordClassificationFormatDefinition : ClassificationFormatDefinition
	{
		public GlslKeywordClassificationFormatDefinition()
		{
			DisplayName = "GLSL Keyword"; //human readable version of the name
			ForegroundColor = Colors.CornflowerBlue;
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
			DisplayName = "GLSL Variable"; //human readable version of the name
			ForegroundColor = Colors.LawnGreen;
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = GlslClassificationTypes.UserKeyword1)]
	[Name(nameof(GlslUserKeyword1ClassificationFormatDefinition))]
	//this should be visible to the end user
	[UserVisible(true)]
	//set the priority to be after the default classifiers
	[Order(Before = Priority.Default)]
	internal sealed class GlslUserKeyword1ClassificationFormatDefinition : ClassificationFormatDefinition
	{
		public GlslUserKeyword1ClassificationFormatDefinition()
		{
			DisplayName = "GLSL UserKeyword1"; //human readable version of the name
			ForegroundColor = Colors.DarkOrange;
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = GlslClassificationTypes.UserKeyword2)]
	[Name(nameof(GlslUserKeyword2ClassificationFormatDefinition))]
	//this should be visible to the end user
	[UserVisible(true)]
	//set the priority to be after the default classifiers
	[Order(Before = Priority.Default)]
	internal sealed class GlslUserKeyword2ClassificationFormatDefinition : ClassificationFormatDefinition
	{
		public GlslUserKeyword2ClassificationFormatDefinition()
		{
			DisplayName = "GLSL UserKeyword2"; //human readable version of the name
			ForegroundColor = Colors.DarkSalmon;
		}
	}

}
