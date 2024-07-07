using Detach;
using Detach.Collisions;
using Detach.Collisions.Primitives3D;
using Detach.Numerics;
using ImGuiNET;
using System.Numerics;
using TwocTools.App.Rendering;
using TwocTools.App.State;
using TwocTools.Core.DataTypes;

namespace TwocTools.App.Ui;

public static class SceneWindow
{
	public static void Render()
	{
		if (ImGui.Begin("Scene"))
		{
			Vector2 framebufferSize = ImGui.GetContentRegionAvail();

			SceneFramebuffer.Initialize(framebufferSize);
			Camera3d.AspectRatio = framebufferSize.X / framebufferSize.Y;

			SceneFramebuffer.RenderFramebuffer(framebufferSize);

			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			drawList.AddImage((IntPtr)SceneFramebuffer.FramebufferTextureId, cursorScreenPos, cursorScreenPos + framebufferSize, Vector2.UnitY, Vector2.UnitX);

			Vector2 cursorPosition = ImGui.GetCursorPos();

			ImGui.SetCursorPos(cursorPosition);
			ImGui.InvisibleButton("3d_view", framebufferSize);
			bool isFocused = ImGui.IsItemHovered();
			Camera3d.Update(ImGui.GetIO().DeltaTime, isFocused);

			PerformRaycast(cursorScreenPos, framebufferSize);
		}

		ImGui.End();
	}

	private static void PerformRaycast(Vector2 cursorScreenPos, Vector2 framebufferSize)
	{
		Matrix4x4 viewProjection = Camera3d.ViewMatrix * Camera3d.Projection;
		Vector2 mousePosition = Input.GlfwInput.CursorPosition - cursorScreenPos;

		Vector2 normalizedMousePosition = new Vector2(mousePosition.X / framebufferSize.X - 0.5f, -(mousePosition.Y / framebufferSize.Y - 0.5f)) * 2;

		Plane farPlane = new(viewProjection.M13 - viewProjection.M14, viewProjection.M23 - viewProjection.M24, viewProjection.M33 - viewProjection.M34, viewProjection.M43 - viewProjection.M44);
		Vector3 rayStartPosition = Camera3d.Position;
		Vector3 rayEndPosition = Camera3d.GetMouseWorldPosition(normalizedMousePosition, farPlane);
		Vector3 rayDirection = Vector3.Normalize(rayEndPosition - rayStartPosition);
		Ray ray = new(rayStartPosition, rayDirection);

		float closestDistance = float.MaxValue;
		Crate? selectedCrate = null;
		foreach (CrateGroup crateGroup in LevelState.CrateGroupCollection)
		{
			for (int i = 0; i < crateGroup.Count; i++)
			{
				Crate crate = crateGroup[i];

				Matrix3 rotation = Matrix3.RotationY(crateGroup.TiltInRadians);
				Obb obb = new(crate.WorldPosition * new Vector3(-1, 1, 1), new Vector3(0.25f), rotation);

				if (Geometry3D.Raycast(obb, ray, out RaycastResult raycastResult) && raycastResult.Distance < closestDistance)
				{
					closestDistance = raycastResult.Distance;
					selectedCrate = crate;
				}
			}
		}

		if (selectedCrate.HasValue)
		{
			ReadOnlySpan<char> tooltip = Inline.Span($"""
				Index: {selectedCrate.Value.Index}
				Group index: {selectedCrate.Value.GroupIndex}
				World position: {selectedCrate.Value.WorldPosition.X} {selectedCrate.Value.WorldPosition.Y} {selectedCrate.Value.WorldPosition.Z}
				Local position: {selectedCrate.Value.LocalPositionX} {selectedCrate.Value.LocalPositionY} {selectedCrate.Value.LocalPositionZ}
				Crate types: {selectedCrate.Value.CrateTypeA} {selectedCrate.Value.CrateTypeB} {selectedCrate.Value.CrateTypeC} {selectedCrate.Value.CrateTypeD}
				Exclamation crate index: {selectedCrate.Value.ExclamationCrateIndex}
				Unknown: {selectedCrate.Value.A} {selectedCrate.Value.F} {selectedCrate.Value.G} {selectedCrate.Value.H} {selectedCrate.Value.I} {selectedCrate.Value.J} {selectedCrate.Value.K}
				""");
			ImGui.SetTooltip(tooltip);
		}
	}
}
