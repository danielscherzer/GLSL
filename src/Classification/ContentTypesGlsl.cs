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

		//[Export]
		//[Name(FragmentShader)]
		//[BaseDefinition(GlslShader)]
		//internal static readonly ContentTypeDefinition glslFragmentContentType;

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
#pragma warning restore 649 //never used warning
	}
}
