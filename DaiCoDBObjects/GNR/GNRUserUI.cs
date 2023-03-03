/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 16.10.2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class GNRUserUI : IObject
  {
    #region Fields
    private long pid;
    private long userPid;
    private string uICode;
    private long uIPid;
    #endregion Fields

    #region Constructors
    public GNRUserUI()
    {
      this.pid = long.MinValue;
      this.userPid = long.MinValue;
      this.uICode = string.Empty;
      this.uIPid = long.MinValue;
    }
    public object Clone()
    {
      GNRUserUI obj = new GNRUserUI();
      obj.Pid = this.pid;
      obj.UserPid = this.userPid;
      obj.UICode = this.uICode;
      obj.UIPid = this.uIPid;
      return obj;
    }
    public string GetTableName()
    {
      return "TblGNRUserUI";
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
    #endregion Properties
  }
}