namespace DMS.GLSL.Classification
{
	using System;
	using System.Runtime.InteropServices;
	using Microsoft.VisualStudio;
	using Microsoft.VisualStudio.Shell;
	using Microsoft.VisualStudio.Shell.Interop;

	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the
	/// IVsPackage interface and uses the registration attributes defined in the framework to
	/// register itself and its components with the shell. These attributes tell the pkgdef creation
	/// utility what data to put into .pkgdef file.
	/// </para>
	/// <para>
	/// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
	/// </para>
	/// </remarks>
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
	[Guid(PackageGuidString)]
	[ProvideOptionPage(typeof(Options), "GLSL language integration", "Configuration", 0, 0, true)]
	public sealed class OptionsPagePackage : AsyncPackage
	{
		public const string PackageGuidString = "fd8ee466-e18c-45fc-b1a1-ca0dc1ec67fb";

		public static Options Options
		{
			get
			{
				if (_options is null)
				{
					EnsurePackageLoaded();
				}
				return _options;
			}
		}

		private static Options _options;
		private static readonly object _syncRoot = new object();

		private static void EnsurePackageLoaded()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			lock (_syncRoot)
			{
				var shell = (IVsShell)GetGlobalService(typeof(SVsShell));
				var guid = new Guid(PackageGuidString);
				if (shell.IsPackageLoaded(ref guid, out IVsPackage package) != VSConstants.S_OK)
				{
					ErrorHandler.Succeeded(shell.LoadPackage(ref guid, out package));
					var myPack = package as OptionsPagePackage;
					_options = (Options)myPack.GetDialogPage(typeof(Options));
				}
			}
		}
	}
}
