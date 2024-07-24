using TwocTools.App.State;
using TwocTools.Core;

namespace TwocTools.App.Extensions;

public static class GameVersionExtensions
{
	public static Endianness GetEndianness(this GameVersion version)
	{
		return version switch
		{
			GameVersion.Ps2Original => Endianness.Little,
			GameVersion.Ps2GreatestHits => Endianness.Little,
			GameVersion.Ps2E3Demo => Endianness.Little,
			GameVersion.Xbox => Endianness.Big,
			GameVersion.GameCube => Endianness.Big,
			_ => throw new ArgumentOutOfRangeException(nameof(version), version, null),
		};
	}
}
