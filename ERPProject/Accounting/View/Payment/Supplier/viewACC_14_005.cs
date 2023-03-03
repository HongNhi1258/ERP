/*
  Author      : Nguyen Van Tron
  Date        : 1/04/2022
  Description : Supplier Payment
  Standard Form: view_SaveMasterDetail
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_14_005 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    public int actionCode = int.MinValue;
    private IList listDeletedPid = new ArrayList();
    private int docTypePid = 247;
    private bool isLoadedPostTransaction = false;
    private int status = 0;
    private DataTable dtObject = new DataTable();
    private int oddNumber = 2;
    private bool isLoadingData = false;
    #endregion Field

    #region Init
    public viewACC_14_005()
    {
      InitializeComponent();
    }

    private void viewACC_14_005_Load(object sender, EventArgs e)
    {
      Utility.Format_UltraNumericEditor(tlpForm);
      // Add ask before closing form even if user change data
      this.SetAutoAskSaveWhenCloseForm(ugbInformation);      
      this.SetBlankForTextOfButton(this);
      this.InitData();
      this.LoadData();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Set Auto Ask Save Data When User Close Form
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoAskSaveWhenCloseForm(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.TextChanged += new System.EventHandler(this.Object_Changed);
        }
        else
        {
          this.SetAutoAskSaveWhenCloseForm(ctr);
        }
      }
    }

    private void SetNeedToSave()
    {
      if (btnSave.Enabled && btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {      
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCSupplierPayment_Init");
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[0], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate", "OddNumber" });           
      Utility.LoadUltraCombo(ucbObject, dsInit.Tables[1], "KeyValue", "Object", false, new string[] { "ObjectPid", "Object", "ObjectType", "KeyValue", "AccountPid" });
      ucbObject.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
      Utility.LoadUltraCombo(ucbCashFund, dsInit.Tables[2], "Value", "Display", false, new string[] { "Value", "AccountPid"});
      Utility.LoadUltraCombo(ucbCompanyBank, dsInit.Tables[3], "Value", "Display", false, new string[] { "Value", "Display", "BankAccount", "AccountPid" });
      ucbCompanyBank.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

      //Init dropdown
      Utility.LoadUltraCBAccountList(ucbddDebit);
      Utility.LoadUltraCBAccountList(ucbddCredit);
      //this.dtObject = dsInit.Tables[1];
      // Set Language
      //this.SetLanguage();
    }

    /// <summary>
    /// Read Only for key main data
    /// </summary>
    private void ReadOnlyKeyMainData()
    {
      bool isReadOnly = false;
      if(ugdData.Rows.Count > 0)
      {
        isReadOnly = true;
      }  
      ucbCurrency.ReadOnly = isReadOnly;
      ucbObject.ReadOnly = isReadOnly;
    }

    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        txtPaymentCode.ReadOnly = true;
        udtPaymentDate.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
        ucbCompanyBank.ReadOnly = true;
        uneBankFeeAmount.ReadOnly = true;
        ucbCashFund.ReadOnly = true;
        ucbBankAccount.ReadOnly = true;
        txtReceiverBankNo.ReadOnly = true;
        txtReceiverAddress.ReadOnly = true;
        txtReceiverBranchName.ReadOnly = true;
        txtReceiverName.ReadOnly = true;
        txtPaymentDesc.ReadOnly = true;        
        btnSave.Enabled = false;
        chkConfirm.Enabled = false;
        if (this.status == 1)
        {
          btnApprove.Visible = true;
          btnPaymentProposal.Enabled = false;
        }
        else if (this.status == 2)
        {
          btnApprove.Enabled = false;
          btnPaymentProposal.Enabled = false;
        }
      }
      if(this.status < 1)
      {
        btnApprove.Visible = false;
      }  
      
      this.ReadOnlyKeyMainData();

      //Hide bank information when action code is 1
      if (this.actionCode == 1)
      {
        lbCompanyBank.Visible = false;
        label3.Visible = false;
        tbCompanyBank.Visible = false;
        lbBankFeeAmount.Visible = false;
        uneBankFeeAmount.Visible = false;
        pnReceiver.Visible = false;
      }
      else if (this.actionCode == 2)
      {
        lbCashFund.Visible = false;
        ucbCashFund.Visible = false;
      }
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        this.actionCode = DBConvert.ParseInt(dtMain.Rows[0]["ActionCode"]);
        txtPaymentCode.Text = dtMain.Rows[0]["PaymentCode"].ToString();
        txtPaymentDesc.Text = dtMain.Rows[0]["PaymentDesc"].ToString();
        udtPaymentDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["PaymentDate"]);
        if (DBConvert.ParseInt(dtMain.Rows[0]["CompanyBankPid"]) > 0)
        {
          ucbCompanyBank.Value = DBConvert.ParseInt(dtMain.Rows[0]["CompanyBankPid"]);
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["CashFundPid"]) > 0)
        {
          ucbCashFund.Value = DBConvert.ParseInt(dtMain.Rows[0]["CashFundPid"]);
        }
        ucbCurrency.Value = DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"]);
        uneExchangeRate.Value = DBConvert.ParseDouble(dtMain.Rows[0]["ExchangeRate"]);
        if (DBConvert.ParseDouble(dtMain.Rows[0]["BankFeeAmount"]) > 0)
        {
          uneBankFeeAmount.Value = DBConvert.ParseInt(dtMain.Rows[0]["BankFeeAmount"]);
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["AccountBankPid"]) > 0)
        {
          ucbBankAccount.Value = DBConvert.ParseInt(dtMain.Rows[0]["AccountBankPid"]);
        }
        txtReceiverName.Text = dtMain.Rows[0]["ReceiverName"].ToString();
        txtReceiverBranchName.Text = dtMain.Rows[0]["ReceiverBranchName"].ToString();
        txtReceiverBankNo.Text = dtMain.Rows[0]["ReceiverBankNo"].ToString();
        txtReceiverAddress.Text = dtMain.Rows[0]["ReceiverAddress"].ToString();
        ucbObject.Value = dtMain.Rows[0]["ObjectValue"];
        lbStatusDes.Text = dtMain.Rows[0]["StatusText"].ToString();
        uneTotalAmount.Value = dtMain.Rows[0]["TotalAmount"];                             
        this.status = DBConvert.ParseInt(dtMain.Rows[0]["Status"]);
        chkConfirm.Checked = (this.status >= 1 ? true : false);
      }
      else
      {
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@NewDocCode", DbType.String, 32, string.Empty) };
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@DocTypePid", DbType.Int32, this.docTypePid);
        inputParam[1] = new DBParameter("@DocDate", DbType.Date, DateTime.Now.Date);

        DataBaseAccess.SearchStoreProcedure("spACCGetNewDocCode", inputParam, outputParam);
        if (outputParam[0].Value.ToString().Length > 0)
        {
          txtPaymentCode.Text = outputParam[0].Value.ToString();
          udtPaymentDate.Value = DateTime.Now;
          
        }
      }
    }

    /// <summary>
    /// Load tab data
    /// </summary>
    private void LoadTabData()
    {
      // Load Tab Data Component
      string tabPageName = utcDetail.SelectedTab.TabPage.Name;
      switch (tabPageName)
      {
        case "utpcList":
          //this.LoadData();
          break;
        case "utpcPost":
          if (!isLoadedPostTransaction)
          {
            if (chkConfirm.Checked)
            {
              this.LoadTransationData();
              this.isLoadedPostTransaction = true;
            }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Load transaction
    /// </summary>
    private void LoadTransationData()
    {
      ugdTransaction.SetDataSource(this.docTypePid, this.viewPid);
    }

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();

      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCSupplierPayment_Load", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 1)
      {
        this.LoadMainData(dsSource.Tables[0]);
        ugdData.DataSource = dsSource.Tables[1];
        lbCount.Text = string.Format(@"Đếm {0}", ugdData.Rows.FilteredInRowCount > 0 ? ugdData.Rows.FilteredInRowCount : 0);
      }

      this.SetStatusControl();
      this.NeedToSave = false;
    }

    private bool CheckValid()
    {
      //check master
      if (udtPaymentDate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày đề nghị không được để trống!!!");
        udtPaymentDate.Focus();
        return false;
      }
      if (ucbCurrency.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Loại tiền tệ không được để trống.");
        ucbCurrency.Focus();
        return false;
      }          
      if (ucbObject.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Đối tượng không được để trống.");
        ucbObject.Focus();
        return false;
      }

      if(this.actionCode == 1)
      {
        if (ucbCashFund.SelectedRow == null)
        {
          WindowUtinity.ShowMessageErrorFromText("Quỹ tiền mặt không được để trống.");
          ucbCashFund.Focus();
          return false;
        }
      }  

      if(this.actionCode == 2)
      {
        if (ucbCompanyBank.SelectedRow == null)
        {
          WindowUtinity.ShowMessageErrorFromText("Ngân hàng không được để trống.");
          ucbCompanyBank.Focus();
          return false;
        }
      }  

      //check detail      
      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        UltraGridRow row = ugdData.Rows[i];
        row.Selected = false;

        if (DBConvert.ParseInt(row.Cells["DebitPid"].Value.ToString()) <= 0)
        {
          row.Cells["DebitPid"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Tài khoản nợ không được để trống.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }

        if (DBConvert.ParseInt(row.Cells["CreditPid"].Value.ToString()) <= 0)
        {
          row.Cells["CreditPid"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Tài khoản có không được để trống.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }


        if (DBConvert.ParseDouble(row.Cells["Amount"].Value.ToString()) <= 0)
        {
          row.Cells["Amount"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Số tiền đề nghị không được để trống và phải lớn hơn 0.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
      }

      // Check total proposal <= debit amount of PO or Invoice
      DataTable dtDetail = new DataTable();
      dtDetail.Columns.Add("Pid", typeof(long));
      dtDetail.Columns.Add("ProposalDetailPid", typeof(long));      
      dtDetail.Columns.Add("Amount", typeof(double));

      DataTable dtSource = (DataTable)ugdData.DataSource;      
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          DataRow rowDetail = dtDetail.NewRow();
          rowDetail["Pid"] = row["Pid"];
          rowDetail["ProposalDetailPid"] = row["ProposalDetailPid"];          
          rowDetail["Amount"] = row["Amount"];
          dtDetail.Rows.Add(rowDetail);
        }
      }
      int paramNumber = 2;
      string storeName = "spACCSupplierPayment_CheckValid";

      SqlDBParameter[] inputParam = new SqlDBParameter[paramNumber];
      inputParam[0] = new SqlDBParameter("@AddedDocList", SqlDbType.NVarChar, DBConvert.ParseXMLString(dtDetail));
      inputParam[1] = new SqlDBParameter("@SupplierPaymentPid", SqlDbType.Int, this.viewPid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@ErrorMessage", SqlDbType.NVarChar, 256, string.Empty) };
      SqlDataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if (outputParam != null && outputParam[0].Value.ToString().Length > 0)
      {
        WindowUtinity.ShowMessageErrorFromText(outputParam[0].Value.ToString());
        return false;
      }  
      return true;
    }

    private bool SaveMain()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[19];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      }
      if (txtPaymentDesc.Text.Trim().Length > 0)
      {
        inputParam[1] = new SqlDBParameter("@PaymentDesc", SqlDbType.NVarChar, txtPaymentDesc.Text.Trim().ToString());
      }
      inputParam[2] = new SqlDBParameter("@PaymentDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtPaymentDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      inputParam[3] = new SqlDBParameter("@ObjectPid", SqlDbType.Int, ucbObject.SelectedRow.Cells["ObjectPid"].Value);
      inputParam[4] = new SqlDBParameter("@ObjectType", SqlDbType.Int, ucbObject.SelectedRow.Cells["ObjectType"].Value);
      if (DBConvert.ParseInt(ucbCompanyBank.Value) > 0)
      {
        inputParam[5] = new SqlDBParameter("@CompanyBankPid", SqlDbType.Int, DBConvert.ParseInt(ucbCompanyBank.Value));
      }
      if (DBConvert.ParseInt(ucbCashFund.Value) > 0)
      {
        inputParam[6] = new SqlDBParameter("@CashFundPid", SqlDbType.Int, DBConvert.ParseInt(ucbCashFund.Value));
      }
      inputParam[7] = new SqlDBParameter("@CurrencyPid", SqlDbType.Int, DBConvert.ParseInt(ucbCurrency.Value));
      inputParam[8] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, DBConvert.ParseDouble(uneExchangeRate.Value));
      if (DBConvert.ParseDouble(uneBankFeeAmount.Value) >= 0)
      {
        inputParam[9] = new SqlDBParameter("@BankFeeAmount", SqlDbType.Float, DBConvert.ParseDouble(uneBankFeeAmount.Value));
      }
      if (DBConvert.ParseInt(ucbBankAccount.Value) > 0)
      {
        inputParam[10] = new SqlDBParameter("@AccountBankPid", SqlDbType.Int, DBConvert.ParseInt(ucbBankAccount.Value));
      }
      if(txtReceiverName.Text.Trim().Length > 0)
      {
        inputParam[11] = new SqlDBParameter("@ReceiverName", SqlDbType.NVarChar, txtReceiverName.Text.Trim().ToString());
      }
      if (txtReceiverBranchName.Text.Trim().Length > 0)
      {
        inputParam[12] = new SqlDBParameter("@ReceiverBranchName", SqlDbType.NVarChar, txtReceiverBranchName.Text.Trim().ToString());
      }
      if (txtReceiverBankNo.Text.Trim().Length > 0)
      {
        inputParam[13] = new SqlDBParameter("@ReceiverBankNo", SqlDbType.NVarChar, txtReceiverBankNo.Text.Trim().ToString());
      }
      if (txtReceiverAddress.Text.Trim().Length > 0)
      {
        inputParam[14] = new SqlDBParameter("@ReceiverAddress", SqlDbType.NVarChar, txtReceiverAddress.Text.Trim().ToString());
      }
      inputParam[15] = new SqlDBParameter("@Status", SqlDbType.Int, chkConfirm.Checked ? 1 : 0);
      inputParam[16] = new SqlDBParameter("@ActionCode", SqlDbType.Int, this.actionCode);
      inputParam[17] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParam[18] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCSupplierPayment_Save", inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        this.viewPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
        return true;
      }
      return false;
    }
    private bool SaveDetail()
    {
      bool success = true;
      // 1. Delete      
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      for (int i = 0; i < listDeletedPid.Count; i++)
      {
        SqlDBParameter[] deleteParam = new SqlDBParameter[] { new SqlDBParameter("@Pid", SqlDbType.BigInt, listDeletedPid[i]) };
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCSupplierPaymentDetail_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ugdData.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          SqlDBParameter[] inputParam = new SqlDBParameter[9];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
          }
          inputParam[1] = new SqlDBParameter("@PaymentPid", SqlDbType.BigInt, this.viewPid);
          inputParam[2] = new SqlDBParameter("@DebitPid", SqlDbType.Int, DBConvert.ParseInt(row["DebitPid"].ToString()));
          inputParam[3] = new SqlDBParameter("@CreditPid", SqlDbType.Int, DBConvert.ParseInt(row["CreditPid"].ToString()));
          if (row["DetailDesc"].ToString().Trim().Length > 0)
          {
            inputParam[4] = new SqlDBParameter("@DetailDesc", SqlDbType.NVarChar, row["DetailDesc"].ToString().Trim());
          }
          inputParam[5] = new SqlDBParameter("@ProposalDetailPid", SqlDbType.BigInt, DBConvert.ParseLong(row["ProposalDetailPid"].ToString()));          
          inputParam[6] = new SqlDBParameter("@Amount", SqlDbType.Float, DBConvert.ParseDouble(row["Amount"].ToString()));
          DateTime invoiceDate = DBConvert.ParseDateTime(row["InvoiceDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          if (invoiceDate != DateTime.MinValue)
          {
            inputParam[7] = new SqlDBParameter("@InvoiceDate", SqlDbType.Date, invoiceDate);
          }
          inputParam[8] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCSupplierPaymentDetail_Save", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }

    private void SaveData()
    {
      if (this.CheckValid())
      {
        bool success = true;
        if (this.SaveMain())
        {
          success = this.SaveDetail();
          //if(success)
          //{
          //  if (chkConfirm.Checked)
          //  {
          //    success = this.SaveConfirm();
          //    if (success)
          //    {
          //      bool isPosted = chkConfirm.Checked;
          //      success = Utility.ACCPostTransaction(this.docTypePid, viewPid, isPosted, SharedObject.UserInfo.UserPid);
          //    }
          //    else
          //    {
          //      success = false;
          //    }
          //  }
          //}
          //else
          //{
          //  success = false;
          //}  
        }
        else
        {
          success = false;
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.LoadData();
      }
      else
      {
        this.SaveSuccess = false;
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }

    /// <summary>
    /// Set Auto Add 4 blank before text of button
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetBlankForTextOfButton(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count > 0)
        {
          this.SetBlankForTextOfButton(ctr);
        }
        else if (ctr.GetType().Name == "Button")
        {
          ctr.Text = string.Format("{0}{1}", "    ", ctr.Text);
        }
      }
    }

    private void SetLanguage()
    {
      btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);

      this.SetBlankForTextOfButton(this);
    }


    private void CalculatePaymentAmount()
    {
      double total = 0;
      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        if (DBConvert.ParseDouble(ugdData.Rows[i].Cells["Amount"].Value) != Double.MinValue)
        {
          total += DBConvert.ParseDouble(ugdData.Rows[i].Cells["Amount"].Value);
        }
      }
      uneTotalAmount.Value = total;
    }
    #endregion Function

    #region Event
    private void ugdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdData);      
      e.Layout.UseFixedHeaders = true;
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      // Allow update, delete, add new
      if (this.status == 0)
      {
        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;        
      }
      else
      {
        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;        
      }

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ProposalDetailPid"].Hidden = true;

      // Set caption column
      e.Layout.Bands[0].Columns["DocCode"].Header.Caption = "Mã chứng từ";
      e.Layout.Bands[0].Columns["ProposalCode"].Header.Caption = "Mã ĐNTT";     
      e.Layout.Bands[0].Columns["TotalAmount"].Header.Caption = "Tổng cộng";
      e.Layout.Bands[0].Columns["Paid"].Header.Caption = "Đã trả";
      e.Layout.Bands[0].Columns["Remain"].Header.Caption = "Còn lại";
      e.Layout.Bands[0].Columns["PercentPayment"].Header.Caption = "% Thanh toán";
      e.Layout.Bands[0].Columns["Amount"].Header.Caption = "Tiền thanh toán";
      e.Layout.Bands[0].Columns["InvoiceDate"].Header.Caption = "Ngày hóa đơn";      
      e.Layout.Bands[0].Columns["DetailDesc"].Header.Caption = "Diễn giải";
      e.Layout.Bands[0].Columns["DebitPid"].Header.Caption = "TK nợ";
      e.Layout.Bands[0].Columns["CreditPid"].Header.Caption = "TK có";

      // Read Only
      e.Layout.Bands[0].Columns["DocCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ProposalCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalAmount"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Paid"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remain"].CellActivation = Activation.ActivateOnly;

      // Column color
      e.Layout.Bands[0].Columns["Amount"].CellAppearance.BackColor = Color.LightGray;

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["DebitPid"].ValueList = ucbddDebit;
      e.Layout.Bands[0].Columns["CreditPid"].ValueList = ucbddCredit;

      //set align
      e.Layout.Bands[0].Columns["DebitPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["CreditPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["DebitPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["DebitPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["CreditPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CreditPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        ugdData.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Amount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";
    }

    private void ugdData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      this.NeedToSave = true;
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      string commandText = string.Empty;
      switch (columnName)
      {
        case "percentpayment":
          if (!this.isLoadingData)
          {
            this.isLoadingData = true;
            double percentPayment = DBConvert.ParseDouble(value);
            double remain = DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value);
            if (remain > 0)
            {
              if (DBConvert.ParseDouble(value) >= 0)
              {
                e.Cell.Row.Cells["Amount"].Value = Math.Round((percentPayment / 100) * remain, this.oddNumber, MidpointRounding.AwayFromZero);
              }
              else
              {
                e.Cell.Row.Cells["Amount"].Value = DBNull.Value;
              }
            }
            this.isLoadingData = false;
          }
          break;
        case "amount":
          if (!this.isLoadingData)
          {
            this.isLoadingData = true;
            double amount = DBConvert.ParseDouble(value);
            double remain = DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value);
            if (remain >= 0)
            {
              e.Cell.Row.Cells["PercentPayment"].Value = Math.Round((amount / remain) * 100, 0, MidpointRounding.AwayFromZero);
            }
            else
            {
              e.Cell.Row.Cells["PercentPayment"].Value = DBNull.Value;
            }
            this.isLoadingData = false;
          }
          this.CalculatePaymentAmount();
          break;
        default:
          break;
      }
    }

    private void ugdData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      string value = e.NewValue.ToString();
      switch (colName)
      {
        case "Amount":
          double oldAmount = DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value);
          double newAmount = DBConvert.ParseDouble(e.NewValue);
          if (newAmount > oldAmount || newAmount <= 0)
          {
            e.Cancel = true;
            DaiCo.Shared.Utility.WindowUtinity.ShowMessageErrorFromText(string.Format("Số tiền đề nghị phải lớn hơn 0 và nhỏ hơn hoặc bằng {0}", oldAmount));
          }
          break;
        case "DebitPid":
          if (ucbddDebit.SelectedRow != null)
          {
            int isLeaf = DBConvert.ParseInt(ucbddDebit.SelectedRow.Cells["IsLeaf"].Value);
            if (isLeaf == 0)
            {
              WindowUtinity.ShowMessageErrorFromText("Bạn không thể hạch toán vào tk cha.");
              e.Cancel = true;
            }
          }
          break;
        case "CreditPid":
          if (ucbddCredit.SelectedRow != null)
          {
            int isLeaf = DBConvert.ParseInt(ucbddCredit.SelectedRow.Cells["IsLeaf"].Value);
            if (isLeaf == 0)
            {
              WindowUtinity.ShowMessageErrorFromText("Bạn không thể hạch toán vào tk cha.");
              e.Cancel = true;
            }
          }
          break;
        default:
          break;
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ugdData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        this.SetNeedToSave();
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdData, "Chi tiết phiếu ĐNTTTT");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugdData);
      }
    }

    private void ugdData_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ugdData.Selected.Rows.Count > 0 || ugdData.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ugdData, new Point(e.X, e.Y));
        }
      }
    }

    private void ugdData_AfterRowInsert(object sender, RowEventArgs e)
    {
          
    }

    private void btnPaymentProposal_Click(object sender, EventArgs e)
    {
      // Check valid
      if (ucbCurrency.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Bạn chưa chọn loại tiền tệ.");
        ucbCurrency.Focus();
        return;
      }
      if (ucbObject.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Bạn chưa chọn đối tượng.");
        ucbObject.Focus();
        return;
      }

      // Transfer data to child form
      int currencyPid = DBConvert.ParseInt(ucbCurrency.Value);
      int objectPid = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["ObjectPid"].Value);
      int objectType = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["ObjectType"].Value);

      DataTable dtDetail = new DataTable();      
      dtDetail.Columns.Add("ProposalDetailPid", typeof(long));      

      DataTable dtSource = (DataTable)ugdData.DataSource;
      DataRow[] rows = dtSource.Select("Pid is null");
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          DataRow rowDetail = dtDetail.NewRow();
          rowDetail["ProposalDetailPid"] = row["ProposalDetailPid"];          
          dtDetail.Rows.Add(rowDetail);
        }
      }

      viewACC_14_006 view = new viewACC_14_006();
      view.currencyPid = currencyPid;
      view.objectPid = objectPid;
      view.objectType = objectType;
      view.dtAddedDocList = dtDetail;
      view.actionCode = this.actionCode;

      Shared.Utility.WindowUtinity.ShowView(view, "Chọn ĐNTT", false, ViewState.ModalWindow);

      // Add selected documents into grid
      if (view.rowSelectedDoc != null && view.rowSelectedDoc.Length > 0)
      {
        int creditPid = 0, debitPid = 0;
        if (this.actionCode == 1) //Tien mat (Phieu chi)
        {
          if (ucbCashFund.SelectedRow != null)
          {
            creditPid = DBConvert.ParseInt(ucbCashFund.SelectedRow.Cells["AccountPid"].Value);
          }
        }
        else //Chuyen khoan (Uy nhiem chi)
        {
          if (ucbCompanyBank.SelectedRow != null)
          {
            creditPid = DBConvert.ParseInt(ucbCompanyBank.SelectedRow.Cells["AccountPid"].Value);
          }
        }
        if (ucbObject.SelectedRow != null)
        {
          debitPid = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["AccountPid"].Value);
        }

        for (int i = 0; i < view.rowSelectedDoc.Length; i++)
        {
          DataRow rowDoc = view.rowSelectedDoc[i];
          DataRow row = dtSource.NewRow();
          row["ProposalDetailPid"] = rowDoc["ProposalDetailPid"];
          row["ProposalCode"] = rowDoc["ProposalCode"];
          row["DocCode"] = rowDoc["DocCode"];
          row["TotalAmount"] = rowDoc["TotalAmount"];
          row["Paid"] = rowDoc["Paid"];
          row["Remain"] = rowDoc["Remain"];
          row["PercentPayment"] = 100;
          row["Amount"] = rowDoc["Remain"];               
          row["InvoiceDate"] = rowDoc["InvoiceDate"];
          row["DetailDesc"] = rowDoc["DetailDesc"];
          if (creditPid > 0)
          {
            row["CreditPid"] = creditPid;
          }
          if(debitPid > 0)
          {
            row["DebitPid"] = debitPid;
          }  
          dtSource.Rows.Add(row);
        }
      }
      this.ReadOnlyKeyMainData();
      this.CalculatePaymentAmount();
    }

    private void btnApprove_Click(object sender, EventArgs e)
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[2];
      inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      inputParam[1] = new SqlDBParameter("@ApproveBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCSupplierPayment_Approve", inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        bool isPosted = chkConfirm.Checked;
        bool success = Utility.ACCPostTransaction(this.docTypePid, viewPid, isPosted, SharedObject.UserInfo.UserPid);
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      this.LoadData();
      this.LoadTransationData();
    }

    private void ugdData_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.ReadOnlyKeyMainData();
      this.CalculatePaymentAmount();
    }
  

    private void ucbCurrency_ValueChanged(object sender, EventArgs e)
    {
      if (ucbCurrency.SelectedRow != null)
      {
        uneExchangeRate.Value = DBConvert.ParseDouble(ucbCurrency.SelectedRow.Cells["ExchangeRate"].Value);
        this.oddNumber = DBConvert.ParseInt(ucbCurrency.SelectedRow.Cells["OddNumber"].Value);
        if (DBConvert.ParseInt(ucbCurrency.Value) == 1) //VND
        {
          uneExchangeRate.ReadOnly = true;
        }
        else
        {
          uneExchangeRate.ReadOnly = false;
        }
      }
      else
      {
        uneExchangeRate.Value = DBNull.Value;
      }
    }  

    private void ucbObject_ValueChanged(object sender, EventArgs e)
    {
      if(ucbObject.SelectedRow != null)
      {
        string cmd = string.Format(@"SELECT Pid, BankAccount, BankName, BankBranch, IsDefault
                                    FROM TblACCBankAccount A
                                    WHERE ObjectPid = {0} AND ObjectType = {1}", ucbObject.SelectedRow.Cells["ObjectPid"].Value, ucbObject.SelectedRow.Cells["ObjectType"].Value);

        DataTable dtBankAccount = DataBaseAccess.SearchCommandTextDataTable(cmd);
        Utility.LoadUltraCombo(ucbBankAccount, dtBankAccount, "Pid", "BankName", false, new string[] {"Pid", "IsDefault" });
        if (dtBankAccount.Rows.Count > 0)
        {
          DataRow[] rowBankAccountDefault = dtBankAccount.Select("IsDefault = 1");
          ucbBankAccount.Value = DBConvert.ParseInt(rowBankAccountDefault[0]["Pid"].ToString());
        }
        txtReceiverName.Text = ucbObject.SelectedRow.Cells["ObjectName"].Value.ToString();
      }  
    }

    private void ucbBankAccount_ValueChanged(object sender, EventArgs e)
    {
      if(ucbBankAccount.SelectedRow != null)
      {
        txtReceiverBankNo.Text = ucbBankAccount.SelectedRow.Cells["BankAccount"].Value.ToString();
        txtReceiverBranchName.Text = ucbBankAccount.SelectedRow.Cells["BankName"].Value.ToString();
      }  
    }
    
    private void btnPost_Click(object sender, EventArgs e)
    {
      bool isPosted = chkConfirm.Checked;
      bool success = Utility.ACCPostTransaction(this.docTypePid, viewPid, isPosted, SharedObject.UserInfo.UserPid);
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      this.LoadData();
    }
   
    private void utcDetail_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
    {
      this.LoadTabData();
    }
    

    private void ucbCashFund_ValueChanged(object sender, EventArgs e)
    {
      if(ucbCashFund.SelectedRow != null)
      {
        for(int i = 0; i < ugdData.Rows.Count; i++)
        {
          ugdData.Rows[i].Cells["CreditPid"].Value = DBConvert.ParseInt(ucbCashFund.SelectedRow.Cells["AccountPid"].Value);
        }  
      }  
    }

    private void ucbCompanyBank_ValueChanged(object sender, EventArgs e)
    {
      if (ucbCompanyBank.SelectedRow != null)
      {
        txtBankAccount.Text = ucbCompanyBank.SelectedRow.Cells["BankAccount"].Value.ToString();
        for (int i = 0; i < ugdData.Rows.Count; i++)
        {
          ugdData.Rows[i].Cells["CreditPid"].Value = DBConvert.ParseInt(ucbCompanyBank.SelectedRow.Cells["AccountPid"].Value);
        }
      }
      else
      {
        txtBankAccount.Text = string.Empty;
      }  
    }
    #endregion Event
  }
}