using ConsoleApp.Layout;
using ConsoleApp.Layout.Models;
using Mindmagma.Curses;

namespace ConsoleApp.App.Views;

/// <summary>
/// Show, navigate and modify the filesystem
/// </summary>
public class FilesystemView : View
{
	public FilesystemView(WindowSize size) : base(size)
	{
	}

	public override int MaxCountPerPanel => 1;

	public override void FocusLost()
	{
		NCurses.MoveWindowAddString(windowObj, 0, 0, "FocusLost");
		NCurses.WindowRefresh(windowObj);
	}

	protected override void FocusGained()
	{
		NCurses.MoveWindowAddString(windowObj, 0, 0, "FocusGained");
		NCurses.WindowRefresh(windowObj);
	}

	protected override UpdateModel UpdateInner(string? keyPressed)
	{
		ToggleBox();

		return new UpdateModel
		{
			SwitchPanelFocus = keyPressed == "left" || keyPressed == "right",
			SwitchViewFocus = keyPressed switch
			{
				"up" => -1,
				"down" => 1,
				_ => 0
			}
		};
	}
}
