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
	public class GameplayTagsEditor : EditorWindow
	{
		const string WINDOW_TITLE = "GameplayTags Editor";
		const string MENU_ITEM = "Gnomedev/" + WINDOW_TITLE;
		const string TAGLINE_UXML = "TagEditorTagLine.uxml";
		const string DEFAULT_EDITOR_FOLDER = "Assets/Modules/GameplayTags/Editor/";

		private VisualTreeAsset windowAsset;
		private VisualTreeAsset tagLineAsset;
		private ListView topView;
		private VisualElement bottomView;
		private Button addTagButton;
		private Button removeTagsButton;
		private Button clearTagsButton;

		private GameplayTagsAsset tagsAsset;
		private List<int> ints = new();

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
			ints.Clear();
			ints.AddRange(new int[] { 0, 1, 2, 3, 4 });
		}

		private void OnDisable() { }

		public void CreateGUI()
		{
			// asset = AssetDatabase.LoadAssetAtPath();
			// ui = asset.Instantiate();
			// root.Add(ui);
			tagLineAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(DEFAULT_EDITOR_FOLDER + TAGLINE_UXML);

			VisualElement root = rootVisualElement;

			TwoPaneSplitView topSplit = new TwoPaneSplitView(0, 400, TwoPaneSplitViewOrientation.Vertical);
			root.Add(topSplit);

			topView = new ListView();
			bottomView = new VisualElement();

			topSplit.Add(topView);
			topSplit.Add(bottomView);

			topView.makeItem = MakeTagListItem;
			topView.bindItem = BindTagListItem;
			//topView.makeItem = () => new Label();
			//topView.bindItem = (item, index) => { (item as Label).text = asset.tagList[index].tagValue; };
			//topView.itemsSource = asset.tagList;
			//topView.bindItem = (item, index) => { (item as Label).text = ints[index].ToString(); };
			topView.itemsSource = ints;
			topView.showAddRemoveFooter = true;
			topView.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
			topView.showBorder = true;
			topView.selectionChanged += OnSelectionChanged;
			topView.selectedIndicesChanged += OnSelectedIndicesChanged;
			topView.itemsSourceChanged += OnTagSourceChanged;
			topView.itemsAdded += OnItemsAdded;
			topView.itemsRemoved += OnItemsRemoved;
			//topView.itemsRemoved += (list) => topView.RefreshItems();
			topView.itemsChosen += (list) => topView.MarkDirtyRepaint();//.RefreshItems();
			topView.selectionType = SelectionType.Multiple;

			addTagButton = new Button(OnAddTagClicked);
			addTagButton.text = "Add Tag";
			addTagButton.focusable = false;
			addTagButton.enabledSelf = false;

			removeTagsButton = new Button(OnRemoveTagsClicked);
			removeTagsButton.text = "Remove Tags";
			removeTagsButton.focusable = false;
			removeTagsButton.enabledSelf = false;

			clearTagsButton = new Button(OnClearTagsClicked);
			clearTagsButton.text = "Clear Tags";
			clearTagsButton.focusable = false;
			clearTagsButton.enabledSelf = false;

			var contentAlign = bottomView.style.alignContent;
			contentAlign.value = Align.Center;
			bottomView.style.alignContent = contentAlign;
			var flexDirection = bottomView.style.flexDirection;
			flexDirection.value = FlexDirection.Row;
			bottomView.style.flexDirection = flexDirection;

			bottomView.Add(addTagButton);
			bottomView.Add(removeTagsButton);
			bottomView.Add(clearTagsButton);

			Button temp = new Button(AddInts);
			temp.text = "Add int";
			temp.focusable = false;
			bottomView.Add(temp);

			
		}

		private VisualElement MakeTagListItem()
		{
			VisualElement line = tagLineAsset.Instantiate();
			line.Q<Button>("AddButton").RegisterCallback<ClickEvent, VisualElement>(TagLine_AddTagClicked, line);
			line.Q<Button>("RemoveButton").RegisterCallback<ClickEvent, VisualElement>(TagLine_RemoveTagClicked, line);
			return line;
		}

		private void BindTagListItem(VisualElement item, int index)
		{
			item.Q<Toggle>().SetValueWithoutNotify(false);
			item.Q<Label>().text = index.ToString();
			item.userData = index;
			Debug.Log($"Binding item {index}");
			//item.Q<Button>("AddButton").RegisterCallback<ClickEvent, int>(TagLine_AddTagClicked, index);
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

		private void AddInts()
		{
			ints.Add(ints.Count);
			ints[0]++;
			//topView.RefreshItem(0);
			topView.RefreshItems();
			Debug.Log(ints.Count);
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

		private void OnSelectionChanged(IEnumerable<object> enumerable)
		{
			Debug.Log($"Selection changed: {enumerable.Count()} items selected.");
			bool areAnySelected = enumerable.Any();
			removeTagsButton.enabledSelf = areAnySelected;
		}

		private void OnSelectedIndicesChanged(IEnumerable<int> enumerable)
		{
			Debug.Log($"Selected indices changed: {enumerable.Count()} items selected.");
			//bool areAnySelected = enumerable.Any();
			//removeTagsButton.enabledSelf = areAnySelected;
		}

		private void Refresh()
		{

		}
	}
}