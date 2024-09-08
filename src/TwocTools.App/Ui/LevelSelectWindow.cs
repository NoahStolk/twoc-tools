using ImGuiNET;
using TwocTools.App.Extensions;
using TwocTools.App.State;
using TwocTools.Core.DataTypes.Crt;
using TwocTools.Core.DataTypes.Wmp;
using TwocTools.Core.Serializers;

namespace TwocTools.App.Ui;

public sealed class LevelSelectWindow
{
	private readonly GameState _gameState;
	private readonly LevelState _levelState;

	private string[]? _levelPaths;

	public LevelSelectWindow(GameState gameState, LevelState levelState)
	{
		_gameState = gameState;
		_levelState = levelState;
	}

	public void Render()
	{
		if (_levelPaths == null && _gameState.IsValid)
			_levelPaths = Directory.GetDirectories(Path.Combine(_gameState.OpenedDirectory, "LEVELS"), "*", SearchOption.AllDirectories).Where(d => Directory.GetDirectories(d).Length == 0).ToArray();

		if (_levelPaths == null)
			return;

		if (ImGui.Begin("Level Select", ImGuiWindowFlags.NoCollapse))
		{
			foreach (string levelPath in _levelPaths)
			{
				string levelName = Path.GetFileName(levelPath);
				if (ImGui.Button(levelName))
				{
					string? wmpFilePath = Directory.GetFiles(levelPath, "*.wmp", SearchOption.AllDirectories).FirstOrDefault();
					string? crtFilePath = Directory.GetFiles(levelPath, "*.crt", SearchOption.AllDirectories).FirstOrDefault();
					ImportLevel(wmpFilePath, crtFilePath);
				}
			}
		}

		ImGui.End();
	}

	private void ImportLevel(string? wmpFilePath, string? crtFilePath)
	{
		CrateGroupCollection crateGroupCollection = CrateGroupCollection.Empty;
		WumpaCollection wumpaCollection = WumpaCollection.Empty;

		if (File.Exists(crtFilePath))
		{
			using FileStream fs = File.OpenRead(crtFilePath);
			crateGroupCollection = CrateSerializer.Deserialize(fs, _gameState.GameVersion.GetEndianness());
		}

		if (File.Exists(wmpFilePath))
		{
			using FileStream fs = File.OpenRead(wmpFilePath);
			wumpaCollection = WumpaSerializer.Deserialize(fs, _gameState.GameVersion.GetEndianness());
		}

		_levelState.SetLevel(
			crtFilePath ?? "<None>",
			wmpFilePath ?? "<None>",
			crateGroupCollection,
			wumpaCollection);
	}
}
