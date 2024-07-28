using System.Numerics;
using TwocTools.Core.DataTypes;
using TwocTools.Core.DataTypes.Wmp;
using TwocTools.Core.Internals;
using TwocTools.Core.Internals.Extensions;

namespace TwocTools.Core.Serializers;

public static class WumpaSerializer
{
	public static WumpaCollection Deserialize(Stream input, Endianness endianness)
	{
		input.Position = 0;

		using BinaryReader reader = endianness switch
		{
			Endianness.Big => new BigEndianBinaryReader(input),
			_ => new BinaryReader(input),
		};
		int wumpaCount = reader.ReadInt32();

		List<Wumpa> wumpas = new(wumpaCount);
		for (int i = 0; i < wumpaCount; i++)
		{
			Vector3 position = reader.ReadVector3();

			Wumpa wumpa = new(position);
			wumpas.Add(wumpa);
		}

		return new WumpaCollection(wumpas);
	}
}
