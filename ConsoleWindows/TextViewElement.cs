using System;
using System.Collections.Generic;
using System.Text;

namespace eMeL.ConsoleWindows
{
  //[ElementDescriptionAttribute(false, false)]                                                     // not editable, and don't use viewmodel
  public class TextViewElement : Region, IElement, ITabStop
  { 
    /// <summary>
    /// Source text to display formatted content.
    /// </summary>
    protected string  text  { get { return _text; }  set { IndicateChange(_text != value); _text = value; } }        //// IElement
    private   string _text;

    public virtual string GetDisplayText(bool editFormat = false)                                 //// IElement
    {
      return text;
    }

    public Func<string>   readContent = null;                                                     // readContent = () => { return "aaa"; };

    public string description               { get; set; }                                         //// IElement

    public bool   tabStop                   { get; set; } = false;                                //// ITabStop

    #region constructors

    public TextViewElement(Region region, Func<string> readContent = null)
      : base(region)
    {
      this.text         = (readContent == null) ? null : readContent();
      this.readContent  = readContent;

      Normalize();
    }


    public TextViewElement (int row, int col, int width, int height = 1, StyleIndex styleIndex = StyleIndex.TextElement, Func<string> readContent = null)
      : base(row, col, width, height, styleIndex)
    {
      this.text         = (readContent == null) ? null : readContent();
      this.readContent  = readContent;

      Normalize();
    }

    public TextViewElement (IPosition position, int width, int height, StyleIndex styleIndex = StyleIndex.TextElement, Func<string> readContent = null)
    : base(position, width, height, styleIndex)
    {
      this.text         = (readContent == null) ? null : readContent();
      this.readContent  = readContent;

      Normalize();
    }

    public TextViewElement (IPosition position, int width, StyleIndex styleIndex = StyleIndex.TextElement, Func<string> readContent = null)
    : base(position, width, 1, styleIndex)
    {
      this.text         = (readContent == null) ? null : readContent();
      this.readContent  = readContent;

      Normalize();
    }

    public TextViewElement (IPosition position, ISize size, StyleIndex styleIndex = StyleIndex.TextElement, Func<string> readContent = null)
    : base(position, size, styleIndex)
    {
      this.text         = (readContent == null) ? null : readContent();
      this.readContent  = readContent;

      Normalize();
    }

    static protected int DefaultContentLength(Func<string> readContent)
    {
      return ((readContent == null) || String.IsNullOrEmpty(readContent()) ? 1 : readContent().Length);
    }

    public TextViewElement (int row, int col, Func<string> readContent, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(row, col, DefaultContentLength(readContent), 1, styleIndex)
    {
      this.text         = (readContent == null) ? null : readContent();
      this.readContent  = readContent;

      Normalize();
    }

    public TextViewElement (IPosition position, Func<string> readContent, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(position, 0, 1, styleIndex)
    {
      this.text         = (readContent == null) ? null : readContent();
      this.readContent  = readContent;

      Normalize();
    }

    public TextViewElement (int row, int col, int width, Func<string> readContent, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(row, col, width, 1, styleIndex)
    {
      this.text         = (readContent == null) ? null : readContent();
      this.readContent  = readContent;

      Normalize();
    }

    public TextViewElement (int row, int col, int width, int height, Func<string> readContent, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(row, col, width, height, styleIndex)
    {
      this.text         = (readContent == null) ? null : readContent();
      this.readContent  = readContent;

      Normalize();
    }

    public TextViewElement (IPosition position, int width, Func<string> readContent, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(position, width, 1, styleIndex)
    {
      this.text         = (readContent == null) ? null : readContent();
      this.readContent  = readContent;

      Normalize();
    }

    public TextViewElement (IPosition position, ISize size, Func<string> readContent, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(position, size, styleIndex)
    {
      this.text         = (readContent == null) ? null : readContent();
      this.readContent  = readContent;

      Normalize();
    }

    protected virtual void Normalize()
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
