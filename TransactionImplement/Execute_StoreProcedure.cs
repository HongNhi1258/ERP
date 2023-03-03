/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 21-06-2008
   Company :  Dai Co   
 */
using DaiCo.Application;
using System;
using System.Data;

namespace DaiCo.TransactionImplement
{
  class Execute_StoreProcedure : ITransaction
  {

    #region Fields
    private IStoreObject obj = null;
    private int commandTimeout = int.MinValue;
    private SqlFactory factory;
    private object result;
    #endregion

    #region Constructor
    // init 
    public Execute_StoreProcedure(IStoreObject obj)
    {
      this.obj = obj;
      this.commandTimeout = int.MinValue;
    }
    public Execute_StoreProcedure(IStoreObject obj, int commandTimeout)
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
        // Make command
        IDbCommand cm = this.factory.MakeCommand(obj.GetStoreName());
        if (this.commandTimeout != int.MinValue)
        {
          cm.CommandTimeout = this.commandTimeout;
        }
        //init command type
        cm.CommandType = CommandType.StoredProcedure;
        obj.PrepareParameters(cm);
        // execute
        this.result = this.factory.ExecuteScalar(cm);
        obj.GetOutputValue(cm);
      }
      catch (Exception e)
      {
        //if process happen error, rool back
        this.result = null;
        //throw to trace log
        throw new TransactionException("ExecuteStoreProcedure", e);
      }
    }
    public object Result
    {
      //result as a data reader
      get { return this.result; }
    }
    #endregion

  }
}
