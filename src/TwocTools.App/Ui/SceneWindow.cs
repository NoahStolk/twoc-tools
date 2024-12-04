using Detach;
using Detach.Collisions;
using Detach.Collisions.Primitives3D;
using Detach.GlfwExtensions;
using Detach.Numerics;
using Hexa.NET.ImGui;
using System.Numerics;
using TwocTools.App.Extensions;
using TwocTools.App.Rendering;
using TwocTools.App.State;
using TwocTools.Core.DataTypes.Crt;

namespace TwocTools.App.Ui;

internal sealed class SceneWindow(LevelState levelState, SceneFramebuffer sceneFramebuffer, GlfwInput glfwInput, Camera3d camera3d)
{
	public void Render()
	{
		if (ImGui.Begin("Scene"))
		{
			Vector2 framebufferSize = ImGui.GetContentRegionAvail();

			if (framebufferSize is { X: > 0, Y: > 0 })
			{
				sceneFramebuffer.Initialize(framebufferSize);
				camera3d.AspectRatio = framebufferSize.X / framebufferSize.Y;

				sceneFramebuffer.RenderFramebuffer(framebufferSize);

				ImDrawListPtr drawList = ImGui.GetWindowDrawList();
				Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
				drawList.AddImage(sceneFramebuffer.FramebufferTextureId, cursorScreenPos, cursorScreenPos + framebufferSize, Vector2.UnitY, Vector2.UnitX);

				Vector2 cursorPosition = ImGui.GetCursorPos();

				ImGui.SetCursorPos(cursorPosition);
				ImGui.InvisibleButton("3d_view", framebufferSize);
				bool isFocused = ImGui.IsItemHovered();
				camera3d.Update(ImGui.GetIO().DeltaTime, isFocused);

				PerformRaycast(cursorScreenPos, framebufferSize);
			}
		}

		ImGui.End();
	}

	private void PerformRaycast(Vector2 cursorScreenPos, Vector2 framebufferSize)
	{
		Matrix4x4 viewProjection = camera3d.ViewMatrix * camera3d.Projection;
		Vector2 mousePosition = glfwInput.CursorPosition - cursorScreenPos;

		Vector2 normalizedMousePosition = new Vector2(mousePosition.X / framebufferSize.X - 0.5f, -(mousePosition.Y / framebufferSize.Y - 0.5f)) * 2;

		Plane farPlane = new(viewProjection.M13 - viewProjection.M14, viewProjection.M23 - viewProjection.M24, viewProjection.M33 - viewProjection.M34, viewProjection.M43 - viewProjection.M44);
		Vector3 rayStartPosition = camera3d.Position;
		Vector3 rayEndPosition = camera3d.GetMouseWorldPosition(normalizedMousePosition, farPlane);
		Vector3 rayDirection = Vector3.Normalize(rayEndPosition - rayStartPosition);
		Ray ray = new(rayStartPosition, rayDirection);

		float closestDistance = float.MaxValue;
		Crate? selectedCrate = null;
		foreach (CrateGroup crateGroup in levelState.CrateGroupCollection)
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
			ReadOnlySpan<byte> tooltip = Inline.Utf8($"""
				Index: {selectedCrate.Value.Index}
				Group index: {selectedCrate.Value.GroupIndex}
				World position: {selectedCrate.Value.WorldPosition.X} {selectedCrate.Value.WorldPosition.Y} {selectedCrate.Value.WorldPosition.Z}
				Local position: {selectedCrate.Value.LocalPositionX} {selectedCrate.Value.LocalPositionY} {selectedCrate.Value.LocalPositionZ}
				Crate types: {selectedCrate.Value.CrateTypeA.ToUtf8Span()} {selectedCrate.Value.CrateTypeB.ToUtf8Span()} {selectedCrate.Value.CrateTypeC.ToUtf8Span()} {selectedCrate.Value.CrateTypeD.ToUtf8Span()}
				Exclamation crate index: {selectedCrate.Value.ExclamationCrateIndex}
				Neighboring crate indices:
					Up:       {selectedCrate.Value.F}
					Down:     {selectedCrate.Value.G}
					Forward:  {selectedCrate.Value.H}
					Backward: {selectedCrate.Value.I}
					Left:     {selectedCrate.Value.J}
					Right:    {selectedCrate.Value.K}
				""");
			ImGui.SetTooltip(tooltip);
		}
	}
}
