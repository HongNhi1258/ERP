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
  public class PLNEnquiryDetail : IObject
  {
    #region Fields
    private long pid;
    private long enquiryPid;
    private string itemCode;
    private int revision;
    private double qty;
    private DateTime requestDate;
    private string remark;
    private int createBy;
    private DateTime createDate;
    private int updateByCSS;
    private DateTime updateDateCSS;
    private int updateByPLN;
    private DateTime updateDatePLN;
    #endregion Fields

    #region Constructors
    public PLNEnquiryDetail()
    {
      this.pid = long.MinValue;
      this.enquiryPid = long.MinValue;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.qty = double.MinValue;
      this.requestDate = DateTime.MinValue;
      this.remark = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateByCSS = int.MinValue;
      this.updateDateCSS = DateTime.MinValue;
      this.updateByPLN = int.MinValue;
      this.updateDatePLN = DateTime.MinValue;
    }
    public object Clone()
    {
      PLNEnquiryDetail obj = new PLNEnquiryDetail();
      obj.Pid = this.pid;
      obj.EnquiryPid = this.enquiryPid;
      obj.ItemCode = this.itemCode;
      obj.Revision = this.revision;
      obj.Qty = this.qty;
      obj.RequestDate = this.requestDate;
      obj.Remark = this.remark;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateByCSS = this.updateByCSS;
      obj.UpdateDateCSS = this.updateDateCSS;
      obj.UpdateByPLN = this.updateByPLN;
      obj.UpdateDatePLN = this.updateDatePLN;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPLNEnquiryDetail";
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
    public long EnquiryPid
    {
      get { return this.enquiryPid; }
      set { this.enquiryPid = value; }
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
    public DateTime RequestDate
    {
      get { return this.requestDate; }
      set { this.requestDate = value; }
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
    #endregion Properties
  }
}