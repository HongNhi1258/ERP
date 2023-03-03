/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 25/09/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class QCDTestingAtWorkArea : IObject
  {
    #region Fields
    private long pid;
    private long workOrderPid;
    private string itemCode;
    private long workAreaPid;
    private long qCEmployeePid;
    private DateTime testingDate;
    private string teamCode;
    private int teamLeader;
    private string groupFur;

    #endregion Fields

    #region Constructors
    public QCDTestingAtWorkArea()
    {
      this.pid = long.MinValue;
      this.workOrderPid = long.MinValue;
      this.itemCode = string.Empty;
      this.workAreaPid = long.MinValue;
      this.qCEmployeePid = long.MinValue;
      this.testingDate = DateTime.MinValue;
      this.teamCode = string.Empty;
      this.teamLeader = int.MinValue;
      this.groupFur = string.Empty;
    }
    public object Clone()
    {
      QCDTestingAtWorkArea obj = new QCDTestingAtWorkArea();
      obj.Pid = this.pid;
      obj.WorkOrderPid = this.workOrderPid;
      obj.ItemCode = this.itemCode;
      obj.WorkAreaPid = this.workAreaPid;
      obj.QCEmployeePid = this.qCEmployeePid;
      obj.TestingDate = this.testingDate;
      obj.TeamCode = this.teamCode;
      obj.TeamLeader = this.teamLeader;
      obj.GroupFur = this.groupFur;
      return obj;
    }
    public string GetTableName()
    {
      return "TblQCDTestingAtWorkArea";
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
    public long WorkOrderPid
    {
      get { return this.workOrderPid; }
      set { this.workOrderPid = value; }
    }
    public string ItemCode
    {
      get { return this.itemCode; }
      set { this.itemCode = value; }
    }
    public long WorkAreaPid
    {
      get { return this.workAreaPid; }
      set { this.workAreaPid = value; }
    }
    public long QCEmployeePid
    {
      get { return this.qCEmployeePid; }
      set { this.qCEmployeePid = value; }
    }
    public DateTime TestingDate
    {
      get { return this.testingDate; }
      set { this.testingDate = value; }
    }
    public string TeamCode
    {
      get { return this.teamCode; }
      set { this.teamCode = value; }
    }
    public int TeamLeader
    {
      get { return this.teamLeader; }
      set { this.teamLeader = value; }
    }
    public string GroupFur
    {
      get { return this.groupFur; }
      set { this.groupFur = value; }
    }
    #endregion Properties
  }
}