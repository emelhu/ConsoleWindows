using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWindowsDemo
{
  using eMeL.ConsoleWindows;

  public class Program
  {
    public static void Main(string[] args)
    {
      Console.BackgroundColor = ConsoleColor.DarkBlue;
      Console.ForegroundColor = ConsoleColor.White;
      Console.Title           = "ConsoleWindowsDemo.exe | a demo for ConsoleWindows.dll | (c) eMeL, www.emel.hu";

      var conWin  = new ConsoleWindows();

      Console.ReadLine();
    }
  }
}
