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
  public class PLNWOInfoDetail : IObject
  {
    #region Fields
    private long pid;
    private long woInfoPid;
    private string itemCode;
    private int revision;
    private int qty;
    private long saleOrderDetailPid;
    private int priority;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private string note;
    #endregion Fields

    #region Constructors
    public PLNWOInfoDetail()
    {
      this.pid = long.MinValue;
      this.woInfoPid = long.MinValue;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.qty = int.MinValue;
      this.saleOrderDetailPid = long.MinValue;
      this.priority = int.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.note = string.Empty;
    }
    public object Clone()
    {
      PLNWOInfoDetail obj = new PLNWOInfoDetail();
      obj.Pid = this.pid;
      obj.WoInfoPid = this.woInfoPid;
      obj.ItemCode = this.itemCode;
      obj.Revision = this.revision;
      obj.Qty = this.qty;
      obj.SaleOrderDetailPid = this.saleOrderDetailPid;
      obj.Priority = this.priority;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.Note = this.note;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPLNWOInfoDetail";
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
    public long WoInfoPid
    {
      get { return this.woInfoPid; }
      set { this.woInfoPid = value; }
    }
    public string ItemCode
    {
      get { return this.itemCode; }
      set { this.itemCode = value; }
    }
    public string Note
    {
      get { return this.note; }
      set { this.note = value; }
    }
    public int Revision
    {
      get { return this.revision; }
      set { this.revision = value; }
    }
    public int Qty
    {
      get { return this.qty; }
      set { this.qty = value; }
    }
    public long SaleOrderDetailPid
    {
      get { return this.saleOrderDetailPid; }
      set { this.saleOrderDetailPid = value; }
    }
    public int Priority
    {
      get { return this.priority; }
      set { this.priority = value; }
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