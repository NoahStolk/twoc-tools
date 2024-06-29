using System.Numerics;

namespace TwocTools.Core.Internals.Extensions;

public static class BinaryReaderExtensions
{
	public static Vector3 ReadVector3(this BinaryReader binaryReader)
	{
		return new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
	}
}
