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
		// - add ability to create files and directories
		// - handle if we don't have permission to access a folder

		// TODO long term:
		// - command list footer
		// - file open view
		// - command runner view
		// - terminal resize shouldn't completely break the app
		// - add ability to save/load view setups (eg open up like x-y at startup, or go to home dir from anywhere), maybe switch with number keys?

#if DEBUG
		// this is a workaround for resizing, since doing it mid-run doesn't work yet
		Console.ReadKey();
#endif

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
		using (var container = new AppContainer(WindowSize.FULLSIZE))
		{
			container.Start();
		}

		// cleanup
		NCurses.EndWin();
	}
}
