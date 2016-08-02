using SharpDX;
using System.Runtime.InteropServices;

namespace MySharpDXGame
{
	[StructLayoutAttribute(LayoutKind.Sequential)]
	public struct VertexPositionColor
	{
		public readonly Vector3 Position;
		public readonly Color4 Color;

		public VertexPositionColor(Vector3 position, Color4 color)
		{
			Position = position;
			Color = color;
		}
	}
}