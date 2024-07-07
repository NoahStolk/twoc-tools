using ImGuiNET;
using System.Numerics;
using TwocTools.App.Rendering;

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
		}

		ImGui.End();
	}
}
