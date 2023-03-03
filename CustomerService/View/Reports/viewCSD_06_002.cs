/*
  Author      : Nguyen Van Tron
  Date        : 10/05/2012
  Description : Report for Customer Service
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Application;
using VBReport;
using System.Collections;
using System.IO;
using System.Diagnostics;
using Infragistics.Win;
using System.Data.OleDb;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Application.Web.Mail;


namespace DaiCo.CustomerService
{
  public partial class viewCSD_06_002 : MainUserControl
  {
    #region fields
    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;
    #endregion fields

    #region functions
    private void LoadDataReports()
    {
      DataTable dtReports = new DataTable();
      dtReports.Columns.Add("value", typeof(System.Int32));
      dtReports.Columns.Add("text", typeof(System.String));
      DataRow row1 = dtReports.NewRow();
      row1["value"] = 1;
      row1["text"] = "Information needed to do paperwork for DVCN";
      dtReports.Rows.Add(row1);
      DataRow row2 = dtReports.NewRow();
      row2["value"] = 2;
      row2["text"] = "Item PackageReport";
      dtReports.Rows.Add(row2);
      DataRow row3 = dtReports.NewRow();
      row3["value"] = 3;
      row3["text"] = "SO Report";
      dtReports.Rows.Add(row3);
      ControlUtility.LoadUltraCombo(ultraCBReport, dtReports, "value", "text", "value");
    }

    private void LoadSaleCodeFromAndTo()
    {
      string commandText = "Select Distinct SaleCode From TblBOMItemBasic Where len(SaleCode) > 0 Order By SaleCode ASC";
      DataTable dtSaleCodeFrom = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBSaleCodeFrom.DataSource = dtSaleCodeFrom;
      DataTable dtSaleCodeTo = FunctionUtility.CloneTable(dtSaleCodeFrom);
      ultraCBSaleCodeTo.DataSource = dtSaleCodeTo;
    }

    private void LoadCustomer()
    {
      string commandText = "SELECT Pid, CustomerCode, Name FROM TblCSDCustomerInfo ORDER BY CustomerCode";
      DataTable dtCustomet = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBCustomer.DataSource = dtCustomet;
      ultCBCustomer.ValueMember = "Pid";
      ultCBCustomer.DisplayMember = "CustomerCode";
      ultCBCustomer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    private bool CheckInvalid()
    {
      if (ultraCBReport.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0125", "Report");
        return false;
      }

      int reportValue = DBConvert.ParseInt(ultraCBReport.Value.ToString());
      switch (reportValue)
      {
        case 1:
          if (ultraCBSaleCodeFrom.Value == null && ultraCBSaleCodeFrom.Text.Trim().Length > 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Sale Code From");
            return false;
          }
          if (ultraCBSaleCodeTo.Value == null && ultraCBSaleCodeTo.Text.Trim().Length > 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Sale Code To");
            return false;
          }
          break;
        default:
          break;
      }
      return true;
    }
    /// <summary>
    /// Item Kind
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExport_Click(object sender, EventArgs e)
    {
      string strTemplateName = "ListItemKind";
      string strSheetName = "ItemKind";
      string strOutFileName = "ItemKind";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = @"\\fsdc01\\public\\Reports\\ItemKind";
      string strPathTemplate = string.Format(@"{0}\ExcelTemplate\CustomerService", strStartupPath);
      //XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spCSDListInfoItemKind");
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow drRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B6:AI6").Copy();
            oXlsReport.RowInsert(5 + i);
            oXlsReport.Cell("B6:AI6", 0, i).Paste();
          }
          oXlsReport.Cell("**1", 0, i).Value = drRow["SaleCode"];
          oXlsReport.Cell("**2", 0, i).Value = drRow["VNStock"];
          //US
          oXlsReport.Cell("**3", 0, i).Value = drRow["USStock"];
          oXlsReport.Cell("**4", 0, i).Value = drRow["PendingOrderUS"];
          oXlsReport.Cell("**5", 0, i).Value = drRow["USOldKindSymbol"];
          oXlsReport.Cell("**6", 0, i).Value = drRow["USNewKindSymbol"];
          //UK
          oXlsReport.Cell("**7", 0, i).Value = drRow["UKStock"];
          oXlsReport.Cell("**8", 0, i).Value = drRow["PendingOrderUK"];
          oXlsReport.Cell("**9", 0, i).Value = drRow["UKOldKindSymbol"];
          oXlsReport.Cell("**10", 0, i).Value = drRow["UKNewKindSymbol"];
          //IT
          oXlsReport.Cell("**11", 0, i).Value = 0;
          oXlsReport.Cell("**12", 0, i).Value = drRow["PendingOrderIT"];
          oXlsReport.Cell("**13", 0, i).Value = drRow["ITOldKindSymbol"];
          oXlsReport.Cell("**14", 0, i).Value = drRow["ITNewKindSymbol"];
          //ME
          oXlsReport.Cell("**15", 0, i).Value = 0;
          oXlsReport.Cell("**16", 0, i).Value = drRow["PendingOrderME"];
          oXlsReport.Cell("**17", 0, i).Value = drRow["MEOldKindSymbol"];
          oXlsReport.Cell("**18", 0, i).Value = drRow["MENewKindSymbol"];
          //RU
          oXlsReport.Cell("**19", 0, i).Value = 0;
          oXlsReport.Cell("**20", 0, i).Value = drRow["PendingOrderRU"];
          oXlsReport.Cell("**21", 0, i).Value = drRow["RUOldKindSymbol"];
          oXlsReport.Cell("**22", 0, i).Value = drRow["RUNewKindSymbol"];
          //AU
          oXlsReport.Cell("**23", 0, i).Value = 0;
          oXlsReport.Cell("**24", 0, i).Value = drRow["PendingOrderAU"];
          oXlsReport.Cell("**25", 0, i).Value = drRow["AUOldKindSymbol"];
          oXlsReport.Cell("**26", 0, i).Value = drRow["AUNewKindSymbol"];
          //CN
          oXlsReport.Cell("**27", 0, i).Value = 0;
          oXlsReport.Cell("**28", 0, i).Value = drRow["PendingOrderCN"];
          oXlsReport.Cell("**29", 0, i).Value = drRow["CNOldKindSymbol"];
          oXlsReport.Cell("**30", 0, i).Value = drRow["CNNewKindSymbol"];
          //EU
          oXlsReport.Cell("**31", 0, i).Value = 0;
          oXlsReport.Cell("**32", 0, i).Value = drRow["PendingOrderEU"];
          oXlsReport.Cell("**33", 0, i).Value = drRow["EUOldKindSymbol"];
          oXlsReport.Cell("**34", 0, i).Value = drRow["EUNewKindSymbol"];
        }
      }
      oXlsReport.Out.File(strOutFileName);
      //Send mail
      MailMessage mailMessage = new MailMessage();
      string body = string.Empty;
      body += "<p><i><font color='6699CC'>";
      body += "Dear all,<br><br>";
      body += "We would like to inform you the item kind is changed as detail in table below due to factory stock is changed.<br>";
      body += "Please update item kind to the website & inform Customer Service team, ";
      body += "Sales team after finih update website.<br><br> ";
      body += "This is automatically email generated from our system. Please do not reply <br>";
      body += "Thanks and kind regards,<br>";
      body += "Jonathan Charles Team ";
      body += "</font></i></p> ";

      string mailTo = "bang_it@vfr.net.vn, minh_it@vfr.net.vn";
      //string commandText = string.Format(@"SELECT email, HoNV + TenNV AS Name FROM VHRNhanVien WHERE ID_NhanVien = {0} AND  Resigned = 0", SharedObject.UserInfo.UserPid);
      //DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      //if (dt != null && dt.Rows.Count > 0)
      //{
      //  mailTo = dt.Rows[0]["email"].ToString();
      //}
      mailMessage.To = mailTo;
      mailMessage.Subject = "LIST ITEM KIND";
      mailMessage.Body = body;
      IList attachments = new ArrayList();

      attachments.Add(strOutFileName);
      mailMessage.Attachfile = attachments;
      mailMessage.SendMail(true);

      WindowUtinity.ShowMessageSuccess("MSG0052");
      this.CloseTab();
      //Process.Start(strOutFileName);
    }
    private void ShowPaperworkInfoForDVCN()
    {
      string strTemplateName = "CSDReports";
      string strSheetName = "Paperwork";
      string strOutFileName = "Paperwork";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate\CustomerService";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DBParameter[] inputParam = new DBParameter[2];
      if (ultraCBSaleCodeFrom.Value != null)
      {
        inputParam[0] = new DBParameter("@SaleCodeFrom", DbType.AnsiString, 16, ultraCBSaleCodeFrom.Value);
      }
      if (ultraCBSaleCodeTo.Value != null)
      {
        inputParam[1] = new DBParameter("@SaleCodeTo", DbType.AnsiString, 16, ultraCBSaleCodeTo.Value);
      }
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spBOMPaperworkInformationForDVCN", inputParam);
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow drRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B7:P7").Copy();
            oXlsReport.RowInsert(6 + i);
            oXlsReport.Cell("B7:P7", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**SaleCode", 0, i).Value = drRow["SaleCode"];
          oXlsReport.Cell("**ItemCode", 0, i).Value = drRow["ItemCode"];
          oXlsReport.Cell("**OldCode", 0, i).Value = drRow["OldCode"];
          oXlsReport.Cell("**ItemName", 0, i).Value = drRow["ItemName"];
          oXlsReport.Cell("**MainMaterial", 0, i).Value = drRow["MainMaterial"];
          oXlsReport.Cell("**Veneer", 0, i).Value = drRow["Veneer"];
          oXlsReport.Cell("**Hardware", 0, i).Value = drRow["Hardware"];
          oXlsReport.Cell("**Leather", 0, i).Value = drRow["Leather"];
          oXlsReport.Cell("**Fabric", 0, i).Value = drRow["Fabric"];
          oXlsReport.Cell("**OtherMaterials", 0, i).Value = drRow["OtherMaterials"];
          oXlsReport.Cell("**Dimensions(cm)", 0, i).Value = drRow["Dimensions(cm)"];
          oXlsReport.Cell("**QtyPerBox", 0, i).Value = drRow["QtyPerBox"];
          oXlsReport.Cell("**Room", 0, i).Value = drRow["Room"];
          oXlsReport.Cell("**CBM", 0, i).Value = drRow["CBM"];
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Create DataTable
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("WO", typeof(System.Int32));
      dt.Columns.Add("CarcassCode", typeof(System.String));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("SaleCode", typeof(System.String));
      return dt;
    }

    /// <summary>
    /// SaleOrder Report
    /// </summary>
    private void SaleOrderReport()
    {
      SqlDBParameter[] input = new SqlDBParameter[4];
      // Customer
      if (ultCBCustomer.Value != null)
      {
        input[0] = new SqlDBParameter("@CustomerPid", SqlDbType.BigInt, DBConvert.ParseLong(ultCBCustomer.Value));
      }
      
      // ItemCode
      if(txtItemCode.Text.Trim().Length > 0)
      {
        input[1] = new SqlDBParameter("@ItemCode", SqlDbType.Text, txtItemCode.Text);
      }

      // SaleCode
      if(txtSaleCode.Text.Trim().Length > 0)
      {
        input[2] = new SqlDBParameter("@SaleCode", SqlDbType.Text, txtSaleCode.Text);
      }
      if (txtLocation.Text.Length > 0)
      {
        DataTable dtSalecode = new DataTable();
        dtSalecode = FunctionUtility.GetExcelToDataSet(txtLocation.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B3:B504]").Tables[0];
        if (dtSalecode != null)
        {
          DataTable dt = this.CreateDataTable();
          for (int i = 0; i < dtSalecode.Rows.Count; i++)
          {  
            DataRow row = dt.NewRow();
            if (dtSalecode.Rows[i]["SaleCode"].ToString().Length > 0)
            {
              row["SaleCode"] = dtSalecode.Rows[i]["SaleCode"];
              dt.Rows.Add(row);
            }
          }
          input[3] = new SqlDBParameter("@StringDataSaleCode", SqlDbType.Structured, dt);
        }
      }
      
      // Result
      DataTable dtResult = SqlDataBaseAccess.SearchStoreProcedureDataTable("spCSDRPTSOInformation_Report", input);
      if(dtResult != null)
      {
        ultData.DataSource = dtResult;
        // Export Excel
        this.ExportExcel();
      }
    }

    /// <summary>
    /// Export Excel
    /// </summary>
    private void ExportExcel()
    {
      Excel.Workbook xlBook;
      ControlUtility.ExportToExcelWithDefaultPath(ultData, out xlBook, "SO Information", 6);

      string filePath = xlBook.FullName;
      Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

      xlSheet.Cells[1, 1] = "Dai Co Co., LTD";
      Excel.Range r = xlSheet.get_Range("A1", "A1");

      xlSheet.Cells[2, 1] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

      xlSheet.Cells[3, 1] = "SO Information";
      r = xlSheet.get_Range("A3", "A3");
      r.Font.Bold = true;
      r.Font.Size = 14;
      r.EntireRow.RowHeight = 20;

      xlSheet.Cells[4, 1] = "Date: ";
      r.Font.Bold = true;
      xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

      xlBook.Application.DisplayAlerts = false;
      xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
      Process.Start(filePath);
    }
    #endregion functions

    #region event
    public viewCSD_06_002()
    {
      InitializeComponent();
    }

    private void viewCSD_06_002_Load(object sender, EventArgs e)
    {
      tableLayoutSaleOrder.Visible = false;
      this.LoadDataReports();
      this.LoadSaleCodeFromAndTo();
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        int reportValue = DBConvert.ParseInt(ultraCBReport.Value.ToString());
        switch (reportValue)
        {
          case 1:
            this.ShowPaperworkInfoForDVCN();
            break;
          case 2:
            System.Diagnostics.Process.Start("IExplore", "http://dcsql/Reports/Pages/Report.aspx?ItemPath=%2fERP%2fItem_PackageReport");
            break;
          case 3:
            this.SaleOrderReport();
            break;
        }
      }
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "CSD_06_002";
      string sheetName = "Sheet1";
      string outFileName = "Template Import SaleCode";
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
      txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void ultraCBReport_ValueChanged(object sender, EventArgs e)
    {
      if(ultraCBReport.Value != null)
      {
        int value = DBConvert.ParseInt(ultraCBReport.Value);
        if (value == 3)
        {
          tableLayoutSaleOrder.Visible = true;
          tableLayoutPanel5.Visible = false;
          this.LoadCustomer();
        }
        else
        {
          tableLayoutSaleOrder.Visible = false;
          tableLayoutPanel5.Visible = true;
        }
      }
    }

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["PONo"].Header.Caption = "PO#";
      e.Layout.Bands[0].Columns["OrderDate"].Header.Caption = "PO Date";
      e.Layout.Bands[0].Columns["CustomerCode"].Header.Caption = "Cust.";
      e.Layout.Bands[0].Columns["SpecialInstruction"].Header.Caption = "Special Remarks";
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Header.Caption = "Original Confirmed ShipDate";
      e.Layout.Bands[0].Columns["CurrentScheduleDelivery"].Header.Caption = "Present Shipping Date";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    #endregion event
  }
}
