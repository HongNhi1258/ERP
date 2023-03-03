/*
 * Author       : 
 * CreateDate   : 21/09/2012
 * Description  : Update Standard Cost & Actual Price
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

namespace DaiCo.Purchasing
{
  public partial class viewPUR_01_012 : MainUserControl
  {
    #region Init
    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;
    private bool flagPur = false;
    public viewPUR_01_012()
    {
      InitializeComponent();
    }

    private void viewPUR_01_012_Load(object sender, EventArgs e)
    {
      // Purchaser
      if (this.btnPerPurchaser.Visible == true)
      {
        this.flagPur = true;
        this.btnPerPurchaser.Visible = false;
      }

      string startupPath = System.Windows.Forms.Application.StartupPath;
      this.pathTemplate = startupPath + @"\ExcelTemplate";
      this.pathExport = startupPath + @"\Report";

      //Load Status
      this.LoadComboStatus();
      //Load Using In BOM
      this.LoadComboUsingBOM();
    }
    #endregion Init

    #region Load Data

    /// <summary>
    /// Load UltraCombo Status (0: Normal / 1: Warning)
    /// </summary>
    private void LoadComboStatus()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 0 ID, 'Normal' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Warning' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBStatus.DataSource = dtSource;
      ultCBStatus.DisplayMember = "Name";
      ultCBStatus.ValueMember = "ID";
      ultCBStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBStatus.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultCBStatus.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Status (0: UsingInBOM / 1: Not UsingInBOM)
    /// </summary>
    private void LoadComboUsingBOM()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 0 ID, 'Used In BOM' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Not Used In BOM' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBUsingBOM.DataSource = dtSource;
      ultCBUsingBOM.DisplayMember = "Name";
      ultCBUsingBOM.ValueMember = "ID";
      ultCBUsingBOM.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBUsingBOM.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultCBUsingBOM.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      ultCBUsingBOM.Value = 0;
    }


    private void Search()
    {
      DBParameter[] param = new DBParameter[3];
      // Material Code
      string code = txtMaterialCode.Text.Trim();
      param[0] = new DBParameter("@Material", DbType.String, code);
      if (ultCBStatus.Value != null)
      {
        if (DBConvert.ParseInt(ultCBStatus.Value.ToString()) != int.MinValue)
        {
          param[1] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ultCBStatus.Value.ToString()));
        }
      }
      if (ultCBUsingBOM.Value != null)
      {
        if (DBConvert.ParseInt(ultCBUsingBOM.Value.ToString()) != int.MinValue)
        {
          param[2] = new DBParameter("@UsingInBOM", DbType.Int32, DBConvert.ParseInt(ultCBUsingBOM.Value.ToString()));
        }
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPURGetInforMaterialStandardCost_Select", 600, param);
      ultData.DataSource = dtSource;
      // To Mau Warning
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Status"].Value.ToString()) == 1)
        {
          row.Appearance.BackColor = Color.Aqua;
        }
      }
    }
    #endregion

    #region Save Data
    private void SaveData()
    {
      bool success = true;
      int count = ultData.Rows.Count;
      DataTable dt = (DataTable)this.ultData.DataSource;
      foreach (DataRow row in dt.Rows)
      {
        if (row.RowState == DataRowState.Modified)
        {
          DBParameter[] param = new DBParameter[7];
          param[0] = new DBParameter("@MaterialCode", DbType.String, row["MaterialCode"].ToString());
          if (DBConvert.ParseDouble(row["StandardCost"].ToString()) != double.MinValue)
          {
            param[1] = new DBParameter("@StandardCost", DbType.Double, DBConvert.ParseDouble(row["StandardCost"].ToString()));
          }
          if (DBConvert.ParseDouble(row["ActualPrice"].ToString()) != double.MinValue)
          {
            param[2] = new DBParameter("@ActualPrice", DbType.Double, DBConvert.ParseDouble(row["ActualPrice"].ToString()));
          }
          param[3] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          param[4] = new DBParameter("@RemarkStandardCost", DbType.String, row["RemarkStandardCost"].ToString());
          if (DBConvert.ParseDouble(row["StandardCostOld"].ToString()) != double.MinValue)
          {
            param[5] = new DBParameter("@StandardCostOld", DbType.Double, DBConvert.ParseDouble(row["StandardCostOld"].ToString()));
          }
          if (DBConvert.ParseDouble(row["ActualPriceOld"].ToString()) != double.MinValue)
          {
            param[6] = new DBParameter("@ActualPriceOld", DbType.Double, DBConvert.ParseDouble(row["ActualPriceOld"].ToString()));
          }
          DBParameter[] outputparam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialStandardCost_Update", param, outputparam);
          if (DBConvert.ParseLong(outputparam[0].Value.ToString()) == 0)
          {
            success = false;
          }
        }
      }

      if (success)
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("WRN0004");
      }
    }

    #endregion

    #region Others Events

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
      this.Search();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.Search();
        btnSearch.Enabled = true;
      }
    }

    private void ExportExcel()
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        //ultData.Rows.Band.Columns["BOM"].Hidden = false;
        ControlUtility.ExportToExcelWithDefaultPath(ultData, out xlBook, "ListOfMaterial", 7);
        //ultData.Rows.Band.Columns["BOM"].Hidden = true;

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "Dai Co Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

        xlSheet.Cells[3, 1] = "List Of Material With Standard Cost And Actual Price";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        xlSheet.Cells[5, 1] = "Note: ";
        r.Font.Bold = true;

        xlSheet.Cells[5, 2] = "Warning";
        r = xlSheet.get_Range("B5", "B5");
        r.Interior.Color = (object)ColorTranslator.ToOle(Color.FromArgb(0, 255, 255));

        for (int i = 0; i < ultData.Rows.Band.Columns.Count; i++)
        {
          string colName = ultData.Rows.Band.Columns[i].Header.Caption;
          if (string.Compare(colName, "Price1 (VND)", true) == 0 ||
             string.Compare(colName, "Price2 (VND)", true) == 0 ||
             string.Compare(colName, "Price3 (VND)", true) == 0 ||
             string.Compare(colName, "PriceMin (VND)", true) == 0 ||
             string.Compare(colName, "PriceMax (VND)", true) == 0 ||
             string.Compare(colName, "StandardCost", true) == 0 ||
             string.Compare(colName, "ActualPrice", true) == 0)
          {
            string colExcel = ExcelColumnFromNumber(i) + "1";
            Excel.Range fm = xlSheet.get_Range("" + colExcel + "", "" + colExcel + "");
            fm.EntireColumn.NumberFormat = "#,###.00";
          }
        }

        xlBook.Application.DisplayAlerts = false;

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }

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
    #endregion

    #region UltraGrid Events

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      //e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[0].Columns["UsingInBOM"].Hidden = true;
      e.Layout.Bands[0].Columns["BOM"].Hidden = true;
      e.Layout.Bands[0].Columns["StandardCostOld"].Hidden = true;
      e.Layout.Bands[0].Columns["ActualPriceOld"].Hidden = true;

      e.Layout.Bands[0].Columns["BOM"].Header.Caption = "UsingInBOM";
      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "Material Name EN";
      e.Layout.Bands[0].Columns["NameVN"].Header.Caption = "Material Name VN";
      e.Layout.Bands[0].Columns["RemarkStandardCost"].Header.Caption = "Remark";
      e.Layout.Bands[0].Columns["MaterialSource"].Header.Caption = "Source";
      e.Layout.Bands[0].Columns["Price3"].Header.Caption = "Price3 (VND)";
      e.Layout.Bands[0].Columns["Price2"].Header.Caption = "Price2 (VND)";
      e.Layout.Bands[0].Columns["Price1"].Header.Caption = "Price1 (VND)";
      e.Layout.Bands[0].Columns["PriceMin"].Header.Caption = "PriceMin (VND)";
      e.Layout.Bands[0].Columns["PriceMax"].Header.Caption = "PriceMax (VND)";
      e.Layout.Bands[0].Columns["WarningPricePercent"].Header.Caption = "Warning(%)";
      e.Layout.Bands[0].Columns["AverageConsumption6Months"].Header.Caption = "Average Consumption 6 Months";

      e.Layout.Bands[0].Columns["MaterialSource"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["AverageConsumption6Months"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Amount"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Date3"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Date2"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Date1"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["UpdateDateStandardCost"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["UpdateDateActualPrice"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameVN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Price3"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Price2"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Price1"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["WarningPricePercent"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PriceMin"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PriceMax"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["StandardCost"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["ActualPrice"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["StandardCostOld"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["ActualPriceOld"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Price3"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Price2"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Price1"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["WarningPricePercent"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["PriceMin"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["PriceMax"].CellAppearance.TextHAlign = HAlign.Right;

      //e.Layout.Bands[0].Columns["AverageConsumption6Months"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["Amount"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["ActualPrice"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["StandardCost"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["ActualPriceOld"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["StandardCostOld"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["Price3"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["Price2"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["Price1"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["PriceMin"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["PriceMax"].Format = "###,###.##";

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      if (this.flagPur == true)
      {
        e.Layout.Bands[0].Columns["StandardCost"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["ActualPrice"].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Alpha.Transparent;
      e.Layout.Override.CellAppearance.BackColorAlpha = Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Alpha.UseAlphaLevel;
    }

    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtImport.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtImport.Text.Trim().Length > 0);
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "RPT_PUR_01_012_001";
      string sheetName = "Data";
      string outFileName = "RPT_PUR_01_012_001";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      if (this.txtImport.Text.Trim().Length == 0)
      {
        return;
      }

      try
      {
        DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImport.Text.Trim(), "SELECT * FROM [Data (1)$B5:E15007]").Tables[0];

        if (dtSource == null)
        {
          return;
        }

        foreach (DataRow row in dtSource.Rows)
        {
          if (row["MaterialCode"].ToString().Trim().Length == 0)
          {
            continue;
          }

          string commandText = string.Empty;
          commandText += " SELECT COUNT(*) ";
          commandText += " FROM TblGNRMaterialInformation ";
          commandText += " WHERE MaterialCode = '" + row["MaterialCode"].ToString() + "' ";
          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dt != null && dt.Rows.Count > 0)
          {
            //if (row["ActualPrice"].ToString().Trim().Length == 0 && row["StandardCost"].ToString().Trim().Length == 0)
            //{
            //  continue;
            //}

            DBParameter[] param = new DBParameter[5];
            param[0] = new DBParameter("@MaterialCode", DbType.String, row["MaterialCode"].ToString());
            if (row["StandardCost"].ToString().Trim().Length > 0 && DBConvert.ParseDouble(row["StandardCost"].ToString().Trim()) != double.MinValue)
            {
              param[1] = new DBParameter("@StandardCost", DbType.Double, DBConvert.ParseDouble(row["StandardCost"].ToString()));
            }

            if (row["ActualPrice"].ToString().Trim().Length > 0 && DBConvert.ParseDouble(row["ActualPrice"].ToString().Trim()) != double.MinValue)
            {
              param[2] = new DBParameter("@ActualPrice", DbType.Double, DBConvert.ParseDouble(row["ActualPrice"].ToString()));
            }

            param[3] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            param[4] = new DBParameter("@RemarkStandardCost", DbType.String, row["Remark"].ToString());
            DBParameter[] outputparam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialStandardCostActualPrice_Update", param, outputparam);
            if (DBConvert.ParseInt(outputparam[0].Value.ToString()) == -1)
            {
              continue;
            }

          }
          else
          {
            continue;
          }
        }

        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0105");
        return;
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      if (string.Compare(ultCBUsingBOM.Text, "Used In BOM", true) == 0)
      {
        this.ExportExcel();
      }
      else
      {
        string strTemplateName = "RPT_PUR_01_012_002";
        string strSheetName = "Sheet1";
        string strOutFileName = "List Of Material With Standard Cost And Actual Price";
        string strStartupPath = System.Windows.Forms.Application.StartupPath;
        string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
        string strPathTemplate = strStartupPath + @"\ExcelTemplate";
        XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

        DataTable dtData = (DataTable)ultData.DataSource;

        if (dtData != null && dtData.Rows.Count > 0)
        {
          oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

          if (dtData != null && dtData.Rows.Count > 0)
          {
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
              DataRow dtRow = dtData.Rows[i];
              if (i > 0)
              {
                oXlsReport.Cell("B7:Y7").Copy();
                oXlsReport.RowInsert(6 + i);
                oXlsReport.Cell("B7:Y7", 0, i).Paste();
              }
              // No
              oXlsReport.Cell("**No", 0, i).Value = i + 1;
              // MaterialCode
              oXlsReport.Cell("**Code", 0, i).Value = dtRow["MaterialCode"].ToString();
              // NameEN
              oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
              // NameVN
              oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameVN"].ToString();
              // Unit
              oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();
              // Material Source
              if (dtRow["MaterialSource"].ToString().Length > 0)
              {
                oXlsReport.Cell("**MaterialSource", 0, i).Value = dtRow["MaterialSource"].ToString();
              }
              else
              {
                oXlsReport.Cell("**MaterialSource", 0, i).Value = "";
              }
              // AverageConsumption6Months
              if (DBConvert.ParseDouble(dtRow["AverageConsumption6Months"].ToString()) > 0)
              {
                oXlsReport.Cell("**AverageConsumption6Months", 0, i).Value = DBConvert.ParseDouble(dtRow["AverageConsumption6Months"].ToString());
              }
              else
              {
                oXlsReport.Cell("**AverageConsumption6Months", 0, i).Value = DBNull.Value;
              }
              // Amount
              if (DBConvert.ParseDouble(dtRow["Amount"].ToString()) > 0)
              {
                oXlsReport.Cell("**Amount", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Amount", 0, i).Value = DBNull.Value;
              }
              // Date1
              if (dtRow["Date1"].ToString().Length > 0)
              {
                oXlsReport.Cell("**Date1", 0, i).Value = dtRow["Date1"].ToString();
              }
              else
              {
                oXlsReport.Cell("**Date1", 0, i).Value = "";
              }
              // Price1
              if (DBConvert.ParseDouble(dtRow["Price1"].ToString()) > 0)
              {
                oXlsReport.Cell("**Price1", 0, i).Value = DBConvert.ParseDouble(dtRow["Price1"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Price1", 0, i).Value = DBNull.Value;
              }
              //SupplierName1
              if (dtRow["SupplierName1"].ToString().Length > 0)
              {
                oXlsReport.Cell("**Supplier1", 0, i).Value = dtRow["SupplierName1"].ToString();
              }
              else
              {
                oXlsReport.Cell("****Supplier1", 0, i).Value = "";
              }
              //PricePO 1
              if (dtRow["PricePO1"].ToString().Length > 0)
              {
                oXlsReport.Cell("**PO1", 0, i).Value = dtRow["PricePO1"].ToString();
              }
              else
              {
                oXlsReport.Cell("**PO1", 0, i).Value = "";
              }
              //Code 1
              if (DBConvert.ParseDouble(dtRow["Code1"].ToString()) > 0)
              {
                oXlsReport.Cell("**Code1", 0, i).Value = DBConvert.ParseDouble(dtRow["Code1"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Code1", 0, i).Value = DBNull.Value;
              }
              // Date2
              if (dtRow["Date2"].ToString().Length > 0)
              {
                oXlsReport.Cell("**Date2", 0, i).Value = dtRow["Date2"].ToString();
              }
              else
              {
                oXlsReport.Cell("**Date2", 0, i).Value = "";
              }
              // Price2
              if (DBConvert.ParseDouble(dtRow["Price2"].ToString()) > 0)
              {
                oXlsReport.Cell("**Price2", 0, i).Value = DBConvert.ParseDouble(dtRow["Price2"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Price2", 0, i).Value = DBNull.Value;
              }
              //SupplierName2
              if (dtRow["SupplierName2"].ToString().Length > 0)
              {
                oXlsReport.Cell("**Supplier2", 0, i).Value = dtRow["SupplierName2"].ToString();
              }
              else
              {
                oXlsReport.Cell("**Supplier2", 0, i).Value = "";
              }
              //PricePO 2
              if (dtRow["PricePO2"].ToString().Length > 0)
              {
                oXlsReport.Cell("**PO2", 0, i).Value = dtRow["PricePO2"].ToString();
              }
              else
              {
                oXlsReport.Cell("**PO2", 0, i).Value = "";
              }
              //Code 2
              if (DBConvert.ParseDouble(dtRow["Code2"].ToString()) > 0)
              {
                oXlsReport.Cell("**Code2", 0, i).Value = DBConvert.ParseDouble(dtRow["Code2"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Code2", 0, i).Value = DBNull.Value;
              }
              // Date3
              if (dtRow["Date3"].ToString().Length > 0)
              {
                oXlsReport.Cell("**Date3", 0, i).Value = dtRow["Date3"].ToString();
              }
              else
              {
                oXlsReport.Cell("**Date3", 0, i).Value = "";
              }
              // Price3
              if (DBConvert.ParseDouble(dtRow["Price3"].ToString()) > 0)
              {
                oXlsReport.Cell("**Price3", 0, i).Value = DBConvert.ParseDouble(dtRow["Price3"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Price3", 0, i).Value = DBNull.Value;
              }
              //SupplierName3
              if (dtRow["SupplierName3"].ToString().Length > 0)
              {
                oXlsReport.Cell("**Supplier3", 0, i).Value = dtRow["SupplierName3"].ToString();
              }
              else
              {
                oXlsReport.Cell("**Supplier3", 0, i).Value = "";
              }
              //Price PO 3
              if (dtRow["PricePO3"].ToString().Length > 0)
              {
                oXlsReport.Cell("**PO3", 0, i).Value = dtRow["PricePO3"].ToString();
              }
              else
              {
                oXlsReport.Cell("**PO3", 0, i).Value = "";
              }
              //Code 3
              if (DBConvert.ParseDouble(dtRow["Code3"].ToString()) > 0)
              {
                oXlsReport.Cell("**Code3", 0, i).Value = DBConvert.ParseDouble(dtRow["Code3"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Code3", 0, i).Value = DBNull.Value;
              }
              // Warning
              if (DBConvert.ParseDouble(dtRow["WarningPricePercent"].ToString()) > 0)
              {
                oXlsReport.Cell("**WarningPricePercent", 0, i).Value = DBConvert.ParseDouble(dtRow["WarningPricePercent"].ToString());
              }
              else
              {
                oXlsReport.Cell("**WarningPricePercent", 0, i).Value = DBNull.Value;
              }
              // Price Min
              if (DBConvert.ParseDouble(dtRow["PriceMin"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**PriceMin", 0, i).Value = DBConvert.ParseDouble(dtRow["PriceMin"].ToString());
              }
              else
              {
                oXlsReport.Cell("**PriceMin", 0, i).Value = "";
              }
              // Price Max
              if (DBConvert.ParseDouble(dtRow["PriceMax"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**PriceMax", 0, i).Value = DBConvert.ParseDouble(dtRow["PriceMax"].ToString());
              }
              else
              {
                oXlsReport.Cell("**PriceMax", 0, i).Value = "";
              }
              // Update StandardCost
              if (dtRow["UpdateDateStandardCost"].ToString().Length > 0)
              {
                oXlsReport.Cell("**UpdateDateStandardCost", 0, i).Value = dtRow["UpdateDateStandardCost"].ToString();
              }
              else
              {
                oXlsReport.Cell("**UpdateDateStandardCost", 0, i).Value = "";
              }
              // StandardCost
              if (DBConvert.ParseDouble(dtRow["StandardCost"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**StandardCost", 0, i).Value = DBConvert.ParseDouble(dtRow["StandardCost"].ToString());
              }
              else
              {
                oXlsReport.Cell("**StandardCost", 0, i).Value = "";
              }
              // Update ActualPrice
              if (dtRow["UpdateDateActualPrice"].ToString().Length > 0)
              {
                oXlsReport.Cell("**UpdateDateActualPrice", 0, i).Value = dtRow["UpdateDateActualPrice"].ToString();
              }
              else
              {
                oXlsReport.Cell("**UpdateDateActualPrice", 0, i).Value = "";
              }
              // ActurePrice
              if (DBConvert.ParseDouble(dtRow["ActualPrice"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**ActurePrice", 0, i).Value = DBConvert.ParseDouble(dtRow["ActualPrice"].ToString());
              }
              else
              {
                oXlsReport.Cell("**ActurePrice", 0, i).Value = "";
              }
              // RemarkStandardCost
              if (dtRow["RemarkStandardCost"].ToString().Length > 0)
              {
                oXlsReport.Cell("**RemarkStandardCost", 0, i).Value = dtRow["RemarkStandardCost"].ToString();
              }
              else
              {
                oXlsReport.Cell("**RemarkStandardCost", 0, i).Value = "";
              }
              // Status
              if (DBConvert.ParseInt(dtRow["Status"].ToString()) == 1)
              {
                oXlsReport.Cell("**Status", 0, i).Value = "Warning";
              }
              else
              {
                oXlsReport.Cell("**Status", 0, i).Value = "";
              }
              // Using In BOM
              if (DBConvert.ParseInt(dtRow["UsingInBOM"].ToString()) == 0)
              {
                oXlsReport.Cell("**UsingInBOM", 0, i).Value = "Yes";
              }
              else
              {
                oXlsReport.Cell("**UsingInBOM", 0, i).Value = DBNull.Value;
              }
            }
          }
          oXlsReport.Out.File(strOutFileName);
          Process.Start(strOutFileName);
        }
      }
    }

    private void ultData_AfterCellActivate(object sender, EventArgs e)
    {
      string colName = ultData.ActiveCell.Column.Header.Caption;
      if (string.Compare(colName, "StandardCost", true) == 0 ||
        string.Compare(colName, "ActualPrice", true) == 0)
      {
        int rowIndex = ultData.ActiveCell.Row.Index;
        int cellIndex = ultData.ActiveCell.Column.Index;

        ultData.Rows[rowIndex].Cells[cellIndex].Activate();
        ultData.PerformAction(UltraGridAction.EnterEditMode, false, false);
      }
    }
    #endregion
  }
}
