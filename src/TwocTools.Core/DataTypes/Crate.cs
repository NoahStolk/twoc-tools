using System.Numerics;

namespace TwocTools.Core.DataTypes;

public record struct Crate
{
	public Crate(
		Vector3 position,
		float a,
		ushort rotationX,
		ushort rotationZ,
		ushort rotationY,
		byte b,
		byte c,
		byte d,
		byte e,
		ushort f,
		ushort g,
		ushort h,
		ushort i,
		ushort j,
		ushort k,
		ushort l)
	{
		Position = position;
		A = a;
		RotationX = rotationX;
		RotationZ = rotationZ;
		RotationY = rotationY;
		B = b;
		C = c;
		D = d;
		E = e;
		F = f;
		G = g;
		H = h;
		I = i;
		J = j;
		K = k;
		L = l;
	}

	public Vector3 Position { get; }

	public float A { get; }

	public ushort RotationX { get; }

	public ushort RotationZ { get; }

	public ushort RotationY { get; }

	// B, C, D, E = crate types.
	// C, D, E are only present when crate group version is 3 or higher.
	public byte B { get; }

	public byte C { get; }

	public byte D { get; }

	public byte E { get; }

	public ushort F { get; }

	public ushort G { get; }

	public ushort H { get; }

	public ushort I { get; }

	public ushort J { get; }

	public ushort K { get; }

	// L is only present when crate group version is 3 or higher.
	public ushort L { get; }
}
