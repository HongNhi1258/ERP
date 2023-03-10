using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
namespace DaiCo.ERPProject
{
  public partial class viewPLN_01_001 : MainUserControl
  {
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    public viewPLN_01_001()
    {
      InitializeComponent();
      dt_OrderDateFrom.Value = DateTime.MinValue;
      dt_OderDateTo.Value = DateTime.MinValue;
      ultReceivedDateFrom.Value = DBNull.Value;
      ultReceivedDateTo.Value = DBNull.Value;
    }

    private void viewPLN_01_001_Load(object sender, EventArgs e)
    {
      this.SetAutoSearchWhenPressEnter(gpbSearch);
      this.LoadData();
    }

    #region LoadData

    private DataSet GetDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("taParent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("No", typeof(System.String));
      taParent.Columns.Add("OrderDate", typeof(System.String));
      taParent.Columns.Add("CusNo", typeof(System.String));
      taParent.Columns.Add("Customer", typeof(System.String));
      taParent.Columns.Add("Direct", typeof(System.String));
      taParent.Columns.Add("Type", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.Double));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("AllocatePid", typeof(System.Int64));
      ds.Tables.Add(taParent);
      DataTable taChild = new DataTable("taChild");
      taChild.Columns.Add("ParentPid", typeof(System.Int64));
      taChild.Columns.Add("SaleCode", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("Name", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("QtyAllocated", typeof(System.Int32));
      taChild.Columns.Add("CBM", typeof(System.Double));
      taChild.Columns.Add("TotalCBM", typeof(System.Double));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("taParent_taChild", taParent.Columns["Pid"], taChild.Columns["ParentPid"]));
      return ds;
    }

    private void Search()
    {
      DBParameter[] param = new DBParameter[15];

      // EN No
      string text = txtNoFrom.Text.Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[0] = new DBParameter("@NoFrom", DbType.AnsiString, 20, text);
      }
      text = txtNoTo.Text.Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[1] = new DBParameter("@NoTo", DbType.AnsiString, 20, text);
      }
      text = txtNoSet.Text.Trim();

      param[2] = new DBParameter("@NoSet", DbType.AnsiString, 1000, text);

      // Customer's EN No
      text = txtCusNoFrom.Text.Trim();
      if (text.Length > 0)
      {
        param[3] = new DBParameter("@CusEnquiryNoFrom", DbType.AnsiString, 20, text);
      }
      text = txtCusNoTo.Text.Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[4] = new DBParameter("@CusEnquiryNoTo", DbType.AnsiString, 20, text);
      }
      text = txtCusNoSet.Text.Trim();
      param[5] = new DBParameter("@CusEnquiryNoSet", DbType.AnsiString, 1000, text);

      //Order Date
      if (dt_OrderDateFrom.Value != DateTime.MinValue)
      {
        DateTime orderDate = dt_OrderDateFrom.Value;
        param[6] = new DBParameter("@OrderDateFrom", DbType.DateTime, new DateTime(orderDate.Year, orderDate.Month, orderDate.Day));
      }

      if (dt_OderDateTo.Value != DateTime.MinValue)
      {
        DateTime orderDate = dt_OderDateTo.Value.Date;
        orderDate = orderDate.AddDays(1);
        param[7] = new DBParameter("@OrderDateTo", DbType.DateTime, orderDate);
      }

      // Customer
      long customerPid = DBConvert.ParseLong(ucbCustomer.Value);
      if (customerPid != long.MinValue)
      {
        param[8] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      }

      // Type
      int value = DBConvert.ParseInt(cmbType.SelectedValue);
      if (value != int.MinValue)
      {
        param[9] = new DBParameter("@Type", DbType.Int32, value);
      }

      // Status
      value = cmbStatus.SelectedIndex;
      if (value > 0)
      {
        param[10] = new DBParameter("@Status", DbType.Int32, value);
      }
      param[11] = new DBParameter("@Cancel", DbType.Int32, chxCancel.Checked ? 1 : 0);
      param[12] = new DBParameter("@Expire", DbType.Int32, chxExpire.Checked ? 1 : 0);

      // ItemCode
      text = txtItemCode.Text.Trim();
      if (text.Length > 0)
      {
        param[11] = new DBParameter("@ItemCode", DbType.AnsiString, 18, "%" + text.Replace("'", "''") + "%");
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

      string storeName = "spPLNListEnquiry";

      //DataSet dsSource = MainBOMLibary.SearchStoreProcedureToDataSet(storeName, param);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, param);
      DataSet dsData = GetDataSet();
      dsData.Tables["taParent"].Merge(dsSource.Tables[0]);
      dsData.Tables["taChild"].Merge(dsSource.Tables[1]);
      ultData.DataSource = dsData;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string status = ultData.Rows[i].Cells["ST"].Value.ToString().Trim();
        if (status == "1")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
        else if (status == "2")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        else if (status == "3")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Pink;
        }
      }
    }
    private void LoadCustomer()
    {
      string commandText = string.Format(@"SELECT Pid, CustomerCode + ' - ' + Name Customer FROM TblCSDCustomerInfo WHERE DeletedFlg = 0 AND ParentPid IS NULL ORDER BY CustomerCode");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbCustomer, dtSource, "Pid", "Customer", "Pid");
    }
    private void LoadData()
    {
      // Customer
      this.LoadCustomer();
      // Type
      Utility.LoadComboboxCodeMst(cmbType, Shared.Utility.ConstantClass.GROUP_SALEORDERTYPE);
      // Disable button Print (Qui, 13/12/2010)
      //btnPrint.Enabled = false;
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
    #endregion LoadData

    #region Event

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      Utility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      //e.Layout.Bands["taParent"].Override.AllowAddNew = AllowAddNew.Yes;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      for (int j = 0; j < e.Layout.Bands[0].Columns.Count; j++)
      {
        e.Layout.Bands[0].Columns[j].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      }
      e.Layout.Bands[0].Columns["ReAllocate"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      e.Layout.Bands["taParent"].Columns["Pid"].Hidden = true;
      e.Layout.Bands["taParent"].Columns["ST"].Hidden = true;
      e.Layout.Bands["taParent"].Columns["Qty"].Hidden = true;
      e.Layout.Bands["taParent"].Columns["Status"].Hidden = true;
      e.Layout.Bands["taParent"].Columns["AllocatePid"].Hidden = true;
      e.Layout.Bands["taParent"].Columns["StatusAllocated"].Hidden = true;
      e.Layout.Bands["taParent"].Columns["ReAllocate"].Hidden = true;
      e.Layout.Bands["taParent"].Columns["No"].Header.Caption = "Enquiry No";
      e.Layout.Bands["taParent"].Columns["CusNo"].Header.Caption = "Customer\nEnquiry No";
      e.Layout.Bands["taParent"].Columns["OrderDate"].Header.Caption = "Order Date";
      e.Layout.Bands["taParent"].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands["taParent"].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands["taParent"].Columns["PLAConfirmDate"].Header.Caption = "PLN Confirm\nDate";
      e.Layout.Bands["taParent"].Columns["PLNConfirmBy"].Header.Caption = "PLN Confirm\nBy";
      e.Layout.Bands["taParent"].Columns["DateToPLA"].Header.Caption = "Date To\nPLN";
      e.Layout.Bands[0].Columns["ReAllocate"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      //e.Layout.Bands["taParent"].Columns["ReAllocate"].CellActivation = Activation.AllowEdit;

      e.Layout.Bands["taParent_taChild"].Columns["ParentPid"].Hidden = true;
      e.Layout.Bands["taParent_taChild"].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["taParent_taChild"].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands["taParent_taChild"].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands["taParent_taChild"].Columns["TotalCBM"].Header.Caption = "Total CBM";
      e.Layout.Bands["taParent_taChild"].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["taParent_taChild"].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["taParent_taChild"].Columns["QtyAllocated"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["taParent_taChild"].Columns["CBM"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["taParent_taChild"].Columns["TotalCBM"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands["taParent_taChild"].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taParent_taChild"].Columns["QtyAllocated"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taParent_taChild"].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taParent_taChild"].Columns["CBM"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taParent_taChild"].Columns["TotalCBM"].CellAppearance.TextHAlign = HAlign.Right;
      //e.Layout.Override.CellClickAction = CellClickAction.RowSelect;

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";

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
        // Disable button Print (Qui, 13/12/2010)
        //if (value == "PLN Confirmed")
        //{
        //  btnPrint.Enabled = true;
        //}
        //else
        //{
        //  btnPrint.Enabled = false;
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
      UltraGridRow row = (ultData.Selected.Rows[0].ParentRow == null) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ParentRow;
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());

      viewPLN_10_010 uc = new viewPLN_10_010();
      uc.enquiryPid = pid;
      WindowUtinity.ShowView(uc, "UPDATE ENQUIRY INFORMATION", false, ViewState.MainWindow);

      this.Search();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtNoFrom.Text = string.Empty;
      txtNoTo.Text = string.Empty;
      txtNoSet.Text = string.Empty;
      txtCusNoFrom.Text = string.Empty;
      txtCusNoTo.Text = string.Empty;
      txtCusNoSet.Text = string.Empty;
      dt_OrderDateFrom.Value = DateTime.MinValue;
      dt_OderDateTo.Value = DateTime.MinValue;
      ucbCustomer.Value = DBNull.Value;
      cmbType.SelectedIndex = 0;
      cmbStatus.SelectedIndex = 0;
      txtItemCode.Text = string.Empty;
      chxCancel.Checked = false;
      chxExpire.Checked = false;
      txtNoFrom.Focus();
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultData, "Enquiry List");
    }

    static public void releaseObject(object obj)
    {
      try
      {
        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
        obj = null;
      }
      catch (Exception ex)
      {
        obj = null;
        throw new Exception("Exception Occured while releasing object " + ex.ToString());
      }
      finally
      {
        GC.Collect();
      }
    }


    private void btnAllocateFUR_Click(object sender, EventArgs e)
    {
      viewPLN_10_051 view = new viewPLN_10_051();
      Shared.Utility.WindowUtinity.ShowView(view, "Import Allocate Furniture for Enquiry", false, Shared.Utility.ViewState.MainWindow);
    }
    private void btnRefreshAllocate_Click(object sender, EventArgs e)
    {
      btnRefreshAllocate.Enabled = false;
      DataBaseAccess.ExecuteStoreProcedure("spPLNAllocateEnquiryFurnitureSO_Capture", 600, null, null);
      WindowUtinity.ShowMessageSuccess("MSG0004");
      btnRefreshAllocate.Enabled = true;
    }
    /// <summary>
    /// ReAllocate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnReAllocate_Click(object sender, EventArgs e)
    {
      viewPLN_01_008 view = new viewPLN_01_008();
      WindowUtinity.ShowView(view, "Reallocate Enquiry", false, ViewState.ModalWindow);
      //btnReAllocate.Enabled = false;
      //bool success = this.ReAllocate();
      //if (success)
      //{
      //  WindowUtinity.ShowMessageSuccess("MSG0004");
      //  this.Search();
      //}
      //else
      //{
      //  WindowUtinity.ShowMessageError("ERR0005");
      //}
      //btnReAllocate.Enabled = true;
    }

    private bool ReAllocate()
    {
      DataTable dt = (DataTable)((DataSet)ultData.DataSource).Tables[0];
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(dt.Rows[i]["ReAllocate"].ToString()) == 1)
        {
          DBParameter[] input = new DBParameter[1];
          input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["Pid"].ToString()));
          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spPLNReAllocateForEnquiry", input, output);
          if (DBConvert.ParseInt(output[0].Value.ToString()) <= 0)
          {
            return false;
          }
        }
      }
      return true;
    }
    #endregion Event

  }
}
