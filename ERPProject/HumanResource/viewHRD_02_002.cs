/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: viewHRD_02_002.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewHRD_02_002 : MainUserControl
  {
    #region field
    private int shiftPid = int.MinValue;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      // Load shift list
      this.LoadShiftList();

      // Load Rounding Type
      Utility.LoadUltraCBMasterData(ucbRoundingType, 10);

      // Load Working Type
      Utility.LoadUltraCBMasterData(ucbWorkingType, 11);

      // Load Salary Category
      Utility.LoadUltraCBMasterData(ucbSalaryCategory, 9);
    }
    private void LoadShiftList()
    {
      string commandShiftList = "SELECT SHI.Pid, ShiftCode, ShiftName, SHI.StartTime, SHI.EndTime, (ShiftCode + ' | ' + ShiftName) DisplayText FROM TblHRDDBShift SHI";
      DataTable dtShiftList = DataBaseAccess.SearchCommandTextDataTable(commandShiftList);
      Utility.LoadUltraCombo(ucbShiftList, dtShiftList, "Pid", "DisplayText", new string[] { "Pid", "DisplayText" });
    }

    private void LoadData()
    {
      int paramNumber = 1;
      string storeName = "spHRDDBShift_Search";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      inputParam[0] = new DBParameter("@ShiftPid", DbType.Int32, this.shiftPid);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        DataRow row = dtSource.Rows[0];
        utxtShiftCode.Text = row["ShiftCode"].ToString();
        utxtShiftName.Text = row["ShiftName"].ToString();
        udtStartTime.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["StartTime"].ToString()));
        udtEndTime.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["EndTime"].ToString()));
        uceEndTimeNextDay.Checked = (DBConvert.ParseInt(row["IsNextDay"]) == 1 ? true : false);
        if (row["LunchStartTime"] != DBNull.Value)
        {
          udtBeginLunch.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["LunchStartTime"].ToString()));
        }
        else
        {
          udtBeginLunch.Value = null;
        }
        uceBeginLunchNextDay.Checked = (DBConvert.ParseInt(row["IsLunchStartNextDay"]) == 1 ? true : false);
        if (row["LunchEndTime"] != DBNull.Value)
        {
          udtEndLunch.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["LunchEndTime"].ToString()));
        }
        else
        {
          udtEndLunch.Value = null;
        }
        uceEndLunchNextDay.Checked = (DBConvert.ParseInt(row["IsLunchEndNextDay"]) == 1 ? true : false);
        utxtWorkingHour.Value = row["WorkingHour"];
        uneCoefficient.Value = row["Coefficient"];
        if (row["InStartTime"] != DBNull.Value)
        {
          udtInStartTime.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["InStartTime"].ToString()));
        }
        else
        {
          udtInStartTime.Value = null;
        }
        if (row["InEndTime"] != DBNull.Value)
        {
          udtInEndTime.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["InEndTime"].ToString()));
        }
        else
        {
          udtInEndTime.Value = null;
        }
        uceInEndTimeNextDay.Checked = (DBConvert.ParseInt(row["IsInEndNextDay"]) == 1 ? true : false);
        if (row["OutStartTime"] != DBNull.Value)
        {
          udtOutStartTime.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["OutStartTime"].ToString()));
        }
        else
        {
          udtOutStartTime.Value = null;
        }
        uceOutStartTimeNextDay.Checked = (DBConvert.ParseInt(row["IsOutStartNextDay"]) == 1 ? true : false);
        if (row["OutEndTime"] != DBNull.Value)
        {
          udtOutEndTime.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["OutEndTime"].ToString()));
        }
        else
        {
          udtOutEndTime.Value = null;
        }
        uceOutEndTimeNextDay.Checked = (DBConvert.ParseInt(row["IsOutEndNextDay"]) == 1 ? true : false);
        uneRoundingMinutes.Value = DBConvert.ParseInt(row["RoundMinute"]);
        if (row["RoundType"] != DBNull.Value)
        {
          ucbRoundingType.Value = row["RoundType"];
        }
        else
        {
          ucbRoundingType.Value = null;
        }
        ucbWorkingType.Value = row["WorkingType"];
        ucbSalaryCategory.Value = row["SalaryCategory"];
        uneLateMinutes.Value = row["LateGraceMinute"];
        uceLateDeduct.Checked = (DBConvert.ParseInt(row["IsLateDeduct"]) == 1 ? true : false);
        uneEarlyMinutes.Value = row["EarlyGraceMinute"];
        uceEarlyDeduct.Checked = (DBConvert.ParseInt(row["IsEarlyDeduct"]) == 1 ? true : false);

        // Overtime 1
        if (row["OT1StartTime"] != DBNull.Value)
        {
          udtOT1StartTime.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["OT1StartTime"].ToString()));
        }
        else
        {
          udtOT1StartTime.Value = null;
        }
        uceOT1StartTimeNextDay.Checked = (DBConvert.ParseInt(row["IsOT1StartNextDay"]) == 1 ? true : false);
        if (row["OT1EndTime"] != DBNull.Value)
        {
          udtOT1EndTime.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["OT1EndTime"].ToString()));
        }
        else
        {
          udtOT1EndTime.Value = null;
        }
        uceOT1EndTimeNextDay.Checked = (DBConvert.ParseInt(row["IsOT1EndNextDay"]) == 1 ? true : false);
        if (row["OT1BeginBreak"] != DBNull.Value)
        {
          udtOT1BeginBreak.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["OT1BeginBreak"].ToString()));
        }
        else
        {
          udtOT1BeginBreak.Value = null;
        }
        uceOT1BeginBreakNextDay.Checked = (DBConvert.ParseInt(row["IsOT1BeginBreakNextDay"]) == 1 ? true : false);
        if (row["OT1EndBreak"] != DBNull.Value)
        {
          udtOT1EndBreak.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["OT1EndBreak"].ToString()));
        }
        else
        {
          udtOT1EndBreak.Value = null;
        }
        uceOT1EndBreakNextDay.Checked = (DBConvert.ParseInt(row["IsOT1EndBreakNextDay"]) == 1 ? true : false);
        uneOT1Coefficient.Value = row["OT1Coefficient"];
        uneOT1MinMinute.Value = DBConvert.ParseInt(row["OT1MinMinute"]);
        // Overtime 2
        if (row["OT2StartTime"] != DBNull.Value)
        {
          udtOT2StartTime.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["OT2StartTime"].ToString()));
        }
        else
        {
          udtOT2StartTime.Value = null;
        }
        uceOT2StartTimeNextDay.Checked = (DBConvert.ParseInt(row["IsOT2StartNextDay"]) == 1 ? true : false);
        if (row["OT2EndTime"] != DBNull.Value)
        {
          udtOT2EndTime.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["OT2EndTime"].ToString()));
        }
        else
        {
          udtOT2EndTime.Value = null;
        }
        uceOT2EndTimeNextDay.Checked = (DBConvert.ParseInt(row["IsOT2EndNextDay"]) == 1 ? true : false);
        if (row["OT2BeginBreak"] != DBNull.Value)
        {
          udtOT2BeginBreak.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["OT2BeginBreak"].ToString()));
        }
        else
        {
          udtOT2BeginBreak.Value = null;
        }
        uceOT2BeginBreakNextDay.Checked = (DBConvert.ParseInt(row["IsOT2BeginBreakNextDay"]) == 1 ? true : false);
        if (row["OT2EndBreak"] != DBNull.Value)
        {
          udtOT2EndBreak.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["OT2EndBreak"].ToString()));
        }
        else
        {
          udtOT2EndBreak.Value = null;
        }
        uceOT2EndBreakNextDay.Checked = (DBConvert.ParseInt(row["IsOT2EndBreakNextDay"]) == 1 ? true : false);
        uneOT2Coefficient.Value = row["OT2Coefficient"];
        uneOT2MinMinute.Value = DBConvert.ParseInt(row["OT2MinMinute"]);
        // Overtime 3
        if (row["OT3StartTime"] != DBNull.Value)
        {
          udtOT3StartTime.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["OT3StartTime"].ToString()));
        }
        else
        {
          udtOT3StartTime.Value = null;
        }
        uceOT3StartTimeNextDay.Checked = (DBConvert.ParseInt(row["IsOT3StartNextDay"]) == 1 ? true : false);
        if (row["OT3EndTime"] != DBNull.Value)
        {
          udtOT3EndTime.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["OT3EndTime"].ToString()));
        }
        else
        {
          udtOT3EndTime.Value = null;
        }
        uceOT3EndTimeNextDay.Checked = (DBConvert.ParseInt(row["IsOT3EndNextDay"]) == 1 ? true : false);
        if (row["OT3BeginBreak"] != DBNull.Value)
        {
          udtOT3BeginBreak.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["OT3BeginBreak"].ToString()));
        }
        else
        {
          udtOT3BeginBreak.Value = null;
        }
        uceOT3BeginBreakNextDay.Checked = (DBConvert.ParseInt(row["IsOT3BeginBreakNextDay"]) == 1 ? true : false);
        if (row["OT3EndBreak"] != DBNull.Value)
        {
          udtOT3EndBreak.Value = DateTime.Now.Date.Add(TimeSpan.Parse(row["OT3EndBreak"].ToString()));
        }
        else
        {
          udtOT3EndBreak.Value = null;
        }
        uceOT3EndBreakNextDay.Checked = (DBConvert.ParseInt(row["IsOT3EndBreakNextDay"]) == 1 ? true : false);
        uneOT3Coefficient.Value = row["OT3Coefficient"];
        uneOT3MinMinute.Value = DBConvert.ParseInt(row["OT3MinMinute"]);
      }
      this.NeedToSave = false;
      btnSave.Enabled = true;
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      if (ucbShiftList.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Shift");
        return;
      }
      else
      {
        this.shiftPid = DBConvert.ParseInt(ucbShiftList.Value);
      }
      btnSearch.Enabled = false;
      this.LoadData();
      btnSearch.Enabled = true;
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
          if (ctr is UltraCheckEditor)
          {
            ((UltraCheckEditor)ctr).CheckedChanged += new System.EventHandler(this.Object_Changed);
          }
          else
          {
            ctr.TextChanged += new System.EventHandler(this.Object_Changed);
          }
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
      // Shift Code
      if (utxtShiftCode.Text.Trim().Length == 0)
      {
        utxtShiftCode.Focus();
        errorMessage = lblShiftCode.Text;
        return false;
      }
      // Shift Name
      if (utxtShiftName.Text.Trim().Length == 0)
      {
        errorMessage = lblShiftName.Text;
        utxtShiftName.Focus();
        return false;
      }
      // Start Time
      if (udtStartTime.Value == null)
      {
        errorMessage = lblStartTime.Text;
        udtStartTime.Focus();
        return false;
      }
      // End Time
      if (udtEndTime.Value == null)
      {
        errorMessage = lblEndTime.Text;
        udtEndTime.Focus();
        return false;
      }
      // In Start Time
      if (udtInStartTime.Value == null)
      {
        errorMessage = lblInStartTime.Text;
        udtInStartTime.Focus();
        return false;
      }
      // In End Time
      if (udtInEndTime.Value == null)
      {
        errorMessage = lblInEndTime.Text;
        udtInEndTime.Focus();
        return false;
      }
      // Out Start Time
      if (udtOutStartTime.Value == null)
      {
        errorMessage = lblOutStartTime.Text;
        udtOutStartTime.Focus();
        return false;
      }
      // Out End Time
      if (udtOutEndTime.Value == null)
      {
        errorMessage = lblOutEndTime.Text;
        udtOutEndTime.Focus();
        return false;
      }
      // Working Hour
      double workingHour = DBConvert.ParseDouble(utxtWorkingHour.Text);
      if (workingHour <= 0)
      {
        errorMessage = lblWorkingHours.Text;
        return false;
      }
      // Working Type
      if (ucbWorkingType.Value == null)
      {
        errorMessage = lblWorkingType.Text;
        ucbWorkingType.Focus();
        return false;
      }
      // Salary Category
      if (ucbSalaryCategory.Value == null)
      {
        errorMessage = lblSalaryCategory.Text;
        ucbSalaryCategory.Focus();
        return false;
      }
      return true;
    }

    private void ClearData()
    {
      // Basic Info
      this.shiftPid = int.MinValue;
      utxtShiftCode.Text = string.Empty;
      utxtShiftName.Text = string.Empty;
      udtStartTime.Value = null;
      udtEndTime.Value = null;
      uceEndTimeNextDay.Checked = false;
      utxtWorkingHour.Text = string.Empty;
      uneCoefficient.Value = 1;
      udtBeginLunch.Value = null;
      uceBeginLunchNextDay.Checked = false;
      udtEndLunch.Value = null;
      uceEndLunchNextDay.Checked = false;
      udtInStartTime.Value = null;
      udtInEndTime.Value = null;
      uceInEndTimeNextDay.Checked = false;
      udtOutStartTime.Value = null;
      uceOutStartTimeNextDay.Checked = false;
      udtOutEndTime.Value = null;
      uceOutEndTimeNextDay.Checked = false;
      uneRoundingMinutes.Value = 1;
      ucbRoundingType.Value = null;
      ucbWorkingType.Value = 0;
      ucbSalaryCategory.Value = 1;
      // Late/Early
      uneLateMinutes.Value = 0;
      uceLateDeduct.Checked = false;
      uneEarlyMinutes.Value = 0;
      uceEarlyDeduct.Checked = false;
      // Overtime
      udtOT1StartTime.Value = null;
      uceOT1StartTimeNextDay.Checked = false;
      udtOT1EndTime.Value = null;
      uceOT1EndTimeNextDay.Checked = false;
      uneOT1Coefficient.Value = 0;
      uneOT1MinMinute.Value = 0;
      udtOT2StartTime.Value = null;
      uceOT2StartTimeNextDay.Checked = false;
      udtOT2EndTime.Value = null;
      uceOT2EndTimeNextDay.Checked = false;
      uneOT2Coefficient.Value = 0;
      uneOT2MinMinute.Value = 0;
      udtOT3StartTime.Value = null;
      uceOT3StartTimeNextDay.Checked = false;
      udtOT3EndTime.Value = null;
      uceOT3EndTimeNextDay.Checked = false;
      uneOT3Coefficient.Value = 0;
      uneOT3MinMinute.Value = 0;
      this.NeedToSave = false;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };

        // Insert / Update
        DBParameter[] inputParam = new DBParameter[58];
        string shiftCode = utxtShiftCode.Text.Trim();
        string shiftName = utxtShiftName.Text.Trim();
        string startTime = udtStartTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
        string endTime = udtEndTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
        int isNextDay = (uceEndTimeNextDay.Checked ? 1 : 0);
        string beginLunch = string.Empty;
        if (udtBeginLunch.Value != null)
        {
          beginLunch = udtBeginLunch.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int beginLunchNextDay = (uceBeginLunchNextDay.Checked ? 1 : 0);
        string endLunch = string.Empty;
        if (udtEndLunch.Value != null)
        {
          endLunch = udtEndLunch.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int endLunchNextDay = (uceEndLunchNextDay.Checked ? 1 : 0);
        double workingHour = DBConvert.ParseDouble(utxtWorkingHour.Text);
        double coefficient = DBConvert.ParseDouble(uneCoefficient.Value);
        // In Start/End Time
        string inStartTime = string.Empty;
        if (udtInStartTime.Value != null)
        {
          inStartTime = udtInStartTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        string inEndTime = string.Empty;
        if (udtInEndTime.Value != null)
        {
          inEndTime = udtInEndTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int inEndTimeNextDay = (uceInEndTimeNextDay.Checked ? 1 : 0);
        // Out Start/End Time
        string outStartTime = string.Empty;
        if (udtOutStartTime.Value != null)
        {
          outStartTime = udtOutStartTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int outStartTimeNextDay = (uceOutStartTimeNextDay.Checked ? 1 : 0);
        string outEndTime = string.Empty;
        if (udtOutEndTime.Value != null)
        {
          outEndTime = udtOutEndTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int outEndTimeNextDay = (uceOutEndTimeNextDay.Checked ? 1 : 0);

        int roundingMinutes = DBConvert.ParseInt(uneRoundingMinutes.Value);
        int roundingType = int.MinValue;
        if (ucbRoundingType.Value != null)
        {
          roundingType = DBConvert.ParseInt(ucbRoundingType.Value);
        }
        int workingType = DBConvert.ParseInt(ucbWorkingType.Value);
        int salaryCategory = DBConvert.ParseInt(ucbSalaryCategory.Value);
        // Allow late, early
        int lateGraceMinute = DBConvert.ParseInt(uneLateMinutes.Value);
        int isLateDeduct = (uceLateDeduct.Checked ? 1 : 0);
        int earlyGraceMinute = DBConvert.ParseInt(uneEarlyMinutes.Value);
        int isEarlyDeduct = (uceEarlyDeduct.Checked ? 1 : 0);
        // Overtime 1
        string ot1StartTime = string.Empty;
        if (udtOT1StartTime.Value != null)
        {
          ot1StartTime = udtOT1StartTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int ot1StartTimeNextDay = (uceOT1StartTimeNextDay.Checked ? 1 : 0);
        string ot1EndTime = string.Empty;
        if (udtOT1EndTime.Value != null)
        {
          ot1EndTime = udtOT1EndTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int ot1EndTimeNextDay = (uceOT1EndTimeNextDay.Checked ? 1 : 0);
        string ot1BeginBreak = string.Empty;
        if (udtOT1BeginBreak.Value != null)
        {
          ot1BeginBreak = udtOT1BeginBreak.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int ot1BeginBreakNextDay = (uceOT1BeginBreakNextDay.Checked ? 1 : 0);
        string ot1EndBreak = string.Empty;
        if (udtOT1EndBreak.Value != null)
        {
          ot1EndBreak = udtOT1EndBreak.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int ot1EndBreakNextDay = (uceOT1EndBreakNextDay.Checked ? 1 : 0);
        double ot1Coefficient = DBConvert.ParseDouble(uneOT1Coefficient.Value);
        int ot1MinMinute = DBConvert.ParseInt(uneOT1MinMinute.Value);
        // Overtime 2
        string ot2StartTime = string.Empty;
        if (udtOT2StartTime.Value != null)
        {
          ot2StartTime = udtOT2StartTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int ot2StartTimeNextDay = (uceOT2StartTimeNextDay.Checked ? 1 : 0);
        string ot2EndTime = string.Empty;
        if (udtOT2EndTime.Value != null)
        {
          ot2EndTime = udtOT2EndTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int ot2EndTimeNextDay = (uceOT2EndTimeNextDay.Checked ? 1 : 0);
        string ot2BeginBreak = string.Empty;
        if (udtOT2BeginBreak.Value != null)
        {
          ot2BeginBreak = udtOT2BeginBreak.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int ot2BeginBreakNextDay = (uceOT2BeginBreakNextDay.Checked ? 1 : 0);
        string ot2EndBreak = string.Empty;
        if (udtOT2EndBreak.Value != null)
        {
          ot2EndBreak = udtOT2EndBreak.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int ot2EndBreakNextDay = (uceOT2EndBreakNextDay.Checked ? 1 : 0);
        double ot2Coefficient = DBConvert.ParseDouble(uneOT2Coefficient.Value);
        int ot2MinMinute = DBConvert.ParseInt(uneOT2MinMinute.Value);
        // Overtime 3
        string ot3StartTime = string.Empty;
        if (udtOT3StartTime.Value != null)
        {
          ot3StartTime = udtOT3StartTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int ot3StartTimeNextDay = (uceOT3StartTimeNextDay.Checked ? 1 : 0);
        string ot3EndTime = string.Empty;
        if (udtOT3EndTime.Value != null)
        {
          ot3EndTime = udtOT3EndTime.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int ot3EndTimeNextDay = (uceOT3EndTimeNextDay.Checked ? 1 : 0);
        string ot3BeginBreak = string.Empty;
        if (udtOT3BeginBreak.Value != null)
        {
          ot3BeginBreak = udtOT3BeginBreak.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int ot3BeginBreakNextDay = (uceOT3BeginBreakNextDay.Checked ? 1 : 0);
        string ot3EndBreak = string.Empty;
        if (udtOT3EndBreak.Value != null)
        {
          ot3EndBreak = udtOT3EndBreak.DateTime.TimeOfDay.ToString().Substring(0, 5);
        }
        int ot3EndBreakNextDay = (uceOT3EndBreakNextDay.Checked ? 1 : 0);
        double ot3Coefficient = DBConvert.ParseDouble(uneOT3Coefficient.Value);
        int ot3MinMinute = DBConvert.ParseInt(uneOT3MinMinute.Value);

        if (this.shiftPid != int.MinValue) // Update
        {
          inputParam[0] = new DBParameter("@ShiftPid", DbType.Int32, this.shiftPid);
        }
        inputParam[1] = new DBParameter("@ShiftCode", DbType.AnsiString, 50, shiftCode);
        inputParam[2] = new DBParameter("@ShiftName", DbType.String, 128, shiftName);
        inputParam[3] = new DBParameter("@WorkingType", DbType.Int32, workingType);
        inputParam[4] = new DBParameter("@StartTime", DbType.AnsiString, 5, startTime);
        inputParam[5] = new DBParameter("@EndTime", DbType.AnsiString, 5, endTime);
        inputParam[6] = new DBParameter("@IsNextDay", DbType.Int32, isNextDay);
        if (beginLunch.Length > 0)
        {
          inputParam[7] = new DBParameter("@LunchStartTime", DbType.AnsiString, 5, beginLunch);
        }
        inputParam[8] = new DBParameter("@IsLunchStartNextDay", DbType.Int32, beginLunchNextDay);
        if (endLunch.Length > 0)
        {
          inputParam[9] = new DBParameter("@LunchEndTime", DbType.AnsiString, 5, endLunch);
        }
        inputParam[10] = new DBParameter("@IsLunchEndNextDay", DbType.Int32, endLunchNextDay);
        if (inStartTime.Length > 0)
        {
          inputParam[11] = new DBParameter("@InStartTime", DbType.AnsiString, 5, inStartTime);
        }
        if (inEndTime.Length > 0)
        {
          inputParam[12] = new DBParameter("@InEndTime", DbType.AnsiString, 5, inEndTime);
        }
        inputParam[13] = new DBParameter("@IsInEndNextDay", DbType.Int32, inEndTimeNextDay);
        if (outStartTime.Length > 0)
        {
          inputParam[14] = new DBParameter("@OutStartTime", DbType.AnsiString, 5, outStartTime);
        }
        inputParam[15] = new DBParameter("@IsOutStartNextDay", DbType.Int32, outStartTimeNextDay);
        if (outEndTime.Length > 0)
        {
          inputParam[16] = new DBParameter("@OutEndTime", DbType.AnsiString, 5, outEndTime);
        }
        inputParam[17] = new DBParameter("@IsOutEndNextDay", DbType.Int32, outEndTimeNextDay);
        inputParam[18] = new DBParameter("@Coefficient", DbType.Double, coefficient);
        inputParam[19] = new DBParameter("@SalaryCategory", DbType.Int32, salaryCategory);
        inputParam[20] = new DBParameter("@WorkingHour", DbType.Double, workingHour);
        inputParam[21] = new DBParameter("@RoundMinute", DbType.Int32, roundingMinutes);
        if (roundingType != int.MinValue)
        {
          inputParam[22] = new DBParameter("@RoundType", DbType.Int32, roundingType);
        }
        inputParam[23] = new DBParameter("@LateGraceMinute", DbType.Int32, lateGraceMinute);
        inputParam[24] = new DBParameter("@IsLateDeduct", DbType.Int32, isLateDeduct);
        inputParam[25] = new DBParameter("@EarlyGraceMinute", DbType.Int32, earlyGraceMinute);
        inputParam[26] = new DBParameter("@IsEarlyDeduct", DbType.Int32, isEarlyDeduct);
        if (ot1StartTime.Length > 0)
        {
          inputParam[27] = new DBParameter("@OT1StartTime", DbType.AnsiString, 5, ot1StartTime);
        }
        inputParam[28] = new DBParameter("@IsOT1StartNextDay", DbType.Int32, ot1StartTimeNextDay);
        if (ot1EndTime.Length > 0)
        {
          inputParam[29] = new DBParameter("@OT1EndTime", DbType.AnsiString, 5, ot1EndTime);
        }
        inputParam[30] = new DBParameter("@IsOT1EndNextDay", DbType.Int32, ot1EndTimeNextDay);
        if (ot1BeginBreak.Length > 0)
        {
          inputParam[31] = new DBParameter("@OT1BeginBreak", DbType.AnsiString, 5, ot1BeginBreak);
        }
        inputParam[32] = new DBParameter("@IsOT1BeginBreakNextDay", DbType.Int32, ot1BeginBreakNextDay);
        if (ot1EndBreak.Length > 0)
        {
          inputParam[33] = new DBParameter("@OT1EndBreak", DbType.AnsiString, 5, ot1EndBreak);
        }
        inputParam[34] = new DBParameter("@IsOT1EndBreakNextDay", DbType.Int32, ot1EndBreakNextDay);
        inputParam[35] = new DBParameter("@OT1Coefficient", DbType.Double, ot1Coefficient);
        inputParam[36] = new DBParameter("@OT1MinMinute", DbType.Int32, ot1MinMinute);
        if (ot2StartTime.Length > 0)
        {
          inputParam[37] = new DBParameter("@OT2StartTime", DbType.AnsiString, 5, ot2StartTime);
        }
        inputParam[38] = new DBParameter("@IsOT2StartNextDay", DbType.Int32, ot2StartTimeNextDay);
        if (ot2EndTime.Length > 0)
        {
          inputParam[39] = new DBParameter("@OT2EndTime", DbType.AnsiString, 5, ot2EndTime);
        }
        inputParam[40] = new DBParameter("@IsOT2EndNextDay", DbType.Int32, ot2EndTimeNextDay);
        if (ot2BeginBreak.Length > 0)
        {
          inputParam[41] = new DBParameter("@OT2BeginBreak", DbType.AnsiString, 5, ot2BeginBreak);
        }
        inputParam[42] = new DBParameter("@IsOT2BeginBreakNextDay", DbType.Int32, ot2BeginBreakNextDay);
        if (ot2EndBreak.Length > 0)
        {
          inputParam[43] = new DBParameter("@OT2EndBreak", DbType.AnsiString, 5, ot2EndBreak);
        }
        inputParam[44] = new DBParameter("@IsOT2EndBreakNextDay", DbType.Int32, ot2EndBreakNextDay);
        inputParam[45] = new DBParameter("@OT2Coefficient", DbType.Double, ot2Coefficient);
        inputParam[46] = new DBParameter("@OT2MinMinute", DbType.Int32, ot2MinMinute);
        if (ot3StartTime.Length > 0)
        {
          inputParam[47] = new DBParameter("@OT3StartTime", DbType.AnsiString, 5, ot3StartTime);
        }
        inputParam[48] = new DBParameter("@IsOT3StartNextDay", DbType.Int32, ot3StartTimeNextDay);
        if (ot3EndTime.Length > 0)
        {
          inputParam[49] = new DBParameter("@OT3EndTime", DbType.AnsiString, 5, ot3EndTime);
        }
        inputParam[50] = new DBParameter("@IsOT3EndNextDay", DbType.Int32, ot3EndTimeNextDay);
        if (ot3BeginBreak.Length > 0)
        {
          inputParam[51] = new DBParameter("@OT3BeginBreak", DbType.AnsiString, 5, ot3BeginBreak);
        }
        inputParam[52] = new DBParameter("@IsOT3BeginBreakNextDay", DbType.Int32, ot3BeginBreakNextDay);
        if (ot3EndBreak.Length > 0)
        {
          inputParam[53] = new DBParameter("@OT3EndBreak", DbType.AnsiString, 5, ot3EndBreak);
        }
        inputParam[54] = new DBParameter("@IsOT3EndBreakNextDay", DbType.Int32, ot3EndBreakNextDay);
        inputParam[55] = new DBParameter("@OT3Coefficient", DbType.Double, ot3Coefficient);
        inputParam[56] = new DBParameter("@OT3MinMinute", DbType.Int32, ot3MinMinute);

        inputParam[57] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        DataBaseAccess.ExecuteStoreProcedure("spHRDDBShift_Edit", inputParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          if (this.shiftPid == int.MinValue)
          {
            this.shiftPid = DBConvert.ParseInt(outputParam[0].Value);
            this.LoadShiftList();
            ucbShiftList.Value = this.shiftPid;
          }
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.LoadData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion function

    #region event
    public viewHRD_02_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewHRD_02_002_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(tableLayoutSearch);

      // Auto Ask To Save When Closing Form
      this.SetAutoAskSaveWhenCloseForm(utcShiftInfo);

      //Init Data
      this.InitData();
      this.ClearData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
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

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
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
      //if (e.Button == MouseButtons.Right)
      //{
      //  if (ugdInformation.Selected.Rows.Count > 0 || ugdInformation.Selected.Columns.Count > 0)
      //  {
      //    popupMenu.Show(ugdInformation, new Point(e.X, e.Y));
      //  }
      //}
    }

    private void ucbShiftList_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["ShiftCode"].Header.Caption = "Shift Code";
      e.Layout.Bands[0].Columns["ShiftName"].Header.Caption = "Shift Name";
      e.Layout.Bands[0].Columns["StartTime"].Header.Caption = "Start Time";
      e.Layout.Bands[0].Columns["EndTime"].Header.Caption = "End Time";

      e.Layout.Bands[0].Columns["StartTime"].Width = 70;
      e.Layout.Bands[0].Columns["EndTime"].Width = 70;
    }

    private void SetWorkingHour(object sender, EventArgs e)
    {
      if (udtStartTime.Value != null && udtEndTime.Value != null)
      {
        double workingHour = 0;
        TimeSpan startTime = udtStartTime.DateTime.TimeOfDay;
        TimeSpan endTime = udtEndTime.DateTime.TimeOfDay;
        if (uceEndTimeNextDay.Checked)
        {
          endTime = endTime.Add(TimeSpan.FromDays(1));
        }
        TimeSpan workingTime = endTime - startTime;

        if (udtBeginLunch.Value != null && udtEndLunch.Value != null)
        {
          TimeSpan startLunch = udtBeginLunch.DateTime.TimeOfDay;
          if (uceBeginLunchNextDay.Checked)
          {
            startLunch = startLunch.Add(TimeSpan.FromDays(1));
          }
          TimeSpan endLunch = udtEndLunch.DateTime.TimeOfDay;
          if (uceEndLunchNextDay.Checked)
          {
            endLunch = endLunch.Add(TimeSpan.FromDays(1));
          }
          TimeSpan lunchTime = endLunch - startLunch;
          workingHour = ((workingTime.Days - lunchTime.Days) * 24) + (workingTime.Hours - lunchTime.Hours) + Math.Round((workingTime.Minutes - lunchTime.Minutes) / 60.0, 2);
        }
        else
        {
          workingHour = (workingTime.Days * 24) + workingTime.Hours + Math.Round(workingTime.Minutes / 60.0, 2);
        }
        utxtWorkingHour.Text = workingHour.ToString();
      }
      else
      {
        utxtWorkingHour.Text = string.Empty;
      }
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      this.ClearData();
      btnSave.Enabled = true;
    }
    #endregion event


  }
}
