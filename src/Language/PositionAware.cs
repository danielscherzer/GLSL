using Sprache;
using System.Linq;

namespace DMS.GLSL.Language
{
	public class PositionAware<T> : IPositionAware<PositionAware<T>>
	{
		public PositionAware(T value)
		{
			Value = value;
		}

		public T Value { get; }
		public Position Start { get; private set; }
		public int Length { get; private set; }
		public PositionAware<T> SetPos(Position startPos, int length)
		{
			Start = startPos;
			Length = length;
			return this;
		}

	}

	public static class ParserExtensions
	{
		public static Parser<PositionAware<T>> WithPosition<T>(this Parser<T> value)
		{
			return value.Select(x => new PositionAware<T>(x)).Positioned();
		}
	}
}
