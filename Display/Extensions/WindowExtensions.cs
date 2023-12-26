using Mindmagma.Curses;

namespace Display.Extensions;
public static class WindowExtensions
{
    /// <summary>
    /// Draws the window border and title
    /// </summary>
    /// <param name="title"></param>
    /// <param name="on"></param>
    public static void ToggleBox(this IntPtr windowObj, string? title = null, bool on = true)
    {
        if (on)
        {
            NCurses.WindowBorder(windowObj, (char)0, (char)0, (char)0, (char)0, (char)0, (char)0, (char)0, (char)0);
            if (title is not null)
            {
                NCurses.GetMaxYX(windowObj, out _, out var colCount);
                colCount -= 6; // 3 chars on each side (-- title --)
                if (title.Length > colCount)
                {
                    title = title.Substring(0, colCount);
                }
                NCurses.MoveWindowAddString(windowObj, 0, 2, $" {title} ");
            }
        }
        else
        {
            NCurses.WindowBorder(windowObj, ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ');
        }
    }

    /// <summary>
    /// Toggles the color pair on or off
    /// </summary>
    /// <param name="pair"></param>
    /// <param name="on"></param>
    public static void ToggleColorPair(this IntPtr windowObj, int pairId, bool on)
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
            NCurses.WindowAttributeOn(windowObj, NCurses.ColorPair(pairId));
        }
        else
        {
            NCurses.WindowAttributeOff(windowObj, NCurses.ColorPair(pairId));
        }
    }

    /// <summary>
    /// Starts at the current cursor position and writes ' ' until the end of the line
    /// </summary>
    /// <param name="rightMargin">It will leave this many characters untouched at the end of the line</param>
    public static void ClearRestOfLine(this IntPtr windowObj, int rightMargin = 1)
    {
        NCurses.GetMaxYX(windowObj, out _, out var colCount);
        NCurses.GetYX(windowObj, out _, out var colCursor);
        for (var i = colCursor; i < colCount - rightMargin; i++)
        {
            NCurses.WindowAddChar(windowObj, ' ');
        }
    }
}
