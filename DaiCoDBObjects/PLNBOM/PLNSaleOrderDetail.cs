/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 05/07/2010
  Company :  Dai Co
  update 21/07/2015 Add Col Return
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class PLNSaleOrderDetail : IObject
  {
    #region Fields
    private long pid;
    private long saleOrderPid;
    private long enquiryConfirmDetailPid;
    private string itemCode;
    private int revision;
    private double qty;
    private DateTime scheduleDelivery;
    private DateTime realDelivery;
    private double price;
    private double secondprice;
    private string remark;
    private string remarkforaccount;
    private string specialDescription;
    private string specialinstruction;
    private string urgentNote;
    private int createBy;
    private DateTime createDate;
    private int updateByCSS;
    private DateTime updateDateCSS;
    private int updateByACC;
    private DateTime updateDateACC;
    private int updateByPLN;
    private DateTime updateDatePLN;
    private int confirmPrice;
    private int foc;
    private int Return;
    //-----------
    //private int shipTogether;
    #endregion Fields

    #region Constructors
    public PLNSaleOrderDetail()
    {
      this.pid = long.MinValue;
      this.saleOrderPid = long.MinValue;
      this.enquiryConfirmDetailPid = long.MinValue;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.qty = double.MinValue;
      this.scheduleDelivery = DateTime.MinValue;
      this.realDelivery = DateTime.MinValue;
      this.price = double.MinValue;
      this.secondprice = double.MinValue;
      this.IReturn = int.MinValue;
      this.remark = string.Empty;
      this.urgentNote = string.Empty;
      this.specialDescription = string.Empty;
      this.specialinstruction = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateByCSS = int.MinValue;
      this.updateDateCSS = DateTime.MinValue;
      this.updateByACC = int.MinValue;
      this.updateDateACC = DateTime.MinValue;
      this.updateByPLN = int.MinValue;
      this.updateDatePLN = DateTime.MinValue;
      this.confirmPrice = int.MinValue;
      this.remarkforaccount = string.Empty;

    }
    public object Clone()
    {
      PLNSaleOrderDetail obj = new PLNSaleOrderDetail();
      obj.Pid = this.pid;
      obj.SaleOrderPid = this.saleOrderPid;
      obj.EnquiryConfirmDetailPid = this.enquiryConfirmDetailPid;
      obj.ItemCode = this.itemCode;
      obj.Revision = this.revision;
      obj.Qty = this.qty;
      obj.ScheduleDelivery = this.scheduleDelivery;
      obj.RealDelivery = this.realDelivery;
      obj.Price = this.price;
      obj.SecondPrice = this.secondprice;
      obj.Remark = this.remark;
      obj.UrgentNote = this.urgentNote;
      obj.RemarkForAccount = this.remarkforaccount;
      obj.SpecialDescription = this.specialDescription;
      obj.SpecialInstruction = this.specialinstruction;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateByCSS = this.updateByCSS;
      obj.UpdateDateCSS = this.updateDateCSS;
      obj.UpdateByACC = this.updateByACC;
      obj.UpdateDateACC = this.updateDateACC;
      obj.UpdateByPLN = this.updateByPLN;
      obj.UpdateDatePLN = this.updateDatePLN;
      obj.confirmPrice = this.confirmPrice;
      obj.Foc = this.foc;
      obj.Return = this.Return;
      //obj.ShipTogether = this.shipTogether;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPLNSaleOrderDetail";
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
    public long SaleOrderPid
    {
      get { return this.saleOrderPid; }
      set { this.saleOrderPid = value; }
    }
    public long EnquiryConfirmDetailPid
    {
      get { return this.enquiryConfirmDetailPid; }
      set { this.enquiryConfirmDetailPid = value; }
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
    public double Qty
    {
      get { return this.qty; }
      set { this.qty = value; }
    }
    public DateTime ScheduleDelivery
    {
      get { return this.scheduleDelivery; }
      set { this.scheduleDelivery = value; }
    }
    public DateTime RealDelivery
    {
      get { return this.realDelivery; }
      set { this.realDelivery = value; }
    }
    public double Price
    {
      get { return this.price; }
      set { this.price = value; }
    }
    public double SecondPrice
    {
      get { return this.secondprice; }
      set { this.secondprice = value; }
    }
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
    }

    public string UrgentNote
    {
      get { return this.urgentNote; }
      set { this.urgentNote = value; }
    }
    public string RemarkForAccount
    {
      get { return this.remarkforaccount; }
      set { this.remarkforaccount = value; }
    }
    public string SpecialDescription
    {
      get { return this.specialDescription; }
      set { this.specialDescription = value; }
    }

    public string SpecialInstruction
    {
      get { return this.specialinstruction; }
      set { this.specialinstruction = value; }
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
    public int UpdateByCSS
    {
      get { return this.updateByCSS; }
      set { this.updateByCSS = value; }
    }
    public DateTime UpdateDateCSS
    {
      get { return this.updateDateCSS; }
      set { this.updateDateCSS = value; }
    }
    public int UpdateByACC
    {
      get { return this.updateByACC; }
      set { this.updateByACC = value; }
    }
    public DateTime UpdateDateACC
    {
      get { return this.updateDateACC; }
      set { this.updateDateACC = value; }
    }
    public int UpdateByPLN
    {
      get { return this.updateByPLN; }
      set { this.updateByPLN = value; }
    }
    public DateTime UpdateDatePLN
    {
      get { return this.updateDatePLN; }
      set { this.updateDatePLN = value; }
    }
    public int ConfirmPrice
    {
      get { return this.confirmPrice; }
      set { this.confirmPrice = value; }
    }

    public int Foc
    {
      get { return this.foc; }
      set { this.foc = value; }
    }

    //Cuong Add
    public int IReturn
    {
      get
      {
        return this.Return;
      }
      set
      {
        this.Return = value;
      }
    }
    //---------

    //public int ShipTogether
    //{
    //  get { return this.shipTogether; }
    //  set { this.shipTogether = value; }
    //}
    #endregion Properties
  }
}