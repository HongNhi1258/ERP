/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 06/09/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class PLNSubconItemPRDetailsPlan : IObject
  {
    #region Fields
    private long pid;
    private long subconDetailPid;
    private int qty;
    private DateTime subplan;
    #endregion Fields

    #region Constructors
    public PLNSubconItemPRDetailsPlan()
    {
      this.pid = long.MinValue;
      this.subconDetailPid = long.MinValue;
      this.qty = int.MinValue;
      this.subplan = DateTime.MinValue;
    }
    public object Clone()
    {
      PLNSubconItemPRDetailsPlan obj = new PLNSubconItemPRDetailsPlan();
      obj.Pid = this.pid;
      obj.SubconDetailPid = this.subconDetailPid;
      obj.Qty = this.qty;
      obj.Subplan = this.subplan;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPLNSubconItemPRDetailsPlan";
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
    public long SubconDetailPid
    {
      get { return this.subconDetailPid; }
      set { this.subconDetailPid = value; }
    }
    public int Qty
    {
      get { return this.qty; }
      set { this.qty = value; }
    }
    public DateTime Subplan
    {
      get { return this.subplan; }
      set { this.subplan = value; }
    }
    #endregion Properties
  }
}