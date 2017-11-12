using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleWindowsDemo
{
  using System.Diagnostics;
  using System.Threading;
  using eMeL.ConsoleWindows;
  using eMeL.ConsoleWindows.Core;

  public class Program
  {
    public const StyleIndex ownStyle1 = (StyleIndex)(int)(StyleIndex.User0);
    public const StyleIndex ownStyle2 = (StyleIndex)(int)(StyleIndex.User0 + 1);
    public const StyleIndex ownStyle3 = (StyleIndex)(int)(StyleIndex.User0 + 2);

    private static Styles styles = new Styles();

    public static void Main(string[] args)
    {
      Console.Title = "ConsoleWindowsDemo | a demo for ConsoleWindows.dll | (c) eMeL, www.emel.hu";

      Console.BackgroundColor = ConsoleColor.DarkGray;
      Console.ForegroundColor = ConsoleColor.Blue;
      Console.WriteLine("**********************************************************************************");
      Console.WriteLine("*** ConsoleWindowsDemo | a demo for ConsoleWindows.dll | (c) eMeL, www.emel.hu ***");
      Console.WriteLine("**********************************************************************************");
      Console.ResetColor();

      Console.WriteLine();

      styles[ownStyle1] = new Style(WinColor.Magenta, WinColor.DarkYellow);
      styles[ownStyle2] = new Style(WinColor.Cyan,    WinColor.Green);
      styles[ownStyle3] = new Style(WinColor.Blue,    WinColor.None);

      char operation;

      if (args.Length > 0)
      { // first parameter [index: 0] is the name of program
        if (args[0].Length != 1)
        {
          Console.WriteLine("Error! The first parameter isn't a single karakter of code of operation!");
          Console.WriteLine();
          DisplayHelp(true);
          Console.WriteLine();
          Console.ReadKey();                                       
          return;
        }

        operation = args[0][0];        
      }
      else
      {
        Console.WriteLine("Select an operation code:\n");
        DisplayHelp(false);
        Console.Write("\nOperation: ");

        operation = (char)Console.ReadKey().KeyChar;

        Console.WriteLine();
        Console.WriteLine();
      }
            
      //

      switch (operation)
      {
        case 'V':                                                                        
        case 'v':
          ViewAndCangeTest1();
          break;      
          
            
        case 'B':                                                                        
        case 'b':
          BorderAndAreaTest1();
          break;     

        case 'E':                                                                        
        case 'e':
          EditFieldsTest1();
          break;     

        case 'T':                                                                        
        case 't':
          SimpleTest1();
          break;   

        default:        
          Console.WriteLine("!!!! Operation code error !!!!");
          Console.WriteLine();
          DisplayHelp(true);
   
          break;
      }
    }    

    public static void DisplayHelp(bool fullText)
    {
      if (fullText)
      {
        Console.WriteLine("usage: ConsoleWindowsDemo <operation code character>");
        Console.WriteLine("  Operation codes and meaning:");
      }

      Console.WriteLine("  'B' : Border and Area test.");
      Console.WriteLine("  'E' : Edit fields test.");
      Console.WriteLine("  'V' : View and change test.");
      Console.WriteLine("  'T' : A simple test.");
    }    

    public static void SimpleTest1()
    {
      VirtualConsole.defaultSetWindowSize = false;

      var con    = new CoreConsole("SimpleTest1 --- ConsoleWindowsDemo", 25, 80, styles);
      var conWin = new ConsoleWindows(con, con.DefaultRootWindow());

      var textArray   = Enumerable.Repeat("0123456789", conWin.cols / 10).ToArray();

      var textElement = new TextViewElement(0, 0, () => { return String.Join("", textArray);});  
      conWin.rootWindow.AddElement(textElement);

      for (int rowLoop = 1; rowLoop < conWin.rows; rowLoop++)
      {
        string text = (rowLoop % 10).ToString();
        textElement = new TextViewElement(rowLoop, 0, () => { return text; } );  
        conWin.rootWindow.AddElement(textElement);
      }

      conWin.styles[StyleIndex.User8] = new Style(WinColor.Red,  WinColor.Yellow);
      conWin.styles[StyleIndex.User9] = new Style(WinColor.Blue, WinColor.White);

      textElement = new TextViewElement(10, 10, () => { return "Pressed key:"; }, StyleIndex.User9);  
      conWin.rootWindow.AddElement(textElement);  

      string stateText = "";

      textElement = new TextViewElement(10, 24, 40, 1, StyleIndex.User8, () => { return stateText; });  
      conWin.rootWindow.AddElement(textElement);  

      con.ApplyPreviewReadedKey(consoleKeyInfo =>
          {
            string text = consoleKeyInfo.Key.ToString() + "  [";

            if (consoleKeyInfo.KeyChar != 0)
            {
              text += consoleKeyInfo.KeyChar; 
            }

            text += "]";

            if ((consoleKeyInfo.Modifiers & ConsoleModifiers.Alt) != 0)
            {
              text += " Alt";
            }

            if ((consoleKeyInfo.Modifiers & ConsoleModifiers.Shift) != 0)
            {
              text += " Shift";
            }

            if ((consoleKeyInfo.Modifiers & ConsoleModifiers.Control) != 0) 
            {
              text += " Ctrl";
            }


            stateText = text;                                                       // textElement.text
        
            return consoleKeyInfo;                                                  // pass on this key to process [but you can modify it, for example uppercase use]
          });

      conWin.Start();
    }

    public static void BorderAndAreaTest1()
    {
      var con = new CoreConsole("BorderAndAreaTest1 --- ConsoleWindowsDemo", 25, 80, styles);
      var conWin = new ConsoleWindows(con, con.DefaultRootWindow());

      var textArray = Enumerable.Repeat("0123456789", conWin.cols / 10).ToArray();
      var textElement = new TextViewElement(0, 0, () => { return String.Join("", textArray); } );  
      conWin.rootWindow.AddElement(textElement);

      for (int rowLoop = 1; rowLoop < conWin.rows; rowLoop++)
      {
        string text = (rowLoop % 10).ToString();
        textElement = new TextViewElement(rowLoop, 0, () => { return text; } );  
        conWin.rootWindow.AddElement(textElement);
      }

      var region = new Region(3, 3, 3, 3, ownStyle1);
      var area   = new Area(  6, 6, 6, 6, ownStyle2, new Border(Border.defaultBorderFrameSingle));

      var area2  = new Area( 10, 20, 20, 8, StyleIndex.Scrollbar, new Border(Border.defaultBorderFrameVertDoubleHorSingle));

      conWin.rootWindow.AddElement(region);
      conWin.rootWindow.AddElement(area);
      conWin.rootWindow.AddElement(area2);

      conWin.rootWindow.AddElement(new TextViewElement(23, 30, () => { return "...press Escape to end..."; } ));

      conWin.Start();
    }

    private static void EditFieldsTest1()
    {
      var con    = new CoreConsole("EditFieldsTest1 --- ConsoleWindowsDemo", 25, 80);
      var conWin = new ConsoleWindows(con, con.DefaultRootWindow());

      conWin.rootWindow.AddElement(new TextViewElement(23, 15, () => { return "...press Escape or alt-F4 or ctrl-C to end..."; }));

      conWin.styles[StyleIndex.User0] = new Style(WinColor.Green, WinColor.Green);
      conWin.styles[StyleIndex.User1] = new Style(WinColor.Blue,  WinColor.Blue);

      conWin.rootWindow.AddElement(new Region(5,  5, 30, 10, StyleIndex.User0));
      conWin.rootWindow.AddElement(new Region(5, 40, 30, 10, StyleIndex.User1));

      string field1 = "aaaaaa";
      string field2 = "bbb";
      string field3 = "cccccccccccccc";
      string field4 = "dd";
      string field5 = "eeeeeeeeeeeeeee eeeeeeeeeeeeeeeeeeeeeeeee eeeeeeeeeeeeeeeeeeeeeeeeeeee eeeeeeeeeeeeeeeeeeeeeeeeeeee eeeeeeeeeeeeeee eeeeeeeee";

      var textField1 = new TextEditElement( 6, 45, 20, () => { return field1; }, (s) => field1 = s );
      var textField2 = new TextEditElement( 7, 45, 20, () => { return field2; }, (s) => field2 = s );
      var textField3 = new TextEditElement( 8, 45, 20, () => { return field3; }, (s) => field3 = s );
      var textField4 = new TextEditElement( 9, 45, 20, () => { return field4; }, (s) => field4 = s );
      var textField5 = new TextEditElement(10, 45, 20, () => { return field5; }, (s) => field5 = s );

      conWin.rootWindow.AddElement(new TextViewElement( 6, 10, () => { return "edit 'aaa':"; }));
      conWin.rootWindow.AddElement(new TextViewElement( 7, 10, () => { return "edit 'bbb':"; }));
      conWin.rootWindow.AddElement(new TextViewElement( 8, 10, () => { return "edit 'ccc':"; }));
      conWin.rootWindow.AddElement(new TextViewElement( 9, 10, () => { return "edit 'ddd':"; }));
      conWin.rootWindow.AddElement(new TextViewElement(10, 10, () => { return "edit 'eee':"; }));

      conWin.rootWindow.AddElement(textField1);
      conWin.rootWindow.AddElement(textField2);
      conWin.rootWindow.AddElement(textField3);
      conWin.rootWindow.AddElement(textField4);
      conWin.rootWindow.AddElement(textField5);

      conWin.Start();

      int dummyCheckReturnContetsOf_fieldX_variables_withDebugger = 0;
    }

    public static void ViewAndCangeTest1()
    { 
      var con    = new CoreConsole("ViewAndCangeTest1 --- ConsoleWindowsDemo", styles);
      var conWin = new ConsoleWindows(con, con.DefaultRootWindow());

      var region = new Region(3, 10, 10, 10, ownStyle1);
      var area   = new Area(  6, 20, 10, 15, ownStyle2, new Border(Border.defaultBorderFramePattern1));

      conWin.rootWindow.AddElement(region);
      conWin.rootWindow.AddElement(area);

      Thread.Sleep(3000);

      var element1 = "proba1";
      var element2 = "proba2";    

      var textElement1 = new TextEditElement(10, 40, () => { return element1; }, (s) => element1 = s );
      var textElement2 = new TextEditElement(10, 60, () => { return element2; }, (s) => element2 = s );

      var textElement3 = new TextViewElement(23, 30, () => { return "...press Escape to end..."; });
      var textElement4 = new TextViewElement(1,  60, () => { return "........"; }, ownStyle3);

      conWin.rootWindow.AddElement(textElement1);
      conWin.rootWindow.AddElement(textElement2);
      
      conWin.rootWindow.AddElement(textElement4);
      conWin.rootWindow.AddElement(textElement3);

      string field1 = "aaaaaa";
      string field2 = "bbb";
      string field3 = "cccccccccccccc";
      string field4 = "dd";
      string field5 = "eeeeeeeeeeeeeee eeeeeeeeeeeeeeeeeeeeeeeee eeeeeeeeeeeeeeeeeeeeeeeeeeee eeeeeeeeeeeeeeeeeeeeeeeeeeee eeeeeeeeeeeeeee eeeeeeeee";

      conWin.rootWindow.AddElement(new TextEditElement(15, 40,     () => { return field1; }, (s) => field1 = s ));
      conWin.rootWindow.AddElement(new TextEditElement(16, 40,     () => { return field2; }, (s) => field2 = s ));
      conWin.rootWindow.AddElement(new TextEditElement(17, 40, 30, () => { return field3; }, (s) => field3 = s ));
      conWin.rootWindow.AddElement(new TextEditElement(18, 40,     () => { return field4; }, (s) => field4 = s ));
      conWin.rootWindow.AddElement(new TextEditElement(19, 40, 30, () => { return field5; }, (s) => field5 = s ));

      Task task = conWin.Start(false);                                                          // Don't wait
      
      Thread.Sleep(3000);

      region.row += 5;
      area.border = new Border(Border.defaultBorderFramePattern2);

      conWin.rootWindow.IndicateChange();

      Thread.Sleep(3000);

      field1 = "PROBA1";                                                                          // textElement1.text 
      conWin.rootWindow.IndicateChange(); 

      Thread.Sleep(3000);

      field2 = "PROBA2";                                                                          // textElement2.text
      conWin.rootWindow.IndicateChange(); 

      Task.Run(() =>
      {
        for (int i = 0; i < 100; i++)
        {
          field4 = (i).ToString();                                                                // textElement4.text
          conWin.rootWindow.IndicateChange();
          Thread.Sleep(1000);
        }
      });  
      
      task.Wait();

      field3 = "  !!!! STOPPED !!!!";                                                             // textElement3.text
      conWin.rootWindow.IndicateChange();

      Thread.Sleep(10000);
    }
  }
}
