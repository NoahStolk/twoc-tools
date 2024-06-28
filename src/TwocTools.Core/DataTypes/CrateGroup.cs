using System.Numerics;

namespace TwocTools.Core.DataTypes;

public record struct CrateGroup
{
	public CrateGroup(Vector3 position, ushort crateOffset, ushort crateCount, ushort tilt, List<Crate> crates)
	{
		Position = position;
		CrateOffset = crateOffset;
		CrateCount = crateCount;
		Tilt = tilt;
		Crates = crates;
	}

	public Vector3 Position { get; }

	public ushort CrateOffset { get; }

	public ushort CrateCount { get; }

	public ushort Tilt { get; }

	public List<Crate> Crates { get; }
}
