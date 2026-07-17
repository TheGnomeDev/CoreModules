using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace Gnomedev.GameplayTags.Editor
{
	[CustomPropertyDrawer(typeof(GameplayTag))]
	public class GameplayTagDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			return base.CreatePropertyGUI(property);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			base.OnGUI(position, property, label);
		}
	}
}