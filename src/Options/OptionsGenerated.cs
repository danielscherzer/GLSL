
namespace DMS.GLSL.Options
{
	using Microsoft.VisualStudio.Shell;
	using System.ComponentModel;

	public partial class Options : DialogPage
	{
		[Category("File extensions")]
		[DisplayName("Auto detect shader type file extensions")]
		[Description("Space or semicolon separated list of extensions that will receive syntax coloring")]
		public string AutoDetectShaderFileExtensions { get; set; } = ".glsl";
		
		[Category("File extensions")]
		[DisplayName("Fragment shader type file extensions")]
		[Description("Space or semicolon separated list of extensions that will receive syntax coloring")]
		public string FragmentShaderFileExtensions { get; set; } = ".frag";
		
		[Category("File extensions")]
		[DisplayName("Vertex shader type file extensions")]
		[Description("Space or semicolon separated list of extensions that will receive syntax coloring")]
		public string VertexShaderFileExtensions { get; set; } = ".vert";
		
		[Category("File extensions")]
		[DisplayName("Geometry shader type file extensions")]
		[Description("Space or semicolon separated list of extensions that will receive syntax coloring")]
		public string GeometryShaderFileExtensions { get; set; } = ".geom";
		
		[Category("File extensions")]
		[DisplayName("Compute shader type file extensions")]
		[Description("Space or semicolon separated list of extensions that will receive syntax coloring")]
		public string ComputeShaderFileExtensions { get; set; } = ".comp";
		
		[Category("File extensions")]
		[DisplayName("Tessellation control shader type file extensions")]
		[Description("Space or semicolon separated list of extensions that will receive syntax coloring")]
		public string TessellationControlShaderFileExtensions { get; set; } = ".tesc";
		
		[Category("File extensions")]
		[DisplayName("Tessellation evaluation shader type file extensions")]
		[Description("Space or semicolon separated list of extensions that will receive syntax coloring")]
		public string TessellationEvaluationShaderFileExtensions { get; set; } = ".tese";
		
		[Category("File extensions")]
		[DisplayName("Mesh shader type file extensions")]
		[Description("Space or semicolon separated list of extensions that will receive syntax coloring")]
		public string MeshShaderFileExtensions { get; set; } = ".mesh";
		
		[Category("File extensions")]
		[DisplayName("Task shader type file extensions")]
		[Description("Space or semicolon separated list of extensions that will receive syntax coloring")]
		public string TaskShaderFileExtensions { get; set; } = ".task";
		
		[Category("File extensions")]
		[DisplayName("Ray generation shader type file extensions")]
		[Description("Space or semicolon separated list of extensions that will receive syntax coloring")]
		public string RayGenerationShaderFileExtensions { get; set; } = ".rgen";
		
		[Category("File extensions")]
		[DisplayName("Ray intersection shader type file extensions")]
		[Description("Space or semicolon separated list of extensions that will receive syntax coloring")]
		public string RayIntersectionShaderFileExtensions { get; set; } = ".rint";
		
		[Category("File extensions")]
		[DisplayName("Ray miss shader type file extensions")]
		[Description("Space or semicolon separated list of extensions that will receive syntax coloring")]
		public string RayMissShaderFileExtensions { get; set; } = ".rmiss";
		
		[Category("File extensions")]
		[DisplayName("Ray any hit shader type file extensions")]
		[Description("Space or semicolon separated list of extensions that will receive syntax coloring")]
		public string RayAnyHitShaderFileExtensions { get; set; } = ".rahit";
		
		[Category("File extensions")]
		[DisplayName("Ray closest hit shader type file extensions")]
		[Description("Space or semicolon separated list of extensions that will receive syntax coloring")]
		public string RayClosestHitShaderFileExtensions { get; set; } = ".rchit";
		
		[Category("File extensions")]
		[DisplayName("Ray callable shader type file extensions")]
		[Description("Space or semicolon separated list of extensions that will receive syntax coloring")]
		public string RayCallableShaderFileExtensions { get; set; } = ".rcall";
		
	}
}
