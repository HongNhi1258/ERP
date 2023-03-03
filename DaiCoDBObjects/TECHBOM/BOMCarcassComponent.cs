/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 05/07/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class BOMCarcassComponent : IObject
  {
    #region Fields
    private long pid;
    private int no;
    private string carcassCode;
    private string componentCode;
    private string descriptionVN;
    private int qty;
    private double waste;
    private double length;
    private double width;
    private double thickness;
    private int lamination;
    private int fingerJoin;
    private int specify;
    private int status;
    private string imageFile;
    private int contractOut;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private int primary;
    #endregion Fields

    #region Constructors
    public BOMCarcassComponent()
    {
      this.pid = long.MinValue;
      this.no = int.MinValue;
      this.carcassCode = string.Empty;
      this.componentCode = string.Empty;
      this.descriptionVN = string.Empty;
      this.qty = int.MinValue;
      this.waste = double.MinValue;
      this.length = double.MinValue;
      this.width = double.MinValue;
      this.thickness = double.MinValue;
      this.lamination = int.MinValue;
      this.fingerJoin = int.MinValue;
      this.specify = int.MinValue;
      this.status = int.MinValue;
      this.imageFile = string.Empty;
      this.contractOut = int.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.primary = int.MinValue;
    }
    public object Clone()
    {
      BOMCarcassComponent obj = new BOMCarcassComponent();
      obj.Pid = this.pid;
      obj.No = this.no;
      obj.CarcassCode = this.carcassCode;
      obj.ComponentCode = this.componentCode;
      obj.DescriptionVN = this.descriptionVN;
      obj.Qty = this.qty;
      obj.Waste = this.waste;
      obj.Length = this.length;
      obj.Width = this.width;
      obj.Thickness = this.thickness;
      obj.Lamination = this.lamination;
      obj.FingerJoin = this.fingerJoin;
      obj.Specify = this.specify;
      obj.Status = this.status;
      obj.ImageFile = this.imageFile;
      obj.ContractOut = this.contractOut;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.Primary = this.primary;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMCarcassComponent";
    }
    public string[] ObjectKey()
    {
      return new string[] { "Pid", "CarcassCode", "ComponentCode" };
    }
    #endregion Constructors

    #region Properties

    public long Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public int No
    {
      get { return this.no; }
      set { this.no = value; }
    }
    public string CarcassCode
    {
      get { return this.carcassCode; }
      set { this.carcassCode = value; }
    }
    public string ComponentCode
    {
      get { return this.componentCode; }
      set { this.componentCode = value; }
    }
    public string DescriptionVN
    {
      get { return this.descriptionVN; }
      set { this.descriptionVN = value; }
    }
    public int Qty
    {
      get { return this.qty; }
      set { this.qty = value; }
    }
    public double Waste
    {
      get { return this.waste; }
      set { this.waste = value; }
    }
    public double Length
    {
      get { return this.length; }
      set { this.length = value; }
    }
    public double Width
    {
      get { return this.width; }
      set { this.width = value; }
    }
    public double Thickness
    {
      get { return this.thickness; }
      set { this.thickness = value; }
    }
    public int Lamination
    {
      get { return this.lamination; }
      set { this.lamination = value; }
    }
    public int FingerJoin
    {
      get { return this.fingerJoin; }
      set { this.fingerJoin = value; }
    }
    public int Specify
    {
      get { return this.specify; }
      set { this.specify = value; }
    }
    public int Status
    {
      get { return this.status; }
      set { this.status = value; }
    }
    public string ImageFile
    {
      get { return this.imageFile; }
      set { this.imageFile = value; }
    }
    public int ContractOut
    {
      get { return this.contractOut; }
      set { this.contractOut = value; }
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
    public DateTime UpdateDate
    {
      get { return this.updateDate; }
      set { this.updateDate = value; }
    }
    public int Primary
    {
      get { return this.primary; }
      set { this.primary = value; }
    }
    #endregion Properties
  }
}