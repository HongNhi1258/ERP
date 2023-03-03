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
  public class BOMGlassInfo : IObject
  {
    #region Fields
    private int pID;
    private string componentCode;
    private string componentName;
    private string componentNameVN;
    private string materialCode;
    private double length;
    private double width;
    private double thickness;
    private int contractOut;
    private int compGroup;
    private string glassAdj;
    private string remark;
    private string link;
    private int confirm;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private int types;
    private int bevel;
    #endregion Fields

    #region Constructors
    public BOMGlassInfo()
    {
      this.pID = int.MinValue;
      this.componentCode = string.Empty;
      this.componentName = string.Empty;
      this.componentNameVN = string.Empty;
      this.materialCode = string.Empty;
      this.length = double.MinValue;
      this.width = double.MinValue;
      this.thickness = double.MinValue;
      this.contractOut = int.MinValue;
      this.compGroup = int.MinValue;
      this.glassAdj = string.Empty;
      this.remark = string.Empty;
      this.link = string.Empty;
      this.confirm = int.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.types = int.MinValue;
      this.bevel = int.MinValue;
    }
    public object Clone()
    {
      BOMGlassInfo obj = new BOMGlassInfo();
      obj.PID = this.pID;
      obj.ComponentCode = this.componentCode;
      obj.ComponentName = this.componentName;
      obj.ComponentNameVN = this.componentNameVN;
      obj.MaterialCode = this.materialCode;
      obj.Length = this.length;
      obj.Width = this.width;
      obj.Thickness = this.thickness;
      obj.ContractOut = this.contractOut;
      obj.CompGroup = this.compGroup;
      obj.GlassAdj = this.glassAdj;
      obj.Remark = this.remark;
      obj.Link = this.link;
      obj.Confirm = this.confirm;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.Types = this.types;
      obj.Bevel = this.bevel;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMGlassInfo";
    }
    public string[] ObjectKey()
    {
      return new string[] { "PID", "ComponentCode" };
    }
    #endregion Constructors

    #region Properties

    public int PID
    {
      get { return this.pID; }
      set { this.pID = value; }
    }
    public string ComponentCode
    {
      get { return this.componentCode; }
      set { this.componentCode = value; }
    }
    public string ComponentName
    {
      get { return this.componentName; }
      set { this.componentName = value; }
    }
    public string ComponentNameVN
    {
      get { return this.componentNameVN; }
      set { this.componentNameVN = value; }
    }
    public string MaterialCode
    {
      get { return this.materialCode; }
      set { this.materialCode = value; }
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
    public int ContractOut
    {
      get { return this.contractOut; }
      set { this.contractOut = value; }
    }
    public int CompGroup
    {
      get { return this.compGroup; }
      set { this.compGroup = value; }
    }
    public string GlassAdj
    {
      get { return this.glassAdj; }
      set { this.glassAdj = value; }
    }
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
    }
    public string Link
    {
      get { return this.link; }
      set { this.link = value; }
    }
    public int Confirm
    {
      get { return this.confirm; }
      set { this.confirm = value; }
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
    public int Types
    {
      get { return this.types; }
      set { this.types = value; }
    }
    public int Bevel
    {
      get { return this.bevel; }
      set { this.bevel = value; }
    }
    #endregion Properties
  }
}