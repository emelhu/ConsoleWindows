//#define USE_refreshWaitTime

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public abstract class VirtualConsole
  {
    #region variables

    private char[]  buffer;
    private byte[]  colors;

    public  const   int   minRows =  10;
    public  const   int   maxRows = 200;
    public  const   int   minCols =  10;
    public  const   int   maxCols = 400;

    public  string  title { get; protected set; }

    public  int     rows  { get; private set; }
    public  int     cols  { get; private set; }

    public  ConColor  foreground  { get; private set; }
    public  ConColor  background  { get; private set; }
    #endregion

    #region refresh wait time

    /// <summary>
    /// Default value for 'refreshWaitTime' property. [milliseconds]
    /// </summary>
    public  static    int     defaultRefreshWaitTime
    {
      get { return _defaultRefreshWaitTime; }
      set { _defaultRefreshWaitTime = RefreshWaitTimeBarrier(value); }
    }
    private static    int     _defaultRefreshWaitTime = RefreshWaitTimeBarrier(200);

    public  const     int     minRefreshWaitTime      = 100;
    public  const     int     maxRefreshWaitTime      = 500;
    
    private static    int     RefreshWaitTimeBarrier(int waitTime)
    {
      return Math.Min(Math.Max(waitTime, minRefreshWaitTime), maxRefreshWaitTime);
    }

    /// <summary>
    /// The maximum length of delay, batching a package of screen refresh. [milliseconds]
    /// </summary>
    public            int     refreshWaitTime
    {
      get { return _refreshWaitTime; }
      set { _refreshWaitTime = RefreshWaitTimeBarrier(value); }
    }
    private           int     _refreshWaitTime = defaultRefreshWaitTime;

    #endregion

    #region constructor

    public VirtualConsole(string  title, int rows = 25, int cols = 80, ConColor foreground = ConColor.Black, ConColor background = ConColor.White, IConsoleMouse consoleMouse = null)
    {
      rows    = Math.Min(Math.Max(rows, minRows), maxRows);
      cols    = Math.Min(Math.Max(cols, minCols), maxCols);

      var cb  = GetColorByte(foreground, background);
      buffer  = Enumerable.Repeat(' ', rows * cols).ToArray();
      colors  = Enumerable.Repeat(cb,  rows * cols).ToArray();

      this.title      = title;
      this.rows       = rows; 
      this.cols       = cols;
      this.foreground = foreground;
      this.background = background;

      ConsoleInit();

      if (consoleMouse != null)
      {
        consoleMouse.Init(this);
      }

      #if DEBUG
      traceEnabled = true;
      #endif
    }

    protected abstract void ConsoleInit();
    #endregion

    #region Write

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

      Array.Copy(text.ToArray(), 0, buffer, position, text.Length);

      if (foreground == WinColor.None)
      {
        foreground = (WinColor)(int)this.foreground;
      }

      if (background == WinColor.None)
      {
        background = (WinColor)(int)this.background;
      }

      byte colorByte  = GetColorByte((ConColor)(int)foreground, (ConColor)(int)background);
      var  colorBytes = Enumerable.Repeat(colorByte, maxLen).ToArray();

      Array.Copy(colorBytes, 0, colors, position, text.Length);

      Refresh();
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
    #endregion

    #region keyboard

    public void Start()
    {

    }

    #endregion

    #region abstract Display

    protected char[] GetDispChars(int pos, int len)
    {
      Debug.Assert(pos >= 0);
      Debug.Assert(len >  0);
      Debug.Assert(pos         < this.rows * this.cols);
      Debug.Assert((pos + len) < this.rows * this.cols);

      var part = new char[len];

      Array.Copy(buffer, pos, part, 0, len);

      return part;
    }

    protected byte   GetColorByte(int pos)
    {
      Debug.Assert(pos >= 0);
      Debug.Assert(pos < this.rows * this.cols);

      return colors[pos];
    }

    public abstract void Display();    

    #endregion

    #region refresh display

    private volatile  bool    refreshWait     = false;   
    private volatile  int     lastRefresh     = 0;
    private volatile  int     lastChange      = 0;

    public void Refresh()
    {
      lastChange = Environment.TickCount;

      if (! refreshWait)
      {
        #if USE_refreshWaitTime
        if (traceEnabled)
        {
          Trace.WriteLine(">> VirtualConsole.Refresh: [! refreshWait] " + lastChange.ToString());
        }

        refreshWait = true;                                                                       // Signal for thereafter Refresh requests. [to be notified Display() will call]

        Task.Run(() =>
          {
            Thread.Sleep(refreshWaitTime);                                                        // Batch requests and all request (in interval) will exequted one time (with one Display() call)

            refreshWait = false;

            InternalDisplay();
          });
        #else
        refreshWait = false;
        InternalDisplay();
        #endif      
      }
    }

    private object lockConsole = new object();

    private void InternalDisplay()
    {
      lock (this.lockConsole)                                                                     // Centralized lock for all windows
      {
        if (lastChange > lastRefresh)
        {
          lastRefresh = Environment.TickCount;                                                    // start time of refresh

          if (traceEnabled)
          {
            Trace.WriteLine(">> VirtualConsole.InternalDisplay: " + lastRefresh.ToString());
          }

          Display();
        }
      }
    }
    #endregion

    #region others

    public static bool traceEnabled = false;

    #endregion
  }
}
