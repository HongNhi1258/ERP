/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 05/07/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class RDDMoreDimension : IObject
  {
    #region Fields
    private long pid;
    private string itemCode;
    private int dimensionKind;
    private int values;
    #endregion Fields

    #region Constructors
    public RDDMoreDimension()
    {
      this.pid = long.MinValue;
      this.itemCode = string.Empty;
      this.dimensionKind = int.MinValue;
      this.values = int.MinValue;
    }
    public object Clone()
    {
      RDDMoreDimension obj = new RDDMoreDimension();
      obj.Pid = this.pid;
      obj.ItemCode = this.itemCode;
      obj.DimensionKind = this.dimensionKind;
      obj.Values = this.values;
      return obj;
    }
    public string GetTableName()
    {
      return "TblRDDMoreDimension";
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