using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class Region : IRegion
  {
    #region private

    private int         _row;
    private int         _col;
    private int         _width;
    private int         _height;

    private StyleIndex  _styleIndex = StyleIndex.TextWrite;
    #endregion

    #region interface
    public  int         row         { get { return _row; }          set { IndicateChange(_row    != value);     _row    = value;      } }
    public  int         col         { get { return _col; }          set { IndicateChange(_col    != value);     _col    = value;      } }
    public  int         width       { get { return _width; }        set { IndicateChange(_width  != value);     _width  = value;      } }
    public  int         height      { get { return _height; }       set { IndicateChange(_height != value);     _height = value;      } }

    public  StyleIndex  styleIndex  { get { return _styleIndex; }   set { IndicateChange(_styleIndex != value); _styleIndex = value;  } }

    public  bool        visible     { get; set; } = true;

    public  event  ChangedEventHandler Changed;

    public Region (int row, int col, int width, int height, StyleIndex styleIndex = StyleIndex.Default)
    {
      this.row        = row;   
      this.col        = col;  
      this.width      = width; 
      this.height     = height;
      this.styleIndex = styleIndex; 
    }

    public Region (IPosition position, int width, int height, StyleIndex styleIndex = StyleIndex.Default)
    {
      this.row        = position.row;   
      this.col        = position.col;  
      this.width      = width; 
      this.height     = height;
      this.styleIndex = styleIndex; 
    }

    public Region (IPosition position, ISize size, StyleIndex styleIndex = StyleIndex.Default)
    {
      this.row        = position.row;   
      this.col        = position.col;  
      this.width      = size.width; 
      this.height     = size.height;
      this.styleIndex = styleIndex; 
    }

    public Region (IRegion region)
    {
      this.row        = region.row;   
      this.col        = region.col;  
      this.width      = region.width; 
      this.height     = region.height;
      this.styleIndex = region.styleIndex; 
    }
    #endregion

    #region internal operation

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IndicateChange(bool valueChanged = true)
    {
      if ((Changed != null) && valueChanged)
      {
        Changed(this);                                                                            // aka: Changed?.Invoke(this);
      }
    }

    #endregion
  }
}
