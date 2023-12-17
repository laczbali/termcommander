using ConsoleApp.App.Containers;
using ConsoleApp.Layout.Models;
using Mindmagma.Curses;

namespace termcommander;

public class Program
{
	static void Main(string[] args)
	{

		// TODO next steps:
		// - add ability to copy-move selected items between panes (use events, maybe)
		// - handle if we don't have permission to access a folder

		// TODO long term:
		// - terminal resize shouldn't completely break the app

		try
		{
			// init ncurses
			var screen = NCurses.InitScreen();
			NCurses.Raw();
			NCurses.NoEcho();
			NCurses.CBreak();
			NCurses.Keypad(screen, true);
			NCurses.NoDelay(screen, true);
			NCurses.Refresh();
		}
		catch (Exception e)
		{
			throw new Exception("Failed init NCurses." +
				"See https://github.com/MV10/dotnet-curses/?tab=readme-ov-file#the-native-library", e);
		}

		// main app
		new AppContainer(WindowSize.FULLSIZE).Start();

		// cleanup
		NCurses.EndWin();
	}
}
