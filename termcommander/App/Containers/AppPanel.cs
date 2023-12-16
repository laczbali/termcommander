using ConsoleApp.App.Views;
using ConsoleApp.Layout;
using ConsoleApp.Layout.Models;

namespace ConsoleApp.App.Containers;
public class AppPanel : Panel
{
	public AppPanel(WindowSize size) : base(size)
	{
	}

	public override List<View> SetStartupViews()
	{
		return new List<View>() {
			new FilesystemView(size.GetTopHalf()),
			new FilesystemView(size.GetBottomHalf())
		};
	}
}
