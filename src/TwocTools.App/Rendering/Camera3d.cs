using ImGuiGlfw;
using Silk.NET.GLFW;
using System.Numerics;
using TwocTools.App.Extensions;

namespace TwocTools.App.Rendering;

public sealed unsafe class Camera3d
{
	private const MouseButton _lookButton = MouseButton.Right;
	private const MouseButton _panButton = MouseButton.Middle;

	private const int _fieldOfView = 2;

	private readonly Glfw _glfw;
	private readonly WindowHandle* _windowHandle;
	private readonly GlfwInput _glfwInput;

	private Vector2 _originalCursor;

	private float _yaw = MathF.PI * 0.25f;
	private float _pitch = -0.5f;
	private float _zoom = 5;

	private Vector3 _focusPoint;

	public Camera3d(Glfw glfw, WindowHandle* windowHandle, GlfwInput glfwInput)
	{
		_glfw = glfw;
		_windowHandle = windowHandle;
		_glfwInput = glfwInput;

		_originalCursor = glfwInput.CursorPosition;

		Rotation = Quaternion.CreateFromYawPitchRoll(_yaw, -_pitch, 0);
	}

	public Quaternion Rotation { get; private set; }
	public Vector3 Position { get; private set; }

	public Matrix4x4 Projection { get; private set; }
	public Matrix4x4 ViewMatrix { get; private set; }
	public float AspectRatio { get; set; }
	public CameraMode Mode { get; private set; }
	public Vector3 FocusPointTarget { get; private set; }

	public Vector3 UpDirection => Vector3.Transform(Vector3.UnitY, Rotation);
	public Vector3 LookDirection => Vector3.Transform(Vector3.UnitZ, Rotation);

	private void SetFocusPointHard(Vector3 focusPoint)
	{
		FocusPointTarget = focusPoint;
		_focusPoint = focusPoint;
	}

	public void Update(float dt, bool isFocused)
	{
		if (isFocused)
		{
			HandleMouse();

			float scroll = _glfwInput.MouseWheelY;
			if (!scroll.IsZero() && !_glfwInput.IsKeyDown(Keys.ControlLeft) && !_glfwInput.IsKeyDown(Keys.ControlRight))
				_zoom = Math.Max(_zoom - scroll, 1);

			if (!_glfwInput.IsKeyDown(Keys.ControlLeft) && !_glfwInput.IsKeyDown(Keys.ControlRight))
			{
				const float speed = 15;
				if (_glfwInput.IsKeyDown(Keys.W))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(0, 0, speed), Rotation) * dt);
				if (_glfwInput.IsKeyDown(Keys.S))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(0, 0, -speed), Rotation) * dt);
				if (_glfwInput.IsKeyDown(Keys.A))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(speed, 0, 0), Rotation) * dt);
				if (_glfwInput.IsKeyDown(Keys.D))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(-speed, 0, 0), Rotation) * dt);
				if (_glfwInput.IsKeyDown(Keys.Space))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(0, speed, 0), Rotation) * dt);
				if (_glfwInput.IsKeyDown(Keys.ShiftLeft))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(0, -speed, 0), Rotation) * dt);
			}
		}
		else
		{
			ResetCameraMode();
		}

		_focusPoint = Vector3.Lerp(_focusPoint, FocusPointTarget, dt * 10);
		Position = _focusPoint + Vector3.Transform(new Vector3(0, 0, -_zoom), Rotation);

		ViewMatrix = Matrix4x4.CreateLookAt(Position, Position + LookDirection, UpDirection);

		const float nearPlaneDistance = 0.05f;
		const float farPlaneDistance = 10000f;
		Projection = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4 * _fieldOfView, AspectRatio, nearPlaneDistance, farPlaneDistance);
	}

	private void HandleMouse()
	{
		Vector2 cursor = _glfwInput.CursorPosition;

		if (Mode == CameraMode.None && (_glfwInput.IsMouseButtonDown(_lookButton) || _glfwInput.IsMouseButtonDown(_panButton)))
		{
			_glfw.SetInputMode(_windowHandle, CursorStateAttribute.Cursor, CursorModeValue.CursorHidden);
			_originalCursor = cursor;
			Mode = _glfwInput.IsMouseButtonDown(_lookButton) ? CameraMode.Look : CameraMode.Pan;
		}
		else if (Mode != CameraMode.None && !_glfwInput.IsMouseButtonDown(_lookButton) && !_glfwInput.IsMouseButtonDown(_panButton))
		{
			ResetCameraMode();
		}

		if (Mode == CameraMode.None)
			return;

		Vector2 delta = cursor - _originalCursor;
		if (Mode == CameraMode.Look)
		{
			const float lookSpeed = 20;
			_yaw -= lookSpeed * delta.X * 0.0001f;
			_pitch -= lookSpeed * delta.Y * 0.0001f;
			Rotation = Quaternion.CreateFromYawPitchRoll(_yaw, -_pitch, 0);

			_glfw.SetCursorPos(_windowHandle, _originalCursor.X, _originalCursor.Y);
		}
		else if (Mode == CameraMode.Pan)
		{
			float multiplier = 0.0005f * _zoom;
			SetFocusPointHard(FocusPointTarget - Vector3.Transform(new Vector3(-delta.X * multiplier, -delta.Y * multiplier, 0), Rotation));

			_glfw.SetCursorPos(_windowHandle, _originalCursor.X, _originalCursor.Y);
		}
	}

	private void ResetCameraMode()
	{
		_glfw.SetInputMode(_windowHandle, CursorStateAttribute.Cursor, CursorModeValue.CursorNormal);
		Mode = CameraMode.None;
	}

	public Vector3 GetMouseWorldPosition(Vector2 normalizedMousePosition, Plane plane)
	{
		Vector3 nearSource = new(normalizedMousePosition.X, normalizedMousePosition.Y, 0f);
		Vector3 farSource = new(normalizedMousePosition.X, normalizedMousePosition.Y, 1f);
		Vector3 nearPoint = UnProject(nearSource, Projection, ViewMatrix, Matrix4x4.Identity);
		Vector3 farPoint = UnProject(farSource, Projection, ViewMatrix, Matrix4x4.Identity);

		// Create a ray from the near clip plane to the far clip plane.
		Vector3 rayDirection = Vector3.Normalize(farPoint - nearPoint);

		// Calculate distance of intersection point from ray.Position.
		float denominator = Vector3.Dot(plane.Normal, rayDirection);
		float numerator = Vector3.Dot(plane.Normal, nearPoint) + plane.D;
		float t = -(numerator / denominator);

		// Calculate the picked position on the y = 0 plane.
		return nearPoint + rayDirection * t;
	}

	private static Vector3 UnProject(Vector3 source, Matrix4x4 projection, Matrix4x4 view, Matrix4x4 world)
	{
		Matrix4x4.Invert(Matrix4x4.Multiply(Matrix4x4.Multiply(world, view), projection), out Matrix4x4 matrix);
		Vector3 vector = Vector3.Transform(source, matrix);
		float a = source.X * matrix.M14 + source.Y * matrix.M24 + source.Z * matrix.M34 + matrix.M44;
		if (WithinEpsilon(a, 1f))
			return vector;

		return vector / a;

		static bool WithinEpsilon(float a, float b)
		{
			float num = a - b;
			return num is >= -float.Epsilon and <= float.Epsilon;
		}
	}
}
