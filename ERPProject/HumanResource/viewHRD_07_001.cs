/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: viewHRD_07_001.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewHRD_07_001 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_07_001).Assembly);
    private IList listDeletedPid = new ArrayList();
    private int monthNo = 0;
    private int yearNo = 0;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      // Load Month No
      string commandText = @"WITH MON AS
                            (	SELECT 1 MonthNo
	                            UNION ALL
	                            SELECT (MonthNo + 1) MonthNo FROM MON WHERE (MonthNo + 1) < 13
                            )
                            SELECT MonthNo FROM MON";
      DataTable dtMonth = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbMonthNo, dtMonth, "MonthNo", "MonthNo", false);

      // Load Year No
      commandText = @"WITH YEA AS
                      (	SELECT YEAR(GETDATE()) YearNo
	                      UNION ALL
	                      SELECT (YearNo - 1) YearNo FROM YEA WHERE (YearNo - 1) > 2009
                      )
                      SELECT YearNo FROM YEA";
      DataTable dtYear = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbYearNo, dtYear, "YearNo", "YearNo", false);

      // Load default Month & Year
      ucbMonthNo.Value = ((DateTime.Now.Month - 1) == 0 ? 1 : (DateTime.Now.Month - 1));
      ucbYearNo.Value = dtYear.Rows[0]["YearNo"];

      // Set Language
      this.SetLanguage();
    }

    private void SetNeedToSave()
    {
      if (btnSave.Enabled && btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      this.NeedToSave = false;
      btnSearch.Enabled = false;
      int paramNumber = 2;
      string storeName = "spTblHRDMakeUpAttendance_Select";
      this.monthNo = (ucbMonthNo.SelectedRow == null ? 0 : DBConvert.ParseInt(ucbMonthNo.SelectedRow.Cells["MonthNo"].Value));
      this.yearNo = (ucbYearNo.SelectedRow == null ? 0 : DBConvert.ParseInt(ucbYearNo.SelectedRow.Cells["YearNo"].Value));

      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (this.yearNo > 0)
      {
        inputParam[0] = new DBParameter("@YearNo", DbType.Int32, this.yearNo);
      }
      if (this.monthNo > 0)
      {
        inputParam[1] = new DBParameter("@MonthNo", DbType.Int32, this.monthNo);
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ugdInformation.DataSource = dtSource;

      lbCount.Text = string.Format("{0}: {1}", rm.GetString("Count", ConstantClass.CULTURE), ugdInformation.Rows.FilteredInRowCount);
      btnSearch.Enabled = true;
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      //if (ultCBWo.Text.Length == 0)
      //{
      //  errorMessage = "Work Order";      
      //  return false;
      //}
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        // 1. Delete      
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        for (int i = 0; i < listDeletedPid.Count; i++)
        {
          DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
          DataBaseAccess.ExecuteStoreProcedure("", deleteParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
        // 2. Insert/Update      
        DataTable dtDetail = (DataTable)ugdInformation.DataSource;

        SqlDBParameter[] sqlOutputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.Int, 0) };
        SqlDBParameter[] sqlInputParam = new SqlDBParameter[1];
        sqlInputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dtDetail);
        SqlDataBaseAccess.ExecuteStoreProcedure("spTblHRDMakeUpAttendance_Edit", sqlInputParam, sqlOutputParam);
        if ((sqlOutputParam == null) || (sqlOutputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }

        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.SaveSuccess = true;
          this.NeedToSave = false;
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    private void FillDataForGrid(DataTable dtItemList)
    {
      ugdInformation.DataSource = dtItemList;
      lbCount.Text = string.Format("{0}: {1}", rm.GetString("Count", ConstantClass.CULTURE), dtItemList.Rows.Count);

      this.NeedToSave = true;
    }

    /// <summary>
    /// Set Auto Add 4 blank before text of button
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetBlankForTextOfButton(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count > 0)
        {
          this.SetBlankForTextOfButton(ctr);
        }
        else if (ctr.GetType().Name == "Button")
        {
          ctr.Text = string.Format("{0}{1}", "    ", ctr.Text);
        }
      }
    }
    private void SetLanguage()
    {
      uEGSearch.Text = rm.GetString("Search", ConstantClass.CULTURE);
      groupBoxMaster.Text = rm.GetString("ImportExcel", ConstantClass.CULTURE);
      lblFilePath.Text = rm.GetString("FilePath", ConstantClass.CULTURE);
      gbInformation.Text = rm.GetString("Information", ConstantClass.CULTURE);
      btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);
      lblMonthNo.Text = rm.GetString("MonthNo", ConstantClass.CULTURE);
      lblYearNo.Text = rm.GetString("YearNo", ConstantClass.CULTURE);
      btnSearch.Text = rm.GetString("Search", ConstantClass.CULTURE);
      btnImport.Text = rm.GetString("Import", ConstantClass.CULTURE);
      btnGetTemplate.Text = rm.GetString("Template", ConstantClass.CULTURE);

      this.SetBlankForTextOfButton(this);
    }
    #endregion function

    #region event
    public viewHRD_07_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewHRD_07_001_Load(object sender, EventArgs e)
    {
      //Init Data
      this.InitData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ugdInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          //e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }
      for (int i = 0; i < ugdInformation.Rows.Count; i++)
      {
        ugdInformation.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

      // Set language
      e.Layout.Bands[0].Columns["MonthNo"].Header.Caption = rm.GetString("MonthNo", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["YearNo"].Header.Caption = rm.GetString("YearNo", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["EID"].Header.Caption = rm.GetString("EID", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["EmpName"].Header.Caption = rm.GetString("EmpName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["Department"].Header.Caption = rm.GetString("DepartmentName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["WorkingDate"].Header.Caption = rm.GetString("WDate", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["DayInWeek"].Header.Caption = rm.GetString("DayName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["InTime"].Header.Caption = rm.GetString("InTime", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["OutTime"].Header.Caption = rm.GetString("OutTime", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["NormalDay"].Header.Caption = rm.GetString("WorkingDay", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["OT1"].Header.Caption = rm.GetString("OT1Hour", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["OT2"].Header.Caption = rm.GetString("OT2Hour", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["OT3"].Header.Caption = rm.GetString("OT3Hour", ConstantClass.CULTURE);

      // Hide column
      //e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      /*
      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns[""].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns[""].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      
      // Enable support for displaying errors through IDataErrorInfo interface. By default
			// the functionality is disabled.
			e.Layout.Override.SupportDataErrorInfo = SupportDataErrorInfo.RowsAndCells;

			// Set data error related appearances.
			e.Layout.Override.DataErrorCellAppearance.ForeColor = Color.Red;
			e.Layout.Override.DataErrorRowAppearance.BackColor = Color.LightYellow;
			e.Layout.Override.DataErrorRowSelectorAppearance.BackColor = Color.Green;

			// Make the row selectors bigger so they can accomodate the data error icon as 
			// well active row indicator.
			e.Layout.Override.RowSelectorWidth = 32;
      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;
      
      // Set Align
      e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
      
      // Set Width
      e.Layout.Bands[0].Columns[""].MaxWidth = 100;
      e.Layout.Bands[0].Columns[""].MinWidth = 100;
      
      // Set Column Style
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      // Add Column Selected
      if (!e.Layout.Bands[0].Columns.Exists("Selected"))
      {
        UltraGridColumn checkedCol = e.Layout.Bands[0].Columns.Add();
        checkedCol.Key = "Selected";
        checkedCol.Header.Caption = string.Empty;
        checkedCol.Header.CheckBoxVisibility = HeaderCheckBoxVisibility.Always;        
        checkedCol.DataType = typeof(bool);
        checkedCol.Header.VisiblePosition = 0;
      } 
      
      // Set color
      ugdInformation.Rows[0].Appearance.BackColor = Color.Yellow;
      ugdInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;
      
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      
      // Read only
      e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ugdInformation.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ugdInformation.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      
      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      // Set language
      e.Layout.Bands[0].Columns["EID"].Header.Caption = rm.GetString("EID", ConstantClass.CULTURE);
      */
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ugdInformation_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        this.SetNeedToSave();
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void ugdInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string colName = e.Cell.Column.ToString();      
      //string value = e.NewValue.ToString();      
      //switch (colName)
      //{
      //  case "CompCode":
      //    WindowUtinity.ShowMessageError("ERR0029", "Comp Code");
      //    e.Cancel = true;          
      //    break;        
      //  default:
      //    break;
      //}
    }

    private void ugdInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void btnBrowseItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcelFile.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtImportExcelFile.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }

      // Get Items Count
      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), "SELECT * FROM [Cong (1)$D1:D2]");
      int itemCount = 0;
      if (dsCount == null || dsCount.Tables.Count == 0 || dsCount.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Số dòng");
        return;
      }
      else
      {
        itemCount = DBConvert.ParseInt(dsCount.Tables[0].Rows[0][0]);
        if (itemCount <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Số dòng");
          return;
        }
      }

      // Get Month No
      DataSet dsMonthNo = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), "SELECT * FROM [Cong (1)$I2:I3]");
      if (dsMonthNo == null || dsMonthNo.Tables.Count == 0 || dsMonthNo.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Tháng");
        return;
      }
      else
      {
        this.monthNo = DBConvert.ParseInt(dsMonthNo.Tables[0].Rows[0][0]);
        DataTable dtMonthNo = (DataTable)ucbMonthNo.DataSource;
        if (dtMonthNo.Select(string.Format("MonthNo = {0}", monthNo)).Length == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Tháng");
          return;
        }
      }

      // Get Year No
      DataSet dsYearNo = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), "SELECT * FROM [Cong (1)$K2:K3]");
      if (dsYearNo == null || dsYearNo.Tables.Count == 0 || dsYearNo.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Năm");
        return;
      }
      else
      {
        this.yearNo = DBConvert.ParseInt(dsYearNo.Tables[0].Rows[0][0]);
        DataTable dtYearNo = (DataTable)ucbYearNo.DataSource;
        if (dtYearNo.Select(string.Format("YearNo = {0}", yearNo)).Length == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Năm");
          return;
        }
      }

      // Get data for items list
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), string.Format("SELECT * FROM [Cong (1)$A4:K{0}]", itemCount + 4));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }

      // Set Columns Name
      DataTable dtEmpAtt = dsItemList.Tables[0];
      dtEmpAtt.Columns.Add("YearNo", typeof(System.Int32), this.yearNo.ToString());
      dtEmpAtt.Columns.Add("MonthNo", typeof(System.Int32), this.monthNo.ToString());

      dtEmpAtt.Columns["YearNo"].SetOrdinal(0);
      dtEmpAtt.Columns["MonthNo"].SetOrdinal(1);

      dtEmpAtt.Columns[2].ColumnName = "EID";
      dtEmpAtt.Columns[3].ColumnName = "EmpName";
      dtEmpAtt.Columns[4].ColumnName = "Department";
      dtEmpAtt.Columns[5].ColumnName = "WorkingDate";
      dtEmpAtt.Columns[6].ColumnName = "DayInWeek";
      dtEmpAtt.Columns[7].ColumnName = "InTime";
      dtEmpAtt.Columns[8].ColumnName = "OutTime";
      dtEmpAtt.Columns[9].ColumnName = "NormalDay";
      dtEmpAtt.Columns[10].ColumnName = "OT1";
      dtEmpAtt.Columns[11].ColumnName = "OT2";
      dtEmpAtt.Columns[12].ColumnName = "OT3";

      // Delete invalid row      
      DataRow[] arrRow = dtEmpAtt.Select("EID = ''");
      foreach (DataRow row in arrRow)
      {
        dtEmpAtt.Rows.Remove(row);
      }

      this.FillDataForGrid(dsItemList.Tables[0]);
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "MakeUpAttTemplate";
      string sheetName = "Cong";
      string outFileName = "Bang Cham Cong Thang";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate\HumanResource";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdInformation, "Cong");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugdInformation);
      }
    }

    private void ugdInformation_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ugdInformation.Selected.Rows.Count > 0 || ugdInformation.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ugdInformation, new Point(e.X, e.Y));
        }
      }
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }
    #endregion event
  }
}
