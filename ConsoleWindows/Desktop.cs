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

    //internal  DesktopActualElementHelper actualElementHelper = null;
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

      //actualElementHelper = new DesktopActualElementHelper(() => this.actualWindow.actualElement);
    }   
  }
}
