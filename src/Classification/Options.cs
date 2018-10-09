namespace DMS.GLSL.Classification
{
	using Microsoft.VisualStudio.Shell;
	using System.ComponentModel;

	public class Options : DialogPage
	{
		[Category("General")]
		[DisplayName("Live Compiling")]
		[Description("Compile the shader code in the background and show resulting errors")]
		public bool LiveCompiling { get; set; } = true;

		[Category("Files")]
		[DisplayName("Auto Detect Shader Type File Extensions")]
		[Description("File extensions that will receive syntax coloring")]
		public string AutoDetectShaderTypeFileExtensions { get; set; } = ".glsl";

		[Category("Files")]
		[DisplayName("Fragment Shader File Extensions")]
		[Description("File extensions that will receive syntax coloring")]
		public string FragmentShaderFileExtensions { get; set; } = ".frag";

		[Category("Files")]
		[DisplayName("Vertex Shader File Extensions")]
		[Description("File extensions that will receive syntax coloring")]
		public string VertexShaderFileExtensions { get; set; } = ".vert";

		[Category("Files")]
		[DisplayName("Geometry Shader File Extensions")]
		[Description("File extensions that will receive syntax coloring")]
		public string GeometryShaderFileExtensions { get; set; } = ".geom";

		[Category("Files")]
		[DisplayName("Compute Shader File Extensions")]
		[Description("File extensions that will receive syntax coloring")]
		public string ComputeShaderFileExtensions { get; set; } = ".comp";

		[Category("Files")]
		[DisplayName("Tessellation Control Shader File Extensions")]
		[Description("File extensions that will receive syntax coloring")]
		public string TessControlShaderFileExtensions { get; set; } = ".tesc";

		[Category("Files")]
		[DisplayName("Tessellation Evaluation Shader File Extensions")]
		[Description("File extensions that will receive syntax coloring")]
		public string TessEvaluationShaderFileExtensions { get; set; } = ".tese";
	}
}
