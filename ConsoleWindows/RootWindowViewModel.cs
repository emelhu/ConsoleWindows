using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class RootWindowViewModel : IViewModel
  {
    #region IViewModel implementation
    public event ChangedEventHandler changed;

    public void IndicateChange()
    {
      if (changed != null)
      {
        changed(this);                                                                            // aka: Changed?.Invoke(this);
      }
    }
    #endregion
  }
}
