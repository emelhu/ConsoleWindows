#define USE_refreshWait                                               // for increase performance | comment of it to debug
//#define USE_traceEnabled                                              // Write trace messages

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace eMeL.ConsoleWindows
{
  public class Window<TViewModel> : Area, IArea, IRegion, IValidating, IDisposable
     where TViewModel : IViewModel
  {
    #region data

    public    TViewModel viewModel    { get; private set; }

    public    Border?     borderActual            { get { return _borderActual; }            set { IndicateChange(Nullable.Compare(_borderActual, value) != 0); _borderActual = value;             } }
    private   Border?    _borderActual;

    public    StyleIndex  styleIndexActualWindow  { get { return _styleIndexActualWindow; }  set { IndicateChange(_styleIndexActualWindow != value);            _styleIndexActualWindow = value;  } }
    private   StyleIndex _styleIndexActualWindow = StyleIndex.WindowActual;

    public    StyleIndex  styleIndexActualElement { get { return _styleIndexActualElement; } set { IndicateChange(_styleIndexActualElement != value);           _styleIndexActualElement = value; } }
    private   StyleIndex _styleIndexActualElement = StyleIndex.TextElementActual;

    public    Window<IViewModel>  parentWindow
    {
      get           { return _parentWindow; }

      internal set  { _parentWindow = value; }      
    }
    private   Window<IViewModel> _parentWindow;

    public    ConsoleWindows      consoleWindows
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
    internal  ConsoleWindows     _consoleWindows  { get; set; }

    public    bool               leaveQuestionAsk { get; set; } = ConsoleWindows.defaultLeaveQuestionAsk;

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

    public Style style
    {
      get
      {
        return GetStyle(this.styleIndex);
      }
    }

    public Styles styles
    {
      get
      {
        if (_styles != null)
        {
          return _styles;
        }

        if (isRootWindow)
        {  
          _styles = _consoleWindows.virtualConsole.styles;
          return _styles;
        }

        Debug.Assert(_parentWindow != null);

        _styles = _parentWindow.styles;                                                           // recursion

        return _styles;                                                                     
      }
    }
    private Styles _styles = null;

    public Style GetStyle(StyleIndex styleIndex)
    {     
      return styles[styleIndex];      
    } 

    public IElement actualElement 
    { 
      get
      {
        return _actualRegion as IElement;
      }
    }

    public Window<IViewModel> actualChildWindow 
    { 
      get
      {
        return _actualRegion as Window<IViewModel>;
      }
    }

    private IRegion _actualRegion;
    
    #endregion

    #region constructor

    public Window(TViewModel viewModel, IRegion region, Border? border = null, Scrollbars? scrollbars = null)
      : this(viewModel, region.row, region.col, region.width, region.height, region.styleIndex, border, scrollbars)
    {
    }

    public Window(TViewModel viewModel, IArea area)
      : this(viewModel, area.row, area.col, area.width, area.height, area.styleIndex, area.border, area.scrollbars)
    {
    }

    public Window(TViewModel viewModel, int row, int col, int width, int height, StyleIndex styleIndex = StyleIndex.Window, Border? border = null, Scrollbars? scrollbars = null)
      : base (row, col, width, height, styleIndex, border, scrollbars)
    {  
      this.state     = State.Hide;  
      this.viewModel = viewModel;    

      if (this.viewModel == null)
      {
        throw new ArgumentException("The 'viewModel' must an IViewModel successor!", nameof(this.viewModel));
      }

      this.state = State.Editable;                                                                     // default state      
    }
    
    public Window(TViewModel viewModel, int row, int col, int width, int height, Border? border = null, Scrollbars? scrollbars = null, StyleIndex styleIndex = StyleIndex.Window)
      : this(viewModel, row, col, width, height, styleIndex, border, scrollbars)
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

      #if USE_traceEnabled
      if (traceEnabled)
      {
        Trace.WriteLine(">> Window.AddElement: " + Environment.TickCount.ToString() + " | " + element.GetType().ToString());
      }
      #endif
      
      element.Changed += new ChangedEventHandler(ChangedEvent);

      if (element is Window<IViewModel> childWindow)
      {
        childWindow._styles      = styles;
        childWindow.parentWindow = this as Window<IViewModel>;;
      }

      if (_actualRegion == null)
      {
        ToFirstItem();
      }
      
      Refresh();
    }

    public void RemoveElement(IRegion element)
    {
      if (element == null)
      {
        throw new ArgumentException("There is null argumentum!", nameof(element));
      }

      if (_actualRegion == element)
      {
        ToPrevItem();                                                                              

        if (_actualRegion == element)
        {
          ToNextItem();
        }

        if (_actualRegion == element)
        {
          _actualRegion = null;
        }
      }

      elements.Remove(element);

      #if USE_traceEnabled
      if (traceEnabled)
      {
        Trace.WriteLine(">> Window.RemoveElement: " + Environment.TickCount.ToString() + " | " + element.GetType().ToString());
      }
      #endif

      element.Changed -= new ChangedEventHandler(ChangedEvent);

      if (element is Window<IViewModel> childWindow)
      {
        childWindow.parentWindow = null;
      }

      if ((_actualRegion == null) || (_actualRegion == element))
      {
        ToFirstItem();                                                                            // There isn't a previous element, this element was the first.
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
    private static    int     _defaultRefreshWaitTime = RefreshWaitTimeBarrier(100);

    public  const     int     minRefreshWaitTime      = 50;
    public  const     int     maxRefreshWaitTime      = 300;
    
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

      if (visible)
      {
        bool dropRequest = refreshWait;

        if (! dropRequest)
        {
          Window<IViewModel> seeker = this.parentWindow;

          while (seeker != null)
          {
            if (seeker.refreshWait || ! seeker.visible)
            {
              dropRequest = true;
              break;
            }

            seeker = seeker.parentWindow;
          }
        }


        if (dropRequest)
        { // This refresh request will dropped because a previous refresh request will call Display() 
          #if USE_traceEnabled
          if (traceEnabled)
          {
            Trace.WriteLine(">> Window.Refresh()/dropRequest: " + lastChange.ToString());
          }
          #endif
        }
        else
        {
          #if USE_refreshWait
          refreshWait = true;                                                                       // Signal for thereafter Refresh requests. [to be notified Display() will call]

          Task.Run(() =>
            {
              Thread.Sleep(refreshWaitTime);                                                        // Batch requests and all request (in interval) will exequted one time (with one Display() call)

              refreshWait = false;

              Display();
            });
          #else
          refreshWait = false;
          Display();
          #endif
        }
      }
    }

    private void ChangedEvent(object source)
    {
      #if USE_traceEnabled
      if (traceEnabled)
      {
        Trace.WriteLine(">> Window.ChangedEvent(): " + Environment.TickCount.ToString() + " | " + source.GetType().ToString());
      }
      #endif

      Refresh();
    }

    private const int repeateCountPerSec = 15;

    public void Display(bool force = false, bool priority = false)
    {
      if (visible)
      {
        if (priority)
        {
          lastChange = Environment.TickCount - (1000 / repeateCountPerSec);                       // optimalize if more then 15 times in sec is happen this call --- containing the amount of time in milliseconds...
        }

        if ((lastChange > lastRefresh) || force)
        {
          lastRefresh = Environment.TickCount;                                                    // start time of refresh

          #if USE_traceEnabled
          if (traceEnabled)
          {
            Trace.WriteLine(">> Window.Display(): " + lastRefresh.ToString());
          }
          #endif

          InternalDisplay();
        }     
      }
    }

    private void InternalDisplay()
    {
      //var partInfo = DisplayConsolePartInfo.CreateFrom(this);
      var partInfo = new DisplayConsolePartInfo(this, GetStyle(this.styleIndex));

      InternalDisplayPart(ref partInfo);                                                                // Display window's IArea

      var orderedElements = GetDisplayOrderedElements();

      foreach (var element in orderedElements)
      {
        if (element is Window<IViewModel> childWindow)
        {
          if (childWindow.visible)
          {
            //var partInfo2 = new DisplayConsolePartInfo(childWindow, GetStyle(childWindow.styleIndex));
            var partInfo2 = DisplayConsolePartInfo.CreateFrom(childWindow);

            InternalDisplayPart(ref partInfo2);                                                          // Display window's IArea

            childWindow.InternalDisplay();
          }
        }
        else
        {
          var partInfo3 = new DisplayConsolePartInfo(element, GetStyle(element.styleIndex));

          InternalDisplayPart(ref partInfo3);
        }
      }
    }

    private void InternalDisplayPart(ref DisplayConsolePartInfo partInfo)
    {
      int dummy; // TODO

      partInfo.width  = Math.Min(partInfo.width,  (this.width  - partInfo.col));   // TODO: this.width  must correction with Border+Scrollbar
      partInfo.height = Math.Min(partInfo.height, (this.height - partInfo.row));   // TODO: this.height must correction with Border+Scrollbar

      partInfo.row    += this.row;
      partInfo.col    += this.col;

      if (partInfo.style.background == WinColor.None)
      {
        partInfo.style.background = style.background;
      }

      if (partInfo.style.foreground == WinColor.None)
      {
        partInfo.style.foreground = style.foreground;
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
      Killed    = 0,
      Hide      = 1,
      ViewOnly  = 2,
      Editable  = 3
    }

    public State state
    {
      get { return _state; }
      set { _state = value; ToFirstItem(); }
    }
    private State _state;

    private bool VisibleState()
    {
      return ((state == State.Editable) || (state == State.ViewOnly));
    }

    private bool EditableState()
    {
      return (state == State.Editable);
    }

    public new bool visible                                                                       // hides Area.visible because it's more complicate
    {
      get
      {
        if (! VisibleState())
        {
          return false;
        }

        if (parentWindow == null)
        {
          if (isRootWindow)
          {
            return VisibleState();
          }

          return false;                                                                           // Orphan window, haven't a valid root window
        }
        else
        {
          return parentWindow.visible;                                                          // Warning: recursive call, because not visible if any(!) hidden in link list.
        }
      }

      set
      {
        if (value)
        {
          if (state == State.Hide)
          { 
            state = State.ViewOnly;
          }
          else if (state == State.Killed)
          {
            throw new Exception("Window.visible = true and state is killed!");
          }
        }
        else
        {
          if (VisibleState())
          {
            state = State.Hide;
          }
        }
      }
    }

    #endregion

    #region field step / proceed

    private static bool IsItemApplicable(IRegion region)
    {
      if (region is ITabStop tabStopable)  
      {            
        return tabStopable.tabStop;
      }
      else if (region is Window<IViewModel> window)
      {
        if (window.EditableState())
        {
          window.ToFirstItem();

          if (window._actualRegion != null)
          {
            return IsItemApplicable(window._actualRegion);
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Check element/window validity.
    /// </summary>
    /// <param name="region">Element or window to check.</param>
    /// <returns>Error text or null if valid.</returns>
    private string IsItemValid(IRegion region)
    {
      if (region != null)
      {
        if (region is IValidating validating)
        {
          return validating.IsValid();
        }
        else if (region is Window<IViewModel> window)
        {
          return window.IsValid();
        }
      }

      return null;
    }

    /// <summary>
    /// Check this element or window validity.
    /// </summary>
    /// <returns>Error text or null if valid.</returns>
    public string IsValid()
    {
      if (state == State.Editable)
      {
        foreach (var region in elements)
        {
          var errorMessage = IsItemValid(region);

          if (errorMessage != null)
          {
            return errorMessage;
          }
        }

        if (validate != null)
        {
          return validate(this);
        }
      }

      return null;                                                                                // Valid, none error message.
    }

    public void ToFirstItem()
    {
      _actualRegion = null;

      if (state == State.Editable)
      { // Appoint first item of window.
        foreach (var region in elements)
        {
          if (IsItemApplicable(region))  
          {            
            _actualRegion = region;
            break;
          }         
        }        
      }
    }

    public void ToLastItem()
    {
      _actualRegion = null;

      if (state == State.Editable)
      { // Appoint last item of window.
        foreach (var region in elements)
        {
          if (IsItemApplicable(region))  
          {      
            if (IsItemValid(_actualRegion) != null)
            {
              return;
            }
           
            _actualRegion = region;
          }          
        }        
      }      
    }

    public void ToNextItem()
    {
      if (state == State.Editable)
      {  
        if (_actualRegion == null)
        {
          ToFirstItem();
          return;
        }
        else if (IsItemValid(_actualRegion) != null) 
        { // This field content isn't valid, can't skip to next 
          return;
        }

        bool found = false;

        foreach (var region in elements)
        {
          if (found)
          {
            if (IsItemApplicable(region))  
            {        
              _actualRegion = region;
              break;
            }
          }
          else
          {
            found = (region == _actualRegion);
          }
        }   
      }  
      else
      {
        _actualRegion = null;
      }
    }

    public void ToPrevItem()
    {
      if (state == State.Editable)
      { 
        if (_actualRegion == null)
        {
          ToFirstItem();
          return;
        }

        IRegion prevRegion = _actualRegion;                                                       // stay here if not found previous field

        foreach (var region in elements)
        {
          if (region == _actualRegion)
          { // Found current
            _actualRegion = prevRegion;
            break;
          }

          if (IsItemApplicable(region))  
          {
            prevRegion = region;
          }              
        } 
      }  
      else
      {
        _actualRegion = null;
      }
    }
    #endregion

    #region actions

    /// <summary>
    /// This action fired when state of window changed (this window is the actual window and it's editable)
    /// example:
    /// StartEditable = (win) => { StartEditableHappen(win); };
    /// </summary>
    public Action<Object>             startEditable             { get; set; }

    /// <summary>
    /// This action fired when state of window changed (this window is the actual window and it's not editable already or state changed from editable)
    /// example:
    /// StopEditable = (win) => { StopEditableHappen(win); };
    /// </summary>
    public Action<Object>             stopEditable              { get; set; }

    /// <summary>
    /// This action fired when state of window changed (this window is visible in display)
    /// example:
    /// StartVisible = (win) => { StartVisibleHappen(win); };
    /// </summary>
    public Action<Object>             startVisible              { get; set; }

    /// <summary>
    /// This action fired when state of window changed (this window is not visible in display)
    /// example:
    /// StopVisible = (win) => { StopVisibleHappen(win); };
    /// </summary>
    public Action<Object>             stopVisible               { get; set; }

    /// <summary>
    /// Define a function for validate content of Window.
    /// It returns a null if no error, or an error text.
    /// </summary>
    public Func<Object, string>       validate                  { get; set; }

    public Func<Object, bool>         closeable                 { get; set; }

    public  bool emptyEnabled                                                                     // IValidating: Only technical property, don't work really
    {  
      get { return true; }
      set {}
    }

    public  bool nullEnabled                                                                      // IValidating: Only technical property, don't work really
    {  
      get { return true; }
      set {}
    }

    public  dynamic   minValue                                                                    // IValidating: Only technical property, don't work really
    {  
      get { return null; }
      set {}
    }

    public  dynamic   maxValue                                                                    // IValidating: Only technical property, don't work really
    {  
      get { return null; }
      set {}
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
          ClearDependencies();

          foreach (var element in elements)
          {
            if (element is Window<IViewModel> childWindow)
            {
              childWindow.ClearDependencies();                                                    // in ClearDependencies() occur the element.Changed -= new ChangedEventHandler(ChangedEvent);
            }
            else
            {
              element.Changed -= new ChangedEventHandler(ChangedEvent);
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
    #if USE_traceEnabled
      public static bool traceEnabled = true;
    #endif

    private void ClearDependencies()
    {
      if (parentWindow != null)
      {
        parentWindow.RemoveElement(this);
      }

      parentWindow    = null;
      _consoleWindows = null;
      _styles         = null;
    }
    #endregion
  }
}