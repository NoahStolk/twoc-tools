using Detach;
using ImGuiNET;
using TwocTools.App.State;
using TwocTools.Core.DataTypes;

namespace TwocTools.App.Ui;

public static class WumpaDisplayWindow
{
	public static void Render()
	{
		if (ImGui.Begin("Wumpa Display"))
		{
			ImGui.Text(Inline.Span($"Wumpa count: {LevelState.WumpaCollection.Count}"));

			if (ImGui.BeginTable("WumpasTable", 1, ImGuiTableFlags.ScrollY))
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
