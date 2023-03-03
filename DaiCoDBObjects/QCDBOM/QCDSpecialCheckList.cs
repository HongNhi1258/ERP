/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 23/09/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class QCDSpecialCheckList : IObject
  {
    #region Fields
    private long pid;
    private string itemCode;
    private int revision;
    private long checkListPid;
    #endregion Fields

    #region Constructors
    public QCDSpecialCheckList()
    {
      this.pid = long.MinValue;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.checkListPid = long.MinValue;
    }
    public object Clone()
    {
      QCDSpecialCheckList obj = new QCDSpecialCheckList();
      obj.Pid = this.pid;
      obj.ItemCode = this.itemCode;
      obj.Revision = this.revision;
      obj.CheckListPid = this.checkListPid;
      return obj;
    }
    public string GetTableName()
    {
      return "TblQCDSpecialCheckList";
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
    public string ItemCode
    {
      get { return this.itemCode; }
      set { this.itemCode = value; }
    }
    public int Revision
    {
      get { return this.revision; }
      set { this.revision = value; }
    }
    public long CheckListPid
    {
      get { return this.checkListPid; }
      set { this.checkListPid = value; }
    }
    #endregion Properties
  }
}