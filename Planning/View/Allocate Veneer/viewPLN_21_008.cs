/*
  Author        : 
  Create date   : 30/05/2013
  Decription    : List Material Relation
 */
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_21_008 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init Data

    public viewPLN_21_008()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load From
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_21_008_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }

    #endregion Init Data

    #region Function

    private void LoadData()
    {
      btnSearch.Enabled = false;

      string commandText = string.Empty;
      commandText += " SELECT MAT.MaterialCode, MAT.MaterialNameEn, MAT.MaterialNameVn, MAT.Unit";
      commandText += " FROM VBOMMaterials MAT";
      commandText += " WHERE MAT.Warehouse = 2";
      commandText += "    AND (MAT.MaterialCode LIKE '%' + '" + txtMaterial.Text + "' +'%'";
      commandText += "      OR MAT.MaterialNameEn LIKE '%' + '" + txtMaterial.Text + "' +'%'";
      commandText += "      OR MAT.MaterialNameVn LIKE '%' + '" + txtMaterial.Text + "' +'%')";

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null)
      {
        ultData.DataSource = dt;
      }

      btnSearch.Enabled = true;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.LoadData();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
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
      string materialCode = row.Cells["MaterialCode"].Value.ToString();
      string materialName = row.Cells["MaterialNameVn"].Value.ToString();

      //1: Material Relation Detail
      viewPLN_21_009 uc = new viewPLN_21_009();
      uc.materialCode = materialCode;
      uc.materialName = materialName;
      WindowUtinity.ShowView(uc, "MATERIAL RELATION DETAIL", false, ViewState.MainWindow);
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

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.LoadData();
    }
  }
}
