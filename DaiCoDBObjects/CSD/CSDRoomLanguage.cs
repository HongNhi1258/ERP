/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 07/10/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class CSDRoomLanguage : IObject
  {
    #region Fields
    private long pid;
    private long roomPid;
    private long languagePid;
    private string roomName;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public CSDRoomLanguage()
    {
      this.pid = long.MinValue;
      this.roomPid = long.MinValue;
      this.languagePid = long.MinValue;
      this.roomName = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      CSDRoomLanguage obj = new CSDRoomLanguage();
      obj.Pid = this.pid;
      obj.RoomPid = this.roomPid;
      obj.LanguagePid = this.languagePid;
      obj.RoomName = this.roomName;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblCSDRoomLanguage";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Pid", "RoomPid", "LanguagePid" };
    }
    #endregion Constructors

    #region Properties

    public long Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public long RoomPid
    {
      get { return this.roomPid; }
      set { this.roomPid = value; }
    }
    public long LanguagePid
    {
      get { return this.languagePid; }
      set { this.languagePid = value; }
    }
    public string RoomName
    {
      get { return this.roomName; }
      set { this.roomName = value; }
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