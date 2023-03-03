/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 12/10/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class WHFShipmentRequestDetail : IObject
  {
    #region Fields
    private long pid;
    private long shipmentRequestPid;
    private long boxPid;
    #endregion Fields

    #region Constructors
    public WHFShipmentRequestDetail()
    {
      this.pid = long.MinValue;
      this.shipmentRequestPid = long.MinValue;
      this.boxPid = long.MinValue;
    }
    public object Clone()
    {
      WHFShipmentRequestDetail obj = new WHFShipmentRequestDetail();
      obj.Pid = this.pid;
      obj.ShipmentRequestPid = this.shipmentRequestPid;
      obj.BoxPid = this.boxPid;
      return obj;
    }
    public string GetTableName()
    {
      return "TblWHFShipmentRequestDetail";
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
    public long ShipmentRequestPid
    {
      get { return this.shipmentRequestPid; }
      set { this.shipmentRequestPid = value; }
    }
    public long BoxPid
    {
      get { return this.boxPid; }
      set { this.boxPid = value; }
    }
    #endregion Properties
  }
}