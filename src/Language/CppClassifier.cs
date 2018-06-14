using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

namespace DMS.GLSL
{
	[Export]
	[PartCreationPolicy(CreationPolicy.Shared)] //default singleton behavior
	class CppClassifier : IPartImportsSatisfiedNotification, IClassifierProvider
	{
		public void OnImportsSatisfied()
		{
			//search for a c/c++ code classifier in visual studio to use for cpp style code classification
			//find providers with content types
			var providerContentTypes = from element in classifierProvidersMetaData
						   from meta in element.Metadata
						   where meta.Key == "ContentTypes"
					   select new { element = element.Value, contentTypes = meta.Value };
			//split into multiple queries because not all meta.values are string[] before where meta.Key == "ContentTypes" => argumentnullexcpetion
			//providers with cpp content type
			var providerCpp = from provider in providerContentTypes
							  from content in provider.contentTypes as string[]
							  where content.ToLower() == "c/c++"
							  select provider.element;
			//only the last query uses Lazy.value which means instances are created for as few as possible
			var singleProviderCpp = from provider in providerCpp
									where provider.GetType().FullName == "Microsoft.VisualC.CppClassifierProvider"
									select provider;
			cppClassifierProvider = singleProviderCpp.FirstOrDefault();
		}

		public IClassifier GetClassifier(ITextBuffer textBuffer)
		{
			if(cppClassifierProvider is null)
			{
				MessageBox.Show("Could not find the Microsoft.VisualC.CppClassifierProvider. Installing the C++ Visual Studio aspect should help.");
			}
			return cppClassifierProvider?.GetClassifier(textBuffer);
		}

		private IClassifierProvider cppClassifierProvider;

		[ImportMany(typeof(IClassifierProvider))]
		private IEnumerable<Lazy<IClassifierProvider, Dictionary<string, object>>> classifierProvidersMetaData = null;
	}
}
