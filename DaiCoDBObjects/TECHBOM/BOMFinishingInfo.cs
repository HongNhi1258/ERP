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
  public class BOMFinishingInfo : IObject
  {
    #region Fields
    private string finCode;
    private string name;
    private string nameVN;
    private int confirm;
    private int deleteFlag;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private double waste;
    private int brassStyle;
    private string sheenLevel;
    private int finishingType;

    #endregion Fields

    #region Constructors
    public BOMFinishingInfo()
    {
      this.finCode = string.Empty;
      this.name = string.Empty;
      this.nameVN = string.Empty;
      this.confirm = int.MinValue;
      this.deleteFlag = int.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.waste = double.MinValue;
      this.brassStyle = int.MinValue;
      this.sheenLevel = string.Empty;
      this.finishingType = int.MinValue;
    }
    public object Clone()
    {
      BOMFinishingInfo obj = new BOMFinishingInfo();
      obj.FinCode = this.finCode;
      obj.Name = this.name;
      obj.NameVN = this.nameVN;
      obj.Confirm = this.confirm;
      obj.DeleteFlag = this.deleteFlag;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.Waste = this.waste;
      obj.BrassStyle = this.brassStyle;
      obj.sheenLevel = this.sheenLevel;
      obj.finishingType = this.finishingType;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMFinishingInfo";
    }
    public string[] ObjectKey()
    {
      return new string[] { "FinCode" };
    }
    #endregion Constructors

    #region Properties

    public string FinCode
    {
      get { return this.finCode; }
      set { this.finCode = value; }
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
    public int Confirm
    {
      get { return this.confirm; }
      set { this.confirm = value; }
    }
    public int DeleteFlag
    {
      get { return this.deleteFlag; }
      set { this.deleteFlag = value; }
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
    public double Waste
    {
      get { return this.waste; }
      set { this.waste = value; }
    }
    public int BrassStyle
    {
      get { return this.brassStyle; }
      set { this.brassStyle = value; }
    }
    public string SheenLevel
    {
      get { return this.sheenLevel; }
      set { this.sheenLevel = value; }
    }
    public int FinishingType
    {
      get { return this.finishingType; }
      set { this.finishingType = value; }
    }
    #endregion Properties
  }
}