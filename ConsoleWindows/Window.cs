using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class Window<TViewModel> : Area, IArea, IRegion, IDisposable
     where TViewModel : IViewModel
  {
    #region data

    public  TViewModel viewModel   { get; private set; }

    public  Window<IViewModel> parentWindow
    {
      get           { return _parentWindow; }

      internal set  { _parentWindow = value; }      
    }
    private Window<IViewModel> _parentWindow;

    public  ConsoleWindows     consoleWindows
    {
      get
      {
        if (_consoleWindows != null)                                                              // Warning: only internal field store data. (Only rootWindow's _consoleWindows contains data)
        {
          return _consoleWindows;
        }

        if (parentWindow != null)
        {
          return parentWindow.consoleWindows;                                                     // Warning: only public property can 'seek' stored data with recursive call.
        }

        return null;                                                                              // Not found
      }
    }
    internal ConsoleWindows _consoleWindows { get; set; }

    public bool isRootWindow
    {
      get
      {
        #if DEBUG
        if (_consoleWindows != null)
        {
          Debug.Assert(_parentWindow == null);
        }
        #endif

        return (_consoleWindows != null);
      }
    } 
    
    #endregion

    #region constructor

    public Window(TViewModel viewModel, IRegion region, Border? border = null, Scrollbars? scrollbars = null)
      : this(viewModel, region.row, region.col, region.width, region.height, region.foreground, region.background, border, scrollbars)
    {
    }

    public Window(TViewModel viewModel, IArea area)
      : this(viewModel, area.row, area.col, area.width, area.height, area.foreground, area.background, area.border, area.scrollbars)
    {
    }

    public Window(TViewModel viewModel, int row, int col, int width, int height, WinColor foreground = WinColor.None, WinColor background = WinColor.None, Border? border = null, Scrollbars? scrollbars = null)
      : base (row, col, width, height, foreground, background, border, scrollbars)
    {  
      this.viewModel = viewModel;    

      if (this.viewModel == null)
      {
        throw new ArgumentException("The 'viewModel' must an IViewModel successor!", nameof(this.viewModel));
      }

      state = State.Editable;                                                                     // default state

      #if DEBUG
      traceEnabled = true;
      #endif
    }
    
    public Window(TViewModel viewModel, int row, int col, int width, int height, Border? border = null, Scrollbars? scrollbars = null, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
      : this(viewModel, row, col, width, height, foreground, background, border, scrollbars)
    {
    }

    #endregion

    #region elements

    private List<IRegion> elements        = new List<IRegion>();

    /// <summary>
    /// Add elements by tab&validate order.
    /// </summary>
    /// <param name="element">An IRegion or IElement object to display it on this window area (part of Console).</param>
    public void AddElement(IRegion element)
    {
      if (element == null)
      {
        throw new ArgumentException("There is null argumentum!", nameof(element));
      }

      elements.Add(element);

      if (traceEnabled)
      {
        Trace.WriteLine(">> Window.AddElement: " + Environment.TickCount.ToString() + " | " + element.GetType().ToString());
      }

      element.Changed += new ChangedEventHandler(ChangedEvent);

      var childWindow = element as Window<IViewModel>;

      if (childWindow != null)
      {
        childWindow.parentWindow = this as Window<IViewModel>;;
      }
      
      Refresh();
    }
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

    #region refresh display

    private volatile  bool    refreshWait     = false;   
    private volatile  int     lastRefresh     = 0;
    private volatile  int     lastChange      = 0;

    public void Refresh()
    {
      lastChange = Environment.TickCount;

      if (isVisible)
      {
        bool dropRequest = refreshWait;

        if (! dropRequest)
        {
          Window<IViewModel> seeker = this.parentWindow;

          while (seeker != null)
          {
            if (seeker.refreshWait || ! seeker.isVisible)
            {
              dropRequest = true;
              break;
            }

            seeker = seeker.parentWindow;
          }
        }


        if (dropRequest)
        { // This refresh request will dropped because a previous refresh request will call Display() 
          if (traceEnabled)
          {
            Trace.WriteLine(">> Window.Refresh()/dropRequest: " + lastChange.ToString());
          }
        }
        else
        {
          refreshWait = true;                                                                       // Signal for thereafter Refresh requests. [to be notified Display() will call]

          Task.Run(() =>
            {
              Thread.Sleep(refreshWaitTime);                                                        // Batch requests and all request (in interval) will exequted one time (with one Display() call)

              refreshWait = false;

              Display();
            });
        }
      }
    }

    private void ChangedEvent(object source)
    {
      if (traceEnabled)
      {
        Trace.WriteLine(">> Window.ChangedEvent(): " + Environment.TickCount.ToString() + " | " + source.GetType().ToString());
      }

      Refresh();
    }

    public void Display()
    {
      if (isVisible)
      {
        lock (ConsoleWindows.lockConsole)                                                           // Centralized lock for all windows
        {
          if (lastChange > lastRefresh)
          {
            lastRefresh = Environment.TickCount;                                                    // start time of refresh

            if (traceEnabled)
            {
              Trace.WriteLine(">> Window.Display(): " + lastRefresh.ToString());
            }

            InternalDisplay();
          }
        }
      }
    }

    private void InternalDisplay()
    {
      var dp = new DisplayConsolePartInfo(this);

      InternalDisplayPart(ref dp);                                                                // Display window's IArea

      var orderedElements = GetDisplayOrderedElements();

      foreach (var element in orderedElements)
      {
        var childWindow = element as Window<IViewModel>;

        if (childWindow != null)
        {
          if (childWindow.isVisible)
          {
            var dp2 = new DisplayConsolePartInfo(childWindow);

            InternalDisplayPart(ref dp2);                                                          // Display window's IArea

            childWindow.InternalDisplay();
          }
        }
        else
        {
          var dp3 = new DisplayConsolePartInfo(element);
          InternalDisplayPart(ref dp3);
        }
      }
    }

    private void InternalDisplayPart(ref DisplayConsolePartInfo partInfo)
    {
      int dummy; // TODO

      partInfo.width  = Math.Max(partInfo.width,  (this.width  - partInfo.col));   // TODO: this.width  must correction with Border+Scrollbar
      partInfo.height = Math.Max(partInfo.height, (this.height - partInfo.row));   // TODO: this.height must correction with Border+Scrollbar

      partInfo.row    += this.row;
      partInfo.col    += this.col;

      if (partInfo.background == WinColor.None)
      {
        partInfo.background = this.background;
      }

      if (partInfo.foreground == WinColor.None)
      {
        partInfo.foreground = this.foreground;
      }

      if (isRootWindow)
      {
        this.consoleWindows.DisplayPart(ref partInfo);
      }
      else if (parentWindow != null)
      {
        parentWindow.InternalDisplayPart(ref partInfo);                                        // recursion: position&size correction to the root. 
      } 
    }

    private IRegion[] GetDisplayOrderedElements()
    {
      IRegion[] orderedElements = elements.OrderBy(x => (x.row * 1000000) + x.col).ToArray();

      return orderedElements;
    }    
    #endregion

    #region usage/visibility/editable

    public enum State
    {
      Hide      = 0,
      ReadOnly  = 1,
      Editable  = 2
    }

    public State state
    {
      get { return _state; }
      set { _state = value; ProceedFirstField(); }
    }
    private State _state;

    public  bool  isVisible
    {
      get
      {
        if (state == State.Hide)                                                                  
        {
          return false;
        }

        if (parentWindow == null)
        {
          if (isRootWindow)
          {
            return (state != State.Hide);
          }

          return false;                                                                           // Orphan window, haven't a valid root window
        }
        else
        {
          return parentWindow.isVisible;                                                          // Warning: recursive call, because not visible if any(!) hidden in link list.
        }
      }
    }

    #endregion    

    #region field step / proceed

    public void ProceedFirstField()
    {
      if (state == State.Editable)
      { // Appoint first field of window.

      }
    }

    public void ProceedField(bool next = true)
    {

    }

    public void ProceedLastField()
    {

    }

    #endregion

    #region IDisposable implementation

    private bool disposedValue = false;                                                           // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
      if (! disposedValue)
      {
        if (disposing)
        {
          //foreach (var element in elements)
          //{
          //  element.Changed -= new ChangedEventHandler(ChangedEvent);
          //}
        }

        if (elements != null)
        {
          foreach (var element in elements)
          {
            element.Changed -= new ChangedEventHandler(ChangedEvent);

            var childWindow = element as Window<IViewModel>;

            if (childWindow != null)
            {
              childWindow.parentWindow = null;
            }
          }

          elements = null;
        }

        disposedValue = true;
      }
    }

    ~Window()
    {
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);
    }
    #endregion

    #region others

    public static bool traceEnabled = false;

    #endregion
  }
}