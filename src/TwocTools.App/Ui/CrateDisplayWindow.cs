using Detach;
using ImGuiNET;
using NativeFileDialogSharp;
using TwocTools.Core.DataTypes;
using TwocTools.Core.Internals;
using TwocTools.Core.Serializers;

namespace TwocTools.App.Ui;

public static class CrateDisplayWindow
{
	private static CrateGroupCollection _crateGroupCollection = CrateGroupCollection.Empty;
	private static List<Crate> _cratesVisualization = [];

	private static void LoadCrates()
	{
		DialogResult dialogResult = Dialog.FileOpen("crt,CRT");
		if (!dialogResult.IsOk)
			return;

		using BigEndianBinaryReader br = new(File.OpenRead(dialogResult.Path));
		_crateGroupCollection = CrateSerializer.Deserialize(br);
		_cratesVisualization = _crateGroupCollection.SelectMany(c => c).ToList();
	}

	public static void Render()
	{
		if (ImGui.Begin("Crate Display"))
		{
			if (ImGui.Button("Load Crate (.crt) file"))
				LoadCrates();

			ImGui.Separator();

			ImGui.Text(Inline.Span($"Version: {_crateGroupCollection.Version}"));
			ImGui.Text(Inline.Span($"Crate group count: {_crateGroupCollection.Count}"));
			ImGui.Text(Inline.Span($"Crate count: {_cratesVisualization.Count}"));

			RenderCratesTable();
		}

		ImGui.End();
	}

	private static unsafe void RenderCratesTable()
	{
		if (ImGui.BeginTable("CratesTable", 8, ImGuiTableFlags.ScrollY | ImGuiTableFlags.Sortable))
		{
			ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 240, 0);
			ImGui.TableSetupColumn("A", ImGuiTableColumnFlags.WidthFixed, 40, 1);
			ImGui.TableSetupColumn("Rotation", ImGuiTableColumnFlags.WidthFixed, 120, 2);
			ImGui.TableSetupColumn("B", ImGuiTableColumnFlags.WidthFixed, 40, 3);
			ImGui.TableSetupColumn("C", ImGuiTableColumnFlags.WidthFixed, 40, 4);
			ImGui.TableSetupColumn("D", ImGuiTableColumnFlags.WidthFixed, 40, 5);
			ImGui.TableSetupColumn("E", ImGuiTableColumnFlags.WidthFixed, 40, 6);
			ImGui.TableSetupColumn("F, G, H, I, J, K, L", ImGuiTableColumnFlags.WidthStretch, 0, 7);

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
					3 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.B.CompareTo(b.B) : -a.B.CompareTo(b.B)),
					4 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.C.CompareTo(b.C) : -a.C.CompareTo(b.C)),
					5 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.D.CompareTo(b.D) : -a.D.CompareTo(b.D)),
					6 => () => _cratesVisualization.Sort((a, b) => sortAscending ? a.E.CompareTo(b.E) : -a.E.CompareTo(b.E)),
					7 => () => _cratesVisualization.Sort((a, b) =>
					{
						int result = a.F.CompareTo(b.F);
						if (result == 0)
							result = a.G.CompareTo(b.G);
						if (result == 0)
							result = a.H.CompareTo(b.H);
						if (result == 0)
							result = a.I.CompareTo(b.I);
						if (result == 0)
							result = a.J.CompareTo(b.J);
						if (result == 0)
							result = a.K.CompareTo(b.K);
						if (result == 0)
							result = a.L.CompareTo(b.L);

						return sortAscending ? result : -result;
					}),
					_ => static () => { },
				};
				sortAction();

				sortsSpecs.SpecsDirty = false;
			}

			foreach (Crate crate in _cratesVisualization)
			{
				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text(Inline.Span(crate.Position));

				ImGui.TableNextColumn();
				ImGui.Text(Inline.Span(crate.A));

				ImGui.TableNextColumn();
				ImGui.Text(Inline.Span($"{crate.RotationX}, {crate.RotationY}, {crate.RotationZ}"));

				ImGui.TableNextColumn();
				ImGui.Text(Inline.Span(crate.B));

				ImGui.TableNextColumn();
				ImGui.Text(Inline.Span(crate.C));

				ImGui.TableNextColumn();
				ImGui.Text(Inline.Span(crate.D));

				ImGui.TableNextColumn();
				ImGui.Text(Inline.Span(crate.E));

				ImGui.TableNextColumn();
				ImGui.Text(Inline.Span($"{crate.F}, {crate.G}, {crate.H}, {crate.I}, {crate.J}, {crate.K}, {crate.L}"));
			}

			ImGui.EndTable();
		}
	}
}
