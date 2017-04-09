using System;
using System.Collections.Generic;
using System.Text;

namespace eMeL.ConsoleWindows
{
  public enum EditMode
  {
    Text,
    Integer,
    Decimal,
    Date,
    Time,
    Bool
  }

  static class EditModeExtensions
  {
    public static EditModeInfo GetInfo(this EditMode editMode)
    {
      var info = new EditModeInfo();

      switch (editMode)
      {
        case EditMode.Text:
          break;
        case EditMode.Integer:
          break;
        case EditMode.Decimal:
          break;
        case EditMode.Date:
          break;
        case EditMode.Time:
          break;
        case EditMode.Bool:
          break;
        default:
          break;
      }

      return info;
    }
  }

  public struct EditModeInfo
  {

  }

  public enum InsertKeyMode
  {
    InsertMode,
    ReplaceMode
  }
}
