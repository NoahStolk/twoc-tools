using ImGuiGlfw;
using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using TwocTools.App.State;
using TwocTools.App.Ui;
using TwocTools.App.Utils;

namespace TwocTools.App;

public sealed class Application
{
	private const float _maxMainDelta = 0.25f;

	private const double _updateRate = 60;
	private const double _mainLoopRate = 300;

	private const double _updateLength = 1 / _updateRate;
	private const double _mainLoopLength = 1 / _mainLoopRate;

	private readonly Glfw _glfw;
	private readonly GL _gl;
	private readonly unsafe WindowHandle* _window;
	private readonly GlfwInput _glfwInput;
	private readonly ImGuiController _imGuiController;
	private readonly GameState _gameState;
	private readonly CrateInfoWindow _crateInfoWindow;
	private readonly GameSelectWindow _gameSelectWindow;
	private readonly LevelSelectWindow _levelSelectWindow;
	private readonly SceneWindow _sceneWindow;
	private readonly WumpaInfoWindow _wumpaInfoWindow;

	private double _currentTime;
	private double _accumulator;
	private double _frameTime;

	public unsafe Application(
		Glfw glfw,
		GL gl,
		WindowHandle* window,
		GlfwInput glfwInput,
		ImGuiController imGuiController,
		GameState gameState,
		CrateInfoWindow crateInfoWindow,
		GameSelectWindow gameSelectWindow,
		LevelSelectWindow levelSelectWindow,
		SceneWindow sceneWindow,
		WumpaInfoWindow wumpaInfoWindow)
	{
		_glfw = glfw;
		_gl = gl;
		_window = window;
		_glfwInput = glfwInput;
		_imGuiController = imGuiController;
		_gameState = gameState;
		_crateInfoWindow = crateInfoWindow;
		_gameSelectWindow = gameSelectWindow;
		_levelSelectWindow = levelSelectWindow;
		_sceneWindow = sceneWindow;
		_wumpaInfoWindow = wumpaInfoWindow;

		_currentTime = glfw.GetTime();

		gl.Viewport(0, 0, WindowConstants.WindowWidth, WindowConstants.WindowHeight);
		glfw.SwapInterval(0); // Turns VSync off.

		glfw.SetFramebufferSizeCallback(window, (_, w, h) =>
		{
			gl.Viewport(0, 0, (uint)w, (uint)h);
			imGuiController.WindowResized(w, h);
		});
	}

	public float FrameTime => (float)_frameTime;

	public unsafe void Run()
	{
		while (!_glfw.WindowShouldClose(_window))
		{
			double expectedNextFrame = _glfw.GetTime() + _mainLoopLength;
			MainLoop();

			while (_glfw.GetTime() < expectedNextFrame)
				Thread.Yield();
		}

		_imGuiController.Destroy();
		_glfw.Terminate();
	}

	private unsafe void MainLoop()
	{
		double mainStartTime = _glfw.GetTime();
		_frameTime = mainStartTime - _currentTime;
		if (_frameTime > _maxMainDelta)
			_frameTime = _maxMainDelta;

		_currentTime = mainStartTime;
		_accumulator += _frameTime;

		_glfw.PollEvents();

		while (_accumulator >= _updateLength)
			_accumulator -= _updateLength;

		Render();

		_glfw.SwapBuffers(_window);
	}

	private void Render()
	{
		_imGuiController.Update((float)_frameTime);

		ImGui.DockSpaceOverViewport(0, null, ImGuiDockNodeFlags.PassthruCentralNode);

		_gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		if (_gameState.IsValid)
		{
			_levelSelectWindow.Render();
			_crateInfoWindow.Render();
			_wumpaInfoWindow.Render();
			_sceneWindow.Render();
		}
		else
		{
			_gameSelectWindow.Render();
		}

		_imGuiController.Render();

		_glfwInput.PostRender();
	}
}
