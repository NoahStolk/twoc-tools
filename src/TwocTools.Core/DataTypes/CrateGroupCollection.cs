using System.Collections;

namespace TwocTools.Core.DataTypes;

public record CrateGroupCollection : IEnumerable<CrateGroup>
{
	private readonly List<CrateGroup> _crateGroups;

	public CrateGroupCollection(uint version, List<CrateGroup> crateGroups)
	{
		Version = version;
		_crateGroups = crateGroups;
	}

	public uint Version { get; }

	public int Count => _crateGroups.Count;

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
