﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class Area : Region, IArea, IRegion
  {
    private Border?     _border;
    private Scrollbars? _scrollbars;

    public  Border?      border     { get { return _border; }       set { IndicateChange(Nullable.Compare(_border, value)     != 0); _border     = value; } }
    public  Scrollbars?  scrollbars { get { return _scrollbars; }   set { IndicateChange(Nullable.Compare(_scrollbars, value) != 0); _scrollbars = value; } }

    public Area(int row, int col, int width, int height, StyleIndex styleIndex = StyleIndex.Default, Border? border = null, Scrollbars? scrollbars = null)
      : base(row, col, width, height, styleIndex)
    {
      this.border     = border;
      this.scrollbars = scrollbars;
    }

    public Area(IRegion region, Border? border = null, Scrollbars? scrollbars = null)
      : base (region)
    {
      this.border     = border;
      this.scrollbars = scrollbars;
    }

    public Area(IArea area)
      : base(area.row, area.col, area.width, area.height, area.styleIndex)
    {
      this.border     = area.border;
      this.scrollbars = area.scrollbars;
    }
  }
}
