/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 09/03/2011
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class TblPURExchangeRateHistory : IObject
  {
    #region Fields
    private long pid;
    private long currencyPid;
    private double exchangeRate;
    private int createBy;
    private DateTime createDate;
    #endregion Fields

    #region Constructors
    public TblPURExchangeRateHistory()
    {
      this.pid = long.MinValue;
      this.currencyPid = long.MinValue;
      this.exchangeRate = double.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
    }
    public object Clone()
    {
      TblPURExchangeRateHistory obj = new TblPURExchangeRateHistory();
      obj.Pid = this.pid;
      obj.CurrencyPid = this.currencyPid;
      obj.ExchangeRate = this.exchangeRate;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblTblPURExchangeRateHistory";
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
    public long CurrencyPid
    {
      get { return this.currencyPid; }
      set { this.currencyPid = value; }
    }
    public double ExchangeRate
    {
      get { return this.exchangeRate; }
      set { this.exchangeRate = value; }
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
    #endregion Properties
  }
}