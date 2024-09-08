using Detach;
using ImGuiNET;
using TwocTools.App.State;
using TwocTools.Core.DataTypes.Wmp;

namespace TwocTools.App.Ui;

public sealed class WumpaInfoWindow
{
	private readonly LevelState _levelState;

	public WumpaInfoWindow(LevelState levelState)
	{
		_levelState = levelState;
	}

	public void Render()
	{
		if (ImGui.Begin("Wumpa Info"))
		{
			ImGui.Text(_levelState.WumpaCollectionPath);

			ImGui.Separator();

			ImGui.Text(Inline.Span($"Wumpa count: {_levelState.WumpaCollection.Count}"));

			ImGui.Separator();

			if (ImGui.BeginTable("WumpaTable", 1, ImGuiTableFlags.ScrollY))
			{
				ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthStretch);

				ImGui.TableSetupScrollFreeze(0, 1);
				ImGui.TableHeadersRow();

				foreach (Wumpa wumpa in _levelState.WumpaCollection)
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
