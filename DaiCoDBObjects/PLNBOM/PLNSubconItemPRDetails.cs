/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 06/09/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class PLNSubconItemPRDetails : IObject
  {
    #region Fields
    private long pid;
    private long subconItemRequestPid;
    private string carcassCode;
    private long supplierPid;
    #endregion Fields

    #region Constructors
    public PLNSubconItemPRDetails()
    {
      this.pid = long.MinValue;
      this.subconItemRequestPid = long.MinValue;
      this.carcassCode = string.Empty;
      this.supplierPid = long.MinValue;
    }
    public object Clone()
    {
      PLNSubconItemPRDetails obj = new PLNSubconItemPRDetails();
      obj.Pid = this.pid;
      obj.SubconItemRequestPid = this.subconItemRequestPid;
      obj.CarcassCode = this.carcassCode;
      obj.SupplierPid = this.supplierPid;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPLNSubconItemPRDetails";
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
    public long SubconItemRequestPid
    {
      get { return this.subconItemRequestPid; }
      set { this.subconItemRequestPid = value; }
    }
    public string CarcassCode
    {
      get { return this.carcassCode; }
      set { this.carcassCode = value; }
    }
    public long SupplierPid
    {
      get { return this.supplierPid; }
      set { this.supplierPid = value; }
    }
    #endregion Properties
  }
}