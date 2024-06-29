using Detach;
using ImGuiNET;
using NativeFileDialogSharp;
using System.Numerics;
using TwocTools.App.Extensions;
using TwocTools.Core;
using TwocTools.Core.DataTypes;
using TwocTools.Core.Serializers;

namespace TwocTools.App.Ui;

public static class CrateDisplayWindow
{
	// Utilities
	private static readonly List<CrateType> _allCrateTypes = Enum.GetValues<CrateType>().ToList();

	// State
	private static CrateGroupCollection _crateGroupCollection = CrateGroupCollection.Empty;

	// Visualization created from state (used for sorting, etc.)
	private static List<CrateGroup> _crateGroupVisualization = [];
	private static List<Crate> _cratesVisualization = [];

	private static void LoadCrates()
	{
		DialogResult dialogResult = Dialog.FileOpen("crt,CRT");
		if (!dialogResult.IsOk)
			return;

		using FileStream fs = File.OpenRead(dialogResult.Path);
		_crateGroupCollection = CrateSerializer.Deserialize(fs, Endianness.Little); // TODO: Choose from UI.
		_crateGroupVisualization = _crateGroupCollection.ToList();
		_cratesVisualization = _crateGroupCollection.SelectMany(c => c).ToList();
	}

	public static void Render()
	{
		if (ImGui.Begin("Crate Display"))
		{
			if (ImGui.Button("Load Crate (.crt) file"))
				LoadCrates();

			ImGui.Separator();

			if (ImGui.BeginTabBar("CrateDisplayTabBar"))
			{
				if (ImGui.BeginTabItem("Info"))
				{
					ImGui.Text(Inline.Span($"Version: {_crateGroupCollection.Version}"));
					ImGui.Text(Inline.Span($"Crate group count: {_crateGroupCollection.Count}"));
					ImGui.Text(Inline.Span($"Crate count: {_cratesVisualization.Count}"));

					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Crate Type Counts"))
				{
					RenderCrateTypeCountsTable();

					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Crate Groups"))
				{
					RenderCrateGroupsTable();

					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Crates"))
				{
					RenderCratesTable();

					ImGui.EndTabItem();
				}

				ImGui.EndTabBar();
			}
		}

		ImGui.End();
	}

	private static void RenderCrateTypeCountsTable()
	{
		if (ImGui.BeginTable("CrateTypeCountsTable", 2))
		{
			ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 160, 0);
			ImGui.TableSetupColumn("Count", ImGuiTableColumnFlags.WidthFixed, 80, 1);

			ImGui.TableHeadersRow();

			foreach (CrateType crateType in _allCrateTypes)
			{
				ImGui.TableNextRow();

				TableNextColumnText(Inline.Span(crateType), crateType.GetColor());
				TableNextColumnText(Inline.Span(_cratesVisualization.Count(c => c.CrateType == crateType)));
			}

			ImGui.EndTable();
		}
	}

	private static unsafe void RenderCrateGroupsTable()
	{
		if (ImGui.BeginTable("CrateGroupsTable", 14, ImGuiTableFlags.ScrollY | ImGuiTableFlags.Sortable))
		{
			ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 280, 0);
			ImGui.TableSetupColumn("Crate Offset", ImGuiTableColumnFlags.WidthFixed, 120, 1);
			ImGui.TableSetupColumn("Crate Count", ImGuiTableColumnFlags.WidthFixed, 120, 2);
			ImGui.TableSetupColumn("Tilt", ImGuiTableColumnFlags.WidthFixed, 40, 3);

			ImGui.TableSetupScrollFreeze(0, 1);
			ImGui.TableHeadersRow();

			ImGuiTableSortSpecsPtr sortsSpecs = ImGui.TableGetSortSpecs();
			if (sortsSpecs.NativePtr != (void*)0 && sortsSpecs.SpecsDirty)
			{
				uint sorting = sortsSpecs.Specs.ColumnUserID;
				bool sortAscending = sortsSpecs.Specs.SortDirection == ImGuiSortDirection.Ascending;

				Action sortAction = sorting switch
				{
					0 => () => _crateGroupVisualization.Sort((a, b) =>
					{
						int result = a.Position.X.CompareTo(b.Position.X);
						if (result == 0)
							result = a.Position.Y.CompareTo(b.Position.Y);
						if (result == 0)
							result = a.Position.Z.CompareTo(b.Position.Z);

						return sortAscending ? result : -result;
					}),
					1 => () => _crateGroupVisualization.Sort((a, b) => sortAscending ? a.CrateOffset.CompareTo(b.CrateOffset) : -a.CrateOffset.CompareTo(b.CrateOffset)),
					2 => () => _crateGroupVisualization.Sort((a, b) => sortAscending ? a.CrateCount.CompareTo(b.CrateCount) : -a.CrateCount.CompareTo(b.CrateCount)),
					3 => () => _crateGroupVisualization.Sort((a, b) => sortAscending ? a.Tilt.CompareTo(b.Tilt) : -a.Tilt.CompareTo(b.Tilt)),
					_ => static () => { },
				};
				sortAction();

				sortsSpecs.SpecsDirty = false;
			}

			foreach (CrateGroup crateGroup in _crateGroupVisualization)
			{
				ImGui.TableNextRow();

				TableNextColumnText(Inline.Span(crateGroup.Position));
				TableNextColumnText(Inline.Span(crateGroup.CrateOffset));
				TableNextColumnText(Inline.Span(crateGroup.CrateCount));
				TableNextColumnText(Inline.Span(crateGroup.Tilt));
			}

			ImGui.EndTable();
		}
	}

	private static unsafe void RenderCratesTable()
	{
		if (ImGui.BeginTable("CratesTable", 14, ImGuiTableFlags.ScrollY | ImGuiTableFlags.Sortable))
		{
			ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 240, 0);
			ImGui.TableSetupColumn("A", ImGuiTableColumnFlags.WidthFixed, 40, 1);
			ImGui.TableSetupColumn("Rotation", ImGuiTableColumnFlags.WidthFixed, 120, 2);
			ImGui.TableSetupColumn("Crate Type", ImGuiTableColumnFlags.WidthFixed, 160, 3);
			ImGui.TableSetupColumn("Crate Type Time Trial", ImGuiTableColumnFlags.WidthFixed, 160, 4);
			ImGui.TableSetupColumn("D", ImGuiTableColumnFlags.WidthFixed, 40, 5);
			ImGui.TableSetupColumn("E", ImGuiTableColumnFlags.WidthFixed, 40, 6);
			ImGui.TableSetupColumn("F", ImGuiTableColumnFlags.WidthFixed, 40, 7);
			ImGui.TableSetupColumn("G", ImGuiTableColumnFlags.WidthFixed, 40, 8);
			ImGui.TableSetupColumn("H", ImGuiTableColumnFlags.WidthFixed, 40, 9);
			ImGui.TableSetupColumn("I", ImGuiTableColumnFlags.WidthFixed, 40, 10);
			ImGui.TableSetupColumn("J", ImGuiTableColumnFlags.WidthFixed, 40, 11);
			ImGui.TableSetupColumn("K", ImGuiTableColumnFlags.WidthFixed, 40, 12);
			ImGui.TableSetupColumn("L", ImGuiTableColumnFlags.WidthFixed, 40, 13);

			ImGui.TableSetupScrollFreeze(0, 1);
			ImGui.TableHeadersRow();

			ImGuiTableSortSpecsPtr sortsSpecs = ImGui.TableGetSortSpecs();
			if (sortsSpecs.NativePtr != (void*)0 && sortsSpecs.SpecsDirty)
			{
				uint sorting = sortsSpecs.Specs.ColumnUserID;
				bool sortAscending = sortsSpecs.Specs.SortDirection == ImGuiSortDirection.Ascending;

				Action sortAction = sorting switch
				{
					0 => () => _cratesVisualization.Sort((a, b) =>
					{
						int result = a.Position.X.CompareTo(b.Position.X);
						if (result == 0)
							result = a.Position.Y.CompareTo(b.Position.Y);
						if (result == 0)
							result = a.Position.Z.CompareTo(b.Position.Z);

						return sortAscending ? result : -result;
					}),
					1 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.A.CompareTo(b.A) : -a.A.CompareTo(b.A)),
					2 => () => _cratesVisualization.Sort((a, b) =>
					{
						int result = a.RotationX.CompareTo(b.RotationX);
						if (result == 0)
							result = a.RotationY.CompareTo(b.RotationY);
						if (result == 0)
							result = a.RotationZ.CompareTo(b.RotationZ);

						return sortAscending ? result : -result;
					}),
					3 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.CrateType.CompareTo(b.CrateType) : -a.CrateType.CompareTo(b.CrateType)),
					4 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.CrateTypeTimeTrial.CompareTo(b.CrateTypeTimeTrial) : -a.CrateTypeTimeTrial.CompareTo(b.CrateTypeTimeTrial)),
					5 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.D.CompareTo(b.D) : -a.D.CompareTo(b.D)),
					6 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.E.CompareTo(b.E) : -a.E.CompareTo(b.E)),
					7 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.F.CompareTo(b.F) : -a.F.CompareTo(b.F)),
					8 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.G.CompareTo(b.G) : -a.G.CompareTo(b.G)),
					9 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.H.CompareTo(b.H) : -a.H.CompareTo(b.H)),
					10 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.I.CompareTo(b.I) : -a.I.CompareTo(b.I)),
					11 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.J.CompareTo(b.J) : -a.J.CompareTo(b.J)),
					12 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.K.CompareTo(b.K) : -a.K.CompareTo(b.K)),
					13 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.L.CompareTo(b.L) : -a.L.CompareTo(b.L)),
					_ => static () => { },
				};
				sortAction();

				sortsSpecs.SpecsDirty = false;
			}

			Vector4 colorDefault = *ImGui.GetStyleColorVec4(ImGuiCol.Text);
			Vector4 colorDisabled = *ImGui.GetStyleColorVec4(ImGuiCol.TextDisabled);
			foreach (Crate crate in _cratesVisualization)
			{
				ImGui.TableNextRow();

				TableNextColumnText(Inline.Span(crate.Position));
				TableNextColumnText(Inline.Span(crate.A), crate.A is > -float.Epsilon and < float.Epsilon ? colorDisabled : colorDefault);
				TableNextColumnText(Inline.Span($"{crate.RotationX}, {crate.RotationY}, {crate.RotationZ}"));
				TableNextColumnText(Inline.Span(crate.CrateType), crate.CrateType.GetColor());
				TableNextColumnText(Inline.Span(crate.CrateTypeTimeTrial), crate.CrateTypeTimeTrial == crate.CrateType ? colorDisabled : crate.CrateTypeTimeTrial.GetColor());
				TableNextColumnText(Inline.Span(crate.D), crate.D == -1 ? colorDisabled : colorDefault);
				TableNextColumnText(Inline.Span(crate.E), crate.E == -1 ? colorDisabled : colorDefault);
				TableNextColumnText(Inline.Span(crate.F), crate.F == -1 ? colorDisabled : colorDefault);
				TableNextColumnText(Inline.Span(crate.G), crate.G == -1 ? colorDisabled : colorDefault);
				TableNextColumnText(Inline.Span(crate.H), crate.H == -1 ? colorDisabled : colorDefault);
				TableNextColumnText(Inline.Span(crate.I), crate.I == -1 ? colorDisabled : colorDefault);
				TableNextColumnText(Inline.Span(crate.J), crate.J == -1 ? colorDisabled : colorDefault);
				TableNextColumnText(Inline.Span(crate.K), crate.K == -1 ? colorDisabled : colorDefault);
				TableNextColumnText(Inline.Span(crate.L), crate.L == -1 ? colorDisabled : colorDefault);
			}

			ImGui.EndTable();
		}
	}

	private static void TableNextColumnText(ReadOnlySpan<char> text, Vector4 color)
	{
		ImGui.TableNextColumn();
		ImGui.TextColored(color, text);
	}

	private static void TableNextColumnText(ReadOnlySpan<char> text)
	{
		ImGui.TableNextColumn();
		ImGui.Text(text);
	}
}
