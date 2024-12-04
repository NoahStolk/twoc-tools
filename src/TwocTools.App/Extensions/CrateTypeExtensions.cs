using Detach.Numerics;
using Hexa.NET.ImGui;
using System.Diagnostics;
using TwocTools.Core.DataTypes.Crt;

namespace TwocTools.App.Extensions;

internal static class CrateTypeExtensions
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

	public static ReadOnlySpan<byte> ToUtf8Span(this CrateType crateType)
	{
		return crateType switch
		{
			CrateType.None => "None"u8,
			CrateType.Empty => "Empty"u8,
			CrateType.Default => "Default"u8,
			CrateType.Life => "Life"u8,
			CrateType.AkuAku => "AkuAku"u8,
			CrateType.Arrow => "Arrow"u8,
			CrateType.QuestionMark => "QuestionMark"u8,
			CrateType.Bounce => "Bounce"u8,
			CrateType.Checkpoint => "Checkpoint"u8,
			CrateType.Slot => "Slot"u8,
			CrateType.Tnt => "Tnt"u8,
			CrateType.TimeTrialOne => "TimeTrialOne"u8,
			CrateType.TimeTrialTwo => "TimeTrialTwo"u8,
			CrateType.TimeTrialThree => "TimeTrialThree"u8,
			CrateType.IronArrow => "IronArrow"u8,
			CrateType.Exclamation => "Exclamation"u8,
			CrateType.Iron => "Iron"u8,
			CrateType.Nitro => "Nitro"u8,
			CrateType.NitroSwitch => "NitroSwitch"u8,
			CrateType.Proximity => "Proximity"u8,
			CrateType.Locked => "Locked"u8,
			CrateType.Invincibility => "Invincibility"u8,
			_ => throw new UnreachableException($"Unknown crate type: {crateType}"),
		};
	}
}
