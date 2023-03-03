/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 23-06-2008
   Company :  Dai Co   
 */

using DaiCo.Application;
using System;
using System.Data;
using System.Data.SqlClient;
namespace DaiCo.TransactionImplement
{
  class SearchStoreProcedure : ITransaction
  {
    #region Fields
    private int commandTimeout = int.MinValue;
    private IStoreObject obj = null;
    private SqlFactory factory;
    private object result;

    #endregion

    #region Constructor
    // init 
    public SearchStoreProcedure(IStoreObject obj)
    {
      this.obj = obj;
      this.commandTimeout = int.MinValue;
    }
    public SearchStoreProcedure(IStoreObject obj, int commandTimeout)
    {
      this.obj = obj;
      this.commandTimeout = commandTimeout;
    }
    #endregion

    #region Basic

    // this method is same for all transaction
    public void GetFactory(SqlFactory factory)
    {
      this.factory = factory;
    }
    public void Execute()
    {
      try
      {

        IDbCommand cm = this.factory.MakeCommand(obj.GetStoreName());
        if (this.commandTimeout != int.MinValue)
        {
          cm.CommandTimeout = this.commandTimeout;
        }
        cm.CommandType = CommandType.StoredProcedure;

        // Set parameters
        obj.PrepareParameters(cm);
        // Get SqlDataAdapter
        SqlDataAdapter dataAdapter = this.factory.ExecuteDataAdapter(cm);
        // return out parameters.
        obj.GetOutputValue(cm);
        this.result = dataAdapter;
      }
      catch (Exception e)
      {
        this.result = null;
        //throw to trace log
        throw new TransactionException("SearchStoreProcedure", e);
      }
    }
    public object Result
    {
      //result as a data reader execute when method is Select
      get { return this.result; }
    }
    #endregion
  }
}
