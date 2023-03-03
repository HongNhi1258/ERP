using System.Runtime.InteropServices;

namespace DaiCo.Shared.Utility
{
  public static class Printers
  {
    [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool SetDefaultPrinter(string Name);
  }
}
