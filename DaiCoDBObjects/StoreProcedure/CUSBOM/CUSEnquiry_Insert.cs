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
  public class CUSEnquiry_Insert : IStoreObject
  {
    #region Fields
    private string customerEnquiryNo;
    private long customerPid;
    private DateTime orderDate;
    private int type;
    private int cancelFlag;
    private int keep;
    private int status;
    private string remark;
    private int createBy;
    private long result;
    #endregion Fields

    #region Constructors
    public CUSEnquiry_Insert()
    {
      this.customerEnquiryNo = string.Empty;
      this.customerPid = long.MinValue;
      this.orderDate = DateTime.MinValue;
      this.type = int.MinValue;
      this.cancelFlag = int.MinValue;
      this.keep = int.MinValue;
      this.status = int.MinValue;
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
      if (this.CustomerEnquiryNo != string.Empty)
      {
        DataParameter.AddParameter(cm, "@CustomerEnquiryNo", DbType.AnsiString, 20, this.CustomerEnquiryNo);
      }
      if (this.CustomerPid != long.MinValue)
      {
        DataParameter.AddParameter(cm, "@CustomerPid", DbType.Int64, this.CustomerPid);
      }
      if (this.OrderDate != DateTime.MinValue)
      {
        DataParameter.AddParameter(cm, "@OrderDate", DbType.DateTime, this.OrderDate);
      }
      if (this.Type != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Type", DbType.Int32, this.Type);
      }
      if (this.CancelFlag != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@CancelFlag", DbType.Int32, this.CancelFlag);
      }
      if (this.Keep != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Keep", DbType.Int32, this.Keep);
      }
      if (this.Status != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Status", DbType.Int32, this.Status);
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
      return "spCUSEnquiry_Insert";
    }
    #endregion Constructors

    #region Properties

    public string CustomerEnquiryNo
    {
      get { return this.customerEnquiryNo; }
      set { this.customerEnquiryNo = value; }
    }
    public long CustomerPid
    {
      get { return this.customerPid; }
      set { this.customerPid = value; }
    }
    public DateTime OrderDate
    {
      get { return this.orderDate; }
      set { this.orderDate = value; }
    }
    public int Type
    {
      get { return this.type; }
      set { this.type = value; }
    }
    public int CancelFlag
    {
      get { return this.cancelFlag; }
      set { this.cancelFlag = value; }
    }
    public int Keep
    {
      get { return this.keep; }
      set { this.keep = value; }
    }
    public int Status
    {
      get { return this.status; }
      set { this.status = value; }
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