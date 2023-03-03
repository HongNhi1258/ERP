using System;

namespace Component
{
  public interface ICusControl
  {
    String UlDataSource { get; set; }
    String UlDataMember { get; set; }
    /// <summary>
    /// Init data source, data binding and events for a control
    /// </summary>
    void InitializeControl();
  }
}
