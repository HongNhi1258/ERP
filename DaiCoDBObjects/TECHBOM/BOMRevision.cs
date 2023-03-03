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
  public class BOMRevision : IObject
  {
    #region Fields
    private int pID;
    private string itemCode;
    private int revision;
    private string description;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private string remark;
    #endregion Fields

    #region Constructors
    public BOMRevision()
    {
      this.pID = int.MinValue;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.description = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.remark = string.Empty;
    }
    public object Clone()
    {
      BOMRevision obj = new BOMRevision();
      obj.PID = this.pID;
      obj.ItemCode = this.itemCode;
      obj.Revision = this.revision;
      obj.Description = this.description;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.Remark = this.remark;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMRevision";
    }
    public string[] ObjectKey()
    {
      return new string[] { "PID" };
    }
    #endregion Constructors

    #region Properties

    public int PID
    {
      get { return this.pID; }
      set { this.pID = value; }
    }
    public string ItemCode
    {
      get { return this.itemCode; }
      set { this.itemCode = value; }
    }
    public int Revision
    {
      get { return this.revision; }
      set { this.revision = value; }
    }
    public string Description
    {
      get { return this.description; }
      set { this.description = value; }
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
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
    }
    #endregion Properties
  }
}