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
  public class WIPRepairListDetails : IObject
  {
    #region Fields
    private long pid;
    private long repairListPid;
    private long workPid;
    private string itemCode;
    private int revision;
    private int qty;
    private DateTime returnDate;
    #endregion Fields

    #region Constructors
    public WIPRepairListDetails()
    {
      this.pid = long.MinValue;
      this.repairListPid = long.MinValue;
      this.workPid = long.MinValue;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.qty = int.MinValue;
      this.returnDate = DateTime.MinValue;
    }
    public object Clone()
    {
      WIPRepairListDetails obj = new WIPRepairListDetails();
      obj.Pid = this.pid;
      obj.RepairListPid = this.repairListPid;
      obj.WorkPid = this.workPid;
      obj.ItemCode = this.itemCode;
      obj.Revision = this.revision;
      obj.Qty = this.qty;
      obj.ReturnDate = this.returnDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblWIPRepairListDetails";
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
    public long RepairListPid
    {
      get { return this.repairListPid; }
      set { this.repairListPid = value; }
    }
    public long WorkPid
    {
      get { return this.workPid; }
      set { this.workPid = value; }
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
    public int Qty
    {
      get { return this.qty; }
      set { this.qty = value; }
    }
    public DateTime ReturnDate
    {
      get { return this.returnDate; }
      set { this.returnDate = value; }
    }
    #endregion Properties
  }
}