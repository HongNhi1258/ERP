using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewCSD_05_001 : MainUserControl
  {
    #region Init
    public viewCSD_05_001()
    {
      InitializeComponent();
      udtEnquiryFromDate.Value = null;
      udtEnquiryToDate.Value = null;
    }
    #endregion Init

    #region Load Data
    /// <summary>
    /// Set Auto Search Data When User Press Enter
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
    }
    private void viewCSD_05_001_Load(object sender, EventArgs e)
    {
      this.SetAutoSearchWhenPressEnter(gpbSearch);
      this.LoadData();
    }
    private void LoadData()
    {
      // Customer
      this.LoadCustomer();
      // Type
      Utility.LoadComboboxCodeMst(cmbType, Shared.Utility.ConstantClass.GROUP_SALEORDERTYPE);
      btnPrint.Enabled = false;
    }

    private void LoadCustomer()
    {
      string commandText = string.Format(@"SELECT Pid, CustomerCode + ' - ' + Name Customer FROM TblCSDCustomerInfo WHERE DeletedFlg = 0 AND ParentPid IS NULL ORDER BY CustomerCode");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbCustomer, dtSource, "Pid", "Customer", "Pid");
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
      if (udtEnquiryFromDate.Value != null)
      {
        param[4] = new DBParameter("@OrderDateFrom", DbType.DateTime, DBConvert.ParseDateTime(udtEnquiryFromDate.Value));
      }
      if (udtEnquiryToDate.Value != null)
      {
        param[5] = new DBParameter("@OrderDateTo", DbType.DateTime, DBConvert.ParseDateTime(udtEnquiryToDate.Value));
      }

      //Customer
      long customerPid = DBConvert.ParseLong(ucbCustomer.Value);
      if (customerPid != long.MinValue)
      {
        param[6] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      }
      //Type
      int value = DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbType));
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
        param[11] = new DBParameter("@SaleCode", DbType.AnsiString, 32, txtSaleCode.Text);
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

        Utility.ExportToExcelWithDefaultPath(AA, out xlBook, "Enquiry List", 7);
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
      Utility.SetPropertiesUltraGrid(ultCUSListEnquiry);
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
      udtEnquiryFromDate.Value = DateTime.MinValue;
      udtEnquiryToDate.Value = DateTime.MinValue;
      ucbCustomer.Value = DBNull.Value;
      cmbType.SelectedIndex = 0;
      cmbStatus.SelectedIndex = 0;
      cmbFlag.SelectedIndex = 0;
      txtNoFrom.Focus();
      txtSaleCode.Text = string.Empty;
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultCUSListEnquiry, "Enquiry List");
    }

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }

    private void btnExportSummary_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }
    #endregion Event
  }
}
