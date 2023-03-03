/*
 * Created By   : 
 * Created Date : 21/05/2013
 * Description  : Allocate , Re-Allocate Department
 * */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_21_100 : MainUserControl
  {
    #region Init
    public viewPLN_21_100()
    {
      InitializeComponent();
    }

    private void viewPLN_21_100_Load(object sender, EventArgs e)
    {
      // Load Department
      this.LoadDepartment();

      // Load Month
      this.LoadMonth();

      // Load Year
      this.LoadYear();
    }
    #endregion Init

    #region function

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
    /// Search From Condition Above
    /// </summary>
    private void Search()
    {
      DBParameter[] inputParam = new DBParameter[4];
      // Department
      if (this.ultDepartment.Value != null && this.ultDepartment.Value.ToString().Length > 0)
      {
        inputParam[0] = new DBParameter("@Depaterment", DbType.AnsiString, 50, this.ultDepartment.Value.ToString());
      }

      // Month
      if (this.ultMonth.Value != null && ultMonth.Value.ToString().Length > 0)
      {
        inputParam[1] = new DBParameter("@Month", DbType.Int32, DBConvert.ParseInt(this.ultMonth.Value.ToString()));
      }

      // Year
      if (this.ultYear.Value != null && this.ultYear.Value.ToString().Length > 0)
      {
        inputParam[2] = new DBParameter("@Year", DbType.Int32, DBConvert.ParseInt(this.ultYear.Value.ToString()));
      }

      // Material
      if (this.txtMaterial.Text.Length > 0)
      {
        inputParam[3] = new DBParameter("@Material", DbType.AnsiString, 128, this.txtMaterial.Text.Trim());
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNVeneerAllocateDepartmentInfomation_Select", inputParam);
      ultraGridWOMaterialDetail.DataSource = dtSource;
    }

    /// <summary>
    /// Load Department
    /// </summary>
    private void LoadDepartment()
    {
      string commandText = string.Empty;
      commandText += " SELECT Department, DeparmentName  ";
      commandText += " FROM VHRDDepartment ";
      commandText += " WHERE DelFlag = 0";
      DataTable dtDepartment = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDepartment.DataSource = dtDepartment;
      ultDepartment.DisplayMember = "DeparmentName";
      ultDepartment.ValueMember = "Department";
      ultDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDepartment.DisplayLayout.Bands[0].Columns["DeparmentName"].Width = 300;
      ultDepartment.DisplayLayout.Bands[0].Columns["Department"].Width = 100;
    }

    /// <summary>
    /// Load Month
    /// </summary>
    private void LoadMonth()
    {
      // Load Data For Month
      DataTable dtMonth = new DataTable();

      DataColumn Month = new DataColumn("Month");
      Month.DataType = System.Type.GetType("System.String");
      dtMonth.Columns.Add(Month);

      for (int i = 1; i < 13; i++)
      {
        DataRow drMonth = dtMonth.NewRow();
        drMonth["Month"] = i.ToString();
        dtMonth.Rows.Add(drMonth);
      }

      ultMonth.DataSource = dtMonth;
      ultMonth.DisplayMember = "Month";
      ultMonth.ValueMember = "Month";
      ultMonth.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultMonth.DisplayLayout.Bands[0].Columns["Month"].Width = 100;
    }

    /// <summary>
    /// Load Year
    /// </summary>
    private void LoadYear()
    {
      // Load Data For Year
      DataTable dtYear = new DataTable();

      DataColumn year = new DataColumn("Year");
      year.DataType = System.Type.GetType("System.String");
      dtYear.Columns.Add(year);

      for (int i = 2013; i < 2050; i++)
      {
        DataRow drYear = dtYear.NewRow();
        drYear["Year"] = i.ToString();
        dtYear.Rows.Add(drYear);
      }

      ultYear.DataSource = dtYear;
      ultYear.DisplayMember = "Year";
      ultYear.ValueMember = "Year";
      ultYear.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultYear.DisplayLayout.Bands[0].Columns["Year"].Width = 100;
    }

    /// <summary>
    /// Export Excel
    /// </summary>
    private void ExportExcel()
    {
      if (ultraGridWOMaterialDetail.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        ControlUtility.ExportToExcelWithDefaultPath(ultraGridWOMaterialDetail, out xlBook, "Allocate", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "VFR Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Binh Chuan Ward, Thuan An Town, Binh Duong Province, Vietnam.";

        xlSheet.Cells[3, 1] = "ALLOCATE DEPARTMENT";
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

    #endregion function

    #region Event

    /// <summary>
    /// Search Click
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
    /// Close Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Layout Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridWOMaterialDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridWOMaterialDetail);
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Columns["Month"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Year"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Department"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalAllocate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Issued"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["StockQty"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Month"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Year"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["TotalAllocate"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Issued"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["StockQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Required"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["TotalAllocate"].Header.Caption = "Total Allocate";
      e.Layout.Bands[0].Columns["StockQty"].Header.Caption = "Stock Qty";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.AllowUpdate = DefaultableBoolean.False;
    }

    /// <summary>
    /// Open Allocate Screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAllocate_Click(object sender, EventArgs e)
    {
      viewPLN_21_101 view = new viewPLN_21_101();
      WindowUtinity.ShowView(view, "INCREASE", true, ViewState.ModalWindow);
      // Search
      this.Search();
    }

    /// <summary>
    /// Open ReAllocate Screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnReAllocate_Click(object sender, EventArgs e)
    {
      viewPLN_21_102 view = new viewPLN_21_102();
      WindowUtinity.ShowView(view, "DECREASE", true, ViewState.ModalWindow);
      // Search
      this.Search();
    }

    /// <summary>
    /// Show History Department Allocation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridWOMaterialDetail_DoubleClick(object sender, EventArgs e)
    {
      if (ultraGridWOMaterialDetail.Selected.Rows.Count > 0 && ultraGridWOMaterialDetail.Selected.Rows[0].Band.ParentBand == null)
      {
        long pid = DBConvert.ParseLong(ultraGridWOMaterialDetail.Selected.Rows[0].Cells["Pid"].Value.ToString());

        viewPLN_21_103 view = new viewPLN_21_103();
        view.departmentAllocatePid = pid;
        WindowUtinity.ShowView(view, "HISTORICAL DEPARTMENT ALLOCATION", true, ViewState.ModalWindow);
      }
    }


    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }
    #endregion Event
  }
}
