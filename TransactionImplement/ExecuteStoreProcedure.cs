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
  class ExecuteStoreProcedure : ITransaction
  {

    #region Fields
    private string storeProcedureName;
    private int commandTimeout = int.MinValue;
    private DBParameter[] inputParameters;
    private DBParameter[] outputParameters;
    private SqlFactory factory;
    private object result;
    #endregion

    #region Constructor
    // init 
    public ExecuteStoreProcedure(string storeProcedureName, DBParameter[] inputParameters, DBParameter[] outputParameters)
    {
      this.storeProcedureName = storeProcedureName;
      this.inputParameters = inputParameters;
      this.outputParameters = outputParameters;
      this.commandTimeout = int.MinValue;
    }
    public ExecuteStoreProcedure(string storeProcedureName, DBParameter[] inputParameters, DBParameter[] outputParameters, int commandTimeout)
    {
      this.storeProcedureName = storeProcedureName;
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

        // Make command
        IDbCommand cm = this.factory.MakeCommand(this.storeProcedureName);
        if (this.commandTimeout != int.MinValue)
        {
          cm.CommandTimeout = this.commandTimeout;
        }
        //init command type
        cm.CommandType = CommandType.StoredProcedure;
        // Set parameters
        // Input Parameters
        if (this.inputParameters != null && this.inputParameters.Length > 0)
        {
          foreach (DBParameter param in this.inputParameters)
          {
            if (param != null)
            {
              DataParameter.AddParameter(cm, param.ParameterName, param.DbType, param.Value);
            }
          }
        }
        // Output Parameters
        if (this.outputParameters != null && this.outputParameters.Length > 0)
        {
          foreach (DBParameter param in this.outputParameters)
          {
            if (param != null)
            {
              DataParameter.AddParameter(cm, ParameterDirection.Output, param.ParameterName, param.DbType, param.Size, param.Value);
            }
          }
        }

        // execute
        this.result = this.factory.ExecuteScalar(cm);

        // return out parameters. Notice: must close data reader before get output parameters
        if (this.outputParameters != null && this.outputParameters.Length > 0)
        {
          foreach (DBParameter outParam in this.outputParameters)
          {
            if (outParam != null)
            {
              IDbDataParameter param = (IDbDataParameter)cm.Parameters[outParam.ParameterName];
              outParam.Value = param.Value;
              outParam.DbType = param.DbType;
              outParam.Size = param.Size;
            }
          }
        }
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
