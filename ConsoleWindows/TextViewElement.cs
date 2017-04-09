using System;
using System.Collections.Generic;
using System.Text;

namespace eMeL.ConsoleWindows
{
  [ElementDescriptionAttribute(false, false)]                                                     // not editable, and don't use viewmodel
  public class TextViewElement : Region, IElement, ITabStop
  { 
    /// <summary>
    /// Source text to display formatted content.
    /// </summary>
    public  string  text   { get { return _text; }  set { IndicateChange(_text != value); _text = value; } }
    private string _text ;

    public string displayText
    {
      get
      {
        return text;
      }
    }

    public string description               { get; set; }

    public bool   tabStop                   { get; set; } = false;

    public Func<Object, bool>   enterCatch  { get; set; }

    #region constructors

    public TextViewElement(Region region, string text = null)
      : base(region)
    {
      this.text = text;

      Normalize();
    }


    public TextViewElement (int row, int col, int width, int height = 1, StyleIndex styleIndex = StyleIndex.TextElement, string text = null)
      : base(row, col, width, height, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextViewElement (IPosition position, int width, int height, StyleIndex styleIndex = StyleIndex.TextElement, string text = null)
    : base(position, width, height, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextViewElement (IPosition position, int width, StyleIndex styleIndex = StyleIndex.TextElement, string text = null)
    : base(position, width, 1, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextViewElement (IPosition position, ISize size, StyleIndex styleIndex = StyleIndex.TextElement, string text = null)
    : base(position, size, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextViewElement (int row, int col, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(row, col, (String.IsNullOrEmpty(text) ? 1 : text.Length), 1, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextViewElement (IPosition position, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(position, 0, 1, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextViewElement (int row, int col, int width, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(row, col, width, 1, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextViewElement (int row, int col, int width, int height, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(row, col, width, height, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextViewElement (IPosition position, int width, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(position, width, 1, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextViewElement (IPosition position, ISize size, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(position, size, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    private void Normalize()
    {
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
