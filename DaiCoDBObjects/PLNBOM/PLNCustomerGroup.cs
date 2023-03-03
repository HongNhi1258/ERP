/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 05/07/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class PLNCustomerGroup : IObject
  {
    #region Fields
    private int pid;
    private string name;
    private int deleteFlg;
    #endregion Fields

    #region Constructors
    public PLNCustomerGroup()
    {
      this.pid = int.MinValue;
      this.name = string.Empty;
      this.deleteFlg = int.MinValue;
    }
    public object Clone()
    {
      PLNCustomerGroup obj = new PLNCustomerGroup();
      obj.Pid = this.pid;
      obj.Name = this.name;
      obj.DeleteFlg = this.deleteFlg;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPLNCustomerGroup";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Pid" };
    }
    #endregion Constructors

    #region Properties

    public int Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public string Name
    {
      get { return this.name; }
      set { this.name = value; }
    }
    public int DeleteFlg
    {
      get { return this.deleteFlg; }
      set { this.deleteFlg = value; }
    }
    #endregion Properties
  }
}