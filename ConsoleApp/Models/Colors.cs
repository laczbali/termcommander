using Display.Models;
using Mindmagma.Curses;

namespace ConsoleApp.Models;

internal static class Colors
{
    internal static List<ColorPair> AsList => new List<ColorPair>
    {
        BlueOnBlack,
        RedOnBlack,
        GreenOnBlack
    };

    internal static ColorPair BlueOnBlack => new ColorPair
    {
        Id = 1,
        Foreground = CursesColor.BLUE,
        Background = CursesColor.BLACK
    };

    internal static ColorPair RedOnBlack => new ColorPair
    {
        Id = 2,
        Foreground = CursesColor.RED,
        Background = CursesColor.BLACK
    };

    internal static ColorPair GreenOnBlack => new ColorPair
    {
        Id = 3,
        Foreground = CursesColor.GREEN,
        Background = CursesColor.BLACK
    };
}
