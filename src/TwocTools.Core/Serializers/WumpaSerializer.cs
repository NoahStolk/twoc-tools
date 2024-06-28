using System.Numerics;
using TwocTools.Core.DataTypes;
using TwocTools.Core.Extensions;

namespace TwocTools.Core.Serializers;

public static class WumpaSerializer
{
	public static List<Wumpa> Deserialize(BinaryReader reader)
	{
		int wumpaCount = reader.ReadInt32();

		List<Wumpa> wumpas = new(wumpaCount);
		for (int i = 0; i < wumpaCount; i++)
		{
			Vector3 position = reader.ReadVector3();

			Wumpa wumpa = new(position);
			wumpas.Add(wumpa);
		}

		return wumpas;
	}
}
