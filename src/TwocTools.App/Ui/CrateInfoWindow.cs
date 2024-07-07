using Detach;
using ImGuiNET;
using System.Numerics;
using TwocTools.App.Extensions;
using TwocTools.App.State;
using TwocTools.Core;
using TwocTools.Core.DataTypes;
using TwocTools.Core.Serializers;

namespace TwocTools.App.Ui;

public static unsafe class CrateInfoWindow
{
	// Utilities
	private static readonly List<CrateType> _allCrateTypes = Enum.GetValues<CrateType>().ToList();
	private static readonly Dictionary<CrateType, string> _crateTypeNames = _allCrateTypes.ToDictionary(t => t, t => t.ToString());

	// Visualization created from state (used for sorting, etc.)
	private static List<CrateGroup> _crateGroupVisualization = [];
	private static List<Crate> _cratesVisualization = [];

	// TODO: Refactor to event handler or something.
	public static void UpdateState()
	{
		_crateGroupVisualization = LevelState.CrateGroupCollection.ToList();
		_cratesVisualization = LevelState.CrateGroupCollection.SelectMany(c => c).ToList();
	}

	public static void Render()
	{
		if (ImGui.Begin("Crate Info"))
		{
			ImGui.Text(LevelState.CrateGroupCollectionPath);

			ImGui.Separator();

			ImGui.Text(Inline.Span($"Version: {LevelState.CrateGroupCollection.Version}"));
			ImGui.Text(Inline.Span($"Crate group count: {LevelState.CrateGroupCollection.Count}"));
			ImGui.Text(Inline.Span($"Crate count: {_cratesVisualization.Count}"));

			ImGui.Separator();

			if (ImGui.BeginTabBar("CrateDisplayTabBar"))
			{
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
		if (ImGui.BeginTable("CrateTypeCountsTable", 5))
		{
			ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 160, 0);
			ImGui.TableSetupColumn("Count A", ImGuiTableColumnFlags.WidthFixed, 80, 1);
			ImGui.TableSetupColumn("Count B", ImGuiTableColumnFlags.WidthFixed, 80, 1);
			ImGui.TableSetupColumn("Count C", ImGuiTableColumnFlags.WidthFixed, 80, 1);
			ImGui.TableSetupColumn("Count D", ImGuiTableColumnFlags.WidthFixed, 80, 1);

			ImGui.TableHeadersRow();

			foreach (CrateType crateType in _allCrateTypes)
			{
				ImGui.TableNextRow();

				Vector4 colorDefault = *ImGui.GetStyleColorVec4(ImGuiCol.Text);
				Vector4 colorDisabled = *ImGui.GetStyleColorVec4(ImGuiCol.TextDisabled);

				int countA = _cratesVisualization.Count(c => c.CrateTypeA == crateType);
				int countB = _cratesVisualization.Count(c => c.CrateTypeB == crateType);
				int countC = _cratesVisualization.Count(c => c.CrateTypeC == crateType);
				int countD = _cratesVisualization.Count(c => c.CrateTypeD == crateType);

				TableNextColumnText(Inline.Span(crateType), crateType.GetColor());
				TableNextColumnText(Inline.Span(countA), countA == 0 ? colorDisabled : colorDefault);
				TableNextColumnText(Inline.Span(countB), countB == 0 ? colorDisabled : colorDefault);
				TableNextColumnText(Inline.Span(countC), countC == 0 ? colorDisabled : colorDefault);
				TableNextColumnText(Inline.Span(countD), countD == 0 ? colorDisabled : colorDefault);
			}

			ImGui.EndTable();
		}
	}

	private static void RenderCrateGroupsTable()
	{
		if (ImGui.BeginTable("CrateGroupsTable", 6, ImGuiTableFlags.ScrollY | ImGuiTableFlags.Sortable))
		{
			ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 280, 0);
			ImGui.TableSetupColumn("Crate Offset", ImGuiTableColumnFlags.WidthFixed, 120, 1);
			ImGui.TableSetupColumn("Crate Count", ImGuiTableColumnFlags.WidthFixed, 120, 2);
			ImGui.TableSetupColumn("Tilt (Raw)", ImGuiTableColumnFlags.WidthFixed, 120, 3);
			ImGui.TableSetupColumn("Tilt (Radians)", ImGuiTableColumnFlags.WidthFixed, 120, 4);
			ImGui.TableSetupColumn("Tilt (Degrees)", ImGuiTableColumnFlags.WidthFixed, 120, 5);

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
					4 => () => _crateGroupVisualization.Sort((a, b) => sortAscending ? a.TiltInRadians.CompareTo(b.TiltInRadians) : -a.TiltInRadians.CompareTo(b.TiltInRadians)),
					5 => () => _crateGroupVisualization.Sort((a, b) => sortAscending ? a.TiltInDegrees.CompareTo(b.TiltInDegrees) : -a.TiltInDegrees.CompareTo(b.TiltInDegrees)),
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
				TableNextColumnText(Inline.Span(crateGroup.TiltInRadians));
				TableNextColumnText(Inline.Span(crateGroup.TiltInDegrees));
			}

			ImGui.EndTable();
		}
	}

	private static void RenderCratesTable()
	{
		const int columnCount = 16;
		if (ImGui.BeginTable("CratesTable", columnCount, ImGuiTableFlags.ScrollY | ImGuiTableFlags.Sortable))
		{
			ImGui.TableSetupColumn("Index", ImGuiTableColumnFlags.WidthFixed, 60, 0);
			ImGui.TableSetupColumn("Group Index", ImGuiTableColumnFlags.WidthFixed, 80, 1);
			ImGui.TableSetupColumn("World Position", ImGuiTableColumnFlags.WidthFixed, 240, 2);
			ImGui.TableSetupColumn("A", ImGuiTableColumnFlags.WidthFixed, 40, 3);
			ImGui.TableSetupColumn("Local Position", ImGuiTableColumnFlags.WidthFixed, 120, 4);
			ImGui.TableSetupColumn("Crate Type A", ImGuiTableColumnFlags.WidthFixed, 160, 5);
			ImGui.TableSetupColumn("Crate Type B", ImGuiTableColumnFlags.WidthFixed, 160, 6);
			ImGui.TableSetupColumn("Crate Type C", ImGuiTableColumnFlags.WidthFixed, 160, 7);
			ImGui.TableSetupColumn("Crate Type D", ImGuiTableColumnFlags.WidthFixed, 160, 8);
			ImGui.TableSetupColumn("F", ImGuiTableColumnFlags.WidthFixed, 40, 9);
			ImGui.TableSetupColumn("G", ImGuiTableColumnFlags.WidthFixed, 40, 10);
			ImGui.TableSetupColumn("H", ImGuiTableColumnFlags.WidthFixed, 40, 11);
			ImGui.TableSetupColumn("I", ImGuiTableColumnFlags.WidthFixed, 40, 12);
			ImGui.TableSetupColumn("J", ImGuiTableColumnFlags.WidthFixed, 40, 13);
			ImGui.TableSetupColumn("K", ImGuiTableColumnFlags.WidthFixed, 40, 14);
			ImGui.TableSetupColumn("L", ImGuiTableColumnFlags.WidthFixed, 40, 15);

			ImGui.TableSetupScrollFreeze(0, 1);
			ImGui.TableHeadersRow();

			for (int i = 0; i < columnCount; i++)
			{
				if (!ImGui.TableSetColumnIndex(i))
					continue;

				ImGui.TableHeader(ImGui.TableGetColumnName(i));
				if (!ImGui.IsItemHovered())
					continue;

				string? tooltip = i switch
				{
					5 => "The default crate type",
					6 => "The crate type used for time trial",
					7 => "For slot crates: The first option\nFor empty crates: The crate type that the empty crate will change into when the corresponding exclamation crate is triggered",
					8 => "For slot crates: The second option",
					_ => null,
				};
				if (tooltip != null)
					ImGui.SetTooltip(tooltip);
			}

			ImGuiTableSortSpecsPtr sortsSpecs = ImGui.TableGetSortSpecs();
			if (sortsSpecs.NativePtr != (void*)0 && sortsSpecs.SpecsDirty)
			{
				uint sorting = sortsSpecs.Specs.ColumnUserID;
				bool sortAscending = sortsSpecs.Specs.SortDirection == ImGuiSortDirection.Ascending;

				Action sortAction = sorting switch
				{
					0 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.Index.CompareTo(b.Index) : -a.Index.CompareTo(b.Index)),
					1 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.GroupIndex.CompareTo(b.GroupIndex) : -a.GroupIndex.CompareTo(b.GroupIndex)),
					2 => () => _cratesVisualization.Sort((a, b) =>
					{
						int result = a.WorldPosition.X.CompareTo(b.WorldPosition.X);
						if (result == 0)
							result = a.WorldPosition.Y.CompareTo(b.WorldPosition.Y);
						if (result == 0)
							result = a.WorldPosition.Z.CompareTo(b.WorldPosition.Z);

						return sortAscending ? result : -result;
					}),
					3 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.A.CompareTo(b.A) : -a.A.CompareTo(b.A)),
					4 => () => _cratesVisualization.Sort((a, b) =>
					{
						int result = a.LocalPositionX.CompareTo(b.LocalPositionX);
						if (result == 0)
							result = a.LocalPositionY.CompareTo(b.LocalPositionY);
						if (result == 0)
							result = a.LocalPositionZ.CompareTo(b.LocalPositionZ);

						return sortAscending ? result : -result;
					}),
					5 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.CrateTypeA.CompareTo(b.CrateTypeA) : -a.CrateTypeA.CompareTo(b.CrateTypeA)),
					6 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.CrateTypeB.CompareTo(b.CrateTypeB) : -a.CrateTypeB.CompareTo(b.CrateTypeB)),
					7 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.CrateTypeC.CompareTo(b.CrateTypeC) : -a.CrateTypeC.CompareTo(b.CrateTypeC)),
					8 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.CrateTypeD.CompareTo(b.CrateTypeD) : -a.CrateTypeD.CompareTo(b.CrateTypeD)),
					9 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.F.CompareTo(b.F) : -a.F.CompareTo(b.F)),
					10 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.G.CompareTo(b.G) : -a.G.CompareTo(b.G)),
					11 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.H.CompareTo(b.H) : -a.H.CompareTo(b.H)),
					12 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.I.CompareTo(b.I) : -a.I.CompareTo(b.I)),
					13 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.J.CompareTo(b.J) : -a.J.CompareTo(b.J)),
					14 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.K.CompareTo(b.K) : -a.K.CompareTo(b.K)),
					15 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.L.CompareTo(b.L) : -a.L.CompareTo(b.L)),
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

				TableNextColumnText(Inline.Span(crate.Index));
				TableNextColumnText(Inline.Span(crate.GroupIndex));
				TableNextColumnText(Inline.Span(crate.WorldPosition));
				TableNextColumnText(Inline.Span(crate.A), crate.A is > -float.Epsilon and < float.Epsilon ? colorDisabled : colorDefault);
				TableNextColumnText(Inline.Span($"{crate.LocalPositionX}, {crate.LocalPositionY}, {crate.LocalPositionZ}"));
				TableNextColumnText(_crateTypeNames[crate.CrateTypeA], crate.CrateTypeA.GetColor());
				TableNextColumnText(_crateTypeNames[crate.CrateTypeB], crate.CrateTypeB.GetColor());
				TableNextColumnText(_crateTypeNames[crate.CrateTypeC], crate.CrateTypeC.GetColor());
				TableNextColumnText(_crateTypeNames[crate.CrateTypeD], crate.CrateTypeD.GetColor());
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
