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
  public class RDDCarcassInfo : IObject
  {
    #region Fields
    private string carcassCode;
    private string description;
    private string descriptionVN;
    private int confirm;
    private int deleteFlag;
    private int contractOut;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public RDDCarcassInfo()
    {
      this.carcassCode = string.Empty;
      this.description = string.Empty;
      this.descriptionVN = string.Empty;
      this.confirm = int.MinValue;
      this.deleteFlag = int.MinValue;
      this.contractOut = int.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      RDDCarcassInfo obj = new RDDCarcassInfo();
      obj.CarcassCode = this.carcassCode;
      obj.Description = this.description;
      obj.DescriptionVN = this.descriptionVN;
      obj.Confirm = this.confirm;
      obj.DeleteFlag = this.deleteFlag;
      obj.ContractOut = this.contractOut;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblRDDCarcassInfo";
    }
    public string[] ObjectKey()
    {
      return new string[] { "CarcassCode" };
    }
    #endregion Constructors

    #region Properties

    public string CarcassCode
    {
      get { return this.carcassCode; }
      set { this.carcassCode = value; }
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
    public int ContractOut
    {
      get { return this.contractOut; }
      set { this.contractOut = value; }
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