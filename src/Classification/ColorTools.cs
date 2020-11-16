using System.Windows.Media;

namespace DMS.GLSL.Classification
{
	public static class ColorTools
	{
		/// <summary>
		/// Convert a color string into a <seealso cref="Color"/>.
		/// Converts named colors like 'white', 'black, 'red'.
		/// or hex strings like '#FFF', '#FFFF', '#FFFFFF' or with alpha '#FFFFFFFF'
		/// </summary>
		/// <param name="hexColor"></param>
		/// <returns></returns>
		public static Color FromHexCode(string hexColor)
		{
			var sysColor = (System.Drawing.Color)converter.ConvertFromString(hexColor);
			return Color.FromArgb(sysColor.A, sysColor.R, sysColor.G, sysColor.B);
		}

		private static readonly System.Drawing.ColorConverter converter = new System.Drawing.ColorConverter();
	}
}
