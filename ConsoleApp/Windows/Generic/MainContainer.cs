using ConsoleApp.Windows.Filesystem;
using NetCurses;
using NetCurses.Models;

namespace ConsoleApp.Windows.Generic;
internal class MainContainer : NcWindow
{
    protected override void InitializeInner()
    {
        var filesystem = new FilesystemContainer();
        filesystem.Initialize(new WindowSize(WindowSize.Fullscreen.Rows - 1, WindowSize.Fullscreen.Columns, 0, 0));
        Children.Add(filesystem);

        var footer = new Footer();
        footer.Initialize(new WindowSize(1, WindowSize.Fullscreen.Columns, WindowSize.Fullscreen.Rows - 1, 0));
        Children.Add(footer);
    }
}
