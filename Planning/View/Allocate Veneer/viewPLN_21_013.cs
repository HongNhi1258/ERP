/*
 * Created By   : 
 * Created Date : 30/07/2013
 * Description  : Planning Allocate WO (Is SubCon) Veneer
 * */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_21_013 : MainUserControl
  {
    #region field
    public viewPLN_21_013()
    {
      InitializeComponent();
    }
    #endregion field

    #region init
    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_21_013_Load(object sender, EventArgs e)
    {
      // Load ItemCode
      this.LoadCarcass();

      // Load Group
      this.LoadMaterialCode();
    }
    #endregion init

    #region function
    /// <summary>
    /// Load Carcass
    /// </summary>
    private void LoadCarcass()
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT IIF.CarcassCode ";
      commandText += " FROM TblPLNWOInfoDetailGeneral WO ";
      commandText += " INNER JOIN TblBOMItemInfo IIF ON WO.ItemCode = IIF.ItemCode";
      commandText += " 	AND WO.Revision = IIF.Revision";
      commandText += " 	AND WO.IsSubCon = 1";
      commandText += " 	ORDER BY IIF.CarcassCode";

      DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucCarcassCode.DataSource = dtCarcass;
      ucCarcassCode.ColumnWidths = "200;400";
      ucCarcassCode.DataBind();
      ucCarcassCode.ValueMember = "CarcassCode";
      ucCarcassCode.DisplayMember = "CarcassCode";
    }

    /// <summary>
    /// Load Carcass Base On WO
    /// </summary>
    private void LoadCarcassBaseOnWO()
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT IIF.CarcassCode";
      commandText += " FROM TblPLNWOInfoDetailGeneral WO ";
      commandText += " INNER JOIN TblBOMItemInfo IIF ON WO.ItemCode = IIF.ItemCode ";
      commandText += " AND WO.Revision = IIF.Revision";
      commandText += " AND WO.IsSubCon = 1";
      commandText += " WHERE 1 = 1 ";

      if (txtWOFrom.Text.Trim().Length > 0 && DBConvert.ParseInt(txtWOFrom.Text.Trim()) != int.MinValue)
      {
        commandText += " 	AND WO.WoInfoPID >= " + DBConvert.ParseInt(txtWOFrom.Text.Trim());
      }

      if (txtWOTo.Text.Trim().Length > 0 && DBConvert.ParseInt(txtWOTo.Text.Trim()) != int.MinValue)
      {
        commandText += " AND	WO.WoInfoPID <= " + DBConvert.ParseInt(txtWOTo.Text.Trim());
      }
      commandText += " ORDER BY IIF.CarcassCode";

      DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucCarcassCode.DataSource = dtCarcass;
      ucCarcassCode.ColumnWidths = "200;400";
      ucCarcassCode.DataBind();
      ucCarcassCode.ValueMember = "CarcassCode";
      ucCarcassCode.DisplayMember = "CarcassCode";
    }

    /// <summary>
    /// Load Material Code
    /// </summary>
    private void LoadMaterialCode()
    {
      string commandText = string.Empty;
      commandText += " SELECT MAT.MaterialCode Code, MAT.NameEN GroupName ";
      commandText += " FROM TblGNRMaterialInformation MAT ";
      commandText += " 	INNER JOIN TblGNRMaterialGroup GRP ON MAT.[Group] = GRP.[Group] ";
      commandText += "          									AND GRP.Warehouse = 2";
      commandText += " WHERE MAT.IsControl = 1 ";
      commandText += "      ORDER BY MAT.MaterialCode ";

      DataTable dtGroup = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucMaterialCode.DataSource = dtGroup;
      ucMaterialCode.ColumnWidths = "200;400";
      ucMaterialCode.DataBind();
      ucMaterialCode.ValueMember = "Code";
      ucMaterialCode.DisplayMember = "Code";
    }

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      DBParameter[] inputParam = new DBParameter[4];

      // WO From
      if (txtWOFrom.Text.Trim().Length > 0 && DBConvert.ParseInt(txtWOFrom.Text.Trim()) != int.MinValue)
      {
        inputParam[0] = new DBParameter("@WOFrom", DbType.Int32, DBConvert.ParseInt(txtWOFrom.Text.Trim()));
      }

      // WO To
      if (txtWOTo.Text.Trim().Length > 0 && DBConvert.ParseInt(txtWOTo.Text.Trim()) != int.MinValue)
      {
        inputParam[1] = new DBParameter("@WOTo", DbType.Int32, DBConvert.ParseInt(txtWOTo.Text.Trim()));
      }

      // Carcass Code
      if (this.txtCarcassCode.Text.Trim().Length > 0)
      {
        inputParam[2] = new DBParameter("@CarcassCode", DbType.AnsiString, 256, this.txtCarcassCode.Text.Trim());
      }

      // Group
      if (txtMaterial.Text.Trim().Length > 0)
      {
        inputParam[3] = new DBParameter("@Group", DbType.AnsiString, 128, txtMaterial.Text.Trim());
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNVeneerPlanningAllocateWOIsSubCon_Select", inputParam);
      ultraGridWOMaterialDetail.DataSource = dtSource;

      for (int i = 0; i < ultraGridWOMaterialDetail.Rows.Count; i++)
      {
        if (DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["QtyAllocated"].Value.ToString()) > 0)
        {
          ultraGridWOMaterialDetail.Rows[i].Appearance.BackColor = Color.Yellow;
          ultraGridWOMaterialDetail.Rows[i].Cells["Auto"].Activation = Activation.ActivateOnly;
        }
      }
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      for (int i = 0; i < ultraGridWOMaterialDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultraGridWOMaterialDetail.Rows[i];
        if (DBConvert.ParseDouble(row.Cells["QtyAllocated"].Value.ToString()) == 0 &&
            DBConvert.ParseDouble(row.Cells["QtyAllocate"].Value.ToString()) > 0)
        {
          string storeName = "spPLNVeneerAllocateForWOIsSubCon_Insert";
          DBParameter[] inputParam = new DBParameter[6];
          inputParam[0] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(row.Cells["WO"].Value.ToString()));
          inputParam[1] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CarcassCode"].Value.ToString());
          inputParam[2] = new DBParameter("@MainCode", DbType.String, row.Cells["MainCode"].Value.ToString());
          if (row.Cells["AltCode"].Value.ToString().Trim().Length > 0)
          {
            inputParam[3] = new DBParameter("@AltCode", DbType.String, row.Cells["AltCode"].Value.ToString());
          }
          inputParam[4] = new DBParameter("@QtyAllocate", DbType.Double, DBConvert.ParseDouble(row.Cells["QtyAllocate"].Value.ToString()));
          inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, int.MinValue);

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
          if (result <= 0)
          {
            return false;
          }
        }
      }
      return true;
    }
    /// <summary>
    /// Search Enter
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }
    #endregion function

    #region event
    /// <summary>
    /// Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.btnSearch.Enabled = false;
      // Search
      this.Search();
      this.btnSearch.Enabled = true;
    }

    /// <summary>
    /// Save Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      bool success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
        return;
      }
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
    /// Check Carcass
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkShowCarcassListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucCarcassCode.Visible = chkShowItemListBox.Checked;
    }

    /// <summary>
    /// Check GroupCode
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkShowMaterialListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucMaterialCode.Visible = chkShowMaterialListBox.Checked;
    }

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridWOMaterialDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CarQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyUnit"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CurrentBOM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyAllocated"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["StockQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyAllocate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Auto"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 2; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["QtyAllocate"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Change Carcass
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    private void ucUltraListCarcass_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtCarcassCode.Text = ucCarcassCode.SelectedValue;
    }

    /// <summary>
    /// Change GroupCode
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    private void ucUltraListMaterialGroup_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtMaterial.Text = ucMaterialCode.SelectedValue;
    }

    /// <summary>
    /// WO From Leave
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtWOFrom_Leave(object sender, EventArgs e)
    {
      // Load Carcass Base On WO
      this.LoadCarcassBaseOnWO();
    }

    /// <summary>
    /// WO To Leave
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtWOTo_Leave(object sender, EventArgs e)
    {
      // Load Carcass Base On WO
      this.LoadCarcassBaseOnWO();
    }

    /// <summary>
    /// Cell Change Auto
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridWOMaterialDetail_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      int value = DBConvert.ParseInt(e.Cell.Text.ToString());
      if (string.Compare(columnName, "Auto", true) == 0)
      {
        if (value == 1)
        {
          e.Cell.Row.Cells["QtyAllocate"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["CurrentBOM"].Value.ToString()) -
                                                  DBConvert.ParseDouble(e.Cell.Row.Cells["QtyAllocated"].Value.ToString());
        }
        else
        {
          e.Cell.Row.Cells["QtyAllocate"].Value = 0;
        }
      }
    }

    /// <summary>
    /// Check All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkAll_CheckedChanged(object sender, EventArgs e)
    {
      int check = (chkAll.Checked ? 1 : 0);
      for (int i = 0; i < ultraGridWOMaterialDetail.Rows.Count; i++)
      {
        if (DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["QtyAllocated"].Value.ToString()) == 0)
        {
          ultraGridWOMaterialDetail.Rows[i].Cells["Auto"].Value = check;
          if (check == 0)
          {
            ultraGridWOMaterialDetail.Rows[i].Cells["QtyAllocate"].Value = 0;
          }
          else
          {
            ultraGridWOMaterialDetail.Rows[i].Cells["QtyAllocate"].Value = DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["CurrentBOM"].Value.ToString()) -
                                                                           DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["QtyAllocated"].Value.ToString());
          }
        }
        else
        {
          ultraGridWOMaterialDetail.Rows[i].Cells["Auto"].Value = 0;
        }
      }
    }
    #endregion event 
  }
}
