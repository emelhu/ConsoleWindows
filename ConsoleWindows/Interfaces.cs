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

  public interface IROPosition
  {
    int       row         { get; }
    int       col         { get; }
  }

  public interface ISize
  {
    int       width       { get; set; }
    int       height      { get; set; }
  }

  public interface IChangedEvent
  {   
    event     ChangedEventHandler Changed;

    void                          IndicateChange(bool valueChanged = true);
  }

  public interface IRegion : IPosition, ISize, IChangedEvent
  {   
    StyleIndex styleIndex  { get; set; }

    bool       visible     { get; set; }
  }

  public interface IArea : IRegion
  {
    Border?     border     { get; set; }
    Scrollbars? scrollbars { get; set; }
  }

  public interface IValidating
  {
    /// <summary>
    /// Check this element or window validity.
    /// </summary>
    /// <returns>Error text or null if valid.</returns>
    string IsValid();

    /// <summary>
    /// Define a function for validate content of element or window.
    /// It returns a null if no error, or an error text.
    /// Function receive reference of sender object.
    /// </summary>
    Func<Object, string> validate { get; set; }

    /// <summary>
    /// Common validation rule.
    /// </summary>
    bool      emptyEnabled  { get; set; }   

    /// <summary>
    /// Common validation rule.
    /// </summary>
    bool      nullEnabled  { get; set; }   

    /// <summary>
    /// Common validation rule.
    /// </summary>
    dynamic   minValue      { get; set; }

    /// <summary>
    /// Common validation rule.
    /// </summary>
    dynamic   maxValue      { get; set; }
  }

  public interface ITabStop
  {
    bool              tabStop       { get; set; }
  }

  public interface IEditable
  {
    bool              readOnly      { get; set; }

    ConsoleKeyInfo?   KeyPress(ConsoleKeyInfo keyInfo);
    
    IROPosition       CursorPosition { get; }
  }

  public interface IElement : IRegion
  {
    string    displayText   { get; }

    string    text          { get; set; }

    string    description   { get; set; }
  }

  public interface IViewModel
  {
    event ChangedEventHandler changed;

    void                      IndicateChange(bool valueChanged = true);
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
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  void IndicateChange(bool valueChanged = true)
  {
    if ((Changed != null) && valueChanged)
    {
      Changed(this);                                                                            // aka: Changed?.Invoke(this);
    }
  }
*/
