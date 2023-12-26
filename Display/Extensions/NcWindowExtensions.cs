using Mindmagma.Curses;

namespace Display.Extensions;
public static class NcWindowExtensions
{
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

    /// <summary>
    /// Starts at the current cursor position and writes ' ' until the end of the line
    /// </summary>
    /// <param name="rightMargin">It will leave this many characters untouched at the end of the line</param>
    public static void ClearRestOfLine(this NcWindow window, int rightMargin = 1)
    {
        NCurses.GetMaxYX(window.windowObj, out _, out var colCount);
        NCurses.GetYX(window.windowObj, out _, out var colCursor);
        for (var i = colCursor; i < colCount - rightMargin; i++)
        {
            NCurses.WindowAddChar(window.windowObj, ' ');
        }
    }
}
