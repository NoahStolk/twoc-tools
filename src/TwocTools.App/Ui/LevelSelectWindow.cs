using ImGuiNET;
using TwocTools.App.Extensions;
using TwocTools.App.State;
using TwocTools.Core.DataTypes;
using TwocTools.Core.Serializers;

namespace TwocTools.App.Ui;

public static class LevelSelectWindow
{
	private static string[]? _levelPaths;

	public static void Render()
	{
		if (_levelPaths == null && GameState.IsValid)
			_levelPaths = Directory.GetDirectories(Path.Combine(GameState.OpenedDirectory, "LEVELS"), "*", SearchOption.AllDirectories).Where(d => Directory.GetDirectories(d).Length == 0).ToArray();

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

	private static void ImportLevel(string? wmpFilePath, string? crtFilePath)
	{
		CrateGroupCollection crateGroupCollection = CrateGroupCollection.Empty;
		WumpaCollection wumpaCollection = WumpaCollection.Empty;

		if (File.Exists(crtFilePath))
		{
			using FileStream fs = File.OpenRead(crtFilePath);
			crateGroupCollection = CrateSerializer.Deserialize(fs, GameState.GameVersion.GetEndianness());
		}

		if (File.Exists(wmpFilePath))
		{
			using FileStream fs = File.OpenRead(wmpFilePath);
			wumpaCollection = WumpaSerializer.Deserialize(fs, GameState.GameVersion.GetEndianness());
		}

		LevelState.SetLevel(
			crtFilePath ?? "<None>",
			wmpFilePath ?? "<None>",
			crateGroupCollection,
			wumpaCollection);
	}
}
