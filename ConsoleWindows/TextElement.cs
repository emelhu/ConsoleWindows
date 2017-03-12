using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  [ElementDescriptionAttribute(true, false)]                                                     // editable, but don't use viewmodel
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

    /// <summary>
    /// Define a function for validate content of TextElement.
    /// It returns a null if no error, or an error text.
    /// </summary>
    public Func<IElement, string> validate  { get; set; }

    public bool   emptyEnabled              { get; set; } = true;

    public string description               { get; set; }

    #region constructors

    public TextElement(Region region, string text = null)
      : base(region)
    {
      this.text = text;

      Normalize();
    }


    public TextElement (int row, int col, int width, int height = 1, StyleIndex styleIndex = StyleIndex.TextElement, string text = null)
      : base(row, col, width, height, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (IPosition position, int width, int height, StyleIndex styleIndex = StyleIndex.TextElement, string text = null)
    : base(position, width, height, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (IPosition position, int width, StyleIndex styleIndex = StyleIndex.TextElement, string text = null)
    : base(position, width, 1, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (IPosition position, ISize size, StyleIndex styleIndex = StyleIndex.TextElement, string text = null)
    : base(position, size, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (int row, int col, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(row, col, (String.IsNullOrEmpty(text) ? 1 : text.Length), 1, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (IPosition position, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(position, 0, 1, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (int row, int col, int width, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(row, col, width, 1, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (int row, int col, int width, int height, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(row, col, width, height, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (IPosition position, int width, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(position, width, 1, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextElement (IPosition position, ISize size, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(position, size, styleIndex)
    {
      this.text = text;

      Normalize();
    }
    #endregion

    #region Initialize / default 

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

    /// <summary>
    /// Check this element validity.
    /// </summary>
    /// <returns>Error text or null if valid.</returns>
    public string IsValid()
    {
      if (! emptyEnabled && ! string.IsNullOrWhiteSpace(text))
      {
        return ConsoleWindows.errorText_Empty;                                                    // Do not leave empty this element!
      }

      if (validate != null)
      {
        return validate(this);
      }

      return null;                                                                                // This element is valid
    }
    #endregion
  }
}
