using System.Numerics;

namespace TwocTools.Core.Extensions;

internal static class BinaryReaderExtensions
{
	public static Vector3 ReadVector3(this BinaryReader br)
	{
		return new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
	}
}
