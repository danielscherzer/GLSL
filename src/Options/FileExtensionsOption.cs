using DMS.GLSL.Contracts;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace DMS.GLSL.Options
{
	[Export(typeof(EditorOptionDefinition))]
	internal sealed partial class FileExtensionsOption : EditorOptionDefinition<string>
	{
		public readonly static EditorOptionKey<string> OptionKey = new EditorOptionKey<string>("GLSL highlighting file extensions");

		private static void RegisterFileExtensions(IFileExtensionRegistryService fileExtensionRegistry, string sExtensions, IContentType contentType, ILogger logger)
		{
			var extensions = sExtensions.Split(new char[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var ext in extensions)
			{
				try
				{
					fileExtensionRegistry.AddFileExtension(ext, contentType);
				}
				catch(InvalidOperationException ioe)
				{
					var titel = "GLSL language integration";
					var message = $"{titel}:Extension {ext} is ignored because it is already registered " +
						$"with a different Visual Studio component. " +
						$"Please remove it from the {titel} options page! Following is the detailed exception message {ioe}";
					logger.Log(message, true);
				}
			}
		}

		public override EditorOptionKey<string> Key => OptionKey;
	}
}
