using ConsoleApp.Layout.Models;
using Mindmagma.Curses;

namespace ConsoleApp.Layout;
public abstract class NcWindow : IDisposable
{
	private static bool colorsInitialized = false;

	protected readonly WindowSize size;
	protected IntPtr windowObj;

	public NcWindow(WindowSize size)
	{
		InitColors();

		this.size = size;

		windowObj = NCurses.NewWindow(size.Rows, size.Columns, size.RowOrigin, size.ColumnsOrigin);
		NCurses.Keypad(windowObj, true);
		NCurses.NoDelay(windowObj, true);
	}

	public void ToggleBox(string? title = null, bool on = true)
	{
		if (on)
		{
			NCurses.WindowBorder(windowObj, (char)0, (char)0, (char)0, (char)0, (char)0, (char)0, (char)0, (char)0);
			if (title is not null)
			{
				NCurses.MoveWindowAddString(windowObj, 0, 2, $" {title} ");
			}
		}
		else
		{
			NCurses.WindowBorder(windowObj, ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ');
		}

		NCurses.WindowRefresh(windowObj);
	}

	public void ToggleColorPair(ColorPairs pair, bool on)
	{
		if (on)
		{
			NCurses.WindowAttributeOn(windowObj, NCurses.ColorPair((int)pair));
		}
		else
		{
			NCurses.WindowAttributeOff(windowObj, NCurses.ColorPair((int)pair));
		}
	}

	public void ClearRestOfLine(int rightMargin = 1, bool refresh = false)
	{
		NCurses.GetYX(windowObj, out _, out var col);
		for (var i = col; i < size.Columns - rightMargin; i++)
		{
			NCurses.WindowAddChar(windowObj, ' ');
		}

		if (refresh)
			NCurses.WindowRefresh(windowObj);
	}

	public static void InitColors()
	{
		if (colorsInitialized) return;
		colorsInitialized = true;

		NCurses.StartColor();
		NCurses.InitPair((short)ColorPairs.BlueOnBlack, CursesColor.BLUE, CursesColor.BLACK);
		NCurses.InitPair((short)ColorPairs.RedOnBlack, CursesColor.RED, CursesColor.BLACK);
	}

	public string ShortenString(string str, int? maxLength = null)
	{
		// 2 chars for borders, 3 chars for left padding
		// 3 chars for "..." if needed
		if (maxLength is null)
		{
			maxLength = size.Columns - 5;
		}

		if (str.Length < maxLength) return str;
		var shorter = str.Substring(0, (int)maxLength - 3);
		return $"{shorter}...";
	}

	/// <summary>
	/// Clears the popup from the screen, disposes of the window object
	/// </summary>
	/// <returns></returns>
	public virtual void Dispose()
	{
		ToggleBox(on: false);
		NCurses.DeleteWindow(windowObj);
	}

	public enum ColorPairs
	{
		BlueOnBlack = 1,
		RedOnBlack = 2,
	}
}
