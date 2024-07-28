using System.Numerics;

namespace TwocTools.Core.DataTypes.Crt;

public record struct Crate
{
	public Crate(
		int index,
		int groupIndex,
		Vector3 worldPosition,
		float a,
		short localPositionX,
		short localPositionY,
		short localPositionZ,
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
		short exclamationCrateIndex)
	{
		Index = index;
		GroupIndex = groupIndex;
		WorldPosition = worldPosition;
		A = a;
		LocalPositionX = localPositionX;
		LocalPositionY = localPositionY;
		LocalPositionZ = localPositionZ;
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
		ExclamationCrateIndex = exclamationCrateIndex;
	}

	public int Index { get; }

	public int GroupIndex { get; }

	public Vector3 WorldPosition { get; }

	public float A { get; }

	public short LocalPositionX { get; }

	public short LocalPositionY { get; }

	public short LocalPositionZ { get; }

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

	// TODO: Figure out which is which exactly. Some data in the game doesn't appear to be correct, so these values might just be unused.
	// Y
	public short F { get; }

	public short G { get; }

	// Z
	public short H { get; }

	public short I { get; }

	// X
	public short J { get; }

	public short K { get; }

	/// <summary>
	/// This value is only present when crate group version is 3 or higher.
	/// It has something to do with crates of type <see cref="CrateType.Empty"/>.
	/// </summary>
	public short ExclamationCrateIndex { get; }
}
