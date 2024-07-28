namespace TwocTools.Core.Internals.Rnc;

// From https://moddingwiki.shikadi.net/wiki/Rob_Northern_Compression
public static unsafe class RncMethod1
{
	public static RncStatus ReadRnc(Stream input, Stream output)
	{
		ArgumentNullException.ThrowIfNull(input);
		ArgumentNullException.ThrowIfNull(output);

		byte[] iBytes = new byte[input.Length];

		int bytesRead = input.Read(iBytes, 0, iBytes.Length);
		if (bytesRead != iBytes.Length)
			throw new IOException("Failed to read the entire input stream.");

		uint oLen, iLen, oSum, iSum;

		int method;
		fixed (byte* b = &iBytes[0])
		{
			if (b[0] != 0x52 || b[1] != 0x4E || b[2] != 0x43)
				return RncStatus.FileIsNotRnc;

			method = b[3];
			oLen = ReadU32BigEndian(b + 4);
			iLen = ReadU32BigEndian(b + 8);
			oSum = ReadU16BigEndian(b + 12);
			iSum = ReadU16BigEndian(b + 14);
		}

		byte[] oBytes = new byte[oLen];

		fixed (byte* iBuf = &iBytes[0])
		{
			fixed (byte* oBuf = &oBytes[0])
			{
				return Unpack(iBuf, iLen, iSum, oBuf, oLen, oSum, method);
			}
		}
	}

	private static RncStatus Unpack(byte* iBuf, uint iLen, uint iSum, byte* oBuf, uint oLen, uint oSum, int method)
	{
		HuffmanTable raw = new();
		HuffmanTable dst = new();
		HuffmanTable len = new();

		byte* oEnd = oBuf + oLen;
		byte* iEnd = iBuf + 18 + iLen;

		iBuf += 18;

		if (ComputeChecksum(iBuf, (int)(iEnd - iBuf)) != iSum)
			return RncStatus.PackedCrcError;

		BitStream stream = default;
		BitStreamInit(&stream, iBuf, iEnd);
		BitStreamAdvance(&stream, 2);

		if (method == 1)
		{
			RncStatus status = UnpackDataMethod1(oBuf, oEnd, raw, dst, len, &stream);
			if (status != RncStatus.Ok)
				return status;
		}
		else if (method == 2)
		{
			throw new NotSupportedException("RNC method 2 is not supported.");
		}
		else
		{
			return RncStatus.FileIsNotRnc;
		}

		if (oEnd != oBuf)
			return RncStatus.FileSizeMismatch;

		return ComputeChecksum(oEnd - oLen, (int)oLen) != oSum
			? RncStatus.UnpackedCrcError
			: RncStatus.Ok;
	}

	private static RncStatus UnpackDataMethod1(byte* oBuf, byte* oEnd, HuffmanTable raw, HuffmanTable dst, HuffmanTable len, BitStream* stream)
	{
		while (oBuf < oEnd)
		{
			ReadHuffmanTable(raw, stream);
			ReadHuffmanTable(dst, stream);
			ReadHuffmanTable(len, stream);

			uint chunks = BitStreamRead(stream, 0xFFFF, 16);

			while (true)
			{
				long length = ReadHuffman(raw, stream);
				if (length == -1)
					return RncStatus.HufDecodeError;

				if (length != 0)
				{
					while (length-- != 0)
						*oBuf++ = *stream->DataPos++;

					BitStreamFix(stream);
				}

				if (--chunks <= 0)
					break;

				long pos = ReadHuffman(dst, stream);
				if (pos == -1)
					return RncStatus.HufDecodeError;

				pos++;

				length = ReadHuffman(len, stream);

				if (length == -1)
					return RncStatus.HufDecodeError;

				length += 2;

				for (; length > 0; length--, oBuf++)
					*oBuf = oBuf[-pos];
			}
		}

		return RncStatus.Ok;
	}

	private static void BitStreamInit(BitStream* stream, byte* dataPos, byte* dataEnd)
	{
		stream->BitBuffer = ReadU16LittleEndian(dataPos);
		stream->BitCount = 16;
		stream->DataPos = dataPos;
		stream->DataEnd = dataEnd;
	}

	private static void BitStreamFix(BitStream* stream)
	{
		stream->BitCount -= 16;
		stream->BitBuffer &= (uint)((1 << stream->BitCount) - 1);

		if (stream->DataPos < stream->DataEnd)
		{
			stream->BitBuffer |= ReadU16LittleEndian(stream->DataPos) << stream->BitCount;
			stream->BitCount += 16;
		}
		else if (stream->DataPos == stream->DataEnd)
		{
			stream->BitBuffer |= (uint)(*stream->DataPos << stream->BitCount);
			stream->BitCount += 8;
		}
	}

	private static uint BitStreamPeek(BitStream* stream, uint mask)
	{
		uint peek = stream->BitBuffer & mask;

		return peek;
	}

	private static void BitStreamAdvance(BitStream* stream, int bits)
	{
		stream->BitBuffer >>= bits;
		stream->BitCount -= bits;

		if (stream->BitCount >= 16)
			return;

		stream->DataPos += 2;

		if (stream->DataPos < stream->DataEnd)
		{
			stream->BitBuffer |= ReadU16LittleEndian(stream->DataPos) << stream->BitCount;
			stream->BitCount += 16;
		}
		else if (stream->DataPos == stream->DataEnd)
		{
			stream->BitBuffer |= (uint)(*stream->DataPos << stream->BitCount);
			stream->BitCount += 8;
		}
	}

	private static uint BitStreamRead(BitStream* stream, uint mask, int bits)
	{
		uint peek = BitStreamPeek(stream, mask);

		BitStreamAdvance(stream, bits);

		return peek;
	}

	private static ushort ComputeChecksum(byte* data, int length)
	{
		ushort val = 0;

		while (length-- != 0)
		{
			val = (ushort)(val ^ *data++);
			val = (ushort)((val >> 8) ^ RncCrcTable.Table[val & 0xFF]);
		}

		return val;
	}

	private static uint MirrorBits(uint value, int bits)
	{
		uint top = (uint)(1 << (bits - 1));
		uint bot = 1u;

		while (top > bot)
		{
			uint mask = top | bot;

			uint masked = value & mask;
			if (masked != 0 && masked != mask)
				value ^= mask;

			top >>= 1;
			bot <<= 1;
		}

		return value;
	}

	private static long ReadHuffman(HuffmanTable table, BitStream* stream)
	{
		int i;

		for (i = 0; i < table.Count; i++)
		{
			uint mask = (uint)((1 << table.Leaves[i].CodeLength) - 1);

			if (BitStreamPeek(stream, mask) == table.Leaves[i].Code)
				break;
		}

		if (i == table.Count)
			return -1;

		BitStreamAdvance(stream, table.Leaves[i].CodeLength);

		uint val = (uint)table.Leaves[i].Value;

		if (val < 2)
			return val;

		val = (uint)(1 << (int)(val - 1));

		val |= BitStreamRead(stream, val - 1, table.Leaves[i].Value - 1);

		return val;
	}

	private static void ReadHuffmanTable(HuffmanTable table, BitStream* stream)
	{
		int i;

		int num = (int)BitStreamRead(stream, 0x1F, 5);
		if (num == 0)
			return;

		int[] leafLen = new int[32];
		int leafMax = 1;

		for (i = 0; i < num; i++)
		{
			leafLen[i] = (int)BitStreamRead(stream, 0x0F, 4);

			if (leafMax < leafLen[i])
				leafMax = leafLen[i];
		}

		int count = 0;
		uint value = 0u; // code as BE

		for (i = 1; i <= leafMax; i++)
		{
			for (int j = 0; j < num; j++)
			{
				if (leafLen[j] != i)
					continue;

				table.Leaves[count].Code = MirrorBits(value, i);
				table.Leaves[count].CodeLength = i;
				table.Leaves[count].Value = j;
				value++;
				count++;
			}

			value <<= 1;
		}

		table.Count = count;
	}

	private static uint ReadU16LittleEndian(byte* p)
	{
		uint n = p[1];
		return (n << 8) + p[0];
	}

	private static uint ReadU16BigEndian(byte* p)
	{
		uint n = p[0];
		return (n << 8) + p[1];
	}

	private static uint ReadU32BigEndian(byte* p)
	{
		uint n = p[0];
		n = (n << 8) + p[1];
		n = (n << 8) + p[2];
		n = (n << 8) + p[3];
		return n;
	}

	private struct BitStream
	{
		public uint BitBuffer;
		public int BitCount;
		public byte* DataEnd;
		public byte* DataPos;
	}

	private sealed class HuffmanTable
	{
		public readonly HuffmanLeaf[] Leaves = new HuffmanLeaf[32];
		public int Count;
	}

	private struct HuffmanLeaf
	{
		public uint Code;
		public int CodeLength;
		public int Value;
	}
}
