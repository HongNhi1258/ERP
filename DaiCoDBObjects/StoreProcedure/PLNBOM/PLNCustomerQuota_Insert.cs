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
  public class PLNCustomerQuota_Insert : IStoreObject
  {
    #region Fields
    private int pid;
    private int month;
    private int year;
    private double quota;
    #endregion Fields

    #region Constructors
    public PLNCustomerQuota_Insert()
    {
      this.pid = int.MinValue;
      this.month = int.MinValue;
      this.year = int.MinValue;
      this.quota = double.MinValue;
    }
    public void GetOutputValue(IDbCommand cm)
    {
    }
    public void PrepareParameters(IDbCommand cm)
    {
      if (this.Pid != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Pid", DbType.Int32, this.Pid);
      }
      if (this.Month != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Month", DbType.Int32, this.Month);
      }
      if (this.Year != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Year", DbType.Int32, this.Year);
      }
      if (this.Quota != double.MinValue)
      {
        DataParameter.AddParameter(cm, "@Quota", DbType.Double, this.Quota);
      }
    }
    public string GetStoreName()
    {
      return "spPLNCustomerQuota_Insert";
    }
    #endregion Constructors

    #region Properties

    public int Pid
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
    public double Quota
    {
      get { return this.quota; }
      set { this.quota = value; }
    }
    #endregion Properties
  }
}