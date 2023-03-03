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
  public class BOMComponentInfoDetail : IObject
  {
    #region Fields
    private long pID;
    private int componentInfoPID;
    private string materialCode;
    private double qty;
    private double totalQty;
    private double waste;
    private double length;
    private double width;
    private double thickness;
    private string alternative;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public BOMComponentInfoDetail()
    {
      this.pID = long.MinValue;
      this.componentInfoPID = int.MinValue;
      this.materialCode = string.Empty;
      this.qty = double.MinValue;
      this.totalQty = double.MinValue;
      this.waste = double.MinValue;
      this.length = double.MinValue;
      this.width = double.MinValue;
      this.thickness = double.MinValue;
      this.alternative = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      BOMComponentInfoDetail obj = new BOMComponentInfoDetail();
      obj.PID = this.pID;
      obj.ComponentInfoPID = this.componentInfoPID;
      obj.MaterialCode = this.materialCode;
      obj.Qty = this.qty;
      obj.TotalQty = this.totalQty;
      obj.Waste = this.waste;
      obj.Length = this.length;
      obj.Width = this.width;
      obj.Thickness = this.thickness;
      obj.Alternative = this.alternative;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMComponentInfoDetail";
    }
    public string[] ObjectKey()
    {
      return new string[] { "PID" };
    }
    #endregion Constructors

    #region Properties

    public long PID
    {
      get { return this.pID; }
      set { this.pID = value; }
    }
    public int ComponentInfoPID
    {
      get { return this.componentInfoPID; }
      set { this.componentInfoPID = value; }
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