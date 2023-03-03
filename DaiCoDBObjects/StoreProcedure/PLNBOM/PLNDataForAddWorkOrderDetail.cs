/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 23/06/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;
using System.Data;

namespace DaiCo.Objects
{
  public class PLNDataForAddWorkOrderDetail : IStoreObject
  {
    #region Fields
    private long currentWorkOrder;
    private DateTime scheduleDeliveryFrom;
    private DateTime scheduleDeliveryTo;
    private string poNoFrom;
    private string poNoTo;
    private string itemCode;
    private int revision;
    private string cusPoNo;
    private string oldCode;
    private long customerPid;
    private string condition;

    #endregion Fields

    #region Constructors
    public PLNDataForAddWorkOrderDetail()
    {
      this.currentWorkOrder = long.MinValue;
      this.scheduleDeliveryFrom = DateTime.MinValue;
      this.scheduleDeliveryTo = DateTime.MinValue;
      this.poNoFrom = string.Empty;
      this.poNoTo = string.Empty;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.cusPoNo = string.Empty;
      this.oldCode = string.Empty;
      this.customerPid = long.MinValue;
      this.condition = string.Empty;
    }
    public void GetOutputValue(IDbCommand cm)
    {
    }
    public void PrepareParameters(IDbCommand cm)
    {
      if (this.ScheduleDeliveryFrom != DateTime.MinValue)
      {
        DataParameter.AddParameter(cm, "@ScheduleDeliveryFrom", DbType.DateTime, this.ScheduleDeliveryFrom);
      }
      if (this.ScheduleDeliveryTo != DateTime.MinValue)
      {
        DataParameter.AddParameter(cm, "@ScheduleDeliveryTo", DbType.DateTime, this.ScheduleDeliveryTo);
      }
      if (this.PoNoFrom != string.Empty)
      {
        DataParameter.AddParameter(cm, "@PoNoFrom", DbType.AnsiString, 16, this.PoNoFrom);
      }
      if (this.PoNoTo != string.Empty)
      {
        DataParameter.AddParameter(cm, "@PoNoTo", DbType.AnsiString, 16, this.PoNoTo);
      }
      if (this.ItemCode != string.Empty)
      {
        DataParameter.AddParameter(cm, "@ItemCode", DbType.AnsiString, 18, this.ItemCode);
      }
      if (this.Revision != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Revision", DbType.Int32, this.Revision);
      }
      if (this.OldCode != string.Empty)
      {
        DataParameter.AddParameter(cm, "@OldCode", DbType.AnsiString, 18, this.OldCode);
      }
      if (this.CusPoNo != string.Empty)
      {
        DataParameter.AddParameter(cm, "@CusPONo", DbType.AnsiString, 16, this.CusPoNo);
      }
      if (this.CustomerPid != long.MinValue)
      {
        DataParameter.AddParameter(cm, "@CustomerPid", DbType.Int64, this.CustomerPid);
      }
      if (this.Condition != string.Empty)
      {
        DataParameter.AddParameter(cm, "@Condition", DbType.String, 4000, this.Condition);
      }
      if (this.currentWorkOrder != long.MinValue)
      {
        DataParameter.AddParameter(cm, "@CurrentWorkOrder", DbType.Int64, this.currentWorkOrder);
      }
    }
    public string GetStoreName()
    {
      return "spPLNDataForAddWorkOrderDetail";
    }
    #endregion Constructors

    #region Properties

    public DateTime ScheduleDeliveryFrom
    {
      get { return this.scheduleDeliveryFrom; }
      set { this.scheduleDeliveryFrom = value; }
    }
    public DateTime ScheduleDeliveryTo
    {
      get { return this.scheduleDeliveryTo; }
      set { this.scheduleDeliveryTo = value; }
    }
    public string PoNoFrom
    {
      get { return this.poNoFrom; }
      set { this.poNoFrom = value; }
    }
    public string PoNoTo
    {
      get { return this.poNoTo; }
      set { this.poNoTo = value; }
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
    public string OldCode
    {
      get { return this.oldCode; }
      set { this.oldCode = value; }
    }
    public string CusPoNo
    {
      get { return this.cusPoNo; }
      set { this.cusPoNo = value; }
    }
    public long CustomerPid
    {
      get { return this.customerPid; }
      set { this.customerPid = value; }
    }
    public string Condition
    {
      get { return this.condition; }
      set { this.condition = value; }
    }
    public long CurrentWorkOrder
    {
      get { return this.currentWorkOrder; }
      set { this.currentWorkOrder = value; }
    }

    #endregion Properties
  }
}