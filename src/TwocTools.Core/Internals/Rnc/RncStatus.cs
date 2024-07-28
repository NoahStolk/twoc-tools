namespace TwocTools.Core.Internals.Rnc;

public enum RncStatus
{
	Ok,
	UnpackedCrcError,
	FileSizeMismatch,
	HufDecodeError,
	PackedCrcError,
	FileIsNotRnc,
}
