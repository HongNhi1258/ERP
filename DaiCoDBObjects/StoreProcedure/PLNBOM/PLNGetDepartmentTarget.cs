/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 23/06/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System.Data;

namespace DaiCo.Objects
{
  public class PLNGetDepartmentTarget : IStoreObject
  {
    #region Fields
    private int yearNo;
    #endregion Fields

    #region Constructors
    public PLNGetDepartmentTarget()
    {
      this.yearNo = int.MinValue;
    }
    public void GetOutputValue(IDbCommand cm)
    {
    }
    public void PrepareParameters(IDbCommand cm)
    {
      if (this.YearNo != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@YearNo", DbType.Int32, this.YearNo);
      }
    }
    public string GetStoreName()
    {
      return "spPLNGetDepartmentTarget";
    }
    #endregion Constructors

    #region Properties

    public int YearNo
    {
      get { return this.yearNo; }
      set { this.yearNo = value; }
    }
    #endregion Properties
  }
}