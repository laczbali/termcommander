using Display;
using Display.Extensions;
using Display.Models;

namespace ConsoleApp.Windows.Generic;
internal class Footer : NcWindow
{
    public static string Text { get; set; } = _defaultText;
    private static string _defaultText => "Terminal Commander - by blaczko";

    public static void SetDefaultText() => Text = _defaultText;

    protected override UpdateResult UpdateInner(string? keypressed)
    {
        this.WriteAtPosition(0, 1, Text);
        this.ClearRestOfLine();
        return new UpdateResult();
    }
}
