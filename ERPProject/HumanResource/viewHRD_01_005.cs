/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: viewHRD_01_005.cs
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
  public partial class viewHRD_01_005 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_01_005).Assembly);
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadDepartment();
      this.LoadEmployee();
      this.LoadEmployeeList();
      this.LoadShift();
      udtOvertimeDate.Value = DateTime.Now;
      udtOvertimeDate.FormatString = ConstantClass.FORMAT_DATETIME;
      udtOvertimeDate.FormatProvider = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER);
    }

    /// <summary>
    /// Load Department
    /// </summary>
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
    /// <summary>
    /// Load Employee
    /// </summary>
    private void LoadEmployeeList()
    {
      string commandText = "SELECT EID, EmpName, (CAST(EID as varchar) + ' | ' + EmpName) DisplayText FROM VHRDDBEmployeeInfo";
      DataTable dtEmployee = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultraCBEmployee, dtEmployee, "EID", "DisplayText", false, "DisplayText");
    }
    private void LoadEmployee()
    {
      string commandText = "SELECT EID, EmpName, DepartmentName, (CAST(EID as varchar) + ' | ' + EmpName) DisplayText FROM VHRDDBEmployeeInfo";
      DataTable dtEmployee = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbEmploye, dtEmployee, "EID", "DisplayText", false, "DisplayText");
      ucbEmploye.DisplayLayout.Bands[0].Columns["DisplayText"].Width = 200;
      ucbEmploye.DisplayLayout.Bands[0].Columns["DepartmentName"].Hidden = true;
    }
    /// <summary>
    /// Load Shift
    /// </summary>
    private void LoadShift()
    {
      string commandEmployee = "SELECT Pid, ShiftCode + ' | ' + ShiftName Name FROM TblHRDDBShift";
      DataTable data = DataBaseAccess.SearchCommandTextDataTable(commandEmployee);
      Utility.LoadUltraCombo(ucbShift, data, "Pid", "Name", false, "Pid");
      ucbShift.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
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
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        btnAdd.Enabled = false;
        this.Cursor = Cursors.WaitCursor;

        int paramNumber = 5;
        string storeName = "spHRDDBDailyOvertime_Select";
        string eIDs = string.Empty;
        string fromTime = udtFromTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
        string toTime = udtToTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
        int inNextDay = (uceNextDay.Checked ? 1 : 0);
        DataTable dtEmpList = (DataTable)ugdInformation.DataSource;
        if (dtEmpList != null)
        {
          foreach (DataRow row in dtEmpList.Select("IsSelected = 1"))
          {
            eIDs = string.Format("{0}{1}{2}", eIDs, (eIDs.Length > 0 ? "|" : string.Empty), row["EID"].ToString());
          }
        }
        DBParameter[] inputParam = new DBParameter[paramNumber];
        inputParam[0] = new DBParameter("@DateTime", DbType.Date, udtOvertimeDate.Value);
        inputParam[1] = new DBParameter("@EIDs", DbType.String, 2048, eIDs);
        inputParam[2] = new DBParameter("@FromTime", DbType.String, 5, fromTime);
        inputParam[3] = new DBParameter("@ToTime", DbType.String, 5, toTime);
        inputParam[4] = new DBParameter("@NextDay", DbType.Int32, inNextDay);
        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
        ugdOvertime.DataSource = dtSource;

        lblCountAtt.Text = string.Format("{0}: {1}", rm.GetString("Count", ConstantClass.CULTURE), ugdOvertime.Rows.FilteredInRowCount);
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
      }
      btnAdd.Enabled = true;
      this.Cursor = Cursors.Default;

    }
    private void LoadData()
    {
      this.Cursor = Cursors.WaitCursor;

      int paramNumber = 5;
      string storeName = "spHRDDBDailyOvertime_Select";
      string eIDs = string.Empty;
      string fromTime = udtFromTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
      string toTime = udtToTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
      int inNextDay = (uceNextDay.Checked ? 1 : 0);
      DataTable dtEmpList = (DataTable)ugdInformation.DataSource;
      if (dtEmpList != null)
      {
        foreach (DataRow row in dtEmpList.Select("IsSelected = 1"))
        {
          eIDs = string.Format("{0}{1}{2}", eIDs, (eIDs.Length > 0 ? "|" : string.Empty), row["EID"].ToString());
        }
      }
      DBParameter[] inputParam = new DBParameter[paramNumber];
      inputParam[0] = new DBParameter("@DateTime", DbType.Date, udtOvertimeDate.Value);
      inputParam[1] = new DBParameter("@EIDs", DbType.String, 2048, eIDs);
      inputParam[2] = new DBParameter("@FromTime", DbType.String, 5, fromTime);
      inputParam[3] = new DBParameter("@ToTime", DbType.String, 5, toTime);
      inputParam[4] = new DBParameter("@NextDay", DbType.Int32, inNextDay);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ugdOvertime.DataSource = dtSource;

      lblCountAtt.Text = string.Format("{0}: {1}", rm.GetString("Count", ConstantClass.CULTURE), ugdOvertime.Rows.FilteredInRowCount);
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
      lblDate.Text = rm.GetString("WDate", ConstantClass.CULTURE);
      lblCountAtt.Text = rm.GetString("Count", ConstantClass.CULTURE) + ":";
      btnSearch.Text = rm.GetString("Search", ConstantClass.CULTURE);
      btnAdd.Text = rm.GetString("Add", ConstantClass.CULTURE);
      btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);

      this.SetBlankForTextOfButton(this);
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      // From Time
      if (udtFromTime.Value == null)
      {
        errorMessage = lblFromTime.Text;
        udtFromTime.Focus();
        return false;
      }
      // To Time
      if (udtToTime.Value == null)
      {
        errorMessage = lblToTime.Text;
        udtToTime.Focus();
        return false;
      }
      //Date
      if (udtOvertimeDate.Value == null)
      {
        errorMessage = lblDate.Text;
        udtOvertimeDate.Focus();
        return false;
      }
      DataTable dtAttendance = (DataTable)ugdOvertime.DataSource;
      if (dtAttendance != null && dtAttendance.Rows.Count > 0)
      {
        foreach (DataRow row in dtAttendance.Rows)
        {
          if (row.RowState != DataRowState.Deleted)
          {
            string fromTime = row["FromTime"].ToString();
            string toTime = row["ToTime"].ToString();
            if (fromTime.Length == 0)
            {
              errorMessage = "FromTime is invalid";
              return false;
            }
            if (toTime.Length == 0)
            {
              errorMessage = "ToTime is invalid";
              return false;
            }
          }
        }
      }
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
        DataTable dtDetail = (DataTable)ugdOvertime.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          if (row.RowState != DataRowState.Deleted)
          {
            DBParameter[] inputParam = new DBParameter[7];
            int eID = DBConvert.ParseInt(row["EmpName"]);
            int shift = DBConvert.ParseInt(row["ShiftName"]);
            DateTime wDate = DBConvert.ParseDateTime(row["WDate"]);

            inputParam[0] = new DBParameter("@EID", DbType.Int32, eID);
            inputParam[1] = new DBParameter("@WDate", DbType.Date, wDate);
            inputParam[2] = new DBParameter("@ShiftPid", DbType.Int32, shift);
            inputParam[3] = new DBParameter("@FromTime", DbType.AnsiString, 5, row["FromTime"].ToString());
            inputParam[4] = new DBParameter("@ToTime", DbType.AnsiString, 5, row["ToTime"].ToString());
            if (DBConvert.ParseInt(row["NextDay"]) == 1)
            {
              inputParam[5] = new DBParameter("@NextDay", DbType.Int32, 1);
            }
            else
            {
              inputParam[5] = new DBParameter("@NextDay", DbType.Int32, 0);
            }
            inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            DataBaseAccess.ExecuteStoreProcedure("spHRDDBDailyOvertime_Edit", inputParam, outputParam);
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
        //this.SearchDataAtt();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }
    #endregion function

    #region event
    public viewHRD_01_005()
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
    private void viewHRD_01_005_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(tableLayoutSearch);

      // Init Data
      this.InitData();

      // Set Language
      this.SetLanguage();
      this.LoadData();
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
      Utility.ExportToExcelWithDefaultPath(ugdOvertime, "Daily Attendance");
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

    private void btnAdd_Click(object sender, EventArgs e)
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
    private void ugdOvertime_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      switch (columnName)
      {
        case "empname":
          if (ucbEmploye.SelectedRow != null)
          {
            e.Cell.Row.Cells["DepartmentName"].Value = ucbEmploye.SelectedRow.Cells["DepartmentName"].Value;
          }
          break;
        case "wdate":
          if (e.Cell.Row.Cells["WDate"].Value != null)
          {
            DateTime date = DBConvert.ParseDateTime(e.Cell.Row.Cells["WDate"].Value);
            int k = date.DayOfWeek - DayOfWeek.Monday;
            if (k == 0)
            {
              e.Cell.Row.Cells["DayName"].Value = "Thứ Hai";
            }
            else if (k == 1)
            {
              e.Cell.Row.Cells["DayName"].Value = "Thứ Ba";
            }
            else if (k == 2)
            {
              e.Cell.Row.Cells["DayName"].Value = "Thứ Tư";
            }
            else if (k == 3)
            {
              e.Cell.Row.Cells["DayName"].Value = "Thứ Năm";
            }
            else if (k == 4)
            {
              e.Cell.Row.Cells["DayName"].Value = "Thứ Sáu";
            }
            else if (k == 5)
            {
              e.Cell.Row.Cells["DayName"].Value = "Thứ Bảy";
            }
            else
            {
              e.Cell.Row.Cells["DayName"].Value = "Chủ Nhật";
            }
          }
          break;
      }
    }
    private void ugdOvertime_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;

      e.Layout.Bands[0].Columns["EmpName"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
      e.Layout.Bands[0].Columns["EmpName"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["ShiftName"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
      e.Layout.Bands[0].Columns["ShiftName"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64) || colType == typeof(System.Decimal))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }
      e.Layout.Bands[0].Columns["EmpName"].ValueList = ucbEmploye;
      e.Layout.Bands[0].Columns["ShiftName"].ValueList = ucbShift;

      e.Layout.Bands[0].Columns["DayName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["DepartmentName"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["NextDay"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["NextDay"].DefaultCellValue = 0;
      e.Layout.Bands[0].Columns["NextDay"].MinWidth = 100;
      e.Layout.Bands[0].Columns["NextDay"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["FromTime"].MinWidth = 150;
      e.Layout.Bands[0].Columns["FromTime"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["ToTime"].MinWidth = 150;
      e.Layout.Bands[0].Columns["ToTime"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["DayName"].MinWidth = 150;
      e.Layout.Bands[0].Columns["DayName"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["WDate"].MinWidth = 150;
      e.Layout.Bands[0].Columns["WDate"].MaxWidth = 150;

      for (int i = 0; i < ugdOvertime.Rows.Count; i++)
      {
        ugdOvertime.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }


      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns["WDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["WDate"].FormatInfo = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER, true);
      //e.Layout.Bands[0].Columns["InTime"].Format = ConstantClass.FORMAT_HOUR;
      //e.Layout.Bands[0].Columns["InTime"].FormatInfo = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER, true);
      //e.Layout.Bands[0].Columns["InTime"].MaskInput = ConstantClass.MASKINPUT_DATETIME;
      //e.Layout.Bands[0].Columns["OutTime"].Format = ConstantClass.FORMAT_HOUR;
      //e.Layout.Bands[0].Columns["OutTime"].FormatInfo = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER, true);
      //e.Layout.Bands[0].Columns["OutTime"].MaskInput = ConstantClass.MASKINPUT_DATETIME;

      // Set language
      e.Layout.Bands[0].Columns["EmpName"].Header.Caption = rm.GetString("EmpName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["WDate"].Header.Caption = rm.GetString("WDate", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["DayName"].Header.Caption = rm.GetString("DayName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["DepartmentName"].Header.Caption = rm.GetString("DepartmentName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["ShiftName"].Header.Caption = rm.GetString("ShiftName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["FromTime"].Header.Caption = rm.GetString("FromTime", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["ToTime"].Header.Caption = rm.GetString("ToTime", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["NextDay"].Header.Caption = rm.GetString("NextDay", ConstantClass.CULTURE);

    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    #endregion event


  }
}
