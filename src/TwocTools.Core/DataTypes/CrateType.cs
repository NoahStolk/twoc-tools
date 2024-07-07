namespace TwocTools.Core.DataTypes;

#pragma warning disable CA1008, CA1027, CA1028
public enum CrateType : byte
#pragma warning restore CA1028, CA1027, CA1008
{
	Empty = 0,
	Default = 1,
	Life = 2,
	AkuAku = 3,
	Arrow = 4,
	QuestionMark = 5,
	Bounce = 6,
	Checkpoint = 7,
	Slot = 8,
	Tnt = 9, // Not sure?
	TimeTrialOne = 10,
	TimeTrialTwo = 11,
	TimeTrialThree = 12,
	Exclamation = 13,
	Unknown14 = 14, // Exclamation trigger?
	Unknown15 = 15,
	Nitro = 16,
	NitroSwitch = 17,
	Locked = 19,
	Invincibility = 20,
	Unknown255 = 255,
}
