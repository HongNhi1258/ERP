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
  public class PURPODetailScheduleDate : IObject
  {
    #region Fields
    private long pid;
    private long pODetatlPid;
    private double quantity;
    private double receiptedQty;
    private DateTime expectDate;
    private DateTime confirmExpectDate;
    private DateTime latestDeliveryDate;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime upateDate;
    #endregion Fields

    #region Constructors
    public PURPODetailScheduleDate()
    {
      this.pid = long.MinValue;
      this.pODetatlPid = long.MinValue;
      this.quantity = double.MinValue;
      this.receiptedQty = double.MinValue;
      this.expectDate = DateTime.MinValue;
      this.confirmExpectDate = DateTime.MinValue;
      this.latestDeliveryDate = DateTime.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.upateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      PURPODetailScheduleDate obj = new PURPODetailScheduleDate();
      obj.Pid = this.pid;
      obj.PODetatlPid = this.pODetatlPid;
      obj.Quantity = this.quantity;
      obj.ReceiptedQty = this.receiptedQty;
      obj.ExpectDate = this.expectDate;
      obj.ConfirmExpectDate = this.confirmExpectDate;
      obj.LatestDeliveryDate = this.latestDeliveryDate;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpateDate = this.upateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPURPODetailScheduleDate";
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
    public long PODetatlPid
    {
      get { return this.pODetatlPid; }
      set { this.pODetatlPid = value; }
    }
    public double Quantity
    {
      get { return this.quantity; }
      set { this.quantity = value; }
    }
    public double ReceiptedQty
    {
      get { return this.receiptedQty; }
      set { this.receiptedQty = value; }
    }
    public DateTime ExpectDate
    {
      get { return this.expectDate; }
      set { this.expectDate = value; }
    }
    public DateTime ConfirmExpectDate
    {
      get { return this.confirmExpectDate; }
      set { this.confirmExpectDate = value; }
    }
    public DateTime LatestDeliveryDate
    {
      get { return this.latestDeliveryDate; }
      set { this.latestDeliveryDate = value; }
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
    public DateTime UpateDate
    {
      get { return this.upateDate; }
      set { this.upateDate = value; }
    }
    #endregion Properties
  }
}