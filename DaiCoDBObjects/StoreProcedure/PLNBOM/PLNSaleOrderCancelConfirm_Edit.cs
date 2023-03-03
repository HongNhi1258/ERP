/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 24/06/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System.Data;
using System.Data.SqlClient;

namespace DaiCo.Objects
{
  public class PLNSaleOrderCancelConfirm_Edit : IStoreObject
  {
    #region Fields
    private long pid;
    private long poCancelDetailPid;
    private long saleOrderDetailPid;
    private int qty;
    private int user;
    private long result;
    #endregion Fields

    #region Constructors
    public PLNSaleOrderCancelConfirm_Edit()
    {
      this.pid = long.MinValue;
      this.poCancelDetailPid = long.MinValue;
      this.saleOrderDetailPid = long.MinValue;
      this.qty = int.MinValue;
      this.user = int.MinValue;
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
      if (this.PoCancelDetailPid != long.MinValue)
      {
        DataParameter.AddParameter(cm, "@PoCancelDetailPid", DbType.Int64, this.PoCancelDetailPid);
      }
      if (this.SaleOrderDetailPid != long.MinValue)
      {
        DataParameter.AddParameter(cm, "@SaleOrderDetailPid", DbType.Int64, this.SaleOrderDetailPid);
      }
      if (this.Qty != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Qty", DbType.Int32, this.Qty);
      }
      if (this.User != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@User", DbType.Int32, this.User);
      }
      DataParameter.AddParameter(cm, ParameterDirection.Output, "@Result", DbType.Int64, this.Result);
    }
    public string GetStoreName()
    {
      return "spPLNSaleOrderCancelConfirm_Edit";
    }
    #endregion Constructors

    #region Properties

    public long Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public long PoCancelDetailPid
    {
      get { return this.poCancelDetailPid; }
      set { this.poCancelDetailPid = value; }
    }
    public long SaleOrderDetailPid
    {
      get { return this.saleOrderDetailPid; }
      set { this.saleOrderDetailPid = value; }
    }
    public int Qty
    {
      get { return this.qty; }
      set { this.qty = value; }
    }
    public int User
    {
      get { return this.user; }
      set { this.user = value; }
    }
    public long Result
    {
      get { return this.result; }
      set { this.result = value; }
    }
    #endregion Properties
  }
}