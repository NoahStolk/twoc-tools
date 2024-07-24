using ImGuiGlfw;
using ImGuiNET;
using Silk.NET.OpenGL;
using TwocTools.App.State;
using TwocTools.App.Ui;

namespace TwocTools.App;

public sealed class Application
{
	private const float _maxMainDelta = 0.25f;

	private const double _updateRate = 60;
	private const double _mainLoopRate = 300;

	private const double _updateLength = 1 / _updateRate;
	private const double _mainLoopLength = 1 / _mainLoopRate;

	private readonly ImGuiController _imGuiController;

	private static Application? _instance;

	private double _currentTime = Graphics.Glfw.GetTime();
	private double _accumulator;
	private double _frameTime;

	public Application(ImGuiController imGuiController)
	{
		_imGuiController = imGuiController;
	}

	public float FrameTime => (float)_frameTime;

	public static Application Instance
	{
		get => _instance ?? throw new InvalidOperationException("Application instance not set.");
		set
		{
			if (_instance != null)
				throw new InvalidOperationException("Application instance already set.");

			_instance = value;
		}
	}

	public unsafe void Run()
	{
		while (!Graphics.Glfw.WindowShouldClose(Graphics.Window))
		{
			double expectedNextFrame = Graphics.Glfw.GetTime() + _mainLoopLength;
			MainLoop();

			while (Graphics.Glfw.GetTime() < expectedNextFrame)
				Thread.Yield();
		}

		_imGuiController.Destroy();
		Graphics.Glfw.Terminate();
	}

	private unsafe void MainLoop()
	{
		double mainStartTime = Graphics.Glfw.GetTime();
		_frameTime = mainStartTime - _currentTime;
		if (_frameTime > _maxMainDelta)
			_frameTime = _maxMainDelta;

		_currentTime = mainStartTime;
		_accumulator += _frameTime;

		Graphics.Glfw.PollEvents();

		while (_accumulator >= _updateLength)
			_accumulator -= _updateLength;

		Render();

		Graphics.Glfw.SwapBuffers(Graphics.Window);
	}

	private void Render()
	{
		_imGuiController.Update((float)_frameTime);

		ImGui.DockSpaceOverViewport(null, ImGuiDockNodeFlags.PassthruCentralNode);

		Graphics.Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		if (GameState.IsValid)
		{
			LevelSelectWindow.Render();
			CrateInfoWindow.Render();
			WumpaInfoWindow.Render();
			SceneWindow.Render();
		}
		else
		{
			GameSelectWindow.Render();
		}

		_imGuiController.Render();

		Input.GlfwInput.PostRender();
	}
}
