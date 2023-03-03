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
  public class CSDItemInfoLanguage : IObject
  {
    #region Fields
    private long pid;
    private long itemPid;
    private long languagePid;
    private string itemName;
    private string pageNo;
    private string makertingDescription;
    private string description;
    private string remark;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public CSDItemInfoLanguage()
    {
      this.pid = long.MinValue;
      this.itemPid = long.MinValue;
      this.languagePid = long.MinValue;
      this.itemName = string.Empty;
      this.pageNo = string.Empty;
      this.makertingDescription = string.Empty;
      this.description = string.Empty;
      this.remark = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      CSDItemInfoLanguage obj = new CSDItemInfoLanguage();
      obj.Pid = this.pid;
      obj.ItemPid = this.itemPid;
      obj.LanguagePid = this.languagePid;
      obj.ItemName = this.itemName;
      obj.PageNo = this.pageNo;
      obj.MakertingDescription = this.makertingDescription;
      obj.Description = this.description;
      obj.Remark = this.remark;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblCSDItemInfoLanguage";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Pid", "ItemPid", "LanguagePid" };
    }
    #endregion Constructors

    #region Properties

    public long Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public long ItemPid
    {
      get { return this.itemPid; }
      set { this.itemPid = value; }
    }
    public long LanguagePid
    {
      get { return this.languagePid; }
      set { this.languagePid = value; }
    }
    public string ItemName
    {
      get { return this.itemName; }
      set { this.itemName = value; }
    }
    public string PageNo
    {
      get { return this.pageNo; }
      set { this.pageNo = value; }
    }
    public string MakertingDescription
    {
      get { return this.makertingDescription; }
      set { this.makertingDescription = value; }
    }
    public string Description
    {
      get { return this.description; }
      set { this.description = value; }
    }
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
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