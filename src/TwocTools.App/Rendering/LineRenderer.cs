﻿using Silk.NET.OpenGL;
using System.Numerics;
using TwocTools.App.Extensions;
using static TwocTools.App.Graphics;

namespace TwocTools.App.Rendering;

public sealed class LineRenderer
{
	private static readonly uint _lineVao = VaoUtils.CreateLineVao([Vector3.Zero, Vector3.UnitZ]);
	private static readonly uint _centeredLineVao = VaoUtils.CreateLineVao([-Vector3.UnitZ, Vector3.UnitZ]);

	private static readonly uint _cubeVao = VaoUtils.CreateLineVao([
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),

		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),

		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),
	]);

	private static readonly Vector3[] _sphereVertices = VertexUtils.GetSphereVertexPositions(8, 16, 1);
	private static readonly uint _sphereVao = VaoUtils.CreateLineVao(_sphereVertices);

	private static readonly Vector3[] _pointVertices = VertexUtils.GetSphereVertexPositions(3, 6, 1);
	private static readonly uint _pointVao = VaoUtils.CreateLineVao(_pointVertices);

	private static readonly uint _planeLineVao = VaoUtils.CreateLineVao([
		new Vector3(-0.5f, -0.5f, 0),
		new Vector3(-0.5f, 0.5f, 0),
		new Vector3(0.5f, 0.5f, 0),
		new Vector3(0.5f, -0.5f, 0),
		new Vector3(-0.5f, -0.5f, 0),
	]);
	private static readonly uint[] _planeLineIndices = [0, 1, 1, 2, 2, 3, 3, 0];

	private readonly Shader _lineShader;
	private readonly int _modelUniform;
	private readonly int _colorUniform;

	public LineRenderer()
	{
		string vertexCode = File.ReadAllText(Path.Combine("Content", "Shaders", "Line.vert"));
		string fragmentCode = File.ReadAllText(Path.Combine("Content", "Shaders", "Line.frag"));

		_lineShader = new Shader(ShaderLoader.Load(vertexCode, fragmentCode));
		_modelUniform = _lineShader.GetUniformLocation("model");
		_colorUniform = _lineShader.GetUniformLocation("color");
	}

	public void Render()
	{
		Gl.UseProgram(_lineShader.Id);

		Gl.UniformMatrix4x4(_lineShader.GetUniformLocation("view"), Camera3d.ViewMatrix);
		Gl.UniformMatrix4x4(_lineShader.GetUniformLocation("projection"), Camera3d.Projection);

		Gl.BindVertexArray(_lineVao);
		RenderOrigin();
		RenderGrid(Vector3.Zero, new Vector4(1, 1, 1, 0.25f), 64, 1);

		Gl.BindVertexArray(_centeredLineVao);
		RenderFocusAxes();
	}

	private void RenderLine(Matrix4x4 modelMatrix, Vector4 color)
	{
		Gl.UniformMatrix4x4(_modelUniform, modelMatrix);
		Gl.Uniform4(_colorUniform, color);
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
	}

	private void RenderOrigin()
	{
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(1, 1, 256);

		Gl.LineWidth(4);
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2), new Vector4(1, 0, 0, 1));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 1.5f), new Vector4(0, 1, 0, 1));
		RenderLine(scaleMatrix, new Vector4(0, 0, 1, 1));

		Gl.LineWidth(2);
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, -MathF.PI / 2), new Vector4(1, 0, 0, 0.5f));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2), new Vector4(0, 1, 0, 0.5f));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI), new Vector4(0, 0, 1, 0.5f));
	}

	private void RenderFocusAxes()
	{
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(1, 1, 128);
		Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(Camera3d.FocusPointTarget);

		Gl.LineWidth(1);
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * translationMatrix, new Vector4(1, 0.5f, 0, 0.5f));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 1.5f) * translationMatrix, new Vector4(0, 1, 0.5f, 0.5f));
		RenderLine(scaleMatrix * translationMatrix, new Vector4(0.5f, 0, 1, 0.5f));
	}

	private void RenderGrid(Vector3 origin, Vector4 color, int cellCount, int cellSize)
	{
		Gl.Uniform4(_colorUniform, color);
		Gl.LineWidth(1);

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
				Gl.UniformMatrix4x4(_modelUniform, scaleMatrix * Matrix4x4.CreateTranslation(new Vector3(i * cellSize, origin.Y, min * cellSize) + offset));
				Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
			}

			if (!origin.Y.IsZero() || !(i * cellSize + offset.Z).IsZero())
			{
				Gl.UniformMatrix4x4(_modelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * Matrix4x4.CreateTranslation(new Vector3(min * cellSize, origin.Y, i * cellSize) + offset));
				Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
			}
		}
	}
}
