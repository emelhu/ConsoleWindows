using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleWindowsDemo
{
  using System.Diagnostics;
  using System.Threading;
  using eMeL.ConsoleWindows;
  using eMeL.ConsoleWindows.Core;

  public class Program
  {
    public static void Main(string[] args)
    {
      Console.Title = "ConsoleWindowsDemo | a demo for ConsoleWindows.dll | (c) eMeL, www.emel.hu";

      Console.BackgroundColor = ConsoleColor.DarkGray;
      Console.ForegroundColor = ConsoleColor.Blue;
      Console.WriteLine("**********************************************************************************");
      Console.WriteLine("*** ConsoleWindowsDemo | a demo for ConsoleWindows.dll | (c) eMeL, www.emel.hu ***");
      Console.WriteLine("**********************************************************************************");
      Console.ResetColor();

      Console.WriteLine();

      char operation;

      if (args.Length > 0)
      { // first parameter [index: 0] is the name of program
        if (args[0].Length != 1)
        {
          Console.WriteLine("Error! The first parameter isn't a single karakter of code of operation!");
          Console.WriteLine();
          DisplayHelp(true);
          Console.WriteLine();
          Console.ReadKey();                                       
          return;
        }

        operation = args[0][0];        
      }
      else
      {
        Console.WriteLine("Select an operation code:\n");
        DisplayHelp(false);
        Console.Write("\nOperation: ");

        operation = (char)Console.ReadKey().KeyChar;

        Console.WriteLine();
        Console.WriteLine();
      }
            
      //

      switch (operation)
      {
        case 'V':                                                                       // Minden tétel betöltése
        case 'v':
          ViewAndCangeTest1();
          break;        

        default:        
          Console.WriteLine("!!!! Operation code error !!!!");
          Console.WriteLine();
          DisplayHelp(true);
   
          break;
      }
    }

    public static void DisplayHelp(bool fullText)
    {
      if (fullText)
      {
        Console.WriteLine("usage: ConsoleWindowsDemo <operation code character>");
        Console.WriteLine("  Operation codes and meaning:");
      }

      Console.WriteLine("  'V' : View and change test.");
      Console.WriteLine("  '.' : ....");
      Console.WriteLine("  '.' : ....");      
    }    

    public static void ViewAndCangeTest1()
    { 
      var con = new CoreConsole("ViewAndCangeTest1 --- ConsoleWindowsDemo");

      Console.BackgroundColor = ConsoleColor.DarkBlue;
      Console.ForegroundColor = ConsoleColor.White;
      Console.SetWindowSize(80, 25);

      TextElement.defaultBackground = WinColor.DarkGray;
      TextElement.defaultForeground = WinColor.Gray;

      var conWin = new ConsoleWindows(con);

      var region = new Region(3, 10, 10, 10, WinColor.Cyan, WinColor.Green);
      var area   = new Area(  6, 20, 10, 15, WinColor.Magenta, WinColor.DarkYellow, new Border(Border.defaultBorderFramePattern1));

      conWin.rootWindow.AddElement(region);
      conWin.rootWindow.AddElement(area);

      Thread.Sleep(3000);

      var textElement1 = new TextElement(15, 40, "proba1");
      var textElement2 = new TextElement(15, 60, "proba2");

      var textElement3 = new TextElement(1, 30, "...press Escape to end...");
      var textElement4 = new TextElement(1, 60, "........", WinColor.DarkRed, WinColor.Green);

      conWin.rootWindow.AddElement(textElement2);
      conWin.rootWindow.AddElement(textElement1);
      conWin.rootWindow.AddElement(textElement4);
      conWin.rootWindow.AddElement(textElement3);
      

      region.row += 5;
      area.border = new Border(Border.defaultBorderFramePattern2);

      Thread.Sleep(3000);

      textElement1.text = "PROBA1";

      Thread.Sleep(3000);

      textElement2.text = "PROBA2";

      Thread.Sleep(3000);

      //

      int count = 0;
      ConsoleKeyInfo keyInfo;

      do
      { 
        Console.SetCursorPosition(0, 23);
        
        if (Console.KeyAvailable)
        {
          keyInfo = Console.ReadKey();
        }
        else
        {
          Thread.Sleep(1000);

          textElement4.text = (++count).ToString();
        }
      } while (keyInfo.Key != ConsoleKey.Escape);
    }
  }
}
