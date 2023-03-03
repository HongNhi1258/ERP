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
  public class BOMCarcassComponentProcess : IObject
  {
    #region Fields
    private long pid;
    private long componentPid;
    private int ordinal;
    private string description;
    private string descriptionVN;
    private long workStation;
    private string profilePid;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private int checkPoint;
    #endregion Fields

    #region Constructors
    public BOMCarcassComponentProcess()
    {
      this.pid = long.MinValue;
      this.componentPid = long.MinValue;
      this.ordinal = int.MinValue;
      this.description = string.Empty;
      this.descriptionVN = string.Empty;
      this.workStation = long.MinValue;
      this.profilePid = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.checkPoint = int.MinValue;
    }
    public object Clone()
    {
      BOMCarcassComponentProcess obj = new BOMCarcassComponentProcess();
      obj.Pid = this.pid;
      obj.ComponentPid = this.componentPid;
      obj.Ordinal = this.ordinal;
      obj.Description = this.description;
      obj.DescriptionVN = this.descriptionVN;
      obj.WorkStation = this.workStation;
      obj.ProfilePid = this.profilePid;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.CheckPoint = this.checkPoint;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMCarcassComponentProcess";
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
    public long ComponentPid
    {
      get { return this.componentPid; }
      set { this.componentPid = value; }
    }
    public int Ordinal
    {
      get { return this.ordinal; }
      set { this.ordinal = value; }
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
    public long WorkStation
    {
      get { return this.workStation; }
      set { this.workStation = value; }
    }
    public string ProfilePid
    {
      get { return this.profilePid; }
      set { this.profilePid = value; }
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
    public int CheckPoint
    {
      get { return this.checkPoint; }
      set { this.checkPoint = value; }
    }
    #endregion Properties
  }
}