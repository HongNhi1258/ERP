/*
  Author      : Ha Anh 
  Description : Group UI Control
  Date        : 13-10-2011
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class GNRGroupUIControl : IObject
  {
    #region Fields
    private long pid;
    private long groupPid;
    private string uICode;
    private long uIPid;
    private string controlName;
    #endregion Fields

    #region Constructors
    public GNRGroupUIControl()
    {
      this.pid = long.MinValue;
      this.groupPid = long.MinValue;
      this.uICode = string.Empty;
      this.uIPid = long.MinValue;
      this.controlName = string.Empty;
    }
    public object Clone()
    {
      GNRGroupUIControl obj = new GNRGroupUIControl();
      obj.Pid = this.pid;
      obj.GroupPid = this.groupPid;
      obj.UICode = this.uICode;
      obj.UIPid = this.uIPid;
      obj.ControlName = this.controlName;
      return obj;
    }
    public string GetTableName()
    {
      return "TblGNRGroupUIControl";
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
    public string ControlName
    {
      get { return this.controlName; }
      set { this.controlName = value; }
    }
    #endregion Properties
  }
}