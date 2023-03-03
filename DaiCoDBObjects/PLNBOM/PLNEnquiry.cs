/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 20/05/2011
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class PLNEnquiry : IObject
  {
    #region Fields
    private long pid;
    private string enquiryNo;
    private string customerEnquiryNo;
    private long customerPid;
    private long directPid;
    private DateTime orderDate;
    private int type;
    private int cancelFlag;
    private int keep;
    private int keepDays;
    private int plnKeepDays;
    private int status;
    private DateTime planningReceiptDate;
    private string remark;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    //Hung them REFNo PODate RequiredShipDate ContractNo,DeliveryRequirement,PackingRequirement,DocumentRequirement
    private string refno;
    private DateTime podate;
    private DateTime requiredshipDate;
    private string contractno;
    private string deliveryrequirement;
    private string packingrequirement;
    private string documentrequirement;
    private string shipmentTerms;
    private string paymentTerms;
    private string environmentStatus;
    private DateTime csconfirmdate;
    #endregion Fields

    #region Constructors
    public PLNEnquiry()
    {
      this.pid = long.MinValue;
      this.enquiryNo = string.Empty;
      this.customerEnquiryNo = string.Empty;
      this.customerPid = long.MinValue;
      this.directPid = long.MinValue;
      this.orderDate = DateTime.MinValue;
      this.type = int.MinValue;
      this.cancelFlag = int.MinValue;
      this.keep = int.MinValue;
      this.keepDays = int.MinValue;
      this.status = int.MinValue;
      this.planningReceiptDate = DateTime.MinValue;
      this.remark = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      //Hung them REFNo PODate RequiredShipDate ContractNo,DeliveryRequirement,PackingRequirement,DocumentRequirement
      this.refno = string.Empty;
      this.requiredshipDate = DateTime.MinValue;
      this.podate = DateTime.MinValue;
      this.contractno = string.Empty;
      this.deliveryrequirement = string.Empty;
      this.packingrequirement = string.Empty;
      this.documentrequirement = string.Empty;
      this.shipmentTerms = string.Empty;
      this.paymentTerms = string.Empty;
      this.environmentStatus = string.Empty;
      this.csconfirmdate = DateTime.MinValue;
      this.plnKeepDays = int.MinValue;

    }
    public object Clone()
    {
      PLNEnquiry obj = new PLNEnquiry();
      obj.Pid = this.pid;
      obj.EnquiryNo = this.enquiryNo;
      obj.CustomerEnquiryNo = this.customerEnquiryNo;
      obj.CustomerPid = this.customerPid;
      obj.DirectPid = this.directPid;
      obj.OrderDate = this.orderDate;
      obj.Type = this.type;
      obj.CancelFlag = this.cancelFlag;
      obj.Keep = this.keep;
      obj.KeepDays = this.keepDays;
      obj.Status = this.status;
      obj.PlanningReceiptDate = this.planningReceiptDate;
      obj.Remark = this.remark;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      //Hung them REFNo PODate RequiredShipDate ContractNo,DeliveryRequirement,PackingRequirement,DocumentRequirement
      obj.REFNo = this.refno;
      obj.PODate = this.podate;
      obj.RequiredShipDate = this.requiredshipDate;
      obj.ContractNo = this.contractno;
      obj.DeliveryRequirement = this.deliveryrequirement;
      obj.PackingRequirement = this.packingrequirement;
      obj.DocumentRequirement = this.documentrequirement;
      obj.ShipmentTerms = this.shipmentTerms;
      obj.PaymentTerms = this.paymentTerms;
      obj.EnvironmentStatus = this.environmentStatus;
      obj.CSConfirmDate = this.csconfirmdate;
      obj.PLNKeepDays = this.plnKeepDays;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPLNEnquiry";
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
    public string EnquiryNo
    {
      get { return this.enquiryNo; }
      set { this.enquiryNo = value; }
    }
    public string CustomerEnquiryNo
    {
      get { return this.customerEnquiryNo; }
      set { this.customerEnquiryNo = value; }
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
    public int CancelFlag
    {
      get { return this.cancelFlag; }
      set { this.cancelFlag = value; }
    }
    public int Keep
    {
      get { return this.keep; }
      set { this.keep = value; }
    }
    public int KeepDays
    {
      get { return this.keepDays; }
      set { this.keepDays = value; }
    }
    public int Status
    {
      get { return this.status; }
      set { this.status = value; }
    }
    public DateTime PlanningReceiptDate
    {
      get { return this.planningReceiptDate; }
      set { this.planningReceiptDate = value; }
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
    //Hung them REFNo PODate RequiredShipDate ContractNo,DeliveryRequirement,PackingRequirement,DocumentRequirement
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
    public string EnvironmentStatus
    {
      get { return this.environmentStatus; }
      set { this.environmentStatus = value; }
    }
    public DateTime CSConfirmDate
    {
      get { return this.csconfirmdate; }
      set { this.csconfirmdate = value; }
    }
    public int PLNKeepDays
    {
      get { return this.plnKeepDays; }
      set { this.plnKeepDays = value; }
    }
    #endregion Properties
  }
}