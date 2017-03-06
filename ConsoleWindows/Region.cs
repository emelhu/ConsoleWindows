using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class Region : IRegion
  {
    #region private

    private int      _row;
    private int      _col;
    private int      _width;
    private int      _height;

    private WinColor _foreground;
    private WinColor _background;
    #endregion

    #region interface
    public  int       row         { get { return _row; }          set { _row = value;        IndicateChange(); } }
    public  int       col         { get { return _col; }          set { _col = value;       IndicateChange(); } }
    public  int       width       { get { return _width; }        set { _width = value;      IndicateChange(); } }
    public  int       height      { get { return _height; }       set { _height = value;     IndicateChange(); } }

    public  WinColor  foreground  { get { return _foreground; }   set { _foreground = value; IndicateChange(); } }
    public  WinColor  background  { get { return _background; }   set { _background = value; IndicateChange(); } }

    public  event  ChangedEventHandler Changed;

    public Region (int row, int col, int width, int height, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
    {
      this.row        = row;   
      this.col        = col;  
      this.width      = width; 
      this.height     = height;
      this.foreground = foreground; 
      this.background = background; 
    }

    public Region (IPosition position, int width, int height, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
    {
      this.row        = position.row;   
      this.col        = position.col;  
      this.width      = width; 
      this.height     = height;
      this.foreground = foreground; 
      this.background = background; 
    }

    public Region (IPosition position, ISize size, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
    {
      this.row        = position.row;   
      this.col        = position.col;  
      this.width      = size.width; 
      this.height     = size.height;
      this.foreground = foreground; 
      this.background = background; 
    }

    public Region (IRegion region)
    {
      this.row        = region.row;   
      this.col        = region.col;  
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
