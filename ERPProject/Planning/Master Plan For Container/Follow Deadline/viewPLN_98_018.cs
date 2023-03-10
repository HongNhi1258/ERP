/*
  Author      : 
  Date        : 13/01/2014
  Description : Combine Item (Carcass WO)
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_98_018 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    #endregion Field

    #region Init
    public viewPLN_98_018()
    {
      InitializeComponent();
    }

    private void viewPLN_98_018_Load(object sender, EventArgs e)
    {
    }
    #endregion Init

    #region Function

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] input = new DBParameter[3];

      // shipDateFrom
      if (this.ultShipDateFrom.Value != null)
      {
        input[0] = new DBParameter("@ShipDateFrom", DbType.DateTime, DBConvert.ParseDateTime(this.ultShipDateFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
      }

      // shipDateTo
      if (this.ultShipDateTo.Value != null)
      {
        input[1] = new DBParameter("@ShipDateTo", DbType.DateTime, DBConvert.ParseDateTime(this.ultShipDateTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME).AddDays(1));
      }

      // Combine To
      if (this.ultCombineTo.Value != null)
      {
        input[2] = new DBParameter("@CombineTo", DbType.DateTime, DBConvert.ParseDateTime(this.ultCombineTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME).AddDays(1));
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMasterPlanCombineDeadline_Exec", 300, input);
      if (dtSource != null)
      {
        ultData.DataSource = dtSource;
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];
          if (row.Cells["StandByEN"].Value.ToString().Length == 0)
          {
            row.Cells["StandByEN"].Appearance.BackColor = Color.Red;
          }

          if (row.Cells["QtyWip"].Value.ToString().Length == 0)
          {
            row.Cells["QtyWip"].Appearance.BackColor = Color.Red;
          }
          if (row.Cells["WO"].Value.ToString().Length == 0)
          {
            row.Cells["WO"].Appearance.BackColor = Color.Red;
          }

          if (row.Cells["PackingDeadLine"].Value.ToString().Length == 0)
          {
            row.Cells["PackingDeadLine"].Appearance.BackColor = Color.LightGray;
          }
          if (row.Cells["isMain"].Value.ToString() == "1")
          {
            row.Cells["ShipDate"].Appearance.BackColor = Color.SkyBlue;
          }
          if (row.Cells["isProcess"].Value.ToString() == "1")
          {
            row.Cells["Deadline Process"].Appearance.BackColor = Color.LightPink;
          }
          if (row.Cells["isCurrent"].Value.ToString() == "1")
          {
            row.Cells["Current Deadline"].Appearance.BackColor = Color.Yellow;
          }

        }
      }
    }

    /// <summary>
    /// Export Excel
    /// </summary>
    private void ExportExcel()
    {
      Excel.Workbook xlBook;
      Utility.ExportToExcelWithDefaultPath(ultData, out xlBook, "Sheet1", 7);
      string filePath = xlBook.FullName;
      Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);


      Excel.Range r = xlSheet.get_Range("B2", "B2");
      r.Font.Bold = true;

      r = xlSheet.get_Range("B3", "B3");
      r.Font.Bold = true;

      xlSheet.Cells[5, 2] = "FOLLOW DEADLINE (COMBINE ITEM)";
      r = xlSheet.get_Range("B5", "B5");
      r.Font.Bold = true;
      r.Font.Size = 14;
      r.EntireRow.RowHeight = 20;

      xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
      Process.Start(filePath);
    }
    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["isMain"].Hidden = true;
      e.Layout.Bands[0].Columns["isProcess"].Hidden = true;
      e.Layout.Bands[0].Columns["isCurrent"].Hidden = true;
      e.Layout.Bands[0].Columns["StandByEN"].Header.Caption = "Status";
      e.Layout.Bands[0].Columns["PackingDeadline"].Header.Caption = "Packing\nDeadline";
      e.Layout.Bands[0].Columns["QtyWIP"].Header.Caption = "Qty\nWIP";
      e.Layout.Bands[0].Columns["PackQty"].Header.Caption = "PAK\nQty";
      e.Layout.Bands[0].Columns["PackCBM"].Header.Caption = "PAK\nCBM";

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (e.Layout.Bands[0].Columns[i].DataType == typeof(DateTime))
        {
          e.Layout.Bands[0].Columns[i].Format = "dd-MMM-yyyy";
        }
      }

      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      btnExportExcel.Enabled = false;
      this.ExportExcel();
      btnExportExcel.Enabled = true;
    }

    private void bntClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.btnSearch.Enabled = false;
      this.LoadData();
      this.btnSearch.Enabled = true;
    }
    #endregion Event

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ultShipDateFrom.Value = DBNull.Value;
      this.ultShipDateTo.Value = DBNull.Value;
      this.ultCombineTo.Value = DBNull.Value;
    }
  }
}
