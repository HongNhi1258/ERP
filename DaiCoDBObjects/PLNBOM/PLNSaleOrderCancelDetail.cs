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
  public class PLNSaleOrderCancelDetail : IObject
  {
    #region Fields
    private long pid;
    private long poCancelPid;
    private long saleOrderPid;
    private string itemCode;
    private int revision;
    private double qty;
    private string remark;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public PLNSaleOrderCancelDetail()
    {
      this.pid = long.MinValue;
      this.poCancelPid = long.MinValue;
      this.saleOrderPid = long.MinValue;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.qty = double.MinValue;
      this.remark = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      PLNSaleOrderCancelDetail obj = new PLNSaleOrderCancelDetail();
      obj.Pid = this.pid;
      obj.PoCancelPid = this.poCancelPid;
      obj.SaleOrderPid = this.saleOrderPid;
      obj.ItemCode = this.itemCode;
      obj.Revision = this.revision;
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
      return "TblPLNSaleOrderCancelDetail";
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
    public long PoCancelPid
    {
      get { return this.poCancelPid; }
      set { this.poCancelPid = value; }
    }
    public long SaleOrderPid
    {
      get { return this.saleOrderPid; }
      set { this.saleOrderPid = value; }
    }
    public string ItemCode
    {
      get { return this.itemCode; }
      set { this.itemCode = value; }
    }
    public int Revision
    {
      get { return this.revision; }
      set { this.revision = value; }
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