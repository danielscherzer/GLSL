
namespace DMS.GLSL.Options
{
	using DMS.GLSL.Classification;
	using Microsoft.VisualStudio.Utilities;
	using System;
	using System.ComponentModel.Composition;

	public sealed partial class FileExtensionsOption
	{
		[ImportingConstructor]
		public FileExtensionsOption([Import] IContentTypeRegistryService contentTypeRegistry, [Import] IFileExtensionRegistryService fileExtensionRegistry)
		{
			var options = OptionsPagePackage.Options;
			RegisterFileExtensions(fileExtensionRegistry, options.AutoDetectShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypes.AutoDetect));
			RegisterFileExtensions(fileExtensionRegistry, options.FragmentShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypes.Fragment));
			RegisterFileExtensions(fileExtensionRegistry, options.VertexShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypes.Vertex));
			RegisterFileExtensions(fileExtensionRegistry, options.GeometryShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypes.Geometry));
			RegisterFileExtensions(fileExtensionRegistry, options.ComputeShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypes.Compute));
			RegisterFileExtensions(fileExtensionRegistry, options.TessellationControlShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypes.TessellationControl));
			RegisterFileExtensions(fileExtensionRegistry, options.TessellationEvaluationShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypes.TessellationEvaluation));
			RegisterFileExtensions(fileExtensionRegistry, options.MeshShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypes.Mesh));
			RegisterFileExtensions(fileExtensionRegistry, options.TaskShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypes.Task));
			RegisterFileExtensions(fileExtensionRegistry, options.RayGenerationShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypes.RayGeneration));
			RegisterFileExtensions(fileExtensionRegistry, options.RayIntersectionShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypes.RayIntersection));
			RegisterFileExtensions(fileExtensionRegistry, options.RayMissShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypes.RayMiss));
			RegisterFileExtensions(fileExtensionRegistry, options.RayAnyHitShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypes.RayAnyHit));
			RegisterFileExtensions(fileExtensionRegistry, options.RayClosestHitShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypes.RayClosestHit));
			RegisterFileExtensions(fileExtensionRegistry, options.RayCallableShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypes.RayCallable));
		}
	}
}
