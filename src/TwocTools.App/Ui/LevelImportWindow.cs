using ImGuiNET;
using NativeFileDialogSharp;
using TwocTools.App.State;
using TwocTools.Core;
using TwocTools.Core.DataTypes;
using TwocTools.Core.Serializers;

namespace TwocTools.App.Ui;

public static class LevelImportWindow
{
	private static string? _wmpFilePath;
	private static string? _crtFilePath;
	private static Endianness _endianness = Endianness.Little;

	public static void Render()
	{
		if (ImGui.Begin("Level Import", ImGuiWindowFlags.NoCollapse))
		{
			if (ImGui.BeginCombo("Endianness", _endianness.ToString()))
			{
				foreach (Endianness endianness in Enum.GetValues<Endianness>())
				{
					bool isSelected = _endianness == endianness;
					if (ImGui.Selectable(endianness.ToString(), isSelected))
						_endianness = endianness;
					if (isSelected)
						ImGui.SetItemDefaultFocus();
				}

				ImGui.EndCombo();
			}

			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("PS2 - little endian\nXbox - big endian\nGameCube - big endian");

			if (ImGui.Button("Choose directory"))
				Import();

			ImGui.Separator();

			ImGui.Text(_wmpFilePath ?? "<None>");
			ImGui.Text(_crtFilePath ?? "<None>");

			if (ImGui.Button("Import"))
			{
				WumpaCollection wumpaCollection = WumpaCollection.Empty;
				CrateGroupCollection crateGroupCollection = CrateGroupCollection.Empty;

				if (File.Exists(_wmpFilePath))
				{
					using FileStream fs = File.OpenRead(_wmpFilePath);
					wumpaCollection = WumpaSerializer.Deserialize(fs, _endianness);
				}

				if (File.Exists(_crtFilePath))
				{
					using FileStream fs = File.OpenRead(_crtFilePath);
					crateGroupCollection = CrateSerializer.Deserialize(fs, _endianness);
				}

				LevelState.SetLevel(crateGroupCollection, wumpaCollection);
			}
		}

		ImGui.End();
	}

	private static void Import()
	{
		DialogResult dialogResult = Dialog.FolderPicker();
		if (!dialogResult.IsOk)
			return;

		_wmpFilePath = Directory.GetFiles(dialogResult.Path, "*.wmp", SearchOption.AllDirectories).FirstOrDefault();
		_crtFilePath = Directory.GetFiles(dialogResult.Path, "*.crt", SearchOption.AllDirectories).FirstOrDefault();
	}
}
