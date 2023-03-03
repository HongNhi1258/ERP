/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 05/07/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class TEST : IObject
  {
    #region Fields
    private long pid;
    private string name;
    private string description;
    #endregion Fields

    #region Constructors
    public TEST()
    {
      this.pid = long.MinValue;
      this.name = string.Empty;
      this.description = string.Empty;
    }
    public object Clone()
    {
      TEST obj = new TEST();
      obj.Pid = this.pid;
      obj.Name = this.name;
      obj.Description = this.description;
      return obj;
    }
    public string GetTableName()
    {
      return "TEST";
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
    #endregion Properties
  }
}