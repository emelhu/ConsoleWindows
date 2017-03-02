using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public struct Position : IPosition
  {                         
    public int  top     { get; set; }
    public int  left    { get; set; }

    public Position(int top, int left)
    {
      this.top  = top; 
      this.left = left;
    }
  }

  public struct Size : ISize
  {
    public int  width   { get; set; }
    public int  height  { get; set; }

    public Size(int width, int height)
    {
      this.width  = width; 
      this.height = height;
    }
  }                     
}
