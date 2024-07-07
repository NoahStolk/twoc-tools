using System.Numerics;
using TwocTools.Core.DataTypes;
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
				Vector3 cratePosition = reader.ReadVector3();
				float a = reader.ReadSingle();
				short localPositionX = reader.ReadInt16();
				short localPositionY = reader.ReadInt16();
				short localPositionZ = reader.ReadInt16();
				CrateType crateTypeA = (CrateType)reader.ReadByte();
				CrateType crateTypeB = CrateType.Unknown255;
				CrateType crateTypeC = CrateType.Unknown255;
				CrateType crateTypeD = CrateType.Unknown255;
				if (version >= 3)
				{
					crateTypeB = (CrateType)reader.ReadByte();
					crateTypeC = (CrateType)reader.ReadByte();
					crateTypeD = (CrateType)reader.ReadByte();
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

				crates.Add(new Crate(
					index: crateOffset + crateIndex,
					groupIndex: crateGroupIndex,
					worldPosition: cratePosition,
					a: a,
					localPositionX: localPositionX,
					localPositionY: localPositionY,
					localPositionZ: localPositionZ,
					crateTypeA: crateTypeA,
					crateTypeB: crateTypeB,
					crateTypeC: crateTypeC,
					crateTypeD: crateTypeD,
					f: f,
					g: g,
					h: h,
					i: i,
					j: j,
					k: k,
					l: l));
			}

			CrateGroup crateGroup = new(position, crateOffset, crateCount, tilt, crates);
			crateGroups.Add(crateGroup);
		}

		return new CrateGroupCollection(version, crateGroups);
	}
}
