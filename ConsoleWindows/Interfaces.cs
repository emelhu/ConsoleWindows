using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public delegate void ChangedEventHandler(object source);

  public interface IPosition
  {
    int       top         { get; set; }
    int       left        { get; set; }
  }

  public interface ISize
  {
    int       width       { get; set; }
    int       height      { get; set; }
  }

  public interface IRegion : IPosition, ISize
  {   
    WinColor  background  { get; set; }
    WinColor  foreground  { get; set; }

    event     ChangedEventHandler Changed;

    void                          IndicateChange();
  }

  public interface IArea : IRegion
  {
    Border?     border     { get; set; }
    Scrollbars? scrollbars { get; set; }
  }

  public interface IElement : IRegion
  {
    string displayText { get; }
  }

  public interface IViewModel
  {
    event ChangedEventHandler Changed;

    void                      IndicateChange();
  }

  public interface IConsoleMouse
  { // http://stackoverflow.com/questions/5241984/findwindowex-from-user32-dll-is-returning-a-handle-of-zero-and-error-code-of-127
    void Init(string title, ISize size);

    IPosition Clicked();
  }
}

/*
  void IndicateChange()
  {
    if (Changed != null)
    {
      Changed(this);                                                                            // aka: Changed?.Invoke(this);
    }
  }
*/
