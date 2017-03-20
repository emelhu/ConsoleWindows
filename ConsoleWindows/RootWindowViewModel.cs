using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class RootWindowViewModel : IViewModel
  {
    #region IViewModel implementation
    public event ChangedEventHandler changed;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IndicateChange(bool valueChanged = true)
    {
      if ((changed != null) && valueChanged)
      {
        changed(this);                                                                            // aka: Changed?.Invoke(this);
      }
    }
    #endregion
  }
}
