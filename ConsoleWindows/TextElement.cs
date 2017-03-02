using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  [ElementDescriptionAttribute(false, false)]                                                     // not editable, don't use viewmodel
  public class TextElement : Region, IElement
  {
    /// <summary>
    /// Source text to display formatted content.
    /// </summary>
    public  string  text   { get { return _text; }  set { _text = value; IndicateChange(); } }
    private string _text ;

    public string displayText
    {
      get
      {
        return text;
      }
    }

    #region constructors

    public TextElement(Region region, string text = null)
      : base(region)
    {
      this.text = text;

      Normalize();
    }


    public TextElement (int top, int left, int width, int height = 1, WinColor foreground = WinColor.None, WinColor background = WinColor.None, string text = null)
      : base(top, left, width, height, foreground, background)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (IPosition position, int width, int height = 1, WinColor foreground = WinColor.None, WinColor background = WinColor.None, string text = null)
    : base(position, width, height, foreground, background)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (IPosition position, ISize size, WinColor foreground = WinColor.None, WinColor background = WinColor.None, string text = null)
    : base(position, size, foreground, background)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (int top, int left, string text, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
      : base(top, left, (String.IsNullOrEmpty(text) ? 1 : text.Length), 1, foreground, background)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (IPosition position, string text, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
      : base(position, 0, 1, foreground, background)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (int top, int left, int width, string text, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
      : base(top, left, width, 1, foreground, background)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (int top, int left, int width, int height, string text, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
      : base(top, left, width, height, foreground, background)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (IPosition position, int width, string text, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
      : base(position, width, 1, foreground, background)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (IPosition position, ISize size, string text, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
      : base(position, size, foreground, background)
    {
      this.text = text;

      Normalize();
    }
    #endregion

    #region Initialize / default 

    public static WinColor defaultForeground = WinColor.None; 
    public static WinColor defaultBackground = WinColor.None;

    private void Normalize()
    {
      if (foreground == WinColor.None)
      {
        foreground = defaultForeground;
      }

      if (background == WinColor.None)
      {
        background = defaultBackground;
      }

      if (width < 1)
      {
        width = String.IsNullOrEmpty(text) ? 1 : text.Length;
      }

      if (height < 1)
      {
        height = 1;
      }
    }

    #endregion
  }
}
