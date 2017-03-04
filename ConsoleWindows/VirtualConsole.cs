using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public abstract class VirtualConsole : IDisposable
  {
    private   MemoryStream buffer;
    private   StreamWriter bufferWriter;
    protected StreamReader bufferReader;
    private   MemoryStream colors;
    private   BinaryWriter colorsWriter;
    protected BinaryReader colorsReader;

    public  const   int   minRows =  10;
    public  const   int   maxRows = 200;
    public  const   int   minCols =  10;
    public  const   int   maxCols = 400;

    public  string  title { get; private set; }

    public  int     rows  { get; private set; }
    public  int     cols  { get; private set; }

    public  ConColor  foreground  { get; private set; }
    public  ConColor  background  { get; private set; }

    public VirtualConsole(string  title, int rows = 25, int cols = 80, ConColor foreground = ConColor.Black, ConColor background = ConColor.White)
    {
      rows = Math.Min(Math.Max(rows, minRows), maxRows);
      cols = Math.Min(Math.Max(cols, minCols), maxCols);

      buffer       = new MemoryStream(rows * cols * sizeof(char));
      bufferWriter = new StreamWriter(buffer, Encoding.Unicode);
      bufferReader = new StreamReader(buffer, Encoding.Unicode);

      colors       = new MemoryStream(rows * cols * sizeof(byte));
      colorsWriter = new BinaryWriter(colors);
      colorsReader = new BinaryReader(colors);

      this.title      = title;
      this.rows       = rows; 
      this.cols       = cols;
      this.foreground = foreground;
      this.background = background;

      ConsoleInit();
    }

    protected abstract void ConsoleInit();

    public void Write(int row, int col, string text, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
    {
      if (text == null)
      {
        throw new ArgumentException("VirtualConsole.Write(): text is null !", nameof(text));
      }

      if ((row >= this.rows) || (row < 0))
      {
        throw new ArgumentException("VirtualConsole.Write(): row=" + row + " is invalid !  [0.." + (this.rows - 1) + "]", nameof(row));
      }

      if ((col >= this.cols) || (col < 0))
      {
        throw new ArgumentException("VirtualConsole.Write(): col=" + col + " is invalid !  [0.." + (this.cols - 1) + "]", nameof(col));
      }

      var position = (row * this.cols + col);
      var maxLen   = (row * this.cols + this.cols) - position;

      if (text.Length > maxLen)
      {
        text = text.Substring(0, maxLen);
      }

      buffer.Position = position * sizeof(char);
      bufferWriter.Write(text);


      if (foreground == WinColor.None)
      {
        foreground = (WinColor)(int)this.foreground;
      }

      if (background == WinColor.None)
      {
        background = (WinColor)(int)this.background;
      }

      byte colorByte = GetColorByte((ConColor)(int)foreground, (ConColor)(int)background);

      colors.Position = position;
      for (int colorByteLoop = 0; colorByteLoop < text.Length; colorByteLoop++)
      {
        colors.WriteByte(colorByte);
      }
    }

    protected byte GetColorByte(ConColor foreground, ConColor background)
    {
      int ret = (int)foreground;

      ret = ret << 4;                                                                             // shift bits

      ret |= (int)background;                                                                     // bitwise

      return (byte)ret;
    }

    protected ConColor GetForegroundColor(byte colorByte)
    {
      int ret = colorByte;

      ret = ret >> 4;   

      return (ConColor)ret;
    }

    protected ConColor GetBackgroundColor(byte colorByte)
    {
      int ret = colorByte;

      ret &= (int)0x0F;  

      return (ConColor)ret;
    }

    public abstract void Display();    

    #region IDisposable implementation

    public void Dispose()
    {
      if (bufferWriter != null)
      {
        bufferWriter.Dispose();
        bufferWriter = null;
      }

      if (bufferReader != null)
      {
        bufferReader.Dispose();
        bufferReader = null;
      }

      if (buffer != null)
      {
        buffer.Dispose();
        buffer = null;
      }

      if (colorsWriter != null)
      {
        colorsWriter.Dispose();
        colorsWriter = null;
      }

      if (colorsReader != null)
      {
        colorsReader.Dispose();
        colorsReader = null;
      }

      if (colors != null)
      {
        colors.Dispose();
        colors = null;
      }
    }

    ~VirtualConsole()
    {
      Dispose();
    }    
    #endregion
  }
}
