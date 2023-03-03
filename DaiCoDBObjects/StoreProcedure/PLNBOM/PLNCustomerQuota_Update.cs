/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 29/06/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System.Data;

namespace DaiCo.Objects
{
  public class PLNCustomerQuota_Update : IStoreObject
  {
    #region Fields
    private long pid;
    private int month;
    private int year;
    private double target;
    #endregion Fields

    #region Constructors
    public PLNCustomerQuota_Update()
    {
      this.pid = long.MinValue;
      this.month = int.MinValue;
      this.year = int.MinValue;
      this.target = double.MinValue;
    }
    public void GetOutputValue(IDbCommand cm)
    {
    }
    public void PrepareParameters(IDbCommand cm)
    {
      if (this.Pid != long.MinValue)
      {
        DataParameter.AddParameter(cm, "@Pid", DbType.Int64, this.Pid);
      }
      if (this.Month != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Month", DbType.Int32, this.Month);
      }
      if (this.Year != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Year", DbType.Int32, this.Year);
      }
      if (this.Target != double.MinValue)
      {
        DataParameter.AddParameter(cm, "@Target", DbType.Double, this.Target);
      }
    }
    public string GetStoreName()
    {
      return "spPLNCustomerQuota_Update";
    }
    #endregion Constructors

    #region Properties

    public long Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
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
    public double Target
    {
      get { return this.target; }
      set { this.target = value; }
    }
    #endregion Properties
  }
}