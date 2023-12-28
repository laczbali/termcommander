using ConsoleApp.Models;
using ConsoleApp.Windows.Generic;
using Display;

Console.WriteLine("Terminal Commander - by blaczko");
Console.WriteLine("Press any key to continue");
Console.ReadKey();

TerminalEnv.InitColors(Colors.AsList);
TerminalEnv.Execute<MainContainer>();

/*
 * TODOs
 *  delete items
 *  copy/move selected items between panes
 *  create files and directories
 *  
 *  command list footer
 *  open files
 *  command runner ?
 */
