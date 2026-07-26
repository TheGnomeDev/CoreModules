using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Gnomedev.GameplayTags;

namespace Gnomedev.GameplayTags.Editor
{
	[CustomEditor(typeof(GameplayTagsAsset))]
	public class GameplayTagsAssetEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}


	}
}