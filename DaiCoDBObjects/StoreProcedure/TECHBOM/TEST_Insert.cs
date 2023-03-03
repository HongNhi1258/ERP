/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 11/06/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System.Data;
using System.Data.SqlClient;

namespace DaiCo.Objects
{
  public class TEST_Insert : IStoreObject
  {
    #region Fields
    private string name;
    private string description;
    private long result;
    #endregion Fields

    #region Constructors
    public TEST_Insert()
    {
      this.name = string.Empty;
      this.description = string.Empty;
      this.result = long.MinValue;
    }
    public void GetOutputValue(IDbCommand cm)
    {
      this.Result = DBConvert.ParseLong(((SqlParameter)(cm.Parameters["@Result"])).Value.ToString());
    }
    public void PrepareParameters(IDbCommand cm)
    {
      if (this.Name != string.Empty)
      {
        DataParameter.AddParameter(cm, "@Name", DbType.AnsiString, 50, this.Name);
      }
      if (this.Description != string.Empty)
      {
        DataParameter.AddParameter(cm, "@Description", DbType.String, 100, this.Description);
      }
      DataParameter.AddParameter(cm, ParameterDirection.Output, "@Result", DbType.Int64, this.Result);
    }
    public string GetStoreName()
    {
      return "spTEST_Insert";
    }
    #endregion Constructors

    #region Properties

    public string Name
    {
      get { return this.name; }
      set { this.name = value; }
    }
    public string Description
    {
      get { return this.description; }
      set { this.description = value; }
    }
    public long Result
    {
      get { return this.result; }
      set { this.result = value; }
    }
    #endregion Properties
  }
}