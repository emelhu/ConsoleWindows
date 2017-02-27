using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public struct Border
  {
    public const int  numberOfBorderChars = 8;

    private char[] borderChars;


    public char topLeft       { get { return borderChars[(int)BorderPosition.topLeft]; }      set { borderChars[(int)BorderPosition.topLeft]     = value; } }
    public char top           { get { return borderChars[(int)BorderPosition.top]; }          set { borderChars[(int)BorderPosition.top]         = value; } }
    public char topRight      { get { return borderChars[(int)BorderPosition.topRight]; }     set { borderChars[(int)BorderPosition.topRight]    = value; } }
    public char right         { get { return borderChars[(int)BorderPosition.right]; }        set { borderChars[(int)BorderPosition.right]       = value; } }
    public char bottomRight   { get { return borderChars[(int)BorderPosition.bottomRight]; }  set { borderChars[(int)BorderPosition.bottomRight] = value; } }
    public char bottom        { get { return borderChars[(int)BorderPosition.bottom]; }       set { borderChars[(int)BorderPosition.bottom]      = value; } }
    public char bottomLeft    { get { return borderChars[(int)BorderPosition.bottomLeft]; }   set { borderChars[(int)BorderPosition.bottomLeft]  = value; } }
    public char left          { get { return borderChars[(int)BorderPosition.left]; }         set { borderChars[(int)BorderPosition.left]        = value; } }

    public Border(char allChars = (char)0)
    {
      borderChars = new char[numberOfBorderChars];

      for (int i = 0; i < numberOfBorderChars; i++)
      {
        borderChars[i] = allChars;
      }     
    }

    public Border(char[] chars)
    {
      this.borderChars = new char[numberOfBorderChars];

      for (int i = 0; i < numberOfBorderChars; i++)
      {
        this.borderChars[i] = (char)0;
      }   

      if ((chars != null) && (chars.Length > 0))
      {
        for (int i = 0; i < chars.Length; i++)
        {
          this.borderChars[i] = chars[i];          
        }
      }
    }
  }

  public enum BorderPosition
  {
    topLeft     = 0,
    top         = 1,
    topRight    = 2,
    right       = 3,
    bottomRight = 4,
    bottom      = 5,
    bottomLeft  = 6,
    left        = 7
  }
}
