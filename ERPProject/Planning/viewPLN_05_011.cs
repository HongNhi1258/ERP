using DaiCo.Application;
using DaiCo.Shared;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;


namespace DaiCo.ERPProject
{
  public partial class viewPLN_05_011 : MainUserControl
  {
    private bool resize = false;
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    public viewPLN_05_011()
    {
      InitializeComponent();
      dt_POCancelDateFrom.Value = DateTime.MinValue;
      dt_POCancelDateTo.Value = DateTime.MinValue;
      ultReceivedDateFrom.Value = DBNull.Value;
      ultReceivedDateTo.Value = DBNull.Value;
    }

    private void viewPLN_05_011_Load(object sender, EventArgs e)
    {
      this.SetAutoSearchWhenPressEnter(gpbSearch);
      this.LoadData();
    }

    #region LoadData

    private void LoadData()
    {
      // Customer
      Utility.LoadCustomer(ucbCustomer);

      // Create By
      Utility.LoadEmployee(cmbCreateBy, "CSD");
      //btnPrint.Enabled = false;
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
    private void Search()
    {
      DBParameter[] param = new DBParameter[15];
      // No
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
      text = txtNoSet.Text.Trim();
      string[] listNo = text.Split(',');
      text = string.Empty;
      foreach (string no in listNo)
      {
        if (no.Trim().Length > 0)
        {
          text = string.Format(@"{0}, '{1}'", text, no);
        }
      }
      if (text.Length > 0)
      {
        text = string.Format("({0})", text.Substring(1, text.Length - 1));
        param[2] = new DBParameter("@NoSet", DbType.AnsiString, 1000, text);
      }

      // Customer's PO No
      text = txtCusNoFrom.Text.Trim();
      if (text.Length > 0)
      {
        param[3] = new DBParameter("@CusCancelNoFrom", DbType.AnsiString, 20, text);
      }
      text = txtCusNoTo.Text.Trim();
      if (text.Length > 0)
      {
        param[4] = new DBParameter("@CusCancelNoTo", DbType.AnsiString, 20, text);
      }
      text = txtCusNoSet.Text.Trim();

      listNo = text.Split(',');
      text = string.Empty;
      foreach (string no in listNo)
      {
        if (no.Trim().Length > 0)
        {
          text = string.Format(@"{0}, '{1}'", text, no);
        }
      }
      if (text.Length > 0)
      {
        text = string.Format("({0})", text.Substring(1, text.Length - 1));
        param[5] = new DBParameter("@CusCancelNoSet", DbType.AnsiString, 1000, text);
      }

      //Cancel Date
      if (dt_POCancelDateFrom.Value != DateTime.MinValue)
      {
        DateTime cancelDate = dt_POCancelDateFrom.Value;
        param[6] = new DBParameter("@CancelDateFrom", DbType.DateTime, new DateTime(cancelDate.Year, cancelDate.Month, cancelDate.Day));
      }

      if (dt_POCancelDateTo.Value != DateTime.MinValue)
      {
        DateTime cancelDate = dt_POCancelDateTo.Value;
        cancelDate = (cancelDate != DateTime.MaxValue) ? cancelDate.AddDays(1) : cancelDate;
        param[7] = new DBParameter("@CancelDateTo", DbType.DateTime, new DateTime(cancelDate.Year, cancelDate.Month, cancelDate.Day));
      }

      // Customer
      long customerPid = DBConvert.ParseLong(ucbCustomer.Value);
      if (customerPid != long.MinValue)
      {
        param[8] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      }

      int value = int.MinValue;
      // Status
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
        param[9] = new DBParameter("@Status", DbType.Int32, value);
      }

      // ItemCode
      text = txtItemCode.Text.Trim();
      if (text.Length > 0)
      {
        param[10] = new DBParameter("@ItemCode", DbType.AnsiString, 18, "%" + text + "%");
      }

      // Create By
      value = DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbCreateBy));
      if (value != int.MinValue)
      {
        param[11] = new DBParameter("@CreateBy", DbType.Int32, value);
      }
      text = txtSaleCode.Text.Trim();
      if (text.Length > 0)
      {
        param[12] = new DBParameter("@SaleCode", DbType.AnsiString, 18, "%" + text + "%");
      }

      if (ultReceivedDateFrom.Value != null)
      {
        DateTime receivedFrom =
            DBConvert.ParseDateTime(ultReceivedDateFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        param[13] = new DBParameter("@ReceivePLADateFrom", DbType.DateTime, receivedFrom);
      }

      if (ultReceivedDateTo.Value != null)
      {
        DateTime receivedTo =
            DBConvert.ParseDateTime(ultReceivedDateTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        receivedTo = receivedTo.AddDays(1);
        param[14] = new DBParameter("@ReceivePLADateTo", DbType.DateTime, receivedTo);
      }
      string storeName = "spCUSSaleOrderCancelAutoFill_List";

      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
      //dsCUSSaleOrderCancel dsData = new dsCUSSaleOrderCancel();
      DataSet dsData = Shared.Utility.CreateDataSet.SaleOrderCancel();
      dsData.Tables["dtParent"].Merge(dsSource.Tables[0]);
      dsData.Tables["dtChild"].Merge(dsSource.Tables[1]);
      dsData.Tables["dtChildSwap"].Merge(dsSource.Tables[2]);

      ultData.DataSource = dsData;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string status = ultData.Rows[i].Cells["ST"].Value.ToString().Trim();
        if (status == "0")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.White;
        }
        else if (status == "1")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
        else if (status == "2")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Cyan;
        }
        else if (status == "3")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        else if (status == "4")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Pink;
        }
      }
    }
    private DataTable CreateData()
    {
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("PoNoCancelNo", typeof(System.String));
      taParent.Columns.Add("CancelDate", typeof(System.String));
      taParent.Columns.Add("CusPoCancelNo", typeof(System.String));
      taParent.Columns.Add("Customer", typeof(System.String));
      taParent.Columns.Add("SaleNo", typeof(System.String));
      taParent.Columns.Add("CustomerPONo", typeof(System.String));
      taParent.Columns.Add("SaleCode", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("Name", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.Int32));
      taParent.Columns.Add("CBM", typeof(System.Double));
      taParent.Columns.Add("TotalCBM", typeof(System.Double));
      return taParent;
    }
    #endregion LoadData

    #region Event

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPLN_05_009 uc = new viewPLN_05_009();
      uc.customerPid = 1;
      uc.btnSave.Enabled = true;
      uc.btnSave.Visible = true;
      uc.btnSaveConfirm.Visible = false;
      uc.chkConfirm.Visible = true;
      uc.chkPLNcomfirm.Visible = false;
      uc.btnPrint.Visible = false;
      uc.btnPrintPDF.Visible = false;
      // Visible cac textbox PLN khong nhap
      uc.label4.Visible = false;
      uc.txtContract.Visible = false;
      uc.label1.Visible = false;
      uc.txtCustomerPoCancelNo.Visible = false;
      uc.label9.Visible = false;
      uc.chkDirect.Visible = false;
      uc.label10.Visible = false;
      uc.cmbDirectCustomer.Visible = false;
      uc.label11.Visible = false;
      uc.txtRef.Visible = false;
      uc.label6.Visible = false;
      uc.txtRemark.Visible = false;
      uc.dtp_date.Value = DateTime.Today;
      Shared.Utility.WindowUtinity.ShowView(uc, "INSERT SALE ORDER CANCEL INFO", false, Shared.Utility.ViewState.MainWindow);
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Utility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["No"].Header.Caption = "PO Cancel No";
      e.Layout.Bands[0].Columns["CusCancelNo"].Header.Caption = "Cus's PO Cancel No";
      e.Layout.Bands[0].Columns["CancelDate"].Header.Caption = "Cancel Date";
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["ParentPid"].Hidden = true;
      e.Layout.Bands[2].Columns["ParentDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[0].Columns["ST"].Hidden = true;
      e.Layout.Bands["dtParent_dtChild"].Columns["ParentPid"].Hidden = true;
      e.Layout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
      if (!this.resize)
      {
        e.Layout.Bands[0].Columns["CusCancelNo"].Width = 110;
        this.resize = true;
      }

      e.Layout.Bands[1].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[1].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[1].Columns["TotalCBM"].Header.Caption = "Total CBM";
      e.Layout.Bands[1].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["CBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["TotalCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[2].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[2].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[2].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      for (int i = 0; i < ultData.DisplayLayout.Bands[1].Columns.Count; i++)
      {
        ultData.DisplayLayout.Bands[1].Columns[i].CellAppearance.BackColor = Color.Wheat;
      }
      for (int j = 0; j < ultData.DisplayLayout.Bands[2].Columns.Count; j++)
      {
        ultData.DisplayLayout.Bands[2].Columns[j].CellAppearance.BackColor = Color.LightBlue;
      }
    }

    private void ultData_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {
        string value = string.Empty;
        try
        {
          value = ultData.Selected.Rows[0].Cells["Status"].Value.ToString().Trim();
        }
        catch { }
        //if (value == "Non Locked")
        //{
        //  btnPrint.Enabled = false;
        //}
        //else
        //{
        //  btnPrint.Enabled = true;
        //}
      }
    }

    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultData.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row;
      if (ultData.Selected.Rows[0].HasChild() == true)
      {
        row = (ultData.Selected.Rows[0].ParentRow == null) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ParentRow;
      }
      else
      {
        row = (ultData.Selected.Rows[0].ParentRow.ParentRow == null) ? ultData.Selected.Rows[0].ParentRow : ultData.Selected.Rows[0].ParentRow.ParentRow;
      }
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());

      viewPLN_05_009 uc = new viewPLN_05_009();
      uc.poCancelPid = pid;
      Shared.Utility.WindowUtinity.ShowView(uc, "UPDATE SALE ORDER CANCEL INFO", false, Shared.Utility.ViewState.MainWindow);
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtNoFrom.Text = string.Empty;
      txtNoTo.Text = string.Empty;
      txtNoSet.Text = string.Empty;
      txtCusNoFrom.Text = string.Empty;
      txtCusNoTo.Text = string.Empty;
      txtCusNoSet.Text = string.Empty;
      dt_POCancelDateFrom.Value = DateTime.MinValue;
      dt_POCancelDateTo.Value = DateTime.MinValue;
      ucbCustomer.Value = DBNull.Value;
      cmbStatus.SelectedIndex = -1;
      cmbCreateBy.SelectedIndex = -1;
      txtItemCode.Text = string.Empty;
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;
        DataTable dtExport = this.CreateData();
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow ur = ultData.Rows[i];
          for (int j = 0; j < ur.ChildBands[0].Rows.Count; j++)
          {
            DataRow dr = dtExport.NewRow();

            dr["PoNoCancelNo"] = ur.Cells["No"].Value.ToString();
            dr["CancelDate"] = ur.Cells["CancelDate"].Value.ToString();
            dr["CusPoCancelNo"] = ur.Cells["CusCancelNo"].Value.ToString();
            dr["Customer"] = ur.Cells["Customer"].Value.ToString();
            UltraGridRow urc = ur.ChildBands[0].Rows[j];
            dr["SaleNo"] = urc.Cells["SaleNo"].Value.ToString();
            dr["CustomerPONo"] = urc.Cells["CustomerPONo"].Value.ToString();
            dr["SaleCode"] = urc.Cells["SaleCode"].Value.ToString();
            dr["ItemCode"] = urc.Cells["ItemCode"].Value.ToString();
            dr["Revision"] = urc.Cells["Revision"].Value.ToString();
            dr["Name"] = urc.Cells["Name"].Value.ToString();
            dr["Qty"] = urc.Cells["Qty"].Value.ToString();
            if (DBConvert.ParseDouble(urc.Cells["CBM"].Value.ToString()) > 0)
            {
              dr["CBM"] = DBConvert.ParseDouble(urc.Cells["CBM"].Value.ToString());
            }
            if (DBConvert.ParseDouble(urc.Cells["TotalCBM"].Value.ToString()) > 0)
            {
              dr["TotalCBM"] = DBConvert.ParseDouble(urc.Cells["TotalCBM"].Value.ToString());
            }
            dtExport.Rows.Add(dr);
          }
        }
        UltraGrid UG = new UltraGrid();
        UG.DataSource = dtExport;
        Controls.Add(UG);
        Utility.ExportToExcelWithDefaultPath(UG, out xlBook, "Report Cancel List", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "Dai Co Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

        xlSheet.Cells[3, 1] = "Report Cancel List";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        //xlBook.Application.DisplayAlerts = false;

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }

    }
    #endregion Event




  }
}
