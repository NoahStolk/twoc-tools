using Detach;
using ImGuiNET;
using TwocTools.App.State;
using TwocTools.Core.DataTypes;
using TwocTools.Core.DataTypes.Wmp;

namespace TwocTools.App.Ui;

public static class WumpaInfoWindow
{
	public static void Render()
	{
		if (ImGui.Begin("Wumpa Info"))
		{
			ImGui.Text(LevelState.WumpaCollectionPath);

			ImGui.Separator();

			ImGui.Text(Inline.Span($"Wumpa count: {LevelState.WumpaCollection.Count}"));

			ImGui.Separator();

			if (ImGui.BeginTable("WumpaTable", 1, ImGuiTableFlags.ScrollY))
			{
				ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthStretch);

				ImGui.TableSetupScrollFreeze(0, 1);
				ImGui.TableHeadersRow();

				foreach (Wumpa wumpa in LevelState.WumpaCollection)
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
