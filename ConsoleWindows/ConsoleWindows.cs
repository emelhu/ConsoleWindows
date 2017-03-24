﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// https://www.codeproject.com/articles/335909/embedding-a-console-in-a-c-application
// https://github.com/dwmkerr/consolecontrol

// https://www.dotnetperls.com/console-color          -- to see a palette picture 

namespace eMeL.ConsoleWindows
{
  public class ConsoleWindows : IDisposable
  {
    #region public variables, properties

    public Window<IViewModel> rootWindow { get; private set; }
    public VirtualConsole virtualConsole { get; private set; }

    public int rows { get { return virtualConsole.rows; } }
    public int cols { get { return virtualConsole.cols; } }

    public Window<IViewModel> actualWindow { get; private set; }

    public Styles styles { get { return virtualConsole.styles; } }

    #endregion

    #region constructor

    public ConsoleWindows(VirtualConsole virtualConsole, Window<IViewModel> rootWindow)
    {
      if (virtualConsole == null)
      {
        throw new NullReferenceException("ConsoleWindows(VirtualConsole virtualConsole): there is a null parameter!");
      }

      if (rootWindow == null)
      {
        throw new NullReferenceException("ConsoleWindows(Window<IViewModel> rootWindow): there is a null parameter!");
      }

      if (!(rootWindow.viewModel is RootWindowViewModel))
      {
        throw new NullReferenceException("ConsoleWindows(Window<IViewModel> rootWindow): rootWindow.viewModel is not a RootWindowViewModel!");
      }

      if (String.IsNullOrWhiteSpace(Console.Title))
      {
        var args = Environment.GetCommandLineArgs();
        Console.Title = args[0];                                                                  // or AssemblyDirectory;
      }

      rootWindow._consoleWindows = this;                                                          // Only in root filled. (All window can seek it by recursive way... it's good for freedom of attach/detach/orphan windows)

      this.virtualConsole = virtualConsole;
      this.rootWindow = rootWindow;

      actualWindow = rootWindow;
      actualWindow.ToFirstItem();

      while (actualWindow.actualChildWindow != null)
      {
        actualWindow = actualWindow.actualChildWindow;
        actualWindow.ToFirstItem();
      }
    }

    #endregion

    #region public

    public static string AssemblyDirectory
    {
      get
      {
        string codeBase = Assembly.GetEntryAssembly().CodeBase;
        UriBuilder uri = new UriBuilder(codeBase);
        string path = Uri.UnescapeDataString(uri.Path);
        return Path.GetDirectoryName(path);
      }
    }

    #endregion

    #region statics

    public static ElementDescriptionInfo? GetDescription(Type type)
    {
      var attribute = type.GetTypeInfo().GetCustomAttribute<ElementDescriptionAttribute>();

      if (attribute == null)
      {
        return null;
      }
      else
      {
        return attribute.description;
      }
    }

    internal void DisplayPart(ref DisplayConsolePartInfo partInfo)
    {
      if ((partInfo.width < 1) || (partInfo.height < 1))
      {
        return;                                                                                   // there is no work to do                                                                     
      }

      Debug.Assert(actualWindow != null);

      bool   isActualWindow   = ReferenceEquals(partInfo.region, actualWindow);                 
      bool   isActualElement  = ReferenceEquals(partInfo.region, actualWindow.actualElement);                 
      bool   isActualItem     = isActualWindow || isActualElement;                 
      string displayText      = isActualItem ? partInfo.editText : partInfo.displayText;

      if (isActualElement)
      {
        SaveActualCursorPosition(ref partInfo);
      }

      if (isActualItem)
      {
        var styleActual = styles[isActualWindow ? actualWindow.styleIndexActualWindow : actualWindow.styleIndexActualElement];

        if (styleActual.foreground != WinColor.None)
        {
          partInfo.style.foreground = styleActual.foreground;
        }

        if (styleActual.background != WinColor.None)
        {
          partInfo.style.background = styleActual.background;
        }
      }      

      if (displayText == null)
      { // Only paint area by space
        var text = new string(' ', partInfo.width);

        for (int rowLoop = 0; rowLoop < partInfo.height; rowLoop++)
        {
          this.virtualConsole.Write(partInfo.row + rowLoop, partInfo.col, text, ref partInfo.style);
        }

        Border? partInfoBorder = partInfo.border;

        if (isActualItem)
        {
          Border? borderActual = partInfo.borderActual;

          if (borderActual != null)
          {
            partInfoBorder = borderActual; 
          }
        }

        if (partInfoBorder != null)
        {
          var border = (Border)partInfoBorder;

          if (border.isVisible())
          {
            int lastCol = partInfo.col + partInfo.width - 1;
            int lastRow = partInfo.row + partInfo.height - 1;
            var borderStyle = styles[border.styleIndex];

            if (borderStyle.foreground == WinColor.None)
            {
              borderStyle.foreground = partInfo.style.foreground;
            }

            if (borderStyle.background == WinColor.None)
            {
              borderStyle.background = partInfo.style.background;
            }

            if (border.topLeft != '\0')
            {
              this.virtualConsole.Write(partInfo.row, partInfo.col, border.topLeft.ToString(), ref borderStyle);
            }

            if (border.topRight != '\0')
            {
              this.virtualConsole.Write(partInfo.row, lastCol, border.topRight.ToString(), ref borderStyle);
            }

            if (border.bottomLeft != '\0')
            {
              this.virtualConsole.Write(lastRow, partInfo.col, border.bottomLeft.ToString(), ref borderStyle);
            }

            if (border.bottomRight != '\0')
            {
              this.virtualConsole.Write(lastRow, lastCol, border.bottomRight.ToString(), ref borderStyle);
            }

            if ((border.top != '\0') && (partInfo.width > 2))
            {
              var line = new string(border.top, partInfo.width - 2);

              this.virtualConsole.Write(partInfo.row, partInfo.col + 1, line, ref borderStyle);
            }

            if ((border.bottom != '\0') && (partInfo.width > 2))
            {
              var line = new string(border.bottom, partInfo.width - 2);

              this.virtualConsole.Write(lastRow, partInfo.col + 1, line, ref borderStyle);
            }

            if ((border.left != '\0') && (partInfo.height > 2))
            {
              for (int rowLoop = 1; rowLoop < partInfo.height - 1; rowLoop++)
              {
                this.virtualConsole.Write(partInfo.row + rowLoop, partInfo.col, border.left.ToString(), ref borderStyle);
              }
            }

            if ((border.right != '\0') && (partInfo.height > 2))
            {
              for (int rowLoop = 1; rowLoop < partInfo.height - 1; rowLoop++)
              {
                this.virtualConsole.Write(partInfo.row + rowLoop, lastCol, border.right.ToString(), ref borderStyle);
              }
            }
          }
        }

        if ((partInfo.scrollbars != null) && (partInfo.scrollbarsInfo != null) && (partInfo.width >= 4) && (partInfo.height >= 4))
        {
          var scrollbars = (Scrollbars)partInfo.scrollbars;
          var scrollbarsInfo = partInfo.scrollbarsInfo;

          int lastCol = partInfo.col + partInfo.width - 1;
          int lastRow = partInfo.row + partInfo.height - 1;

          var scrollbarStyle = styles[scrollbars.styleIndex];

          if (scrollbarStyle.foreground == WinColor.None)
          {
            scrollbarStyle.foreground = partInfo.style.foreground;
          }

          if (scrollbarStyle.background == WinColor.None)
          {
            scrollbarStyle.background = partInfo.style.background;
          }

          if (scrollbarsInfo.leftArrowVisible && (scrollbars.horizontalLeft != '\0'))
          {
            // TODO
          }

          if (scrollbarsInfo.rightArrowVisible && (scrollbars.horizontalRight != '\0'))
          {
            // TODO
          }

          if (scrollbarsInfo.upArrowVisible && (scrollbars.verticalUp != '\0'))
          {
            // TODO
          }

          if (scrollbarsInfo.downArrowVisible && (scrollbars.verticalBottom != '\0'))
          {
            // TODO
          }

          if ((scrollbarsInfo.horizontalPosition != null) && (scrollbars.horizontalLine != '\0') && (partInfo.width >= 5))
          {
            // TODO
          }

          if ((scrollbarsInfo.verticalPosition != null) && (scrollbars.verticalLine != '\0') && (partInfo.height >= 5))
          {
            // TODO
          }
        }
      }
      else if (partInfo.height == 1)
      { // Only one line
        this.virtualConsole.Write(partInfo.row, partInfo.col, FormattedDisplayText(displayText, partInfo.width), ref partInfo.style);
      }
      else
      { // multiple line
        var lines = displayText.Split('\n');

        for (int rowLoop = 0; rowLoop < partInfo.height; rowLoop++)
        {
          string text = null;

          if (rowLoop < lines.Length)
          {
            text = lines[rowLoop];
          }

          this.virtualConsole.Write(partInfo.row + rowLoop, partInfo.col, FormattedDisplayText(text, partInfo.width), ref partInfo.style);
        }
      }
    }

    private string FormattedDisplayText(string text, int width)
    {
      if (string.IsNullOrWhiteSpace(text))
      { // Only paint area by blank
        text = new string(' ', width);
      }
      else if (text.Length > width)
      {
        text = text.Substring(0, width);
      }
      else if (text.Length < width)
      {
        text += new string(' ', (width - text.Length));
      }

      return text;
    }

    #endregion

    #region Cursor positioning

    private   IRegion   _cursorRelativePositionValidRegion  = null;
    private   Position  _cursorRelativePosition             = new Position();

    private void SaveActualCursorPosition(ref DisplayConsolePartInfo partInfo)
    {
      if (_cursorRelativePositionValidRegion != partInfo.region)
      {
        _cursorRelativePosition.row = 0;
        _cursorRelativePosition.col = 0;

        // TODO: left fit/right fit/center

        _cursorRelativePositionValidRegion = partInfo.region;
      }

      virtualConsole.actualCursorPosition.row = partInfo.row + _cursorRelativePosition.row;
      virtualConsole.actualCursorPosition.col = partInfo.col + _cursorRelativePosition.col;
    }

    #endregion

    #region Start/Process

    static string defaultCtrlC_Question = "Indeed interrupts the program run?";

    private void ExitWindowProcess()
    {
      Debug.Assert(actualWindow != null);

      bool closeable = true;

      if (actualWindow.closeable != null)
      {
        closeable = actualWindow.closeable(actualWindow);
      }

      if (closeable)
      {
        if (actualWindow.isRootWindow)
        {
          cancellationTokenSource.Cancel();
        }
        else
        {
          if (actualWindow.parentWindow == null)
          {
            throw new NullReferenceException("");
          }

          actualWindow.state = Window<IViewModel>.State.Hide;
          actualWindow = actualWindow.parentWindow;
          actualWindow.ToFirstItem();
        }
      }
    }

    private CancellationTokenSource cancellationTokenSource;

    public Task Start(bool wait = true)
    {
      cancellationTokenSource = new CancellationTokenSource();
      var cancellationToken = cancellationTokenSource.Token;

      var task = Task.Run((Action)(() =>
        {
          while (!cancellationToken.IsCancellationRequested)
          {
            Debug.Assert(this.actualWindow != null);

            while (!this.actualWindow.visible)
            { // correction
              if (this.actualWindow.isRootWindow)
              {
                if (!this.actualWindow.visible)
                {
                  this.actualWindow.state = Window<IViewModel>.State.ViewOnly;                         // correction
                }

                break;
              }

              this.actualWindow = this.actualWindow.parentWindow;
            }


            ConsoleKeyInfo? keyInfoNullable = this.virtualConsole.ReadKeyEx(cancellationToken);

            if (keyInfoNullable != null)
            {
              ConsoleKeyInfo keyInfo = (ConsoleKeyInfo)keyInfoNullable;

              switch (keyInfo.Key)
              {
                case ConsoleKey.Escape:
                  this.ExitWindowProcess();
                  break;

                case ConsoleKey.F4:                                                               // Alt-F4
                  if ((keyInfo.Modifiers & ConsoleModifiers.Alt) != 0)
                  {
                    this.ExitWindowProcess();
                  }
                  break;

                case ConsoleKey.C:                                                                // ctrl-C
                  if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                  {
                    if (this.AskBool(defaultCtrlC_Question))
                    {
                      this.cancellationTokenSource.Cancel();
                    }
                  }
                  break;

                case ConsoleKey.Enter:
                case ConsoleKey.Tab:
                  bool shiftKey = ((keyInfo.Modifiers & ConsoleModifiers.Shift) != 0);
                  this.actualWindow.ToNextItem(shiftKey);                                              // Shift-tab to previous item             
                  this.AfterElementPositioning();
                  break;

                case ConsoleKey.Home:
                  if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)                        // Ctrl-Home: to first item
                  {
                    this.actualWindow.ToFirstItem();
                    this.AfterElementPositioning();
                  }
                  else                                                                            // Home: first character in edit field
                  {
                    // TODO     
                  }
                  break;

                case ConsoleKey.End:
                  if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)                        // Ctrl-End: to last item
                  {
                    this.actualWindow.ToLastItem();
                    this.AfterElementPositioning();
                  }
                  else                                                                            // Home: last character in edit field
                  {
                    // TODO    
                  }
                  break;

                case ConsoleKey.LeftArrow:                                                        // to left character in edit field
                  // TODO
                  break;

                case ConsoleKey.RightArrow:                                                       // to right character in edit field
                  // TODO
                  break;

                case ConsoleKey.UpArrow:                                                          // to previous line in multiline edit field
                  // TODO  
                  break;

                case ConsoleKey.DownArrow:                                                        // to next line in multiline edit field
                  // TODO  
                  break;

                case ConsoleKey.PageUp:
                  bool multilineEditElement = false;  // TODO

                  if (multilineEditElement)
                  {
                    // TODO: if multiline edit
                  }
                  else
                  {
                    this.actualWindow.ToFirstItem();
                    this.AfterElementPositioning();
                  }
                  break;

                case ConsoleKey.PageDown:
                  bool multilineEditElement2 = false;  // TODO

                  if (multilineEditElement2)
                  {
                    // TODO: if multiline edit
                  }
                  else
                  {
                    this.actualWindow.ToLastItem();
                    this.AfterElementPositioning();
                  }
                  break;

                case ConsoleKey.Backspace:                                                        // delete previous character of edit field
                  this.DisplayMessage(null, MessageType.error);
                  break;

                case ConsoleKey.Delete:                                                           // delete actual character of edit field
                  this.DisplayMessage(null, MessageType.error);
                  break;

                case ConsoleKey.Insert:                                                           // insert a blank character of edit field
                  this.DisplayMessage(null, MessageType.error);
                  break;

                case ConsoleKey.Clear:                                                            // shift-numpad5
                case ConsoleKey.OemClear:                                                         // clear content of field
                  this.DisplayMessage(null, MessageType.error);
                  break;

                case ConsoleKey.Decimal:
                case ConsoleKey.OemPeriod:
                case ConsoleKey.OemComma:                                                         // change part of real number (integer <--> decimal)
                  break;

                case ConsoleKey.Subtract:                                                         // change to negative presage (integer/real field)
                case ConsoleKey.OemMinus:
                  this.DisplayMessage(null, MessageType.error);
                  break;

                case ConsoleKey.Add:                                                              // change to positive presage (integer/real field)
                case ConsoleKey.OemPlus:
                  this.DisplayMessage(null, MessageType.error);
                  break;

                case ConsoleKey.F1:                                                               // Help
                  break;

                case ConsoleKey.F12:                                                              // Refresh display                
                  if (((keyInfo.Modifiers & ConsoleModifiers.Shift) != 0) ||
                      ((keyInfo.Modifiers & ConsoleModifiers.Alt) != 0) ||
                      ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0))
                  {
                    this.virtualConsole.Display();                                                     // immediate
                  }
                  else
                  {
                    this.virtualConsole.Refresh();                                                     // request
                  }
                  break;
              }
            }
          }
        }), cancellationToken);

      if (wait)
      {
        task.Wait();                                                                                // cancellationTokenSource.Cancel() for stop waiting
      }

      return task;
    }

    public void Stop()
    {
      if (cancellationTokenSource != null)
      {
        cancellationTokenSource.Cancel();
      }
    }

    private void AfterElementPositioning()
    {
      while (actualWindow.actualChildWindow != null)
      {
        actualWindow = actualWindow.actualChildWindow;
      }

      if (actualWindow.actualElement != null)
      {
        DisplayMessage(actualWindow.actualElement.description, MessageType.description);

        if (actualWindow.actualElement is IValidating validating)
        {
          string errorMessage = validating.IsValid();

          if (!String.IsNullOrWhiteSpace(errorMessage))
          {
            DisplayMessage(errorMessage, MessageType.error);
          }
        }
      }
    }
    #endregion

    #region Asks & messages & communication

    public static string defaultTrueText = "yes";
    public static string defaultFalseText = "no";

    public string trueText = defaultTrueText;
    public string falseText = defaultFalseText;

    public bool AskBool(string message, string trueText = null, string falseText = null)
    {
      trueText = trueText ?? this.trueText;
      falseText = falseText ?? this.falseText;

      // TODO

      return false;     // TODO
    }

    public enum MessageType
    {
      description,
      message,
      error,
      ask
    }

    public AnswerDisplayInfo? DisplayMessage(string message, MessageType messageType = MessageType.message, int askLength = 0, bool? useErrorLine = null)
    {
      int dispRow;
      Style style;

      CheckAndNormalizeTextLineNumbers();

      switch (messageType)
      {
        case MessageType.description:
          dispRow = (useErrorLine ?? false) ? messageTextLine : descriptionTextLine;
          style = styles[StyleIndex.Description];
          break;

        case MessageType.error:
          dispRow = (useErrorLine ?? true) ? messageTextLine : descriptionTextLine;
          style = styles[StyleIndex.Error];
          break;

        case MessageType.ask:
          dispRow = (useErrorLine ?? true) ? messageTextLine : descriptionTextLine;
          style = styles[StyleIndex.Question];
          break;

        case MessageType.message:
          dispRow = (useErrorLine ?? true) ? messageTextLine : descriptionTextLine;
          style = styles[StyleIndex.Message];
          break;

        default:
          throw new ArgumentException("DisplayMessage(): Invalid MessageType parameter!", nameof(messageType));
      }

      int backWidth = virtualConsole.cols;
      int leftMargin = 0;                                                                  // TODO: !
      int rightMargin = 0;                                                                  // TODO: !  

      backWidth -= (leftMargin + rightMargin);

      if (string.IsNullOrWhiteSpace(message))
      { // Clear message
        this.virtualConsole.Write(dispRow, leftMargin, new string(' ', backWidth), ref style);

        return null;
      }
      else
      {
        this.virtualConsole.Write(dispRow, leftMargin, new string(' ', backWidth), ref style);

        message = message.Trim();

        if (messageType == MessageType.ask)
        {
          askLength = Math.Min(Math.Max(askLength, 1), 12);

          int freeSpace = (backWidth - askLength) - message.Length;

          if (freeSpace > 2)
          {
            message += " : ";
          }
          else if (freeSpace > 1)
          {
            message += ": ";
          }
          else if (freeSpace > 0)
          {
            message += ":";
          }

          if (message.Length > (backWidth - askLength))
          {
            message = message.Substring(0, (backWidth - askLength));
          }

          int displayPosition = (backWidth - askLength - message.Length) / 2;

          var answerInfo = new AnswerDisplayInfo(dispRow, leftMargin + displayPosition + message.Length, askLength, ref style);
          this.virtualConsole.Write(dispRow, leftMargin + displayPosition, message, ref style);
          DisplayAnswer(ref answerInfo);

          return answerInfo;
        }
        else
        {
          if (message.Length < backWidth)
          {
            string padText = new string(' ', (backWidth - message.Length) / 2);

            message = padText + message + padText;

            if (message.Length < backWidth)
            {
              message += ' ';
            }
          }

          if (message.Length > backWidth)
          {
            message = message.Substring(0, backWidth);
          }
        }
      }

      this.virtualConsole.Write(dispRow, leftMargin + ((backWidth - message.Length) / 2), message, ref style);

      return null;
    }

    private void CheckAndNormalizeTextLineNumbers()
    {
      if (descriptionTextLine < 0)
      {
        descriptionTextLine = virtualConsole.rows - descriptionTextLine;
      }

      if (messageTextLine < 0)
      {
        messageTextLine = virtualConsole.rows - messageTextLine;
      }

      if (descriptionTextLine < 0)
      {
        descriptionTextLine = 0;
      }

      if (messageTextLine < 0)
      {
        messageTextLine = 0;
      }

      if (descriptionTextLine >= virtualConsole.rows)
      {
        descriptionTextLine = virtualConsole.rows - 1;
      }

      if (messageTextLine >= virtualConsole.rows)
      {
        messageTextLine = virtualConsole.rows - 1;
      }

      if (descriptionTextLine == messageTextLine)
      {
        descriptionTextLine--;
      }
    }

    private void DisplayAnswer(ref AnswerDisplayInfo answerInfo)
    {
      this.virtualConsole.Write(answerInfo.row, answerInfo.col, answerInfo.fulltext, ref answerInfo.style);
    }

    public struct AnswerDisplayInfo
    {
      public int row;
      public int col;
      public int len;
      public string text;
      public char placeholder;
      public Style style;

      public AnswerDisplayInfo(int row, int col, int len, ref Style style)
      {
        this.row = row;
        this.col = col;
        this.len = len;
        this.text = String.Empty;
        this.placeholder = '□';
        this.style = style;
      }

      public string fulltext
      {
        get
        {
          if (String.IsNullOrEmpty(text))
          {
            text = String.Empty;
          }

          return text + new string(this.placeholder, this.len - text.Length);
        }
      }
    }

    #region common

    #region description
    /// <summary>
    /// when positive, it's a line number, when negative it's a relative position to count of rows
    /// </summary>
    public static int defaultDescriptionTextLine = -2;

    /// <summary>
    /// when positive, it's a line number, when negative it's a relative position to count of rows
    /// </summary>
    public int descriptionTextLine = defaultDescriptionTextLine;
    #endregion

    #region error/message/question                       
    ///
    /// when positive, it's a line number, when negative it's a relative position to count of rows
    /// 
    public static int defaultMessageTextLine = -1;

    /// <summary>
    /// when positive, it's a line number, when negative it's a relative position to count of rows
    /// </summary>
    public int messageTextLine = defaultMessageTextLine;
    #endregion

    #region message texts
    public static string errorText_Empty = "Do not leave empty this element!";
    #endregion

    #endregion

    #endregion

    #region IDisposable implementation

    ~ConsoleWindows()
    {
      Dispose();
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      if (this.virtualConsole != null)
      {
        this.virtualConsole = null;
      }

      if (this.rootWindow != null)
      {
        this.rootWindow = null;
      }
    }
    #endregion    
  }
}

// https://technet.microsoft.com/en-us/library/mt427362.aspx