using NetCurses;
using NetCurses.Extensions;
using NetCurses.Models;

namespace ConsoleApp.Windows.Generic;
internal class ErrorPopup : NcWindow
{
    private NcWindow? _parentWindow = null;
    private bool _initLoopOver = false;
    private string _oldFooterText = string.Empty;

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

    public string InitPopup(NcWindow parentWindow, string message)
    {
        Initialize(parentWindow.Size.GetHorizontalPartial(5, 1, 3).GetVerticalPartial(3, 1));
        _parentWindow = parentWindow;
        _message = message.ShortenString(Size.Columns - 2);
        _parentWindow.Children.Add(this);
        _oldFooterText = Footer.Text;
        DisplayContents();
        return Id;
    }

    protected override UpdateResult UpdateInner(string? keypressed)
    {
        Footer.Text = "(ENT)Close";

        if (keypressed == "enter" && _initLoopOver)
        {
            Footer.Text = _oldFooterText;
            return new UpdateResult
            {
                WindowId = Id,
                RemoveSelfFromParent = true
            };
        }

        _initLoopOver = true;
        DisplayContents();
        return new UpdateResult();
    }

    private void DisplayContents()
    {
        this.ToggleBox("ERROR");

        var verticalOffset = Size.GetVerticalCenter(-1 * (_messageLines.Length / 2)) - 1;
        foreach (var item in _messageLines)
        {
            this.WriteAtPosition(verticalOffset, Size.GetHorizontalCenter(-1 * (item.Length / 2)), item);
            verticalOffset++;
        }
    }
}
