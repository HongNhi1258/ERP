/*
  Author      : 
  Date        : 26/08/2013
  Description : Definition Image Veneer
  Standard From : viewGNR_90_003
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_21_014 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init

    /// <summary>
    /// viewPLN_21_014
    /// </summary>
    public viewPLN_21_014()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load
    /// </summary>
    private void viewPLN_21_014_Load(object sender, EventArgs e)
    {

    }
    #endregion Init

    #region Function

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      this.btnSearch.Enabled = false;
      string comandText = string.Empty;
      comandText += " SELECT MAT.MaterialCode, MAT.NameEN, MAT.NameVN, UNI.Symbol Unit, ISNULL(MAT.ImageVeneer, 0) [Image]";
      comandText += " FROM TblGNRMaterialInformation MAT";
      comandText += " 	INNER JOIN TblGNRMaterialGroup GRP ON GRP.[Group] = MAT.[Group] AND GRP.Warehouse = 2";
      comandText += " 	LEFT JOIN TblGNRMaterialUnit UNI ON UNI.Pid = MAT.Unit";
      comandText += " WHERE MAT.MaterialCode LIKE '%" + txtMaterial.Text + "%' ";
      comandText += "   OR MAT.NameEN LIKE '%" + txtMaterial.Text + "%'";
      comandText += "   OR MAT.NameVN LIKE '%" + txtMaterial.Text + "%'";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(comandText);
      if (dtSource != null)
      {
        ultData.DataSource = dtSource;
      }
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      DataTable dtSource = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          // Input
          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@MaterialCode", DbType.String, row["MaterialCode"].ToString());
          inputParam[1] = new DBParameter("@Image", DbType.Int32, DBConvert.ParseInt(row["Image"].ToString()));
          inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          // Output
          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          // Execute
          DataBaseAccess.ExecuteStoreProcedure("spPLNVeneerDefinitionImageVeneer_Update", inputParam, outputParam);
          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result <= 0)
          {
            return false;
          }
        }
      }
      return true;
    }
    #endregion Function

    #region Event

    /// <summary>
    /// Init Layout 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      // Allow update, delete, add new
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;

      // Set Column Style
      e.Layout.Bands[0].Columns["Image"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Image"].DefaultCellValue = 0;

      // Read only
      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameVN"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["unit"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Search 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
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

    /// <summary>
    /// Save
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      // Save Data
      bool success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      // Search Data
      this.Search();
    }

    /// <summary>
    /// ProcessCmdKey
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    #endregion Event
  }
}
