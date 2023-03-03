using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared;
using VBReport;
using System.Diagnostics;
using DaiCo.Shared.Utility;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_05_003 : MainUserControl
  {
    public viewCSD_05_003()
    {
      InitializeComponent();
      dt_OrderDateFrom.Value = DateTime.MinValue;
      dt_OrderDateTo.Value = DateTime.MinValue;
    }
    void viewCSD_05_003_Load(object sender, System.EventArgs e)
    {
      this.LoadData();
    }
    
    #region LoadData

    private void LoadData()
    {
      // Customer
      Shared.Utility.ControlUtility.LoadCustomer(cmbCustomer);

      // Type
      Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbType, Shared.Utility.ConstantClass.GROUP_SALEORDERTYPE);
      
      //Status
      this.LoadStatus();

      // Create By
      Shared.Utility.ControlUtility.LoadEmployee(cmbCreateBy, "CSD");
      btnPrint.Enabled = false;
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
      ControlUtility.LoadCombobox(cmbStatus, dtStatus, "Value", "Text");
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
      DateTime orderDateFrom = dt_OrderDateFrom.Value;
      if (orderDateFrom != DateTime.MinValue)
      {
        param[4] = new DBParameter("@OrderDateFrom", DbType.DateTime, orderDateFrom);
      }

      DateTime orderDateTo = dt_OrderDateTo.Value;
      if (orderDateTo != DateTime.MinValue)
      {
        orderDateTo = (orderDateTo != DateTime.MaxValue) ? orderDateTo.AddDays(1) : orderDateTo;
        param[5] = new DBParameter("@OrderDateTo", DbType.DateTime, orderDateTo);
      }

      // Customer
      long customerPid = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCustomer));
      if (customerPid != long.MinValue)
      {
        param[6] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      }

      // Type
      int value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbType));
      if (value != int.MinValue)
      {
        param[7] = new DBParameter("@Type", DbType.Int32, value);
      }

      // Status
      value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbStatus));      
      if (value >= 0)
      {
        param[8] = new DBParameter("@Status", DbType.Int32, value);
      }

      // Create By
      value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCreateBy));
      if (value != int.MinValue)
      {
        param[9] = new DBParameter("@CreateBy", DbType.Int32, value);
      }
      if (txtSaleCode.Text.Trim().Length > 0)
      {
        param[10] = new DBParameter("@SaleCode", DbType.AnsiString, 32, txtSaleCode.Text );
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
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraData);
      e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands["dtParent_dtChild"].Columns["ParentPid"].Hidden = true;
      e.Layout.Bands["dtParent_dtChild"].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent_dtChild"].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands["dtParent_dtChild"].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands["dtParent_dtChild"].Columns["TotalCBM"].Header.Caption = "Total CBM";
      e.Layout.Bands["dtParent_dtChild"].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent_dtChild"].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent_dtChild"].Columns["CBM"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent_dtChild"].Columns["TotalCBM"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent_dtChild"].Columns["CBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtParent_dtChild"].Columns["TotalCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtParent_dtChild"].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands["dtParent"].Columns["No"].Header.Caption = "Sale No";
      e.Layout.Bands["dtParent"].Columns["CusNo"].Header.Caption = "Customer's PoNo";
      e.Layout.Bands["dtParent"].Columns["OrderDate"].Header.Caption = "Order Date";
      e.Layout.Bands["dtParent"].Columns["Pid"].Hidden = true;
      e.Layout.Bands["dtParent"].Columns["Status"].Hidden = true;
      e.Layout.Bands["dtParent"].Columns["ST"].Hidden = true;
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
      dt_OrderDateFrom.Value = DateTime.MinValue; 
      dt_OrderDateTo.Value = DateTime.MinValue;
      cmbCustomer.SelectedIndex = -1;
      cmbType.SelectedIndex = -1;
      cmbStatus.SelectedIndex = -1;
      cmbCreateBy.SelectedIndex = -1;
      txtSaleCode.Text = string.Empty;
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      DBParameter[] param = new DBParameter[10];
      string text = txtNoFrom.Text.Trim();
      if (text.Length > 0)
      {
        param[0] = new DBParameter("@SaleNoFrom", DbType.AnsiString, 20, text);
      }
      text = txtNoTo.Text.Trim();
      if (text.Length > 0)
      {
        param[1] = new DBParameter("@SaleNoTo", DbType.AnsiString, 20, text);
      }
      text = txtCusNoFrom.Text.Trim();
      if (text.Length > 0)
      {
        param[2] = new DBParameter("@CusNoFrom", DbType.AnsiString, 20, text);
      }
      text = txtCusNoTo.Text.Trim();
      if (text.Length > 0)
      {
        param[3] = new DBParameter("@CusNoTo", DbType.AnsiString, 20, text);
      }
      DateTime orderDateFrom = dt_OrderDateFrom.Value;
      if (orderDateFrom != DateTime.MinValue)
      {
        param[4] = new DBParameter("@OrderDateFrom", DbType.DateTime, orderDateFrom);
      }

      DateTime orderDateTo = dt_OrderDateTo.Value;
      if (orderDateTo != DateTime.MinValue)
      {
        orderDateTo = (orderDateTo != DateTime.MaxValue) ? orderDateTo.AddDays(1) : orderDateTo;
        param[5] = new DBParameter("@OrderDateTo", DbType.DateTime, orderDateTo);
      }
      long customerPid = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCustomer));
      if (customerPid != long.MinValue)
      {
        param[6] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      }
      int value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbType));
      if (value != int.MinValue)
      {
        param[7] = new DBParameter("@Type", DbType.Int32, value);
      }
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
      value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCreateBy));
      if (value != int.MinValue)
      {
        param[9] = new DBParameter("@CreateBy", DbType.Int32, value);
      }
      string storeName = "spCUSListSaleOrder_Export";
      DataSet dsource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
      DataTable dt = dsource.Tables[0];
      string strTemplateName = "CSDSaleOrderListTemplate";
      string strSheetName = "SaleOrderList";
      string strOutFileName = "SaleOrderList";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate\CustomerService";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow dtRow = dt.Rows[i];
        if (i > 0)
        {
          oXlsReport.Cell("A6:V6").Copy();
          oXlsReport.RowInsert(5 + i);
          oXlsReport.Cell("A6:V6", 0, i).Paste();
        }
        for (int c = 0; c < dt.Columns.Count; c++)
        {
          oXlsReport.Cell("**1", 0, i).Value = i + 1;
          oXlsReport.Cell("**2", 0, i).Value = dtRow["CUSName"];
          oXlsReport.Cell("**3", 0, i).Value = dtRow["DIRName"];
          oXlsReport.Cell("**4", 0, i).Value = dtRow["EnquiryNo"];
          oXlsReport.Cell("**5", 0, i).Value = dtRow["CustomerPONo"];
          oXlsReport.Cell("**6", 0, i).Value = dtRow["SaleNo"];
          oXlsReport.Cell("**7", 0, i).Value = dtRow["SOType"];
          oXlsReport.Cell("**8", 0, i).Value = dtRow["ItemCode"];
          oXlsReport.Cell("**9", 0, i).Value = dtRow["Revision"];
          oXlsReport.Cell("**10", 0, i).Value = dtRow["Name"];
          oXlsReport.Cell("**11", 0, i).Value = dtRow["Qty"];
          oXlsReport.Cell("**12", 0, i).Value = dtRow["Unit"];
          oXlsReport.Cell("**13", 0, i).Value = dtRow["CBM"];
          oXlsReport.Cell("**14", 0, i).Value = dtRow["TotalCBM"];
          oXlsReport.Cell("**15", 0, i).Value = dtRow["Price"];
          oXlsReport.Cell("**16", 0, i).Value = dtRow["Amount"];
          oXlsReport.Cell("**17", 0, i).Value = dtRow["SecondPrice"];
          oXlsReport.Cell("**18", 0, i).Value = dtRow["SecondAmount"];
          oXlsReport.Cell("**19", 0, i).Value = dtRow["ScheduleDate"];
          oXlsReport.Cell("**20", 0, i).Value = dtRow["SpecialInstruction"];
          oXlsReport.Cell("**21", 0, i).Value = dtRow["Remark"];
          oXlsReport.Cell("**22", 0, i).Value = dtRow["SaleCode"];

        }
      }

      int cnt = dt.Rows.Count + 5;
      oXlsReport.Cell("**SumQty").Value = "=SUM(L6:L" + cnt.ToString() + ")";
      oXlsReport.Cell("**SumTotalCBM").Value = "=SUM(O6:O" + cnt.ToString() + ")";
      oXlsReport.Cell("**SumAmount").Value = "=SUM(Q6:Q" + cnt.ToString() + ")";
      oXlsReport.Cell("**SumSecondAmount").Value = "=SUM(S6:S" + cnt.ToString() + ")";
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
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

    private void dt_OrderDateFrom_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void dt_OrderDateTo_KeyDown(object sender, KeyEventArgs e)
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

    private void cmbCreateBy_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    #endregion Event
  }
}
