/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 20-06-2008
   Company :  Dai Co   
 */

using DaiCo.Application;
using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace DaiCo.SqlGateway
{
  /// <summary>
  /// This class makes sql query for the transaction
  /// </summary>
  public class ListObjectSqlFactory : IQuery
  {
    #region Fields
    private IObject obj;
    private DBParameter[] arrParameters;
    private string strWhereClause;
    #endregion

    #region Constructor
    public ListObjectSqlFactory(IObject obj, string whereClause, params DBParameter[] arrParameters)
    {
      this.obj = obj;
      this.strWhereClause = whereClause;
      this.arrParameters = arrParameters;
    }
    #endregion Constructor

    #region CommandText
    // make sql query
    public string CommandText
    {
      get
      {
        string listQuery = string.Format(@" SELECT * FROM {0} {1}", obj.GetTableName(), this.strWhereClause);
        // end generate load query
        return listQuery;
      }
    }
    #endregion CommandText

    #region PrepareParameters
    // make parameters
    public void PrepareParameters(IDbCommand cm)
    {
      // set param for Input Parameters
      if (this.arrParameters != null)
      {
        foreach (DBParameter param in this.arrParameters)
        {
          DataParameter.AddParameter(cm, param.ParameterName, param.DbType, param.Value);
        }
      }
    }
    #endregion PrepareParameters

    #region ReadResult

    // get result from the query
    public object ReadResult(object value)
    {
      IDataReader dre = (IDataReader)value;
      FieldInfo[] myFieldInfo;

      //get Type of object
      Type myType = obj.GetType();

      // Get the type and fields of FieldInfoClass.
      myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
      IList result = new ArrayList();
      while (dre.Read())
      {
        this.SetValueForObject(dre, myFieldInfo);
        IObject iobj = (IObject)obj.Clone();

        result.Add(iobj);
      }
      return result;
    }
    #endregion ReadResult

    #region Methods
    // Set value from database for fields of the object
    private void SetValueForObject(IDataReader dre, FieldInfo[] myFieldInfo)
    {
      for (int i = 0; i < myFieldInfo.Length; i++)
      {
        //get type of field        
        Type type = myFieldInfo[i].FieldType;
        string strParamName = myFieldInfo[i].Name;

        // convert value to DB value
        object value = DBConvert.ParseToDBValue(myFieldInfo[i].GetValue(obj));

        // if type is char
        if (type == typeof(System.Char))
        {
          myFieldInfo[i].SetValue(obj, DBConvert.ParseDBToChar(dre, strParamName));
        }
        // if type is string
        else if (type == typeof(System.String))
        {
          myFieldInfo[i].SetValue(obj, DBConvert.ParseDBToString(dre, strParamName));
        }
        // if type is smallint
        else if (type == typeof(System.Int16))
        {
          myFieldInfo[i].SetValue(obj, DBConvert.ParseDBToSmallInt(dre, strParamName));
        }
        // if type is int
        else if (type == typeof(System.Int32))
        {
          myFieldInfo[i].SetValue(obj, DBConvert.ParseDBToInt(dre, strParamName));
        }
        // if type is long
        else if (type == typeof(System.Int64))
        {
          myFieldInfo[i].SetValue(obj, DBConvert.ParseDBToLong(dre, strParamName));
        }
        // if type is decimal
        else if (type == typeof(System.Decimal))
        {
          myFieldInfo[i].SetValue(obj, DBConvert.ParseDBToDecimal(dre, strParamName));
        }
        // if type is double
        else if (type == typeof(System.Double))
        {
          myFieldInfo[i].SetValue(obj, DBConvert.ParseDBToDouble(dre, strParamName));
        }
        // if type is datetime
        else if (type == typeof(System.DateTime))
        {
          myFieldInfo[i].SetValue(obj, DBConvert.ParseDBToDateTime(dre, strParamName));
        }
      }
    }
    #endregion Methods
  }
}
