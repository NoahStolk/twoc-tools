using System.Numerics;
using TwocTools.Core.DataTypes.Crt;
using TwocTools.Core.Internals;
using TwocTools.Core.Internals.Extensions;

namespace TwocTools.Core.Serializers;

public static class CrateSerializer
{
	public static CrateGroupCollection Deserialize(Stream input, Endianness endianness)
	{
		input.Position = 0;

		using BinaryReader reader = endianness switch
		{
			Endianness.Big => new BigEndianBinaryReader(input),
			_ => new BinaryReader(input),
		};
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
				Vector3 worldPosition = reader.ReadVector3();
				float a = reader.ReadSingle();
				short localPositionX = reader.ReadInt16();
				short localPositionY = reader.ReadInt16();
				short localPositionZ = reader.ReadInt16();
				CrateType crateTypeA = (CrateType)reader.ReadSByte();
				CrateType crateTypeB = CrateType.None;
				CrateType crateTypeC = CrateType.None;
				CrateType crateTypeD = CrateType.None;
				if (version >= 3)
				{
					crateTypeB = (CrateType)reader.ReadSByte();
					crateTypeC = (CrateType)reader.ReadSByte();
					crateTypeD = (CrateType)reader.ReadSByte();
				}

				short neighborIndexPosY = reader.ReadInt16();
				short neighborIndexNegY = reader.ReadInt16();
				short neighborIndexPosZ = reader.ReadInt16();
				short neighborIndexNegZ = reader.ReadInt16();
				short neighborIndexPosX = reader.ReadInt16();
				short neighborIndexNegX = reader.ReadInt16();
				short exclamationCrateIndex = -1;
				if (version >= 3)
				{
					exclamationCrateIndex = reader.ReadInt16();
				}

				crates.Add(new Crate(
					Index: crateOffset + crateIndex,
					GroupIndex: crateGroupIndex,
					WorldPosition: worldPosition,
					A: a,
					LocalPositionX: localPositionX,
					LocalPositionY: localPositionY,
					LocalPositionZ: localPositionZ,
					CrateTypeA: crateTypeA,
					CrateTypeB: crateTypeB,
					CrateTypeC: crateTypeC,
					CrateTypeD: crateTypeD,
					NeighborIndexPosY: neighborIndexPosY,
					NeighborIndexNegY: neighborIndexNegY,
					NeighborIndexPosZ: neighborIndexPosZ,
					NeighborIndexNegZ: neighborIndexNegZ,
					NeighborIndexPosX: neighborIndexPosX,
					NeighborIndexNegX: neighborIndexNegX,
					ExclamationCrateIndex: exclamationCrateIndex));
			}

			CrateGroup crateGroup = new(position, crateOffset, crateCount, tilt, crates);
			crateGroups.Add(crateGroup);
		}

		return new CrateGroupCollection(version, crateGroups);
	}
}
