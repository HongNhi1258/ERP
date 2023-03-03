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
  public class BOMCarcassComponentDetail : IObject
  {
    #region Fields
    private long pid;
    private long componentPid;
    private string materialCode;
    private double waste;
    private double length;
    private double width;
    private double thickness;
    private string alternative;
    private double qty;
    private double totalQty;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public BOMCarcassComponentDetail()
    {
      this.pid = long.MinValue;
      this.componentPid = long.MinValue;
      this.materialCode = string.Empty;
      this.waste = double.MinValue;
      this.length = double.MinValue;
      this.width = double.MinValue;
      this.thickness = double.MinValue;
      this.alternative = string.Empty;
      this.qty = double.MinValue;
      this.totalQty = double.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      BOMCarcassComponentDetail obj = new BOMCarcassComponentDetail();
      obj.Pid = this.pid;
      obj.ComponentPid = this.componentPid;
      obj.MaterialCode = this.materialCode;
      obj.Waste = this.waste;
      obj.Length = this.length;
      obj.Width = this.width;
      obj.Thickness = this.thickness;
      obj.Alternative = this.alternative;
      obj.Qty = this.qty;
      obj.TotalQty = this.totalQty;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMCarcassComponentDetail";
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
    public long ComponentPid
    {
      get { return this.componentPid; }
      set { this.componentPid = value; }
    }
    public string MaterialCode
    {
      get { return this.materialCode; }
      set { this.materialCode = value; }
    }
    public double Waste
    {
      get { return this.waste; }
      set { this.waste = value; }
    }
    public double Length
    {
      get { return this.length; }
      set { this.length = value; }
    }
    public double Width
    {
      get { return this.width; }
      set { this.width = value; }
    }
    public double Thickness
    {
      get { return this.thickness; }
      set { this.thickness = value; }
    }
    public string Alternative
    {
      get { return this.alternative; }
      set { this.alternative = value; }
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