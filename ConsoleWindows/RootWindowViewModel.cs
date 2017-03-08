using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  public class RootWindowViewModel : IViewModel
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
}
