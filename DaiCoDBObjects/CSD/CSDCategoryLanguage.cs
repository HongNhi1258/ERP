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
  public class CSDCategoryLanguage : IObject
  {
    #region Fields
    private long pid;
    private long categoryPid;
    private long languagePid;
    private string categoryName;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public CSDCategoryLanguage()
    {
      this.pid = long.MinValue;
      this.categoryPid = long.MinValue;
      this.languagePid = long.MinValue;
      this.categoryName = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      CSDCategoryLanguage obj = new CSDCategoryLanguage();
      obj.Pid = this.pid;
      obj.CategoryPid = this.categoryPid;
      obj.LanguagePid = this.languagePid;
      obj.CategoryName = this.categoryName;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblCSDCategoryLanguage";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Pid", "CategoryPid", "LanguagePid" };
    }
    #endregion Constructors

    #region Properties

    public long Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public long CategoryPid
    {
      get { return this.categoryPid; }
      set { this.categoryPid = value; }
    }
    public long LanguagePid
    {
      get { return this.languagePid; }
      set { this.languagePid = value; }
    }
    public string CategoryName
    {
      get { return this.categoryName; }
      set { this.categoryName = value; }
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