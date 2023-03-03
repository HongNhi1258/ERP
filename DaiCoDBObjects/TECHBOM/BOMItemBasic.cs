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
  public class BOMItemBasic : IObject
  {
    #region Fields
    private string itemCode;
    private string saleCode;
    private string carcassCode;
    private string name;
    private string nameVN;
    private string description;
    private int category;
    private int collection;
    private int widthDefault;
    private int depthDefault;
    private int highDefault;
    private double netWeightDefault;
    private string fGCode;
    private string mainFinish;
    private string otherFinish;
    private string mainMaterial;
    private string otherMaterial;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private int allowUser;
    private int confirm;
    private int kD;
    private double grossWeight;
    private string unit;
    private int itemPerBox;
    private int boxPerItem;
    private double cBM;
    private string itemRelative;
    private int revisionActive;
    #endregion Fields

    #region Constructors
    public BOMItemBasic()
    {
      this.itemCode = string.Empty;
      this.saleCode = string.Empty;
      this.carcassCode = string.Empty;
      this.name = string.Empty;
      this.nameVN = string.Empty;
      this.description = string.Empty;
      this.category = int.MinValue;
      this.collection = int.MinValue;
      this.widthDefault = int.MinValue;
      this.depthDefault = int.MinValue;
      this.highDefault = int.MinValue;
      this.netWeightDefault = double.MinValue;
      this.fGCode = string.Empty;
      this.mainFinish = string.Empty;
      this.otherFinish = string.Empty;
      this.mainMaterial = string.Empty;
      this.otherMaterial = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.allowUser = int.MinValue;
      this.confirm = int.MinValue;
      this.kD = int.MinValue;
      this.grossWeight = double.MinValue;
      this.unit = string.Empty;
      this.itemPerBox = int.MinValue;
      this.boxPerItem = int.MinValue;
      this.cBM = double.MinValue;
      this.itemRelative = string.Empty;
      this.revisionActive = int.MinValue;
    }
    public object Clone()
    {
      BOMItemBasic obj = new BOMItemBasic();
      obj.ItemCode = this.itemCode;
      obj.SaleCode = this.saleCode;
      obj.CarcassCode = this.carcassCode;
      obj.Name = this.name;
      obj.NameVN = this.nameVN;
      obj.Description = this.description;
      obj.Category = this.category;
      obj.Collection = this.collection;
      obj.WidthDefault = this.widthDefault;
      obj.DepthDefault = this.depthDefault;
      obj.HighDefault = this.highDefault;
      obj.NetWeightDefault = this.netWeightDefault;
      obj.FGCode = this.fGCode;
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
      obj.KD = this.kD;
      obj.GrossWeight = this.grossWeight;
      obj.Unit = this.unit;
      obj.ItemPerBox = this.itemPerBox;
      obj.BoxPerItem = this.boxPerItem;
      obj.CBM = this.cBM;
      obj.ItemRelative = this.itemRelative;
      obj.RevisionActive = this.revisionActive;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMItemBasic";
    }
    public string[] ObjectKey()
    {
      return new string[] { "ItemCode" };
    }
    #endregion Constructors

    #region Properties

    public string ItemCode
    {
      get { return this.itemCode; }
      set { this.itemCode = value; }
    }
    public string SaleCode
    {
      get { return this.saleCode; }
      set { this.saleCode = value; }
    }
    public string CarcassCode
    {
      get { return this.carcassCode; }
      set { this.carcassCode = value; }
    }
    public string Name
    {
      get { return this.name; }
      set { this.name = value; }
    }
    public string NameVN
    {
      get { return this.nameVN; }
      set { this.nameVN = value; }
    }
    public string Description
    {
      get { return this.description; }
      set { this.description = value; }
    }
    public int Category
    {
      get { return this.category; }
      set { this.category = value; }
    }
    public int Collection
    {
      get { return this.collection; }
      set { this.collection = value; }
    }
    public int WidthDefault
    {
      get { return this.widthDefault; }
      set { this.widthDefault = value; }
    }
    public int DepthDefault
    {
      get { return this.depthDefault; }
      set { this.depthDefault = value; }
    }
    public int HighDefault
    {
      get { return this.highDefault; }
      set { this.highDefault = value; }
    }
    public double NetWeightDefault
    {
      get { return this.netWeightDefault; }
      set { this.netWeightDefault = value; }
    }
    public string FGCode
    {
      get { return this.fGCode; }
      set { this.fGCode = value; }
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
    public string MainMaterial
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
    public int KD
    {
      get { return this.kD; }
      set { this.kD = value; }
    }
    public double GrossWeight
    {
      get { return this.grossWeight; }
      set { this.grossWeight = value; }
    }
    public string Unit
    {
      get { return this.unit; }
      set { this.unit = value; }
    }
    public int ItemPerBox
    {
      get { return this.itemPerBox; }
      set { this.itemPerBox = value; }
    }
    public int BoxPerItem
    {
      get { return this.boxPerItem; }
      set { this.boxPerItem = value; }
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
    public int RevisionActive
    {
      get { return this.revisionActive; }
      set { this.revisionActive = value; }
    }
    #endregion Properties
  }
}