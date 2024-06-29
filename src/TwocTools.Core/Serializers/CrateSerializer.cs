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
				short rotationX = reader.ReadInt16();
				short rotationZ = reader.ReadInt16();
				short rotationY = reader.ReadInt16();
				CrateType crateType = (CrateType)reader.ReadByte();
				CrateType crateTypeTimeTrial = (CrateType)0xFF;
				sbyte d = -1;
				sbyte e = -1;
				if (version >= 3)
				{
					crateTypeTimeTrial = (CrateType)reader.ReadByte();
					d = reader.ReadSByte();
					e = reader.ReadSByte();
				}

				short f = reader.ReadInt16();
				short g = reader.ReadInt16();
				short h = reader.ReadInt16();
				short i = reader.ReadInt16();
				short j = reader.ReadInt16();
				short k = reader.ReadInt16();
				short l = -1;
				if (version >= 3)
				{
					l = reader.ReadInt16();
				}

				Crate crate = new(cratePosition, a, rotationX, rotationZ, rotationY, crateType, crateTypeTimeTrial, d, e, f, g, h, i, j, k, l);
				crates.Add(crate);
			}

			CrateGroup crateGroup = new(position, crateOffset, crateCount, tilt, crates);
			crateGroups.Add(crateGroup);
		}

		return new CrateGroupCollection(version, crateGroups);
	}
}
