using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared;
using VBReport;
using System.Diagnostics;
using DaiCo.Shared.Utility;
namespace DaiCo.CustomerService
{
  public partial class viewCSD_05_001 : MainUserControl
  {
    #region Init
    public viewCSD_05_001()
    {
      InitializeComponent();
      dt_EnquiryFrom.Value = DateTime.MinValue;
      dt_EnquiryTo.Value = DateTime.MinValue;
    }
    #endregion Init

    #region Load Data

    private void viewCSD_05_001_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void LoadData()
    {
      // Customer
      Shared.Utility.ControlUtility.LoadCustomer(cmbCustomer);
      // Type
      Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbType, Shared.Utility.ConstantClass.GROUP_SALEORDERTYPE);
      btnPrint.Enabled = false;
    }

    #endregion Load Data

    #region Search

    private void Search()
    {
      DBParameter[] param = new DBParameter[12];
      //No
      string text = txtNoFrom.Text.Trim();
      if (text.Length > 0)
      {
        param[0] = new DBParameter("@NoFrom", DbType.AnsiString, 20, text);
      }
      text = txtNoTo.Text.Trim();
      if (text.Length > 0)
      {
        param[1] = new DBParameter("@NoTo", DbType.AnsiString, 20, text);
      }
      // Customer's EN Po
      text = txtCusNoFrom.Text.Trim();
      if (text.Length > 0)
      {
        param[2] = new DBParameter("@CusOrderNoFrom", DbType.AnsiString, 20, text);
      }
      text = txtCusNoTo.Text.Trim();
      if (text.Length > 0)
      {
        param[3] = new DBParameter("@CusOrderNoTo", DbType.AnsiString, 20, text);
      }
      //Enquiry Date
      if (dt_EnquiryFrom.Value != DateTime.MinValue)
      {
        DateTime enquiryDate = dt_EnquiryFrom.Value;
        param[4] = new DBParameter("@OrderDateFrom", DbType.DateTime, new DateTime(enquiryDate.Year, enquiryDate.Month, enquiryDate.Day));
      }
      if (dt_EnquiryTo.Value != DateTime.MinValue)
      {
        DateTime enquiryDate = dt_EnquiryTo.Value;
        enquiryDate = (enquiryDate != DateTime.MaxValue) ? enquiryDate.AddDays(1) : enquiryDate;
        param[5] = new DBParameter("@OrderDateTo", DbType.DateTime, new DateTime(enquiryDate.Year, enquiryDate.Month, enquiryDate.Day));
      }

      //Customer
      long customerPid = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCustomer));
      if (customerPid != long.MinValue)
      {
        param[6] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      }
      //Type
      int value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbType));
      if (value != int.MinValue)
      {
        param[7] = new DBParameter("@Type", DbType.Int32, value);
      }
      //Status
      try
      {
        value = cmbStatus.SelectedIndex - 1;
      }
      catch
      {
        value = int.MinValue;
      }
      if (value >= 0)
      {
        param[8] = new DBParameter("@Status", DbType.Int32, value);
      }
      int cancel = (cmbFlag.SelectedIndex == 1) ? 1 : int.MinValue;
      if (cancel != int.MinValue)
      {
        param[9] = new DBParameter("@Cancel", DbType.Int32, cancel);
      }
      int keep = (cmbFlag.SelectedIndex == 2) ? 1 : int.MinValue;
      if (keep != int.MinValue)
      {
        param[10] = new DBParameter("@Keep", DbType.Int32, keep);
      }
      if (txtSaleCode.Text.Trim().Length > 0)
      {
        param[11] = new DBParameter("@SaleCode", DbType.AnsiString, 32, txtSaleCode.Text );
      }
      string storeName = "spCUSListEnquiry";
      DataSet dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
      DataSet dataSet = Shared.Utility.CreateDataSet.SaleOrder_Enquiry();
      dataSet.Tables["dtParent"].Merge(dtSource.Tables[0]);
      dataSet.Tables["dtChild"].Merge(dtSource.Tables[1]);
      ultCUSListEnquiry.DataSource = dataSet;
      for (int i = 0; i < ultCUSListEnquiry.Rows.Count; i++)
      {
        string status = ultCUSListEnquiry.Rows[i].Cells["Flag"].Value.ToString().Trim();
        if (status == "0")
        {
          ultCUSListEnquiry.Rows[i].CellAppearance.BackColor = Color.White;
        }
        else if (status == "1")
        {
          ultCUSListEnquiry.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
        else if (status == "2")
        {
          ultCUSListEnquiry.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        else if (status == "3")
        {
          ultCUSListEnquiry.Rows[i].CellAppearance.BackColor = Color.Pink;
        }

        int cancelFlag = DBConvert.ParseInt(ultCUSListEnquiry.Rows[i].Cells["Cancelled"].Value.ToString().Trim());
        if (cancelFlag == 1)
        {
          ultCUSListEnquiry.Rows[i].CellAppearance.BackColor = Color.DarkGray;
        }
      }
    }

    /// <summary>
    /// Export Excel
    /// </summary>
    private void ExportExcel()
    {
      if (ultCUSListEnquiry.Rows.Count > 0)
      {
        Excel.Workbook xlBook;
        UltraGrid AA = ultCUSListEnquiry;

        ControlUtility.ExportToExcelWithDefaultPath(AA, out xlBook, "Enquiry List", 7);
        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "Dai Co Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

        xlSheet.Cells[3, 1] = "Enquiry List";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
        r.Interior.Color = (object)ColorTranslator.ToOle(Color.FromArgb(144, 238, 144));

        xlBook.Application.DisplayAlerts = false;
        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }
    #endregion Search

    #region Event

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnCloseListEnquiry_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnNewEnquiry_Click(object sender, EventArgs e)
    {
      viewCSD_05_002 uc = new viewCSD_05_002();
      Shared.Utility.WindowUtinity.ShowView(uc, "ENQUIRY INFO", false, Shared.Utility.ViewState.MainWindow);
    }

    private void ultCUSListEnquiry_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultCUSListEnquiry);
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Flag"].Hidden = true;
      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[0].Columns["Cancelled"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["No"].Header.Caption = "Enquiry No";
      e.Layout.Bands[0].Columns["CusNo"].Header.Caption = "Cus' Enquiry No";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Created Date";
      e.Layout.Bands[0].Columns["OrderDate"].Header.Caption = "Order Date";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Created By";
      e.Layout.Bands[1].Columns["ParentPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[1].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[1].Columns["TotalCBM"].Header.Caption = "Total CBM";
      e.Layout.Bands[1].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["CBM"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["TotalCBM"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.CellClickAction = CellClickAction.RowSelect; 
    }


    private void ultCUSListEnquiry_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultCUSListEnquiry.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (ultCUSListEnquiry.Selected.Rows[0].ParentRow == null) ? ultCUSListEnquiry.Selected.Rows[0] : ultCUSListEnquiry.Selected.Rows[0].ParentRow;
      viewCSD_05_002 uc = new viewCSD_05_002();
      long pid = long.MinValue;
      try
      {
        pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      }
      catch { }
      int status = int.MinValue;
      try
      {
        status = DBConvert.ParseInt(row.Cells["Flag"].Value.ToString());
      }
      catch { }
      uc.enquiryPid = pid;
      uc.status = status;
      Shared.Utility.WindowUtinity.ShowView(uc, "ENQUIRY INFO", false, Shared.Utility.ViewState.MainWindow);
    }

    private void ultCUSListEnquiry_Click(object sender, EventArgs e)
    {
      string status = string.Empty;
      try
      {
        status = ultCUSListEnquiry.Selected.Rows[0].Cells["Status"].Value.ToString().Trim();
      }
      catch
      {
      }
      if (status == "New")
      {
        btnPrint.Enabled = false;
      }
      else
      {
        btnPrint.Enabled = true;
      }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtNoFrom.Text = string.Empty;
      txtNoTo.Text = string.Empty;
      txtCusNoFrom.Text = string.Empty;
      txtCusNoTo.Text = string.Empty;
      dt_EnquiryFrom.Value = DateTime.MinValue;
      dt_EnquiryTo.Value = DateTime.MinValue;
      cmbCustomer.SelectedIndex = 0;
      cmbType.SelectedIndex = 0;
      cmbStatus.SelectedIndex = 0;
      cmbFlag.SelectedIndex = 0;
      txtNoFrom.Focus();
      txtSaleCode.Text = string.Empty;
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultCUSListEnquiry, "Enquiry List");
      //DataSet ds = (DataSet)ultCUSListEnquiry.DataSource;
      //DataTable dtMain = new DataTable();
      //if (ds == null)
      //{
      //  return;
      //}
      //for (int c = 1; c < ds.Tables[0].Columns.Count - 1; c++)
      //{
      //  dtMain.Columns.Add(ds.Tables[0].Columns[c].ColumnName, ds.Tables[0].Columns[c].DataType);
      //}
      //for (int c = 1; c < ds.Tables[1].Columns.Count; c++)
      //{
      //  dtMain.Columns.Add(ds.Tables[1].Columns[c].ColumnName + "_Detail", ds.Tables[1].Columns[c].DataType);
      //}

      //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
      //{
      //  if (!ultCUSListEnquiry.Rows[i].IsFilteredOut)
      //  {
      //    DataRow dr = dtMain.NewRow();
      //    for (int c = 1; c < ds.Tables[0].Columns.Count - 1; c++)
      //    {
      //      dr[ds.Tables[0].Columns[c].ColumnName] = ds.Tables[0].Rows[i][ds.Tables[0].Columns[c].ColumnName];
      //    }
      //    dtMain.Rows.Add(dr);
      //    for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
      //    {
      //      if (ds.Tables[1].Rows[j]["ParentPid"].ToString() == ds.Tables[0].Rows[i]["Pid"].ToString())
      //      {
      //        DataRow drChild = dtMain.NewRow();
      //        for (int c = 1; c < ds.Tables[1].Columns.Count; c++)
      //        {
      //          drChild[ds.Tables[1].Columns[c].ColumnName + "_Detail"] = ds.Tables[1].Rows[j][ds.Tables[1].Columns[c].ColumnName];
      //        }
      //        dtMain.Rows.Add(drChild);
      //        //break;
      //      }
      //    }
      //  }
      //}
      //if (dtMain.Rows.Count > 0 && dtMain != null)
      //{
      //  string strTemplateName = "EnquiryCustomerServiceTemplate";
      //  string strSheetName = "Sheet1";
      //  string strOutFileName = "Enquiry Template";
      //  string strStartupPath = System.Windows.Forms.Application.StartupPath;
      //  string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      //  string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      //  XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //  for (int i = 0; i < dtMain.Rows.Count; i++)
      //  {
      //    DataRow dtRow = dtMain.Rows[i];
      //    if (i > 0)
      //    {
      //      oXlsReport.Cell("B3:R3").Copy();
      //      oXlsReport.RowInsert(2 + i);
      //      oXlsReport.Cell("B3:R3", 0, i).Paste();
      //    }
      //    oXlsReport.Cell("**stt", 0, i).Value = i + 1;
      //    oXlsReport.Cell("**1", 0, i).Value = dtRow["No"].ToString();
      //    oXlsReport.Cell("**2", 0, i).Value = dtRow["OrderDate"].ToString();
      //    oXlsReport.Cell("**3", 0, i).Value = dtRow["CusNo"].ToString();
      //    oXlsReport.Cell("**4", 0, i).Value = dtRow["Customer"].ToString();
      //    oXlsReport.Cell("**5", 0, i).Value = dtRow["Direct"].ToString();
      //    oXlsReport.Cell("**6", 0, i).Value = dtRow["Type"].ToString();
      //    oXlsReport.Cell("**7", 0, i).Value = dtRow["Status"].ToString();
      //    oXlsReport.Cell("**8", 0, i).Value = dtRow["CreateDate"].ToString();
      //    oXlsReport.Cell("**9", 0, i).Value = dtRow["CreateBy"].ToString();
      //    oXlsReport.Cell("**10", 0, i).Value = dtRow["Remark"].ToString();
      //    oXlsReport.Cell("**11", 0, i).Value = dtRow["SaleCode_Detail"].ToString();
      //    oXlsReport.Cell("**12", 0, i).Value = dtRow["ItemCode_Detail"].ToString();
      //    oXlsReport.Cell("**13", 0, i).Value = dtRow["Revision_Detail"].ToString();
      //    oXlsReport.Cell("**14", 0, i).Value = dtRow["Name_Detail"].ToString();
      //    oXlsReport.Cell("**15", 0, i).Value = dtRow["Qty_Detail"].ToString();
      //    oXlsReport.Cell("**16", 0, i).Value = dtRow["CBM_Detail"].ToString();
      //    oXlsReport.Cell("**17", 0, i).Value = dtRow["TotalCBM_Detail"].ToString();
      //  }
      //  oXlsReport.Out.File(strOutFileName);
      //  Process.Start(strOutFileName);
      //}
    }

    private void txtNoFrom_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtNoTo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtCusNoFrom_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtCusNoTo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void dt_EnquiryFrom_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void dt_EnquiryTo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cmbCustomer_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cmbStatus_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cmbType_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cmbFlag_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtSaleCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void btnExportSummary_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }
    #endregion Event
  }
}
