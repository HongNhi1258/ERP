/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 10/08/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class WIPRepairItemList : IObject
  {
    #region Fields
    private long pid;
    private string refNo;
    private long workAreaRepairPid;
    private long workAreaForPid;
    private long issuerPid;
    private long receiverPid;
    private DateTime receiveDate;
    private DateTime createDate;
    #endregion Fields

    #region Constructors
    public WIPRepairItemList()
    {
      this.pid = long.MinValue;
      this.refNo = string.Empty;
      this.workAreaRepairPid = long.MinValue;
      this.workAreaForPid = long.MinValue;
      this.issuerPid = long.MinValue;
      this.receiverPid = long.MinValue;
      this.receiveDate = DateTime.MinValue;
      this.createDate = DateTime.MinValue;
    }
    public object Clone()
    {
      WIPRepairItemList obj = new WIPRepairItemList();
      obj.Pid = this.pid;
      obj.RefNo = this.refNo;
      obj.WorkAreaRepairPid = this.workAreaRepairPid;
      obj.WorkAreaForPid = this.workAreaForPid;
      obj.IssuerPid = this.issuerPid;
      obj.ReceiverPid = this.receiverPid;
      obj.ReceiveDate = this.receiveDate;
      obj.CreateDate = this.createDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblWIPRepairItemList";
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
    public string RefNo
    {
      get { return this.refNo; }
      set { this.refNo = value; }
    }
    public long WorkAreaRepairPid
    {
      get { return this.workAreaRepairPid; }
      set { this.workAreaRepairPid = value; }
    }
    public long WorkAreaForPid
    {
      get { return this.workAreaForPid; }
      set { this.workAreaForPid = value; }
    }
    public long IssuerPid
    {
      get { return this.issuerPid; }
      set { this.issuerPid = value; }
    }
    public long ReceiverPid
    {
      get { return this.receiverPid; }
      set { this.receiverPid = value; }
    }
    public DateTime ReceiveDate
    {
      get { return this.receiveDate; }
      set { this.receiveDate = value; }
    }
    public DateTime CreateDate
    {
      get { return this.createDate; }
      set { this.createDate = value; }
    }
    #endregion Properties
  }
}