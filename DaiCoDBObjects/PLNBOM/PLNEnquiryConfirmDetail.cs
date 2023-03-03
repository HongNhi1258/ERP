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
  public class PLNEnquiryConfirmDetail : IObject
  {
    #region Fields
    private long pid;
    private long enquiryDetailPid;
    private double qty;
    private DateTime scheduleDate;
    private int expire;
    private int keep;
    private int nonPlan;
    private int loadedFlg;
    private string remark;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public PLNEnquiryConfirmDetail()
    {
      this.pid = long.MinValue;
      this.enquiryDetailPid = long.MinValue;
      this.qty = double.MinValue;
      this.scheduleDate = DateTime.MinValue;
      this.expire = int.MinValue;
      this.keep = int.MinValue;
      this.nonPlan = int.MinValue;
      this.loadedFlg = int.MinValue;
      this.remark = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      PLNEnquiryConfirmDetail obj = new PLNEnquiryConfirmDetail();
      obj.Pid = this.pid;
      obj.EnquiryDetailPid = this.enquiryDetailPid;
      obj.Qty = this.qty;
      obj.ScheduleDate = this.scheduleDate;
      obj.Expire = this.expire;
      obj.Keep = this.keep;
      obj.NonPlan = this.nonPlan;
      obj.LoadedFlg = this.loadedFlg;
      obj.Remark = this.remark;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPlnEnquiryConfirmDetail";
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
    public long EnquiryDetailPid
    {
      get { return this.enquiryDetailPid; }
      set { this.enquiryDetailPid = value; }
    }
    public double Qty
    {
      get { return this.qty; }
      set { this.qty = value; }
    }
    public DateTime ScheduleDate
    {
      get { return this.scheduleDate; }
      set { this.scheduleDate = value; }
    }
    public int Expire
    {
      get { return this.expire; }
      set { this.expire = value; }
    }
    public int Keep
    {
      get { return this.keep; }
      set { this.keep = value; }
    }
    public int NonPlan
    {
      get { return this.nonPlan; }
      set { this.nonPlan = value; }
    }
    public int LoadedFlg
    {
      get { return this.loadedFlg; }
      set { this.loadedFlg = value; }
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