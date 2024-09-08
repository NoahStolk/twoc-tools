using ImGuiNET;
using NativeFileDialogSharp;
using TwocTools.App.State;

namespace TwocTools.App.Ui;

public sealed class GameSelectWindow
{
	private readonly GameState _gameState;

	private GameVersion _gameVersion = GameVersion.Ps2GreatestHits;

	public GameSelectWindow(GameState gameState)
	{
		_gameState = gameState;
	}

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
	}

	private void Import()
	{
		DialogResult dialogResult = Dialog.FolderPicker();
		if (dialogResult.IsOk)
			_gameState.SetGame(dialogResult.Path, _gameVersion);
	}
}
