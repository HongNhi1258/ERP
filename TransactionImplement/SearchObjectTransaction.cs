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
  class SearchObjectTransaction : ITransaction
  {
    #region Fields
    private IObject obj;
    private CommandType commandType;
    private string commandText;
    private DBParameter[] inputParameters;
    private DBParameter[] outputParameters;
    private int comandTimeOut;
    private SqlFactory factory;
    private object result;

    #endregion

    #region Constructor
    // init 
    public SearchObjectTransaction(IObject obj, CommandType commandType, string commandText, DBParameter[] inputParameters, DBParameter[] outputParameters)
    {
      this.obj = obj;
      this.commandType = commandType;
      this.commandText = commandText;
      this.inputParameters = inputParameters;
      this.outputParameters = outputParameters;
      this.comandTimeOut = int.MinValue;
    }
    public SearchObjectTransaction(IObject obj, CommandType commandType, string commandText, DBParameter[] inputParameters, DBParameter[] outputParameters, int comandTimeOut)
    {
      this.obj = obj;
      this.commandType = commandType;
      this.commandText = commandText;
      this.inputParameters = inputParameters;
      this.outputParameters = outputParameters;
      this.comandTimeOut = comandTimeOut;
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

        IQuery query = new SearchObjectSqlFactory(this.obj, this.inputParameters, this.outputParameters);

        // Make command
        IDbCommand cm = this.factory.MakeCommand(this.commandText);
        if (this.comandTimeOut > 0)
        {
          cm.CommandTimeout = this.comandTimeOut;
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

        // execute query          
        IDataReader dre = this.factory.ExecuteReader(cm);

        // read data to object
        this.result = query.ReadResult(dre);

        // close data reader
        dre.Close();

        // return out parameters. Notice: must close data reader before get output parameters
        if (this.outputParameters != null)
        {
          foreach (DBParameter outParam in this.outputParameters)
          {
            IDbDataParameter param = (IDbDataParameter)cm.Parameters[outParam.ParameterName];
            outParam.Value = param.Value;
            outParam.DbType = param.DbType;
            outParam.Size = param.Size;
          }
        }

      }
      catch (Exception e)
      {
        this.result = null;
        //throw to trace log
        throw new TransactionException("SearchObjectTransaction", e);
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
