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
  public class BOMProfile : IObject
  {
    #region Fields
    private long pid;
    private string profileCode;
    private string description;
    private string descriptionVN;
    private int confirm;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public BOMProfile()
    {
      this.pid = long.MinValue;
      this.profileCode = string.Empty;
      this.description = string.Empty;
      this.descriptionVN = string.Empty;
      this.confirm = int.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      BOMProfile obj = new BOMProfile();
      obj.Pid = this.pid;
      obj.ProfileCode = this.profileCode;
      obj.Description = this.description;
      obj.DescriptionVN = this.descriptionVN;
      obj.Confirm = this.confirm;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMProfile";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Pid", "ProfileCode" };
    }
    #endregion Constructors

    #region Properties

    public long Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public string ProfileCode
    {
      get { return this.profileCode; }
      set { this.profileCode = value; }
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