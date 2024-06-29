using Detach.Numerics;
using TwocTools.Core.DataTypes;

namespace TwocTools.App.Extensions;

public static class CrateTypeExtensions
{
	public static Rgb GetColor(this CrateType crateType)
	{
		return crateType switch
		{
			CrateType.Default => new Rgb(160, 100, 0),
			CrateType.Life => new Rgb(255, 127, 255),
			CrateType.AkuAku => Rgb.Orange,
			CrateType.Arrow => Rgb.Yellow,
			CrateType.QuestionMark => Rgb.Yellow,
			CrateType.Bounce => new Rgb(160, 100, 0),
			CrateType.Checkpoint => Rgb.Yellow,
			CrateType.Slot => Rgb.White,
			CrateType.Tnt => Rgb.Red,
			CrateType.TimeTrialOne => new Rgb(255, 191, 0),
			CrateType.TimeTrialTwo => new Rgb(255, 191, 0),
			CrateType.TimeTrialThree => new Rgb(255, 191, 0),
			CrateType.Exclamation => new Rgb(191, 191, 255),
			CrateType.Nitro => Rgb.Green,
			CrateType.NitroSwitch => new Rgb(0, 191, 0),
			_ => Rgb.White,
		};
	}
}
