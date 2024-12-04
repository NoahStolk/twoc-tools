using Silk.NET.OpenGL;

namespace TwocTools.App.Rendering;

internal sealed class Shader(uint id)
{
	private readonly Dictionary<string, int> _uniformLocations = new();

	public uint Id { get; } = id;

	public int GetUniformLocation(GL gl, string name)
	{
		if (_uniformLocations.TryGetValue(name, out int location))
			return location;

		location = gl.GetUniformLocation(Id, name);
		_uniformLocations.Add(name, location);

		return location;
	}
}
