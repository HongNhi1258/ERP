/*
  Author        : 
  Create date   : 24/06/2013
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
  public partial class viewPLN_21_104 : MainUserControl
  {
    #region Field
    long wo = long.MinValue;
    #endregion Field

    #region Init
    public viewPLN_21_104()
    {
      InitializeComponent();
    }

    private void viewPLN_21_104_Load(object sender, EventArgs e)
    {
      this.LoadReport();
      this.LoadMaterialCode();
      this.LoadDepartment();
      this.LoadWO();
      this.LoadCarcass(wo);
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
      commandText += " SELECT 1 ID, 'Allocate Department' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Supplement' Name";
      commandText += " UNION";
      commandText += " SELECT 3 ID, 'Closing WO' Name";
      commandText += " UNION";
      commandText += " SELECT 5 ID, 'Allocate Special' Name";
      commandText += " UNION";
      commandText += " SELECT 6 ID, 'Allocate Special Detail' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBReport.DataSource = dtSource;
      ultCBReport.DisplayMember = "Name";
      ultCBReport.ValueMember = "ID";
      ultCBReport.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBReport.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      ultCBReport.DisplayLayout.AutoFitColumns = true;
    }

    private void LoadDepartment()
    {
      string commandText = string.Empty;
      commandText += " SELECT Department, Department + ' - ' + DeparmentName AS DeparmentName FROM VHRDDepartment";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBDepartment.DataSource = dtSource;
      ultCBDepartment.DisplayMember = "DeparmentName";
      ultCBDepartment.ValueMember = "Department";
      ultCBDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
      ultCBDepartment.DisplayLayout.AutoFitColumns = true;
    }

    private void LoadMaterialCode()
    {
      string commandText = string.Empty;
      commandText += "SELECT MaterialCode, MaterialCode + ' - ' + MaterialNameVn AS Name FROM VBOMMaterials WHERE Warehouse = 2 ORDER BY MaterialCode";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBMaterialCode.DataSource = dtSource;
      ultCBMaterialCode.DisplayMember = "Name";
      ultCBMaterialCode.ValueMember = "MaterialCode";
      ultCBMaterialCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBMaterialCode.DisplayLayout.Bands[0].Columns["MaterialCode"].Hidden = true;
      ultCBMaterialCode.DisplayLayout.AutoFitColumns = true;
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

    private void LoadCarcass(long wo)
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT CarcassCode ";
      commandText += " FROM dbo.TblPLNWorkOrderConfirmedDetails ";
      if (wo != long.MinValue)
      {
        commandText += " WHERE WorkOrderPid = " + wo;
      }
      commandText += " ORDER BY CarcassCode ";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultCBCarcassCode.DataSource = dtSource;
        ultCBCarcassCode.ValueMember = "CarcassCode";
        ultCBCarcassCode.DisplayMember = "CarcassCode";
        ultCBCarcassCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultCBCarcassCode.DisplayLayout.AutoFitColumns = true;
      }
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
        this.ReportAllocateDepartment();
      }
      else if (value == 2)
      {
        this.ReportSupplementList();
      }
      else if (value == 3)
      {
        if (ultCBWO.Value == null)
        {
          WindowUtinity.ShowMessageError("ERR0001", "WO");
          btnExport.Enabled = true;
          return;
        }
        this.ReportClosingWO();
      }
      else if (value == 5)
      {
        this.ReportAllocateSpecial();
      }
      else if (value == 6)
      {
        this.ReportAllocateSpecialDetail();
      }
      btnExport.Enabled = true;
    }
    /// <summary>
    /// Allocation Department
    /// </summary>
    private void ReportAllocateDepartment()
    {
      string strTemplateName = "RPT_PLN_21_104_01";
      string strSheetName = "Sheet1";
      string strOutFileName = "Department Allocation";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DBParameter[] input = new DBParameter[4];
      if (ultDTDateFrom.Value != null)
      {
        input[0] = new DBParameter("@DateFrom", DbType.DateTime, DBConvert.ParseDateTime(ultDTDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }
      if (ultDTDateTo.Value != null)
      {
        input[1] = new DBParameter("@DateTo", DbType.DateTime, DBConvert.ParseDateTime(ultDTDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME).AddDays(1));
      }
      if (ultCBDepartment.Value != null && ultCBDepartment.Text.Length > 0)
      {
        input[2] = new DBParameter("@Department", DbType.String, ultCBDepartment.Value.ToString());
      }
      if (ultCBMaterialCode.Value != null && ultCBMaterialCode.Text.Length > 0)
      {
        input[3] = new DBParameter("@MaterialCode", DbType.String, ultCBMaterialCode.Value.ToString());
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNRPTVeneerAllocateDepartment_Select", input);
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:K8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:K8", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**CreateDate", 0, i).Value = dtRow["CreateDate"].ToString();
          oXlsReport.Cell("**Department", 0, i).Value = dtRow["Department"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**Name", 0, i).Value = dtRow["Name"].ToString();
          if (DBConvert.ParseDouble(dtRow["Qty"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["Qty"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Allocated"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Allocated", 0, i).Value = DBConvert.ParseDouble(dtRow["Allocated"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Allocated", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Issued"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Issued", 0, i).Value = DBConvert.ParseDouble(dtRow["Issued"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Issued", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Remain"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Remain", 0, i).Value = DBConvert.ParseDouble(dtRow["Remain"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Remain", 0, i).Value = DBNull.Value;
          }

          oXlsReport.Cell("**Reason", 0, i).Value = dtRow["Reason"].ToString();
        }
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Supplement
    /// </summary>
    private void ReportSupplementList()
    {
      string strTemplateName = "RPT_PLN_21_104_02";
      string strSheetName = "Sheet1";
      string strOutFileName = "Supplement";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DBParameter[] input = new DBParameter[5];
      if (ultDTDateFrom.Value != null)
      {
        input[0] = new DBParameter("@DateFrom", DbType.DateTime, DBConvert.ParseDateTime(ultDTDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }
      if (ultDTDateTo.Value != null)
      {
        input[1] = new DBParameter("@DateTo", DbType.DateTime, DBConvert.ParseDateTime(ultDTDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME).AddDays(1));
      }
      if (ultCBDepartment.Value != null && ultCBDepartment.Text.Length > 0)
      {
        input[2] = new DBParameter("@Department", DbType.String, ultCBDepartment.Value.ToString());
      }
      if (ultCBWO.Value != null && ultCBWO.Text.Length > 0)
      {
        input[3] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ultCBWO.Value.ToString()));
      }
      if (ultCBCarcassCode.Value != null && ultCBCarcassCode.Text.Length > 0)
      {
        input[4] = new DBParameter("@CarcassCode", DbType.String, ultCBCarcassCode.Value.ToString());
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNRPTVeneerSupplementList_Select", input);
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:L8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:L8", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**CreateDate", 0, i).Value = dtRow["CreateDate"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**Name", 0, i).Value = dtRow["Name"].ToString();
          if (DBConvert.ParseLong(dtRow["WO"].ToString()) != long.MinValue)
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBConvert.ParseLong(dtRow["WO"].ToString());
          }
          else
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBNull.Value;
          }

          oXlsReport.Cell("**Carcass", 0, i).Value = dtRow["CarcassCode"].ToString();
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();

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

          if (DBConvert.ParseDouble(dtRow["Remain"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Remain", 0, i).Value = DBConvert.ParseDouble(dtRow["Remain"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Remain", 0, i).Value = DBNull.Value;
          }

          oXlsReport.Cell("**Reason", 0, i).Value = dtRow["Reason"].ToString();
        }
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Closing WO
    /// </summary>
    private void ReportClosingWO()
    {
      string strTemplateName = "RPT_PLN_21_104_03";
      string strSheetName = "Sheet1";
      string strOutFileName = "Closing WO";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DBParameter[] input = new DBParameter[5];
      if (ultDTDateFrom.Value != null)
      {
        input[0] = new DBParameter("@DateFrom", DbType.DateTime, DBConvert.ParseDateTime(ultDTDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }
      if (ultDTDateTo.Value != null)
      {
        input[1] = new DBParameter("@DateTo", DbType.DateTime, DBConvert.ParseDateTime(ultDTDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME).AddDays(1));
      }
      if (ultCBWO.Value != null && ultCBWO.Text.Length > 0)
      {
        input[3] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ultCBWO.Value.ToString()));
      }
      if (ultCBCarcassCode.Value != null && ultCBCarcassCode.Text.Length > 0)
      {
        input[4] = new DBParameter("@CarcassCode", DbType.String, ultCBCarcassCode.Value.ToString());
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNRPTVeneerClosingWO_Select", input);
      if (dtData != null && dtData.Rows.Count > 0)
      {
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
          oXlsReport.Cell("**CloseDate", 0, i).Value = dtRow["CloseDate"].ToString();
          if (DBConvert.ParseLong(dtRow["WO"].ToString()) != long.MinValue)
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBConvert.ParseLong(dtRow["WO"].ToString());
          }
          else
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBNull.Value;
          }

          oXlsReport.Cell("**CarcassCode", 0, i).Value = dtRow["CarcassCode"].ToString();
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MainCode"].ToString();
          oXlsReport.Cell("**Name", 0, i).Value = dtRow["MainName"].ToString();
          if (dtRow["AltCode"].ToString().Length > 0)
          {
            oXlsReport.Cell("**AltMaterialCode", 0, i).Value = dtRow["AltCode"].ToString();
          }
          else
          {
            oXlsReport.Cell("**AltMaterialCode", 0, i).Value = "";
          }

          if (dtRow["AltName"].ToString().Length > 0)
          {
            oXlsReport.Cell("**AltName", 0, i).Value = dtRow["AltName"].ToString();
          }
          else
          {
            oXlsReport.Cell("**AltName", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(dtRow["TotalQty"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**TotalQty", 0, i).Value = DBConvert.ParseDouble(dtRow["TotalQty"].ToString());
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

          oXlsReport.Cell("**CSTCompleted", 0, i).Value = dtRow["PercentCST"].ToString();
          oXlsReport.Cell("**FWGCompleted", 0, i).Value = dtRow["PercentFGW"].ToString();
          oXlsReport.Cell("**MATCompleted", 0, i).Value = dtRow["PercentMAT"].ToString();
          oXlsReport.Cell("**Status", 0, i).Value = dtRow["Status"].ToString();
        }
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Allocate Special
    /// </summary>
    private void ReportAllocateSpecial()
    {
      string strTemplateName = "RPT_PLN_21_104_05";
      string strSheetName = "Sheet1";
      string strOutFileName = "Allocate Special";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DBParameter[] input = new DBParameter[3];
      if (ultCBWO.Value != null && ultCBWO.Text.Length > 0)
      {
        input[0] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ultCBWO.Value.ToString()));
      }
      if (ultCBCarcassCode.Value != null && ultCBCarcassCode.Text.Length > 0)
      {
        input[1] = new DBParameter("@CarcassCode", DbType.String, ultCBCarcassCode.Value.ToString());
      }
      if (ultCBMaterialCode.Value != null && ultCBMaterialCode.Text.Length > 0)
      {
        input[2] = new DBParameter("@MaterialCode", DbType.String, ultCBMaterialCode.Value.ToString());
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNRPTVeneerAllocateSpecial_Select", input);
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:K8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:K8", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**WO", 0, i).Value = DBConvert.ParseLong(dtRow["WO"].ToString());
          oXlsReport.Cell("**Carcass", 0, i).Value = dtRow["CarcassCode"].ToString();
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameVn", 0, i).Value = dtRow["MaterialNameEn"].ToString();
          oXlsReport.Cell("**NameEn", 0, i).Value = dtRow["MaterialNameVn"].ToString();
          oXlsReport.Cell("**Location", 0, i).Value = dtRow["Location"].ToString();
          if (DBConvert.ParseInt(dtRow["QtyAllocated"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**QtyAllocated", 0, i).Value = DBConvert.ParseInt(dtRow["QtyAllocated"].ToString());
          }
          else
          {
            oXlsReport.Cell("**QtyAllocated", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["QtyAllocatedM2"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**QtyAllocatedM2", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyAllocatedM2"].ToString());
          }
          else
          {
            oXlsReport.Cell("**QtyAllocatedM2", 0, i).Value = DBNull.Value;
          }
        }
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Allocate Special Detail
    /// </summary>
    private void ReportAllocateSpecialDetail()
    {
      string strTemplateName = "RPT_PLN_21_104_06";
      string strSheetName = "Sheet1";
      string strOutFileName = "Allocate Special Detail";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DBParameter[] input = new DBParameter[3];
      if (ultCBWO.Value != null && ultCBWO.Text.Length > 0)
      {
        input[0] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ultCBWO.Value.ToString()));
      }
      if (ultCBCarcassCode.Value != null && ultCBCarcassCode.Text.Length > 0)
      {
        input[1] = new DBParameter("@CarcassCode", DbType.String, ultCBCarcassCode.Value.ToString());
      }
      if (ultCBMaterialCode.Value != null && ultCBMaterialCode.Text.Length > 0)
      {
        input[2] = new DBParameter("@MaterialCode", DbType.String, ultCBMaterialCode.Value.ToString());
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNRPTVeneerAllocateSpecialDetail_Select", input);
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:Q8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:Q8", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**WO", 0, i).Value = DBConvert.ParseLong(dtRow["WO"].ToString());
          oXlsReport.Cell("**Carcass", 0, i).Value = dtRow["CarcassCode"].ToString();
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameVn", 0, i).Value = dtRow["MaterialNameEn"].ToString();
          oXlsReport.Cell("**NameEn", 0, i).Value = dtRow["MaterialNameVn"].ToString();
          oXlsReport.Cell("**IDVeneer", 0, i).Value = dtRow["LotNoId"].ToString();
          if (DBConvert.ParseDouble(dtRow["Length"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Length", 0, i).Value = DBConvert.ParseDouble(dtRow["Length"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Length", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Width"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Width", 0, i).Value = DBConvert.ParseDouble(dtRow["Width"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Width", 0, i).Value = DBNull.Value;
          }

          oXlsReport.Cell("**Location", 0, i).Value = dtRow["Location"].ToString();
          if (DBConvert.ParseInt(dtRow["QtyAllocated"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**QtyAllocated", 0, i).Value = DBConvert.ParseInt(dtRow["QtyAllocated"].ToString());
          }
          else
          {
            oXlsReport.Cell("**QtyAllocated", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["QtyAllocatedM2"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**QtyAllocatedM2", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyAllocatedM2"].ToString());
          }
          else
          {
            oXlsReport.Cell("**QtyAllocatedM2", 0, i).Value = DBNull.Value;
          }
          oXlsReport.Cell("**AllocatedSpecialDate", 0, i).Value = dtRow["AllocatedSpecialDate"].ToString();
          oXlsReport.Cell("**WIPCurrent", 0, i).Value = dtRow["WIPCurrent"].ToString();
          if (DBConvert.ParseDouble(dtRow["MCHComplete"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**MCHComplete", 0, i).Value = DBConvert.ParseDouble(dtRow["MCHComplete"].ToString());
          }
          else
          {
            oXlsReport.Cell("**MCHComplete", 0, i).Value = DBNull.Value;
          }
          if (DBConvert.ParseInt(dtRow["ShippedQty"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**ShippedQty", 0, i).Value = DBConvert.ParseInt(dtRow["ShippedQty"].ToString());
          }
          else
          {
            oXlsReport.Cell("**ShippedQty", 0, i).Value = DBNull.Value;
          }
        }
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    #endregion Function

    #region Event
    private void ultCBWO_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBWO.Value != null && ultCBWO.Text.Length > 0)
      {
        this.LoadCarcass(DBConvert.ParseLong(ultCBWO.Value.ToString()));
      }
      else
      {
        this.LoadCarcass(long.MinValue);
      }
    }

    private void ultCBReport_ValueChanged(object sender, EventArgs e)
    {
      int value = int.MinValue;
      if (ultCBReport.Value != null)
      {
        labDateFrom.ForeColor = System.Drawing.SystemColors.ControlText;
        labDateTo.ForeColor = System.Drawing.SystemColors.ControlText;
        labWO.ForeColor = System.Drawing.SystemColors.ControlText;
        labCarcass.ForeColor = System.Drawing.SystemColors.ControlText;
        labDepartment.ForeColor = System.Drawing.SystemColors.ControlText;
        labMaterialCode.ForeColor = System.Drawing.SystemColors.ControlText;

        value = DBConvert.ParseInt(ultCBReport.Value.ToString());
        if (value == 1)
        {
          // Allocate Department
          labDateFrom.ForeColor = System.Drawing.SystemColors.HotTrack;
          labDateTo.ForeColor = System.Drawing.SystemColors.HotTrack;
          labDepartment.ForeColor = System.Drawing.SystemColors.HotTrack;
        }
        else if (value == 2)
        {
          // Supplement
          labDateFrom.ForeColor = System.Drawing.SystemColors.HotTrack;
          labDateTo.ForeColor = System.Drawing.SystemColors.HotTrack;
          labWO.ForeColor = System.Drawing.SystemColors.HotTrack;
          labCarcass.ForeColor = System.Drawing.SystemColors.HotTrack;
        }
        else if (value == 3)
        {
          // Closing WO
          labDateFrom.ForeColor = System.Drawing.SystemColors.HotTrack;
          labDateTo.ForeColor = System.Drawing.SystemColors.HotTrack;
          labWO.ForeColor = System.Drawing.SystemColors.HotTrack;
          labCarcass.ForeColor = System.Drawing.SystemColors.HotTrack;
        }
        else if (value == 5 || value == 6)
        {
          // Allocate Special 
          labWO.ForeColor = System.Drawing.SystemColors.HotTrack;
          labCarcass.ForeColor = System.Drawing.SystemColors.HotTrack;
          labMaterialCode.ForeColor = System.Drawing.SystemColors.HotTrack;
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
