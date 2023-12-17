using ConsoleApp.App.Popups;
using ConsoleApp.Layout;
using ConsoleApp.Layout.Models;
using Mindmagma.Curses;

namespace ConsoleApp.App.Views;

/// <summary>
/// Show, navigate and modify the filesystem
/// </summary>
public class FilesystemView : View
{
	// directory and contents
	private string currentPath;
	private List<string> folderItemPaths = new();
	private List<string> folderItemNames = new();

	// directory display
	private int listOffset = 0;
	private int cursorItemIndex = 0;
	private Dictionary<string, (int offset, int selected)> scrollHistory = new();

	// scrolling
	private int firstDisplayedItemIndex = -1;
	private int lastDisplayedItemIndex = -1;
	private const int CURSOR_LAG = 5;

	// selecting
	private HashSet<int> selectedItems = new();

	// popups
	ConfirmDeletePopup? confirmDeletePopup = null;

	public FilesystemView(WindowSize size) : base(size)
	{
		currentPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		SetFolder(currentPath);
		DisplayCurrentFolder();
	}

	public override int MaxCountPerPanel => 1;

	public override void FocusLost() => DisplayCurrentFolder();

	protected override void FocusGained() => DisplayCurrentFolder();

	public override void Dispose()
	{
		if (confirmDeletePopup is not null)
		{
			confirmDeletePopup.Dispose();
			confirmDeletePopup = null;
			selectedItems.Clear();
		}
		base.Dispose();
	}

	protected override UpdateModel UpdateInner(string? keyPressed)
	{
		ToggleBox(currentPath);
		var refreshScreen = false;

		// popup handling - item deletion
		if (confirmDeletePopup is not null && (keyPressed == "enter" || keyPressed == "backspace"))
		{
			if (keyPressed == "enter")
			{
				foreach (var item in selectedItems)
				{
					var itemPath = folderItemPaths[item];
					if (Directory.Exists(itemPath)) Directory.Delete(itemPath, true);
					if (File.Exists(itemPath)) File.Delete(itemPath);
				}
				SetFolder(currentPath);
			}

			keyPressed = null;
			confirmDeletePopup.Dispose();
			confirmDeletePopup = null;
			selectedItems.Clear();
			refreshScreen = true;
		}

		// handle vertical navigation
		if (keyPressed == "up" || keyPressed == "down")
		{
			cursorItemIndex = keyPressed == "up" ? cursorItemIndex - 1 : cursorItemIndex + 1;

			if (cursorItemIndex < 0) cursorItemIndex = 0;
			if (cursorItemIndex >= folderItemNames.Count) cursorItemIndex = folderItemNames.Count - 1;

			if (cursorItemIndex - firstDisplayedItemIndex < CURSOR_LAG) listOffset -= 1;
			if (lastDisplayedItemIndex - cursorItemIndex < CURSOR_LAG) listOffset += 1;

			if (listOffset > size.Rows - 4) listOffset = size.Rows - 3; // top-bottom borders, scroll-up indicator
			if ((folderItemNames.Count - listOffset) < (size.Rows - 3)) listOffset--;
			if (listOffset < 0) listOffset = 0;

			refreshScreen = true;
		}

		// handle item opening
		if (keyPressed == "enter")
		{
			var selectedItem = folderItemPaths[cursorItemIndex];
			if (Directory.Exists(selectedItem))
			{
				if (scrollHistory.ContainsKey(currentPath))
				{
					scrollHistory.Remove(currentPath);
				}

				scrollHistory.Add(currentPath, (listOffset, cursorItemIndex));
				SetFolder(selectedItem);
				refreshScreen = true;
			}
		}

		// handle going up the tree
		if (keyPressed == "backspace")
		{
			var parentFolder = Directory.GetParent(currentPath)?.FullName;
			if (Directory.Exists(parentFolder))
			{
				SetFolder(parentFolder);
				refreshScreen = true;
			}
		}

		// handle item marking
		if (keyPressed == "space")
		{
			if (selectedItems.Contains(cursorItemIndex)) selectedItems.Remove(cursorItemIndex);
			else selectedItems.Add(cursorItemIndex);
			refreshScreen = true;
		}

		// handle item deletion
		if (keyPressed == "del")
		{
			if (!selectedItems.Contains(cursorItemIndex))
			{
				selectedItems.Add(cursorItemIndex);
				DisplayCurrentFolder();
			}

			// if the delete item list can fit on one screen, we will display all
			// if not, we'll just show the number of affected items
			var popupRows = (selectedItems.Count + 2) < (size.Rows - 4) ? (selectedItems.Count + 2) : 3;
			var popupCols = Math.Min(folderItemNames.Max(n => n.Length), size.Rows - 4);
			var popupSize = new WindowSize
			{
				Rows = Math.Max(popupRows, ConfirmDeletePopup.MinRows),
				Columns = Math.Max(popupCols, ConfirmDeletePopup.MinCols),
				RowOrigin = (size.Rows / 2) - (popupRows / 2),
				ColumnsOrigin = (size.Columns / 2) - (popupCols / 2)
			};

			var delItemNames = selectedItems.Select(i => folderItemNames[i]).ToList();

			confirmDeletePopup = new ConfirmDeletePopup(delItemNames, popupSize);
			confirmDeletePopup.Show();
		}

		// refresh and return
		if (refreshScreen) DisplayCurrentFolder();
		return new UpdateModel
		{
			SwitchPanelFocus = keyPressed == "left" || keyPressed == "right",
		};
	}

	private void SetFolder(string folderPath)
	{
		// set default values
		// read history, if there is any
		currentPath = folderPath;
		if (scrollHistory.TryGetValue(currentPath, out var pos))
		{
			listOffset = pos.offset;
			cursorItemIndex = pos.selected;
		}
		else
		{
			listOffset = 0;
			cursorItemIndex = 0;
		}
		firstDisplayedItemIndex = -1;
		lastDisplayedItemIndex = -1;
		selectedItems = new();

		// read folder contents, generate lists
		var contents = GetFolderContents(folderPath);
		folderItemPaths = new List<string>();
		folderItemNames = new List<string>();

		foreach (var dir in contents.dirs)
		{
			folderItemPaths.Add(dir);
			folderItemNames.Add($" / {ShortenString(Path.GetFileName(dir))}");
		}
		foreach (var file in contents.files)
		{
			folderItemPaths.Add(file);
			folderItemNames.Add($"   {ShortenString(Path.GetFileName(file))}");
		}

		// display current location in the header
		ToggleBox(currentPath);
	}

	private void DisplayCurrentFolder()
	{
		if (listOffset > 0)
		{
			// if we have an offset, we need a scroll-up indicator
			NCurses.MoveWindowAddString(windowObj, 1, 1, " .");
			ClearRestOfLine();
		}

		var maxLineIndex = Math.Min(size.Rows - 2, folderItemNames.Count); // top-bottom borders
		if (listOffset > 0) maxLineIndex -= 1; // scroll-up indicator
		if (folderItemNames.Count - listOffset > maxLineIndex) maxLineIndex -= 1; // scroll-down indicator

		var itemIndex = listOffset;
		for (var lineIndex = 0; lineIndex < maxLineIndex; lineIndex++)
		{
			var item = folderItemNames[itemIndex];

			// Go down by one line per displayed item, +1 for top border,
			// +1 if we need to leave space for a scroll-up indicator
			// Go right 1 for left border
			var yOffset = listOffset == 0 ? lineIndex + 1 : lineIndex + 2;
			var xOffset = 1;

			// color directories differently, color selected items differently
			// color cursor item differently, color inactive cursor item differently
			if (item.StartsWith(" / ")) ToggleColorPair(ColorPairs.BlueOnBlack, true);
			if (selectedItems.Contains(itemIndex)) ToggleColorPair(ColorPairs.RedOnBlack, true);
			if (itemIndex == cursorItemIndex && IsActive) NCurses.WindowAttributeOn(windowObj, CursesAttribute.STANDOUT);
			if (itemIndex == cursorItemIndex && (!IsActive)) NCurses.WindowAttributeOn(windowObj, CursesAttribute.BOLD);

			// write line, overwrite rest of the line
			// in case the current line is shorter then what it was previously
			NCurses.MoveWindowAddString(windowObj, yOffset, xOffset, item);
			ClearRestOfLine();

			ToggleColorPair(ColorPairs.BlueOnBlack, false);
			ToggleColorPair(ColorPairs.RedOnBlack, false);
			NCurses.WindowAttributeOff(windowObj, CursesAttribute.STANDOUT);
			NCurses.WindowAttributeOff(windowObj, CursesAttribute.BOLD);

			if (lineIndex == 0) firstDisplayedItemIndex = itemIndex;
			if (lineIndex == maxLineIndex - 1) lastDisplayedItemIndex = itemIndex;
			itemIndex++;
		}

		// clear rest of the lines in the window
		for (var lineIndex = folderItemNames.Count; lineIndex < size.Rows - 2; lineIndex++)
		{
			NCurses.WindowMove(windowObj, lineIndex + 1, 1);
			ClearRestOfLine();
		}

		// if we have more items left, but can't display them, show the scroll-down indicator
		if (folderItemNames.Count - listOffset > maxLineIndex)
		{
			NCurses.MoveWindowAddString(windowObj, size.Rows - 2, 1, " .");
			ClearRestOfLine();
		}

		NCurses.WindowRefresh(windowObj);
	}

	private (List<string> dirs, List<string> files) GetFolderContents(string folder)
	{
		var dirs = Directory.GetDirectories(folder).ToList();
		var files = Directory.GetFiles(folder).ToList();
		return (dirs, files);
	}
}
