using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  /// <summary>
  /// Enabled colors for Console
  /// </summary>
  public enum ConColor
  {
    Black       =  0,
    DarkBlue    =  1,
    DarkGreen   =  2,
    DarkCyan    =  3,                                                                             // (dark blue-green).
    DarkRed     =  4,
    DarkMagenta =  5,                                                                             // (dark purplish-red).
    DarkYellow  =  6,
    Gray        =  7,
    DarkGray    =  8,
    Blue        =  9,
    Green       = 10,
    Cyan        = 11,                                                                             // (blue-green)
    Red         = 12,
    Magenta     = 13,                                                                             // (purplish-red)
    Yellow      = 14,
    White       = 15
  }

  /// <summary>
  /// Enabled colors for Console; extended with virtual color codes
  /// </summary>
  public enum WinColor
  {
    None        = -1,                                                                             // transparent or inherited
    Inherited   = -1,                                                                             // None=Inherited 
    Black       =  0,
    DarkBlue    =  1,
    DarkGreen   =  2,
    DarkCyan    =  3,                                                                             // (dark blue-green).
    DarkRed     =  4,
    DarkMagenta =  5,                                                                             // (dark purplish-red).
    DarkYellow  =  6,
    Gray        =  7,
    DarkGray    =  8,
    Blue        =  9,
    Green       = 10,
    Cyan        = 11,                                                                             // (blue-green)
    Red         = 12,
    Magenta     = 13,                                                                             // (purplish-red)
    Yellow      = 14,
    White       = 15
  }
}
