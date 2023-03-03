/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 06/09/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class PLNSubconItemPR : IObject
  {
    #region Fields
    private long pid;
    private long workOrderPid;
    private DateTime requestDate;
    private int purchasingConfirm;
    private long createrPid;
    private DateTime createDate;
    #endregion Fields

    #region Constructors
    public PLNSubconItemPR()
    {
      this.pid = long.MinValue;
      this.workOrderPid = long.MinValue;
      this.requestDate = DateTime.MinValue;
      this.purchasingConfirm = int.MinValue;
      this.createrPid = long.MinValue;
      this.createDate = DateTime.MinValue;
    }
    public object Clone()
    {
      PLNSubconItemPR obj = new PLNSubconItemPR();
      obj.Pid = this.pid;
      obj.WorkOrderPid = this.workOrderPid;
      obj.RequestDate = this.requestDate;
      obj.PurchasingConfirm = this.purchasingConfirm;
      obj.CreaterPid = this.createrPid;
      obj.CreateDate = this.createDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPLNSubconItemPR";
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
    public DateTime RequestDate
    {
      get { return this.requestDate; }
      set { this.requestDate = value; }
    }
    public int PurchasingConfirm
    {
      get { return this.purchasingConfirm; }
      set { this.purchasingConfirm = value; }
    }
    public long CreaterPid
    {
      get { return this.createrPid; }
      set { this.createrPid = value; }
    }
    public DateTime CreateDate
    {
      get { return this.createDate; }
      set { this.createDate = value; }
    }
    #endregion Properties
  }
}