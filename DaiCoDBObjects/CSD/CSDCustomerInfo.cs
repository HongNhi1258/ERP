/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 19/10/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class CSDCustomerInfo : IObject
  {
    #region Fields
    private long pid;
    private string customerCode;
    private int kind;
    private long parentPid;
    private long customerGroupPid;
    private string name;
    private int responsiblePerson;
    private string bankAccount;
    private string switf;
    private string bankAddress;
    private string bankName;
    private string bankAttn;
    private string customerTelephone;
    private string customerFax;
    private string customerEmail;
    private string contactPerson;
    private long country;
    private string postalCode;
    private int region;
    private string city;
    private string streetAddress;
    private string poBox;
    private long saleThroughtPid;
    private long forwarderPid;
    private int paymentTerm;
    private int priceBase;
    private double markup;
    private double discount;
    private double deposit;
    private int currencySign;
    private int priceType;
    private int enquiryExpireDays;
    private string contactRemark;
    private string shippingRemark;
    private string pricingRemark;
    private int deletedFlg;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private string terriorCode;
    private int priceOption;
    #endregion Fields

    #region Constructors
    public CSDCustomerInfo()
    {
      this.pid = long.MinValue;
      this.customerCode = string.Empty;
      this.kind = int.MinValue;
      this.parentPid = long.MinValue;
      this.customerGroupPid = long.MinValue;
      this.name = string.Empty;
      this.responsiblePerson = int.MinValue;
      this.bankAccount = string.Empty;
      this.switf = string.Empty;
      this.bankAddress = string.Empty;
      this.bankName = string.Empty;
      this.bankAttn = string.Empty;
      this.customerTelephone = string.Empty;
      this.customerFax = string.Empty;
      this.customerEmail = string.Empty;
      this.contactPerson = string.Empty;
      this.country = long.MinValue;
      this.postalCode = string.Empty;
      this.region = int.MinValue;
      this.city = string.Empty;
      this.streetAddress = string.Empty;
      this.poBox = string.Empty;
      this.saleThroughtPid = long.MinValue;
      this.forwarderPid = long.MinValue;
      this.paymentTerm = int.MinValue;
      this.priceBase = int.MinValue;
      this.markup = double.MinValue;
      this.discount = double.MinValue;
      this.deposit = double.MinValue;
      this.currencySign = int.MinValue;
      this.priceType = int.MinValue;
      this.enquiryExpireDays = int.MinValue;
      this.contactRemark = string.Empty;
      this.shippingRemark = string.Empty;
      this.pricingRemark = string.Empty;
      this.deletedFlg = int.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.terriorCode = string.Empty;
      this.priceOption = int.MinValue;
    }
    public object Clone()
    {
      CSDCustomerInfo obj = new CSDCustomerInfo();
      obj.Pid = this.pid;
      obj.CustomerCode = this.customerCode;
      obj.Kind = this.kind;
      obj.ParentPid = this.parentPid;
      obj.CustomerGroupPid = this.customerGroupPid;
      obj.Name = this.name;
      obj.ResponsiblePerson = this.responsiblePerson;
      obj.BankAccount = this.bankAccount;
      obj.Switf = this.switf;
      obj.BankAddress = this.bankAddress;
      obj.BankName = this.bankName;
      obj.BankAttn = this.bankAttn;
      obj.CustomerTelephone = this.customerTelephone;
      obj.CustomerFax = this.customerFax;
      obj.CustomerEmail = this.customerEmail;
      obj.ContactPerson = this.contactPerson;
      obj.Country = this.country;
      obj.PostalCode = this.postalCode;
      obj.Region = this.region;
      obj.City = this.city;
      obj.StreetAddress = this.streetAddress;
      obj.PoBox = this.poBox;
      obj.SaleThroughtPid = this.saleThroughtPid;
      obj.ForwarderPid = this.forwarderPid;
      obj.PaymentTerm = this.paymentTerm;
      obj.PriceBase = this.priceBase;
      obj.Markup = this.markup;
      obj.Discount = this.discount;
      obj.Deposit = this.deposit;
      obj.CurrencySign = this.currencySign;
      obj.PriceType = this.priceType;
      obj.EnquiryExpireDays = this.enquiryExpireDays;
      obj.ContactRemark = this.contactRemark;
      obj.ShippingRemark = this.shippingRemark;
      obj.PricingRemark = this.pricingRemark;
      obj.DeletedFlg = this.deletedFlg;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.terriorCode = this.terriorCode;
      obj.priceOption = this.priceOption;
      return obj;
    }
    public string GetTableName()
    {
      return "TblCSDCustomerInfo";
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
    public string CustomerCode
    {
      get { return this.customerCode; }
      set { this.customerCode = value; }
    }
    public int Kind
    {
      get { return this.kind; }
      set { this.kind = value; }
    }
    public long ParentPid
    {
      get { return this.parentPid; }
      set { this.parentPid = value; }
    }
    public long CustomerGroupPid
    {
      get { return this.customerGroupPid; }
      set { this.customerGroupPid = value; }
    }
    public string Name
    {
      get { return this.name; }
      set { this.name = value; }
    }
    public int ResponsiblePerson
    {
      get { return this.responsiblePerson; }
      set { this.responsiblePerson = value; }
    }
    public string BankAccount
    {
      get { return this.bankAccount; }
      set { this.bankAccount = value; }
    }
    public string Switf
    {
      get { return this.switf; }
      set { this.switf = value; }
    }
    public string BankAddress
    {
      get { return this.bankAddress; }
      set { this.bankAddress = value; }
    }
    public string BankName
    {
      get { return this.bankName; }
      set { this.bankName = value; }
    }
    public string BankAttn
    {
      get { return this.bankAttn; }
      set { this.bankAttn = value; }
    }
    public string CustomerTelephone
    {
      get { return this.customerTelephone; }
      set { this.customerTelephone = value; }
    }
    public string CustomerFax
    {
      get { return this.customerFax; }
      set { this.customerFax = value; }
    }
    public string CustomerEmail
    {
      get { return this.customerEmail; }
      set { this.customerEmail = value; }
    }
    public string ContactPerson
    {
      get { return this.contactPerson; }
      set { this.contactPerson = value; }
    }
    public long Country
    {
      get { return this.country; }
      set { this.country = value; }
    }
    public string PostalCode
    {
      get { return this.postalCode; }
      set { this.postalCode = value; }
    }
    public int Region
    {
      get { return this.region; }
      set { this.region = value; }
    }
    public string City
    {
      get { return this.city; }
      set { this.city = value; }
    }
    public string StreetAddress
    {
      get { return this.streetAddress; }
      set { this.streetAddress = value; }
    }
    public string PoBox
    {
      get { return this.poBox; }
      set { this.poBox = value; }
    }
    public long SaleThroughtPid
    {
      get { return this.saleThroughtPid; }
      set { this.saleThroughtPid = value; }
    }
    public long ForwarderPid
    {
      get { return this.forwarderPid; }
      set { this.forwarderPid = value; }
    }
    public int PaymentTerm
    {
      get { return this.paymentTerm; }
      set { this.paymentTerm = value; }
    }
    public int PriceBase
    {
      get { return this.priceBase; }
      set { this.priceBase = value; }
    }
    public double Markup
    {
      get { return this.markup; }
      set { this.markup = value; }
    }
    public double Discount
    {
      get { return this.discount; }
      set { this.discount = value; }
    }
    public double Deposit
    {
      get { return this.deposit; }
      set { this.deposit = value; }
    }
    public int CurrencySign
    {
      get { return this.currencySign; }
      set { this.currencySign = value; }
    }
    public int PriceType
    {
      get { return this.priceType; }
      set { this.priceType = value; }
    }
    public int EnquiryExpireDays
    {
      get { return this.enquiryExpireDays; }
      set { this.enquiryExpireDays = value; }
    }
    public string ContactRemark
    {
      get { return this.contactRemark; }
      set { this.contactRemark = value; }
    }
    public string ShippingRemark
    {
      get { return this.shippingRemark; }
      set { this.shippingRemark = value; }
    }
    public string PricingRemark
    {
      get { return this.pricingRemark; }
      set { this.pricingRemark = value; }
    }
    public int DeletedFlg
    {
      get { return this.deletedFlg; }
      set { this.deletedFlg = value; }
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
    public string TerriorCode
    {
      get { return this.terriorCode; }
      set { this.terriorCode = value; }
    }
    public int PriceOption
    {
      get { return this.priceOption; }
      set { this.priceOption = value; }
    }
    #endregion Properties
  }
}