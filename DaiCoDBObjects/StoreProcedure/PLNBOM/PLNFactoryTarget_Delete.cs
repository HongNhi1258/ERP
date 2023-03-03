/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 17/06/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System.Data;

namespace DaiCo.Objects
{
  public class PLNFactoryTarget_Delete : IStoreObject
  {
    #region Fields
    private int group;
    private int code;
    private int month;
    private int year;
    #endregion Fields

    #region Constructors
    public PLNFactoryTarget_Delete()
    {
      this.group = int.MinValue;
      this.code = int.MinValue;
      this.month = int.MinValue;
      this.year = int.MinValue;
    }
    public void GetOutputValue(IDbCommand cm)
    {
    }
    public void PrepareParameters(IDbCommand cm)
    {
      if (this.Group != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Group", DbType.Int32, this.Group);
      }
      if (this.Code != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Code", DbType.Int32, this.Code);
      }
      if (this.Month != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Month", DbType.Int32, this.Month);
      }
      if (this.Year != int.MinValue)
      {
        DataParameter.AddParameter(cm, "@Year", DbType.Int32, this.Year);
      }
    }
    public string GetStoreName()
    {
      return "spPLNFactoryTarget_Delete";
    }
    #endregion Constructors

    #region Properties

    public int Group
    {
      get { return this.group; }
      set { this.group = value; }
    }
    public int Code
    {
      get { return this.code; }
      set { this.code = value; }
    }
    public int Month
    {
      get { return this.month; }
      set { this.month = value; }
    }
    public int Year
    {
      get { return this.year; }
      set { this.year = value; }
    }
    #endregion Properties
  }
}