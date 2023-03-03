/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 23-06-2008
   Company :  Dai Co   
 */

using DaiCo.Application;
using System.Data;

namespace DaiCo.SqlGateway
{
  /// <summary>
  /// This class makes sql query for the transaction
  /// </summary>
  public class SearchDataAdapterSqlFactory : IQuery
  {
    #region Fields
    private DBParameter[] arrInParameters;
    private DBParameter[] arrOutParameters;
    #endregion

    #region Constructor
    public SearchDataAdapterSqlFactory(DBParameter[] arrInParameters, DBParameter[] arrOutParameters)
    {
      this.arrInParameters = arrInParameters;
      this.arrOutParameters = arrOutParameters;
    }
    #endregion Constructor

    #region CommandText
    // make sql query
    public string CommandText
    {
      get
      {
        return string.Empty;
      }
    }
    #endregion CommandText

    #region PrepareParameters
    // make parameters
    public void PrepareParameters(IDbCommand cm)
    {
      // set param for Input Parameters
      if (this.arrInParameters != null)
      {
        foreach (DBParameter param in this.arrInParameters)
        {
          if (param != null)
          {
            DataParameter.AddParameter(cm, param.ParameterName, param.DbType, param.Value);
          }
        }
      }
      // set param for Output Parameters
      if (this.arrOutParameters != null)
      {
        foreach (DBParameter param in this.arrOutParameters)
        {
          DataParameter.AddParameter(cm, ParameterDirection.Output, param.ParameterName, param.DbType, param.Size, param.Value);
        }
      }
    }
    #endregion PrepareParameters

    #region ReadResult
    public object ReadResult(object value)
    {
      return null;
    }
    #endregion ReadResult
  }
}