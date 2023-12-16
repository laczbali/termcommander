using ConsoleApp.Layout.Models;
using Mindmagma.Curses;

namespace ConsoleApp.Layout;
public abstract class NcWindow
{
	protected readonly WindowSize size;
	protected IntPtr windowObj;

	public NcWindow(WindowSize size)
	{
		this.size = size;

		windowObj = NCurses.NewWindow(size.Rows, size.Columns, size.RowOrigin, size.ColumnsOrigin);
		NCurses.Keypad(windowObj, true);
		NCurses.NoDelay(windowObj, true);
	}

	public void ToggleBox(bool on = true)
	{
		if (on)
		{
			NCurses.WindowBorder(windowObj, '|', '|', '-', '-', '+', '+', '+', '+');
		}
		else
		{
			NCurses.WindowBorder(windowObj, ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ');
		}

		NCurses.WindowRefresh(windowObj);
	}
}
