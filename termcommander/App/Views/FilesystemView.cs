using ConsoleApp.Layout;
using ConsoleApp.Layout.Models;
using Mindmagma.Curses;

namespace ConsoleApp.App.Views;

/// <summary>
/// Show, navigate and modify the filesystem
/// </summary>
public class FilesystemView : View
{
	private string currentPath;
	private List<string> folderItemPaths = new();
	private List<string> folderItemNames = new();

	private int listOffset = 0;
	private int selectedItemIndex = 0;

	private int firstDisplayedItemIndex = -1;
	private int lastDisplayedItemIndex = -1;

	private Dictionary<string, (int offset, int selected)> scrollHistory = new();

	private const int CURSOR_LAG = 5;

	public FilesystemView(WindowSize size) : base(size)
	{
		currentPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		SetFolder(currentPath);
		DisplayCurrentFolder();
	}

	public override int MaxCountPerPanel => 1;

	public override void FocusLost()
	{
		DisplayCurrentFolder();
	}

	protected override void FocusGained()
	{
		DisplayCurrentFolder();
	}

	protected override UpdateModel UpdateInner(string? keyPressed)
	{
		ToggleBox();

		if (keyPressed == "up")
		{
			selectedItemIndex -= 1;
		}
		if (keyPressed == "down")
		{
			selectedItemIndex += 1;
		}
		if (keyPressed == "up" || keyPressed == "down")
		{
			if (selectedItemIndex < 0) selectedItemIndex = 0;
			if (selectedItemIndex >= folderItemNames.Count) selectedItemIndex = folderItemNames.Count - 1;

			if (selectedItemIndex - firstDisplayedItemIndex < CURSOR_LAG) listOffset -= 1;
			if (lastDisplayedItemIndex - selectedItemIndex < CURSOR_LAG) listOffset += 1;

			if (listOffset < 0) listOffset = 0;
			if (listOffset > size.Rows - 4) listOffset = size.Rows - 3; // top-bottom borders, scroll-up indicator

			DisplayCurrentFolder();
		}

		if (keyPressed == "enter")
		{
			var selectedItem = folderItemPaths[selectedItemIndex];
			if (Directory.Exists(selectedItem))
			{
				if (scrollHistory.ContainsKey(currentPath))
				{
					scrollHistory.Remove(currentPath);
				}

				scrollHistory.Add(currentPath, (listOffset, selectedItemIndex));
				SetFolder(selectedItem);
				DisplayCurrentFolder();
			}
		}

		if (keyPressed == "backspace")
		{
			var parentFolder = Directory.GetParent(currentPath)?.FullName;
			if (Directory.Exists(parentFolder))
			{
				SetFolder(parentFolder);
				DisplayCurrentFolder();
			}
		}

		return new UpdateModel
		{
			SwitchPanelFocus = keyPressed == "left" || keyPressed == "right",
		};
	}

	private void SetFolder(string folderPath)
	{
		currentPath = folderPath;

		if (scrollHistory.TryGetValue(currentPath, out var pos))
		{
			listOffset = pos.offset;
			selectedItemIndex = pos.selected;
		}
		else
		{
			listOffset = 0;
			selectedItemIndex = 0;
		}

		firstDisplayedItemIndex = -1;
		lastDisplayedItemIndex = -1;

		var contents = GetFolderContents(folderPath);
		folderItemPaths = new List<string>();
		folderItemNames = new List<string>();
		string Shorten(string str)
		{
			// 2 chars for borders, 3 chars for left padding
			// 3 chars for "..." if needed

			if (str.Length < size.Columns - 5) return str;
			var shorter = str.Substring(0, size.Columns - 8);
			return $"{shorter}...";
		}
		foreach (var dir in contents.dirs)
		{
			folderItemPaths.Add(dir);
			folderItemNames.Add($" / {Shorten(Path.GetFileName(dir))}");
		}
		foreach (var file in contents.files)
		{
			folderItemPaths.Add(file);
			folderItemNames.Add($"   {Shorten(Path.GetFileName(file))}");
		}
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

			// color directories differently
			if (item.StartsWith(" / ")) ToggleColorPair(ColorPairs.BlueOnBlack, true);
			if (itemIndex == selectedItemIndex && IsActive) NCurses.WindowAttributeOn(windowObj, CursesAttribute.STANDOUT);
			if (itemIndex == selectedItemIndex && (!IsActive)) NCurses.WindowAttributeOn(windowObj, CursesAttribute.BOLD);

			// write line,
			// overwrite rest of the line
			// in case the current line is shorter then what it was previously
			NCurses.MoveWindowAddString(windowObj, yOffset, xOffset, item);
			ClearRestOfLine();

			ToggleColorPair(ColorPairs.BlueOnBlack, false);
			NCurses.WindowAttributeOff(windowObj, CursesAttribute.STANDOUT);
			NCurses.WindowAttributeOff(windowObj, CursesAttribute.BOLD);

			if (lineIndex == 0) firstDisplayedItemIndex = itemIndex;
			if (lineIndex == maxLineIndex - 1) lastDisplayedItemIndex = itemIndex;
			itemIndex++;
		}

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
