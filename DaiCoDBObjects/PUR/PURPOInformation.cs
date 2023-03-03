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
  public class PURPOInformation : IObject
  {
    #region Fields
    private string pONo;
    private DateTime finishedDate;
    private int status;
    private long groupInCharge;
    private int approvedBy;
    private DateTime approveDate;
    private long supplierPid;
    private string contactPerson;
    private double totalMoney;
    private double depositMoney;
    private string remark;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private string updateDate;
    #endregion Fields

    #region Constructors
    public PURPOInformation()
    {
      this.pONo = string.Empty;
      this.finishedDate = DateTime.MinValue;
      this.status = int.MinValue;
      this.groupInCharge = long.MinValue;
      this.approvedBy = int.MinValue;
      this.approveDate = DateTime.MinValue;
      this.supplierPid = long.MinValue;
      this.contactPerson = string.Empty;
      this.totalMoney = double.MinValue;
      this.depositMoney = double.MinValue;
      this.remark = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = string.Empty;
    }
    public object Clone()
    {
      PURPOInformation obj = new PURPOInformation();
      obj.PONo = this.pONo;
      obj.FinishedDate = this.finishedDate;
      obj.Status = this.status;
      obj.GroupInCharge = this.groupInCharge;
      obj.ApprovedBy = this.approvedBy;
      obj.ApproveDate = this.approveDate;
      obj.SupplierPid = this.supplierPid;
      obj.ContactPerson = this.contactPerson;
      obj.TotalMoney = this.totalMoney;
      obj.DepositMoney = this.depositMoney;
      obj.Remark = this.remark;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPURPOInformation";
    }
    public string[] ObjectKey()
    {
      return new string[] { "PONo" };
    }
    #endregion Constructors

    #region Properties

    public string PONo
    {
      get { return this.pONo; }
      set { this.pONo = value; }
    }
    public DateTime FinishedDate
    {
      get { return this.finishedDate; }
      set { this.finishedDate = value; }
    }
    public int Status
    {
      get { return this.status; }
      set { this.status = value; }
    }
    public long GroupInCharge
    {
      get { return this.groupInCharge; }
      set { this.groupInCharge = value; }
    }
    public int ApprovedBy
    {
      get { return this.approvedBy; }
      set { this.approvedBy = value; }
    }
    public DateTime ApproveDate
    {
      get { return this.approveDate; }
      set { this.approveDate = value; }
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
    public double TotalMoney
    {
      get { return this.totalMoney; }
      set { this.totalMoney = value; }
    }
    public double DepositMoney
    {
      get { return this.depositMoney; }
      set { this.depositMoney = value; }
    }
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
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
    public string UpdateDate
    {
      get { return this.updateDate; }
      set { this.updateDate = value; }
    }
    #endregion Properties
  }
}