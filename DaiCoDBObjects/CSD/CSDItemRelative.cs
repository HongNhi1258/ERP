/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 07/10/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class CSDItemRelative : IObject
  {
    #region Fields
    private long pid;
    private string itemCode;
    private string relativeItem;
    private string description;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public CSDItemRelative()
    {
      this.pid = long.MinValue;
      this.itemCode = string.Empty;
      this.relativeItem = string.Empty;
      this.description = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      CSDItemRelative obj = new CSDItemRelative();
      obj.Pid = this.pid;
      obj.ItemCode = this.itemCode;
      obj.RelativeItem = this.relativeItem;
      obj.Description = this.description;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblCSDItemRelative";
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
    public string ItemCode
    {
      get { return this.itemCode; }
      set { this.itemCode = value; }
    }
    public string RelativeItem
    {
      get { return this.relativeItem; }
      set { this.relativeItem = value; }
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
    #endregion Properties
  }
}