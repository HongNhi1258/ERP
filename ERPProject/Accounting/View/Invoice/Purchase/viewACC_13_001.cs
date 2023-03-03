/*
  Author      : Nguyen Thanh Binh
  Date        : 25/11/2021
  Description : Invoice Detail
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
  public partial class viewACC_13_001 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(view_SaveMasterDetail).Assembly);
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private int docTypePid = ConstantClass.Invoice_Purchaser;
    private int status = 0;
    public int actionCode = 1;    
    public int creditPid = int.MinValue;    
    public string listPO = string.Empty;
    public string listRec = string.Empty;
    private int oddNumber = 2;
    private bool isLoadingData = false;
    #endregion Field

    #region Init
    public viewACC_13_001()
    {
      InitializeComponent();
    }

    private void viewACC_13_001_Load(object sender, EventArgs e)
    {
      Utility.Format_UltraNumericEditor(tlpForm);
      // Add ask before closing form even if user change data
      this.SetAutoAskSaveWhenCloseForm(uegbInfo);
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
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCInvoiceDetail_Init");
      Utility.LoadUltraCombo(ucbInputVATDocType, dsInit.Tables[0], "Code", "Value", false, "Code");      
      Utility.LoadUltraCombo(ucbObject, dsInit.Tables[1], "KeyValue", "Object", false, new string[] { "ObjectPid", "Object", "ObjectType", "KeyValue" });
      Utility.LoadUltraCombo(ucbSource, dsInit.Tables[2], "Code", "Value", false, "Code");
      Utility.LoadUltraCombo(ucbProject, dsInit.Tables[3], "Code", "Value", false, "Code");
      Utility.LoadUltraCombo(ucbPortCode, dsInit.Tables[4], "Code", "Value", false, "Code");
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[5], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate", "OddNumber" });

      //Load drop down for grid
      Utility.LoadUltraCombo(ucbddNation, dsInit.Tables[6], "Code", "Value", false, "Code");
      Utility.LoadUltraCombo(ucbddEnvironment, dsInit.Tables[7], "Code", "Value", false, "Code");
      Utility.LoadUltraCombo(ucbddCostCenter, dsInit.Tables[8], "Code", "Value", false, "Code");
      Utility.LoadUltraCombo(ucbddSegment, dsInit.Tables[9], "Code", "Value", false, "Code");
      Utility.LoadUltraCombo(ucbddUnCostConstruction, dsInit.Tables[10], "Code", "Value", false, "Code");
      Utility.LoadUltraCombo(ucbddMaterial, dsInit.Tables[11], "MaterialCode", "MaterialDescription", true);

      ucbddMaterial.DisplayLayout.Bands[0].Columns["MaterialCode"].Header.Caption = "Mã sản phẩm";
      ucbddMaterial.DisplayLayout.Bands[0].Columns["MaterialDescription"].Header.Caption = "Mô tả";
      // Set Language
      this.SetLanguage();
    }

    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        chkConfirm.Checked = true;
        chkConfirm.Enabled = false;
        btnSave.Enabled = false;
        ucbInputVATDocType.ReadOnly = true;
        txtVATFormNo.ReadOnly = true;
        txtVATSymbol.ReadOnly = true;
        txtVATInvoiceNo.ReadOnly = true;
        udtVATDate.ReadOnly = true;
        udtInvoiceDate.ReadOnly = true;
        ucbObject.ReadOnly = true;
        txtInvoiceDesc.ReadOnly = true;
        txtContractNo.ReadOnly = true;
        udtContractDate.ReadOnly = true;
        txtPackNo.ReadOnly = true;
        txtLogListNo.ReadOnly = true;
        ucbSource.ReadOnly = true;
        txtPackingList.ReadOnly = true;
        txtBillOfLanding.ReadOnly = true;
        ucbProject.ReadOnly = true;
        ucbPortCode.ReadOnly = true;
        ucbCurrency.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
        txtRemark.ReadOnly = true;
        txtInvoiceAddress.ReadOnly = true;
        uneSubTotalAmount.ReadOnly = true;
        uneDiscountPercent.ReadOnly = true;
        uneShippingFee.ReadOnly = true;
        uneExtraFee.ReadOnly = true;
        uneTaxPercent.ReadOnly = true;
        uneTotalAmount.ReadOnly = true;
        btnClearDeposit.Visible = true;
        txtDeliveryAddress.ReadOnly = true;
        udtPaymentDate.ReadOnly = true;
      }
      else
      {
        btnClearDeposit.Visible = false;
      }  
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        this.actionCode = DBConvert.ParseInt(dtMain.Rows[0]["ActionCode"]);
        if (DBConvert.ParseInt(dtMain.Rows[0]["InputVATDocType"].ToString()) != int.MinValue)
        {
          ucbInputVATDocType.Value = DBConvert.ParseInt(dtMain.Rows[0]["InputVATDocType"].ToString());
        }
        txtVATFormNo.Text = dtMain.Rows[0]["VATFormNo"].ToString();
        txtVATSymbol.Text = dtMain.Rows[0]["VATSymbol"].ToString();
        txtVATInvoiceNo.Text = dtMain.Rows[0]["VATInvoiceNo"].ToString();
        if (DBConvert.ParseDateTime(dtMain.Rows[0]["VATDate"]) != DateTime.MinValue)
        {
          udtVATDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["VATDate"]);
        }
        txtInvoiceCode.Text = dtMain.Rows[0]["InvoiceCode"].ToString();
        if (DBConvert.ParseDateTime(dtMain.Rows[0]["InvoiceDate"]) != DateTime.MinValue)
        {
          udtInvoiceDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["InvoiceDate"]);
        }
        ucbObject.Value = dtMain.Rows[0]["ObjectValue"];
        txtInvoiceDesc.Text = dtMain.Rows[0]["InvoiceDesc"].ToString();
        txtContractNo.Text = dtMain.Rows[0]["ContractNo"].ToString();
        if (DBConvert.ParseDateTime(dtMain.Rows[0]["ContractDate"]) != DateTime.MinValue)
        {
          udtContractDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["ContractDate"]);
        }
        if (DBConvert.ParseDateTime(dtMain.Rows[0]["PaymentDate"]) != DateTime.MinValue)
        {
          udtPaymentDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["PaymentDate"]);
        }
        txtPackNo.Text = dtMain.Rows[0]["PackNo"].ToString();
        if (DBConvert.ParseInt(dtMain.Rows[0]["Source"].ToString()) != int.MinValue)
        {
          ucbSource.Value = DBConvert.ParseInt(dtMain.Rows[0]["Source"].ToString());
        }
        txtPackingList.Text = dtMain.Rows[0]["PackingList"].ToString();
        txtBillOfLanding.Text = dtMain.Rows[0]["BillOfLanding"].ToString();
        txtStatus.Text = DBConvert.ParseInt(dtMain.Rows[0]["Status"].ToString()) == 0 ? "New" : "Hoàn tất";
        this.status = DBConvert.ParseInt(dtMain.Rows[0]["Status"].ToString());
        if (DBConvert.ParseInt(dtMain.Rows[0]["ProjectPid"].ToString()) != int.MinValue)
        {
          ucbProject.Value = DBConvert.ParseInt(dtMain.Rows[0]["ProjectPid"].ToString());
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["PortCode"].ToString()) != int.MinValue)
        {
          ucbPortCode.Value = DBConvert.ParseInt(dtMain.Rows[0]["PortCode"].ToString());
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"].ToString()) != int.MinValue)
        {
          ucbCurrency.Value = DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"].ToString());
        }
        uneExchangeRate.Value = DBConvert.ParseDouble(dtMain.Rows[0]["ExchangeRate"].ToString());
        txtRemark.Text = dtMain.Rows[0]["Remark"].ToString();
        txtDeliveryAddress.Text = dtMain.Rows[0]["DeliveryAddress"].ToString();
        txtInvoiceAddress.Text = dtMain.Rows[0]["InvoiceAddress"].ToString();
        uneSubTotalAmount.Value = DBConvert.ParseDouble(dtMain.Rows[0]["SubTotalAmount"].ToString());
        uneDiscountPercent.Value = DBConvert.ParseDouble(dtMain.Rows[0]["DiscountPercent"].ToString());
        uneDiscountAmount.Value = DBConvert.ParseDouble(dtMain.Rows[0]["DiscountAmount"].ToString());
        uneShippingFee.Value = DBConvert.ParseDouble(dtMain.Rows[0]["ShippingFee"].ToString());
        uneExtraFee.Value = DBConvert.ParseDouble(dtMain.Rows[0]["ExtraFee"].ToString());
        uneTaxPercent.Value = DBConvert.ParseDouble(dtMain.Rows[0]["TaxPercent"].ToString());
        uneTaxAmount.Value = DBConvert.ParseDouble(dtMain.Rows[0]["TaxAmount"].ToString());
        uneTotalAmount.Value = DBConvert.ParseDouble(dtMain.Rows[0]["TotalAmount"].ToString());
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
          txtInvoiceCode.Text = outputParam[0].Value.ToString();
          udtInvoiceDate.Value = DateTime.Now;
        }
      }  
    }

    /// <summary>
    /// Load detail
    /// </summary>
    private void LoadInvoiceDetail()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      DataSet dsSource = SqlDataBaseAccess.SearchStoreProcedure("spACCInvoiceDetail_LoadDetail", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        ugdData.DataSource = dsSource.Tables[0];
      }
    }

    /// <summary>
    /// Load transaction
    /// </summary>
    private void LoadTransationData()
    {      
      ugdTransaction.SetDataSource(this.docTypePid, this.viewPid);
    }


    /// <summary>
    /// Load tab data
    /// </summary>
    private void LoadTabData()
    {
      this.LoadInvoiceDetail();
      this.LoadTransationData();       
    }

    private void LoadData()
    {
      this.isLoadingData = true;
      this.listDeletedPid = new ArrayList();

      if (this.viewPid != long.MinValue || this.actionCode == 1)
      {
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCInvoiceDetail_LoadMaster", inputParam);
        if (dsSource != null && dsSource.Tables.Count > 0)
        {
          this.LoadMainData(dsSource.Tables[0]);
        }        
        this.LoadTabData();
        this.ReadOnlyKeyMainData();
      }
      else
      {
        if (this.actionCode == 2)
        {
          viewACC_13_003 view = new viewACC_13_003();
          Shared.Utility.WindowUtinity.ShowView(view, "Chọn đơn mua hàng", false, ViewState.ModalWindow, FormWindowState.Normal);
          this.listPO = view.listPO;
          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@ListPO", DbType.String, this.listPO);
          inputParam[1] = new DBParameter("@DocTypePid", DbType.Int32, this.docTypePid);
          inputParam[2] = new DBParameter("@ActionCode", DbType.Int32, this.actionCode);
          DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCInvoiceMadeByPO_Load", inputParam);
          if (dsSource != null && dsSource.Tables.Count > 1)
          {
            this.LoadMainData(dsSource.Tables[0]);
            ugdData.DataSource = dsSource.Tables[1];
          }
          this.ReadOnlyKeyMainData();
        }
        else if (this.actionCode == 3)
        {
          viewACC_13_004 view = new viewACC_13_004();
          Shared.Utility.WindowUtinity.ShowView(view, "Chọn phiếu nhập kho mua hàng", false, ViewState.ModalWindow, FormWindowState.Normal);
          this.listRec = view.listRec;
          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@ListReceiving", DbType.String, this.listRec);
          inputParam[1] = new DBParameter("@DocTypePid", DbType.Int32, this.docTypePid);
          inputParam[2] = new DBParameter("@ActionCode", DbType.Int32, this.actionCode);
          DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCInvoiceMadeByReceiving_Load", 300, inputParam);
          if (dsSource != null && dsSource.Tables.Count > 1)
          {
            this.LoadMainData(dsSource.Tables[0]);
            ugdData.DataSource = dsSource.Tables[1];
          }
          this.ReadOnlyKeyMainData();
        }
      }
      this.SetStatusControl();
      this.NeedToSave = false;
      this.isLoadingData = false;
    }

    /// <summary>
    /// Read Only for key main data
    /// </summary>
    private void ReadOnlyKeyMainData()
    {
      bool isReadOnly = false;
      if (ugdData.Rows.Count > 0)
      {
        isReadOnly = true;
      }
      ucbCurrency.ReadOnly = isReadOnly;
      ucbObject.ReadOnly = isReadOnly;
    }
    private bool CheckValid()
    {

      if (udtInvoiceDate.Value.ToString().Length == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày chứng từ không được để trống.");
        udtInvoiceDate.Focus();
        return false;
      }

      if (ucbObject.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Đối tượng không được để trống.");
        ucbObject.Focus();
        return false;
      }

      if (ucbSource.Text.ToString().Length == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Nguồn gốc không được để trống.");
        ucbSource.Focus();
        return false;
      }

      if (DBConvert.ParseInt(ucbSource.Value) == 1) // Noi dia
      {
        if (DBConvert.ParseInt(ucbCurrency.Value) != 1)
        {
          WindowUtinity.ShowMessageErrorFromText("Tiền tệ không hợp lệ.");
          return false;
        }
      }

      //Check duplicate VAT Invoice No in TblACCAPInvoiceIn
      if (txtVATInvoiceNo.TextLength > 0)
      {
        int paramNumber = 3;
        string storeName = "spACCInvoiceDetail_CheckDuplicateVATInvoice";

        SqlDBParameter[] inputParam = new SqlDBParameter[paramNumber];
        if (this.viewPid > 0)
        {
          inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
        }
        if (ucbObject.Value.ToString().Length > 0)
        {
          inputParam[1] = new SqlDBParameter("@ObjectKeyValue", SqlDbType.VarChar, ucbObject.Value.ToString());
        }
        if (txtVATInvoiceNo.TextLength > 0)
        {
          inputParam[2] = new SqlDBParameter("@VATInvoiceNo", SqlDbType.VarChar, txtVATInvoiceNo.Text.Trim().ToString());
        }

        SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@ErrorMessage", SqlDbType.NVarChar, string.Empty) };
        SqlDataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
        if (outputParam != null && outputParam[0].Value.ToString().Length > 0)
        {
          WindowUtinity.ShowMessageErrorFromText(outputParam[0].Value.ToString());
          return false;
        }
      }  
      


      return true;
    }


    private bool SaveMain()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[37];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      }
      if (txtInvoiceDesc.Text.Trim().Length > 0)
      {
        inputParam[1] = new SqlDBParameter("@InvoiceDesc", SqlDbType.NVarChar, txtInvoiceDesc.Text.Trim().ToString());
      }
      inputParam[2] = new SqlDBParameter("@InvoiceDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtInvoiceDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));      
      inputParam[3] = new SqlDBParameter("@ObjectPid", SqlDbType.Int, ucbObject.SelectedRow.Cells["ObjectPid"].Value);
      inputParam[4] = new SqlDBParameter("@ObjectType", SqlDbType.Int, ucbObject.SelectedRow.Cells["ObjectType"].Value);      
      inputParam[5] = new SqlDBParameter("@CurrencyPid", SqlDbType.Int, DBConvert.ParseInt(ucbCurrency.Value));
      inputParam[6] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, uneExchangeRate.Value);
      if (txtContractNo.Text.Trim().Length > 0)
      {
        inputParam[7] = new SqlDBParameter("@ContractNo", SqlDbType.VarChar, txtContractNo.Text.Trim().ToString());
      }
      if (DBConvert.ParseDateTime(udtContractDate.Value) != DateTime.MinValue)
      {
        inputParam[8] = new SqlDBParameter("@ContractDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtContractDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }
      if (DBConvert.ParseInt(ucbSource.Value) != int.MinValue)
      {
        inputParam[9] = new SqlDBParameter("@Source", SqlDbType.Int, DBConvert.ParseInt(ucbSource.Value));
      }
      if (txtPackNo.Text.Trim().Length > 0)
      {
        inputParam[10] = new SqlDBParameter("@PackNo", SqlDbType.VarChar, txtPackNo.Text.Trim().ToString());
      }
      if (txtPackingList.Text.Trim().Length > 0)
      {
        inputParam[11] = new SqlDBParameter("@PackingList", SqlDbType.VarChar, txtPackingList.Text.Trim().ToString());
      }
      if (txtBillOfLanding.Text.Trim().Length > 0)
      {
        inputParam[12] = new SqlDBParameter("@BillOfLanding", SqlDbType.VarChar, txtBillOfLanding.Text.Trim().ToString());
      }
      if (txtLogListNo.Text.Trim().Length > 0)
      {
        inputParam[13] = new SqlDBParameter("@LogListNo", SqlDbType.VarChar, txtLogListNo.Text.Trim().ToString());
      }
      if (DBConvert.ParseDateTime(udtPaymentDate.Value) != DateTime.MinValue)
      {
        inputParam[14] = new SqlDBParameter("@PaymentDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtPaymentDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }
      if (DBConvert.ParseInt(ucbProject.Value) != int.MinValue)
      {
        inputParam[15] = new SqlDBParameter("@ProjectPid", SqlDbType.Int, DBConvert.ParseInt(ucbProject.Value));
      }
      if (DBConvert.ParseInt(ucbPortCode.Value) != int.MinValue)
      {
        inputParam[16] = new SqlDBParameter("@PortCode", SqlDbType.Int, DBConvert.ParseInt(ucbPortCode.Value));
      }
      if (txtInvoiceAddress.Text.Trim().Length > 0)
      {
        inputParam[17] = new SqlDBParameter("@InvoiceAddress", SqlDbType.NVarChar, txtInvoiceAddress.Text.Trim().ToString());
      }
      if (txtDeliveryAddress.Text.Trim().Length > 0)
      {
        inputParam[18] = new SqlDBParameter("@DeliveryAddress", SqlDbType.NVarChar, txtDeliveryAddress.Text.Trim().ToString());
      }
      if (txtRemark.Text.Trim().Length > 0)
      {
        inputParam[19] = new SqlDBParameter("@Remark", SqlDbType.VarChar, txtRemark.Text.Trim().ToString());
      }
      if (DBConvert.ParseDouble(uneSubTotalAmount.Value) >= 0)
      {
        inputParam[20] = new SqlDBParameter("@SubTotalAmount", SqlDbType.Float, DBConvert.ParseDouble(uneSubTotalAmount.Value));
      }
      if (DBConvert.ParseDouble(uneDiscountPercent.Value) >= 0)
      {
        inputParam[21] = new SqlDBParameter("@DiscountPercent", SqlDbType.Float, DBConvert.ParseDouble(uneDiscountPercent.Value));
      }
      if (DBConvert.ParseDouble(uneDiscountAmount.Value) >= 0)
      {
        inputParam[22] = new SqlDBParameter("@DiscountAmount", SqlDbType.Float, DBConvert.ParseDouble(uneDiscountAmount.Value));
      }
      if (DBConvert.ParseDouble(uneShippingFee.Value) >= 0)
      {
        inputParam[23] = new SqlDBParameter("@ShippingFee", SqlDbType.Float, DBConvert.ParseDouble(uneShippingFee.Value));
      }
      if (DBConvert.ParseDouble(uneExtraFee.Value) >= 0)
      {
        inputParam[24] = new SqlDBParameter("@ExtraFee", SqlDbType.Float, DBConvert.ParseDouble(uneExtraFee.Value));
      }
      if (DBConvert.ParseDouble(uneTaxPercent.Value) >= 0)
      {
        inputParam[25] = new SqlDBParameter("@TaxPercent", SqlDbType.Float, DBConvert.ParseDouble(uneTaxPercent.Value));
      }
      if (DBConvert.ParseDouble(uneTaxAmount.Value) >= 0)
      {
        inputParam[26] = new SqlDBParameter("@TaxAmount", SqlDbType.Float, DBConvert.ParseDouble(uneTaxAmount.Value));
      }
      if (DBConvert.ParseDouble(uneTotalAmount.Value) >= 0)
      {
        inputParam[27] = new SqlDBParameter("@TotalAmount", SqlDbType.Float, DBConvert.ParseDouble(uneTotalAmount.Value));
      }
      inputParam[28] = new SqlDBParameter("@Status", SqlDbType.Bit, chkConfirm.Checked ? 1 : 0);
      inputParam[29] = new SqlDBParameter("@ActionCode", SqlDbType.Int, this.actionCode);
      inputParam[30] = new SqlDBParameter("@PostedStatus", SqlDbType.Bit, chkConfirm.Checked ? 1 : 0);
      if (txtVATInvoiceNo.Text.Trim().Length > 0)
      {
        inputParam[31] = new SqlDBParameter("@VATInvoiceNo", SqlDbType.VarChar, txtVATInvoiceNo.Text.Trim().ToString());
      }
      if (DBConvert.ParseDateTime(udtVATDate.Value) != DateTime.MinValue)
      {
        inputParam[32] = new SqlDBParameter("@VATDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtVATDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }
      if (txtVATFormNo.Text.Trim().Length > 0)
      {
        inputParam[33] = new SqlDBParameter("@VATFormNo", SqlDbType.VarChar, txtVATFormNo.Text.Trim().ToString());
      }
      if (txtVATSymbol.Text.Trim().Length > 0)
      {
        inputParam[34] = new SqlDBParameter("@VATSymbol", SqlDbType.VarChar, txtVATSymbol.Text.Trim().ToString());
      }
      if (DBConvert.ParseInt(ucbInputVATDocType.Value) != int.MinValue)
      {
        inputParam[35] = new SqlDBParameter("@InputVATDocType", SqlDbType.Int, DBConvert.ParseInt(ucbInputVATDocType.Value));
      }
      inputParam[36] = new SqlDBParameter("@CreateBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCInvoiceDetail_SaveMaster", inputParam, outputParam);
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
        SqlDataBaseAccess.ExecuteStoreProcedure("", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ugdData.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        
          SqlDBParameter[] inputParam = new SqlDBParameter[29];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (pid != long.MinValue)
          {
            inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
          }
          inputParam[1] = new SqlDBParameter("@InvoiceInPid", SqlDbType.BigInt, this.viewPid);
          if (DBConvert.ParseInt(row["AccountPid"].ToString()) >= 0)
          {
            inputParam[2] = new SqlDBParameter("@AccountPid", SqlDbType.Int, DBConvert.ParseInt(row["AccountPid"].ToString()));
          }
          if (DBConvert.ParseLong(row["PODetailPid"].ToString()) >= 0)
          {
            inputParam[3] = new SqlDBParameter("@PODetailPid", SqlDbType.BigInt, DBConvert.ParseLong(row["PODetailPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["ACOffsetPid"].ToString()) >= 0)
          {
            inputParam[4] = new SqlDBParameter("@ACOffsetPid", SqlDbType.Int, DBConvert.ParseInt(row["ACOffsetPid"].ToString()));
          }
          if (DBConvert.ParseLong(row["WHReceiptDetailPid"].ToString()) >= 0)
          {
            inputParam[5] = new SqlDBParameter("@WHReceiptDetailPid", SqlDbType.BigInt, DBConvert.ParseLong(row["WHReceiptDetailPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["IDKho"].ToString()) >= 0)
          {
            inputParam[6] = new SqlDBParameter("@IDKho", SqlDbType.Int, DBConvert.ParseInt(row["IDKho"].ToString()));
          }
          if (DBConvert.ParseInt(row["CountryPid"].ToString()) >= 0)
          {
            inputParam[7] = new SqlDBParameter("@CountryPid", SqlDbType.Int, DBConvert.ParseInt(row["CountryPid"].ToString()));
          }
          if (row["MaterialCode"].ToString().Trim().Length > 0)
          {
            inputParam[8] = new SqlDBParameter("@MaterialCode", SqlDbType.VarChar, row["MaterialCode"].ToString());
          }
          if (DBConvert.ParseDouble(row["Quantity"].ToString()) >= 0)
          {
            inputParam[9] = new SqlDBParameter("@Quantity", SqlDbType.Float, DBConvert.ParseDouble(row["Quantity"].ToString()));
          }
          if (DBConvert.ParseDouble(row["UnitCost"].ToString()) >= 0)
          {
            inputParam[10] = new SqlDBParameter("@UnitCost", SqlDbType.Float, DBConvert.ParseDouble(row["UnitCost"].ToString()));
          }
          if (DBConvert.ParseDouble(row["TotalCost"].ToString()) >= 0)
          {
            inputParam[11] = new SqlDBParameter("@TotalCost", SqlDbType.Float, DBConvert.ParseDouble(row["TotalCost"].ToString()));
          }
          if (row["DetailDesc"].ToString().Trim().Length > 0)
          {
            inputParam[12] = new SqlDBParameter("@DetailDesc", SqlDbType.NVarChar, row["DetailDesc"].ToString());
          }
          if (DBConvert.ParseDouble(row["CountOfQty"].ToString()) >= 0)
          {
            inputParam[13] = new SqlDBParameter("@CountOfQty", SqlDbType.Float, DBConvert.ParseDouble(row["CountOfQty"].ToString()));
          }
          if (DBConvert.ParseDouble(row["CountOfQtyUnitCost"].ToString()) >= 0)
          {
            inputParam[14] = new SqlDBParameter("@CountOfQtyUnitCost", SqlDbType.Float, DBConvert.ParseDouble(row["CountOfQtyUnitCost"].ToString()));
          }
          if (DBConvert.ParseDouble(row["CountOfQtyTotalCost"].ToString()) >= 0)
          {
            inputParam[15] = new SqlDBParameter("@CountOfQtyTotalCost", SqlDbType.Float, DBConvert.ParseDouble(row["CountOfQtyTotalCost"].ToString()));
          }
          if (DBConvert.ParseInt(row["EnvironmentInfo"].ToString()) > 0)
          {
            inputParam[16] = new SqlDBParameter("@EnvironmentInfo", SqlDbType.Int, DBConvert.ParseInt(row["EnvironmentInfo"].ToString()));
          }
          if (DBConvert.ParseInt(row["QualityPid"].ToString()) > 0)
          {
            inputParam[17] = new SqlDBParameter("@QualityPid", SqlDbType.Int, DBConvert.ParseInt(row["QualityPid"].ToString()));
          }
          if (DBConvert.ParseDouble(row["Length"].ToString()) >= 0)
          {
            inputParam[18] = new SqlDBParameter("@Length", SqlDbType.Float, DBConvert.ParseDouble(row["Length"].ToString()));
          }
          if (DBConvert.ParseDouble(row["Width"].ToString()) >= 0)
          {
            inputParam[19] = new SqlDBParameter("@Width", SqlDbType.Float, DBConvert.ParseDouble(row["Width"].ToString()));
          }
          if (DBConvert.ParseDouble(row["Thickness"].ToString()) >= 0)
          {
            inputParam[20] = new SqlDBParameter("@Thickness", SqlDbType.Float, DBConvert.ParseDouble(row["Thickness"].ToString()));
          }
          if (DBConvert.ParseDouble(row["Perimeter"].ToString()) >= 0)
          {
            inputParam[21] = new SqlDBParameter("@Perimeter", SqlDbType.Float, DBConvert.ParseDouble(row["Perimeter"].ToString()));
          }
          if (row["ContainerNo"].ToString().Trim().Length > 0)
          {
            inputParam[22] = new SqlDBParameter("@ContainerNo", SqlDbType.VarChar, row["ContainerNo"].ToString());
          }
          if (row["SerialNo"].ToString().Trim().Length > 0)
          {
            inputParam[23] = new SqlDBParameter("@SerialNo", SqlDbType.VarChar, row["SerialNo"].ToString());
          }
          if (row["LotNo"].ToString().Trim().Length > 0)
          {
            inputParam[24] = new SqlDBParameter("@LotNo", SqlDbType.VarChar, row["LotNo"].ToString());
          }
          if (DBConvert.ParseInt(row["CostCenterPid"].ToString()) > 0)
          {
            inputParam[25] = new SqlDBParameter("@CostCenterPid", SqlDbType.Int, DBConvert.ParseInt(row["CostCenterPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["SegmentPid"].ToString()) > 0)
          {
            inputParam[26] = new SqlDBParameter("@SegmentPid", SqlDbType.Int, DBConvert.ParseInt(row["SegmentPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["UnCostConstructionPid"].ToString()) > 0)
          {
            inputParam[27] = new SqlDBParameter("@UnCostConstructionPid", SqlDbType.Int, DBConvert.ParseInt(row["UnCostConstructionPid"].ToString()));
          }
          inputParam[28] = new SqlDBParameter("@CreateBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCInvoiceDetail_SaveDetail", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
       
      }
      return success;
    }

    private bool SaveConfirm()
    {
      bool success = true;
      SqlDBParameter[] inputParam = new SqlDBParameter[2];
      inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);

      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCInvoiceDetail_Confirm", inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        success = true;
      }
      else
      {
        success = false;
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
              success = this.SaveConfirm();
              if(success)
              {
                bool isPosted = chkConfirm.Checked;
                success = Utility.ACCPostTransaction(this.docTypePid, viewPid, isPosted, SharedObject.UserInfo.UserPid);
              }  
             
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
      //btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      //btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      //btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);      

      this.SetBlankForTextOfButton(this);
    }


    /// <summary>
    /// Function summary price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SummaryTotalAmount()
    {
      double total = 0;
      int currency = DBConvert.ParseInt(ucbCurrency.Value);
      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        if (DBConvert.ParseDouble(ugdData.Rows[i].Cells["TotalCost"].Value) >= 0)
        {
          total += DBConvert.ParseDouble(ugdData.Rows[i].Cells["TotalCost"].Value);
        }
      }
      uneSubTotalAmount.Value = total;
    }


    private void SetValueAmount()
    {
      if (!this.isLoadingData)
      {
        this.isLoadingData = true;
        double subTotalAmount = 0, discountPercent = 0, discountAmount = 0, taxPercent = 0, taxAmount = 0, totalAmount = 0, shippingFee = 0, extraFee = 0;
        DataTable dtDetail = (DataTable)ugdData.DataSource;
        double amount = DBConvert.ParseDouble(dtDetail.Compute("Sum(TotalCost)", "Quantity > 0").ToString());
        if (amount >= 0)
        {
          subTotalAmount = Math.Round(amount, this.oddNumber);
        }
        discountPercent = DBConvert.ParseDouble(uneDiscountPercent.Value);
        if (discountPercent >= 0)
        {
          discountAmount = Math.Round(subTotalAmount * (discountPercent / 100), this.oddNumber);
        }
        shippingFee = (DBConvert.ParseDouble(uneShippingFee.Value) == double.MinValue ? 0 : DBConvert.ParseDouble(uneShippingFee.Value));
        extraFee = (DBConvert.ParseDouble(uneExtraFee.Value) == double.MinValue ? 0 : DBConvert.ParseDouble(uneExtraFee.Value));
        taxPercent = DBConvert.ParseDouble(uneTaxPercent.Value);
        if (taxPercent >= 0)
        {
          taxAmount = Math.Round((subTotalAmount - discountAmount + shippingFee + extraFee) * (taxPercent / 100), this.oddNumber);
        }
        totalAmount = subTotalAmount - discountAmount + shippingFee + extraFee + taxAmount;

        uneSubTotalAmount.Value = subTotalAmount;
        uneDiscountAmount.Value = discountAmount;
        uneTaxAmount.Value = taxAmount;
        uneTotalAmount.Value = totalAmount;
        this.isLoadingData = false;
      }
    }
    #endregion Function

    #region Event
    private void ugdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdData);
      Utility.HideColumnsOnGrid(ugdData);   
      e.Layout.UseFixedHeaders = true;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["Quantity"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      if (this.actionCode == 1)
      {
        e.Layout.Override.AllowAddNew = AllowAddNew.Yes;
        e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
        e.Layout.Bands[0].Columns["Quantity"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      }  
      if (this.status > 0)
      {
        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Override.AllowAddNew = AllowAddNew.No;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      }

      

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["CountryPid"].ValueList = ucbddNation;
      e.Layout.Bands[0].Columns["EnvironmentInfo"].ValueList = ucbddEnvironment;
      e.Layout.Bands[0].Columns["CostCenterPid"].ValueList = ucbddCostCenter;
      e.Layout.Bands[0].Columns["SegmentPid"].ValueList = ucbddSegment;
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].ValueList = ucbddUnCostConstruction;
      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = ucbddMaterial;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["CountryPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CountryPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["EnvironmentInfo"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["EnvironmentInfo"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["CostCenterPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CostCenterPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["SegmentPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["SegmentPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["MaterialCode"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["MaterialCode"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["CountryPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["EnvironmentInfo"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["CostCenterPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["SegmentPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["InvoiceInPid"].Hidden = true;
      e.Layout.Bands[0].Columns["AccountPid"].Hidden = true;
      e.Layout.Bands[0].Columns["PODetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ACOffsetPid"].Hidden = true;
      e.Layout.Bands[0].Columns["WHReceiptDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["IDKho"].Hidden = true;
      // Set caption column
      e.Layout.Bands[0].Columns["CountryPid"].Header.Caption = "Xuất xứ";
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Mã sản phẩm";
      e.Layout.Bands[0].Columns["MaterialDescription"].Header.Caption = "Mô tả";
      e.Layout.Bands[0].Columns["Quantity"].Header.Caption = "Số lượng";
      e.Layout.Bands[0].Columns["UnitCost"].Header.Caption = "Đơn giá";
      e.Layout.Bands[0].Columns["TotalCost"].Header.Caption = "Tổng giá mua";
      e.Layout.Bands[0].Columns["DetailDesc"].Header.Caption = "Ghi chú";
      e.Layout.Bands[0].Columns["CountOfQty"].Header.Caption = "Số thanh\n(tấm)";
      e.Layout.Bands[0].Columns["CountOfQtyUnitCost"].Header.Caption = "Giá (tấm)";
      e.Layout.Bands[0].Columns["CountOfQtyTotalCost"].Header.Caption = "Thành tiền\n(tấm)";
      e.Layout.Bands[0].Columns["EnvironmentInfo"].Header.Caption = "Thông tin\nmôi trường";
      e.Layout.Bands[0].Columns["QualityPid"].Header.Caption = "Chất lượng";
      e.Layout.Bands[0].Columns["Length"].Header.Caption = "Dài";
      e.Layout.Bands[0].Columns["Width"].Header.Caption = "Rộng";
      e.Layout.Bands[0].Columns["Thickness"].Header.Caption = "Dày";
      e.Layout.Bands[0].Columns["Perimeter"].Header.Caption = "Hoành";
      e.Layout.Bands[0].Columns["ContainerNo"].Header.Caption = "Mã cont";
      e.Layout.Bands[0].Columns["SerialNo"].Header.Caption = "Mã kiện";
      e.Layout.Bands[0].Columns["LotNo"].Header.Caption = "Mã lô hàng";
      e.Layout.Bands[0].Columns["CostCenterPid"].Header.Caption = "TTCP";
      e.Layout.Bands[0].Columns["SegmentPid"].Header.Caption = "Khoản mục chi phí";
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].Header.Caption = "CPXDCBDD";
      /*
      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns[""].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns[""].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      
      // Enable support for displaying errors through IDataErrorInfo interface. By default
			// the functionality is disabled.
			e.Layout.Override.SupportDataErrorInfo = SupportDataErrorInfo.RowsAndCells;

			// Set data error related appearances.
			e.Layout.Override.DataErrorCellAppearance.ForeColor = Color.Red;
			e.Layout.Override.DataErrorRowAppearance.BackColor = Color.LightYellow;
			e.Layout.Override.DataErrorRowSelectorAppearance.BackColor = Color.Green;

			// Make the row selectors bigger so they can accomodate the data error icon as 
			// well active row indicator.
			e.Layout.Override.RowSelectorWidth = 32;

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      
   
      
      // Set Align
      e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
      
      // Set Width
      e.Layout.Bands[0].Columns[""].MaxWidth = 100;
      e.Layout.Bands[0].Columns[""].MinWidth = 100;
      
      // Set Column Style
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      // Add Column Selected
      if (!e.Layout.Bands[0].Columns.Exists("Selected"))
      {
        UltraGridColumn checkedCol = e.Layout.Bands[0].Columns.Add();
        checkedCol.Key = "Selected";
        checkedCol.Header.Caption = string.Empty;
        checkedCol.Header.CheckBoxVisibility = HeaderCheckBoxVisibility.Always;        
        checkedCol.DataType = typeof(bool);
        checkedCol.Header.VisiblePosition = 0;
      } 
      
      // Set color
      ultraGridInformation.Rows[0].Appearance.BackColor = Color.Yellow;
      ultraGridInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;
      
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      
      // Read only
      e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      
      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      // Set language
      e.Layout.Bands[0].Columns["EID"].Header.Caption = rm.GetString("EID", ConstantClass.CULTURE);
      //Total
      
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Amount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "Tổng = {0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";
      */
    }

    private void ugdData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      this.NeedToSave = true;
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString();
      string value = e.Cell.Value.ToString();
      string commandText = string.Empty;
      switch (columnName)
      {
        case "MaterialCode":
          e.Cell.Row.Cells["MaterialDescription"].Value = ucbddMaterial.SelectedRow.Cells["MaterialDescription"].Value.ToString();
          break;
        case "Quantity":
        case "UnitCost":
          double qty = DBConvert.ParseDouble(e.Cell.Row.Cells["Quantity"].Value);
          double unitPrice = DBConvert.ParseDouble(e.Cell.Row.Cells["UnitCost"].Value);
          if (qty >= 0 && unitPrice >= 0)
          {
            e.Cell.Row.Cells["TotalCost"].Value = Math.Round(qty * unitPrice, this.oddNumber);
          }
          else
          {
            e.Cell.Row.Cells["TotalCost"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
      this.SummaryTotalAmount();
    }

    private void ugdData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().Trim();
      string text = e.Cell.Text.Trim();
      string value = e.NewValue.ToString();
      switch (colName)
      {
        case "UnitCost":
          {
            if (text.Trim().Length == 0 || DBConvert.ParseDouble(value) == double.MinValue || DBConvert.ParseDouble(value) < 0)
            {
              WindowUtinity.ShowMessageErrorFromText("Đơn giá không hợp lệ.");
              e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
              e.Cancel = true;
            }
          }
          break;
        case "Quantity":
          {
            if (text.Trim().Length == 0 || DBConvert.ParseDouble(value) == double.MinValue || DBConvert.ParseDouble(value) < 0)
            {
              WindowUtinity.ShowMessageErrorFromText("Số lượng không hợp lệ.");
              e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
              e.Cancel = true;
            }
          }
          break;
        case "Length":
          {
            if (text.Trim().Length == 0 || DBConvert.ParseDouble(value) == double.MinValue || DBConvert.ParseDouble(value) < 0)
            {
              WindowUtinity.ShowMessageErrorFromText("Độ dài không hợp lệ.");
              e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
              e.Cancel = true;
            }
          }
          break;
        case "Width":
          {
            if (text.Trim().Length == 0 || DBConvert.ParseDouble(value) == double.MinValue || DBConvert.ParseDouble(value) < 0)
            {
              WindowUtinity.ShowMessageErrorFromText("Độ rộng không hợp lệ.");
              e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
              e.Cancel = true;
            }
          }
          break;
        case "Thickness":
          {
            if (text.Trim().Length == 0 || DBConvert.ParseDouble(value) == double.MinValue || DBConvert.ParseDouble(value) < 0)
            {
              WindowUtinity.ShowMessageErrorFromText("Độ dày không hợp lệ.");
              e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
              e.Cancel = true;
            }
          }
          break;
        case "Perimeter":
          {
            if (text.Trim().Length == 0 || DBConvert.ParseDouble(value) == double.MinValue || DBConvert.ParseDouble(value) < 0)
            {
              WindowUtinity.ShowMessageErrorFromText("Độ hoành không hợp lệ.");
              e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
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
      //Utility.ExportToExcelWithDefaultPath(ugdData, "Data");
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


    private void ucbObject_ValueChanged(object sender, EventArgs e)
    {
      
    }


    private void uneSubTotalAmount_ValueChanged(object sender, EventArgs e)
    {
      double subTotalAmount, discount, shippingFee, extraFee, taxFee;
      subTotalAmount = DBConvert.ParseDouble(uneSubTotalAmount.Value) >= 0 ? DBConvert.ParseDouble(uneSubTotalAmount.Value) : 0;
      discount = DBConvert.ParseDouble(uneDiscountAmount.Value) >= 0 ? DBConvert.ParseDouble(uneDiscountAmount.Value) : 0;
      shippingFee = DBConvert.ParseDouble(uneShippingFee.Value) >= 0 ? DBConvert.ParseDouble(uneShippingFee.Value) : 0;
      extraFee = DBConvert.ParseDouble(uneExtraFee.Value) >= 0 ? DBConvert.ParseDouble(uneExtraFee.Value) : 0;
      taxFee = DBConvert.ParseDouble(uneTaxAmount.Value) >= 0 ? DBConvert.ParseDouble(uneTaxAmount.Value) : 0;
      uneTotalAmount.Value = subTotalAmount + shippingFee + extraFee + taxFee - discount;
    }

    private void uneDiscountAmount_ValueChanged(object sender, EventArgs e)
    {
      double subTotalAmount, discount, shippingFee, extraFee, taxFee;
      subTotalAmount = DBConvert.ParseDouble(uneSubTotalAmount.Value) >= 0 ? DBConvert.ParseDouble(uneSubTotalAmount.Value) : 0;
      discount = DBConvert.ParseDouble(uneDiscountAmount.Value) >= 0 ? DBConvert.ParseDouble(uneDiscountAmount.Value) : 0;
      shippingFee = DBConvert.ParseDouble(uneShippingFee.Value) >= 0 ? DBConvert.ParseDouble(uneShippingFee.Value) : 0;
      extraFee = DBConvert.ParseDouble(uneExtraFee.Value) >= 0 ? DBConvert.ParseDouble(uneExtraFee.Value) : 0;
      taxFee = DBConvert.ParseDouble(uneTaxAmount.Value) >= 0 ? DBConvert.ParseDouble(uneTaxAmount.Value) : 0;
      uneTotalAmount.Value = subTotalAmount + shippingFee + extraFee + taxFee - discount;
    }

    private void uneShippingFee_ValueChanged(object sender, EventArgs e)
    {
      double subTotalAmount, discount, shippingFee, extraFee, taxFee;
      subTotalAmount = DBConvert.ParseDouble(uneSubTotalAmount.Value) >= 0 ? DBConvert.ParseDouble(uneSubTotalAmount.Value) : 0;
      discount = DBConvert.ParseDouble(uneDiscountAmount.Value) >= 0 ? DBConvert.ParseDouble(uneDiscountAmount.Value) : 0;
      shippingFee = DBConvert.ParseDouble(uneShippingFee.Value) >= 0 ? DBConvert.ParseDouble(uneShippingFee.Value) : 0;
      extraFee = DBConvert.ParseDouble(uneExtraFee.Value) >= 0 ? DBConvert.ParseDouble(uneExtraFee.Value) : 0;
      taxFee = DBConvert.ParseDouble(uneTaxAmount.Value) >= 0 ? DBConvert.ParseDouble(uneTaxAmount.Value) : 0;
      uneTotalAmount.Value = subTotalAmount + shippingFee + extraFee + taxFee - discount;
    }

    private void uneExtraFee_ValueChanged(object sender, EventArgs e)
    {
      double subTotalAmount, discount, shippingFee, extraFee, taxFee;
      subTotalAmount = DBConvert.ParseDouble(uneSubTotalAmount.Value) >= 0 ? DBConvert.ParseDouble(uneSubTotalAmount.Value) : 0;
      discount = DBConvert.ParseDouble(uneDiscountAmount.Value) >= 0 ? DBConvert.ParseDouble(uneDiscountAmount.Value) : 0;
      shippingFee = DBConvert.ParseDouble(uneShippingFee.Value) >= 0 ? DBConvert.ParseDouble(uneShippingFee.Value) : 0;
      extraFee = DBConvert.ParseDouble(uneExtraFee.Value) >= 0 ? DBConvert.ParseDouble(uneExtraFee.Value) : 0;
      taxFee = DBConvert.ParseDouble(uneTaxAmount.Value) >= 0 ? DBConvert.ParseDouble(uneTaxAmount.Value) : 0;
      uneTotalAmount.Value = subTotalAmount + shippingFee + extraFee + taxFee - discount;
    }

    private void uneTaxAmount_ValueChanged(object sender, EventArgs e)
    {
      double subTotalAmount, discount, shippingFee, extraFee, taxFee;
      subTotalAmount = DBConvert.ParseDouble(uneSubTotalAmount.Value) >= 0 ? DBConvert.ParseDouble(uneSubTotalAmount.Value) : 0;
      discount = DBConvert.ParseDouble(uneDiscountAmount.Value) >= 0 ? DBConvert.ParseDouble(uneDiscountAmount.Value) : 0;
      shippingFee = DBConvert.ParseDouble(uneShippingFee.Value) >= 0 ? DBConvert.ParseDouble(uneShippingFee.Value) : 0;
      extraFee = DBConvert.ParseDouble(uneExtraFee.Value) >= 0 ? DBConvert.ParseDouble(uneExtraFee.Value) : 0;
      taxFee = DBConvert.ParseDouble(uneTaxAmount.Value) >= 0 ? DBConvert.ParseDouble(uneTaxAmount.Value) : 0;
      uneTotalAmount.Value = subTotalAmount + shippingFee + extraFee + taxFee - discount;
    }

    private void uneDiscountPercent_ValueChanged(object sender, EventArgs e)
    {
      double subTotalAmount, discountPercent, discountAmount;
      subTotalAmount = DBConvert.ParseDouble(uneSubTotalAmount.Value) >= 0 ? DBConvert.ParseDouble(uneSubTotalAmount.Value) : 0;
      discountPercent = DBConvert.ParseDouble(uneDiscountPercent.Value) >= 0 ? DBConvert.ParseDouble(uneDiscountPercent.Value) : 0;
      discountAmount = DBConvert.ParseInt(ucbCurrency.Value) == 1 ? Math.Round((discountPercent * subTotalAmount) / 100, 0, MidpointRounding.AwayFromZero)
                                                              : Math.Round((discountPercent * subTotalAmount) / 100, 2, MidpointRounding.AwayFromZero);
      uneDiscountAmount.Value = discountAmount;
    }

    private void uneTaxPercent_ValueChanged(object sender, EventArgs e)
    {
      double subTotalAmount, taxPercent, taxAmount, shippingFee, extraFee, discount;
      subTotalAmount = DBConvert.ParseDouble(uneSubTotalAmount.Value) >= 0 ? DBConvert.ParseDouble(uneSubTotalAmount.Value) : 0;
      taxPercent = DBConvert.ParseDouble(uneTaxPercent.Value) >= 0 ? DBConvert.ParseDouble(uneTaxPercent.Value) : 0;
    
      discount = DBConvert.ParseDouble(uneDiscountAmount.Value) >= 0 ? DBConvert.ParseDouble(uneDiscountAmount.Value) : 0;
      shippingFee = DBConvert.ParseDouble(uneShippingFee.Value) >= 0 ? DBConvert.ParseDouble(uneShippingFee.Value) : 0;
      extraFee = DBConvert.ParseDouble(uneExtraFee.Value) >= 0 ? DBConvert.ParseDouble(uneExtraFee.Value) : 0;
      taxPercent = DBConvert.ParseDouble(uneTaxPercent.Value) >= 0 ? DBConvert.ParseDouble(uneTaxPercent.Value) : 0;
      taxAmount = DBConvert.ParseInt(ucbCurrency.Value) == 1 ? Math.Round((taxPercent * (subTotalAmount + shippingFee + extraFee - discount)) / 100, 0, MidpointRounding.AwayFromZero)
                                                            : Math.Round((taxPercent * (subTotalAmount + shippingFee + extraFee - discount)) / 100, 2, MidpointRounding.AwayFromZero);
      uneTaxAmount.Value = taxAmount;
    }

    private void ucbSource_ValueChanged(object sender, EventArgs e)
    {
      if(DBConvert.ParseInt(ucbSource.Value) == 1)
      {
        ucbCurrency.Value = 1;
        ucbCurrency.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
      }
      else
      {
        ucbCurrency.ReadOnly = false;
        uneExchangeRate.ReadOnly = false;
      }
    }
    private void ucbCurrency_ValueChanged(object sender, EventArgs e)
    {      
      if (ucbCurrency.SelectedRow != null)
      {
        uneExchangeRate.Value = DBConvert.ParseDouble(ucbCurrency.SelectedRow.Cells["ExchangeRate"].Value);
      }
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
    private void btnClearDeposit_Click(object sender, EventArgs e)
    {
      viewACC_13_005 view = new viewACC_13_005();
      view.invoiceInPid = this.viewPid;
      Shared.Utility.WindowUtinity.ShowView(view, "Cấn trừ phiếu chi", false, ViewState.Window);
    }
    private void Amount_ValueChanged(object sender, EventArgs e)
    {
      this.SetValueAmount();
    }
    #endregion Event
  }
}
