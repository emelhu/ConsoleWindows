namespace eMeL.ConsoleWindows
{
  public class Window : IArea, IRegion
  {
    public int      top     { get; set; }
    public int      left    { get; set; }
    public int      width   { get; set; }
    public int      height  { get; set; }

    public WinColor color   { get; set; }

    public Border   border  { get; set; }

    public Window(IRegion region)
    {
      this.top    = region.top;   
      this.left   = region.left;  
      this.width  = region.width; 
      this.height = region.height;
      
      this.color  = region.color;
      
      this.border = new Border(Border.defaultBorderNoFrame);
    }

    public Window(IArea area)
    {
      this.top    = area.top;   
      this.left   = area.left;  
      this.width  = area.width; 
      this.height = area.height;
      
      this.color  = area.color;
      
      this.border = area.border;
    }

    public Window(int top, int left, int width, int height, WinColor color = WinColor.None)
    {
      this.top    = top;   
      this.left   = left;  
      this.width  = width; 
      this.height = height;
      
      this.color  = color;
      
      this.border = new Border(Border.defaultBorderNoFrame);
    }

    public Window(int top, int left, int width, int height, Border border, WinColor color = WinColor.None)
    {
      this.top    = top;   
      this.left   = left;  
      this.width  = width; 
      this.height = height;
      
      this.color  = color;
      
      this.border = border;
    }
  }
}