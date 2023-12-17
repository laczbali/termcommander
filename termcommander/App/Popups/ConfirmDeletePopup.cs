using ConsoleApp.Layout;
using ConsoleApp.Layout.Models;
using Mindmagma.Curses;

namespace ConsoleApp.App.Popups;
public class ConfirmDeletePopup : NcWindow, IPopup
{
	private readonly bool showItemList;
	private readonly List<string> itemNames;

	public ConfirmDeletePopup(List<string> itemNames, WindowSize size) : base(size)
	{
		if (size.Columns < MinCols) throw new ArgumentException("Selected width is smaller then the minimum");
		if (size.Rows < MinRows) throw new ArgumentException("Selected height is smaller then the minimum");

		showItemList = (size.Rows - 2) >= itemNames.Count;
		this.itemNames = itemNames;
	}

	public static int MinCols => title.Length + 6;

	public static int MinRows => 3;

	private static string title => "Confirm delete - ENTER/BSP";

	public void Show()
	{
		ToggleBox(title);

		if (showItemList)
		{
			var lineIndex = 1;
			foreach (var item in itemNames)
			{
				NCurses.MoveWindowAddString(windowObj, lineIndex, 1, ShortenString(item));
				lineIndex++;
			}
		}
		else
		{
			var msg = $"Deleting {itemNames.Count} item(s)";
			NCurses.MoveWindowAddString(windowObj, 1, (size.Columns / 2) - (msg.Length / 2), msg);
		}

		NCurses.WindowRefresh(windowObj);
	}
}
