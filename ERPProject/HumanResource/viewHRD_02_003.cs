/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: viewHRD_02_003.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using System;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewHRD_02_003 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadDepartment();
      this.LoadEmployee();

      udtFromDate.Value = DateTime.Now.AddDays(-1);
      udtFromDate.FormatString = ConstantClass.FORMAT_DATETIME;
      udtFromDate.FormatProvider = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER);
      udtToDate.Value = DateTime.Now.AddDays(-1);
      udtToDate.FormatString = ConstantClass.FORMAT_DATETIME;
      udtToDate.FormatProvider = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER);
    }
    private void LoadDepartment()
    {
      string commandText = "SELECT ID, Department, ParentID, LevelID FROM VHRDDBDepartmentInfo";
      DataTable dtDepartment = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dtDepartment != null)
      {
        DataRow[] rows = dtDepartment.Select("LevelID = 0");
        foreach (DataRow row in rows)
        {
          UltraTreeNode node = new UltraTreeNode(row["ID"].ToString(), row["Department"].ToString());
          this.LoadSubDepartment(node, dtDepartment);
          ultraTreeDepartment.Nodes.Add(node);
        }
      }
      ultraTreeDepartment.ExpandAll();
    }

    private void LoadEmployee()
    {
      string commandText = "SELECT EID, EmpName, (CAST(EID as varchar) + ' | ' + EmpName) DisplayText FROM VHRDDBEmployeeInfo";
      DataTable dtEmployee = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultraCBEmployee, dtEmployee, "EID", "DisplayText", false, "DisplayText");
    }

    private void LoadSubDepartment(UltraTreeNode parentNode, DataTable dtDepartment)
    {
      string id = parentNode.Key;
      DataRow[] rows = dtDepartment.Select(string.Format("ParentID = {0}", id));
      foreach (DataRow row in rows)
      {
        UltraTreeNode node = new UltraTreeNode(row["ID"].ToString(), row["Department"].ToString());
        this.LoadSubDepartment(node, dtDepartment);
        parentNode.Nodes.Add(node);
      }
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      this.Cursor = Cursors.WaitCursor;

      int paramNumber = 2;
      string storeName = "spHRDDBEmployeeList";
      string departments = this.GetDepartmentValue();

      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (departments.Length > 0)
      {
        inputParam[0] = new DBParameter("@Departments", DbType.AnsiString, 128, departments);
      }
      if (ultraCBEmployee.Value != null)
      {
        inputParam[1] = new DBParameter("@EID", DbType.Int32, ultraCBEmployee.Value);
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ugdInformation.DataSource = dtSource;
      lblCount.Text = string.Format("{0}: {1}", rm.GetString("Count", ConstantClass.CULTURE), ugdInformation.Rows.FilteredInRowCount);

      btnSearch.Enabled = true;
      this.Cursor = Cursors.Default;
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchDataAtt()
    {
      btnSearchAtt.Enabled = false;
      this.Cursor = Cursors.WaitCursor;

      int paramNumber = 3;
      string storeName = "spHRDDBMonthlyAttendance_Select";
      string eIDs = string.Empty;
      DataTable dtEmpList = (DataTable)ugdInformation.DataSource;
      if (dtEmpList != null)
      {
        foreach (DataRow row in dtEmpList.Select("IsSelected = 1"))
        {
          eIDs = string.Format("{0}{1}{2}", eIDs, (eIDs.Length > 0 ? "|" : string.Empty), row["EID"].ToString());
        }
        DBParameter[] inputParam = new DBParameter[paramNumber];
        inputParam[0] = new DBParameter("@FromDate", DbType.Date, udtFromDate.Value);
        inputParam[1] = new DBParameter("@ToDate", DbType.Date, udtToDate.Value);
        inputParam[2] = new DBParameter("@EIDs", DbType.String, 2048, eIDs);
        DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
        if (dsSource != null)
        {
          ugdDailyAttendance.DataSource = dsSource.Tables[0];
          if (dsSource.Tables.Count > 1)
          {
            ugdMonthlyAttendance.DataSource = dsSource.Tables[1];
          }
          else
          {
            ugdMonthlyAttendance.DataSource = null;
          }
        }
        else
        {
          ugdDailyAttendance.DataSource = null;
          ugdMonthlyAttendance.DataSource = null;
        }
        lblCountAtt.Text = string.Format("{0}: {1}", rm.GetString("Count", ConstantClass.CULTURE), ugdDailyAttendance.Rows.FilteredInRowCount);
      }

      btnSearchAtt.Enabled = true;
      this.Cursor = Cursors.Default;
    }

    /// <summary>
    /// Set Auto Search Data When User Press Enter
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
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

    private string GetDepartmentText()
    {
      string department = string.Empty;
      for (int i = 0; i < ultraTreeDepartment.Nodes.Count; i++)
      {
        UltraTreeNode node = ultraTreeDepartment.Nodes[i];
        if (node.CheckedState == CheckState.Checked)
        {
          department = string.Format("{0}{1}{2}", department, (department.Length > 0 ? ", " : string.Empty), node.Text);
        }
        else if (node.CheckedState == CheckState.Indeterminate)
        {
          department = string.Format("{0}{1}{2}", department, (department.Length > 0 ? ", " : string.Empty), this.GetSubDepartmentText(node));
        }
      }
      return department;
    }

    private string GetSubDepartmentText(UltraTreeNode parentNode)
    {
      string department = string.Empty;
      for (int i = 0; i < parentNode.Nodes.Count; i++)
      {
        UltraTreeNode node = parentNode.Nodes[i];
        if (node.CheckedState == CheckState.Checked)
        {
          department = string.Format("{0}{1}{2}", department, (department.Length > 0 ? ", " : string.Empty), node.Text);
        }
        else if (node.CheckedState == CheckState.Indeterminate)
        {
          department = string.Format("{0}{1}{2}", department, (department.Length > 0 ? ", " : string.Empty), this.GetSubDepartmentText(node));
        }
      }
      return department;
    }

    private string GetDepartmentValue()
    {
      string department = string.Empty;
      for (int i = 0; i < ultraTreeDepartment.Nodes.Count; i++)
      {
        UltraTreeNode node = ultraTreeDepartment.Nodes[i];
        if (node.CheckedState == CheckState.Checked)
        {
          department = string.Format("{0}{1}{2}", department, (department.Length > 0 ? "|" : string.Empty), node.Key);
        }
        else if (node.CheckedState == CheckState.Indeterminate)
        {
          department = string.Format("{0}{1}{2}", department, (department.Length > 0 ? "|" : string.Empty), this.GetSubDepartmentValue(node));
        }
      }
      return department;
    }

    private string GetSubDepartmentValue(UltraTreeNode parentNode)
    {
      string department = string.Empty;
      for (int i = 0; i < parentNode.Nodes.Count; i++)
      {
        UltraTreeNode node = parentNode.Nodes[i];
        if (node.CheckedState == CheckState.Checked)
        {
          department = string.Format("{0}{1}{2}", department, (department.Length > 0 ? "|" : string.Empty), node.Key);
        }
        else if (node.CheckedState == CheckState.Indeterminate)
        {
          department = string.Format("{0}{1}{2}", department, (department.Length > 0 ? "|" : string.Empty), this.GetSubDepartmentValue(node));
        }
      }
      return department;
    }

    private void SetLanguage()
    {
      lblDept.Text = rm.GetString("Department", ConstantClass.CULTURE);
      lblEmployee.Text = rm.GetString("Employee", ConstantClass.CULTURE);
      lblCount.Text = rm.GetString("Count", ConstantClass.CULTURE) + ":";
      lblFromDate.Text = rm.GetString("FromDate", ConstantClass.CULTURE);
      lblToDate.Text = rm.GetString("ToDate", ConstantClass.CULTURE);
      lblCountAtt.Text = rm.GetString("Count", ConstantClass.CULTURE) + ":";
      btnSearch.Text = rm.GetString("Search", ConstantClass.CULTURE);
      btnSearchAtt.Text = rm.GetString("Search", ConstantClass.CULTURE);
      btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);
      rabDetail.Text = rm.GetString("Detail", ConstantClass.CULTURE);
      rabSummary.Text = rm.GetString("Summary", ConstantClass.CULTURE);

      this.SetBlankForTextOfButton(this);
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      //DataTable dtAttendance = (DataTable)ugdDailyAttendance.DataSource;
      //foreach(DataRow row in dtAttendance.Rows)
      //{
      //  if(row.RowState == DataRowState.Modified)
      //  {
      //    string inTime = row["InTime"].ToString();
      //    string outTime = row["OutTime"].ToString();
      //    if(inTime.Length > 0 && outTime.Length > 0 && DBConvert.ParseDateTime(inTime) > DBConvert.ParseDateTime(outTime))
      //    {
      //      errorMessage = string.Format();
      //    }
      //  }
      //}
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        // Insert/Update      
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataTable dtDetail = (DataTable)ugdDailyAttendance.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          if (row.RowState == DataRowState.Modified)
          {
            DBParameter[] inputParam = new DBParameter[7];
            int eID = DBConvert.ParseInt(row["EID"]);
            DateTime wDate = DBConvert.ParseDateTime(row["WDate"]);
            DateTime inTime = DBConvert.ParseDateTime(row["InTime"]);
            DateTime outTime = DBConvert.ParseDateTime(row["OutTime"]);
            DateTime originalInTime = DBConvert.ParseDateTime(row["OriginalInTime"]);
            DateTime originalOutTime = DBConvert.ParseDateTime(row["OriginalOutTime"]);

            inputParam[0] = new DBParameter("@EID", DbType.Int32, eID);
            inputParam[1] = new DBParameter("@WDate", DbType.Date, wDate);
            if (inTime != DateTime.MinValue)
            {
              inputParam[2] = new DBParameter("@InTime", DbType.DateTime, inTime);
            }
            if (outTime != DateTime.MinValue)
            {
              inputParam[3] = new DBParameter("@OutTime", DbType.DateTime, outTime);
            }
            if (originalInTime != DateTime.MinValue)
            {
              inputParam[4] = new DBParameter("@OriginalInTime", DbType.DateTime, originalInTime);
            }
            if (originalOutTime != DateTime.MinValue)
            {
              inputParam[5] = new DBParameter("@OriginalOutTime", DbType.DateTime, originalOutTime);
            }
            inputParam[6] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            DataBaseAccess.ExecuteStoreProcedure("spHRDDBMonthlyAttendance_Edit", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.SearchDataAtt();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }
    #endregion function

    #region event
    public viewHRD_02_003()
    {
      InitializeComponent();
    }

    protected override void OnCreateControl()
    {
      base.OnCreateControl();
      if (DesignMode || ultraTextEditorDepartment.ButtonsRight.Exists("DropDownButton"))
        return;

      DropDownEditorButton editorButton = new DropDownEditorButton();
      editorButton.Key = "DropDownButton";
      editorButton.RightAlignDropDown = Infragistics.Win.DefaultableBoolean.False;
      editorButton.AutoFocus = false;
      editorButton.Control = ultraTreeDepartment; // popGrid1 is an User Control
      ultraTextEditorDepartment.ButtonsRight.Add(editorButton);
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewHRD_02_003_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(tableLayoutSearch);

      // Init Data
      this.InitData();

      // Set Language
      this.SetLanguage();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.SearchData();
      }
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

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
      for (int i = 0; i < ugdInformation.Rows.Count; i++)
      {
        ugdInformation.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns["StartDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["StartDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

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

      // Hidden Column
      e.Layout.Bands[0].Columns["IsSelected"].Hidden = true;

      // Set language
      e.Layout.Bands[0].Columns["EID"].Header.Caption = rm.GetString("EID", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["EmpName"].Header.Caption = rm.GetString("EmpName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["ScheduleName"].Header.Caption = rm.GetString("ScheduleName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["DepartmentName"].Header.Caption = rm.GetString("DepartmentName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["StartDate"].Header.Caption = rm.GetString("StartDate", ConstantClass.CULTURE);
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      btnExportExcel.Enabled = false;
      this.Cursor = Cursors.WaitCursor;
      if (rabDetail.Checked)
      {
        Utility.ExportToExcelWithDefaultPath(ugdDailyAttendance, "Daily Attendance");
      }
      else
      {
        Utility.ExportToExcelWithDefaultPath(ugdMonthlyAttendance, "Monthly Attendance");
      }
      btnExportExcel.Enabled = true;
      this.Cursor = Cursors.Default;
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugdInformation);
      }
    }

    private void ultraGridInformation_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ugdInformation.Selected.Rows.Count > 0 || ugdInformation.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ugdInformation, new Point(e.X, e.Y));
        }
      }
    }

    private void ultraTextEditorDepartment_BeforeEditorButtonDropDown(object sender, Infragistics.Win.UltraWinEditors.BeforeEditorButtonDropDownEventArgs e)
    {
      ultraTreeDepartment.Width = ultraTextEditorDepartment.Width;
      ultraTreeDepartment.Height = 441;
    }

    private void ultraTextEditorDepartment_AfterEditorButtonCloseUp(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
    {
      ultraTextEditorDepartment.Text = this.GetDepartmentText();
      this.SearchData();
    }

    private void ultraTreeDepartment_AfterSelect(object sender, SelectEventArgs e)
    {
      if (ultraTreeDepartment.SelectedNodes.Count > 0)
      {
        UltraTreeNode node = ultraTreeDepartment.SelectedNodes[0];
        node.CheckedState = (node.CheckedState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
      }
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void btnSearchAtt_Click(object sender, EventArgs e)
    {
      this.SearchDataAtt();
    }

    private void ugdInformation_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      if (e.Cell.Column.Key == "Selected")
      {
        ((DataTable)ugdInformation.DataSource).Rows[e.Cell.Row.Index]["IsSelected"] = (((bool)e.Cell.Value) ? 1 : 0);
      }
    }

    private void ugdDailyAttendance_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64) || colType == typeof(System.Decimal))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["InTime"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["OutTime"].CellActivation = Activation.AllowEdit;
      for (int i = 0; i < ugdDailyAttendance.Rows.Count; i++)
      {
        ugdDailyAttendance.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
        if (DBConvert.ParseInt(ugdDailyAttendance.Rows[i].Cells["LateMinutes"].Value) > 0)
        {
          ugdDailyAttendance.Rows[i].Cells["InTime"].Appearance.BackColor = Color.LightPink;
          ugdDailyAttendance.Rows[i].Cells["LateMinutes"].Appearance.BackColor = Color.LightPink;
        }
        if (DBConvert.ParseInt(ugdDailyAttendance.Rows[i].Cells["EarlyMinutes"].Value) > 0)
        {
          ugdDailyAttendance.Rows[i].Cells["OutTime"].Appearance.BackColor = Color.LightPink;
          ugdDailyAttendance.Rows[i].Cells["EarlyMinutes"].Appearance.BackColor = Color.LightPink;
        }
      }

      // Hide columns
      e.Layout.Bands[0].Columns["OriginalInTime"].Hidden = true;
      e.Layout.Bands[0].Columns["OriginalOutTime"].Hidden = true;

      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns["WDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["WDate"].FormatInfo = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER, true);
      e.Layout.Bands[0].Columns["InTime"].Format = ConstantClass.FORMAT_HOUR;
      e.Layout.Bands[0].Columns["InTime"].FormatInfo = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER, true);
      e.Layout.Bands[0].Columns["InTime"].MaskInput = ConstantClass.MASKINPUT_DATETIME;
      e.Layout.Bands[0].Columns["OutTime"].Format = ConstantClass.FORMAT_HOUR;
      e.Layout.Bands[0].Columns["OutTime"].FormatInfo = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER, true);
      e.Layout.Bands[0].Columns["OutTime"].MaskInput = ConstantClass.MASKINPUT_DATETIME;

      // Set language
      e.Layout.Bands[0].Columns["EID"].Header.Caption = rm.GetString("EID", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["EmpName"].Header.Caption = rm.GetString("EmpName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["DepartmentName"].Header.Caption = rm.GetString("DepartmentName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["WDate"].Header.Caption = rm.GetString("WDate", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["DayName"].Header.Caption = rm.GetString("DayName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["InTime"].Header.Caption = rm.GetString("InTime", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["OutTime"].Header.Caption = rm.GetString("OutTime", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["ShiftName"].Header.Caption = rm.GetString("ShiftName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["LateMinutes"].Header.Caption = rm.GetString("LateMinutes", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["EarlyMinutes"].Header.Caption = rm.GetString("EarlyMinutes", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["WorkingHour"].Header.Caption = rm.GetString("WorkingHour", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["WorkingDay"].Header.Caption = rm.GetString("WorkingDay", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["OT1Hour"].Header.Caption = rm.GetString("OT1Hour", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["OT2Hour"].Header.Caption = rm.GetString("OT2Hour", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["OT3Hour"].Header.Caption = rm.GetString("OT3Hour", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["Total"].Header.Caption = rm.GetString("Total", ConstantClass.CULTURE);
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }
    #endregion event

    private void ugdMonthlyAttendance_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      // Set language
      e.Layout.Bands[0].Columns["EID"].Header.Caption = rm.GetString("EID", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["EmpName"].Header.Caption = rm.GetString("EmpName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["DepartmentName"].Header.Caption = rm.GetString("DepartmentName", ConstantClass.CULTURE);

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64) || colType == typeof(System.Decimal))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;

        // Set language
        double colName = DBConvert.ParseDouble(e.Layout.Bands[0].Columns[i].Header.Caption);
        if (colName > 0)
        {
          e.Layout.Bands[0].Columns[i].Header.Caption = (colName == 1 ? rm.GetString("WorkingDay", ConstantClass.CULTURE) : string.Format("{0} x {1}", rm.GetString("Overtime", ConstantClass.CULTURE), colName));
        }
      }

      // Set color
      for (int i = 0; i < ugdMonthlyAttendance.Rows.Count; i++)
      {
        ugdMonthlyAttendance.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }
    }
  }
}
