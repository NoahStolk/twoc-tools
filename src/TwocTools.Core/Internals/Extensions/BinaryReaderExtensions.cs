using System.Numerics;
using System.Text;

namespace TwocTools.Core.Internals.Extensions;

public static class BinaryReaderExtensions
{
	public static Vector3 ReadVector3(this BinaryReader binaryReader)
	{
		return new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
	}

	public static string ReadNullTerminatedString(this BinaryReader br)
	{
		StringBuilder sb = new();
		char c;
		while ((c = br.ReadChar()) != 0)
			sb.Append(c);
		return sb.ToString();
	}
}
