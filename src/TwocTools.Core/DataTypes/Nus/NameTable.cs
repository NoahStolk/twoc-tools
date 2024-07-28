namespace TwocTools.Core.DataTypes.Nus;

public sealed class NameTable
{
	public uint Size;
	public IReadOnlyList<string> Names;

	public NameTable(uint size, IReadOnlyList<string> names)
	{
		Size = size;
		Names = names;
	}
}
