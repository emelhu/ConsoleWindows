using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic ;                                                    // Install-Package System.Dynamic.Runtime 
using System.Text.RegularExpressions;

namespace eMeL.ConsoleWindows
{
  //[ElementDescriptionAttribute(true, false)]                                                     // editable, but don't use viewmodel
  public class TextEditElement : TextViewElement, IValidating, IEditable
  {
    #region interfaces 
  
    /// <summary>
    /// Define a function for validate content of TextElement.
    /// It returns a null if no error, or an error text.
    /// </summary>
    public Func<Object, string> validate      { get; set; }                                       //// IValidating
    /// </summary>

    public bool     emptyEnabled              { get; set; } = true;                               //// IValidating

    public bool     nullEnabled               { get; set; } = false;                              //// IValidating

    public bool     readOnly                  { get; set; } = false;                              //// IEditable

    //public bool     tabStop                                                                     //// ITabStop  >>> see Normalize() for set default to TRUE;

    public dynamic  minValue                  { get; set; } = null;
    public dynamic  maxValue                  { get; set; } = null;
    public int      maxEditLength             { get; set; } = 0;
    
    public  IROPosition       CursorPosition  { get { return _cursorPosition;} }                  //// IEditable

    /// <summary>
    /// Check this element validity. --- IValidating
    /// </summary>
    /// <returns>Error text or null if valid.</returns>
    public string IsValid()                                                                       //// IValidating
    {
      if (! emptyEnabled && ! string.IsNullOrWhiteSpace(text))
      {
        return ConsoleWindows.errorText_Empty;                                                      // Do not leave empty this element!
      }

      //if ((minValue != null) && (String.Compare(text, minValue) < 0))
      //{
      //  return String.Format(ConsoleWindows.errorText_MinValue, SizeLimitedText(minValue));             // Less then minimal!
      //}

      //if ((maxValue != null) && (String.Compare(text, maxValue) > 0))
      //{
      //  return String.Format(ConsoleWindows.errorText_MaxValue, SizeLimitedText(maxValue));             // More then maximal!
      //}

      if ((maxEditLength > 0) && (text.Length > maxEditLength))
      {
        return String.Format(ConsoleWindows.errorText_MaxLength, maxEditLength);                    // Length more then maximal!
      }


      if (validate != null)
      {
        return validate(this);
      }

      return null;                                                                                  // This element is valid, 'null' signals 'no error text'
    }

    public virtual ConsoleKeyInfo? KeyPress(ConsoleKeyInfo keyInfo)                               //// IEditable --- this can be overrided but don't need because virtual called functions.
    {
      bool controlKey = ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0);
      bool shiftKey   = ((keyInfo.Modifiers & ConsoleModifiers.Shift)   != 0);
      bool altKey     = ((keyInfo.Modifiers & ConsoleModifiers.Alt)     != 0);

      switch (keyInfo.Key)
      {    
        case ConsoleKey.Home:
          ModifyRelativeCursorCol(int.MinValue / 16);
          return null;                                                                    // retun null --> swallow typed key, it's handled, so don't do anything else in caller

        case ConsoleKey.End:
          ModifyRelativeCursorCol(int.MaxValue / 16);
          return null;                                                                    // retun null --> swallow typed key, it's handled, so don't do anything else in caller

        case ConsoleKey.LeftArrow:                                                        // to left character in edit field
          ModifyRelativeCursorCol(-1);
          return null;                                                                    // retun null --> swallow typed key, it's handled, so don't do anything else in caller

        case ConsoleKey.RightArrow:                                                       // to right character in edit field
          ModifyRelativeCursorCol(+1);
          return null;                                                                    // retun null --> swallow typed key, it's handled, so don't do anything else in caller

        case ConsoleKey.UpArrow:                                                          // to previous line in multiline edit field
          if (ModifyRelativeCursorRow(-1))
          {
            return null;
          }
          break;
          
        case ConsoleKey.DownArrow:                                                        // to next line in multiline edit field
          if (ModifyRelativeCursorRow(+1))  
          {
            return null;
          }
          break;

        case ConsoleKey.PageUp:
          if (ModifyRelativeCursorRow(-(height - 1)))
          {
            return null;
          }
          break;

        case ConsoleKey.PageDown:
          if (ModifyRelativeCursorRow(height - 1))
          {
            return null;
          }
          break;

        case ConsoleKey.Enter:
        case ConsoleKey.Tab:
          return OtherPositInsideElement(keyInfo);         
          
        case ConsoleKey.Backspace:                                                        // delete previous character of edit field
          DeleteCharacter(backSpace : true);
          return null;                                                                    // retun null --> swallow typed key, it's handled, so don't do anything else in caller

        case ConsoleKey.Delete:                                                           // delete actual character of edit field
          DeleteCharacter(backSpace : false);
          return null;                                                                    // retun null --> swallow typed key, it's handled, so don't do anything else in caller

        case ConsoleKey.Insert when shiftKey:                                             // insert a blank character of edit field                  
          InsertCharacter(' ');
          return null;                                                                    // retun null --> swallow typed key, it's handled, so don't do anything else in caller

        case ConsoleKey.Clear:                                                            // shift-numpad5
        case ConsoleKey.OemClear:                                                         // clear content of field
          text = "";
          return null;                                                                    // retun null --> swallow typed key, it's handled, so don't do anything else in caller

        //case ConsoleKey.Decimal when (editable != null) && (editable.editMode == EditMode.Decimal):
        //case ConsoleKey.OemPeriod when (editable != null) && (editable.editMode == EditMode.Decimal):
        //case ConsoleKey.OemComma when (editable != null) && (editable.editMode == EditMode.Decimal):
        //case ConsoleKey.Subtract when (editable != null) && ((editable.editMode == EditMode.Decimal) || (editable.editMode == EditMode.Integer)):
        //case ConsoleKey.OemMinus when (editable != null) && ((editable.editMode == EditMode.Decimal) || (editable.editMode == EditMode.Integer)):
        //case ConsoleKey.Add when (editable != null) && ((editable.editMode == EditMode.Decimal) || (editable.editMode == EditMode.Integer)):
        //case ConsoleKey.OemPlus when (editable != null) && ((editable.editMode == EditMode.Decimal) || (editable.editMode == EditMode.Integer)):
      }

      //

      bool appropriateKey = true;

      if (enabledCharsRegexPattern != null)
      {
        Regex regex = new Regex(enabledCharsRegexPattern, RegexOptions.None);
        Match m = regex.Match(keyInfo.KeyChar.ToString());
        appropriateKey =  m.Success;
      }

      if (appropriateKey)
      {
        if (ConsoleWindows.insertKeyMode)
        {
          InsertCharacter(keyInfo.KeyChar);
        }
        else
        {
          ReplaceCharacter(keyInfo.KeyChar);
        }
      }

      return keyInfo;
    }

    #endregion

    #region cursor info

    protected Position      _textPosition;
    protected Position      _cursorPosition;

    protected virtual void  ModifyRelativeCursorCol(int columnsCount)
    {
      _cursorPosition.col += columnsCount;     
      _textPosition.col   += columnsCount;   

      NormalizePositions();
    }

    protected virtual bool  ModifyRelativeCursorRow(int rowsCount)
    { // precisely: Only one line!
      bool success = false;

      if (height > 1)
      {
        success = true;
      }

      NormalizePositions();

      return success;
    }

    protected virtual ConsoleKeyInfo  OtherPositInsideElement(ConsoleKeyInfo keyInfo)
    { // precisely: Only one line! so don't do anithing :)
      NormalizePositions();

      return keyInfo;                                                                             // key don't handled, it is passed back to caller
    }

    protected virtual void  NormalizePositions()
    {
      _cursorPosition.row = 0;                                                                    // because it's a ONE LINE edit element's normalizer

      //

      if (_cursorPosition.col < 0)
      {
        _cursorPosition.col = 0;
      }

      if (_cursorPosition.col >= width)
      {
        _cursorPosition.col = width - 1;
      }

      //
 
      if (_textPosition.col < 0)
      {
        _textPosition.col = 0;
      }

      if (_textPosition.col >= _text.Length)
      {
        _textPosition.col = _text.Length - 1;
      }

      //
  

      // TODO: ! positions !
    }

    #endregion

    public string enabledCharsRegexPattern  { get; set; } = @"[a-z]";                            // @"\d\w\W [a-z][A-Z][0-9]\.\+\-";

    #region text edit

    protected virtual void  InsertCharacter(char c)
    {
      if (_textPosition.col >= _text.Length)
      {
        text += c;              //TEST: !!!
      }
      else
      {
        text = text.Insert(_textPosition.col, c.ToString());          //TEST: !!!
      }

      NormalizePositions();
    }

    protected virtual void  ReplaceCharacter(char c)
    {
      if (_textPosition.col >= _text.Length)
      {
        text += c;                                                    //TEST: !!!
      }
      else
      {
        var arr = _text.ToCharArray();
        arr[_textPosition.col] = c;                                   //TEST: !!!
        text = new string(arr);
      }

      ModifyRelativeCursorCol(+1);

      NormalizePositions();
    }

    protected virtual void  DeleteCharacter(bool backSpace)
    {
      if (backSpace)
      {
        if ((_textPosition.col > 0) && (_textPosition.col <= _text.Length))
        {
          _text.Remove(_textPosition.col - 1);
          ModifyRelativeCursorCol(-1);     
        }
      }
      else if (_textPosition.col < _text.Length)
      {
        _text.Remove(_textPosition.col);                                              //TEST: !!! 
      }

      NormalizePositions();
    }

    #endregion

    #region constructors

    protected  TextEditElement()                                                                  // only for object hierarchy for example MemoEditElement : TextEditElement
      : base(new Region(0,0,0,0))
    {
      throw new NotImplementedException("OOP object hierarchy design error!");
    }

    public TextEditElement(Region region, string text = null)
      : base(region)
    {
      this.text = text;

      Normalize();
    }


    public TextEditElement (int row, int col, int width, int height = 1, StyleIndex styleIndex = StyleIndex.TextElement, string text = null)
      : base(row, col, width, height, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextEditElement (IPosition position, int width, int height, StyleIndex styleIndex = StyleIndex.TextElement, string text = null)
    : base(position, width, height, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextEditElement (IPosition position, int width, StyleIndex styleIndex = StyleIndex.TextElement, string text = null)
    : base(position, width, 1, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextEditElement (IPosition position, ISize size, StyleIndex styleIndex = StyleIndex.TextElement, string text = null)
    : base(position, size, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextEditElement (int row, int col, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(row, col, (String.IsNullOrEmpty(text) ? 1 : text.Length), 1, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextEditElement (IPosition position, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(position, 0, 1, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextEditElement (int row, int col, int width, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(row, col, width, 1, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextEditElement (int row, int col, int width, int height, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(row, col, width, height, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextEditElement (IPosition position, int width, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(position, width, 1, styleIndex)
    {
      this.text = text;

      Normalize();
    }

    public TextEditElement (IPosition position, ISize size, string text, StyleIndex styleIndex = StyleIndex.TextElement)
      : base(position, size, styleIndex)
    {
      this.text = text;

      Normalize();
    }
    #endregion

    //#region Initialize / default 

    protected override void Normalize()
    {
      base.Normalize();

      tabStop = true;                                                                             // Default value for Edit Element
    }



    //public string SizeLimitedText(string text, int maxLen = 40)
    //{
    //  if ((text.Length > maxLen) && (maxLen > 6))
    //  {
    //    return text.Substring(0, maxLen - 1) + "►";
    //  }

    //  return text;
    //}   

    //#endregion
  }
}
