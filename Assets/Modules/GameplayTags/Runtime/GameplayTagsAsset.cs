using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Gnomedev.GameplayTags-Editor")]
namespace Gnomedev.GameplayTags
{
	//[CreateAssetMenu(fileName = "GameplayTagsAsset", menuName = "GameplayTagsAsset")]
	internal class GameplayTagsAsset : ScriptableObject
	{
		internal const string ALLOWED_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		private const string MODULE_PATH = "Modules/GameplayTags";
		private const string RESOURCE_PATH = "Resources/";
		private const string ASSET_PATH = MODULE_PATH + "/" + RESOURCE_PATH;
		private const string ASSET_NAME = nameof(GameplayTagsAsset);
		private const string INTERNAL_ASSET_NAME = ASSET_NAME + "_Internal";

		[Serializable]
		internal class GameplayTagData
		{
			internal string tagValue;
			[NonSerialized] internal GameplayTagData parent;
			[NonSerialized] internal List<GameplayTagData> children;

			internal GameplayTagData()
			{
				tagValue = string.Empty;
				parent = null;
				children = null;
			}

			internal void AddChild(GameplayTagData child)
			{
				children.Add(child);
				ChildUpdated(child);
			}

			internal void RemoveChild(GameplayTagData child)
			{
				children.Remove(child);
			}

			internal void ChildUpdated(GameplayTagData child)
			{
				children.Sort((x, y) => x.tagValue.CompareTo(y.tagValue));
			}
		}

		internal List<GameplayTagData> tagList;
		private Dictionary<string, GameplayTagData> tags;
		private Dictionary<GameplayTagData, GameplayTagData> parentTags;
		private Dictionary<GameplayTagData, List<GameplayTagData>> childrenTags;

		[UnityEditor.MenuItem("Gnomedev/Gameplay Tags/Get or Create Asset")]
		internal static GameplayTagsAsset GetOrCreateAsset()
		{
			if (LoadAssetInternal(out GameplayTagsAsset ia))
				Debug.Log("Internal asset found.");

			if (!TryLoadAsset(out GameplayTagsAsset asset))
			{
				Debug.Log("Creating asset");
				asset = CreateInstance<GameplayTagsAsset>();
				asset.name = ASSET_NAME;
				SaveAsset(asset);
			}

			return asset;
		}

		internal static bool TryLoadAsset(out GameplayTagsAsset asset)
		{
			asset = Resources.Load<GameplayTagsAsset>(ASSET_NAME);
			Debug.Log("Asset found: " + (asset != null));
			return asset != null;
		}

		internal static bool LoadAssetInternal(out GameplayTagsAsset asset)
		{
			asset = Resources.Load<GameplayTagsAsset>(INTERNAL_ASSET_NAME);
			return asset != null;
		}

		internal static void SaveAsset(GameplayTagsAsset asset)
		{
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.CreateAsset(asset, $"Assets/{ASSET_PATH}/{asset.name}.asset");
			Debug.Log("Asset created at: " + UnityEditor.AssetDatabase.GetAssetPath(asset));
#endif
		}

		internal string SanitizeTagValue(string tagValue) { return tagValue.Trim('.'); }

		internal void AddTag(string tagValue)
		{
			tagValue = SanitizeTagValue(tagValue);
			if (tags.ContainsKey(tagValue))
				return;

			// create tag data from value
			// get ancestry and create ancestry tags
			// ensure each ancestry tag is added

			GameplayTagData newTag = new GameplayTagData();
			newTag.tagValue = tagValue;
			string[] ancestry = GetAncestry(tagValue);
			//int parentIndex = Array.FindLastIndex(ancestry, x => x.)
			for (int i = ancestry.Length - 1; i >= 0; i--)
			{
				if (tags.ContainsKey(ancestry[i]))
				{
					GameplayTagData parent = tags[ancestry[i]];
					newTag.parent = parent;
					
				}
			}
		}

		internal void DeleteTag(string tagValue)
		{

		}

		private string[] GetAncestry(string tagValue)
		{
			tagValue = tagValue.Trim('.');
			List<string> ancestors = new();
			var chars = tagValue.AsSpan();
			for (int i = 0; i < chars.Length; i++)
			{
				if (chars[i] == '.')
				{
					string parent = tagValue.Substring(0, i);
					ancestors.Add(parent);
				}
			}
			return ancestors.ToArray();
		}

		private void InitAsset(GameplayTagsAsset asset)
		{
			tagList = new List<GameplayTagData>();
			tags = new Dictionary<string, GameplayTagData>();
			parentTags = new Dictionary<GameplayTagData, GameplayTagData>();
			childrenTags = new Dictionary<GameplayTagData, List<GameplayTagData>>();
		}
	}
}