/*
  Author      : Nguyen Thanh Binh
  Date        : 11/04/2021
  Description : payment voucher
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
  public partial class viewACC_10_013 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private int status = 0;
    private DataTable dtObject = new DataTable();
    private int docTypePid = ConstantClass.Payment_BankDebit;
    public int actionCode = 1;
    private int currency = int.MinValue;
    public int objectTye = int.MinValue;
    public int creditPid = int.MinValue;
    private bool isLoadedPostTransaction = false;
    #endregion Field

    #region Init
    public viewACC_10_013()
    {
      InitializeComponent();
    }

    private void viewACC_10_013_Load(object sender, EventArgs e)
    {
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
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCBankDebit_Init", inputParam);
      Utility.LoadUltraCombo(ucbCompanyBank, dsInit.Tables[0], "Pid", "BankName", false, new string[] { "Pid", "BankAccount", "AccountPid", "CurrencyPid" });
      Utility.LoadUltraCombo(ucbObject, dsInit.Tables[1], "ObjectPid", "Object", false, new string[] { "ObjectPid", "ObjectType", "Address", "BankAccount", "BankName" });
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[2], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate" });
      Utility.LoadUltraCombo(ucbAccountBank, dsInit.Tables[3], "Pid", "BankName", false, new string[] { "Pid", "BankBranch", "BankAccount", "Holder", "HolderIDNumber", "HolderIDCardDate", "HolderCityPid" });
      Utility.LoadUltraCombo(ucbLoanReceipt, dsInit.Tables[4], "Value", "Display", false, "Value");


      //Init dropdown
      Utility.LoadUltraCBAccountList(ucbddDebit);
      Utility.LoadUltraCBAccountList(ucbddCredit);
      Utility.LoadUltraCombo(ucbddProjectPid, dsInit.Tables[6], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbddSegmentPid, dsInit.Tables[7], "Value", "Display", false, "Value");


      this.dtObject = dsInit.Tables[5];
      // Set Language
      //this.SetLanguage();
    }

    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        txtBankDebitCode.ReadOnly = true;
        udtBankDebitDate.ReadOnly = true;
        ucbCurrency.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
        txtBankDesc.ReadOnly = true;
        btnSave.Enabled = false;
        chkConfirm.Enabled = false;
      }
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtBankDebitCode.Text = dtMain.Rows[0]["BankDebitCode"].ToString();
        udtBankDebitDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["BankDebitDate"]);
        ucbObject.Value = DBConvert.ParseInt(dtMain.Rows[0]["ObjectPid"]);
        if (DBConvert.ParseInt(ucbLoanReceipt.Value) != int.MinValue)
        {
          ucbLoanReceipt.Value = DBConvert.ParseInt(dtMain.Rows[0]["LoanReceiptPid"]);
        }
        ucbCompanyBank.Value = DBConvert.ParseInt(dtMain.Rows[0]["CompanyBankPid"]);
        txtBankNumber.Text = dtMain.Rows[0]["BankAccount"].ToString();
        ucbCurrency.Value = DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"]);
        uneExchangeRate.Value = dtMain.Rows[0]["ExchangeRate"];
        txtBankDesc.Text = dtMain.Rows[0]["BankDebitDesc"].ToString();

        if (DBConvert.ParseInt(dtMain.Rows[0]["AccountBankPid"]) != int.MinValue)
        {
          ucbAccountBank.Value = DBConvert.ParseInt(dtMain.Rows[0]["AccountBankPid"]);
        }
        txtRecBranch.Text = dtMain.Rows[0]["ReceiverBranchName"].ToString();
        txtReceiverName.Text = dtMain.Rows[0]["ReceiverName"].ToString();
        txtRecAccount.Text = dtMain.Rows[0]["ReceiverBankNo"].ToString();
        txtRecIDCardNumber.Text = dtMain.Rows[0]["ReceiverIDNumber"].ToString();
        if (DBConvert.ParseDateTime(dtMain.Rows[0]["ReceiverIDCardDate"]) != DateTime.MinValue)
        {
          udtRecCardDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["ReceiverIDCardDate"]);
        }
        txtHolderCity.Text = dtMain.Rows[0]["HolderCity"].ToString();
        txtRecAddress.Text = dtMain.Rows[0]["ReceiverAddress"].ToString();
        chkConfirm.Checked = (bool)dtMain.Rows[0]["Status"];
        this.status = (bool)dtMain.Rows[0]["Status"] ? 1 : 0;
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
          txtBankDebitCode.Text = outputParam[0].Value.ToString();
          udtBankDebitDate.Value = DateTime.Now;
          udtRecCardDate.Value = null;
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

        case "utpcListBankDebitDetail":
          //this.LoadData();
          break;
        case "utpcPostBankDebit":
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

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();

      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCBankDebit_Load", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        this.LoadMainData(dsSource.Tables[0]);
        //ugdData.DataSource = dsSource.Tables[1];
      }
      this.LoadBankDebitDetail();
      this.SetStatusControl();
      this.NeedToSave = false;
    }

    private void LoadBankDebitDetail()
    {
      DBParameter[] inputParamMain = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
      DataTable dtNoInvoice = DataBaseAccess.SearchStoreProcedureDataTable("spACCBankDebitDetail_Load", inputParamMain);
      ugdData.DataSource = dtNoInvoice;
    }

    /// <summary>
    /// Load transaction
    /// </summary>
    private void LoadTransationData()
    {
      ugdTransaction.SetDataSource(this.docTypePid, this.viewPid);
    }

    private bool CheckValid()
    {
      //check master
      if (udtBankDebitDate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày chứng từ không được để trống!!!");
        udtBankDebitDate.Focus();
        return false;
      }
      if (ucbCurrency.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Loại tiền tệ không được để trống!!!");
        ucbCurrency.Focus();
        return false;
      }

      if (uneExchangeRate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Tỉ giá không được để trống!!!");
        uneExchangeRate.Focus();
        return false;
      }

      //check detail

      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        UltraGridRow row = ugdData.Rows[i];
        row.Selected = false;
        if (DBConvert.ParseDouble(row.Cells["SubAmount"].Value) <= 0)
        {
          row.Cells["SubAmount"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Tiền hàng không được để trống và phải lớn hơn 0.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
      }
      return true;
    }

    private bool SaveMain()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[21];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      }
      if (txtBankDesc.Text.Trim().Length > 0)
      {
        inputParam[1] = new SqlDBParameter("@BankDebitDesc", SqlDbType.NVarChar, txtBankDesc.Text.Trim().ToString());
      }
      inputParam[2] = new SqlDBParameter("@BankDebitDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtBankDebitDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      if (ucbObject.SelectedRow != null)
      {
        inputParam[3] = new SqlDBParameter("@ObjectPid", SqlDbType.Int, DBConvert.ParseInt(ucbObject.Value));
      }
      inputParam[4] = new SqlDBParameter("@ObjectType", SqlDbType.Int, this.objectTye);
      if (ucbCompanyBank.SelectedRow != null)
      {
        inputParam[5] = new SqlDBParameter("@CompanyBankPid", SqlDbType.Int, DBConvert.ParseInt(ucbCompanyBank.Value));
      }
      if (ucbLoanReceipt.SelectedRow != null)
      {
        inputParam[6] = new SqlDBParameter("@LoanReceiptPid", SqlDbType.Int, DBConvert.ParseInt(ucbLoanReceipt.Value));
      }
      inputParam[7] = new SqlDBParameter("@CurrencyPid", SqlDbType.Int, DBConvert.ParseInt(ucbCurrency.Value));
      inputParam[8] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, uneExchangeRate.Value);
      if (ucbAccountBank.SelectedRow != null)
      {
        inputParam[9] = new SqlDBParameter("@AccountBankPid", SqlDbType.Int, DBConvert.ParseInt(ucbAccountBank.Value));
      }
      if (txtReceiverName.Text.Trim().Length > 0)
      {
        inputParam[10] = new SqlDBParameter("@ReceiverName", SqlDbType.NVarChar, txtReceiverName.Text.Trim().ToString());
      }
      if (txtRecBranch.Text.Trim().Length > 0)
      {
        inputParam[11] = new SqlDBParameter("@ReceiverBranchName", SqlDbType.NVarChar, txtRecBranch.Text.Trim().ToString());
      }
      if (txtRecAccount.Text.Trim().Length > 0)
      {
        inputParam[12] = new SqlDBParameter("@ReceiverBankNo", SqlDbType.NVarChar, txtRecAccount.Text.Trim().ToString());
      }
      if (txtRecIDCardNumber.Text.Trim().Length > 0)
      {
        inputParam[13] = new SqlDBParameter("@ReceiverIDNumber", SqlDbType.NVarChar, txtRecIDCardNumber.Text.Trim().ToString());
      }
      if (DBConvert.ParseDateTime(udtRecCardDate.Value) != DateTime.MinValue)
      {
        inputParam[14] = new SqlDBParameter("@ReceiverIDCardDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtRecCardDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME)); ;
      }
      if (txtHolderCity.Text.Trim().Length > 0)
      {
        inputParam[15] = new SqlDBParameter("@HolderCity", SqlDbType.NVarChar, txtHolderCity.Text.Trim().ToString());
      }
      if (txtRecAddress.Text.Trim().Length > 0)
      {
        inputParam[16] = new SqlDBParameter("@ReceiverAddress", SqlDbType.NVarChar, txtRecAddress.Text.Trim().ToString());
      }

      inputParam[17] = new SqlDBParameter("@ActionCode", SqlDbType.Int, this.actionCode);
      inputParam[18] = new SqlDBParameter("@Status", SqlDbType.Bit, chkConfirm.Checked ? 1 : 0);
      inputParam[19] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParam[20] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCBankDebit_Save", inputParam, outputParam);
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
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCBankDebit_Delete", deleteParam, outputParam);
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
          SqlDBParameter[] inputParam = new SqlDBParameter[10];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
          }
          inputParam[1] = new SqlDBParameter("@BankDebitPid", SqlDbType.BigInt, this.viewPid);

          if (row["DetailDesc"].ToString().Trim().Length > 0)
          {
            inputParam[2] = new SqlDBParameter("@DetailDesc", SqlDbType.NVarChar, row["DetailDesc"].ToString());
          }
          inputParam[3] = new SqlDBParameter("@DebitPid", SqlDbType.Int, DBConvert.ParseInt(row["DebitPid"].ToString()));
          inputParam[4] = new SqlDBParameter("@CreditPid", SqlDbType.Int, DBConvert.ParseInt(row["CreditPid"].ToString()));
          inputParam[5] = new SqlDBParameter("@SubAmount", SqlDbType.Float, DBConvert.ParseDouble(row["SubAmount"].ToString()));
          inputParam[6] = new SqlDBParameter("@TotalAmount", SqlDbType.Float, DBConvert.ParseDouble(row["TotalAmount"].ToString()));
          if (DBConvert.ParseInt(row["ProjectPid"].ToString()) >= 0)
          {
            inputParam[7] = new SqlDBParameter("@ProjectPid", SqlDbType.Int, DBConvert.ParseInt(row["ProjectPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["SegmentPid"].ToString()) >= 0)
          {
            inputParam[8] = new SqlDBParameter("@SegmentPid", SqlDbType.Int, DBConvert.ParseInt(row["SegmentPid"].ToString()));
          }
          inputParam[9] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCBankDebitDetail_Save", inputParam, outputParam);
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
              bool isPosted = chkConfirm.Checked;
              success = Utility.ACCPostTransaction(this.docTypePid, viewPid, isPosted, SharedObject.UserInfo.UserPid);
            }
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


    private void TotalAmount()
    {
      double total = 0;
      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        if (DBConvert.ParseDouble(ugdData.Rows[i].Cells["TotalAmount"].Value) != Double.MinValue)
        {
          total += DBConvert.ParseDouble(ugdData.Rows[i].Cells["TotalAmount"].Value);
        }
      }
    }
    #endregion Function

    #region Event
    private void ugdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdData);      
      e.Layout.UseFixedHeaders = true;
      e.Layout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True;
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      // Allow update, delete, add new
      if (this.status == 0)
      {
        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnTop;
      }
      else
      {
        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      }
      e.Layout.Bands[0].Columns["DebitPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["CreditPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["ProjectPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["SegmentPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Set caption column
      e.Layout.Bands[0].Columns["DetailDesc"].Header.Caption = "Diễn giải";
      e.Layout.Bands[0].Columns["DebitPid"].Header.Caption = "Tài khoản nợ";
      e.Layout.Bands[0].Columns["CreditPid"].Header.Caption = "Tài khoản có";
      e.Layout.Bands[0].Columns["SubAmount"].Header.Caption = "Số tiền";
      e.Layout.Bands[0].Columns["TotalAmount"].Header.Caption = "Tổng thanh toán";
      e.Layout.Bands[0].Columns["SubAmountExchange"].Header.Caption = "Tiền quy đổi";
      e.Layout.Bands[0].Columns["TotalAmountExchange"].Header.Caption = "Tổng tiền quy đổi";
      e.Layout.Bands[0].Columns["ProjectPid"].Header.Caption = "Dự án";
      e.Layout.Bands[0].Columns["SegmentPid"].Header.Caption = "Khoản mục chi phí";

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["DebitPid"].ValueList = ucbddDebit;
      e.Layout.Bands[0].Columns["CreditPid"].ValueList = ucbddCredit;
      e.Layout.Bands[0].Columns["ProjectPid"].ValueList = ucbddProjectPid;
      e.Layout.Bands[0].Columns["SegmentPid"].ValueList = ucbddSegmentPid;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["DebitPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["DebitPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["CreditPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CreditPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["ProjectPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ProjectPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["SegmentPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["SegmentPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;



      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalAmount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "Tổng = {0:#,##0.##}";
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalAmountExchange"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "Tổng = {0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";

      e.Layout.Bands[0].Columns["CreditPid"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SubAmountExchange"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalAmountExchange"].CellActivation = Activation.ActivateOnly;
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
        case "subamount":
          try
          {
            if (DBConvert.ParseDouble(value) >= 0)
            {
              e.Cell.Row.Cells["TotalAmount"].Value = DBConvert.ParseDouble(value);
              if (DBConvert.ParseInt(ucbCurrency.Value) == 1)
              {
                e.Cell.Row.Cells["SubAmountExchange"].Value = Math.Round(DBConvert.ParseDouble(value) * DBConvert.ParseDouble(uneExchangeRate.Value), 0, MidpointRounding.AwayFromZero);
                e.Cell.Row.Cells["TotalAmountExchange"].Value = Math.Round(DBConvert.ParseDouble(value) * DBConvert.ParseDouble(uneExchangeRate.Value), 0, MidpointRounding.AwayFromZero);
              }
              else
              {
                e.Cell.Row.Cells["SubAmountExchange"].Value = Math.Round(DBConvert.ParseDouble(value) * DBConvert.ParseDouble(uneExchangeRate.Value), 2, MidpointRounding.AwayFromZero);
                e.Cell.Row.Cells["TotalAmountExchange"].Value = Math.Round(DBConvert.ParseDouble(value) * DBConvert.ParseDouble(uneExchangeRate.Value), 2, MidpointRounding.AwayFromZero);
              }
            }
          }
          catch
          {
            e.Cell.Row.Cells["SubAmountExchange"].Value = DBNull.Value;
            e.Cell.Row.Cells["TotalAmountExchange"].Value = DBNull.Value;
            e.Cell.Row.Cells["TotalAmount"].Value = DBNull.Value;
          }
          break;
        case "totalamount":
          try
          {
            //this.TotalAmount();
          }
          catch
          {

          }
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
        case "SubAmount":
          if (DBConvert.ParseDouble(value) < 0)
          {
            WindowUtinity.ShowMessageErrorFromText("Số tiền không được nhỏ hơn 0.");
            e.Cancel = true;
          }          
          break;
        case "DebitPid":
          if(ucbddDebit.SelectedRow != null)
          {
            int isLeaf = DBConvert.ParseInt(ucbddDebit.SelectedRow.Cells["IsLeaf"].Value);
            if(isLeaf == 0)
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
      Utility.ExportToExcelWithDefaultPath(ugdData, "Chi tiết nợ ngân hàng");
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


    private void ucbCurrency_ValueChanged(object sender, EventArgs e)
    {
      if (ucbCurrency.SelectedRow != null)
      {
        this.currency = DBConvert.ParseInt(ucbCurrency.Value);
        uneExchangeRate.Value = ucbCurrency.SelectedRow.Cells["ExchangeRate"].Value;
        if (DBConvert.ParseInt(ucbCurrency.SelectedRow.Cells["Pid"].Value) == 1)
        {
          uneExchangeRate.ReadOnly = true;
        }
        else
        {
          uneExchangeRate.ReadOnly = false;
        }
      }
    }


    private void ucbObjectType_ValueChanged(object sender, EventArgs e)
    {
      string cmd = string.Empty;
    }

    private void ucbObject_ValueChanged(object sender, EventArgs e)
    {
      if (ucbObject.SelectedRow != null)
      {
        this.objectTye = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["ObjectType"].Value.ToString());
        txtRecBranch.Text = ucbObject.SelectedRow.Cells["BankName"].Value.ToString();
        txtRecAccount.Text = ucbObject.SelectedRow.Cells["BankAccount"].Value.ToString();
        txtRecAddress.Text = ucbObject.SelectedRow.Cells["Address"].Value.ToString();
      }
    }


    private void ugdData_AfterRowInsert(object sender, RowEventArgs e)
    {
      e.Row.Cells["SubAmount"].Value = 0;
      e.Row.Cells["TotalAmount"].Value = 0;
      e.Row.Cells["SubAmountExchange"].Value = 0;
      e.Row.Cells["TotalAmountExchange"].Value = 0;
      if (this.creditPid != int.MinValue)
      {
        e.Row.Cells["CreditPid"].Value = this.creditPid;
      }
      else
      {
        e.Row.Cells["CreditPid"].Value = DBNull.Value;
      }  
      //read only
      ucbCurrency.ReadOnly = true;
      uneExchangeRate.ReadOnly = true;
      ucbObject.ReadOnly = true;
    }

    private void utcDetail_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
    {
      this.LoadTabData();
    }

    private void ugdData_AfterRowsDeleted(object sender, EventArgs e)
    {
      if (ugdData.Rows.Count == 0)
      {
        ucbObject.ReadOnly = false;
        ucbCurrency.ReadOnly = false;
      }
      this.TotalAmount();
    }

    private void ucbCompanyBank_ValueChanged(object sender, EventArgs e)
    {
      if (ucbCompanyBank.SelectedRow != null)
      {
        txtBankNumber.Text = ucbCompanyBank.SelectedRow.Cells["BankAccount"].Value.ToString();
        this.creditPid = DBConvert.ParseInt(ucbCompanyBank.SelectedRow.Cells["AccountPid"].Value.ToString());
        ucbCurrency.Value = DBConvert.ParseInt(ucbCompanyBank.SelectedRow.Cells["CurrencyPid"].Value.ToString());
      }
      else
      {
        this.creditPid = int.MinValue;
      }  
    }

    private void ucbAccountBank_ValueChanged(object sender, EventArgs e)
    {
      //if (ucbAccountBank.SelectedRow != null)
      //{
      //  txtRecBranch.Text = ucbAccountBank.SelectedRow.Cells["BankBranch"].Value.ToString();
      //  txtRecAccount.Text = ucbAccountBank.SelectedRow.Cells["BankAccount"].Value.ToString();
      //  txtReceiverName.Text = ucbAccountBank.SelectedRow.Cells["Holder"].Value.ToString();
      //  txtRecIDCardNumber.Text = ucbAccountBank.SelectedRow.Cells["HolderIDNumber"].Value.ToString();
      //  udtRecCardDate.Value = DBConvert.ParseDateTime(ucbAccountBank.SelectedRow.Cells["HolderIDCardDate"].Value.ToString());
      //}
    }
    #endregion Event
  }
}