/*
 * Author       : 
 * CreateDate   : 18/11/2013
 * Description  : Reports General
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using System.IO;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared;
using VBReport;
using System.Diagnostics;

namespace DaiCo.General
{
  public partial class viewGNR_98_001 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init

    public viewGNR_98_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load viewWHD_99_003
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_98_001_Load(object sender, EventArgs e)
    {
      this.LoadReport();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Load Type Report
    /// </summary>
    private void LoadReport()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 ID, 'Follow IT Team Project ' Name";
      commandText += " UNION ";
      commandText += " SELECT 2 ID, 'Follow IT Staff Finished Task Report ' Name";
      commandText += " UNION ";
      commandText += " SELECT 3 ID, 'IT Staff Had Registered From To Date' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBReport.DataSource = dtSource;
      ultCBReport.DisplayMember = "Name";
      ultCBReport.ValueMember = "ID";
      ultCBReport.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBReport.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      ultCBReport.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;

      // Load Default
      ultCBReport.Value = 1;
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    private bool CheckValid(int value, out string message)
    {
      message = string.Empty;
      if(value == 3)
      {
        // Check FromDate
        if (ultdrpDateFrom.Value == null ||
            DBConvert.ParseDateTime(ultdrpDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
        {
          message = "DateFrom";
          return false;
        }

        // Check ToDate
        if (ultdrpDateTo.Value == null ||
            DBConvert.ParseDateTime(ultdrpDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
        {
          message = "DateTo";
          return false;
        }

      }
      return true;
    }

    /// <summary>
    /// Export Report
    /// </summary>
    private void ExportFollowITStaffFinishedReport()
    {
      string strTemplateName = string.Empty;

      strTemplateName = "RPT_GNR_99_001_02";

      string strSheetName = "Sheet1";
      string strOutFileName = "IT Staff Finished Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spGNRFollowITStaffFollowTask_Select");

      oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

      for (int i = 0; i < dtData.Rows.Count; i++)
      {
        DataRow dtRow = dtData.Rows[i];
        if (i > 0)
        {
          oXlsReport.Cell("B8:T8").Copy();
          oXlsReport.RowInsert(7 + i);
          oXlsReport.Cell("B8:T8", 0, i).Paste();
        }

        oXlsReport.Cell("**No", 0, i).Value = i + 1;
        oXlsReport.Cell("**Name", 0, i).Value = dtRow["Staff"].ToString();

        oXlsReport.Cell("**0", 0, i).Value = DBConvert.ParseDouble(dtRow["AveCoff"].ToString());
        oXlsReport.Cell("**1", 0, i).Value = DBConvert.ParseDouble(dtRow["WRK4Weeks"].ToString());
        oXlsReport.Cell("**2", 0, i).Value = DBConvert.ParseDouble(dtRow["Finished4Weeks"].ToString());
        oXlsReport.Cell("**3", 0, i).Value = DBConvert.ParseDouble(dtRow["Coff4Weeks"].ToString());
        oXlsReport.Cell("**4", 0, i).Value = DBConvert.ParseDouble(dtRow["WRK3Weeks"].ToString());
        oXlsReport.Cell("**5", 0, i).Value = DBConvert.ParseDouble(dtRow["Finished3Weeks"].ToString());
        oXlsReport.Cell("**6", 0, i).Value = DBConvert.ParseDouble(dtRow["Coff3Weeks"].ToString());
        oXlsReport.Cell("**7", 0, i).Value = DBConvert.ParseDouble(dtRow["WRK2Weeks"].ToString());
        oXlsReport.Cell("**8", 0, i).Value = DBConvert.ParseDouble(dtRow["Finished2Weeks"].ToString());
        oXlsReport.Cell("**9", 0, i).Value = DBConvert.ParseDouble(dtRow["Coff2Weeks"].ToString());
        oXlsReport.Cell("**10", 0, i).Value = DBConvert.ParseDouble(dtRow["WRK1Weeks"].ToString());
        oXlsReport.Cell("**11", 0, i).Value = DBConvert.ParseDouble(dtRow["Finished1Weeks"].ToString());
        oXlsReport.Cell("**12", 0, i).Value = DBConvert.ParseDouble(dtRow["Coff1Weeks"].ToString());
        oXlsReport.Cell("**13", 0, i).Value = DBConvert.ParseDouble(dtRow["WRKNextWeek"].ToString());
        oXlsReport.Cell("**14", 0, i).Value = DBConvert.ParseDouble(dtRow["ScheduleNextWeek"].ToString());
        oXlsReport.Cell("**15", 0, i).Value = DBConvert.ParseDouble(dtRow["ScheCoffNextWeek"].ToString());
        oXlsReport.Cell("**16", 0, i).Value = DBConvert.ParseDouble(dtRow["ScheduleOverNextWeek"].ToString());
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Export Report
    /// </summary>
    private void ExportFollowITTeamReport()
    {
      string strTemplateName = string.Empty;

      strTemplateName = "RPT_GNR_99_001_01";

      string strSheetName = "Sheet1";
      string strOutFileName = "IT Team Project";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      if (DBConvert.ParseInt(this.txtStartDay.Text) == int.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Calculate Start Day");
        return;
      }

      if (DBConvert.ParseInt(this.txtEndDay.Text) == int.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Calculate End Day");
        return;
      }

      //Search
      DBParameter[] arrInput = new DBParameter[2];

      arrInput[0] = new DBParameter("@ParameterStart", DbType.Int32, DBConvert.ParseInt(this.txtStartDay.Text));
      arrInput[1] = new DBParameter("@ParameterEnd", DbType.Int32, DBConvert.ParseInt(this.txtEndDay.Text));

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spGNRFollowITTeamProject_Select", arrInput);

      oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
      oXlsReport.Cell("**StartDay").Value = DBConvert.ParseString(DBConvert.ParseInt(this.txtStartDay.Text) + 7) + " Days Later";
      oXlsReport.Cell("**EndDay").Value = "Next " + DBConvert.ParseString(DBConvert.ParseInt(this.txtEndDay.Text) + 7) + " Days";

      for (int i = 0; i < dtData.Rows.Count; i++)
      {
        DataRow dtRow = dtData.Rows[i];
        if (i > 0)
        {
          oXlsReport.Cell("B8:O8").Copy();
          oXlsReport.RowInsert(7 + i);
          oXlsReport.Cell("B8:O8", 0, i).Paste();
        }

        oXlsReport.Cell("**No", 0, i).Value = i + 1;
        oXlsReport.Cell("**TeamName", 0, i).Value = dtRow["TeamName"].ToString();
        oXlsReport.Cell("**ParameterStart", 0, i).Value = DBConvert.ParseDouble(dtRow["ParameterStart"].ToString());
        oXlsReport.Cell("**PendingParameterStart", 0, i).Value = DBConvert.ParseDouble(dtRow["PendingParameterStart"].ToString());
        oXlsReport.Cell("**WorkingDateParameterStart", 0, i).Value = DBConvert.ParseDouble(dtRow["WorkingDateParameterStart"].ToString());
        oXlsReport.Cell("**OneWeekLater", 0, i).Value = DBConvert.ParseDouble(dtRow["OneWeekLater"].ToString());
        oXlsReport.Cell("**PendingOneWeekLater", 0, i).Value = DBConvert.ParseDouble(dtRow["PendingOneWeekLater"].ToString());
        oXlsReport.Cell("**WorkingDateOneWeekLater", 0, i).Value = DBConvert.ParseDouble(dtRow["WorkingDateOneWeekLater"].ToString());
        oXlsReport.Cell("**CurrentDate", 0, i).Value = DBConvert.ParseDouble(dtRow["CurrentDate"].ToString());
        oXlsReport.Cell("**PendingCurrentDate", 0, i).Value = DBConvert.ParseDouble(dtRow["PendingCurrentDate"].ToString());
        oXlsReport.Cell("**WorkingDateCurrentDate", 0, i).Value = DBConvert.ParseDouble(dtRow["WorkingDateCurrentDate"].ToString());
        oXlsReport.Cell("**NextWeek", 0, i).Value = DBConvert.ParseDouble(dtRow["NextWeek"].ToString());
        oXlsReport.Cell("**ParameterEnd", 0, i).Value = DBConvert.ParseDouble(dtRow["ParameterEnd"].ToString());
        oXlsReport.Cell("**MaxDeadline", 0, i).Value = dtRow["MaxDeadline"].ToString();
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Export IT Staff Registered From To Date
    /// </summary>
    private void ExportRegisterFromToDate()
    {
      string strTemplateName = string.Empty;
      strTemplateName = "RPT_GNR_99_001_03";

      string strSheetName = "Sheet1";
      string strOutFileName = "IT Staff Had Registered From To Date";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[2];

      DateTime fromDate = DBConvert.ParseDateTime(ultdrpDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      DateTime toDate = DBConvert.ParseDateTime(ultdrpDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);

      oXlsReport.Cell("**Date").Value = DBConvert.ParseString(fromDate, ConstantClass.FORMAT_DATETIME) + " To " + DBConvert.ParseString(toDate, ConstantClass.FORMAT_DATETIME);

      // Add Today + 1
      toDate = toDate.AddDays(1);

      arrInput[0] = new DBParameter("@DateFrom", DbType.DateTime, fromDate);
      arrInput[1] = new DBParameter("@DateTo", DbType.DateTime, toDate);

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spGNRITStaffHadRegisterrdOnProject_Select", arrInput);
      for (int i = 0; i < dtData.Rows.Count; i++)
      {
        DataRow dtRow = dtData.Rows[i];
        if (i > 0)
        {
          oXlsReport.Cell("B8:I8").Copy();
          oXlsReport.RowInsert(7 + i);
          oXlsReport.Cell("B8:I8", 0, i).Paste();
        }

        oXlsReport.Cell("**No", 0, i).Value = i + 1;
        oXlsReport.Cell("**Staff", 0, i).Value = dtRow["EmpName"].ToString();
        oXlsReport.Cell("**NameProject", 0, i).Value = dtRow["NameProject"].ToString();
        oXlsReport.Cell("**Type", 0, i).Value = dtRow["Value"].ToString();
        oXlsReport.Cell("**RegisterTime", 0, i).Value = DBConvert.ParseDouble(dtRow["TimeDoIt"].ToString());
        oXlsReport.Cell("**DefaultTime", 0, i).Value = DBConvert.ParseDouble(dtRow["TimeDefault"].ToString());
        oXlsReport.Cell("**ReadEndTime", 0, i).Value = dtRow["RealEndTime"].ToString();
        oXlsReport.Cell("**Remark", 0, i).Value = dtRow["Remark"].ToString();
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Export Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExport_Click(object sender, EventArgs e)
    {
      int value = int.MinValue;
      if (ultCBReport.Value != null)
      {
        value = DBConvert.ParseInt(ultCBReport.Value.ToString());
        string message = string.Empty;

        // Check Valid
        bool success = this.CheckValid(value, out message);
        if (success == false)
        {
          WindowUtinity.ShowMessageError("ERR0001", message);
          return;
        }
        // Follow IT Team Project
        if (value == 1)
        {
          this.ExportFollowITTeamReport();
        } // Follow IT Staff Finished Task Report
        else if (value == 2)
        {
          this.ExportFollowITStaffFinishedReport();
        }
        else if (value == 3)
        {
          this.ExportRegisterFromToDate();
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", "Report");
        return;
      }
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

    /// <summary>
    /// Value Change Report
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCBReport_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBReport.Value != null)
      {
        if (DBConvert.ParseInt(this.ultCBReport.Value.ToString()) == 1)
        {
          this.txtStartDay.Text = "30";
          this.txtEndDay.Text = "30";
        }
        else
        {
          this.txtStartDay.Text = "";
          this.txtEndDay.Text = "";
        }

        int value = DBConvert.ParseInt(ultCBReport.Value.ToString());
        labDateFrom.ForeColor = System.Drawing.SystemColors.ControlText;
        labDateTo.ForeColor = System.Drawing.SystemColors.ControlText;
        labStartDay.ForeColor = System.Drawing.SystemColors.ControlText;
        labEndDay.ForeColor = System.Drawing.SystemColors.ControlText;


        if (value == 1)
        {
          labStartDay.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labEndDay.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
        else if (value == 3)
        {
          labDateFrom.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labDateTo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
      }
    }
    #endregion Event   
  }
}
