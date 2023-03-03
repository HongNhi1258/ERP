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
  public class BOMMachine : IObject
  {
    #region Fields
    private long pid;
    private string nameEn;
    private string nameVn;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private int deleteFlag;
    #endregion Fields

    #region Constructors
    public BOMMachine()
    {
      this.pid = long.MinValue;
      this.nameEn = string.Empty;
      this.nameVn = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.deleteFlag = int.MinValue;
    }
    public object Clone()
    {
      BOMMachine obj = new BOMMachine();
      obj.Pid = this.pid;
      obj.NameEn = this.nameEn;
      obj.NameVn = this.nameVn;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.DeleteFlag = this.deleteFlag;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMMachine";
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
    public string NameEn
    {
      get { return this.nameEn; }
      set { this.nameEn = value; }
    }
    public string NameVn
    {
      get { return this.nameVn; }
      set { this.nameVn = value; }
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
    public int DeleteFlag
    {
      get { return this.deleteFlag; }
      set { this.deleteFlag = value; }
    }
    #endregion Properties
  }
}