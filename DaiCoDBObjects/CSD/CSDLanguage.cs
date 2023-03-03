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
  public class CSDLanguage : IObject
  {
    #region Fields
    private long pid;
    private string nameEN;
    private string nameVN;
    private int orderBy;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public CSDLanguage()
    {
      this.pid = long.MinValue;
      this.nameEN = string.Empty;
      this.nameVN = string.Empty;
      this.orderBy = int.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      CSDLanguage obj = new CSDLanguage();
      obj.Pid = this.pid;
      obj.NameEN = this.nameEN;
      obj.NameVN = this.nameVN;
      obj.OrderBy = this.orderBy;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblCSDLanguage";
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
    public string NameEN
    {
      get { return this.nameEN; }
      set { this.nameEN = value; }
    }
    public string NameVN
    {
      get { return this.nameVN; }
      set { this.nameVN = value; }
    }
    public int OrderBy
    {
      get { return this.orderBy; }
      set { this.orderBy = value; }
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