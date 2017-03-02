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
      Console.InputEncoding   = Encoding.Unicode;
      Console.OutputEncoding  = Encoding.Unicode;
      Console.SetBufferSize(Console.BufferWidth, Console.BufferHeight);

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

    #endregion
  }
}
