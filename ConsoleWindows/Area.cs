using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class Area : Region, IArea, IRegion
  {
    private Border     _border;
    private Scrollbars _scrollbars;


    public  Border      border      { get { return _border; }       set { _border     = value; IndicateChange(); } }
    public  Scrollbars  scrollbars  { get { return _scrollbars; }   set { _scrollbars = value; IndicateChange(); } }

    public Area(int top, int left, int width, int height, WinColor foreground = WinColor.None, WinColor background = WinColor.None, Border? border = null, Scrollbars? scrollbars = null)
      : base(top, left, width, height, foreground, background)
    {
      this.border     = border     ?? new Border();
      this.scrollbars = scrollbars ?? new Scrollbars();
    }

    public Area(IRegion region, Border? border = null, Scrollbars? scrollbars = null)
      : base (region)
    {
      this.border     = border     ?? new Border();
      this.scrollbars = scrollbars ?? new Scrollbars();
    }

    public Area(IArea area)
      : base(area.top, area.left, area.width, area.height, area.foreground, area.background)
    {
      this.border     = area.border;
      this.scrollbars = area.scrollbars;
    }
  }
}
