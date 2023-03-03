/*
 * Author       : 
 * CreateDate   : 09/10/2012
 * Description  : Reports Purchasing
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using VBReport;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_08_001 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init

    public viewPUR_08_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load viewPUR_08_001
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_08_001_Load(object sender, EventArgs e)
    {
      this.LoadReport();
      this.LoadSupplier();
      this.ExpenditureDepartment();
      this.LoadComboMonth(ultCBFromMonth);
      this.LoadComboYear(ultCBFromYear);
      this.LoadComboMonth(ultToMonth);
      this.LoadComboYear(ultToYear);
      this.LoadMaterialGroup();
      this.LoadSupplierMany();
      this.LoadGroupInCharge();
      this.LoadUsedInBOM();
      this.txtConditionNumber.Text = "2";
      this.radAll.Checked = true;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Used In BOM
    /// </summary>
    private void LoadUsedInBOM()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 Pid, 'Yes' Name";
      commandText += " UNION";
      commandText += " SELECT 2 Pid, 'No' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultUsedInBOM.DataSource = dtSource;
      ultUsedInBOM.DisplayMember = "Name";
      ultUsedInBOM.ValueMember = "Pid";
      ultUsedInBOM.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultUsedInBOM.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      ultUsedInBOM.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;

      // Load Default
      ultUsedInBOM.Value = 1;
    }

    /// <summary>
    /// Load Type Report
    /// </summary>
    private void LoadReport()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 ID, 'Department Report' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Supplier Report' Name";
      commandText += " UNION";
      commandText += " SELECT 3 ID, 'Purchase Amount Report' Name";
      commandText += " UNION";
      commandText += " SELECT 4 ID, 'Delivery PR Report' Name";
      commandText += " UNION";
      commandText += " SELECT 5 ID, 'Consumption Of Material Report' Name";
      commandText += " UNION";
      commandText += " SELECT 6 ID, 'KPI Delivery PR Report' Name";
      commandText += " UNION";
      commandText += " SELECT 7 ID, 'KPI Delivery PO Report' Name";
      commandText += " UNION";
      commandText += " SELECT 8 ID, 'Alternative Supplier Report' Name";
      commandText += " UNION";
      commandText += " SELECT 9 ID, 'Payment Term Report' Name";
      commandText += " UNION";
      commandText += " SELECT 10 ID, 'PR Follow Up Report' Name";
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
    /// ExpenditureDepartment
    /// </summary>
    private void ExpenditureDepartment()
    {
      this.LoadDepartment();
    }

    /// <summary>
    /// Load Department
    /// </summary>
    private void LoadDepartment()
    {
      string commandText = "SELECT Department, Department + ' - ' + DeparmentName AS Name FROM VHRDDepartment ORDER BY Department";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBDepartment.DataSource = dtSource;
      ultCBDepartment.DisplayMember = "Name";
      ultCBDepartment.ValueMember = "Department";
      ultCBDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBDepartment.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultCBDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
    }

    /// <summary>
    /// Load Group In Charge
    /// </summary>
    private void LoadGroupInCharge()
    {
      string commandText = "SELECT Pid, GroupName FROM TblPURStaffGroup";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultGroupInCharge.DataSource = dtSource;
      ultGroupInCharge.DisplayMember = "GroupName";
      ultGroupInCharge.ValueMember = "Pid";
      ultGroupInCharge.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultGroupInCharge.DisplayLayout.Bands[0].Columns["GroupName"].Width = 250;
      ultGroupInCharge.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    private void LoadSupplier()
    {
      string commandText = "SELECT Pid , EnglishName + ' - ' + SupplierCode EnglishName FROM TblPURSupplierInfo WHERE Confirm = 2 AND DeleteFlg = 0 	AND LEN(EnglishName) > 2 ORDER BY EnglishName";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBSupplier.DataSource = dtSource;
      ultCBSupplier.DisplayMember = "EnglishName";
      ultCBSupplier.ValueMember = "Pid";
      ultCBSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBSupplier.DisplayLayout.Bands[0].Columns["EnglishName"].Width = 250;
      ultCBSupplier.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }
    /// <summary>
    /// Check Valid
    /// </summary>
    private bool CheckValid(int value, out string message)
    {
      message = string.Empty;
      if (value == 1)
      {
        // Check Department
        if (ultCBDepartment.Text.Length > 0)
        {
          if (ultCBDepartment.Value == null)
          {
            btnExport.Enabled = true;
            message = "Department";
            return false;
          }
        }
      }
      else if (value == 2)
      {
        if (ultCBSupplier.Text.Length > 0)
        {
          // Check Supplier
          if (ultCBSupplier.Value == null)
          {
            btnExport.Enabled = true;
            message = "Supplier";
            return false;
          }
        }
      }
      else if (value == 5)
      {
        if (ultCBFromYear.Value == null)
        {
          btnExport.Enabled = true;
          message = "From Year";
          return false;
        }
        if (ultCBFromMonth.Value == null)
        {
          btnExport.Enabled = true;
          message = "From Month";
          return false;
        }
        if (ultToYear.Value == null)
        {
          btnExport.Enabled = true;
          message = "To Year";
          return false;
        }
        if (ultToMonth.Value == null)
        {
          btnExport.Enabled = true;
          message = "To Month";
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Supplier Purchase Order
    /// </summary>
    private void SupplierPurchaseOrder()
    {
      string strTemplateName = "RPT_PUR_08_001_02";
      string strSheetName = "Supplier";
      string strOutFileName = "Supplier Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[3];

      long supplier = long.MinValue;
      DateTime poFromDate = DateTime.MinValue;
      DateTime poToDate = DateTime.MinValue;

      // Department
      if (ultCBSupplier.Value != null)
      {
        supplier = DBConvert.ParseLong(ultCBSupplier.Value.ToString());
        arrInput[0] = new DBParameter("@Supplier", DbType.AnsiString, supplier);
      }

      // PR From Date
      if (ultDTFromDate.Value != null)
      {
        poFromDate = DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        arrInput[1] = new DBParameter("@POFromDate", DbType.DateTime, poFromDate);
      }

      // PR To Date
      if (ultDTToDate.Value != null)
      {
        poToDate = DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        arrInput[2] = new DBParameter("@POToDate", DbType.DateTime, poToDate.AddDays(1));
      }
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPURRPTSupplierPurchaseOrder", 300, arrInput);

      // Supplier
      if (ultCBSupplier.Value != null)
      {
        string commandText = string.Format("SELECT SupplierCode + ' - ' + EnglishName AS Name FROM TblPURSupplierInfo WHERE Pid =  {0}", DBConvert.ParseLong(ultCBSupplier.Value.ToString()));
        DataTable dtSupplier = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtSupplier != null && dtSupplier.Rows.Count > 0)
        {
          oXlsReport.Cell("**Supplier").Value = dtSupplier.Rows[0]["Name"].ToString();
        }
      }
      if (ultDTFromDate.Value != null && DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        oXlsReport.Cell("**PRFromDate").Value = DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }
      if (ultDTToDate.Value != null && DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        oXlsReport.Cell("**PRToDate").Value = DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }

      if (dtData != null && dtData.Rows.Count > 0)
      {
        Double totalAmount = 0;
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B10:T10").Copy();
            oXlsReport.RowInsert(9 + i);
            oXlsReport.Cell("B10:T10", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**PONo", 0, i).Value = dtRow["PONo"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
          oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameVN"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();
          if (DBConvert.ParseDouble(dtRow["UnitPrice"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**UnitPrice", 0, i).Value = DBConvert.ParseDouble(dtRow["UnitPrice"].ToString());
          }
          else
          {
            oXlsReport.Cell("**UnitPrice", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseInt(dtRow["VAT"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**VAT", 0, i).Value = DBConvert.ParseInt(dtRow["VAT"].ToString());
          }
          else
          {
            oXlsReport.Cell("**VAT", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Quantity"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["Quantity"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Receipted"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**QtyReceived", 0, i).Value = DBConvert.ParseDouble(dtRow["Receipted"].ToString());
          }
          else
          {
            oXlsReport.Cell("**QtyReceived", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Cancel"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**QtyCancel", 0, i).Value = DBConvert.ParseDouble(dtRow["Cancel"].ToString());
          }
          else
          {
            oXlsReport.Cell("**QtyCancel", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Balance"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Balance", 0, i).Value = DBConvert.ParseDouble(dtRow["Balance"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Balance", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Amount"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Amount", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Amount", 0, i).Value = DBNull.Value;
          }

          oXlsReport.Cell("**PODate", 0, i).Value = dtRow["PODate"].ToString();
          oXlsReport.Cell("**ReceiptDate", 0, i).Value = dtRow["ReceiptedDate"].ToString();
          oXlsReport.Cell("**PRNo", 0, i).Value = dtRow["PRNo"].ToString();
          oXlsReport.Cell("**RequestDepartment", 0, i).Value = dtRow["Department"].ToString();

          if (dtRow["ExpectDate"].ToString().Length > 0)
          {
            oXlsReport.Cell("**PODeliveryDate", 0, i).Value = dtRow["ExpectDate"].ToString();
          }
          else
          {
            oXlsReport.Cell("**PODeliveryDate", 0, i).Value = DBNull.Value;
          }

          if (dtRow["Amount"] != null && dtRow["Amount"].ToString().Trim().Length > 0)
          {
            totalAmount = totalAmount + DBConvert.ParseDouble(dtRow["Amount"].ToString());
          }
        }
        oXlsReport.Cell("**TotalAmount").Value = totalAmount;
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Expenditure
    /// </summary>
    private void ExpenditureForDepartmentReport()
    {
      string strTemplateName = "RPT_PUR_08_001_01";
      string strSheetName = "Expenditure";
      string strOutFileName = "Expenditure Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[3];

      string deparment = string.Empty;
      DateTime prFromDate = DateTime.MinValue;
      DateTime prToDate = DateTime.MinValue;

      // Department
      if (ultCBDepartment.Value != null)
      {
        deparment = ultCBDepartment.Value.ToString();
        arrInput[0] = new DBParameter("@Department", DbType.AnsiString, deparment);
      }

      // PR From Date
      if (ultDTFromDate.Value != null)
      {
        prFromDate = DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        arrInput[1] = new DBParameter("@PRFromDate", DbType.DateTime, prFromDate);
      }

      // PR To Date
      if (ultDTToDate.Value != null)
      {
        prToDate = DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        arrInput[2] = new DBParameter("@PRToDate", DbType.DateTime, prToDate.AddDays(1));
      }
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPURRPTExpenditureDepartment", 600, arrInput);

      if (ultCBDepartment.Value != null)
      {
        oXlsReport.Cell("**DepartmentRequest").Value = ultCBDepartment.Value.ToString();
      }
      if (ultDTFromDate.Value != null && DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        oXlsReport.Cell("**PRFromDate").Value = DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }
      if (ultDTToDate.Value != null && DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        oXlsReport.Cell("**PRToDate").Value = DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }

      if (dtData != null && dtData.Rows.Count > 0)
      {
        Double totalAmount = 0;
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B9:S9").Copy();
            oXlsReport.RowInsert(8 + i);
            oXlsReport.Cell("B9:S9", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          //Tien Add
          oXlsReport.Cell("**GroupName", 0, i).Value = dtRow["GroupName"].ToString();
          //
          oXlsReport.Cell("**Department", 0, i).Value = dtRow["Department"].ToString();
          oXlsReport.Cell("**PRNo", 0, i).Value = dtRow["PRNo"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
          oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameVN"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();
          oXlsReport.Cell("**LeadTime", 0, i).Value = dtRow["LeadTime"].ToString();
          if (DBConvert.ParseDouble(dtRow["LastedPrice"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**LastedPrice", 0, i).Value = DBConvert.ParseDouble(dtRow["LastedPrice"].ToString());
          }
          else
          {
            oXlsReport.Cell("**LastedPrice", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["UnitPrice"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**UnitPrice", 0, i).Value = DBConvert.ParseDouble(dtRow["UnitPrice"].ToString());
          }
          else
          {
            oXlsReport.Cell("**UnitPrice", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseInt(dtRow["VAT"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**VAT", 0, i).Value = DBConvert.ParseInt(dtRow["VAT"].ToString());
          }
          {
            oXlsReport.Cell("**VAT", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Qty"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["Qty"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["QtyReceived"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**QtyReceived", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyReceived"].ToString());
          }
          else
          {
            oXlsReport.Cell("**QtyReceived", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Amount"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Amount", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Amount", 0, i).Value = DBNull.Value;
          }

          oXlsReport.Cell("**RequestDate", 0, i).Value = dtRow["RequestDate"].ToString();
          oXlsReport.Cell("**RequiredDeliveryDate", 0, i).Value = dtRow["RequiredDeliveryDate"].ToString();
          oXlsReport.Cell("**ProjectCode", 0, i).Value = dtRow["ProjectCode"].ToString();
          oXlsReport.Cell("**Purpose", 0, i).Value = dtRow["Purpose"].ToString();

          if (dtRow["Amount"] != null && dtRow["Amount"].ToString().Trim().Length > 0)
          {
            totalAmount = totalAmount + DBConvert.ParseDouble(dtRow["Amount"].ToString());
          }
        }
        oXlsReport.Cell("**TotalAmount").Value = totalAmount;
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Purchase Amount
    /// </summary>
    private void PurchaseAmountSupplier()
    {
      string strTemplateName = "RPT_PUR_08_001_04";
      string strSheetName = "PurchaseAmount";
      string strOutFileName = "Purchase Amount Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[2];

      DateTime prFromDate = DateTime.MinValue;
      DateTime prToDate = DateTime.MinValue;

      // PR From Date
      if (ultDTFromDate.Value != null)
      {
        prFromDate = DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        arrInput[0] = new DBParameter("@DateFrom", DbType.DateTime, prFromDate);
      }

      // PR To Date
      if (ultDTToDate.Value != null)
      {
        prToDate = DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        arrInput[1] = new DBParameter("@DateTo", DbType.DateTime, prToDate.AddDays(1));
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPURRPTPurchaseAmountSupplier", arrInput);

      if (ultDTFromDate.Value != null && DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        oXlsReport.Cell("**PRFromDate").Value = DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }
      if (ultDTToDate.Value != null && DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        oXlsReport.Cell("**PRToDate").Value = DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }
      if (dtData != null)
      {
        DataTable dtNew = new DataTable();
        int columnCount = dtData.Columns.Count;
        string colName = string.Empty;
        for (int col = 3; col <= dtData.Columns.Count; col++)
        {
          long number = 66 + col;
          if (number > 90)
          {
            number = 65 + number - 91;
            colName = System.Convert.ToChar(number).ToString();
            colName = "A" + colName;
          }
          else
          {
            colName = System.Convert.ToChar(number).ToString();
          }
          if (col > 3)
          {
            oXlsReport.Cell("E8:E9").Copy();
            oXlsReport.Cell(string.Format("{0}8:{1}9", colName, colName)).Paste();
          }
          oXlsReport.Cell(string.Format("{0}8:{1}8", colName, colName)).Value = dtData.Columns[col - 1].ToString();
        }
        long min = 0;
        string coll = string.Empty;
        string colName1 = string.Empty;
        long number1 = 66 + columnCount + 1;
        if (number1 > 90)
        {
          min = 65 + number1 - 91;
          coll = System.Convert.ToChar(min).ToString();
          colName1 = "A" + coll;
        }
        else
        {
          colName1 = System.Convert.ToChar(number1).ToString();
        }
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell(string.Format(@"B9:{0}9", colName1)).Copy();
            oXlsReport.RowInsert(8 + i);
            oXlsReport.Cell(string.Format(@"B9:{0}9", colName1), 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          string colName2 = string.Empty;
          for (int j = 0; j < dtData.Columns.Count; j++)
          {
            long number2 = 67 + j;
            if (number2 > 90)
            {
              number2 = 65 + number2 - 91;
              colName2 = System.Convert.ToChar(number2).ToString();
              colName2 = "A" + colName2;
            }
            else
            {
              colName2 = System.Convert.ToChar(number2).ToString();
            }
            if (DBConvert.ParseDouble(dtData.Rows[i][j].ToString()) != double.MinValue && j > 1)
            {
              oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", colName2, 9 + i, colName2, 9 + i)).Value = DBConvert.ParseDouble(dtData.Rows[i][j].ToString());
            }
            else
            {
              oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", colName2, 9 + i, colName2, 9 + i)).Value = dtData.Rows[i][j].ToString();
            }
          }
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void PerformenceForPurchaser()
    {
      string strTemplateName = "RPT_PUR_08_001_05";
      string strSheetName = "Sheet1";
      string strOutFileName = "Delivery PR Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[3];
      DateTime prFromDate = DateTime.MinValue;
      DateTime prToDate = DateTime.MinValue;

      // Department
      if (ultCBDepartment.Value != null)
      {
        arrInput[0] = new DBParameter("@Department", DbType.AnsiString, ultCBDepartment.Value.ToString());
      }

      // PR From Date
      if (ultDTFromDate.Value != null)
      {
        prFromDate = DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        arrInput[1] = new DBParameter("@DateFrom", DbType.DateTime, prFromDate);
      }

      // PR To Date
      if (ultDTToDate.Value != null)
      {
        prToDate = DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        arrInput[2] = new DBParameter("@DateTo", DbType.DateTime, prToDate.AddDays(1));
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPURRPTPerformentForPurchaser", arrInput);

      if (ultDTFromDate.Value != null && DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        oXlsReport.Cell("**PRFromDate").Value = DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }
      if (ultDTToDate.Value != null && DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        oXlsReport.Cell("**PRToDate").Value = DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }

      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B10:X10").Copy();
            oXlsReport.RowInsert(9 + i);
            oXlsReport.Cell("B10:X10", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**GroupPurChaser", 0, i).Value = dtRow["GroupPurchaser"].ToString();
          oXlsReport.Cell("**Department", 0, i).Value = dtRow["Department"].ToString();
          oXlsReport.Cell("**PRNo", 0, i).Value = dtRow["PRNo"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["MaterialNameEn"].ToString();
          oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["MaterialNameVn"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();
          if (DBConvert.ParseDouble(dtRow["UnitPrice"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**UnitPrice", 0, i).Value = DBConvert.ParseDouble(dtRow["UnitPrice"].ToString());
          }
          else
          {
            oXlsReport.Cell("**UnitPrice", 0, i).Value = "";
          }

          if (DBConvert.ParseInt(dtRow["VAT"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**VAT", 0, i).Value = DBConvert.ParseInt(dtRow["VAT"].ToString());
          }
          else
          {
            oXlsReport.Cell("**VAT", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(dtRow["Quantity"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["Quantity"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Qty", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(dtRow["Amount"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Amount", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Amount", 0, i).Value = "";
          }

          oXlsReport.Cell("**RequestDate", 0, i).Value = dtRow["RequestDate"].ToString();
          oXlsReport.Cell("**RequiredDeliveryDate", 0, i).Value = dtRow["RequiredDeliveryDate"].ToString();
          oXlsReport.Cell("**POCreateDate", 0, i).Value = dtRow["CreateDatePO"].ToString();
          oXlsReport.Cell("**ExpectDatePO", 0, i).Value = dtRow["ExpectDatePO"].ToString();
          oXlsReport.Cell("**ActualReceipt", 0, i).Value = dtRow["ActualReceipt"].ToString();

          if (DBConvert.ParseInt(dtRow["Difference1-2"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**Difference1-2", 0, i).Value = DBConvert.ParseInt(dtRow["Difference1-2"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Difference1-2", 0, i).Value = "";
          }

          if (DBConvert.ParseInt(dtRow["Difference1-3"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**Difference1-3", 0, i).Value = DBConvert.ParseInt(dtRow["Difference1-3"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Difference1-3", 0, i).Value = "";
          }

          if (DBConvert.ParseInt(dtRow["Difference2-4"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**Difference2-4", 0, i).Value = DBConvert.ParseInt(dtRow["Difference2-4"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Difference2-4", 0, i).Value = "";
          }

          if (DBConvert.ParseInt(dtRow["Difference2-5"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**Difference2-5", 0, i).Value = DBConvert.ParseInt(dtRow["Difference2-5"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Difference2-5", 0, i).Value = "";
          }

          if (DBConvert.ParseInt(dtRow["Difference4-5"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**Difference4-5", 0, i).Value = DBConvert.ParseInt(dtRow["Difference4-5"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Difference4-5", 0, i).Value = "";
          }
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void KPIDeliveryPRReport()
    {
      string strTemplateName = "RPT_PUR_08_001_07";
      string strSheetName = "Sheet1";
      string strOutFileName = "KPI Delivery PR Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      if (ultDTFromDate.Value == null ||
          DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Date From");
        return;
      }

      if (ultDTToDate.Value == null ||
          DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Date To");
        return;
      }

      //Search
      DBParameter[] arrInput = new DBParameter[3];
      DateTime prFromDate = DateTime.MinValue;
      DateTime prToDate = DateTime.MinValue;

      // PR From Date
      prFromDate = DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      arrInput[0] = new DBParameter("@DateFrom", DbType.DateTime, prFromDate);

      // PR To Date
      prToDate = DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      arrInput[1] = new DBParameter("@DateTo", DbType.DateTime, prToDate.AddDays(1));

      if (this.ultGroupInCharge.Value != null
          && DBConvert.ParseInt(this.ultGroupInCharge.Value.ToString()) != int.MinValue)
      {
        arrInput[2] = new DBParameter("@GroupInCharge", DbType.Int32, DBConvert.ParseInt(this.ultGroupInCharge.Value.ToString()));
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPURRPTDeliveryPRReport_Select", 500, arrInput);

      if (ultDTFromDate.Value != null && DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        oXlsReport.Cell("**PRFromDate").Value = DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }
      if (ultDTToDate.Value != null && DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        oXlsReport.Cell("**PRToDate").Value = DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }

      if (dtData != null && dtData.Rows.Count > 0)
      {
        int localTotal = 0;
        int importTotal = 0;
        int local = 0;
        int import = 0;
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B10:Z10").Copy();
            oXlsReport.RowInsert(9 + i);
            oXlsReport.Cell("B10:Z10", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**PRNo", 0, i).Value = dtRow["PRNo"].ToString();
          if (dtRow["GroupName"].ToString().Length > 0)
          {
            oXlsReport.Cell("**GroupPurChaser", 0, i).Value = dtRow["GroupName"].ToString();
          }
          oXlsReport.Cell("**Department", 0, i).Value = dtRow["Department"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();

          if (dtRow["Source"].ToString().Length > 0)
          {
            oXlsReport.Cell("**Source", 0, i).Value = dtRow["Source"].ToString();
          }
          else
          {
            oXlsReport.Cell("**Source", 0, i).Value = "";
          }

          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();

          if (DBConvert.ParseDouble(dtRow["Qty"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["Qty"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Qty", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(dtRow["UnitPriceVND"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**UnitPrice", 0, i).Value = DBConvert.ParseDouble(dtRow["UnitPriceVND"].ToString());
          }
          else
          {
            oXlsReport.Cell("**UnitPrice", 0, i).Value = "";
          }

          if (DBConvert.ParseInt(dtRow["VAT"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**VAT", 0, i).Value = DBConvert.ParseInt(dtRow["VAT"].ToString());
          }
          else
          {
            oXlsReport.Cell("**VAT", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(dtRow["AmountVND"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Amount", 0, i).Value = DBConvert.ParseDouble(dtRow["AmountVND"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Amount", 0, i).Value = "";
          }

          oXlsReport.Cell("**RequiredDeliveryDate", 0, i).Value = dtRow["RequestDate"].ToString();
          if (dtRow["ActualReceipted"].ToString().Length > 0)
          {
            oXlsReport.Cell("**Actual", 0, i).Value = dtRow["ActualReceipted"].ToString();
          }
          else
          {
            oXlsReport.Cell("**Actual", 0, i).Value = "";
          }

          if (dtRow["Diff"].ToString().Length > 0)
          {
            oXlsReport.Cell("**Diff", 0, i).Value = DBConvert.ParseInt(dtRow["Diff"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Diff", 0, i).Value = "";
          }

          if (dtRow["Local"].ToString().Length > 0)
          {
            oXlsReport.Cell("**Local", 0, i).Value = DBConvert.ParseInt(dtRow["Local"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Local", 0, i).Value = "";
          }

          if (dtRow["Import"].ToString().Length > 0)
          {
            oXlsReport.Cell("**Import", 0, i).Value = DBConvert.ParseInt(dtRow["Import"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Import", 0, i).Value = "";
          }
          if (dtRow["CurrentPRRequestDate"].ToString().Length > 0)
          {
            oXlsReport.Cell("**1", 0, i).Value = dtRow["CurrentPRRequestDate"].ToString();
          }
          else
          {
            oXlsReport.Cell("**1", 0, i).Value = "";
          }
          if (dtRow["PRCreateDate"].ToString().Length > 0)
          {
            oXlsReport.Cell("**2", 0, i).Value = dtRow["PRCreateDate"].ToString();
          }
          else
          {
            oXlsReport.Cell("**2", 0, i).Value = "";
          }
          if (dtRow["LeadtimeNumber"].ToString().Length > 0)
          {
            oXlsReport.Cell("**3", 0, i).Value = DBConvert.ParseInt(dtRow["LeadtimeNumber"].ToString());
          }
          else
          {
            oXlsReport.Cell("**3", 0, i).Value = "";
          }
          if (dtRow["LeadTimeDate"].ToString().Length > 0)
          {
            oXlsReport.Cell("**4", 0, i).Value = dtRow["LeadTimeDate"].ToString();

          }
          else
          {
            oXlsReport.Cell("**4", 0, i).Value = "";
          }
          if (dtRow["DiffrentBetweenRequiredDateAndLeadtime"].ToString().Length > 0)
          {
            oXlsReport.Cell("**5", 0, i).Value = DBConvert.ParseInt(dtRow["DiffrentBetweenRequiredDateAndLeadtime"].ToString());
          }
          else
          {
            oXlsReport.Cell("**5", 0, i).Value = "";
          }
          if (dtRow["PROntime"].ToString().Length > 0)
          {
            oXlsReport.Cell("**6", 0, i).Value = DBConvert.ParseInt(dtRow["PROntime"].ToString());
          }
          else
          {
            oXlsReport.Cell("**6", 0, i).Value = "";
          }
          if (dtRow["Value"].ToString().Length > 0)
          {
            oXlsReport.Cell("**7", 0, i).Value = dtRow["Value"].ToString();
          }
          else
          {
            oXlsReport.Cell("**7", 0, i).Value = "";
          }
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void FollowUpReport()
    {
      string strTemplateName = "RPT_PUR_08_001_12";
      string strSheetName = "Sheet1";
      string strOutFileName = "PR Follow Up Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      if (ultCBDepartment.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Department");
        return;
      }

      if (ultDTFromDate.Value == null ||
          DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Date From");
        return;
      }

      if (ultDTToDate.Value == null ||
          DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Date To");
        return;
      }

      //Search
      DBParameter[] arrInput = new DBParameter[4];

      string deparment = string.Empty;
      DateTime prFromDate = DateTime.MinValue;
      DateTime prToDate = DateTime.MinValue;

      // Department
      if (ultCBDepartment.Value != null)
      {
        deparment = ultCBDepartment.Value.ToString();
        arrInput[0] = new DBParameter("@Department", DbType.AnsiString, deparment);
      }

      // PR From Date
      if (ultDTFromDate.Value != null)
      {
        prFromDate = DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        arrInput[1] = new DBParameter("@PRFromDate", DbType.DateTime, prFromDate);
      }

      // PR To Date
      if (ultDTToDate.Value != null)
      {
        prToDate = DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        arrInput[2] = new DBParameter("@PRToDate", DbType.DateTime, prToDate.AddDays(1));
      }

      // Status
      if (this.radRemain.Checked)
      {
        arrInput[3] = new DBParameter("@Status", DbType.Int32, 1);
      }
      else
      {
        arrInput[3] = new DBParameter("@Status", DbType.Int32, 0);
      }
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPURRPTPRFollowUpReport", arrInput);

      oXlsReport.Cell("**DateFrom").Value = prFromDate.ToShortDateString();
      oXlsReport.Cell("**DateTo").Value = prToDate.ToShortDateString();

      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:X8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:X8", 0, i).Paste();
          }
          int j = i + 1;
          oXlsReport.Cell("**No", 0, i).Value = j.ToString();
          oXlsReport.Cell("**PRNo", 0, i).Value = dtRow["PROnlineNo"].ToString();
          oXlsReport.Cell("**Department", 0, i).Value = dtRow["Department"].ToString();
          oXlsReport.Cell("**Requester", 0, i).Value = dtRow["EmpName"].ToString();
          oXlsReport.Cell("**CreateDate", 0, i).Value = dtRow["CreateDate"];

          if (dtRow["PurposeOfRequisition"].ToString().Length > 0)
          {
            oXlsReport.Cell("**Purpose", 0, i).Value = dtRow["PurposeOfRequisition"].ToString();
          }
          else
          {
            oXlsReport.Cell("**Purpose", 0, i).Value = "";
          }

          oXlsReport.Cell("**Status", 0, i).Value = dtRow["Status"].ToString();
          oXlsReport.Cell("**Note", 0, i).Value = dtRow["Note"].ToString();
          oXlsReport.Cell("**RequestDate", 0, i).Value = dtRow["RequestDate"];

          if (DBConvert.ParseInt(dtRow["WO"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBConvert.ParseInt(dtRow["WO"].ToString());
          }
          else
          {
            oXlsReport.Cell("**WO", 0, i).Value = DBNull.Value;
          }

          if (dtRow["CarcassCode"].ToString().Length > 0)
          {
            oXlsReport.Cell("**CarcassCode", 0, i).Value = dtRow["CarcassCode"].ToString();
          }
          else
          {
            oXlsReport.Cell("**CarcassCode", 0, i).Value = "";
          }

          if (dtRow["ItemCode"].ToString().Length > 0)
          {
            oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
          }
          else
          {
            oXlsReport.Cell("**ItemCode", 0, i).Value = "";
          }

          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["MaterialNameEn"].ToString();
          oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["MaterialNameVn"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();
          oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["Qty"].ToString());

          if (DBConvert.ParseDouble(dtRow["QtyCancel"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Cancel", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyCancel"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Cancel", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["ReceiptedQty"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Receipted", 0, i).Value = DBConvert.ParseDouble(dtRow["ReceiptedQty"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Receipted", 0, i).Value = DBNull.Value;
          }

          oXlsReport.Cell("**StatusDetail", 0, i).Value = dtRow["StatusDetail"].ToString();
          oXlsReport.Cell("**PurposeDetail", 0, i).Value = dtRow["Remark"].ToString();

          if (dtRow["OtherFinish"].ToString().Length > 0)
          {
            oXlsReport.Cell("**ReceivedDate", 0, i).Value = dtRow["OtherFinish"].ToString();
          }
          else
          {
            oXlsReport.Cell("**ReceivedDate", 0, i).Value = "";
          }

          oXlsReport.Cell("**Balance", 0, i).Value = DBConvert.ParseDouble(dtRow["Balance"].ToString());
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void PaymentTermReport()
    {
      string strTemplateName = "RPT_PUR_08_001_11";
      string strSheetName = "Sheet1";
      string strOutFileName = "Payment Term Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      if (this.txtMonthFrom.Text.Length == 0 ||
          DBConvert.ParseInt(this.txtMonthFrom.Text) == int.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Month From");
        return;
      }

      if (this.txtYearFrom.Text.Length == 0 ||
          DBConvert.ParseInt(this.txtYearFrom.Text) == int.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Year From");
        return;
      }

      if (this.txtMonthTo.Text.Length == 0 ||
          DBConvert.ParseInt(this.txtMonthTo.Text) == int.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Month To");
        return;
      }

      if (this.txtYearTo.Text.Length == 0 ||
          DBConvert.ParseInt(this.txtYearTo.Text) == int.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Year To");
        return;
      }

      //Search
      DBParameter[] arrInput = new DBParameter[5];

      arrInput[0] = new DBParameter("@MonthFrom", DbType.Int32, DBConvert.ParseInt(this.txtMonthFrom.Text));
      arrInput[1] = new DBParameter("@YearFrom", DbType.Int32, DBConvert.ParseInt(this.txtYearFrom.Text));
      arrInput[2] = new DBParameter("@MonthTo", DbType.Int32, DBConvert.ParseInt(this.txtMonthTo.Text));
      arrInput[3] = new DBParameter("@YearTo", DbType.Int32, DBConvert.ParseInt(this.txtYearTo.Text));

      if (this.txtSupplierGroup.Text.Length > 0)
      {
        arrInput[4] = new DBParameter("@SupplierMany", DbType.String, this.txtSupplierGroup.Text);
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPURRPTPaymentTerm_Select", 500, arrInput);

      oXlsReport.Cell("**From").Value = "(" + this.txtMonthFrom.Text + "/" + this.txtYearFrom.Text + ")";
      oXlsReport.Cell("**To").Value = "(" + this.txtMonthTo.Text + "/" + this.txtYearTo.Text + ")";

      if (dtData != null && dtData.Rows.Count > 0)
      {
        double totalSupplier = 0;
        double totalDifferent = 0;

        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B9:G9").Copy();
            oXlsReport.RowInsert(8 + i);
            oXlsReport.Cell("B9:G9", 0, i).Paste();
          }
          oXlsReport.Cell("**SupplierCode", 0, i).Value = dtRow["SupplierCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
          oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameEN"].ToString();

          if (dtRow["DebitFrom"].ToString().Length > 0)
          {
            oXlsReport.Cell("**FromDetail", 0, i).Value = dtRow["DebitFrom"].ToString();
          }
          else
          {
            oXlsReport.Cell("**FromDetail", 0, i).Value = "";
          }

          if (dtRow["DebitTo"].ToString().Length > 0)
          {
            oXlsReport.Cell("**ToDetail", 0, i).Value = dtRow["DebitTo"].ToString();
          }
          else
          {
            oXlsReport.Cell("**ToDetail", 0, i).Value = "";
          }

          oXlsReport.Cell("**Diff", 0, i).Value = DBConvert.ParseInt(dtRow["FromKind"].ToString())
                          - DBConvert.ParseInt(dtRow["ToKind"].ToString());
          totalDifferent += DBConvert.ParseInt(dtRow["FromKind"].ToString())
                          - DBConvert.ParseInt(dtRow["ToKind"].ToString());
          totalSupplier += 1;
        }

        if (totalDifferent == 0)
        {
          oXlsReport.Cell("**A").Value = 0;
        }
        else
        {
          double percent = (totalDifferent / totalSupplier) - 10;
          oXlsReport.Cell("**A").Value = Math.Round(percent, 2);
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void AlternativeSupplierReport()
    {
      string strTemplateName = "RPT_PUR_08_001_10";
      string strSheetName = "Sheet1";
      string strOutFileName = "Alternative Supplier Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      if (this.ultUsedInBOM.Value == null ||
          DBConvert.ParseInt(this.ultUsedInBOM.Value.ToString()) == int.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Used In BOM");
        return;
      }

      if (this.txtConditionNumber.Text.Length == 0
          || DBConvert.ParseInt(this.txtConditionNumber.Text) == int.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Condition Number");
        return;
      }

      //Search
      DBParameter[] arrInput = new DBParameter[5];

      arrInput[0] = new DBParameter("@MaterialCode", DbType.String, this.txtMaterialCode.Text);
      arrInput[1] = new DBParameter("@UsedInBOM", DbType.Int32, DBConvert.ParseInt(this.ultUsedInBOM.Value.ToString()));

      if (this.ultClassification.Value != null &&
          DBConvert.ParseInt(this.ultClassification.Value.ToString()) != int.MinValue)
      {
        arrInput[2] = new DBParameter("@Classification", DbType.Int32, DBConvert.ParseInt(this.ultClassification.Value.ToString()));
      }

      if (this.txtLastPriceFrom.Text.Length > 0
          && DBConvert.ParseDouble(this.txtLastPriceFrom.Text) != double.MinValue)
      {
        arrInput[3] = new DBParameter("@LastPriceFrom", DbType.Double, DBConvert.ParseDouble(this.txtLastPriceFrom.Text));
      }

      if (this.txtLastPriceTo.Text.Length > 0
          && DBConvert.ParseDouble(this.txtLastPriceTo.Text) != double.MinValue)
      {
        arrInput[4] = new DBParameter("@LastPriceTo", DbType.Double, DBConvert.ParseDouble(this.txtLastPriceTo.Text));
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPURRAlternativeSupplierReport_Select", 500, arrInput);

      oXlsReport.Cell("**Remark").Value = "A = (count the total material which >= " + this.txtConditionNumber.Text
                  + " suppliers : total materials) x 100%";

      if (dtData != null && dtData.Rows.Count > 0)
      {
        int total = 0;
        int pass = 0;
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B9:F9").Copy();
            oXlsReport.RowInsert(8 + i);
            oXlsReport.Cell("B9:F9", 0, i).Paste();
          }
          oXlsReport.Cell("**StockCode", 0, i).Value = dtRow["StockCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();
          if (DBConvert.ParseDouble(dtRow["PriceVND"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**UnitPrice", 0, i).Value = DBConvert.ParseDouble(dtRow["PriceVND"].ToString());
          }
          else
          {
            oXlsReport.Cell("**UnitPrice", 0, i).Value = "";
          }
          oXlsReport.Cell("**TotalSupplier", 0, i).Value = DBConvert.ParseInt(dtRow["TotalSupplier"].ToString());

          if (DBConvert.ParseInt(dtRow["TotalSupplier"].ToString())
              >= DBConvert.ParseInt(this.txtConditionNumber.Text))
          {
            pass += 1;
          }
          total += 1;
        }
        if (pass == 0)
        {
          oXlsReport.Cell("**A").Value = 0;
        }
        else
        {
          double percent = DBConvert.ParseDouble(pass) / DBConvert.ParseDouble(total) * 100;
          oXlsReport.Cell("**A").Value = Math.Round(percent, 2);
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void KPIDeliveryPOReport()
    {
      string strTemplateName = "RPT_PUR_08_001_08";
      string strSheetName = "Sheet1";
      string strOutFileName = "KPI Delivery PO Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      if (ultDTFromDate.Value == null ||
          DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Date From");
        return;
      }

      if (ultDTToDate.Value == null ||
          DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Date To");
        return;
      }

      //Search
      DBParameter[] arrInput = new DBParameter[3];
      DateTime prFromDate = DateTime.MinValue;
      DateTime prToDate = DateTime.MinValue;

      // PR From Date
      prFromDate = DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      arrInput[0] = new DBParameter("@DateFrom", DbType.DateTime, prFromDate);

      // PR To Date
      prToDate = DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      arrInput[1] = new DBParameter("@DateTo", DbType.DateTime, prToDate.AddDays(1));

      if (this.ultGroupInCharge.Value != null
          && DBConvert.ParseInt(this.ultGroupInCharge.Value.ToString()) != int.MinValue)
      {
        arrInput[2] = new DBParameter("@GroupInCharge", DbType.Int32, DBConvert.ParseInt(this.ultGroupInCharge.Value.ToString()));
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPURRPTDeliveryPOReport_Select", 500, arrInput);

      if (ultDTFromDate.Value != null && DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        oXlsReport.Cell("**POFromDate").Value = DBConvert.ParseDateTime(ultDTFromDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }
      if (ultDTToDate.Value != null && DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        oXlsReport.Cell("**POToDate").Value = DBConvert.ParseDateTime(ultDTToDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }

      if (dtData != null && dtData.Rows.Count > 0)
      {
        int localTotal = 0;
        int importTotal = 0;
        int local = 0;
        int import = 0;
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B10:U10").Copy();
            oXlsReport.RowInsert(9 + i);
            oXlsReport.Cell("B10:U10", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**PONo", 0, i).Value = dtRow["PONo"].ToString();
          oXlsReport.Cell("**SupplierName", 0, i).Value = dtRow["SupplierName"].ToString();
          oXlsReport.Cell("**PRNo", 0, i).Value = dtRow["PRNo"].ToString();
          if (dtRow["GroupName"].ToString().Length > 0)
          {
            oXlsReport.Cell("**GroupPurChaser", 0, i).Value = dtRow["GroupName"].ToString();
          }
          oXlsReport.Cell("**Department", 0, i).Value = dtRow["Department"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();

          if (dtRow["Source"].ToString().Length > 0)
          {
            oXlsReport.Cell("**Source", 0, i).Value = dtRow["Source"].ToString();
          }
          else
          {
            oXlsReport.Cell("**Source", 0, i).Value = "";
          }

          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();

          if (DBConvert.ParseDouble(dtRow["Quantity"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["Quantity"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Qty", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(dtRow["UnitPriceVND"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**UnitPrice", 0, i).Value = DBConvert.ParseDouble(dtRow["UnitPriceVND"].ToString());
          }
          else
          {
            oXlsReport.Cell("**UnitPrice", 0, i).Value = "";
          }

          if (DBConvert.ParseInt(dtRow["VAT"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**VAT", 0, i).Value = DBConvert.ParseInt(dtRow["VAT"].ToString());
          }
          else
          {
            oXlsReport.Cell("**VAT", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(dtRow["AmountVND"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Amount", 0, i).Value = DBConvert.ParseDouble(dtRow["AmountVND"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Amount", 0, i).Value = "";
          }

          oXlsReport.Cell("**RequiredDeliveryDate", 0, i).Value = dtRow["ExpectDate"].ToString();
          if (dtRow["ActualReceipted"].ToString().Length > 0)
          {
            oXlsReport.Cell("**Actual", 0, i).Value = dtRow["ActualReceipted"].ToString();
          }
          else
          {
            oXlsReport.Cell("**Actual", 0, i).Value = "";
          }

          if (dtRow["Diff"].ToString().Length > 0)
          {
            oXlsReport.Cell("**Diff", 0, i).Value = DBConvert.ParseInt(dtRow["Diff"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Diff", 0, i).Value = "";
          }

          if (dtRow["Local"].ToString().Length > 0)
          {
            oXlsReport.Cell("**Local", 0, i).Value = DBConvert.ParseInt(dtRow["Local"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Local", 0, i).Value = "";
          }

          if (dtRow["Import"].ToString().Length > 0)
          {
            oXlsReport.Cell("**Import", 0, i).Value = DBConvert.ParseInt(dtRow["Import"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Import", 0, i).Value = "";
          }

          if (DBConvert.ParseInt(dtRow["Code"].ToString()) == 1)
          {
            if (DBConvert.ParseInt(dtRow["Local"].ToString()) == 1)
            {
              local += 1;
            }
            if (dtRow["Local"].ToString().Length > 0)
            {
              localTotal += 1;
            }
          }
          else if (DBConvert.ParseInt(dtRow["Code"].ToString()) == 2)
          {
            if (DBConvert.ParseInt(dtRow["Import"].ToString()) == 1)
            {
              import += 1;
            }
            if (dtRow["Import"].ToString().Length > 0)
            {
              importTotal += 1;
            }
          }
        }
        if (local == 0)
        {
          oXlsReport.Cell("**A").Value = 0;
        }
        else
        {
          double localA = DBConvert.ParseDouble(local.ToString()) / DBConvert.ParseDouble(localTotal.ToString()) * 100;
          oXlsReport.Cell("**A").Value = Math.Round(localA, 2);
        }

        if (import == 0)
        {
          oXlsReport.Cell("**B").Value = 0;
        }
        else
        {
          double importB = DBConvert.ParseDouble(import.ToString()) / DBConvert.ParseDouble(importTotal.ToString()) * 100;
          oXlsReport.Cell("**B").Value = Math.Round(importB, 2);
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void ConsumptionMaterial()
    {
      string strTemplateName = "RPT_PUR_08_001_06";
      string strSheetName = "Sheet1";
      string strOutFileName = "Consumption Of Material";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DateTime fromMonthDate = DBConvert.ParseDateTime(string.Format("01/{0}/{1}", DBConvert.ParseInt(ultCBFromMonth.Value.ToString()), DBConvert.ParseInt(ultCBFromYear.Value.ToString())), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      DateTime toMonthDate = DBConvert.ParseDateTime(string.Format("01/{0}/{1}", DBConvert.ParseInt(ultToMonth.Value.ToString()), DBConvert.ParseInt(ultToYear.Value.ToString())), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);

      string fromMonth = fromMonthDate.ToString("MMM/yyyy");
      string toMonth = toMonthDate.ToString("MMM/yyyy");

      oXlsReport.Cell("**Datetime").Value = "From " + fromMonth + " To " + toMonth;

      DBParameter[] inputParam = new DBParameter[5];
      inputParam[0] = new DBParameter("@FromMonth", DbType.Int32, DBConvert.ParseInt(ultCBFromMonth.Value.ToString()));
      inputParam[1] = new DBParameter("@FromYear", DbType.Int32, DBConvert.ParseInt(ultCBFromYear.Value.ToString()));
      inputParam[2] = new DBParameter("@ToMonth", DbType.Int32, DBConvert.ParseInt(ultToMonth.Value.ToString()));
      inputParam[3] = new DBParameter("@ToYear", DbType.Int32, DBConvert.ParseInt(ultToYear.Value.ToString()));
      if (txtMaterialGroup.Text.Trim().Length > 0)
      {
        inputParam[4] = new DBParameter("@MaterialGroup", DbType.String, txtMaterialGroup.Text.Trim());
      }
      DataTable dtMain = DataBaseAccess.SearchStoreProcedureDataTable("spPURRPTConsumptionOfMaterial", inputParam);
      if (dtMain != null && dtMain.Rows.Count > 0)
      {
        for (int col = 0; col < dtMain.Columns.Count; col++)
        {
          if (col > 0)
          {
            oXlsReport.Cell("B8:B9").Copy();
            oXlsReport.ColumnInsert(2 + col);
            oXlsReport.Cell("B8:B9", col, 0).Paste();
          }
          oXlsReport.Cell("**ColName", col, 0).Value = dtMain.Columns[col].Caption;
        }

        int colNumberStart = 2;
        int colNumberFinish = colNumberStart + dtMain.Columns.Count;
        string colNameFinish = this.ExcelColumnFromNumber(colNumberFinish);

        for (int i = 0; i < dtMain.Rows.Count; i++)
        {
          DataRow dtRow = dtMain.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell(string.Format("B9:{0}9", colNameFinish)).Copy();
            oXlsReport.RowInsert(8 + i);
            oXlsReport.Cell(string.Format("B9:{0}9", colNameFinish), 0, i).Paste();
          }
          for (int j = 0; j < colNumberFinish - 2; j++)
          {
            string colName = this.ExcelColumnFromNumber(j + 1);
            if (dtRow[j].ToString().Length > 0)
            {
              oXlsReport.Cell(string.Format("{0}{1}:{0}{1}", colName, i + 9)).Value = dtRow[j];
            }
            else
            {
              oXlsReport.Cell(string.Format("{0}{1}:{0}{1}", colName, i + 9)).Value = "";
            }
          }
        }
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
    /// Load Month
    /// </summary>
    private void LoadComboMonth(UltraCombo ultCB)
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Month", typeof(Int32));
      for (int i = 1; i < 13; i++)
      {
        DataRow dr = dt.NewRow();
        dr["Month"] = i;
        dt.Rows.Add(dr);
      }
      ultCB.DataSource = dt;
      ultCB.DisplayMember = "Month";
      ultCB.ValueMember = "Month";
      ultCB.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Year
    /// </summary>
    private void LoadComboYear(UltraCombo ultCB)
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Year", typeof(Int32));
      for (int i = 2008; i < 2099; i++)
      {
        DataRow dr = dt.NewRow();
        dr["Year"] = i;
        dt.Rows.Add(dr);
      }
      ultCB.DataSource = dt;
      ultCB.DisplayMember = "Year";
      ultCB.ValueMember = "Year";
      ultCB.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Material Group
    /// </summary>
    private void LoadMaterialGroup()
    {
      string commandText = string.Empty;
      commandText += " SELECT [Group] + '-' + Category AS [Group], Name ";
      commandText += " FROM TblGNRMaterialCategory ";

      DataTable dtMaterialGroup = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucMaterialGroup.DataSource = dtMaterialGroup;
      ucMaterialGroup.ColumnWidths = "100;200";
      ucMaterialGroup.DataBind();
      ucMaterialGroup.ValueMember = "Group";
    }

    /// <summary>
    /// Load Supplier
    /// </summary>
    private void LoadSupplierMany()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, EnglishName ";
      commandText += " FROM TblPURSupplierInfo ";
      commandText += " WHERE Confirm = 2 ";
      commandText += " 	AND LEN(EnglishName) > 0 ";
      commandText += " ORDER BY EnglishName ";

      DataTable dtSupplier = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucSupplier.DataSource = dtSupplier;
      ucSupplier.ColumnWidths = "0;200";
      ucSupplier.DataBind();
      ucSupplier.ValueMember = "Pid";
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
          this.ExpenditureForDepartmentReport();
        }
        else if (value == 2)
        {
          this.SupplierPurchaseOrder();
        }
        else if (value == 3)
        {
          this.PurchaseAmountSupplier();
        }
        else if (value == 4)
        {
          this.PerformenceForPurchaser();
        }
        else if (value == 5)
        {
          this.ConsumptionMaterial();
        }
        else if (value == 6)
        {
          this.KPIDeliveryPRReport();
        }
        else if (value == 7)
        {
          this.KPIDeliveryPOReport();
        }
        else if (value == 8)
        {
          this.AlternativeSupplierReport();
        }
        else if (value == 9)
        {
          this.PaymentTermReport();
        }
        else if (value == 10)
        {
          this.FollowUpReport();
        }
      }
      else
      {
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
        labDepartment.ForeColor = System.Drawing.SystemColors.ControlText;
        labPRFromDate.ForeColor = System.Drawing.SystemColors.ControlText;
        labPRToDate.ForeColor = System.Drawing.SystemColors.ControlText;
        labSupplier.ForeColor = System.Drawing.SystemColors.ControlText;
        labFromMonthYear.ForeColor = System.Drawing.SystemColors.ControlText;
        labToMonthYear.ForeColor = System.Drawing.SystemColors.ControlText;
        labFromMonth.ForeColor = System.Drawing.SystemColors.ControlText;
        labFromYear.ForeColor = System.Drawing.SystemColors.ControlText;
        labToMonth.ForeColor = System.Drawing.SystemColors.ControlText;
        labToYear.ForeColor = System.Drawing.SystemColors.ControlText;
        labMaterialGroup.ForeColor = System.Drawing.SystemColors.ControlText;
        labPRFromDate.ForeColor = System.Drawing.SystemColors.ControlText;
        labPRToDate.ForeColor = System.Drawing.SystemColors.ControlText;
        lblGroup.ForeColor = System.Drawing.SystemColors.ControlText;
        lblMaterialCode.ForeColor = System.Drawing.SystemColors.ControlText;
        lblUsedInBOM.ForeColor = System.Drawing.SystemColors.ControlText;
        lblClassification.ForeColor = System.Drawing.SystemColors.ControlText;
        lblConditionNumber.ForeColor = System.Drawing.SystemColors.ControlText;
        lblLastPriceFrom.ForeColor = System.Drawing.SystemColors.ControlText;
        lblLastPriceTo.ForeColor = System.Drawing.SystemColors.ControlText;
        lblSupplier.ForeColor = System.Drawing.SystemColors.ControlText;
        lblMonthFrom.ForeColor = System.Drawing.SystemColors.ControlText;
        lblYearFrom.ForeColor = System.Drawing.SystemColors.ControlText;
        lblMonthTo.ForeColor = System.Drawing.SystemColors.ControlText;
        lblYearTo.ForeColor = System.Drawing.SystemColors.ControlText;
        radAll.ForeColor = System.Drawing.SystemColors.ControlText;
        radRemain.ForeColor = System.Drawing.SystemColors.ControlText;

        // End

        value = DBConvert.ParseInt(ultCBReport.Value.ToString());
        if (value == 1)
        {
          // To Mau Control
          labDepartment.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labPRFromDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labPRToDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          // End
        }
        else if (value == 2)
        {
          // To Mau Control
          labSupplier.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labPRFromDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labPRToDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          // End
        }
        else if (value == 3)
        {
          labPRFromDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labPRToDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
        else if (value == 4)
        {
          labDepartment.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labPRFromDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labPRToDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
        else if (value == 5)
        {
          labFromMonthYear.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labToMonthYear.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labFromMonth.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labFromYear.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labToMonth.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labToYear.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labMaterialGroup.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
        else if (value == 6)
        {
          labPRFromDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labPRToDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          lblGroup.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
        else if (value == 7)
        {
          labPRFromDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labPRToDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          lblGroup.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
        else if (value == 8)
        {
          lblMaterialCode.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          lblUsedInBOM.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          lblClassification.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          lblConditionNumber.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          lblLastPriceFrom.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          lblLastPriceTo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
        else if (value == 9)
        {
          lblSupplier.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          lblMonthFrom.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          lblYearFrom.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          lblMonthTo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          lblYearTo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
        else if (value == 10)
        {
          // To Mau Control
          labDepartment.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labPRFromDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labPRToDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          radAll.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          radRemain.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", "Report");
        return;
      }
    }

    private void ucMaterialGroup_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtMaterialGroup.Text = this.ucMaterialGroup.SelectedValue;
    }

    private void ucSupplier_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtSupplierGroup.Text = this.ucSupplier.SelectedValue;
    }

    private void chkMaterialGroup_CheckedChanged(object sender, EventArgs e)
    {
      ucMaterialGroup.Visible = chkMaterialGroup.Checked;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultUsedInBOM_ValueChanged(object sender, EventArgs e)
    {
      if (this.ultUsedInBOM.Value != null
          && DBConvert.ParseInt(this.ultUsedInBOM.Value.ToString()) == 1)
      {
        string commandText = string.Empty;
        commandText += " SELECT NULL ID, 'ALL' Name";
        commandText += " UNION";
        commandText += " SELECT 9 ID, 'Main Materials' Name";
        commandText += " UNION";
        commandText += " SELECT 3 ID, 'Support' Name";
        commandText += " UNION";
        commandText += " SELECT 6 ID, 'Chemical' Name";
        commandText += " UNION";
        commandText += " SELECT 4 ID, 'Accessory' Name";
        commandText += " UNION";
        commandText += " SELECT 5 ID, 'Upholstery' Name";
        commandText += " UNION";
        commandText += " SELECT 2 ID, 'Glass' Name";
        commandText += " UNION";
        commandText += " SELECT 7 ID, 'Packing' Name";
        commandText += " UNION";
        commandText += " SELECT 1 ID, 'Hardware' Name";
        System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

        ultClassification.DataSource = dtSource;
        ultClassification.DisplayMember = "Name";
        ultClassification.ValueMember = "ID";
        ultClassification.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultClassification.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
        ultClassification.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      }
      else
      {
        ultClassification.DataSource = null;
      }
    }

    private void chkSupplier_CheckedChanged(object sender, EventArgs e)
    {
      ucSupplier.Visible = chkSupplier.Checked;
    }
    #endregion Event   
  }
}
