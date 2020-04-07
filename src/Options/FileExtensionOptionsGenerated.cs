
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
			RegisterFileExtensions(fileExtensionRegistry, options.AutoDetectShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypesGenerated.AutoDetect));
			RegisterFileExtensions(fileExtensionRegistry, options.FragmentShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypesGenerated.Fragment));
			RegisterFileExtensions(fileExtensionRegistry, options.VertexShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypesGenerated.Vertex));
			RegisterFileExtensions(fileExtensionRegistry, options.GeometryShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypesGenerated.Geometry));
			RegisterFileExtensions(fileExtensionRegistry, options.ComputeShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypesGenerated.Compute));
			RegisterFileExtensions(fileExtensionRegistry, options.TessellationControlShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypesGenerated.TessellationControl));
			RegisterFileExtensions(fileExtensionRegistry, options.TessellationEvaluationShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypesGenerated.TessellationEvaluation));
			RegisterFileExtensions(fileExtensionRegistry, options.MeshShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypesGenerated.Mesh));
			RegisterFileExtensions(fileExtensionRegistry, options.TaskShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypesGenerated.Task));
			RegisterFileExtensions(fileExtensionRegistry, options.RayGenerationShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypesGenerated.RayGeneration));
			RegisterFileExtensions(fileExtensionRegistry, options.RayIntersectionShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypesGenerated.RayIntersection));
			RegisterFileExtensions(fileExtensionRegistry, options.RayMissShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypesGenerated.RayMiss));
			RegisterFileExtensions(fileExtensionRegistry, options.RayAnyHitShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypesGenerated.RayAnyHit));
			RegisterFileExtensions(fileExtensionRegistry, options.RayClosestHitShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypesGenerated.RayClosestHit));
			RegisterFileExtensions(fileExtensionRegistry, options.RayCallableShaderFileExtensions, contentTypeRegistry.GetContentType(ShaderContentTypesGenerated.RayCallable));
		}
	}
}
