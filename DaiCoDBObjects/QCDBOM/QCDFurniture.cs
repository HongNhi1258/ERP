/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 24/09/2010
  Company :  Dai Co
*/
using DaiCo.Application;
using System;

namespace DaiCo.Objects
{
  public class QCDFurniture : IObject
  {
    #region Fields
    private long pid;
    private string furnitureCode;
    private string itemCode;
    private int revision;
    private long workOrderPid;
    private DateTime createDate;
    private string remark;
    #endregion Fields

    #region Constructors
    public QCDFurniture()
    {
      this.pid = long.MinValue;
      this.furnitureCode = string.Empty;
      this.itemCode = string.Empty;
      this.revision = int.MinValue;
      this.workOrderPid = long.MinValue;
      this.createDate = DateTime.MinValue;
      this.remark = string.Empty;
    }
    public object Clone()
    {
      QCDFurniture obj = new QCDFurniture();
      obj.Pid = this.pid;
      obj.FurnitureCode = this.furnitureCode;
      obj.ItemCode = this.itemCode;
      obj.Revision = this.revision;
      obj.WorkOrderPid = this.workOrderPid;
      obj.CreateDate = this.createDate;
      obj.Remark = this.remark;
      return obj;
    }
    public string GetTableName()
    {
      return "TblQCDFurniture";
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
    public string FurnitureCode
    {
      get { return this.furnitureCode; }
      set { this.furnitureCode = value; }
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
    public long WorkOrderPid
    {
      get { return this.workOrderPid; }
      set { this.workOrderPid = value; }
    }
    public DateTime CreateDate
    {
      get { return this.createDate; }
      set { this.createDate = value; }
    }
    public string Remark
    {
      get { return this.remark; }
      set { this.remark = value; }
    }
    #endregion Properties
  }
}