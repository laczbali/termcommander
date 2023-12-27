using Display.Models;
using Mindmagma.Curses;

namespace Display.Extensions;
public static class NcWindowExtensions
{
    /// <summary>
    /// Move cursor to the specified position, and write the string
    /// </summary>
    public static void WriteAtPosition(this NcWindow window, int y, int x, string str)
        => NCurses.MoveWindowAddString(window.windowObj, y, x, str);

    /// <summary>
    /// Write the string at the current cursor position
    /// </summary>
    public static void Write(this NcWindow window, string str)
        => NCurses.WindowAddString(window.windowObj, str);

    /// <summary>
    /// Move the cursor to the specified position.
    /// If a dimension is null, the cursor will not be moved in that dimension.
    /// </summary>
    public static void MoveCursor(this NcWindow window, int? y = null, int? x = null)
    {
        NCurses.GetYX(window.windowObj, out var curY, out var curX);
        NCurses.WindowMove(window.windowObj, y ?? curY, x ?? curX);
    }

    /// <summary>
    /// Toggle a CursesAttribute on or off
    /// </summary>
    public static void ToggleAttribute(this NcWindow window, uint attribute, bool on)
    {
        if (on)
        {
            NCurses.WindowAttributeOn(window.windowObj, attribute);
        }
        else
        {
            NCurses.WindowAttributeOff(window.windowObj, attribute);
        }
    }

    /// <summary>
    /// Draws the window border and title
    /// </summary>
    /// <param name="title"></param>
    /// <param name="on"></param>
    public static void ToggleBox(this NcWindow window, string? title = null, bool on = true)
    {
        if (on)
        {
            NCurses.WindowBorder(window.windowObj, (char)0, (char)0, (char)0, (char)0, (char)0, (char)0, (char)0, (char)0);
            if (title is not null)
            {
                NCurses.GetMaxYX(window.windowObj, out _, out var colCount);
                colCount -= 6; // 3 chars on each side (-- title --)
                if (title.Length > colCount)
                {
                    title = title.Substring(0, colCount);
                }
                NCurses.MoveWindowAddString(window.windowObj, 0, 2, $" {title} ");
            }
        }
        else
        {
            NCurses.WindowBorder(window.windowObj, ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ');
        }
    }

    /// <summary>
    /// Toggles the color pair on or off
    /// </summary>
    /// <param name="pair"></param>
    /// <param name="on"></param>
    public static void ToggleColorPair(this NcWindow window, int pairId, bool on)
    {
        if (TerminalEnv.InitializedColorPairIds is null)
        {
            throw new InvalidOperationException("Color pairs have not been initialized");
        }
        if (!TerminalEnv.InitializedColorPairIds.Contains(pairId))
        {
            throw new ArgumentException($"Color pair {pairId} has not been initialized");
        }

        if (on)
        {
            NCurses.WindowAttributeOn(window.windowObj, NCurses.ColorPair(pairId));
        }
        else
        {
            NCurses.WindowAttributeOff(window.windowObj, NCurses.ColorPair(pairId));
        }
    }

    /// <inheritdoc cref="ToggleColorPair(NcWindow, int, bool)"/>
    public static void ToggleColorPair(this NcWindow window, ColorPair color, bool on)
        => ToggleColorPair(window, color.Id, on);

    /// <summary>
    /// Starts at the current cursor position and writes ' ' until the end of the line
    /// </summary>
    /// <param name="rightMargin">It will leave this many characters untouched at the end of the line</param>
    public static void ClearRestOfLine(this NcWindow window, int rightMargin = 1)
    {
        NCurses.GetYX(window.windowObj, out var rowCursor, out var colCursor);
        NCurses.GetMaxYX(window.windowObj, out var maxRow, out var maxCol);
        maxCol -= rightMargin;

        for (var i = colCursor; i < maxCol; i++)
        {
            NCurses.WindowAddChar(window.windowObj, ' ');
        }
    }
}
