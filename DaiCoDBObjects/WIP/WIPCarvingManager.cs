/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 09/08/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class WIPCarvingManager : IObject
  {
    #region Fields
    private long pid;
    private long workPid;
    private string carcassCode;
    private long componentPid;
    private long employeePid;
    private DateTime startDate;
    private DateTime endDate;
    private int qty;
    private double overTime;
    private int isFinished;
    private string remark;
    private long createPid;
    private DateTime createDate;
    #endregion Fields

    #region Constructors
    public WIPCarvingManager()
    {
      this.pid = long.MinValue;
      this.workPid = long.MinValue;
      this.carcassCode = string.Empty;
      this.componentPid = long.MinValue;
      this.employeePid = long.MinValue;
      this.startDate = DateTime.MinValue;
      this.endDate = DateTime.MinValue;
      this.qty = int.MinValue;
      this.overTime = double.MinValue;
      this.isFinished = int.MinValue;
      this.remark = string.Empty;
      this.createPid = long.MinValue;
      this.createDate = DateTime.MinValue;
    }
    public object Clone()
    {
      WIPCarvingManager obj = new WIPCarvingManager();
      obj.Pid = this.pid;
      obj.WorkPid = this.workPid;
      obj.CarcassCode = this.carcassCode;
      obj.ComponentPid = this.componentPid;
      obj.EmployeePid = this.employeePid;
      obj.StartDate = this.startDate;
      obj.EndDate = this.endDate;
      obj.Qty = this.qty;
      obj.OverTime = this.overTime;
      obj.IsFinished = this.isFinished;
      obj.Remark = this.remark;
      obj.CreatePid = this.createPid;
      obj.CreateDate = this.createDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblWIPCarvingManager";
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
    public long WorkPid
    {
      get { return this.workPid; }
      set { this.workPid = value; }
    }
    public string CarcassCode
    {
      get { return this.carcassCode; }
      set { this.carcassCode = value; }
    }
    public long ComponentPid
    {
      get { return this.componentPid; }
      set { this.componentPid = value; }
    }
    public long EmployeePid
    {
      get { return this.employeePid; }
      set { this.employeePid = value; }
    }
    public DateTime StartDate
    {
      get { return this.startDate; }
      set { this.startDate = value; }
    }
    public DateTime EndDate
    {
      get { return this.endDate; }
      set { this.endDate = value; }
    }
    public int Qty
    {
      get { return this.qty; }
      set { this.qty = value; }
    }
    public Double OverTime
    {
      get { return this.overTime; }
      set { this.overTime = value; }
    }
    public int IsFinished
    {
      get { return this.isFinished; }
      set { this.isFinished = value; }
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
    #endregion Properties
  }
}