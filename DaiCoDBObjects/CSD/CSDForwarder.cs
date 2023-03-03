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
  public class CSDForwarder : IObject
  {
    #region Fields
    private long pid;
    private string forwarderCode;
    private string name;
    private string tel;
    private string fax;
    private string email;
    private string contactPerson;
    private long country;
    private string postalCode;
    private string region;
    private string city;
    private string streetAdress;
    private string pOBox;
    private int deleteFlag;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public CSDForwarder()
    {
      this.pid = long.MinValue;
      this.forwarderCode = string.Empty;
      this.name = string.Empty;
      this.tel = string.Empty;
      this.fax = string.Empty;
      this.email = string.Empty;
      this.contactPerson = string.Empty;
      this.country = long.MinValue;
      this.postalCode = string.Empty;
      this.region = string.Empty;
      this.city = string.Empty;
      this.streetAdress = string.Empty;
      this.pOBox = string.Empty;
      this.deleteFlag = int.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      CSDForwarder obj = new CSDForwarder();
      obj.Pid = this.pid;
      obj.ForwarderCode = this.forwarderCode;
      obj.Name = this.name;
      obj.Tel = this.tel;
      obj.Fax = this.fax;
      obj.Email = this.email;
      obj.ContactPerson = this.contactPerson;
      obj.Country = this.country;
      obj.PostalCode = this.postalCode;
      obj.Region = this.region;
      obj.City = this.city;
      obj.StreetAdress = this.streetAdress;
      obj.POBox = this.pOBox;
      obj.DeleteFlag = this.deleteFlag;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblCSDForwarder";
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
    public string ForwarderCode
    {
      get { return this.forwarderCode; }
      set { this.forwarderCode = value; }
    }
    public string Name
    {
      get { return this.name; }
      set { this.name = value; }
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
    public int DeleteFlag
    {
      get { return this.deleteFlag; }
      set { this.deleteFlag = value; }
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