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
  public class PURPRInformation : IObject
  {
    #region Fields
    private string pRNo;
    private int requestBy;
    private string department;
    private int status;
    private int headDepartmentApproved;
    private int purchaserApproved;
    private int purchaseManagerApproved;
    private int directorApproved1;
    private int directorApproved2;
    private double scheduleTotalMoney;
    private string remark;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public PURPRInformation()
    {
      this.pRNo = string.Empty;
      this.requestBy = int.MinValue;
      this.department = string.Empty;
      this.status = int.MinValue;
      this.headDepartmentApproved = int.MinValue;
      this.purchaserApproved = int.MinValue;
      this.purchaseManagerApproved = int.MinValue;
      this.directorApproved1 = int.MinValue;
      this.directorApproved2 = int.MinValue;
      this.scheduleTotalMoney = double.MinValue;
      this.remark = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      PURPRInformation obj = new PURPRInformation();
      obj.PRNo = this.pRNo;
      obj.RequestBy = this.requestBy;
      obj.Department = this.department;
      obj.Status = this.status;
      obj.HeadDepartmentApproved = this.headDepartmentApproved;
      obj.PurchaserApproved = this.purchaserApproved;
      obj.PurchaseManagerApproved = this.purchaseManagerApproved;
      obj.DirectorApproved1 = this.directorApproved1;
      obj.DirectorApproved2 = this.directorApproved2;
      obj.ScheduleTotalMoney = this.scheduleTotalMoney;
      obj.Remark = this.remark;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPURPRInformation";
    }
    public string[] ObjectKey()
    {
      return new string[] { "PRNo" };
    }
    #endregion Constructors

    #region Properties

    public string PRNo
    {
      get { return this.pRNo; }
      set { this.pRNo = value; }
    }
    public int RequestBy
    {
      get { return this.requestBy; }
      set { this.requestBy = value; }
    }
    public string Department
    {
      get { return this.department; }
      set { this.department = value; }
    }
    public int Status
    {
      get { return this.status; }
      set { this.status = value; }
    }
    public int HeadDepartmentApproved
    {
      get { return this.headDepartmentApproved; }
      set { this.headDepartmentApproved = value; }
    }
    public int PurchaserApproved
    {
      get { return this.purchaserApproved; }
      set { this.purchaserApproved = value; }
    }
    public int PurchaseManagerApproved
    {
      get { return this.purchaseManagerApproved; }
      set { this.purchaseManagerApproved = value; }
    }
    public int DirectorApproved1
    {
      get { return this.directorApproved1; }
      set { this.directorApproved1 = value; }
    }
    public int DirectorApproved2
    {
      get { return this.directorApproved2; }
      set { this.directorApproved2 = value; }
    }
    public double ScheduleTotalMoney
    {
      get { return this.scheduleTotalMoney; }
      set { this.scheduleTotalMoney = value; }
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
    public DateTime UpdateDate
    {
      get { return this.updateDate; }
      set { this.updateDate = value; }
    }
    #endregion Properties
  }
}