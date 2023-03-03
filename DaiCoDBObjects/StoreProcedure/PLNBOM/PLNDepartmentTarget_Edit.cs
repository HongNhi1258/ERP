/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 17/06/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System.Data;
using System.Data.SqlClient;

namespace DaiCo.Objects
{
  public class PLNDepartmentTarget_Edit : IStoreObject
  {
    #region Fields
    private string code;
    private int yearNo;
    private int monthNo;
    private double target;
    private long result;
    #endregion Fields

    #region Constructors
    public PLNDepartmentTarget_Edit()
    {
      this.code = string.Empty;
      this.yearNo = int.MinValue;
      this.monthNo = int.MinValue;
      this.target = double.MinValue;
      this.result = long.MinValue;
    }
    public void GetOutputValue(IDbCommand cm)
    {
      this.Result = DBConvert.ParseLong(((SqlParameter)(cm.Parameters["@Result"])).Value.ToString());
    }
    public void PrepareParameters(IDbCommand cm)
    {
      if (this.Code != string.Empty)
      {
        DataParameter.AddParameter(cm, "@Code", DbType.AnsiString, 20, this.Code);
      }
      if (this.YearNo != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@YearNo", DbType.Int32, this.YearNo);
      }
      if (this.MonthNo != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@MonthNo", DbType.Int32, this.MonthNo);
      }
      if (this.Target != double.MinValue)
      {
        DataParameter.AddParameter(cm, "@Target", DbType.Double, this.Target);
      }
      DataParameter.AddParameter(cm, ParameterDirection.Output, "@Result", DbType.Int64, this.Result);
    }
    public string GetStoreName()
    {
      return "spPLNDepartmentTarget_Edit";
    }
    #endregion Constructors

    #region Properties

    public string Code
    {
      get { return this.code; }
      set { this.code = value; }
    }
    public int YearNo
    {
      get { return this.yearNo; }
      set { this.yearNo = value; }
    }
    public int MonthNo
    {
      get { return this.monthNo; }
      set { this.monthNo = value; }
    }
    public double Target
    {
      get { return this.target; }
      set { this.target = value; }
    }
    public long Result
    {
      get { return this.result; }
      set { this.result = value; }
    }
    #endregion Properties
  }
}