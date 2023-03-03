/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 09/03/2011
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class PURCurrencyInfo : IObject
  {
    #region Fields
    private long pid;
    private string code;
    private string name;
    #endregion Fields

    #region Constructors
    public PURCurrencyInfo()
    {
      this.pid = long.MinValue;
      this.code = string.Empty;
      this.name = string.Empty;
    }
    public object Clone()
    {
      PURCurrencyInfo obj = new PURCurrencyInfo();
      obj.Pid = this.pid;
      obj.Code = this.code;
      obj.Name = this.name;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPURCurrencyInfo";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Pid" };
    }
    #endregion Constructors

    #region Properties

    public long Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public string Code
    {
      get { return this.code; }
      set { this.code = value; }
    }
    public string Name
    {
      get { return this.name; }
      set { this.name = value; }
    }
    #endregion Properties
  }
}