/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 05/07/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class BOMUser : IObject
  {
    #region Fields
    private long pid;
    private int employeePid;
    private string userName;
    private string passwordI;
    private string passwordII;
    private DateTime createDate;
    #endregion Fields

    #region Constructors
    public BOMUser()
    {
      this.pid = long.MinValue;
      this.employeePid = int.MinValue;
      this.userName = string.Empty;
      this.passwordI = string.Empty;
      this.passwordII = string.Empty;
      this.createDate = DateTime.MinValue;
    }
    public object Clone()
    {
      BOMUser obj = new BOMUser();
      obj.Pid = this.pid;
      obj.EmployeePid = this.employeePid;
      obj.UserName = this.userName;
      obj.PasswordI = this.passwordI;
      obj.PasswordII = this.passwordII;
      obj.CreateDate = this.createDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMUser";
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
    public int EmployeePid
    {
      get { return this.employeePid; }
      set { this.employeePid = value; }
    }
    public string UserName
    {
      get { return this.userName; }
      set { this.userName = value; }
    }
    public string PasswordI
    {
      get { return this.passwordI; }
      set { this.passwordI = value; }
    }
    public string PasswordII
    {
      get { return this.passwordII; }
      set { this.passwordII = value; }
    }
    public DateTime CreateDate
    {
      get { return this.createDate; }
      set { this.createDate = value; }
    }
    #endregion Properties
  }
}