using System.Collections;

namespace TwocTools.Core.DataTypes;

public record WumpaCollection : IEnumerable<Wumpa>
{
	private readonly List<Wumpa> _wumpas;

	internal WumpaCollection(List<Wumpa> wumpas)
	{
		_wumpas = wumpas;
	}

	public int Count => _wumpas.Count;

	public static WumpaCollection Empty => new(new List<Wumpa>());

	public Wumpa this[int index] => _wumpas[index];

	public IEnumerator<Wumpa> GetEnumerator()
	{
		return _wumpas.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
