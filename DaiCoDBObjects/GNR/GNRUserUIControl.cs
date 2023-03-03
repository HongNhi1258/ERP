/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 16.10.2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class GNRUserUIControl : IObject
  {
    #region Fields
    private long pid;
    private long userPid;
    private string uICode;
    private long uIPid;
    private string controlName;
    #endregion Fields

    #region Constructors
    public GNRUserUIControl()
    {
      this.pid = long.MinValue;
      this.userPid = long.MinValue;
      this.uICode = string.Empty;
      this.uIPid = long.MinValue;
      this.controlName = string.Empty;
    }
    public object Clone()
    {
      GNRUserUIControl obj = new GNRUserUIControl();
      obj.Pid = this.pid;
      obj.UserPid = this.userPid;
      obj.UICode = this.uICode;
      obj.UIPid = this.uIPid;
      obj.ControlName = this.controlName;
      return obj;
    }
    public string GetTableName()
    {
      return "TblGNRUserUIControl";
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
    public long UserPid
    {
      get { return this.userPid; }
      set { this.userPid = value; }
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