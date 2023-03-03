/*
  Author  : Vo Hoa Lu 
  Email   : luvh_it@daico-furniture.com
  Date    : 22/09/2010
  Company :  Dai Co
*/
using DaiCo.Application;

namespace DaiCo.Objects
{
  public class QCDCheckListGroup : IObject
  {
    #region Fields
    private long pid;
    private string description;
    private string descriptionEN;
    private int type;
    private int isDeleted;
    private string tolerence;
    #endregion Fields

    #region Constructors
    public QCDCheckListGroup()
    {
      this.pid = long.MinValue;
      this.description = string.Empty;
      this.descriptionEN = string.Empty;
      this.type = int.MinValue;
      this.isDeleted = int.MinValue;
    }
    public object Clone()
    {
      QCDCheckListGroup obj = new QCDCheckListGroup();
      obj.Pid = this.pid;
      obj.Description = this.description;
      obj.DescriptionEN = this.descriptionEN;
      obj.Type = this.type;
      obj.IsDeleted = this.isDeleted;
      return obj;
    }
    public string GetTableName()
    {
      return "TblQCDCheckListGroup";
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
    public string Description
    {
      get { return this.description; }
      set { this.description = value; }
    }
    public string DescriptionEN
    {
      get { return this.descriptionEN; }
      set { this.descriptionEN = value; }
    }
    public string Tolerence
    {
      get
      {
        return this.tolerence;
      }
      set
      {
        this.tolerence = value;
      }
    }
    public int Type
    {
      get { return this.type; }
      set { this.type = value; }
    }
    public int IsDeleted
    {
      get { return this.isDeleted; }
      set { this.isDeleted = value; }
    }
    #endregion Properties
  }
}