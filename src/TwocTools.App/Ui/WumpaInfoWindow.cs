using Detach;
using Hexa.NET.ImGui;
using TwocTools.App.State;
using TwocTools.Core.DataTypes.Wmp;

namespace TwocTools.App.Ui;

internal sealed class WumpaInfoWindow(LevelState levelState)
{
	public void Render()
	{
		if (ImGui.Begin("Wumpa Info"))
		{
			ImGui.Text(levelState.WumpaCollectionPath);

			ImGui.Separator();

			ImGui.Text(Inline.Utf8($"Wumpa count: {levelState.WumpaCollection.Count}"));

			ImGui.Separator();

			if (ImGui.BeginTable("WumpaTable", 1, ImGuiTableFlags.ScrollY))
			{
				ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthStretch);

				ImGui.TableSetupScrollFreeze(0, 1);
				ImGui.TableHeadersRow();

				foreach (Wumpa wumpa in levelState.WumpaCollection)
				{
					ImGui.TableNextRow();

					ImGui.TableNextColumn();
					ImGui.Text(Inline.Utf8(wumpa.Position));
				}

				ImGui.EndTable();
			}
		}

		ImGui.End();
	}
}
