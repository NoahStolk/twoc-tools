using Silk.NET.OpenGL;

namespace TwocTools.App.Rendering;

public sealed class Shader
{
	private readonly Dictionary<string, int> _uniformLocations = new();

	public Shader(uint id)
	{
		Id = id;
	}

	public uint Id { get; }

	public int GetUniformLocation(GL gl, string name)
	{
		if (_uniformLocations.TryGetValue(name, out int location))
			return location;

		location = gl.GetUniformLocation(Id, name);
		_uniformLocations.Add(name, location);

		return location;
	}
}
