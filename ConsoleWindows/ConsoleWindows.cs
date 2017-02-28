using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class ConsoleWindows
  {
    #region private variables, properties

    private IConsoleMouse consoleMouse;    

    public class ConsoleWindowsViewModel
    {

    }

    public ConsoleWindowsViewModel viewModel { get; private set; } = new ConsoleWindowsViewModel();

    #endregion

    #region public variables, properties

    public Window<ConsoleWindowsViewModel> rootWindow { get; private set; }

    #endregion

    #region constructor

    public ConsoleWindows(IConsoleMouse consoleMouse = null)
    {
      Console.InputEncoding   = Encoding.Unicode;
      Console.OutputEncoding  = Encoding.Unicode;

      if (consoleMouse != null)
      {
        if (String.IsNullOrWhiteSpace(Console.Title))
        {
          Console.Title = Guid.NewGuid().ToString();
        }

        // TODO: looking for same title and add a counter to this title is any found

        consoleMouse.Init(Console.Title);
      }

      this.consoleMouse = consoleMouse;

      var area = new Area(0, 0, Console.WindowWidth, Console.WindowHeight, (WinColor)(int)Console.ForegroundColor, (WinColor)(int)Console.BackgroundColor);

      rootWindow = new Window<ConsoleWindowsViewModel>(viewModel, area);

      this.SetDefaultLayout();                                                                    // ConsoleWindowsExtension.cs
    }
    #endregion

    #region public

    public void ClearRootWindowLayout()
    {

    }

    #endregion
  }
}
