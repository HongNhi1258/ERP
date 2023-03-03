/*
 * Author       : 
 * CreateDate   : 16/01/2013
 * Description  : Reports Master Plan
 */
using CrystalDecisions.CrystalReports.Engine;
using DaiCo.Application;
using DaiCo.ERPProject.Planning.DataSetFile;
using DaiCo.ERPProject.Planning.Reports;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_98_014 : MainUserControl
  {
    #region Field
    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;
    private string reportName = string.Empty;
    #endregion Field

    #region Init

    public viewPLN_98_014()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load viewPLN_98_014
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_98_014_Load(object sender, EventArgs e)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      this.pathTemplate = startupPath + @"\ExcelTemplate";
      this.pathExport = startupPath + @"\Report";
      this.LoadReport();
    }
    #endregion Init

    #region Function

    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadItemCode()
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT ItemCode ";
      commandText += " FROM TblBOMItemBasic ";
      commandText += " ORDER BY ItemCode ";

      System.Data.DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucItemCode.DataSource = dtItem;
      ucItemCode.ColumnWidths = "200";
      ucItemCode.DataBind();
      ucItemCode.ValueMember = "ItemCode";
    }

    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadContainer()
    {
      string commandText = string.Empty;
      commandText = "  SELECT ContainerNo, ContainerNo + ' - ' + CONVERT(varchar, ShipDate, 103) Container";
      commandText += " FROM TblPLNSHPContainer";
      commandText += " ORDER BY ShipDate DESC";

      System.Data.DataTable dtContaier = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucContainer.DataSource = dtContaier;
      ucContainer.ColumnWidths = "0";
      ucContainer.DataBind();
      ucContainer.ValueMember = "ContainerNo";
      ucContainer.DisplayMember = "Container";

    }

    /// <summary>
    /// Load Carcass Base On WO
    /// </summary>
    private void LoadContainerBaseOnShipDate()
    {
      string commandText = string.Empty;
      commandText = "  SELECT ContainerNo, ContainerNo + ' - (' + CONVERT(varchar, ShipDate, 103) + ')' Container";
      commandText += " FROM TblPLNSHPContainer";
      commandText += " WHERE 1 = 1 ";

      if (ultDTShipDateFrom.Value != null)
      {
        DateTime dateFrom = DBConvert.ParseDateTime(ultDTShipDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        string stringDateFrom = dateFrom.ToString("dd/MMM/yyyy");
        commandText += " AND ShipDate >= '" + stringDateFrom + "'";
      }

      if (ultDTShipDateTo.Value != null)
      {
        DateTime dateTo = DBConvert.ParseDateTime(ultDTShipDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        string stringDateTo = dateTo.ToString("dd/MMM/yyyy");
        commandText += " AND ShipDate <= '" + stringDateTo + "'";
      }
      commandText += " ORDER BY ShipDate DESC ";

      System.Data.DataTable dtContaier = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucContainer.DataSource = dtContaier;
      ucContainer.ColumnWidths = "0";
      ucContainer.DataBind();
      ucContainer.ValueMember = "ContainerNo";
      ucContainer.DisplayMember = "Container";
    }

    /// <summary>
    /// Load Type Report
    /// </summary>
    private void LoadReport()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 ID, 'OverView Container Plan' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Shipped History Report' Name";
      commandText += " UNION";
      //commandText += " SELECT 3 ID, 'Loading List Report' Name";
      //commandText += " UNION";
      //commandText += " SELECT 4 ID, 'OverView Status Report' Name";
      //commandText += " UNION";
      commandText += " SELECT 5 ID, 'Container Status Report' Name";
      commandText += " UNION";
      commandText += " SELECT 6 ID, 'Item Shipped Monthly Report' Name";
      commandText += " UNION";
      commandText += " SELECT 7 ID, 'Loading List' Name";
      commandText += " UNION";
      commandText += " SELECT 8 ID, 'Container Loading Plan' Name";
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
      if (value == 1 || value == 3 || value == 6)
      {
        // Check Ship Date From
        if (ultDTShipDateFrom.Value == null)
        {
          btnExport.Enabled = true;
          message = "ShipDate From";
          return false;
        }
        // Check Ship Date From
        if (ultDTShipDateTo.Value == null)
        {
          btnExport.Enabled = true;
          message = "ShipDate To";
          return false;
        }
      }
      else if (value == 6) // Item Shipped Monthly
      {
        // Check CurrentDate
        if (ultDTCurrentDate.Value == null)
        {
          btnExport.Enabled = true;
          message = "CurrentDate";
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Loading List Report
    /// </summary>
    private void LoadingListReport()
    {
      string strTemplateName = "RPT_PLN_98_014_03";
      string strSheetName = "Sheet1";
      string strOutFileName = "Loading List Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      // Printed Date
      oXlsReport.Cell("**PrintedDate").Value = DBConvert.ParseDateTime(DateTime.Now.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      // Printed Name Report
      DateTime fromDate = (DateTime)ultDTShipDateFrom.Value;
      DateTime toDate = (DateTime)ultDTShipDateFrom.Value;
      string fromShipDate = fromDate.ToString("MMM / yy");
      string toShipDate = toDate.ToString("MMM / yy");
      oXlsReport.Cell("**NameReport").Value = fromShipDate + " To " + toShipDate + " LOADING LIST REPORT";

      //Search
      DBParameter[] arrInput = new DBParameter[2];

      DateTime shipDateFrom = DateTime.MinValue;
      DateTime shipDateTo = DateTime.MinValue;
      // Ship Date From
      if (ultDTShipDateFrom.Value != null)
      {
        shipDateFrom = DBConvert.ParseDateTime(ultDTShipDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        arrInput[0] = new DBParameter("@ShipDateFrom", DbType.DateTime, shipDateFrom);
      }

      // Ship Date To
      if (ultDTShipDateTo.Value != null)
      {
        shipDateTo = DBConvert.ParseDateTime(ultDTShipDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        arrInput[1] = new DBParameter("@ShipDateTo", DbType.DateTime, shipDateTo);
      }
      System.Data.DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMasterPlanLoadingListPlanReport_Select", 300, arrInput);

      if (dtData != null && dtData.Rows.Count > 0)
      {
        int indexColRevisionMth = int.MinValue;
        int indexColItemCodeMth = int.MinValue;
        int indexColQty = int.MinValue;
        int indexColShipDate = int.MinValue;
        // Gan Name Column
        for (int i = 0; i < dtData.Columns.Count; i++)
        {
          DataColumn dtCol = dtData.Columns[i];

          if (string.Compare(dtCol.Caption, "ItemCodeMth", true) == 0)
          {
            indexColItemCodeMth = i + 1;
          }
          else if (string.Compare(dtCol.Caption, "RevisionMth", true) == 0)
          {
            indexColRevisionMth = i + 1;
          }
          else if (string.Compare(dtCol.Caption, "Qty", true) == 0)
          {
            indexColQty = i + 1;
          }
          else if (string.Compare(dtCol.Caption, "ShipDate", true) == 0)
          {
            indexColShipDate = i + 1;
          }

          if (i > 0)
          {
            oXlsReport.Cell("B5:B6").Copy();
            oXlsReport.ColumnInsert(2 + i);
            oXlsReport.Cell("B5:B6", i, 0).Paste();
          }
          oXlsReport.Cell("**ColName", i, 0).Value = dtCol.Caption;
        }

        int colNumberStart = 1;
        int colNumberFinish = colNumberStart + dtData.Columns.Count;
        string colNameFinish = this.ExcelColumnFromNumber(colNumberFinish);

        // Gan Row
        for (int k = 6; k < dtData.Rows.Count + 6; k++)
        {
          DataRow dtRow = dtData.Rows[k - 6];
          if (k - 6 > 0)
          {
            oXlsReport.Cell(string.Format("A6:{0}6", colNameFinish)).Copy();
            oXlsReport.RowInsert(5 + (k - 6));
            oXlsReport.Cell(string.Format("A6:{0}6", colNameFinish), 0, k - 6).Paste();
          }
          oXlsReport.Cell("**No", 0, k - 6).Value = k - 6;

          for (int icol = 1; icol < colNumberFinish; icol++)
          {
            string colName = this.ExcelColumnFromNumber(icol);

            if (indexColRevisionMth < icol && icol < indexColQty)
            {
              oXlsReport.Cell(string.Format("{0}{1}:{0}{1}", colName, k)).Attr.BackColor = xlColor.xcCyan;
            }
            if (dtRow[icol - 1].ToString().Length > 0)
            {
              oXlsReport.Cell(string.Format("{0}{1}:{0}{1}", colName, k)).Value = dtRow[icol - 1];
            }
            else
            {
              oXlsReport.Cell(string.Format("{0}{1}:{0}{1}", colName, k)).Value = "";
            }
          }
        }
        oXlsReport.ColumnDelete(indexColShipDate);
        oXlsReport.ColumnDelete(indexColRevisionMth - 1);
        oXlsReport.ColumnDelete(indexColItemCodeMth - 1);
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Return Index Column
    /// </summary>
    /// <param name="column"></param>
    /// <returns></returns>
    private string ExcelColumnFromNumber(int column)
    {
      int intFirstLetter = (column / 676) + 64;
      int intSecondLetter = ((column % 676) / 26) + 64;
      int intThirdLetter = (column % 26) + 65;

      char FirstLetter = (intFirstLetter > 64) ? (char)intFirstLetter : ' ';
      char SecondLetter = (intSecondLetter > 64) ? (char)intSecondLetter : ' ';
      char ThirdLetter = (char)intThirdLetter;

      return string.Concat(FirstLetter, SecondLetter, ThirdLetter).Trim();
    }

    /// <summary>
    /// Shipped History Report
    /// </summary>
    private void ShippedHistoryReport()
    {
      string strTemplateName = "RPT_PLN_98_014_02";
      string strSheetName = "Sheet1";
      string strOutFileName = "Overview Container Plan";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      // Printed Date
      oXlsReport.Cell("**PrintedDate").Value = DBConvert.ParseDateTime(DateTime.Now.ToString(), ConstantClass.FORMAT_DATETIME);

      DBParameter[] input = new DBParameter[1];
      string text = string.Empty;
      text = this.txtItemCode.Text;
      System.Data.DataTable dtSource = new System.Data.DataTable();
      if (txtItemImport.Text.Length > 0)
      {
        try
        {
          // Import Excel
          try
          {
            dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtItemImport.Text.Trim(), "SELECT * FROM [Sheet1 (1)$C5:C99]").Tables[0];
          }
          catch
          {
            dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtItemImport.Text.Trim(), "SELECT * FROM [Sheet1$C5:C99]").Tables[0];
          }

          if (dtSource != null)
          {
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
              if (dtSource.Rows[i][0].ToString().Trim().Length > 0)
              {
                text = text + ";" + dtSource.Rows[i][0].ToString();
              }
            }
          }
        }
        catch
        {
          WindowUtinity.ShowMessageError("ERR0105");
          return;
        }
      }
      // ItemCode
      if (text.Length > 0)
      {
        input[0] = new DBParameter("@ItemCode", DbType.String, text);
      }
      System.Data.DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMasterPlanShippedHistoryReport_Select", 300, input);

      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("A6:U6").Copy();
            oXlsReport.RowInsert(5 + i);
            oXlsReport.Cell("A6:U6", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = DBConvert.ParseInt(dtRow["No"].ToString());
          oXlsReport.Cell("**CarcassCode", 0, i).Value = dtRow["CarcassCode"].ToString();
          oXlsReport.Cell("**SaleCode", 0, i).Value = dtRow["SaleCode"].ToString();
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
          oXlsReport.Cell("**OldCode", 0, i).Value = dtRow["OldCode"].ToString();
          oXlsReport.Cell("**6M", 0, i).Value = DBConvert.ParseInt(dtRow["JCUS6M"].ToString());
          oXlsReport.Cell("**12M", 0, i).Value = DBConvert.ParseInt(dtRow["JCUS12M"].ToString());
          oXlsReport.Cell("**24M", 0, i).Value = DBConvert.ParseInt(dtRow["JCUS24M"].ToString());
          oXlsReport.Cell("**Total", 0, i).Value = DBConvert.ParseInt(dtRow["JCUSTotal"].ToString());
          oXlsReport.Cell("**6M1", 0, i).Value = DBConvert.ParseInt(dtRow["JCDIR6M"].ToString());
          oXlsReport.Cell("**12M1", 0, i).Value = DBConvert.ParseInt(dtRow["JCDIR12M"].ToString());
          oXlsReport.Cell("**24M1", 0, i).Value = DBConvert.ParseInt(dtRow["JCDIR24M"].ToString());
          oXlsReport.Cell("**Total1", 0, i).Value = DBConvert.ParseInt(dtRow["JCDIRTotal"].ToString());
          oXlsReport.Cell("**6M2", 0, i).Value = DBConvert.ParseInt(dtRow["OTH6M"].ToString());
          oXlsReport.Cell("**12M2", 0, i).Value = DBConvert.ParseInt(dtRow["OTH12M"].ToString());
          oXlsReport.Cell("**24M2", 0, i).Value = DBConvert.ParseInt(dtRow["OTH24M"].ToString());
          oXlsReport.Cell("**Total2", 0, i).Value = DBConvert.ParseInt(dtRow["OTHTotal"].ToString());
          oXlsReport.Cell("**6M3", 0, i).Value = DBConvert.ParseInt(dtRow["Grand6M"].ToString());
          oXlsReport.Cell("**12M3", 0, i).Value = DBConvert.ParseInt(dtRow["Grand12M"].ToString());
          oXlsReport.Cell("**24M3", 0, i).Value = DBConvert.ParseInt(dtRow["Grand24M"].ToString());
          oXlsReport.Cell("**Total3", 0, i).Value = DBConvert.ParseInt(dtRow["GrandTotal"].ToString());
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Supplier Purchase Order
    /// </summary>
    private void OverviewContainerPlan()
    {
      string strTemplateName = "RPT_PLN_98_014_01";
      string strSheetName = "Sheet1";
      string strOutFileName = "Overview Container Plan";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      // Printed Date
      oXlsReport.Cell("**PrintedDate").Value = DBConvert.ParseDateTime(DateTime.Now.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);

      //Search
      DBParameter[] arrInput = new DBParameter[2];

      DateTime shipDateFrom = DateTime.MinValue;
      DateTime shipDateTo = DateTime.MinValue;

      // Ship Date From
      if (ultDTShipDateFrom.Value != null)
      {
        shipDateFrom = DBConvert.ParseDateTime(ultDTShipDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        arrInput[0] = new DBParameter("@ShipDateFrom", DbType.DateTime, shipDateFrom);
      }

      // Ship Date To
      if (ultDTShipDateTo.Value != null)
      {
        shipDateTo = DBConvert.ParseDateTime(ultDTShipDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        arrInput[1] = new DBParameter("@ShipDateTo", DbType.DateTime, shipDateTo);
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNRPTMasterplanOverviewReport_Select", 3000, arrInput);
      System.Data.DataTable dtData = dsSource.Tables[0];
      System.Data.DataTable dtData1 = dsSource.Tables[1];
      System.Data.DataTable dtData2 = dsSource.Tables[2];

      // Row Number
      int row = dtData.Rows.Count;
      int row1 = dtData1.Rows.Count;

      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("A6:T6").Copy();
            oXlsReport.RowInsert(5 + i);
            oXlsReport.Cell("A6:T6", 0, i).Paste();
          }

          if (dtRow["Week"].ToString().Length > 0 && dtRow["Year"].ToString().Length > 0)
          {
            oXlsReport.Cell("**No", 0, i).Value = DBConvert.ParseInt(dtRow["Priority"].ToString());
            oXlsReport.Cell("**Revision", 0, i).Value = dtRow["RevCon"].ToString();
            oXlsReport.Cell("**Container", 0, i).Value = dtRow["ContainerNo"].ToString();
            oXlsReport.Cell("**ContType", 0, i).Value = dtRow["Value"].ToString();

            if (dtRow["LoadingDate"].ToString().Trim().Length > 0)
            {
              oXlsReport.Cell("**LoadingDate", 0, i).Value = DBConvert.ParseDateTime(dtRow["LoadingDate"].ToString(), ConstantClass.FORMAT_DATETIME);
            }
            else
            {
              oXlsReport.Cell("**LoadingDate", 0, i).Value = "";
            }

            if (DBConvert.ParseInt(dtRow["Qty"].ToString()) != int.MinValue)
            {
              oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseInt(dtRow["Qty"].ToString());
            }
            else
            {
              oXlsReport.Cell("**Qty", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["CBM"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**PlannedLoadingCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["CBM"].ToString());
            }
            else
            {
              oXlsReport.Cell("**PlannedLoadingCBM", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["ValuePointCBM"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**VP", 0, i).Value = DBConvert.ParseDouble(dtRow["ValuePointCBM"].ToString());
            }
            else
            {
              oXlsReport.Cell("**VP", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["PackingCBM"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**NeedToPackCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["PackingCBM"].ToString());
            }
            else
            {
              oXlsReport.Cell("**NeedToPackCBM", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["PackingPercent"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**NeedToPackPercent", 0, i).Value = DBConvert.ParseDouble(dtRow["PackingPercent"].ToString());
            }
            else
            {
              oXlsReport.Cell("**NeedToPackPercent", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["Shipped"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**Shipped", 0, i).Value = DBConvert.ParseDouble(dtRow["Shipped"].ToString());
            }
            else
            {
              oXlsReport.Cell("**Shipped", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["FEU"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**FEU", 0, i).Value = DBConvert.ParseDouble(dtRow["FEU"].ToString());
            }
            else
            {
              oXlsReport.Cell("**FEU", 0, i).Value = "";
            }

            oXlsReport.Cell("**Distributor", 0, i).Value = dtRow["Distributor"].ToString();

            oXlsReport.Cell("**Remark", 0, i).Value = dtRow["Remark"].ToString();

            if (DBConvert.ParseDouble(dtRow["ITW"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**ITWCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["ITW"].ToString());
            }
            else
            {
              oXlsReport.Cell("**ITWCBM", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["ASS"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**ASS", 0, i).Value = DBConvert.ParseDouble(dtRow["ASS"].ToString());
            }
            else
            {
              oXlsReport.Cell("**ASS", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["CST"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**CSTCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["CST"].ToString());
            }
            else
            {
              oXlsReport.Cell("**CSTCBM", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["MCH"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**MCHCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["MCH"].ToString());
            }
            else
            {
              oXlsReport.Cell("**MCHCBM", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["SUB"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**SUBCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["SUB"].ToString());
            }
            else
            {
              oXlsReport.Cell("**SUBCBM", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["FOU"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**FOUCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["FOU"].ToString());
            }
            else
            {
              oXlsReport.Cell("**FOUCBM", 0, i).Value = "";
            }
          }
          else if (dtRow["MonthYear"].ToString().Length > 0 && string.Compare(dtRow["MonthYear"].ToString(), "ALL") == -1)
          {
            string week = dtRow["MonthYear"].ToString().Split('/')[0];

            if (string.Compare(week, "1", true) == 0)
            {
              oXlsReport.Cell("**No", 0, i).Value = week + "st week";
            }
            else if (string.Compare(week, "2", true) == 0)
            {
              oXlsReport.Cell("**No", 0, i).Value = week + "nd week";
            }
            else if (string.Compare(week, "3", true) == 0)
            {
              oXlsReport.Cell("**No", 0, i).Value = week + "rd week";
            }
            else
            {
              oXlsReport.Cell("**No", 0, i).Value = week + "th week";
            }

            oXlsReport.Cell("**Revision", 0, i).Value = "";
            oXlsReport.Cell("**Container", 0, i).Value = "Sub-Total";
            oXlsReport.Cell("**ContType", 0, i).Value = "";
            oXlsReport.Cell("**LoadingDate", 0, i).Value = "";

            if (DBConvert.ParseInt(dtRow["Qty"].ToString()) != int.MinValue)
            {
              oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseInt(dtRow["Qty"].ToString());
            }
            else
            {
              oXlsReport.Cell("**Qty", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["CBM"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**PlannedLoadingCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["CBM"].ToString());
            }
            else
            {
              oXlsReport.Cell("**PlannedLoadingCBM", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["ValuePointCBM"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**VP", 0, i).Value = DBConvert.ParseDouble(dtRow["ValuePointCBM"].ToString());
            }
            else
            {
              oXlsReport.Cell("**VP", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["PackingCBM"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**NeedToPackCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["PackingCBM"].ToString());
            }
            else
            {
              oXlsReport.Cell("**NeedToPackCBM", 0, i).Value = "";
            }

            oXlsReport.Cell("**NeedToPackPercent", 0, i).Value = Math.Round((DBConvert.ParseDouble(dtRow["PackingCBM"].ToString()) / DBConvert.ParseDouble(dtRow["CBM"].ToString())) * 100, 2) + "%";

            if (DBConvert.ParseDouble(dtRow["Shipped"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**Shipped", 0, i).Value = DBConvert.ParseDouble(dtRow["Shipped"].ToString());
            }
            else
            {
              oXlsReport.Cell("**Shipped", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["FEU"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**FEU", 0, i).Value = DBConvert.ParseDouble(dtRow["FEU"].ToString());
            }
            else
            {
              oXlsReport.Cell("**FEU", 0, i).Value = "";
            }

            oXlsReport.Cell("**Remark", 0, i).Value = "";
            oXlsReport.Cell("**Distributor", 0, i).Value = "";

            if (DBConvert.ParseDouble(dtRow["ITW"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**ITWCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["ITW"].ToString());
            }
            else
            {
              oXlsReport.Cell("**ITWCBM", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["ASS"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**ASS", 0, i).Value = DBConvert.ParseDouble(dtRow["ASS"].ToString());
            }
            else
            {
              oXlsReport.Cell("**ASS", 0, i).Value = "";
            }


            if (DBConvert.ParseDouble(dtRow["CST"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**CSTCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["CST"].ToString());
            }
            else
            {
              oXlsReport.Cell("**CSTCBM", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["MCH"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**MCHCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["MCH"].ToString());
            }
            else
            {
              oXlsReport.Cell("**MCHCBM", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["SUB"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**SUBCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["SUB"].ToString());
            }
            else
            {
              oXlsReport.Cell("**SUBCBM", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["FOU"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**FOUCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["FOU"].ToString());
            }
            else
            {
              oXlsReport.Cell("**FOUCBM", 0, i).Value = "";
            }

            oXlsReport.Cell(string.Format(@"A{0}:Q{1}", i + 6, i + 6)).Attr.BackColor = xlColor.xcGreen;
            oXlsReport.Cell(string.Format(@"A{0}:Q{1}", i + 6, i + 6)).Attr.FontStyle = xlFontStyle.xsItalic;
          }
          else
          {
            oXlsReport.Cell("**No", 0, i).Value = "Total";
            oXlsReport.Cell("**Revision", 0, i).Value = "";
            oXlsReport.Cell("**Container", 0, i).Value = "Sub-Total";
            oXlsReport.Cell("**ContType", 0, i).Value = "";
            oXlsReport.Cell("**LoadingDate", 0, i).Value = "";

            if (DBConvert.ParseInt(dtRow["Qty"].ToString()) != int.MinValue)
            {
              oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseInt(dtRow["Qty"].ToString());
            }
            else
            {
              oXlsReport.Cell("**Qty", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["ValuePointCBM"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**VP", 0, i).Value = DBConvert.ParseDouble(dtRow["ValuePointCBM"].ToString());
            }
            else
            {
              oXlsReport.Cell("**VP", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["CBM"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**VP", 0, i).Value = DBConvert.ParseDouble(dtRow["CBM"].ToString());
            }
            else
            {
              oXlsReport.Cell("**VP", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["PackingCBM"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**NeedToPackCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["PackingCBM"].ToString());
            }
            else
            {
              oXlsReport.Cell("**NeedToPackCBM", 0, i).Value = "";
            }

            oXlsReport.Cell("**NeedToPackPercent", 0, i).Value = Math.Round((DBConvert.ParseDouble(dtRow["PackingCBM"].ToString()) / DBConvert.ParseDouble(dtRow["CBM"].ToString())) * 100, 2) + "%";

            if (DBConvert.ParseDouble(dtRow["Shipped"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**Shipped", 0, i).Value = DBConvert.ParseDouble(dtRow["Shipped"].ToString());
            }
            else
            {
              oXlsReport.Cell("**Shipped", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["FEU"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**FEU", 0, i).Value = DBConvert.ParseDouble(dtRow["FEU"].ToString());
            }
            else
            {
              oXlsReport.Cell("**FEU", 0, i).Value = "";
            }

            oXlsReport.Cell("**Distributor", 0, i).Value = "";
            oXlsReport.Cell("**Remark", 0, i).Value = "";

            if (DBConvert.ParseDouble(dtRow["ITW"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**ITWCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["ITW"].ToString());
            }
            else
            {
              oXlsReport.Cell("**ITWCBM", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["ASS"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**ASS", 0, i).Value = DBConvert.ParseDouble(dtRow["ASS"].ToString());
            }
            else
            {
              oXlsReport.Cell("**ASS", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["CST"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**CSTCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["CST"].ToString());
            }
            else
            {
              oXlsReport.Cell("**CSTCBM", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["MCH"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**MCHCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["MCH"].ToString());
            }
            else
            {
              oXlsReport.Cell("**MCHCBM", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["SUB"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**SUBCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["SUB"].ToString());
            }
            else
            {
              oXlsReport.Cell("**SUBCBM", 0, i).Value = "";
            }

            if (DBConvert.ParseDouble(dtRow["FOU"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**FOUCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["FOU"].ToString());
            }
            else
            {
              oXlsReport.Cell("**FOUCBM", 0, i).Value = "";
            }
          }
        }
      }

      if (dtData1 != null && dtData1.Rows.Count > 0)
      {
        int k = 6 + row + 3;
        for (int i = 0; i < dtData1.Rows.Count; i++)
        {
          DataRow dtRow = dtData1.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell(string.Format(@"A{0}:C{1}", k, k)).Copy();
            oXlsReport.RowInsert(k - 1 + i);
            oXlsReport.Cell(string.Format(@"A{0}:C{1}", k, k), 0, i).Paste();
          }

          oXlsReport.Cell(string.Format(@"A{0}:A{1}", k + i, k + i)).Value = dtRow["TimeNo"].ToString();
          oXlsReport.Cell(string.Format(@"B{0}:B{1}", k + i, k + i)).Value = DBConvert.ParseInt(dtRow["WorkingDays"].ToString());
          oXlsReport.Cell(string.Format(@"C{0}:C{1}", k + i, k + i)).Value = DBConvert.ParseInt(dtRow["RemainWorkingDays"].ToString());
        }
      }

      if (dtData2 != null && dtData2.Rows.Count > 0)
      {
        int k = 6 + row + 3 + row1 + 3;
        for (int i = 0; i < dtData2.Rows.Count; i++)
        {
          DataRow dtRow = dtData2.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell(string.Format(@"A{0}:J{1}", k, k)).Copy();
            oXlsReport.RowInsert(k - 1 + i);
            oXlsReport.Cell(string.Format(@"A{0}:J{1}", k, k), 0, i).Paste();
          }
          if (dtRow["YearNo"].ToString().Length == 0)
          {
            oXlsReport.Cell(string.Format(@"A{0}:A{1}", k + i, k + i)).Value = dtRow["TimeNo"].ToString();
            oXlsReport.Cell(string.Format(@"B{0}:B{1}", k + i, k + i)).Value = DBNull.Value;
            oXlsReport.Cell(string.Format(@"C{0}:C{1}", k + i, k + i)).Value = DBConvert.ParseDouble(dtRow["DistributorTarget"].ToString());
            oXlsReport.Cell(string.Format(@"D{0}:D{1}", k + i, k + i)).Value = DBConvert.ParseDouble(dtRow["CountContainer"].ToString());
            oXlsReport.Cell(string.Format(@"E{0}:E{1}", k + i, k + i)).Value = DBConvert.ParseDouble(dtRow["ActualShipped"].ToString());
            oXlsReport.Cell(string.Format(@"F{0}:F{1}", k + i, k + i)).Value = DBConvert.ParseDouble(dtRow["Remained"].ToString());
            oXlsReport.Cell(string.Format(@"G{0}:G{1}", k + i, k + i)).Value = DBConvert.ParseDouble(dtRow["CBM"].ToString());
            oXlsReport.Cell(string.Format(@"H{0}:H{1}", k + i, k + i)).Value = DBConvert.ParseDouble(dtRow["ValuePointCBM"].ToString());
            oXlsReport.Cell(string.Format(@"I{0}:I{1}", k + i, k + i)).Value = DBConvert.ParseDouble(dtRow["Diff"].ToString());

            oXlsReport.Cell(string.Format(@"A{0}:A{1}", k + i, k + i)).Attr.FontColor = xlColor.xcRed;
            oXlsReport.Cell(string.Format(@"A{0}:A{1}", k + i, k + i)).Attr.FontStyle = xlFontStyle.xsBold;
            //oXlsReport.Cell(string.Format(@"A{0}:A{1}", k + i, k + i)).Attr.FontStyle = xlFontStyle.xsItalic;

            oXlsReport.Cell(string.Format(@"B{0}:B{1}", k + i, k + i)).Attr.FontColor = xlColor.xcRed;
            oXlsReport.Cell(string.Format(@"B{0}:B{1}", k + i, k + i)).Attr.FontStyle = xlFontStyle.xsBold;
            //oXlsReport.Cell(string.Format(@"B{0}:B{1}", k + i, k + i)).Attr.FontStyle = xlFontStyle.xsItalic;

            oXlsReport.Cell(string.Format(@"C{0}:C{1}", k + i, k + i)).Attr.FontColor = xlColor.xcRed;
            oXlsReport.Cell(string.Format(@"C{0}:C{1}", k + i, k + i)).Attr.FontStyle = xlFontStyle.xsBold;
            //oXlsReport.Cell(string.Format(@"C{0}:C{1}", k + i, k + i)).Attr.FontStyle = xlFontStyle.xsItalic;

            oXlsReport.Cell(string.Format(@"D{0}:D{1}", k + i, k + i)).Attr.FontColor = xlColor.xcRed;
            oXlsReport.Cell(string.Format(@"D{0}:D{1}", k + i, k + i)).Attr.FontStyle = xlFontStyle.xsBold;
            //oXlsReport.Cell(string.Format(@"D{0}:D{1}", k + i, k + i)).Attr.FontStyle = xlFontStyle.xsItalic;

            oXlsReport.Cell(string.Format(@"E{0}:E{1}", k + i, k + i)).Attr.FontColor = xlColor.xcRed;
            oXlsReport.Cell(string.Format(@"E{0}:E{1}", k + i, k + i)).Attr.FontStyle = xlFontStyle.xsBold;
            //oXlsReport.Cell(string.Format(@"E{0}:E{1}", k + i, k + i)).Attr.FontStyle = xlFontStyle.xsItalic;

            oXlsReport.Cell(string.Format(@"F{0}:F{1}", k + i, k + i)).Attr.FontColor = xlColor.xcRed;
            oXlsReport.Cell(string.Format(@"F{0}:F{1}", k + i, k + i)).Attr.FontStyle = xlFontStyle.xsBold;
            //oXlsReport.Cell(string.Format(@"E{0}:E{1}", k + i, k + i)).Attr.FontStyle = xlFontStyle.xsItalic;

            oXlsReport.Cell(string.Format(@"G{0}:G{1}", k + i, k + i)).Attr.FontColor = xlColor.xcRed;
            oXlsReport.Cell(string.Format(@"G{0}:G{1}", k + i, k + i)).Attr.FontStyle = xlFontStyle.xsBold;

            oXlsReport.Cell(string.Format(@"H{0}:H{1}", k + i, k + i)).Attr.FontColor = xlColor.xcRed;
            oXlsReport.Cell(string.Format(@"H{0}:H{1}", k + i, k + i)).Attr.FontStyle = xlFontStyle.xsBold;

            oXlsReport.Cell(string.Format(@"I{0}:I{1}", k + i, k + i)).Attr.FontColor = xlColor.xcRed;
            oXlsReport.Cell(string.Format(@"I{0}:I{1}", k + i, k + i)).Attr.FontStyle = xlFontStyle.xsBold;
          }
          else
          {
            oXlsReport.Cell(string.Format(@"A{0}:A{1}", k + i, k + i)).Value = dtRow["TimeNo"].ToString();
            oXlsReport.Cell(string.Format(@"B{0}:B{1}", k + i, k + i)).Value = dtRow["Distributor"].ToString();
            oXlsReport.Cell(string.Format(@"C{0}:C{1}", k + i, k + i)).Value = DBConvert.ParseDouble(dtRow["DistributorTarget"].ToString());
            oXlsReport.Cell(string.Format(@"D{0}:D{1}", k + i, k + i)).Value = DBConvert.ParseDouble(dtRow["CountContainer"].ToString());
            oXlsReport.Cell(string.Format(@"E{0}:E{1}", k + i, k + i)).Value = DBConvert.ParseDouble(dtRow["ActualShipped"].ToString());
            oXlsReport.Cell(string.Format(@"F{0}:F{1}", k + i, k + i)).Value = DBConvert.ParseDouble(dtRow["Remained"].ToString());
            oXlsReport.Cell(string.Format(@"G{0}:G{1}", k + i, k + i)).Value = DBConvert.ParseDouble(dtRow["CBM"].ToString());
            oXlsReport.Cell(string.Format(@"H{0}:H{1}", k + i, k + i)).Value = DBConvert.ParseDouble(dtRow["ValuePointCBM"].ToString());
            oXlsReport.Cell(string.Format(@"I{0}:I{1}", k + i, k + i)).Value = DBConvert.ParseDouble(dtRow["Diff"].ToString());
          }
        }
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void OveriviewStatusReportFinal()
    {
      string strTemplateName = "RPT_PLN_98_014_06";
      string strOutFileName = "Overview Status Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";

      Microsoft.Office.Interop.Excel.Application xlApp;
      Microsoft.Office.Interop.Excel.Worksheet xlSheetOverviewStatus;
      Microsoft.Office.Interop.Excel.Worksheet xlSheetFGW;
      Microsoft.Office.Interop.Excel.Worksheet xlSheetPAC;
      Microsoft.Office.Interop.Excel.Worksheet xlSheetWAX;
      Microsoft.Office.Interop.Excel.Worksheet xlSheetANT;
      Microsoft.Office.Interop.Excel.Worksheet xlSheetFIN;
      Microsoft.Office.Interop.Excel.Worksheet xlSheetSAN;
      Microsoft.Office.Interop.Excel.Worksheet xlSheetITW;
      Microsoft.Office.Interop.Excel.Worksheet xlSheetASS;
      Microsoft.Office.Interop.Excel.Worksheet xlSheetCST;
      Microsoft.Office.Interop.Excel.Worksheet xlSheetSUB;
      Microsoft.Office.Interop.Excel.Worksheet xlSheetMCH;
      Microsoft.Office.Interop.Excel.Worksheet xlSheetFOU;
      Microsoft.Office.Interop.Excel.Workbook xlBook;

      object missValue = System.Reflection.Missing.Value;

      xlApp = new Microsoft.Office.Interop.Excel.Application();
      xlBook = xlApp.Workbooks.Add(missValue);

      FunctionUtility.InitializeOutputdirectory(strPathOutputFile);
      strTemplateName = string.Format(@"{0}\{1}.xlsx", strPathTemplate, strTemplateName);
      strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xlsx", strPathOutputFile, strOutFileName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);

      xlBook = (xlApp.Workbooks).Open(strTemplateName, missValue,
          missValue, missValue, missValue, missValue, missValue, missValue
         , missValue, missValue, missValue, missValue, missValue, missValue, missValue);

      xlSheetOverviewStatus = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("OverviewStatus");
      xlSheetFGW = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("FGW");
      xlSheetPAC = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("PAC");
      xlSheetWAX = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("WAX");
      xlSheetANT = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("ANT");
      xlSheetFIN = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("FIN");
      xlSheetSAN = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("SAND");
      xlSheetITW = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("ITW");
      xlSheetASS = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("ASS");
      xlSheetCST = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("CST");
      xlSheetSUB = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("SUB");
      xlSheetMCH = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("MCH");
      xlSheetFOU = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item("FOU");

      // Overview Status
      DateTime dateCur = DateTime.Now;
      xlSheetOverviewStatus.Cells[1, 1] = "Status Overview for cont. in " + dateCur.ToString("MMM/yy");
      xlSheetOverviewStatus.Cells[2, 1] = "(Data updated until " + dateCur.ToString("dd/MMM/yy") + ")";

      DBParameter[] arrInput = new DBParameter[2];
      arrInput[0] = new DBParameter("@Month", DbType.Int32, DBConvert.ParseInt(dateCur.Month));
      arrInput[1] = new DBParameter("@Year", DbType.Int32, DBConvert.ParseInt(dateCur.Year));

      DataSet dsData = DataBaseAccess.SearchStoreProcedure("spPLNOverviewStatusReport_Select", 300, arrInput);

      if (dsData != null)
      {
        if (dsData.Tables[0].Rows.Count > 0)
        {
          xlSheetOverviewStatus.Cells[4, 2] = DBConvert.ParseInt(dsData.Tables[0].Rows[0][0].ToString());
        }

        // Info
        if (dsData.Tables[1].Rows.Count > 0)
        {
          xlSheetOverviewStatus.Cells[5, 2] = DBConvert.ParseDouble(dsData.Tables[1].Rows[0]["Plan"].ToString());
          xlSheetOverviewStatus.Cells[6, 2] = DBConvert.ParseDouble(dsData.Tables[1].Rows[0]["ActualFinished"].ToString());
          xlSheetOverviewStatus.Cells[7, 2] = DBConvert.ParseDouble(dsData.Tables[1].Rows[0]["Remain"].ToString());
        }

        // Detail
        if (dsData.Tables[2].Rows.Count > 0)
        {
          for (int rowCell = 0; rowCell <= 12; rowCell++)
          {
            for (int i = 0; i < dsData.Tables[2].Rows.Count; i++)
            {
              if (string.Compare(xlSheetOverviewStatus.get_Range(xlSheetOverviewStatus.Cells[rowCell + 11, 1], xlSheetOverviewStatus.Cells[rowCell + 11, 1]).Value2.ToString(), dsData.Tables[2].Rows[i]["Location"].ToString(), true) == 0)
              {
                for (int j = 2; j < dsData.Tables[2].Columns.Count; j++)
                {
                  if (DBConvert.ParseDouble(dsData.Tables[2].Rows[i][j].ToString()) != double.MinValue)
                  {
                    xlSheetOverviewStatus.Cells[rowCell + 11, j] = DBConvert.ParseDouble(dsData.Tables[2].Rows[i][j].ToString());
                  }
                  else
                  {
                    xlSheetOverviewStatus.Cells[rowCell + 11, j] = "";
                  }
                }
              }
            }
          }
        }
        // Load OutPut CBM
        if (dsData.Tables[3].Rows.Count > 0)
        {
          DateTime previousDate = DBConvert.ParseDateTime(dsData.Tables[3].Rows[0]["PreviousDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          DateTime currDate = DBConvert.ParseDateTime(dsData.Tables[3].Rows[0]["CurrDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          DateTime firstDayMonth = DBConvert.ParseDateTime(dsData.Tables[3].Rows[0]["FirstDayMonth"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          string previous = previousDate.ToString("dd/MMM");
          string curr = currDate.ToString("dd/MMM");
          string firstDay = firstDayMonth.ToString("dd/MMM");

          xlSheetOverviewStatus.Cells[41, 5] = DBConvert.ParseInt(dsData.Tables[3].Rows[0]["TwoMonth"].ToString());
          xlSheetOverviewStatus.Cells[42, 1] = "Output of Assembly from " + previous + " To " + curr;

          xlSheetOverviewStatus.Cells[44, 5] = DBConvert.ParseInt(dsData.Tables[3].Rows[0]["InMonth"].ToString());
          xlSheetOverviewStatus.Cells[45, 1] = "Output of Packing from " + firstDay + " To " + curr;

          xlSheetOverviewStatus.Cells[42, 6] = DBConvert.ParseDouble(dsData.Tables[3].Rows[0]["OutputASS"].ToString());
          xlSheetOverviewStatus.Cells[45, 6] = DBConvert.ParseDouble(dsData.Tables[3].Rows[0]["OutputPAK"].ToString());
        }
      }

      // FGW
      System.Data.DataTable dtMain = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMasterPlanWorkAreaReport_Select");
      if (dtMain != null && dtMain.Rows.Count > 0)
      {
        xlSheetFGW.Cells[1, 1] = "ITEMS AT FGW FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
        xlSheetFGW.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

        string selectFGW = "WorkAreaPid = 42";
        DataRow[] foundRowFGW = dtMain.Select(selectFGW);

        this.WriteCell(xlSheetFGW, foundRowFGW);

        // PAC
        xlSheetPAC.Cells[1, 1] = "ITEMS AT PAC FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
        xlSheetPAC.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

        string selectPAC = "WorkAreaPid = 37";
        DataRow[] foundRowPAC = dtMain.Select(selectPAC);
        this.WriteCell(xlSheetPAC, foundRowPAC);

        // WAX
        xlSheetWAX.Cells[1, 1] = "ITEMS AT WAX FOR CONTAINER IN  " + dateCur.ToString("MMM/yy");
        xlSheetWAX.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

        string selectWAX = "WorkAreaPid = 36";
        DataRow[] foundRowWAX = dtMain.Select(selectWAX);
        this.WriteCell(xlSheetWAX, foundRowWAX);

        // ANT
        xlSheetANT.Cells[1, 1] = "ITEMS AT ANT FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
        xlSheetANT.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

        string selectANT = "WorkAreaPid = 38";
        DataRow[] foundRowANT = dtMain.Select(selectANT);
        this.WriteCell(xlSheetANT, foundRowANT);

        // FIN
        xlSheetFIN.Cells[1, 1] = "ITEMS AT FIN FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
        xlSheetFIN.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

        string selectFIN = "WorkAreaPid = 41";
        DataRow[] foundRowFIN = dtMain.Select(selectFIN);
        this.WriteCell(xlSheetFIN, foundRowFIN);

        // SAN
        xlSheetSAN.Cells[1, 1] = "ITEMS AT SAND FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
        xlSheetSAN.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

        string selectSAN = "WorkAreaPid = 34";
        DataRow[] foundRowSAN = dtMain.Select(selectSAN);
        this.WriteCell(xlSheetSAN, foundRowSAN);

        // ITW
        xlSheetITW.Cells[1, 1] = "ITEMS AT ITW FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
        xlSheetITW.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

        string selectITW = "WorkAreaPid = 33";
        DataRow[] foundRowITW = dtMain.Select(selectITW);
        this.WriteCell(xlSheetITW, foundRowITW);

        // ASS
        xlSheetASS.Cells[1, 1] = "ITEMS AT ASS FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
        xlSheetASS.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

        string selectASS = "WorkAreaPid = 32";
        DataRow[] foundRowASS = dtMain.Select(selectASS);
        this.WriteCell(xlSheetASS, foundRowASS);

        // CST
        xlSheetCST.Cells[1, 1] = "ITEMS AT CST FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
        xlSheetCST.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

        string selectCST = "WorkAreaPid = 31";
        DataRow[] foundRowCST = dtMain.Select(selectCST);
        this.WriteCell(xlSheetCST, foundRowCST);

        // SUB
        xlSheetSUB.Cells[1, 1] = "ITEMS AT SUB FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
        xlSheetSUB.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

        string selectSUB = "WorkAreaPid = 39";
        DataRow[] foundRowSUB = dtMain.Select(selectSUB);
        this.WriteCell(xlSheetSUB, foundRowSUB);

        // MCH
        xlSheetMCH.Cells[1, 1] = "ITEMS AT MCH FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
        xlSheetMCH.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

        string selectMCH = "WorkAreaPid = 43";
        DataRow[] foundRowMCH = dtMain.Select(selectMCH);
        this.WriteCell(xlSheetMCH, foundRowMCH);

        // FOU
        //xlSheetFOU.Cells[1, 1] = "ITEMS AT FOU FOR CONTAINER IN " + dateCur.ToString("MMM/yy");
        //xlSheetFOU.Cells[2, 2] = dateCur.ToString("dd/MMM/yy");

        //string selectFOU = "WorkAreaPid = 43";
        //DataRow[] foundRowFOU = dtMain.Select(selectFOU);
        //this.WriteCell(xlSheetFOU, foundRowFOU);
      }

      xlBook.SaveAs(strOutFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook,
        missValue, missValue, missValue, missValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive,
              missValue, missValue, missValue, missValue, missValue);

      xlBook.Close(true, missValue, missValue);
      xlApp.Quit();
      Process.Start(strOutFileName);
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
          xlSheet.get_Range(string.Format("N{0}", i + 5), string.Format("N{0}", i + 5)).BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
        }
      }
    }

    /// <summary>
    /// Container Status Report
    /// </summary>
    private void ContainerStatusReport()
    {
      DBParameter[] inputParam = new DBParameter[6];

      if (ultDTShipDateFrom.Value != null)
      {
        DateTime shipDateFrom = DBConvert.ParseDateTime(ultDTShipDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        inputParam[0] = new DBParameter("@ShipDateFrom", DbType.DateTime, shipDateFrom);
      }

      if (ultDTShipDateTo.Value != null)
      {
        DateTime shipDateTo = DBConvert.ParseDateTime(ultDTShipDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        shipDateTo = shipDateTo.AddDays(1);
        inputParam[1] = new DBParameter("@ShipDateTo", DbType.DateTime, shipDateTo);
      }

      if (txtContainer.Text.Length > 0)
      {
        inputParam[2] = new DBParameter("@ContainerPid", DbType.String, txtContainer.Text);
      }

      inputParam[3] = new DBParameter("@UserPid", DbType.Int32, SharedObject.UserInfo.UserPid);

      if (txtNeedPackFrom.Text.Length > 0 && DBConvert.ParseInt(txtNeedPackFrom.Text) > 0)
      {
        inputParam[4] = new DBParameter("@NeedPakFrom", DbType.Int32, DBConvert.ParseInt(txtNeedPackFrom.Text));
      }

      if (txtNeedPackTo.Text.Length > 0 && DBConvert.ParseInt(txtNeedPackTo.Text) > 0)
      {
        inputParam[5] = new DBParameter("@NeedPakTo", DbType.Int32, DBConvert.ParseInt(txtNeedPackTo.Text));
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNMasterPlanContainerWipStatus_Report", 300, inputParam);
      if (ds.Tables[0].Rows.Count > 0)
      {
        dsPLNContainerStatus dsSource = new dsPLNContainerStatus();

        dsSource.Tables["dtInfo"].Merge(ds.Tables[0]);
        dsSource.Tables["dtDetail"].Merge(ds.Tables[1]);
        dsSource.Tables["dtWIPStatus"].Merge(ds.Tables[2]);
        dsSource.Tables["dtDeadLine"].Merge(ds.Tables[3]);

        foreach (DataRow row in dsSource.Tables["dtDetail"].Rows)
        {
          try
          {
            string imgPath = FunctionUtility.BOMGetItemImage(row["ItemCode"].ToString(), DBConvert.ParseInt(row["Revision"].ToString()));
            row["Picture"] = FunctionUtility.ImageToByteArrayWithFormat(imgPath, 380, 1.77, "JPG");
          }
          catch { }

          try
          {
            int status = DBConvert.ParseInt(row["Status"].ToString());
            int customer = DBConvert.ParseInt(row["CustomerPid"].ToString());
            row["PicItemKind"] = FunctionUtility.GetLocalItemKindIcon(status, customer); // 12 = JCUSA
          }
          catch { }
        }

        ReportClass cpt = null;
        DaiCo.Shared.View_Report report = null;
        cpt = new cptPLNContainerStatus();
        cpt.SetDataSource(dsSource);
        ControlUtility.ViewCrystalReport(cpt);
      }
    }

    /// <summary>
    /// Item Shipped Monthly Report
    /// </summary>
    private void ItemShippedMonthlyReport()
    {
      string strTemplateName = "RPT_PLN_98_014_07";
      string strSheetName = "Sheet1";
      string strOutFileName = "Item Shipped Monthly";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DateTime shippedDateFrom = DBConvert.ParseDateTime(ultDTShipDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      DateTime shippedDateTo = DBConvert.ParseDateTime(ultDTShipDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      DateTime currentDate = DBConvert.ParseDateTime(ultDTCurrentDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      DateTime startDate = DBConvert.ParseDateTime("01-Jan-" + currentDate.Year.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);

      oXlsReport.Cell("B7:B7").Value = "ShipDate From: " + shippedDateFrom.ToString("dd/MMM/yyyy") + "   To   " + shippedDateTo.ToString("dd/MMM/yyyy");
      oXlsReport.Cell("B8:B8").Value = "Reported Date: " + currentDate.ToString("dd/MMM/yyyy");
      oXlsReport.Cell("B7:B7").Attr.FontColor = xlColor.xcRed;
      oXlsReport.Cell("B8:B8").Attr.FontColor = xlColor.xcRed;

      oXlsReport.Cell("F7:F7").Value = "Note: ";
      oXlsReport.Cell("F7:F7").Attr.FontColor = xlColor.xcRed;
      oXlsReport.Cell("F7:F7").Attr.FontStyle = xlFontStyle.xsBold;

      oXlsReport.Cell("G7:G7").Value = "Total: (Sum Total Qty From: " + shippedDateFrom.ToString("dd/MMM/yyyy") + "   To   " + shippedDateTo.ToString("dd/MMM/yyyy") + ")";
      oXlsReport.Cell("G7:G7").Attr.FontColor = xlColor.xcRed;
      oXlsReport.Cell("G7:G7").Attr.FontStyle = xlFontStyle.xsBold;

      oXlsReport.Cell("G8:G8").Value = "QtyYTD: (Sum Total Qty From: " + startDate.ToString("dd/MMM/yyyy") + "   To   " + currentDate.ToString("dd/MMM/yyyy") + ")";
      oXlsReport.Cell("G8:G8").Attr.FontColor = xlColor.xcRed;
      oXlsReport.Cell("G8:G8").Attr.FontStyle = xlFontStyle.xsBold;

      DBParameter[] input = new DBParameter[3];
      input[0] = new DBParameter("@ShipDateFrom", DbType.DateTime, shippedDateFrom);
      input[1] = new DBParameter("@ShipDateTo", DbType.DateTime, shippedDateTo);
      input[2] = new DBParameter("@CurrentDate", DbType.DateTime, currentDate);

      System.Data.DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNRPTItemShippedMonthly_Select", 300, input);
      if (dtSource != null)
      {
        // Column Copy (B10:B10)
        // Field Column
        oXlsReport.Cell("B10:B10").Copy();
        for (int colName = 0; colName < dtSource.Columns.Count; colName++)
        {
          string nameColumn = ExcelColumnFromNumber(colName + 1);
          oXlsReport.Cell(string.Format("{0}10:{1}10", nameColumn, nameColumn)).Paste();
          oXlsReport.Cell(string.Format("{0}10:{1}10", nameColumn, nameColumn)).Value = dtSource.Columns[colName].ToString();
        }

        // Field Row
        oXlsReport.Cell("B11:B11").Copy();
        for (int irow = 0; irow < dtSource.Rows.Count; irow++)
        {
          for (int jCol = 0; jCol < dtSource.Columns.Count; jCol++)
          {
            string nameColumn = ExcelColumnFromNumber(jCol + 1);
            oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", irow + 11, nameColumn, irow + 11, nameColumn)).Paste();
            if (dtSource.Rows[irow][jCol].ToString().Length > 0)
            {
              if (jCol > 1)
              {
                if (DBConvert.ParseInt(dtSource.Rows[irow][jCol].ToString()) > 0)
                {
                  oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", irow + 11, nameColumn, irow + 11, nameColumn)).Value = DBConvert.ParseInt(dtSource.Rows[irow][jCol].ToString());
                }
              }
              else // ItemCode & SaleCode
              {
                oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", irow + 11, nameColumn, irow + 11, nameColumn)).Value = dtSource.Rows[irow][jCol].ToString();
              }
            }
          }
        }

        //// SUM
        //for (int sumCol = 2; sumCol < dtSource.Columns.Count; sumCol++)
        //{
        //  string sumColName = ExcelColumnFromNumber(sumCol + 1);
        //  oXlsReport.Cell(string.Format(@"{0}{1}:{2}{3}", sumColName, dtSource.Rows.Count + 11, sumColName, dtSource.Rows.Count + 11)).Value = string.Format(@"= SUM({0}{1}:{2}{3})", sumColName, 11, sumColName, dtSource.Rows.Count + 10);
        //  oXlsReport.Cell(string.Format(@"{0}{1}:{2}{3}", sumColName, dtSource.Rows.Count + 11, sumColName, dtSource.Rows.Count + 11)).Attr.FontColor = xlColor.xcBlue;
        //  oXlsReport.Cell(string.Format(@"{0}{1}:{2}{3}", sumColName, dtSource.Rows.Count + 11, sumColName, dtSource.Rows.Count + 11)).Attr.FontStyle = xlFontStyle.xsBold;
        //}
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Loading List Report
    /// </summary>
    private void LoadingList()
    {
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@ShipDateFrom", DbType.DateTime, DBConvert.ParseDateTime(ultDTShipDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      input[1] = new DBParameter("@ShipDateTo", DbType.DateTime, DBConvert.ParseDateTime(ultDTShipDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      DataSet dsMain = DataBaseAccess.SearchStoreProcedure("spPLNRPTLoadingList_Select", 300, input);
      if (dsMain != null)
      {
        //ultData.DataSource = null;
        ultData.DataSource = dsMain.Tables[0];
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["FlagQty"].Value.ToString()) == 1)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
          }
        }
        ultData.DisplayLayout.Bands[0].Columns["FlagQty"].Hidden = true;
        if (ultData.Rows.Count > 0)
        {
          Utility.ExportToExcelWithDefaultPath(ultData, "LOADING LIST REPORT");
          //Excel.Workbook xlBook;

          //ControlUtility.ExportToExcelWithDefaultPath(ultData, out xlBook, "LOADING LIST REPORT", 6);

          //string filePath = xlBook.FullName;
          //Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

          //xlSheet.Cells[1, 1] = "Dai Co Co., LTD";
          //Excel.Range r = xlSheet.get_Range("A1", "A1");

          //xlSheet.Cells[2, 1] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

          //xlSheet.Cells[3, 1] = "LOADING LIST REPORT";
          //r = xlSheet.get_Range("A3", "A3");
          //r.Font.Bold = true;
          //r.Font.Size = 14;
          //r.EntireRow.RowHeight = 20;

          //xlSheet.Cells[4, 1] = "Date: ";
          //r.Font.Bold = true;
          //xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

          ////xlBook.Application.DisplayAlerts = false;

          //xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
          //Process.Start(filePath);
        }
      }
    }

    /// <summary>
    /// Container Loading Plan Report
    /// </summary>
    private void ContainerLoadingPlan()
    {
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@ShipDateFrom", DbType.DateTime, DBConvert.ParseDateTime(ultDTShipDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      input[1] = new DBParameter("@ShipDateTo", DbType.DateTime, DBConvert.ParseDateTime(ultDTShipDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      DataSet dsMain = DataBaseAccess.SearchStoreProcedure("spPLNRPTContainerLoadingPlan_Select", 300, input);
      if (dsMain != null)
      {
        ultData.DataSource = dsMain.Tables[0];
        if (ultData.Rows.Count > 0)
        {
          Utility.ExportToExcelWithDefaultPath(ultData, "CONTAINER LOADING PLAN REPORT");
        }
      }
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
      btnExport.Enabled = false;
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

        // Expenditure Department
        if (value == 1)
        {
          this.OverviewContainerPlan();
        }
        // Shipped History Report
        else if (value == 2)
        {
          this.ShippedHistoryReport();
        }
        else if (value == 3)
        {
          this.LoadingListReport();
        }
        else if (value == 4)
        {
          this.OveriviewStatusReportFinal();
        }
        else if (value == 5)
        {
          this.ContainerStatusReport();
        }
        else if (value == 6)
        {
          this.ItemShippedMonthlyReport();
        }
        else if (value == 7)
        {
          this.reportName = "LoadingList";
          this.LoadingList();
        }
        else if (value == 8)
        {
          this.reportName = "ContainerLoadingPlan";
          this.ContainerLoadingPlan();
        }
      }
      else
      {
        btnExport.Enabled = true;
        WindowUtinity.ShowMessageError("ERR0001", "Report");
        return;
      }
      btnExport.Enabled = true;
    }

    private void ultCBReport_ValueChanged(object sender, EventArgs e)
    {
      int value = int.MinValue;
      if (ultCBReport.Value != null)
      {
        // To Mau Control
        labShipDateFrom.ForeColor = System.Drawing.SystemColors.ControlText;
        labShipDateTo.ForeColor = System.Drawing.SystemColors.ControlText;
        labItemCode.ForeColor = System.Drawing.SystemColors.ControlText;
        labItemCodeImport.ForeColor = System.Drawing.SystemColors.ControlText;
        labContainer.ForeColor = System.Drawing.SystemColors.ControlText;
        labCurrentDate.ForeColor = System.Drawing.SystemColors.ControlText;
        labNeedPackFrom.ForeColor = System.Drawing.SystemColors.ControlText;
        labNeedPackTo.ForeColor = System.Drawing.SystemColors.ControlText;
        // End

        value = DBConvert.ParseInt(ultCBReport.Value.ToString());
        if (value == 1)
        {
          // To Mau Control
          labShipDateFrom.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labShipDateTo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          // End
        }
        else if (value == 2)
        {
          this.LoadItemCode();
          // To Mau Control
          labItemCode.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labItemCodeImport.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          // End
        }
        else if (value == 3)
        {
          labShipDateFrom.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labShipDateTo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
        else if (value == 5)
        {
          this.LoadContainerBaseOnShipDate();
          labShipDateFrom.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labShipDateTo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labContainer.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labNeedPackFrom.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labNeedPackTo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
        else if (value == 6)
        {
          labShipDateFrom.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labShipDateTo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labCurrentDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
        else if (value == 7)
        {
          labShipDateFrom.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labShipDateTo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
        else if (value == 8)
        {
          labShipDateFrom.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labShipDateTo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", "Report");
        return;
      }
    }

    private void ucItemCode_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtItemCode.Text = this.ucItemCode.SelectedValue;
    }

    private void ucContainer_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtContainer.Text = this.ucContainer.SelectedValue;
    }

    private void chkItemCode_CheckedChanged(object sender, EventArgs e)
    {
      tableLayoutPanel6.Visible = chkItemCode.Checked;
    }

    private void chkContainer_CheckedChanged(object sender, EventArgs e)
    {
      tableLayoutPanel11.Visible = chkContainer.Checked;
    }

    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtItemImport.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnGettemplate_Click(object sender, EventArgs e)
    {
      string templateName = "TemplateImportItemCode";
      string sheetName = "Sheet1";
      string outFileName = "TemplateImportItemCode";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void ultDTShipDateFrom_Leave(object sender, EventArgs e)
    {
      if (ultCBReport.Value != null && DBConvert.ParseInt(ultCBReport.Value.ToString()) == 5)
      {
        this.LoadContainerBaseOnShipDate();
      }
    }

    private void ultDTShipDateTo_Leave(object sender, EventArgs e)
    {
      if (ultCBReport.Value != null && DBConvert.ParseInt(ultCBReport.Value.ToString()) == 5)
      {
        this.LoadContainerBaseOnShipDate();
      }
    }

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Reset();
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].Header.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
        e.Layout.Bands[0].Columns[i].Header.Appearance.BackColor = Color.Aqua;
        e.Layout.Bands[0].Columns[i].Header.Appearance.ForeColor = Color.Black;
        e.Layout.Bands[0].Columns[i].Header.Appearance.FontData.SizeInPoints = 9;
      }
      if (string.Compare("LoadingList", this.reportName, true) == 0)
      {
        e.Layout.Bands[0].Columns["KD"].Header.Caption = "KD";
        e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "New ItemCode";
        e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "FacCode  ";
        e.Layout.Bands[0].Columns["PackingQty"].Header.Caption = "Need Pack q'ty";
        e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Priority";
        e.Layout.Bands[0].Columns["CustomerPONo"].Header.Caption = "PO#";
        e.Layout.Bands[0].Columns["CustomerCode"].Header.Caption = "Cust.";
        e.Layout.Bands[0].Columns["WO"].Header.Caption = "WO#";
        e.Layout.Bands[0].Columns["ContainerNo"].Header.Caption = "Cont no.";
        e.Layout.Bands[0].Columns["PlannedLoadingCBM"].Header.Caption = "CBM";
        e.Layout.Bands[0].Columns["ShipDate"].Header.Caption = "Loading date";

        e.Layout.Bands[0].Summaries.Add(Infragistics.Win.UltraWinGrid.SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], Infragistics.Win.UltraWinGrid.SummaryPosition.UseSummaryPositionColumn);
        e.Layout.Bands[0].Summaries.Add(Infragistics.Win.UltraWinGrid.SummaryType.Sum, e.Layout.Bands[0].Columns["PlannedLoadingCBM"], Infragistics.Win.UltraWinGrid.SummaryPosition.UseSummaryPositionColumn);
        e.Layout.Bands[0].Summaries.Add(Infragistics.Win.UltraWinGrid.SummaryType.Sum, e.Layout.Bands[0].Columns["PackingQty"], Infragistics.Win.UltraWinGrid.SummaryPosition.UseSummaryPositionColumn);
        e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.00}";
        e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,###}";
        e.Layout.Bands[0].Summaries[2].DisplayFormat = "{0:###,###}";
        e.Layout.Bands[0].Columns["PlannedLoadingCBM"].Format = "###,###.##";
        e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
        e.Layout.Bands[0].Summaries[1].Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
        e.Layout.Bands[0].Summaries[2].Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
      }
      else if (string.Compare("ContainerLoadingPlan", this.reportName, true) == 0)
      {
        DateTime dt = DateTime.Today;
        string month1 = dt.ToString("MMM-yyyy");
        string month2 = dt.AddMonths(1).ToString("MMM-yyyy");
        string month3 = dt.AddMonths(2).ToString("MMM-yyyy");
        string month4 = dt.AddMonths(3).ToString("MMM-yyyy");
        e.Layout.Bands[0].Columns["Month1"].Header.Caption = month1;
        e.Layout.Bands[0].Columns["Month2"].Header.Caption = month2;
        e.Layout.Bands[0].Columns["Month3"].Header.Caption = month3;
        e.Layout.Bands[0].Columns["Month4"].Header.Caption = month4;

        e.Layout.Bands[0].Columns["KD"].Header.Caption = "KD";
        e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "New ItemCode";
        e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "FacCode  ";
        e.Layout.Bands[0].Columns["PackingQty"].Header.Caption = "Need Pack q'ty";
        e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Priority";
        e.Layout.Bands[0].Columns["CustomerPONo"].Header.Caption = "PO#";
        e.Layout.Bands[0].Columns["PODate"].Header.Caption = "PO date";
        e.Layout.Bands[0].Columns["CustomerCode"].Header.Caption = "Cust.";
        e.Layout.Bands[0].Columns["WO"].Header.Caption = "WO#";
        e.Layout.Bands[0].Columns["ContainerNo"].Header.Caption = "Cont no.";
        e.Layout.Bands[0].Columns["PlannedLoadingCBM"].Header.Caption = "CBM";
        e.Layout.Bands[0].Columns["ShipDate"].Header.Caption = "Loading date";
        e.Layout.Bands[0].Columns["MCHDeadline"].Header.Caption = "MCH schedule";
        e.Layout.Bands[0].Columns["SUBDeadline"].Header.Caption = "Sub-con schedule";
        e.Layout.Bands[0].Columns["FOUDeadline"].Header.Caption = "Foundry schedule";
        e.Layout.Bands[0].Columns["SpecialRemark"].Header.Caption = "Special remarks";

        e.Layout.Bands[0].Columns["RemarkTransferCon"].Header.Caption = "Remarks of changes";
        e.Layout.Bands[0].Columns["TotalItem"].Header.Caption = "Total qty need";
        e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Planned QTY";
        e.Layout.Bands[0].Columns["PackingQty"].Header.Caption = "Need Pac Qty";
        e.Layout.Bands[0].Columns["QtyRepair"].Header.Caption = "Need Repair Qty";
        e.Layout.Bands[0].Columns["PlannedLoadingCBM"].Header.Caption = "Planned loading CBM";
        e.Layout.Bands[0].Columns["OldShipDate"].Header.Caption = "Old loading date";

      }

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (e.Layout.Bands[0].Columns[i].DataType == typeof(DateTime))
        {
          e.Layout.Bands[0].Columns[i].Format = "dd-MMM-yyyy";
        }
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

    #endregion Event
  }
}
