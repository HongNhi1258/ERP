/*
  Author        : 
  Date          : 23/Apr/2015
  Description   : Import Draft Deadline
  Standard From : viewGNR_90_006
  Remark        : WorkStation = 30: COM1, 1: ASS HW, 32: ASSY, 39: SubCon, 2: Material, 3: Final Fitting HW, 37: Carcass
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
  public partial class viewPLN_29_003 : MainUserControl
  {
    #region Field
    public int workStation = int.MinValue;
    string strWorkStation = string.Empty;
    #endregion Field

    #region Init
    public viewPLN_29_003()
    {
      InitializeComponent();
    }

    private void viewPLN_29_003_Load(object sender, EventArgs e)
    {
      if (workStation == 30)
      {
        strWorkStation = "COM1";
      }
      else if (workStation == 1)
      {
        strWorkStation = "ASS HardWare";
      }
      else if (workStation == 32)
      {
        strWorkStation = "ASSY";
      }
      else if (workStation == 39)
      {
        strWorkStation = "SubCon";
      }
      else if (workStation == 2)
      {
        strWorkStation = "Material";
      }
      else if (workStation == 3)
      {
        strWorkStation = "Final Fitting HardWare";
      }
      else if (workStation == 37)
      {
        strWorkStation = "Carcass";
      }

      // GroupBox Text
      grbImportData.Text = "Import Deadline For " + strWorkStation;
    }

    /// <summary>
    ///  Check Vailid
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

      e.Layout.Bands[0].Columns["Error"].Hidden = true;
      e.Layout.Bands[0].Columns["ReasonPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SuggestDeadline"].Format = "dd-MMM-yyyy";

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
          WindowUtinity.ShowMessageSuccess("MSG0004");
          btnSave.Enabled = true;
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
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
      string templateName = "PLNSuggestDeadline";
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
      dtSource = FunctionUtility.GetExcelToDataSetVersion2(txtLocation.Text.Trim(), "SELECT * FROM [Data (1)$B5:F505]").Tables[0];
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
        if (row["Hangtag"].ToString().Length > 0)
        {
          // Hangtag
          if (row["Hangtag"].ToString().Trim().Length > 0)
          {
            rowadd["StringHangtag"] = row["Hangtag"];
          }

          // Input Deadline
          if (row["Deadline"].ToString().Trim().Length > 0)
          {
            rowadd["InputDeadline"] = DBConvert.ParseDateTime(row["Deadline"].ToString(), "dd-MM-yyyy");
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
      DataTable dtResultDeadline = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNPlanningImportSuggestDeadline", sqlinput);
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
    /// Save Draft Deadline
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      DataTable dtDeadline = (DataTable)ultData.DataSource;
      DataTable dtDeadlineInput = this.dtDeadlineResult();

      //Input
      SqlDBParameter[] sqlinput = new SqlDBParameter[2];
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
            rowadd["InputDeadline"] = DBConvert.ParseDateTime(row["SuggestDeadline"].ToString(), "dd-MMM-yyyy");
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
      if (dtDeadlineInput.Rows.Count > 0)
      {
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
      return true;
    }
    #endregion Function
  }
}
