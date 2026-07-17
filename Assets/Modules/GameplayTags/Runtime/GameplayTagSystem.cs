using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gnomedev.GameplayTags
{
	public class GameplayTagSystem
	{
		internal static Dictionary<ulong, GameplayTag> tags;
		internal static Dictionary<ulong, GameplayTag> parents;
		internal static Dictionary<ulong, GameplayTag[]> children;
		internal static ulong counter;

#if UNITY_EDITOR
		[InitializeOnLoadMethod]
#endif
		private static void Init()
		{
			if (tags == null)
			{
				tags = new();
				parents = new();
				children = new();
				counter = 0;
			}
		}

		public static GameplayTag GetTag(string tagValue)
		{
			// should this be a thing?

			throw new System.NotImplementedException();
		}

		public static GameplayTag GetTag(ulong tagValue)
		{
			// should this be a thing?

			throw new System.NotImplementedException();
		}

		internal static GameplayTag CreateTag(string tagValue)
		{
			// parse the string for . separators
			// have an editor list/dictionary of string->ulong?
			// check if tag value exists
			// walk up ancestry to find existing tags
			// add in any needed ancestors
			// link children and parents
			// finally add this tag and link

			throw new System.NotImplementedException();
		}

		internal static GameplayTag[] GetAncestry(GameplayTag tag)
		{
			// check if tag exists
			// if not, throw exception in editor, return empty array otherwise
			// if exists, just use parent values and get all those tags

			throw new System.NotImplementedException();
		}

		private static void LoadData()
		{
			// load scriptable from resources

		}
	}
}