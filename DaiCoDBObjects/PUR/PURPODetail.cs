/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 09/03/2011
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class PURPODetail : IObject
  {
    #region Fields
    private long pid;
    private string pONo;
    private long pRDetailPid;
    private int status;
    private double quantity;
    private double price;
    private long currencyPid;
    private double exchangeRate;
    private int vAT;
    private double amountWithoutVAT;
    private double amountIncludeVAT;
    private string remark;
    #endregion Fields

    #region Constructors
    public PURPODetail()
    {
      this.pid = long.MinValue;
      this.pONo = string.Empty;
      this.pRDetailPid = long.MinValue;
      this.status = int.MinValue;
      this.quantity = double.MinValue;
      this.price = double.MinValue;
      this.currencyPid = long.MinValue;
      this.exchangeRate = double.MinValue;
      this.vAT = int.MinValue;
      this.amountWithoutVAT = double.MinValue;
      this.amountIncludeVAT = double.MinValue;
      this.remark = string.Empty;
    }
    public object Clone()
    {
      PURPODetail obj = new PURPODetail();
      obj.Pid = this.pid;
      obj.PONo = this.pONo;
      obj.PRDetailPid = this.pRDetailPid;
      obj.Status = this.status;
      obj.Quantity = this.quantity;
      obj.Price = this.price;
      obj.CurrencyPid = this.currencyPid;
      obj.ExchangeRate = this.exchangeRate;
      obj.VAT = this.vAT;
      obj.AmountWithoutVAT = this.amountWithoutVAT;
      obj.AmountIncludeVAT = this.amountIncludeVAT;
      obj.Remark = this.remark;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPURPODetail";
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
    public string PONo
    {
      get { return this.pONo; }
      set { this.pONo = value; }
    }
    public long PRDetailPid
    {
      get { return this.pRDetailPid; }
      set { this.pRDetailPid = value; }
    }
    public int Status
    {
      get { return this.status; }
      set { this.status = value; }
    }
    public double Quantity
    {
      get { return this.quantity; }
      set { this.quantity = value; }
    }
    public double Price
    {
      get { return this.price; }
      set { this.price = value; }
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
    public int VAT
    {
      get { return this.vAT; }
      set { this.vAT = value; }
    }
    public double AmountWithoutVAT
    {
      get { return this.amountWithoutVAT; }
      set { this.amountWithoutVAT = value; }
    }
    public double AmountIncludeVAT
    {
      get { return this.amountIncludeVAT; }
      set { this.amountIncludeVAT = value; }
    }
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
    }
    #endregion Properties
  }
}