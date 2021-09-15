﻿using System;
using System.ComponentModel.Composition;

using DMS.GLSL.Contracts;

using Microsoft.VisualStudio.Utilities;

namespace DMS.GLSL.Options
{
    [Export(typeof(RegisterVSFileExtensions))]
    internal sealed partial class RegisterVSFileExtensions
    {
        private static void RegisterFileExtensions(IFileExtensionRegistryService fileExtensionRegistry, string sExtensions, IContentType contentType, ILogger logger)
        {
            var extensions = sExtensions.Split(new char[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var ext in extensions)
            {
                try
                {
                    //fileExtensionRegistry.RemoveFileExtension(ext);
                    fileExtensionRegistry.AddFileExtension(ext, contentType);
                }
                catch (InvalidOperationException ioe)
                {
                    var otherContentType = fileExtensionRegistry.GetContentTypeForExtension(ext);
                    const string title = "GLSL language integration";
                    var message = $"{title}:Extension '{ext}' is ignored because it is already registered " +
                        $"with the content type '{otherContentType.TypeName}'. " +
                        $"Please use a different extension on the {title} options page!" +
                        $"Following is the detailed exception message {ioe}";
                    logger.Log(message, true);
                }
            }
        }
    }
}
