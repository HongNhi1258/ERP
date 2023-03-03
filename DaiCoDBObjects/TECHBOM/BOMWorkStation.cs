/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 24.07.2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class BOMWorkStation : IObject
  {
    #region Fields
    private long pid;
    private string station;
    private string stationVN;
    private string team;
    private int checkPoint;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    #endregion Fields

    #region Constructors
    public BOMWorkStation()
    {
      this.pid = long.MinValue;
      this.station = string.Empty;
      this.stationVN = string.Empty;
      this.team = string.Empty;
      this.checkPoint = int.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
    }
    public object Clone()
    {
      BOMWorkStation obj = new BOMWorkStation();
      obj.Pid = this.pid;
      obj.Station = this.station;
      obj.StationVN = this.stationVN;
      obj.Team = this.team;
      obj.CheckPoint = this.checkPoint;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMWorkStation";
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
    public string Station
    {
      get { return this.station; }
      set { this.station = value; }
    }
    public string StationVN
    {
      get { return this.stationVN; }
      set { this.stationVN = value; }
    }
    public string Team
    {
      get { return this.team; }
      set { this.team = value; }
    }
    public int CheckPoint
    {
      get { return this.checkPoint; }
      set { this.checkPoint = value; }
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