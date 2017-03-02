using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWindowsDemo
{
  using System.Diagnostics;
  using System.Threading;
  using eMeL.ConsoleWindows;

  public class Program
  {
    public static void Main(string[] args)
    {
      Console.BackgroundColor = ConsoleColor.DarkBlue;
      Console.ForegroundColor = ConsoleColor.White;
      Console.Title = "ConsoleWindowsDemo | a demo for ConsoleWindows.dll | (c) eMeL, www.emel.hu";

      Console.SetWindowSize(90, 30);

      TextElement.defaultBackground = WinColor.DarkGray;
      TextElement.defaultForeground = WinColor.Gray;

      var conWin = new ConsoleWindows();

      var region = new Region(10, 10, 10, 10, WinColor.Cyan, WinColor.Green);
      var area = new Area(20, 20, 10, 10, WinColor.Magenta, WinColor.DarkYellow, new Border(Border.defaultBorderFramePattern1));

      conWin.rootWindow.AddElement(region);
      conWin.rootWindow.AddElement(area);

      Thread.Sleep(3000);

      var textElement1 = new TextElement(40, 40, "proba1");
      var textElement2 = new TextElement(40, 40, "proba2");

      conWin.rootWindow.AddElement(textElement1);
      conWin.rootWindow.AddElement(textElement2);

      region.top += 20;
      area.border = new Border(Border.defaultBorderFramePattern2);

      Thread.Sleep(3000);

      Console.WriteLine("PROBA1");
      textElement1.text = "PROBA1";

      Thread.Sleep(3000);

      Console.WriteLine("PROBA2");
      textElement2.text = "PROBA2";

      Thread.Sleep(3000);

      Console.WriteLine("...press Enter to end...");
      Console.ReadLine();
    }
  }
}
