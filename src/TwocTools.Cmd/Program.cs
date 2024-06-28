using TwocTools.Core.DataTypes;
using TwocTools.Core.Internals;
using TwocTools.Core.Serializers;

const string levelPath = @"F:\Games\Game Stuff\Crash Bandicoot\The Wrath Of Cortex\ISO Extract\levels\a\western";

foreach (string filePath in Directory.GetFiles(levelPath, "*.WMP"))
{
	using BigEndianBinaryReader br = new(File.OpenRead(filePath));
	List<Wumpa> wumpas = WumpaSerializer.Deserialize(br);

	Console.WriteLine(wumpas.Count);
	foreach (Wumpa wumpa in wumpas)
	{
		Console.WriteLine(wumpa);
	}
}

foreach (string filePath in Directory.GetFiles(levelPath, "*.CRT"))
{
	using BigEndianBinaryReader br = new(File.OpenRead(filePath));
	List<CrateGroup> crateCollection = CrateSerializer.Deserialize(br);

	Console.WriteLine(crateCollection.Sum(cg => cg.Crates.Count));
	foreach (CrateGroup crateGroup in crateCollection)
	{
		Console.WriteLine(crateGroup.Position);
		Console.WriteLine(crateGroup.Crates.Count);
		foreach (Crate crate in crateGroup.Crates)
		{
			Console.WriteLine($"{crate.B} {crate.C} {crate.D} {crate.E} {crate.Position:0.00}");
		}
	}
}
