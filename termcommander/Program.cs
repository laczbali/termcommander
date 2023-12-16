using ConsoleApp.App.Containers;
using ConsoleApp.Layout.Models;
using Mindmagma.Curses;

namespace termcommander;

public class Program
{
	static void Main(string[] args)
	{
		try
		{
			// init ncurses
			var screen = NCurses.InitScreen();
			NCurses.Raw();
			NCurses.NoEcho();
			NCurses.Keypad(screen, true);
			NCurses.NoDelay(screen, true);
		}
		catch (Exception e)
		{
			throw new Exception("Failed init NCurses. DLL is most likely missing." +
				"See https://github.com/MV10/dotnet-curses/?tab=readme-ov-file#the-native-library", e);
		}

		// main app
		new AppContainer(WindowSize.FULLSIZE).Start();

		// cleanup
		NCurses.EndWin();
	}
}