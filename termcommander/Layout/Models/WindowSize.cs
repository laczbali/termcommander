using Mindmagma.Curses;

namespace ConsoleApp.Layout.Models;
public class WindowSize
{
	public int Rows { get; set; }
	public int Columns { get; set; }
	public int RowOrigin { get; set; }
	public int ColumnsOrigin { get; set; }

	public WindowSize GetLeftHalf() => new WindowSize
	{
		Rows = Rows,
		Columns = Columns / 2,
		RowOrigin = RowOrigin,
		ColumnsOrigin = ColumnsOrigin
	};

	public WindowSize GetRightHalf() => new WindowSize
	{
		Rows = Rows,
		Columns = Columns / 2,
		RowOrigin = RowOrigin,
		ColumnsOrigin = (Columns / 2) + 1
	};

	public WindowSize GetTopHalf() => new WindowSize
	{
		Rows = Rows / 2,
		Columns = Columns,
		RowOrigin = RowOrigin,
		ColumnsOrigin = ColumnsOrigin
	};

	public WindowSize GetBottomHalf() => new WindowSize
	{
		Rows = Rows / 2,
		Columns = Columns,
		RowOrigin = (Rows / 2) + 1,
		ColumnsOrigin = ColumnsOrigin
	};

	public static WindowSize FULLSIZE => new WindowSize
	{
		Rows = NCurses.Lines,
		Columns = NCurses.Columns,
		ColumnsOrigin = 0,
		RowOrigin = 0
	};
}
