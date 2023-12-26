using Display.Models;
using Mindmagma.Curses;

namespace Display;

public abstract class NcWindow
{
    public bool IsActive = false;

    private IntPtr _windowObj = IntPtr.Zero;
    internal IntPtr windowObj => _windowObj;
    private WindowSize _size = WindowSize.Null;

    protected bool isInitialized => _windowObj != IntPtr.Zero;
    protected WindowSize Size => _size;
    protected List<NcWindow> Children = new();

    /// <summary>
    /// Run any necessary startup code here.
    /// </summary>
    /// <param name="softInit">If set to true, we are just reloading. Otherwise it is a fresh start.</param>
    protected virtual void InitializeInner() { }
    /// <summary>
    /// Run the core logic of the window here
    /// </summary>
    protected virtual UpdateResult UpdateInner(string? keypressed) { return new UpdateResult(); }
    /// <summary>
    /// Run any necessary cleanup code here.
    /// </summary>
    /// <param name="softDispose">If set to true, we are just reloading. Otherwise we are stopping for good.</param>
    protected virtual void DisposeInner() { }

    /// <summary>
    /// Creates and applies default config to a new window, <br/>
    /// then calls InitializeInner
    /// </summary>
    public void Initialize(WindowSize size)
    {
        if (isInitialized) return;

        _size = size;
        _windowObj = NCurses.NewWindow(size.Rows, size.Columns, size.RowOrigin, size.ColumnsOrigin);
        NCurses.Keypad(_windowObj, true);
        NCurses.NoDelay(_windowObj, true);

        InitializeInner();
    }

    /// <summary>
    /// Calls UpdateInner, refresh, then calls Update on all children
    /// </summary>
    public void Update(string? keypressed)
    {
        var result = UpdateInner(keypressed);
        if (result.RefreshWindow) NCurses.WindowRefresh(_windowObj);
        Children?.ForEach(x => x?.Update(keypressed));
    }

    /// <summary>
    /// 1. Calls Dispose on all children    <br/>
    /// 2. Calls virtual DisposeInner       <br/>
    /// 3. Deletes the window object        <br/>
    /// </summary>
    public void Dispose()
    {
        Children?.ForEach(x => x?.Dispose());
        DisposeInner();
        NCurses.DeleteWindow(_windowObj);
        _windowObj = IntPtr.Zero;
    }
}
