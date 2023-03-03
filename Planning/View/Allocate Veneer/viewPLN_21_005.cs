/*
 * Created By   : 
 * Created Date : 11/6/2013
 * Description  : Historical Allocation
 * */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using System;
using System.Data;

namespace DaiCo.Planning
{
  public partial class viewPLN_21_005 : MainUserControl
  {
    #region fields
    public long wo = long.MinValue;
    public string carcassCode = string.Empty;
    public string mainCode = string.Empty;
    public string altCode = string.Empty;
    #endregion fields

    #region function 
    /// <summary>
    /// Load history of allocation or register
    /// </summary>
    private void LoadHistory()
    {
      DBParameter[] inputParam = new DBParameter[4];
      inputParam[0] = new DBParameter("@Wo", DbType.Int64, this.wo);
      inputParam[1] = new DBParameter("@CarcassCode", DbType.String, this.carcassCode);
      inputParam[2] = new DBParameter("@MainCode", DbType.String, this.mainCode);
      inputParam[3] = new DBParameter("@AltCode", DbType.String, this.altCode);

      DataTable dtAllocationHistory = DataBaseAccess.SearchStoreProcedureDataTable("spPLNVeneerHistoricalAllocation_Select", inputParam);
      ultraGridAllocationHistory.DataSource = dtAllocationHistory;
    }
    #endregion function

    #region event
    public viewPLN_21_005()
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
