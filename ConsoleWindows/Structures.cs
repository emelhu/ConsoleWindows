using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public struct Position : IPosition
  {                         
    public int  row    { get; set; }
    public int  col    { get; set; }

    public Position(int row, int col)
    {
      this.row  = row; 
      this.col  = col;
    }

    public Position(Position pos)
    {
      this.row  = pos.row; 
      this.col  = pos.col;
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

    public Size(Size size)
    {
      this.width  = size.width; 
      this.height = size.height;
    }
  }                     
}
