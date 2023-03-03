/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 09/03/2011
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class PURSupplierContactPerson : IObject
  {
    #region Fields
    private long pid;
    private long supplierPid;
    private string name;
    private string position;
    private string mobile;
    private string email;
    private int sex;
    #endregion Fields

    #region Constructors
    public PURSupplierContactPerson()
    {
      this.pid = long.MinValue;
      this.supplierPid = long.MinValue;
      this.name = string.Empty;
      this.position = string.Empty;
      this.mobile = string.Empty;
      this.email = string.Empty;
      this.sex = int.MinValue;
    }
    public object Clone()
    {
      PURSupplierContactPerson obj = new PURSupplierContactPerson();
      obj.Pid = this.pid;
      obj.SupplierPid = this.supplierPid;
      obj.Name = this.name;
      obj.Position = this.position;
      obj.Mobile = this.mobile;
      obj.Email = this.email;
      obj.Sex = this.sex;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPURSupplierContactPerson";
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
    public long SupplierPid
    {
      get { return this.supplierPid; }
      set { this.supplierPid = value; }
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
    public string Mobile
    {
      get { return this.mobile; }
      set { this.mobile = value; }
    }
    public string Email
    {
      get { return this.email; }
      set { this.email = value; }
    }
    public int Sex
    {
      get { return this.sex; }
      set { this.sex = value; }
    }
    #endregion Properties
  }
}