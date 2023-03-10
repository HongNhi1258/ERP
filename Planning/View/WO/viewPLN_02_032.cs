/*
  Author      : Nguyễn Văn Trọn
  Date        : 23/04/2014
  Description : Import Data For Change Deadline
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
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_02_032 : MainUserControl
  {
    #region Field
    public long transactionPid = long.MinValue;
    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    #endregion Field

    #region Init
    public viewPLN_02_032()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_032_Load(object sender, EventArgs e)
    {
      txtTransaction.Text = DataBaseAccess.ExecuteScalarCommandText("SELECT dbo.FCSDGetNewOutputCodeForChangeNoteDeadline('DEA')").ToString();

      string commandText = "SELECT Pid, WorkAreaName FROM VWIPWorkAreaForChangeDeadline";
      DataTable dtWorkArea = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadUltraCombo(ultraCBWorkArea, dtWorkArea, "Pid", "WorkAreaName", false, "Pid");
      ultraCBWorkArea.Value = 65;
    }

    #endregion Init

    #region Function
    /// <summary>
    /// Import Data From File Excel
    /// </summary>
    private void ImportData()
    {
      // Get Items Count
      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtLocation.Text.Trim(), "SELECT * FROM [COM2 (1)$K2:K3]");
      int rowsCount = 0;
      if (dsCount == null || dsCount.Tables.Count == 0 || dsCount.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Rows Count");
        return;
      }
      else
      {
        rowsCount = DBConvert.ParseInt(dsCount.Tables[0].Rows[0][0]);
        if (rowsCount <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Rows Count");
          return;
        }
      }

      // Get data for change deadline
      DataSet dsChangeDeadline = FunctionUtility.GetExcelToDataSet(txtLocation.Text.Trim(), string.Format("SELECT * FROM [COM2 (1)$B6:H{0}]", 6 + rowsCount));
      if (dsChangeDeadline == null || dsChangeDeadline.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      DataTable dt = dsChangeDeadline.Tables[0];

      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dt);
      DataTable dtCheck = Shared.DataBaseUtility.SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNCheckDataChangeDeadlineCOM2", 600, inputParam);
      ultData.DataSource = dtCheck;

      lblCount.Text = string.Format("Count: {0}", ultData.Rows.FilteredInRowCount);
    }

    /// <summary>
    /// Check Vaild
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckVaild(out string message)
    {
      message = string.Empty;
      if (ultraCBWorkArea.Value == null)
      {
        message = "Work Area";
        return false;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Status"].Value.ToString()) != 0)
        {
          message = "Data";
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Transaction Master
    /// </summary>
    /// <returns></returns>
    private bool SaveMaster(int department)
    {
      DBParameter[] input = new DBParameter[6];
      if (this.transactionPid != long.MinValue)
      {
        input[0] = new DBParameter("@Pid", DbType.Int64, this.transactionPid);
      }
      input[1] = new DBParameter("@TransactionCode", DbType.AnsiString, 16, txtTransaction.Text);
      input[2] = new DBParameter("@Status", DbType.Int32, 0);
      input[3] = new DBParameter("@CurrencyPid", DbType.Int32, SharedObject.UserInfo.UserPid);
      input[4] = new DBParameter("@DepartmentCreated", DbType.Int32, department);
      input[5] = new DBParameter("@WorkAreaPid", DbType.Int64, ultraCBWorkArea.Value);

      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteDeadline_Edit", input, output);
      long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      this.transactionPid = resultSave;
      if (resultSave == 0)
      {
        return false;
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
        input[1] = new DBParameter("@WoPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Wo"].Value.ToString()));
        if (row.Cells["ItemCode"].Value.ToString().Trim().Length > 0)
        {
          input[2] = new DBParameter("@ItemCode", DbType.String, row.Cells["ItemCode"].Value.ToString());
        }
        if (DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()) > 0)
        {
          input[3] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
        }
        input[4] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CompCode"].Value.ToString());
        input[5] = new DBParameter("@Reason", DbType.Int32, DBConvert.ParseInt(row.Cells["ReasonCode"].Value.ToString()));
        input[6] = new DBParameter("@NewDeadline", DbType.DateTime, row.Cells["NewDeadline"].Value);
        input[7] = new DBParameter("@OldDeadline", DbType.DateTime, row.Cells["OldDeadline"].Value);
        input[8] = new DBParameter("@Qty", DbType.Int32, row.Cells["Qty"].Value);
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
    private void SaveData(int department)
    {
      string message = string.Empty;
      bool success = this.CheckVaild(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      success = this.SaveMaster(department);
      if (success)
      {
        success = this.SaveDetail();
      }
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      this.CloseTab();
    }

    #endregion Function

    #region Event

    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtLocation.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }
      this.ImportData();
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "ChangeDeadline";
      string sheetName = "COM2";
      string outFileName = "ChangeDeadlineCOM2";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate\Planning";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
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
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[0].Columns["ReasonCode"].Hidden = true;
      e.Layout.Bands[0].Columns["TransactionPendingPid"].Hidden = true;

      e.Layout.Bands[0].Columns["OldDeadline"].Format = ConstantClass.NEW_FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["NewDeadline"].Format = ConstantClass.NEW_FORMAT_DATETIME;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }

      // Set status color
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        int status = DBConvert.ParseInt(row.Cells["Status"].Value);
        switch (status)
        {
          case 1: // Component is invalid
            row.Appearance.BackColor = Color.Moccasin;
            break;
          case 2: // Deadline is invalid
            row.Appearance.BackColor = Color.Pink;
            break;
          case 3: // Reason is invalid
            row.Appearance.BackColor = Color.Yellow;
            break;
          case 4: // Total deadline qty <> Remain Qty of WO
            row.Appearance.BackColor = Color.SkyBlue;
            break;
          case 5: // The same Wo, Item, Revision, Comp is pending in other transaction
            row.Appearance.BackColor = Color.GreenYellow;
            break;
          default: // Normal
            break;
        }
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

    private void btnPLNSave_Click(object sender, EventArgs e)
    {
      this.SaveData(1);
    }

    private void btnCOM2Save_Click(object sender, EventArgs e)
    {
      this.SaveData(5);
    }

    private void btnExportToExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultData, "ComponentDeadline");
    }
    #endregion Event   
  }
}
