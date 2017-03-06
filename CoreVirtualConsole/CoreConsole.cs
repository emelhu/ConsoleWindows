using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// System.Console

namespace eMeL.ConsoleWindows.Core
{
  using System.Diagnostics;
  using System.Text;
  using eMeL.ConsoleWindows;

  public class CoreConsole : VirtualConsole
  {
    /// <summary>
    /// Create a VirtualConsole for DotNet Core with attributions.
    /// </summary>
    /// <param name="title">Title of window.</param>
    /// <param name="rows">Count of rows in window.</param>
    /// <param name="cols">Count of cols in window.</param>
    /// <param name="foreground">Default color of foreground.</param>
    /// <param name="background">Default color of background.</param>
    public CoreConsole(string  title, int rows = 25, int cols = 80, ConColor foreground = ConColor.Black, ConColor background = ConColor.White)
      : base(title, ((rows == 0) ? Console.BufferHeight : rows), ((cols == 0) ? Console.BufferWidth : cols), foreground, background)
    {
    }

    public override void Display()
    {
      Console.SetCursorPosition(0, 0);                                

      int   colorByte;
      int   colorPacketByte  = GetColorByte(0);                                       // "charLoop == 0" situation
      int   colorPacketStart = 0;                                                     // "charLoop == 0" situation                                                               

      for (int charLoop = 1; charLoop < (this.rows * this.cols); charLoop++)          // Warning, not 0 because variables initialized at 0
      {       
        colorByte = GetColorByte(charLoop);

        if ((colorPacketByte != colorByte) || ((charLoop % this.cols) == 0))
        { // This is a color change or start a new line.
          int col = (int)(colorPacketStart % this.cols);
          int row = (int)(colorPacketStart / this.cols);

          Debug.Assert((col >= 0) && (col < this.cols));
          Debug.Assert((row >= 0) && (row < this.rows));

          Console.SetCursorPosition(col, row);
          Console.ForegroundColor = (ConsoleColor)(int)GetForegroundColor((byte)colorPacketByte);
          Console.BackgroundColor = (ConsoleColor)(int)GetBackgroundColor((byte)colorPacketByte);

          int len = charLoop - colorPacketStart;
          Debug.Assert(len > 0);

          Console.Write(GetDispChars(colorPacketStart, len));

          colorPacketByte  = colorByte;
          colorPacketStart = charLoop;         
        }
      }      
    }

    protected override void ConsoleInit()
    {
      if ((Console.BufferWidth < this.cols) && (Console.BufferHeight < this.rows))
      {
        Console.SetBufferSize(this.cols, this.rows);
      }

      if (String.IsNullOrWhiteSpace(this.title))
      {
        this.title = Environment.GetCommandLineArgs()[0];        
      } 

      Console.InputEncoding   = Encoding.Unicode;
      Console.OutputEncoding  = Encoding.Unicode;
      Console.ForegroundColor = (ConsoleColor)(int)this.foreground;
      Console.BackgroundColor = (ConsoleColor)(int)this.background;
      Console.Title           = this.title;
    }
  }
}
