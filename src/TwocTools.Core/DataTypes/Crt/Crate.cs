using System.Numerics;

namespace TwocTools.Core.DataTypes.Crt;

public record struct Crate(
	int Index,
	int GroupIndex,
	Vector3 WorldPosition,
	float A,
	short LocalPositionX,
	short LocalPositionY,
	short LocalPositionZ,
	CrateType CrateTypeA,
	CrateType CrateTypeB,
	CrateType CrateTypeC,
	CrateType CrateTypeD,
	short NeighborIndexPosY,
	short NeighborIndexNegY,
	short NeighborIndexPosZ,
	short NeighborIndexNegZ,
	short NeighborIndexPosX,
	short NeighborIndexNegX,
	short ExclamationCrateIndex)
{
	public int Index { get; } = Index;

	public int GroupIndex { get; } = GroupIndex;

	public Vector3 WorldPosition { get; } = WorldPosition;

	public float A { get; } = A;

	public short LocalPositionX { get; } = LocalPositionX;

	public short LocalPositionY { get; } = LocalPositionY;

	public short LocalPositionZ { get; } = LocalPositionZ;

	/// <summary>
	/// The default crate type.
	/// </summary>
	public CrateType CrateTypeA { get; } = CrateTypeA;

	/// <summary>
	/// The second crate type. This seems to be used for time trial.
	/// </summary>
	/// <remarks>This value is only present when crate group version is 3 or higher.</remarks>
	public CrateType CrateTypeB { get; } = CrateTypeB;

	/// <summary>
	/// This third crate type. This seems to be used for crates of type <see cref="CrateType.Slot"/> and <see cref="CrateType.Empty"/>.
	/// </summary>
	/// <remarks>This value is only present when crate group version is 3 or higher.</remarks>
	public CrateType CrateTypeC { get; } = CrateTypeC;

	/// <summary>
	/// This third crate type. This seems to be used for crates of type <see cref="CrateType.Slot"/>.
	/// </summary>
	/// <remarks>This value is only present when crate group version is 3 or higher.</remarks>
	public CrateType CrateTypeD { get; } = CrateTypeD;

	// Some data in the game doesn't appear to be correct, so these values might be unused?
	public short NeighborIndexPosY { get; } = NeighborIndexPosY;

	public short NeighborIndexNegY { get; } = NeighborIndexNegY;

	public short NeighborIndexPosZ { get; } = NeighborIndexPosZ;

	public short NeighborIndexNegZ { get; } = NeighborIndexNegZ;

	public short NeighborIndexPosX { get; } = NeighborIndexPosX;

	public short NeighborIndexNegX { get; } = NeighborIndexNegX;

	/// <summary>
	/// This value is only present when crate group version is 3 or higher.
	/// It has something to do with crates of type <see cref="CrateType.Empty"/>.
	/// </summary>
	public short ExclamationCrateIndex { get; } = ExclamationCrateIndex;
}
