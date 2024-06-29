using TwocTools.Core.DataTypes;
using TwocTools.Core.Serializers;

const string levelPath = @"F:\Games\Game Stuff\Crash Bandicoot\The Wrath Of Cortex\ISO Extract\levels\a\western";

foreach (string filePath in Directory.GetFiles(levelPath, "*.WMP"))
{
	using FileStream fileStream = File.OpenRead(filePath);
	WumpaCollection wumpaCollection = WumpaSerializer.Deserialize(fileStream);

	Console.WriteLine(wumpaCollection.Count);
	foreach (Wumpa wumpa in wumpaCollection)
	{
		Console.WriteLine(wumpa);
	}
}

foreach (string filePath in Directory.GetFiles(levelPath, "*.CRT"))
{
	using FileStream fileStream = File.OpenRead(filePath);
	CrateGroupCollection crateCollection = CrateSerializer.Deserialize(fileStream);

	Console.WriteLine(crateCollection.Sum(cg => cg.Count));
	foreach (CrateGroup crateGroup in crateCollection)
	{
		Console.WriteLine(crateGroup.Position);
		Console.WriteLine(crateGroup.Count);
		foreach (Crate crate in crateGroup)
		{
			Console.WriteLine($"{crate.CrateType} {crate.CrateTypeTimeTrial} {crate.D} {crate.E} {crate.Position:0.00}");
		}
	}
}
