/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 22/09/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class QCDCheckLists : IObject
  {
    #region Fields
    private long pid;
    private long groupPid;
    private long checkListPid;
    #endregion Fields

    #region Constructors
    public QCDCheckLists()
    {
      this.pid = long.MinValue;
      this.groupPid = long.MinValue;
      this.checkListPid = long.MinValue;
    }
    public object Clone()
    {
      QCDCheckLists obj = new QCDCheckLists();
      obj.Pid = this.pid;
      obj.GroupPid = this.groupPid;
      obj.CheckListPid = this.checkListPid;
      return obj;
    }
    public string GetTableName()
    {
      return "TblQCDCheckLists";
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
    public long GroupPid
    {
      get { return this.groupPid; }
      set { this.groupPid = value; }
    }
    public long CheckListPid
    {
      get { return this.checkListPid; }
      set { this.checkListPid = value; }
    }
    #endregion Properties
  }
}