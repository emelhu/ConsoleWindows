using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public struct Region : IRegion
  {
    public int      top     { get; set; }
    public int      left    { get; set; }
    public int      width   { get; set; }
    public int      height  { get; set; }

    public WinColor color   { get; set; }

    public Region (int top, int left, int width, int height, WinColor color = WinColor.None)
    {
      this.top    = top;   
      this.left   = left;  
      this.width  = width; 
      this.height = height;
      this.color  = color; 
    }
  }
}
