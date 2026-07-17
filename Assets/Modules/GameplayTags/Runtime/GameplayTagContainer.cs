using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace Gnomedev.GameplayTags
{
	/// <summary>
	/// Contains a <see cref="GameplayTag"/> and a counter
	/// </summary>
	internal class GameplayTagCounter
	{
		internal GameplayTag tag;
		internal uint count;

		internal GameplayTagCounter()
		{
			tag = new GameplayTag();
			count = 0;
		}

		internal GameplayTagCounter(GameplayTag tag)
		{
			this.tag = tag;
			count = 0;
		}

		internal bool MatchesTag(GameplayTag tag) { return this.tag.Equals(tag); }

		internal bool MatchesTagValue(ulong value) { return tag.value == value; }

		internal void Increment() { count++; }

		internal void Decrement() { count--; }

		internal void Reset() { count = 0; }
	}

	/// <summary>
	/// A container for GameplayTag objects
	/// </summary>
	public class GameplayTagContainer
	{
		internal List<GameplayTagCounter> tags;

		public void AddTag(GameplayTag tag)
		{
			if (!TryGetTag(tag.value, out GameplayTagCounter counter))
			{
				counter = new GameplayTagCounter(tag);
				tags.Add(counter);
			}

			counter.Increment();
		}

		public void RemoveTag(GameplayTag tag)
		{
			if (TryGetTag(tag.value, out GameplayTagCounter counter))
			{
				counter.Decrement();
				if (counter.count == 0)
					tags.RemoveSwapBack(counter);
			}
		}

		public bool ContainsTag(GameplayTag tag)
		{
			return tags.Find(x => x.MatchesTag(tag)) != null;
		}

		public void ClearTag(GameplayTag tag)
		{
			int idx = tags.FindIndex(x => x.MatchesTag(tag));
			if (idx != -1)
				tags.RemoveAtSwapBack(idx);
		}

		public void Clear()
		{
			tags.Clear();
		}

		internal bool TryGetTag(ulong tagValue, out GameplayTagCounter tagCounter)
		{
			foreach (GameplayTagCounter tag in tags)
			{
				if (tag.MatchesTagValue(tagValue))
				{
					tagCounter = tag;
					return true;
				}
			}

			tagCounter = null;
			return false;
		}
	}
}