using Hexa.NET.ImGui;
using NativeFileDialogSharp;
using TwocTools.App.State;

namespace TwocTools.App.Ui;

internal sealed class GameSelectWindow(GameState gameState)
{
	private GameVersion _gameVersion = GameVersion.Ps2GreatestHits;

	public void Render()
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

		ImGui.End();
	}

	private void Import()
	{
		DialogResult dialogResult = Dialog.FolderPicker();
		if (dialogResult.IsOk)
			gameState.SetGame(dialogResult.Path, _gameVersion);
	}
}
