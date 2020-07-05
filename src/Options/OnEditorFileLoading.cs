using Microsoft.VisualStudio.Composition;
using Microsoft.VisualStudio.Text.Editor;
using System.ComponentModel.Composition;

namespace DMS.GLSL.Options
{
	[Export(typeof(EditorOptionDefinition))] //will run constructor as soon as any file is loaded into the VS editor 
	internal sealed partial class OnEditorFileLoading : EditorOptionDefinition<string>
	{

		public readonly static EditorOptionKey<string> OptionKey = new EditorOptionKey<string>("GLSL highlighting file extensions");

		[ImportingConstructor]
		public OnEditorFileLoading(RegisterVSFileExtensions fileExtensions)
		{
			if (fileExtensions is null)
			{
				throw new System.ArgumentNullException(nameof(fileExtensions));
			}
			// registers file extensions
		}

		public override EditorOptionKey<string> Key => OptionKey;
	}
}
