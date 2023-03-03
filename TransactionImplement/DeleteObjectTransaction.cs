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
  public class DeleteObjectTransaction : ITransaction
  {
    #region Fields
    private IObject obj;
    private string[] primaryKeyNames;
    private SqlFactory factory;
    private object result;

    #endregion

    #region Constructor
    // init 
    public DeleteObjectTransaction(IObject obj, params string[] primaryKeyNames)
    {
      this.obj = obj;
      this.primaryKeyNames = primaryKeyNames;
      if (primaryKeyNames.Length == 0)
      {
        throw new TransactionException("DeleteObjectTransaction", "primaryKeyNames in this method is invalid");
      }
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
        IQuery query = new DeleteObjectSqlFactory(this.obj, this.primaryKeyNames);

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
        throw new TransactionException("DeleteObjectTransaction", e);
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
