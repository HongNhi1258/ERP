/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 14.10.2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class GNRDefineUIControl : IObject
  {
    #region Fields
    private long pid;
    private string uICode;
    private string controlName;
    private string description;
    private string otherInfo;
    #endregion Fields

    #region Constructors
    public GNRDefineUIControl()
    {
      this.pid = long.MinValue;
      this.uICode = string.Empty;
      this.controlName = string.Empty;
      this.description = string.Empty;
      this.otherInfo = string.Empty;
    }
    public object Clone()
    {
      GNRDefineUIControl obj = new GNRDefineUIControl();
      obj.Pid = this.pid;
      obj.UICode = this.uICode;
      obj.ControlName = this.controlName;
      obj.Description = this.description;
      obj.OtherInfo = this.otherInfo;
      return obj;
    }
    public string GetTableName()
    {
      return "TblGNRDefineUIControl";
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
    public string UICode
    {
      get { return this.uICode; }
      set { this.uICode = value; }
    }
    public string ControlName
    {
      get { return this.controlName; }
      set { this.controlName = value; }
    }
    public string Description
    {
      get { return this.description; }
      set { this.description = value; }
    }
    public string OtherInfo
    {
      get { return this.otherInfo; }
      set { this.otherInfo = value; }
    }
    #endregion Properties
  }
}