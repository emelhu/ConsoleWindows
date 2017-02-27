using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class ConsoleWindows
  {
    #region private

    private IConsoleMouse consoleMouse;

    #endregion

    #region public

    Window rootWindow;

    #endregion


    public ConsoleWindows(string title, ConColor foregruond = ConColor.Black, ConColor backgruond = ConColor.White, IConsoleMouse consoleMouse = null)
    {
      Console.Title = title;

      if (consoleMouse != null)
      {
        consoleMouse.Init(title);
      }

      this.consoleMouse = consoleMouse;
    }
  }
}
