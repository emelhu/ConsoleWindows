using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public interface IRegion
  {
    int       top     { get; set; }
    int       left    { get; set; }
    int       width   { get; set; }
    int       height  { get; set; }

    WinColor  color   { get; set; }
  }
}
