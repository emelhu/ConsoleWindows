namespace eMeL.ConsoleWindows
{
  public class Window<TViewModel> : IArea, IRegion
     where TViewModel : class
  {
    public TViewModel viewModel   { get; set; }  

    public int        top         { get; set; }
    public int        left        { get; set; }
    public int        width       { get; set; }
    public int        height      { get; set; }

    public WinColor   background  { get; set; }
    public WinColor   foreground  { get; set; }

    public Border     border      { get; set; }
    public Scrollbars scrollbars  { get; set; }

    public Window(TViewModel viewModel, IRegion region, Border? border = null, Scrollbars? scrollbars = null)
      : this(viewModel, region.top, region.left, region.width, region.height, region.foreground, region.background, border, scrollbars)
    {
    }

    public Window(TViewModel viewModel, IArea area)
      : this(viewModel, area.top, area.left, area.width, area.height, area.foreground, area.background, area.border, area.scrollbars)
    {
    }

    public Window(TViewModel viewModel, int top, int left, int width, int height, WinColor foreground = WinColor.None, WinColor background = WinColor.None, Border? border = null, Scrollbars? scrollbars = null)
    {
      this.top        = top;
      this.left       = left;
      this.width      = width;
      this.height     = height;
      this.foreground = foreground;
      this.background = background;

      this.border     = border     ?? new Border();
      this.scrollbars = scrollbars ?? new Scrollbars();
    }
    
    public Window(TViewModel viewModel, int top, int left, int width, int height, Border? border = null, Scrollbars? scrollbars = null, WinColor foreground = WinColor.None, WinColor background = WinColor.None)
      : this(viewModel, top, left, width, height, foreground, background, border, scrollbars)
    {
    }
  }
}