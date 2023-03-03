using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.ComponentModel;
using System.Data;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace Component
{
  public partial class CusUltraGrid : Infragistics.Win.UltraWinGrid.UltraGrid, ICusControl
  {
    #region Variables
    protected String _UlDataSource;
    protected String _UlDataMember;
    protected int _UlLanguage;
    protected int _DocTypePid;
    protected long _DocPid;
    #endregion
    #region Public Property
    [Category("UL")]
    public String UlDataSource
    {
      get
      {
        return _UlDataSource;
      }
      set
      {
        _UlDataSource = value;
      }
    }

    [Category("UL")]
    public String UlDataMember
    {
      get
      {
        return _UlDataMember;
      }
      set
      {
        _UlDataMember = value;
      }
    }
    [Category("UL")]
    public int UlLanguage
    {
      get
      {
        return _UlLanguage;
      }
      set
      {
        _UlLanguage = value;
      }
    }
    [Category("UL")]
    public int DocTypePid
    {
      get
      {
        return _DocTypePid;
      }
      set
      {
        _DocTypePid = value;
      }
    }
    [Category("UL")]
    public long DocPid
    {
      get
      {
        return _DocPid;
      }
      set
      {
        _DocPid = value;
      }
    }
    #endregion
    #region Constructor
    public CusUltraGrid()
    {
      InitializeComponent();
    }

    public CusUltraGrid(IContainer container)
    {
      container.Add(this);

      InitializeComponent();

    }
    #endregion
    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {

    }



    #endregion
    #region Function for init GridControl
    public virtual void InitializeControl()
    {
      InitCommonData();


    }
    protected override void InitLayout()
    {
      base.InitLayout();
      CusInitializeLayout();
    }
    protected virtual void CusInitializeLayout()
    {
      //Do some thing

      this.InitializeLayout += CusUltraGrid_InitializeLayout;
    }

    private void CusUltraGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //Default init

      this.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
      this.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortMulti;
      this.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
      this.DisplayLayout.Override.SupportDataErrorInfo = SupportDataErrorInfo.RowsAndCells;
      this.DisplayLayout.Override.RowSelectorWidth = 32;
      this.StyleSetName = "Excel2013";
      e.Layout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True;

      // Set Align
      for (int i = 0; i < this.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        Type colType = this.DisplayLayout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          this.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          this.DisplayLayout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }


      for (int i = 0; i < this.Rows.Count; i++)
      {
        this.Rows[i].Appearance.BackColor = (i % 2 > 0 ? System.Drawing.Color.White : System.Drawing.Color.LightCyan);
      }

      DBParameter[] inputParam = new DBParameter[5];
      inputParam[0] = new DBParameter("@DataSourceName", DbType.AnsiString, 16, this.UlDataSource);
      DataTable dtDataSourceAliasList = DataBaseAccess.SearchStoreProcedureDataTable("spGetDataSourceAlias", inputParam);

      if (dtDataSourceAliasList.Rows.Count > 0)
      {
        string captionCol = "VNCaption";
        if (UlLanguage != 0) captionCol = "RegionCaption";
        foreach (DataRow item in dtDataSourceAliasList.Rows)
        {
          if (item["Name"] != DBNull.Value && e.Layout.Bands[0].Columns[item["Name"].ToString()] != null)
          {
            e.Layout.Bands[0].Columns[item["Name"].ToString()].Header.Caption = item[captionCol] != DBNull.Value ? item[captionCol].ToString() : string.Empty;
            e.Layout.Bands[0].Columns[item["Name"].ToString()].CellActivation
                    = item["Activation"] != DBNull.Value ? (Activation)Enum.ToObject(typeof(Activation), int.Parse(item["Activation"].ToString())) : Activation.ActivateOnly;
            e.Layout.Bands[0].Columns[item["Name"].ToString()].Hidden = item["Hidden"] != DBNull.Value ? bool.Parse(item["Hidden"].ToString()) : false;
            if (item["Index"] != DBNull.Value)
              e.Layout.Bands[0].Columns[item["Name"].ToString()].Header.VisiblePosition = int.Parse(item["Index"].ToString());
            if (item["Format"] != DBNull.Value)
              e.Layout.Bands[0].Columns[item["Name"].ToString()].Format = item["Format"].ToString();
            if (item["Style"] != DBNull.Value)
              e.Layout.Bands[0].Columns[item["Name"].ToString()].Style = (ColumnStyle)Enum.ToObject(typeof(ColumnStyle), int.Parse(item["Style"].ToString()));

          }
        }
      }


    }

    private void InitCommonData()
    {
    }
    public virtual void InitUIDataSource()
    {

    }
    #endregion
  }
}
