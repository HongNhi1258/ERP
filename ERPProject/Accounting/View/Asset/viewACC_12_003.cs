/*
  Author      : Nguyen Thanh Binh
  Date        : 11/04/2021
  Description : bank asset
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
  public partial class viewACC_12_003 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private int status = 0;
    private DataTable dtObject = new DataTable();
    private int docTypePid = ConstantClass.Asset_Receipt;
    public int actionCode = 1;
    private int currency = int.MinValue;
    public int objectTye = int.MinValue;
    public int creditPid = int.MinValue;
    private bool isLoadedDetail = false;
    private bool isLoadedPostTransaction = false;
    private bool isLoadingAsset = false;
    #endregion Field

    #region Init
    public viewACC_12_003()
    {
      InitializeComponent();
    }

    private void viewACC_12_003_Load(object sender, EventArgs e)
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
      SqlDBParameter[] inputParam = new SqlDBParameter[2];
      inputParam[0] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      inputParam[1] = new SqlDBParameter("@DocumentPid", SqlDbType.BigInt, this.viewPid);
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCAssetReceipt_Init", inputParam);
      Utility.LoadUltraCombo(ucbActionCode, dsInit.Tables[0], "ActionCode", "ActionName", false, "ActionCode");
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[1], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate" });

      //Init dropdown
      Utility.LoadUltraCombo(ucbddEmployee, dsInit.Tables[2], "EmployeePid", "EmployeeName", false, "EmployeePid");
      Utility.LoadUltraCombo(ucbddDepartment, dsInit.Tables[3], "DepartmentPid", "DeparmentName", false, "DepartmentPid");
      Utility.LoadUltraCombo(ucbddMaterial, dsInit.Tables[4], "MaterialCode", "MaterialCode", true, new string[] { "IDKho", "PositionPid" });
      ucbddMaterial.DisplayLayout.Bands[0].Columns["MaterialCode"].Header.Caption = "Mã SP";
      ucbddMaterial.DisplayLayout.Bands[0].Columns["MaterialName"].Header.Caption = "Tên SP";
      ucbddMaterial.DisplayLayout.Bands[0].Columns["BatchNo"].Header.Caption = "Mã lô hàng";
      ucbddMaterial.DisplayLayout.Bands[0].Columns["PositionName"].Header.Caption = "Vị trí"; 
      Utility.LoadUltraCombo(ucbddWarehouse, dsInit.Tables[5], "Pid", "Code", false, "Pid");
      Utility.LoadUltraCombo(ucbddAssetType, dsInit.Tables[6], "Pid", "GroupName", false, new string[] { "Pid", "AccountPid" });
      Utility.LoadUltraCombo(ucbAsset, dsInit.Tables[7], "Pid", "AssetCode", false, new string[] { "Pid", "DepartmentPid", "UsedEmployeePid", "AccountPid", "AssetType", "DepreciationMonth" });
      Utility.LoadUltraCombo(ucbddUnCostConstruction, dsInit.Tables[8], "Value", "Display", false, "Value");
      Utility.LoadUltraCBAccountList(ucbddAccount);

      this.dtObject = dsInit.Tables[5];
      Utility.LoadUltraCBSupplier(ucbSupplier);            
      // Set Language
      //this.SetLanguage();
    }

    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        txtAssetReceiptCode.ReadOnly = true;
        udtAssetReceiptDate.ReadOnly = true;
        ucbCurrency.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
        txtAssetReceiptDesc.ReadOnly = true;
        btnSave.Enabled = false;
        chkConfirm.Enabled = false;
      }
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtAssetReceiptCode.Text = dtMain.Rows[0]["ReceiptCode"].ToString();
        txtAssetReceiptDesc.Text = dtMain.Rows[0]["ReceiptDesc"].ToString();
        udtAssetReceiptDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["ReceiptDate"]);
        ucbSupplier.Value = dtMain.Rows[0]["SupplierPid"];
        ucbCurrency.Value = DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"].ToString());
        uneExchangeRate.Value = DBConvert.ParseDouble(dtMain.Rows[0]["ExchangeRate"].ToString());
        ucbActionCode.Value = DBConvert.ParseInt(dtMain.Rows[0]["ActionCode"].ToString());
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
          txtAssetReceiptCode.Text = outputParam[0].Value.ToString();
          udtAssetReceiptDate.Value = DateTime.Now;
          ucbActionCode.Value = this.actionCode;
        }
      }
    }

    private void LoadTransationData()
    {
      ugdTransaction.SetDataSource(this.docTypePid, this.viewPid);
    }

    private void LoadAssetReceiptDetail()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      DataSet dsSource = SqlDataBaseAccess.SearchStoreProcedure("spACCAssetReceiptDetail_Load", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        ugdData.DataSource = dsSource.Tables[0];
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

        case "utpcAssetReceiptDetail":
          if (!isLoadedDetail)
          {
            this.LoadAssetReceiptDetail();
            this.isLoadedDetail = true;
          }
          break;
        case "utpcPostAssetReceipt":
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
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCAssetReceipt_Load", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        this.LoadMainData(dsSource.Tables[0]);
      }
      this.isLoadedDetail = false;
      this.isLoadedPostTransaction = false;
      this.LoadTabData();
      this.SetStatusControl();
      this.NeedToSave = false;
    }

    private bool CheckValid()
    {
      //check master
      if (udtAssetReceiptDate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày chứng từ không được để trống!!!");
        udtAssetReceiptDate.Focus();
        return false;
      }
      if (ucbSupplier.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Nhà cung cấp không được để trống!!!");
        ucbSupplier.Focus();
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
        if (this.actionCode == 1)
        {
          if (row.Cells["AssetPid"].Value.ToString().Length <= 0)
          {
            row.Cells["AssetPid"].Appearance.BackColor = Color.Yellow;
            WindowUtinity.ShowMessageErrorFromText("Mã TS không được để trống.");
            row.Selected = true;
            ugdData.ActiveRowScrollRegion.FirstRow = row;
            return false;
          }
        }
        else
        {
          if (row.Cells["MaterialCode"].Value.ToString().Length <= 0)
          {
            row.Cells["MaterialCode"].Appearance.BackColor = Color.Yellow;
            WindowUtinity.ShowMessageErrorFromText("Mã SP không được để trống.");
            row.Selected = true;
            ugdData.ActiveRowScrollRegion.FirstRow = row;
            return false;
          }
        }

        if (DBConvert.ParseDouble(row.Cells["OriginalAmount"].Value) <= 0)
        {
          row.Cells["OriginalAmount"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Nguyên giá không được để trống và phải lớn hơn 0.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        if (DBConvert.ParseInt(row.Cells["DepreciationMonth"].Value) < 0)
        {
          row.Cells["DepreciationMonth"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Số tháng khấu hao không được để trống.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }        

        if (DBConvert.ParseDouble(row.Cells["DepreciationAmount"].Value) > DBConvert.ParseDouble(row.Cells["OriginalAmount"].Value))
        {
          row.Cells["DepreciationAmount"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Giá trị khấu hao không được lớn hơn nguyên giá.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        //AssetType
        if (DBConvert.ParseInt(row.Cells["AssetType"].Value) < 0)
        {
          row.Cells["AssetType"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Loại tài sản không được để trống.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }


      }
      return true;
    }

    private bool SaveMain()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[10];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      }
      if (txtAssetReceiptDesc.Text.Trim().Length > 0)
      {
        inputParam[1] = new SqlDBParameter("@ReceiptDesc", SqlDbType.NVarChar, txtAssetReceiptDesc.Text.Trim().ToString());
      }
      inputParam[2] = new SqlDBParameter("@ReceiptDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtAssetReceiptDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      inputParam[3] = new SqlDBParameter("@SupplierPid", SqlDbType.BigInt, ucbSupplier.Value);
      inputParam[4] = new SqlDBParameter("@CurrencyPid", SqlDbType.Int, DBConvert.ParseInt(ucbCurrency.Value));
      inputParam[5] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, uneExchangeRate.Value);
      inputParam[6] = new SqlDBParameter("@ActionCode", SqlDbType.Int, this.actionCode);
      inputParam[7] = new SqlDBParameter("@Status", SqlDbType.Bit, chkConfirm.Checked ? 1 : 0);
      inputParam[8] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParam[9] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCAssetReceiptMaster_Save", inputParam, outputParam);
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
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCAssetReceipt_Delete", deleteParam, outputParam);
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
          SqlDBParameter[] inputParam = new SqlDBParameter[17];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
          }
          inputParam[1] = new SqlDBParameter("@AssetReceiptPid", SqlDbType.BigInt, this.viewPid);
          if (DBConvert.ParseInt(row["AccountPid"].ToString()) >= 0)
          {
            inputParam[2] = new SqlDBParameter("@AccountPid", SqlDbType.Int, DBConvert.ParseInt(row["AccountPid"].ToString()));
          }
          if (row["MaterialCode"].ToString().Trim().Length > 0)
          {
            inputParam[3] = new SqlDBParameter("@MaterialCode", SqlDbType.VarChar, row["MaterialCode"].ToString());
          }
          if (DBConvert.ParseInt(row["UsedEmployeePid"].ToString()) > 0)
          {
            inputParam[4] = new SqlDBParameter("@UsedEmployeePid", SqlDbType.Int, DBConvert.ParseInt(row["UsedEmployeePid"].ToString()));
          }
          if (DBConvert.ParseInt(row["DepartmentPid"].ToString()) > 0)
          {
            inputParam[5] = new SqlDBParameter("@DepartmentPid", SqlDbType.Int, DBConvert.ParseInt(row["DepartmentPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["UnCostConstructionPid"].ToString()) > 0)
          {
            inputParam[6] = new SqlDBParameter("@UnCostConstructionPid", SqlDbType.Int, DBConvert.ParseInt(row["UnCostConstructionPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["WarehousePid"].ToString()) > 0)
          {
            inputParam[7] = new SqlDBParameter("@WarehousePid", SqlDbType.Int, DBConvert.ParseInt(row["WarehousePid"].ToString()));
          }
          if (row["BatchNo"].ToString().Trim().Length > 0)
          {
            inputParam[8] = new SqlDBParameter("@BatchNo", SqlDbType.VarChar, row["BatchNo"].ToString());
          }
          inputParam[9] = new SqlDBParameter("@OriginalAmount", SqlDbType.Float, DBConvert.ParseDouble(row["OriginalAmount"].ToString()));
          inputParam[10] = new SqlDBParameter("@Qty", SqlDbType.Int, DBConvert.ParseInt(row["Qty"].ToString()));
          inputParam[11] = new SqlDBParameter("@DepreciationMonth", SqlDbType.Int, DBConvert.ParseInt(row["DepreciationMonth"].ToString()));
          if (DBConvert.ParseDouble(row["DepreciationAmount"]) > 0)
          {
            inputParam[12] = new SqlDBParameter("@DepreciationAmount", SqlDbType.Float, DBConvert.ParseDouble(row["DepreciationAmount"].ToString()));
          }
          if (DBConvert.ParseInt(row["AssetType"].ToString()) > 0)
          {
            inputParam[13] = new SqlDBParameter("@AssetType", SqlDbType.Int, DBConvert.ParseInt(row["AssetType"].ToString()));
          }
          inputParam[14] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          if (DBConvert.ParseLong(row["AssetPid"]) > 0)
          {
            inputParam[15] = new SqlDBParameter("@AssetPid", SqlDbType.BigInt, DBConvert.ParseLong(row["AssetPid"]));
          }
          if (DBConvert.ParseInt(row["PositionPid"].ToString()) > 0)
          {
            inputParam[16] = new SqlDBParameter("@PositionPid", SqlDbType.Int, DBConvert.ParseInt(row["PositionPid"].ToString()));
          }
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCAssetReceiptDetail_Save", inputParam, outputParam);
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
              DBParameter[] inputParam = new DBParameter[2];
              inputParam[0] = new DBParameter("@ReceiptPid", DbType.Int64, this.viewPid);
              inputParam[1] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };

              DataTable dtAssetList = DataBaseAccess.SearchStoreProcedureDataTable("spACCAssetReceipt_Confirm", inputParam, outputParam);
              if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
              {
                success = false;
              }
              else
              {
                Utility.LoadUltraCombo(ucbAsset, dtAssetList, "Pid", "AssetCode", false, new string[] { "Pid", "DepartmentPid", "UsedEmployeePid", "AccountPid", "AssetType", "DepreciationMonth" });
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
      btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);

      this.SetBlankForTextOfButton(this);
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
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      }
      else
      {
        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      }
      e.Layout.Bands[0].Columns["DepartmentPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["UsedEmployeePid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["AccountPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["WarehousePid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["PositionPid"].Hidden = true;

      // Set caption column
      e.Layout.Bands[0].Columns["DepartmentPid"].Header.Caption = "Phòng ban";
      e.Layout.Bands[0].Columns["UsedEmployeePid"].Header.Caption = "Người sử dụng";
      e.Layout.Bands[0].Columns["AssetPid"].Header.Caption = "Mã TSCĐ";
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Mã sản phẩm";
      e.Layout.Bands[0].Columns["MaterialName"].Header.Caption = "Tên sản phẩm";
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].Header.Caption = "CP XDCBDD";
      e.Layout.Bands[0].Columns["OriginalAmount"].Header.Caption = "Nguyên giá";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Số lượng";
      e.Layout.Bands[0].Columns["WarehousePid"].Header.Caption = "Kho";
      e.Layout.Bands[0].Columns["PositionName"].Header.Caption = "Vị trí";
      e.Layout.Bands[0].Columns["BatchNo"].Header.Caption = "Mã lô";
      e.Layout.Bands[0].Columns["DepreciationMonth"].Header.Caption = "Số tháng khấu hao";
      e.Layout.Bands[0].Columns["DepreciationAmount"].Header.Caption = "Giá trị khấu hao";
      e.Layout.Bands[0].Columns["AssetType"].Header.Caption = "Loại tài sản";
      e.Layout.Bands[0].Columns["AccountPid"].Header.Caption = "Tài khoản";

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["DepartmentPid"].ValueList = ucbddDepartment;
      e.Layout.Bands[0].Columns["UsedEmployeePid"].ValueList = ucbddEmployee;
      e.Layout.Bands[0].Columns["AssetPid"].ValueList = ucbAsset;
      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = ucbddMaterial;
      e.Layout.Bands[0].Columns["WarehousePid"].ValueList = ucbddWarehouse;
      e.Layout.Bands[0].Columns["AssetType"].ValueList = ucbddAssetType;
      e.Layout.Bands[0].Columns["AccountPid"].ValueList = ucbddAccount;
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].ValueList = ucbddUnCostConstruction;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["DepartmentPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["DepartmentPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["UsedEmployeePid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["UsedEmployeePid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["AccountPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["AccountPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["WarehousePid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["WarehousePid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["MaterialCode"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["MaterialCode"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["AssetType"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["AssetType"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["AssetPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["AssetPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      if (this.actionCode == 1) //tang moi
      {
        e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["WarehousePid"].CellActivation = Activation.ActivateOnly;
      }
      else
      {
        e.Layout.Bands[0].Columns["AssetPid"].CellActivation = Activation.ActivateOnly;
      }


      //e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalAmount"], SummaryPosition.UseSummaryPositionColumn);
      //e.Layout.Bands[0].Summaries[0].DisplayFormat = "Tổng = {0:#,##0.##}";
      //e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalAmountExchange"], SummaryPosition.UseSummaryPositionColumn);
      //e.Layout.Bands[0].Summaries[1].DisplayFormat = "Tổng = {0:#,##0.##}";
      //e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";

      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      //e.Layout.Bands[0].Columns["SubAmountExchange"].CellActivation = Activation.ActivateOnly;
      //e.Layout.Bands[0].Columns["TotalAmountExchange"].CellActivation = Activation.ActivateOnly;
    }

    private void ugdData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      this.NeedToSave = true;      
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      
      switch (columnName)
      {
        case "assetpid":
          if (ucbAsset.SelectedRow != null)
          {
            this.isLoadingAsset = true;
            e.Cell.Row.Cells["MaterialName"].Value = ucbAsset.SelectedRow.Cells["AssetName"].Value;
            e.Cell.Row.Cells["DepartmentPid"].Value = ucbAsset.SelectedRow.Cells["DepartmentPid"].Value;
            e.Cell.Row.Cells["UsedEmployeePid"].Value = ucbAsset.SelectedRow.Cells["UsedEmployeePid"].Value;
            e.Cell.Row.Cells["DepreciationMonth"].Value = ucbAsset.SelectedRow.Cells["DepreciationMonth"].Value;
            e.Cell.Row.Cells["AccountPid"].Value = ucbAsset.SelectedRow.Cells["AccountPid"].Value;
            e.Cell.Row.Cells["AssetType"].Value = ucbAsset.SelectedRow.Cells["AssetType"].Value;
            this.isLoadingAsset = false;
          }
          break;
        case "materialcode":
          if (!isLoadingAsset && ucbddMaterial.SelectedRow != null)
          {
            e.Cell.Row.Cells["MaterialName"].Value = ucbddMaterial.SelectedRow.Cells["MaterialName"].Value.ToString();
            int warehousePid = DBConvert.ParseInt(ucbddMaterial.SelectedRow.Cells["IDKho"].Value);
            if (warehousePid > 0)
            {
              e.Cell.Row.Cells["WarehousePid"].Value = warehousePid;
            }
            else
            {
              e.Cell.Row.Cells["WarehousePid"].Value = DBNull.Value;
            }
            e.Cell.Row.Cells["BatchNo"].Value = ucbddMaterial.SelectedRow.Cells["BatchNo"].Value;
            e.Cell.Row.Cells["PositionName"].Value = ucbddMaterial.SelectedRow.Cells["PositionName"].Value;
            e.Cell.Row.Cells["PositionPid"].Value = ucbddMaterial.SelectedRow.Cells["PositionPid"].Value;
          }
          break;
        case "assettype":
          if (!isLoadingAsset)
          {
            e.Cell.Row.Cells["AccountPid"].Value = DBConvert.ParseInt(ucbddAssetType.SelectedRow.Cells["AccountPid"].Value);
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
        case "subamount":
          if (DBConvert.ParseDouble(value) < 0)
          {
            WindowUtinity.ShowMessageErrorFromText("Số tiền không được nhỏ hơn 0.");
          }
          e.Cancel = true;
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

    private void ugdData_AfterRowInsert(object sender, RowEventArgs e)
    {
      //read only
      if (ucbCurrency.Value != null)
      {
        ucbCurrency.ReadOnly = true;
      }
      if (uneExchangeRate.Value != null)
      {
        uneExchangeRate.ReadOnly = true;
      }
      e.Row.Cells["Qty"].Value = 1;
    }

    private void utcDetail_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
    {
      this.LoadTabData();
    }

    private void ugdData_AfterRowsDeleted(object sender, EventArgs e)
    {
      if (ugdData.Rows.Count == 0)
      {

      }
    }

    private void ucbSupplier_ValueChanged(object sender, EventArgs e)
    {
      string taxCode = string.Empty;
      string address = string.Empty;
      if (ucbSupplier.SelectedRow != null)
      {        
        string commandText = string.Format("SELECT TaxNo, [Address] FROM TblPURSupplierInfo WHERE Pid = {0}", ucbSupplier.Value);
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if(dtSource != null && dtSource.Rows.Count > 0)
        {
          taxCode = dtSource.Rows[0]["TaxNo"].ToString();
          address = dtSource.Rows[0]["Address"].ToString();
        }  
      }
      txtSupplierTaxCode.Text = taxCode;
      txtSupplierAddress.Text = address;
    }
    #endregion Event
  }
}