/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 07/10/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class CSDItemRoom : IObject
  {
    #region Fields
    private long pid;
    private string itemCode;
    private long roomPid;
    private string description;
    #endregion Fields

    #region Constructors
    public CSDItemRoom()
    {
      this.pid = long.MinValue;
      this.itemCode = string.Empty;
      this.roomPid = long.MinValue;
      this.description = string.Empty;
    }
    public object Clone()
    {
      CSDItemRoom obj = new CSDItemRoom();
      obj.Pid = this.pid;
      obj.ItemCode = this.itemCode;
      obj.RoomPid = this.roomPid;
      obj.Description = this.description;
      return obj;
    }
    public string GetTableName()
    {
      return "TblCSDItemRoom";
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
    public long RoomPid
    {
      get { return this.roomPid; }
      set { this.roomPid = value; }
    }
    public string Description
    {
      get { return this.description; }
      set { this.description = value; }
    }
    #endregion Properties
  }
}