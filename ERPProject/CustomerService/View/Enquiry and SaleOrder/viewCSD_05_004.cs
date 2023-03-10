/*
 * Created By   : Nguyen Van Tron
 * Created Date : 28/06/2010
 * Description  : Create or Update Sale Order (Customer Service Department)
 * Update       : Tran Hung
 */

using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.CustomerService;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewCSD_05_004 : MainUserControl
  {
    # region Fied
    private bool load = false;
    private UltraGridCell cellDropDown = null;
    public string saleNo = string.Empty;
    public long saleOrderPid = long.MinValue;
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    private bool confirmSO = false;    
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;    
    private bool isReloadPaymentSchedule = false;
    private bool isLoadingData = false;
    private int oddNumber = 2;
    private int docTypePid = 249;
    #endregion Fied
    public viewCSD_05_004()
    {
      InitializeComponent();
      //ultOrderDate.Value = DateTime.MinValue;
    }
    void viewCSD_05_004_Load(object sender, System.EventArgs e)
    {
      Utility.Format_UltraNumericEditor(tlpForm);
      this.InitData();
      this.LoadData();
      this.load = true;
    }

    #region function

    private void Object_Changed(object sender, EventArgs e)
    {
      if (this.load)
      {
        this.NeedToSave = true;        
      }
    }
    #endregion function

    #region LoadData
    /// <summary>
    /// LoadDropdownItemCode
    /// </summary>    
    private void LoadDropdownItemCode(UltraDropDown udrpItemCode)
    {
      string commandText = string.Format(@"SELECT DISTINCT INF.ItemCode, BS.Name, BS.NameVN
                                           FROM TblBOMItemInfo INF INNER JOIN TblBOMItemBasic BS ON (INF.ItemCode = BS.ItemCode)");
      udrpItemCode.DataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpItemCode.ValueMember = "ItemCode";
      udrpItemCode.DisplayMember = "ItemCode";
      udrpItemCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      udrpItemCode.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      udrpItemCode.DisplayLayout.Bands[0].Columns["NameVN"].Hidden = true;
      udrpItemCode.DisplayLayout.Bands[0].Columns["ItemCode"].Width = 150;
    }

    /// <summary>
    /// LoadGrid
    /// </summary>    
    private void LoadGrid(DataTable dtSource)
    {
      this.listDeletedPid = new ArrayList();
      ultDetail.DataSource = dtSource;
      
      //Customer, Type
      if (ultDetail.Rows.Count > 0)
      {
        this.EnableCustomer(false);
      }
      else
      {
        this.EnableCustomer(true);
      }
    }

    /// <summary>
    /// Enable or disable customer, type
    /// </summary>    
    private void EnableCustomer(bool status)
    {
      ucbCustomer.ReadOnly = !status;
      chkDirect.Enabled = status;
      cmbDirectCus.Enabled = status;
      ucbType.ReadOnly = !status;
    }

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spCSDSaleOrder_Init");
      // 0.Sale Order Type
      Utility.LoadUltraCombo(ucbType, dsInit.Tables[0], "TypeCode", "TypeName", false, "TypeCode");
      // 1.Enviroment Info
      Utility.LoadUltraCombo(ucbEnvironmentStatus, dsInit.Tables[1], "EnvironmentPid", "EnvironmentName", false, "EnvironmentPid");
      // 2.Customer
      Utility.LoadUltraCombo(ucbCustomer, dsInit.Tables[2], "Pid", "DisplayText", false, new string[] { "Pid", "DisplayText", "StreetAddress" });
      ucbCustomer.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
      // 3.Payment Type
      Utility.LoadUltraCombo(ucbPaymentTypeMain, dsInit.Tables[3], "TypeCode", "TypeName", false, "TypeCode");
      Utility.LoadUltraCombo(ucbPaymentType, Utility.CloneTable(dsInit.Tables[3]), "TypeCode", "TypeName", false, "TypeCode");
      // 4.Payment Term for Customer
      Utility.LoadUltraCombo(ucbPaymentTerm, dsInit.Tables[4], "Pid", "DisplayText", false, new string[] { "Pid", "DisplayText" });
      ucbPaymentTerm.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
      // 5.Currency
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[5], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate", "OddNumber" });
      // 6. Payment Kind
      Utility.LoadUltraCombo(ucbPaymentKind, dsInit.Tables[6], "PaymentKindCode", "PaymentKindName", false, "PaymentKindCode");
      // 7. Company Bank
      Utility.LoadUltraCombo(ucbCompanyBank, dsInit.Tables[7], "CompanyBankPid", "DisplayText", false, new string[] { "CompanyBankPid", "DisplayText" });
      ucbCompanyBank.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
    }

    /// <summary>
    /// LoadData
    /// </summary>    
    private void LoadMainData(DataTable dtSource)
    {
      if (this.saleOrderPid == long.MinValue)
      {
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@NewDocCode", DbType.String, 32, string.Empty) };
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@DocTypePid", DbType.Int32, this.docTypePid);
        inputParam[1] = new DBParameter("@DocDate", DbType.Date, DateTime.Now.Date);

        DataBaseAccess.SearchStoreProcedure("spACCGetNewDocCode", inputParam, outputParam);
        if (outputParam[0].Value.ToString().Length > 0)
        {
          txtSaleNo.Text = outputParam[0].Value.ToString();
        }
        ultPODate.Value = DateTime.Now;
        ultOrderDate.Value = DateTime.Now;
        udtRequiredShipDate.Value = DBNull.Value;
      }
      else
      {
        txtSaleNo.Text = dtSource.Rows[0]["SaleNo"].ToString();
        txtCustomerPoNo.Text = dtSource.Rows[0]["CustomerPONo"].ToString();
        txtRef.Text = dtSource.Rows[0]["REFNo"].ToString();
        txtRemark.Text = dtSource.Rows[0]["Remark"].ToString();
        txtcontract.Text = dtSource.Rows[0]["ContractNo"].ToString();
        DateTime poDate = DBConvert.ParseDateTime(dtSource.Rows[0]["PODate"]);
        if (poDate != DateTime.MinValue)
        {
          ultPODate.Value = poDate;
        }
        else
        {
          ultPODate.Value = DBNull.Value;
        }  
        ultOrderDate.Value = DBConvert.ParseDateTime(dtSource.Rows[0]["OrderDate"]);
        ucbType.Value = DBConvert.ParseInt(dtSource.Rows[0]["Type"]);
        ucbEnvironmentStatus.Value = DBConvert.ParseInt(dtSource.Rows[0]["EnvironmentStatus"]);
        ucbCustomer.Value = DBConvert.ParseLong(dtSource.Rows[0]["CustomerPid"].ToString());
        Utility.LoadDirectCustomer(cmbDirectCus, DBConvert.ParseLong(dtSource.Rows[0]["CustomerPid"].ToString()));
        long directPid = DBConvert.ParseLong(dtSource.Rows[0]["DirectPid"].ToString());
        if (directPid > 0)
        {
          try
          {
            chkDirect.Enabled = true;
            chkDirect.Checked = true;
            cmbDirectCus.Visible = true;
            cmbDirectCus.SelectedValue = directPid;
          }
          catch { }
        }
        int paymentType = DBConvert.ParseInt(dtSource.Rows[0]["PaymentType"]);
        if (paymentType > 0)
        {
          ucbPaymentTypeMain.Value = paymentType;
        }
        int companyBankPid = DBConvert.ParseInt(dtSource.Rows[0]["CompanyBankPid"]);
        if (companyBankPid > 0)
        {
          ucbCompanyBank.Value = companyBankPid;
        }
        string loadingPort = dtSource.Rows[0]["LoadingPort"].ToString();
        if (loadingPort.Length > 0)
        {
          txtLoadingPort.Text = loadingPort;
        }
        string dischargePort = dtSource.Rows[0]["DischargePort"].ToString();
        if (dischargePort.Length > 0)
        {
          txtDischargePort.Text = dischargePort;
        }
        ucbPaymentTerm.Value = dtSource.Rows[0]["PaymentTermPid"];
        ucbCurrency.Value = dtSource.Rows[0]["CurrencyPid"];
        uneExchangeRate.Value = dtSource.Rows[0]["ExchangeRate"];
        DateTime requiredShipDate = DBConvert.ParseDateTime(dtSource.Rows[0]["RequiredShipDate"]);
        if (requiredShipDate != DateTime.MinValue)
        {
          udtRequiredShipDate.Value = requiredShipDate;
        }
        else
        {
          udtRequiredShipDate.Value = DBNull.Value;
        }  
        utxtSODesc.Text = dtSource.Rows[0]["SODesc"].ToString();
        utxtInvoiceAddress.Text = dtSource.Rows[0]["InvoiceAddress"].ToString();
        utxtDeliveryAddress.Text = dtSource.Rows[0]["DeliveryAddress"].ToString();
        uneSubTotalAmount.Value = dtSource.Rows[0]["SubTotalAmount"];
        double discountPercent = DBConvert.ParseDouble(dtSource.Rows[0]["DiscountPercent"]);
        if (discountPercent > 0)
        {
          uneDiscountPercent.Value = discountPercent;
        }
        double discountAmount = DBConvert.ParseDouble(dtSource.Rows[0]["DiscountAmount"]);
        if (discountAmount > 0)
        {
          uneDiscountAmount.Value = discountAmount;
        }
        double taxPercent = DBConvert.ParseDouble(dtSource.Rows[0]["TaxPercent"]);
        if (taxPercent > 0)
        {
          uneTaxPercent.Value = taxPercent;
        }
        double taxAmount = DBConvert.ParseDouble(dtSource.Rows[0]["TaxAmount"]);
        if (taxAmount > 0)
        {
          uneTaxAmount.Value = taxAmount;
        }
        uneTotalAmount.Value = dtSource.Rows[0]["TotalAmount"];

        this.confirmSO = (DBConvert.ParseInt(dtSource.Rows[0]["Status"].ToString()) >= 1);
        chkConfirm.Checked = this.confirmSO;
      }
    }

      /// <summary>
      /// LoadData
      /// </summary>    
      private void LoadData()
    {
      this.confirmSO = false;
      this.load = false;
      this.isLoadingData = true;

      string commandText = "spCSDSaleOrder_Select";
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.saleOrderPid);
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(commandText, inputParam);
            
      if (dsSource == null || dsSource.Tables.Count < 3)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0007");
        this.CloseTab();
        return;
      }
      this.LoadMainData(dsSource.Tables[0]);
      this.LoadGrid(dsSource.Tables[1]);
      ugdPaymentSchedule.DataSource = dsSource.Tables[2];
      this.SetStatusControl();
      this.load = true;
      this.isLoadingData = false;
      this.isReloadPaymentSchedule = false;
    }

    private void Amount_ValueChanged(object sender, EventArgs e)
    {
      this.SetValueAmount();
    }

    private void SetValueAmount()
    {
      if (!this.isLoadingData)
      {
        double subTotalAmount = 0, discountPercent = 0, discountAmount = 0, taxPercent = 0, taxAmount = 0, totalAmount = 0;
        DataTable dtDetail = (DataTable)ultDetail.DataSource;
        double amount = DBConvert.ParseDouble(dtDetail.Compute("Sum(Amount)", "Qty > 0").ToString());
        if (amount >= 0)
        {
          subTotalAmount = Math.Round(amount, this.oddNumber);
        }
        discountPercent = DBConvert.ParseDouble(uneDiscountPercent.Value);
        if (discountPercent >= 0)
        {
          discountAmount = Math.Round(subTotalAmount * (discountPercent / 100), this.oddNumber);
        }
        taxPercent = DBConvert.ParseDouble(uneTaxPercent.Value);
        if (taxPercent >= 0)
        {
          taxAmount = Math.Round((subTotalAmount - discountAmount) * (taxPercent / 100), this.oddNumber);
        }
        totalAmount = subTotalAmount - discountAmount + taxAmount;

        uneSubTotalAmount.Value = subTotalAmount;
        uneDiscountAmount.Value = discountAmount;
        uneTaxAmount.Value = taxAmount;
        uneTotalAmount.Value = totalAmount;
      }
    }

    private void SetStatusControl()
    {
      if (this.confirmSO)
      {
        txtCustomerPoNo.ReadOnly = true;
        txtRef.ReadOnly = true;
        txtcontract.ReadOnly = true;
        ultOrderDate.ReadOnly = true;
        ultPODate.ReadOnly = true;
        ucbType.ReadOnly = true;
        ucbEnvironmentStatus.ReadOnly = true;
        ucbCustomer.ReadOnly = true;
        chkDirect.Enabled = false;
        cmbDirectCus.Enabled = false;
        ucbPaymentTypeMain.ReadOnly = true;
        ucbPaymentTerm.ReadOnly = true;
        ucbCurrency.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
        txtRemark.ReadOnly = true;
        btnAddItem.Enabled = false;
        btnSaveAll.Enabled = false;       
        chkConfirm.Enabled = false;
        udtRequiredShipDate.ReadOnly = true;
        ucbCompanyBank.ReadOnly = true;
        txtLoadingPort.ReadOnly = true;
        txtDischargePort.ReadOnly = true;
        utxtDeliveryAddress.ReadOnly = true;
        utxtInvoiceAddress.ReadOnly = true;
        utxtSODesc.ReadOnly = true;
        uneDiscountPercent.ReadOnly = true;
        uneTaxPercent.ReadOnly = true;
        for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          ultDetail.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
        for (int i = 0; i < ugdPaymentSchedule.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          ugdPaymentSchedule.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
      }
    }
    private UltraDropDown LoadRevisionByItemCode(UltraDropDown udrpTemp, string itemCode)
    {
      if (udrpTemp == null)
      {
        udrpTemp = new UltraDropDown();
        this.Controls.Add(udrpTemp);
      }
      string commandText = string.Format("Select Revision From TblBOMRevision Where ItemCode = '{0}' Order by Revision DESC", itemCode);
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpTemp.DataSource = dtSource;
      udrpTemp.ValueMember = "Revision";
      udrpTemp.DisplayMember = "Revision";
      udrpTemp.DisplayLayout.Bands[0].ColHeadersVisible = false;
      return udrpTemp;
    }

    private void PaymentSchedule_ValueChanged(object sender, EventArgs e)
    {
      if (!this.isLoadingData)
      {
        this.isReloadPaymentSchedule = true;
      }
      this.LoadSOPaymentSchedule();
    }

    private void LoadSOPaymentSchedule()
    {
      if (this.isReloadPaymentSchedule)
      {
        DataTable dtPaymentSchedule;
        int paymentTermPid = DBConvert.ParseInt(ucbPaymentTerm.Value);
        DataTable dtDetail = (DataTable)ultDetail.DataSource;
        if (paymentTermPid > 0 && dtDetail.Rows.Count > 0)
        {
          double totalAmount = DBConvert.ParseDouble(uneTotalAmount.Value);
          if (totalAmount > 0)
          {
            DBParameter[] inputParam = new DBParameter[5];
            if (ucbPaymentTypeMain.Value != null)
            {
              inputParam[0] = new DBParameter("@PaymentType", DbType.Int32, ucbPaymentTypeMain.Value);
            }
            inputParam[1] = new DBParameter("@PaymentTermPid", DbType.Int32, ucbPaymentTerm.Value);
            if (ultOrderDate.Value != null)
            {
              inputParam[2] = new DBParameter("@DocDate", DbType.Date, DBConvert.ParseDateTime(ultOrderDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
            }
            if (udtRequiredShipDate.Value != null)
            {
              inputParam[3] = new DBParameter("@DeliveryDate", DbType.Date, DBConvert.ParseDateTime(udtRequiredShipDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
            }
            inputParam[4] = new DBParameter("@TotalAmount", DbType.Double, totalAmount);
            dtPaymentSchedule = DataBaseAccess.SearchStoreProcedureDataTable("spPURPOPaymentSchedule", inputParam);
          }
          else
          {
            dtPaymentSchedule = null;
          }
        }
        else
        {
          dtPaymentSchedule = null;
        }
        ugdPaymentSchedule.DataSource = dtPaymentSchedule;
        this.isReloadPaymentSchedule = false;
      }
    }
    #endregion LoadData

    #region CheckValid And Save

    private bool CheckIsValid(out string message)
    {
      message = string.Empty;
      // Order Date :
      DateTime obj = DBConvert.ParseDateTime(ultOrderDate.Value.ToString(), formatConvert);
      if (obj == DateTime.MinValue)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), lblOrderDate.Text);
        return false;
      }

      // Type :
      int type = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ucbType));
      if (type == int.MinValue)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), lblType.Text);
        return false;
      }

      // Environmental status:
      int environmental = DBConvert.ParseInt(ucbEnvironmentStatus.Value);
      if (environmental == int.MinValue)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), lblEnvironmentStatus.Text);
        return false;
      }

      // Customer :
      long customerPid = DBConvert.ParseLong(ucbCustomer.Value);
      if (customerPid == long.MinValue)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), lblCustomer.Text);
        return false;
      }

      //Direct Customer :
      if ((chkDirect.Checked) && (cmbDirectCus.SelectedIndex == 0))
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), chkDirect.Text);
        return false;
      }

      if (ucbPaymentTerm.SelectedRow == null)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), lblPaymentTerm.Text);
        return false;
      }

      if (ucbCurrency.SelectedRow == null)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), lblCurrency.Text);
        return false;
      }
      if(DBConvert.ParseDouble(uneExchangeRate.Value) <= 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), lblExchangeRate.Text);
        return false;
      }  

      // Confirm
      if (chkConfirm.Checked && ultDetail.Rows.Count == 0)
      {
        message = Shared.Utility.FunctionUtility.GetMessage("WRN0001");
        return false;
      }
      //check detail      
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultDetail.Rows[i];
        row.Selected = false;
        if (DBConvert.ParseDouble(row.Cells["Qty"].Value) <= 0)
        {
          row.Cells["Qty"].Appearance.BackColor = Color.Yellow;
          message = string.Format("{0} không được để trống và phải lớn hơn 0", row.Cells["Qty"].Column.Header.Caption);
          row.Selected = true;
          ultDetail.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        if (DBConvert.ParseDouble(row.Cells["Price"].Value) < 0)
        {
          row.Cells["Price"].Appearance.BackColor = Color.Yellow;
          message = string.Format("{0} không được để trống và phải lớn hơn hoặc bằng 0", row.Cells["Price"].Column.Header.Caption);
          row.Selected = true;
          ultDetail.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
      }
      // Check total SO QTy <= EN Qty
      DataTable dtDetail = new DataTable();
      dtDetail.Columns.Add("Pid", typeof(long));
      dtDetail.Columns.Add("EnquiryConfirmDetailPid", typeof(long));
      dtDetail.Columns.Add("Qty", typeof(double));

      DataTable dtSource = (DataTable)ultDetail.DataSource;
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          DataRow rowDetail = dtDetail.NewRow();
          rowDetail["Pid"] = row["Pid"];
          rowDetail["EnquiryConfirmDetailPid"] = row["EnquiryConfirmDetailPid"];
          rowDetail["Qty"] = row["Qty"];
          dtDetail.Rows.Add(rowDetail);
        }
      }
      int paramNumber = 2;
      string storeName = "spCSDSaleOrder_CheckInvalid";

      SqlDBParameter[] inputParam = new SqlDBParameter[paramNumber];
      inputParam[0] = new SqlDBParameter("@AddedEnquiryList", SqlDbType.NVarChar, DBConvert.ParseXMLString(dtDetail));
      inputParam[1] = new SqlDBParameter("@SaleOrderPid", SqlDbType.BigInt, this.saleOrderPid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@ErrorMessage", SqlDbType.NVarChar, string.Empty) };
      SqlDataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if (outputParam != null && outputParam[0].Value.ToString().Length > 0)
      {
        message = outputParam[0].Value.ToString();
        return false;
      }
      return true;
    }

    private bool SaveMain()
    {
      bool success = true;
      string storeName = "spCSDSaleOrder_Edit";
      int paramNumber = 30;
      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (this.saleOrderPid != long.MinValue) //Update
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.saleOrderPid);
      }
      string customerPoNo = txtCustomerPoNo.Text.Trim();
      if (customerPoNo.Length > 0)
      {
        inputParam[1] = new DBParameter("@CustomerPONo", DbType.AnsiString, 128, customerPoNo);
      }
      string refNo = txtRef.Text.Trim();
      if (refNo.Length > 0)
      {
        inputParam[2] = new DBParameter("@REFNo", DbType.AnsiString, 32, refNo);
      }
      string contractNo = txtcontract.Text.Trim();
      if (contractNo.Length > 0)
      {
        inputParam[3] = new DBParameter("@ContractNo", DbType.AnsiString, 32, contractNo);
      }
      if(ultOrderDate.Value != null)
      {
        inputParam[4] = new DBParameter("@OrderDate", DbType.DateTime, DBConvert.ParseDateTime(ultOrderDate.Value.ToString(), formatConvert));
      }
      if (ultPODate.Value != null)
      {
        inputParam[5] = new DBParameter("@PODate", DbType.DateTime, DBConvert.ParseDateTime(ultPODate.Value.ToString(), formatConvert));
      }
      if (ucbType.SelectedRow != null)
      {
        inputParam[6] = new DBParameter("@Type", DbType.Int32, ucbType.Value);
      }
      if(ucbEnvironmentStatus.SelectedRow != null)
      {
        inputParam[7] = new DBParameter("@EnvironmentStatus", DbType.Int32, ucbEnvironmentStatus.Value);
      }
      if (ucbCustomer.SelectedRow != null)
      {
        inputParam[8] = new DBParameter("@CustomerPid", DbType.Int64, ucbCustomer.Value);
      }
      if (chkDirect.Checked)
      {
        inputParam[9] = new DBParameter("@DirectPid", DbType.Int64, DBConvert.ParseLong(Utility.GetSelectedValueCombobox(cmbDirectCus)));
      }
      if (ucbPaymentTypeMain.SelectedRow != null)
      {
        inputParam[10] = new DBParameter("@PaymentType", DbType.Int32, ucbPaymentTypeMain.Value);
      }
      if (ucbPaymentTerm.SelectedRow != null)
      {
        inputParam[11] = new DBParameter("@PaymentTermPid", DbType.Int32, ucbPaymentTerm.Value);
      }
      if (ucbCurrency.SelectedRow != null)
      {
        inputParam[12] = new DBParameter("@CurrencyPid", DbType.Int32, ucbCurrency.Value);
      }
      inputParam[13] = new DBParameter("@ExchangeRate", DbType.Double, uneExchangeRate.Value);
      if (udtRequiredShipDate.Value != null)
      {
        inputParam[14] = new DBParameter("@RequiredShipDate", DbType.DateTime, DBConvert.ParseDateTime(udtRequiredShipDate.Value.ToString(), formatConvert));
      }
      string remark = txtRemark.Text.Trim();
      if (remark.Length > 0)
      {
        inputParam[15] = new DBParameter("@Remark", DbType.String, 4000, remark);
      }
      string soDesc = utxtSODesc.Text.Trim();
      if(soDesc.Length > 0)
      {
        inputParam[16] = new DBParameter("@SODesc", DbType.String, 256, soDesc);
      }
      string invoiceAddress = utxtInvoiceAddress.Text.Trim();
      if (invoiceAddress.Length > 0)
      {
        inputParam[17] = new DBParameter("@InvoiceAddress", DbType.String, 256, invoiceAddress);
      }
      string deliveryAddress = utxtDeliveryAddress.Text.Trim();
      if (deliveryAddress.Length > 0)
      {
        inputParam[18] = new DBParameter("@DeliveryAddress", DbType.String, 256, deliveryAddress);
      }
      double subTotalAmount = DBConvert.ParseDouble(uneSubTotalAmount.Value);
      if(subTotalAmount > 0)
      {
        inputParam[19] = new DBParameter("@SubTotalAmount", DbType.Double, subTotalAmount);
      }
      double discountPercent = DBConvert.ParseDouble(uneDiscountPercent.Value);
      if (discountPercent > 0)
      {
        inputParam[20] = new DBParameter("@DiscountPercent", DbType.Double, discountPercent);
      }
      double discountAmount = DBConvert.ParseDouble(uneDiscountAmount.Value);
      if (discountAmount > 0)
      {
        inputParam[21] = new DBParameter("@DiscountAmount", DbType.Double, discountAmount);
      }
      double taxPercent = DBConvert.ParseDouble(uneTaxPercent.Value);
      if (taxPercent > 0)
      {
        inputParam[22] = new DBParameter("@TaxPercent", DbType.Double, taxPercent);
      }
      double taxAmount = DBConvert.ParseDouble(uneTaxAmount.Value);
      if (taxAmount > 0)
      {
        inputParam[23] = new DBParameter("@TaxAmount", DbType.Double, taxAmount);
      }
      double totalAmount = DBConvert.ParseDouble(uneTotalAmount.Value);
      if (totalAmount > 0)
      {
        inputParam[24] = new DBParameter("@TotalAmount", DbType.Double, totalAmount);
      }
      int confirm = (chkConfirm.Checked ? 1 : 0);
      inputParam[25] = new DBParameter("@Status", DbType.Int32, confirm);
      inputParam[26] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      if(ucbCompanyBank.SelectedRow != null)
      {
        inputParam[27] = new DBParameter("@CompanyBankPid", DbType.Int32, ucbCompanyBank.Value);
      }
      string loadingPort = txtLoadingPort.Text.Trim();
      if (loadingPort.Length > 0)
      {
        inputParam[28] = new DBParameter("@LoadingPort", DbType.String, 256, loadingPort);
      }
      string dischargePort = txtDischargePort.Text.Trim();
      if (dischargePort.Length > 0)
      {
        inputParam[29] = new DBParameter("@DischargePort", DbType.String, 256, dischargePort);
      }

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        this.saleOrderPid = DBConvert.ParseLong(outputParam[0].Value.ToString());        
      }
      else
      {
        success = false;
      }     
      return success;
    }
    
    private bool SaveDetail()
    {
      bool success = true;
      //1. Delete
      foreach (long pid in this.listDeletedPid)
      {
        PLNSaleOrderDetail saleOrderDT = new PLNSaleOrderDetail();
        saleOrderDT.Pid = pid;
        saleOrderDT = (PLNSaleOrderDetail)Shared.DataBaseUtility.DataBaseAccess.LoadObject(saleOrderDT, new string[] { "Pid" });
        long pidENConfirm = saleOrderDT.EnquiryConfirmDetailPid;
        bool isDeleted = Shared.DataBaseUtility.DataBaseAccess.DeleteObject(saleOrderDT, new string[] { "Pid" });
        if (!isDeleted)
        {
          success = false;
        }        
      }

      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ultDetail.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          DBParameter[] inputParam = new DBParameter[11];
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          long pid = DBConvert.ParseLong(row["Pid"]);
          string itemCode = row["ItemCode"].ToString();
          int revision = DBConvert.ParseInt(row["Revision"]);
          double qty = DBConvert.ParseDouble(row["Qty"]);
          double price = DBConvert.ParseDouble(row["Price"]);
          DateTime scheduleDelivery = DBConvert.ParseDateTime(row["ScheduleDelivery"].ToString(), formatConvert);
          DateTime confirmedShipDate = DBConvert.ParseDateTime(row["ConfirmedShipDate"].ToString(), formatConvert);
          int enquiryConfirmDetailPid = DBConvert.ParseInt(row["EnquiryConfirmDetailPid"]);
          string remark = row["Remark"].ToString().Trim();

          if (pid > 0)
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          inputParam[2] = new DBParameter("@Revision", DbType.Int32, revision);
          inputParam[3] = new DBParameter("@Qty", DbType.Double, qty);
          inputParam[4] = new DBParameter("@Price", DbType.Double, price);
          if (scheduleDelivery != DateTime.MinValue)
          {
            inputParam[5] = new DBParameter("@ScheduleDelivery", DbType.DateTime, scheduleDelivery);
          }
          if (confirmedShipDate != DateTime.MinValue)
          {
            inputParam[6] = new DBParameter("@ConfirmedShipDate", DbType.DateTime, confirmedShipDate);
          }
          inputParam[7] = new DBParameter("@EnquiryConfirmDetailPid", DbType.Int64, enquiryConfirmDetailPid);
          if (remark.Length > 0)
          {
            inputParam[8] = new DBParameter("@Remark", DbType.String, 512, remark);
          }          
          inputParam[9] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          inputParam[10] = new DBParameter("@SaleOrderPid", DbType.Int64, this.saleOrderPid);
          DataBaseAccess.ExecuteStoreProcedure("spCSDSaleOrderDetail_Edit", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }      
      return success;
    }

    private bool SavePOPaymentSchedule()
    {
      bool success = true;
      // Delete
      DBParameter[] inputDelete = new DBParameter[1];
      inputDelete[0] = new DBParameter("@SaleOrderPid", DbType.Int64, this.saleOrderPid);
      DataBaseAccess.ExecuteStoreProcedure("spPLNSaleOrderPaymentSchedule_Delete", inputDelete);

      // Insert
      int currencyPid = DBConvert.ParseInt(ucbCurrency.Value);
      double exchangeRate = DBConvert.ParseDouble(uneExchangeRate.Value);
      for (int i = 0; i < ugdPaymentSchedule.Rows.Count; i++)
      {
        UltraGridRow row = ugdPaymentSchedule.Rows[i];
        int paymentTermPid = DBConvert.ParseInt(row.Cells["PaymentTermPid"].Value);
        int paymentKindCode = DBConvert.ParseInt(row.Cells["PaymentKindCode"].Value);
        DateTime dueDate = DBConvert.ParseDateTime(row.Cells["DueDate"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        int paymentType = DBConvert.ParseInt(row.Cells["PaymentType"].Value);
        double amount = DBConvert.ParseDouble(row.Cells["Amount"].Value);

        DBParameter[] inputInsert = new DBParameter[9];
        inputInsert[0] = new DBParameter("@SaleOrderPid", DbType.Int64, this.saleOrderPid);
        inputInsert[1] = new DBParameter("@PaymentTermPid", DbType.Int32, paymentTermPid);
        inputInsert[2] = new DBParameter("@PaymentKindCode", DbType.Int32, paymentKindCode);
        inputInsert[3] = new DBParameter("@DueDate", DbType.Date, dueDate);
        if (paymentType > 0)
        {
          inputInsert[4] = new DBParameter("@PaymentType", DbType.Int32, paymentType);
        }
        inputInsert[5] = new DBParameter("@Amount", DbType.Double, amount);
        inputInsert[6] = new DBParameter("@CurrencyPid", DbType.Int32, currencyPid);
        inputInsert[7] = new DBParameter("@ExchangeRate", DbType.Double, exchangeRate);
        inputInsert[8] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

        DBParameter[] outputInsert = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNSaleOrderPaymentSchedule_Insert", inputInsert, outputInsert);

        if (DBConvert.ParseInt(outputInsert[0].Value) == 0)
        {
          success = false;
        }
      }
      return success;
    }

    private void SaveData()
    {
      string message = string.Empty;
      bool success = this.CheckIsValid(out message);
      if (!success)
      {
        Shared.Utility.WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      success = this.SaveMain();
      if (success)
      {        
        success = this.SaveDetail();
        if (success)
        {          
          // Save SO Payment Schedule
          success = this.SavePOPaymentSchedule();          
        }  
      }
      if (!success)
      {
        //Unlock        
        if (chkConfirm.Checked)
        {
          string commandUpdate = string.Format("Update TblPLNSaleOrder Set Status = 0 Where Pid = {0}", this.saleOrderPid);
          Shared.DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(commandUpdate);
        }
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
      }
      else
      {
        if (chkConfirm.Checked)
        {
          DBParameter[] param = new DBParameter[2];
          param[0] = new DBParameter("@SaleorderPid", DbType.Int64, this.saleOrderPid);
          param[1] = new DBParameter("@PLNConfirmBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNSaleOrderConfirmByPLN", param);

          bool isPosted = chkConfirm.Checked;
          success = Utility.ACCPostTransaction(this.docTypePid, this.saleOrderPid, isPosted, SharedObject.UserInfo.UserPid);
        }
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      this.LoadData();
      this.NeedToSave = false;
    }
    #endregion CheckValid And Save

    #region Events
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ultDetail);
      e.Layout.AutoFitStyle = AutoFitStyle.None;
      e.Layout.Bands[0].Summaries.Clear();      
      for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        Type colType = ultDetail.DisplayLayout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          ultDetail.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Price"].CellActivation = Activation.AllowEdit;      
      e.Layout.Bands[0].Columns["ConfirmedShipDate"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.AllowEdit;

      e.Layout.Bands[0].ColHeaderLines = 2;      

      e.Layout.Bands[0].Columns["EnquiryNo"].Header.Caption = "Mã Enquiry";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Mã SP";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Mã SP KH";      
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Tên SP";
      e.Layout.Bands[0].Columns["Revision"].Header.Caption = "Phiên bản";
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Header.Caption = "Ngày yêu cầu\ngiao hàng";
      e.Layout.Bands[0].Columns["ConfirmedShipDate"].Header.Caption = "Ngày xác nhận\ngiao hàng";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Số lượng";
      e.Layout.Bands[0].Columns["Price"].Header.Caption = "Đơn giá";
      e.Layout.Bands[0].Columns["Amount"].Header.Caption = "Thành tiền";
      e.Layout.Bands[0].Columns["Unit"].Header.Caption = "Đơn vị";
      e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Ghi chú";

      // Set width
      e.Layout.Bands[0].Columns["EnquiryNo"].Width = 80;
      e.Layout.Bands[0].Columns["ItemCode"].Width = 70;
      e.Layout.Bands[0].Columns["SaleCode"].Width = 70;
      e.Layout.Bands[0].Columns["Revision"].Width = 60;
      e.Layout.Bands[0].Columns["Qty"].Width = 70;
      e.Layout.Bands[0].Columns["Price"].Width = 80;
      e.Layout.Bands[0].Columns["Amount"].Width = 90;
      e.Layout.Bands[0].Columns["Unit"].Width = 70;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;      
      e.Layout.Bands[0].Columns["EnquiryConfirmDetailPid"].Hidden = true;      

      e.Layout.Bands[0].Columns["ScheduleDelivery"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["ConfirmedShipDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["ConfirmedShipDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,###.##}";
      e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Amount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,###.##}";
      e.Layout.Bands[0].Summaries[1].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Override.AllowDelete = (this.confirmSO ? DefaultableBoolean.False : DefaultableBoolean.True);
    }

    private void ultDetail_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      this.NeedToSave = true;      
      string columnName = e.Cell.Column.ToString().ToLower();      
      switch (columnName)
      {
        case "price":          
        case "qty":
          double qty = DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value);
          double price = DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value);
          if (qty >= 0 && price >= 0)
          {
            e.Cell.Row.Cells["Amount"].Value = qty * price;
          }
          else
          {
            e.Cell.Row.Cells["Amount"].Value = DBNull.Value;
          }
          this.SetValueAmount();
          break;
        default:
          break;
      }      
    }

    private void ultDetail_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.NeedToSave = true;
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
      this.SetValueAmount();
      //Customer, Type
      if (ultDetail.Rows.Count > 0)
      {
        this.EnableCustomer(false);
      }
      else
      {
        this.EnableCustomer(true);
      }
    }

    private void ultDetail_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long detailPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (detailPid != long.MinValue)
        {
          listDeletingPid.Add(detailPid);
        }
      }
    }
   
    private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.load == true)
      {
        this.NeedToSave = true;       
      }
    }

    private void btnAddItem_Click(object sender, EventArgs e)
    {
      #region Check
      // Customer :
      long customerPid = DBConvert.ParseLong(ucbCustomer.Value);
      if (customerPid == long.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", lblCustomer.Text);
        return;
      }
      // Environment :
      int environment = DBConvert.ParseInt(ucbEnvironmentStatus.Value);
      if (customerPid == int.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", lblEnvironmentStatus.Text);
        return;
      }
      //Direct Customer :
      long directCus = long.MinValue;
      if (chkDirect.Checked)
      {
        directCus = DBConvert.ParseLong(Utility.GetSelectedValueCombobox(cmbDirectCus));
        if (directCus == long.MinValue)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Direct Customer");
          return;
        }
      }

      // Type :
      int type = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ucbType));
      if (type == int.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", lblType.Text);
        return;
      }
      #endregion Check

      DataTable dtSource = (DataTable)ultDetail.DataSource;

      DataTable dtAddedEnquiryList = new DataTable();
      dtAddedEnquiryList.Columns.Add("EnquiryConfirmDetailPid", typeof(long));      
      DataRow[] rows = dtSource.Select("EnquiryConfirmDetailPid is not null");
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          DataRow rowDetail = dtAddedEnquiryList.NewRow();
          rowDetail["EnquiryConfirmDetailPid"] = row["EnquiryConfirmDetailPid"];
          dtAddedEnquiryList.Rows.Add(rowDetail);
        }
      }

      viewCSD_05_007 view = new viewCSD_05_007();
      view.customerPid = customerPid;
      view.customerDirect = directCus;
      view.type = type;
      view.dtAddedEnquiryList = dtAddedEnquiryList;
      view.dtExistingSource = dtSource;
      DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "Thêm Sản Phẩm", false, DaiCo.Shared.Utility.ViewState.ModalWindow);

      //Customet, type
      if (dtSource.Rows.Count > 0)
      {
        this.EnableCustomer(false);
      }
      else
      {
        this.EnableCustomer(true);
      }
      this.SetValueAmount();
    }

    private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
      //Direct Customer
      if (this.load == true)
      {
        this.NeedToSave = true;
        long customerPid = long.MinValue;
        try
        {
          lbDirect.Visible = false;
          cmbDirectCus.Visible = false;
          chkDirect.Checked = false;

          customerPid = DBConvert.ParseLong(ucbCustomer.Value);
          string commandText = string.Format(@"SELECT Pid FROM TblCSDCustomerInfo Where ParentPid = {0}", customerPid);
          DataTable dtDirectCus = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtDirectCus.Rows.Count > 0)
          {
            chkDirect.Enabled = true;
          }
          else
          {
            chkDirect.Enabled = false;
          }
        }
        catch
        {
          chkDirect.Enabled = false;
        }
      }
    }

    private void chkDirect_CheckedChanged(object sender, EventArgs e)
    {
      if (chkDirect.Checked)
      {
        lbDirect.Visible = true;
        cmbDirectCus.Visible = true;
        try
        {
          long cusPid = DBConvert.ParseLong(ucbCustomer.Value);
          Utility.LoadDirectCustomer(cmbDirectCus, cusPid);
        }
        catch { }
      }
      else
      {
        lbDirect.Visible = false;
        cmbDirectCus.Visible = false;
      }
    }

    private void btnSaveAll_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ultDetail_MouseClick(object sender, MouseEventArgs e)
    {
      Utility.BOMShowItemImage(ultDetail, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      Utility.BOMShowItemImage(ultDetail, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultDetail, "Danh sach SP");
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
    private void utcPOInfo_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
    {
      string tabPageName = utcPOInfo.SelectedTab.TabPage.Name;
      switch (tabPageName)
      {
        case "utpPaymentSchedule":
          this.LoadSOPaymentSchedule();
          break;
        default:
          break;
      }
    }

    private void ugdPaymentSchedule_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdPaymentSchedule);
      e.Layout.AutoFitStyle = AutoFitStyle.None;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      // Hide column      
      e.Layout.Bands[0].Columns["PaymentTermPid"].Hidden = true;

      // Set caption column
      e.Layout.Bands[0].Columns["PaymentTermName"].Header.Caption = "Điều khoản thanh toán";
      e.Layout.Bands[0].Columns["PaymentKindCode"].Header.Caption = "Loại thanh toán";
      e.Layout.Bands[0].Columns["DueDate"].Header.Caption = "Ngày thanh toán";
      e.Layout.Bands[0].Columns["PaymentType"].Header.Caption = "Phương thức TT";
      e.Layout.Bands[0].Columns["Amount"].Header.Caption = "Số tiền";

      // Set width
      e.Layout.Bands[0].Columns["DueDate"].Width = 110;
      e.Layout.Bands[0].Columns["PaymentType"].Width = 110;
      e.Layout.Bands[0].Columns["Amount"].Width = 100;

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["PaymentKindCode"].ValueList = ucbPaymentKind;
      e.Layout.Bands[0].Columns["PaymentType"].ValueList = ucbPaymentType;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["PaymentKindCode"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["PaymentKindCode"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["PaymentType"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["PaymentType"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      // Read only
      e.Layout.Bands[0].Columns["PaymentTermName"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      // Format      
      e.Layout.Bands[0].Columns["Amount"].Format = "###,###.##";

      // Total
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Amount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "Tổng = {0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";
    }

    private void ucbCustomer_ValueChanged(object sender, EventArgs e)
    {
      string address = string.Empty;
      if (ucbCustomer.SelectedRow != null)
      {
        address = ucbCustomer.SelectedRow.Cells["StreetAddress"].Value.ToString();
      }
      utxtDeliveryAddress.Value = address;
      utxtInvoiceAddress.Value = address;
    }

    private void udtRequiredShipDate_Leave(object sender, EventArgs e)
    {
      // kiểm tra xem cái chọn ngày nó có chọn ngày hay k
      if (udtRequiredShipDate.Value == null)
      {
        //Show cũng đc k show cũng đc - đi vào đaya
        WindowUtinity.ShowMessageConfirmFromText("Chưa chọn Required Ship Date");
        return; // thoát k làm gì nữa.
      };
      // nếu chọn ngày thì mới xử lý tiếp 
      DateTime confirmShipDate = DBConvert.ParseDateTime(udtRequiredShipDate.Value.ToString(), formatConvert);
      if (confirmShipDate != DateTime.MinValue)
      {
        //nó đi xuống đây r nè
        if (WindowUtinity.ShowMessageConfirmFromText("Bạn có muốn thay đổi ngày xác nhận giao hàng cho những sản phẩm bên dưới không?") == DialogResult.Yes)
        {
          foreach (UltraGridRow row in ultDetail.Rows)
          {
            row.Cells["ConfirmedShipDate"].Value = confirmShipDate;
          }
        }
      }
    }
    #endregion Events
  }
}
