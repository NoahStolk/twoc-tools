using TwocTools.App.Ui;
using TwocTools.Core.DataTypes;
using TwocTools.Core.DataTypes.Crt;
using TwocTools.Core.DataTypes.Wmp;

namespace TwocTools.App.State;

public static class LevelState
{
	public static string CrateGroupCollectionPath { get; private set; } = string.Empty;
	public static string WumpaCollectionPath { get; private set; } = string.Empty;
	public static CrateGroupCollection CrateGroupCollection { get; private set; } = CrateGroupCollection.Empty;
	public static WumpaCollection WumpaCollection { get; private set; } = WumpaCollection.Empty;

	public static void SetLevel(
		string crateGroupCollectionPath,
		string wumpaCollectionPath,
		CrateGroupCollection crateGroupCollection,
		WumpaCollection wumpaCollection)
	{
		CrateGroupCollectionPath = crateGroupCollectionPath;
		WumpaCollectionPath = wumpaCollectionPath;

		CrateGroupCollection = crateGroupCollection;
		WumpaCollection = wumpaCollection;

		// TODO: Refactor.
		CrateInfoWindow.UpdateState();
	}
}
