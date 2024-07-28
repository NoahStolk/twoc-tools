namespace TwocTools.Core.Internals.Rnc;

public static class RncMethod2
{
	private enum ErrorCodes
	{
		None = 0,
		CorruptedInputData = 4,
		CrcCheckFailed = 5,
		WrongRncHeader = 6,
		WrongRncHeader2 = 7,
		DecryptionKeyRequired = 10,
		NoRncArchivesWereFound = 11,
	}

	private static byte ReadByte(byte[] buf, ref long offset)
	{
		return buf[offset++];
	}

	private static ushort ReadWordBe(byte[] buf, ref long offset)
	{
		byte b1 = ReadByte(buf, ref offset);
		byte b2 = ReadByte(buf, ref offset);
		return (ushort)(b1 << 8 | b2);
	}

	private static uint ReadDwordBe(byte[] buf, ref long offset)
	{
		ushort w1 = ReadWordBe(buf, ref offset);
		ushort w2 = ReadWordBe(buf, ref offset);
		return (uint)(w1 << 16 | w2);
	}

	private static void ReadBuf(byte[] dest, byte[] source, ref long offset, int size)
	{
		Buffer.BlockCopy(source, (int)offset, dest, 0, size);
		offset += size;
	}

	private static void WriteBuf(byte[] dest, ref long offset, byte[] source, int size)
	{
		Buffer.BlockCopy(source, 0, dest, (int)offset, size);
		offset += size;
	}

	private static ushort CrcBlock(byte[] buf, long offset, int size)
	{
		ushort crc = 0;
		while (size-- > 0)
		{
			crc ^= ReadByte(buf, ref offset);
			crc = (ushort)(crc >> 8 ^ RncCrcTable.Table[crc & 0xFF]);
		}

		return crc;
	}

	private static void RorW(ref ushort x)
	{
		x = (ushort)((x & 1) != 0 ? 0x8000 | x >> 1 : x >> 1);
	}

	private static Vars InitVars()
	{
		return new Vars
		{
			EncKey = 0,
			UnpackedCrcReal = 0,
			DictSize = 0x8000,
			ReadStartOffset = 0,
			InputOffset = 0,
			OutputOffset = 0,
		};
	}

	private static byte ReadSourceByte(Vars v)
	{
		if (v.PackBlockStart.ToInt32() == 0xFFFD)
		{
			int leftSize = (int)(v.FileSize - v.InputOffset);
			int sizeToRead = Math.Min(leftSize, 0xFFFD);

			v.PackBlockStart = IntPtr.Zero;
			ReadBuf(v.Mem1, v.Input, ref v.InputOffset, sizeToRead);

			if (leftSize - sizeToRead > 2)
				leftSize = 2;
			else
				leftSize -= sizeToRead;

			ReadBuf(v.Mem1[sizeToRead..], v.Input, ref v.InputOffset, leftSize);
			v.InputOffset -= leftSize;
		}

		byte result = v.Mem1[v.PackBlockStart.ToInt32()];
		v.PackBlockStart = v.PackBlockStart.ToInt32() + 1;
		return result;
	}

	private static uint InputBitsM2(Vars v, short count)
	{
		uint bits = 0;
		while (count-- > 0)
		{
			if (v.BitCount == 0)
			{
				v.BitBuffer = ReadSourceByte(v);
				v.BitCount = 8;
			}

			bits <<= 1;

			if ((v.BitBuffer & 0x80) != 0)
				bits |= 1;

			v.BitBuffer <<= 1;
			v.BitCount--;
		}

		return bits;
	}

	private static void DecodeMatchCount(Vars v)
	{
		v.MatchCount = (ushort)(InputBitsM2(v, 1) + 4);

		if (InputBitsM2(v, 1) != 0)
			v.MatchCount = (ushort)((v.MatchCount - 1 << 1) + InputBitsM2(v, 1));
	}

	private static void DecodeMatchOffset(Vars v)
	{
		v.MatchOffset = 0;
		if (InputBitsM2(v, 1) != 0)
		{
			v.MatchOffset = (ushort)InputBitsM2(v, 1);
			if (InputBitsM2(v, 1) != 0)
			{
				v.MatchOffset = (ushort)(v.MatchOffset << 1 | InputBitsM2(v, 1) | 4);

				if (InputBitsM2(v, 1) == 0)
					v.MatchOffset = (ushort)(v.MatchOffset << 1 | InputBitsM2(v, 1));
			}
			else if (v.MatchOffset == 0)
			{
				v.MatchOffset = (ushort)(InputBitsM2(v, 1) + 2);
			}
		}

		v.MatchOffset = (ushort)((v.MatchOffset << 8 | ReadSourceByte(v)) + 1);
	}

	private static void WriteDecodedByte(Vars v, byte b)
	{
		if (v.Window.ToInt32() == 0xFFFF)
		{
			WriteBuf(v.Output, ref v.OutputOffset, v.Decoded[v.DictSize..], 0xFFFF - v.DictSize);
			Array.Copy(v.Decoded, v.Window - v.DictSize, v.Decoded, 0, v.DictSize);
			v.Window = (IntPtr)v.DictSize;
		}

		v.Decoded[v.Window.ToInt32()] = b;
		v.Window += 1;
		v.UnpackedCrcReal = (ushort)(RncCrcTable.Table[(v.UnpackedCrcReal ^ b) & 0xFF] ^ v.UnpackedCrcReal >> 8);
	}

	private static int UnpackDataM2(Vars v)
	{
		while (v.ProcessedSize < v.InputSize)
		{
			while (true)
			{
				if (InputBitsM2(v, 1) == 0)
				{
					WriteDecodedByte(v, (byte)((v.EncKey ^ ReadSourceByte(v)) & 0xFF));
					RorW(ref v.EncKey);
					v.ProcessedSize++;
				}
				else
				{
					if (InputBitsM2(v, 1) != 0)
					{
						if (InputBitsM2(v, 1) != 0)
						{
							if (InputBitsM2(v, 1) != 0)
							{
								v.MatchCount = (ushort)(ReadSourceByte(v) + 8);
								if (v.MatchCount == 8)
								{
									InputBitsM2(v, 1);
									break;
								}
							}
							else
							{
								v.MatchCount = 3;
							}

							DecodeMatchOffset(v);
						}
						else
						{
							v.MatchCount = 2;
							v.MatchOffset = (ushort)(ReadSourceByte(v) + 1);
						}

						v.ProcessedSize += v.MatchCount;

						while (v.MatchCount-- > 0)
							WriteDecodedByte(v, v.Decoded[v.Window.ToInt32() - v.MatchOffset]);
					}
					else
					{
						DecodeMatchCount(v);

						if (v.MatchCount != 9)
						{
							DecodeMatchOffset(v);
							v.ProcessedSize += v.MatchCount;
							while (v.MatchCount-- > 0)
								WriteDecodedByte(v, v.Decoded[v.Window.ToInt32() - v.MatchOffset]);
						}
						else
						{
							uint dataLength = (InputBitsM2(v, 4) << 2) + 12;
							v.ProcessedSize += dataLength;

							while (dataLength-- > 0)
								WriteDecodedByte(v, (byte)((v.EncKey ^ ReadSourceByte(v)) & 0xFF));

							RorW(ref v.EncKey);
						}
					}
				}
			}
		}

		WriteBuf(v.Output, ref v.OutputOffset, v.Decoded[v.DictSize..], v.Window.ToInt32() - v.DictSize);
		return 0;
	}

	private static ErrorCodes DoUnpackData(Vars v)
	{
		long startPos = v.InputOffset;
		uint sign = ReadDwordBe(v.Input, ref v.InputOffset);
		if (sign >> 8 != 0x524E43) // RNC
		{
			return ErrorCodes.WrongRncHeader;
		}
		v.Method = sign & 3;
		v.InputSize = ReadDwordBe(v.Input, ref v.InputOffset);
		v.PackedSize = ReadDwordBe(v.Input, ref v.InputOffset);
		if (v.FileSize < v.PackedSize)
		{
			return ErrorCodes.WrongRncHeader2;
		}
		v.UnpackedCrc = ReadWordBe(v.Input, ref v.InputOffset);
		v.PackedCrc = ReadWordBe(v.Input, ref v.InputOffset);
		ReadByte(v.Input, ref v.InputOffset);
		ReadByte(v.Input, ref v.InputOffset);
		if (CrcBlock(v.Input, v.InputOffset, (int)v.PackedSize) != v.PackedCrc)
		{
			return ErrorCodes.CorruptedInputData;
		}
		v.Mem1 = new byte[0xFFFF];
		v.Decoded = new byte[0xFFFF];
		v.PackBlockStart = 0xFFFD;
		v.Window = (IntPtr)v.DictSize;
		v.UnpackedCrcReal = 0;
		v.BitCount = 0;
		v.BitBuffer = 0;
		v.ProcessedSize = 0;
		ushort specifiedKey = v.EncKey;
		ErrorCodes errorCode = 0;
		InputBitsM2(v, 1);
		if (errorCode == 0)
		{
			if (InputBitsM2(v, 1) != 0 && v.EncKey == 0)
			{
				errorCode = ErrorCodes.DecryptionKeyRequired;
			}
		}
		if (errorCode == 0)
		{
			if (v.Method == 2)
			{
				errorCode = (ErrorCodes)UnpackDataM2(v);
			}
		}
		v.EncKey = specifiedKey;
		v.InputOffset = startPos + v.PackedSize + 0x12;
		if (errorCode != 0)
		{
			return errorCode;
		}
		if (v.UnpackedCrc != v.UnpackedCrcReal)
		{
			Console.WriteLine($"CRC check failed: {v.UnpackedCrc} != {v.UnpackedCrcReal}");
			return ErrorCodes.CrcCheckFailed;
		}
		return ErrorCodes.None;
	}

	private static ErrorCodes DoUnpack(Vars v)
	{
		v.PackedSize = v.FileSize;
		if (v.FileSize < 0x12)
		{
			return ErrorCodes.WrongRncHeader;
		}
		return DoUnpackData(v);
	}

	public static byte[] Unpack(byte[] input)
	{
		Vars v = InitVars();
		v.ReadStartOffset = 0;
		v.InputOffset = 0;
		v.OutputOffset = 0;

		using (MemoryStream inFile = new(input))
		{
			v.FileSize = (uint)(inFile.Length - v.ReadStartOffset);
			v.Input = new byte[v.FileSize];
			inFile.Seek(v.ReadStartOffset, SeekOrigin.Begin);
			inFile.Read(v.Input, 0, (int)v.FileSize);
		}

		v.Output = new byte[0x1E00000];
		ErrorCodes errorCode = DoUnpack(v);
		switch (errorCode)
		{
			case ErrorCodes.None:
				Console.WriteLine("File successfully unpacked!");
				Console.WriteLine($"Original/new size: {v.PackedSize + 0x12}/{v.OutputOffset} bytes");

				using (MemoryStream outFile = new())
				{
					outFile.Write(v.Output, 0, (int)v.OutputOffset);
					return outFile.ToArray();
				}

			case ErrorCodes.CorruptedInputData:
				Console.WriteLine("Corrupted input data.");
				break;
			case ErrorCodes.CrcCheckFailed:
				Console.WriteLine("CRC check failed.");
				break;
			case ErrorCodes.WrongRncHeader:
			case ErrorCodes.WrongRncHeader2:
				Console.WriteLine("Wrong RNC header.");
				break;
			case ErrorCodes.DecryptionKeyRequired:
				Console.WriteLine("Decryption key required.");
				break;
			case ErrorCodes.NoRncArchivesWereFound:
				Console.WriteLine("No RNC archives were found.");
				break;
			default:
				Console.WriteLine($"Cannot process file. Error code: {errorCode}");
				break;
		}

		throw new Exception("Failed to unpack file.");
	}

	private sealed class Vars
	{
		public ushort EncKey;
		public ushort DictSize = 0x8000;
		public uint Method;
		public uint InputSize;
		public uint FileSize;
		public uint PackedSize;
		public uint ProcessedSize;
		public ushort BitCount;
		public ushort MatchCount;
		public ushort MatchOffset;
		public uint BitBuffer;
		public ushort UnpackedCrc;
		public ushort UnpackedCrcReal;
		public ushort PackedCrc;
		public byte[] Mem1 = null!;
		public IntPtr PackBlockStart;
		public byte[] Decoded = null!;
		public IntPtr Window;
		public long ReadStartOffset;
		public byte[] Input = null!;
		public byte[] Output = null!;
		public long InputOffset;
		public long OutputOffset;
	}
}
