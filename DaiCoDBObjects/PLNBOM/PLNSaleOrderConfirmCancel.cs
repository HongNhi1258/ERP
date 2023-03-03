/*
   Author  : Nguyen Van Tron
   Date    : 16/07/2010
   Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class PLNSaleOrderConfirmCancel : IObject
  {
    #region Fields
    private long pid;
    private long poCancelDetailPid;
    private long saleOrderDetailPid;
    private double qty;
    private string remark;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public PLNSaleOrderConfirmCancel()
    {
      this.pid = long.MinValue;
      this.poCancelDetailPid = long.MinValue;
      this.saleOrderDetailPid = long.MinValue;
      this.qty = double.MinValue;
      this.remark = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      PLNSaleOrderConfirmCancel obj = new PLNSaleOrderConfirmCancel();
      obj.Pid = this.pid;
      obj.PoCancelDetailPid = this.poCancelDetailPid;
      obj.SaleOrderDetailPid = this.saleOrderDetailPid;
      obj.Qty = this.qty;
      obj.Remark = this.remark;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPLNSaleOrderConfirmCancel";
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
    public long PoCancelDetailPid
    {
      get { return this.poCancelDetailPid; }
      set { this.poCancelDetailPid = value; }
    }
    public long SaleOrderDetailPid
    {
      get { return this.saleOrderDetailPid; }
      set { this.saleOrderDetailPid = value; }
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