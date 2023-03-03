/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 09/03/2011
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class PURStaffGroupDetail : IObject
  {
    #region Fields
    private long pid;
    private long group;
    private int employee;
    #endregion Fields

    #region Constructors
    public PURStaffGroupDetail()
    {
      this.pid = long.MinValue;
      this.group = long.MinValue;
      this.employee = int.MinValue;
    }
    public object Clone()
    {
      PURStaffGroupDetail obj = new PURStaffGroupDetail();
      obj.Pid = this.pid;
      obj.Group = this.group;
      obj.Employee = this.employee;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPURStaffGroupDetail";
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
    public long Group
    {
      get { return this.group; }
      set { this.group = value; }
    }
    public int Employee
    {
      get { return this.employee; }
      set { this.employee = value; }
    }
    #endregion Properties
  }
}