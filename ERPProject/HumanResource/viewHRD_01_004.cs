/*
  Author      : Huynh Thi Bang
  Date        : 21/12/2017
  Description : 
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewHRD_01_004 : MainUserControl
  {
    #region field
    public int pid = int.MinValue;
    private int schedulePid = int.MinValue;
    private UltraGrid ultraGridInformation;
    #endregion field

    #region function
    public viewHRD_01_004()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewHRD_01_004_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(tableLayoutSearch);

      //Init Data
      this.InitData();
    }

    private void InitData()
    {
      this.LoadSchedule();
      this.LoadShift();
    }
    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      this.NeedToSave = false;
      int paramNumber = 1;
      string storeName = "spHRDDBSchedule_Search";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      inputParam[0] = new DBParameter("@Pid", DbType.Int32, this.schedulePid);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      if (dtSource != null)
      {
        DataRow row = dtSource.Rows[0];
        utxtScheduleCode.Text = row["ScheduleCode"].ToString();
        utxtScheduleName.Text = row["ScheduleName"].ToString();
        ugdScheduleDetail.DataSource = dtSource;
      }
      lblCount.Text = string.Format("Count: {0}", ugdScheduleDetail.Rows.Count);
    }
    /// <summary>
    /// Load Schedule
    /// </summary>
    private void LoadSchedule()
    {
      string commandEmployee = "SELECT Pid, ScheduleCode + ' | ' + ScheduleName Name FROM TblHRDDBSchedule";
      DataTable data = DataBaseAccess.SearchCommandTextDataTable(commandEmployee);
      Utility.LoadUltraCombo(ucbScheduleList, data, "Pid", "Name", false, "Pid");
      ucbScheduleList.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
    }
    /// <summary>
    /// Load Shift
    /// </summary>
    private void LoadShift()
    {
      string commandEmployee = "SELECT Pid, ShiftCode + ' | ' + ShiftName Name FROM TblHRDDBShift";
      DataTable data = DataBaseAccess.SearchCommandTextDataTable(commandEmployee);
      Utility.LoadUltraCombo(ucbShift, data, "Pid", "Name", false, "Pid");
      ucbShift.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
    }
    private void ClearData()
    {
      this.schedulePid = int.MinValue;
      utxtScheduleCode.Text = string.Empty;
      utxtScheduleName.Text = string.Empty;
    }
    /// <summary>
    /// Set Auto Ask Save Data When User Close Form
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoAskSaveWhenCloseForm(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.TextChanged += new System.EventHandler(this.Object_Changed);
        }
        else
        {
          this.SetAutoAskSaveWhenCloseForm(ctr);
        }
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
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

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      if (utxtScheduleCode.Text.Trim().Length == 0)
      {
        errorMessage = "ScheduleCode is invalid";
        utxtScheduleCode.Focus();
        return false;
      }
      if (utxtScheduleName.Text.Trim().Length == 0)
      {
        errorMessage = "SchduleName is invalid";
        utxtScheduleName.Focus();
        return false;
      }
      DataTable dt = (DataTable)ugdScheduleDetail.DataSource;
      if (dt != null)
      {
        for (int i = 0; i < ugdScheduleDetail.Rows.Count; i++)
        {
          UltraGridRow row = ugdScheduleDetail.Rows[i];
          int schedule = DBConvert.ParseInt(row.Cells["SchedulePid"].Value);
          if (schedule == int.MinValue)
          {
            string cmd = string.Format("SELECT Pid FROM TblHRDDBSchedule WHERE ScheduleCode = '{0}'", utxtScheduleCode.Value);
            DataTable Data = DataBaseAccess.SearchCommandTextDataTable(cmd);
            if (Data != null && Data.Rows.Count > 0)
            {
              errorMessage = "SchduleCode is exists";
              utxtScheduleCode.Focus();
              return false;
            }
          }
        }
      }
      return true;
    }
    /// <summary>
    /// Save Master
    /// </summary>
    private bool SaveMaster()
    {

      DBParameter[] input = new DBParameter[4];
      input[0] = new DBParameter("@ScheduleCode", DbType.AnsiString, 50, utxtScheduleCode.Text.Trim());
      input[1] = new DBParameter("@ScheduleName", DbType.String, 128, utxtScheduleName.Text.Trim());
      if (uchkSaturday.Checked)
      {
        input[2] = new DBParameter("@SaturdayOnOff", DbType.Int32, 1);
      }
      else
      {
        input[2] = new DBParameter("@SaturdayOnOff", DbType.Int32, 0);
      }
      input[3] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spHRDDBSchedule_Edit", input, output);
      int result = DBConvert.ParseInt(output[0].Value.ToString());
      if (result == 0)
      {
        return false;
      }
      this.schedulePid = result;
      return true;
    }
    /// <summary>
    /// SAVE DETAIL
    /// </summary>
    private bool SaveDetail()
    {
      bool success = true;
      string storeName = string.Empty;
      DataTable dtDetail = (DataTable)ugdScheduleDetail.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[5];
          long pid = DBConvert.ParseInt(row["Pid"].ToString());
          int shift = DBConvert.ParseInt(row["ShiftPid"].ToString());
          int dayID = DBConvert.ParseInt(row["DayID"].ToString());
          if (pid < 0 || pid > 0 && shift != int.MinValue) // Insert or Update
          {
            storeName = "spHRDDBScheduleByWeek_Edit";
          }
          if (pid > 0 && shift == int.MinValue)
          {
            storeName = "spHRDDBScheduleByWeek_Delete";
          }
          inputParam[0] = new DBParameter("@Pid", DbType.Int32, pid);
          inputParam[1] = new DBParameter("@DayID", DbType.Int32, dayID);
          inputParam[2] = new DBParameter("@SchedulePid", DbType.Int32, this.schedulePid);
          if (shift != int.MinValue)
          {
            inputParam[3] = new DBParameter("@ShiftPid", DbType.Int32, shift);
          }
          inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }

    #endregion function

    #region event

    private void btnSearch_Click(object sender, EventArgs e)
    {
      if (ucbScheduleList.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Schedule");
        return;
      }
      else
      {
        this.schedulePid = DBConvert.ParseInt(ucbScheduleList.Value);
      }
      btnSearch.Enabled = false;
      this.LoadData();
      btnSearch.Enabled = true;
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
        this.LoadData();
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // 1: Check Valid
      bool success = CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      //2: Save Data
      success = this.SaveMaster();

      if (success)
      {
        success = this.SaveDetail();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          // 3: Load Data
          ucbScheduleList.Value = this.schedulePid;
          this.LoadSchedule();
          this.LoadData();
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0037", "Data");
        }
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdScheduleDetail, "Data");
    }
    private void btnNew_Click(object sender, EventArgs e)
    {
      this.ClearData();
      btnSave.Enabled = true;
      this.LoadData();
      //viewHRD_01_005 view = new viewHRD_01_005();
      //Shared.Utility.WindowUtinity.ShowView(view, " New Schedule Information", false, Shared.Utility.ViewState.ModalWindow);
      //this.LoadSchedule();
      //this.pid = view.pid;
      //ucbScheduleList.Value = this.pid;
    }
    private void ugdScheduleDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;


      e.Layout.Bands[0].Columns["ShiftPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
      e.Layout.Bands[0].Columns["ShiftPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["DayID"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ScheduleCode"].Hidden = true;
      e.Layout.Bands[0].Columns["ScheduleName"].Hidden = true;
      e.Layout.Bands[0].Columns["SchedulePid"].Hidden = true;

      e.Layout.Bands[0].Columns["DayName"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["DayName"].MinWidth = 150;

      e.Layout.Bands[0].Columns["DayName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ShiftPid"].ValueList = ucbShift;
      e.Layout.Bands[0].Columns["ShiftPid"].Header.Caption = "Shift Name";
      e.Layout.Bands[0].Columns["DayName"].Header.Caption = "Day Name";
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
    #endregion event

  }
}
