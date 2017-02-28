using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class ConsoleWindows
  {
    #region private

    private IConsoleMouse consoleMouse;

    #endregion

    #region public

    public Window rootWindow { get; private set; }

    #endregion


    public ConsoleWindows(string title, ConColor foreground = ConColor.Black, ConColor background = ConColor.White, IConsoleMouse consoleMouse = null)
    {
      Console.Title = title;
      Console.ForegroundColor = (ConsoleColor)(int)foreground;
      Console.BackgroundColor = (ConsoleColor)(int)background;
      Console.InputEncoding   = Encoding.Unicode;
      Console.OutputEncoding  = Encoding.Unicode;

      if (consoleMouse != null)
      {
        consoleMouse.Init(title);
      }

      this.consoleMouse = consoleMouse;

      Region region = new Region();
    }
  }
}
