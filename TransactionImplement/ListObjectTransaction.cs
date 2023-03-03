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
  class ListObjectTransaction : ITransaction
  {
    #region Fields
    private IObject obj;
    private int typeOutput;
    private SqlFactory factory;
    private DBParameter[] parameters;
    private string whereClause;
    private object result;

    #endregion

    #region Constructor
    // init 
    public ListObjectTransaction(IObject obj, int typeOutput, string whereClause, params DBParameter[] parameters)
    {
      this.obj = obj;
      this.typeOutput = typeOutput;
      this.whereClause = whereClause;
      this.parameters = parameters;
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
        // get the query
        IQuery query = new ListObjectSqlFactory(this.obj, this.whereClause, this.parameters);

        // Make command
        IDbCommand cm = this.factory.MakeCommand(query.CommandText);

        // Set Parameters
        query.PrepareParameters(cm);
        if (typeOutput == 1)
        {
          // execute query          
          IDataReader dre = this.factory.ExecuteReader(cm);

          // read data to object

          this.result = query.ReadResult(dre);

          // close data reader
          dre.Close();
        }
        else
        {
          this.result = this.factory.ExecuteDataAdapter(cm);
        }
      }
      catch (Exception e)
      {
        //throw to trace log
        throw new TransactionException("ListObjectTransaction", e);
      }
    }
    public object Result
    {
      //result as a data reader
      get { return this.result; }
    }
    #endregion

    #region Methods
    #endregion
  }
}
