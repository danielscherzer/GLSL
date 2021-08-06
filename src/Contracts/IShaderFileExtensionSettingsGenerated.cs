

namespace DMS.GLSL.Contracts
{
	public interface IShaderFileExtensions
	{
		string AutoDetectShaderFileExtensions { get; }

		string FragmentShaderFileExtensions { get; }

		string VertexShaderFileExtensions { get; }

		string GeometryShaderFileExtensions { get; }

		string ComputeShaderFileExtensions { get; }

		string TessellationControlShaderFileExtensions { get; }

		string TessellationEvaluationShaderFileExtensions { get; }

		string MeshShaderFileExtensions { get; }

		string TaskShaderFileExtensions { get; }

		string RayGenerationShaderFileExtensions { get; }

		string RayIntersectionShaderFileExtensions { get; }

		string RayMissShaderFileExtensions { get; }

		string RayAnyHitShaderFileExtensions { get; }

		string RayClosestHitShaderFileExtensions { get; }

		string RayCallableShaderFileExtensions { get; }
	}
}
