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
  public class PURSupplierInfo : IObject
  {
    #region Fields
    private long pid;
    private string supplierCode;
    private string englishName;
    private string vietnameseName;
    private string tradeName;
    private string address;
    private int tradeType;
    private string website;
    private string taxNo;
    private string bankName;
    private string bankAccount;
    private int personInCharge;
    private int introducePerson;
    private int debit;
    private int confirm;
    private int deleteFlg;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public PURSupplierInfo()
    {
      this.pid = long.MinValue;
      this.supplierCode = string.Empty;
      this.englishName = string.Empty;
      this.vietnameseName = string.Empty;
      this.tradeName = string.Empty;
      this.address = string.Empty;
      this.tradeType = int.MinValue;
      this.website = string.Empty;
      this.taxNo = string.Empty;
      this.bankName = string.Empty;
      this.bankAccount = string.Empty;
      this.personInCharge = int.MinValue;
      this.introducePerson = int.MinValue;
      this.debit = int.MinValue;
      this.confirm = int.MinValue;
      this.deleteFlg = int.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      PURSupplierInfo obj = new PURSupplierInfo();
      obj.Pid = this.pid;
      obj.SupplierCode = this.supplierCode;
      obj.EnglishName = this.englishName;
      obj.VietnameseName = this.vietnameseName;
      obj.TradeName = this.tradeName;
      obj.Address = this.address;
      obj.TradeType = this.tradeType;
      obj.Website = this.website;
      obj.TaxNo = this.taxNo;
      obj.BankName = this.bankName;
      obj.BankAccount = this.bankAccount;
      obj.PersonInCharge = this.personInCharge;
      obj.IntroducePerson = this.introducePerson;
      obj.Debit = this.debit;
      obj.Confirm = this.confirm;
      obj.DeleteFlg = this.deleteFlg;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPURSupplierInfo";
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
    public string SupplierCode
    {
      get { return this.supplierCode; }
      set { this.supplierCode = value; }
    }
    public string EnglishName
    {
      get { return this.englishName; }
      set { this.englishName = value; }
    }
    public string VietnameseName
    {
      get { return this.vietnameseName; }
      set { this.vietnameseName = value; }
    }
    public string TradeName
    {
      get { return this.tradeName; }
      set { this.tradeName = value; }
    }
    public string Address
    {
      get { return this.address; }
      set { this.address = value; }
    }
    public int TradeType
    {
      get { return this.tradeType; }
      set { this.tradeType = value; }
    }
    public string Website
    {
      get { return this.website; }
      set { this.website = value; }
    }
    public string TaxNo
    {
      get { return this.taxNo; }
      set { this.taxNo = value; }
    }
    public string BankName
    {
      get { return this.bankName; }
      set { this.bankName = value; }
    }
    public string BankAccount
    {
      get { return this.bankAccount; }
      set { this.bankAccount = value; }
    }
    public int PersonInCharge
    {
      get { return this.personInCharge; }
      set { this.personInCharge = value; }
    }
    public int IntroducePerson
    {
      get { return this.introducePerson; }
      set { this.introducePerson = value; }
    }
    public int Debit
    {
      get { return this.debit; }
      set { this.debit = value; }
    }
    public int Confirm
    {
      get { return this.confirm; }
      set { this.confirm = value; }
    }
    public int DeleteFlg
    {
      get { return this.deleteFlg; }
      set { this.deleteFlg = value; }
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