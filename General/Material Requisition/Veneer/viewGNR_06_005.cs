/*
  Author      : 
  Date        : 11/06/2013
  Description : Choose IDVeneer For Allocate Wo & Carcass
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
  public partial class viewGNR_06_005 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    public string materialCode = string.Empty;
    public string materialName = string.Empty;
    public double qtysuggest = double.MinValue;
    public double qtyAfterSave = 0;
    private double totalIssuedM2 = 0;

    #endregion Field

    #region Init

    public viewGNR_06_005()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_06_005_Load(object sender, EventArgs e)
    {
      labMaterial.Text = this.materialCode + " - " + this.materialName;
      labQtySuggest.Text = this.qtysuggest.ToString();

      // Load Location
      this.LoadLocation();
    }

    /// <summary>
    /// Load Location
    /// </summary>
    private void LoadLocation()
    {
      string commandText = string.Empty;
      commandText = "SELECT ID_Position LocationPid, Name + ' - ' + Remark AS Location FROM VWHDLocation";
      DataTable dtLocation = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBLocation.DataSource = dtLocation;
      ultCBLocation.ValueMember = "LocationPid";
      ultCBLocation.DisplayMember = "Location";
      ultCBLocation.DisplayLayout.Bands[0].Columns["LocationPid"].Hidden = true;
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
        this.btnSearch.Enabled = false;

        // Search
        this.Search();

        this.btnSearch.Enabled = true;
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      DBParameter[] input = new DBParameter[6];

      if (txtLength.Text.Length > 0 && DBConvert.ParseDouble(txtLength.Text) != Double.MinValue)
      {
        input[0] = new DBParameter("@Length", DbType.Double, DBConvert.ParseDouble(txtLength.Text));
      }

      if (txtWidth.Text.Length > 0 && DBConvert.ParseDouble(txtWidth.Text) != Double.MinValue)
      {
        input[1] = new DBParameter("@Width", DbType.Double, DBConvert.ParseDouble(txtWidth.Text));
      }

      if (txtLengthTo.Text.Length > 0 && DBConvert.ParseDouble(txtLengthTo.Text) != Double.MinValue)
      {
        input[2] = new DBParameter("@LengthTo", DbType.Double, DBConvert.ParseDouble(txtLengthTo.Text));
      }

      if (txtWidthTo.Text.Length > 0 && DBConvert.ParseDouble(txtWidthTo.Text) != Double.MinValue)
      {
        input[3] = new DBParameter("@WidthTo", DbType.Double, DBConvert.ParseDouble(txtWidthTo.Text));
      }

      if(ultCBLocation.Value != null && ultCBLocation.Text.Length > 0)
      {
        input[4] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(ultCBLocation.Value.ToString()));
      }
      input[5] = new DBParameter("@MaterialCode", DbType.String, this.materialCode);

      DataTable dtMain = DataBaseAccess.SearchStoreProcedureDataTable("spGNRVeneerGetRequestOnlineDetailByLotNoID_Select", input);
      if(dtMain != null)
      {
        ultData.DataSource = dtMain;
      }

      // To mau LotNoID co Image
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        string lotNoId = row.Cells["LotNoId"].Value.ToString();
        string locationImage = @FunctionUtility.GetImagePathByPid(11) + string.Format(@"\{0}", lotNoId); ;
        if (Directory.Exists(locationImage))
        {
          row.Appearance.BackColor = Color.Yellow;
        }
      }
    }

    static bool IsValidImage(string filePath)
    {
      return File.Exists(filePath);
    }

    private bool CheckValid()
    {
      string message = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        // Check Require
        if (DBConvert.ParseInt(row.Cells["QtyIssued"].Value.ToString()) > 0)
        {
          if (DBConvert.ParseInt(row.Cells["QtyIssued"].Value.ToString())
              > DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()))
          {
            message = "QtyIssued Must Less Or Equal Qty";
            WindowUtinity.ShowMessageErrorFromText(message);
            return false;
          }
        }
      }

      // Check total
      if (this.totalIssuedM2 >  this.qtysuggest)
      {
        message = "Require Must Less Or Equal Sugguest Qty";
        WindowUtinity.ShowMessageErrorFromText(message);
        return false;
      }
      return true;
    }

    private bool SaveData()
    {
      DataTable dtMain = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtMain.Rows.Count; i++)
      {
        DataRow row = dtMain.Rows[i];
        if (DBConvert.ParseInt(row["QtyIssued"].ToString()) > 0)
        {
          DBParameter[] input = new DBParameter[7];
          input[0] = new DBParameter("@VRNPid", DbType.Int64, this.pid);
          input[1] = new DBParameter("@Type", DbType.Int32, 2);
          input[2] = new DBParameter("@LotNoId", DbType.String, row["LotNoId"].ToString());
          input[3] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseDouble(row["QtyIssued"].ToString()));
          input[4] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(row["LocationPid"].ToString()));
          input[5] = new DBParameter("@Dimension", DbType.Int64, DBConvert.ParseLong(row["DimensionPid"].ToString()));
          input[6] = new DBParameter("@MaterialCode", DbType.String, this.materialCode);

          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure("spGNRVeneerRequisitionNoteDetailByLotNoID_Insert", input, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result <= 0)
          {
            return false;
          }

          this.qtyAfterSave = this.qtyAfterSave + DBConvert.ParseDouble(row["QtyIssuedM2"].ToString());
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
      e.Layout.Bands[0].Columns["LotNoId"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Length"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Width"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Location"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyM2"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyIssuedM2"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyM2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Sheet";
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyIssued"].Header.Caption = "Issued(Sheet)";
      e.Layout.Bands[0].Columns["QtyIssued"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyIssuedM2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Auto"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["QtyIssued"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["QtyIssuedM2"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Auto"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Auto"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 70;
      e.Layout.Bands[0].Columns["QtyM2"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["QtyM2"].MinWidth = 70;
      e.Layout.Bands[0].Columns["QtyIssued"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["QtyIssued"].MinWidth = 70;
      e.Layout.Bands[0].Columns["QtyIssuedM2"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["QtyIssuedM2"].MinWidth = 70;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Auto Check
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
        ultData.Rows[i].Cells["Auto"].Value = selected;
        if (selected == 1)
        {
          ultData.Rows[i].Cells["QtyIssued"].Value = DBConvert.ParseInt(ultData.Rows[i].Cells["Qty"].Value.ToString());
          ultData.Rows[i].Cells["QtyIssuedM2"].Value = DBConvert.ParseDouble(ultData.Rows[i].Cells["QtyM2"].Value.ToString());
        }
        else
        {
          ultData.Rows[i].Cells["QtyIssued"].Value = 0;
          ultData.Rows[i].Cells["QtyIssuedM2"].Value = 0;
        }
      }
    }

    /// <summary>
    /// Cell Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "auto":
          if (DBConvert.ParseInt(e.Cell.Row.Cells["Auto"].Text) == 1)
          {
            e.Cell.Row.Cells["QtyIssued"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString());
            e.Cell.Row.Cells["QtyIssuedM2"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["QtyM2"].Value.ToString());
          }
          else
          {
            e.Cell.Row.Cells["QtyIssued"].Value = 0;
            e.Cell.Row.Cells["QtyIssuedM2"].Value = 0;
          }
          break;
        default:
          break;
      }
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
        // Location Image(ID = 11)
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
    /// After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;

      switch (columnName)
      {
        case "qtyissued":
          if (DBConvert.ParseInt(row.Cells["Qty"].Value.ToString()) < DBConvert.ParseInt(row.Cells["QtyIssued"].Value.ToString()))
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "QtyIssued <= Qty");
            WindowUtinity.ShowMessageErrorFromText(message);
            row.Cells["QtyIssued"].Appearance.BackColor = Color.Yellow;
            row.Cells["QtyIssuedM2"].Appearance.BackColor = Color.Yellow;
          }
          else
          {
            row.Cells["QtyIssued"].Appearance.BackColor = Color.LightBlue;
            row.Cells["QtyIssuedM2"].Appearance.BackColor = Color.LightBlue;

            // Tinh QtyIssuedM2
            double qtyIssuedM2 = 0;
            qtyIssuedM2 = DBConvert.ParseInt(row.Cells["QtyIssued"].Value.ToString()) *
                          DBConvert.ParseDouble(row.Cells["Length"].Value.ToString()) *
                          DBConvert.ParseDouble(row.Cells["Width"].Value.ToString()) / 1000000;

            row.Cells["QtyIssuedM2"].Value = qtyIssuedM2;
          }

          this.totalIssuedM2 = 0;
          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            if (DBConvert.ParseDouble(ultData.Rows[i].Cells["QtyIssuedM2"].Value.ToString()) > 0)
            {
              this.totalIssuedM2 = this.totalIssuedM2 + DBConvert.ParseDouble(ultData.Rows[i].Cells["QtyIssuedM2"].Value.ToString());
            }
          }
          labQtyIssued.Text = this.totalIssuedM2.ToString();

          break;

        case "auto":
          //if (DBConvert.ParseInt(row.Cells["Auto"].Value.ToString()) == 1)
          //{
          //  row.Cells["QtyIssued"].Value = DBConvert.ParseInt(row.Cells["Qty"].Value.ToString());
          //  row.Cells["QtyIssuedM2"].Value = DBConvert.ParseDouble(row.Cells["QtyM2"].Value.ToString());
          //}
          //else
          //{
          //  row.Cells["QtyIssued"].Value = DBNull.Value;
          //  row.Cells["QtyIssuedM2"].Value = DBNull.Value;
          //}

          break;
        default:
          break;
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
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckValid();
      if (!success)
      {
        return;
      }
      // Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      // Search
      this.CloseTab();
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
