using ConsoleApp.Layout;
using ConsoleApp.Layout.Models;

namespace ConsoleApp.App.Containers;
public class AppContainer : Container
{
	public AppContainer(WindowSize size) : base(size)
	{
	}

	protected override (Panel left, Panel right) GetStartupPanels()
	{
		return (
			new AppPanel(size.GetLeftHalf()),
			new AppPanel(size.GetRightHalf())
		);
	}
}
