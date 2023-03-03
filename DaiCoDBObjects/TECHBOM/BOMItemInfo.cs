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
  public class BOMItemInfo : IObject
  {
    #region Fields
    private string itemCode;
    private string carcassCode;
    private int revision;
    private int width;
    private int depth;
    private int high;
    private int kD;
    private string mainFinish;
    private string otherFinish;
    private int mainMaterial;
    private string otherMaterial;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private int allowUser;
    private int confirm;
    private string unit;
    private double cBM;
    private string itemRelative;
    private string supCode;
    private string packageCode;
    #endregion Fields

    #region Constructors
    public BOMItemInfo()
    {
      this.itemCode = string.Empty;
      this.carcassCode = string.Empty;
      this.revision = int.MinValue;
      this.width = int.MinValue;
      this.depth = int.MinValue;
      this.high = int.MinValue;
      this.kD = int.MinValue;
      this.mainFinish = string.Empty;
      this.otherFinish = string.Empty;
      this.mainMaterial = int.MinValue;
      this.otherMaterial = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.allowUser = int.MinValue;
      this.confirm = int.MinValue;
      this.unit = string.Empty;
      this.cBM = double.MinValue;
      this.itemRelative = string.Empty;
      this.supCode = string.Empty;
      this.packageCode = string.Empty;
    }
    public object Clone()
    {
      BOMItemInfo obj = new BOMItemInfo();
      obj.ItemCode = this.itemCode;
      obj.CarcassCode = this.carcassCode;
      obj.Revision = this.revision;
      obj.Width = this.width;
      obj.Depth = this.depth;
      obj.High = this.high;
      obj.KD = this.kD;
      obj.MainFinish = this.mainFinish;
      obj.OtherFinish = this.otherFinish;
      obj.MainMaterial = this.mainMaterial;
      obj.OtherMaterial = this.otherMaterial;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.AllowUser = this.allowUser;
      obj.Confirm = this.confirm;
      obj.Unit = this.unit;
      obj.CBM = this.cBM;
      obj.ItemRelative = this.itemRelative;
      obj.SupCode = this.supCode;
      obj.PackageCode = this.packageCode;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMItemInfo";
    }
    public string[] ObjectKey()
    {
      return new string[] { "ItemCode", "Revision" };
    }
    #endregion Constructors

    #region Properties

    public string ItemCode
    {
      get { return this.itemCode; }
      set { this.itemCode = value; }
    }
    public string CarcassCode
    {
      get { return this.carcassCode; }
      set { this.carcassCode = value; }
    }
    public int Revision
    {
      get { return this.revision; }
      set { this.revision = value; }
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
    public int High
    {
      get { return this.high; }
      set { this.high = value; }
    }
    public int KD
    {
      get { return this.kD; }
      set { this.kD = value; }
    }
    public string MainFinish
    {
      get { return this.mainFinish; }
      set { this.mainFinish = value; }
    }
    public string OtherFinish
    {
      get { return this.otherFinish; }
      set { this.otherFinish = value; }
    }
    public int MainMaterial
    {
      get { return this.mainMaterial; }
      set { this.mainMaterial = value; }
    }
    public string OtherMaterial
    {
      get { return this.otherMaterial; }
      set { this.otherMaterial = value; }
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
    public int AllowUser
    {
      get { return this.allowUser; }
      set { this.allowUser = value; }
    }
    public int Confirm
    {
      get { return this.confirm; }
      set { this.confirm = value; }
    }
    public string Unit
    {
      get { return this.unit; }
      set { this.unit = value; }
    }
    public double CBM
    {
      get { return this.cBM; }
      set { this.cBM = value; }
    }
    public string ItemRelative
    {
      get { return this.itemRelative; }
      set { this.itemRelative = value; }
    }
    public string SupCode
    {
      get { return this.supCode; }
      set { this.supCode = value; }
    }
    public string PackageCode
    {
      get { return this.packageCode; }
      set { this.packageCode = value; }
    }
    #endregion Properties
  }
}