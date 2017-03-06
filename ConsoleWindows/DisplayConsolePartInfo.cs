using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public struct DisplayConsolePartInfo
  {
    public int          row;
    public int          col;
    public int          width;
    public int          height;

    public WinColor     foreground;
    public WinColor     background;

    public string       displayText;                                                              // nullable if no content to display

    public Border?      border;                                                                   // nullable if no content to display
    public Scrollbars?  scrollbars;                                                               // nullable if no content to display


    //public DisplayConsolePartInfo (int row, int col, int width, int height, WinColor foreground, WinColor background, string displayText = null)
    //{
    //  this.row          = row;   
    //  this.col          = col;  
    //  this.width        = width; 
    //  this.height       = height;
    //  this.foreground   = foreground; 
    //  this.background   = background; 
    //  this.displayText  = displayText;
    //}

    public DisplayConsolePartInfo(IRegion region)
    {
      this.row          = region.row;
      this.col          = region.col;
      this.width        = region.width;
      this.height       = region.height;
      this.foreground   = region.foreground;
      this.background   = region.background;
      this.displayText  = null;
      this.border       = null; 
      this.scrollbars   = null; 


      if (region is IElement)
      {
        this.displayText = (region as IElement).displayText;
      }
      else if (region is IArea)
      {
        IArea area = (region as IArea);

        if ((area.border != null) && ((Border)area.border).isVisible())
        {
          this.border = area.border;
        }

        if ((area.scrollbars != null) && ((Scrollbars)area.scrollbars).isVisible())
        {
          this.scrollbars = area.scrollbars;
        }
      }
    }
  }    
}
