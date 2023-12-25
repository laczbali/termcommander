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
        this.size = size;
        InitColors();
        Init();
    }

    /// <summary>
    /// Draws the window border and title
    /// </summary>
    /// <param name="title"></param>
    /// <param name="on"></param>
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

    /// <summary>
    /// Toggles the color pair on or off
    /// </summary>
    /// <param name="pair"></param>
    /// <param name="on"></param>
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

    /// <summary>
    /// Starts at the current cursor position and writes ' ' until the end of the line
    /// </summary>
    /// <param name="rightMargin"></param>
    /// <param name="refresh"></param>
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

    /// <summary>
    /// Initializes the color pairs (if not already initialized)
    /// </summary>
    public static void InitColors()
    {
        if (colorsInitialized) return;
        colorsInitialized = true;

        NCurses.StartColor();
        NCurses.InitPair((short)ColorPairs.BlueOnBlack, CursesColor.BLUE, CursesColor.BLACK);
        NCurses.InitPair((short)ColorPairs.RedOnBlack, CursesColor.RED, CursesColor.BLACK);
    }

    /// <summary>
    /// If the string is longer than the max length, shorten it and add "..."
    /// <br/>
    /// If maxLength is null, use the window's width
    /// </summary>
    /// <param name="str"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
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
    /// Creates the window object, sets up keypad and no-delay
    /// </summary>
    private void Init()
    {
        if (windowObj != IntPtr.Zero)
        {
            throw new InvalidOperationException("Window already initialized");
        }

        windowObj = NCurses.NewWindow(size.Rows, size.Columns, size.RowOrigin, size.ColumnsOrigin);
        NCurses.Keypad(windowObj, true);
        NCurses.NoDelay(windowObj, true);
    }

    /// <summary>
    /// Clears the popup from the screen, disposes of the window object
    /// </summary>
    /// <returns></returns>
    public virtual void Dispose()
    {
        ToggleBox(on: false);
        NCurses.DeleteWindow(windowObj);
        windowObj = IntPtr.Zero;
    }

    public enum ColorPairs
    {
        BlueOnBlack = 1,
        RedOnBlack = 2,
    }
}
