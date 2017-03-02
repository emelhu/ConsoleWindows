using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class Region : IRegion
  {
    #region private

    private int      _top;
    private int      _left;
    private int      _width;
    private int      _height;

    private WinColor _foreground;
    private WinColor _background;
    #endregion

    #region interface
    public  int       top         { get { return _top; }          set { _top = value;        IndicateChange(); } }
    public  int       left        { get { return _left; }         set { _left = value;       IndicateChange(); } }
    public  int       width       { get { return _width; }        set { _width = value;      IndicateChange(); } }
    public  int       height      { get { return _height; }       set { _height = value;     IndicateChange(); } }

    public  WinColor  foreground  { get { return _foreground; }   set { _foreground = value; IndicateChange(); } }
    public  WinColor  background  { get { return _background; }   set { _background = value; IndicateChange(); } }

    public  event  ChangedEventHandler Changed;

    public Region (int top, int left, int width, int height, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
    {
      this.top        = top;   
      this.left       = left;  
      this.width      = width; 
      this.height     = height;
      this.foreground = foreground; 
      this.background = background; 
    }

    public Region (IPosition position, int width, int height, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
    {
      this.top        = position.top;   
      this.left       = position.left;  
      this.width      = width; 
      this.height     = height;
      this.foreground = foreground; 
      this.background = background; 
    }

    public Region (IPosition position, ISize size, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
    {
      this.top        = position.top;   
      this.left       = position.left;  
      this.width      = size.width; 
      this.height     = size.height;
      this.foreground = foreground; 
      this.background = background; 
    }

    public Region (IRegion region)
    {
      this.top        = region.top;   
      this.left       = region.left;  
      this.width      = region.width; 
      this.height     = region.height;
      this.foreground = region.foreground; 
      this.background = region.background; 
    }
    #endregion

    #region internal operation

    public void IndicateChange()
    {
      if (Changed != null)
      {
        Changed(this);                                                                            // aka: Changed?.Invoke(this);
      }
    }

    #endregion
  }
}
