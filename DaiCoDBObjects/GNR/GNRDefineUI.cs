/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 03.11.2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class GNRDefineUI : IObject
  {
    #region Fields
    private long pid;
    private string uICode;
    private string uIParam;
    private string nameSpace;
    private string title;
    private string description;
    private string otherInfo;
    private int viewState;
    private int windowState;
    private long parentPid;
    private int orderBy;
    private int isActive;
    #endregion Fields

    #region Constructors
    public GNRDefineUI()
    {
      this.pid = long.MinValue;
      this.uICode = string.Empty;
      this.uIParam = string.Empty;
      this.nameSpace = string.Empty;
      this.title = string.Empty;
      this.description = string.Empty;
      this.otherInfo = string.Empty;
      this.viewState = int.MinValue;
      this.windowState = int.MinValue;
      this.parentPid = long.MinValue;
      this.orderBy = int.MinValue;
      this.isActive = int.MinValue;
    }
    public object Clone()
    {
      GNRDefineUI obj = new GNRDefineUI();
      obj.Pid = this.pid;
      obj.UICode = this.uICode;
      obj.UIParam = this.uIParam;
      obj.NameSpace = this.nameSpace;
      obj.Title = this.title;
      obj.Description = this.description;
      obj.OtherInfo = this.otherInfo;
      obj.ViewState = this.viewState;
      obj.WindowState = this.windowState;
      obj.ParentPid = this.parentPid;
      obj.OrderBy = this.orderBy;
      obj.IsActive = this.isActive;
      return obj;
    }
    public string GetTableName()
    {
      return "TblGNRDefineUI";
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
    public string UICode
    {
      get { return this.uICode; }
      set { this.uICode = value; }
    }
    public string UIParam
    {
      get { return this.uIParam; }
      set { this.uIParam = value; }
    }
    public string NameSpace
    {
      get { return this.nameSpace; }
      set { this.nameSpace = value; }
    }
    public string Title
    {
      get { return this.title; }
      set { this.title = value; }
    }
    public string Description
    {
      get { return this.description; }
      set { this.description = value; }
    }
    public string OtherInfo
    {
      get { return this.otherInfo; }
      set { this.otherInfo = value; }
    }
    public int ViewState
    {
      get { return this.viewState; }
      set { this.viewState = value; }
    }
    public int WindowState
    {
      get { return this.windowState; }
      set { this.windowState = value; }
    }
    public long ParentPid
    {
      get { return this.parentPid; }
      set { this.parentPid = value; }
    }
    public int OrderBy
    {
      get { return this.orderBy; }
      set { this.orderBy = value; }
    }
    public int IsActive
    {
      get { return this.isActive; }
      set { this.isActive = value; }
    }
    #endregion Properties
  }
}