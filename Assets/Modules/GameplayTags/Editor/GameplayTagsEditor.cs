using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using Gnomedev.GameplayTags;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Gnomedev.GameplayTags.Editor
{
	public class TagLineElementData
	{
		public VisualElement visualElement;

		public VisualElement hierarchyIcon;
		public Label labelField;
		public Button addButton;
		public Button removeButton;

		public string tagValue;
		public int id;

		public TagLineElementData() { }

		public TagLineElementData(VisualElement element) { Init(element); }

		public void Init(VisualElement element)
		{
			visualElement = element;
			hierarchyIcon = element.Q<Image>("HierarchySprite");
			labelField = element.Q<Label>();
			addButton = element.Q<Button>("AddButton");
			removeButton = element.Q<Button>("RemoveButton");
		}

		public void Bind(VisualElement element, int index, string tag)
		{
			if (visualElement == null)
				Init(element);
			id = index;
			tagValue = tag;
			labelField.text = tag;

			Debug.Log($"Bind element at {index} to tag {tag}");
		}
	}

	public class GameplayTagsEditor : EditorWindow
	{
		const string WINDOW_TITLE = "GameplayTags Editor";
		const string MENU_ITEM = "Gnomedev/" + WINDOW_TITLE;
		const string DEFAULT_EDITOR_FOLDER = "Assets/Modules/GameplayTags/Editor/";
		const string WINDOW_UXML = "GameplayTagsEditorWindow.uxml";
		const string TAGLINE_UXML = "TagEditorTagLine.uxml";

		// uxml assets
		private VisualTreeAsset windowAsset;
		private VisualTreeAsset tagLineAsset;

		// top area - search
		private VisualElement searchArea;
		private TextField searchField;
		private Button clearSearchButton;

		// middle area - tag list (tree)
		private VisualElement treeArea;
		private TreeView tree;
		private Button removeTagsButton;
		private Label noTagsLabel;

		// bottom area - info
		private VisualElement infoArea;
		private Label tagCountLabel;


		private GameplayTagsAsset tagsAsset;
		private List<TreeViewItemData<TagLineElementData>> tagList = new();

		[MenuItem(MENU_ITEM)]
		public static GameplayTagsEditor Open()
		{
			var window = GetWindow<GameplayTagsEditor>(false, WINDOW_TITLE, true);
			window.Show();
			return window;
		}

		private void OnEnable()
		{
			if (tagsAsset == null)
				tagsAsset = GameplayTagsAsset.GetOrCreateAsset();
		}

		private void OnDisable() { }

		public void CreateGUI()
		{
			VisualElement root = rootVisualElement;
			windowAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(DEFAULT_EDITOR_FOLDER + WINDOW_UXML);
			var window = windowAsset.Instantiate();
			root.Add(window);
			window.StretchToParentSize();
			tagLineAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(DEFAULT_EDITOR_FOLDER + TAGLINE_UXML);
			//Resources.UnloadAsset(windowAsset);

			// grab and set up UI elements
			// top area
			searchArea = window.Q<VisualElement>("SearchArea");
			searchField = searchArea.Q<TextField>();
			searchField.RegisterValueChangedCallback(OnSearchValueChanged);
			clearSearchButton = searchArea.Q<Button>();
			SetDisplayStyle(clearSearchButton, false);
			clearSearchButton.clicked += OnClearSearchClicked;

			// middle area
			tree = window.Q<TreeView>();
			tree.makeItem = MakeTagListItem;
			tree.bindItem = BindTagListItem;
			tree.destroyItem = DestroyTagListItem;
			tree.SetRootItems(tagList);
			tree.autoExpand = true;
			tree.selectedIndicesChanged += OnSelectedIndicesChanged;
			treeArea = tree.parent;
			noTagsLabel = treeArea.Q<Label>("NoTagsFound");
			SetDisplayStyle(noTagsLabel, false);

			// bottom area
			infoArea = window.Q<VisualElement>("InfoArea");
			tagCountLabel = infoArea.Q<Label>("TagCounts");
			FillTempItems();
			UpdateTagCounts();
		}

		private void OnDestroy()
		{
			if (clearSearchButton != null)
				clearSearchButton.clicked -= OnClearSearchClicked;
			if (tree != null)
				tree.selectedIndicesChanged -= OnSelectedIndicesChanged;
			if (searchField != null)
				searchField.UnregisterValueChangedCallback(OnSearchValueChanged);
		}

		// search area methods

		private void OnSearchValueChanged(ChangeEvent<string> evt)
		{
			Debug.Log($"Search field changed from {evt.previousValue} to {evt.newValue}.");
			bool hasString = !string.IsNullOrEmpty(evt.newValue);
			SetDisplayStyle(clearSearchButton, hasString);
		}

		private void OnClearSearchClicked()
		{
			Debug.Log("Clear search.");
			SetDisplayStyle(clearSearchButton, false);
			searchField.SetValueWithoutNotify(string.Empty);
		}

		// tree view methods
		private VisualElement MakeTagListItem()
		{
			Debug.Log("Making item");
			VisualElement line = tagLineAsset.Instantiate();
			line.Q<Button>("AddButton").RegisterCallback<ClickEvent, VisualElement>(TagLine_AddTagClicked, line);
			line.Q<Button>("RemoveButton").RegisterCallback<ClickEvent, VisualElement>(TagLine_RemoveTagClicked, line);
			return line;
		}

		private void BindTagListItem(VisualElement item, int index)
		{
			item.Q<Label>().text = index.ToString();
			item.userData = index;
			Debug.Log($"Binding item {index}");
			//item.Q<Button>("AddButton").RegisterCallback<ClickEvent, int>(TagLine_AddTagClicked, index);
		}

		private void DestroyTagListItem(VisualElement element)
		{
			Debug.Log($"Destroy item: {tree.IndexOf(element)}.");
			element.Q<Button>("AddButton").UnregisterCallback<ClickEvent, VisualElement>(TagLine_AddTagClicked);
			element.Q<Button>("RemoveButton").UnregisterCallback<ClickEvent, VisualElement>(TagLine_RemoveTagClicked);
		}

		private void TagLine_AddTagClicked(ClickEvent evt, VisualElement item)
		{
			VisualElement element = evt.target as VisualElement;
			VisualElement curElement = evt.currentTarget as VisualElement;
			Debug.Log($"Clicked Add on item {item.userData} for element {element.name} with curElement {curElement.name}");
			//if (evt.currentTarget is VisualElement element && element == item)
			//{
			//	Debug.Log($"Clicked Add on item {item.userData}");
			//}
		}

		private void TagLine_RemoveTagClicked(ClickEvent evt, VisualElement item)
		{
			VisualElement element = evt.target as VisualElement;
			VisualElement curElement = evt.currentTarget as VisualElement;
			Debug.Log($"Clicked Remove on item {item.userData} for element {element.name} with curElement {curElement.name}");
			//if (evt.currentTarget is VisualElement element && curElement == item)
			//{
			//	Debug.Log($"Clicked Remove on item {item.userData}");
			//}
		}

		private void OnItemsAdded(IEnumerable<int> enumerable)
		{
			Debug.Log("Added");
		}

		private void OnItemsRemoved(IEnumerable<int> enumerable)
		{
			Debug.Log("Removed");
		}

		private void OnTagSourceChanged()
		{
			Debug.Log("Source changed");
		}

		private void OnAddTagClicked()
		{
			Debug.Log("Add");
		}

		private void OnRemoveTagsClicked()
		{
			Debug.Log("Remove");
		}

		private void OnClearTagsClicked()
		{
			Debug.Log("Clear");
		}

		private void OnSelectedIndicesChanged(IEnumerable<int> enumerable)
		{
			Debug.Log($"Selected indices changed: {enumerable.Count()} items selected.");
			//bool areAnySelected = enumerable.Any();
			//removeTagsButton.enabledSelf = areAnySelected;
		}

		private void OnSelectionChanged(IEnumerable<object> enumerable)
		{
			Debug.Log($"Selection changed: {enumerable.Count()} items selected.");
			bool areAnySelected = enumerable.Any();
			removeTagsButton.enabledSelf = areAnySelected;
		}

		private void UpdateTagCounts()
		{
			int totalCount = tagList.Count;
			int filteredCount = tree.GetTreeCount();
			int selectedCount = tree.selectedItems.Count();
			tagCountLabel.text = $"Total tags: {totalCount}. Filtered: {filteredCount}. Selected: {selectedCount}.";
		}

		// general

		private void SetDisplayStyle(VisualElement element, bool display)
		{
			StyleEnum<DisplayStyle> displayStyle = element.style.display;
			displayStyle.value = display ? DisplayStyle.Flex : DisplayStyle.None;
			element.style.display = displayStyle;
		}

		private void Refresh()
		{

		}

		// temp

		private void FillTempItems()
		{
			for (int i = 0; i < 5; i++)
			{
				VisualElement newElement = tagLineAsset.Instantiate();
				TagLineElementData elementData = new TagLineElementData(newElement);
				elementData.Bind(newElement, i, $"Tag {i}");
				TreeViewItemData<TagLineElementData> treeData = new TreeViewItemData<TagLineElementData>(i, elementData);
				tree.AddItem(treeData, -1, -1, false);
				tree.Rebuild();
			}
		}
	}
}