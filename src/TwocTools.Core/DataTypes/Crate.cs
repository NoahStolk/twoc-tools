using System.Numerics;

namespace TwocTools.Core.DataTypes;

public record struct Crate
{
	public Crate(
		int index,
		int groupIndex,
		Vector3 position,
		float a,
		short rotationX,
		short rotationZ,
		short rotationY,
		CrateType crateTypeA,
		CrateType crateTypeB,
		CrateType crateTypeC,
		CrateType crateTypeD,
		short f,
		short g,
		short h,
		short i,
		short j,
		short k,
		short l)
	{
		Index = index;
		GroupIndex = groupIndex;
		Position = position;
		A = a;
		RotationX = rotationX;
		RotationZ = rotationZ;
		RotationY = rotationY;
		CrateTypeA = crateTypeA;
		CrateTypeB = crateTypeB;
		CrateTypeC = crateTypeC;
		CrateTypeD = crateTypeD;
		F = f;
		G = g;
		H = h;
		I = i;
		J = j;
		K = k;
		L = l;
	}

	public int Index { get; }

	public int GroupIndex { get; }

	public Vector3 Position { get; }

	public float A { get; }

	public short RotationX { get; }

	public short RotationZ { get; }

	public short RotationY { get; }

	/// <summary>
	/// The default crate type.
	/// </summary>
	public CrateType CrateTypeA { get; }

	/// <summary>
	/// The second crate type. This seems to be used for time trial.
	/// </summary>
	/// <remarks>This value is only present when crate group version is 3 or higher.</remarks>
	public CrateType CrateTypeB { get; }

	/// <summary>
	/// This third crate type. This seems to be used for crates of type <see cref="CrateType.Slot"/> and <see cref="CrateType.Empty"/>.
	/// </summary>
	/// <remarks>This value is only present when crate group version is 3 or higher.</remarks>
	public CrateType CrateTypeC { get; }

	/// <summary>
	/// This third crate type. This seems to be used for crates of type <see cref="CrateType.Slot"/>.
	/// </summary>
	/// <remarks>This value is only present when crate group version is 3 or higher.</remarks>
	public CrateType CrateTypeD { get; }

	public short F { get; }

	public short G { get; }

	public short H { get; }

	public short I { get; }

	public short J { get; }

	public short K { get; }

	/// <summary>
	/// This value is only present when crate group version is 3 or higher.
	/// It has something to do with crates of type <see cref="CrateType.Empty"/>.
	/// </summary>
	public short L { get; }
}
