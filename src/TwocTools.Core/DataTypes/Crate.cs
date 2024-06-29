using System.Numerics;

namespace TwocTools.Core.DataTypes;

public record struct Crate
{
	public Crate(
		Vector3 position,
		float a,
		short rotationX,
		short rotationZ,
		short rotationY,
		CrateType crateType,
		CrateType crateTypeTimeTrial,
		sbyte d,
		sbyte e,
		short f,
		short g,
		short h,
		short i,
		short j,
		short k,
		short l)
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

	public short RotationX { get; }

	public short RotationZ { get; }

	public short RotationY { get; }

	public CrateType CrateType { get; }

	/// <summary>
	/// This value is only present when crate group version is 3 or higher.
	/// </summary>
	public CrateType CrateTypeTimeTrial { get; }

	/// <summary>
	/// This value is only present when crate group version is 3 or higher.
	/// </summary>
	public sbyte D { get; }

	/// <summary>
	/// This value is only present when crate group version is 3 or higher.
	/// </summary>
	public sbyte E { get; }

	public short F { get; }

	public short G { get; }

	public short H { get; }

	public short I { get; }

	public short J { get; }

	public short K { get; }

	/// <summary>
	/// This value is only present when crate group version is 3 or higher.
	/// </summary>
	public short L { get; }
}
