using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace Component
{
  public partial class PostTranGrid : Infragistics.Win.UltraWinGrid.UltraGrid
  {
    #region Variables    
    private Infragistics.Win.UltraWinGrid.UltraCombo ucbAccountList;
    #endregion
    #region Public Property

    #endregion
    #region Constructor
    public PostTranGrid()
    {
      InitializeComponent();
    }

    public PostTranGrid(IContainer container)
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
      this.ucbAccountList = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ucbAccountList.Size = new System.Drawing.Size(400, 22);
      this.ucbAccountList.Visible = false;
      this.Controls.Add(this.ucbAccountList);

      this.DisplayLayout.GroupByBox.Hidden = true;
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
      this.InitializeLayout += PostTranGrid_InitializeLayout;
    }

    private void PostTranGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //Default init
      this.ResetDisplayLayout();
      this.DisplayLayout.AutoFitStyle = AutoFitStyle.None;
      this.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortMulti;
      this.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
      this.DisplayLayout.Override.SupportDataErrorInfo = SupportDataErrorInfo.RowsAndCells;
      this.DisplayLayout.Override.RowSelectorWidth = 32;
      this.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
      this.DisplayLayout.Override.AllowColSizing = AllowColSizing.Free;
      this.DisplayLayout.Override.CellAppearance.BorderColor = Color.Red;
      this.DisplayLayout.Override.FixedHeaderIndicator = FixedHeaderIndicator.None;
      this.DisplayLayout.Override.FixedCellSeparatorColor = Color.Transparent;
      this.DisplayLayout.Override.RowSizing = RowSizing.AutoFree;
      this.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand, AutoResizeColumnWidthOptions.All);
      this.StyleSetName = "Excel2013";
      this.SyncWithCurrencyManager = false;

      // Set Align
      for (int i = 0; i < this.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        Type colType = this.DisplayLayout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          this.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          this.DisplayLayout.Bands[0].Columns[i].Format = "#,##0.##";
        }
        if (colType == typeof(System.DateTime))
        {
          this.DisplayLayout.Bands[0].Columns[i].Format = ConstantClass.FORMAT_DATETIME;
        }
      }


      for (int i = 0; i < this.Rows.Count; i++)
      {
        this.Rows[i].Appearance.BackColor = (i % 2 > 0 ? System.Drawing.Color.White : System.Drawing.Color.LightCyan);
      }

      DBParameter[] inputParam = new DBParameter[2];
      string language = "VN";
      if (ConstantClass.CULTURE == CultureInfo.CreateSpecificCulture("en-US"))
      {
        language = "EN";
      }
      inputParam[0] = new DBParameter("@DocCode", DbType.AnsiString, 256, "PostTran");
      inputParam[1] = new DBParameter("@Language", DbType.AnsiString, 2, language);
      DataTable dtColumnAlias = DataBaseAccess.SearchStoreProcedureDataTable("spGetColumnAlias", inputParam);

      if (dtColumnAlias.Rows.Count > 0)
      {
        foreach (DataRow item in dtColumnAlias.Rows)
        {
          if (e.Layout.Bands[0].Columns[item["ColumnName"].ToString()] != null)
          {
            e.Layout.Bands[0].Columns[item["ColumnName"].ToString()].Header.Caption = item["Caption"].ToString();
            e.Layout.Bands[0].Columns[item["ColumnName"].ToString()].CellActivation
                    = item["Activation"] != DBNull.Value ? (Activation)Enum.ToObject(typeof(Activation), int.Parse(item["Activation"].ToString())) : Activation.ActivateOnly;
            e.Layout.Bands[0].Columns[item["ColumnName"].ToString()].Hidden = item["Hidden"] != DBNull.Value ? bool.Parse(item["Hidden"].ToString()) : false;
            if (item["Index"] != DBNull.Value)
              e.Layout.Bands[0].Columns[item["ColumnName"].ToString()].Header.VisiblePosition = int.Parse(item["Index"].ToString());
            if (item["Format"] != DBNull.Value)
              e.Layout.Bands[0].Columns[item["ColumnName"].ToString()].Format = item["Format"].ToString();
            if (item["Style"] != DBNull.Value)
              e.Layout.Bands[0].Columns[item["ColumnName"].ToString()].Style = (ColumnStyle)Enum.ToObject(typeof(ColumnStyle), int.Parse(item["Style"].ToString()));
          }
        }
      }
    }

    private void InitCommonData()
    {
      
    }
    public virtual void SetDataSource(int docTypePid, long docPid)
    {
      //1. Load source for account combobox
      string commandText = string.Format(@"SELECT AccountCode, AccountName FROM TblACCAccount ORDER BY AccountCode");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      this.ucbAccountList.DataSource = dtSource;      
      if (dtSource != null)
      {
        this.ucbAccountList.ValueMember = "AccountCode";
        this.ucbAccountList.DisplayMember = "AccountCode";
        this.ucbAccountList.DisplayLayout.Bands[0].Columns["AccountCode"].Width = 40;
      }
      this.ucbAccountList.DropDownWidth = 400;
      this.ucbAccountList.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
      this.ucbAccountList.DisplayLayout.Bands[0].ColHeadersVisible = false;
      this.ucbAccountList.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
      this.ucbAccountList.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      this.ucbAccountList.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;      

      //2. Load source for Posting Grid
      SqlDBParameter[] inputParam = new SqlDBParameter[2];
      inputParam[0] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, docTypePid);
      inputParam[1] = new SqlDBParameter("@Pid", SqlDbType.BigInt, docPid);
      DataSet dsSource = SqlDataBaseAccess.SearchStoreProcedure("spACCLoadTransaction", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        this.DataSource = dsSource.Tables[0];
        this.DisplayLayout.Bands[0].Columns["AccountCode"].ValueList = ucbAccountList;
      }
    }
    #endregion
  }
}
