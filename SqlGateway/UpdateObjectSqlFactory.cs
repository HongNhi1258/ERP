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
  public class UpdateObjectSqlFactory : IQuery
  {

    #region Fields
    private IObject obj;
    private IList arrColumns;
    private string identityFieldName;
    private string[] arrPrimaryKeyNames;
    #endregion

    #region Constructor
    public UpdateObjectSqlFactory(IObject obj, IList arrColumns, string identityFieldName, string[] arrPrimaryKeyNames)
    {
      this.obj = obj;
      this.arrColumns = arrColumns;
      this.identityFieldName = identityFieldName;
      this.arrPrimaryKeyNames = arrPrimaryKeyNames;
    }
    public UpdateObjectSqlFactory(IObject obj, IList arrColumns, string identityFieldName)
    {
      this.obj = obj;
      this.arrColumns = arrColumns;
      this.identityFieldName = identityFieldName;
      this.arrPrimaryKeyNames = obj.ObjectKey();
    }
    #endregion Constructor

    #region CommandText
    // make sql query
    public string CommandText
    {
      get
      {
        string strUpdateQuery = string.Empty;

        FieldInfo[] myFieldInfo;

        //get Type of object
        Type myType = this.obj.GetType();

        // Get the type and fields of FieldInfoClass.
        myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        //Generate insert query
        string strUpdateClause = string.Format(@" UPDATE {0} SET", this.obj.GetTableName());
        string strWhereClause = " WHERE ";

        for (int i = 0; i < myFieldInfo.Length; i++)
        {
          // if the field is not identity field, can update
          if (!(identityFieldName != null && identityFieldName.Equals(myFieldInfo[i].Name, StringComparison.OrdinalIgnoreCase)))
          {
            strUpdateClause = string.Format("{0} [{1}] = @{1},", strUpdateClause, myFieldInfo[i].Name);
          }
        }
        strUpdateClause = strUpdateClause.Remove(strUpdateClause.Length - 1, 1);
        // generate where clause
        if (this.arrPrimaryKeyNames != null)
        {
          foreach (string strKeyName in this.arrPrimaryKeyNames)
          {
            strWhereClause = string.Format("{0}[{1}] = @{1} AND ", strWhereClause, strKeyName);
          }
        }
        strWhereClause = strWhereClause.Remove(strWhereClause.Length - 5, 5);
        strUpdateQuery = string.Format("{0}{1}", strUpdateClause, strWhereClause);
        // end generate update query

        return strUpdateQuery;
      }
    }
    #endregion CommandText

    #region PrepareParameters
    // make parameters
    public void PrepareParameters(IDbCommand cm)
    {

      // set parameter
      FieldInfo[] myFieldInfo;

      //get Type of object
      Type myType = this.obj.GetType();

      // Get the type and fields of FieldInfoClass.
      myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

      for (int i = 0; i < myFieldInfo.Length; i++)
      {
        //get type of field        
        Type type = myFieldInfo[i].FieldType;
        // convert value to DB value
        object value = DBConvert.ParseToDBValue(myFieldInfo[i].GetValue(this.obj));
        //set paramname
        this.AddParam(cm, type, myFieldInfo[i].Name, value);
      }
    }
    #endregion PrepareParameters

    #region ReadResult

    // get result from the query
    public object ReadResult(object value)
    {
      return null;
    }
    #endregion ReadResult

    #region Methods

    // Return length of a char, varchar, nvarchar column  
    private int LengthOfStringColumns(IList arrColumns, string strColumnName)
    {
      foreach (DBParameter param in arrColumns)
      {
        if (param.ParameterName.Equals(strColumnName, StringComparison.OrdinalIgnoreCase))
        {
          return (int)param.Value;
        }
      }
      return 0;
    }

    // add prammeter
    private void AddParam(IDbCommand cm, Type type, string strFieldName, object value)
    {
      string strParamName = string.Format("@{0}", strFieldName);

      // if type is char
      if (type == typeof(System.Char))
      {
        DataParameter.AddParameter(cm, strParamName, DbType.AnsiStringFixedLength, LengthOfStringColumns(this.arrColumns, strFieldName), value);
      }
      // if type is string
      else if (type == typeof(System.String))
      {
        int length = LengthOfStringColumns(this.arrColumns, strFieldName);
        if (length <= 4000)
        {
          DataParameter.AddParameter(cm, strParamName, DbType.String, length, value);
        }
        else
        {
          DataParameter.AddParameter(cm, strParamName, DbType.AnsiString, length, value);
        }
      }
      // if type is smallint
      else if (type == typeof(System.Int16))
      {
        DataParameter.AddParameter(cm, strParamName, DbType.Int16, value);
      }
      // if type is int
      else if (type == typeof(System.Int32))
      {
        DataParameter.AddParameter(cm, strParamName, DbType.Int32, value);
      }
      // if type is long
      else if (type == typeof(System.Int64))
      {
        DataParameter.AddParameter(cm, strParamName, DbType.Int64, value);
      }
      // if type is decimal
      else if (type == typeof(System.Decimal))
      {
        DataParameter.AddParameter(cm, strParamName, DbType.Decimal, value);
      }
      // if type is double
      else if (type == typeof(System.Double))
      {
        DataParameter.AddParameter(cm, strParamName, DbType.Double, value);
      }
      // if type is datetime
      else if (type == typeof(System.DateTime))
      {
        DataParameter.AddParameter(cm, strParamName, DbType.DateTime, value);
      }
    }
    #endregion Methods
  }
}
