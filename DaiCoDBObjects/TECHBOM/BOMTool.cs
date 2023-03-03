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
  public class BOMTool : IObject
  {
    #region Fields
    private long pid;
    private string toolCode;
    private string description;
    private string descriptionVN;
    private int diameter;
    private int length;
    private int thickness;
    private int spindleDiameter;
    private int knifeQty;
    private long machinePid;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private int disContinued;
    #endregion Fields

    #region Constructors
    public BOMTool()
    {
      this.pid = long.MinValue;
      this.toolCode = string.Empty;
      this.description = string.Empty;
      this.descriptionVN = string.Empty;
      this.diameter = int.MinValue;
      this.length = int.MinValue;
      this.thickness = int.MinValue;
      this.spindleDiameter = int.MinValue;
      this.knifeQty = int.MinValue;
      this.machinePid = long.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.disContinued = int.MinValue;
    }
    public object Clone()
    {
      BOMTool obj = new BOMTool();
      obj.Pid = this.pid;
      obj.ToolCode = this.toolCode;
      obj.Description = this.description;
      obj.DescriptionVN = this.descriptionVN;
      obj.Diameter = this.diameter;
      obj.Length = this.length;
      obj.Thickness = this.thickness;
      obj.SpindleDiameter = this.spindleDiameter;
      obj.KnifeQty = this.knifeQty;
      obj.MachinePid = this.machinePid;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.DisContinued = this.disContinued;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMTool";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Pid", "ToolCode" };
    }
    #endregion Constructors

    #region Properties

    public long Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public string ToolCode
    {
      get { return this.toolCode; }
      set { this.toolCode = value; }
    }
    public string Description
    {
      get { return this.description; }
      set { this.description = value; }
    }
    public string DescriptionVN
    {
      get { return this.descriptionVN; }
      set { this.descriptionVN = value; }
    }
    public int Diameter
    {
      get { return this.diameter; }
      set { this.diameter = value; }
    }
    public int Length
    {
      get { return this.length; }
      set { this.length = value; }
    }
    public int Thickness
    {
      get { return this.thickness; }
      set { this.thickness = value; }
    }
    public int SpindleDiameter
    {
      get { return this.spindleDiameter; }
      set { this.spindleDiameter = value; }
    }
    public int KnifeQty
    {
      get { return this.knifeQty; }
      set { this.knifeQty = value; }
    }
    public long MachinePid
    {
      get { return this.machinePid; }
      set { this.machinePid = value; }
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
    public int DisContinued
    {
      get { return this.disContinued; }
      set { this.disContinued = value; }
    }
    #endregion Properties
  }
}