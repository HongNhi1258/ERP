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
  public class PLNSubconItemPRDetailsMCH : IObject
  {
    #region Fields
    private long pid;
    private long subconDetailPid;
    private string componentCode;
    private int qty;
    private DateTime mCH1;
    private DateTime mCH2;
    #endregion Fields

    #region Constructors
    public PLNSubconItemPRDetailsMCH()
    {
      this.pid = long.MinValue;
      this.subconDetailPid = long.MinValue;
      this.componentCode = string.Empty;
      this.qty = int.MinValue;
      this.mCH1 = DateTime.MinValue;
      this.mCH2 = DateTime.MinValue;
    }
    public object Clone()
    {
      PLNSubconItemPRDetailsMCH obj = new PLNSubconItemPRDetailsMCH();
      obj.Pid = this.pid;
      obj.SubconDetailPid = this.subconDetailPid;
      obj.ComponentCode = this.componentCode;
      obj.Qty = this.qty;
      obj.MCH1 = this.mCH1;
      obj.MCH2 = this.mCH2;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPLNSubconItemPRDetailsMCH";
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
    public string ComponentCode
    {
      get { return this.componentCode; }
      set { this.componentCode = value; }
    }
    public int Qty
    {
      get { return this.qty; }
      set { this.qty = value; }
    }
    public DateTime MCH1
    {
      get { return this.mCH1; }
      set { this.mCH1 = value; }
    }
    public DateTime MCH2
    {
      get { return this.mCH2; }
      set { this.mCH2 = value; }
    }
    #endregion Properties
  }
}