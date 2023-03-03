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
  public class BOMComponentInfo : IObject
  {
    #region Fields
    private long pid;
    private int no;
    private string componentCode;
    private string componentName;
    private string alternative;
    private double length;
    private double width;
    private double thickness;
    private int compGroup;
    private int adjective;
    private string remark;
    private string link;
    private int confirm;
    private int createBy;
    private DateTime createDate;
    private int updateBy;
    private DateTime updateDate;
    private int contractOut;
    private string componentNameVN;
    #endregion Fields

    #region Constructors
    public BOMComponentInfo()
    {
      this.pid = long.MinValue;
      this.no = int.MinValue;
      this.componentCode = string.Empty;
      this.componentName = string.Empty;
      this.alternative = string.Empty;
      this.length = double.MinValue;
      this.width = double.MinValue;
      this.thickness = double.MinValue;
      this.compGroup = int.MinValue;
      this.adjective = int.MinValue;
      this.remark = string.Empty;
      this.link = string.Empty;
      this.confirm = int.MinValue;
      this.createBy = int.MinValue;
      this.createDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.contractOut = int.MinValue;
      this.componentNameVN = string.Empty;
    }
    public object Clone()
    {
      BOMComponentInfo obj = new BOMComponentInfo();
      obj.Pid = this.pid;
      obj.No = this.no;
      obj.ComponentCode = this.componentCode;
      obj.ComponentName = this.componentName;
      obj.Alternative = this.alternative;
      obj.Length = this.length;
      obj.Width = this.width;
      obj.Thickness = this.thickness;
      obj.CompGroup = this.compGroup;
      obj.Adjective = this.adjective;
      obj.Remark = this.remark;
      obj.Link = this.link;
      obj.Confirm = this.confirm;
      obj.CreateBy = this.createBy;
      obj.CreateDate = this.createDate;
      obj.UpdateBy = this.updateBy;
      obj.UpdateDate = this.updateDate;
      obj.ContractOut = this.contractOut;
      obj.ComponentNameVN = this.componentNameVN;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMComponentInfo";
    }
    public string[] ObjectKey()
    {
      return new string[] { "PID" };
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
    public string ComponentCode
    {
      get { return this.componentCode; }
      set { this.componentCode = value; }
    }
    public string ComponentName
    {
      get { return this.componentName; }
      set { this.componentName = value; }
    }
    public string Alternative
    {
      get { return this.alternative; }
      set { this.alternative = value; }
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
    public int CompGroup
    {
      get { return this.compGroup; }
      set { this.compGroup = value; }
    }
    public int Adjective
    {
      get { return this.adjective; }
      set { this.adjective = value; }
    }
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
    }
    public string Link
    {
      get { return this.link; }
      set { this.link = value; }
    }
    public int Confirm
    {
      get { return this.confirm; }
      set { this.confirm = value; }
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
    public int ContractOut
    {
      get { return this.contractOut; }
      set { this.contractOut = value; }
    }
    public string ComponentNameVN
    {
      get { return this.componentNameVN; }
      set { this.componentNameVN = value; }
    }
    #endregion Properties
  }
}