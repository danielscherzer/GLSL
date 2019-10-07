namespace DMS.GLSL.Options
{
	using Microsoft.VisualStudio.Shell;
	using System.ComponentModel;

	public partial class Options : DialogPage
	{
		private string _userKeyWords;

		[Category("General")]
		[DisplayName("Arguments for the external compiler executable")]
		[Description("Command line arguments for the external compiler executable")]
		public string ExternalCompilerArguments { get; set; } = string.Empty;

		[Category("General")]
		[DisplayName("External compiler executable file path (without quotes)")]
		[Description("If non empty this file will be executed for each shader and the output parsed for error squiggles")]
		public string ExternalCompilerExeFilePath { get; set; } = string.Empty;

		[Category("General")]
		[DisplayName("Live compiling")]
		[Description("Compile the shader code in the background and show resulting errors")]
		public bool LiveCompiling { get; set; } = true;

		[Category("General")]
		[DisplayName("User key words")]
		[Description("Space separated list of user key words (used for coloring)")]
		public string UserKeyWords
		{
			get => _userKeyWords;
			set
			{
				_userKeyWords = value;
				GlslSpecification.SetUserKeyWords(value);
			}
		}

		[Category("General")]
		[DisplayName("Compile delay(ms)")]
		[Description("When you change content continuously, only compile the last time")]
		public int CompileDelay { get; set; } = 1000;
	}
}
