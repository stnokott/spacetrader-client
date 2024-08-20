using Godot;

namespace MathExtensionMethods
{
	public static class VectorExtensions
	{
		public static Vector2I AsInt(this Vector2 v)
		{
			return new Vector2I((int)v.X, (int)v.Y);
		}
	}

	public static class RectExtensions
	{
		public static Rect2I AsInt(this Rect2 r)
		{
			return new Rect2I(r.Position.AsInt(), r.Size.AsInt());
		}
	}
}
