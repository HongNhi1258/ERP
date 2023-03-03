/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 07/10/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class CSDItemInfo : IObject
  {
    #region Fields
    private long pid;
    private string itemCode;
    private string customerItemCodeDefault;
    private int discontinueFlag;
    private int samplePid;
    private int uSActive;
    private int uSStock;
    private int uKActive;
    private int uKStock;
    private int spainActive;
    private int spainStock;
    private string pageNo;
    private string makertingDescription;
    private string remark;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public CSDItemInfo()
    {
      this.pid = long.MinValue;
      this.itemCode = string.Empty;
      this.customerItemCodeDefault = string.Empty;
      this.discontinueFlag = int.MinValue;
      this.samplePid = int.MinValue;
      this.uSActive = int.MinValue;
      this.uSStock = int.MinValue;
      this.uKActive = int.MinValue;
      this.uKStock = int.MinValue;
      this.spainActive = int.MinValue;
      this.spainStock = int.MinValue;
      this.pageNo = string.Empty;
      this.makertingDescription = string.Empty;
      this.remark = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      CSDItemInfo obj = new CSDItemInfo();
      obj.Pid = this.pid;
      obj.ItemCode = this.itemCode;
      obj.CustomerItemCodeDefault = this.customerItemCodeDefault;
      obj.DiscontinueFlag = this.discontinueFlag;
      obj.SamplePid = this.samplePid;
      obj.USActive = this.uSActive;
      obj.USStock = this.uSStock;
      obj.UKActive = this.uKActive;
      obj.UKStock = this.uKStock;
      obj.SpainActive = this.spainActive;
      obj.SpainStock = this.spainStock;
      obj.PageNo = this.pageNo;
      obj.MakertingDescription = this.makertingDescription;
      obj.Remark = this.remark;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblCSDItemInfo";
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
    public string ItemCode
    {
      get { return this.itemCode; }
      set { this.itemCode = value; }
    }
    public string CustomerItemCodeDefault
    {
      get { return this.customerItemCodeDefault; }
      set { this.customerItemCodeDefault = value; }
    }
    public int DiscontinueFlag
    {
      get { return this.discontinueFlag; }
      set { this.discontinueFlag = value; }
    }
    public int SamplePid
    {
      get { return this.samplePid; }
      set { this.samplePid = value; }
    }
    public int USActive
    {
      get { return this.uSActive; }
      set { this.uSActive = value; }
    }
    public int USStock
    {
      get { return this.uSStock; }
      set { this.uSStock = value; }
    }
    public int UKActive
    {
      get { return this.uKActive; }
      set { this.uKActive = value; }
    }
    public int UKStock
    {
      get { return this.uKStock; }
      set { this.uKStock = value; }
    }
    public int SpainActive
    {
      get { return this.spainActive; }
      set { this.spainActive = value; }
    }
    public int SpainStock
    {
      get { return this.spainStock; }
      set { this.spainStock = value; }
    }
    public string PageNo
    {
      get { return this.pageNo; }
      set { this.pageNo = value; }
    }
    public string MakertingDescription
    {
      get { return this.makertingDescription; }
      set { this.makertingDescription = value; }
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
    #endregion Properties
  }
}