using ConsoleApp.Layout.Models;
using Mindmagma.Curses;

namespace ConsoleApp.Layout;
public abstract class NcWindow
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

	public void ToggleBox(bool on = true)
	{
		if (on)
		{
			NCurses.WindowBorder(windowObj, (char)0, (char)0, (char)0, (char)0, (char)0, (char)0, (char)0, (char)0);
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

	public enum ColorPairs
	{
		BlueOnBlack = 1,
		RedOnBlack = 2,
	}
}
