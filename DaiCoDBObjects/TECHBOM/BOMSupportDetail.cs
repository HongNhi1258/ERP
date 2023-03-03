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
  public class BOMSupportDetail : IObject
  {
    #region Fields
    private long pid;
    private string supCode;
    private string materialCode;
    private double qty;
    private double totalQty;
    private double waste;
    private int width;
    private int depth;
    private int height;
    private string remark;
    private string alternative;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public BOMSupportDetail()
    {
      this.pid = long.MinValue;
      this.supCode = string.Empty;
      this.materialCode = string.Empty;
      this.qty = double.MinValue;
      this.totalQty = double.MinValue;
      this.waste = double.MinValue;
      this.width = int.MinValue;
      this.depth = int.MinValue;
      this.height = int.MinValue;
      this.remark = string.Empty;
      this.alternative = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      BOMSupportDetail obj = new BOMSupportDetail();
      obj.Pid = this.pid;
      obj.SupCode = this.supCode;
      obj.MaterialCode = this.materialCode;
      obj.Qty = this.qty;
      obj.TotalQty = this.totalQty;
      obj.Waste = this.waste;
      obj.Width = this.width;
      obj.Depth = this.depth;
      obj.Height = this.height;
      obj.Remark = this.remark;
      obj.Alternative = this.alternative;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMSupportDetail";
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
    public string SupCode
    {
      get { return this.supCode; }
      set { this.supCode = value; }
    }
    public string MaterialCode
    {
      get { return this.materialCode; }
      set { this.materialCode = value; }
    }
    public double Qty
    {
      get { return this.qty; }
      set { this.qty = value; }
    }
    public double TotalQty
    {
      get { return this.totalQty; }
      set { this.totalQty = value; }
    }
    public double Waste
    {
      get { return this.waste; }
      set { this.waste = value; }
    }
    public int Width
    {
      get { return this.width; }
      set { this.width = value; }
    }
    public int Depth
    {
      get { return this.depth; }
      set { this.depth = value; }
    }
    public int Height
    {
      get { return this.height; }
      set { this.height = value; }
    }
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
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
    #endregion Properties
  }
}