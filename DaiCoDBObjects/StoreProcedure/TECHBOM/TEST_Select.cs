/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 11/06/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System.Data;

namespace DaiCo.Objects
{
  public class TEST_Select : IStoreObject
  {
    #region Fields
    #endregion Fields

    #region Constructors
    public TEST_Select()
    {
    }
    public void GetOutputValue(IDbCommand cm)
    {
    }
    public void PrepareParameters(IDbCommand cm)
    {

    }
    public string GetStoreName()
    {
      return "spTEST_Select";
    }
    #endregion Constructors

    #region Properties

    #endregion Properties
  }
}