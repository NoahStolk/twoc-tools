using Detach.Extensions;
using Detach.Numerics;
using Silk.NET.OpenGL;
using System.Numerics;
using TwocTools.App.Extensions;
using TwocTools.App.State;
using TwocTools.Core.DataTypes.Crt;
using TwocTools.Core.DataTypes.Wmp;

namespace TwocTools.App.Rendering;

public sealed class LineRenderer
{
	private readonly uint _lineVao;
	private readonly uint _centeredLineVao;

	private readonly Vector3[] _cubeVertices;
	private readonly uint _cubeVao;

	private readonly Vector3[] _sphereVertices;
	private readonly uint _sphereVao;

	private readonly GL _gl;
	private readonly LevelState _levelState;
	private readonly Camera3d _camera3d;

	private readonly Shader _lineShader;
	private readonly int _modelUniform;
	private readonly int _colorUniform;

	public LineRenderer(GL gl, LevelState levelState, Camera3d camera3d, ShaderLoader shaderLoader)
	{
		_gl = gl;
		_levelState = levelState;
		_camera3d = camera3d;
		string vertexCode = File.ReadAllText(Path.Combine("Content", "Shaders", "Line.vert"));
		string fragmentCode = File.ReadAllText(Path.Combine("Content", "Shaders", "Line.frag"));

		_lineShader = new Shader(shaderLoader.Load(vertexCode, fragmentCode));
		_modelUniform = _lineShader.GetUniformLocation(_gl, "model");
		_colorUniform = _lineShader.GetUniformLocation(_gl, "color");

		_lineVao = VaoUtils.CreateLineVao(_gl, [Vector3.Zero, Vector3.UnitZ]);
		_centeredLineVao = VaoUtils.CreateLineVao(_gl, [-Vector3.UnitZ, Vector3.UnitZ]);

		_cubeVertices = VertexUtils.GetCubeVertexPositions();
		_cubeVao = VaoUtils.CreateLineVao(_gl, _cubeVertices);

		_sphereVertices = VertexUtils.GetSphereVertexPositions(4, 8, 1);
		_sphereVao = VaoUtils.CreateLineVao(_gl, _sphereVertices);
	}

	public void Render()
	{
		_gl.UseProgram(_lineShader.Id);

		_gl.UniformMatrix4x4(_lineShader.GetUniformLocation(_gl, "view"), _camera3d.ViewMatrix);
		_gl.UniformMatrix4x4(_lineShader.GetUniformLocation(_gl, "projection"), _camera3d.Projection);

		_gl.BindVertexArray(_lineVao);
		RenderOrigin();
		RenderGrid(_camera3d.Position.Round(0) with { Y = 0 }, new Vector4(1, 1, 1, 0.25f), 64, 1);

		_gl.BindVertexArray(_centeredLineVao);
		_gl.LineWidth(1);
		RenderFocusAxes();

		_gl.BindVertexArray(_cubeVao);
		_gl.LineWidth(4);
		RenderCrates();

		_gl.BindVertexArray(_sphereVao);
		_gl.LineWidth(1);
		RenderWumpa();
	}

	private void RenderLine(Matrix4x4 modelMatrix, Vector4 color)
	{
		_gl.UniformMatrix4x4(_modelUniform, modelMatrix);
		_gl.Uniform4(_colorUniform, color);
		_gl.DrawArrays(PrimitiveType.Lines, 0, 2);
	}

	private void RenderOrigin()
	{
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(1, 1, 256);

		_gl.LineWidth(4);
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2), new Vector4(1, 0, 0, 1));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 1.5f), new Vector4(0, 1, 0, 1));
		RenderLine(scaleMatrix, new Vector4(0, 0, 1, 1));

		_gl.LineWidth(2);
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, -MathF.PI / 2), new Vector4(1, 0, 0, 0.5f));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2), new Vector4(0, 1, 0, 0.5f));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI), new Vector4(0, 0, 1, 0.5f));
	}

	private void RenderGrid(Vector3 origin, Vector4 color, int cellCount, int cellSize)
	{
		_gl.Uniform4(_colorUniform, color);
		_gl.LineWidth(1);

		int min = -cellCount;
		int max = cellCount;
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(new Vector3(1, 1, (max - min) * cellSize));
		Vector3 offset = new(MathF.Round(origin.X), 0, MathF.Round(origin.Z));
		offset.X = MathF.Round(offset.X / cellSize) * cellSize;
		offset.Z = MathF.Round(offset.Z / cellSize) * cellSize;

		for (int i = min; i <= max; i++)
		{
			// Prevent rendering grid lines on top of origin lines (Z-fighting).
			if (!origin.Y.IsZero() || !(i * cellSize + offset.X).IsZero())
			{
				_gl.UniformMatrix4x4(_modelUniform, scaleMatrix * Matrix4x4.CreateTranslation(new Vector3(i * cellSize, origin.Y, min * cellSize) + offset));
				_gl.DrawArrays(PrimitiveType.Lines, 0, 2);
			}

			if (!origin.Y.IsZero() || !(i * cellSize + offset.Z).IsZero())
			{
				_gl.UniformMatrix4x4(_modelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * Matrix4x4.CreateTranslation(new Vector3(min * cellSize, origin.Y, i * cellSize) + offset));
				_gl.DrawArrays(PrimitiveType.Lines, 0, 2);
			}
		}
	}

	private void RenderFocusAxes()
	{
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(1, 1, 128);
		Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(_camera3d.FocusPointTarget);

		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * translationMatrix, new Vector4(1, 0.5f, 0, 0.5f));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 1.5f) * translationMatrix, new Vector4(0, 1, 0.5f, 0.5f));
		RenderLine(scaleMatrix * translationMatrix, new Vector4(0.5f, 0, 1, 0.5f));
	}

	private void RenderCrates()
	{
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(0.5f);
		foreach (CrateGroup crateGroup in _levelState.CrateGroupCollection)
		{
			for (int i = 0; i < crateGroup.Count; i++)
			{
				Crate crate = crateGroup[i];

				Matrix4x4 rotationMatrix = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, crateGroup.TiltInRadians);

				_gl.UniformMatrix4x4(_modelUniform, scaleMatrix * rotationMatrix * Matrix4x4.CreateTranslation(crate.WorldPosition * new Vector3(-1, 1, 1)));
				_gl.Uniform4(_colorUniform, crate.CrateTypeA.GetColor());
				_gl.DrawArrays(PrimitiveType.Lines, 0, (uint)_cubeVertices.Length);
			}
		}
	}

	private void RenderWumpa()
	{
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(0.25f);
		for (int i = 0; i < _levelState.WumpaCollection.Count; i++)
		{
			Wumpa wumpa = _levelState.WumpaCollection[i];

			_gl.UniformMatrix4x4(_modelUniform, scaleMatrix * Matrix4x4.CreateTranslation(wumpa.Position * new Vector3(-1, 1, 1)));
			_gl.Uniform4(_colorUniform, Rgba.Orange with { A = 160 });
			_gl.DrawArrays(PrimitiveType.Lines, 0, (uint)_sphereVertices.Length);
		}
	}
}
