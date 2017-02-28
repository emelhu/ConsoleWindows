using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public interface IArea : IRegion
  {
    Border     border     { get; set; }
    Scrollbars scrollbars { get; set; }
  }
}
