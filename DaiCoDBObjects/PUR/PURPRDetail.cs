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
  public class PURPRDetail : IObject
  {
    #region Fields
    private long pid;
    private string pRNo;
    private string materialCode;
    private int status;
    private double quantity;
    private double price;
    private long currencyPid;
    private double exchangeRate;
    private int urgent;
    private DateTime requestDate;
    private long supplierPid;
    private string contactPerson;
    private double vAT;
    private int imported;
    private long projectPid;
    private int purchaser;
    private string remark;
    #endregion Fields

    #region Constructors
    public PURPRDetail()
    {
      this.pid = long.MinValue;
      this.pRNo = string.Empty;
      this.materialCode = string.Empty;
      this.status = int.MinValue;
      this.quantity = double.MinValue;
      this.price = double.MinValue;
      this.currencyPid = long.MinValue;
      this.exchangeRate = double.MinValue;
      this.urgent = int.MinValue;
      this.requestDate = DateTime.MinValue;
      this.supplierPid = long.MinValue;
      this.contactPerson = string.Empty;
      this.vAT = double.MinValue;
      this.imported = int.MinValue;
      this.projectPid = long.MinValue;
      this.purchaser = int.MinValue;
      this.remark = string.Empty;
    }
    public object Clone()
    {
      PURPRDetail obj = new PURPRDetail();
      obj.Pid = this.pid;
      obj.PRNo = this.pRNo;
      obj.MaterialCode = this.materialCode;
      obj.Status = this.status;
      obj.Quantity = this.quantity;
      obj.Price = this.price;
      obj.CurrencyPid = this.currencyPid;
      obj.ExchangeRate = this.exchangeRate;
      obj.Urgent = this.urgent;
      obj.RequestDate = this.requestDate;
      obj.SupplierPid = this.supplierPid;
      obj.ContactPerson = this.contactPerson;
      obj.VAT = this.vAT;
      obj.Imported = this.imported;
      obj.ProjectPid = this.projectPid;
      obj.Purchaser = this.purchaser;
      obj.Remark = this.remark;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPURPRDetail";
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
    public string PRNo
    {
      get { return this.pRNo; }
      set { this.pRNo = value; }
    }
    public string MaterialCode
    {
      get { return this.materialCode; }
      set { this.materialCode = value; }
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
    public int Urgent
    {
      get { return this.urgent; }
      set { this.urgent = value; }
    }
    public DateTime RequestDate
    {
      get { return this.requestDate; }
      set { this.requestDate = value; }
    }
    public long SupplierPid
    {
      get { return this.supplierPid; }
      set { this.supplierPid = value; }
    }
    public string ContactPerson
    {
      get { return this.contactPerson; }
      set { this.contactPerson = value; }
    }
    public double VAT
    {
      get { return this.vAT; }
      set { this.vAT = value; }
    }
    public int Imported
    {
      get { return this.imported; }
      set { this.imported = value; }
    }
    public long ProjectPid
    {
      get { return this.projectPid; }
      set { this.projectPid = value; }
    }
    public int Purchaser
    {
      get { return this.purchaser; }
      set { this.purchaser = value; }
    }
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
    }
    #endregion Properties
  }
}