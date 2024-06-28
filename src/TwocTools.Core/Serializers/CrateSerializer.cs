using System.Numerics;
using TwocTools.Core.DataTypes;
using TwocTools.Core.Extensions;

namespace TwocTools.Core.Serializers;

public static class CrateSerializer
{
	public static CrateGroupCollection Deserialize(BinaryReader reader)
	{
		uint version = reader.ReadUInt32(); // Default version is 4?
		ushort crateGroupCount = reader.ReadUInt16();

		List<CrateGroup> crateGroups = new(crateGroupCount);
		for (int crateGroupIndex = 0; crateGroupIndex < crateGroupCount; crateGroupIndex++)
		{
			Vector3 position = reader.ReadVector3();
			ushort crateOffset = reader.ReadUInt16();
			ushort crateCount = reader.ReadUInt16();
			ushort tilt = reader.ReadUInt16();

			List<Crate> crates = new(crateCount);
			for (int crateIndex = 0; crateIndex < crateCount; crateIndex++)
			{
				Vector3 cratePosition = reader.ReadVector3();
				float a = reader.ReadSingle();
				ushort rotationX = reader.ReadUInt16();
				ushort rotationZ = reader.ReadUInt16();
				ushort rotationY = reader.ReadUInt16();
				byte b = reader.ReadByte();
				byte c = 0xFF;
				byte d = 0xFF;
				byte e = 0xFF;
				if (version >= 3)
				{
					c = reader.ReadByte();
					d = reader.ReadByte();
					e = reader.ReadByte();
				}

				ushort f = reader.ReadUInt16();
				ushort g = reader.ReadUInt16();
				ushort h = reader.ReadUInt16();
				ushort i = reader.ReadUInt16();
				ushort j = reader.ReadUInt16();
				ushort k = reader.ReadUInt16();
				ushort l = 0xFFFF;
				if (version >= 3)
				{
					l = reader.ReadUInt16();
				}

				Crate crate = new(cratePosition, a, rotationX, rotationZ, rotationY, b, c, d, e, f, g, h, i, j, k, l);
				crates.Add(crate);
			}

			CrateGroup crateGroup = new(position, crateOffset, crateCount, tilt, crates);
			crateGroups.Add(crateGroup);
		}

		return new CrateGroupCollection(version, crateGroups);
	}
}
