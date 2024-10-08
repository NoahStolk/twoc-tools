﻿using System.Collections;
using System.Numerics;

namespace TwocTools.Core.DataTypes.Crt;

public record CrateGroup : IEnumerable<Crate>
{
	private readonly List<Crate> _crates;

	internal CrateGroup(Vector3 position, ushort crateOffset, ushort crateCount, ushort tilt, List<Crate> crates)
	{
		Position = position;
		CrateOffset = crateOffset;
		CrateCount = crateCount;
		Tilt = tilt;
		_crates = crates;
	}

	public Vector3 Position { get; }

	public ushort CrateOffset { get; }

	public ushort CrateCount { get; }

	public ushort Tilt { get; }

	public int Count => _crates.Count;

	public float TiltInRadians => Tilt * (2 * MathF.PI / 65536f);

	public float TiltInDegrees => Tilt * (360f / 65536f);

	public Crate this[int index] => _crates[index];

	public IEnumerator<Crate> GetEnumerator()
	{
		return _crates.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
