using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwocTools.Core.Internals.Rnc;

namespace TwocTools.Core.Tests;

[TestClass]
public class RncTests
{
	[DataTestMethod]
	[DataRow("PAUSE")]
	[DataRow("CRATES")]
	public void ExtractGscToNus(string fileName)
	{
		byte[] expectedNusInput = File.ReadAllBytes(Path.Combine("Resources", $"{fileName}.NUS"));

		using FileStream gscFileStream = File.OpenRead(Path.Combine("Resources", $"{fileName}.GSC"));
		byte[] nusOutput = RncMethod2.Unpack(gscFileStream);

		Assert.AreEqual(expectedNusInput.Length, nusOutput.Length);

		for (int i = 0; i < expectedNusInput.Length; i++)
			Assert.AreEqual(expectedNusInput[i], nusOutput[i]);
	}
}
