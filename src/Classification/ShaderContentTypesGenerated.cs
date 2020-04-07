
namespace DMS.GLSL.Classification
{
	using Microsoft.VisualStudio.Utilities;
	using System.ComponentModel.Composition;
	internal class ShaderContentTypesGenerated
	{
		public const string GlslShader = "glslShader";

		public static string DefaultFileExtension(string shaderType)
		{
			switch(shaderType)
			{
				case AutoDetect: return ".glsl";
				case Fragment: return ".frag";
				case Vertex: return ".vert";
				case Geometry: return ".geom";
				case Compute: return ".comp";
				case TessellationControl: return ".tesc";
				case TessellationEvaluation: return ".tese";
				case Mesh: return ".mesh";
				case Task: return ".task";
				case RayGeneration: return ".rgen";
				case RayIntersection: return ".rint";
				case RayMiss: return ".rmiss";
				case RayAnyHit: return ".rahit";
				case RayClosestHit: return ".rchit";
				case RayCallable: return ".rcall";
			}
			return ".frag";
		}

#pragma warning disable 649 //never used warning
		[Export]
		[Name(GlslShader)]
		[BaseDefinition("code")]
		internal static readonly ContentTypeDefinition glslContentType;

		
		public const string AutoDetect = "glslAutoDetect";
		[Export]
		[Name(AutoDetect)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslAutoDetect;

		public const string Fragment = "glslFragment";
		[Export]
		[Name(Fragment)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslFragment;

		public const string Vertex = "glslVertex";
		[Export]
		[Name(Vertex)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslVertex;

		public const string Geometry = "glslGeometry";
		[Export]
		[Name(Geometry)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslGeometry;

		public const string Compute = "glslCompute";
		[Export]
		[Name(Compute)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslCompute;

		public const string TessellationControl = "glslTessellationControl";
		[Export]
		[Name(TessellationControl)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslTessellationControl;

		public const string TessellationEvaluation = "glslTessellationEvaluation";
		[Export]
		[Name(TessellationEvaluation)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslTessellationEvaluation;

		public const string Mesh = "glslMesh";
		[Export]
		[Name(Mesh)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslMesh;

		public const string Task = "glslTask";
		[Export]
		[Name(Task)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslTask;

		public const string RayGeneration = "glslRayGeneration";
		[Export]
		[Name(RayGeneration)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslRayGeneration;

		public const string RayIntersection = "glslRayIntersection";
		[Export]
		[Name(RayIntersection)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslRayIntersection;

		public const string RayMiss = "glslRayMiss";
		[Export]
		[Name(RayMiss)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslRayMiss;

		public const string RayAnyHit = "glslRayAnyHit";
		[Export]
		[Name(RayAnyHit)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslRayAnyHit;

		public const string RayClosestHit = "glslRayClosestHit";
		[Export]
		[Name(RayClosestHit)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslRayClosestHit;

		public const string RayCallable = "glslRayCallable";
		[Export]
		[Name(RayCallable)]
		[BaseDefinition(GlslShader)]
		internal static readonly ContentTypeDefinition glslRayCallable;
#pragma warning restore 649 //never used warning
	}
}
