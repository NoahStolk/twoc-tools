using ImGuiGlfw;
using ImGuiNET;
using System.Numerics;
using TwocTools.App;

const int windowWidth = 1920;
const int windowHeight = 1080;
const int windowWidthMin = 1024;
const int windowHeightMin = 768;
const int windowWidthMax = 4096;
const int windowHeightMax = 2160;

Graphics.CreateWindow(new Graphics.WindowState("twoc-tools", windowWidth, windowHeight, false));
Graphics.SetWindowSizeLimits(windowWidthMin, windowHeightMin, windowWidthMax, windowHeightMax);

ImGuiController imGuiController = new(Graphics.Gl, Input.GlfwInput, windowWidth, windowHeight);
imGuiController.CreateDefaultFont();

ImGuiStylePtr style = ImGui.GetStyle();
style.WindowPadding = new Vector2(4, 4);
style.ItemSpacing = new Vector2(4, 4);

Graphics.OnChangeWindowSize = (w, h) =>
{
	Graphics.Gl.Viewport(0, 0, (uint)w, (uint)h);
	imGuiController.WindowResized(w, h);
};

Application application = new(imGuiController);
Application.Instance = application;
application.Run();
