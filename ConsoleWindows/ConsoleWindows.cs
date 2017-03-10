using System;
using System.Collections.Generic;
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
    public IElement           actualElement { get; private set; }

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
      
      this.virtualConsole = virtualConsole;  

      var area = new Area(0, 0, virtualConsole.cols, Console.WindowHeight, (WinColor)(int)Console.ForegroundColor, (WinColor)(int)Console.BackgroundColor);

      this.rootWindow = rootWindow;

      rootWindow._consoleWindows = this;                                                          // Only in root filled. (All window can seek it by recursive way... it's good for freedom of attach/detach/orphan windows)

      actualWindow = rootWindow;     
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
          this.virtualConsole.Write(partInfo.row + rowLoop, partInfo.col, text, partInfo.foreground, partInfo.background);
        }

        if (partInfo.border != null) 
        {
          var border = (Border)partInfo.border;

          if (border.isVisible())
          {
            int lastCol     = partInfo.col + partInfo.width  - 1; 
            int lastRow     = partInfo.row + partInfo.height - 1;
            var foreground  = border.foreground == WinColor.None ? partInfo.foreground : border.foreground;
            var background  = border.background == WinColor.None ? partInfo.background : border.background;


            if (border.topLeft != '\0')
            {
              this.virtualConsole.Write(partInfo.row, partInfo.col, border.topLeft.ToString(),      foreground, background);
            }

            if (border.topRight != '\0')
            {
              this.virtualConsole.Write(partInfo.row, lastCol,      border.topRight.ToString(),     foreground, background);
            }

            if (border.bottomLeft != '\0')
            {
              this.virtualConsole.Write(lastRow,      partInfo.col, border.bottomLeft.ToString(),   foreground, background);
            }

            if (border.bottomRight != '\0')
            {
              this.virtualConsole.Write(lastRow,      lastCol,      border.bottomRight.ToString(),  foreground, background);
            }

            if ((border.top != '\0') && (partInfo.width > 2))
            {
              var line = new string(border.top, partInfo.width - 2);

              this.virtualConsole.Write(partInfo.row, partInfo.col + 1,   line,                     foreground, background);
            }

            if ((border.bottom != '\0') && (partInfo.width > 2))
            {
              var line = new string(border.bottom, partInfo.width - 2);

              this.virtualConsole.Write(lastRow,      partInfo.col + 1,   line,                     foreground, background);
            }

            if ((border.left != '\0') && (partInfo.height > 2))
            {
              for (int rowLoop = 1; rowLoop < partInfo.height - 1; rowLoop++)
              {
                this.virtualConsole.Write(partInfo.row + rowLoop, partInfo.col, border.left.ToString(), foreground, background);
              }
            }

            if ((border.right != '\0') && (partInfo.height > 2))
            {
              for (int rowLoop = 1; rowLoop < partInfo.height - 1; rowLoop++)
              {
                this.virtualConsole.Write(partInfo.row + rowLoop, lastCol, border.right.ToString(),     foreground, background);
              }
            }
          }
        }

        if ((partInfo.scrollbars != null) && (partInfo.scrollbarsInfo != null) && (partInfo.width >= 4) && (partInfo.height >= 4))
        {
          var scrollbars      = (Scrollbars)partInfo.scrollbars;
          var scrollbarsInfo  = partInfo.scrollbarsInfo;

          int lastCol     = partInfo.col + partInfo.width  - 1; 
          int lastRow     = partInfo.row + partInfo.height - 1;
          var foreground  = scrollbars.foreground == WinColor.None ? partInfo.foreground : scrollbars.foreground;
          var background  = scrollbars.background == WinColor.None ? partInfo.background : scrollbars.background;

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
        this.virtualConsole.Write(partInfo.row, partInfo.col, FormattedDisplayText(partInfo.displayText, partInfo.width), partInfo.foreground, partInfo.background);
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

          this.virtualConsole.Write(partInfo.row + rowLoop, partInfo.col, FormattedDisplayText(text, partInfo.width), partInfo.foreground, partInfo.background);
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

    private CancellationTokenSource cancellationTokenSource;

    public void Start(bool wait = true)
    {
      cancellationTokenSource = new CancellationTokenSource();
      var cancellationToken   = cancellationTokenSource.Token;

      var task = Task.Run(() =>
        {
          while (! cancellationToken.IsCancellationRequested)
          {
            ConsoleKeyInfo? keyInfoNullable = this.virtualConsole.ReadKeyEx(cancellationToken);

            if (keyInfoNullable != null)
            {
              ConsoleKeyInfo keyInfo = (ConsoleKeyInfo)keyInfoNullable;

              switch (keyInfo.Key)
              {
                case ConsoleKey.Escape:
                  // TODO: ablak kilépési engedély vizsgálat
                  if (rootWindow.isRootWindow)
                  {
                    cancellationTokenSource.Cancel();
                  }
                  else
                  {

                  }
                  break;

                case ConsoleKey.F4:                                                               // Alt-F4
                  if ((keyInfo.Modifiers & ConsoleModifiers.Alt) != 0)
                  {
                    // TODO: ablak kilépési engedély vizsgálat
                    if (rootWindow.isRootWindow)
                    {
                      cancellationTokenSource.Cancel();
                    }
                    else
                    {

                    }
                  }
                  break;

                case ConsoleKey.C:                                                                // ctrl-C
                  if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                  {
                    // TODO: ablak kilépési engedély vizsgálat
                    cancellationTokenSource.Cancel();
                  }
                  break;

                case ConsoleKey.Enter:
                case ConsoleKey.Tab:
                  break;     
                  
                case ConsoleKey.Home:
                  break;   
                  
                case ConsoleKey.End:
                  break;      
                  
                case ConsoleKey.LeftArrow:
                  break;         

                case ConsoleKey.RightArrow:
                  break;     

                case ConsoleKey.UpArrow:
                  break;  

                case ConsoleKey.DownArrow:
                  break;  
                  
                case ConsoleKey.PageUp:
                  break;      

                case ConsoleKey.PageDown:
                  break;   

                case ConsoleKey.Backspace:
                  break;

                case ConsoleKey.Delete:
                  break;

                case ConsoleKey.Insert:
                  break;

                case ConsoleKey.Clear:                                                            // shift-numpad5
                case ConsoleKey.OemClear:
                  break;

                case ConsoleKey.Decimal:
                case ConsoleKey.OemPeriod:
                case ConsoleKey.OemComma:
                  break;

                case ConsoleKey.Subtract:
                case ConsoleKey.OemMinus:
                  break;

                case ConsoleKey.Add:
                case ConsoleKey.OemPlus:
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