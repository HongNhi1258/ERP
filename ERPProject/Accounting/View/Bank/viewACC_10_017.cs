/*
  Author      : Nguyen Thanh Binh
  Date        : 11/04/2021
  Description : Loan Receipt
  Standard Form: view_SaveMasterDetail
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_10_017 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private int status = 0;
    private DataTable dtObject = new DataTable();
    private int docTypePid = ConstantClass.Loan_Receipt;
    public int actionCode = 1;
    private int currency = int.MinValue;
    private bool isLoaded = false;
    private bool isLoadedLoanDoc = false;
    private bool isLoadedLoanPaidDoc = false;
    private bool isLoadedLoanPlanDoc = false;
    private bool isLoadedLoanInterestRate = false;
    public string listLoanDocPid = string.Empty;
    #endregion Field

    #region Init
    public viewACC_10_017()
    {
      InitializeComponent();
    }

    private void viewACC_10_017_Load(object sender, EventArgs e)
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
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCLoanReceipt_Init", inputParam);
      Utility.LoadUltraCombo(ucbLoanAgreement, dsInit.Tables[0], "Pid", "ContractCode", false, "Pid");
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[1], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate" });
      Utility.LoadUltraCombo(ucbLoanAccountPid, dsInit.Tables[2], "Pid", "LoanTypeName", false, new string[] { "Pid", "ACLoanPid" });
      Utility.LoadUltraCombo(ucbInterestMethod, dsInit.Tables[3], "Code", "Value", false, "Code");
      // Set Language
      //this.SetLanguage();
    }

    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        txtReceiptCode.ReadOnly = true;
        udtReceiptDate.ReadOnly = true;
        ucbCurrency.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
        txtReceiptDesc.ReadOnly = true;
        btnSave.Enabled = false;
        chkConfirm.Enabled = false;
      }
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtReceiptCode.Text = dtMain.Rows[0]["ReceiptCode"].ToString();
        udtReceiptDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["ReceiptDate"]);
        udtEndDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["EndDate"]);
        ucbLoanAgreement.Value = DBConvert.ParseInt(dtMain.Rows[0]["LoanAgreementPid"]);
        uneInterestRate.Value = dtMain.Rows[0]["InterestRate"];
        uneOverdueInterestRate.Value = dtMain.Rows[0]["OverdueInterestRate"];
        ucbCurrency.Value = DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"]);
        uneExchangeRate.Value = dtMain.Rows[0]["ExchangeRate"];
        uneMonthTerm.Value = dtMain.Rows[0]["MonthTerm"];
        uneReceiptAmount.Value = dtMain.Rows[0]["ReceiptAmount"];
        uneOpeningPaidAmount.Value = dtMain.Rows[0]["OpeningPaidAmount"];
        unePaidAmount.Value = dtMain.Rows[0]["PaidAmount"];
        ucbLoanAccountPid.Value = DBConvert.ParseInt(dtMain.Rows[0]["LoanAccountPid"]);
        ucbInterestMethod.Value = DBConvert.ParseInt(dtMain.Rows[0]["InterestMethod"]);
        uneRemainAmount.Value = dtMain.Rows[0]["RemainAmount"];
        uneMonthlyPaidAmount.Value = dtMain.Rows[0]["MonthlyPaidAmount"];
        txtReceiptDesc.Text = dtMain.Rows[0]["ReceiptDesc"].ToString();
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
          txtReceiptCode.Text = outputParam[0].Value.ToString();
          udtReceiptDate.Value = DateTime.Now;
        }
      }
    }

    //private void LoadTransationData()
    //{
    //  SqlDBParameter[] inputParam = new SqlDBParameter[2];
    //  inputParam[0] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
    //  inputParam[1] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
    //  DataSet dsSource = SqlDataBaseAccess.SearchStoreProcedure("spACCTransaction_Load", inputParam);
    //  if (dsSource != null && dsSource.Tables.Count > 0)
    //  {
    //    ugdTransaction.DataSource = dsSource.Tables[0];
    //  }
    //}

    private void LoadLoanDoc()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      DataSet dsSource = SqlDataBaseAccess.SearchStoreProcedure("spACCLoanDoc_Load", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        ugdLoanDoc.DataSource = dsSource.Tables[0];
      }
    }

    private void LoadLoanPaidDoc()
    {
      SqlDBParameter[] inputParam1 = new SqlDBParameter[1];
      inputParam1[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      DataSet dsSource = SqlDataBaseAccess.SearchStoreProcedure("spACCLoanPaidDoc_Load", inputParam1);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        ugdLoanPaidDoc.DataSource = dsSource.Tables[0];
      }
    }

    private void LoadLoanPlanDoc()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      DataSet dsSource = SqlDataBaseAccess.SearchStoreProcedure("", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        ugdPlanDoc.DataSource = dsSource.Tables[0];
      }
    }

    private void LoadLoanInterestRate()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      DataSet dsSource = SqlDataBaseAccess.SearchStoreProcedure("", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        ugdInterestRate.DataSource = dsSource.Tables[0];
      }
    }

    /// <summary>
    /// Load tab data
    /// </summary>
    private void LoadTabData()
    {
      // Load Tab Data Component
      string tabPageName = utcLoanAgreementList.SelectedTab.TabPage.Name;
      switch (tabPageName)
      {
        case "utpcListLoanAgreement":
          if (!isLoadedLoanDoc)
          {
            this.LoadLoanDoc();
            this.isLoadedLoanDoc = true;
          }
          break;
        case "utpcListLoanPaidDoc":
          if (!isLoadedLoanPaidDoc)
          {
            this.LoadLoanPaidDoc();
            this.isLoadedLoanPaidDoc = true;
          }
          break;
        case "utpcListLoanPlanDoc":
          if (!isLoadedLoanPlanDoc)
          {

          }
          break;
        case "utpcListLoanInterestRateChange":
          if (!isLoadedLoanInterestRate)
          {

          }
          break;
        default:
          break;
      }
    }

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();
      this.isLoaded = true;
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCLoanReceipt_Load", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        this.LoadMainData(dsSource.Tables[0]);
      }
      this.isLoadedLoanDoc = false;
      this.isLoadedLoanPaidDoc = false;
      this.isLoadedLoanPlanDoc = false;
      this.isLoadedLoanInterestRate = false;
      this.LoadTabData();
      this.SetStatusControl();
      this.NeedToSave = false;
    }

    private bool CheckValid()
    {
      //check master
      if (udtReceiptDate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày chứng từ không được để trống!!!");
        udtReceiptDate.Focus();
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

      for (int i = 0; i < ugdLoanDoc.Rows.Count; i++)
      {
        UltraGridRow row = ugdLoanDoc.Rows[i];
        row.Selected = false;
        if (DBConvert.ParseDouble(row.Cells["SubAmount"].Value) <= 0)
        {
          row.Cells["SubAmount"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Tiền hàng không được để trống và phải lớn hơn 0.");
          row.Selected = true;
          ugdLoanDoc.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
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
      inputParam[1] = new SqlDBParameter("@ReceiptDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtReceiptDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      inputParam[2] = new SqlDBParameter("@EndDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtEndDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      inputParam[3] = new SqlDBParameter("@LoanAgreementPid", SqlDbType.Int, DBConvert.ParseInt(ucbLoanAgreement.Value));
      inputParam[4] = new SqlDBParameter("@InterestRate", SqlDbType.Float, uneInterestRate.Value);
      if (DBConvert.ParseDouble(uneOverdueInterestRate.Value) >= 0)
      {
        inputParam[5] = new SqlDBParameter("@OverdueInterestRate", SqlDbType.Float, uneOverdueInterestRate.Value);
      }
      inputParam[6] = new SqlDBParameter("@CurrencyPid", SqlDbType.Int, DBConvert.ParseInt(ucbCurrency.Value));
      inputParam[7] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, uneExchangeRate.Value);
      if (DBConvert.ParseInt(uneMonthTerm.Value) >= 0)
      {
        inputParam[8] = new SqlDBParameter("@MonthTerm", SqlDbType.Int, uneMonthTerm.Value);
      }
      if (DBConvert.ParseDouble(uneReceiptAmount.Value) >= 0)
      {
        inputParam[9] = new SqlDBParameter("@ReceiptAmount", SqlDbType.Float, uneReceiptAmount.Value);
      }
      if (DBConvert.ParseDouble(uneOpeningPaidAmount.Value) >= 0)
      {
        inputParam[10] = new SqlDBParameter("@OpeningPaidAmount", SqlDbType.Float, uneOpeningPaidAmount.Value);
      }
      if (DBConvert.ParseDouble(unePaidAmount.Value) >= 0)
      {
        inputParam[11] = new SqlDBParameter("@PaidAmount", SqlDbType.Float, unePaidAmount.Value);
      }
      inputParam[12] = new SqlDBParameter("@LoanAccountPid", SqlDbType.Int, DBConvert.ParseInt(ucbLoanAccountPid.Value));
      if (ucbInterestMethod.SelectedRow != null)
      {
        inputParam[13] = new SqlDBParameter("@InterestMethod", SqlDbType.Int, DBConvert.ParseInt(ucbInterestMethod.Value));
      }
      if (DBConvert.ParseDouble(uneRemainAmount.Value) >= 0)
      {
        inputParam[14] = new SqlDBParameter("@RemainAmount", SqlDbType.Float, uneRemainAmount.Value);
      }
      if (DBConvert.ParseDouble(uneMonthlyPaidAmount.Value) >= 0)
      {
        inputParam[15] = new SqlDBParameter("@MonthlyPaidAmount", SqlDbType.Float, uneMonthlyPaidAmount.Value);
      }
      if (txtReceiptDesc.Text.Trim().Length > 0)
      {
        inputParam[16] = new SqlDBParameter("@ReceiptDesc", SqlDbType.NVarChar, txtReceiptDesc.Text.Trim().ToString());
      }
      inputParam[17] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParam[18] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCBankLoanReceipt_Save", inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        this.viewPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
        return true;
      }
      return false;
    }

    private bool SaveLoanDoc()
    {
      bool success = true;
      // 1. Delete      
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      for (int i = 0; i < listDeletedPid.Count; i++)
      {
        SqlDBParameter[] deleteParam = new SqlDBParameter[] { new SqlDBParameter("@Pid", SqlDbType.BigInt, listDeletedPid[i]) };
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCLoanDoc_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ugdLoanDoc.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          SqlDBParameter[] inputParam = new SqlDBParameter[8];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
          }
          inputParam[1] = new SqlDBParameter("@LoanReceiptPid", SqlDbType.BigInt, this.viewPid);
          inputParam[2] = new SqlDBParameter("@ReferenceNo", SqlDbType.VarChar, row["ReferenceNo"].ToString());
          inputParam[3] = new SqlDBParameter("@ReferenceDate", SqlDbType.DateTime, DBConvert.ParseDateTime(row["ReferenceDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
          inputParam[4] = new SqlDBParameter("@TotalAmount", SqlDbType.Float, DBConvert.ParseDouble(row["TotalAmount"].ToString()));
          inputParam[5] = new SqlDBParameter("@Currency", SqlDbType.VarChar, row["Currency"].ToString());
          inputParam[6] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, DBConvert.ParseDouble(row["ExchangeRate"].ToString()));
          if (row["LoanDocDesc"].ToString().Trim().Length > 0)
          {
            inputParam[7] = new SqlDBParameter("@LoanDocDesc", SqlDbType.NVarChar, row["LoanDocDesc"].ToString());
          }
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCLoanDoc_Save", inputParam, outputParam);
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
          success = this.SaveLoanDoc();
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
      for (int i = 0; i < ugdLoanDoc.Rows.Count; i++)
      {
        if (DBConvert.ParseDouble(ugdLoanDoc.Rows[i].Cells["TotalAmount"].Value) != Double.MinValue)
        {
          total += DBConvert.ParseDouble(ugdLoanDoc.Rows[i].Cells["TotalAmount"].Value);
        }
      }
      uneReceiptAmount.Value = total;
      uneRemainAmount.Value = DBConvert.ParseDouble(uneReceiptAmount.Value) - DBConvert.ParseDouble(unePaidAmount.Value);
    }
    #endregion Function

    #region Event


    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdLoanDoc, "Chi tiết báo có ngân hàng");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugdData);
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

    }


    private void ugdData_AfterRowInsert(object sender, RowEventArgs e)
    {
      //read only
      ucbCurrency.ReadOnly = true;
      uneExchangeRate.ReadOnly = true;
      ucbLoanAgreement.ReadOnly = true;
    }

    private void utcDetail_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
    {
      if (this.isLoaded)
      {
        this.LoadTabData();
      }
    }


    private void ucbLoanAgreement_ValueChanged(object sender, EventArgs e)
    {

    }

    private void ugdLoanDoc_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Utility.InitLayout_UltraGrid(ugdLoanDoc);
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Columns["ReferenceNo"].Header.Caption = "Mã chứng từ";
      e.Layout.Bands[0].Columns["ReferenceDate"].Header.Caption = "Ngày chứng từ";
      e.Layout.Bands[0].Columns["Currency"].Header.Caption = "Tiền tệ";
      e.Layout.Bands[0].Columns["ExchangeRate"].Header.Caption = "Tỉ giá";
      e.Layout.Bands[0].Columns["TotalAmount"].Header.Caption = "Tổng tiền của chứng từ";
      e.Layout.Bands[0].Columns["LoanDocDesc"].Header.Caption = "Ghi chú";
    }

    private void btnListBankCredit_Click(object sender, EventArgs e)
    {
      viewACC_10_018 view = new viewACC_10_018();
      view.loanAccountPid = DBConvert.ParseInt(ucbLoanAccountPid.Value);
      Shared.Utility.WindowUtinity.ShowView(view, "Chọn chứng từ vay", false, ViewState.ModalWindow, FormWindowState.Normal);
      DataTable dtDetail = view.dtDetail;
      DataTable dtMain = (DataTable)ugdLoanDoc.DataSource;
      for (int i = 0; i < dtDetail.Rows.Count; i++)
      {
        DataRow row = dtDetail.Rows[i];
        DataRow rowAdd = dtMain.NewRow();
        rowAdd["ReferenceNo"] = row["ReferenceNo"].ToString();
        rowAdd["ReferenceDate"] = DBConvert.ParseDateTime(row["ReferenceDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        rowAdd["Currency"] = row["Currency"].ToString();
        rowAdd["ExchangeRate"] = DBConvert.ParseDouble(row["ExchangeRate"].ToString());
        rowAdd["TotalAmount"] = row["TotalAmount"].ToString();
        rowAdd["LoanDocDesc"] = row["LoanDesc"].ToString();
        dtMain.Rows.Add(rowAdd);
      }
      ugdLoanDoc.DataSource = dtMain;
      this.TotalAmount();
    }
    #endregion Event
  }
}
