/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 13-08-2008
   Company :  Dai Co   
 */

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace DaiCo.Application
{
  public class SqlFactory
  {
    #region Fields
    private string connectionString;
    private SqlConnection connection;
    private SqlTransaction transaction;
    #endregion Fields

    #region Contructors
    public SqlFactory()
    {
    }
    #endregion Contructors

    #region Properties
    public static SqlFactory Default
    {
      get { return new SqlFactory(); }
    }
    public string ConnectionString
    {
      set { this.connectionString = value; }
    }
    public SqlConnection Connection
    {
      get
      {
        Debug.Assert(connection != null);
        if (connection.State == ConnectionState.Closed)
        {
          connection.Open();
        }
        return connection;
      }
    }
    #endregion Properties

    #region Methods
    public SqlConnection MakeConnection(string connectionString)
    {
      if (connection == null || connection.State == ConnectionState.Closed)
      {
        connection = new SqlConnection(connectionString);
        connection.Open();
      }
      return connection;
    }
    private SqlConnection MakeConnection()
    {
      if (connection == null || connection.State == ConnectionState.Closed)
      {
        connection = new SqlConnection(connectionString);
        connection.Open();
      }
      return connection;
    }
    private SqlTransaction MakeTransaction()
    {
      if (transaction == null)
      {
        transaction = connection.BeginTransaction();
      }
      return transaction;
    }
    public SqlCommand MakeCommand(string command)
    {
      connection = this.MakeConnection();
      SqlCommand cm = new SqlCommand(command, connection);
      if (transaction != null)
      {
        cm.Transaction = transaction;
      }
      return cm;
    }
    public SqlTransaction BeginTransaction()
    {
      return (transaction = Connection.BeginTransaction());
    }
    public void ReleaseTransaction()
    {
      transaction = null;
    }
    public void Release()
    {
      ReleaseTransaction();
      if (connection != null)
      {
        connection.Close();
        connection.Dispose();
      }
    }
    public void Rollback()
    {
      if (transaction != null)
      {
        transaction.Rollback();
      }
    }
    public void Commit()
    {
      if (transaction != null)
      {
        transaction.Commit();
      }
    }
    public void ExecuteNonQuery(IDbCommand cm)
    {
      try
      {
        cm.ExecuteNonQuery();
      }
      catch (Exception e)
      {
        cm.Dispose();
        throw new SQLException(this.BuildSQLErrorString(cm), e);
      }
      finally
      {
        cm.Dispose();
      }
    }
    public IDataReader ExecuteReader(IDbCommand cm)
    {
      IDataReader dre = null;
      try
      {
        dre = cm.ExecuteReader();
      }
      catch (Exception e)
      {
        cm.Dispose();
        throw new SQLException(this.BuildSQLErrorString(cm), e);
      }
      finally
      {
        cm.Dispose();
      }
      return dre;
    }
    public object ExecuteScalar(IDbCommand cm)
    {
      object result = null;
      try
      {
        result = cm.ExecuteScalar();
      }
      catch (Exception e)
      {
        cm.Dispose();
        throw new SQLException(this.BuildSQLErrorString(cm), e);
      }
      finally
      {
        cm.Dispose();
      }
      return result;
    }
    public SqlDataAdapter ExecuteDataAdapter(IDbCommand cm)
    {
      SqlDataAdapter dataAdapter = null;
      try
      {
        dataAdapter = new SqlDataAdapter((SqlCommand)cm);
      }
      catch (Exception e)
      {
        cm.Dispose();
        throw new SQLException(this.BuildSQLErrorString(cm), e);
      }
      finally
      {
        cm.Dispose();
      }
      return dataAdapter;
    }
    private string BuildSQLErrorString(IDbCommand cm)
    {
      Debug.Assert(cm != null);
      StringBuilder result = new StringBuilder();
      IDataParameterCollection @params = cm.Parameters;
      int count = @params.Count;
      IDbDataParameter param = null;

      result.Append(cm.CommandText);
      for (int i = 0; i < count; i++)
      {
        param = (IDbDataParameter)@params[i];
        result.AppendFormat("\n\r\r{0}={1}", param.ParameterName, param.Value);
      }
      return result.ToString();
    }
    #endregion Methods
  }
}
