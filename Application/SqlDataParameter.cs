/*
   Author  : Nguyen Van Tron
   Email   : tron_it@daico-furniture.com
   Date    : 28/06/2012
   Company :  Dai Co   
 */

using System.Data;

using System.Data.SqlClient;

namespace DaiCo.Application
{
  public class SqlDBParameter
  {
    private string parameterName;
    private SqlDbType sqlDbType;
    private int size;
    private object value;

    public SqlDBParameter() { }

    public SqlDBParameter(string parameterName, SqlDbType type, object value)
    {
      this.parameterName = parameterName;
      this.sqlDbType = type;
      this.value = value;
    }
    public SqlDBParameter(string parameterName, SqlDbType type, int size, object value)
    {
      this.parameterName = parameterName;
      this.sqlDbType = type;
      this.value = value;
      this.size = size;
    }
    public string ParameterName
    {
      get { return this.parameterName; }
      set { this.parameterName = value; }
    }
    public SqlDbType SqlDbType
    {
      get { return this.sqlDbType; }
      set { this.sqlDbType = value; }
    }
    public int Size
    {
      get { return this.size; }
      set { this.size = value; }
    }
    public object Value
    {
      get { return this.value; }
      set { this.value = value; }
    }

  }
  /// <summary>
  /// Summary description for DataParameter.
  /// </summary>
  public class SqlDataParameter
  {

    #region Fields
    public const byte NameLength = 100;
    public const byte AddressLength = 100;
    public const byte CharacterLength = 1;
    public const byte UserNameLength = 50;
    #endregion Fields

    public SqlDataParameter() { }

    public static void AddParameter(SqlCommand cm, string parameterName, SqlDbType type, int size, object value)
    {
      SqlParameter para = cm.CreateParameter();
      para.ParameterName = parameterName;
      para.SqlDbType = type;
      para.Size = size;
      para.Value = value;
      cm.Parameters.Add(para);
    }

    public static void AddParameter(SqlCommand cm, string parameterName, SqlDbType type, object value)
    {
      SqlParameter para = cm.CreateParameter();
      para.ParameterName = parameterName;
      para.SqlDbType = type;
      para.Value = value;
      cm.Parameters.Add(para);
    }
    public static void AddParameter(SqlCommand cm, ParameterDirection direction, string parameterName, SqlDbType type, int size, object value)
    {
      SqlParameter para = cm.CreateParameter();
      para.ParameterName = parameterName;
      para.SqlDbType = type;
      para.Size = size;
      para.Value = value;
      para.Direction = direction;
      cm.Parameters.Add(para);
    }
    public static void AddParameter(SqlCommand cm, ParameterDirection direction, string parameterName, SqlDbType type, object value)
    {
      SqlParameter para = cm.CreateParameter();
      para.ParameterName = parameterName;
      para.SqlDbType = type;
      para.Value = value;
      para.Direction = direction;
      cm.Parameters.Add(para);
    }
  }
}
