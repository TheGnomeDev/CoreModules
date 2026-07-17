using UnityEngine;
using UnityEditor;
using Gnomedev.GameplayTags;

namespace Gnomedev.GameplayTags.Editor
{
	[CustomEditor(typeof(GameplayTagsAsset))]
	public class GameplayTagsAssetEditor : UnityEditor.Editor
	{
		[MenuItem("Gnomedev/Gameplay Tags")]
		public static void InitGameplayTags()
		{
			GameplayTagsAsset asset = GameplayTagsAsset.GetOrCreateAsset();
		}
	}
}