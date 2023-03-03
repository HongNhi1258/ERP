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
  public class BOMMachineDetails : IObject
  {
    #region Fields
    private long pid;
    private long machinePid;
    private string machineCode;
    private string machineNameEN;
    private string machineNameVN;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private int deleteFlag;
    #endregion Fields

    #region Constructors
    public BOMMachineDetails()
    {
      this.pid = long.MinValue;
      this.machinePid = long.MinValue;
      this.machineCode = string.Empty;
      this.machineNameEN = string.Empty;
      this.machineNameVN = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.deleteFlag = int.MinValue;
    }
    public object Clone()
    {
      BOMMachineDetails obj = new BOMMachineDetails();
      obj.Pid = this.pid;
      obj.MachinePid = this.machinePid;
      obj.MachineCode = this.machineCode;
      obj.MachineNameEN = this.machineNameEN;
      obj.MachineNameVN = this.machineNameVN;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.DeleteFlag = this.deleteFlag;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMMachineDetails";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Pid", "MachineCode" };
    }
    #endregion Constructors

    #region Properties

    public long Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public long MachinePid
    {
      get { return this.machinePid; }
      set { this.machinePid = value; }
    }
    public string MachineCode
    {
      get { return this.machineCode; }
      set { this.machineCode = value; }
    }
    public string MachineNameEN
    {
      get { return this.machineNameEN; }
      set { this.machineNameEN = value; }
    }
    public string MachineNameVN
    {
      get { return this.machineNameVN; }
      set { this.machineNameVN = value; }
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