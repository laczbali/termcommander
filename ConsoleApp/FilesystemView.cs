using Display;
using Display.Extensions;
using Display.Models;

namespace ConsoleApp;
internal class FilesystemView : NcWindow
{
    protected override UpdateResult UpdateInner(string? keypressed)
    {
        this.ToggleBox($"ACTIVE - {IsActive}");
        return new UpdateResult();
    }
}
