#define USE_refreshWait                                                         // for increase performance | comment of it to debug
//#define USE_traceEnabled                                                        // Write trace messages

using System;
using System.Collections.Generic;
using System.Collections;
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

    public static bool defaultSetWindowSize = true;
    private       bool setWindowSize        = defaultSetWindowSize;
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

    public VirtualConsole(string title, int rows = 25, int cols = 80, ConColor foreground = ConColor.Black, ConColor background = ConColor.White, IConsoleMouse consoleMouse = null, bool? setWindowSize = null)
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

      this.setWindowSize = setWindowSize ?? defaultSetWindowSize;

      ConsoleInit(this.setWindowSize);

      if (consoleMouse != null)
      {
        consoleMouse.Init(this);
      }
    }

    protected abstract void ConsoleInit(bool setWindowSize);
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

    #region child class Display helpers

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
    #endregion

    #region abstract

    public    abstract  void            Display();

    protected abstract  bool            KeyAvailable { get; }
    protected abstract  ConsoleKeyInfo  ReadKey();

    #endregion

    #region keyboard management

    internal ConsoleKeyInfo?  ReadKeyEx(CancellationToken cancellationToken)
    {
      while (! cancellationToken.IsCancellationRequested)
      {
        if (KeyAvailable)
        {
          ConsoleKeyInfo? cki = ReadKey();

          if ((cki != null) && (previewReadedKeyDictionary != null))
          {
            if (previewReadedKeyDictionary.ContainsKey(((ConsoleKeyInfo)cki).Key))
            {
              var func = previewReadedKeyDictionary[((ConsoleKeyInfo)cki).Key];                     // previewReadedKeyDictionary[cki.Key]?.Invoke(cki);

              if (func != null)
              {
                cki = func((ConsoleKeyInfo)cki);
              }
            }
          }

          if ((cki != null) && (previewReadedKey != null))                                          // Application process
          {
            cki = previewReadedKey((ConsoleKeyInfo)cki);
          }

          if (cki != null)                                                                          // return data to ConsoleWindows process
          {
            return cki;
          }
        }
        else
        {
          Thread.Sleep(50);
        }
      }

      return null;
    }

    /// <summary>
    /// Define a function for preview readed console key and a possibility to catch it.
    /// example: 
    /// PushPreviewReadedKey(consoleKeyInfo =>
    ///  {
    ///    if (consoleKeyInfo.Key == ConsoleKey.Print)
    ///    {
    ///      PrintContent();
    ///
    ///      return null;                                                         // catched key, do not process further
    ///    }
    ///    
    ///    return consoleKeyInfo;                                                 // pass on this key to process [but you can modify it, for example uppercase use]
    ///  });
    /// Typical usage mode apply a new function [by this] at 'Window.StartEditable()' for preview and process key types.
    /// At the same time use a RestorePreviewReadedKey() at 'Window.StopEditable()'.
    /// Warning: you can catch key before (away from) ConsoleWindows process.
    /// </summary>
    public void ApplyPreviewReadedKey(Func<ConsoleKeyInfo, ConsoleKeyInfo?> previewReadedKey)
    {
      previewReadedKeyStack.Push(this.previewReadedKey);
      this.previewReadedKey = previewReadedKey;
    }

    /// <summary>
    /// Drop a pushed function [by PushPreviewReadedKey()] for preview readed console key and a possibility to catch it.
    /// 
    /// Typical usage mode apply a new function [by ApplyPreviewReadedKey()] at 'Window.StartEditable()' for preview and process key types.
    /// At the same time use a RestorePreviewReadedKey() at 'Window.StopEditable()'.
    /// </summary>
    public void RestorePreviewReadedKey() 
    {
      if (previewReadedKeyStack.Count == 0)
      {
        throw new IndexOutOfRangeException("VirtualConsole.DropPreviewReadedKey(): there isn't pushed 'previewReadedKey' function to drop it.");
      }

      this.previewReadedKey = previewReadedKeyStack.Pop();
    }

    private   Stack<Func<ConsoleKeyInfo, ConsoleKeyInfo?>>   previewReadedKeyStack    = new Stack<Func<ConsoleKeyInfo, ConsoleKeyInfo?>>();
    private   Func<ConsoleKeyInfo, ConsoleKeyInfo?>          previewReadedKey         = null;                         // for application    

    private   Dictionary<ConsoleKey, Func<ConsoleKeyInfo, ConsoleKeyInfo?>>   previewReadedKeyDictionary = null;

    /// <summary>
    /// Watch a keypress by a function for preview readed console key and a possibility to catch it.
    /// example: 
    /// WatchPreviewReadedKey(ConsoleKey.Print, consoleKeyInfo =>
    ///  {
    ///    PrintContent(consoleWindows.actualWindow);
    ///
    ///    return null;                                                         // catched key, do not process further    
    ///  });
    /// Warning: you can catch key before (away from) ConsoleWindows process.
    /// </summary>
    ///  
    public void WatchPreviewReadedKey(ConsoleKey key, Func<ConsoleKeyInfo, ConsoleKeyInfo?> previewReadedKey)
    {
      if (previewReadedKeyDictionary == null)
      {
        previewReadedKeyDictionary = new Dictionary<ConsoleKey, Func<ConsoleKeyInfo, ConsoleKeyInfo?>>();
      }

      previewReadedKeyDictionary[key] = previewReadedKey;
    }

    public void UnwatchPreviewReadedKey(ConsoleKey key, Func<ConsoleKeyInfo, ConsoleKeyInfo?> previewReadedKey)
    {
      if ((previewReadedKeyDictionary == null) || ! previewReadedKeyDictionary.ContainsKey(key))
      {
        throw new InvalidOperationException("VirtualConsole.UnwatchPreviewReadedKey(" + key.ToString() + "): there isn't this key watching!");
      }

      previewReadedKeyDictionary.Remove(key);
    }

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
        #if USE_refreshWait
        #if USE_traceEnabled
        if (traceEnabled)
        {
          Trace.WriteLine(">> VirtualConsole.Refresh: [! refreshWait] " + lastChange.ToString());
        }
        #endif

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

          #if USE_traceEnabled
          if (traceEnabled)
          {
            Trace.WriteLine(">> VirtualConsole.InternalDisplay: " + lastRefresh.ToString());
          }
          #endif

          Display();                                                                              // abstract class
        }
      }
    }
    #endregion

    #region others

    #if USE_traceEnabled
    public static bool traceEnabled = true;
    #endif
    #endregion
  }
}
