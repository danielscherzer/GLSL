using OpenTK;
using OpenTK.Graphics;
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

		public IGraphicsContext Context => window.Context;

		private GameWindow window;
	}
}
