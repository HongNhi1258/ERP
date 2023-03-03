/*
  Author      : Nguyen Thanh Binh
  Date        : 24/05/2022
  Description : AR Invoice Out Detail
  Standard Form: view_SaveMasterDetail
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_13_006 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(view_SaveMasterDetail).Assembly);
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private int docTypePid = ConstantClass.Invoice_Seller;
    private int status = 0;
    public int actionCode = 1;
    public int creditPid = int.MinValue;        
    public string listPO = string.Empty;
    public string listRec = string.Empty;
    private int oddNumber = 2;
    private bool isLoadingData = false;
    #endregion Field

    #region Init
    public viewACC_13_006()
    {
      InitializeComponent();
    }

    private void viewACC_13_006_Load(object sender, EventArgs e)
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
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCARInvoiceOutDetail_Init");
      Utility.LoadUltraCombo(ucbObject, dsInit.Tables[0], "KeyValue", "Object", false, new string[] { "ObjectPid", "Object", "ObjectType", "KeyValue", "StreetAddress" });
      ucbObject.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[1], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate", "OddNumber" });
      Utility.LoadUltraCombo(ucbPaymentType, dsInit.Tables[2], "Code", "Value", false, "Code");
      Utility.LoadUltraCombo(ucbVATCode, dsInit.Tables[3], "VATCode", "VATName", false, new string[] { "VATCode", "VATFormNo", "VATSymbol" });
      Utility.LoadUltraCombo(ucbInputVATDocType, dsInit.Tables[4], "Code", "Value", false, "Code");
      Utility.LoadUltraCombo(ucbIDKho, dsInit.Tables[6], "Code", "Value", false, "Code");


      //Load drop down for grid
      Utility.LoadUltraCombo(ucbddItemCode, dsInit.Tables[5], "ItemCode", "ItemCode", true);
      ucbddItemCode.DisplayLayout.Bands[0].Columns["ItemCode"].Header.Caption = "Mã sản phẩm";
      ucbddItemCode.DisplayLayout.Bands[0].Columns["ItemName"].Header.Caption = "Tên sản phẩm";

      Utility.LoadUltraCombo(ucbddIDKho, dsInit.Tables[6], "Code", "Value", false, "Code");
      Utility.LoadUltraCombo(ucbddProductSerialPid, dsInit.Tables[7], "Code", "Value", false, "Code");


      //Init dropdown
      Utility.LoadUltraCBAccountList(ucbddAccountPid);
      Utility.LoadUltraCBAccountList(ucbddACOffeetPid);
      Utility.LoadUltraCBAccountList(ucbddACRevenuePid);
      // Set Language
      this.SetLanguage();
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

    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        chkConfirm.Enabled = false;
        btnSave.Enabled = false;

        txtInvoiceDesc.ReadOnly = true;
        udtInvoiceDate.ReadOnly = true;
        ucbObject.ReadOnly = true;
        ucbCurrency.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
        ucbPaymentType.ReadOnly = true;
        udtDueDate.ReadOnly = true;
        txtInvoiceAddress.ReadOnly = true;
        txtDeliveryAddress.ReadOnly = true;
        uneSubTotalAmount.ReadOnly = true;
        uneDiscountPercent.ReadOnly = true;
        uneDiscountAmount.ReadOnly = true;
        uneTaxPercent.ReadOnly = true;
        uneTaxAmount.ReadOnly = true;
        uneTotalAmount.ReadOnly = true;
        chkShipment.Enabled = false;
        ucbVATCode.ReadOnly = true;
        udtVATDate.ReadOnly = true;
        txtVATFormNo.ReadOnly = true;
        txtVATSymbol.ReadOnly = true;
        ucbInputVATDocType.ReadOnly = true;       
        txtVATInvoiceNo.ReadOnly = true;
        ucbIDKho.ReadOnly = true;
        btnSelectData.Visible = false;
      }
      else
      {
        btnClearDeposit.Visible = false;
      }
      if (this.actionCode == 1)
      {
        btnSelectData.Visible = false;
      }
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtInvoiceCode.Text = dtMain.Rows[0]["InvoiceCode"].ToString();
        txtInvoiceDesc.Text = dtMain.Rows[0]["InvoiceDesc"].ToString();
        if (DBConvert.ParseDateTime(dtMain.Rows[0]["InvoiceDate"]) != DateTime.MinValue)
        {
          udtInvoiceDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["InvoiceDate"]);
        }
        else
        {
          udtInvoiceDate.Value = null;
        }  
        ucbObject.Value = dtMain.Rows[0]["ObjectValue"];
        if (DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"].ToString()) != int.MinValue)
        {
          ucbCurrency.Value = DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"].ToString());
        }
        uneExchangeRate.Value = DBConvert.ParseDouble(dtMain.Rows[0]["ExchangeRate"].ToString());
        if (DBConvert.ParseInt(dtMain.Rows[0]["PaymentType"].ToString()) != int.MinValue)
        {
          ucbInputVATDocType.Value = DBConvert.ParseInt(dtMain.Rows[0]["PaymentType"].ToString());
        }
        if (DBConvert.ParseDateTime(dtMain.Rows[0]["DueDate"]) != DateTime.MinValue)
        {
          udtDueDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["DueDate"]);
        }
        else
        {
          udtDueDate.Value = null;
        }  
        txtInvoiceAddress.Text = dtMain.Rows[0]["InvoiceAddress"].ToString();
        txtDeliveryAddress.Text = dtMain.Rows[0]["DeliveryAddress"].ToString();
        double subTotalAmount = DBConvert.ParseDouble(dtMain.Rows[0]["SubTotalAmount"]);
        if (subTotalAmount >= 0)
        {
          uneSubTotalAmount.Value = subTotalAmount;
        }
        else
        {
          uneSubTotalAmount.Value = null;
        }  
        double discountPercent = DBConvert.ParseDouble(dtMain.Rows[0]["DiscountPercent"]);
        if (discountPercent >= 0)
        {
          uneDiscountPercent.Value = discountPercent;
        }
        else
        {
          uneDiscountPercent.Value = null;
        }  
        double discountAmount = DBConvert.ParseDouble(dtMain.Rows[0]["DiscountAmount"]);
        if (discountAmount >= 0)
        {
          uneDiscountAmount.Value = discountAmount;
        }
        else
        {
          uneDiscountAmount.Value = null;
        }  
        double taxPercent = DBConvert.ParseDouble(dtMain.Rows[0]["TaxPercent"]);
        if (taxPercent >= 0)
        {
          uneTaxPercent.Value = taxPercent;
        }
        else
        {
          uneTaxPercent.Value = null;
        }  
        double taxAmount = DBConvert.ParseDouble(dtMain.Rows[0]["TaxAmount"]);
        if (taxAmount >= 0)
        {
          uneTaxAmount.Value = taxAmount;
        }
        else
        {
          uneTaxAmount.Value = null;
        }  
        double totalAmount = DBConvert.ParseDouble(dtMain.Rows[0]["TotalAmount"]);
        if (totalAmount >= 0)
        {
          uneTotalAmount.Value = totalAmount;
        }
        else
        {
          uneTotalAmount.Value = null;
        }  

        chkShipment.Checked = (bool)dtMain.Rows[0]["IsShipment"];
        txtStatus.Text = dtMain.Rows[0]["StatusText"].ToString();
        
        this.status = DBConvert.ParseInt(dtMain.Rows[0]["Status"].ToString());
        chkConfirm.Checked = this.status > 0 ? true : false;
        this.actionCode = DBConvert.ParseInt(dtMain.Rows[0]["ActionCode"].ToString());
        if (DBConvert.ParseInt(dtMain.Rows[0]["VATCode"].ToString()) != int.MinValue)
        {
          ucbVATCode.Value = DBConvert.ParseInt(dtMain.Rows[0]["VATCode"].ToString());
        }
        if (DBConvert.ParseDateTime(dtMain.Rows[0]["VATDate"]) != DateTime.MinValue)
        {
          udtVATDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["VATDate"]);
        }
        else
        {
          udtVATDate.Value = null;
        }  
        txtVATFormNo.Text = dtMain.Rows[0]["VATFormNo"].ToString();
        txtVATSymbol.Text = dtMain.Rows[0]["VATSymbol"].ToString();
        if (DBConvert.ParseInt(dtMain.Rows[0]["VATDocumentType"].ToString()) != int.MinValue)
        {
          ucbInputVATDocType.Value = DBConvert.ParseInt(dtMain.Rows[0]["VATDocumentType"].ToString());
        }
        txtVATInvoiceNo.Text = dtMain.Rows[0]["VATInvoiceNo"].ToString();
        int idKho = DBConvert.ParseInt(dtMain.Rows[0]["IDKho"]);
        if(idKho > 0)
        {
          ucbIDKho.Value = idKho;
        }  
        else
        {
          ucbIDKho.Value = null;
        }  
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
          udtDueDate.Value = null;
          udtVATDate.Value = null;
        }
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

    }

    private void LoadData()
    {
      this.isLoadingData = true;
      this.listDeletedPid = new ArrayList(); 
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCARInvoiceOutDetail_Load", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        this.LoadMainData(dsSource.Tables[0]);
        ugdData.DataSource = dsSource.Tables[1];
        this.LoadTransationData();
      }            
      //this.LoadTabData();   
      this.SetStatusControl();
      this.ReadOnlyKeyMainData();
      this.NeedToSave = false;
      this.isLoadingData = false;
    }

    private void SetValueAmount()
    {
      if (!this.isLoadingData)
      {
        this.isLoadingData = true;
        double subTotalAmount = 0, discountPercent = 0, discountAmount = 0, taxPercent = 0, taxAmount = 0, totalAmount = 0;
        DataTable dtDetail = (DataTable)ugdData.DataSource;
        double amount = DBConvert.ParseDouble(dtDetail.Compute("Sum(TotalPrice)", "Qty > 0").ToString());
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
        this.isLoadingData = false;
      }
    }

    private bool CheckValid()
    {
      if (ucbCurrency.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Bạn chưa chọn loại tiền tệ.");
        ucbCurrency.Focus();
        return false; 
      }

      if (ucbObject.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Đối tượng không được để trống.");
        ucbObject.Focus();
        return false;
      }

      if (udtInvoiceDate.Value.ToString().Length == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày chứng từ không được để trống.");
        udtInvoiceDate.Focus();
        return false;
      }
      if (DBConvert.ParseDouble(uneExchangeRate.Value) <= 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Tỉ giá không được để trống và phải lớn hơn 0.");
        uneExchangeRate.Focus();
        return false;
      }
      
      DataTable dtDetail = new DataTable();
      dtDetail.Columns.Add("Pid", typeof(long));
      dtDetail.Columns.Add("SaleOrderPid", typeof(long));
      dtDetail.Columns.Add("ItemCode", typeof(string));
      dtDetail.Columns.Add("Revision", typeof(int));
      dtDetail.Columns.Add("Qty", typeof(double));   

      //check detail      
      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        UltraGridRow row = ugdData.Rows[i];
        row.Selected = false;
        if (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) <= 0)
        {
          row.Cells["Qty"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Số lượng không được để trống và phải lớn hơn 0.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }

        if (DBConvert.ParseDouble(row.Cells["UnitPrice"].Value.ToString()) < 0)
        {
          row.Cells["UnitPrice"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Đơn giá không được để trống và phải lớn hơn hoặc bằng 0.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }

        DataRow rowDetail = dtDetail.NewRow();
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value);
        if (pid > 0)
        {
          rowDetail["Pid"] = pid;
        }
        rowDetail["SaleOrderPid"] = row.Cells["SaleOrderPid"].Value;
        rowDetail["ItemCode"] = row.Cells["ItemCode"].Value;
        rowDetail["Revision"] = row.Cells["Revision"].Value;
        rowDetail["Qty"] = row.Cells["Qty"].Value;
        dtDetail.Rows.Add(rowDetail);
      }

      // Check total invoice qty <= qty of Sale Order
      int paramNumber = 2;
      string storeName = "spACCARInvoiceOut_CheckValid";

      SqlDBParameter[] inputParam = new SqlDBParameter[paramNumber];
      inputParam[0] = new SqlDBParameter("@SOItemList", SqlDbType.NVarChar, DBConvert.ParseXMLString(dtDetail));
      inputParam[1] = new SqlDBParameter("@InvoiceOutPid", SqlDbType.BigInt, this.viewPid);
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
      SqlDBParameter[] inputParam = new SqlDBParameter[28];
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
      if (DBConvert.ParseInt(ucbPaymentType.Value) != int.MinValue)
      {
        inputParam[7] = new SqlDBParameter("@PaymentType", SqlDbType.Int, DBConvert.ParseInt(ucbPaymentType.Value));
      }
      if (DBConvert.ParseDateTime(udtDueDate.Value) != DateTime.MinValue)
      {
        inputParam[8] = new SqlDBParameter("@DueDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtDueDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }
      if (txtInvoiceAddress.Text.Trim().Length > 0)
      {
        inputParam[9] = new SqlDBParameter("@InvoiceAddress", SqlDbType.NVarChar, txtInvoiceAddress.Text.Trim().ToString());
      }
      if (txtDeliveryAddress.Text.Trim().Length > 0)
      {
        inputParam[10] = new SqlDBParameter("@DeliveryAddress", SqlDbType.NVarChar, txtDeliveryAddress.Text.Trim().ToString());
      }
      if (DBConvert.ParseDouble(uneSubTotalAmount.Value) >= 0)
      {
        inputParam[11] = new SqlDBParameter("@SubTotalAmount", SqlDbType.Float, DBConvert.ParseDouble(uneSubTotalAmount.Value));
      }
      if (DBConvert.ParseDouble(uneDiscountPercent.Value) >= 0)
      {
        inputParam[12] = new SqlDBParameter("@DiscountPercent", SqlDbType.Float, DBConvert.ParseDouble(uneDiscountPercent.Value));
      }
      if (DBConvert.ParseDouble(uneDiscountAmount.Value) >= 0)
      {
        inputParam[13] = new SqlDBParameter("@DiscountAmount", SqlDbType.Float, DBConvert.ParseDouble(uneDiscountAmount.Value));
      }
      if (DBConvert.ParseDouble(uneTaxPercent.Value) >= 0)
      {
        inputParam[14] = new SqlDBParameter("@TaxPercent", SqlDbType.Float, DBConvert.ParseDouble(uneTaxPercent.Value));
      }
      if (DBConvert.ParseDouble(uneTaxAmount.Value) >= 0)
      {
        inputParam[15] = new SqlDBParameter("@TaxAmount", SqlDbType.Float, DBConvert.ParseDouble(uneTaxAmount.Value));
      }
      if (DBConvert.ParseDouble(uneTotalAmount.Value) >= 0)
      {
        inputParam[16] = new SqlDBParameter("@TotalAmount", SqlDbType.Float, DBConvert.ParseDouble(uneTotalAmount.Value));
      }
      if (DBConvert.ParseInt(ucbIDKho.Value) != int.MinValue)
      {
        inputParam[17] = new SqlDBParameter("@IDKho", SqlDbType.Int, DBConvert.ParseInt(ucbIDKho.Value));
      }
      inputParam[18] = new SqlDBParameter("@IsShipment", SqlDbType.Bit, chkShipment.Checked ? 1 : 0);
      inputParam[19] = new SqlDBParameter("@ActionCode", SqlDbType.Int, this.actionCode);
      inputParam[20] = new SqlDBParameter("@Status", SqlDbType.Int, chkConfirm.Checked ? 1 : 0);
      if (DBConvert.ParseInt(ucbVATCode.Value) != int.MinValue)
      {
        inputParam[21] = new SqlDBParameter("@VATCode", SqlDbType.Int, DBConvert.ParseInt(ucbVATCode.Value));
      }
      if (txtVATInvoiceNo.Text.Trim().Length > 0)
      {
        inputParam[22] = new SqlDBParameter("@VATInvoiceNo", SqlDbType.VarChar, txtVATInvoiceNo.Text.Trim().ToString());
      }
      if (DBConvert.ParseDateTime(udtVATDate.Value) != DateTime.MinValue)
      {
        inputParam[23] = new SqlDBParameter("@VATDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtVATDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }      
      if (txtVATFormNo.Text.Trim().Length > 0)
      {
        inputParam[24] = new SqlDBParameter("@VATFormNo", SqlDbType.VarChar, txtVATFormNo.Text.Trim().ToString());
      }
      if (txtVATSymbol.Text.Trim().Length > 0)
      {
        inputParam[25] = new SqlDBParameter("@VATSymbol", SqlDbType.VarChar, txtVATSymbol.Text.Trim().ToString());
      }
      if (DBConvert.ParseInt(ucbInputVATDocType.Value) != int.MinValue)
      {
        inputParam[26] = new SqlDBParameter("@VATDocumentType", SqlDbType.Int, DBConvert.ParseInt(ucbInputVATDocType.Value));
      }
      inputParam[27] = new SqlDBParameter("@CreateBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCARInvoiceOutDetail_SaveMaster", inputParam, outputParam);
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
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCARInvoiceOutDetail_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ugdData.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          SqlDBParameter[] inputParam = new SqlDBParameter[19];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (pid != long.MinValue)
          {
            inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
          }
          inputParam[1] = new SqlDBParameter("@InvoiceOutPid", SqlDbType.BigInt, this.viewPid);
          if (row["DetailDesc"].ToString().Trim().Length > 0)
          {
            inputParam[2] = new SqlDBParameter("@DetailDesc", SqlDbType.NVarChar, row["DetailDesc"].ToString());
          }
          if (row["ItemCode"].ToString().Trim().Length > 0)
          {
            inputParam[3] = new SqlDBParameter("@ItemCode", SqlDbType.VarChar, row["ItemCode"].ToString());
          }
          if (DBConvert.ParseInt(row["AccountPid"].ToString()) >= 0)
          {
            inputParam[4] = new SqlDBParameter("@AccountPid", SqlDbType.Int, DBConvert.ParseInt(row["AccountPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["ACSalePid"].ToString()) >= 0)
          {
            inputParam[5] = new SqlDBParameter("@ACSalePid", SqlDbType.Int, DBConvert.ParseInt(row["ACSalePid"].ToString()));
          }
          if (DBConvert.ParseLong(row["ACRevenuePid"].ToString()) >= 0)
          {
            inputParam[6] = new SqlDBParameter("@ACRevenuePid", SqlDbType.BigInt, DBConvert.ParseLong(row["ACRevenuePid"].ToString()));
          }
          if (DBConvert.ParseLong(row["SaleOrderPid"].ToString()) >= 0)
          {
            inputParam[7] = new SqlDBParameter("@SaleOrderPid", SqlDbType.BigInt, DBConvert.ParseLong(row["SaleOrderPid"].ToString()));
          }
          if (DBConvert.ParseLong(row["LoadingPid"].ToString()) >= 0)
          {
            inputParam[8] = new SqlDBParameter("@LoadingPid", SqlDbType.BigInt, DBConvert.ParseLong(row["LoadingPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["IDKho"].ToString()) >= 0)
          {
            inputParam[9] = new SqlDBParameter("@IDKho", SqlDbType.Int, DBConvert.ParseInt(row["IDKho"].ToString()));
          }
          if (DBConvert.ParseInt(row["ProductSerialPid"].ToString()) >= 0)
          {
            inputParam[10] = new SqlDBParameter("@ProductSerialPid", SqlDbType.Int, DBConvert.ParseInt(row["ProductSerialPid"].ToString()));
          }
          if (DBConvert.ParseDouble(row["Qty"].ToString()) >= 0)
          {
            inputParam[11] = new SqlDBParameter("@Qty", SqlDbType.Float, DBConvert.ParseDouble(row["Qty"].ToString()));
          }
          if (DBConvert.ParseDouble(row["UnitPrice"].ToString()) >= 0)
          {
            inputParam[12] = new SqlDBParameter("@UnitPrice", SqlDbType.Float, DBConvert.ParseDouble(row["UnitPrice"].ToString()));
          }
          if (DBConvert.ParseDouble(row["TotalPrice"].ToString()) >= 0)
          {
            inputParam[13] = new SqlDBParameter("@TotalPrice", SqlDbType.Float, DBConvert.ParseDouble(row["TotalPrice"].ToString()));
          }
          if (DBConvert.ParseDouble(row["CountOfQty"].ToString()) >= 0)
          {
            inputParam[14] = new SqlDBParameter("@CountOfQty", SqlDbType.Float, DBConvert.ParseDouble(row["CountOfQty"].ToString()));
          }
          if (DBConvert.ParseDouble(row["CountOfQtyUnitCost"].ToString()) >= 0)
          {
            inputParam[15] = new SqlDBParameter("@CountOfQtyUnitCost", SqlDbType.Float, DBConvert.ParseDouble(row["CountOfQtyUnitCost"].ToString()));
          }
          if (DBConvert.ParseDouble(row["CountOfQtyTotalCost"].ToString()) >= 0)
          {
            inputParam[16] = new SqlDBParameter("@CountOfQtyTotalCost", SqlDbType.Float, DBConvert.ParseDouble(row["CountOfQtyTotalCost"].ToString()));
          }
          inputParam[17] = new SqlDBParameter("@CreateBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          inputParam[18] = new SqlDBParameter("@Revision", SqlDbType.Int, row["Revision"]);

          SqlDataBaseAccess.ExecuteStoreProcedure("spACCARInvoiceOutDetail_SaveDetail", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
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
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCARInvoiceOutDetail_Confirm", inputParam, outputParam);
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
              if (success)
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
      DataTable dtSource = (DataTable)ugdData.DataSource;      
      uneSubTotalAmount.Value = dtSource.Compute("Sum(TotalPrice)", "TotalPrice > 0"); ;
    }
    #endregion Function

    #region Event
    private void ugdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdData);      
      e.Layout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
      e.Layout.UseFixedHeaders = true;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      if (this.actionCode == 1)
      {
        e.Layout.Override.AllowAddNew = AllowAddNew.Yes;
        e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      }
      if (this.status > 0)
      {
        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Override.AllowAddNew = AllowAddNew.No;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      }
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["AccountPid"].ValueList = ucbddAccountPid;
      e.Layout.Bands[0].Columns["ACSalePid"].ValueList = ucbddACOffeetPid;
      e.Layout.Bands[0].Columns["ACRevenuePid"].ValueList = ucbddACRevenuePid;
      e.Layout.Bands[0].Columns["ItemCode"].ValueList = ucbddItemCode;
      e.Layout.Bands[0].Columns["ProductSerialPid"].ValueList = ucbddProductSerialPid;
      e.Layout.Bands[0].Columns["IDKho"].ValueList = ucbddIDKho;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["AccountPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["AccountPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["ACSalePid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ACSalePid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["ACRevenuePid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ACRevenuePid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["ItemCode"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ItemCode"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["ProductSerialPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ProductSerialPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["IDKho"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["IDKho"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["AccountPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["ACSalePid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["ACRevenuePid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["ProductSerialPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["IDKho"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["InvoiceOutPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ProductSerialPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleOrderPid"].Hidden = true;
      e.Layout.Bands[0].Columns["LoadingPid"].Hidden = true;
      e.Layout.Bands[0].Columns["AccountPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ACSalePid"].Hidden = true;
      e.Layout.Bands[0].Columns["ACRevenuePid"].Hidden = true;

      // Read Only
      e.Layout.Bands[0].Columns["TotalPrice"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CountOfQtyTotalCost"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SaleOrder"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["LoadingNo"].CellActivation = Activation.ActivateOnly;

      if (this.actionCode == 1)
      {
        e.Layout.Bands[0].Columns["SaleOrder"].Hidden = true;
        e.Layout.Bands[0].Columns["LoadingNo"].Hidden = true;
      }
      else if (this.actionCode == 2)
      {
        e.Layout.Bands[0].Columns["LoadingNo"].Hidden = true;        
      } 
  
      // Set caption column
      e.Layout.Bands[0].Columns["DetailDesc"].Header.Caption = "Ghi chú";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Mã SP";
      e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Tên sản phẩm";
      e.Layout.Bands[0].Columns["Revision"].Header.Caption = "Phiên bản";
      e.Layout.Bands[0].Columns["LoadingNo"].Header.Caption = "Số Container";
      e.Layout.Bands[0].Columns["SaleOrder"].Header.Caption = "Mã đơn hàng";
      e.Layout.Bands[0].Columns["IDKho"].Header.Caption = "Kho";
      e.Layout.Bands[0].Columns["ProductSerialPid"].Header.Caption = "Mã kiện";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Số lượng";
      e.Layout.Bands[0].Columns["Unit"].Header.Caption = "Đơn vị";
      e.Layout.Bands[0].Columns["UnitPrice"].Header.Caption = "Đơn giá";
      e.Layout.Bands[0].Columns["TotalPrice"].Header.Caption = "Thành tiền";
     
      e.Layout.Bands[0].Columns["CountOfQty"].Header.Caption = "Số thanh\n(tấm)";
      e.Layout.Bands[0].Columns["CountOfQtyUnitCost"].Header.Caption = "Giá (tấm)";
      e.Layout.Bands[0].Columns["CountOfQtyTotalCost"].Header.Caption = "Thành tiền\n(tấm)";
            
      // Set Width
      e.Layout.Bands[0].Columns["ItemCode"].Width = 80;
      e.Layout.Bands[0].Columns["ItemName"].Width = 200;        
      e.Layout.Bands[0].Columns["Revision"].Width = 70;
      e.Layout.Bands[0].Columns["Qty"].Width = 80;
      e.Layout.Bands[0].Columns["Unit"].Width = 70;
      e.Layout.Bands[0].Columns["UnitPrice"].Width = 100;
      e.Layout.Bands[0].Columns["TotalPrice"].Width = 100;
      e.Layout.Bands[0].Columns["LoadingNo"].Width = 100;
      e.Layout.Bands[0].Columns["SaleOrder"].Width = 100;
      e.Layout.Bands[0].Columns["IDKho"].Width = 100;

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalPrice"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";
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
        case "ItemCode":
          e.Cell.Row.Cells["ItemName"].Value = ucbddItemCode.SelectedRow.Cells["ItemName"].Value.ToString();
          break;
        case "Qty":
        case "UnitPrice":
          double qty = DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value);
          double unitPrice = DBConvert.ParseDouble(e.Cell.Row.Cells["UnitPrice"].Value);
          if(qty >= 0 && unitPrice >= 0)
          {
            e.Cell.Row.Cells["TotalPrice"].Value = Math.Round(qty * unitPrice, this.oddNumber);
          }
          else
          {
            e.Cell.Row.Cells["TotalPrice"].Value = DBNull.Value;
          }
          break;
        case "CountOfQty":
        case "CountOfQtyUnitCost":
          double countOfQty = DBConvert.ParseDouble(e.Cell.Row.Cells["CountOfQty"].Value);
          double countOfQtyUnitCost = DBConvert.ParseDouble(e.Cell.Row.Cells["CountOfQtyUnitCost"].Value);
          if (countOfQty >= 0 && countOfQtyUnitCost >= 0)
          {
            e.Cell.Row.Cells["CountOfQtyTotalCost"].Value = Math.Round(countOfQty * countOfQtyUnitCost, this.oddNumber);
          }
          else
          {
            e.Cell.Row.Cells["CountOfQtyTotalCost"].Value = DBNull.Value;
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
        case "UnitPrice":
          {
            if (text.Trim().Length > 0 && (DBConvert.ParseDouble(value) == double.MinValue || DBConvert.ParseDouble(value) < 0))
            {
              WindowUtinity.ShowMessageErrorFromText("Đơn giá không hợp lệ.");
              e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
              e.Cancel = true;
            }
          }
          break;
        case "Qty":
          {
            if (text.Trim().Length > 0 && (DBConvert.ParseDouble(value) == double.MinValue || DBConvert.ParseDouble(value) < 0))
            {
              WindowUtinity.ShowMessageErrorFromText("Số lượng không hợp lệ.");
              e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
              e.Cancel = true;
            }
          }
          break;
        case "CountOfQty":
          {
            if (text.Trim().Length > 0 && (DBConvert.ParseDouble(value) == double.MinValue || DBConvert.ParseDouble(value) < 0))
            {
              WindowUtinity.ShowMessageErrorFromText("Số tấm không hợp lệ.");
              e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
              e.Cancel = true;
            }
          }
          break;
        case "CountOfQtyUnitCost":
          {
            if (text.Trim().Length > 0 && (DBConvert.ParseDouble(value) == double.MinValue || DBConvert.ParseDouble(value) < 0))
            {
              WindowUtinity.ShowMessageErrorFromText("Giá (tấm) không hợp lệ.");
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
      if (ucbObject.SelectedRow != null)
      {
        txtDeliveryAddress.Text = ucbObject.SelectedRow.Cells["StreetAddress"].Value.ToString();
        txtInvoiceAddress.Text = ucbObject.SelectedRow.Cells["StreetAddress"].Value.ToString();
      }
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

    private void btnClearDeposit_Click(object sender, EventArgs e)
    {
      viewACC_13_005 view = new viewACC_13_005();
      view.invoiceInPid = this.viewPid;
      Shared.Utility.WindowUtinity.ShowView(view, "Cấn trừ phiếu chi", false, ViewState.Window);
    }

    private void btnSelectData_Click(object sender, EventArgs e)
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
      if (this.actionCode == 2)
      {
        // Transfer data to child form
        int currencyPid = DBConvert.ParseInt(ucbCurrency.Value);
        int objectPid = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["ObjectPid"].Value);
        int objectType = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["ObjectType"].Value);
        DataTable dtDetail = new DataTable();
        dtDetail.Columns.Add("SaleOrderPid", typeof(long));
        dtDetail.Columns.Add("ItemCode", typeof(string));
        DataTable dtSource = (DataTable)ugdData.DataSource;
        DataRow[] rows = dtSource.Select("Pid is null");
        foreach (DataRow row in dtSource.Rows)
        {
          if (row.RowState != DataRowState.Deleted)
          {
            DataRow rowDetail = dtDetail.NewRow();
            rowDetail["SaleOrderPid"] = row["SaleOrderPid"];
            rowDetail["ItemCode"] = row["ItemCode"];
            dtDetail.Rows.Add(rowDetail);
          }
        }
        viewACC_13_008 view = new viewACC_13_008();
        view.documentPid = this.viewPid;
        view.currencyPid = currencyPid;
        view.objectPid = objectPid;
        view.objectType = objectType;
        view.dtAddedDocList = dtDetail;
        view.actionCode = this.actionCode;
        Shared.Utility.WindowUtinity.ShowView(view, "Chọn đơn hàng", false, ViewState.ModalWindow);
        // Add selected documents into grid
        if (view.rowSelectedDoc != null && view.rowSelectedDoc.Length > 0)
        {
          for (int i = 0; i < view.rowSelectedDoc.Length; i++)
          {
            DataRow rowDoc = view.rowSelectedDoc[i];
            DataRow row = dtSource.NewRow();
            row["SaleOrderPid"] = rowDoc["SaleOrderPid"];
            row["SaleOrder"] = rowDoc["SaleOrder"];
            row["ItemCode"] = rowDoc["ItemCode"];
            row["Revision"] = rowDoc["Revision"];
            row["Unit"] = rowDoc["Unit"];
            row["ItemName"] = rowDoc["ItemName"];
            row["Qty"] = rowDoc["Qty"];
            row["UnitPrice"] = rowDoc["UnitPrice"];
            row["TotalPrice"] = rowDoc["TotalPrice"];
            dtSource.Rows.Add(row);
          }
        }
      }
      else if (this.actionCode == 3)
      {
        // Transfer data to child form
        int currencyPid = DBConvert.ParseInt(ucbCurrency.Value);
        int objectPid = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["ObjectPid"].Value);
        int objectType = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["ObjectType"].Value);
        DataTable dtDetail = new DataTable();
        dtDetail.Columns.Add("LoadingPid", typeof(long));
        dtDetail.Columns.Add("SaleOrderPid", typeof(long));
        dtDetail.Columns.Add("ItemCode", typeof(string));
        DataTable dtSource = (DataTable)ugdData.DataSource;
        DataRow[] rows = dtSource.Select("Pid is null");
        foreach (DataRow row in dtSource.Rows)
        {
          if (row.RowState != DataRowState.Deleted)
          {
            DataRow rowDetail = dtDetail.NewRow();
            rowDetail["LoadingPid"] = row["LoadingPid"];
            rowDetail["SaleOrderPid"] = row["SaleOrderPid"];
            rowDetail["ItemCode"] = row["ItemCode"];
            dtDetail.Rows.Add(rowDetail);
          }
        }
        viewACC_13_009 view = new viewACC_13_009();
        view.documentPid = this.viewPid;
        view.currencyPid = currencyPid;
        view.objectPid = objectPid;
        view.objectType = objectType;
        view.dtAddedDocList = dtDetail;
        view.actionCode = this.actionCode;
        Shared.Utility.WindowUtinity.ShowView(view, "Chọn Loading No", false, ViewState.ModalWindow);
        // Add selected documents into grid
        if (view.rowSelectedDoc != null && view.rowSelectedDoc.Length > 0)
        {
          for (int i = 0; i < view.rowSelectedDoc.Length; i++)
          {
            DataRow rowDoc = view.rowSelectedDoc[i];
            DataRow row = dtSource.NewRow();
            row["LoadingPid"] = rowDoc["LoadingPid"];
            row["LoadingNo"] = rowDoc["LoadingNo"];
            row["SaleOrderPid"] = rowDoc["SaleOrderPid"];
            row["SaleOrder"] = rowDoc["SaleOrder"];
            row["ItemCode"] = rowDoc["ItemCode"];
            row["Revision"] = rowDoc["Revision"];
            row["Unit"] = rowDoc["Unit"];
            row["ItemName"] = rowDoc["ItemName"];
            row["Qty"] = rowDoc["Qty"];
            row["UnitPrice"] = rowDoc["UnitPrice"];
            row["TotalPrice"] = rowDoc["TotalPrice"];
            dtSource.Rows.Add(row);
          }
        }
      }
      this.ReadOnlyKeyMainData();
      this.SetValueAmount();
    }
   
    private void ucbVATCode_ValueChanged(object sender, EventArgs e)
    {
      if(ucbVATCode.SelectedRow != null)
      {
        txtVATFormNo.Text = ucbVATCode.SelectedRow.Cells["VATFormNo"].Value.ToString();
        txtVATSymbol.Text = ucbVATCode.SelectedRow.Cells["VATSymbol"].Value.ToString();
      }  
    }
   
   

    private void ugdData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      if (ucbCurrency.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Bạn chưa chọn loại tiền tệ.");
        ucbCurrency.Focus();
        e.Cancel = true;
      }

      if (ucbObject.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Đối tượng không được để trống.");
        ucbObject.Focus();
        e.Cancel = true;
      }
    }

    private void utcDetail_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
    {
      this.LoadTabData();
    }
    

    private void ucbIDKho_ValueChanged(object sender, EventArgs e)
    {
           
    }

    private void ugdData_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.ReadOnlyKeyMainData();
    }

    private void ucbIDKho_Leave(object sender, EventArgs e)
    {
      if (ucbIDKho.SelectedRow != null && ugdData.Rows.Count > 0)
      {
        if (WindowUtinity.ShowMessageConfirmFromText("Bạn có muốn thay đổi kho cho những sản phẩm bên dưới không?") == DialogResult.Yes)
        {
          for (int i = 0; i < ugdData.Rows.Count; i++)
          {
            ugdData.Rows[i].Cells["IDKho"].Value = ucbIDKho.SelectedRow.Cells["Code"].Value;
          }
        }
      }
    }

    private void Amount_ValueChanged(object sender, EventArgs e)
    {
      this.SetValueAmount();
    }
    #endregion Event
  }
}
