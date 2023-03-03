/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 21-06-2008
   Company :  Dai Co   
 */
using DaiCo.Application;
using DaiCo.SqlGateway;
using System;
using System.Data;

namespace DaiCo.TransactionImplement
{
  /// <summary>
  /// Summary description for DeleteObjectTransaction.
  /// </summary>
  public class DeleteObjectByKeyTransaction : ITransaction
  {
    #region Fields
    private IObject obj;
    private SqlFactory factory;
    private object result;

    #endregion

    #region Constructor
    // init 
    public DeleteObjectByKeyTransaction(IObject obj)
    {
      this.obj = obj;
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
        //get sql query
        IQuery query = new DeleteObjectSqlFactory(this.obj);

        // Make command
        IDbCommand cm = this.factory.MakeCommand(query.CommandText);

        // Set parameters
        query.PrepareParameters(cm);

        // execute query          
        this.factory.ExecuteNonQuery(cm);
        result = true;
      }
      catch (Exception e)
      {

        result = false;
        //throw to trace log
        throw new TransactionException("DeleteObjectByKeyTransaction", e);
      }
    }

    public object Result
    {
      //result as true or false
      get { return this.result; }
    }
    #endregion
  }
}
