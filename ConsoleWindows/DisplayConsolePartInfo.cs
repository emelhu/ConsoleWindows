using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public IRegion        region          { get; private set; } 

    #region region info

    public string         displayText                                                             // nullable if no content to display
    {
      get
      {
        if (region is IElement element)
        {
          return element.GetDisplayText(false);
        }

        return null;
      }
    }

    public string         editText                                                              // nullable if no content to display
    {
      get
      {
        if (region is IElement element)
        {
          if (region is IEditable editable)
          {
            return element.GetDisplayText(! editable.readOnly);
          }

          return element.GetDisplayText(false);
        }

        return null;
      }
    }

    public Border?        border                                                                  // nullable if no content to display
    {
      get
      {
        if (region is IArea area)
        {
          if ((area.border != null) && ((Border)area.border).isVisible())
          {
            return area.border;
          }
        }

        return null;
      }
    }

    public Border?    borderActual
    {
      get
      {
        if (region is Window<IViewModel> window)
        {
          if ((window.borderActual != null) && ((Border)window.borderActual).isVisible())
          {
            return window.borderActual;
          }
        }

        return null;
      }
    }

    public Scrollbars?    scrollbars                                                              // nullable if no content to display
    {
      get
      {
        if (region is IArea area)
        {
          if ((area.scrollbars != null) && ((Scrollbars)area.scrollbars).isVisible())
          {
            if (area is IScrollbarInfo)
            {
              return area.scrollbars;
            }
          }
        }

        return null;
      }
    }

    public IScrollbarInfo scrollbarsInfo
    {
      get
      {
        if (region is IArea area)
        {
          if ((area.scrollbars != null) && ((Scrollbars)area.scrollbars).isVisible())
          {
            return area as IScrollbarInfo;
          }
        }

        return null;
      }
    }

    #endregion

    public static DisplayConsolePartInfo CreateFrom(Window<IViewModel> window)       
    {
      return new DisplayConsolePartInfo(window, window.GetStyle(window.styleIndex));
    }

    public DisplayConsolePartInfo(IRegion region, Style style)
    {
      Debug.Assert(region != null);

      this.region         = region;

      this.row            = region.row;
      this.col            = region.col;
      this.width          = region.width;
      this.height         = region.height;
      this.style          = style;      
    }
  }    
}
