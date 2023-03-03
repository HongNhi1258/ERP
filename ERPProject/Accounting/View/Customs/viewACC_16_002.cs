/*
  Author      : Nguyen Van Tron
  Date        : 09/08/2022
  Description : Customs Declaration
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
  public partial class viewACC_16_002 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    public int actionCode = int.MinValue;
    public string actionName = string.Empty;
    private IList listDeletedPid = new ArrayList();
    private int docTypePid = 115;
    private long invoicePid = 0;
    private bool isLoadedPostTransaction = false;
    private int status = 0;
    private DataTable dtObject = new DataTable();
    private int oddNumber = 2;
    private bool isLoadingData = false;
    #endregion Field

    #region Init
    public viewACC_16_002()
    {
      InitializeComponent();
    }

    private void viewACC_16_002_Load(object sender, EventArgs e)
    {
      Utility.Format_UltraNumericEditor(tlpForm);
      // Add ask before closing form even if user change data
      this.SetAutoAskSaveWhenCloseForm(ugbInformation);      
      this.SetBlankForTextOfButton(this);      
      this.LoadData();
      this.InitData();
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
      SqlDBParameter[] inputParam = new SqlDBParameter[] { new SqlDBParameter("@ActionCode", SqlDbType.Int, this.actionCode) };
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCCustomsDeclaration_Init", inputParam);

      Utility.LoadUltraCombo(ucbObject, dsInit.Tables[0], "KeyValue", "Object", false, new string[] { "ObjectPid", "Object", "ObjectType", "KeyValue" });
      ucbObject.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[1], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate", "OddNumber" });
      ucbCurrency.Value = 2; //USD
      Utility.LoadUltraCombo(ucbType, dsInit.Tables[2], "Pid", "DisplayText", false, new string[] { "Pid", "DisplayText" });
      ucbType.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

      // Set Language
      this.SetLanguage();
    }

    /// <summary>
    /// Read Only for key main data
    /// </summary>
    private void ReadOnlyKeyMainData()
    {      
      if(ugdData.Rows.Count > 0)
      {
        ucbObject.ReadOnly = true;
        btnInvoice.Enabled = false;
      }
      else if(this.status == 0)
      {
        ucbObject.ReadOnly = false;
        btnInvoice.Enabled = true;
      }
    }

    private void SetStatusControl()
    {
      if (this.status > 0)
      {        
        txtDeclarationDesc.ReadOnly = true;
        udtDeclarationDate.ReadOnly = true;
        txtDeclarationNo.ReadOnly = true;
        udtRegisterDate.ReadOnly = true;
        txtBillOfLadingNo.ReadOnly = true;
        udtBillOfLadingDate.ReadOnly = true;
        ucbObject.ReadOnly = true;
        ucbCurrency.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
        ucbType.ReadOnly = true;
        txtTaxNo.ReadOnly = true;
        txtCommercialInvoiceNo.ReadOnly = true;
        txtContractNo.ReadOnly = true;
        udtContractDate.ReadOnly = true;
        udtVATDate.ReadOnly = true;        
        txtReferenceNo.ReadOnly = true;
        uneTotalTaxAmount.ReadOnly = true;
        uneTaxPercent.ReadOnly = true;
        uneVATTaxPercent.ReadOnly = true;
        uneConsumptionTaxPercent.ReadOnly = true;
        uneEnviromentTaxPercent.ReadOnly = true;

        btnSave.Enabled = false;
        chkConfirm.Enabled = false;
        btnInvoice.Enabled = false;
      }

      this.ReadOnlyKeyMainData();      
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        this.actionCode = DBConvert.ParseInt(dtMain.Rows[0]["ActionCode"]);
        this.invoicePid = DBConvert.ParseLong(dtMain.Rows[0]["InvoicePid"]);
        txtDeclarationCode.Text = dtMain.Rows[0]["DeclarationCode"].ToString();
        txtDeclarationDesc.Text = dtMain.Rows[0]["DeclarationDesc"].ToString();
        udtDeclarationDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["DeclarationDate"]);
        txtActionName.Text = dtMain.Rows[0]["ActionName"].ToString();
        lbStatusDes.Text = dtMain.Rows[0]["StatusText"].ToString();
        txtDeclarationNo.Text = dtMain.Rows[0]["DeclarationNo"].ToString();
        udtRegisterDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["RegisterDate"]);
        txtBillOfLadingNo.Text = dtMain.Rows[0]["BillOfLadingNo"].ToString();
        if(dtMain.Rows[0]["BillOfLadingDate"].ToString().Length > 0) 
        {
          udtBillOfLadingDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["BillOfLadingDate"]);
        }
        ucbObject.Value = dtMain.Rows[0]["ObjectValue"];
        ucbCurrency.Value = DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"]);
        uneExchangeRate.Value = DBConvert.ParseDouble(dtMain.Rows[0]["ExchangeRate"]);
        if (DBConvert.ParseInt(dtMain.Rows[0]["Type"]) > 0)
        {
          ucbType.Value = DBConvert.ParseInt(dtMain.Rows[0]["Type"]);
        }
        txtTaxNo.Text = dtMain.Rows[0]["TaxNo"].ToString();
        txtCommercialInvoiceNo.Text = dtMain.Rows[0]["CommercialInvoiceNo"].ToString();
        txtContractNo.Text = dtMain.Rows[0]["ContractNo"].ToString();
        if (dtMain.Rows[0]["ContractDate"].ToString().Length > 0)
        {
          udtContractDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["ContractDate"]);
        }
        if (dtMain.Rows[0]["ClearanceDate"].ToString().Length > 0)
        {
          udtClearanceDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["ClearanceDate"]);
        }
        if (dtMain.Rows[0]["VATDate"].ToString().Length > 0)
        {
          udtVATDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["VATDate"]);
        }
        txtReferenceNo.Text = dtMain.Rows[0]["ReferenceNo"].ToString();
        uneTotalTaxAmount.Value = dtMain.Rows[0]["TotalTaxAmount"];
        uneTaxPercent.Value = dtMain.Rows[0]["TaxPercent"];
        uneVATTaxPercent.Value = dtMain.Rows[0]["VATTaxPercent"];
        uneConsumptionTaxPercent.Value = dtMain.Rows[0]["ConsumptionTaxPercent"];
        uneEnviromentTaxPercent.Value = dtMain.Rows[0]["EnviromentTaxPercent"];

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
          txtDeclarationCode.Text = outputParam[0].Value.ToString();
          udtDeclarationDate.Value = DateTime.Now;          
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
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCCustomsDeclaration_Load", inputParam);
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
      if (udtDeclarationDate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText(string.Format("{0} không được để trống!!!", lbDeclarationDate.Text));
        udtDeclarationDate.Focus();
        return false;
      }
      if (txtDeclarationNo.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageErrorFromText(string.Format("{0} không được để trống!!!", lblDeclarationNo.Text));
        txtDeclarationNo.Focus();
        return false;
      }
      if (udtRegisterDate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText(string.Format("{0} không được để trống!!!", lblRegisterDate.Text));
        udtRegisterDate.Focus();
        return false;
      }
      if (txtBillOfLadingNo.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageErrorFromText(string.Format("{0} không được để trống!!!", lblBillOfLadingNo.Text));
        txtBillOfLadingNo.Focus();
        return false;
      }
      if (ucbObject.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText(string.Format("{0} không được để trống!!!", lblObject.Text));
        ucbObject.Focus();
        return false;
      }
      if (ucbCurrency.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText(string.Format("{0} không được để trống!!!", lblCurrency.Text));
        ucbCurrency.Focus();
        return false;
      }          
      if(uneExchangeRate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText(string.Format("{0} không được để trống!!!", lblExchangeRate.Text));
        uneExchangeRate.Focus();
        return false;
      }

      return true;
    }

    private bool SaveMain()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[26];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      }
      if (txtDeclarationDesc.Text.Trim().Length > 0)
      {
        inputParam[1] = new SqlDBParameter("@DeclarationDesc", SqlDbType.NVarChar, txtDeclarationDesc.Text.Trim());
      }
      inputParam[2] = new SqlDBParameter("@DeclarationDate", SqlDbType.Date, DBConvert.ParseDateTime(udtDeclarationDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      inputParam[3] = new SqlDBParameter("@ObjectPid", SqlDbType.Int, ucbObject.SelectedRow.Cells["ObjectPid"].Value);
      inputParam[4] = new SqlDBParameter("@ObjectType", SqlDbType.Int, ucbObject.SelectedRow.Cells["ObjectType"].Value);
      if (txtReferenceNo.Text.Trim().Length > 0)
      {
        inputParam[5] = new SqlDBParameter("@ReferenceNo", SqlDbType.VarChar, txtReferenceNo.Text.Trim());
      }
      if (txtDeclarationNo.Text.Trim().Length > 0)
      {
        inputParam[6] = new SqlDBParameter("@DeclarationNo", SqlDbType.VarChar, txtDeclarationNo.Text.Trim());
      }
      if (txtBillOfLadingNo.Text.Trim().Length > 0)
      {
        inputParam[7] = new SqlDBParameter("@BillOfLadingNo", SqlDbType.VarChar, txtBillOfLadingNo.Text.Trim());
      }
      DateTime billOfLadingDate = DateTime.MinValue;
      if (udtBillOfLadingDate.Value != null)
      {
        billOfLadingDate = DBConvert.ParseDateTime(udtBillOfLadingDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }
      if (billOfLadingDate != DateTime.MinValue)
      {
        inputParam[8] = new SqlDBParameter("@BillOfLadingDate", SqlDbType.Date, billOfLadingDate);
      }
      DateTime registerDate = DBConvert.ParseDateTime(udtRegisterDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      if (registerDate != DateTime.MinValue)
      {
        inputParam[9] = new SqlDBParameter("@RegisterDate", SqlDbType.Date, registerDate);
      }
      if(ucbType.SelectedRow != null)
      {
        inputParam[10] = new SqlDBParameter("@Type", SqlDbType.Int, ucbType.Value);
      }
      if (txtContractNo.Text.Trim().Length > 0)
      {
        inputParam[11] = new SqlDBParameter("@ContractNo", SqlDbType.VarChar, txtContractNo.Text.Trim());
      }
      DateTime contractDate = DateTime.MinValue;
      if (udtContractDate.Value != null)
      {
        contractDate = DBConvert.ParseDateTime(udtContractDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }
      if (contractDate != DateTime.MinValue)
      {
        inputParam[12] = new SqlDBParameter("@ContractDate", SqlDbType.Date, contractDate);
      }
      if (txtCommercialInvoiceNo.Text.Trim().Length > 0)
      {
        inputParam[13] = new SqlDBParameter("@CommercialInvoiceNo", SqlDbType.VarChar, txtCommercialInvoiceNo.Text.Trim());
      }
      DateTime vATDate = DateTime.MinValue;
      if (udtVATDate.Value != null)
      {
        vATDate = DBConvert.ParseDateTime(udtVATDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }
      if (vATDate != DateTime.MinValue)
      {
        inputParam[14] = new SqlDBParameter("@VATDate", SqlDbType.Date, vATDate);
      }
      DateTime clearanceDate = DateTime.MinValue;
      if (udtClearanceDate.Value != null)
      {
        clearanceDate = DBConvert.ParseDateTime(udtClearanceDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }
      if (clearanceDate != DateTime.MinValue)
      {
        inputParam[15] = new SqlDBParameter("@ClearanceDate", SqlDbType.Date, clearanceDate);
      }
      inputParam[16] = new SqlDBParameter("@CurrencyPid", SqlDbType.Int, DBConvert.ParseInt(ucbCurrency.Value));
      inputParam[17] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, DBConvert.ParseDouble(uneExchangeRate.Value));
      if(uneTaxPercent.Value != null)
      {
        inputParam[18] = new SqlDBParameter("@TaxPercent", SqlDbType.Float, uneTaxPercent.Value);
      }
      if (uneVATTaxPercent.Value != null)
      {
        inputParam[19] = new SqlDBParameter("@VATTaxPercent", SqlDbType.Float, uneVATTaxPercent.Value);
      }
      if (uneConsumptionTaxPercent.Value != null)
      {
        inputParam[20] = new SqlDBParameter("@ConsumptionTaxPercent", SqlDbType.Float, uneConsumptionTaxPercent.Value);
      }
      if (uneEnviromentTaxPercent.Value != null)
      {
        inputParam[21] = new SqlDBParameter("@EnviromentTaxPercent", SqlDbType.Float, uneEnviromentTaxPercent.Value);
      }
      if (this.invoicePid > 0)
      {
        inputParam[22] = new SqlDBParameter("@InvoicePid", SqlDbType.Int, this.invoicePid);
      }
      inputParam[23] = new SqlDBParameter("@ActionCode", SqlDbType.Int, this.actionCode);
      inputParam[24] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParam[25] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCCustomsDeclaration_Save", inputParam, outputParam);
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
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCCustomsDeclarationDetail_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ugdData.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        SqlDBParameter[] inputParam = new SqlDBParameter[17];
        long pid = DBConvert.ParseLong(row["Pid"].ToString());
        if (pid > 0) // Update
        {
          inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
        }
        inputParam[1] = new SqlDBParameter("@DeclarationPid", SqlDbType.BigInt, this.viewPid);
        inputParam[2] = new SqlDBParameter("@ProductCode", SqlDbType.VarChar, row["ProductCode"].ToString());
        inputParam[3] = new SqlDBParameter("@Qty", SqlDbType.Float, DBConvert.ParseDouble(row["Qty"].ToString()));
        inputParam[4] = new SqlDBParameter("@UnitCost", SqlDbType.Float, DBConvert.ParseDouble(row["UnitCost"].ToString()));
        inputParam[5] = new SqlDBParameter("@UnitCostExchange", SqlDbType.Float, DBConvert.ParseDouble(row["UnitCostExchange"].ToString()));
        inputParam[6] = new SqlDBParameter("@TotalCost", SqlDbType.Float, DBConvert.ParseDouble(row["TotalCost"].ToString()));
        inputParam[7] = new SqlDBParameter("@TotalCostExchange", SqlDbType.Float, DBConvert.ParseDouble(row["TotalCostExchange"].ToString()));
        double taxPercent = DBConvert.ParseDouble(row["TaxPercent"].ToString());
        double taxAmount = DBConvert.ParseDouble(row["TaxAmount"].ToString());
        double vATTaxPercent = DBConvert.ParseDouble(row["VATTaxPercent"].ToString());
        double vATTaxAmount = DBConvert.ParseDouble(row["VATTaxAmount"].ToString());
        double consumptionTaxPercent = DBConvert.ParseDouble(row["ConsumptionTaxPercent"].ToString());
        double consumptionTaxAmount = DBConvert.ParseDouble(row["ConsumptionTaxAmount"].ToString());
        double enviromentTaxPercent = DBConvert.ParseDouble(row["EnviromentTaxPercent"].ToString());
        double enviromentTaxAmount = DBConvert.ParseDouble(row["EnviromentTaxAmount"].ToString());
        if (taxPercent >= 0)
        {
          inputParam[8] = new SqlDBParameter("@TaxPercent", SqlDbType.Float, taxPercent);
        }
        if (taxAmount >= 0)
        {
          inputParam[9] = new SqlDBParameter("@TaxAmount", SqlDbType.Float, taxAmount);
        }
        if (vATTaxPercent >= 0)
        {
          inputParam[10] = new SqlDBParameter("@VATTaxPercent", SqlDbType.Float, vATTaxPercent);
        }
        if (vATTaxAmount >= 0)
        {
          inputParam[11] = new SqlDBParameter("@VATTaxAmount", SqlDbType.Float, vATTaxAmount);
        }
        if (consumptionTaxPercent >= 0)
        {
          inputParam[12] = new SqlDBParameter("@ConsumptionTaxPercent", SqlDbType.Float, consumptionTaxPercent);
        }
        if (consumptionTaxAmount >= 0)
        {
          inputParam[13] = new SqlDBParameter("@ConsumptionTaxAmount", SqlDbType.Float, consumptionTaxAmount);
        }
        if (enviromentTaxPercent >= 0)
        {
          inputParam[14] = new SqlDBParameter("@EnviromentTaxPercent", SqlDbType.Float, enviromentTaxPercent);
        }
        if (enviromentTaxAmount >= 0)
        {
          inputParam[15] = new SqlDBParameter("@EnviromentTaxAmount", SqlDbType.Float, enviromentTaxAmount);
        }
        inputParam[16] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCCustomsDeclarationDetail_Save", inputParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;

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
              this.ConfirmDeclaration();
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


    private void CalculateDeclarationAmount()
    {
      DataTable dtSource = (DataTable)ugdData.DataSource;
      uneTotalTaxAmount.Value = dtSource.Compute("Sum(TotalTaxAmount)", "TotalTaxAmount > 0");      
    }
    #endregion Function

    #region Event
    private void ugdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdData);      
      e.Layout.UseFixedHeaders = true;
      e.Layout.AutoFitStyle = AutoFitStyle.None;
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      // Allow update, delete, add new
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      if (this.status == 0)
      {
        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;          
      }
      else
      {
        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;                
      }

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Set caption column
      e.Layout.Bands[0].Columns["ProductCode"].Header.Caption = "Mã sản phẩm";
      e.Layout.Bands[0].Columns["ProductName"].Header.Caption = "Tên sản phẩm";     
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Số lượng";
      e.Layout.Bands[0].Columns["Unit"].Header.Caption = "Đơn vị";
      e.Layout.Bands[0].Columns["UnitCost"].Header.Caption = "Đơn giá\nnguyên tệ";
      e.Layout.Bands[0].Columns["UnitCostExchange"].Header.Caption = "Đơn giá\nVNĐ";
      e.Layout.Bands[0].Columns["TotalCost"].Header.Caption = "Thành tiền\nnguyên tệ";
      e.Layout.Bands[0].Columns["TotalCostExchange"].Header.Caption = "Thành tiền\nVNĐ";      
      e.Layout.Bands[0].Columns["TaxPercent"].Header.Caption = "% Thuế\nnhập khẩu";
      e.Layout.Bands[0].Columns["TaxAmount"].Header.Caption = "Tiền thuế\nnhập khẩu";
      e.Layout.Bands[0].Columns["VATTaxPercent"].Header.Caption = "% Thuế\nGTGT";
      e.Layout.Bands[0].Columns["VATTaxAmount"].Header.Caption = "Tiền thuế\nGTGT";
      e.Layout.Bands[0].Columns["ConsumptionTaxPercent"].Header.Caption = "% Thuế tiêu thụ\nđặc biệt";
      e.Layout.Bands[0].Columns["ConsumptionTaxAmount"].Header.Caption = "Tiền thuế tiêu thụ\nđặc biệt";
      e.Layout.Bands[0].Columns["EnviromentTaxPercent"].Header.Caption = "% Thuế bảo vệ\nmôi trường";
      e.Layout.Bands[0].Columns["EnviromentTaxAmount"].Header.Caption = "Tiền thuế bảo vệ\nmôi trường";
      e.Layout.Bands[0].Columns["TotalTaxAmount"].Header.Caption = "Tổng tiền thuế";


      // Read Only
      e.Layout.Bands[0].Columns["ProductCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ProductName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["UnitCost"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["UnitCostExchange"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalCost"].CellActivation = Activation.ActivateOnly;      
      e.Layout.Bands[0].Columns["TotalTaxAmount"].CellActivation = Activation.ActivateOnly;

      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        ugdData.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalTaxAmount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";
    }

    private void ugdData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      this.NeedToSave = true;      
      string columnName = e.Cell.Column.ToString();
      string value = e.Cell.Value.ToString();      
      switch (columnName)
      {
        case "TotalCostExchange":
        case "TaxPercent":        
        case "VATTaxPercent":        
        case "ConsumptionTaxPercent":        
        case "EnviromentTaxPercent":        
          if (!this.isLoadingData)
          {
            this.isLoadingData = true;
            double totalCostExchange = 0, taxPercent = 0, consumptionTaxPercent = 0, enviromentTaxPercent = 0, vATTaxPercent = 0;
            double taxAmount = 0, consumptionTaxAmount = 0, enviromentTaxAmount = 0, vATTaxAmount = 0, totalTaxAmount = 0;
            totalCostExchange = DBConvert.ParseDouble(e.Cell.Row.Cells["TotalCostExchange"].Value);
            taxPercent = DBConvert.ParseDouble(e.Cell.Row.Cells["TaxPercent"].Value);
            consumptionTaxPercent = DBConvert.ParseDouble(e.Cell.Row.Cells["ConsumptionTaxPercent"].Value);
            enviromentTaxPercent = DBConvert.ParseDouble(e.Cell.Row.Cells["EnviromentTaxPercent"].Value);
            vATTaxPercent = DBConvert.ParseDouble(e.Cell.Row.Cells["VATTaxPercent"].Value);
            if (taxPercent >= 0)
            {
              taxAmount = Math.Round((taxPercent / 100) * totalCostExchange, 0, MidpointRounding.AwayFromZero);
              e.Cell.Row.Cells["TaxAmount"].Value = taxAmount;
              totalTaxAmount += taxAmount;
            }  
            else
            {
              e.Cell.Row.Cells["TaxAmount"].Value = DBNull.Value;
            }
            if (consumptionTaxPercent >= 0)
            {
              consumptionTaxAmount = Math.Round((consumptionTaxPercent / 100) * (totalCostExchange + taxAmount), 0, MidpointRounding.AwayFromZero);
              e.Cell.Row.Cells["ConsumptionTaxAmount"].Value = consumptionTaxAmount;
              totalTaxAmount += consumptionTaxAmount;
            }
            else
            {
              e.Cell.Row.Cells["ConsumptionTaxAmount"].Value = DBNull.Value;
            }
            if (enviromentTaxPercent >= 0)
            {
              enviromentTaxAmount = Math.Round((enviromentTaxPercent / 100) * totalCostExchange, 0, MidpointRounding.AwayFromZero);
              e.Cell.Row.Cells["EnviromentTaxAmount"].Value = enviromentTaxAmount;
              totalTaxAmount += enviromentTaxAmount;
            }
            else
            {
              e.Cell.Row.Cells["EnviromentTaxAmount"].Value = DBNull.Value;
            }
            if (vATTaxPercent >= 0)
            {
              vATTaxAmount = Math.Round((vATTaxPercent / 100) * (totalCostExchange + taxAmount + consumptionTaxAmount + enviromentTaxAmount), 0, MidpointRounding.AwayFromZero);
              e.Cell.Row.Cells["VATTaxAmount"].Value = vATTaxAmount;
              totalTaxAmount += vATTaxAmount;
            }
            else
            {
              e.Cell.Row.Cells["VATTaxAmount"].Value = DBNull.Value;
            }
            e.Cell.Row.Cells["TotalTaxAmount"].Value = totalTaxAmount;

            this.isLoadingData = false;
          }
          break;        
        case "TaxAmount":        
        case "VATTaxAmount":        
        case "ConsumptionTaxAmount":        
        case "EnviromentTaxAmount":
          if (!this.isLoadingData)
          {
            this.isLoadingData = true;
            double totalCostExchange = 0, taxPercent = 0, consumptionTaxPercent = 0, enviromentTaxPercent = 0, vATTaxPercent = 0;
            double taxAmount = 0, consumptionTaxAmount = 0, enviromentTaxAmount = 0, vATTaxAmount = 0, totalTaxAmount = 0;
            totalCostExchange = DBConvert.ParseDouble(e.Cell.Row.Cells["TotalCostExchange"].Value);
            totalCostExchange = (totalCostExchange == Double.MinValue ? 0 : totalCostExchange);
            taxAmount = DBConvert.ParseDouble(e.Cell.Row.Cells["TaxAmount"].Value);
            taxAmount = (taxAmount == Double.MinValue ? 0 : taxAmount);
            consumptionTaxAmount = DBConvert.ParseDouble(e.Cell.Row.Cells["ConsumptionTaxAmount"].Value);
            consumptionTaxAmount = (consumptionTaxAmount == Double.MinValue ? 0 : consumptionTaxAmount);
            enviromentTaxAmount = DBConvert.ParseDouble(e.Cell.Row.Cells["EnviromentTaxAmount"].Value);
            enviromentTaxAmount = (enviromentTaxAmount == Double.MinValue ? 0 : enviromentTaxAmount);
            vATTaxAmount = DBConvert.ParseDouble(e.Cell.Row.Cells["VATTaxAmount"].Value);
            vATTaxAmount = (vATTaxAmount == Double.MinValue ? 0 : vATTaxAmount);
            if (taxAmount >= 0)
            {
              taxPercent = Math.Round((taxAmount / totalCostExchange) * 100, 2, MidpointRounding.AwayFromZero);
              e.Cell.Row.Cells["TaxPercent"].Value = taxPercent;
              totalTaxAmount += taxAmount;
            }
            else
            {
              e.Cell.Row.Cells["TaxPercent"].Value = DBNull.Value;
            }
            if (consumptionTaxAmount >= 0)
            {
              consumptionTaxPercent = Math.Round((consumptionTaxAmount / (totalCostExchange + taxAmount)) * 100, 2, MidpointRounding.AwayFromZero);
              e.Cell.Row.Cells["ConsumptionTaxPercent"].Value = consumptionTaxPercent;
              totalTaxAmount += consumptionTaxAmount;
            }
            else
            {
              e.Cell.Row.Cells["ConsumptionTaxPercent"].Value = DBNull.Value;
            }
            if (enviromentTaxAmount >= 0)
            {
              enviromentTaxPercent = Math.Round((enviromentTaxAmount / totalCostExchange) * 100, 2, MidpointRounding.AwayFromZero);
              e.Cell.Row.Cells["EnviromentTaxPercent"].Value = enviromentTaxPercent;
              totalTaxAmount += enviromentTaxAmount;
            }
            else
            {
              e.Cell.Row.Cells["EnviromentTaxPercent"].Value = DBNull.Value;
            }
            if (vATTaxAmount >= 0)
            {
              vATTaxPercent = Math.Round((vATTaxPercent / (totalCostExchange + taxAmount + consumptionTaxAmount + enviromentTaxAmount)) * 100, 2, MidpointRounding.AwayFromZero);
              e.Cell.Row.Cells["VATTaxPercent"].Value = vATTaxPercent;
              totalTaxAmount += vATTaxAmount;
            }
            else
            {
              e.Cell.Row.Cells["VATTaxPercent"].Value = DBNull.Value;
            }
            e.Cell.Row.Cells["TotalTaxAmount"].Value = totalTaxAmount;

            this.isLoadingData = false;
          }
          break;
        default:
          break;
      }
      this.CalculateDeclarationAmount();
    }

    private void ugdData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      string value = e.NewValue.ToString();
      if (value != null && value.Length > 0)
      {
        switch (colName)
        {
          case "TotalCostExchange":
          case "TaxPercent":
          case "TaxAmount":
          case "VATTaxPercent":
          case "VATTaxAmount":
          case "ConsumptionTaxPercent":
          case "ConsumptionTaxAmount":
          case "EnviromentTaxPercent":
          case "EnviromentTaxAmount":
            double amount = DBConvert.ParseDouble(value);
            if (amount < 0)
            {
              e.Cancel = true;
              DaiCo.Shared.Utility.WindowUtinity.ShowMessageErrorFromText(string.Format("Giá trị nhập vào cột {0} phải >= 0 !!!", e.Cell.Column.Header.Caption));
            }
            break;
          default:
            break;
        }
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
      Utility.ExportToExcelWithDefaultPath(ugdData, "Data");
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

    private void ConfirmDeclaration()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[2];
      inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      inputParam[1] = new SqlDBParameter("@ConfirmBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCCustomsDeclaration_Confirm", inputParam, outputParam);
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
      this.CalculateDeclarationAmount();
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

    private void btnInvoice_Click(object sender, EventArgs e)
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

      viewACC_16_003 view = new viewACC_16_003();
      view.currencyPid = currencyPid;
      view.objectPid = objectPid;
      view.objectType = objectType;
      view.actionCode = this.actionCode;

      Shared.Utility.WindowUtinity.ShowView(view, "Chọn Hóa Đơn", false, ViewState.ModalWindow);

      // Add selected documents into grid
      if (view.rowSelectedDoc != null && view.rowSelectedDoc.Length > 0)
      {
        this.invoicePid = DBConvert.ParseLong(view.rowSelectedDoc[0]["InvoicePid"]);        
        double exchangeRate = DBConvert.ParseDouble(view.rowSelectedDoc[0]["ExchangeRate"]);
        uneExchangeRate.Value = exchangeRate;

        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@ActionCode", DbType.Int32, this.actionCode);
        inputParam[1] = new DBParameter("@InvoicePid", DbType.Int64, this.invoicePid);
        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spACCCustomsDeclarationDetail_FromInvoice", inputParam);
        ugdData.DataSource = dtSource;
        foreach(DataRow row in dtSource.Rows)
        {
          row.SetAdded();
        }  
        lbCount.Text = string.Format(@"Đếm {0}", ugdData.Rows.FilteredInRowCount > 0 ? ugdData.Rows.FilteredInRowCount : 0);
      }
      this.ReadOnlyKeyMainData();
      this.CalculateDeclarationAmount();
    }
    private void uneExchangeRate_ValueChanged(object sender, EventArgs e)
    {
      double exchangeRate = DBConvert.ParseDouble(uneExchangeRate.Value);
      if (exchangeRate >= 0 && !this.isLoadingData)
      {
        this.isLoadingData = true;
        foreach (UltraGridRow row in ugdData.Rows)
        {          
          double totalCost = 0, taxPercent = 0, consumptionTaxPercent = 0, enviromentTaxPercent = 0, vATTaxPercent = 0;
          double totalCostExchange = 0, taxAmount = 0, consumptionTaxAmount = 0, enviromentTaxAmount = 0, vATTaxAmount = 0;
          totalCost = DBConvert.ParseDouble(row.Cells["TotalCost"].Value);
          totalCostExchange = Math.Round(totalCost * exchangeRate, 0, MidpointRounding.AwayFromZero);
          taxPercent = DBConvert.ParseDouble(row.Cells["TaxPercent"].Value);
          consumptionTaxPercent = DBConvert.ParseDouble(row.Cells["ConsumptionTaxPercent"].Value);
          enviromentTaxPercent = DBConvert.ParseDouble(row.Cells["EnviromentTaxPercent"].Value);
          vATTaxPercent = DBConvert.ParseDouble(row.Cells["VATTaxPercent"].Value);
          if (taxPercent >= 0)
          {
            taxAmount = Math.Round((taxPercent / 100) * totalCostExchange, 0, MidpointRounding.AwayFromZero);
            row.Cells["TaxAmount"].Value = taxAmount;
          }
          else
          {
            row.Cells["TaxAmount"].Value = DBNull.Value;
          }
          if (consumptionTaxPercent >= 0)
          {
            consumptionTaxAmount = Math.Round((consumptionTaxPercent / 100) * (totalCostExchange + taxAmount), 0, MidpointRounding.AwayFromZero);
            row.Cells["ConsumptionTaxAmount"].Value = consumptionTaxAmount;
          }
          else
          {
            row.Cells["ConsumptionTaxAmount"].Value = DBNull.Value;
          }
          if (enviromentTaxPercent >= 0)
          {
            enviromentTaxAmount = Math.Round((enviromentTaxPercent / 100) * totalCostExchange, 0, MidpointRounding.AwayFromZero);
            row.Cells["EnviromentTaxAmount"].Value = enviromentTaxAmount;
          }
          else
          {
            row.Cells["EnviromentTaxAmount"].Value = DBNull.Value;
          }
          if (vATTaxPercent >= 0)
          {
            vATTaxAmount = Math.Round((vATTaxPercent / 100) * (totalCostExchange + taxAmount + consumptionTaxAmount + enviromentTaxAmount), 0, MidpointRounding.AwayFromZero);
            row.Cells["VATTaxAmount"].Value = vATTaxAmount;
          }
          else
          {
            row.Cells["VATTaxAmount"].Value = DBNull.Value;
          }
        }
        this.isLoadingData = false;
      }
    }

    private void uneTaxPercent_Leave(object sender, EventArgs e)
    {
      if (uneTaxPercent.ReadOnly)
        return;

      double taxPercent = 0;
      if (uneTaxPercent.Value != null)
      {
        taxPercent = DBConvert.ParseDouble(uneTaxPercent.Value);

        if (WindowUtinity.ShowMessageConfirmFromText(string.Format("Bạn có muốn thay đổi thuế {0} cho những sản phẩm bên dưới không?", lblTax.Text)) == DialogResult.Yes)
        {          
          foreach (UltraGridRow row in ugdData.Rows)
          {
            row.Cells["TaxPercent"].Value = taxPercent;            
          }
        }
      }
    }

    private void uneConsumptionTaxPercent_Leave(object sender, EventArgs e)
    {
      if (uneConsumptionTaxPercent.ReadOnly)
        return;

      double consumptionTaxPercent = 0;
      if (uneConsumptionTaxPercent.Value != null)
      {
        consumptionTaxPercent = DBConvert.ParseDouble(uneConsumptionTaxPercent.Value);

        if (WindowUtinity.ShowMessageConfirmFromText(string.Format("Bạn có muốn thay đổi thuế {0} cho những sản phẩm bên dưới không?", lblConsumptionTax.Text)) == DialogResult.Yes)
        {
          foreach (UltraGridRow row in ugdData.Rows)
          {
            row.Cells["ConsumptionTaxPercent"].Value = consumptionTaxPercent;
          }          
        }
      }
    }

    private void uneVATTaxPercent_Leave(object sender, EventArgs e)
    {
      if (uneVATTaxPercent.ReadOnly)
        return;

      double vATTaxPercent = 0;
      if (uneVATTaxPercent.Value != null)
      {
        vATTaxPercent = DBConvert.ParseDouble(uneVATTaxPercent.Value);

        if (WindowUtinity.ShowMessageConfirmFromText(string.Format("Bạn có muốn thay đổi thuế {0} cho những sản phẩm bên dưới không?", lblVATTax.Text)) == DialogResult.Yes)
        {
          foreach (UltraGridRow row in ugdData.Rows)
          {
            row.Cells["VATTaxPercent"].Value = vATTaxPercent;
          }          
        }
      }
    }

    private void uneEnviromentTaxPercent_Leave(object sender, EventArgs e)
    {
      if (uneEnviromentTaxPercent.ReadOnly)
        return;

      double enviromentTaxPercent = 0;
      if (uneEnviromentTaxPercent.Value != null)
      {
        enviromentTaxPercent = DBConvert.ParseDouble(uneEnviromentTaxPercent.Value);

        if (WindowUtinity.ShowMessageConfirmFromText(string.Format("Bạn có muốn thay đổi thuế {0} cho những sản phẩm bên dưới không?", lblTax.Text)) == DialogResult.Yes)
        {
          foreach (UltraGridRow row in ugdData.Rows)
          {
            row.Cells["EnviromentTaxPercent"].Value = enviromentTaxPercent;
          }         
        }
      }
    }
    #endregion Event
  }
}