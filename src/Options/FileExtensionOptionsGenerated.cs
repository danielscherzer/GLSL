
namespace DMS.GLSL.Options
{
	using DMS.GLSL.Contracts;
	using DMS.GLSL.Errors;
	using Microsoft.VisualStudio.Utilities;
	using System.ComponentModel.Composition;

	internal sealed partial class FileExtensionsOption
	{
		[ImportingConstructor]
		public FileExtensionsOption([Import] IContentTypeRegistryService contentTypeRegistry, [Import] IFileExtensionRegistryService fileExtensionRegistry, [Import] ILogger logger)
		{
			var options = OptionsPagePackage.Options;
			void Register(string sExtensions, string contentType) => RegisterFileExtensions(fileExtensionRegistry, sExtensions, contentTypeRegistry.GetContentType(contentType), logger);
			Register(options.AutoDetectShaderFileExtensions, ShaderContentTypes.AutoDetect);
			Register(options.FragmentShaderFileExtensions, ShaderContentTypes.Fragment);
			Register(options.VertexShaderFileExtensions, ShaderContentTypes.Vertex);
			Register(options.GeometryShaderFileExtensions, ShaderContentTypes.Geometry);
			Register(options.ComputeShaderFileExtensions, ShaderContentTypes.Compute);
			Register(options.TessellationControlShaderFileExtensions, ShaderContentTypes.TessellationControl);
			Register(options.TessellationEvaluationShaderFileExtensions, ShaderContentTypes.TessellationEvaluation);
			Register(options.MeshShaderFileExtensions, ShaderContentTypes.Mesh);
			Register(options.TaskShaderFileExtensions, ShaderContentTypes.Task);
			Register(options.RayGenerationShaderFileExtensions, ShaderContentTypes.RayGeneration);
			Register(options.RayIntersectionShaderFileExtensions, ShaderContentTypes.RayIntersection);
			Register(options.RayMissShaderFileExtensions, ShaderContentTypes.RayMiss);
			Register(options.RayAnyHitShaderFileExtensions, ShaderContentTypes.RayAnyHit);
			Register(options.RayClosestHitShaderFileExtensions, ShaderContentTypes.RayClosestHit);
			Register(options.RayCallableShaderFileExtensions, ShaderContentTypes.RayCallable);
		}
	}
}
