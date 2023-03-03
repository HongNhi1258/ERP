/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 05/07/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class BOMCodeMaster : IObject
  {
    #region Fields
    private int group;
    private int code;
    private string value;
    private int deleteFlag;
    private int sort;
    private string description;
    #endregion Fields

    #region Constructors
    public BOMCodeMaster()
    {
      this.group = int.MinValue;
      this.code = int.MinValue;
      this.value = string.Empty;
      this.deleteFlag = int.MinValue;
      this.sort = int.MinValue;
      this.description = string.Empty;
    }
    public object Clone()
    {
      BOMCodeMaster obj = new BOMCodeMaster();
      obj.Group = this.group;
      obj.Code = this.code;
      obj.Value = this.value;
      obj.DeleteFlag = this.deleteFlag;
      obj.Sort = this.sort;
      obj.Description = this.description;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMCodeMaster";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Group", "Code" };
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
    public string Value
    {
      get { return this.value; }
      set { this.value = value; }
    }
    public int DeleteFlag
    {
      get { return this.deleteFlag; }
      set { this.deleteFlag = value; }
    }
    public int Sort
    {
      get { return this.sort; }
      set { this.sort = value; }
    }
    public string Description
    {
      get { return this.description; }
      set { this.description = value; }
    }
    #endregion Properties
  }
}