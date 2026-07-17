using System;
using UnityEngine;

namespace Gnomedev.GameplayTags
{
	/// <summary>
	/// A single GameplayTag tag
	/// </summary>
	public class GameplayTag : IEquatable<GameplayTag>, IComparable<GameplayTag>
	{
		public static readonly GameplayTag Default = new GameplayTag();

		internal string tag;
		internal ulong value;
		internal ulong parentValue;

		internal GameplayTag()
		{
			tag = string.Empty;
			value = 0;
			parentValue = 0;
		}

		/// <summary>
		/// A <see cref="GameplayTag"/> is valid if its <see cref="value"/> is not 0
		/// </summary>
		public bool IsValid() { return value != 0; }

		/// <summary>
		/// A <see cref="GameplayTag"/> has a parent if its <see cref="parentValue"/> is not 0
		/// </summary>
		public bool HasParent() { return parentValue != 0; }

		public bool Equals(GameplayTag other)
		{
			return other is not null && value == other.value;
		}

		public int CompareTo(GameplayTag other)
		{
			if (other is null)
				return 1;

			return value.CompareTo(other.value);
		}

		public override bool Equals(object obj)
		{
			if (obj is GameplayTag other)
				return Equals(other);

			return false;
		}

		public override int GetHashCode() { return value.GetHashCode(); }

		public override string ToString() { return tag; }

		public string ToStringFull() { return $"GameplayTag(\"{tag}\") [IsValid: {IsValid()}, HasParent: {HasParent()}]"; }
	}
}