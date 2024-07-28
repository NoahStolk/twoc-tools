using System.Collections;

namespace TwocTools.Core.DataTypes.Crt;

public record CrateGroupCollection : IEnumerable<CrateGroup>
{
	private readonly List<CrateGroup> _crateGroups;

	internal CrateGroupCollection(uint version, List<CrateGroup> crateGroups)
	{
		Version = version;
		_crateGroups = crateGroups;
	}

	public uint Version { get; }

	public int Count => _crateGroups.Count;

	public static CrateGroupCollection Empty => new(4, []);

	public CrateGroup this[int index] => _crateGroups[index];

	public IEnumerator<CrateGroup> GetEnumerator()
	{
		return _crateGroups.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
