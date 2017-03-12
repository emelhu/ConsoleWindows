using System;
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

namespace eMeL.ConsoleWindows
{
  public class ConsoleWindows : IDisposable
  {
    #region private variables, properties

    #endregion

    #region public variables, properties

    public Window<IViewModel> rootWindow      { get; private set; }
    public VirtualConsole     virtualConsole  { get; private set; }

    public int rows { get { return virtualConsole.rows; } }
    public int cols { get { return virtualConsole.cols; } }
    
    public Window<IViewModel> actualWindow  { get; private set; }

    public Styles styles  { get { return virtualConsole.styles; } }

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

      if (! (rootWindow.viewModel is RootWindowViewModel))
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
      this.rootWindow     = rootWindow;
     
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
    
      if (partInfo.displayText == null)
      { // Only paint area by space
        var text = new string(' ', partInfo.width);

        for (int rowLoop = 0; rowLoop < partInfo.height; rowLoop++)
        {
          this.virtualConsole.Write(partInfo.row + rowLoop, partInfo.col, text, ref partInfo.style);
        }

        if (partInfo.border != null) 
        {
          var border = (Border)partInfo.border;

          if (border.isVisible())
          {
            int lastCol     = partInfo.col + partInfo.width  - 1; 
            int lastRow     = partInfo.row + partInfo.height - 1;
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
              this.virtualConsole.Write(partInfo.row, partInfo.col, border.topLeft.ToString(),          ref borderStyle);
            }

            if (border.topRight != '\0')
            {
              this.virtualConsole.Write(partInfo.row, lastCol,      border.topRight.ToString(),         ref borderStyle);
            }

            if (border.bottomLeft != '\0')
            {
              this.virtualConsole.Write(lastRow,      partInfo.col, border.bottomLeft.ToString(),       ref borderStyle);
            }

            if (border.bottomRight != '\0')
            {
              this.virtualConsole.Write(lastRow,      lastCol,      border.bottomRight.ToString(),      ref borderStyle);
            }

            if ((border.top != '\0') && (partInfo.width > 2))
            {
              var line = new string(border.top, partInfo.width - 2);

              this.virtualConsole.Write(partInfo.row, partInfo.col + 1,   line,                         ref borderStyle);
            }

            if ((border.bottom != '\0') && (partInfo.width > 2))
            {
              var line = new string(border.bottom, partInfo.width - 2);

              this.virtualConsole.Write(lastRow,      partInfo.col + 1,   line,                         ref borderStyle);
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
                this.virtualConsole.Write(partInfo.row + rowLoop, lastCol, border.right.ToString(),     ref borderStyle);
              }
            }
          }
        }

        if ((partInfo.scrollbars != null) && (partInfo.scrollbarsInfo != null) && (partInfo.width >= 4) && (partInfo.height >= 4))
        {
          var scrollbars      = (Scrollbars)partInfo.scrollbars;
          var scrollbarsInfo  = partInfo.scrollbarsInfo;

          int lastCol         = partInfo.col + partInfo.width  - 1; 
          int lastRow         = partInfo.row + partInfo.height - 1;
          
          var scrollbarStyle  = styles[scrollbars.styleIndex];

          if (scrollbarStyle.foreground == WinColor.None)
          {
            scrollbarStyle.foreground = partInfo.style.foreground;
          }

          if (scrollbarStyle.background == WinColor.None)
          {
            scrollbarStyle.background = partInfo.style.background;
          }

          if (scrollbarsInfo.leftArrowVisible  && (scrollbars.horizontalLeft  != '\0'))
          {
            // TODO
          }
          
          if (scrollbarsInfo.rightArrowVisible && (scrollbars.horizontalRight != '\0'))
          {
            // TODO
          }

          if (scrollbarsInfo.upArrowVisible    && (scrollbars.verticalUp      != '\0'))
          {
            // TODO
          }

          if (scrollbarsInfo.downArrowVisible  && (scrollbars.verticalBottom  != '\0'))
          {
            // TODO
          }

          if ((scrollbarsInfo.horizontalPosition != null) && (scrollbars.horizontalLine != '\0') && (partInfo.width >= 5))
          {
            // TODO
          }

          if ((scrollbarsInfo.verticalPosition   != null) && (scrollbars.verticalLine   != '\0') && (partInfo.height >= 5))
          {
            // TODO
          }
        }
      }
      else if (partInfo.height == 1)
      { // Only one line
        this.virtualConsole.Write(partInfo.row, partInfo.col, FormattedDisplayText(partInfo.displayText, partInfo.width), ref partInfo.style);
      }
      else
      { // multiple line
        var lines = partInfo.displayText.Split('\n');

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

    public void Start(bool wait = true)
    {
      cancellationTokenSource = new CancellationTokenSource();
      var cancellationToken   = cancellationTokenSource.Token;

      var task = Task.Run(() =>
        {
          while (! cancellationToken.IsCancellationRequested)
          {
            Debug.Assert(actualWindow != null);

            while (! actualWindow.isVisible)
            { // correction
              if (actualWindow.isRootWindow)
              {
                if (! actualWindow.isVisible)
                {
                  actualWindow.state = Window<IViewModel>.State.ViewOnly;                         // correction
                }

                break;
              }

              actualWindow = actualWindow.parentWindow;
            }


            ConsoleKeyInfo? keyInfoNullable = this.virtualConsole.ReadKeyEx(cancellationToken);

            if (keyInfoNullable != null)
            {
              ConsoleKeyInfo keyInfo = (ConsoleKeyInfo)keyInfoNullable;

              switch (keyInfo.Key)
              {
                case ConsoleKey.Escape:
                  ExitWindowProcess();
                  break;

                case ConsoleKey.F4:                                                               // Alt-F4
                  if ((keyInfo.Modifiers & ConsoleModifiers.Alt) != 0)
                  {
                    ExitWindowProcess();
                  }
                  break;

                case ConsoleKey.C:                                                                // ctrl-C
                  if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                  {
                    if (AskBool(defaultCtrlC_Question))
                    {
                      cancellationTokenSource.Cancel();
                    }
                  }
                  break;

                case ConsoleKey.Enter:
                case ConsoleKey.Tab:
                  actualWindow.ToNextItem(((keyInfo.Modifiers & ConsoleModifiers.Shift) != 0));   // Shift-tab to previous item             
                  AfterElementPositioning();
                  break;     
                  
                case ConsoleKey.Home:
                  if ((keyInfo.Modifiers & ConsoleModifiers.Control) == 0)                        // Ctrl-Home: to first item
                  {
                    actualWindow.ToFirstItem();
                    AfterElementPositioning();
                  }
                  else                                                                            // Home: first character in edit field
                  {
                    // TODO     
                  }     
                  break;   
                  
                case ConsoleKey.End:
                  if ((keyInfo.Modifiers & ConsoleModifiers.Control) == 0)                        // Ctrl-End: to last item
                  {
                    actualWindow.ToLastItem();
                    AfterElementPositioning();
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
                    actualWindow.ToFirstItem();
                    AfterElementPositioning();
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
                    actualWindow.ToLastItem();
                    AfterElementPositioning();
                  }
                  break;   

                case ConsoleKey.Backspace:                                                        // delete previous character of edit field
                  DisplayMessage(null, MessageType.error);
                  break;

                case ConsoleKey.Delete:                                                           // delete actual character of edit field
                  DisplayMessage(null, MessageType.error);
                  break;

                case ConsoleKey.Insert:                                                           // insert a blank character of edit field
                  DisplayMessage(null, MessageType.error);
                  break;

                case ConsoleKey.Clear:                                                            // shift-numpad5
                case ConsoleKey.OemClear:                                                         // clear content of field
                  DisplayMessage(null, MessageType.error);
                  break;

                case ConsoleKey.Decimal:
                case ConsoleKey.OemPeriod:
                case ConsoleKey.OemComma:                                                         // change part of real number (integer <--> decimal)
                  break;

                case ConsoleKey.Subtract:                                                         // change to negative presage (integer/real field)
                case ConsoleKey.OemMinus:
                  DisplayMessage(null, MessageType.error);
                  break;

                case ConsoleKey.Add:                                                              // change to positive presage (integer/real field)
                case ConsoleKey.OemPlus:
                  DisplayMessage(null, MessageType.error);
                  break;

                case ConsoleKey.F1:                                                               // Help
                  break;
                
                case ConsoleKey.F12:                                                              // Refresh display                
                  if (((keyInfo.Modifiers & ConsoleModifiers.Shift)   != 0) ||
                      ((keyInfo.Modifiers & ConsoleModifiers.Alt)     != 0) ||
                      ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0))
                  {
                    virtualConsole.Display();                                                     // immediate
                  }
                  else
                  {
                    virtualConsole.Refresh();                                                     // request
                  }                 
                  break;                
              }              
            }
          }
        }, cancellationToken);

      if (wait)
      {
        task.Wait();                                                                                // cancellationTokenSource.Cancel() for stop waiting
      }
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

        string errorMessage = actualWindow.actualElement.IsValid();

        if (! String.IsNullOrWhiteSpace(errorMessage))
        {
          DisplayMessage(errorMessage, MessageType.error);
        }
      }
    }
    #endregion

    #region Asks & messages & communication
                                          
    public static string defaultTrueText  = "yes";
    public static string defaultFalseText = "no";

    public        string trueText         = defaultTrueText;
    public        string falseText        = defaultFalseText;
                                               
    public bool AskBool(string message, string trueText = null, string falseText = null)
    {
      trueText  = trueText  ?? this.trueText;
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
      int       dispRow;
      Style     style;

      switch (messageType)
      {
        case MessageType.description:
          dispRow     = (useErrorLine ?? false) ? messageTextLine : descriptionTextLine;
          style       = styles[StyleIndex.Description];
          break;

        case MessageType.error:
          dispRow     = (useErrorLine ?? true) ? messageTextLine : descriptionTextLine;
          style       = styles[StyleIndex.Error];
          break;

        case MessageType.ask:
          dispRow     = (useErrorLine ?? true) ? messageTextLine : descriptionTextLine;
          style       = styles[StyleIndex.Question];
          break;

        case MessageType.message:
          dispRow     = (useErrorLine ?? true) ? messageTextLine : descriptionTextLine;
          style       = styles[StyleIndex.Message];
          break;

        default:
          throw new ArgumentException("DisplayMessage(): Invalid MessageType parameter!", nameof(messageType));
      }

      int backWidth   = VirtualConsole.maxCols;  
      int leftMargin  = 0;                                                                  // TODO: !
      int rightMargin = 0;                                                                  // TODO: !  

      backWidth -= (leftMargin + rightMargin);     

      if (message == null)
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

    private void DisplayAnswer(ref AnswerDisplayInfo answerInfo)
    {
      this.virtualConsole.Write(answerInfo.row, answerInfo.col, answerInfo.fulltext, ref answerInfo.style);
    }

    public struct AnswerDisplayInfo
    {
      public int      row; 
      public int      col; 
      public int      len;
      public string   text;
      public char     placeholder;
      public Style    style;

      public AnswerDisplayInfo(int row, int col, int len, ref Style style)
      {
        this.row          = row; 
        this.col          = col; 
        this.len          = len;
        this.text         = String.Empty;
        this.placeholder  = '□';
        this.style        = style;
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
    public static int       defaultDescriptionTextLine    = VirtualConsole.maxRows - 2;
    public        int       descriptionTextLine           = defaultDescriptionTextLine;
    #endregion

    #region error/message/question                                                       
    public static int       defaultMessageTextLine        = VirtualConsole.maxRows - 1;
    public        int       messageTextLine               = defaultMessageTextLine;
    #endregion

    #region message texts
    public static string    errorText_Empty               = "Do not leave empty this element!";
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