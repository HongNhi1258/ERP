/*
  Author      : Ha Anh
  Description : Object Group - UI
  Date        : 13-10-2011
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class GNRGroupUI : IObject
  {
    #region Fields
    private long pid;
    private long groupPid;
    private string uICode;
    private long uIPid;
    #endregion Fields

    #region Constructors
    public GNRGroupUI()
    {
      this.pid = long.MinValue;
      this.groupPid = long.MinValue;
      this.uICode = string.Empty;
      this.uIPid = long.MinValue;
    }
    public object Clone()
    {
      GNRGroupUI obj = new GNRGroupUI();
      obj.Pid = this.pid;
      obj.GroupPid = this.groupPid;
      obj.UICode = this.uICode;
      obj.UIPid = this.uIPid;
      return obj;
    }
    public string GetTableName()
    {
      return "TblGNRGroupUI";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Pid" };
    }
    #endregion Constructors

    #region Properties

    public long Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public long GroupPid
    {
      get { return this.groupPid; }
      set { this.groupPid = value; }
    }
    public string UICode
    {
      get { return this.uICode; }
      set { this.uICode = value; }
    }
    public long UIPid
    {
      get { return this.uIPid; }
      set { this.uIPid = value; }
    }
    #endregion Properties
  }
}