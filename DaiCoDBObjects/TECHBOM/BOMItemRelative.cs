/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 05/07/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class BOMItemRelative : IObject
  {
    #region Fields
    private long pid;
    private string itemCode;
    private int revision;
    private string itemRelative;
    private string description;
    #endregion Fields

    #region Constructors
    public BOMItemRelative()
    {
      this.pid = long.MinValue;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.itemRelative = string.Empty;
      this.description = string.Empty;
    }
    public object Clone()
    {
      BOMItemRelative obj = new BOMItemRelative();
      obj.Pid = this.pid;
      obj.ItemCode = this.itemCode;
      obj.Revision = this.revision;
      obj.ItemRelative = this.itemRelative;
      obj.Description = this.description;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMItemRelative";
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
    public string ItemCode
    {
      get { return this.itemCode; }
      set { this.itemCode = value; }
    }
    public int Revision
    {
      get { return this.revision; }
      set { this.revision = value; }
    }
    public string ItemRelative
    {
      get { return this.itemRelative; }
      set { this.itemRelative = value; }
    }
    public string Description
    {
      get { return this.description; }
      set { this.description = value; }
    }
    #endregion Properties
  }
}