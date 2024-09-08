using TwocTools.Core.DataTypes.Crt;
using TwocTools.Core.DataTypes.Wmp;

namespace TwocTools.App.State;

public sealed class LevelState
{
	// TODO: Refactor.
	// Visualization created from state (used for sorting, etc.)
	public List<CrateGroup> CrateGroupVisualization { get; set; } = [];
	public List<Crate> CratesVisualization { get; set; } = [];

	public string CrateGroupCollectionPath { get; private set; } = string.Empty;
	public string WumpaCollectionPath { get; private set; } = string.Empty;
	public CrateGroupCollection CrateGroupCollection { get; private set; } = CrateGroupCollection.Empty;
	public WumpaCollection WumpaCollection { get; private set; } = WumpaCollection.Empty;

	public void SetLevel(
		string crateGroupCollectionPath,
		string wumpaCollectionPath,
		CrateGroupCollection crateGroupCollection,
		WumpaCollection wumpaCollection)
	{
		CrateGroupCollectionPath = crateGroupCollectionPath;
		WumpaCollectionPath = wumpaCollectionPath;

		CrateGroupCollection = crateGroupCollection;
		WumpaCollection = wumpaCollection;

		CrateGroupVisualization = CrateGroupCollection.ToList();
		CratesVisualization = CrateGroupCollection.SelectMany(c => c).ToList();
	}
}
