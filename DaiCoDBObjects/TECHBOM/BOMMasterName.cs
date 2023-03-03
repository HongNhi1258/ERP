/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 05/07/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class BOMMasterName : IObject
  {
    #region Fields
    private int group;
    private string nameEn;
    private string nameVn;
    #endregion Fields

    #region Constructors
    public BOMMasterName()
    {
      this.group = int.MinValue;
      this.nameEn = string.Empty;
      this.nameVn = string.Empty;
    }
    public object Clone()
    {
      BOMMasterName obj = new BOMMasterName();
      obj.Group = this.group;
      obj.NameEn = this.nameEn;
      obj.NameVn = this.nameVn;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMMasterName";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Group" };
    }
    #endregion Constructors

    #region Properties

    public int Group
    {
      get { return this.group; }
      set { this.group = value; }
    }
    public string NameEn
    {
      get { return this.nameEn; }
      set { this.nameEn = value; }
    }
    public string NameVn
    {
      get { return this.nameVn; }
      set { this.nameVn = value; }
    }
    #endregion Properties
  }
}