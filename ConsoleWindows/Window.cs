using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class Window<TViewModel> : Area, IArea, IRegion, IDisposable
     where TViewModel : IViewModel
  {

    public TViewModel viewModel   { get; private set; }

    #region constructor

    public Window(TViewModel viewModel, IRegion region, Border? border = null, Scrollbars? scrollbars = null)
      : this(viewModel, region.top, region.left, region.width, region.height, region.foreground, region.background, border, scrollbars)
    {
    }

    public Window(TViewModel viewModel, IArea area)
      : this(viewModel, area.top, area.left, area.width, area.height, area.foreground, area.background, area.border, area.scrollbars)
    {
    }

    public Window(TViewModel viewModel, int top, int left, int width, int height, WinColor foreground = WinColor.None, WinColor background = WinColor.None, Border? border = null, Scrollbars? scrollbars = null)
      : base (top, left, width, height, foreground, background, border, scrollbars)
    {  
      this.viewModel = viewModel;    

      if (this.viewModel == null)
      {
        throw new ArgumentException("The 'viewModel' must an IViewModel successor!", nameof(this.viewModel));
      }
    }
    
    public Window(TViewModel viewModel, int top, int left, int width, int height, Border? border = null, Scrollbars? scrollbars = null, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
      : this(viewModel, top, left, width, height, foreground, background, border, scrollbars)
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
                          
      Trace.WriteLine(">> [Added]: " + Environment.TickCount.ToString() + " | " + element.GetType().ToString());

      element.Changed += new ChangedEventHandler(ChangedEvent);
      
      Refresh();
    }
    #endregion

    #region refresh display

    private const int     refreshWaitTime = 300;

    private object        lockConsole     = new object();

    private volatile bool refreshWait     = false;   
    private volatile int  lastRefresh     = 0;
    private volatile int  lastChange      = 0;

    public void Refresh()
    {
      lastChange = Environment.TickCount;

      if (refreshWait)
      {
        Trace.WriteLine(">> *wait* : " + lastChange.ToString());
      }
      else
      {
        refreshWait = true;

        Task.Run(() => 
          {           
            Thread.Sleep(refreshWaitTime);
           
            refreshWait = false;

            Display();     
          });
      }
    }

    private void ChangedEvent(object source)
    {
      Trace.WriteLine(">> Changed: " + Environment.TickCount.ToString() + " | " + source.GetType().ToString());

      Refresh();
    }

    public void Display()
    {
      lock (lockConsole)
      {
        if (lastChange > lastRefresh)
        {
          lastRefresh = Environment.TickCount;                                                    // start time of refresh

          Trace.WriteLine(">> Display: " + lastRefresh.ToString());



          Console.WriteLine("Display: " + lastRefresh.ToString());          
          Task.Delay(100);  // simulate display process
        }
      }
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
  }
}