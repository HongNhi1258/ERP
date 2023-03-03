/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 05/07/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class BOMSupportInfo : IObject
  {
    #region Fields
    private string supCode;
    private string description;
    private string descriptionVN;
    private int confirm;
    private int deleteFlag;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private string remark;
    #endregion Fields

    #region Constructors
    public BOMSupportInfo()
    {
      this.supCode = string.Empty;
      this.description = string.Empty;
      this.descriptionVN = string.Empty;
      this.confirm = int.MinValue;
      this.deleteFlag = int.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.remark = string.Empty;
    }
    public object Clone()
    {
      BOMSupportInfo obj = new BOMSupportInfo();
      obj.SupCode = this.supCode;
      obj.Description = this.description;
      obj.DescriptionVN = this.descriptionVN;
      obj.Confirm = this.confirm;
      obj.DeleteFlag = this.deleteFlag;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.Remark = this.remark;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMSupportInfo";
    }
    public string[] ObjectKey()
    {
      return new string[] { "SupCode" };
    }
    #endregion Constructors

    #region Properties

    public string SupCode
    {
      get { return this.supCode; }
      set { this.supCode = value; }
    }
    public string Description
    {
      get { return this.description; }
      set { this.description = value; }
    }
    public string DescriptionVN
    {
      get { return this.descriptionVN; }
      set { this.descriptionVN = value; }
    }
    public int Confirm
    {
      get { return this.confirm; }
      set { this.confirm = value; }
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
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
    }
    #endregion Properties
  }
}