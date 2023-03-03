using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using DaiCo.CustomerService.DataSetSource;
using DaiCo.Shared.DataSetSource.CustomerService;
using System.IO;
using DaiCo.Shared.ReportTemplate.CustomerService;
using VBReport;
using System.Diagnostics;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_016 : MainUserControl
  {
    #region Field    
    private string pathTemplate = string.Empty;
    #endregion Field

    #region Init
    public viewCSD_04_016()
    {
      InitializeComponent();
    }

    private void viewCSD_04_016_Load(object sender, EventArgs e)
    {
      ControlUtility.LoadUltraCBCustomer(ultCBCustomer);
      //ultCBCustomer.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      //ultCBCustomer.DisplayLayout.Bands[0].Columns["Name"].Hidden = true;
      this.LoadDDSaleCode();
      this.LoadData();
    }
    #endregion Init

    #region Function
    private void LoadData()
    {
      long cusPid = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultCBCustomer));
      string commandText = string.Format(@"SELECT	BS.SaleCode ItemCode, BS.ItemCode Code, BS.[Description], INFO.Unit, CAST(NULL AS varchar) MOQPerCarcass, PRI.ActualPrice Price
                                             FROM	  TblBOMItemBasic BS
	                                                  INNER JOIN TblBOMItemInfo INFO 
                                                      ON BS.ItemCode = INFO.ItemCode AND BS.RevisionActive = INFO.Revision AND BS.SaleCode IS NOT NULL
                                                    LEFT JOIN TblCSDItemSalePrice PRI ON BS.ItemCode = PRI.ItemCode AND PRI.CustomerPid = BS.CustomerPid
                                             WHERE	BS.CustomerPid = {0}", cusPid);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultData.DataSource = dtSource;
    }

    private void LoadDDSaleCode()
    {
      string commandText = string.Empty;
      if (ultCBCustomer.SelectedRow != null)
      {
        long cusPid = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultCBCustomer));
        commandText = string.Format(@"SELECT	BS.SaleCode, BS.ItemCode, CSD.FactoryDescription [Description], INFO.Unit, CSD.MOQ, PRI.ActualPrice Price
                                      FROM    TblBOMItemBasic BS 
                                        INNER JOIN TblBOMItemInfo INFO 
                                          ON BS.ItemCode = INFO.ItemCode AND BS.RevisionActive = INFO.Revision AND BS.CustomerPid = {0}
                                            AND BS.SaleCode IS NOT NULL AND LEN(BS.SaleCode) > 0
                                        INNER JOIN TblCSDItemInfo CSD ON BS.ItemCode = CSD.ItemCode
                                        LEFT JOIN TblCSDItemSalePrice PRI ON BS.ItemCode = PRI.ItemCode AND PRI.CustomerPid = BS.CustomerPid
                                      ORDER BY BS.SaleCode", cusPid);
      }      
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataTable dtSourceItem = (dtSource == null ? null : FunctionUtility.CloneTable(dtSource));
      ultDDSaleCode.DataSource = dtSource;
      ultDDSaleCode.DisplayMember = "SaleCode";
      ultDDSaleCode.ValueMember = "SaleCode";      

      ControlUtility.LoadUltraDropDown(ultDDItemCode, dtSourceItem, "ItemCode", "ItemCode");      
    }

    private void ExportExcel()
    {
      if (ultCBCustomer.SelectedRow != null)
      {
        long cusPid = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultCBCustomer));
        DataTable dt = (DataTable)ultData.DataSource;
        DataTable dtPrint = dt.Copy();
        if (dtPrint != null && dtPrint.Rows.Count > 0)
        {
          if (dtPrint.Columns.Contains("Code"))
          {
            dtPrint.Columns.Remove("Code");
          }
          if (dtPrint.Columns.Contains("Description"))
          {
            dtPrint.Columns.Remove("Description");
          }
          if (dtPrint.Columns.Contains("Unit"))
          {
            dtPrint.Columns.Remove("Unit");
          }
          SqlDBParameter[] input = new SqlDBParameter[2];
          input[0] = new SqlDBParameter("@CustomerPid", SqlDbType.BigInt, cusPid);
          input[1] = new SqlDBParameter("@ItemPriceList", SqlDbType.Structured, dtPrint);

          DataSet ds = SqlDataBaseAccess.SearchStoreProcedure("spRDDQuotation", input);
          if (ds != null)
          {
            string strTemplateName = "CSDReports";
            string strSheetName = "Quotation";
            string strOutFileName = "Quotation Report";
            string strStartupPath = System.Windows.Forms.Application.StartupPath;
            string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
            string strPathTemplate = strStartupPath + @"\ExcelTemplate\CustomerService";
            XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

            // Add customer info
            DataTable dtCus = ds.Tables[0];
            if (dtCus.Rows.Count > 0)
            {
              dtCus.Rows[0]["CustomerAddress"] = dtCus.Rows[0]["CustomerAddress"].ToString().TrimStart(',').Trim();
            }
            oXlsReport.Cell("**Date").Value = DateTime.Today.ToShortDateString();
            oXlsReport.Cell("**CusName").Value = dtCus.Rows[0]["CustomerName"];
            oXlsReport.Cell("**CusAddress").Value = dtCus.Rows[0]["CustomerAddress"];
            oXlsReport.Cell("**CusTelephone").Value = dtCus.Rows[0]["CustomerTelephone"];
            oXlsReport.Cell("**CusFax").Value = dtCus.Rows[0]["CustomerFax"];
            oXlsReport.Cell("**ContactPerson").Value = dtCus.Rows[0]["ContactPerson"];
            oXlsReport.Cell("**TermPayment").Value = txtTermPayment.Text.Trim();
            oXlsReport.Cell("**Note").Value = txtNote.Text.Trim();
            // End

            DataTable dtItem = ds.Tables[1];
            if (dtItem != null && dtItem.Rows.Count > 0)
            {
              for (int i = 0; i < dtItem.Rows.Count; i++)
              {
                DataRow dtRow = dtItem.Rows[i];
                if (i > 0)
                {
                  oXlsReport.Cell("B17:H17").Copy();
                  oXlsReport.RowInsert(16 + i);
                  oXlsReport.Cell("B17:H17", 0, i).Paste();
                }

                oXlsReport.Cell("**SaleCode", 0, i).Value = dtRow["SaleCode"];
                oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"];
                oXlsReport.Cell("**Description", 0, i).Value = dtRow["Description"];
                oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"];
                oXlsReport.Cell("**PackedCBM", 0, i).Value = dtRow["PackedCBM"];
                oXlsReport.Cell("**MOQPerCarcass", 0, i).Value = dtRow["MOQPerCarcass"];
                oXlsReport.Cell("**Price", 0, i).Value = dtRow["Price"];
              }
            }
            oXlsReport.Out.File(strOutFileName);
            Process.Start(strOutFileName);
          }
        }
      }
    }
    #endregion Function

    #region Event
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["ItemCode"].ValueList = ultDDSaleCode;
      e.Layout.Bands[0].Columns["Code"].ValueList = ultDDItemCode;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 120;      
      e.Layout.Bands[0].Columns["Code"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["Code"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Code"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Description"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MOQPerCarcass"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["MOQPerCarcass"].Header.Caption = "MOQ Per Carcass";
      e.Layout.Bands[0].Columns["MOQPerCarcass"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["MOQPerCarcass"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Price"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Price"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 50;
    }

    private void ultData_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().Trim();
      switch (colName)
      { 
        case "ItemCode":
          if (ultDDSaleCode.SelectedRow != null)
          {
            e.Cell.Row.Cells["Code"].Value = ultDDSaleCode.SelectedRow.Cells["ItemCode"].Value;
            e.Cell.Row.Cells["Description"].Value = ultDDSaleCode.SelectedRow.Cells["Description"].Value;
            e.Cell.Row.Cells["Unit"].Value = ultDDSaleCode.SelectedRow.Cells["Unit"].Value;
            e.Cell.Row.Cells["MOQPerCarcass"].Value = ultDDSaleCode.SelectedRow.Cells["MOQ"].Value;
            e.Cell.Row.Cells["Price"].Value = ultDDSaleCode.SelectedRow.Cells["Price"].Value;
          }
          break;
        case "Code":
          if (ultDDItemCode.SelectedRow != null)
          {
            e.Cell.Row.Cells["ItemCode"].Value = ultDDItemCode.SelectedRow.Cells["SaleCode"].Value;
            e.Cell.Row.Cells["Description"].Value = ultDDItemCode.SelectedRow.Cells["Description"].Value;
            e.Cell.Row.Cells["Unit"].Value = ultDDItemCode.SelectedRow.Cells["Unit"].Value;
            e.Cell.Row.Cells["Price"].Value = ultDDItemCode.SelectedRow.Cells["Price"].Value;
          }
          break;
        default:
          break;
      }
      ultCBCustomer.ReadOnly = (ultData.Rows.Count > 0 ? true : false);
    }

    private void ultData_BeforeCellUpdate(object sender, Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().Trim();
      switch (colName)
      { 
        case "ItemCode":
          string saleCode = e.NewValue.ToString().Trim();
          string cmdCount = string.Format("SELECT COUNT(*) FROM TblBOMItemBasic WHERE SaleCode = '{0}'", saleCode);
          int count = (int)DataBaseAccess.ExecuteScalarCommandText(cmdCount);
          if (count == 0)
          {
            WindowUtinity.ShowMessageError("ERR0011", "Code");
            e.Cancel = true;
          }
          else
          {
            DataTable dtSale = (DataTable)ultDDSaleCode.DataSource;
            DataRow[] row = dtSale.Select(string.Format("SaleCode = '{0}'", saleCode));
            if (row.Length > 0)
            {
              int index = dtSale.Rows.IndexOf(row[0]);
              ultDDSaleCode.SelectedRow = ultDDSaleCode.Rows[index];
            }
          }
          break;
        case "Code":
          string itemCode = e.NewValue.ToString().Trim();
          string cmdCountItem = string.Format("SELECT COUNT(*) FROM TblBOMItemBasic WHERE ItemCode = '{0}'", itemCode);
          int countItem = (int)DataBaseAccess.ExecuteScalarCommandText(cmdCountItem);
          if (countItem == 0)
          {
            WindowUtinity.ShowMessageError("ERR0011", "Code");
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      if (ultCBCustomer.SelectedRow != null)
      {
        long cusPid = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultCBCustomer));
        DataTable dt = (DataTable)ultData.DataSource;
        DataTable dtPrint = dt.Copy();
        if (dtPrint != null && dtPrint.Rows.Count > 0)
        {
          if (dtPrint.Columns.Contains("Code"))
          {
            dtPrint.Columns.Remove("Code");
          }
          if (dtPrint.Columns.Contains("Description"))
          {
            dtPrint.Columns.Remove("Description");
          }
          if (dtPrint.Columns.Contains("Unit"))
          {
            dtPrint.Columns.Remove("Unit");
          }
          SqlDBParameter[] input = new SqlDBParameter[2];
          input[0] = new SqlDBParameter("@CustomerPid", SqlDbType.BigInt, cusPid);
          input[1] = new SqlDBParameter("@ItemPriceList", SqlDbType.Structured, dtPrint);

          DataSet ds = SqlDataBaseAccess.SearchStoreProcedure("spRDDQuotation", input);
          dsCSDQuotation dsSource = new dsCSDQuotation();
          if (ds != null)
          {
            DataTable dtCus = ds.Tables[0];
            if (dtCus.Rows.Count > 0)
            {
              if (!dtCus.Columns.Contains("LogoPic"))
              {
                DataColumn col = new DataColumn("LogoPic", typeof(System.Byte[]));
                dtCus.Columns.Add(col);
              }
              dtCus.Rows[0]["LogoPic"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.VFRLogo_New);
              dtCus.Rows[0]["CustomerAddress"] = dtCus.Rows[0]["CustomerAddress"].ToString().TrimStart(',').Trim();
              dtCus.Rows[0]["Title"] = txtTitle.Text.Trim();
            }
            DataTable dtItem = ds.Tables[1];
            if (!dtItem.Columns.Contains("Picture"))
            {
              DataColumn col = new DataColumn("Picture", typeof(System.Byte[]));
              dtItem.Columns.Add(col);
            }
            if (dtItem.Rows.Count > 0)
            {
              foreach (DataRow rowItem in dtItem.Rows)
              {
                string imgPath = FunctionUtility.RDDGetItemImage(rowItem["ItemCode"].ToString().Trim());
                rowItem["Picture"] = FunctionUtility.ImageToByteArrayWithFormat(imgPath, 380, 1.68, "jpg");
              }
            }

            dsSource.Tables[0].Merge(dtCus);
            dsSource.Tables[1].Merge(dtItem);
          }
          cptCSDQuotation cpt = new cptCSDQuotation();
          cpt.SetDataSource(dsSource);
          cpt.SetParameterValue("TermPayment", txtTermPayment.Text.Trim());
          cpt.SetParameterValue("Note", txtNote.Text.Trim());
          ControlUtility.ViewCrystalReport(cpt);
          //View_Report report = new View_Report(cpt);
          //report.IsShowGroupTree = false;
          //report.ShowReport(ViewState.Window, FormWindowState.Maximized);
        }
      }
    }

    private void ultCBCustomer_ValueChanged(object sender, EventArgs e)
    {
      DataTable dt = (DataTable)ultData.DataSource;
      if (dt != null)
      {
        dt.Rows.Clear();
      }
      this.LoadDDSaleCode();
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      btnExport.Enabled = false;
      this.ExportExcel();
      btnExport.Enabled = true;
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = string.Empty;
      templateName = "CSD_04_016";
      string sheetName = "Sheet1";
      string outFileName = "Template Import Quotation";
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
      dialog.Title = "Select a excel file";
      txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      // List Item Error
      string itemCodeError = string.Empty;
      txtItemCodeError.Text = string.Empty;

      // Check Customer
      if (ultCBCustomer.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Customer");
        return;
      }

      // Check invalid file
      if (!File.Exists(txtLocation.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }

      DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtLocation.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B5:C200]").Tables[0];
      if (dtSource == null || dtSource.Rows.Count == 0 || string.Compare(dtSource.Columns[0].ToString(), "ItemCode") != 0 || string.Compare(dtSource.Columns[1].ToString(), "Price") != 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }

      DataTable dtItemInfo = (DataTable)ultDDItemCode.DataSource;
      DataTable dtMain = (DataTable)ultData.DataSource;
      foreach (DataRow drSource in dtSource.Rows)
      {
        string itemCode = drSource["ItemCode"].ToString().Trim();
        double price = DBConvert.ParseDouble(drSource["Price"]);
        if (itemCode.Length > 0)
        {
          if (dtItemInfo.Select(string.Format("ItemCode = '{0}'", itemCode)).Length > 0)
          {
            DataRow rowItemInfo = dtItemInfo.Select(string.Format("ItemCode = '{0}'", itemCode))[0];                
            DataRow row = dtMain.NewRow();
            row["Code"] = rowItemInfo["ItemCode"].ToString();
            row["ItemCode"] = rowItemInfo["SaleCode"].ToString();
            row["Description"] = rowItemInfo["Description"].ToString();
            row["Unit"] = rowItemInfo["Unit"].ToString();
            if (DBConvert.ParseInt(rowItemInfo["MOQ"]) > 0)
            {
              row["MOQPerCarcass"] = DBConvert.ParseInt(rowItemInfo["MOQ"]);
            }
            if (chkGetPrice.Checked)
            {
              if (price > 0)
              {
                row["Price"] = price;
              }
            }
            else if (DBConvert.ParseDouble(rowItemInfo["Price"]) > 0)
            {
              row["Price"] = DBConvert.ParseDouble(rowItemInfo["Price"]);
            }
            dtMain.Rows.Add(row);
          }
          else
          {
            itemCodeError = itemCodeError + ", " + drSource["ItemCode"].ToString();
          }
        }
      }
      if (itemCodeError.Length > 0)
      {
        txtItemCodeError.Text = itemCodeError;
      }
      // Gan DataSource
      ultData.DataSource = dtMain;
      ultCBCustomer.ReadOnly = (ultData.Rows.Count > 0 ? true : false);      
    }

    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      ultCBCustomer.ReadOnly = (ultData.Rows.Count > 0 ? true : false);
    }
    #endregion Event
  }
}
