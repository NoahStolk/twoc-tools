using ImGuiNET;
using NativeFileDialogSharp;
using TwocTools.App.State;

namespace TwocTools.App.Ui;

public static class GameSelectWindow
{
	private static GameVersion _gameVersion = GameVersion.Ps2GreatestHits;

	public static void Render()
	{
		if (ImGui.Begin("Game Select", ImGuiWindowFlags.NoCollapse))
		{
			if (ImGui.BeginCombo("Game Version", _gameVersion.ToString()))
			{
				foreach (GameVersion gameVersion in Enum.GetValues<GameVersion>())
				{
					bool isSelected = _gameVersion == gameVersion;
					if (ImGui.Selectable(gameVersion.ToString(), isSelected))
						_gameVersion = gameVersion;
					if (isSelected)
						ImGui.SetItemDefaultFocus();
				}

				ImGui.EndCombo();
			}

			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("Currently, only PS2 Greatest Hits is tested. Other versions may not work.");

			if (ImGui.Button("Choose directory"))
				Import();
		}
	}

	private static void Import()
	{
		DialogResult dialogResult = Dialog.FolderPicker();
		if (dialogResult.IsOk)
			GameState.SetGame(dialogResult.Path, _gameVersion);
	}
}
