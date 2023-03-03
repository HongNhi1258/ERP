/*
  Author        : 
  Create date   : 01/07/2013
  Decription    : Report 1 Day 1 Process
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_22_001 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewPLN_22_001()
    {
      InitializeComponent();
    }

    private void viewPLN_22_001_Load(object sender, EventArgs e)
    {
      this.LoadReport();
    }

    #endregion Init

    #region Function
    private void LoadReport()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 ID, '1 Day 1 Process Carcass By Date' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, '1 Day 1 Process Carcass By Week' Name";
      commandText += " UNION";
      commandText += " SELECT 3 ID, '1 Day 1 Process Carcass By Month' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBReport.DataSource = dtSource;
      ultCBReport.DisplayMember = "Name";
      ultCBReport.ValueMember = "ID";
      ultCBReport.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBReport.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      ultCBReport.DisplayLayout.AutoFitColumns = true;
    }

    private bool CheckParam()
    {
      if (ultCBReport.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Report");
        btnExport.Enabled = true;
        return false;
      }

      if (txtMaterialCode.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "MaterialCode");
        btnExport.Enabled = true;
        return false;
      }

      int value = DBConvert.ParseInt(ultCBReport.Value.ToString());
      if (value == 1)
      {
        if (txtMonth.Text.Length > 0)
        {
          if (DBConvert.ParseInt(txtMonth.Text) < 0 || DBConvert.ParseInt(txtMonth.Text) > 12)
          {
            WindowUtinity.ShowMessageError("ERR0001", "0 < Month <= 12");
            btnExport.Enabled = true;
            return false;
          }
        }

        if (txtYear.Text.Length > 0)
        {
          if (DBConvert.ParseInt(txtYear.Text) < 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "0 < Year");
            btnExport.Enabled = true;
            return false;
          }
        }
      }

      if (value == 2 || value == 3)
      {
        if (txtMonthFrom.Text.Length > 0)
        {
          if (DBConvert.ParseInt(txtMonthFrom.Text) < 0 || DBConvert.ParseInt(txtMonthFrom.Text) > 12)
          {
            WindowUtinity.ShowMessageError("ERR0001", "0 < MonthFrom <= 12");
            btnExport.Enabled = true;
            return false;
          }
        }

        if (txtMonthTo.Text.Length > 0)
        {
          if (DBConvert.ParseInt(txtMonthTo.Text) < 0 || DBConvert.ParseInt(txtMonthTo.Text) > 12)
          {
            WindowUtinity.ShowMessageError("ERR0001", "0 < MonthTo <= 12");
            btnExport.Enabled = true;
            return false;
          }
        }

        if (txtYearFrom.Text.Length > 0)
        {
          if (DBConvert.ParseInt(txtYearFrom.Text) < 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "0 < YearFrom");
            btnExport.Enabled = true;
            return false;
          }
        }

        if (txttYearTo.Text.Length > 0)
        {
          if (DBConvert.ParseInt(txttYearTo.Text) < 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "0 < YearTo");
            btnExport.Enabled = true;
            return false;
          }
        }
      }
      return true;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        btnExport.Enabled = false;
        bool success = this.CheckParam();
        if (success)
        {
          this.ExportExcel();
        }
        btnExport.Enabled = true;
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    private void ExportExcel()
    {
      int value = DBConvert.ParseInt(ultCBReport.Value.ToString());
      if (value == 1)
      {
        this.Report1Day1ProcessCarcassByDate();
      }
      else if (value == 2)
      {
        this.Report1Day1ProcessCarcassByWeek();
      }
      else if (value == 3)
      {
        this.Report1Day1ProcessCarcassByMonth();
      }
    }

    private void Report1Day1ProcessCarcassByDate()
    {
      string strTemplateName = "RPT_PLN_22_001_01";
      string strSheetName = "Sheet1";
      string strOutFileName = "1 Day 1 Process Carcass By Date";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DBParameter[] input = new DBParameter[3];
      if (txtMonth.Text.Length > 0 && DBConvert.ParseInt(txtMonth.Text) != int.MinValue)
      {
        input[0] = new DBParameter("@Month", DbType.Int32, DBConvert.ParseInt(txtMonth.Text));
      }

      if (txtYear.Text.Length > 0 && DBConvert.ParseInt(txtYear.Text) != int.MinValue)
      {
        input[1] = new DBParameter("@Year", DbType.Int32, DBConvert.ParseInt(txtYear.Text));
      }

      if (txtMaterialCode.Text.Length > 0)
      {
        input[2] = new DBParameter("@MaterialCode", DbType.String, txtMaterialCode.Text);
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNRPT1Day1ProcessMaterialWithPR_Select", input);
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:M8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:M8", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**WDate", 0, i).Value = dtRow["WDate"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEn", 0, i).Value = dtRow["MaterialNameEn"].ToString();
          oXlsReport.Cell("**NameVn", 0, i).Value = dtRow["MaterialNameVn"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();
          if (DBConvert.ParseDouble(dtRow["BOH"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**BOH", 0, i).Value = DBConvert.ParseDouble(dtRow["BOH"].ToString());
          }
          else
          {
            oXlsReport.Cell("**BOH", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["PRReceived"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**PRReceived", 0, i).Value = DBConvert.ParseDouble(dtRow["PRReceived"].ToString());
          }
          else
          {
            oXlsReport.Cell("**PRReceived", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["PRPending"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**PRPending", 0, i).Value = DBConvert.ParseDouble(dtRow["PRPending"].ToString());
          }
          else
          {
            oXlsReport.Cell("**PRPending", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["StockPR"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**StockPR", 0, i).Value = DBConvert.ParseDouble(dtRow["StockPR"].ToString());
          }
          else
          {
            oXlsReport.Cell("**StockPR", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["NeedQty"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**NeedQty", 0, i).Value = DBConvert.ParseDouble(dtRow["NeedQty"].ToString());
          }
          else
          {
            oXlsReport.Cell("**NeedQty", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Remain"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Remain", 0, i).Value = DBConvert.ParseDouble(dtRow["Remain"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Remain", 0, i).Value = DBNull.Value;
          }
        }
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void Report1Day1ProcessCarcassByWeek()
    {
      string strTemplateName = "RPT_PLN_22_001_02";
      string strSheetName = "Sheet1";
      string strOutFileName = "1 Day 1 Process Carcass By Week";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DBParameter[] input = new DBParameter[5];
      if (txtMonthFrom.Text.Length > 0 && DBConvert.ParseInt(txtMonthFrom.Text) != int.MinValue)
      {
        input[0] = new DBParameter("@MonthFrom", DbType.Int32, DBConvert.ParseInt(txtMonthFrom.Text));
      }

      if (txtMonthTo.Text.Length > 0 && DBConvert.ParseInt(txtMonthTo.Text) != int.MinValue)
      {
        input[1] = new DBParameter("@MonthTo", DbType.Int32, DBConvert.ParseInt(txtMonthTo.Text));
      }

      if (txtYearFrom.Text.Length > 0 && DBConvert.ParseInt(txtYearFrom.Text) != int.MinValue)
      {
        input[2] = new DBParameter("@YearFrom", DbType.Int32, DBConvert.ParseInt(txtYearFrom.Text));
      }

      if (txttYearTo.Text.Length > 0 && DBConvert.ParseInt(txttYearTo.Text) != int.MinValue)
      {
        input[3] = new DBParameter("@YearTo", DbType.Int32, DBConvert.ParseInt(txttYearTo.Text));
      }

      if (txtMaterialCode.Text.Length > 0)
      {
        input[4] = new DBParameter("@MaterialCode", DbType.String, txtMaterialCode.Text);
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNRPT1Day1ProcessMaterialWithPRGroupByWeek_Select", input);
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:N8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:N8", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          if (DBConvert.ParseInt(dtRow["MonthNo"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**MonthNo", 0, i).Value = DBConvert.ParseInt(dtRow["MonthNo"].ToString());
          }
          else
          {
            oXlsReport.Cell("**MonthNo", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseInt(dtRow["WeekNo"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**WeekNo", 0, i).Value = DBConvert.ParseInt(dtRow["WeekNo"].ToString());
          }
          else
          {
            oXlsReport.Cell("**WeekNo", 0, i).Value = DBNull.Value;
          }

          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEn", 0, i).Value = dtRow["MaterialNameEn"].ToString();
          oXlsReport.Cell("**NameVn", 0, i).Value = dtRow["MaterialNameVn"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();
          if (DBConvert.ParseDouble(dtRow["BOH"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**BOH", 0, i).Value = DBConvert.ParseDouble(dtRow["BOH"].ToString());
          }
          else
          {
            oXlsReport.Cell("**BOH", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["PRReceived"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**PRReceived", 0, i).Value = DBConvert.ParseDouble(dtRow["PRReceived"].ToString());
          }
          else
          {
            oXlsReport.Cell("**PRReceived", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["PRPending"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**PRPending", 0, i).Value = DBConvert.ParseDouble(dtRow["PRPending"].ToString());
          }
          else
          {
            oXlsReport.Cell("**PRPending", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["StockPR"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**StockPR", 0, i).Value = DBConvert.ParseDouble(dtRow["StockPR"].ToString());
          }
          else
          {
            oXlsReport.Cell("**StockPR", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["NeedQty"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**NeedQty", 0, i).Value = DBConvert.ParseDouble(dtRow["NeedQty"].ToString());
          }
          else
          {
            oXlsReport.Cell("**NeedQty", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Remain"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Remain", 0, i).Value = DBConvert.ParseDouble(dtRow["Remain"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Remain", 0, i).Value = DBNull.Value;
          }
        }
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void Report1Day1ProcessCarcassByMonth()
    {
      string strTemplateName = "RPT_PLN_22_001_03";
      string strSheetName = "Sheet1";
      string strOutFileName = "1 Day 1 Process Carcass By Month";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DBParameter[] input = new DBParameter[5];
      if (txtMonthFrom.Text.Length > 0 && DBConvert.ParseInt(txtMonthFrom.Text) != int.MinValue)
      {
        input[0] = new DBParameter("@MonthFrom", DbType.Int32, DBConvert.ParseInt(txtMonthFrom.Text));
      }

      if (txtMonthTo.Text.Length > 0 && DBConvert.ParseInt(txtMonthTo.Text) != int.MinValue)
      {
        input[1] = new DBParameter("@MonthTo", DbType.Int32, DBConvert.ParseInt(txtMonthTo.Text));
      }

      if (txtYearFrom.Text.Length > 0 && DBConvert.ParseInt(txtYearFrom.Text) != int.MinValue)
      {
        input[2] = new DBParameter("@YearFrom", DbType.Int32, DBConvert.ParseInt(txtYearFrom.Text));
      }

      if (txttYearTo.Text.Length > 0 && DBConvert.ParseInt(txttYearTo.Text) != int.MinValue)
      {
        input[3] = new DBParameter("@YearTo", DbType.Int32, DBConvert.ParseInt(txttYearTo.Text));
      }

      if (txtMaterialCode.Text.Length > 0)
      {
        input[4] = new DBParameter("@MaterialCode", DbType.String, txtMaterialCode.Text);
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNRPT1Day1ProcessMaterialWithPRGroupByMonth_Select", input);
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:N8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:N8", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          if (DBConvert.ParseInt(dtRow["MonthNo"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**MonthNo", 0, i).Value = DBConvert.ParseInt(dtRow["MonthNo"].ToString());
          }
          else
          {
            oXlsReport.Cell("**MonthNo", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseInt(dtRow["YearNo"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**YearNo", 0, i).Value = DBConvert.ParseInt(dtRow["YearNo"].ToString());
          }
          else
          {
            oXlsReport.Cell("**YearNo", 0, i).Value = DBNull.Value;
          }

          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEn", 0, i).Value = dtRow["MaterialNameEn"].ToString();
          oXlsReport.Cell("**NameVn", 0, i).Value = dtRow["MaterialNameVn"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();
          if (DBConvert.ParseDouble(dtRow["BOH"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**BOH", 0, i).Value = DBConvert.ParseDouble(dtRow["BOH"].ToString());
          }
          else
          {
            oXlsReport.Cell("**BOH", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["PRReceived"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**PRReceived", 0, i).Value = DBConvert.ParseDouble(dtRow["PRReceived"].ToString());
          }
          else
          {
            oXlsReport.Cell("**PRReceived", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["PRPending"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**PRPending", 0, i).Value = DBConvert.ParseDouble(dtRow["PRPending"].ToString());
          }
          else
          {
            oXlsReport.Cell("**PRPending", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["StockPR"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**StockPR", 0, i).Value = DBConvert.ParseDouble(dtRow["StockPR"].ToString());
          }
          else
          {
            oXlsReport.Cell("**StockPR", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["NeedQty"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**NeedQty", 0, i).Value = DBConvert.ParseDouble(dtRow["NeedQty"].ToString());
          }
          else
          {
            oXlsReport.Cell("**NeedQty", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Remain"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Remain", 0, i).Value = DBConvert.ParseDouble(dtRow["Remain"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Remain", 0, i).Value = DBNull.Value;
          }
        }
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }
    #endregion Function

    #region Event
    private void ultCBReport_ValueChanged(object sender, EventArgs e)
    {
      int value = int.MinValue;
      if (ultCBReport.Value != null)
      {
        labMonth.ForeColor = System.Drawing.SystemColors.ControlText;
        labYear.ForeColor = System.Drawing.SystemColors.ControlText;
        labMonthFrom.ForeColor = System.Drawing.SystemColors.ControlText;
        labMonthTo.ForeColor = System.Drawing.SystemColors.ControlText;
        labYearFrom.ForeColor = System.Drawing.SystemColors.ControlText;
        labYearTo.ForeColor = System.Drawing.SystemColors.ControlText;
        labMaterialCode.ForeColor = System.Drawing.SystemColors.ControlText;

        value = DBConvert.ParseInt(ultCBReport.Value.ToString());
        if (value == 1)
        {
          labMonth.ForeColor = System.Drawing.SystemColors.HotTrack;
          labYear.ForeColor = System.Drawing.SystemColors.HotTrack;
          labMaterialCode.ForeColor = System.Drawing.SystemColors.HotTrack;
        }
        else if (value == 2 || value == 3)
        {
          labMonthFrom.ForeColor = System.Drawing.SystemColors.HotTrack;
          labMonthTo.ForeColor = System.Drawing.SystemColors.HotTrack;
          labYearFrom.ForeColor = System.Drawing.SystemColors.HotTrack;
          labYearTo.ForeColor = System.Drawing.SystemColors.HotTrack;
          labMaterialCode.ForeColor = System.Drawing.SystemColors.HotTrack;
        }
      }
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      btnExport.Enabled = false;
      bool success = this.CheckParam();
      if (success)
      {
        this.ExportExcel();
      }
      btnExport.Enabled = true;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event
  }
}
