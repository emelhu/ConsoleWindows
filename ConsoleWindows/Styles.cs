using System;
using System.Collections.Generic;
using System.Text;

namespace eMeL.ConsoleWindows
{
  /// <summary>
  /// Index of style in 0..255 interval. (remain free positions for user defined styles)
  /// expansion example: public const StyleIndex ownBrilliantStyle = (StyleIndex)(int)(StyleIndex.User0);
  ///                    public const StyleIndex ownExcellentStyle = (StyleIndex)(int)(StyleIndex.User0 + 15);
  /// </summary>
  public enum StyleIndex : byte
  {
    Default           =  0,
    RootWindow        =  1,
    Window            =  2,
    WindowActual      =  3,
    Border            =  4,
    BorderActual      =  5,
    Scrollbar         =  6,
    Description       =  7,
    Message           =  8,
    Warning           =  9,
    Error             = 10,
    Question          = 11,
    TextWrite         = 12,
    TextElement       = 13,
    TextElementActual = 14,

    User0             = 32,
    User1             = User0 + 1,
    User2             = User0 + 2,
    User3             = User0 + 3,
    User4             = User0 + 4,
    User5             = User0 + 5,
    User6             = User0 + 6,
    User7             = User0 + 7,
    User8             = User0 + 8,
    User9             = User0 + 9
  }

  public struct Style                                                                             // Warning: Struct so retrieved copy is modifiable
  {
    #if DRAWCONSOLE
    // further/possible develop way a drawed console in any graphical environment 
    #else
    public WinColor   background;
    public WinColor   foreground;    

    public ConColor   sufficientBackground 
    { 
      get 
      {         
        return (this.background == WinColor.None) ? defaultBackground : (ConColor)(int)background;
      } 
    }

    public ConColor   sufficientForeground 
    { 
      get 
      { 
        return (this.foreground == WinColor.None) ? defaultForeground : (ConColor)(int)foreground;
      } 
    }
    #endif

    public Style(WinColor foreground  = WinColor.None, WinColor background = WinColor.None)
    {
      this.background  = background;
      this.foreground  = foreground;
    }

    /// <summary>
    /// If any property is insufficient (for example WinColor.None) .
    /// </summary>
    public void NormalizeInsufficient()                                                    
    {
      if (this.background == WinColor.None)
      {
        this.background = (WinColor)(int)defaultBackground;
      }

      if (this.foreground == WinColor.None)
      {
        this.foreground = (WinColor)(int)defaultForeground;
      }
    }

    public static ConColor defaultForeground = ConColor.Black;
    public static ConColor defaultBackground = ConColor.White;
  }

  public class Styles
  {
    private List<Style> styles = new List<Style>();

    public  Style       this[StyleIndex index] 
    { 
      get { CheckStyleIndex(index); return styles[(int)index]; } 
      set { CheckStyleIndex(index); styles[(int)index] = value; }
    }

    private void CheckStyleIndex(StyleIndex index)
    {
      while (styles.Count <= (int)index)
      {
        styles.Add(new Style());
      }
    }

    public Styles()
    {
      Initialize();
    }

    /// <summary>
    /// Virtual initialization, so you can override it in derived class.
    /// </summary>
    //  https://www.dotnetperls.com/console-color          -- to see a palette picture 
    public virtual void Initialize()                                                    
    {
      this[StyleIndex.Default           ] = new Style(WinColor.None,     WinColor.None); 
      this[StyleIndex.RootWindow        ] = new Style(WinColor.Black,    WinColor.White); 
      this[StyleIndex.Window            ] = new Style(WinColor.Black,    WinColor.White); 
      this[StyleIndex.WindowActual      ] = new Style(WinColor.Black,    WinColor.Gray); 
      this[StyleIndex.Border            ] = new Style(WinColor.None,     WinColor.None); 
      this[StyleIndex.BorderActual      ] = new Style(WinColor.None,     WinColor.None); 
      this[StyleIndex.Scrollbar         ] = new Style(WinColor.None,     WinColor.None); 
      this[StyleIndex.Description       ] = new Style(WinColor.DarkGray, WinColor.Gray); 
      this[StyleIndex.Message           ] = new Style(WinColor.Blue,     WinColor.Gray); 
      this[StyleIndex.Warning           ] = new Style(WinColor.Blue,     WinColor.Yellow); 
      this[StyleIndex.Error             ] = new Style(WinColor.Red,      WinColor.Yellow); 
      this[StyleIndex.Question          ] = new Style(WinColor.Green,    WinColor.Gray); 
      this[StyleIndex.TextWrite         ] = new Style(WinColor.Black,    WinColor.Gray); 
      this[StyleIndex.TextElement       ] = new Style(WinColor.Black,    WinColor.White); 
      this[StyleIndex.TextElementActual ] = new Style(WinColor.DarkBlue, WinColor.Cyan); 
    }    
  }
}
