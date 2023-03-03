/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 13/08/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class PLNSubconSupplier : IObject
  {
    #region Fields
    private long pid;
    private string subconSupplierName;
    private string address;
    private string officePhone;
    private string cellPhone;
    private string email;
    private string remark;
    private int isDeleted;
    private int cMBAbilityJan;
    private int cMBAbilityFer;
    private int cMBAbilityMar;
    private int cMBAbilityApr;
    private int cMBAbilityMay;
    private int cMBAbilityJun;
    private int cMBAbilityJul;
    private int cMBAbilityAug;
    private int cMBAbilitySep;
    private int cMBAbilityNov;
    private int cMBAbilityDec;
    private int cMBAbilityOct;
    #endregion Fields

    #region Constructors
    public PLNSubconSupplier()
    {
      this.pid = long.MinValue;
      this.subconSupplierName = string.Empty;
      this.address = string.Empty;
      this.officePhone = string.Empty;
      this.cellPhone = string.Empty;
      this.email = string.Empty;
      this.remark = string.Empty;
      this.isDeleted = int.MinValue;
      this.cMBAbilityJan = int.MinValue;
      this.cMBAbilityFer = int.MinValue;
      this.cMBAbilityMar = int.MinValue;
      this.cMBAbilityApr = int.MinValue;
      this.cMBAbilityMay = int.MinValue;
      this.cMBAbilityJun = int.MinValue;
      this.cMBAbilityJul = int.MinValue;
      this.cMBAbilityAug = int.MinValue;
      this.cMBAbilitySep = int.MinValue;
      this.cMBAbilityNov = int.MinValue;
      this.cMBAbilityDec = int.MinValue;
      this.cMBAbilityOct = int.MinValue;
    }
    public object Clone()
    {
      PLNSubconSupplier obj = new PLNSubconSupplier();
      obj.Pid = this.pid;
      obj.SubconSupplierName = this.subconSupplierName;
      obj.Address = this.address;
      obj.OfficePhone = this.officePhone;
      obj.CellPhone = this.cellPhone;
      obj.Email = this.email;
      obj.Remark = this.remark;
      obj.IsDeleted = this.isDeleted;
      obj.CMBAbilityJan = this.cMBAbilityJan;
      obj.CMBAbilityFer = this.cMBAbilityFer;
      obj.CMBAbilityMar = this.cMBAbilityMar;
      obj.CMBAbilityApr = this.cMBAbilityApr;
      obj.CMBAbilityMay = this.cMBAbilityMay;
      obj.CMBAbilityJun = this.cMBAbilityJun;
      obj.CMBAbilityJul = this.cMBAbilityJul;
      obj.CMBAbilityAug = this.cMBAbilityAug;
      obj.CMBAbilitySep = this.cMBAbilitySep;
      obj.CMBAbilityNov = this.cMBAbilityNov;
      obj.CMBAbilityDec = this.cMBAbilityDec;
      obj.CMBAbilityOct = this.cMBAbilityOct;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPLNSubconSupplier";
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
    public string SubconSupplierName
    {
      get { return this.subconSupplierName; }
      set { this.subconSupplierName = value; }
    }
    public string Address
    {
      get { return this.address; }
      set { this.address = value; }
    }
    public string OfficePhone
    {
      get { return this.officePhone; }
      set { this.officePhone = value; }
    }
    public string CellPhone
    {
      get { return this.cellPhone; }
      set { this.cellPhone = value; }
    }
    public string Email
    {
      get { return this.email; }
      set { this.email = value; }
    }
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
    }
    public int IsDeleted
    {
      get { return this.isDeleted; }
      set { this.isDeleted = value; }
    }
    public int CMBAbilityJan
    {
      get { return this.cMBAbilityJan; }
      set { this.cMBAbilityJan = value; }
    }
    public int CMBAbilityFer
    {
      get { return this.cMBAbilityFer; }
      set { this.cMBAbilityFer = value; }
    }
    public int CMBAbilityMar
    {
      get { return this.cMBAbilityMar; }
      set { this.cMBAbilityMar = value; }
    }
    public int CMBAbilityApr
    {
      get { return this.cMBAbilityApr; }
      set { this.cMBAbilityApr = value; }
    }
    public int CMBAbilityMay
    {
      get { return this.cMBAbilityMay; }
      set { this.cMBAbilityMay = value; }
    }
    public int CMBAbilityJun
    {
      get { return this.cMBAbilityJun; }
      set { this.cMBAbilityJun = value; }
    }
    public int CMBAbilityJul
    {
      get { return this.cMBAbilityJul; }
      set { this.cMBAbilityJul = value; }
    }
    public int CMBAbilityAug
    {
      get { return this.cMBAbilityAug; }
      set { this.cMBAbilityAug = value; }
    }
    public int CMBAbilitySep
    {
      get { return this.cMBAbilitySep; }
      set { this.cMBAbilitySep = value; }
    }
    public int CMBAbilityNov
    {
      get { return this.cMBAbilityNov; }
      set { this.cMBAbilityNov = value; }
    }
    public int CMBAbilityDec
    {
      get { return this.cMBAbilityDec; }
      set { this.cMBAbilityDec = value; }
    }
    public int CMBAbilityOct
    {
      get { return this.cMBAbilityOct; }
      set { this.cMBAbilityOct = value; }
    }
    #endregion Properties
  }
}