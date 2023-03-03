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
  public class PLNSaleOrderCancel : IObject
  {
    #region Fields
    private long pid;
    private string poCancelNo;
    private string customerPoCancelNo;
    private long customerPid;
    private long directPid;
    private DateTime cancelDate;
    private int status;
    private string remark;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private string refNo;
    private string contract;
    #endregion Fields

    #region Constructors
    public PLNSaleOrderCancel()
    {
      this.pid = long.MinValue;
      this.poCancelNo = string.Empty;
      this.customerPoCancelNo = string.Empty;
      this.customerPid = long.MinValue;
      this.directPid = long.MinValue;
      this.cancelDate = DateTime.MinValue;
      this.status = int.MinValue;
      this.remark = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.refNo = string.Empty;
      this.contract = string.Empty;
    }
    public object Clone()
    {
      PLNSaleOrderCancel obj = new PLNSaleOrderCancel();
      obj.Pid = this.pid;
      obj.PoCancelNo = this.poCancelNo;
      obj.CustomerPoCancelNo = this.customerPoCancelNo;
      obj.CustomerPid = this.customerPid;
      obj.DirectPid = this.directPid;
      obj.CancelDate = this.cancelDate;
      obj.Status = this.status;
      obj.Remark = this.remark;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.RefNo = this.refNo;
      obj.Contract = this.contract;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPLNSaleOrderCancel";
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
    public string PoCancelNo
    {
      get { return this.poCancelNo; }
      set { this.poCancelNo = value; }
    }
    public string CustomerPoCancelNo
    {
      get { return this.customerPoCancelNo; }
      set { this.customerPoCancelNo = value; }
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
    public DateTime CancelDate
    {
      get { return this.cancelDate; }
      set { this.cancelDate = value; }
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
    public string RefNo
    {
      get
      {
        return this.refNo;
      }
      set
      {
        this.refNo = value;
      }
    }
    public string Contract
    {
      get
      {
        return this.contract;
      }
      set
      {
        this.contract = value;
      }
    }
    #endregion Properties
  }
}