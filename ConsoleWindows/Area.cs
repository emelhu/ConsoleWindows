using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public struct Area : IArea
  {
    public int        top         { get; set; }
    public int        left        { get; set; }
    public int        width       { get; set; }
    public int        height      { get; set; }

    public WinColor   foreground  { get; set; }
    public WinColor   background  { get; set; }

    public Border     border      { get; set; }
    public Scrollbars scrollbars  { get; set; }

    public Area(int top, int left, int width, int height, WinColor foreground = WinColor.None, WinColor background = WinColor.None, Border? border = null, Scrollbars? scrollbars = null)
    {
      this.top        = top;
      this.left       = left;
      this.width      = width;
      this.height     = height;
      this.foreground = foreground;
      this.background = background;

      this.border     = border     ?? new Border();
      this.scrollbars = scrollbars ?? new Scrollbars();
    }

    public Area(IRegion region, Border? border = null, Scrollbars? scrollbars = null)
    {
      this.top        = region.top;
      this.left       = region.left;
      this.width      = region.width;
      this.height     = region.height;
      this.foreground = region.foreground;
      this.background = region.background;

      this.border     = border     ?? new Border();
      this.scrollbars = scrollbars ?? new Scrollbars();
    }
  }
}
