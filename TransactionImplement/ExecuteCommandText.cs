
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
  class ExecuteCommandText : ITransaction
  {

    #region Fields
    private SqlFactory factory;
    private DBParameter[] parameters;
    private string commandText;
    private object result;
    #endregion

    #region Constructor
    // init 
    public ExecuteCommandText(string commandText, params DBParameter[] parameters)
    {
      this.commandText = commandText;
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

        // Make command
        IDbCommand cm = this.factory.MakeCommand(commandText);

        // Set parameters
        // set param for Input Parameters
        if (this.parameters != null)
        {
          foreach (DBParameter param in this.parameters)
          {
            if (param != null)
            {
              DataParameter.AddParameter(cm, param.ParameterName, param.DbType, param.Value);
            }
          }
        }

        // execute query          
        this.result = this.factory.ExecuteScalar(cm);
      }
      catch (Exception e)
      {
        //if process happen error, rool back
        this.result = null;
        //throw to trace log
        throw new TransactionException("ExecuteCommandText", e);
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
