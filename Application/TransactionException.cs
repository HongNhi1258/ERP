/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 13-08-2008
   Company :  Dai Co   
 */

using System;
namespace DaiCo.Application
{

  public class TransactionException : Error
  {

    #region Fields
    public string transactionName;
    #endregion Fields

    #region Contructors
    public TransactionException() : base() { }
    public TransactionException(string transactionName)
      : base()
    {
      this.transactionName = transactionName;
    }
    public TransactionException(string transactionName, string message)
      : base(message)
    {
      this.transactionName = transactionName;
    }
    public TransactionException(string transactionName, string message, Exception inner)
      : base(message, inner)
    {
      this.transactionName = transactionName;
    }
    public TransactionException(string message, Exception inner) : base(message, inner) { }
    #endregion Contructors

    #region Properties
    public override string Message
    {
      get
      {
        string result;
        result = base.Message;
        if (transactionName != null && transactionName.Length > 0)
        {
          result = string.Concat(transactionName, ": ", result);
        }
        return result;
      }
    }

    #endregion Properties

    #region Methods
    #endregion Methods
  }
}
