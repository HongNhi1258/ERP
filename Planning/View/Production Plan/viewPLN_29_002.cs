/*
  Author        : 
  Date          : 23/Apr/2015
  Description   : Import Draft Deadline
  Standard From : viewGNR_90_006
  Remark        : WorkStation = 30: COM1, 32: ASSY, 39: SUBCON, 37: CAR, 1: ASSHW, 2: FFHW, 3: MAT
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
using System.Drawing;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_29_002 : MainUserControl
  {
    #region Field
    private int workStation = int.MinValue;
    public viewPLN_29_001 parentUC = null;
    public string strWorkStation = string.Empty;
    public bool flagSearch = false;
    #endregion Field

    #region Init
    public viewPLN_29_002()
    {
      InitializeComponent();
    }

    private void viewPLN_29_002_Load(object sender, EventArgs e)
    {
      if (this.strWorkStation == "COM1")
      {
        this.workStation = 30;
        this.LoadInputDeadline();
        ultcbInputDeadline.Value = this.workStation;
        ultcbInputDeadline.Enabled = false;
        chkActive.Checked = true;
        chkActive.Enabled = false;
      }
      else if (this.strWorkStation == "ASSY")
      {
        this.workStation = 32;
        this.LoadInputDeadline();
        ultcbInputDeadline.Value = this.workStation;
        ultcbInputDeadline.Enabled = false;
        chkActive.Checked = true;
        chkActive.Enabled = false;
      }
      else if (this.strWorkStation == "SUBCON")
      {
        this.workStation = 39;
        this.LoadInputDeadline();
        ultcbInputDeadline.Value = this.workStation;
        ultcbInputDeadline.Enabled = false;
        chkActive.Checked = true;
        chkActive.Enabled = false;
      }
      else if (this.strWorkStation == "CAR")
      {
        this.workStation = 37;
        this.LoadInputDeadline();
        ultcbInputDeadline.Value = this.workStation;
        ultcbInputDeadline.Enabled = false;
        chkActive.Checked = true;
        chkActive.Enabled = false;
      }
      else if (this.strWorkStation == "ASSHW")
      {
        this.workStation = 1;
        this.LoadInputDeadline();
        ultcbInputDeadline.Value = this.workStation;
        ultcbInputDeadline.Enabled = false;
        chkActive.Checked = true;
        chkActive.Enabled = false;
      }
      else if (this.strWorkStation == "FFHW")
      {
        this.workStation = 2;
        this.LoadInputDeadline();
        ultcbInputDeadline.Value = this.workStation;
        ultcbInputDeadline.Enabled = false;
        chkActive.Checked = true;
        chkActive.Enabled = false;
      }
      else if (this.strWorkStation == "MAT")
      {
        this.workStation = 3;
        this.LoadInputDeadline();
        ultcbInputDeadline.Value = this.workStation;
        ultcbInputDeadline.Enabled = false;
        chkActive.Checked = true;
        chkActive.Enabled = false;
      }
      else if (this.strWorkStation == "PLN")
      {
        this.LoadInputDeadline();
        ultcbInputDeadline.Value = 0;
        btnImport.Enabled = false;
        btnBrown.Enabled = false;
        btnExportExcel.Enabled = false;
        btnGetTemplate.Enabled = false;
        btnSave.Enabled = false;
      }

      // GroupBox Text
      grbImportData.Text = strWorkStation + " Input Deadline";
    }

    /// <summary>
    /// Load Type Input Deadline
    /// </summary>
    private void LoadInputDeadline()
    {
      string cm = @"SELECT 0 Value, 'Input Target Packing Deadline' Display
                    UNION
                    SELECT 37 Value, 'Input Confirm Deadline For CAR' Display
                    UNION
                    SELECT 30 Value, 'Input Confirm Deadline For COM1' Display
                    UNION
                    SELECT 32 Value, 'Input Confirm Deadline For ASSY + SAN' Display
                    UNION
                    SELECT 39 Value, 'Input Confirm Deadline SUBCON' Display
                    UNION
                    SELECT 1 Value, 'Input Confirm Deadline For ASSHW' Display
                    UNION
                    SELECT 2 Value, 'Input Confirm Deadline For FFHW' Display
                    UNION
                    SELECT 3 Value, 'Input Confirm Deadline For MAT' Display
                    ";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      ControlUtility.LoadUltraCombo(ultcbInputDeadline, dt, "Value", "Display", false, "Value");
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <returns></returns>
    private bool CheckValid()
    {
      DataTable dtSource = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow dr = dtSource.Rows[i];
        if (DBConvert.ParseInt(dr["Error"].ToString()) > 0)
        {
          {
            WindowUtinity.ShowMessageError("ERR0050", "Error");
            return false;
          }
        }
      }
      return true;
    }

    #endregion Init

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.AutoFitColumns = true;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      // Hide column
      e.Layout.Bands[0].Columns["Error"].Hidden = true;
      e.Layout.Bands[0].Columns["ReasonPid"].Hidden = true;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      this.ExportData();
    }

    private void btnBrown_Click(object sender, EventArgs e)
    {
      this.Brown();
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      this.ImportData();
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      this.GetTemplate();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      // Save Draft Deadline
      if (CheckValid())
      {
        btnSave.Enabled = false;
        bool success = this.SaveData();
        if (success)
        {
          this.flagSearch = true;
          WindowUtinity.ShowMessageSuccess("MSG0004");
          btnSave.Enabled = true;
          this.CloseTab();
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
    }

    private void chkActive_CheckedChanged(object sender, EventArgs e)
    {
      if (chkActive.Checked)
      {
        ultcbInputDeadline.Enabled = false;
        chkActive.Enabled = false;
        btnImport.Enabled = true;
        btnBrown.Enabled = true;
        btnExportExcel.Enabled = true;
        btnGetTemplate.Enabled = true;
        btnSave.Enabled = true;
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event

    #region Function

    /// <summary>
    /// Import Data
    /// </summary>
    private void Brown()
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    /// <summary>
    /// Import Data
    /// </summary>
    private void GetTemplate()
    {
      string templateName = "viewPLN_29_002";
      string sheetName = "Sheet1";
      string outFileName = "Input Deadline For " + strWorkStation + "";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    /// <summary>
    /// Deadline Result After Import
    /// </summary>
    /// <returns></returns>
    private DataTable dtDeadlineResult()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("StringHangtag", typeof(System.String));
      dt.Columns.Add("InputDeadline", typeof(System.DateTime));
      dt.Columns.Add("Qty", typeof(System.Int32));
      dt.Columns.Add("ReasonPid", typeof(System.Int32));
      dt.Columns.Add("Remark", typeof(System.String));
      return dt;
    }

    /// <summary>
    /// Import Data
    /// </summary>
    private void ImportData()
    {
      if (this.txtLocation.Text.Trim().Length == 0)
      {
        return;
      }
      // Get Data Table From Excel
      DataTable dtSource = new DataTable();
      //dtSource = FunctionUtility.GetExcelToDataSet(txtLocation.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B5:F805]").Tables[0];
      dtSource = FunctionUtility.GetDataFromExcel(txtLocation.Text.Trim(), "Sheet1 (1)", "B5:F805");
      if (dtSource == null)
      {
        return;
      }

      // Input ------- 
      SqlDBParameter[] sqlinput = new SqlDBParameter[1];
      DataTable dtDeadlineInput = this.dtDeadlineResult();

      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        DataRow rowadd = dtDeadlineInput.NewRow();
        if (row["KeyInput"].ToString().Length > 0)
        {
          // KeyInput
          if (row["KeyInput"].ToString().Trim().Length > 0)
          {
            rowadd["StringHangtag"] = row["KeyInput"];
          }

          // Input Deadline
          if (row["Deadline"].ToString().Trim().Length > 0)
          {
            rowadd["InputDeadline"] = DBConvert.ParseDateTime(row["Deadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // Qty
          if (DBConvert.ParseInt(row["Qty"].ToString()) != int.MinValue)
          {
            rowadd["Qty"] = DBConvert.ParseInt(row["Qty"]);
          }

          // Reason
          if (DBConvert.ParseInt(row["Reason"].ToString()) != int.MinValue)
          {
            rowadd["ReasonPid"] = DBConvert.ParseInt(row["Reason"]);
          }

          //Remark
          if (row["Remark"].ToString().Length > 0)
          {
            rowadd["Remark"] = row["Remark"];
          }
          //Add row datatable
          dtDeadlineInput.Rows.Add(rowadd);
        }
      }
      sqlinput[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtDeadlineInput);
      DataTable dtResultDeadline = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNPlanningImportSuggestDeadline", 1000, sqlinput);
      ultData.DataSource = dtResultDeadline;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) != 0)
        {
          ultData.Rows[i].Appearance.BackColor = Color.Yellow;
        }
      }
    }

    /// <summary>
    /// Import Data
    /// </summary>
    private void ExportData()
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        ControlUtility.ExportToExcelWithDefaultPath(ultData, out xlBook, "Deadline", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "VFR Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Binh Chuan Ward, Thuan An Town, Binh Duong Province, Vietnam.";

        xlSheet.Cells[3, 1] = "Deadline";
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

    /// <summary>
    /// Import Search Deadline
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Save Draft Deadline
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      DataTable dtDeadline = (DataTable)ultData.DataSource;
      DataTable dtDeadlineInput = this.dtDeadlineResult();

      for (int i = 0; i < dtDeadline.Rows.Count; i++)
      {
        DataRow row = dtDeadline.Rows[i];
        DataRow rowadd = dtDeadlineInput.NewRow();
        if (DBConvert.ParseInt(row["Error"].ToString()) == 0)
        {
          // Hangtag
          if (row["StringHangtag"].ToString().Trim().Length > 0)
          {
            rowadd["StringHangtag"] = row["StringHangtag"];
          }

          // Input Deadline
          if (row["SuggestDeadline"].ToString().Trim().Length > 0)
          {
            rowadd["InputDeadline"] = DBConvert.ParseDateTime(row["SuggestDeadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // Qty
          if (DBConvert.ParseInt(row["Qty"].ToString()) != int.MinValue)
          {
            rowadd["Qty"] = DBConvert.ParseInt(row["Qty"]);
          }

          // Reason
          if (DBConvert.ParseInt(row["ReasonPid"].ToString()) != int.MinValue)
          {
            rowadd["ReasonPid"] = DBConvert.ParseInt(row["ReasonPid"]);
          }

          //Remark
          if (row["Remark"].ToString().Length > 0)
          {
            rowadd["Remark"] = row["Remark"];
          }

          //Add row datatable
          dtDeadlineInput.Rows.Add(rowadd);
        }
      }
      // Planning Suggest Deadline
      if (string.Compare(this.strWorkStation, "PLN", true) == 0 && DBConvert.ParseInt(ultcbInputDeadline.Value) == 0)
      {
        if (dtDeadlineInput.Rows.Count > 0)
        {
          //Input
          SqlDBParameter[] sqlinput = new SqlDBParameter[2];
          sqlinput[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtDeadlineInput);
          sqlinput[1] = new SqlDBParameter("@CreateBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);

          // Output ------
          SqlDBParameter[] sqloutput = new SqlDBParameter[1];
          sqloutput[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue);

          // Result ------
          DataTable dtResultDeadline = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNPlanningSuggestDeadline_Insert", 500, sqlinput, sqloutput);
          long result = DBConvert.ParseLong(sqloutput[0].Value.ToString());
          if (result <= 0)
          {
            return false;
          }
        }
      }
      else // Supply Change Deadline
      {
        if (dtDeadlineInput.Rows.Count > 0)
        {
          //Input
          SqlDBParameter[] sqlinput = new SqlDBParameter[4];
          sqlinput[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtDeadlineInput);
          sqlinput[1] = new SqlDBParameter("@CreateBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          sqlinput[2] = new SqlDBParameter("@WorkStation", SqlDbType.Int, DBConvert.ParseInt(ultcbInputDeadline.Value));
          sqlinput[3] = new SqlDBParameter("@InputDept", SqlDbType.Text, this.strWorkStation);
          // Output ------
          SqlDBParameter[] sqloutput = new SqlDBParameter[1];
          sqloutput[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue);

          // Result ------
          DataTable dtResultDeadline = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNSupplyInputDeadline_Insert", 500, sqlinput, sqloutput);
          long result = DBConvert.ParseLong(sqloutput[0].Value.ToString());
          if (result <= 0)
          {
            return false;
          }
        }
      }
      return true;
    }
    #endregion Function
  }
}
