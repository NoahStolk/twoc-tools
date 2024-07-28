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
		byte[] gscInput = File.ReadAllBytes(Path.Combine("Resources", $"{fileName}.GSC"));
		byte[] expectedNusInput = File.ReadAllBytes(Path.Combine("Resources", $"{fileName}.NUS"));

		byte[] nusOutput = RncMethod2.Unpack(gscInput);

		Assert.AreEqual(expectedNusInput.Length, nusOutput.Length);

		for (int i = 0; i < expectedNusInput.Length; i++)
			Assert.AreEqual(expectedNusInput[i], nusOutput[i]);
	}
}
