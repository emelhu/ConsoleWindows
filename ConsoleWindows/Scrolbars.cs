using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public struct Scrollbars : IComparable
  {
    public const int  numberOfScrollbarsChars = 7;

    private char[] scrollbarsChars;

    public StyleIndex   styleIndex { get; set; }

    public const string defaultNoneScrollbars                     = null;                         // "\0\0\0\0\0\0\0";

    public const string defaultScrollbarsSingle                   = "◄─►▲│▼●";
    public const string defaultScrollbarsDouble                   = "◄═►▲║▼●";
    public const string defaultScrollbarsBox1                     = "◄□►▲□▼●";
    public const string defaultScrollbarsEmptyLine                = "◄\0►▲\0▼\0";

    public char horizontalLeft  { get { return scrollbarsChars[(int)ScrollbarsPosition.horizontalLeft ]; }  set { scrollbarsChars[(int)ScrollbarsPosition.horizontalLeft ] = value; } }
    public char horizontalLine  { get { return scrollbarsChars[(int)ScrollbarsPosition.horizontalLine ]; }  set { scrollbarsChars[(int)ScrollbarsPosition.horizontalLine ] = value; } }
    public char horizontalRight { get { return scrollbarsChars[(int)ScrollbarsPosition.horizontalRight]; }  set { scrollbarsChars[(int)ScrollbarsPosition.horizontalRight] = value; } }
    public char verticalUp      { get { return scrollbarsChars[(int)ScrollbarsPosition.verticalUp     ]; }  set { scrollbarsChars[(int)ScrollbarsPosition.verticalUp     ] = value; } }
    public char verticalLine    { get { return scrollbarsChars[(int)ScrollbarsPosition.verticalLine   ]; }  set { scrollbarsChars[(int)ScrollbarsPosition.verticalLine   ] = value; } }
    public char verticalBottom  { get { return scrollbarsChars[(int)ScrollbarsPosition.verticalBottom ]; }  set { scrollbarsChars[(int)ScrollbarsPosition.verticalBottom ] = value; } }
    public char scrollPosition  { get { return scrollbarsChars[(int)ScrollbarsPosition.scrollPosition ]; }  set { scrollbarsChars[(int)ScrollbarsPosition.scrollPosition ] = value; } }

    public Scrollbars(char[] chars, StyleIndex styleIndex = StyleIndex.Scrollbar)
    {
      this.scrollbarsChars = new char[numberOfScrollbarsChars];
      this.styleIndex      = styleIndex;

      //this.scrollbarsChars.Initialize();                                                              // initialize all element with default value ('\0'Ö 

      if ((chars != null) && (chars.Length > 0))
      {
        for (int i = 0; i < Math.Max(chars.Length, numberOfScrollbarsChars); i++)
        {
          this.scrollbarsChars[i] = chars[i];          
        }
      }
    }

    public Scrollbars(string chars = defaultNoneScrollbars, StyleIndex styleIndex = StyleIndex.Scrollbar)
    {
      this.scrollbarsChars = new char[numberOfScrollbarsChars];
      this.styleIndex      = styleIndex;

      //this.scrollbarsChars.Initialize();                                                              // initialize all element with default value ('\0'Ö

      if ((chars != null) && (chars.Length > 0))
      {
        for (int i = 0; i < Math.Max(chars.Length, numberOfScrollbarsChars); i++)
        {
          this.scrollbarsChars[i] = chars[i];          
        }
      }
    }

    public bool isVisible()
    {
      const char noScrollbarChar = '\0';

      for (int i = 0; i < numberOfScrollbarsChars; i++)
      {
        if (this.scrollbarsChars[i] != noScrollbarChar)
        {
          return true;
        }
      }

      return false;
    }

    public int CompareTo(object obj)
    {
      int ret = 1;

      if (ReferenceEquals(this, obj))
      {
        ret = 0;
      }
      else if ((obj != null) && (obj is Scrollbars otherScrollbars))
      {
        ret = ((IComparable)this.styleIndex).CompareTo(otherScrollbars.styleIndex);

        if (ret == 0)
        {
          ret = ((IComparable)this.scrollbarsChars.Length).CompareTo(otherScrollbars.scrollbarsChars.Length);

          if (ret == 0)
          {
            for (int i = 0; ((i < this.scrollbarsChars.Length) && (ret == 0)); i++)
            {
              ret = ((IComparable)this.scrollbarsChars[i]).CompareTo(otherScrollbars.scrollbarsChars[i]);
            }
          }
        }
      }

      return ret;
    }
  }

  public enum ScrollbarsPosition
  {
    horizontalLeft  = 0,
    horizontalLine  = 1,
    horizontalRight = 2,
    verticalUp      = 3,
    verticalLine    = 4,
    verticalBottom  = 5,
    scrollPosition  = 6
  }
}
