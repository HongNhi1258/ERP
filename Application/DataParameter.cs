/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 13-08-2008
   Company :  Dai Co   
 */

using System.Data;

namespace DaiCo.Application
{
  public class DBParameter
  {
    private string parameterName;
    private DbType dbType;
    private int size;
    private object value;

    public DBParameter() { }

    public DBParameter(string parameterName, DbType type, object value)
    {
      this.parameterName = parameterName;
      this.dbType = type;
      this.value = value;
    }
    public DBParameter(string parameterName, DbType type, int size, object value)
    {
      this.parameterName = parameterName;
      this.dbType = type;
      this.value = value;
      this.size = size;
    }
    public string ParameterName
    {
      get { return this.parameterName; }
      set { this.parameterName = value; }
    }
    public DbType DbType
    {
      get { return this.dbType; }
      set { this.dbType = value; }
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
  public class DataParameter
  {

    #region Fields
    public const byte NameLength = 100;
    public const byte AddressLength = 100;
    public const byte CharacterLength = 1;
    public const byte UserNameLength = 50;
    #endregion Fields

    public DataParameter() { }

    public static void AddParameter(IDbCommand cm, string parameterName, DbType type, int size, object value)
    {
      IDbDataParameter para = cm.CreateParameter();
      para.ParameterName = parameterName;
      para.DbType = type;
      para.Size = size;
      para.Value = value;
      cm.Parameters.Add(para);
    }

    public static void AddParameter(IDbCommand cm, string parameterName, DbType type, object value)
    {
      IDbDataParameter para = cm.CreateParameter();
      para.ParameterName = parameterName;
      para.DbType = type;
      para.Value = value;
      cm.Parameters.Add(para);
    }
    public static void AddParameter(IDbCommand cm, ParameterDirection direction, string parameterName, DbType type, int size, object value)
    {
      IDbDataParameter para = cm.CreateParameter();
      para.ParameterName = parameterName;
      para.DbType = type;
      para.Size = size;
      para.Value = value;
      para.Direction = direction;
      cm.Parameters.Add(para);
    }

    public static void AddParameter(IDbCommand cm, IDbDataParameter _param)
    {
      IDbDataParameter para = cm.CreateParameter();
      para.ParameterName = _param.ParameterName;
      para.DbType = _param.DbType;
      para.Value = _param.Value;
      para.Size = _param.Size;
      cm.Parameters.Add(para);
    }

    public static void AddParameter(IDbCommand cm, ParameterDirection direction, string parameterName, DbType type, object value)
    {
      IDbDataParameter para = cm.CreateParameter();
      para.ParameterName = parameterName;
      para.DbType = type;
      para.Value = value;
      para.Direction = direction;
      cm.Parameters.Add(para);
    }
  }
}
