using Detach.Numerics;
using ImGuiNET;
using TwocTools.Core.DataTypes.Crt;

namespace TwocTools.App.Extensions;

public static class CrateTypeExtensions
{
	public static unsafe Rgba GetColor(this CrateType crateType)
	{
		return crateType switch
		{
			CrateType.Empty => new Rgba(191, 191, 255, 127),
			CrateType.Default => new Rgba(159, 95, 0),
			CrateType.Life => new Rgba(255, 127, 255),
			CrateType.AkuAku => Rgba.Orange,
			CrateType.Arrow => Rgba.Yellow,
			CrateType.QuestionMark => Rgba.Yellow,
			CrateType.Bounce => new Rgba(159, 95, 0),
			CrateType.Checkpoint => Rgba.Yellow,
			CrateType.Slot => Rgba.White,
			CrateType.Tnt => Rgba.Red,
			CrateType.TimeTrialOne => new Rgba(255, 191, 0),
			CrateType.TimeTrialTwo => new Rgba(255, 191, 0),
			CrateType.TimeTrialThree => new Rgba(255, 191, 0),
			CrateType.IronArrow => new Rgba(191, 191, 191),
			CrateType.Exclamation => new Rgba(191, 191, 255),
			CrateType.Iron => new Rgba(191, 191, 191),
			CrateType.Nitro => Rgba.Green,
			CrateType.NitroSwitch => new Rgba(0, 191, 0),
			CrateType.Proximity => new Rgba(191, 127, 0),
			CrateType.Locked => new Rgba(159, 159, 255),
			CrateType.Invincibility => new Rgba(191, 191, 127),
			_ => Rgba.FromVector4(*ImGui.GetStyleColorVec4(ImGuiCol.TextDisabled)),
		};
	}
}
