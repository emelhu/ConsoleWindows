//#define USE_traceEnabled                                                        // Write trace messages

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    /// <param name="styles">Styles for windows and items.</param>
    /// <param name="foreground">Default color of foreground.</param>
    /// <param name="background">Default color of background.</param>
    public CoreConsole(string  title, int rows = 25, int cols = 80, Styles styles = null, ConColor? foreground = null, ConColor? background = null)
      : base(title, ((rows < 1) ? Console.BufferHeight : rows), ((cols < 1) ? Console.BufferWidth : cols), styles)
    {
      Style rootStyle = this.styles[StyleIndex.RootWindow];                                            // struct

      if (foreground != null)
      {
        rootStyle.foreground = (WinColor)(int)foreground;
      }

      if (background != null)
      {
        rootStyle.background = (WinColor)(int)background;
      }

      if (rootStyle.foreground == WinColor.None)
      {
        rootStyle.foreground = (WinColor)((foreground != null) ? (int)foreground : (int)Style.defaultForeground);
      }

      if (rootStyle.background == WinColor.None)
      {
        rootStyle.background = (WinColor)((background != null) ? (int)background : (int)Style.defaultBackground);
      }

      this.styles[StyleIndex.RootWindow] = rootStyle;  

      #if USE_traceEnabled
      traceEnabled = true;
      #endif
    }

    /// <summary>
    /// Create a VirtualConsole for DotNet Core with attributions.
    /// </summary>
    /// <param name="title">Title of window.</param>
    /// <param name="rows">Count of rows in window.</param>
    /// <param name="styles">Styles for windows and items.</param>
    /// <param name="cols">Count of cols in window.</param>
    /// <param name="foreground">Default color of foreground.</param>
    /// <param name="background">Default color of background.</param>
    public CoreConsole(string  title, Styles styles, int rows = 25, int cols = 80, ConColor? foreground = null, ConColor? background = null)
      : this(title, ((rows < 1) ? Console.BufferHeight : rows), ((cols < 1) ? Console.BufferWidth : cols), styles, foreground, background)
    {
    }

    protected override bool KeyAvailable
    {
      get
      {
        return Console.KeyAvailable;
      }
    }

    protected override ConsoleKeyInfo ReadKey()
    {
      return Console.ReadKey(true);
    }

    public override void Display()
    {
      if (traceEnabled)
      {
        Trace.WriteLine(">> CoreConsole.Display: " + Environment.TickCount.ToString());
      }

      Console.SetCursorPosition(0, 0);                                

      int   colorByte        = 0;
      int   colorPacketByte  = GetPackedStyle(0);                                                   // "charLoop == 0" situation
      int   colorPacketStart = 0;                                                                 // "charLoop == 0" situation                                                               
      int   maxPosition      = this.rows * this.cols;
      
      for (int charLoop = 1; charLoop <= maxPosition; charLoop++)                                 // Warning, not 0 because variables initialized at 0
      {
        if (charLoop < maxPosition)
        {                                                                                         // (charLoop == maxPosition) is possible but for display last block too.
          colorByte = GetPackedStyle(charLoop);
        }

        if ((colorPacketByte != colorByte) || ((charLoop % this.cols) == 0) || (charLoop == maxPosition))
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

          var chars = GetDispChars(colorPacketStart, len);
          Console.Write(chars);

          if (traceEnabled)
          {
            string text = new string(chars);
            Trace.WriteLine(String.Format(">> CoreConsole.Display: (Console.Write) {0,2}:{1,-2}|{2:X}/{3}/{4}", row, col, colorPacketByte, text, text.Length));
          }

          colorPacketByte  = colorByte;
          colorPacketStart = charLoop;         
        }
      }      
    }

    protected override void ConsoleInit(bool setWindowSize)
    {
      if ((Console.BufferWidth < this.cols) && (Console.BufferHeight < this.rows))
      {
        Console.SetBufferSize(this.cols, this.rows);
      }
      else
      {
        if (Console.BufferWidth < this.cols)
        {
          Console.SetBufferSize(this.cols, Console.BufferHeight);
        }
        else if (Console.BufferHeight < this.rows)
        {
          Console.SetBufferSize(Console.BufferWidth, this.rows);
        }
      }

      if (setWindowSize)
      {
        Console.SetWindowSize(this.cols, this.rows);
      }

      if (String.IsNullOrWhiteSpace(this.title))
      {
        this.title = Environment.GetCommandLineArgs()[0];        
      } 

      Style style = styles[StyleIndex.RootWindow];

      style.NormalizeInsufficient();

      Console.InputEncoding   = Encoding.Unicode;
      Console.OutputEncoding  = Encoding.Unicode;
      Console.ForegroundColor = (ConsoleColor)(int)style.sufficientForeground;
      Console.BackgroundColor = (ConsoleColor)(int)style.sufficientBackground;
      Console.Title           = this.title;
    }

    public override void SetCursorPosition(Position position)
    {
      Console.SetCursorPosition(position.col, position.row);
    }

    #region others

    public static bool traceEnabled = false;

    #endregion
  }
}
