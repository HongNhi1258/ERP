using DaiCo.Application;
using System;
using System.Data;
using System.Reflection;

namespace DaiCo.Shared.DataBaseUtility
{
  public class DeleteObject
  {
    IObject obj;
    string[] arrPrimaryKeyNames;
    public DeleteObject(IObject _obj, string[] _arrPrimaryKeyNames)
    {
      this.obj = _obj;
      this.arrPrimaryKeyNames = _arrPrimaryKeyNames;
    }

    public string CommandText
    {
      get
      {
        string strDeleteQuery = string.Empty;

        FieldInfo[] myFieldInfo;

        //get Type of object
        Type myType = obj.GetType();

        // Get the type and fields of FieldInfoClass.
        myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        //Generate delete query
        string strDeleteClause = string.Format(@" DELETE FROM {0}", obj.GetTableName());
        string strWhereClause = " WHERE ";
        // Generate where clause
        if (this.arrPrimaryKeyNames != null)
        {
          foreach (string strKeyName in this.arrPrimaryKeyNames)
          {
            strWhereClause = string.Format(" {0} [{1}] = @{1} AND ", strWhereClause, strKeyName);
          }
        }
        strWhereClause = strWhereClause.Remove(strWhereClause.Length - 5, 5);
        strDeleteQuery = string.Format("{0}{1}", strDeleteClause, strWhereClause);
        // End generate delete query
        return strDeleteQuery;
      }
    }

    public void PrepareParameters(IDbCommand cm)
    {
      FieldInfo[] myFieldInfo;

      //get Type of object
      Type myType = obj.GetType();

      // Get the type and fields of FieldInfoClass.
      myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

      // if exist any parameters
      if (this.arrPrimaryKeyNames != null)
      {
        foreach (string strKeyName in this.arrPrimaryKeyNames)
        {
          FieldInfo field = myType.GetField(strKeyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

          //get type of field        
          Type type = field.FieldType;
          // convert value to DB value
          object value = DBConvert.ParseToDBValue(field.GetValue(obj));

          //get paramname
          string strParamName = "@" + field.Name;

          // if type is char
          if (type == typeof(System.Char))
          {
            DataParameter.AddParameter(cm, strParamName, DbType.AnsiStringFixedLength, value);
          }
          // if type is string
          else if (type == typeof(System.String))
          {
            DataParameter.AddParameter(cm, strParamName, DbType.String, value);
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
  }
}
