using Display.Components;
using Display.Extensions;
using Display.Models;

namespace ConsoleApp.Windows.Filesystem;
internal class FilesystemView : ScrollMenu
{
    private string _currentPath = string.Empty;
    private List<string> _files = new List<string>();
    private List<string> _dirs = new List<string>();

    protected override void InitializeInner()
    {
        var startupPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        SelectFolder(startupPath);
    }

    protected override UpdateResult UpdateInner(string? keypressed)
    {
        this.ToggleBox(_currentPath);
        return new UpdateResult();
    }

    private void SelectFolder(string path)
    {
        if (path is null || path == string.Empty) throw new ArgumentException("Path must not be empty");
        _currentPath = path;

        var contents = GetFolderContents(_currentPath);
        _dirs = contents.dirs;
        _files = contents.files;
    }

    private (List<string> dirs, List<string> files) GetFolderContents(string folder)
    {
        var dirs = Directory.GetDirectories(folder).ToList();
        var files = Directory.GetFiles(folder).ToList();
        return (dirs, files);
    }
}
