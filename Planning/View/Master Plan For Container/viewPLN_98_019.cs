/*
  Author      : 
  Date        : 08/03/2014
  Description : WIP WorkArea On Container
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;

namespace DaiCo.Planning
{
  public partial class viewPLN_98_019 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewPLN_98_019()
    {
      InitializeComponent();
    }

    private void viewPLN_98_019_Load(object sender, EventArgs e)
    {
      this.LoadWorkArea();
    }

    #endregion Init

    #region Function

    /// <summary>
    /// Load WorkArea
    /// </summary>
    private void LoadWorkArea()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, WorkAreaCode + ' - ' + WorkAreaName AS WorkAreaName";
      commandText += " FROM TblWIPWorkArea";

      System.Data.DataTable dtWorkArea = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBWorkArea.DataSource = dtWorkArea;
      ultCBWorkArea.ValueMember = "Pid";
      ultCBWorkArea.DisplayMember = "WorkAreaName";
      ultCBWorkArea.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@WorkArea", DbType.Int32, DBConvert.ParseInt(ultCBWorkArea.Value.ToString()));
      if (txtItemCode.Text.Trim().Length > 0)
      {
        input[1] = new DBParameter("@ItemCodeLike", DbType.String, txtItemCode.Text);
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMasterPlanWipWorkAreaOnContainer_Select", input);
      if (dtSource != null)
      {
        ultData.DataSource = dtSource;
      }
    }

    /// <summary>
    /// Export Excel
    /// </summary>
    private void ExportExcel()
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;
        ControlUtility.ExportToExcelWithDefaultPath(ultData, out xlBook, "WIP WORKAREA ON CONTAINER", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "VFR Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Binh Chuan Ward, Thuan An Town, Binh Duong Province, Vietnam.";

        xlSheet.Cells[3, 1] = "WIP WORKAREA ON CONTAINER";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        //xlBook.Application.DisplayAlerts = false;

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }

    #endregion Function

    #region Event
    private void btnSearch_Click(object sender, EventArgs e)
    {
      // Check WorkArea
      if (ultCBWorkArea.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "WorkArea");
        return;
      }
      // End Check

      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["QtyWip"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    #endregion Event
  }
}
