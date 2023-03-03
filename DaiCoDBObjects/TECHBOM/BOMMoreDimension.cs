/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 05/07/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class BOMMoreDimension : IObject
  {
    #region Fields
    private int pid;
    private string itemCode;
    private int revision;
    private int dimensionKind;
    private int values;
    #endregion Fields

    #region Constructors
    public BOMMoreDimension()
    {
      this.pid = int.MinValue;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.dimensionKind = int.MinValue;
      this.values = int.MinValue;
    }
    public object Clone()
    {
      BOMMoreDimension obj = new BOMMoreDimension();
      obj.Pid = this.pid;
      obj.ItemCode = this.itemCode;
      obj.Revision = this.revision;
      obj.DimensionKind = this.dimensionKind;
      obj.Values = this.values;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMMoreDimension";
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
    public int DimensionKind
    {
      get { return this.dimensionKind; }
      set { this.dimensionKind = value; }
    }
    public int Values
    {
      get { return this.values; }
      set { this.values = value; }
    }
    #endregion Properties
  }
}