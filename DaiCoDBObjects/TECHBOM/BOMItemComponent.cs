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
  public class BOMItemComponent : IObject
  {
    #region Fields
    private long pid;
    private string itemCode;
    private int revision;
    private string componentCode;
    private int compRevision;
    private double qty;
    private double waste;
    private int confirm;
    private string alternative;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private string remark;
    private int compGroup;
    private int alterRevision;
    #endregion Fields

    #region Constructors
    public BOMItemComponent()
    {
      this.pid = long.MinValue;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.componentCode = string.Empty;
      this.compRevision = int.MinValue;
      this.qty = double.MinValue;
      this.waste = double.MinValue;
      this.confirm = int.MinValue;
      this.alternative = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.remark = string.Empty;
      this.compGroup = int.MinValue;
      this.alterRevision = int.MinValue;
    }
    public object Clone()
    {
      BOMItemComponent obj = new BOMItemComponent();
      obj.Pid = this.pid;
      obj.ItemCode = this.itemCode;
      obj.Revision = this.revision;
      obj.ComponentCode = this.componentCode;
      obj.CompRevision = this.compRevision;
      obj.Qty = this.qty;
      obj.Waste = this.waste;
      obj.Confirm = this.confirm;
      obj.Alternative = this.alternative;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.Remark = this.remark;
      obj.CompGroup = this.compGroup;
      obj.AlterRevision = this.alterRevision;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMItemComponent";
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
    public int Revision
    {
      get { return this.revision; }
      set { this.revision = value; }
    }
    public string ComponentCode
    {
      get { return this.componentCode; }
      set { this.componentCode = value; }
    }
    public int CompRevision
    {
      get { return this.compRevision; }
      set { this.compRevision = value; }
    }
    public double Qty
    {
      get { return this.qty; }
      set { this.qty = value; }
    }
    public double Waste
    {
      get { return this.waste; }
      set { this.waste = value; }
    }
    public int Confirm
    {
      get { return this.confirm; }
      set { this.confirm = value; }
    }
    public string Alternative
    {
      get { return this.alternative; }
      set { this.alternative = value; }
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
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
    }
    public int CompGroup
    {
      get { return this.compGroup; }
      set { this.compGroup = value; }
    }
    public int AlterRevision
    {
      get { return this.alterRevision; }
      set { this.alterRevision = value; }
    }
    #endregion Properties
  }
}