using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public delegate void ChangedEventHandler(object source);

  public interface IPosition
  {
    int       row         { get; set; }
    int       col         { get; set; }
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
    string  displayText   { get; }

    bool    emptyEnabled  { get; set; }

    string  description   { get; set; }

    /// <summary>
    /// Define a function for validate content of element.
    /// It returns a null if no error, or an error text.
    /// </summary>
    Func<IElement, string> validate { get; set; }

    /// <summary>
    /// Check this element validity.
    /// </summary>
    /// <returns>Error text or null if valid.</returns>
    string IsValid();
  }

  public interface IViewModel
  {
    event ChangedEventHandler changed;

    void                      IndicateChange();
  }

  public interface IConsoleMouse
  { // http://stackoverflow.com/questions/5241984/findwindowex-from-user32-dll-is-returning-a-handle-of-zero-and-error-code-of-127
    void Init(VirtualConsole virtualConsole);

    IPosition Clicked();
  }

  public interface IScrollbarInfo
  {
    bool    upArrowVisible        { get; set; }
    bool    downArrowVisible      { get; set; }
    int?    verticalPosition      { get; set; }

    bool    leftArrowVisible      { get; set; }
    bool    rightArrowVisible     { get; set; }
    int?    horizontalPosition    { get; set; }
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
