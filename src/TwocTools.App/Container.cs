using ImGuiGlfw;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using StrongInject;
using TwocTools.App.Extensions;
using TwocTools.App.Rendering;
using TwocTools.App.State;
using TwocTools.App.Ui;
using TwocTools.App.Utils;

namespace TwocTools.App;

[Register<GlfwInput>(Scope.SingleInstance)]
[Register<Application>(Scope.SingleInstance)]

[Register<CrateInfoWindow>(Scope.SingleInstance)]
[Register<GameSelectWindow>(Scope.SingleInstance)]
[Register<LevelSelectWindow>(Scope.SingleInstance)]
[Register<SceneWindow>(Scope.SingleInstance)]
[Register<WumpaInfoWindow>(Scope.SingleInstance)]

[Register<GameState>(Scope.SingleInstance)]
[Register<LevelState>(Scope.SingleInstance)]

[Register<Camera3d>(Scope.SingleInstance)]
[Register<LineRenderer>(Scope.SingleInstance)]
[Register<SceneFramebuffer>(Scope.SingleInstance)]
[Register<ShaderLoader>(Scope.SingleInstance)]
#pragma warning disable S3881 // "IDisposable" should be implemented correctly. The source generator already implements IDisposable correctly.
public sealed partial class Container : IContainer<Application>
#pragma warning restore S3881
{
	[Factory(Scope.SingleInstance)]
	private static Glfw GetGlfw()
	{
		Glfw glfw = Glfw.GetApi();
		glfw.Init();
		glfw.CheckError();

		glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
		glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
		glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
		glfw.WindowHint(WindowHintBool.Focused, true);
		glfw.WindowHint(WindowHintBool.Resizable, true);
		glfw.CheckError();

		return glfw;
	}

	[Factory(Scope.SingleInstance)]
	private static unsafe WindowHandle* CreateWindow(Glfw glfw, GlfwInput glfwInput)
	{
		WindowHandle* window = glfw.CreateWindow(WindowConstants.WindowWidth, WindowConstants.WindowHeight, WindowConstants.WindowTitle, null, null);
		glfw.CheckError();
		if (window == null)
			throw new InvalidOperationException("Could not create window.");

		glfw.SetCursorPosCallback(window, (_, x, y) => glfwInput.CursorPosCallback(x, y));
		glfw.SetScrollCallback(window, (_, _, y) => glfwInput.MouseWheelCallback(y));
		glfw.SetMouseButtonCallback(window, (_, button, state, _) => glfwInput.MouseButtonCallback(button, state));
		glfw.SetKeyCallback(window, (_, keys, _, state, _) => glfwInput.KeyCallback(keys, state));
		glfw.SetCharCallback(window, (_, codepoint) => glfwInput.CharCallback(codepoint));

		(int windowX, int windowY) = glfw.GetInitialWindowPos(WindowConstants.WindowWidth, WindowConstants.WindowHeight);
		glfw.SetWindowPos(window, windowX, windowY);

		glfw.MakeContextCurrent(window);
		glfw.SetWindowSizeLimits(window, 1024, 768, -1, -1);

		return window;
	}

	[Factory(Scope.SingleInstance)]
	private static GL GetGl(Glfw glfw)
	{
		return GL.GetApi(glfw.GetProcAddress);
	}

	[Factory(Scope.SingleInstance)]
	private static ImGuiController CreateImGuiController(GL gl, GlfwInput glfwInput)
	{
		ImGuiController imGuiController = new(gl, glfwInput, WindowConstants.WindowWidth, WindowConstants.WindowHeight);
		imGuiController.CreateDefaultFont();
		return imGuiController;
	}
}
