using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace eMeL.ConsoleWindows
{
  public class Desktop
  {
    #region properties
    public    Window<IViewModel>  rootWindow    { get; private  set; }
    public    Window<IViewModel>  actualWindow  { get; internal set; }

    private   IRegion _cursorRelativePositionValidRegion  = null;

    private   int     _actualElementRow = 0;
    private   int     _actualElementCol = 0;

    internal  int     actualElementRow 
                      { 
                        get { InitCursorRelativePosition(); return _actualElementRow; } 

                        set 
                        { 
                          InitCursorRelativePosition(); 

                          _actualElementRow = value;

                          // TODO: check borders
                        }
                      }

    internal  int     actualElementCol 
                      { 
                        get { InitCursorRelativePosition(); return _actualElementCol; } 

                        set 
                        { 
                          InitCursorRelativePosition(); 

                          _actualElementCol = value;

                          // TODO: check borders
                        }
                      }
    #endregion    
      
    public Desktop(Window<IViewModel> rootWindow)
    {
      this.rootWindow   = rootWindow ?? throw new ArgumentNullException(nameof(rootWindow), "eMeL.ConsoleWindows.Desktop(rootWindow): parameter is null!");
      this.actualWindow = rootWindow;

      actualWindow.ToFirstItem();

      while (actualWindow.actualChildWindow != null)
      {
        actualWindow = actualWindow.actualChildWindow;
        actualWindow.ToFirstItem();
      }
    }

    private void  InitCursorRelativePosition()
    {
      if ((_cursorRelativePositionValidRegion == null) || (_cursorRelativePositionValidRegion != this.actualWindow.actualElement))
      {
        _actualElementRow = 0;
        _actualElementCol = 0;

        _cursorRelativePositionValidRegion  = this.actualWindow.actualElement;

        if (this.actualWindow.actualElement is IEditable editable)
        { // Set default position (correction)
          switch (editable.editMode)
          {
            case EditMode.Text:
            case EditMode.Date:
            case EditMode.Time:
            case EditMode.Bool:              
              break;

            case EditMode.Integer:
              _actualElementCol = _cursorRelativePositionValidRegion.col - 1;
              break;

            case EditMode.Decimal:
              if (editable.decimalDigits < 1)
              {
                _actualElementCol = _cursorRelativePositionValidRegion.col - 1;
              }
              else
              {
                _actualElementCol = _cursorRelativePositionValidRegion.col - 2 - editable.decimalDigits;     // before decimal point
              }
              break;
            
            default:
              Debug.Fail("IEditable.editMode invalid enum value!");
              break;
          }
        }
      }
    }
  }
}
