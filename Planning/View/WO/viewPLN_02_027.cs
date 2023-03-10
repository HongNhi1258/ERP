/*
  Author      : 
  Date        : 04-03-2014
  Description : Import Date For Change Deadline
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_02_027 : MainUserControl
  {
    #region Field
    public long transactionPid = long.MinValue;

    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

    #endregion Field

    #region Init
    public viewPLN_02_027()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_027_Load(object sender, EventArgs e)
    {

    }

    #endregion Init

    #region Function

    // Create Data Table
    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("WO", typeof(System.Int64));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("CarcassCode", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Int32));
      dt.Columns.Add("Deadline", typeof(System.DateTime));
      dt.Columns.Add("Reason", typeof(System.Int32));
      return dt;
    }

    // Create Data Table
    private DataTable CreateDataTableResult()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("WO", typeof(System.Int64));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("CarcassCode", typeof(System.String));
      dt.Columns.Add("OldQty", typeof(System.Int32));
      dt.Columns.Add("OldDeadline", typeof(System.DateTime));
      dt.Columns.Add("NewQty", typeof(System.Int32));
      dt.Columns.Add("NewDeadline", typeof(System.DateTime));
      dt.Columns.Add("Reason", typeof(System.Int32));
      dt.Columns.Add("Error", typeof(System.Int32));
      return dt;
    }

    /// <summary>
    /// Import Data From File Excel
    /// </summary>
    private void ImportData()
    {
      if (this.txtLocation.Text.Trim().Length == 0)
      {
        return;
      }
      // Get Data Table From Excel
      DataTable dtSource = new DataTable();
      dtSource = FunctionUtility.GetExcelToDataSetVersion2(txtLocation.Text.Trim(), "SELECT * FROM [Data (1)$B6:H30000]").Tables[0];
      int ttt = dtSource.Rows.Count;
      if (dtSource == null)
      {
        return;
      }

      DataTable dt = this.CreateDataTable();
      try
      {
        lblCount.Text = "Count: " + dtSource.Rows.Count.ToString();
      }
      catch
      {
        lblCount.Text = "";
      }
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow rowData = dtSource.Rows[i];
        if (rowData["WO"].ToString().Length > 0)
        {
          DataRow row = dt.NewRow();
          if (DBConvert.ParseLong(rowData["WO"].ToString()) != long.MinValue)
          {
            row["WO"] = DBConvert.ParseLong(rowData["WO"].ToString());
          }

          row["ItemCode"] = rowData["ItemCode"];

          if (DBConvert.ParseInt(rowData["Revision"].ToString()) != int.MinValue)
          {
            row["Revision"] = DBConvert.ParseInt(rowData["Revision"].ToString());
          }

          row["CarcassCode"] = rowData["CarcassCode"];

          if (DBConvert.ParseInt(rowData["Qty"].ToString()) > 0)
          {
            row["Qty"] = DBConvert.ParseInt(rowData["Qty"].ToString());
          }

          if (rowData["Deadline"].ToString().Trim().Length > 0 &&
            DBConvert.ParseDateTime(rowData["Deadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            row["Deadline"] = DBConvert.ParseDateTime(rowData["Deadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          if (DBConvert.ParseInt(rowData["Reason"].ToString()) != int.MinValue)
          {
            row["Reason"] = DBConvert.ParseInt(rowData["Reason"].ToString());
          }

          dt.Rows.Add(row);
        }
      }
      // Get Store Name
      string storeName = string.Empty;
      string commandText = string.Format(@"SELECT WorkAreaPid FROM TblPLNWoChangeNoteDeadline WHERE Pid = {0}", this.transactionPid);
      DataTable dtWorkArea = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtWorkArea != null)
      {
        int workAreaPid = DBConvert.ParseInt(dtWorkArea.Rows[0][0]);
        if (workAreaPid == 37) // Deadline Packing
        {
          storeName = "spPLNCheckDataForChangeDeadlinePacking_Select";
        }
        else
        {
          storeName = "spPLNCheckDataForChangeDeadline_Select";
        }
      }
      // End Get Store Name

      SqlDBParameter[] inputParam = new SqlDBParameter[2];
      inputParam[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dt);
      inputParam[1] = new SqlDBParameter("@TransactionPid", SqlDbType.BigInt, this.transactionPid);
      DataTable dtCheck = Shared.DataBaseUtility.SqlDataBaseAccess.SearchStoreProcedureDataTable(storeName, 600, inputParam);

      if (dtCheck != null && dtCheck.Rows.Count > 0)
      {
        ultData.DataSource = dtCheck;

        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 1) // WO
          {
            row.Appearance.BackColor = Color.Yellow;
          }
          else if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 2) // Item & Revision
          {
            row.Appearance.BackColor = Color.Lime;
          }
          else if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 3) // Carcass
          {
            row.Appearance.BackColor = Color.CornflowerBlue;
          }
          else if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 4)  // Qty
          {
            row.Appearance.BackColor = Color.LightCoral;
          }
          else if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 5) // Deadline
          {
            row.Appearance.BackColor = Color.Violet;
          }
          else if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 6) // Reason
          {
            row.Appearance.BackColor = Color.Pink;
          }
          else if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 7) // Pending
          {
            row.Appearance.BackColor = Color.SkyBlue;
          }
        }
      }
    }

    /// <summary>
    /// Check Vaild
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckVaild(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) != 0)
        {
          message = "Data";
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        DBParameter[] input = new DBParameter[10];
        input[0] = new DBParameter("@TransactionPid", DbType.Int64, this.transactionPid);
        input[1] = new DBParameter("@WoPid", DbType.Int64, DBConvert.ParseLong(row.Cells["WO"].Value.ToString()));
        if (row.Cells["ItemCode"].Value.ToString().Trim().Length > 0)
        {
          input[2] = new DBParameter("@ItemCode", DbType.String, row.Cells["ItemCode"].Value.ToString());
        }
        if (DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()) >= 0)
        {
          input[3] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
        }
        if (row.Cells["CarcassCode"].Value.ToString().Trim().Length > 0)
        {
          input[4] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CarcassCode"].Value.ToString());
        }
        if (DBConvert.ParseInt(row.Cells["ReasonPid"].Value.ToString()) != int.MinValue)
        {
          input[5] = new DBParameter("@Reason", DbType.Int32, DBConvert.ParseInt(row.Cells["ReasonPid"].Value.ToString()));
        }

        if (DBConvert.ParseDateTime(row.Cells["NewDeadline"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
        {
          input[6] = new DBParameter("@NewDeadline", DbType.DateTime, DBConvert.ParseDateTime(row.Cells["NewDeadline"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
        }

        if (DBConvert.ParseDateTime(row.Cells["OldDeadline"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
        {
          input[7] = new DBParameter("@OldDeadline", DbType.DateTime, DBConvert.ParseDateTime(row.Cells["OldDeadline"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
        }

        input[8] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(row.Cells["NewQty"].Value.ToString()));
        input[9] = new DBParameter("@Approved", DbType.Int32, 0);

        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteDeadlineDetail_Edit", input, output);
        long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
        if (resultSave == 0)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    private void SaveData()
    {
      string message = string.Empty;
      bool success = this.CheckVaild(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      success = this.SaveDetail();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");

        this.CloseTab();
        viewPLN_02_017 uc = new viewPLN_02_017();
        uc.transactionPid = transactionPid;
        Shared.Utility.WindowUtinity.ShowView(uc, "CONFIRMED DEADLINE ADJUSTMENT", false, Shared.Utility.ViewState.MainWindow);
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
    }

    #endregion Function

    #region Event

    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtLocation.Text.Trim().Length > 0);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      this.ImportData();
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "RPT_PLN_02_027";
      string sheetName = "Data";
      string outFileName = "TEMPLATE IMPORT DATA FOR CHANGE DEADLINE";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["ReasonPid"].Hidden = true;
      e.Layout.Bands[0].Columns["TransactionPendingPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Error"].Hidden = true;

      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["OldQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["NewQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
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
      if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 7)
      {
        long transantionPending = DBConvert.ParseLong(row.Cells["TransactionPendingPid"].Value.ToString());
        if (transantionPending > 0)
        {
          this.CloseTab();

          viewPLN_02_017 uc = new viewPLN_02_017();
          uc.transactionPid = transantionPending;
          WindowUtinity.ShowView(uc, "CONFIRMED DEADLINE ADJUSTMENT", false, Shared.Utility.ViewState.MainWindow);
        }
      }
    }
    #endregion Event

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;
        ControlUtility.ExportToExcelWithDefaultPath(ultData, out xlBook, "Master Plan For Container", 7);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "Dai Co Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

        xlSheet.Cells[3, 1] = "DATA";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        xlBook.Application.DisplayAlerts = false;
        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }
  }
}
