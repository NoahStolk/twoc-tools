using System.Text;
using TwocTools.Core.DataTypes.Nus;
using TwocTools.Core.Internals.Extensions;
using TwocTools.Core.Internals.Rnc;

namespace TwocTools.Core.Serializers;

// Based on:
// - https://github.com/Open-Travelers/travelers-toolkit/blob/main/nu/scene.cpp#L31
// - https://github.com/Open-Travelers/LibTWOC/blob/main/Serializable%20Objects/DOCS.md
public static class NusSerializer
{
	public static void Extract(Stream input)
	{
		byte[] nusInput = RncMethod2.Unpack(input);
		ExtractNus(nusInput);
	}

	private static void ExtractNus(byte[] nusInput)
	{
		using MemoryStream inputStream = new(nusInput);
		using BinaryReader br = new(inputStream);
		ReadOnlySpan<byte> header = br.ReadBytes(4);
		if (!header.SequenceEqual("NU20"u8))
			throw new NotSupportedException($"Unsupported header: {Encoding.UTF8.GetString(header)}");

		br.BaseStream.Seek(12, SeekOrigin.Current);

		while (br.BaseStream.Position < br.BaseStream.Length)
		{
			long originalPosition = br.BaseStream.Position;

			ReadOnlySpan<byte> blockId = br.ReadBytes(4);
			uint size = br.ReadUInt32();

			Console.WriteLine($"Found block {Encoding.UTF8.GetString(blockId)} with size {size}.");
			if (blockId.SequenceEqual("NTBL"u8))
			{
				NameTable nameTable = ParseNameTable(br);
				Console.WriteLine($"Parsed name table with {nameTable.Names.Count} names: {string.Join(", ", nameTable.Names)}");
			}
			else if (blockId.SequenceEqual("TST0"u8))
			{
				ParseTextures(br);
			}

			br.BaseStream.Seek(originalPosition + size, SeekOrigin.Begin);
		}
	}

	private static NameTable ParseNameTable(BinaryReader br)
	{
		uint size = br.ReadUInt32();

		long start = br.BaseStream.Position;
		List<string> names = [];
		while (br.BaseStream.Position < start + size)
		{
			string name = br.ReadNullTerminatedString();
			names.Add(name);
		}

		return new NameTable(size, names);
	}

	private static void ParseTextures(BinaryReader br)
	{
	}
}
