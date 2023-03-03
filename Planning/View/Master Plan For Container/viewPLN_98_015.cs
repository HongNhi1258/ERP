/*
  Author      : 
  Date        : 23/09/2013
  Description : Check Report
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_98_015 : MainUserControl
  {
    #region Field
    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;

    #endregion Field

    #region Init
    public viewPLN_98_015()
    {
      InitializeComponent();
    }

    private void viewPLN_98_015_Load(object sender, EventArgs e)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      this.pathTemplate = startupPath + @"\ExcelTemplate";
      this.pathExport = startupPath + @"\Report";
    }
    #endregion Init

    #region Function

    /// <summary>
    /// ProcessCmdKey
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;

      // ItemCode
      string listOfItemCode = string.Empty;
      if (txtItemCode.Text.Length > 0)
      {
        listOfItemCode = txtItemCode.Text;
      }

      if (txtListItemCode.Text.Length > 0)
      {
        DataTable dtSource = new DataTable();
        // Import Excel
        try
        {
          dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtListItemCode.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B3:B200]").Tables[0];
        }
        catch
        {
          dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtListItemCode.Text.Trim(), "SELECT * FROM [Sheet1$B3:B200]").Tables[0];
        }
        if (dtSource != null)
        {
          for (int i = 0; i < dtSource.Rows.Count; i++)
          {
            if (dtSource.Rows[i][0].ToString().Length > 0)
            {
              listOfItemCode = listOfItemCode + ";" + dtSource.Rows[i][0].ToString();
            }
          }
        }
      }

      if (listOfItemCode.Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "ItemCode");
        btnSearch.Enabled = true;
        return;
      }

      DBParameter[] input = new DBParameter[2];
      if (txtCustomerPONo.Text.Length > 0)
      {
        input[0] = new DBParameter("@CustomerPONo", DbType.String, txtCustomerPONo.Text);
      }

      // ItemCode
      if (listOfItemCode.Length > 0)
      {
        input[1] = new DBParameter("@ItemCode", DbType.String, listOfItemCode);
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNCheckWOLinkSO_Select", 10000, input);
      if (dsSource != null)
      {
        this.ultWIP.DataSource = null;
        ultWIP.DataSource = dsSource.Tables[0];

        for (int i = 0; i < this.ultWIP.Rows.Count; i++)
        {
          UltraGridRow row = this.ultWIP.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 1)
          {
            ultWIP.Rows[i].CellAppearance.BackColor = Color.LightGreen;
          }
        }

        this.ultCancellation.DataSource = null;
        ultCancellation.DataSource = dsSource.Tables[1];

        this.ultWOLinkSOBeforeShipped.DataSource = null;
        ultWOLinkSOBeforeShipped.DataSource = dsSource.Tables[2];
        for (int i = 0; i < this.ultWOLinkSOBeforeShipped.Rows.Count; i++)
        {
          UltraGridRow row = this.ultWOLinkSOBeforeShipped.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 1)
          {
            ultWOLinkSOBeforeShipped.Rows[i].CellAppearance.BackColor = Color.LightGreen;
          }
        }

        this.ultShippedQty.DataSource = null;
        ultShippedQty.DataSource = dsSource.Tables[3];

        this.ultWOLinkSOAfterShipped.DataSource = null;
        ultWOLinkSOAfterShipped.DataSource = dsSource.Tables[4];
        for (int i = 0; i < this.ultWOLinkSOAfterShipped.Rows.Count; i++)
        {
          UltraGridRow row = this.ultWOLinkSOAfterShipped.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 1)
          {
            ultWOLinkSOAfterShipped.Rows[i].CellAppearance.BackColor = Color.LightGreen;
          }
        }

        this.ultResult.DataSource = null;
        ultResult.DataSource = dsSource.Tables[5];
        for (int i = 0; i < this.ultResult.Rows.Count; i++)
        {
          UltraGridRow row = this.ultResult.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 1)
          {
            ultResult.Rows[i].CellAppearance.BackColor = Color.LightGreen;
          }
        }
      }
      btnSearch.Enabled = true;
    }

    private void ExportWIP()
    {
      string strTemplateName = "RPT_PLN_98_015_02";
      string strSheetName = "Sheet1";
      string strOutFileName = "WIP";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DataTable dtData = (DataTable)ultWIP.DataSource;
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B5:F5").Copy();
            oXlsReport.RowInsert(4 + i);
            oXlsReport.Cell("B5:F5", 0, i).Paste();
          }
          if (DBConvert.ParseInt(dtRow["WO"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBConvert.ParseInt(dtRow["WO"].ToString());
          }
          else
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBNull.Value;
          }
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
          oXlsReport.Cell("**Revision", 0, i).Value = DBConvert.ParseInt(dtRow["Revision"].ToString());
          if (DBConvert.ParseDouble(dtRow["Qty"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["Qty"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBNull.Value;
          }
          oXlsReport.Cell("**StandByEN", 0, i).Value = dtRow["StandByEN"].ToString();
        }
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void ExportCancellation()
    {
      string strTemplateName = "RPT_PLN_98_015_03";
      string strSheetName = "Sheet1";
      string strOutFileName = "Cancellation";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DataTable dtData = (DataTable)ultCancellation.DataSource;
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B5:F5").Copy();
            oXlsReport.RowInsert(4 + i);
            oXlsReport.Cell("B5:F5", 0, i).Paste();
          }
          oXlsReport.Cell("**SaleNo", 0, i).Value = dtRow["SaleNo"].ToString();
          oXlsReport.Cell("**PONo", 0, i).Value = dtRow["PONo"].ToString();
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
          oXlsReport.Cell("**Revision", 0, i).Value = DBConvert.ParseInt(dtRow["Revision"].ToString());
          if (DBConvert.ParseDouble(dtRow["CancellationQty"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**CancellationQty", 0, i).Value = DBConvert.ParseDouble(dtRow["CancellationQty"].ToString());
          }
          else
          {
            oXlsReport.Cell("**CancellationQty", 0, i).Value = DBNull.Value;
          }
        }
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void ExportWOLinkSOBeforeShipped()
    {
      string strTemplateName = "RPT_PLN_98_015_04";
      string strSheetName = "Sheet1";
      string strOutFileName = "WO Link SO Before Shipped";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DataTable dtData = (DataTable)ultWOLinkSOBeforeShipped.DataSource;
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B5:E5").Copy();
            oXlsReport.RowInsert(4 + i);
            oXlsReport.Cell("B5:E5", 0, i).Paste();
          }
          if (DBConvert.ParseInt(dtRow["WoInfoPID"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBConvert.ParseInt(dtRow["WoInfoPID"].ToString());
          }
          else
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBNull.Value;
          }
          oXlsReport.Cell("**CustomerPONo", 0, i).Value = dtRow["CustomerPONo"].ToString();
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
          if (DBConvert.ParseDouble(dtRow["Qty"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["Qty"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBNull.Value;
          }
        }
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void ExportShipped()
    {
      string strTemplateName = "RPT_PLN_98_015_05";
      string strSheetName = "Sheet1";
      string strOutFileName = "Shipped";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DataTable dtData = (DataTable)ultShippedQty.DataSource;
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B5:E5").Copy();
            oXlsReport.RowInsert(4 + i);
            oXlsReport.Cell("B5:E5", 0, i).Paste();
          }
          oXlsReport.Cell("**CustomerPONo", 0, i).Value = dtRow["CustomerPONo"].ToString();
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
          oXlsReport.Cell("**Revision", 0, i).Value = DBConvert.ParseInt(dtRow["Revision"].ToString());
          if (DBConvert.ParseDouble(dtRow["ShippedQty"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**ShippedQty", 0, i).Value = DBConvert.ParseDouble(dtRow["ShippedQty"].ToString());
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

    private void ExportWOLinkSOAfterShipped()
    {
      string strTemplateName = "RPT_PLN_98_015_06";
      string strSheetName = "Sheet1";
      string strOutFileName = "WO Link SO After Shipped";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DataTable dtData = (DataTable)ultWOLinkSOAfterShipped.DataSource;
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B5:H5").Copy();
            oXlsReport.RowInsert(4 + i);
            oXlsReport.Cell("B5:H5", 0, i).Paste();
          }
          oXlsReport.Cell("**CustomerPONo", 0, i).Value = dtRow["CustomerPONo"].ToString();
          oXlsReport.Cell("**ScheduleDelivery", 0, i).Value = dtRow["ScheduleDelivery"].ToString();
          if (DBConvert.ParseInt(dtRow["WO"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBConvert.ParseInt(dtRow["WO"].ToString());
          }
          else
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBNull.Value;
          }
          if (DBConvert.ParseInt(dtRow["Priority"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**Priority", 0, i).Value = DBConvert.ParseInt(dtRow["Priority"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Priority", 0, i).Value = DBNull.Value;
          }
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
          oXlsReport.Cell("**Revision", 0, i).Value = DBConvert.ParseInt(dtRow["Revision"].ToString());
          if (DBConvert.ParseDouble(dtRow["Qty"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["Qty"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBNull.Value;
          }
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void ExportResult()
    {
      string strTemplateName = "RPT_PLN_98_015_07";
      string strSheetName = "Sheet1";
      string strOutFileName = "Result";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DataTable dtData = (DataTable)ultResult.DataSource;
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B5:H5").Copy();
            oXlsReport.RowInsert(4 + i);
            oXlsReport.Cell("B5:H5", 0, i).Paste();
          }
          if (DBConvert.ParseInt(dtRow["WO"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBConvert.ParseInt(dtRow["WO"].ToString());
          }
          else
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBNull.Value;
          }
          oXlsReport.Cell("**CustomerPONo", 0, i).Value = dtRow["CustomerPONo"].ToString();
          if (DBConvert.ParseInt(dtRow["Priority"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**Priority", 0, i).Value = DBConvert.ParseInt(dtRow["Priority"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Priority", 0, i).Value = DBNull.Value;
          }
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
          oXlsReport.Cell("**Revision", 0, i).Value = DBConvert.ParseInt(dtRow["Revision"].ToString());
          oXlsReport.Cell("**StandByEN", 0, i).Value = dtRow["StandByEN"].ToString();
          if (DBConvert.ParseDouble(dtRow["Qty"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["Qty"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBNull.Value;
          }
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    #endregion Function

    #region Event
    private void ultWIP_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;

      //Sum Qty And Total CBM
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultCancellation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["CancellationQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      //Sum Qty And Total CBM
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["CancellationQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultWOLinkSOBeforeShipped_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["WoInfoPID"].Header.Caption = "WO";
      e.Layout.Bands[0].Columns["WoInfoPID"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      //Sum Qty And Total CBM
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultShippedQty_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ShippedQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      //Sum Qty And Total CBM
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["ShippedQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultWOLinkSOAfterShipped_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      //Sum Qty And Total CBM
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultResult_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      //Sum Qty And Total CBM
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      btnExportExcel.Enabled = false;
      // Wip
      if (tabControl1.SelectedTab == tabControl1.TabPages["tabWIP"])
      {
        this.ExportWIP();
      }
      // Cancellation
      if (tabControl1.SelectedTab == tabControl1.TabPages["tabCancellation"])
      {
        this.ExportCancellation();
      }
      // Wo Link SO Before Shipped
      if (tabControl1.SelectedTab == tabControl1.TabPages["tabWOLinkSOBeforeShipped"])
      {
        this.ExportWOLinkSOBeforeShipped();
      }
      // Shipped
      if (tabControl1.SelectedTab == tabControl1.TabPages["tabShippedQty"])
      {
        this.ExportShipped();
      }
      // Wo Link SO After Shipped
      if (tabControl1.SelectedTab == tabControl1.TabPages["tabWOLinkSOAfterShipped"])
      {
        this.ExportWOLinkSOAfterShipped();
      }
      // Result
      if (tabControl1.SelectedTab == tabControl1.TabPages["tabResult"])
      {
        this.ExportResult();
      }
      btnExportExcel.Enabled = true;
    }

    private void btnGettemplate_Click(object sender, EventArgs e)
    {
      string templateName = "RPT_PLN_98_015_01";
      string sheetName = "Sheet1";
      string outFileName = "Template Import ItemCode";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtListItemCode.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.txtCustomerPONo.Text = string.Empty;
      this.txtItemCode.Text = string.Empty;
      this.txtListItemCode.Text = string.Empty;
    }
    #endregion Event 
  }
}
