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
  public class SearchObjectSqlFactory : IQuery
  {
    #region Fields
    private IObject obj;
    private DBParameter[] arrInParameters;
    private DBParameter[] arrOutParameters;
    #endregion

    #region Constructor
    public SearchObjectSqlFactory(IObject obj, DBParameter[] arrInParameters, DBParameter[] arrOutParameters)
    {
      this.obj = obj;
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

    // get result from the query
    public object ReadResult(object value)
    {
      IList result = new ArrayList();

      if (this.obj != null)
      {
        IDataReader dre = (IDataReader)value;

        FieldInfo[] myFieldInfo;

        //get Type of object
        Type myType = obj.GetType();

        // Get the type and fields of FieldInfoClass.
        myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        if (myFieldInfo.Length <= dre.FieldCount)
        {
          while (dre.Read())
          {
            this.SetValueForObject(dre, myFieldInfo);
            IObject iobj = (IObject)obj.Clone();
            result.Add(iobj);
          }
        }
        else
        {
          while (dre.Read())
          {
            this.SetValueForObject(dre, myType);
            IObject iobj = (IObject)obj.Clone();
            result.Add(iobj);
          }
        }
      }
      return result;
    }
    #endregion ReadResult

    #region Methods

    private void SetValueForObject(IDataReader dre, Type type)
    {
      for (int i = 0; i < dre.FieldCount; i++)
      {
        //get type of field
        string fieldName = dre.GetName(i);
        PropertyInfo property = type.GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (property == null)
        {
          continue;
        }
        this.SetValue(property, dre);
      }
    }

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

    private void SetValue(PropertyInfo field, IDataReader dre)
    {
      // if type is char
      if (field.PropertyType == typeof(System.Char))
      {
        field.SetValue(obj, DBConvert.ParseDBToChar(dre, field.Name), null);
      }
      // if type is string
      else if (field.PropertyType == typeof(System.String))
      {
        field.SetValue(obj, DBConvert.ParseDBToString(dre, field.Name), null);
      }
      // if type is smallint
      else if (field.PropertyType == typeof(System.Int16))
      {
        field.SetValue(obj, DBConvert.ParseDBToSmallInt(dre, field.Name), null);
      }
      // if type is int
      else if (field.PropertyType == typeof(System.Int32))
      {
        field.SetValue(obj, DBConvert.ParseDBToInt(dre, field.Name), null);
      }
      // if type is long
      else if (field.PropertyType == typeof(System.Int64))
      {
        field.SetValue(obj, DBConvert.ParseDBToLong(dre, field.Name), null);
      }
      // if type is decimal
      else if (field.PropertyType == typeof(System.Decimal))
      {
        field.SetValue(obj, DBConvert.ParseDBToDecimal(dre, field.Name), null);
      }
      // if type is double
      else if (field.PropertyType == typeof(System.Double))
      {
        field.SetValue(obj, DBConvert.ParseDBToDouble(dre, field.Name), null);
      }
      // if type is datetime
      else if (field.PropertyType == typeof(System.DateTime))
      {
        field.SetValue(obj, DBConvert.ParseDBToDateTime(dre, field.Name), null);
      }
    }
    #endregion Methods
  }
}