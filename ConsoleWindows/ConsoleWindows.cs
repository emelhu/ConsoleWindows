using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class ConsoleWindows
  {
    #region private variables, properties

    #region Console saved start state

    public ConsoleColor  consoleStartBackground  { get; private set; }
    public ConsoleColor  consoleStartForeground  { get; private set; }

    #endregion

    private IConsoleMouse consoleMouse;    

    public class ConsoleWindowsViewModel : IViewModel
    {
      #region IViewModel implementation
      public event ChangedEventHandler Changed;

      public void IndicateChange()
      {
        if (Changed != null)
        {
          Changed(this);                                                                            // aka: Changed?.Invoke(this);
        }
      }
      #endregion
    }

    public ConsoleWindowsViewModel viewModel { get; private set; } = new ConsoleWindowsViewModel();

    #endregion

    #region public variables, properties

    public Window<ConsoleWindowsViewModel> rootWindow { get; private set; }

    #endregion

    #region constructor

    public ConsoleWindows(IConsoleMouse consoleMouse = null)
    {
      Console.SetBufferSize(Console.BufferWidth, Console.BufferHeight);

      Console.InputEncoding   = Encoding.Unicode;
      Console.OutputEncoding  = Encoding.Unicode;
      
      consoleStartBackground  = Console.BackgroundColor;
      consoleStartForeground  = Console.ForegroundColor;

      if (String.IsNullOrWhiteSpace(Console.Title))
      {
        var args = Environment.GetCommandLineArgs();
        Console.Title = args[0];                                                            // or AssemblyDirectory;
      }
      
      if (consoleMouse != null)
      {
        var size = new Size() { width=Console.WindowWidth, height = Console.WindowHeight };
        consoleMouse.Init(Console.Title, size);
      }

      this.consoleMouse = consoleMouse;

      var area = new Area(0, 0, Console.WindowWidth, Console.WindowHeight, (WinColor)(int)Console.ForegroundColor, (WinColor)(int)Console.BackgroundColor);

      rootWindow = new Window<ConsoleWindowsViewModel>(viewModel, area);

      rootWindow._consoleWindows = this;                                                          // Only in root filled. (All window can seek it by recursive way... it's good for freedom of attach/detach windows)

      this.SetDefaultLayout();                                                                    // ConsoleWindowsExtension.cs
    }
    #endregion

    #region public

    public void ClearRootWindowLayout()
    {

    }

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

    /// <summary>
    /// There is for lock Console for write, because SetCursorPosition/ForegroundColor/BackgroundColor/Write sequence isn't atomic.
    /// </summary>
    internal static object lockConsole = new object();

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

    internal void DisplayPart(DisplayConsolePartInfo partInfo)
    {
      if ((partInfo.width < 1) || (partInfo.height < 1))
      {
        return;                                                                                   // there is no work to do                                                                     
      }

      if (partInfo.background == WinColor.None)
      {
        Console.BackgroundColor = this.consoleStartBackground;
      }
      else
      {
        Console.BackgroundColor = (ConsoleColor)(int)partInfo.background;
      }

      if (partInfo.foreground == WinColor.None)
      {
        Console.BackgroundColor = this.consoleStartForeground;
      }
      else
      {
        Console.BackgroundColor = (ConsoleColor)(int)partInfo.foreground;
      }

      if (partInfo.displayText == null)
      { // Only paint area by space
        var text = new string(' ', partInfo.width);

        for (int row = 0; row < partInfo.height; row++)
        {
          Console.SetCursorPosition(partInfo.left, partInfo.top + row);
          Console.Write(text);
        }
      }
      else if (partInfo.height == 1)
      { // Only one line
        Console.SetCursorPosition(partInfo.left, partInfo.top);
        Console.Write(FormattedDisplayText(partInfo.displayText, partInfo.width));
      }
      else
      { // multiple line
        var lines = partInfo.displayText.Split('\n');

        for (int row = 0; row < partInfo.height; row++)
        {
          string text = null;

          if (row < lines.Length)
          {
            text = lines[row];
          }

          Console.SetCursorPosition(partInfo.left, partInfo.top + row);
          Console.Write(FormattedDisplayText(text, partInfo.width));
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
  }
}

// https://technet.microsoft.com/en-us/library/mt427362.aspx