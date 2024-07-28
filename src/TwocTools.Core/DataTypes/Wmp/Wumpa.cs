using System.Numerics;

namespace TwocTools.Core.DataTypes.Wmp;

public record struct Wumpa
{
	public Wumpa(Vector3 position)
	{
		Position = position;
	}

	public Vector3 Position { get; }
}
