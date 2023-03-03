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
  public partial class viewACC_10_006 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    public int status = 0;
    private DataTable dtObject = new DataTable();
    private int docTypePid = ConstantClass.Payment_Voucher;
    public int actionCode = int.MinValue;
    private int currency = int.MinValue;
    #endregion Field

    #region Init
    public viewACC_10_006()
    {
      InitializeComponent();
    }

    private void viewACC_10_006_Load(object sender, EventArgs e)
    {
      // Add ask before closing form even if user change data
      this.SetAutoAskSaveWhenCloseForm(ugbInformation);
      this.SetBlankForTextOfButton(this);
      if (this.actionCode == 1)
      {
        btnAdd.Visible = false;
      }
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
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCPaymentVoucher_Init", inputParam);
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[0], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate" });
      Utility.LoadUltraCombo(ucbCashFund, dsInit.Tables[1], "Value", "Display", false, new string[] { "Value", "AccountPid" });
      Utility.LoadUltraCombo(ucbLoanReceipt, dsInit.Tables[2], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbLoanReceiptPlan, dsInit.Tables[3], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbObjectType, dsInit.Tables[4], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbObject, dsInit.Tables[5], "ObjectPid", "Object", false, new string[] { "ObjectPid", "ObjectType" });

      //Init dropdown
      Utility.LoadUltraCBAccountList(ucbddDebit);
      Utility.LoadUltraCBAccountList(ucbddCredit);
      Utility.LoadUltraCombo(ucbddInputVATDocType, dsInit.Tables[7], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbddProjectPid, dsInit.Tables[8], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbddSegmentPid, dsInit.Tables[9], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbddUnCostConstructionPid, dsInit.Tables[10], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbddCostObjectPid, dsInit.Tables[11], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbddEInvoiceTypePid, dsInit.Tables[12], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbddCostCenterPid, dsInit.Tables[13], "Value", "Display", false, "Value");

      this.dtObject = dsInit.Tables[5];

      if (this.actionCode == 2) //Từ thanh toán trực tiếp
      {
        btnAdd.Text = "    Chọn đề nghị";
      }
        // Set Language
        //this.SetLanguage();
      }

    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        txtPaymentCode.ReadOnly = true;
        udtPaymentDate.ReadOnly = true;
        ucbCurrency.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
        txtPaymentDesc.ReadOnly = true;
        btnSave.Enabled = false;
        chkConfirm.Enabled = false;
      }
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtPaymentCode.Text = dtMain.Rows[0]["PaymentCode"].ToString();
        udtPaymentDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["PaymentDate"]);
        ucbCurrency.Value = DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"]);
        uneExchangeRate.Value = dtMain.Rows[0]["ExchangeRate"];
        ucbCashFund.Value = DBConvert.ParseInt(dtMain.Rows[0]["CashFundPid"]);
        if (DBConvert.ParseInt(ucbLoanReceipt.Value) != int.MinValue)
        {
          ucbLoanReceipt.Value = DBConvert.ParseInt(dtMain.Rows[0]["LoanReceiptPid"]);
        }
        if (DBConvert.ParseInt(ucbLoanReceiptPlan.Value) != int.MinValue)
        {
          ucbLoanReceiptPlan.Value = DBConvert.ParseInt(dtMain.Rows[0]["LoanReceiptPlanPid"]);
        }
        ucbObjectType.Value = DBConvert.ParseInt(dtMain.Rows[0]["ObjectType"]);
        ucbObject.Value = DBConvert.ParseInt(dtMain.Rows[0]["ObjectPid"]);
        txtReceiverName.Text = dtMain.Rows[0]["ReceiverName"].ToString();
        txtPaymentDesc.Text = dtMain.Rows[0]["PaymentDesc"].ToString();
        uneTotalAmount.Value = dtMain.Rows[0]["TotalAmount"];
        lbStatusDes.Text = (bool)dtMain.Rows[0]["Status"] ? "Đã xác nhận" : "Tạo mới";
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
        case "utpcListPaymentVoucher":
          //this.LoadData();
          break;
        case "utpcPostPaymentVoucher":
          if (chkConfirm.Checked)
            this.LoadTransationData();
          break;
        default:
          break;
      }
    }

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();

      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCPaymentVoucher_Load", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 1)
      {
        this.LoadMainData(dsSource.Tables[0]);
        ugdData.DataSource = dsSource.Tables[1];
        lbCount.Text = string.Format(@"Đếm {0}", ugdData.Rows.FilteredInRowCount > 0 ? ugdData.Rows.FilteredInRowCount : 0);
      }

      this.SetStatusControl();
      this.NeedToSave = false;
    }
    /// <summary>
    /// Load transaction
    /// </summary>
    private void LoadTransationData()
    {
      grdPostTran.SetDataSource(this.docTypePid, this.viewPid);
    }
    private bool CheckValid()
    {
      //check master
      if (udtPaymentDate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày chứng từ không được để trống!!!");
        udtPaymentDate.Focus();
        return false;
      }
      if (ucbCurrency.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Loại tiền tệ không được để trống!!!");
        ucbCurrency.Focus();
        return false;
      }
      if (ucbCashFund.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Quỹ tiền mặt không được để trống!!!");
        ucbCashFund.Focus();
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
        if (DBConvert.ParseDouble(row.Cells["DebitPid"].Value) <= 0)
        {
          row.Cells["DebitPid"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Tài khoản nợ không được để trống.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        if (DBConvert.ParseDouble(row.Cells["CreditPid"].Value) <= 0)
        {
          row.Cells["CreditPid"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Tài khoản có không được để trống.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        if (DBConvert.ParseDouble(row.Cells["SubAmount"].Value) <= 0)
        {
          row.Cells["SubAmount"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Tiền hàng không được để trống và phải lớn hơn 0.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        if (this.actionCode == 5) // advance
        {
          SqlDBParameter[] input = new SqlDBParameter[2];
          SqlDBParameter[] output = new SqlDBParameter[1];
          input[0] = new SqlDBParameter("@PaymentAdvanceDetailPid", SqlDbType.BigInt, DBConvert.ParseDouble(row.Cells["PaymentAdvanceDetailPid"].Value));
          input[1] = new SqlDBParameter("@Amount", SqlDbType.Float, DBConvert.ParseDouble(row.Cells["SubAmount"].Value));

          output[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, 0);
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentRequestReceivingCheckAdvance", input, output);
          if ((output == null) || (output[0].Value.ToString() == "0"))
          {
            row.Cells["SubAmount"].Appearance.BackColor = Color.Yellow;
            WindowUtinity.ShowMessageErrorFromText("Tiền chi không được lớn hơn số tiền tạm ứng.");
            row.Selected = true;
            ugdData.ActiveRowScrollRegion.FirstRow = row;
            return false;
          }
        }
        else if (this.actionCode == 2) // direct
        {
          SqlDBParameter[] input = new SqlDBParameter[2];
          SqlDBParameter[] output = new SqlDBParameter[1];
          input[0] = new SqlDBParameter("@PaymentRequestDetailPid", SqlDbType.BigInt, DBConvert.ParseDouble(row.Cells["PaymentRequestDetailPid"].Value));
          input[1] = new SqlDBParameter("@Amount", SqlDbType.Float, DBConvert.ParseDouble(row.Cells["SubAmount"].Value));

          output[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, 0);
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentVoucherForDirect_Check", input, output);
          if ((output == null) || (output[0].Value.ToString() == "0"))
          {
            row.Cells["SubAmount"].Appearance.BackColor = Color.Yellow;
            WindowUtinity.ShowMessageErrorFromText("Tiền chi không được lớn hơn số tiền đề nghị thanh toán trực tiếp.");
            row.Selected = true;
            ugdData.ActiveRowScrollRegion.FirstRow = row;
            return false;
          }
        }
      }
      return true;
    }

    private bool SaveMain()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[15];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      }
      if (txtPaymentDesc.Text.Trim().Length > 0)
      {
        inputParam[1] = new SqlDBParameter("@PaymentDesc", SqlDbType.NVarChar, txtPaymentDesc.Text.Trim().ToString());
      }
      inputParam[2] = new SqlDBParameter("@PaymentDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtPaymentDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      if (txtReceiverName.Text.Trim().Length > 0)
      {
        inputParam[3] = new SqlDBParameter("@ReceiverName", SqlDbType.NVarChar, txtReceiverName.Text.Trim().ToString());
      }
      if (ucbCashFund.SelectedRow != null)
      {
        inputParam[4] = new SqlDBParameter("@CashFundPid", SqlDbType.Int, DBConvert.ParseInt(ucbCashFund.Value));
      }
      if (ucbLoanReceipt.SelectedRow != null)
      {
        inputParam[5] = new SqlDBParameter("@LoanReceiptPid", SqlDbType.Int, DBConvert.ParseInt(ucbLoanReceipt.Value));
      }
      if (ucbLoanReceiptPlan.SelectedRow != null)
      {
        inputParam[6] = new SqlDBParameter("@LoanReceiptPlanPid", SqlDbType.Int, DBConvert.ParseInt(ucbLoanReceiptPlan.Value));
      }
      inputParam[7] = new SqlDBParameter("@CurrencyPid", SqlDbType.Int, DBConvert.ParseInt(ucbCurrency.Value));
      inputParam[8] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, uneExchangeRate.Value);
      if (ucbObjectType.SelectedRow != null)
      {
        inputParam[9] = new SqlDBParameter("@ObjectType", SqlDbType.Int, DBConvert.ParseInt(ucbObjectType.Value));
      }
      if (ucbObject.SelectedRow != null)
      {
        inputParam[10] = new SqlDBParameter("@Object", SqlDbType.Int, DBConvert.ParseInt(ucbObject.Value));
      }
      inputParam[11] = new SqlDBParameter("@ActionCode", SqlDbType.Int, this.actionCode);
      inputParam[12] = new SqlDBParameter("@Status", SqlDbType.Bit, chkConfirm.Checked ? 1 : 0);
      inputParam[13] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParam[14] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentVoucherMaster_Save", inputParam, outputParam);
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
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentVoucherDetail_Delete", deleteParam, outputParam);
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
          SqlDBParameter[] inputParam = new SqlDBParameter[25];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
          }
          inputParam[1] = new SqlDBParameter("@PaymentVoucherPid", SqlDbType.BigInt, this.viewPid);

          if (DBConvert.ParseInt(row["PaymentAdvanceDetailPid"].ToString()) > 0)
          {
            inputParam[2] = new SqlDBParameter("@PaymentAdvanceDetailPid", SqlDbType.Int, DBConvert.ParseInt(row["PaymentAdvanceDetailPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["PaymentRequestDetailPid"].ToString()) > 0)
          {
            inputParam[3] = new SqlDBParameter("@PaymentRequestDetailPid", SqlDbType.Int, DBConvert.ParseInt(row["PaymentRequestDetailPid"].ToString()));
          }
          if (row["DetailDesc"].ToString().Trim().Length > 0)
          {
            inputParam[4] = new SqlDBParameter("@DetailDesc", SqlDbType.NVarChar, row["DetailDesc"].ToString());
          }
          inputParam[5] = new SqlDBParameter("@DebitPid", SqlDbType.Int, DBConvert.ParseInt(row["DebitPid"].ToString()));
          inputParam[6] = new SqlDBParameter("@CreditPid", SqlDbType.Int, DBConvert.ParseInt(row["CreditPid"].ToString()));
          inputParam[7] = new SqlDBParameter("@SubAmount", SqlDbType.Float, DBConvert.ParseDouble(row["SubAmount"].ToString()));
          inputParam[8] = new SqlDBParameter("@TaxPercent", SqlDbType.Float, DBConvert.ParseDouble(row["TaxPercent"].ToString()));
          inputParam[9] = new SqlDBParameter("@TaxAmount", SqlDbType.Float, DBConvert.ParseDouble(row["TaxAmount"].ToString()));
          inputParam[10] = new SqlDBParameter("@TotalAmount", SqlDbType.Float, DBConvert.ParseDouble(row["TotalAmount"].ToString()));
          if (row["TaxNumber"].ToString().Trim().Length > 0)
          {
            inputParam[11] = new SqlDBParameter("@TaxNumber", SqlDbType.VarChar, row["TaxNumber"].ToString());
          }
          if (DBConvert.ParseDateTime(row["VATDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            inputParam[12] = new SqlDBParameter("@VATDate", SqlDbType.Date, DBConvert.ParseDateTime(row["VATDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
          }
          if (row["VATInvoiceNo"].ToString().Trim().Length > 0)
          {
            inputParam[13] = new SqlDBParameter("@VATInvoiceNo", SqlDbType.VarChar, row["VATInvoiceNo"].ToString());
          }
          if (row["VATFormNo"].ToString().Trim().Length > 0)
          {
            inputParam[14] = new SqlDBParameter("@VATFormNo", SqlDbType.VarChar, row["VATFormNo"].ToString());
          }
          if (row["VATSymbol"].ToString().Trim().Length > 0)
          {
            inputParam[15] = new SqlDBParameter("@VATSymbol", SqlDbType.VarChar, row["VATSymbol"].ToString());
          }
          if (DBConvert.ParseInt(row["InputVATDocType"].ToString()) > 0)
          {
            inputParam[16] = new SqlDBParameter("@InputVATDocType", SqlDbType.Int, DBConvert.ParseInt(row["InputVATDocType"].ToString()));
          }
          if (DBConvert.ParseInt(row["ProjectPid"].ToString()) >= 0)
          {
            inputParam[17] = new SqlDBParameter("@ProjectPid", SqlDbType.Int, DBConvert.ParseInt(row["ProjectPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["SegmentPid"].ToString()) >= 0)
          {
            inputParam[18] = new SqlDBParameter("@SegmentPid", SqlDbType.Int, DBConvert.ParseInt(row["SegmentPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["UnCostConstructionPid"].ToString()) >= 0)
          {
            inputParam[19] = new SqlDBParameter("@UnCostConstructionPid", SqlDbType.Int, DBConvert.ParseInt(row["UnCostConstructionPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["CostObjectPid"].ToString()) >= 0)
          {
            inputParam[20] = new SqlDBParameter("@CostObjectPid", SqlDbType.Int, DBConvert.ParseInt(row["CostObjectPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["EInvoiceTypePid"].ToString()) >= 0)
          {
            inputParam[21] = new SqlDBParameter("@EInvoiceTypePid", SqlDbType.Int, DBConvert.ParseInt(row["EInvoiceTypePid"].ToString()));
          }
          if (DBConvert.ParseInt(row["CostCenterPid"].ToString()) >= 0)
          {
            inputParam[22] = new SqlDBParameter("@CostCenterPid", SqlDbType.Int, DBConvert.ParseInt(row["CostCenterPid"].ToString()));
          }
          inputParam[23] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          inputParam[24] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentVoucherDetail_Save", inputParam, outputParam);
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
          if (this.SaveDetail())
          {
            if (chkConfirm.Checked)
            {
              bool isPosted = chkConfirm.Checked;
              success = Utility.ACCPostTransaction(this.docTypePid, this.viewPid, isPosted, SharedObject.UserInfo.UserPid);
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
      uneTotalAmount.Value = total;
    }
    #endregion Function

    #region Event
    private void ugdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdData);
      e.Layout.AutoFitStyle = AutoFitStyle.None;
      
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
      if (this.actionCode == 2) //Từ thanh toán trực tiếp
      {
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      }

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Columns["PaymentAdvanceDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["PaymentRequestDetailPid"].Hidden = true;

      e.Layout.Bands[0].Columns["DetailDesc"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["DebitPid"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CreditPid"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["SubAmount"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["TaxPercent"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["TaxAmount"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["TotalAmount"].Header.Fixed = true;


      // Set caption column
      e.Layout.Bands[0].Columns["DetailDesc"].Header.Caption = "Diễn giải";
      e.Layout.Bands[0].Columns["DebitPid"].Header.Caption = "TK nợ";
      e.Layout.Bands[0].Columns["CreditPid"].Header.Caption = "TK có";
      e.Layout.Bands[0].Columns["SubAmount"].Header.Caption = "Tiền hàng";
      e.Layout.Bands[0].Columns["TaxPercent"].Header.Caption = "% thuế";
      e.Layout.Bands[0].Columns["TaxAmount"].Header.Caption = "Tiền thuế";
      e.Layout.Bands[0].Columns["TotalAmount"].Header.Caption = "Tổng\nthanh toán";
      e.Layout.Bands[0].Columns["VATDate"].Header.Caption = "Ngày\nhóa đơn";
      e.Layout.Bands[0].Columns["InputVATDocType"].Header.Caption = "Loại\nhóa đơn";
      e.Layout.Bands[0].Columns["VATFormNo"].Header.Caption = "Mẫu số";
      e.Layout.Bands[0].Columns["VATSymbol"].Header.Caption = "Ký hiệu";
      e.Layout.Bands[0].Columns["TaxNumber"].Header.Caption = "MST";
      e.Layout.Bands[0].Columns["VATInvoiceNo"].Header.Caption = "Số\nhóa đơn";
      e.Layout.Bands[0].Columns["ProjectPid"].Header.Caption = "Dự án";
      e.Layout.Bands[0].Columns["SegmentPid"].Header.Caption = "Khoản mục\nchi phí";
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].Header.Caption = "CPXD cơ bản\ndở dang";
      e.Layout.Bands[0].Columns["CostObjectPid"].Header.Caption = "Chi phí chờ\nphân bổ";
      e.Layout.Bands[0].Columns["EInvoiceTypePid"].Header.Caption = "Mã hóa đơn\nGTGT";
      e.Layout.Bands[0].Columns["CostCenterPid"].Header.Caption = "TTCP";

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["DebitPid"].ValueList = ucbddDebit;
      e.Layout.Bands[0].Columns["CreditPid"].ValueList = ucbddCredit;
      e.Layout.Bands[0].Columns["InputVATDocType"].ValueList = ucbddInputVATDocType;
      e.Layout.Bands[0].Columns["ProjectPid"].ValueList = ucbddProjectPid;
      e.Layout.Bands[0].Columns["SegmentPid"].ValueList = ucbddSegmentPid;
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].ValueList = ucbddUnCostConstructionPid;
      e.Layout.Bands[0].Columns["CostObjectPid"].ValueList = ucbddCostObjectPid;
      e.Layout.Bands[0].Columns["EInvoiceTypePid"].ValueList = ucbddEInvoiceTypePid;
      e.Layout.Bands[0].Columns["CostCenterPid"].ValueList = ucbddCostCenterPid;
      //set align
      e.Layout.Bands[0].Columns["DebitPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["CreditPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["InputVATDocType"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["ProjectPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["SegmentPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["CostObjectPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["EInvoiceTypePid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["CostCenterPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["DebitPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["DebitPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["CreditPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CreditPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["InputVATDocType"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["InputVATDocType"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["ProjectPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ProjectPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["SegmentPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["SegmentPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["CostObjectPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CostObjectPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["EInvoiceTypePid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["EInvoiceTypePid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["CostCenterPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CostCenterPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;


      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalAmount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "Tổng = {0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";

      e.Layout.Bands[0].Columns["CreditPid"].CellActivation = Activation.ActivateOnly;
    }

    private void ugdData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      this.NeedToSave = true;
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      string commandText = string.Empty;
      double vat = DBConvert.ParseDouble(e.Cell.Row.Cells["TaxPercent"].Value);
      switch (columnName)
      {
        case "subamount":
          try
          {
            if (DBConvert.ParseDouble(value) >= 0)
            {
              if (vat >= 0)
              {
                if (this.currency == 1)
                {
                  e.Cell.Row.Cells["TaxAmount"].Value = Math.Round(DBConvert.ParseDouble(value) * (vat / 100), 0, MidpointRounding.AwayFromZero);

                }
                else
                {
                  e.Cell.Row.Cells["TaxAmount"].Value = Math.Round(DBConvert.ParseDouble(value) * (vat / 100), 2, MidpointRounding.AwayFromZero);
                }
                e.Cell.Row.Cells["TotalAmount"].Value = DBConvert.ParseDouble(value) + DBConvert.ParseDouble(e.Cell.Row.Cells["TaxAmount"].Value);
              }
            }
          }
          catch
          {
            e.Cell.Row.Cells["TaxAmount"].Value = DBNull.Value;
            e.Cell.Row.Cells["TotalAmount"].Value = DBNull.Value;
          }
          break;
        case "taxpercent":
          try
          {
            if (DBConvert.ParseDouble(value) >= 0)
            {
              if (this.currency == 1)
              {
                e.Cell.Row.Cells["TaxAmount"].Value = Math.Round(DBConvert.ParseDouble(e.Cell.Row.Cells["SubAmount"].Value) * (DBConvert.ParseDouble(value) / 100), 0, MidpointRounding.AwayFromZero);
              }
              else
              {
                e.Cell.Row.Cells["TaxAmount"].Value = Math.Round(DBConvert.ParseDouble(e.Cell.Row.Cells["SubAmount"].Value) * (DBConvert.ParseDouble(value) / 100), 2, MidpointRounding.AwayFromZero);
              }
              e.Cell.Row.Cells["TotalAmount"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["SubAmount"].Value) + DBConvert.ParseDouble(e.Cell.Row.Cells["TaxAmount"].Value);
            }
          }
          catch
          {
            e.Cell.Row.Cells["TotalAmount"].Value = DBNull.Value;
          }
          break;
        case "totalamount":
          try
          {
            this.TotalAmount();
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
      if (ucbObjectType.SelectedRow != null)
      {
        if (DBConvert.ParseInt(ucbObjectType.Value) == 1)
        {
          cmd = string.Format(@"SELECT ObjectPid, CAST(ObjectCode as nvarchar) + ' - ' + [ObjectName] [Object], [ObjectName], ObjectType
	                            FROM VACCObjectList
                            WHERE ObjectType = 1");
        }
        else if (DBConvert.ParseInt(ucbObjectType.Value) == 2)
        {
          cmd = string.Format(@"SELECT ObjectPid, CAST(ObjectCode as nvarchar) + ' - ' + [ObjectName] [Object], [ObjectName], ObjectType
	                            FROM VACCObjectList
                            WHERE ObjectType = 2");
        }
        else if (DBConvert.ParseInt(ucbObjectType.Value) == 3)
        {
          cmd = string.Format(@"SELECT ObjectPid, CAST(ObjectCode as nvarchar) + ' - ' + [ObjectName] [Object], [ObjectName], ObjectType
	                            FROM VACCObjectList
                            WHERE ObjectType = 3");
        }
        DataTable dtTempObject = SqlDataBaseAccess.SearchCommandTextDataTable(cmd);
        Utility.LoadUltraCombo(ucbObject, dtTempObject, "ObjectPid", "Object", false, new string[] { "ObjectPid", "ObjectName", "ObjectType" });
      }
      else
      {
        if (dtObject.Rows.Count > 0)
        {
          Utility.LoadUltraCombo(ucbObject, this.dtObject, "ObjectPid", "Object", false, new string[] { "ObjectPid", "ObjectName", "ObjectType" });
        }
      }
      if (DBConvert.ParseInt(ucbObject.Value) <= 0)
      {
        ucbObject.Value = null;
      }
    }

    private void ucbObject_ValueChanged(object sender, EventArgs e)
    {
      if (ucbObject.SelectedRow != null)
      {
        ucbObjectType.Value = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["ObjectType"].Value);
        txtReceiverName.Text = ucbObject.SelectedRow.Cells["ObjectName"].Value.ToString();
      }
    }


    private void ugdData_AfterRowInsert(object sender, RowEventArgs e)
    {
      if (ucbCashFund.SelectedRow != null)
      {
        e.Row.Cells["CreditPid"].Value = DBConvert.ParseInt(ucbCashFund.SelectedRow.Cells["AccountPid"].Value);
      }
      e.Row.Cells["SubAmount"].Value = 0;
      e.Row.Cells["TaxPercent"].Value = 0;
      e.Row.Cells["TaxAmount"].Value = 0;
      e.Row.Cells["TotalAmount"].Value = 0;

      //read only
      ucbCurrency.ReadOnly = true;
      ucbObject.ReadOnly = true;
      ucbObjectType.ReadOnly = true;
    }


    private void btnAdd_Click(object sender, EventArgs e)
    {
      if (this.actionCode == 5)
      {
        viewACC_10_007 view = new viewACC_10_007();
        if (ucbObjectType.SelectedRow != null)
        {
          view.objectType = DBConvert.ParseInt(ucbObjectType.Value);
        }
        if (ucbObject.SelectedRow != null)
        {
          view.objectPid = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["ObjectPid"].Value);
        }
        if (ucbCurrency.SelectedRow != null)
        {
          view.currency = DBConvert.ParseInt(ucbCurrency.SelectedRow.Cells["Pid"].Value);
        }
        WindowUtinity.ShowView(view, "Chọn phiếu tạm ứng", true, ViewState.ModalWindow, FormWindowState.Maximized);
        if (view.dtAdvance != null && view.dtAdvance.Rows.Count > 0)
        {
          DataTable dtDetail = view.dtAdvance;
          //Master
          if (ucbCurrency.SelectedRow == null)
          {
            ucbCurrency.Value = DBConvert.ParseInt(dtDetail.Rows[0]["CurrencyPid"]);
          }
          ucbObjectType.Value = 3;
          if (ucbObject.SelectedRow == null)
          {
            ucbObject.Value = DBConvert.ParseInt(dtDetail.Rows[0]["ObjectPid"]);
          }
          DataTable dtMain = (DataTable)ugdData.DataSource;
          for (int i = 0; i < dtDetail.Rows.Count; i++)
          {
            DataRow row = dtDetail.Rows[i];
            DataRow rowAdd = dtMain.NewRow();
            rowAdd["PaymentAdvanceDetailPid"] = DBConvert.ParseInt(row["Pid"].ToString());
            rowAdd["DetailDesc"] = row["DetailDesc"].ToString();
            rowAdd["DebitPid"] = DBConvert.ParseInt(row["DebitPid"].ToString());
            if (ucbCashFund.SelectedRow != null)
            {
              rowAdd["CreditPid"] = DBConvert.ParseInt(ucbCashFund.SelectedRow.Cells["AccountPid"].Value);
            }
            if (DBConvert.ParseInt(row["ProjectPid"].ToString()) > 0)
            {
              rowAdd["ProjectPid"] = DBConvert.ParseInt(row["ProjectPid"].ToString());
            }
            if (DBConvert.ParseInt(row["SegmentPid"].ToString()) > 0)
            {
              rowAdd["SegmentPid"] = DBConvert.ParseInt(row["SegmentPid"].ToString());
            }
            rowAdd["SubAmount"] = DBConvert.ParseDouble(row["Remain"].ToString());
            rowAdd["TaxPercent"] = 0;
            rowAdd["TaxAmount"] = 0;
            rowAdd["TotalAmount"] = DBConvert.ParseDouble(row["Remain"].ToString());
            dtMain.Rows.Add(rowAdd);
          }
          ugdData.DataSource = dtMain;
          this.TotalAmount();
        }
      }
      else if (this.actionCode == 2)
      {

        viewACC_10_008 view = new viewACC_10_008();
        if (ucbObject.SelectedRow != null)
        {
          view.objectPid = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["ObjectPid"].Value);
        }
        if (ucbCurrency.SelectedRow != null)
        {
          view.currency = DBConvert.ParseInt(ucbCurrency.SelectedRow.Cells["Pid"].Value);
        }
        WindowUtinity.ShowView(view, "Chọn phiếu đề nghị thanh toán trực tiếp", true, ViewState.ModalWindow, FormWindowState.Maximized);
        if (view.dtAdvance != null && view.dtAdvance.Rows.Count > 0)
        {
          DataTable dtDetail = view.dtAdvance;
          //Master
          if (ucbCurrency.SelectedRow == null)
          {
            ucbCurrency.Value = DBConvert.ParseInt(dtDetail.Rows[0]["CurrencyPid"]);
          }
          ucbObjectType.Value = 3;
          if (ucbObject.SelectedRow == null)
          {
            ucbObject.Value = DBConvert.ParseInt(dtDetail.Rows[0]["ObjectPid"]);
          }
          DataTable dtMain = (DataTable)ugdData.DataSource;
          for (int i = 0; i < dtDetail.Rows.Count; i++)
          {
            DataRow row = dtDetail.Rows[i];
            DataRow rowAdd = dtMain.NewRow();
            rowAdd["PaymentRequestDetailPid"] = DBConvert.ParseInt(row["Pid"].ToString());
            rowAdd["DetailDesc"] = row["DetailDesc"].ToString();
            rowAdd["DebitPid"] = DBConvert.ParseInt(row["DebitPid"].ToString());
            if (ucbCashFund.SelectedRow != null)
            {
              rowAdd["CreditPid"] = DBConvert.ParseInt(ucbCashFund.SelectedRow.Cells["AccountPid"].Value);
            }
            if (DBConvert.ParseInt(row["ProjectPid"].ToString()) > 0)
            {
              rowAdd["ProjectPid"] = DBConvert.ParseInt(row["ProjectPid"].ToString());
            }
            if (DBConvert.ParseInt(row["SegmentPid"].ToString()) > 0)
            {
              rowAdd["SegmentPid"] = DBConvert.ParseInt(row["SegmentPid"].ToString());
            }
            rowAdd["SubAmount"] = DBConvert.ParseDouble(row["Remain"].ToString());
            rowAdd["TaxPercent"] = DBConvert.ParseDouble(row["VATPercent"].ToString());
            rowAdd["TaxAmount"] = 0;
            rowAdd["TotalAmount"] = DBConvert.ParseDouble(row["Remain"].ToString());

            rowAdd["TaxNumber"] = row["VATNumber"].ToString();
            rowAdd["VATDate"] = DBConvert.ParseDateTime(row["VATDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            rowAdd["VATInvoiceNo"] = row["VATInvoiceNo"].ToString();
            rowAdd["VATFormNo"] = row["VATFormNo"].ToString();
            rowAdd["VATSymbol"] = row["VATSymbol"].ToString();

            if (DBConvert.ParseInt(row["InputVATDocType"].ToString()) > 0)
            {
              rowAdd["InputVATDocType"] = DBConvert.ParseInt(row["InputVATDocType"].ToString());
            }
            if (DBConvert.ParseInt(row["UnCostConstructionPid"].ToString()) > 0)
            {
              rowAdd["UnCostConstructionPid"] = DBConvert.ParseInt(row["UnCostConstructionPid"].ToString());
            }
            if (DBConvert.ParseInt(row["CostObjectPid"].ToString()) > 0)
            {
              rowAdd["CostObjectPid"] = DBConvert.ParseInt(row["CostObjectPid"].ToString());
            }
            if (DBConvert.ParseInt(row["EInvoiceTypePid"].ToString()) > 0)
            {
              rowAdd["EInvoiceTypePid"] = DBConvert.ParseInt(row["EInvoiceTypePid"].ToString());
            }
            if (DBConvert.ParseInt(row["CostCenterPid"].ToString()) > 0)
            {
              rowAdd["CostCenterPid"] = DBConvert.ParseInt(row["CostCenterPid"].ToString());
            }

            dtMain.Rows.Add(rowAdd);
          }
          ugdData.DataSource = dtMain;
          this.TotalAmount();
        }
      }
    }

    private void ucbCashFund_ValueChanged(object sender, EventArgs e)
    {
      if (ucbCashFund.SelectedRow != null && ugdData.Rows.Count > 0)
      {
        for (int i = 0; i < ugdData.Rows.Count; i++)
        {
          ugdData.Rows[i].Cells["CreditPid"].Value = DBConvert.ParseInt(ucbCashFund.SelectedRow.Cells["AccountPid"].Value);
        }
      }
    }

    private void utcDetail_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
    {
      this.LoadTabData();
    }


    private void ugdData_AfterRowsDeleted(object sender, EventArgs e)
    {
      if (ugdData.Rows.Count == 0)
      {
        ucbObjectType.ReadOnly = false;
        ucbObject.ReadOnly = false;
        ucbCurrency.ReadOnly = false;
      }
      this.TotalAmount();
    }

    #endregion Event
  }
}