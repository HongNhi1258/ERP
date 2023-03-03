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
  public class InsertObjectSqlFactory : IQuery
  {
    #region Fields
    private IObject obj;
    private IList arrColumns;
    #endregion

    #region Constructor
    public InsertObjectSqlFactory(IObject obj, IList arrColumns)
    {
      this.obj = obj;
      this.arrColumns = arrColumns;
    }
    #endregion Constructor

    #region CommandText
    // make sql query
    public string CommandText
    {
      get
      {
        string strInsertQuery = string.Empty;

        FieldInfo[] myFieldInfo;

        //get Type of object
        Type myType = obj.GetType();

        // Get the type and fields of FieldInfoClass.
        myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        //Generate insert query
        string strInsert = string.Format(@" INSERT INTO {0} (", obj.GetTableName());
        string strValue = " VALUES (";

        for (int i = 0; i < myFieldInfo.Length; i++)
        {

          // if this field not have minvalue of long or int, set query
          if (!((myFieldInfo[i].FieldType == typeof(int) && (int)myFieldInfo[i].GetValue(obj) == int.MinValue) || (myFieldInfo[i].FieldType == typeof(long) && (long)myFieldInfo[i].GetValue(obj) == long.MinValue)))
          {
            strInsert = string.Format("{0} [{1}],", strInsert, myFieldInfo[i].Name);
            strValue = string.Format("{0} @{1},", strValue, myFieldInfo[i].Name);
          }
        }
        strInsert = string.Format("{0})", strInsert.Remove(strInsert.Length - 1, 1));
        strValue = string.Format("{0})", strValue.Remove(strValue.Length - 1, 1));
        strInsertQuery = string.Format("{0}{1} SELECT @@IDENTITY", strInsert, strValue);
        // end generate insert query
        return strInsertQuery;
      }
    }
    #endregion CommandText

    #region PrepareParameters
    // make parameters
    public void PrepareParameters(IDbCommand cm)
    {

      FieldInfo[] myFieldInfo;

      //get Type of object
      Type myType = obj.GetType();

      // Get the type and fields of FieldInfoClass.
      myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

      for (int i = 0; i < myFieldInfo.Length; i++)
      {
        //get type of field        
        Type type = myFieldInfo[i].FieldType;

        // if this field not have minvalue of long or int, add parameter
        if (!((myFieldInfo[i].FieldType == typeof(int) && (int)myFieldInfo[i].GetValue(obj) == int.MinValue) || (myFieldInfo[i].FieldType == typeof(long) && (long)myFieldInfo[i].GetValue(obj) == long.MinValue)))
        {
          // convert value to DB value
          object value = DBConvert.ParseToDBValue(myFieldInfo[i].GetValue(obj));

          //get paramname
          string strParamName = string.Format("@{0}", myFieldInfo[i].Name);

          // if type is char
          if (type == typeof(System.Char))
          {
            DataParameter.AddParameter(cm, strParamName, DbType.AnsiStringFixedLength, LengthOfStringColumns(this.arrColumns, myFieldInfo[i].Name), value);
          }
          // if type is string
          else if (type == typeof(System.String))
          {
            int length = LengthOfStringColumns(this.arrColumns, myFieldInfo[i].Name);
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
      }
    }
    #endregion PrepareParameters

    #region ReadResult

    // get result from the query
    public object ReadResult(object value)
    {
      return value;
    }
    #endregion ReadResult

    #region Methods
    // Return length of a char, varchar, nvarchar column  
    private int LengthOfStringColumns(IList arrColumns, string columnName)
    {
      foreach (DBParameter param in arrColumns)
      {
        if (param.ParameterName.Equals(columnName, StringComparison.OrdinalIgnoreCase))
        {
          return (int)param.Value;
        }
      }
      return 0;
    }
    #endregion Methods
  }
}
