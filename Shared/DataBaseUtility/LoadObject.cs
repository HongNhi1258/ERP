using DaiCo.Application;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace DaiCo.Shared.DataBaseUtility
{
  public class LoadObject
  {
    IObject obj;
    string[] arrPrimaryKeyNames;
    public LoadObject(IObject _obj, string[] _arrPrimaryKeyNames)
    {
      this.obj = _obj;
      this.arrPrimaryKeyNames = _arrPrimaryKeyNames;
    }

    public string CommandText
    {
      get
      {
        string strLoadQuery = string.Empty;

        //get Type of object
        Type myType = obj.GetType();

        //Generate insert query
        string strSelectClause = string.Format(@" SELECT TOP 1 * FROM {0}", obj.GetTableName());
        string strWhereClause = "WHERE ";

        // generate where clause
        if (this.arrPrimaryKeyNames != null)
        {
          foreach (string primaryKeyName in this.arrPrimaryKeyNames)
          {
            strWhereClause = string.Format("{0}[{1}] = @{1} AND ", strWhereClause, primaryKeyName);
          }
        }
        strWhereClause = strWhereClause.Remove(strWhereClause.Length - 5, 5);
        strLoadQuery = string.Format("{0} {1}", strSelectClause, strWhereClause);
        // end generate load query

        return strLoadQuery;
      }
    }

    public void PrepareParameters(IDbCommand cm)
    {
      FieldInfo[] myFieldInfo;

      //get Type of object
      Type myType = obj.GetType();

      // Get the type and fields of FieldInfoClass.
      myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

      if (this.arrPrimaryKeyNames != null)
      {
        foreach (string strKeyName in arrPrimaryKeyNames)
        {
          for (int i = 0; i < myFieldInfo.Length; i++)
          {
            //get paramname
            string strParamName = string.Format("@{0}", myFieldInfo[i].Name);

            // if( strKeyName is equal fieldname)  
            if (strKeyName.Equals(myFieldInfo[i].Name, StringComparison.OrdinalIgnoreCase))
            {
              //get type of field        
              Type type = myFieldInfo[i].FieldType;

              // convert value to DB value
              object value = DBConvert.ParseToDBValue(myFieldInfo[i].GetValue(obj));

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
              break;
            }
          }
        }
      }
    }

    public IObject ReadResult(SqlDataReader dre)
    {

      IObject iobj = (IObject)obj.Clone();

      FieldInfo[] myFieldInfo;

      //get Type of object
      Type myType = obj.GetType();

      // Get the type and fields of FieldInfoClass.
      myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

      if (dre.Read())
      {
        for (int i = 0; i < myFieldInfo.Length; i++)
        {
          //get type of field        
          Type type = myFieldInfo[i].FieldType;
          string strParamName = myFieldInfo[i].Name;

          // if type is char
          if (type == typeof(System.Char))
          {
            myFieldInfo[i].SetValue(iobj, DBConvert.ParseDBToChar(dre, strParamName));
          }
          // if type is string
          else if (type == typeof(System.String))
          {
            myFieldInfo[i].SetValue(iobj, DBConvert.ParseDBToString(dre, strParamName));
          }
          // if type is smallint
          else if (type == typeof(System.Int16))
          {
            myFieldInfo[i].SetValue(iobj, DBConvert.ParseDBToSmallInt(dre, strParamName));
          }
          // if type is int
          else if (type == typeof(System.Int32))
          {
            myFieldInfo[i].SetValue(iobj, DBConvert.ParseDBToInt(dre, strParamName));
          }
          // if type is long
          else if (type == typeof(System.Int64))
          {
            myFieldInfo[i].SetValue(iobj, DBConvert.ParseDBToLong(dre, strParamName));
          }
          // if type is decimal
          else if (type == typeof(System.Decimal))
          {
            myFieldInfo[i].SetValue(iobj, DBConvert.ParseDBToDecimal(dre, strParamName));
          }
          // if type is double
          else if (type == typeof(System.Double))
          {
            myFieldInfo[i].SetValue(iobj, DBConvert.ParseDBToDouble(dre, strParamName));
          }
          // if type is datetime
          else if (type == typeof(System.DateTime))
          {
            myFieldInfo[i].SetValue(iobj, DBConvert.ParseDBToDateTime(dre, strParamName));
          }
        }
        return iobj;
      }
      return null;
    }
  }
}
