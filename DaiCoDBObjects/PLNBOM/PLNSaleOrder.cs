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
  public class PLNSaleOrder : IObject
  {
    #region Fields
    private long pid;
    private string saleNo;
    private string customerPONo;
    private long customerPid;
    private long directPid;
    private DateTime orderDate;
    private int type;
    private int deleteFlag;
    private int status;
    private string remark;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    // Hung them REFNo PODate RequiredShipDate ContractNo,ConfirmedShipDate,ConfirmedShipDate,DeliveryRequirement,PackingRequirement,DocumentRequirement,Cancel,CancelDate,CancelBy
    private string refno;
    private DateTime podate;
    private DateTime requiredshipDate;
    private DateTime confirmedshipDate;
    private string contractno;
    private string deliveryrequirement;
    private string packingrequirement;
    private string documentrequirement;
    private string shipmentTerms;
    private string paymentTerms;
    private int environmentStatus;
    private int cancel;
    private DateTime canceldate;
    private int cancelby;
    private DateTime csconfirmdate;
    private int csconfirmby;
    private DateTime accconfirmdate;
    private int accconfirmby;
    private int priceType;
    #endregion Fields

    #region Constructors
    public PLNSaleOrder()
    {
      this.pid = long.MinValue;
      this.saleNo = string.Empty;
      this.customerPONo = string.Empty;
      this.customerPid = long.MinValue;
      this.directPid = long.MinValue;
      this.orderDate = DateTime.MinValue;
      this.type = int.MinValue;
      this.deleteFlag = int.MinValue;
      this.status = int.MinValue;
      this.remark = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      //Hung them REFNo PODate RequiredShipDate ContractNo,ConfirmedShipDate,ConfirmedShipDate,DeliveryRequirement,PackingRequirement,DocumentRequirement,Cancel,CancelDate,CancelBy
      this.refno = string.Empty;
      this.requiredshipDate = DateTime.MinValue;
      this.confirmedshipDate = DateTime.MinValue;
      this.podate = DateTime.MinValue;
      this.contractno = string.Empty;
      this.deliveryrequirement = string.Empty;
      this.packingrequirement = string.Empty;
      this.documentrequirement = string.Empty;
      this.cancel = int.MinValue;
      this.canceldate = DateTime.MinValue;
      this.cancelby = int.MinValue;
      this.csconfirmdate = DateTime.MinValue;
      this.csconfirmby = int.MinValue;
      this.accconfirmdate = DateTime.MinValue;
      this.accconfirmby = int.MinValue;
      this.priceType = int.MinValue;
    }
    public object Clone()
    {
      PLNSaleOrder obj = new PLNSaleOrder();
      obj.Pid = this.pid;
      obj.SaleNo = this.saleNo;
      obj.CustomerPONo = this.customerPONo;
      obj.CustomerPid = this.customerPid;
      obj.DirectPid = this.directPid;
      obj.OrderDate = this.orderDate;
      obj.Type = this.type;
      obj.DeleteFlag = this.deleteFlag;
      obj.Status = this.status;
      obj.Remark = this.remark;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      //Hung them REFNo PODate RequiredShipDate ContractNo,ConfirmedShipDate,DeliveryRequirement,PackingRequirement,DocumentRequirement
      obj.REFNo = this.refno;
      obj.PODate = this.podate;
      obj.RequiredShipDate = this.requiredshipDate;
      obj.ConfirmedShipDate = this.confirmedshipDate;
      obj.ContractNo = this.contractno;
      obj.DeliveryRequirement = this.deliveryrequirement;
      obj.PackingRequirement = this.packingrequirement;
      obj.DocumentRequirement = this.documentrequirement;
      obj.ShipmentTerms = this.shipmentTerms;
      obj.PaymentTerms = this.paymentTerms;
      obj.EnvironmentStatus = this.environmentStatus;
      obj.Cancel = this.cancel;
      obj.CancelDate = this.canceldate;
      obj.CancelBy = this.cancelby;
      obj.CSConfirmDate = this.csconfirmdate;
      obj.CSConfirmBy = this.csconfirmby;
      obj.ACCConfirmDate = this.accconfirmdate;
      obj.ACCConfirmBy = this.accconfirmby;
      obj.priceType = this.priceType;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPLNSaleOrder";
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
    public string SaleNo
    {
      get { return this.saleNo; }
      set { this.saleNo = value; }
    }
    public string CustomerPONo
    {
      get { return this.customerPONo; }
      set { this.customerPONo = value; }
    }
    public long CustomerPid
    {
      get { return this.customerPid; }
      set { this.customerPid = value; }
    }
    public long DirectPid
    {
      get { return this.directPid; }
      set { this.directPid = value; }
    }
    public DateTime OrderDate
    {
      get { return this.orderDate; }
      set { this.orderDate = value; }
    }
    public int Type
    {
      get { return this.type; }
      set { this.type = value; }
    }
    public int DeleteFlag
    {
      get { return this.deleteFlag; }
      set { this.deleteFlag = value; }
    }
    public int Status
    {
      get { return this.status; }
      set { this.status = value; }
    }
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
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
    public string REFNo
    {
      get { return this.refno; }
      set { this.refno = value; }
    }
    public DateTime PODate
    {
      get { return this.podate; }
      set { this.podate = value; }
    }
    public DateTime RequiredShipDate
    {
      get { return this.requiredshipDate; }
      set { this.requiredshipDate = value; }
    }
    public DateTime ConfirmedShipDate
    {
      get { return this.confirmedshipDate; }
      set { this.confirmedshipDate = value; }
    }
    public string ContractNo
    {
      get { return this.contractno; }
      set { this.contractno = value; }
    }
    public string DeliveryRequirement
    {
      get { return this.deliveryrequirement; }
      set { this.deliveryrequirement = value; }
    }
    public string PackingRequirement
    {
      get { return this.packingrequirement; }
      set { this.packingrequirement = value; }
    }
    public string DocumentRequirement
    {
      get { return this.documentrequirement; }
      set { this.documentrequirement = value; }
    }
    public string ShipmentTerms
    {
      get { return this.shipmentTerms; }
      set { this.shipmentTerms = value; }
    }
    public string PaymentTerms
    {
      get { return this.paymentTerms; }
      set { this.paymentTerms = value; }
    }
    public int EnvironmentStatus
    {
      get { return this.environmentStatus; }
      set { this.environmentStatus = value; }
    }
    public int Cancel
    {
      get { return this.cancel; }
      set { this.cancel = value; }
    }
    public DateTime CancelDate
    {
      get { return this.canceldate; }
      set { this.canceldate = value; }
    }
    public int CancelBy
    {
      get { return this.cancelby; }
      set { this.cancelby = value; }
    }
    public DateTime CSConfirmDate
    {
      get { return this.csconfirmdate; }
      set { this.csconfirmdate = value; }
    }
    public int CSConfirmBy
    {
      get { return this.csconfirmby; }
      set { this.csconfirmby = value; }
    }
    public DateTime ACCConfirmDate
    {
      get { return this.accconfirmdate; }
      set { this.accconfirmdate = value; }
    }
    public int ACCConfirmBy
    {
      get { return this.accconfirmby; }
      set { this.accconfirmby = value; }
    }
    public int PriceType
    {
      get { return this.priceType; }
      set { this.priceType = value; }
    }

    #endregion Properties
  }
}