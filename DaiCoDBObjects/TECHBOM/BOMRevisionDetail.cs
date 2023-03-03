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
  public class BOMRevisionDetail : IObject
  {
    #region Fields
    private int pID;
    private int revisionPID;
    private string note;
    private int changeKind;
    private string linked;
    private DateTime createDate;
    private int createBy;
    private DateTime updateDate;
    private int updateBy;
    #endregion Fields

    #region Constructors
    public BOMRevisionDetail()
    {
      this.pID = int.MinValue;
      this.revisionPID = int.MinValue;
      this.note = string.Empty;
      this.changeKind = int.MinValue;
      this.linked = string.Empty;
      this.createDate = DateTime.MinValue;
      this.createBy = int.MinValue;
      this.updateDate = DateTime.MinValue;
      this.updateBy = int.MinValue;
    }
    public object Clone()
    {
      BOMRevisionDetail obj = new BOMRevisionDetail();
      obj.PID = this.pID;
      obj.RevisionPID = this.revisionPID;
      obj.Note = this.note;
      obj.ChangeKind = this.changeKind;
      obj.Linked = this.linked;
      obj.CreateDate = this.createDate;
      obj.CreateBy = this.createBy;
      obj.UpdateDate = this.updateDate;
      obj.UpdateBy = this.updateBy;
      return obj;
    }
    public string GetTableName()
    {
      return "TblBOMRevisionDetail";
    }
    public string[] ObjectKey()
    {
      return new string[] { "PID" };
    }
    #endregion Constructors

    #region Properties

    public int PID
    {
      get { return this.pID; }
      set { this.pID = value; }
    }
    public int RevisionPID
    {
      get { return this.revisionPID; }
      set { this.revisionPID = value; }
    }
    public string Note
    {
      get { return this.note; }
      set { this.note = value; }
    }
    public int ChangeKind
    {
      get { return this.changeKind; }
      set { this.changeKind = value; }
    }
    public string Linked
    {
      get { return this.linked; }
      set { this.linked = value; }
    }
    public DateTime CreateDate
    {
      get { return this.createDate; }
      set { this.createDate = value; }
    }
    public int CreateBy
    {
      get { return this.createBy; }
      set { this.createBy = value; }
    }
    public DateTime UpdateDate
    {
      get { return this.updateDate; }
      set { this.updateDate = value; }
    }
    public int UpdateBy
    {
      get { return this.updateBy; }
      set { this.updateBy = value; }
    }
    #endregion Properties
  }
}