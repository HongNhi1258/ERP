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
  public class PLNWorkOrderDeadline : IObject
  {
    #region Fields
    private long pid;
    private long woPid;
    private long workStation;
    private string itemCode;
    private int revision;
    private string carcassCode;
    private int qty;
    private DateTime deadline;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    //hung them DeadlineStatus
    private int deadlinestatus;
    private int flagIdentity;
    #endregion Fields

    #region Constructors
    public PLNWorkOrderDeadline()
    {
      this.pid = long.MinValue;
      this.woPid = long.MinValue;
      this.workStation = long.MinValue;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.carcassCode = string.Empty;
      this.qty = int.MinValue;
      this.deadline = DateTime.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.deadlinestatus = int.MinValue;
      this.flagIdentity = int.MinValue;
    }
    public object Clone()
    {
      PLNWorkOrderDeadline obj = new PLNWorkOrderDeadline();
      obj.Pid = this.pid;
      obj.WoPid = this.woPid;
      obj.WorkStation = this.workStation;
      obj.ItemCode = this.itemCode;
      obj.Revision = this.revision;
      obj.CarcassCode = this.carcassCode;
      obj.Qty = this.qty;
      obj.Deadline = this.deadline;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.DeadlineStatus = this.deadlinestatus;
      obj.FlagIdentity = this.flagIdentity;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPLNWorkOrderDeadline";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Pid", "WoPid", "WorkStation", "ItemCode", "Revision", "CarcassCode", "Deadline", "DeadlineStatus", "FlagIdentity" };
    }
    #endregion Constructors

    #region Properties

    public long Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public long WoPid
    {
      get { return this.woPid; }
      set { this.woPid = value; }
    }
    public long WorkStation
    {
      get { return this.workStation; }
      set { this.workStation = value; }
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
    public string CarcassCode
    {
      get { return this.carcassCode; }
      set { this.carcassCode = value; }
    }
    public int Qty
    {
      get { return this.qty; }
      set { this.qty = value; }
    }
    public DateTime Deadline
    {
      get { return this.deadline; }
      set { this.deadline = value; }
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
    public int DeadlineStatus
    {
      get { return this.deadlinestatus; }
      set { this.deadlinestatus = value; }
    }
    public int FlagIdentity
    {
      get { return this.flagIdentity; }
      set { this.flagIdentity = value; }
    }
    #endregion Properties
  }
}