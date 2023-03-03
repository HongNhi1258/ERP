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
  public class BOMGroupProcess : IObject
  {
    #region Fields
    private int pid;
    private string groupProcessName;
    private string groupProcessNameVN;
    private int deleteFlag;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public BOMGroupProcess()
    {
      this.pid = int.MinValue;
      this.groupProcessName = string.Empty;
      this.groupProcessNameVN = string.Empty;
      this.deleteFlag = int.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      BOMGroupProcess obj = new BOMGroupProcess();
      obj.Pid = this.pid;
      obj.GroupProcessName = this.groupProcessName;
      obj.GroupProcessNameVN = this.groupProcessNameVN;
      obj.DeleteFlag = this.deleteFlag;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMGroupProcess";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Pid" };
    }
    #endregion Constructors

    #region Properties

    public int Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public string GroupProcessName
    {
      get { return this.groupProcessName; }
      set { this.groupProcessName = value; }
    }
    public string GroupProcessNameVN
    {
      get { return this.groupProcessNameVN; }
      set { this.groupProcessNameVN = value; }
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
    #endregion Properties
  }
}