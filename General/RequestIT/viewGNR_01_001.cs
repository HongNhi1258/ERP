/*
  Author      : 
  Description : List Request IT
  Date        : 17-10-2011
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using System.Collections;
using DaiCo.Shared.Utility;
using DaiCo.Shared.DataSetSource.General;

namespace DaiCo.General
{
  public partial class viewGNR_01_001 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewGNR_01_001()
    {
      InitializeComponent();
    }

    private void viewGNR_01_001_Load(object sender, EventArgs e)
    {
      this.ultdtDateFrom.Value = DBNull.Value;
      this.ultdtDateTo.Value = DBNull.Value;
      this.ultdtExpectFrom.Value = DBNull.Value;
      this.ultdtExpectTo.Value = DBNull.Value;
      this.ultdtStartDate.Value = DBNull.Value;
      this.ultdtFinishDate.Value = DBNull.Value;
      this.ultdtPromiseDate.Value = DBNull.Value;

      //Load Data
      this.LoadData();
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      this.LoadComboDepartment();
      this.LoadComboProgramModule();
      this.LoadComboUrgentLevel();
      this.LoadDropDownITStaff();
      this.LoadComboStatus();
      this.LoadComboType();
    }
    /// <summary>
    /// Load UltraCombo Status (0: New / 1: Department Confirmed/ 2: Finish )
    /// </summary>
    private void LoadComboStatus()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 0 ID, 'New' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Department Confirmed' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'IT Confirmed' Name";
      commandText += " UNION";
      commandText += " SELECT 3 ID, 'Finished' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultcbStatus.DataSource = dtSource;
      ultcbStatus.DisplayMember = "Name";
      ultcbStatus.ValueMember = "ID";
      ultcbStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbStatus.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultcbStatus.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
    }

    /// <summary>
    /// Load DropDown IT Staff
    /// </summary>
    private void LoadDropDownITStaff()
    {
      string commandText = string.Empty;
      commandText += "SELECT NV.Pid, CONVERT(VARCHAR, NV.Pid) + ' - ' + NV.EmpName ITUser ";
      commandText += "FROM VHRMEmployee NV ";
      commandText += "  INNER JOIN TblBOMCodeMaster CM ON CM.[Group] = 9200 ";
      commandText += "                      AND NV.Department = CM.Value ";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultcbITStaff.DataSource = dtSource;
      ultcbITStaff.DisplayMember = "ITUser";
      ultcbITStaff.ValueMember = "Pid";
      ultcbITStaff.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbITStaff.DisplayLayout.Bands[0].Columns["ITUser"].Width = 200;
      ultcbITStaff.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Department
    /// </summary>
    private void LoadComboDepartment()
    {
      string commandText = "SELECT Department, Department + ' - ' + DeparmentName AS Name FROM VHRDDepartment";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultcbDepartment.DataSource = dtSource;
      ultcbDepartment.DisplayMember = "Name";
      ultcbDepartment.ValueMember = "Department";
      ultcbDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbDepartment.DisplayLayout.Bands[0].Columns["Name"].Width = 350;
      ultcbDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo ProgramModule
    /// </summary>
    private void LoadComboProgramModule()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_PROGRAMMODULE;
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultcbProgramModule.DataSource = dtSource;
      ultcbProgramModule.DisplayMember = "Value";
      ultcbProgramModule.ValueMember = "Code";
      ultcbProgramModule.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbProgramModule.DisplayLayout.Bands[0].Columns["Value"].Width = 350;
      ultcbProgramModule.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo UrgentLevel
    /// </summary>
    private void LoadComboUrgentLevel()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_URGENTLEVEL;
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultcbUrgentLevel.DataSource = dtSource;
      ultcbUrgentLevel.DisplayMember = "Value";
      ultcbUrgentLevel.ValueMember = "Code";
      ultcbUrgentLevel.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbUrgentLevel.DisplayLayout.Bands[0].Columns["Value"].Width = 350;
      ultcbUrgentLevel.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo UrgentLevel
    /// </summary>
    private void LoadComboType()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_TYPE;
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBType.DataSource = dtSource;
      ultCBType.DisplayMember = "Value";
      ultCBType.ValueMember = "Code";
      ultCBType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBType.DisplayLayout.Bands[0].Columns["Value"].Width = 350;
      ultCBType.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      int type = int.MinValue;
      string department = string.Empty;
      int programModule = int.MinValue;
      int urgentLevel = int.MinValue;
      int itUser = int.MinValue;
      int status = int.MinValue;
      string noFrom = txtNoFrom.Text.Trim().Replace("'", "''");
      string noTo = txtNoTo.Text.Trim().Replace("'", "''");
      string noSet = txtNoSet.Text.Trim();

      string[] listNo = noSet.Split(',');
      foreach (string no in listNo)
      {
        if (no.Trim().Length > 0)
        {
          noSet += string.Format(",'{0}'", no.Replace("'", "").Trim());
        }
      }

      if(ultCBType.Value != null)
      {
        type = DBConvert.ParseInt(this.ultCBType.Value.ToString());
      }
      if(ultcbDepartment.Value != null)
      {
        department = this.ultcbDepartment.Value.ToString();
      }
      if(ultcbProgramModule.Value != null)
      {
        programModule = DBConvert.ParseInt(this.ultcbProgramModule.Value.ToString());
      }
      if(ultcbUrgentLevel.Value != null)
      {
        urgentLevel = DBConvert.ParseInt(this.ultcbUrgentLevel.Value.ToString());
      }
      if(ultcbITStaff.Value != null)
      {
        itUser = DBConvert.ParseInt(this.ultcbITStaff.Value.ToString());
      }
      if(ultcbStatus.Value != null)
      {
        status = DBConvert.ParseInt(this.ultcbStatus.Value.ToString());
      }

      DateTime createDateFrom = DateTime.MinValue;
      if (ultdtDateFrom.Value != null)
      {
        createDateFrom = (DateTime)ultdtDateFrom.Value;
      }

      DateTime createDateTo = DateTime.MinValue;
      if (ultdtDateTo.Value != null)
      {
        createDateTo = (DateTime)ultdtDateTo.Value;
      }

      DateTime expectDateFrom = DateTime.MinValue;
      if (ultdtExpectFrom.Value != null)
      {
        expectDateFrom = (DateTime)ultdtExpectFrom.Value;
      }

      DateTime expectDateTo = DateTime.MinValue;
      if (ultdtExpectTo.Value != null)
      {
        expectDateTo = (DateTime)ultdtExpectTo.Value;
      }

      DateTime promiseDate = DateTime.MinValue;
      if (ultdtPromiseDate.Value != null)
      {
        promiseDate = (DateTime)ultdtPromiseDate.Value;
      }

      DateTime startDate = DateTime.MinValue;
      if (ultdtStartDate.Value != null)
      {
        startDate = (DateTime)ultdtStartDate.Value;
      }

      DateTime finishDate = DateTime.MinValue;
      if (ultdtFinishDate.Value != null)
      {
        finishDate = (DateTime)ultdtFinishDate.Value;
      }

      DBParameter[] intputParam = new DBParameter[16];
      if (noFrom.Length > 0)
      {
        intputParam[0] = new DBParameter("@NoFrom", DbType.AnsiString, 24, noFrom);
      }
      if(noTo.Length > 0)
      {
        intputParam[1] = new DBParameter("@NoTo", DbType.AnsiString, 24, noTo);
      }
      if(noSet.Length > 0)
      {
        noSet = string.Format("{0}", noSet.Remove(0, 1));
        intputParam[2] = new DBParameter("@NoSet", DbType.AnsiString, 1024, noSet);
      }

      if (ultdtDateFrom.Value != null)
      {
        intputParam[3] = new DBParameter("@CreateDateFrom", DbType.DateTime, createDateFrom);
      }
      if (ultdtDateTo.Value != null)
      {
        createDateTo = createDateTo != (DateTime.MaxValue) ? createDateTo.AddDays(1) : createDateTo;
        intputParam[4] = new DBParameter("@CreateDateTo", DbType.DateTime, createDateTo);
      }
      if (ultdtExpectFrom.Value != null)
      {
        intputParam[5] = new DBParameter("@ExpectDateFrom", DbType.DateTime, expectDateFrom);
      }
      if (ultdtExpectTo.Value != null)
      {
        expectDateTo = expectDateTo != (DateTime.MaxValue) ? expectDateTo.AddDays(1) : expectDateTo;
        intputParam[6] = new DBParameter("@ExpectDateTo", DbType.DateTime, expectDateTo);
      }
      if (ultdtPromiseDate.Value != null)
      {
        intputParam[7] = new DBParameter("@PromiseDate", DbType.DateTime, promiseDate);
      }
      if (ultdtStartDate.Value != null)
      {
        intputParam[8] = new DBParameter("@StartDate", DbType.DateTime, startDate);
      }
      if (ultdtFinishDate.Value != null)
      {
        intputParam[9] = new DBParameter("@FinishDate", DbType.DateTime, finishDate);
      }
      if(department != string.Empty)
      {
        intputParam[10] = new DBParameter("@Department", DbType.AnsiString, 256, department);
      }
      if(urgentLevel != int.MinValue)
      {
        intputParam[11] = new DBParameter("@UrgentLevel",DbType.Int32, urgentLevel);
      }
      if(programModule != int.MinValue)
      {
        intputParam[12] = new DBParameter("@ProgramModule", DbType.Int32, programModule);
      }
      if(itUser != int.MinValue)
      {
        intputParam[13] = new DBParameter("@ITStaff", DbType.Int32, itUser);
      }
      if(status != int.MinValue)
      {
        intputParam[14] = new DBParameter("@Status", DbType.Int32, status);
      }
      if(type != int.MinValue)
      {
        intputParam[15] = new DBParameter("@Type", DbType.Int32, type);
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spGNRListRequestIT_Select", intputParam);
      dsGNRListRequestIT dslist = new dsGNRListRequestIT();
      if (ds != null)
      {
        dslist.Tables["RequestIT"].Merge(ds.Tables[0]);
        dslist.Tables["RequestITDetail"].Merge(ds.Tables[1]);
      }
      ultData.DataSource = dslist;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["StatusId"].Value.ToString()) == 3)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen; 
        }
        else if (DBConvert.ParseInt(ultData.Rows[i].Cells["StatusId"].Value.ToString()) == 1)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Pink;
        }
        else if (DBConvert.ParseInt(ultData.Rows[i].Cells["StatusId"].Value.ToString()) == 2)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
      }
      this.chkExpandAll.Checked = false;
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Clear
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      this.txtNoFrom.Text = string.Empty;
      this.txtNoTo.Text = string.Empty;
      this.txtNoSet.Text = string.Empty;
      //this.ultdtDateFrom.Value = DateTime.Today;
      //this.ultdtDateTo.Value = DateTime.Today;
      this.ultdtDateFrom.Value = DBNull.Value;
      this.ultdtDateTo.Value = DBNull.Value;
      this.ultdtExpectFrom.Value = DBNull.Value;
      this.ultdtExpectTo.Value = DBNull.Value;
      this.ultdtStartDate.Value = DBNull.Value;
      this.ultdtFinishDate.Value = DBNull.Value;
      this.ultdtPromiseDate.Value = DBNull.Value;

      this.ultcbDepartment.Text = string.Empty;
      this.ultcbProgramModule.Text = string.Empty;
      this.ultcbUrgentLevel.Text = string.Empty;
      this.ultcbITStaff.Text = string.Empty;
      this.ultcbStatus.Text = string.Empty;
      this.ultCBType.Text = string.Empty;
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["StatusId"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["RequestITPid"].Hidden = true;
      
      e.Layout.Bands[0].Columns["RequestCode"].Header.Caption = "Request Code";
      e.Layout.Bands[0].Columns["ProgramModule"].Header.Caption = "Program Module";
      e.Layout.Bands[0].Columns["UrgenLevel"].Header.Caption = "Urgent Level";

      e.Layout.Bands[1].Columns["ITUser"].Header.Caption = "IT Staff";
      e.Layout.Bands[1].Columns["PromiseDate"].Header.Caption = "Promise Date";
      e.Layout.Bands[1].Columns["StartDate"].Header.Caption = "Start Date";
      e.Layout.Bands[1].Columns["FinishDate"].Header.Caption = "Finish Date";

      e.Layout.Bands[0].Columns["RequestCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["RequestCode"].MinWidth = 120;
      e.Layout.Bands[0].Columns["Department"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["Department"].MinWidth = 150;
      e.Layout.Bands[0].Columns["ProgramModule"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["ProgramModule"].MinWidth = 70;
      e.Layout.Bands[0].Columns["UrgenLevel"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["UrgenLevel"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Status"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["Status"].MinWidth = 150;
      e.Layout.Bands[1].Columns["Remark"].MaxWidth = 350;
      e.Layout.Bands[1].Columns["Remark"].MinWidth = 350;

      e.Layout.Bands[1].Columns["PromiseDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[1].Columns["StartDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[1].Columns["FinishDate"].Format = ConstantClass.FORMAT_DATETIME;

      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
    }

    /// <summary>
    /// Expand All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultData.Rows.ExpandAll(true);
      }
      else
      {
        ultData.Rows.CollapseAll(true);
      }
    }

    /// <summary>
    /// Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
      UltraGridRow row = (ultData.Selected.Rows[0].ParentRow == null) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ParentRow;
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      int userPid = SharedObject.UserInfo.UserPid;
      string department = row.Cells["Department"].Value.ToString();

      string commandText = string.Format(@"SELECT DE.Department + '- ' + DE.DeparmentName AS Department 
                                          FROM VHRNhanVien NV
                                          INNER JOIN VHRDDepartment DE ON NV.Department = DE.Department AND NV.ID_NhanVien = {0}",userPid);
      string department1 = DataBaseAccess.SearchCommandTextDataTable(commandText).Rows[0][0].ToString();
      int check = string.Compare(department, department1, true);
      string status = row.Cells["Status"].Value.ToString();
      if (string.Compare(status, "Finished", true) == 0 || string.Compare(status, "Department Confirmed", true) == 0 || string.Compare(status, "IT Confirmed", true) == 0 )
      {
        viewGNR_01_002_001 view = new viewGNR_01_002_001();
        view.requestITPid = pid;
        DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "REQUEST IT", false, DaiCo.Shared.Utility.ViewState.MainWindow);
        this.Search();
      }
      else if(check == 0)
      {
        viewGNR_01_002_001 view = new viewGNR_01_002_001();
        view.requestITPid = pid;
        DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "REQUEST IT", false, DaiCo.Shared.Utility.ViewState.MainWindow);
        this.Search();      
      }
      this.chkExpandAll.Checked = false;
    }

    /// <summary>
    /// New Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewGNR_01_002_001 view = new viewGNR_01_002_001();
      DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "REQUEST IT", false, DaiCo.Shared.Utility.ViewState.MainWindow);

    }
  
    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event
  }
}
