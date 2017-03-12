using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public struct DisplayConsolePartInfo
  {
    public int            row;
    public int            col;
    public int            width;
    public int            height;

    public Style          style;

    public string         displayText;                                                            // nullable if no content to display

    public Border?        border;                                                                 // nullable if no content to display
    public Scrollbars?    scrollbars;                                                             // nullable if no content to display
    public IScrollbarInfo scrollbarsInfo;


    //public DisplayConsolePartInfo (int row, int col, int width, int height, ref Style style, string displayText = null)
    //{
    //  this.row          = row;   
    //  this.col          = col;  
    //  this.width        = width; 
    //  this.height       = height;
    //  this.style        = style; 
    //  this.displayText  = displayText;
    //}

    public static DisplayConsolePartInfo CreateFrom(Window<IViewModel> window)       
    {

      return new DisplayConsolePartInfo(window, window.GetStyle(window.styleIndex));
    }

    public DisplayConsolePartInfo(IRegion region, Style style)
    {
      this.row            = region.row;
      this.col            = region.col;
      this.width          = region.width;
      this.height         = region.height;
      this.style          = style;
      this.displayText    = null;
      this.border         = null; 
      this.scrollbars     = null;
      this.scrollbarsInfo = null;


      if (region is IElement element)                                                             //  IElement element = (region as IElement);
      {
        this.displayText = element.displayText;        
      }
      else if (region is IArea area)                                                              // IArea area = (region as IArea);
      {
        if ((area.border != null) && ((Border)area.border).isVisible())
        {
          this.border = area.border;
        }

        if ((area.scrollbars != null) && ((Scrollbars)area.scrollbars).isVisible())
        {
          this.scrollbars     = area.scrollbars;
          this.scrollbarsInfo = area as IScrollbarInfo;
        }
      }
    }
  }    
}
