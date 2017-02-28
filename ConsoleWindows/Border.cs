using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public struct Border
  {
    public static Border noFrameBorder = new Border();

    public const int    numberOfBorderChars = 8;

    private char[]      borderChars;

    public WinColor     foreground { get; set; }
    public WinColor     background { get; set; }

    public const char   defaultBorderNoFrame                      = '\0';

    public const string defaultBorderFrameSingle                  = "┌─┐│┘─└│";
    public const string defaultBorderFrameDouble                  = "╔═╗║╝═╚║";
    public const string defaultBorderFrameVertSingleHorDouble     = "╒═╕│╛═╘│";
    public const string defaultBorderFrameVertDoubleHorSingle     = "╓─╖║╜─╙║";

    public const char   defaultBorderFramePattern1                = '░';
    public const char   defaultBorderFramePattern2                = '▒';
    public const char   defaultBorderFramePattern3                = '▓';
    public const char   defaultBorderFramePattern4                = '█';
    public const char   defaultBorderFramePattern5                = '■'; 
    public const char   defaultBorderFramePattern6                = '□';


    public char topLeft       { get { return borderChars[(int)BorderPosition.topLeft]; }      set { borderChars[(int)BorderPosition.topLeft]     = value; } }
    public char top           { get { return borderChars[(int)BorderPosition.top]; }          set { borderChars[(int)BorderPosition.top]         = value; } }
    public char topRight      { get { return borderChars[(int)BorderPosition.topRight]; }     set { borderChars[(int)BorderPosition.topRight]    = value; } }
    public char right         { get { return borderChars[(int)BorderPosition.right]; }        set { borderChars[(int)BorderPosition.right]       = value; } }
    public char bottomRight   { get { return borderChars[(int)BorderPosition.bottomRight]; }  set { borderChars[(int)BorderPosition.bottomRight] = value; } }
    public char bottom        { get { return borderChars[(int)BorderPosition.bottom]; }       set { borderChars[(int)BorderPosition.bottom]      = value; } }
    public char bottomLeft    { get { return borderChars[(int)BorderPosition.bottomLeft]; }   set { borderChars[(int)BorderPosition.bottomLeft]  = value; } }
    public char left          { get { return borderChars[(int)BorderPosition.left]; }         set { borderChars[(int)BorderPosition.left]        = value; } }

    public Border(char allChars = defaultBorderNoFrame, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
    {
      this.borderChars = new char[numberOfBorderChars];
      this.foreground  = foreground;
      this.background  = background;

      for (int i = 0; i < numberOfBorderChars; i++)
      {
        borderChars[i] = allChars;
      }     
    }

    public Border(char[] chars, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
    {
      this.borderChars = new char[numberOfBorderChars];
      this.foreground  = foreground;
      this.background  = background;

      //this.borderChars.Initialize();                                                              // initialize all element with default value ('\0') 

      if ((chars != null) && (chars.Length > 0))
      {
        for (int i = 0; i < Math.Max(chars.Length, numberOfBorderChars); i++)
        {
          this.borderChars[i] = chars[i];          
        }
      }
    }

    public Border(string chars, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
    {
      this.borderChars = new char[numberOfBorderChars];
      this.foreground  = foreground;
      this.background  = background;

      //this.borderChars.Initialize();                                                              // initialize all element with default value ('\0')

      if ((chars != null) && (chars.Length > 0))
      {
        for (int i = 0; i < Math.Max(chars.Length, numberOfBorderChars); i++)
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
