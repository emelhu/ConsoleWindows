using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public static class ConsoleWindowsExtensions
  {
    public static void SetDefaultLayout(this ConsoleWindows consoleWindows)
    {
      consoleWindows.ClearRootWindowLayout();

      // consoleWindows.rootWindow.layout.add()
    }

    public static void SetLayout_BorderFrameDouble(this ConsoleWindows consoleWindows)
    {
      consoleWindows.ClearRootWindowLayout();

      consoleWindows.rootWindow.border = new Border(Border.defaultBorderFrameDouble);

      // consoleWindows.rootWindow.layout.add()
    }
  }
}
