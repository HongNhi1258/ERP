/*
  Author      : Dang Xuan Truong
  Date        : 28/05/2012
  Description : Receiving Note For Material(03REC)
*/
using CrystalDecisions.CrystalReports.Engine;
using DaiCo.Application;
using DaiCo.Application.Web.Mail;
using DaiCo.ERPProject.Warehouse.Material.DataSetSource;
using DaiCo.ERPProject.Warehouse.Material.Reports;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_05_001 : MainUserControl
  {
    #region Field
    private int whPid = 0;
    private int docTypePid = 48;

    public string listInvoiceInDetail = string.Empty;
    //REC
    public string receivingNo = string.Empty;
    public long receivingPid = long.MinValue;
    public int actionCode = int.MinValue;
    // Status
    int status = int.MinValue;
    // Delete
    private IList listDeletingDetailPid = new ArrayList();
    private IList listDeletedDetailPid = new ArrayList();
    private IList listMaterialDeletingPid = new ArrayList();
    private IList listMaterialDeletedPid = new ArrayList();
    // Store            
    string SP_WHDStatusPRPO_Update = "spWHDStatusPRPO_Update";
    private int oddNumber = 2;
    #endregion Field

    #region Init
    public viewWHD_05_001()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load ViewWHD_05_001
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_05_001_Load(object sender, EventArgs e)
    {
      Utility.Format_UltraNumericEditor(tlpForm);
      // Load Warehouse List
      Utility.LoadUltraCBMaterialWHListByUser(ucbWarehouse);

      // Load Data
      this.InitData();
      this.LoadData();      
    }

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spWHDMaterialInStore_Init");
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[0], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate", "OddNumber" });
      Utility.LoadUltraCombo(ucbEnvironment, dsInit.Tables[1], "EnvironmentPid", "EnvironmentName", false, "EnvironmentPid");
      Utility.LoadUltraCombo(ucbCountry, dsInit.Tables[2], "Pid", "NationVN", false, "Pid");
      Utility.LoadUltraCombo(ultCBSupplier, dsInit.Tables[3], "ObjectCode", "DisplayText", false, "DisplayText");
      Utility.LoadUltraCombo(ucbModel, dsInit.Tables[4], "ModelPid", "ModelName", false, "ModelPid");
      Utility.LoadUltraCombo(ucbModelDetail, dsInit.Tables[5], "ModelDetailPid", "ModelDetailName", false, new string[] { "ModelPid", "ModelDetailPid" });
      Utility.LoadUltraCombo(ucbQuality, dsInit.Tables[6], "QualityPid", "QualityName", false, "QualityPid");

      // Set Language
      this.SetLanguage();
    }

    private void SetLanguage()
    {
      //btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      //btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      //btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);      

      this.SetBlankForTextOfButton(this);
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
    #endregion Init

    #region Function

    /// <summary>
    /// Load UltraCombo List PO
    /// </summary>
    //private void LoadComboListPO()
    //{
    //  ultCBPONo.DataSource = null;
    //  ultCBPONo.Text = string.Empty;
    //  if (this.whPid > 0)
    //  {
    //    string supplierCode = string.Empty;
    //    if (ultCBSupplier.Value != null)
    //    {
    //      supplierCode = ultCBSupplier.Value.ToString();
    //    }
    //    DBParameter[] input = new DBParameter[2];
    //    if (supplierCode.Length > 0)
    //    {
    //      input[0] = new DBParameter("@SupplierCode", DbType.String, supplierCode);
    //    }
    //    input[1] = new DBParameter("@WHPid", DbType.Int32, this.whPid);
    //    DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHDGetListPONo_Select", input);
    //    if (dtSource != null && dtSource.Rows.Count > 0)
    //    {
    //      ultCBPONo.DataSource = dtSource;
    //      ultCBPONo.DisplayMember = "Name";
    //      ultCBPONo.ValueMember = "PONo";
    //      ultCBPONo.DisplayLayout.Bands[0].ColHeadersVisible = false;
    //      ultCBPONo.DisplayLayout.Bands[0].Columns["Name"].Width = 550;
    //      ultCBPONo.DisplayLayout.Bands[0].Columns["PONo"].Hidden = true;
    //      ultCBPONo.DisplayLayout.Bands[0].Columns["SupplierCode"].Hidden = true;
    //    }
    //  }
    //}

    private void LoadMainData(DataTable dtReceivingInfo)
    {
      if (dtReceivingInfo.Rows.Count > 0)
      {
        DataRow row = dtReceivingInfo.Rows[0];
        txtReceivingNote.Text = dtReceivingInfo.Rows[0]["ReceivingNo"].ToString();
        if (DBConvert.ParseLong(dtReceivingInfo.Rows[0]["Pid"]) > 0)
        {
          this.receivingPid = DBConvert.ParseLong(dtReceivingInfo.Rows[0]["Pid"]);
        }
        txtTitle.Text = dtReceivingInfo.Rows[0]["Title"].ToString();        
        txtDate.Text = dtReceivingInfo.Rows[0]["CreateDate"].ToString();
        txtLotNo.Text = dtReceivingInfo.Rows[0]["LotNo"].ToString();
        ucbCurrency.Value = dtReceivingInfo.Rows[0]["CurrencyPid"];
        uneExchangeRate.Value = DBConvert.ParseDouble(dtReceivingInfo.Rows[0]["ExchangeRate"]);
        int environmentPid = DBConvert.ParseInt(dtReceivingInfo.Rows[0]["EnvironmentPid"]);
        if(environmentPid > 0)
        {
          ucbEnvironment.Value = environmentPid;
        }  
        else
        {
          ucbEnvironment.Value = null;
        }
        int countryPid = DBConvert.ParseInt(dtReceivingInfo.Rows[0]["CountryPid"]);
        if (countryPid > 0)
        {
          ucbCountry.Value = countryPid;
        }
        else
        {
          ucbCountry.Value = null;
        }
        
        ultCBSupplier.Value = dtReceivingInfo.Rows[0]["Supplier"];
        if (DBConvert.ParseInt(dtReceivingInfo.Rows[0]["WHPid"]) > 0)
        {
          this.whPid = DBConvert.ParseInt(dtReceivingInfo.Rows[0]["WHPid"]);
          ucbWarehouse.Value = this.whPid;
        }

        int locationPid = DBConvert.ParseInt(dtReceivingInfo.Rows[0]["LocationPid"]);
        if (locationPid > 0)
        {
          Utility.LoadUltraCBLocationListByWHPid(ucbLocationDefault, this.whPid);
          ucbLocationDefault.Value = locationPid;
        }
        else
        {
          ucbLocationDefault.Value = null;
        }
        this.status = DBConvert.ParseInt(dtReceivingInfo.Rows[0]["Posting"].ToString());
        chkConfirm.Checked = this.status == 0 ? false : true;
        uneSubTotalAmount.Value = DBConvert.ParseDouble(dtReceivingInfo.Rows[0]["SubTotalAmount"].ToString());
        uneDiscountPercent.Value = DBConvert.ParseDouble(dtReceivingInfo.Rows[0]["DiscountPercent"].ToString());
        uneDiscountAmount.Value = DBConvert.ParseDouble(dtReceivingInfo.Rows[0]["DiscountAmount"].ToString());
        uneShippingFee.Value = DBConvert.ParseDouble(dtReceivingInfo.Rows[0]["ShippingFee"].ToString());
        uneExtraFee.Value = DBConvert.ParseDouble(dtReceivingInfo.Rows[0]["ExtraFee"].ToString());
        uneTaxPercent.Value = DBConvert.ParseDouble(dtReceivingInfo.Rows[0]["TaxPercent"].ToString());
        uneTaxAmount.Value = DBConvert.ParseDouble(dtReceivingInfo.Rows[0]["TaxAmount"].ToString());
        uneTotalAmount.Value = DBConvert.ParseDouble(dtReceivingInfo.Rows[0]["TotalAmount"].ToString());
      }
      else
      {
        DBParameter[] outputReceiptCode = new DBParameter[] { new DBParameter("@NewDocCode", DbType.String, 32, string.Empty) };
        DBParameter[] inputReceiptCode = new DBParameter[2];
        inputReceiptCode[0] = new DBParameter("@DocTypePid", DbType.Int32, this.docTypePid);
        inputReceiptCode[1] = new DBParameter("@DocDate", DbType.Date, DateTime.Now.Date);

        DataBaseAccess.SearchStoreProcedure("spACCGetNewDocCode", inputReceiptCode, outputReceiptCode);
        if (outputReceiptCode[0].Value.ToString().Length > 0)
        {
          txtReceivingNote.Text = outputReceiptCode[0].Value.ToString();
          this.txtDate.Text = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);          
          this.status = 0;
        }

        // Load default WH
        if (ucbWarehouse.Rows.Count > 0)
        {
          ucbWarehouse.Rows[0].Selected = true;
        }
      }
    }


    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      if (this.receivingPid > 0)
      {
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@RECNo", DbType.AnsiString, 16, this.receivingNo) };
        DataSet dsMain = DataBaseAccess.SearchStoreProcedure("spWHDMaterialInStore_Select", inputParam);
        this.LoadMainData(dsMain.Tables[0]);
        // Load Detail
        ultRecInfomation.DataSource = dsMain.Tables[1];
        this.LoadTransationData();
        // Set Status control
        this.SetStatusControl();
      }
      else if (this.actionCode == 1) //normal
      {

      }
      else if (this.actionCode == 2) //Invoice
      {
        viewWHD_05_009 view = new viewWHD_05_009();
        Shared.Utility.WindowUtinity.ShowView(view, "Choose Invoice To Make Receiving Note", false, ViewState.ModalWindow, FormWindowState.Normal);
        if (view.dtInvoiceDetail == null || view.dtInvoiceDetail.Rows.Count == 0)
        {
          this.CloseTab();
          return;
        }
        else
        {   
          DataTable dtInvoiceDetail = view.dtInvoiceDetail;
          SqlDBParameter[] inputParam = new SqlDBParameter[3];
          inputParam[0] = new SqlDBParameter("@InvoiceMaterialList", SqlDbType.NVarChar, DBConvert.ParseXMLString(dtInvoiceDetail));
          inputParam[1] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
          inputParam[2] = new SqlDBParameter("@CreateBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          DataSet dsSource = SqlDataBaseAccess.SearchStoreProcedure("spWHDMaterialInStore_FromInvoice", inputParam);
          if (dsSource != null && dsSource.Tables.Count > 1)
          {
            this.LoadMainData(dsSource.Tables[0]);
            ultRecInfomation.DataSource = dsSource.Tables[1];
          }
        }
      }
      else if (this.actionCode == 3) //PO
      {
        viewWHD_05_008 view = new viewWHD_05_008();
        Shared.Utility.WindowUtinity.ShowView(view, "Chọn phiếu mua hàng", false, ViewState.ModalWindow, FormWindowState.Normal);
        if (view.dtPODetail == null || view.dtPODetail.Rows.Count == 0)
        {
          this.CloseTab();
          return;
        }
        else
        {
          DataTable dtPODetail = view.dtPODetail;
          SqlDBParameter[] inputParam = new SqlDBParameter[3];
          inputParam[0] = new SqlDBParameter("@POMaterialList", SqlDbType.NVarChar, DBConvert.ParseXMLString(dtPODetail));
          inputParam[1] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
          inputParam[2] = new SqlDBParameter("@CreateBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          DataSet dsSource = SqlDataBaseAccess.SearchStoreProcedure("spWHDMaterialInStore_FromPO", inputParam);
          if (dsSource != null && dsSource.Tables.Count > 1)
          {
            this.LoadMainData(dsSource.Tables[0]);
            ultRecInfomation.DataSource = dsSource.Tables[1];
          }
        }
      }
      if (ultRecInfomation.Rows.Count > 0)
      {
        ultCBSupplier.ReadOnly = true;
        ucbCurrency.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
      }
      else
      {
        ultCBSupplier.ReadOnly = false;
        ucbCurrency.ReadOnly = false;
        uneExchangeRate.ReadOnly = false;
      }
      lblCount.Text = string.Format(@"Đếm: {0}", ultRecInfomation.Rows.FilteredInRowCount > 0 ? ultRecInfomation.Rows.FilteredInRowCount : 0);
    }

    /// <summary>
    /// Load transaction
    /// </summary>
    private void LoadTransationData()
    {
      grdPostTran.SetDataSource(this.docTypePid, this.receivingPid);
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      bool isReadOnly = (this.status == 1 ? true : false) ;
      if (this.status == 1)
      {
        txtTitle.ReadOnly = isReadOnly;
        ultCBSupplier.ReadOnly = isReadOnly;
        ucbWarehouse.ReadOnly = isReadOnly;        
        chkConfirm.Checked = isReadOnly;
        txtLotNo.ReadOnly = isReadOnly;
        ucbCurrency.ReadOnly = isReadOnly;
        uneExchangeRate.ReadOnly = isReadOnly;
        ucbEnvironment.ReadOnly = isReadOnly;
        ucbCountry.ReadOnly = isReadOnly;
        ucbLocationDefault.ReadOnly = isReadOnly;
      }

      if (this.status == 0 && this.receivingNo.Length > 0)
      {
        this.chkConfirm.Enabled = true;
        this.btnSave.Enabled = true;
      }
      else
      {
        this.chkConfirm.Enabled = false;
        this.btnSave.Enabled = false;
      }

      if (ultRecInfomation.Rows.Count > 0)
      {
        ultCBSupplier.ReadOnly = true;
        ucbCurrency.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
      }
      else
      {
        ultCBSupplier.ReadOnly = false;
        ucbCurrency.ReadOnly = false;
        uneExchangeRate.ReadOnly = false;
      }      
    }
    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool success = this.SaveReceivingInfo();
      if (!success)
      {
        return false;
      }
      success = this.SaveReceivingDetail();
      if (!success)
      {
        return false;
      }
      return success;
    }

    /// <summary>
    /// Save Receiving Info
    /// </summary>
    /// <returns></returns>
    private bool SaveReceivingInfo()
    {

      DBParameter[] inputParam = new DBParameter[23];

      inputParam[0] = new DBParameter("@DocTypePid", DbType.Int32, this.docTypePid);
      inputParam[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      if (txtTitle.Text.Length > 0)
      {
        inputParam[2] = new DBParameter("@Title", DbType.String, 4000, txtTitle.Text);
      }
      inputParam[3] = new DBParameter("@Posting", DbType.Int32, (chkConfirm.Checked ? 1 : 0));      
      inputParam[4] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);
      inputParam[5] = new DBParameter("@Type", DbType.Int32, 1);
      inputParam[6] = new DBParameter("@Supplier", DbType.String, 50, ultCBSupplier.Value.ToString());

      inputParam[7] = new DBParameter("@ActionCode", DbType.Int32, this.actionCode);
      if (DBConvert.ParseDouble(uneSubTotalAmount.Value) >= 0)
      {
        inputParam[8] = new DBParameter("@SubTotalAmount", DbType.Double, DBConvert.ParseDouble(uneSubTotalAmount.Value));
      }
      if (DBConvert.ParseDouble(uneDiscountPercent.Value) >= 0)
      {
        inputParam[9] = new DBParameter("@DiscountPercent", DbType.Double, DBConvert.ParseDouble(uneDiscountPercent.Value));
      }
      if (DBConvert.ParseDouble(uneDiscountAmount.Value) >= 0)
      {
        inputParam[10] = new DBParameter("@DiscountAmount", DbType.Double, DBConvert.ParseDouble(uneDiscountAmount.Value));
      }
      if (DBConvert.ParseDouble(uneShippingFee.Value) >= 0)
      {
        inputParam[11] = new DBParameter("@ShippingFee", DbType.Double, DBConvert.ParseDouble(uneShippingFee.Value));
      }
      if (DBConvert.ParseDouble(uneExtraFee.Value) >= 0)
      {
        inputParam[12] = new DBParameter("@ExtraFee", DbType.Double, DBConvert.ParseDouble(uneExtraFee.Value));
      }
      if (DBConvert.ParseDouble(uneTaxPercent.Value) >= 0)
      {
        inputParam[13] = new DBParameter("@TaxPercent", DbType.Double, DBConvert.ParseDouble(uneTaxPercent.Value));
      }
      if (DBConvert.ParseDouble(uneTaxAmount.Value) >= 0)
      {
        inputParam[14] = new DBParameter("@TaxAmount", DbType.Double, DBConvert.ParseDouble(uneTaxAmount.Value));
      }
      if (DBConvert.ParseDouble(uneTotalAmount.Value) >= 0)
      {
        inputParam[15] = new DBParameter("@TotalAmount", DbType.Double, DBConvert.ParseDouble(uneTotalAmount.Value));
      }
      if (this.receivingPid != long.MinValue)
      {
        inputParam[16] = new DBParameter("@Pid", DbType.Int64, this.receivingPid);
      }
      inputParam[17] = new DBParameter("@CurrencyPid", DbType.Int32, ucbCurrency.Value);
      inputParam[18] = new DBParameter("@ExchangeRate", DbType.Double, uneExchangeRate.Value);
      if (ucbEnvironment.SelectedRow != null)
      {
        inputParam[19] = new DBParameter("@EnvironmentPid", DbType.Int32, ucbEnvironment.Value);
      }
      if (ucbCountry.SelectedRow != null)
      {
        inputParam[20] = new DBParameter("@CountryPid", DbType.Int32, ucbCountry.Value);
      }
      if (txtLotNo.Text.Trim().Length > 0)
      {
        inputParam[21] = new DBParameter("@LotNo", DbType.AnsiString, 50, txtLotNo.Text.Trim());
      }
      if (ucbLocationDefault.SelectedRow != null)
      {
        inputParam[22] = new DBParameter("@LocationPid", DbType.Int32, ucbLocationDefault.Value);
      }
      DBParameter[] outputParam = new DBParameter[2];
      outputParam[0] = new DBParameter("@ResultRECNo", DbType.AnsiString, 50, string.Empty);
      outputParam[1] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spWHDMaterialInStore_Edit", inputParam, outputParam);
      this.receivingNo = outputParam[0].Value.ToString();
      this.receivingPid = DBConvert.ParseLong(outputParam[1].Value);
      long result = DBConvert.ParseLong(outputParam[1].Value);
      if (result == 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save Receiving Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveReceivingDetail()
    {
      // 1. Delete      
      foreach (long recDetailPid in this.listDeletedDetailPid)
      {
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@ReceivingDetailPid", DbType.Int64, recDetailPid);
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure("spWHDMaterialInStoreDetail_Delete", input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result == 0)
        {
          return false;
        }
      }
      this.listDeletedDetailPid.Clear();
      // 2. Insert/Update
      if (ultRecInfomation.Rows.Count > 0)
      {
        for (int i = 0; i < ultRecInfomation.Rows.Count; i++)
        {
          UltraGridRow rowInfo = ultRecInfomation.Rows[i];
          
          if (DBConvert.ParseDouble(rowInfo.Cells["Qty"].Value.ToString()) != 0)
          {
            double countOfQty = DBConvert.ParseDouble(rowInfo.Cells["CountOfQty"].Value);
            double countOfQtyUnitCost = DBConvert.ParseDouble(rowInfo.Cells["CountOfQtyUnitCost"].Value);
            double countOfQtyTotalCost = DBConvert.ParseDouble(rowInfo.Cells["CountOfQtyTotalCost"].Value);
            double perimeter = DBConvert.ParseDouble(rowInfo.Cells["Perimeter"].Value);
            double length = DBConvert.ParseDouble(rowInfo.Cells["Length"].Value);
            double lengthMin = DBConvert.ParseDouble(rowInfo.Cells["LengthMin"].Value);
            double lengthMax = DBConvert.ParseDouble(rowInfo.Cells["LengthMax"].Value);
            double width = DBConvert.ParseDouble(rowInfo.Cells["Width"].Value);
            double widthMin = DBConvert.ParseDouble(rowInfo.Cells["WidthMin"].Value);
            double widthMax = DBConvert.ParseDouble(rowInfo.Cells["WidthMax"].Value);
            int modelPid = DBConvert.ParseInt(rowInfo.Cells["ModelPid"].Value);
            int modelDetailPid = DBConvert.ParseInt(rowInfo.Cells["ModelDetailPid"].Value);
            int qualityPid = DBConvert.ParseInt(rowInfo.Cells["QualityPid"].Value);
            long productSerialPid = DBConvert.ParseLong(rowInfo.Cells["ProductSerialPid"].Value);
                        
            DBParameter[] inputParam = new DBParameter[28];
            long receivingDetailPid = DBConvert.ParseLong(rowInfo.Cells["ReceivingDetailPid"].Value);
            if (receivingDetailPid > 0)
            {
              inputParam[0] = new DBParameter("@ReceivingDetailPid", DbType.Int64, receivingDetailPid);
            }
            inputParam[1] = new DBParameter("@ReceivingNo", DbType.AnsiString, 50, this.receivingNo);
            inputParam[2] = new DBParameter("@MaterialCode", DbType.AnsiString, 255, rowInfo.Cells["MaterialCode"].Value.ToString());
            inputParam[3] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(rowInfo.Cells["Qty"].Value.ToString()));
            inputParam[4] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);
            inputParam[5] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            inputParam[6] = new DBParameter("@PONo", DbType.AnsiString, 16, rowInfo.Cells["PONo"].Value.ToString());
            inputParam[7] = new DBParameter("@Currency", DbType.Int32, ucbCurrency.Value);
            inputParam[8] = new DBParameter("@ExchangeRate", DbType.Double, uneExchangeRate.Value);
            inputParam[9] = new DBParameter("@Price", DbType.Double, DBConvert.ParseDouble(rowInfo.Cells["Price"].Value.ToString()));
            if (rowInfo.Cells["LotNo"].Value.ToString().Trim().Length > 0)
            {
              inputParam[10] = new DBParameter("@LotNo", DbType.AnsiString, 50, rowInfo.Cells["LotNo"].Value.ToString());
            }
            if (rowInfo.Cells["SerialNo"].Value.ToString().Trim().Length > 0)
            {
              inputParam[11] = new DBParameter("@SerialNo", DbType.AnsiString, 20, rowInfo.Cells["SerialNo"].Value.ToString());
            }
            inputParam[12] = new DBParameter("@Location", DbType.Int32, DBConvert.ParseInt(rowInfo.Cells["Location"].Value.ToString()));
            if(DBConvert.ParseLong(rowInfo.Cells["InvoiceInDetailPid"].Value.ToString()) > 0)
            {
              inputParam[13] = new DBParameter("@InvoiceInDetailPid", DbType.Int64, DBConvert.ParseLong(rowInfo.Cells["InvoiceInDetailPid"].Value.ToString()));
            }
            if (countOfQty > 0)
            {
              inputParam[14] = new DBParameter("@CountOfQty", DbType.Double, countOfQty);
            }
            if (countOfQtyUnitCost > 0)
            {
              inputParam[15] = new DBParameter("@CountOfQtyUnitCost", DbType.Double, countOfQtyUnitCost);
            }
            if (countOfQtyTotalCost > 0)
            {
              inputParam[16] = new DBParameter("@CountOfQtyTotalCost", DbType.Double, countOfQtyTotalCost);
            }
            if (perimeter > 0)
            {
              inputParam[17] = new DBParameter("@Perimeter", DbType.Double, perimeter);
            }
            if (length > 0)
            {
              inputParam[18] = new DBParameter("@Length", DbType.Double, length);
            }
            if (lengthMin > 0)
            {
              inputParam[19] = new DBParameter("@LengthMin", DbType.Double, lengthMin);
            }
            if (lengthMax > 0)
            {
              inputParam[20] = new DBParameter("@LengthMax", DbType.Double, lengthMax);
            }
            if (width > 0)
            {
              inputParam[21] = new DBParameter("@Width", DbType.Double, width);
            }
            if (widthMin > 0)
            {
              inputParam[22] = new DBParameter("@WidthMin", DbType.Double, widthMin);
            }
            if (widthMax > 0)
            {
              inputParam[23] = new DBParameter("@WidthMax", DbType.Double, widthMax);
            }
            if (modelPid > 0)
            {
              inputParam[24] = new DBParameter("@ModelPid", DbType.Int32, modelPid);
            }
            if (modelDetailPid > 0)
            {
              inputParam[25] = new DBParameter("@ModelDetailPid", DbType.Double, modelDetailPid);
            }
            if (qualityPid > 0)
            {
              inputParam[26] = new DBParameter("@QualityPid", DbType.Double, qualityPid);
            }
            if (productSerialPid > 0)
            {
              inputParam[27] = new DBParameter("@ProductSerialPid", DbType.Double, productSerialPid);
            }
            DBParameter[] outputParam = new DBParameter[1];
            outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

            DataBaseAccess.ExecuteStoreProcedure("spWHDMaterialInStoreDetail_Edit", inputParam, outputParam);
            long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
            if (result == 0)
            {
              return false;
            }
          }
        }      
      }
      return true;
    }    

    /// <summary>
    /// Save Mapping PO
    /// </summary>
    /// <returns></returns>
    private bool SaveMappingPOReceiving()
    {
      if (chkConfirm.Checked)
      {
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@IDPhieuNhap", DbType.AnsiString, 50, txtReceivingNote.Text);
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure("spWHDMaterialInStore_Confirm", 5000, input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result == 0)
        {
          return false;
        }
        else
        {
          // Update Status PR And PO
          DataBaseAccess.ExecuteStoreProcedure(SP_WHDStatusPRPO_Update, 5000, input, output);
          result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result == 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Check ValidInfo
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidInfo()
    {
      string message = string.Empty;
      if (ultCBSupplier.SelectedRow == null)
      {
        message = lblSupplier.Text;
        WindowUtinity.ShowMessageError("ERR0001", message);
        return false;
      }
      if (ucbWarehouse.SelectedRow == null)
      {
        message = lblWarehouse.Text;
        WindowUtinity.ShowMessageError("ERR0001", message);
        return false;
      }  
      if (ucbCurrency.SelectedRow == null)
      {
        message = lblCurrency.Text;
        WindowUtinity.ShowMessageError("ERR0001", message);
        return false;
      }
      if (DBConvert.ParseDouble(uneExchangeRate.Value) <= 0)
      {
        message = lblExchangeRate.Text;
        WindowUtinity.ShowMessageError("ERR0001", message);
        return false;
      }
      // Create new receiving note
      if (this.receivingNo.Length == 0)
      {
        // Check WH Summary of preMonth
        if (!Utility.CheckWHPreMonthSummary(this.whPid))
        {
          return false;
        }
      }
      // Check Lot No, Serial No, Qty
      DataTable dtDataCheck = new DataTable();
      dtDataCheck.Columns.Add("ReceivingPid", typeof(System.Int64));
      dtDataCheck.Columns.Add("PONo", typeof(System.String));
      dtDataCheck.Columns.Add("SerialNo", typeof(System.String));
      dtDataCheck.Columns.Add("MaterialCode", typeof(System.String));
      dtDataCheck.Columns.Add("Qty", typeof(System.Double));
      for (int i = 0; i < ultRecInfomation.Rows.Count; i++)
      {
        UltraGridRow rowGrid = this.ultRecInfomation.Rows[i];
        if (rowGrid.Cells["LotNo"].Value.ToString().Length == 0)
        {          
          WindowUtinity.ShowMessageErrorFromText("Mã lô không được để trống");
          return false;
        }
        if (DBConvert.ParseInt(rowGrid.Cells["Location"].Value) == int.MinValue)
        {
          WindowUtinity.ShowMessageErrorFromText("Vị trí không được để trống");
          return false;
        }
        DataRow rowCheck = dtDataCheck.NewRow();
        rowCheck["ReceivingPid"] = this.receivingPid;
        rowCheck["PONo"] = rowGrid.Cells["PONo"].Value;
        rowCheck["SerialNo"] = rowGrid.Cells["SerialNo"].Value;
        rowCheck["MaterialCode"] = rowGrid.Cells["MaterialCode"].Value;
        rowCheck["Qty"] = rowGrid.Cells["Qty"].Value;
        dtDataCheck.Rows.Add(rowCheck);
      }
      
      SqlDBParameter[] inputParam = new SqlDBParameter[] { new SqlDBParameter("@POMaterialList", SqlDbType.NVarChar, DBConvert.ParseXMLString(dtDataCheck)) };
      DataSet dsDataCheck = SqlDataBaseAccess.SearchStoreProcedure("spWHDMaterialInStore_CheckValid", inputParam);
      if(dsDataCheck != null && dsDataCheck.Tables.Count > 1)
      {
        // Check SerialNo
        if(dsDataCheck.Tables[0].Rows.Count > 0)
        {
          WindowUtinity.ShowMessageErrorFromText(string.Format("{0} bị trùng lắp", dsDataCheck.Tables[0].Rows[0]["SerialNo"]));
          return false;
        }

        // Check qty
        if (dsDataCheck.Tables[1].Rows.Count > 0)
        {
          DataRow row = dsDataCheck.Tables[1].Rows[0];
          WindowUtinity.ShowMessageErrorFromText(string.Format("Số lượng sản phẩm {0} của đơn hàng {1} không được vượt quá {2}", row["MaterialCode"], row["PONo"], row["Remain"]));
          return false;
        }
      }

      return true;
    }

    /// <summary>
    ///  Check Valid Receiving
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidAfter(out string message)
    {
      message = string.Empty;
      DataTable dtLocation = (DataTable)ucbLocation.DataSource;
      for (int i = 0; i < ultRecInfomation.Rows.Count; i++)
      {
        UltraGridRow row = ultRecInfomation.Rows[i];
        int iLocation = DBConvert.ParseInt(row.Cells["Location"].Value);
        DataRow[] locationRows = dtLocation.Select(string.Format("Pid = {0}", iLocation));
        if (locationRows.Length == 0)
        {
          message = "Location";
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Send Email
    /// </summary>
    private void SendEmail()
    {
      if (chkConfirm.Checked == true)
      {
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ReceivingNote", DbType.AnsiString, 48, txtReceivingNote.Text.Trim()) };
        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDRPTReceivingNoteSendToPurchasing_Materials", inputParam);
        dsWHDRPTReceivingInfoSendToPurchasing dsReceiving = new dsWHDRPTReceivingInfoSendToPurchasing();
        dsReceiving.Tables["dtReceivingInfo"].Merge(dsSource.Tables[0]);
        dsReceiving.Tables["dtReceivingDetail"].Merge(dsSource.Tables[1]);
        string receivingNote = dsReceiving.Tables["dtReceivingInfo"].Rows[0]["ReceivingNote"].ToString();
        string createBy = dsReceiving.Tables["dtReceivingInfo"].Rows[0]["CreateBy"].ToString();
        double totalQty = 0;
        for (int i = 0; i < dsReceiving.Tables["dtReceivingDetail"].Rows.Count; i++)
        {
          if (DBConvert.ParseDouble(dsReceiving.Tables["dtReceivingDetail"].Rows[i]["Qty"].ToString()) != double.MinValue)
          {
            totalQty = totalQty + DBConvert.ParseDouble(dsReceiving.Tables["dtReceivingDetail"].Rows[i]["Qty"].ToString());
          }
        }
        // Mail
        MailMessage mailMessage = new MailMessage();
        string body = string.Empty;
        string pathImage = @"Logo.JPG";
        try
        {
          body += "<p><i><font color='6699CC'>";
          body += "Dear Both,<br>";
          body += "Please see below:<br><br>";
          body += "<table>";
          body += "<tr>";
          body += "<th ROWSPAN= 2>";

          body += "<img src =\"" + pathImage + "\"  alt='Smiley face' height='43' width='234'>";
          body += "</th>";
          body += "<td>";
          body += "&nbsp &nbsp &nbsp Vietnam Furniture Resources Company limited (VFR Co.,ltd)";
          body += "</td>";
          body += "</tr>";
          body += "<tr>";
          body += "<td>";
          body += "&nbsp &nbsp &nbsp Binh Chuan ward, Thuan An town, Binh Duong province.";
          body += "</td>";
          body += "</tr>";
          body += "</table>";
          body += "GOODS RECEIVING CONFIRMATION:<br><br>";
          body += "WE'D LIKE TO CONFIRM THAT YOUR PR, AS DETAILED BELOW HAS BEEN RECEIVED INTO OUR WAREHOUSE <br>";
          body += "</font></i></p> ";
          body += "<p><i><font color='red'>";
          body += "RECEIVING NOTE: ";
          body += dsReceiving.Tables["dtReceivingInfo"].Rows[0]["ReceivingNote"].ToString();
          body += "</font></i></p> ";
          // Table
          body += "<table border = '1'>";
          // Row Name
          body += "<tr>";
          body += "<th>";
          body += "No";
          body += "</th>";
          body += "<th>";
          body += "PRNo";
          body += "</th>";
          body += "<th>";
          body += "Material Code";
          body += "</th>";
          body += "<th>";
          body += "Name EN";
          body += "</th>";
          body += "<th>";
          body += "Name VN";
          body += "</th>";
          body += "<th>";
          body += "Unit";
          body += "</th>";
          body += "<th>";
          body += "Qty";
          body += "</th>";
          body += "<th>";
          body += "Purpose of PR";
          body += "</th>";
          body += "</tr>";
          // Row Detail
          for (int i = 0; i < dsReceiving.Tables["dtReceivingDetail"].Rows.Count; i++)
          {
            body += "<tr>";
            body += "<td>";
            body += i + 1;
            body += "</td>";
            body += "<td>";
            body += dsReceiving.Tables["dtReceivingDetail"].Rows[i]["PRNo"].ToString();
            body += "</td>";
            body += "<td>";
            body += dsReceiving.Tables["dtReceivingDetail"].Rows[i]["MaterialCode"].ToString();
            body += "</td>";
            body += "<td>";
            body += dsReceiving.Tables["dtReceivingDetail"].Rows[i]["NameEN"].ToString();
            body += "</td>";
            body += "<td>";
            body += dsReceiving.Tables["dtReceivingDetail"].Rows[i]["NameVN"].ToString();
            body += "</td>";
            body += "<td>";
            body += dsReceiving.Tables["dtReceivingDetail"].Rows[i]["Unit"].ToString();
            body += "</td>";
            body += "<td>";
            body += dsReceiving.Tables["dtReceivingDetail"].Rows[i]["Qty"].ToString();
            body += "</td>";
            body += "<td>";
            body += dsReceiving.Tables["dtReceivingDetail"].Rows[i]["Remark"].ToString();
            body += "</td>";
            body += "</tr>";
          }
          body += "</table>";
          body += "<p><i><font color='6699CC'>";
          body += "THIS E-MAIL IS AUTOMATICALLY SENT TO PURCHASER & REQUESTER, PLEASE SHOULD NOT REPLY. THANKS. <br><br><br>";
          body += "</font></i></p> ";
          body += "<p><i><font color='red'>";
          body += "Automatic Email From ERP System.";
          body += "</font></i></p> ";
          // End body
        }
        catch (Exception e)
        {
          MessageBox.Show("Message 1: " + e.Message);
        }
        string mailTo = string.Empty;
        try
        {
          DBParameter[] input = new DBParameter[1];
          input[0] = new DBParameter("@ReceivingNote", DbType.String, dsReceiving.Tables["dtReceivingInfo"].Rows[0]["ReceivingNote"].ToString());
          DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHDGetListEmailWhenReceivingMaterial", input);
          if (dtSource != null && dtSource.Rows.Count > 0)
          {
            mailTo = dtSource.Rows[0]["Email"].ToString();
          }

          mailMessage.ServerName = "10.0.0.6";
          mailMessage.Username = "dc@vfr.net.vn";
          mailMessage.Password = "dc123456";
          mailMessage.From = "dc@vfr.net.vn";

          mailMessage.To = mailTo;
          //mailMessage.To = "tien_it@vfr.net.vn";
          mailMessage.Subject = "RECEIVING NOTE";
          mailMessage.Body = body;
          //mailMessage.Bcc = "Chau@daico-furniture.com,minh_it@daico-furniture.com,truong_it@daico-furniture.com";
          //mailMessage.Bcc = "truong_it@daico-furniture.com";
          IList attachments = new ArrayList();
          attachments.Add(pathImage);
          mailMessage.Attachfile = attachments;
          mailMessage.SendMail(true);
        }
        catch (Exception e)
        {
          MessageBox.Show("Message 2:" + e.Message);
        }
      }
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Init Receivinf Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultRecInfomation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ultRecInfomation);
      Utility.HideColumnsOnGrid(ultRecInfomation);
      e.Layout.AutoFitStyle = AutoFitStyle.None;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
     
      if (this.status == 1)
      {
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      }
 
      e.Layout.Bands[0].Columns["ReceivingDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["InvoiceInDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ProductSerialPid"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialName"].Hidden = true;

      e.Layout.Bands[0].Columns["Location"].ValueList = ucbLocation;
      e.Layout.Bands[0].Columns["Location"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["Location"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["ModelPid"].ValueList = ucbModel;
      e.Layout.Bands[0].Columns["ModelPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ModelPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["ModelDetailPid"].ValueList = ucbModelDetail;
      e.Layout.Bands[0].Columns["ModelDetailPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ModelDetailPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["QualityPid"].ValueList = ucbQuality;
      e.Layout.Bands[0].Columns["QualityPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["QualityPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["Location"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;      

      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialNameVn"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;      
      e.Layout.Bands[0].Columns["Price"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalCost"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CountOfQtyTotalCost"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["PONo"].Header.Caption = "Đơn mua hàng";
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Mã sản phẩm";
      e.Layout.Bands[0].Columns["MaterialNameVn"].Header.Caption = "Tên sản phẩm";
      e.Layout.Bands[0].Columns["Unit"].Header.Caption = "Đơn vị";
      e.Layout.Bands[0].Columns["Price"].Header.Caption = "Đơn giá";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Số lượng";
      e.Layout.Bands[0].Columns["TotalCost"].Header.Caption = "Tổng tiền";
      e.Layout.Bands[0].Columns["Location"].Header.Caption = "Vị trí";
      e.Layout.Bands[0].Columns["LotNo"].Header.Caption = "Mã lô";
      e.Layout.Bands[0].Columns["SerialNo"].Header.Caption = "Mã kiện";
      e.Layout.Bands[0].Columns["CountOfQty"].Header.Caption = "Số tấm";
      e.Layout.Bands[0].Columns["CountOfQtyUnitCost"].Header.Caption = "Đơn giá\n(tấm)";
      e.Layout.Bands[0].Columns["CountOfQtyTotalCost"].Header.Caption = "Thành tiền\n(tấm)";
      e.Layout.Bands[0].Columns["Perimeter"].Header.Caption = "Hoành\n(mm)";
      e.Layout.Bands[0].Columns["Length"].Header.Caption = "Dài\n(mm)";
      e.Layout.Bands[0].Columns["LengthMin"].Header.Caption = "Dài min\n(mm)";
      e.Layout.Bands[0].Columns["LengthMax"].Header.Caption = "Dài max\n(mm)";
      e.Layout.Bands[0].Columns["Width"].Header.Caption = "Rộng\n(mm)";
      e.Layout.Bands[0].Columns["WidthMin"].Header.Caption = "Rộng min\n(mm)";
      e.Layout.Bands[0].Columns["WidthMax"].Header.Caption = "Rộng max\n(mm)";
      e.Layout.Bands[0].Columns["ModelPid"].Header.Caption = "Nhãn hiệu";
      e.Layout.Bands[0].Columns["ModelDetailPid"].Header.Caption = "Model";
      e.Layout.Bands[0].Columns["QualityPid"].Header.Caption = "Chất lượng";

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
    }

    /// <summary>
    ///  Add
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckValidInfo();
      if (!success)
      {        
        return;
      }
      success = this.SaveData();
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
    /// <summary>
    /// Value Change PONo
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //private void ultCBPONo_ValueChanged(object sender, EventArgs e)
    //{
    //  if (ultCBPONo.SelectedRow != null)
    //  {
    //    if (ultCBSupplier.Value == null)
    //    {
    //      ultCBSupplier.Value = ultCBPONo.SelectedRow.Cells["SupplierCode"].Value.ToString();
    //    }
    //    else
    //    {
    //      string value = ultCBPONo.Value.ToString().Trim();
    //      DBParameter[] inputParam = new DBParameter[2];
    //      inputParam[0] = new DBParameter("@PONo", DbType.AnsiString, 16, value);
    //      inputParam[1] = new DBParameter("@WHPid", DbType.Int32, this.whPid);
    //      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(SP_WHDListMaterialMakeByPONo, inputParam);
    //      dsWHDMaterialMakeByPONo ds = new dsWHDMaterialMakeByPONo();
    //      ds.Tables["dtInfo"].Merge(dsSource.Tables[0]);
    //      ds.Tables["dtDetail"].Merge(dsSource.Tables[1]);
    //      this.ultPOInfomation.DataSource = ds;
    //      if (dsSource != null)
    //      {
    //        this.ultPOInfomation.DataSource = ds;
    //      }
    //      // End button Add
    //      if (this.status == 0)
    //      {
    //        btnAdd.Enabled = true;
    //      }
    //      else
    //      {
    //        btnAdd.Enabled = false;
    //      }
    //    }
    //  }
    //  else
    //  {
    //    ultPOInfomation.DataSource = null;
    //    btnAdd.Enabled = false;
    //    return;
    //  }
    //}
    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckValidInfo();
      if (!success)
      {        
        return;
      }
      // Check Valid
      success = this.CheckValidAfter(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      // Save Info
      success = this.SaveData();
      if (success)
      {
        if (chkConfirm.Checked)
        {
          // Save Mapping When Confirm
          success = this.SaveMappingPOReceiving();
          if (success)
          {
            bool isPosted = chkConfirm.Checked;
            success = Utility.ACCPostTransaction(this.docTypePid, this.receivingPid, isPosted, SharedObject.UserInfo.UserPid);
            // Send Email Purchasing 
            //this.SendEmail();
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        // Load lai Data
        this.LoadData();      
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }     
    }

    /// <summary>
    /// Before Delete Row of Receiving
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultRecInfomation_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingDetailPid = new ArrayList();
      this.listMaterialDeletingPid = new ArrayList();

      foreach (UltraGridRow row in e.Rows)
      {
        long recDetailPid = long.MinValue;
        string materialCode = string.Empty;
        try
        {
          recDetailPid = DBConvert.ParseLong(row.Cells["ReceivingDetailPid"].Value.ToString());
        }
        catch { }

        try
        {
          materialCode = row.Cells["MaterialCode"].Value.ToString();
        }
        catch { }

        if (recDetailPid != long.MinValue)
        {
          this.listDeletingDetailPid.Add(recDetailPid);
        }
        if (materialCode != string.Empty)
        {
          this.listMaterialDeletingPid.Add(materialCode);
        }
      }
    }

    /// <summary>
    /// After Delete Row of Receiving
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultRecInfomation_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long recDetailpid in this.listDeletingDetailPid)
      {
        this.listDeletedDetailPid.Add(recDetailpid);
      }
      foreach (string materialCode in this.listMaterialDeletingPid)
      {
        this.listMaterialDeletedPid.Add(materialCode);
      }
    }

    /// <summary>
    /// After Cell Update Receiving
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultRecInfomation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "qty":
          double qty = DBConvert.ParseDouble(e.Cell.Value);
          double price = DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value);
          double totalCost = Math.Round(qty * price, this.oddNumber);
          if (totalCost >= 0)
          {
            e.Cell.Row.Cells["TotalCost"].Value = totalCost;
          }
          else
          {
            e.Cell.Row.Cells["TotalCost"].Value = DBNull.Value;
          }  
          break;
        case "countofqty":
        case "countofqtyunitcost":
          double countOfQty = DBConvert.ParseDouble(e.Cell.Row.Cells["CountOfQty"].Value);
          double countOfQtyUnitCost = DBConvert.ParseDouble(e.Cell.Row.Cells["CountOfQtyUnitCost"].Value);
          double countOfQtyTotalCost = Math.Round(countOfQty * countOfQtyUnitCost, this.oddNumber);
          if(countOfQtyTotalCost >= 0)
          {
            e.Cell.Row.Cells["CountOfQtyTotalCost"].Value = countOfQtyTotalCost;
          }  
          else
          {
            e.Cell.Row.Cells["CountOfQtyTotalCost"].Value = DBNull.Value;
          }  
          break;
        case "lotno":
          if(ultRecInfomation.DisplayLayout.Bands[0].Columns["SerialNo"].Hidden)
          {
            e.Cell.Row.Cells["SerialNo"].Value = e.Cell.Value;
          }  
          break;
        default:
          break;
      }
    }


    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }


    /// <summary>
    /// Print
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrint_Click(object sender, EventArgs e)
    {
      DBParameter[] input = new DBParameter[] { new DBParameter("@ReceivingNote", DbType.AnsiString, 48, txtReceivingNote.Text.Trim()) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDRPTReceivingNotePrint_Materials", input);
      dsWHDRPTMaterialsReceivingNote dsReceiving = new dsWHDRPTMaterialsReceivingNote();
      dsReceiving.Tables["dtReceivingInfo"].Merge(dsSource.Tables[0]);
      dsReceiving.Tables["dtReceivingDetail"].Merge(dsSource.Tables[1]);
      double totalQty = 0;
      for (int i = 0; i < dsReceiving.Tables["dtReceivingDetail"].Rows.Count; i++)
      {
        if (DBConvert.ParseDouble(dsReceiving.Tables["dtReceivingDetail"].Rows[i]["Qty"].ToString()) != double.MinValue)
        {
          totalQty = totalQty + DBConvert.ParseDouble(dsReceiving.Tables["dtReceivingDetail"].Rows[i]["Qty"].ToString());
        }
      }

      ReportClass cpt = null;
      DaiCo.Shared.View_Report report = null;

      cpt = new cptMaterialsReceivingNote();
      cpt.SetDataSource(dsReceiving);
      cpt.SetParameterValue("TotalQty", totalQty);
      cpt.SetParameterValue("Title", "MATERIALS RECEIVING NOTE");
      cpt.SetParameterValue("Receivedby", "Received by: ");
      report = new DaiCo.Shared.View_Report(cpt);
      report.IsShowGroupTree = false;
      report.ShowReport(Shared.Utility.ViewState.ModalWindow);
    }

    private void ucbWarehouse_ValueChanged(object sender, EventArgs e)
    {
      if (ucbWarehouse.SelectedRow != null)
      {
        this.whPid = DBConvert.ParseInt(ucbWarehouse.Value);
      }
      else
      {
        this.whPid = 0;
      }
      Utility.LoadUltraCBLocationListByWHPid(ucbLocation, this.whPid);
      ucbLocationDefault.Value = null;
      Utility.LoadUltraCBLocationListByWHPid(ucbLocationDefault, this.whPid);
      foreach (UltraGridRow row in ultRecInfomation.Rows)
      {
        row.Cells["Location"].Value = DBNull.Value;
      }
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
      discountAmount = Math.Round((discountPercent * subTotalAmount) / 100);
      uneDiscountAmount.Value = discountAmount;
    }

    private void uneTaxPercent_ValueChanged(object sender, EventArgs e)
    {
      double subTotalAmount, taxPercent, taxAmount, shippingFee, extraFee, discount;
      subTotalAmount = DBConvert.ParseDouble(uneSubTotalAmount.Value) >= 0 ? DBConvert.ParseDouble(uneSubTotalAmount.Value) : 0;
      discount = DBConvert.ParseDouble(uneDiscountAmount.Value) >= 0 ? DBConvert.ParseDouble(uneDiscountAmount.Value) : 0;
      shippingFee = DBConvert.ParseDouble(uneShippingFee.Value) >= 0 ? DBConvert.ParseDouble(uneShippingFee.Value) : 0;
      extraFee = DBConvert.ParseDouble(uneExtraFee.Value) >= 0 ? DBConvert.ParseDouble(uneExtraFee.Value) : 0;
      taxPercent = DBConvert.ParseDouble(uneTaxPercent.Value) >= 0 ? DBConvert.ParseDouble(uneTaxPercent.Value) : 0;
      taxAmount = Math.Round((taxPercent * (subTotalAmount + shippingFee + extraFee - discount)) / 100);
      uneTaxAmount.Value = taxAmount;
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

    private void txtLotNo_TextChanged(object sender, EventArgs e)
    {
      
    }
    private void txtLotNo_Leave(object sender, EventArgs e)
    {
      if (txtLotNo.ReadOnly)
        return;
      string lotNo = txtLotNo.Text.Trim();
      if (lotNo.Length > 0)
      {
        if (WindowUtinity.ShowMessageConfirmFromText("Bạn có muốn thay đổi mã lô cho những sản phẩm bên dưới không?") == DialogResult.Yes)
        {
          foreach (UltraGridRow row in ultRecInfomation.Rows)
          {
            row.Cells["LotNo"].Value = lotNo;
          }
        }
      }
    }

    private void ucbLocationDefault_Leave(object sender, EventArgs e)
    {
      if (ucbLocationDefault.ReadOnly)
        return;
      if(ucbLocationDefault.SelectedRow != null)      
      {
        int location = DBConvert.ParseInt(ucbLocationDefault.Value);
        if (WindowUtinity.ShowMessageConfirmFromText("Bạn có muốn thay đổi vị trí cho những sản phẩm bên dưới không?") == DialogResult.Yes)
        {
          foreach (UltraGridRow row in ultRecInfomation.Rows)
          {
            row.Cells["Location"].Value = location;
          }
        }
      }
    }
    #endregion Event 
  }
}
