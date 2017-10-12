using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace DMS.GLSL.Classification
{
	class ContentTypeGlsl
	{
#pragma warning disable 169 //never used warning
		[Export]
		[Name("glslShader")]
		[BaseDefinition("code")]
		private static ContentTypeDefinition glslContentType;

		[Export]
		[Name("glslFragmentShader")]
		[BaseDefinition("glslShader")]
		private static ContentTypeDefinition glslFragmentContentType;

		[Export]
		[Name("glslVertexShader")]
		[BaseDefinition("glslShader")]
		private static ContentTypeDefinition glslVertexContentType;

		[Export]
		[Name("glslGeometryShader")]
		[BaseDefinition("glslShader")]
		private static ContentTypeDefinition glslGeometryContentType;

		[Export]
		[Name("glslComputeShader")]
		[BaseDefinition("glslShader")]
		private static ContentTypeDefinition glslComputeContentType;

		[Export]
		[Name("glslTessControlShader")]
		[BaseDefinition("glslShader")]
		private static ContentTypeDefinition glslTessControlContentType;

		[Export]
		[Name("glslTessEvaluationShader")]
		[BaseDefinition("glslShader")]
		private static ContentTypeDefinition glslTessEvaluationContentType;

		[Export]
		[FileExtension(".glsl")]
		[ContentType("glslFragmentShader")]
		private static FileExtensionToContentTypeDefinition glslFileType;

		[Export]
		[FileExtension(".frag")]
		[ContentType("glslFragmentShader")]
		private static FileExtensionToContentTypeDefinition fragFileType;

		[Export]
		[FileExtension(".vert")]
		[ContentType("glslVertexShader")]
		private static FileExtensionToContentTypeDefinition vertFileType;

		[Export]
		[FileExtension(".geom")]
		[ContentType("glslGeometryShader")]
		private static FileExtensionToContentTypeDefinition geometryFileType;

		[Export]
		[FileExtension(".comp")]
		[ContentType("glslComputeShader")]
		private static FileExtensionToContentTypeDefinition computeFileType;

		[Export]
		[FileExtension(".tesc")]
		[ContentType("glslTessControlShader")]
		private static FileExtensionToContentTypeDefinition tessControlFileType;

		[Export]
		[FileExtension(".tese")]
		[ContentType("glslTessEvaluationShader")]
		private static FileExtensionToContentTypeDefinition tessEvaluationFileType;

#pragma warning restore 169 //never used warning
	}
}
