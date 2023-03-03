/*
 * Created By   : 
 * Created Date : 20/03/2013
 * Description  : Historical Allocation
 * */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using System;
using System.Data;

namespace DaiCo.Planning
{
  public partial class viewPLN_20_005 : MainUserControl
  {
    #region fields
    public long wo = long.MinValue;
    public string carcassCode = string.Empty;
    public string group = string.Empty;
    public string category = string.Empty;
    public string altGroup = string.Empty;
    public string altCategory = string.Empty;
    #endregion fields

    #region function 
    /// <summary>
    /// Load history of allocation or register
    /// </summary>
    private void LoadHistory()
    {
      DBParameter[] inputParam = new DBParameter[6];
      inputParam[0] = new DBParameter("@Wo", DbType.Int64, this.wo);
      inputParam[1] = new DBParameter("@CarcassCode", DbType.String, this.carcassCode);
      inputParam[2] = new DBParameter("@MainGroup", DbType.String, this.group);
      inputParam[3] = new DBParameter("@MainCategory", DbType.String, this.category);
      inputParam[4] = new DBParameter("@AltGroup", DbType.String, this.altGroup);
      inputParam[5] = new DBParameter("@AltCategory", DbType.String, this.altCategory);

      DataTable dtAllocationHistory = DataBaseAccess.SearchStoreProcedureDataTable("spPLNWoodsHistoricalAllocation_Select", inputParam);
      ultraGridAllocationHistory.DataSource = dtAllocationHistory;
    }
    #endregion function

    #region event
    public viewPLN_20_005()
    {
      InitializeComponent();
    }

    private void viewPLN_07_005_Load(object sender, EventArgs e)
    {
      this.LoadHistory();
    }

    private void ultraGridHistory_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion event
  }
}
