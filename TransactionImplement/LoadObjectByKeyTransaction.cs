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
  /// Summary description for LoadObjectTransaction.
  /// </summary>
  public class LoadObjectByKeyTransaction : ITransaction
  {
    #region Fields

    private IObject obj;
    private SqlFactory factory;
    private object result;

    #endregion

    #region Constructor
    // init 
    public LoadObjectByKeyTransaction(IObject obj)
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
        // get sql query
        IQuery query = new LoadObjectSqlFactory(this.obj);

        // Make command
        IDbCommand cm = this.factory.MakeCommand(query.CommandText);

        // Set parameters
        query.PrepareParameters(cm);

        // execute query          
        IDataReader dre = this.factory.ExecuteReader(cm);

        // read data to object
        result = query.ReadResult(dre);

        // close data reader
        dre.Close();
      }
      catch (Exception e)
      {
        //throw to trace log
        throw new TransactionException("LoadObjectByKeyTransaction", e);
      }
    }
    public object Result
    {
      //result as true or false
      get { return this.result; }
    }
    #endregion

    #region Methods
    #endregion
  }
}
