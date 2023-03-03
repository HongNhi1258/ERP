/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 21-06-2008
   Company : Dai Co   
 */

using DaiCo.Application;
using DaiCo.SqlGateway;
using System;
using System.Collections;
using System.Data;
using System.Reflection;
namespace DaiCo.TransactionImplement
{

  /// <summary>
  /// Summary description for InsertTransaction.
  /// </summary>
  public class UpdateObjectByKeyTransaction : ITransaction
  {

    #region Fields
    private IObject obj;
    private SqlFactory factory;
    private object result;
    #endregion

    #region Constructor
    // init 
    public UpdateObjectByKeyTransaction(IObject obj)
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

        // Get length of string(char, varchar, nvarchar) columns
        string strTableName = this.obj.GetTableName();
        IList arrColumns = this.GetLengthOfStringColumns(strTableName);
        // make sql query
        IQuery query = new UpdateObjectSqlFactory(this.obj, arrColumns, this.GetIdentityFieldName(strTableName));

        // Make command
        IDbCommand cm = this.factory.MakeCommand(query.CommandText);

        // Set parameters
        query.PrepareParameters(cm);

        // execute query          
        this.factory.ExecuteNonQuery(cm);

        result = true;
      }
      catch (Exception e)
      {
        result = false;
        //throw to trace log
        throw new TransactionException("UpdateObjectByKeyTransaction", e);
      }
    }

    public object Result
    {
      //result as true or false
      get { return this.result; }
    }
    #endregion

    #region Methods
    // get List Of char, varchar, nvarchar columns
    private IList GetLengthOfStringColumns(string strTableName)
    {
      // Make command
      IDbCommand cm = this.factory.MakeCommand(string.Format(@"
        SELECT column_name, data_type, CASE WHEN data_type = 'nvarchar' THEN  character_octet_length/2 ELSE character_octet_length END character_octet_length
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE table_name='TBLPLNSaleOrder' AND data_type IN ('char', 'varchar', 'nvarchar')", strTableName));

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
    private string GetIdentityFieldName(string strTableName)
    {
      // Make command
      IDbCommand cm = this.factory.MakeCommand(string.Format(@"
        SELECT c.name
        FROM syscolumns c JOIN sysobjects o  ON c.id = o.id
        WHERE  c.status = 128 AND o.name = '{0}'", strTableName));
      // execute query          
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
}
