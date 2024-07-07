using TwocTools.App.Ui;
using TwocTools.Core.DataTypes;

namespace TwocTools.App.State;

public static class LevelState
{
	public static CrateGroupCollection CrateGroupCollection { get; private set; } = CrateGroupCollection.Empty;
	public static WumpaCollection WumpaCollection { get; private set; } = WumpaCollection.Empty;

	public static void SetLevel(CrateGroupCollection crateGroupCollection, WumpaCollection wumpaCollection)
	{
		CrateGroupCollection = crateGroupCollection;
		WumpaCollection = wumpaCollection;

		// TODO: Refactor.
		CrateDisplayWindow.UpdateState();
	}
}
