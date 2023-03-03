/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 21/06/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DaiCo.Objects
{
  public class CUSEnquiryDetail_Update : IStoreObject
  {
    #region Fields
    private long pid;
    private long enquiryPid;
    private string itemCode;
    private int revision;
    private int qty;
    private DateTime requestDate;
    private string remark;
    private int updateByCSS;
    private long result;
    #endregion Fields

    #region Constructors
    public CUSEnquiryDetail_Update()
    {
      this.pid = long.MinValue;
      this.enquiryPid = long.MinValue;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.qty = int.MinValue;
      this.requestDate = DateTime.MinValue;
      this.remark = string.Empty;
      this.updateByCSS = int.MinValue;
      this.result = long.MinValue;
    }
    public void GetOutputValue(IDbCommand cm)
    {
      this.Result = DBConvert.ParseLong(((SqlParameter)(cm.Parameters["@Result"])).Value.ToString());
    }
    public void PrepareParameters(IDbCommand cm)
    {
      if (this.Pid != long.MinValue)
      {
        DataParameter.AddParameter(cm, "@Pid", DbType.Int64, this.Pid);
      }
      if (this.EnquiryPid != long.MinValue)
      {
        DataParameter.AddParameter(cm, "@EnquiryPid", DbType.Int64, this.EnquiryPid);
      }
      if (this.ItemCode != string.Empty)
      {
        DataParameter.AddParameter(cm, "@ItemCode", DbType.AnsiString, 16, this.ItemCode);
      }
      if (this.Revision != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Revision", DbType.Int32, this.Revision);
      }
      if (this.Qty != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Qty", DbType.Int32, this.Qty);
      }
      if (this.RequestDate != DateTime.MinValue)
      {
        DataParameter.AddParameter(cm, "@RequestDate", DbType.DateTime, this.RequestDate);
      }
      if (this.Remark != string.Empty)
      {
        DataParameter.AddParameter(cm, "@Remark", DbType.AnsiString, 4000, this.Remark);
      }
      if (this.UpdateByCSS != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@UpdateByCSS", DbType.Int32, this.UpdateByCSS);
      }
      DataParameter.AddParameter(cm, ParameterDirection.Output, "@Result", DbType.Int64, this.Result);
    }
    public string GetStoreName()
    {
      return "spCUSEnquiryDetail_Update";
    }
    #endregion Constructors

    #region Properties

    public long Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public long EnquiryPid
    {
      get { return this.enquiryPid; }
      set { this.enquiryPid = value; }
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
    public int Qty
    {
      get { return this.qty; }
      set { this.qty = value; }
    }
    public DateTime RequestDate
    {
      get { return this.requestDate; }
      set { this.requestDate = value; }
    }
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
    }
    public int UpdateByCSS
    {
      get { return this.updateByCSS; }
      set { this.updateByCSS = value; }
    }
    public long Result
    {
      get { return this.result; }
      set { this.result = value; }
    }
    #endregion Properties
  }
}