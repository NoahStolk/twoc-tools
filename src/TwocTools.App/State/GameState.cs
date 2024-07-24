namespace TwocTools.App.State;

public static class GameState
{
	public static bool IsValid { get; private set; }

	public static string OpenedDirectory { get; private set; } = string.Empty;

	public static GameVersion GameVersion { get; private set; } = GameVersion.Ps2GreatestHits;

	public static void SetGame(string directory, GameVersion version)
	{
		IsValid = Directory.Exists(directory);
		OpenedDirectory = directory;
		GameVersion = version;
	}
}
