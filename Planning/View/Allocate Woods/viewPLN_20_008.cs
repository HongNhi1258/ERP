/*
  Author        : 
  Create date   : 02/03/2013
  Decription    : List Group Relation
 */
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;

namespace DaiCo.Planning
{
  public partial class viewPLN_20_008 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init Data

    public viewPLN_20_008()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load From
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_20_008_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }

    #endregion Init Data

    #region Function

    private void LoadData()
    {
      string commandText = string.Empty;
      commandText += " SELECT MGR.[Group], MGR.Name GroupName, MCA.Category, MCA.Name CategoryName";
      commandText += " FROM TblGNRMaterialGroup MGR";
      commandText += " INNER JOIN TblGNRMaterialCategory MCA ON MGR.[Group] = MCA.[Group]";
      commandText += " WHERE MGR.Warehouse = 3";

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt == null)
      {
        return;
      }
      ultData.DataSource = dt;
    }
    #endregion Function

    #region Evnet

    /// <summary>
    /// Double Click Gird
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultData.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (ultData.Selected.Rows[0].ParentRow == null) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ParentRow;
      string parentGroup = row.Cells["Group"].Value.ToString();
      string parentCatagory = row.Cells["Category"].Value.ToString();
      string categoryName = row.Cells["CategoryName"].Value.ToString();

      //1: Group Relation Detail
      viewPLN_20_009 uc = new viewPLN_20_009();
      uc.groupParent = parentGroup;
      uc.categoryParent = parentCatagory;
      uc.categoryName = categoryName;

      WindowUtinity.ShowView(uc, "GROUP RELATION DETAIL", false, ViewState.MainWindow);
    }

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    #endregion Event
  }
}
