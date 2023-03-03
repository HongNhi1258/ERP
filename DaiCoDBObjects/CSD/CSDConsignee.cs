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
  public class CSDConsignee : IObject
  {
    #region Fields
    private long pid;
    //private long customerPid;
    private string consigneeCode;
    private string name;
    private long country;
    private string postalCode;
    private string region;
    private string city;
    private string streetAdress;
    private string pOBox;
    private string tel;
    private string fax;
    private string email;
    private string contact;
    private int billOfLading;
    private int certificate;
    private int fumigation;
    private int packingList;
    private int invoice;
    private string doorDelivery;
    private string portOfDischarge;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public CSDConsignee()
    {
      this.pid = long.MinValue;
      //this.customerPid = long.MinValue;
      this.consigneeCode = string.Empty;
      this.name = string.Empty;
      this.country = long.MinValue;
      this.postalCode = string.Empty;
      this.region = string.Empty;
      this.city = string.Empty;
      this.streetAdress = string.Empty;
      this.pOBox = string.Empty;
      this.tel = string.Empty;
      this.fax = string.Empty;
      this.email = string.Empty;
      this.contact = string.Empty;
      this.billOfLading = int.MinValue;
      this.certificate = int.MinValue;
      this.fumigation = int.MinValue;
      this.packingList = int.MinValue;
      this.invoice = int.MinValue;
      this.doorDelivery = string.Empty;
      this.portOfDischarge = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      CSDConsignee obj = new CSDConsignee();
      obj.Pid = this.pid;
      //obj.CustomerPid = this.customerPid;
      obj.ConsigneeCode = this.consigneeCode;
      obj.Name = this.name;
      obj.Country = this.country;
      obj.PostalCode = this.postalCode;
      obj.Region = this.region;
      obj.City = this.city;
      obj.StreetAdress = this.streetAdress;
      obj.POBox = this.pOBox;
      obj.Tel = this.tel;
      obj.Fax = this.fax;
      obj.Email = this.email;
      obj.Contact = this.contact;
      obj.BillOfLading = this.billOfLading;
      obj.Certificate = this.certificate;
      obj.Fumigation = this.fumigation;
      obj.PackingList = this.packingList;
      obj.Invoice = this.invoice;
      obj.DoorDelivery = this.doorDelivery;
      obj.PortOfDischarge = this.portOfDischarge;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblCSDConsignee";
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
    //public long CustomerPid
    //{
    //  get { return this.customerPid; }
    //  set { this.customerPid = value; }
    //}
    public string ConsigneeCode
    {
      get { return this.consigneeCode; }
      set { this.consigneeCode = value; }
    }
    public string Name
    {
      get { return this.name; }
      set { this.name = value; }
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
    public string Region
    {
      get { return this.region; }
      set { this.region = value; }
    }
    public string City
    {
      get { return this.city; }
      set { this.city = value; }
    }
    public string StreetAdress
    {
      get { return this.streetAdress; }
      set { this.streetAdress = value; }
    }
    public string POBox
    {
      get { return this.pOBox; }
      set { this.pOBox = value; }
    }
    public string Tel
    {
      get { return this.tel; }
      set { this.tel = value; }
    }
    public string Fax
    {
      get { return this.fax; }
      set { this.fax = value; }
    }
    public string Email
    {
      get { return this.email; }
      set { this.email = value; }
    }
    public string Contact
    {
      get { return this.contact; }
      set { this.contact = value; }
    }
    public int BillOfLading
    {
      get { return this.billOfLading; }
      set { this.billOfLading = value; }
    }
    public int Certificate
    {
      get { return this.certificate; }
      set { this.certificate = value; }
    }
    public int Fumigation
    {
      get { return this.fumigation; }
      set { this.fumigation = value; }
    }
    public int PackingList
    {
      get { return this.packingList; }
      set { this.packingList = value; }
    }
    public int Invoice
    {
      get { return this.invoice; }
      set { this.invoice = value; }
    }
    public string DoorDelivery
    {
      get { return this.doorDelivery; }
      set { this.doorDelivery = value; }
    }
    public string PortOfDischarge
    {
      get { return this.portOfDischarge; }
      set { this.portOfDischarge = value; }
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