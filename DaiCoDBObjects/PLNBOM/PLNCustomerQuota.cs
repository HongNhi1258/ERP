/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 05/07/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class PLNCustomerQuota : IObject
  {
    #region Fields
    private int pid;
    private int customerGroupPid;
    private int month;
    private int year;
    private double quota;
    #endregion Fields

    #region Constructors
    public PLNCustomerQuota()
    {
      this.pid = int.MinValue;
      this.customerGroupPid = int.MinValue;
      this.month = int.MinValue;
      this.year = int.MinValue;
      this.quota = double.MinValue;
    }
    public object Clone()
    {
      PLNCustomerQuota obj = new PLNCustomerQuota();
      obj.Pid = this.pid;
      obj.CustomerGroupPid = this.customerGroupPid;
      obj.Month = this.month;
      obj.Year = this.year;
      obj.Quota = this.quota;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPLNCustomerQuota";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Pid" };
    }
    #endregion Constructors

    #region Properties

    public int Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public int CustomerGroupPid
    {
      get { return this.customerGroupPid; }
      set { this.customerGroupPid = value; }
    }
    public int Month
    {
      get { return this.month; }
      set { this.month = value; }
    }
    public int Year
    {
      get { return this.year; }
      set { this.year = value; }
    }
    public double Quota
    {
      get { return this.quota; }
      set { this.quota = value; }
    }
    #endregion Properties
  }
}