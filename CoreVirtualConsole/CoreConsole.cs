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

  public class CoreConsole : VirtualConsole, IDisposable
  {
    public CoreConsole(string  title, int rows = 25, int cols = 80, ConColor foreground = ConColor.Black, ConColor background = ConColor.White)
      : base(title, rows, cols, foreground, background)
    {
    }

    public override void Display()
    {
      Console.SetCursorPosition(0, 0);

      colorsReader.BaseStream.Position = 0;                                   

      int   colorByte;
      int   colorPacketByte  = -1;                                                                // Start state signal, this will not a color change
      long  colorPacketStart = -1;                                                                // Start state signal, this will not a color change

      while ((colorByte = colorsReader.BaseStream.ReadByte()) >= 0)
      {
        if (colorPacketByte != colorByte)
        { // This is a color change.
          if (colorPacketStart >= 0)
          { // This is a valid color change, content display required.
            int col = (int)(colorPacketStart % this.cols);
            int row = (int)(colorPacketStart / this.cols);

            Debug.Assert((col >= 0) && (col < this.cols));
            Debug.Assert((row >= 0) && (row < this.rows));

            Console.SetCursorPosition(col, row);
            Console.ForegroundColor = (ConsoleColor)(int)GetForegroundColor((byte)colorPacketByte);
            Console.BackgroundColor = (ConsoleColor)(int)GetBackgroundColor((byte)colorPacketByte);

            long len = colorsReader.BaseStream.Position - colorPacketStart;

            Debug.Assert(len > 0);

            bufferReader.BaseStream.Position = colorsReader.BaseStream.Position * sizeof(char);

            char[] chars    = new char[len];             
            int readedChars = bufferReader.Read(chars, 0, (int)len);

            Debug.Assert(readedChars == len);

            Console.Write(chars);   
          }

          colorPacketByte  = colorByte;                                                       
          colorPacketStart = colorsReader.BaseStream.Position;
        }
      }      
    }

    protected override void ConsoleInit()
    {
      if ((Console.BufferWidth < this.rows) && (Console.BufferHeight < this.cols))
      {
        Console.SetBufferSize(this.cols, this.rows);
      }

      Console.InputEncoding   = Encoding.Unicode;
      Console.OutputEncoding  = Encoding.Unicode;
      Console.ForegroundColor = (ConsoleColor)(int)this.foreground;
      Console.BackgroundColor = (ConsoleColor)(int)this.background;
      Console.Title           = this.title;
    }
  }
}
