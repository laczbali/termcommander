using Display;
using Display.Models;

namespace ConsoleApp.Windows.Filesystem;
internal class FilesystemContainer : NcWindow
{
    protected override void InitializeInner()
    {
        var leftPanel = new FilesystemView();
        var rightPanel = new FilesystemView();

        leftPanel.Initialize(Size.GetHorizontalPartial(2, 0));
        rightPanel.Initialize(Size.GetHorizontalPartial(2, 1));

        Children.Add(leftPanel);
        Children.Add(rightPanel);

        Children[0].IsActive = true;
    }

    protected override UpdateResult UpdateInner(string? keypressed)
    {
        if (Children.Any(x => ((FilesystemView)x).HasActivePopup)) return new UpdateResult();

        if (keypressed == "left")
        {
            Children[0].IsActive = true;
            Children[1].IsActive = false;
        }
        if (keypressed == "right")
        {
            Children[0].IsActive = false;
            Children[1].IsActive = true;
        }

        return new UpdateResult();
    }

    protected override void DisposeInner()
    {
        Children.Clear();
    }
}
