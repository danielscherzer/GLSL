using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace DMS.GLSL.Classification
{
	internal class ContentTypesGlsl
	{
		public const string GlslShader = "glslShader";
		public const string FragmentShader = "glslFragmentShader";
		public const string VertexShader = "glslVertexShader";
		public const string GeometryShader = "glslGeometryShader";
		public const string TessControlShader = "glslTessControlShader";
		public const string TessEvaluationShader = "glslTessEvaluationShader";
		public const string ComputeShader = "glslComputeShader";

#pragma warning disable 649 //never used warning
		[Export]
		[Name(GlslShader)]
		[BaseDefinition("code")]
		internal static readonly ContentTypeDefinition glslContentType;

		[Export]
		[Name(FragmentShader)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslFragmentContentType;

		[Export]
		[Name(VertexShader)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslVertexContentType;

		[Export]
		[Name(GeometryShader)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslGeometryContentType;

		[Export]
		[Name(ComputeShader)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslComputeContentType;

		[Export]
		[Name(TessControlShader)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslTessControlContentType;

		[Export]
		[Name(TessEvaluationShader)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslTessEvaluationContentType;

		//[Export]
		//[FileExtension(".glsl")]
		//[ContentType(FragmentShader)]
		//internal static readonly FileExtensionToContentTypeDefinition glslFileType;

		//[Export]
		//[FileExtension(".frag")]
		//[ContentType(FragmentShader)]
		//internal static readonly FileExtensionToContentTypeDefinition fragFileType;

		//[Export]
		//[FileExtension(".vert")]
		//[ContentType(VertexShader)]
		//internal static readonly FileExtensionToContentTypeDefinition vertFileType;

		//[Export]
		//[FileExtension(".geom")]
		//[ContentType(GeometryShader)]
		//internal static readonly FileExtensionToContentTypeDefinition geometryFileType;

		//[Export]
		//[FileExtension(".comp")]
		//[ContentType(ComputeShader)]
		//internal static readonly FileExtensionToContentTypeDefinition computeFileType;

		//[Export]
		//[FileExtension(".tesc")]
		//[ContentType(TessControlShader)]
		//internal static readonly FileExtensionToContentTypeDefinition tessControlFileType;

		//[Export]
		//[FileExtension(".tese")]
		//[ContentType(TessEvaluationShader)]
		//internal static readonly FileExtensionToContentTypeDefinition tessEvaluationFileType;

#pragma warning restore 649 //never used warning
	}
}
