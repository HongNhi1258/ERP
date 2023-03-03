/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 23-06-2008
   Company :  Dai Co   
 */

using DaiCo.Application;
using DaiCo.SqlGateway;
using System;
using System.Data;
using System.Data.SqlClient;
namespace DaiCo.TransactionImplement
{
  class SearchDataAdapterTransaction : ITransaction
  {
    #region Fields
    private CommandType commandType;
    private string commandText;
    private int commandTimeout = int.MinValue;
    private DBParameter[] inputParameters;
    private DBParameter[] outputParameters;
    private SqlFactory factory;
    private object result;

    #endregion

    #region Constructor
    // init 
    public SearchDataAdapterTransaction(CommandType commandType, string commandText, DBParameter[] inputParameters, DBParameter[] outputParameters)
    {
      this.commandType = commandType;
      this.commandText = commandText;
      this.inputParameters = inputParameters;
      this.outputParameters = outputParameters;
      this.commandTimeout = int.MinValue;
    }
    public SearchDataAdapterTransaction(CommandType commandType, string commandText, DBParameter[] inputParameters, DBParameter[] outputParameters, int commandTimeout)
    {
      this.commandType = commandType;
      this.commandText = commandText;
      this.inputParameters = inputParameters;
      this.outputParameters = outputParameters;
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

        IQuery query = new SearchDataAdapterSqlFactory(this.inputParameters, this.outputParameters);

        // Make command
        IDbCommand cm = this.factory.MakeCommand(this.commandText);
        if (this.commandTimeout != int.MinValue)
        {
          cm.CommandTimeout = this.commandTimeout;
        }
        //init command type
        if (this.commandType == CommandType.StoredProcedure)
        {
          cm.CommandType = CommandType.StoredProcedure;
        }
        else if (this.commandType == CommandType.Text)
        {
          cm.CommandType = CommandType.Text;
        }
        // Set parameters
        query.PrepareParameters(cm);
        // Get SqlDataAdapter
        SqlDataAdapter dataAdapter = this.factory.ExecuteDataAdapter(cm);

        // return out parameters.
        if (this.outputParameters != null)
        {
          dataAdapter.SelectCommand.ExecuteNonQuery();
          foreach (DBParameter outParam in this.outputParameters)
          {
            IDbDataParameter param = (IDbDataParameter)cm.Parameters[outParam.ParameterName];
            outParam.Value = param.Value;
            outParam.DbType = param.DbType;
            outParam.Size = param.Size;
          }
        }
        this.result = dataAdapter;
      }
      catch (Exception e)
      {
        this.result = null;
        //throw to trace log
        throw new TransactionException("SearchDataAdapterTransaction", e);
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
