/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 09/03/2011
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class PURStaffGroup : IObject
  {
    #region Fields
    private long pid;
    private string groupName;
    private int leaderGroup;
    private int deleteFlg;
    #endregion Fields

    #region Constructors
    public PURStaffGroup()
    {
      this.pid = long.MinValue;
      this.groupName = string.Empty;
      this.leaderGroup = int.MinValue;
      this.deleteFlg = int.MinValue;
    }
    public object Clone()
    {
      PURStaffGroup obj = new PURStaffGroup();
      obj.Pid = this.pid;
      obj.GroupName = this.groupName;
      obj.LeaderGroup = this.leaderGroup;
      obj.DeleteFlg = this.deleteFlg;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPURStaffGroup";
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
    public string GroupName
    {
      get { return this.groupName; }
      set { this.groupName = value; }
    }
    public int LeaderGroup
    {
      get { return this.leaderGroup; }
      set { this.leaderGroup = value; }
    }
    public int DeleteFlg
    {
      get { return this.deleteFlg; }
      set { this.deleteFlg = value; }
    }
    #endregion Properties
  }
}