using DaiCo.Application;
using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace DaiCo.Shared.DataBaseUtility
{

  public class UpdateObject
  {
    IObject obj;
    string[] arrPrimaryKeyNames;
    IList arrColumns;
    public UpdateObject(IObject _obj, string[] _arrPrimaryKeyNames)
    {
      this.obj = _obj;
      this.arrPrimaryKeyNames = _arrPrimaryKeyNames;
    }
    public string CommandText
    {
      get
      {
        string identityFieldName = GetIdentityFieldName(obj.GetTableName());
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
          foreach (string strKeyName in arrPrimaryKeyNames)
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

    // make parameters
    public void PrepareParameters(IDbCommand cm)
    {
      arrColumns = GetLengthOfStringColumns(obj.GetTableName());
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

    // get List Of char, varchar, nvarchar columns
    private IList GetLengthOfStringColumns(string tableName)
    {
      // Make command
      string cmm = string.Format(@"
        SELECT column_name, data_type, CASE WHEN data_type = 'nvarchar' THEN  character_octet_length/2 ELSE character_octet_length END character_octet_length
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE table_name='{0}' AND data_type IN ('char', 'varchar', 'nvarchar')", tableName);

      DataSet ds = DataBaseAccess.SearchCommandText(cmm);

      IList arrParams = new ArrayList();
      if (ds != null)
      {
        DataTable dt = ds.Tables[0];
        foreach (DataRow row in dt.Rows)
        {
          arrParams.Add(new DBParameter((string)row["column_name"], DbType.String, (int)row["character_octet_length"]));
        }
      }
      return arrParams;
    }

    //get identity field name
    private string GetIdentityFieldName(string tableName)
    {
      // Make command
      string cmm = string.Format(@"
        SELECT c.name
          FROM syscolumns c JOIN sysobjects o 
           ON c.id = o.id
          WHERE  c.status = 128 
          AND o.name = '{0}'", tableName);

      // execute query
      string identityName = string.Empty;
      try
      {
        identityName = (string)DataBaseAccess.ExecuteScalarCommandText(cmm);
      }
      catch
      {
        return null;
      }
      if (identityName == null || identityName.Length == 0)
      {
        return null;
      }
      FieldInfo[] myFieldInfo;
      //get Type of object
      Type myType = obj.GetType();
      // Get the type and fields of FieldInfoClass.
      myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
      for (int i = 0; i < myFieldInfo.Length; i++)
      {
        string fieldDBName = myFieldInfo[i].Name;
        if (identityName.Equals(fieldDBName, StringComparison.OrdinalIgnoreCase))
        {
          return myFieldInfo[i].Name;
        }
      }
      return null;
    }

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
  }
}
