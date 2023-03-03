/*
  Author      : Nguyen Van Tron
  Date        : 1/04/2022
  Description : Customer Payment
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
  public partial class viewACC_14_008 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    public int actionCode = int.MinValue;
    private IList listDeletedPid = new ArrayList();
    private int docTypePid = 250;
    private bool isLoadedPostTransaction = false;
    private int status = 0;
    private DataTable dtObject = new DataTable();
    private int oddNumber = 2;
    private bool isLoadingData = false;
    #endregion Field

    #region Init
    public viewACC_14_008()
    {
      InitializeComponent();
    }

    private void viewACC_14_008_Load(object sender, EventArgs e)
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
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCCustomerPayment_Init");
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
        txtPaymentDesc.ReadOnly = true;        
        btnSave.Enabled = false;
        chkConfirm.Enabled = false;
        if (this.status == 1)
        {          
          btnDebtDocument.Enabled = false;
        }
        else if (this.status == 2)
        {
          
          btnDebtDocument.Enabled = false;
        }
      }      

      this.ReadOnlyKeyMainData();

      //Hide bank information when action code is 1
      if (this.actionCode == 1)
      {
        lbCompanyBank.Visible = false;
        lbBankWarning.Visible = false;
        tbCompanyBank.Visible = false;
        lbBankFeeAmount.Visible = false;
        uneBankFeeAmount.Visible = false;
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
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCCustomerPayment_Load", inputParam);
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
      dtDetail.Columns.Add("ARTranPid", typeof(long));      
      dtDetail.Columns.Add("Amount", typeof(double));

      DataTable dtSource = (DataTable)ugdData.DataSource;      
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          DataRow rowDetail = dtDetail.NewRow();
          rowDetail["Pid"] = row["Pid"];
          rowDetail["ARTranPid"] = row["ARTranPid"];          
          rowDetail["Amount"] = row["Amount"];
          dtDetail.Rows.Add(rowDetail);
        }
      }
      int paramNumber = 2;
      string storeName = "spACCCustomerPayment_CheckValid";

      SqlDBParameter[] inputParam = new SqlDBParameter[paramNumber];
      inputParam[0] = new SqlDBParameter("@AddedDocList", SqlDbType.NVarChar, DBConvert.ParseXMLString(dtDetail));
      inputParam[1] = new SqlDBParameter("@CustomerPaymentPid", SqlDbType.Int, this.viewPid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@ErrorMessage", SqlDbType.NVarChar, string.Empty) };
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
      SqlDBParameter[] inputParam = new SqlDBParameter[12];
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
      inputParam[9] = new SqlDBParameter("@ActionCode", SqlDbType.Int, this.actionCode);
      inputParam[10] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParam[11] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCCustomerPayment_Save", inputParam, outputParam);
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
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCCustomerPaymentDetail_Delete", deleteParam, outputParam);
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
          SqlDBParameter[] inputParam = new SqlDBParameter[13];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
          }
          inputParam[1] = new SqlDBParameter("@PaymentPid", SqlDbType.BigInt, this.viewPid);
          int debitPid = DBConvert.ParseInt(row["DebitPid"].ToString());
          if (debitPid > 0)
          {
            inputParam[2] = new SqlDBParameter("@DebitPid", SqlDbType.Int, debitPid);
          }
          int creditPid = DBConvert.ParseInt(row["CreditPid"].ToString());
          if (creditPid > 0)
          {
            inputParam[3] = new SqlDBParameter("@CreditPid", SqlDbType.Int, creditPid);
          }
          if (row["DetailDesc"].ToString().Trim().Length > 0)
          {
            inputParam[4] = new SqlDBParameter("@DetailDesc", SqlDbType.NVarChar, row["DetailDesc"].ToString().Trim());
          }
          inputParam[5] = new SqlDBParameter("@ARTranPid", SqlDbType.BigInt, DBConvert.ParseLong(row["ARTranPid"].ToString()));
          inputParam[6] = new SqlDBParameter("@TotalAmount", SqlDbType.Float, DBConvert.ParseDouble(row["TotalAmount"].ToString()));
          double paymentAmount = DBConvert.ParseDouble(row["Paid"].ToString());
          if(paymentAmount >= 0)
          {
            inputParam[7] = new SqlDBParameter("@PaymentAmount", SqlDbType.Float, paymentAmount);
          }  
          double remainAmount = DBConvert.ParseDouble(row["Remain"].ToString());
          if(remainAmount >= 0)
          {
            inputParam[8] = new SqlDBParameter("@RemainAmount", SqlDbType.Float, remainAmount);
          }
          inputParam[9] = new SqlDBParameter("@AmountPercent", SqlDbType.Float, DBConvert.ParseDouble(row["PercentPayment"].ToString()));
          inputParam[10] = new SqlDBParameter("@Amount", SqlDbType.Float, DBConvert.ParseDouble(row["Amount"].ToString()));
          double bankFeeAmount = DBConvert.ParseDouble(row["BankFeeAmount"].ToString());
          if (bankFeeAmount >= 0)
          {
            inputParam[11] = new SqlDBParameter("@BankFeeAmount", SqlDbType.Float, bankFeeAmount);
          }
          inputParam[12] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCCustomerPaymentDetail_Save", inputParam, outputParam);
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
          if (success)
          {
            if (chkConfirm.Checked)
            {
              this.ConfirmPayment();
              return;
            }
          }
          else
          {
            success = false;
          }
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
      DataTable dtSource = (DataTable)ugdData.DataSource;
      uneTotalAmount.Value = dtSource.Compute("Sum(Amount)", "Amount > 0");
      uneBankFeeAmount.Value = dtSource.Compute("Sum(BankFeeAmount)", "BankFeeAmount > 0");
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
      e.Layout.Bands[0].Columns["ARTranPid"].Hidden = true;      
      e.Layout.Bands[0].Columns["DebitPid"].Hidden = true;
      e.Layout.Bands[0].Columns["CreditPid"].Hidden = true;

      // Set caption column
      e.Layout.Bands[0].Columns["DocCode"].Header.Caption = "Mã chứng từ";
      e.Layout.Bands[0].Columns["SONo"].Header.Caption = "Mã Đơn Hàng";     
      e.Layout.Bands[0].Columns["TotalAmount"].Header.Caption = "Tổng cộng";
      e.Layout.Bands[0].Columns["Paid"].Header.Caption = "Đã trả";
      e.Layout.Bands[0].Columns["Remain"].Header.Caption = "Còn lại";
      e.Layout.Bands[0].Columns["PercentPayment"].Header.Caption = "% Thanh toán";
      e.Layout.Bands[0].Columns["Amount"].Header.Caption = "Tiền thanh toán";
      e.Layout.Bands[0].Columns["BankFeeAmount"].Header.Caption = "Phí ngân hàng";      
      e.Layout.Bands[0].Columns["DetailDesc"].Header.Caption = "Diễn giải";
      e.Layout.Bands[0].Columns["DebitPid"].Header.Caption = "TK nợ";
      e.Layout.Bands[0].Columns["CreditPid"].Header.Caption = "TK có";

      // Read Only
      e.Layout.Bands[0].Columns["DocCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SONo"].CellActivation = Activation.ActivateOnly;
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
        case "bankfeeamount":        
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

    private void btnDebtDocument_Click(object sender, EventArgs e)
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
      dtDetail.Columns.Add("ARTranPid", typeof(long));

      DataTable dtSource = (DataTable)ugdData.DataSource;
      DataRow[] rows = dtSource.Select("Pid is null");
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          DataRow rowDetail = dtDetail.NewRow();
          rowDetail["ARTranPid"] = row["ARTranPid"];
          dtDetail.Rows.Add(rowDetail);
        }
      }

      viewACC_14_009 view = new viewACC_14_009();
      view.currencyPid = currencyPid;
      view.objectPid = objectPid;
      view.objectType = objectType;
      view.dtAddedDocList = dtDetail;
      view.actionCode = this.actionCode;

      Shared.Utility.WindowUtinity.ShowView(view, "Chọn CT Nợ", false, ViewState.ModalWindow);

      // Add selected documents into grid
      if (view.rowSelectedDoc != null && view.rowSelectedDoc.Length > 0)
      {
        int debitPid = 0, creditPid = 0;
        if (this.actionCode == 1) //Tien mat (Phieu chi)
        {
          if (ucbCashFund.SelectedRow != null)
          {
            debitPid = DBConvert.ParseInt(ucbCashFund.SelectedRow.Cells["AccountPid"].Value);
          }
        }
        else //Chuyen khoan (Uy nhiem chi)
        {
          if (ucbCompanyBank.SelectedRow != null)
          {
            debitPid = DBConvert.ParseInt(ucbCompanyBank.SelectedRow.Cells["AccountPid"].Value);
          }
        }
        if (ucbObject.SelectedRow != null)
        {
          creditPid = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["AccountPid"].Value);
        }

        for (int i = 0; i < view.rowSelectedDoc.Length; i++)
        {
          DataRow rowDoc = view.rowSelectedDoc[i];
          DataRow row = dtSource.NewRow();
          row["ARTranPid"] = rowDoc["ARTranPid"];
          row["SONo"] = rowDoc["SONo"];
          row["DocCode"] = rowDoc["DocCode"];
          row["TotalAmount"] = rowDoc["TotalAmount"];
          row["Paid"] = rowDoc["Paid"];
          row["Remain"] = rowDoc["Remain"];
          row["PercentPayment"] = 100;
          row["Amount"] = rowDoc["Remain"];
          if (debitPid > 0)
          {
            row["DebitPid"] = debitPid;
          }
          if (creditPid > 0)
          {
            row["CreditPid"] = creditPid;
          }
          dtSource.Rows.Add(row);
        }
      }
      this.ReadOnlyKeyMainData();
      this.CalculatePaymentAmount();
    }

    private void ConfirmPayment()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[2];
      inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      inputParam[1] = new SqlDBParameter("@ConfirmBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCCustomerPayment_Confirm", inputParam, outputParam);
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
          ugdData.Rows[i].Cells["DebitPid"].Value = DBConvert.ParseInt(ucbCashFund.SelectedRow.Cells["AccountPid"].Value);
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
          ugdData.Rows[i].Cells["DebitPid"].Value = DBConvert.ParseInt(ucbCompanyBank.SelectedRow.Cells["AccountPid"].Value);
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