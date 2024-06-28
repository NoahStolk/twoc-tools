using Detach;
using ImGuiNET;
using NativeFileDialogSharp;
using TwocTools.Core.DataTypes;
using TwocTools.Core.Internals;
using TwocTools.Core.Serializers;

namespace TwocTools.App.Ui;

public static class WumpaDisplayWindow
{
	private static WumpaCollection _wumpaCollection = WumpaCollection.Empty;

	private static void LoadWumpas()
	{
		DialogResult dialogResult = Dialog.FileOpen("wmp,WMP");
		if (!dialogResult.IsOk)
			return;

		using BigEndianBinaryReader br = new(File.OpenRead(dialogResult.Path));
		_wumpaCollection = WumpaSerializer.Deserialize(br);
	}

	public static void Render()
	{
		if (ImGui.Begin("Wumpa Display"))
		{
			if (ImGui.Button("Load Wumpa (.wmp) file"))
				LoadWumpas();

			ImGui.Separator();

			ImGui.Text($"Wumpa count: {_wumpaCollection.Count}");

			if (ImGui.BeginTable("WumpasTable", 1, ImGuiTableFlags.ScrollY))
			{
				ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthStretch);

				ImGui.TableSetupScrollFreeze(0, 1);
				ImGui.TableHeadersRow();

				foreach (Wumpa wumpa in _wumpaCollection)
				{
					ImGui.TableNextRow();
					ImGui.TableNextColumn();

					ImGui.Text(Inline.Span(wumpa.Position));
				}

				ImGui.EndTable();
			}
		}

		ImGui.End();
	}
}
