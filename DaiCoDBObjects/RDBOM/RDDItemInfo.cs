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
  public class RDDItemInfo : IObject
  {
    #region Fields
    private string itemCode;
    private string oldCode;
    private string saleCode;
    private string carcassCode;
    private string name;
    private string shortName;
    private string nameVN;
    private string description;
    private int category;
    private int collection;
    private double widthDefault;
    private double depthDefault;
    private double highDefault;
    private string mainFinish;
    private string otherFinish;
    private string mainMaterial;
    private string otherMaterial;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private int confirm;
    private int kD;
    private string unit;
    private double cBM;
    private string packageCode;
    private int isNoLevel2;
    private long customerPid;
    private int exhibition;
    private int itemKind;
    private double customerOwnMaterial;
    private long designerPid;
    private double estCBM;
    private string rdNote;

    #endregion Fields

    #region Constructors
    public RDDItemInfo()
    {
      this.itemCode = string.Empty;
      this.oldCode = string.Empty;
      this.saleCode = string.Empty;
      this.carcassCode = string.Empty;
      this.name = string.Empty;
      this.shortName = string.Empty;
      this.nameVN = string.Empty;
      this.description = string.Empty;
      this.category = int.MinValue;
      this.collection = int.MinValue;
      this.widthDefault = double.MinValue;
      this.depthDefault = double.MinValue;
      this.highDefault = double.MinValue;
      this.mainFinish = string.Empty;
      this.otherFinish = string.Empty;
      this.mainMaterial = string.Empty;
      this.otherMaterial = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.confirm = int.MinValue;
      this.kD = int.MinValue;
      this.unit = string.Empty;
      this.cBM = double.MinValue;
      this.packageCode = string.Empty;
      this.isNoLevel2 = int.MinValue;
      this.customerPid = long.MinValue;
      this.exhibition = int.MinValue;
      this.itemKind = int.MinValue;
      this.customerOwnMaterial = double.MinValue;
      this.designerPid = long.MinValue;
      this.estCBM = double.MinValue;
      this.rdNote = string.Empty;
    }
    public object Clone()
    {
      RDDItemInfo obj = new RDDItemInfo();
      obj.ItemCode = this.itemCode;
      obj.OldCode = this.oldCode;
      obj.SaleCode = this.saleCode;
      obj.CarcassCode = this.carcassCode;
      obj.Name = this.name;
      obj.ShortName = this.shortName;
      obj.NameVN = this.nameVN;
      obj.Description = this.description;
      obj.Category = this.category;
      obj.Collection = this.collection;
      obj.WidthDefault = this.widthDefault;
      obj.DepthDefault = this.depthDefault;
      obj.HighDefault = this.highDefault;
      obj.MainFinish = this.mainFinish;
      obj.OtherFinish = this.otherFinish;
      obj.MainMaterial = this.mainMaterial;
      obj.OtherMaterial = this.otherMaterial;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.Confirm = this.confirm;
      obj.KD = this.kD;
      obj.Unit = this.unit;
      obj.CBM = this.cBM;
      obj.PackageCode = this.packageCode;
      obj.IsNoLevel2 = this.isNoLevel2;
      obj.CustomerPid = this.customerPid;
      obj.Exhibition = this.exhibition;
      obj.ItemKind = this.itemKind;
      obj.CustomerOwnMaterial = this.customerOwnMaterial;
      obj.DesignerPid = this.designerPid;
      obj.EstCBM = this.estCBM;
      obj.RDNote = this.rdNote;
      return obj;
    }
    public string GetTableName()
    {
      return "TblRDDItemInfo";
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
    public string OldCode
    {
      get { return this.oldCode; }
      set { this.oldCode = value; }
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
    public string ShortName
    {
      get { return this.shortName; }
      set { this.shortName = value; }
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
    public double WidthDefault
    {
      get { return this.widthDefault; }
      set { this.widthDefault = value; }
    }
    public double DepthDefault
    {
      get { return this.depthDefault; }
      set { this.depthDefault = value; }
    }
    public double HighDefault
    {
      get { return this.highDefault; }
      set { this.highDefault = value; }
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
    public string PackageCode
    {
      get { return this.packageCode; }
      set { this.packageCode = value; }
    }
    public int IsNoLevel2
    {
      get { return this.isNoLevel2; }
      set { this.isNoLevel2 = value; }
    }
    public long CustomerPid
    {
      get { return this.customerPid; }
      set { this.customerPid = value; }
    }
    public int Exhibition
    {
      get { return this.exhibition; }
      set { this.exhibition = value; }
    }
    public int ItemKind
    {
      get { return this.itemKind; }
      set { this.itemKind = value; }
    }
    public double CustomerOwnMaterial
    {
      get { return this.customerOwnMaterial; }
      set { this.customerOwnMaterial = value; }
    }
    public long DesignerPid
    {
      get { return this.designerPid; }
      set { this.designerPid = value; }
    }

    public double EstCBM
    {
      get { return this.estCBM; }
      set { this.estCBM = value; }
    }

    public string RDNote
    {
      get { return this.rdNote; }
      set { this.rdNote = value; }
    }
    #endregion Properties
  }
}