/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 09/09/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class WIPAdjustmentDetails : IObject
  {
    #region Fields
    private long pid;
    private long adjustmentPid;
    private long workOrderPid;
    private string itemCode;
    private int revision;
    private string carcassCode;
    private string componentCode;
    private int qty;
    #endregion Fields

    #region Constructors
    public WIPAdjustmentDetails()
    {
      this.pid = long.MinValue;
      this.adjustmentPid = long.MinValue;
      this.workOrderPid = long.MinValue;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.carcassCode = string.Empty;
      this.componentCode = string.Empty;
      this.qty = int.MinValue;
    }
    public object Clone()
    {
      WIPAdjustmentDetails obj = new WIPAdjustmentDetails();
      obj.Pid = this.pid;
      obj.AdjustmentPid = this.adjustmentPid;
      obj.WorkOrderPid = this.workOrderPid;
      obj.ItemCode = this.itemCode;
      obj.Revision = this.revision;
      obj.CarcassCode = this.carcassCode;
      obj.ComponentCode = this.componentCode;
      obj.Qty = this.qty;
      return obj;
    }
    public string GetTableName()
    {
      return "TblWIPAdjustmentDetails";
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
    public long AdjustmentPid
    {
      get { return this.adjustmentPid; }
      set { this.adjustmentPid = value; }
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
    public string ComponentCode
    {
      get { return this.componentCode; }
      set { this.componentCode = value; }
    }
    public int Qty
    {
      get { return this.qty; }
      set { this.qty = value; }
    }
    #endregion Properties
  }
}