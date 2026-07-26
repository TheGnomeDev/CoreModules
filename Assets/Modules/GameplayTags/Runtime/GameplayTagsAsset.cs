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
		#region Constants
		internal const string ALLOWED_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		private const string MODULE_PATH = "Modules/GameplayTags";
		private const string RESOURCE_PATH = "Resources/";
		private const string ASSET_PATH = MODULE_PATH + "/" + RESOURCE_PATH;
		private const string ASSET_NAME = nameof(GameplayTagsAsset);
		private const string INTERNAL_ASSET_NAME = ASSET_NAME + "_Internal";
		#endregion

		#region GameplayTagData
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

			internal GameplayTagData(string tagValue) : base()
			{
				this.tagValue = tagValue;
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
		#endregion

		#region Fields
		internal List<GameplayTagData> tagList;
		private Dictionary<string, GameplayTagData> tags;
		//private Dictionary<GameplayTagData, GameplayTagData> parentTags;
		//private Dictionary<GameplayTagData, List<GameplayTagData>> childrenTags;
		#endregion

#if UNITY_EDITOR
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
#endif

		#region Methods
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

		/// <summary>
		/// Recursive add tag - adds all parents and hooks up parents and children
		/// </summary>
		/// <returns>A new or existing <see cref="GameplayTagData"/> of the tag value</returns>
		internal GameplayTagData AddTag(string tagValue)
		{
			tagValue = GameplayTagSystem.SanitizeTagValue(tagValue);
			if (tags.TryGetValue(tagValue, out GameplayTagData tag))
				return tag;

			tag = new GameplayTagData(tagValue);
			tags.Add(tagValue, tag);

			if (GameplayTagSystem.GetTagValueParent(tagValue, out string parentValue))
			{
				if (!tags.TryGetValue(parentValue, out GameplayTagData parentTag))
					parentTag = AddTag(parentValue);

				parentTag.AddChild(tag);
				tag.parent = parentTag;
			}

			return tag;
		}

		/// <summary>
		/// Delete a tag by value
		/// </summary>
		internal void DeleteTag(string tagValue)
		{
			tagValue = GameplayTagSystem.SanitizeTagValue(tagValue);
			if (tags.TryGetValue(tagValue, out GameplayTagData tag))
				DeleteTag(tag);
		}

		/// <summary>
		/// Delete tag - disconnects parent tag and deletes all decendents
		/// </summary>
		private void DeleteTag(GameplayTagData tag)
		{
			List<GameplayTagData> visitedTags = new() { tag };
			List<GameplayTagData> tagsToRemove = new();

			// grab all decendents
			while (visitedTags.Count > 0)
			{
				GameplayTagData currentTag = visitedTags[visitedTags.Count - 1];
				visitedTags.RemoveAt(visitedTags.Count - 1);
				tagsToRemove.Add(currentTag);
				visitedTags.AddRange(currentTag.children);
			}

			// remove them
			foreach (GameplayTagData t in tagsToRemove)
				tags.Remove(t.tagValue);

			// and disconnect the original tag from its parent
			tag.parent.RemoveChild(tag);
		}

		private void InitAsset(GameplayTagsAsset asset)
		{
			tagList = new List<GameplayTagData>();
			tags = new Dictionary<string, GameplayTagData>();
			//parentTags = new Dictionary<GameplayTagData, GameplayTagData>();
			//childrenTags = new Dictionary<GameplayTagData, List<GameplayTagData>>();
		}
		#endregion
	}
}