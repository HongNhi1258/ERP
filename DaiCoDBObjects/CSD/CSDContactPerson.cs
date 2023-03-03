/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 07/10/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class CSDContactPerson : IObject
  {
    #region Fields
    private long pid;
    private long customerPid;
    private string name;
    private string position;
    private string tel1;
    private string tel2;
    private string email1;
    private string email2;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public CSDContactPerson()
    {
      this.pid = long.MinValue;
      this.customerPid = long.MinValue;
      this.name = string.Empty;
      this.position = string.Empty;
      this.tel1 = string.Empty;
      this.tel2 = string.Empty;
      this.email1 = string.Empty;
      this.email2 = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      CSDContactPerson obj = new CSDContactPerson();
      obj.Pid = this.pid;
      obj.CustomerPid = this.customerPid;
      obj.Name = this.name;
      obj.Position = this.position;
      obj.Tel1 = this.tel1;
      obj.Tel2 = this.tel2;
      obj.Email1 = this.email1;
      obj.Email2 = this.email2;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblCSDContactPerson";
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
    public long CustomerPid
    {
      get { return this.customerPid; }
      set { this.customerPid = value; }
    }
    public string Name
    {
      get { return this.name; }
      set { this.name = value; }
    }
    public string Position
    {
      get { return this.position; }
      set { this.position = value; }
    }
    public string Tel1
    {
      get { return this.tel1; }
      set { this.tel1 = value; }
    }
    public string Tel2
    {
      get { return this.tel2; }
      set { this.tel2 = value; }
    }
    public string Email1
    {
      get { return this.email1; }
      set { this.email1 = value; }
    }
    public string Email2
    {
      get { return this.email2; }
      set { this.email2 = value; }
    }
    public int CreateBy
    {
      get { return this.createBy; }
      set { this.createBy = value; }
    }
    public DateTime CreateDate
    {
      get { return this.createDate; }
      set { this.createDate = value; }
    }
    public int UpdateBy
    {
      get { return this.updateBy; }
      set { this.updateBy = value; }
    }
    public DateTime UpdateDate
    {
      get { return this.updateDate; }
      set { this.updateDate = value; }
    }
    #endregion Properties
  }
}