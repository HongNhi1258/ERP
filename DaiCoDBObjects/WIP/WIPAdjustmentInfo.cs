/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 09/09/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class WIPAdjustmentInfo : IObject
  {
    #region Fields
    private long pid;
    private int adjustmentType;
    private string devision;
    private long workAreaPid;
    private int reason;
    private string remark;
    private long createPid;
    private DateTime createDate;
    private int isConfirmed;
    #endregion Fields

    #region Constructors
    public WIPAdjustmentInfo()
    {
      this.pid = long.MinValue;
      this.adjustmentType = int.MinValue;
      this.devision = string.Empty;
      this.workAreaPid = long.MinValue;
      this.reason = int.MinValue;
      this.remark = string.Empty;
      this.createPid = long.MinValue;
      this.createDate = DateTime.MinValue;
      this.isConfirmed = int.MinValue;
    }
    public object Clone()
    {
      WIPAdjustmentInfo obj = new WIPAdjustmentInfo();
      obj.Pid = this.pid;
      obj.AdjustmentType = this.adjustmentType;
      obj.Devision = this.devision;
      obj.WorkAreaPid = this.workAreaPid;
      obj.Reason = this.reason;
      obj.Remark = this.remark;
      obj.CreatePid = this.createPid;
      obj.CreateDate = this.createDate;
      obj.IsConfirmed = this.isConfirmed;
      return obj;
    }
    public string GetTableName()
    {
      return "TblWIPAdjustmentInfo";
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
    public int AdjustmentType
    {
      get { return this.adjustmentType; }
      set { this.adjustmentType = value; }
    }
    public string Devision
    {
      get { return this.devision; }
      set { this.devision = value; }
    }
    public long WorkAreaPid
    {
      get { return this.workAreaPid; }
      set { this.workAreaPid = value; }
    }
    public int Reason
    {
      get { return this.reason; }
      set { this.reason = value; }
    }
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
    }
    public long CreatePid
    {
      get { return this.createPid; }
      set { this.createPid = value; }
    }
    public DateTime CreateDate
    {
      get { return this.createDate; }
      set { this.createDate = value; }
    }
    public int IsConfirmed
    {
      get { return this.isConfirmed; }
      set { this.isConfirmed = value; }
    }
    #endregion Properties
  }
}