using DMS.GLSL.Contracts;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DMS.GLSL.Options
{
	public partial class OptionPage : DialogPage, ICompilerSettings, IUserKeywords
	{
		private string _userKeyWords1;
		private string _userKeyWords2;

		[Category("General")]
		[DisplayName("Arguments for the external compiler executable")]
		[Description("Command line arguments for the external compiler executable. Can contain environment variables, like %USERPROFILE% and also the Visual Studio variable $(SolutionDir).")]
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
		[DisplayName("User key words 1")]
		[Description("Space separated list of user key words (used for coloring)")]
		public string UserKeyWords1
		{
			get => _userKeyWords1;
			set
			{
				_userKeyWords1 = value;
				UserKeywordArray1 = ParseWords(value);
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserKeywordArray1)));
			}
		}

		public IEnumerable<string> UserKeywordArray1 { get; private set; } = Enumerable.Empty<string>();

		[Category("General")]
		[DisplayName("User key words 2")]
		[Description("Space separated list of user key words (used for coloring)")]
		public string UserKeyWords2
		{
			get => _userKeyWords2;
			set
			{
				_userKeyWords2 = value;
				UserKeywordArray2 = ParseWords(value);
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserKeywordArray2)));
			}
		}

		public IEnumerable<string> UserKeywordArray2 { get; private set; } = Enumerable.Empty<string>();

		[Category("General")]
		[DisplayName("Compile delay(ms)")]
		[Description("Minimal delay between two compiles.")]
		public int CompileDelay { get; set; } = 200;

		[Category("General")]
		[DisplayName("Print compilation result")]
		[Description("Print shader code compilation result to output window pane")]
		public bool PrintCompilationResult { get; set; } = true;

		public event PropertyChangedEventHandler PropertyChanged;

		private static string[] ParseWords(string words)
		{
			char[] blanks = { ' ' };
			return words.Split(blanks, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}
