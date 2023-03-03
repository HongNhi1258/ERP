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
  public class BOMFinishingDetail : IObject
  {
    #region Fields
    private long pid;
    private string finCode;
    private string materialCode;
    private double qty;
    private string remark;
    private int inCompany;
    private int contractOut;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public BOMFinishingDetail()
    {
      this.pid = long.MinValue;
      this.finCode = string.Empty;
      this.materialCode = string.Empty;
      this.qty = double.MinValue;
      this.remark = string.Empty;
      this.inCompany = int.MinValue;
      this.contractOut = int.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      BOMFinishingDetail obj = new BOMFinishingDetail();
      obj.Pid = this.pid;
      obj.FinCode = this.finCode;
      obj.MaterialCode = this.materialCode;
      obj.Qty = this.qty;
      obj.Remark = this.remark;
      obj.InCompany = this.inCompany;
      obj.ContractOut = this.contractOut;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMFinishingDetail";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Pid", "FinCode", "MaterialCode" };
    }
    #endregion Constructors

    #region Properties

    public long Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public string FinCode
    {
      get { return this.finCode; }
      set { this.finCode = value; }
    }
    public string MaterialCode
    {
      get { return this.materialCode; }
      set { this.materialCode = value; }
    }
    public double Qty
    {
      get { return this.qty; }
      set { this.qty = value; }
    }
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
    }
    public int InCompany
    {
      get { return this.inCompany; }
      set { this.inCompany = value; }
    }
    public int ContractOut
    {
      get { return this.contractOut; }
      set { this.contractOut = value; }
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