/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 21/06/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System.Data;
using System.Data.SqlClient;

namespace DaiCo.Objects
{
  public class CUSEnquiryConfirmDetail_Update : IStoreObject
  {
    #region Fields
    private long pid;
    private int keep;
    private int keepDays;
    private int expire;
    private long result;
    #endregion Fields

    #region Constructors
    public CUSEnquiryConfirmDetail_Update()
    {
      this.pid = long.MinValue;
      this.keep = int.MinValue;
      this.keepDays = int.MinValue;
      this.expire = int.MinValue;
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
      if (this.Keep != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Keep", DbType.Int32, this.Keep);
      }
      if (this.KeepDays != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@KeepDays", DbType.Int32, this.KeepDays);
      }
      if (this.Expire != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Expire", DbType.Int32, this.Expire);
      }
      DataParameter.AddParameter(cm, ParameterDirection.Output, "@Result", DbType.Int64, this.Result);
    }
    public string GetStoreName()
    {
      return "spCUSEnquiryConfirmDetail_Update";
    }
    #endregion Constructors

    #region Properties

    public long Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public int Keep
    {
      get { return this.keep; }
      set { this.keep = value; }
    }
    public int KeepDays
    {
      get { return this.keepDays; }
      set { this.keepDays = value; }
    }
    public int Expire
    {
      get { return this.expire; }
      set { this.expire = value; }
    }
    public long Result
    {
      get { return this.result; }
      set { this.result = value; }
    }
    #endregion Properties
  }
}