using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class ConsoleWindows : IDisposable
  {
    #region private variables, properties

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

    public Window<ConsoleWindowsViewModel>  rootWindow      { get; private set; }
    public VirtualConsole                   virtualConsole  { get; private set; }

    public int rows { get { return virtualConsole.rows; } }
    public int cols { get { return virtualConsole.cols; } }

    #endregion

    #region constructor

    public ConsoleWindows(VirtualConsole virtualConsole)
    {
      if (String.IsNullOrWhiteSpace(Console.Title))
      {
        var args = Environment.GetCommandLineArgs();
        Console.Title = args[0];                                                                  // or AssemblyDirectory;
      }   
      
      this.virtualConsole = virtualConsole;  
      this.virtualConsole.previewReadedKeyInternal = (consoleKeyInfo) => ConsoleKeyHappen(consoleKeyInfo);

      var area = new Area(0, 0, virtualConsole.cols, Console.WindowHeight, (WinColor)(int)Console.ForegroundColor, (WinColor)(int)Console.BackgroundColor);

      rootWindow = new Window<ConsoleWindowsViewModel>(viewModel, area);

      rootWindow._consoleWindows = this;                                                          // Only in root filled. (All window can seek it by recursive way... it's good for freedom of attach/detach windows)

      this.SetDefaultLayout();                                                                    // ConsoleWindowsExtension.cs
    }

    private bool ConsoleKeyHappen(ConsoleKeyInfo consoleKeyInfo)
    {
      throw new NotImplementedException("ConsoleKeyHappen");
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

    #region Start

    public void Start()
    {

    }

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
        this.virtualConsole.previewReadedKeyInternal = null;
        this.virtualConsole                          = null;
      }
    }
    #endregion

    #endregion
  }
}

// https://technet.microsoft.com/en-us/library/mt427362.aspx