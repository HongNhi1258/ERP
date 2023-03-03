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
  public class CUSEnquiryDetail_Insert : IStoreObject
  {
    #region Fields
    private long enquiryPid;
    private string itemCode;
    private int revision;
    private int qty;
    private DateTime requestDate;
    private string remark;
    private int createBy;
    private long result;
    #endregion Fields

    #region Constructors
    public CUSEnquiryDetail_Insert()
    {
      this.enquiryPid = long.MinValue;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.qty = int.MinValue;
      this.requestDate = DateTime.MinValue;
      this.remark = string.Empty;
      this.createBy = int.MinValue;
      this.result = long.MinValue;
    }
    public void GetOutputValue(IDbCommand cm)
    {
      this.Result = DBConvert.ParseLong(((SqlParameter)(cm.Parameters["@Result"])).Value.ToString());
    }
    public void PrepareParameters(IDbCommand cm)
    {
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
      if (this.CreateBy != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@CreateBy", DbType.Int32, this.CreateBy);
      }
      DataParameter.AddParameter(cm, ParameterDirection.Output, "@Result", DbType.Int64, this.Result);
    }
    public string GetStoreName()
    {
      return "spCUSEnquiryDetail_Insert";
    }
    #endregion Constructors

    #region Properties

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
    public int CreateBy
    {
      get { return this.createBy; }
      set { this.createBy = value; }
    }
    public long Result
    {
      get { return this.result; }
      set { this.result = value; }
    }
    #endregion Properties
  }
}