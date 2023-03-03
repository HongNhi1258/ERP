/*
  Author      : 
  Date        : 11/06/2013
  Description : Request Online (Allocate Special)
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using System.IO;
using System.Diagnostics;
namespace DaiCo.General
{
  public partial class viewGNR_06_006 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;

    #endregion Field

    #region Init

    public viewGNR_06_006()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_06_006_Load(object sender, EventArgs e)
    {
      // Load Carcass
      this.LoadCarcassBaseOnWO();
      // Load MaterialCode
      this.LoadMaterialCode();
    }

    /// <summary>
    /// Load MateialCode
    /// </summary>
    private void LoadMaterialCode()
    {
      string commandText = string.Empty;
      commandText = " SELECT MaterialCode, MaterialCode + ' - ' + MaterialNameVn AS Name FROM VBOMMaterials WHERE Warehouse = 2";
      DataTable dtMaterialCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBMaterialCode.DataSource = dtMaterialCode;
      ultCBMaterialCode.ValueMember = "MaterialCode";
      ultCBMaterialCode.DisplayMember = "Name";
      ultCBMaterialCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBMaterialCode.DisplayLayout.Bands[0].Columns["MaterialCode"].Hidden = true;
    }

    /// <summary>
    /// Load Carcass Base On WO
    /// </summary>
    private void LoadCarcassBaseOnWO()
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT ALLO.CarcassCode, CAR.[Description] ";
      commandText += " FROM TblPLNVeneerAllocateWorkOrderSummary ALLO ";
      commandText += " 	INNER JOIN TblBOMCarcass CAR ON ALLO.CarcassCode = CAR.CarcassCode ";
      commandText += " WHERE 1 = 1 ";
      if (txtWOFrom.Text.Trim().Length > 0 && DBConvert.ParseInt(txtWOFrom.Text.Trim()) != int.MinValue)
      {
        commandText += " 	AND ALLO.WO >= " + DBConvert.ParseInt(txtWOFrom.Text.Trim());
      }

      if (txtWOTo.Text.Trim().Length > 0 && DBConvert.ParseInt(txtWOTo.Text.Trim()) != int.MinValue)
      {
        commandText += " 	AND ALLO.WO <= " + DBConvert.ParseInt(txtWOTo.Text.Trim());
      }
      commandText += " ORDER BY ALLO.CarcassCode ";

      DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucCarcass.DataSource = dtCarcass;
      ucCarcass.ColumnWidths = "200;400";
      ucCarcass.DataBind();
      ucCarcass.ValueMember = "CarcassCode";
      ucCarcass.DisplayMember = "CarcassCode";
    }

    #endregion Init

    #region Function
    /// <summary>
    /// Enter Search
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

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      DBParameter[] input = new DBParameter[8];
      // WO From
      if (txtWOFrom.Text.Length > 0 && DBConvert.ParseInt(txtWOFrom.Text) != int.MinValue)
      {
        input[0] = new DBParameter("@WOFrom", DbType.Int32, DBConvert.ParseInt(txtWOFrom.Text));
      }
      // WO To
      if (txtWOTo.Text.Length > 0 && DBConvert.ParseInt(txtWOTo.Text) != int.MinValue)
      {
        input[1] = new DBParameter("@WOTo", DbType.Int32, DBConvert.ParseInt(txtWOTo.Text));
      }
      // CarcassCode
      if (txtCarcassCode.Text.Length > 0)
      {
        input[2] = new DBParameter("@CarcassCode", DbType.String, txtCarcassCode.Text);
      }
      // MaterialCode
      if (ultCBMaterialCode.Text.Length > 0 && ultCBMaterialCode.Value != null)
      {
        input[3] = new DBParameter("@MaterialCode", DbType.String, ultCBMaterialCode.Value.ToString());
      }
      // Length From
      if (txtLength.Text.Length > 0 && DBConvert.ParseDouble(txtLength.Text) != Double.MinValue)
      {
        input[4] = new DBParameter("@Length", DbType.Double, DBConvert.ParseDouble(txtLength.Text));
      }
      // Width From
      if (txtWidth.Text.Length > 0 && DBConvert.ParseDouble(txtWidth.Text) != Double.MinValue)
      {
        input[5] = new DBParameter("@Width", DbType.Double, DBConvert.ParseDouble(txtWidth.Text));
      }
      // Length To
      if (txtLengthTo.Text.Length > 0 && DBConvert.ParseDouble(txtLengthTo.Text) != Double.MinValue)
      {
        input[6] = new DBParameter("@LengthTo", DbType.Double, DBConvert.ParseDouble(txtLengthTo.Text));
      }
      // Width To
      if (txtWidthTo.Text.Length > 0 && DBConvert.ParseDouble(txtWidthTo.Text) != Double.MinValue)
      {
        input[7] = new DBParameter("@WidthTo", DbType.Double, DBConvert.ParseDouble(txtWidthTo.Text));
      }

      DataTable dtMain = DataBaseAccess.SearchStoreProcedureDataTable("spGNRVeneerGetRequestOnlineSpecial_Select", input);
      if (dtMain != null)
      {
        ultData.DataSource = dtMain;
        // To mau LotNoID co Image
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];
          string lotNoId = row.Cells["LotNoId"].Value.ToString();
          string locationImage = FunctionUtility.GetImagePathByPid(11) + string.Format(@"\{0}", lotNoId);
          if (Directory.Exists(locationImage))
          {
            row.Appearance.BackColor = Color.Yellow;
          }
        }
      }
    }

    static bool IsValidImage(string filePath)
    {
      return File.Exists(filePath);
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if(DBConvert.ParseInt(row.Cells["QtyRequest"].Value.ToString()) < 0 ||
          DBConvert.ParseInt(row.Cells["QtyRequest"].Value.ToString()) > DBConvert.ParseInt(row.Cells["Qty"].Value.ToString()))
        {
          message = "QtyRequest";
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      DataTable dtMain = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtMain.Rows.Count; i++)
      {
        DataRow row = dtMain.Rows[i];
        if (DBConvert.ParseInt(row["QtyRequest"].ToString()) > 0)
        {
          DBParameter[] input = new DBParameter[11];
          input[0] = new DBParameter("@VRNPid", DbType.Int64, this.pid);
          input[1] = new DBParameter("@Type", DbType.Int32, 5);
          input[2] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(row["WO"].ToString()));
          input[3] = new DBParameter("@CarcassCode", DbType.String, row["CarcassCode"].ToString());
          input[4] = new DBParameter("@LotNoId", DbType.String, row["LotNoId"].ToString());
          input[5] = new DBParameter("@MaterialCode", DbType.String, row["MaterialCode"].ToString());
          input[6] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(row["QtyRequest"].ToString()));
          input[7] = new DBParameter("@QtyM2", DbType.Double, DBConvert.ParseDouble(row["QtyRequestM2"].ToString()));
          input[8] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(row["LocationPid"].ToString()));
          input[9] = new DBParameter("@Dimension", DbType.Int64, DBConvert.ParseLong(row["DimensionPid"].ToString()));
          if (row["MainCode"].ToString().Trim().Length > 0)
          {
            input[10] = new DBParameter("@MainCode", DbType.String, row["MainCode"].ToString());
          }

          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure("spGNRVeneerRequisitionNoteDetailSpecial_Edit", input, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
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
    /// Init Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      ControlUtility.SetPropertiesUltraGrid(ultData);

      e.Layout.Bands[0].Columns["DimensionPid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Select"].Header.Caption = "Auto";
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Sheet";
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyM2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyRequest"].Header.Caption = "Request(Sheet)";
      e.Layout.Bands[0].Columns["QtyRequest"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyRequestM2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["QtyRequestM2"].CellActivation = Activation.ActivateOnly;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 3; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["QtyRequest"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.00}";

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["QtyRequestM2"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.00}";

      e.Layout.Bands[0].Columns["QtyRequest"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["QtyRequestM2"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Check Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkAuto_CheckedChanged(object sender, EventArgs e)
    {
      int selected = 0;
      if (chkAuto.Checked)
      {
        selected = 1;
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        ultData.Rows[i].Cells["Select"].Value = selected;
        if (selected == 1)
        {
          ultData.Rows[i].Cells["QtyRequest"].Value = DBConvert.ParseInt(ultData.Rows[i].Cells["Qty"].Value.ToString());
          ultData.Rows[i].Cells["QtyRequestM2"].Value = DBConvert.ParseDouble(ultData.Rows[i].Cells["QtyM2"].Value.ToString());
        }
        else
        {
          ultData.Rows[i].Cells["QtyRequest"].Value = 0;
          ultData.Rows[i].Cells["QtyRequestM2"].Value = 0;
        }
      }
    }

    /// <summary>
    /// Afte Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "qtyrequest":
          if (DBConvert.ParseInt(row.Cells["QtyRequest"].Value.ToString()) < 0 ||
              DBConvert.ParseInt(row.Cells["QtyRequest"].Value.ToString()) > DBConvert.ParseInt(row.Cells["Qty"].Value.ToString()))
          {
            row.Cells["QtyRequest"].Appearance.BackColor = Color.Yellow;
            row.Cells["QtyRequestM2"].Appearance.BackColor = Color.Yellow;
          }
          else
          {
            row.Cells["QtyRequest"].Appearance.BackColor = Color.LightBlue;
            row.Cells["QtyRequestM2"].Appearance.BackColor = Color.LightBlue;
          }

          double totalRequestM2 = Math.Round(DBConvert.ParseInt(row.Cells["QtyRequest"].Value.ToString())
                                  * DBConvert.ParseDouble(row.Cells["Length"].Value.ToString())
                                  * DBConvert.ParseDouble(row.Cells["Width"].Value.ToString()) / 1000000, 2);
          row.Cells["QtyRequestM2"].Value = totalRequestM2;
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Change Carcass
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    private void ucCarcassCode_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtCarcassCode.Text = ucCarcass.SelectedValue;
    }

    private void txtWOFrom_Leave(object sender, EventArgs e)
    {
      // Load Carcass Base On WO
      this.LoadCarcassBaseOnWO();
    }

    private void txtWOTo_Leave(object sender, EventArgs e)
    {
      // Load Carcass Base On WO
      this.LoadCarcassBaseOnWO();
    }

    // Check Show Group
    private void chkShowGroup_CheckedChanged(object sender, EventArgs e)
    {
      ucCarcass.Visible = chkShowCarcass.Checked;
    }

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

      UltraGridRow row = ultData.Selected.Rows[0];
      string lotNoId = row.Cells["LotNoId"].Value.ToString();
      try
      {
        string locationImage = @FunctionUtility.GetImagePathByPid(11) + string.Format(@"\{0}\{1}_1.jpg", lotNoId, lotNoId);
        Process p = new Process();
        p.StartInfo.FileName = "rundll32.exe";
        if (IsValidImage(locationImage))
        {
          p.StartInfo.Arguments = @"C:\WINDOWS\System32\shimgvw.dll,ImageView_Fullscreen " + locationImage;
        }
        else
        {
          p.StartInfo.Arguments = @"C:\WINDOWS\System32\shimgvw.dll,ImageView_Fullscreen ";
        }
        p.Start();
      }
      catch
      {
      }
    }

    /// <summary>
    /// Search Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Save Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      // Check Data
      string message = string.Empty;
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      // Search
      this.Search();
    }

    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "select":
          if (DBConvert.ParseInt(e.Cell.Row.Cells["Select"].Text) == 1)
          {
            e.Cell.Row.Cells["QtyRequest"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString());
            e.Cell.Row.Cells["QtyRequestM2"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["QtyM2"].Value.ToString());
          }
          else
          {
            e.Cell.Row.Cells["QtyRequest"].Value = 0;
            e.Cell.Row.Cells["QtyRequestM2"].Value = 0;
          }
          break;
        default:
          break;
      }
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
