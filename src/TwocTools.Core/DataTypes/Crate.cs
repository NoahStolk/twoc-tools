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
		CrateType crateType,
		CrateType crateTypeTimeTrial,
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
		CrateType = crateType;
		CrateTypeTimeTrial = crateTypeTimeTrial;
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

	public CrateType CrateType { get; }

	/// <summary>
	/// This value is only present when crate group version is 3 or higher.
	/// </summary>
	public CrateType CrateTypeTimeTrial { get; }

	/// <summary>
	/// This value is only present when crate group version is 3 or higher.
	/// </summary>
	public byte D { get; }

	/// <summary>
	/// This value is only present when crate group version is 3 or higher.
	/// </summary>
	public byte E { get; }

	public ushort F { get; }

	public ushort G { get; }

	public ushort H { get; }

	public ushort I { get; }

	public ushort J { get; }

	public ushort K { get; }

	/// <summary>
	/// This value is only present when crate group version is 3 or higher.
	/// </summary>
	public ushort L { get; }
}
