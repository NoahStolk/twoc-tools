namespace TwocTools.App.State;

public sealed class GameState
{
	public bool IsValid { get; private set; }

	public string OpenedDirectory { get; private set; } = string.Empty;

	public GameVersion GameVersion { get; private set; } = GameVersion.Ps2GreatestHits;

	public void SetGame(string directory, GameVersion version)
	{
		IsValid = Directory.Exists(directory);
		OpenedDirectory = directory;
		GameVersion = version;
	}
}
