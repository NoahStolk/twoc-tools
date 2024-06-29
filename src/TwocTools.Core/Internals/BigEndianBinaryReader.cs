using System.Buffers.Binary;
using System.Numerics;
using System.Text;

namespace TwocTools.Core.Internals;

internal sealed class BigEndianBinaryReader : BinaryReader
{
	public BigEndianBinaryReader(Stream input)
		: base(input)
	{
	}

	public BigEndianBinaryReader(Stream input, Encoding encoding)
		: base(input, encoding)
	{
	}

	public BigEndianBinaryReader(Stream input, Encoding encoding, bool leaveOpen)
		: base(input, encoding, leaveOpen)
	{
	}

	public override short ReadInt16()
	{
		Span<byte> bytes = stackalloc byte[sizeof(short)];
		BaseStream.ReadExactly(bytes);
		return BinaryPrimitives.ReadInt16BigEndian(bytes);
	}

	public override int ReadInt32()
	{
		Span<byte> bytes = stackalloc byte[sizeof(int)];
		BaseStream.ReadExactly(bytes);
		return BinaryPrimitives.ReadInt32BigEndian(bytes);
	}

	public override long ReadInt64()
	{
		Span<byte> bytes = stackalloc byte[sizeof(long)];
		BaseStream.ReadExactly(bytes);
		return BinaryPrimitives.ReadInt64BigEndian(bytes);
	}

	public override ushort ReadUInt16()
	{
		Span<byte> bytes = stackalloc byte[sizeof(ushort)];
		BaseStream.ReadExactly(bytes);
		return BinaryPrimitives.ReadUInt16BigEndian(bytes);
	}

	public override uint ReadUInt32()
	{
		Span<byte> bytes = stackalloc byte[sizeof(uint)];
		BaseStream.ReadExactly(bytes);
		return BinaryPrimitives.ReadUInt32BigEndian(bytes);
	}

	public override ulong ReadUInt64()
	{
		Span<byte> bytes = stackalloc byte[sizeof(ulong)];
		BaseStream.ReadExactly(bytes);
		return BinaryPrimitives.ReadUInt64BigEndian(bytes);
	}

	public override float ReadSingle()
	{
		Span<byte> bytes = stackalloc byte[sizeof(float)];
		BaseStream.ReadExactly(bytes);
		return BinaryPrimitives.ReadSingleBigEndian(bytes);
	}

	public override double ReadDouble()
	{
		Span<byte> bytes = stackalloc byte[sizeof(double)];
		BaseStream.ReadExactly(bytes);
		return BinaryPrimitives.ReadDoubleBigEndian(bytes);
	}

	public Vector3 ReadVector3()
	{
		return new Vector3(ReadSingle(), ReadSingle(), ReadSingle());
	}
}
