using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public static class VirtualConsoleExtension
  {
    public static Window<IViewModel> DefaultRootWindow(this VirtualConsole vc)
    {
      IViewModel vm = new RootWindowViewModel();

      Window<IViewModel> root = new Window<IViewModel>(vm, 0, 0, vc.cols, vc.rows, StyleIndex.RootWindow);

      return root;
    }
  }
}
