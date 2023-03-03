/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 09/03/2011
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class PURMaterialSupplier : IObject
  {
    #region Fields
    private long pid;
    private string materialCode;
    private long supplierPid;
    private string supplierReferenceName;
    private string supplierReferenceCode;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime creadate;
    #endregion Fields

    #region Constructors
    public PURMaterialSupplier()
    {
      this.pid = long.MinValue;
      this.materialCode = string.Empty;
      this.supplierPid = long.MinValue;
      this.supplierReferenceName = string.Empty;
      this.supplierReferenceCode = string.Empty;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.creadate = DateTime.MinValue;
    }
    public object Clone()
    {
      PURMaterialSupplier obj = new PURMaterialSupplier();
      obj.Pid = this.pid;
      obj.MaterialCode = this.materialCode;
      obj.SupplierPid = this.supplierPid;
      obj.SupplierReferenceName = this.supplierReferenceName;
      obj.SupplierReferenceCode = this.supplierReferenceCode;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.Creadate = this.creadate;
      return obj;
    }
    public string GetTableName()
    {
      return "TblPURMaterialSupplier";
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
    public string MaterialCode
    {
      get { return this.materialCode; }
      set { this.materialCode = value; }
    }
    public long SupplierPid
    {
      get { return this.supplierPid; }
      set { this.supplierPid = value; }
    }
    public string SupplierReferenceName
    {
      get { return this.supplierReferenceName; }
      set { this.supplierReferenceName = value; }
    }
    public string SupplierReferenceCode
    {
      get { return this.supplierReferenceCode; }
      set { this.supplierReferenceCode = value; }
    }
    public int CreateBy
    {
      get { return this.createBy; }
      set { this.createBy = value; }
    }
    public DateTime CreateDate
    {
      get { return this.createDate; }
      set { this.createDate = value; }
    }
    public int UpdateBy
    {
      get { return this.updateBy; }
      set { this.updateBy = value; }
    }
    public DateTime Creadate
    {
      get { return this.creadate; }
      set { this.creadate = value; }
    }
    #endregion Properties
  }
}