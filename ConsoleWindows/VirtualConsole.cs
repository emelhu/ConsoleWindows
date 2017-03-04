using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class VirtualConsole : IDisposable
  {
    private MemoryStream buffer;
    private StreamWriter writer;
    private StreamReader reader;
    private MemoryStream colors;

    public  const   int   minRows =  10;
    public  const   int   maxRows = 200;
    public  const   int   minCols =  10;
    public  const   int   maxCols = 400;

    public  string  title { get; private set; }

    public  int     rows  { get; private set; }
    public  int     cols  { get; private set; }

    public  ConsoleColor  foreground  { get; private set; }
    public  ConsoleColor  background  { get; private set; }

    public VirtualConsole(string  title, int rows = 25, int cols = 80, ConsoleColor foreground = ConsoleColor.Black, ConsoleColor background = ConsoleColor.White)
    {
      rows = Math.Min(Math.Max(rows, minRows), maxRows);
      cols = Math.Min(Math.Max(cols, minCols), maxCols);

      buffer = new MemoryStream(rows * cols * sizeof(char));
      writer = new StreamWriter(buffer);

      colors = new MemoryStream(rows * cols * sizeof(byte));

      this.title      = title;
      this.rows       = rows; 
      this.cols       = cols;
      this.foreground = foreground;
      this.background = background;

      ConsoleInit();
    }

    private void ConsoleInit()
    {
      Console.SetBufferSize(this.cols, this.rows);
      Console.InputEncoding   = Encoding.Unicode;
      Console.OutputEncoding  = Encoding.Unicode;
      Console.ForegroundColor = this.foreground;
      Console.BackgroundColor = this.background;
      Console.Title           = this.title;
    }

    public void Write(int row, int col, string text)
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
      writer.Write(text);

      colors.Position = position;
      // TODO: colors.write()
      int dummy = 0;         // TODO
    }

    public void Display()
    {
      Console.SetCursorPosition(0, 0);

      buffer.Position = 0;                                                    // TODO: It's a simple way only! Not the final version.
      Console.Write(reader.ReadToEnd());
    }

    #region IDisposable implementation

    public void Dispose()
    {
      if (writer != null)
      {
        writer.Dispose();
        writer = null;
      }

      if (reader != null)
      {
        reader.Dispose();
        reader = null;
      }

      if (buffer != null)
      {
        buffer.Dispose();
        buffer = null;
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
