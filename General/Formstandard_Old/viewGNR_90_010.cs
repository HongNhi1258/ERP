/*
  Author      : 
  Date        : 19/8/2013
  Description : Condition When Make VB Report
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared;
using Infragistics.Win;
using System.Diagnostics;
using VBReport;
using System.IO;
using System.Data.SqlClient;

namespace DaiCo.General
{
  public partial class viewGNR_90_010 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init

    public viewGNR_90_010()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load ViewVEN_05_001
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_90_010_Load(object sender, EventArgs e)
    {
      // Load All Data Report
      this.LoadData();
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      // Init Data

      this.LoadComboType();
    }

    ///// <summary>
    ///// Load UltraCombo
    ///// </summary>
    //private void LoadCombo()
    //{
    //  string commandText = string.Empty;
    //  commandText += " ....";
    //  DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
    //  if (dtSource != null)
    //  {
    //    ultControl.DataSource = dtSource;
    //    ultControl.DisplayMember = "...";
    //    ultControl.ValueMember = "...";
    //    // Format Grid
    //    ultControl.DisplayLayout.Bands[0].ColHeadersVisible = false;
    //    ultControl.DisplayLayout.Bands[0].Columns["..."].Width = 250;
    //    ultControl.DisplayLayout.Bands[0].Columns["..."].Hidden = true;
    //  }
    //}

    /// <summary>
    /// Load UltraCombo Type (Stock Balance Report / Stock Balance Detail Report/ Monthly Stock Balance Report/ Receiving Detail Report/ Issuing Detail Report 
    /// Stock Movement Report/ Stock Movement Detail Report/ Ageing )
    /// </summary>
    private void LoadComboType()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 ID, 'Stock Balance Report' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Stock Balance Detail Report' Name";
      commandText += " UNION ";
      commandText += " SELECT 3 ID, 'Monthly Stock Balance Report' Name";
      commandText += " UNION ";
      commandText += " SELECT 4 ID, 'Receiving Detail Report' Name";
      commandText += " UNION ";
      commandText += " SELECT 5 ID, 'Issuing Detail Report' Name";
      commandText += " UNION ";
      commandText += " SELECT 6 ID, 'Stock Movement Report' Name";
      commandText += " UNION ";
      commandText += " SELECT 7 ID, 'Stock Movement Detail Report' Name";
      //commandText += " UNION ";
      //commandText += " SELECT 12 ID, 'Stock Movement Detail With Allocate Data Report' Name";
      commandText += " UNION ";
      commandText += " SELECT 8 ID, 'Aging' Name";
      commandText += " UNION ";
      commandText += " SELECT 9 ID, 'Stock Balance Base Choose Time' Name";
      commandText += " UNION ";
      commandText += " SELECT 10 ID, 'Consumption Veneer' Name";
      commandText += " UNION ";
      commandText += " SELECT 11 ID, 'Aging Monthly' Name";
      commandText += " UNION ";
      commandText += " SELECT 13 ID, 'Aging Non-Allocate' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultType.DataSource = dtSource;
      ultType.DisplayMember = "Name";
      ultType.ValueMember = "ID";
      ultType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultType.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      ultType.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
    }
    #endregion Init

    #region Event

    /// <summary>
    /// Export
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExport_Click(object sender, EventArgs e)
    {
      int value = int.MinValue;
      if (this.ultType.Value != null)
      {
        value = DBConvert.ParseInt(this.ultType.Value.ToString());

        //if (value == 1)
        //{
        //  this.ExportStockBalance();
        //}
        //else if (value == 2)
        //{
        //  this.ExportStockBalanceDetail();
        //}
      }
    }
    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click_1(object sender, EventArgs e)
    {
      this.CloseTab();
    }
   
    #endregion Event

    #region Function

    /// <summary>
    /// Export VB Report
    /// </summary>
    private void ExportVBReport()
    {
      //string strTemplateName = "RPT_VEN_03_001_02";
      //string strSheetName = "Sheet1";
      //string strOutFileName = "Stock Balance"; 
      //string strStartupPath = System.Windows.Forms.Application.StartupPath;
      //string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      //string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      //XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      ////Search
      //DBParameter[] arrInput = new DBParameter[2];

      //long locationPid = long.MinValue;
      //string materialCode = string.Empty;

      //// Location
      //if (this.ultLocation.Value != null)
      //{
      //  locationPid = DBConvert.ParseLong(this.ultLocation.Value.ToString());
      //  if (locationPid != long.MinValue)
      //  {
      //    arrInput[0] = new DBParameter("@LocationPid", DbType.Int64, locationPid);
      //  }
      //}
      //// Material Code
      //if (this.ultMaterialCode.Value != null)
      //{
      //  materialCode = this.ultMaterialCode.Value.ToString();
      //  if (materialCode.Length > 0)
      //  {
      //    arrInput[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
      //  }
      //}
      //DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTStockBalanceVeneer_Select", arrInput);

      //oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

      //if (dtData != null && dtData.Rows.Count > 0)
      //{
      //  Double totalCBM = 0;
      //  for (int i = 0; i < dtData.Rows.Count; i++)
      //  {
      //    DataRow dtRow = dtData.Rows[i];
      //    if (i > 0)
      //    {
      //      oXlsReport.Cell("B8:H8").Copy();
      //      oXlsReport.RowInsert(7 + i);
      //      oXlsReport.Cell("B8:H8", 0, i).Paste();
      //    }
      //    oXlsReport.Cell("**No", 0, i).Value = i + 1;
      //    oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
      //    oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
      //    oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameVN"].ToString();
      //    oXlsReport.Cell("**Unit", 0, i).Value = dtRow["TenDonViEN"].ToString();
      //    oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["EOH(Balance)"].ToString());
      //    oXlsReport.Cell("**Location", 0, i).Value = dtRow["Location"].ToString();

      //    if (dtRow["EOH(Balance)"] != null && dtRow["EOH(Balance)"].ToString().Trim().Length > 0)
      //    {
      //      totalCBM = totalCBM + DBConvert.ParseDouble(dtRow["EOH(Balance)"].ToString());
      //    }
      //  }
      //  oXlsReport.Cell("**TotalQtyM2").Value = totalCBM;
      //}
      //oXlsReport.Out.File(strOutFileName);
      //Process.Start(strOutFileName);
    }

    /// <summary>
    /// Export VB Report Excel 2007
    /// </summary>
    private void ExportVBReportExcel2007()
    {
      //string strTemplateName = "RPT_PLN_98_014_06";
      //string strOutFileName = "Overview Status Report";
      //string strStartupPath = System.Windows.Forms.Application.StartupPath;
      //string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      //string strPathTemplate = strStartupPath + @"\ExcelTemplate";

      //Microsoft.Office.Interop.Excel.ApplicationClass xlApp;
      //Microsoft.Office.Interop.Excel.Worksheet xlSheetOverviewStatus;
      //Microsoft.Office.Interop.Excel.Worksheet xlSheetFGW;
      //Microsoft.Office.Interop.Excel.Worksheet xlSheetPAC;
      //Microsoft.Office.Interop.Excel.Worksheet xlSheetWAX;
      //Microsoft.Office.Interop.Excel.Worksheet xlSheetANT;
      //Microsoft.Office.Interop.Excel.Worksheet xlSheetFIN;
      //Microsoft.Office.Interop.Excel.Worksheet xlSheetSAN;
      //Microsoft.Office.Interop.Excel.Worksheet xlSheetITW;
      //Microsoft.Office.Interop.Excel.Worksheet xlSheetASS;
      //Microsoft.Office.Interop.Excel.Worksheet xlSheetCST;
      //Microsoft.Office.Interop.Excel.Worksheet xlSheetSUB;
      //Microsoft.Office.Interop.Excel.Worksheet xlSheetMCH;
      //Microsoft.Office.Interop.Excel.Worksheet xlSheetFOU;
      //Microsoft.Office.Interop.Excel.Workbook xlBook;

      //object missValue = System.Reflection.Missing.Value;

      //xlApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
      //xlBook = xlApp.Workbooks.Add(missValue);

      //FunctionUtility.InitializeOutputdirectory(strPathOutputFile);
      //strTemplateName = string.Format(@"{0}\{1}.xlsx", strPathTemplate, strTemplateName);
      //strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xlsx", strPathOutputFile, strOutFileName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);

      //xlBook = (xlApp.Workbooks).Open(strTemplateName, missValue,
      //    missValue, missValue, missValue, missValue, missValue, missValue
      //   , missValue, missValue, missValue, missValue, missValue, missValue, missValue);

      //xlSheetOverviewStatus = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("OverviewStatus");
      //xlSheetFGW = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("FGW");
      //xlSheetPAC = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("PAC");
      //xlSheetWAX = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("WAX");
      //xlSheetANT = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("ANT");
      //xlSheetFIN = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("FIN");
      //xlSheetSAN = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("SAND");
      //xlSheetITW = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("ITW");
      //xlSheetASS = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("ASS");
      //xlSheetCST = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("CST");
      //xlSheetSUB = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("SUB");
      //xlSheetMCH = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("MCH");
      //xlSheetFOU = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("FOU");

      //// Overview Status
      //DateTime dateCur = DateTime.Now;
      //xlSheetOverviewStatus.Cells[1, 1] = "Status Overview for cont. in " + dateCur.ToString("MMM/yy");
      //xlSheetOverviewStatus.Cells[2, 1] = "(Data updated until " + dateCur.ToString("dd/MMM/yy") + ")";

      //DBParameter[] arrInput = new DBParameter[2];
      //arrInput[0] = new DBParameter("@Month", DbType.Int32, DBConvert.ParseInt(dateCur.Month));
      //arrInput[1] = new DBParameter("@Year", DbType.Int32, DBConvert.ParseInt(dateCur.Year));

      //DataSet dsData = DataBaseAccess.SearchStoreProcedure("spPLNOverviewStatusReport_Select", arrInput);

      //if (dsData != null)
      //{
      //  if (dsData.Tables[0].Rows.Count > 0)
      //  {
      //    xlSheetOverviewStatus.Cells[4, 2] = DBConvert.ParseInt(dsData.Tables[0].Rows[0][0].ToString());
      //  }

      //  // Info
      //  if (dsData.Tables[1].Rows.Count > 0)
      //  {
      //    xlSheetOverviewStatus.Cells[5, 2] = DBConvert.ParseDouble(dsData.Tables[1].Rows[0]["Plan"].ToString());
      //    xlSheetOverviewStatus.Cells[6, 2] = DBConvert.ParseDouble(dsData.Tables[1].Rows[0]["ActualFinished"].ToString());
      //    xlSheetOverviewStatus.Cells[7, 2] = DBConvert.ParseDouble(dsData.Tables[1].Rows[0]["Remain"].ToString());
      //  }

      //  // Detail
      //  if (dsData.Tables[2].Rows.Count > 0)
      //  {
      //    for (int rowCell = 0; rowCell <= 12; rowCell++)
      //    {
      //      for (int i = 0; i < dsData.Tables[2].Rows.Count; i++)
      //      {
      //        if (string.Compare(xlSheetOverviewStatus.get_Range(xlSheetOverviewStatus.Cells[rowCell + 11, 1], xlSheetOverviewStatus.Cells[rowCell + 11, 1]).Value2.ToString(), dsData.Tables[2].Rows[i]["Location"].ToString(), true) == 0)
      //        {
      //          for (int j = 2; j < dsData.Tables[2].Columns.Count; j++)
      //          {
      //            if (DBConvert.ParseDouble(dsData.Tables[2].Rows[i][j].ToString()) != double.MinValue)
      //            {
      //              xlSheetOverviewStatus.Cells[rowCell + 11, j] = DBConvert.ParseDouble(dsData.Tables[2].Rows[i][j].ToString());
      //            }
      //            else
      //            {
      //              xlSheetOverviewStatus.Cells[rowCell + 11, j] = "";
      //            }
      //          }
      //        }
      //      }
      //    }
      //  }
      //  // Load OutPut CBM
      //  if (dsData.Tables[3].Rows.Count > 0)
      //  {
      //    DateTime previousDate = DBConvert.ParseDateTime(dsData.Tables[3].Rows[0]["PreviousDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      //    DateTime currDate = DBConvert.ParseDateTime(dsData.Tables[3].Rows[0]["CurrDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      //    DateTime firstDayMonth = DBConvert.ParseDateTime(dsData.Tables[3].Rows[0]["FirstDayMonth"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      //    string previous = previousDate.ToString("dd/MMM");
      //    string curr = currDate.ToString("dd/MMM");
      //    string firstDay = firstDayMonth.ToString("dd/MMM");

      //    xlSheetOverviewStatus.Cells[41, 5] = DBConvert.ParseInt(dsData.Tables[3].Rows[0]["TwoMonth"].ToString());
      //    xlSheetOverviewStatus.Cells[42, 1] = "Output of Assembly from " + previous + " To " + curr;

      //    xlSheetOverviewStatus.Cells[44, 5] = DBConvert.ParseInt(dsData.Tables[3].Rows[0]["InMonth"].ToString());
      //    xlSheetOverviewStatus.Cells[45, 1] = "Output of Packing from " + firstDay + " To " + curr;

      //    xlSheetOverviewStatus.Cells[42, 6] = DBConvert.ParseDouble(dsData.Tables[3].Rows[0]["OutputASS"].ToString());
      //    xlSheetOverviewStatus.Cells[45, 6] = DBConvert.ParseDouble(dsData.Tables[3].Rows[0]["OutputPAK"].ToString());
      //  }
      //}

      //// FGW
      //System.Data.DataTable dtMain = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMasterPlanWorkAreaReport_Select");
      //if (dtMain != null && dtMain.Rows.Count > 0)
      //{
      //  xlSheetFGW.Cells[1, 1] = "ITEMS AT FGW FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
      //  xlSheetFGW.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

      //  string selectFGW = "WorkAreaPid = 42";
      //  DataRow[] foundRowFGW = dtMain.Select(selectFGW);

      //  this.WriteCell(xlSheetFGW, foundRowFGW);

      //  // PAC
      //  xlSheetPAC.Cells[1, 1] = "ITEMS AT PAC FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
      //  xlSheetPAC.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

      //  string selectPAC = "WorkAreaPid = 37";
      //  DataRow[] foundRowPAC = dtMain.Select(selectPAC);
      //  this.WriteCell(xlSheetPAC, foundRowPAC);

      //  // WAX
      //  xlSheetWAX.Cells[1, 1] = "ITEMS AT WAX FOR CONTAINER IN  " + dateCur.ToString("MMM/yy");
      //  xlSheetWAX.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

      //  string selectWAX = "WorkAreaPid = 36";
      //  DataRow[] foundRowWAX = dtMain.Select(selectWAX);
      //  this.WriteCell(xlSheetWAX, foundRowWAX);

      //  // ANT
      //  xlSheetANT.Cells[1, 1] = "ITEMS AT ANT FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
      //  xlSheetANT.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

      //  string selectANT = "WorkAreaPid = 38";
      //  DataRow[] foundRowANT = dtMain.Select(selectANT);
      //  this.WriteCell(xlSheetANT, foundRowANT);

      //  // FIN
      //  xlSheetFIN.Cells[1, 1] = "ITEMS AT FIN FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
      //  xlSheetFIN.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

      //  string selectFIN = "WorkAreaPid = 41";
      //  DataRow[] foundRowFIN = dtMain.Select(selectFIN);
      //  this.WriteCell(xlSheetFIN, foundRowFIN);

      //  // SAN
      //  xlSheetSAN.Cells[1, 1] = "ITEMS AT SAND FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
      //  xlSheetSAN.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

      //  string selectSAN = "WorkAreaPid = 34";
      //  DataRow[] foundRowSAN = dtMain.Select(selectSAN);
      //  this.WriteCell(xlSheetSAN, foundRowSAN);

      //  // ITW
      //  xlSheetITW.Cells[1, 1] = "ITEMS AT ITW FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
      //  xlSheetITW.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

      //  string selectITW = "WorkAreaPid = 33";
      //  DataRow[] foundRowITW = dtMain.Select(selectITW);
      //  this.WriteCell(xlSheetITW, foundRowITW);

      //  // ASS
      //  xlSheetASS.Cells[1, 1] = "ITEMS AT ASS FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
      //  xlSheetASS.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

      //  string selectASS = "WorkAreaPid = 32";
      //  DataRow[] foundRowASS = dtMain.Select(selectASS);
      //  this.WriteCell(xlSheetASS, foundRowASS);

      //  // CST
      //  xlSheetCST.Cells[1, 1] = "ITEMS AT CST FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
      //  xlSheetCST.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

      //  string selectCST = "WorkAreaPid = 31";
      //  DataRow[] foundRowCST = dtMain.Select(selectCST);
      //  this.WriteCell(xlSheetCST, foundRowCST);

      //  // SUB
      //  xlSheetSUB.Cells[1, 1] = "ITEMS AT SUB FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
      //  xlSheetSUB.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

      //  string selectSUB = "WorkAreaPid = 39";
      //  DataRow[] foundRowSUB = dtMain.Select(selectSUB);
      //  this.WriteCell(xlSheetSUB, foundRowSUB);

      //  // MCH
      //  xlSheetMCH.Cells[1, 1] = "ITEMS AT MCH FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
      //  xlSheetMCH.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

      //  string selectMCH = "WorkAreaPid = 43";
      //  DataRow[] foundRowMCH = dtMain.Select(selectMCH);
      //  this.WriteCell(xlSheetMCH, foundRowMCH);

      //  // FOU
      //  //xlSheetFOU.Cells[1, 1] = "ITEMS AT FOU FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
      //  //xlSheetFOU.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

      //  //string selectFOU = "WorkAreaPid = 43";
      //  //DataRow[] foundRowFOU = dtMain.Select(selectFOU);
      //  //this.WriteCell(xlSheetFOU, foundRowFOU);
      //}

      //xlBook.SaveAs(strOutFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook,
      //  missValue, missValue, missValue, missValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive,
      //        missValue, missValue, missValue, missValue, missValue);

      //xlBook.Close(true, missValue, missValue);
      //xlApp.Quit();
      //Process.Start(strOutFileName);
    }

    private void WriteCell(Microsoft.Office.Interop.Excel.Worksheet xlSheet, DataRow[] foundRow)
    {
      if (foundRow.Length > 0)
      {
        int k = int.MinValue;
        for (int i = 0; i < foundRow.Length; i++)
        {
          k = 0;
          xlSheet.Cells[i + 5, k + 1] = foundRow[i]["ItemCode"].ToString();
          xlSheet.get_Range(string.Format("A{0}", i + 5), string.Format("A{0}", i + 5)).BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
          xlSheet.Cells[i + 5, k + 2] = foundRow[i]["Revision"].ToString();
          xlSheet.get_Range(string.Format("B{0}", i + 5), string.Format("B{0}", i + 5)).BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
          xlSheet.Cells[i + 5, k + 3] = foundRow[i]["SaleCode"].ToString();
          xlSheet.get_Range(string.Format("C{0}", i + 5), string.Format("C{0}", i + 5)).BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
          xlSheet.Cells[i + 5, k + 4] = foundRow[i]["CarcassCode"].ToString();
          xlSheet.get_Range(string.Format("D{0}", i + 5), string.Format("D{0}", i + 5)).BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
          xlSheet.Cells[i + 5, k + 5] = foundRow[i]["OldCode"].ToString();
          xlSheet.get_Range(string.Format("E{0}", i + 5), string.Format("E{0}", i + 5)).BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
          xlSheet.Cells[i + 5, k + 6] = foundRow[i]["WO"].ToString();
          xlSheet.get_Range(string.Format("F{0}", i + 5), string.Format("F{0}", i + 5)).BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
          xlSheet.Cells[i + 5, k + 7] = foundRow[i]["UCBM"].ToString();
          xlSheet.get_Range(string.Format("G{0}", i + 5), string.Format("G{0}", i + 5)).BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
          xlSheet.Cells[i + 5, k + 8] = foundRow[i]["ContainerNo"].ToString();
          xlSheet.get_Range(string.Format("H{0}", i + 5), string.Format("H{0}", i + 5)).BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
          xlSheet.Cells[i + 5, k + 9] = foundRow[i]["ShipDateStr"].ToString();
          xlSheet.get_Range(string.Format("I{0}", i + 5), string.Format("I{0}", i + 5)).BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
          xlSheet.Cells[i + 5, k + 10] = foundRow[i]["Qty"].ToString();
          xlSheet.get_Range(string.Format("J{0}", i + 5), string.Format("J{0}", i + 5)).BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
          xlSheet.Cells[i + 5, k + 11] = foundRow[i]["LoadingCBM"].ToString();
          xlSheet.get_Range(string.Format("K{0}", i + 5), string.Format("K{0}", i + 5)).BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
          xlSheet.Cells[i + 5, k + 12] = foundRow[i]["StatusWIP"].ToString();
          xlSheet.get_Range(string.Format("L{0}", i + 5), string.Format("L{0}", i + 5)).BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
          xlSheet.Cells[i + 5, k + 13] = foundRow[i]["QtyWip"].ToString();
          xlSheet.get_Range(string.Format("M{0}", i + 5), string.Format("M{0}", i + 5)).BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
          xlSheet.Cells[i + 5, k + 14] = foundRow[i]["TCBM"].ToString();
          xlSheet.get_Range(string.Format("N{0}", i + 5), string.Format("N{0}", i + 5)).BorderAround(Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
        }
      }
    }
    #endregion Function
  }
}
