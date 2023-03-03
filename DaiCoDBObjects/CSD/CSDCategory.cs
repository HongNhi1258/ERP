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
  public class CSDCategory : IObject
  {
    #region Fields
    private long pid;
    private string usCateCode;
    private string category;
    private long parentPid;
    private string moreDescription;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public CSDCategory()
    {
      this.pid = long.MinValue;
      this.usCateCode = string.Empty;
      this.category = string.Empty;
      this.parentPid = long.MinValue;
      this.moreDescription = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      CSDCategory obj = new CSDCategory();
      obj.Pid = this.pid;
      obj.USCateCode = this.usCateCode;
      obj.Category = this.category;
      obj.ParentPid = this.parentPid;
      obj.MoreDescription = this.moreDescription;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblCSDCategory";
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
    public string USCateCode
    {
      get { return this.usCateCode; }
      set { this.usCateCode = value; }
    }
    public string Category
    {
      get { return this.category; }
      set { this.category = value; }
    }
    public long ParentPid
    {
      get { return this.parentPid; }
      set { this.parentPid = value; }
    }
    public string MoreDescription
    {
      get { return this.moreDescription; }
      set { this.moreDescription = value; }
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