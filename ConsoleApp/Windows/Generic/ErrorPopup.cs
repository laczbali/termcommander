using Display;
using Display.Extensions;
using Display.Models;

namespace ConsoleApp.Windows.Generic;
internal class ErrorPopup : NcWindow
{
    private string _message = string.Empty;
    public string Message
    {
        get => _message;
        set
        {
            _message = value.ShortenString(Size.Columns - 2);
        }
    }

    private string[] _messageLines => Message.Split("\n");

    protected override UpdateResult UpdateInner(string? keypressed)
    {
        this.ToggleBox("ERROR");

        var verticalOffset = Size.GetVerticalCenter(-1 * (_messageLines.Length / 2)) - 1;
        foreach (var item in _messageLines)
        {
            this.WriteAtPosition(verticalOffset, Size.GetHorizontalCenter(-1 * (item.Length / 2)), item);
            verticalOffset++;
        }

        return new UpdateResult();
    }
}
