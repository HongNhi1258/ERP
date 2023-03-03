/*
  Author        : 
  Create date   : 19/12/2013
  Decription    : Report
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

namespace DaiCo.Planning
{
  public partial class viewPLN_07_104 : MainUserControl
  {
    #region Field
    long wo = long.MinValue;
    #endregion Field

    #region Init
    public viewPLN_07_104()
    {
      InitializeComponent();
    }

    private void viewPLN_07_104_Load(object sender, EventArgs e)
    {
      this.LoadReport();
      this.LoadWO();
    }

    #endregion Init

    #region Function
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.ExportData();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    private void LoadReport()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 ID, 'Closing WO' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBReport.DataSource = dtSource;
      ultCBReport.DisplayMember = "Name";
      ultCBReport.ValueMember = "ID";
      ultCBReport.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBReport.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      ultCBReport.DisplayLayout.AutoFitColumns = true;
    }

    private void LoadWO()
    {
      string commandText = "SELECT Pid FROM TblPLNWorkOrder ORDER BY Pid DESC";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBWO.DataSource = dtSource;
      ultCBWO.DisplayMember = "Pid";
      ultCBWO.ValueMember = "Pid";
      ultCBWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBWO.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Export Data
    /// </summary>
    private void ExportData()
    {
      btnExport.Enabled = false;
      // Check Report
      if (ultCBReport.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Report");
        btnExport.Enabled = true;
        return;
      }

      int value = DBConvert.ParseInt(ultCBReport.Value.ToString());
      if (value == 1)
      {
        if (ultCBWO.Value == null)
        {
          WindowUtinity.ShowMessageError("ERR0001", "WO");
          btnExport.Enabled = true;
          return;
        }
        this.ReportClosingWO();
      }
      btnExport.Enabled = true;
    }

    /// <summary>
    /// Closing WO
    /// </summary>
    private void ReportClosingWO()
    {
      string strTemplateName = "RPT_PLN_07_104_03";
      string strSheetName = "Sheet1";
      string strOutFileName = "Closing WO";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@Wo", DbType.Int64, DBConvert.ParseLong(ultCBWO.Value.ToString()));

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNRPTMaterialCodeClosingWO_Select", input);
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:R8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:R8", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**CloseDate", 0, i).Value = dtRow["CloseDate"].ToString();
          if (DBConvert.ParseLong(dtRow["WO"].ToString()) != long.MinValue)
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBConvert.ParseLong(dtRow["WO"].ToString());
          }
          else
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBNull.Value;
          }

          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
          oXlsReport.Cell("**Revision", 0, i).Value = DBConvert.ParseInt(dtRow["Revision"].ToString());
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**Name", 0, i).Value = dtRow["MaterialName"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();

          if (DBConvert.ParseDouble(dtRow["TotalMaterial"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**TotalQty", 0, i).Value = DBConvert.ParseDouble(dtRow["TotalMaterial"].ToString());
          }
          else
          {
            oXlsReport.Cell("**TotalQty", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Allocated"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Allocated", 0, i).Value = DBConvert.ParseDouble(dtRow["Allocated"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Allocated", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Supplement"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Supplement", 0, i).Value = DBConvert.ParseDouble(dtRow["Supplement"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Supplement", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Issued"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Issued", 0, i).Value = DBConvert.ParseDouble(dtRow["Issued"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Issued", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["NonIssue"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**NonIssue", 0, i).Value = DBConvert.ParseDouble(dtRow["NonIssue"].ToString());
          }
          else
          {
            oXlsReport.Cell("**NonIssue", 0, i).Value = DBNull.Value;
          }

          if (dtRow["PercentFGW"].ToString().Length > 0)
          {
            oXlsReport.Cell("**FWGCompleted", 0, i).Value = dtRow["PercentFGW"].ToString();
          }
          else
          {
            oXlsReport.Cell("**FWGCompleted", 0, i).Value = DBNull.Value;
          }

          oXlsReport.Cell("**MATCompleted", 0, i).Value = dtRow["PercentMAT"].ToString();
          oXlsReport.Cell("**Status", 0, i).Value = dtRow["Status"].ToString();
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
        labWO.ForeColor = System.Drawing.SystemColors.ControlText;

        value = DBConvert.ParseInt(ultCBReport.Value.ToString());
        if (value == 1)
        {
          // Allocate Department
          labWO.ForeColor = System.Drawing.SystemColors.HotTrack;
        }
      }
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      this.ExportData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Even
  }
}
