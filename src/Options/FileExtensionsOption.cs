namespace DMS.GLSL.Options
{
	using DMS.GLSL.Errors;
	using Microsoft.VisualStudio.Text.Editor;
	using Microsoft.VisualStudio.Utilities;
	using System;
	using System.ComponentModel.Composition;

	[Export(typeof(EditorOptionDefinition))]
	public sealed partial class FileExtensionsOption : EditorOptionDefinition<string>
	{
		public readonly static EditorOptionKey<string> OptionKey = new EditorOptionKey<string>("GLSL highlighting file extensions");

		private static void RegisterFileExtensions(IFileExtensionRegistryService fileExtensionRegistry, string sExtensions, IContentType contentType)
		{
			var extensions = sExtensions.Split(new char[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var ext in extensions)
			{
				try
				{
					fileExtensionRegistry.AddFileExtension(ext, contentType);
				}
				catch(InvalidOperationException)
				{
					var titel = "GLSL language integration";
					var message = $"{titel}:Extension {ext} is ignored because it is already registered " +
						$"with a different Visual Studio component. " +
						$"Please remove it from the {titel} options page!";
					OutMessage.PaneAndBar(message);
				}
			}
		}

		public override EditorOptionKey<string> Key => OptionKey;
	}
}
