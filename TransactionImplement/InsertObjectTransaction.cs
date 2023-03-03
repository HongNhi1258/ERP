/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 21-06-2008
   Company :  Dai Co   
 */

using DaiCo.Application;
using DaiCo.SqlGateway;
using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace DaiCo.TransactionImplement
{

  #region Class InsertObjectTransaction

  /// <summary>
  /// Summary description for InsertTransaction.
  /// </summary>
  public class InsertObjectTransaction : ITransaction
  {
    #region Fields

    private IObject obj;
    private SqlFactory factory;
    private object result;

    #endregion

    #region Constructor
    // init 
    public InsertObjectTransaction(IObject obj)
    {
      this.obj = obj;
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
        // get length of string columns from database
        string tableName = obj.GetTableName();
        IList listColumns = this.GetLengthOfStringColumns(tableName);

        // get info of identity parameter
        string identityName = this.GetIdentityFieldName(tableName);

        // make sql query
        IQuery query = new InsertObjectSqlFactory(this.obj, listColumns);

        // Make command
        IDbCommand cm = this.factory.MakeCommand(query.CommandText);

        // Set parameters
        query.PrepareParameters(cm);

        // execute query          

        object newID = query.ReadResult(this.factory.ExecuteScalar(cm));

        //Set value for 
        if (identityName != null)
        {
          // set value for identity key
          Type type = obj.GetType();
          FieldInfo field = type.GetField(identityName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

          if (field.FieldType == typeof(System.Int64))
          {
            field.SetValue(obj, Convert.ToInt64(newID));
          }
          else if (field.FieldType == typeof(System.Int32))
          {
            field.SetValue(obj, Convert.ToInt32(newID));
          }
          else if (field.FieldType == typeof(System.Int16))
          {
            field.SetValue(obj, Convert.ToInt16(newID));
          }
        }
        this.result = this.obj.Clone();
        cm.Dispose();
      }
      catch (Exception e)
      {
        this.result = null;
        //throw to trace log
        throw new TransactionException("InsertObjectTransaction", e);
      }
    }
    public object Result
    {
      // return true if insert sucessfully
      get { return this.result; }
    }
    #endregion

    #region Methods

    // get List Of char, varchar, nvarchar columns
    private IList GetLengthOfStringColumns(string tableName)
    {
      // Make command
      IDbCommand cm = this.factory.MakeCommand(string.Format(@"
        SELECT column_name, data_type, CASE WHEN data_type = 'nvarchar' THEN  character_octet_length/2 ELSE character_octet_length END character_octet_length
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE table_name='TBLPLNSaleOrder' AND data_type IN ('char', 'varchar', 'nvarchar')", tableName));

      // execute query          
      IDataReader dre = this.factory.ExecuteReader(cm);
      IList arrParams = new ArrayList();
      while (dre.Read())
      {
        arrParams.Add(new DBParameter(DBConvert.ParseDBToString(dre, "column_name"), DbType.String, DBConvert.ParseDBToInt(dre, "character_octet_length")));
      }
      dre.Close();
      return arrParams;

    }
    //get identity field name
    private string GetIdentityFieldName(string tableName)
    {
      // Make command
      IDbCommand cm = this.factory.MakeCommand(string.Format(@"
        SELECT c.name
          FROM syscolumns c JOIN sysobjects o 
           ON c.id = o.id
          WHERE  c.status = 128 
          AND o.name = '{0}'", tableName));

      // execute query
      string identityName = string.Empty;
      try
      {
        identityName = (string)this.factory.ExecuteScalar(cm);
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
    #endregion
  }
  #endregion Class InsertObjectTransaction
}
