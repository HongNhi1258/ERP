using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewCSD_05_003 : MainUserControl
  {
    public viewCSD_05_003()
    {
      InitializeComponent();
      udtOrderDateFrom.Value = null;
      udtOrderDateTo.Value = null;
    }
    void viewCSD_05_003_Load(object sender, System.EventArgs e)
    {
      this.SetAutoSearchWhenPressEnter(ultraExpandableGroupBox1);
      this.LoadData();
    }

    #region LoadData

    private void LoadData()
    {
      // Customer
      this.LoadCustomer();

      // Type
      Utility.LoadUltraComboCodeMst(ucbType, Shared.Utility.ConstantClass.GROUP_SALEORDERTYPE);

      //Status
      this.LoadStatus();

      // Create By
      this.LoadEmployee();
      btnPrint.Enabled = false;
    }

    private void LoadEmployee()
    {
      string commandText = string.Format(@"SELECT DISTINCT CR.Pid, CAST(CR.Pid as varchar) + ' - ' + CR.EmpName EmpName 
                                            FROM TblPLNSaleOrder SO
                                            INNER JOIN VHRMEmployee CR ON SO.CreateBy = CR.Pid
                                            ORDER BY CR.Pid");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbCreateBy, dtSource, "Pid", "EmpName", false, "Pid");
    }
    private void LoadCustomer()
    {
      string commandText = string.Format(@"SELECT Pid, CustomerCode + ' - ' + Name Customer FROM TblCSDCustomerInfo WHERE DeletedFlg = 0 AND ParentPid IS NULL ORDER BY CustomerCode");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbCustomer, dtSource, "Pid", "Customer", "Pid");
    }
    private void LoadStatus()
    {
      DataTable dtStatus = new DataTable();
      dtStatus.Columns.Add("Value", typeof(System.Int16));
      dtStatus.Columns.Add("Text", typeof(System.String));
      DataRow row1 = dtStatus.NewRow();
      row1["Value"] = 0;
      row1["Text"] = "New";
      dtStatus.Rows.Add(row1);
      DataRow row2 = dtStatus.NewRow();
      row2["Value"] = 2;
      row2["Text"] = "CS Confirm";
      dtStatus.Rows.Add(row2);
      DataRow row3 = dtStatus.NewRow();
      row3["Value"] = 3;
      row3["Text"] = "PLN Confirm";
      dtStatus.Rows.Add(row3);
      Utility.LoadUltraCombo(ucbStatus, dtStatus, "Value", "Text", true);
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
      DBParameter[] param = new DBParameter[11];

      // Sale No
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

      // Customer's EN No
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

      //Order Date
      DateTime orderDateFrom = DBConvert.ParseDateTime(udtOrderDateFrom.Value);
      if (orderDateFrom != DateTime.MinValue)
      {
        param[4] = new DBParameter("@OrderDateFrom", DbType.DateTime, orderDateFrom);
      }

      DateTime orderDateTo = DBConvert.ParseDateTime(udtOrderDateTo.Value);
      if (orderDateTo != DateTime.MinValue)
      {
        orderDateTo = (orderDateTo != DateTime.MaxValue) ? orderDateTo.AddDays(1) : orderDateTo;
        param[5] = new DBParameter("@OrderDateTo", DbType.DateTime, orderDateTo);
      }

      // Customer
      long customerPid = DBConvert.ParseLong(ucbCustomer.Value);
      if (customerPid != long.MinValue)
      {
        param[6] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      }

      // Type
      int value = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ucbType));
      if (value != int.MinValue)
      {
        param[7] = new DBParameter("@Type", DbType.Int32, value);
      }

      // Status
      value = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ucbStatus));
      if (value >= 0)
      {
        param[8] = new DBParameter("@Status", DbType.Int32, value);
      }

      // Create By
      value = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ucbCreateBy));
      if (value != int.MinValue)
      {
        param[9] = new DBParameter("@CreateBy", DbType.Int32, value);
      }
      if (txtSaleCode.Text.Trim().Length > 0)
      {
        param[10] = new DBParameter("@SaleCode", DbType.AnsiString, 32, txtSaleCode.Text);
      }
      string storeName = "spCUSListSaleOrder";

      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
      DataSet dsData = Shared.Utility.CreateDataSet.SaleOrder_Enquiry();
      dsData.Tables["dtParent"].Merge(dsSource.Tables[0]);
      dsData.Tables["dtChild"].Merge(dsSource.Tables[1]);

      ultraData.DataSource = dsData;
      for (int i = 0; i < ultraData.Rows.Count; i++)
      {
        ultraData.Rows[i].Activation = Activation.ActivateOnly;
      }
      for (int i = 0; i < ultraData.Rows.Count; i++)
      {
        string status = ultraData.Rows[i].Cells["ST"].Value.ToString().Trim();
        if (status == "0")
        {
          ultraData.Rows[i].CellAppearance.BackColor = Color.White;
        }
        else if (status == "1")
        {
          ultraData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
        else if (status == "2")
        {
          ultraData.Rows[i].CellAppearance.BackColor = Color.Cyan;
        }
        else if (status == "3")
        {
          ultraData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        else if (status == "4")
        {
          ultraData.Rows[i].CellAppearance.BackColor = Color.Pink;
        }
      }
      lbCount.Text = string.Format("Đếm: {0}", ultraData.Rows.FilteredInRowCount);
    }

    #endregion LoadData

    #region Event

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnNewSaleOrder_Click(object sender, EventArgs e)
    {
      viewCSD_05_004 uc = new viewCSD_05_004();
      Shared.Utility.WindowUtinity.ShowView(uc, "INSERT SALE ORDER INFO", true, Shared.Utility.ViewState.MainWindow);
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultraData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Utility.SetPropertiesUltraGrid(ultraData);
      e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands["dtParent_dtChild"].Columns["ParentPid"].Hidden = true;
      e.Layout.Bands["dtParent_dtChild"].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent_dtChild"].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent_dtChild"].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent_dtChild"].Columns["CBM"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent_dtChild"].Columns["TotalCBM"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent_dtChild"].Columns["CBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtParent_dtChild"].Columns["TotalCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtParent_dtChild"].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands["dtParent_dtChild"].Columns["ItemCode"].Header.Caption = "Mã SP";
      e.Layout.Bands["dtParent_dtChild"].Columns["SaleCode"].Header.Caption = "Mã SP KH";
      e.Layout.Bands["dtParent_dtChild"].Columns["Revision"].Header.Caption = "Phiên bản";
      e.Layout.Bands["dtParent_dtChild"].Columns["Name"].Header.Caption = "Tên sản phẩm";
      e.Layout.Bands["dtParent_dtChild"].Columns["Qty"].Header.Caption = "Số lượng";
      e.Layout.Bands["dtParent_dtChild"].Columns["Price"].Header.Caption = "Đơn giá";
      e.Layout.Bands["dtParent"].Columns["No"].Header.Caption = "Mã đơn hàng";
      e.Layout.Bands["dtParent"].Columns["CusNo"].Header.Caption = "Mã ĐH KH";
      e.Layout.Bands["dtParent"].Columns["OrderDate"].Header.Caption = "Ngày đặt";
      e.Layout.Bands["dtParent"].Columns["Customer"].Header.Caption = "Khách hàng";
      e.Layout.Bands["dtParent"].Columns["Direct"].Header.Caption = "KH trực tiếp";
      e.Layout.Bands["dtParent"].Columns["Type"].Header.Caption = "Loại ĐH";
      e.Layout.Bands["dtParent"].Columns["CreateDate"].Header.Caption = "Ngày tạo";
      e.Layout.Bands["dtParent"].Columns["CreateBy"].Header.Caption = "Người tạo";
      e.Layout.Bands["dtParent"].Columns["Remark"].Header.Caption = "Diễn giải";

      e.Layout.Bands["dtParent"].Columns["Pid"].Hidden = true;
      e.Layout.Bands["dtParent"].Columns["Status"].Hidden = true;
      e.Layout.Bands["dtParent"].Columns["ST"].Hidden = true;
      e.Layout.Bands["dtParent"].Columns["DateToPLA"].Hidden = true;
      e.Layout.Bands["dtParent"].Columns["PLN Confirmed Date"].Hidden = true;
      e.Layout.Bands["dtParent_dtChild"].Columns["PO Number"].Hidden = true;
      e.Layout.Bands["dtParent_dtChild"].Columns["CBM"].Hidden = true;
      e.Layout.Bands["dtParent_dtChild"].Columns["TotalCBM"].Hidden = true;
      e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
      e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
    }

    private void ultraData_Click(object sender, EventArgs e)
    {
      if (ultraData.Rows.Count > 0)
      {
        string value = string.Empty;
        try
        {
          value = ultraData.Selected.Rows[0].Cells["Status"].Value.ToString().Trim();
        }
        catch { }
        if (value == "Non Confirm")
        {
          btnPrint.Enabled = false;
        }
        else
        {
          btnPrint.Enabled = true;
        }
      }
    }

    private void ultraData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultraData.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (ultraData.Selected.Rows[0].ParentRow == null) ? ultraData.Selected.Rows[0] : ultraData.Selected.Rows[0].ParentRow;

      viewCSD_05_004 uc = new viewCSD_05_004();
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());

      uc.saleOrderPid = pid;
      Shared.Utility.WindowUtinity.ShowView(uc, "UPDATE SALE ORDER INFO", false, Shared.Utility.ViewState.MainWindow);
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtNoFrom.Text = string.Empty;
      txtNoTo.Text = string.Empty;
      txtCusNoFrom.Text = string.Empty;
      txtCusNoTo.Text = string.Empty;
      udtOrderDateFrom.Value = DateTime.MinValue;
      udtOrderDateTo.Value = DateTime.MinValue;
      //ucbCustomer.SelectedRow.Index = -1;
      ucbCustomer.Value = DBNull.Value;
      ucbType.Value = null;
      ucbStatus.Value = null;
      ucbCreateBy.Value = null;
      txtSaleCode.Text = string.Empty;
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultraData, "Data");
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

    #endregion Event
  }
}
