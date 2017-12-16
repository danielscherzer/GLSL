using OpenTK;
using System.ComponentModel.Composition;

namespace ContextGL
{
	[Export]
	[PartCreationPolicy(CreationPolicy.Shared)] //default singleton behavior
	public class SharedContextGL
	{
		public SharedContextGL()
		{
			window = new GameWindow(1, 1);
		}

		public void MakeCurrent()
		{
			window.MakeCurrent();
		}

		private GameWindow window;
	}
}
