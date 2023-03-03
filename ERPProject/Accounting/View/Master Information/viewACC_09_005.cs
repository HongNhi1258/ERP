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
  public partial class viewACC_09_005 : MainUserControl
  {
    #region Field
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private IList listCashDeletedPid = new ArrayList();
    private bool isDuplicateProcess = false;
    private bool isDuplicateProcessBank = false;
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewACC_09_005).Assembly);
    #endregion Field

    #region Init
    public viewACC_09_005()
    {
      InitializeComponent();
    }

    private void view_SaveMasterDetail_Load(object sender, EventArgs e)
    {
      // Add ask before closing form even if user change data
      this.SetAutoAskSaveWhenCloseForm(uegCompanyInfo);
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
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spACCCompanyBank_InitData");
      //team
      Utility.LoadUltraCombo(ucboAccountCode, ds.Tables[0], "Value", "AccountCode", false, "Value");
      //employee
      Utility.LoadUltraCombo(ucboCurrency, ds.Tables[1], "Value", "Display", false, "Value");

      //team
      Utility.LoadUltraCombo(ucboAccountCash, ds.Tables[0], "Value", "AccountCode", false, "Value");
      //employee
      Utility.LoadUltraCombo(ucboCurrencyCash, ds.Tables[1], "Value", "Display", false, "Value");

      // Set Language
      this.SetLanguage();
    }


    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtAddress.Text = dtMain.Rows[0]["Address"].ToString();
        txtCompanyName.Text = dtMain.Rows[0]["FullName"].ToString();
        txtEmail.Text = dtMain.Rows[0]["Email"].ToString();
        txtFax.Text = dtMain.Rows[0]["Fax"].ToString();
        txtShortName.Text = dtMain.Rows[0]["ShortName"].ToString();
        txtTaxCode.Text = dtMain.Rows[0]["TaxCode"].ToString();
        txtTell.Text = dtMain.Rows[0]["Tel"].ToString();
        txtWebsite.Text = dtMain.Rows[0]["Website"].ToString();

      }
    }

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCCompanyInfo_Select");
      if (dsSource != null && dsSource.Tables.Count > 1)
      {
        this.LoadMainData(dsSource.Tables[0]);
        ugrdData.DataSource = dsSource.Tables[1];
        ugrCashFund.DataSource = dsSource.Tables[2];
      }

      this.NeedToSave = false;
    }

    //Check duplicate Bank
    private void CheckProcessDuplicateBank()
    {
      isDuplicateProcessBank = false;
      for (int k = 0; k < ugrdData.Rows.Count; k++)
      {
        UltraGridRow rowcurent = ugrdData.Rows[k];
        rowcurent.CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ugrdData.Rows.Count; i++)
      {

        UltraGridRow rowcurentA = ugrdData.Rows[i];
        string bankCode = rowcurentA.Cells["BankCode"].Value.ToString();
        string bankAccount = rowcurentA.Cells["BankAccount"].Value.ToString();
        if (bankCode.Length > 0)
        {
          for (int j = i + 1; j < ugrdData.Rows.Count; j++)
          {
            UltraGridRow rowcurentB = ugrdData.Rows[j];

            string bankCodeB = rowcurentB.Cells["BankCode"].Value.ToString();
            string bankAccountB = rowcurentB.Cells["BankAccount"].Value.ToString();
            if (bankCode == bankCodeB)
            {
              rowcurentA.CellAppearance.BackColor = Color.Yellow;
              rowcurentB.CellAppearance.BackColor = Color.Yellow;
              isDuplicateProcessBank = true;
            }
            if (bankAccount == bankAccountB)
            {
              rowcurentA.CellAppearance.BackColor = Color.Yellow;
              rowcurentB.CellAppearance.BackColor = Color.Yellow;
              isDuplicateProcessBank = true;
            }
          }
        }
      }
    }

    //Check duplicate Cash fund
    private void CheckProcessDuplicate()
    {
      isDuplicateProcess = false;
      for (int k = 0; k < ugrCashFund.Rows.Count; k++)
      {
        UltraGridRow rowcurent = ugrCashFund.Rows[k];
        rowcurent.CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ugrCashFund.Rows.Count; i++)
      {

        UltraGridRow rowcurentA = ugrCashFund.Rows[i];
        string seriesNo = rowcurentA.Cells["CashFundCode"].Value.ToString();
        if (seriesNo.Length > 0)
        {
          for (int j = i + 1; j < ugrCashFund.Rows.Count; j++)
          {
            UltraGridRow rowcurentB = ugrCashFund.Rows[j];

            string seriesNocom = rowcurentB.Cells["CashFundCode"].Value.ToString();
            if (seriesNo == seriesNocom)
            {
              rowcurentA.CellAppearance.BackColor = Color.Yellow;
              rowcurentB.CellAppearance.BackColor = Color.Yellow;
              isDuplicateProcess = true;
            }
          }
        }
      }
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      //1. Check text box
      if (txtCompanyName.Text.Length <= 0)
      {
        errorMessage = "Company Name";
        return false;
      }
      //2. Check data on view Bank
      for (int i = 0; i < ugrdData.Rows.Count; i++)
      {
        string bankCode = ugrdData.Rows[i].Cells["BankCode"].Value.ToString();
        string bankName = ugrdData.Rows[i].Cells["BankName"].Value.ToString();
        string bankAccount = ugrdData.Rows[i].Cells["BankAccount"].Value.ToString();
        int accountPidBank = DBConvert.ParseInt(ugrdData.Rows[i].Cells["AccountPid"].Value);
        int currencyPidBank = DBConvert.ParseInt(ugrdData.Rows[i].Cells["CurrencyPid"].Value);
        //Check bank Code
        if (bankCode.Length <= 0)
        {
          errorMessage = "Bank Code";
          ugrdData.Rows[i].Cells["BankCode"].Selected = true;
          ugrdData.ActiveRowScrollRegion.FirstRow = ugrdData.Rows[i];
          return false;
        }
        //End
        //Check bank name
        if (bankName.Length <= 0)
        {
          errorMessage = "Bank Name";
          ugrdData.Rows[i].Cells["BankName"].Selected = true;
          ugrdData.ActiveRowScrollRegion.FirstRow = ugrdData.Rows[i];
          return false;
        }
        //End
        //Check bank Account
        if (bankAccount.Length <= 0)
        {
          errorMessage = "Bank Account";
          ugrdData.Rows[i].Cells["BankAccount"].Selected = true;
          ugrdData.ActiveRowScrollRegion.FirstRow = ugrdData.Rows[i];
          return false;
        }
        //end
        //Check Account Code
        if (ugrdData.Rows[i].Cells["AccountPid"].Value.ToString().Length <= 0)
        {
          errorMessage = "Account Code";
          ugrdData.Rows[i].Cells["AccountPid"].Selected = true;
          ugrdData.ActiveRowScrollRegion.FirstRow = ugrdData.Rows[i];
          return false;
        }

        if (ugrdData.Rows[i].Cells["AccountPid"].Value.ToString().Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", accountPidBank);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Account Code  is not exists";
            ugrdData.Rows[i].Cells["AccountPid"].Selected = true;
            return false;
          }
        }
        //end
        //Check currency
        if (ugrdData.Rows[i].Cells["CurrencyPid"].Value.ToString().Length <= 0)
        {
          errorMessage = "Currency";
          ugrdData.Rows[i].Cells["CurrencyPid"].Selected = true;
          ugrdData.ActiveRowScrollRegion.FirstRow = ugrdData.Rows[i];
          return false;
        }

        if (ugrdData.Rows[i].Cells["CurrencyPid"].Value.ToString().Length > 0)
        {
          string cm2 = string.Format(@"SELECT Pid
		                                  FROM TblPURCurrencyInfo 
		                                  WHERE Pid ='{0}'", currencyPidBank);
          DataTable dSource2 = DataBaseAccess.SearchCommandTextDataTable(cm2);
          if (dSource2.Rows.Count <= 0)
          {
            errorMessage = "Currency  is not exists";
            ugrdData.Rows[i].Cells["CurrencyPid"].Selected = true;
            return false;
          }
        }
        //end
      }
      //Check duplicate Bank
      this.CheckProcessDuplicateBank();
      if (isDuplicateProcessBank)
      {
        errorMessage = "Row Bank duplicate";
        return false;
      }

      //3. Check data on view Cash
      for (int i = 0; i < ugrCashFund.Rows.Count; i++)
      {
        string cashFundCode = ugrCashFund.Rows[i].Cells["CashFundCode"].Value.ToString();
        string cashFundName = ugrCashFund.Rows[i].Cells["CashFundName"].Value.ToString();
        //string bankAccount = ugrCashFund.Rows[i].Cells["BankAccount"].Value.ToString();
        int accountPidCash = DBConvert.ParseInt(ugrCashFund.Rows[i].Cells["AccountCashPid"].Value);
        int currencyPidCash = DBConvert.ParseInt(ugrCashFund.Rows[i].Cells["CurrencyCashPid"].Value);
        //Check bank Code
        if (cashFundCode.Length <= 0)
        {
          errorMessage = "Cash Fund Code";
          ugrCashFund.Rows[i].Cells["CashFundCode"].Selected = true;
          ugrCashFund.ActiveRowScrollRegion.FirstRow = ugrCashFund.Rows[i];
          return false;
        }
        //End
        //Check bank name
        if (cashFundName.Length <= 0)
        {
          errorMessage = "Cash Fund Name";
          ugrCashFund.Rows[i].Cells["CashFundName"].Selected = true;
          ugrCashFund.ActiveRowScrollRegion.FirstRow = ugrCashFund.Rows[i];
          return false;
        }
        //End
        //Check Account Code
        if (ugrCashFund.Rows[i].Cells["AccountCashPid"].Value.ToString().Length <= 0)
        {
          errorMessage = "Account Code";
          ugrCashFund.Rows[i].Cells["AccountCashPid"].Selected = true;
          ugrCashFund.ActiveRowScrollRegion.FirstRow = ugrCashFund.Rows[i];
          return false;
        }

        if (ugrCashFund.Rows[i].Cells["AccountCashPid"].Value.ToString().Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", accountPidCash);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Account Code  is not exists";
            ugrCashFund.Rows[i].Cells["AccountCashPid"].Selected = true;
            return false;
          }
        }
        //end
        //Check currency
        if (ugrCashFund.Rows[i].Cells["CurrencyCashPid"].Value.ToString().Length <= 0)
        {
          errorMessage = "Currency";
          ugrCashFund.Rows[i].Cells["CurrencyCashPid"].Selected = true;
          ugrCashFund.ActiveRowScrollRegion.FirstRow = ugrCashFund.Rows[i];
          return false;
        }

        if (ugrCashFund.Rows[i].Cells["CurrencyCashPid"].Value.ToString().Length > 0)
        {
          string cm2 = string.Format(@"SELECT Pid
		                                  FROM TblPURCurrencyInfo 
		                                  WHERE Pid ='{0}'", currencyPidCash);
          DataTable dSource2 = DataBaseAccess.SearchCommandTextDataTable(cm2);
          if (dSource2.Rows.Count <= 0)
          {
            errorMessage = "Currency  is not exists";
            ugrCashFund.Rows[i].Cells["CurrencyCashPid"].Selected = true;
            return false;
          }
        }
        //end
      }

      //Check duplicate
      this.CheckProcessDuplicate();
      if (isDuplicateProcess)
      {
        errorMessage = "Row Cash Fund duplicate";
        return false;
      }
      return true;
    }

    private bool SaveMain()
    {
      string storeName = "spACCCompanyBank_Save";
      int paramNumber = 9;
      DBParameter[] inputParam = new DBParameter[paramNumber];
      inputParam[0] = new DBParameter("@FullName", DbType.String, 128, txtCompanyName.Text.ToString());
      if (txtShortName.Text.Length > 0)
      {
        inputParam[1] = new DBParameter("@ShortName", DbType.String, 128, txtShortName.Text.ToString());
      }
      if (txtAddress.Text.Length > 0)
      {
        inputParam[2] = new DBParameter("@Address", DbType.String, 256, txtAddress.Text.ToString());
      }
      if (txtEmail.Text.Length > 0)
      {
        inputParam[3] = new DBParameter("@Email", DbType.String, 50, txtEmail.Text.ToString());
      }
      if (txtTell.Text.Length > 0)
      {
        inputParam[4] = new DBParameter("@Tel", DbType.String, 50, txtTell.Text.ToString());
      }
      if (txtFax.Text.Length > 0)
      {
        inputParam[5] = new DBParameter("@Fax", DbType.String, 50, txtFax.Text.ToString());
      }
      if (txtWebsite.Text.Length > 0)
      {
        inputParam[6] = new DBParameter("@Website", DbType.String, 50, txtWebsite.Text.ToString());
      }
      if (txtTaxCode.Text.Length > 0)
      {
        inputParam[7] = new DBParameter("@TaxCode", DbType.String, 16, txtTaxCode.Text.ToString());
      }
      inputParam[8] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.String, "") };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if (outputParam[0].Value.ToString().Length > 0)
      {
        return true;
      }
      return false;
    }

    private bool SaveDetail()
    {
      bool success = true;
      // 1. Delete      
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("spACCCompanyBank_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ugrdData.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[10];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          if (row["BankCode"].ToString().Length > 0)
          {
            inputParam[1] = new DBParameter("@BankCode", DbType.String, row["BankCode"].ToString());
          }
          if (row["BankName"].ToString().Length > 0)
          {
            inputParam[2] = new DBParameter("@BankName", DbType.String, row["BankName"].ToString());
          }
          if (row["BankAccount"].ToString().Length > 0)
          {
            inputParam[3] = new DBParameter("@BankAccount", DbType.String, row["BankAccount"].ToString());
          }
          if (row["Address"].ToString().Length > 0)
          {
            inputParam[4] = new DBParameter("@Address", DbType.String, row["Address"].ToString());
          }
          if (row["SwiftCode"].ToString().Length > 0)
          {
            inputParam[5] = new DBParameter("@SwiftCode", DbType.String, row["SwiftCode"].ToString());
          }
          if (DBConvert.ParseInt(row["AccountPid"].ToString()) != int.MinValue)
          {
            inputParam[6] = new DBParameter("@AccountPid", DbType.Int32, DBConvert.ParseInt(row["AccountPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["CurrencyPid"].ToString()) != int.MinValue)
          {
            inputParam[7] = new DBParameter("@CurrencyPid", DbType.Int32, DBConvert.ParseInt(row["CurrencyPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["Default"].ToString()) == 1)
          {
            inputParam[8] = new DBParameter("@IsDefault", DbType.Int32, 1);
          }
          else
          {
            inputParam[8] = new DBParameter("@IsDefault", DbType.Int32, 0);
          }
          inputParam[9] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DataBaseAccess.ExecuteStoreProcedure("spACCCompanyBank_Edit", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }


    //Save Cash Fund Detail
    private bool SaveDetailCash()
    {
      bool success = true;
      // 1. Delete      
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listCashDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listCashDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("spACCCashFund_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ugrCashFund.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[7];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          if (row["CashFundCode"].ToString().Length > 0)
          {
            inputParam[1] = new DBParameter("@CashFundCode", DbType.String, row["CashFundCode"].ToString());
          }
          if (row["CashFundName"].ToString().Length > 0)
          {
            inputParam[2] = new DBParameter("@CashFundName", DbType.String, row["CashFundName"].ToString());
          }
          if (DBConvert.ParseInt(row["AccountCashPid"].ToString()) != int.MinValue)
          {
            inputParam[3] = new DBParameter("@AccountCashPid", DbType.Int32, DBConvert.ParseInt(row["AccountCashPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["CurrencyCashPid"].ToString()) != int.MinValue)
          {
            inputParam[4] = new DBParameter("@CurrencyCashPid", DbType.Int32, DBConvert.ParseInt(row["CurrencyCashPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["DefaultCash"].ToString()) == 1)
          {
            inputParam[5] = new DBParameter("@IsDefault", DbType.Int32, 1);
          }
          else
          {
            inputParam[5] = new DBParameter("@IsDefault", DbType.Int32, 0);
          }
          inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DataBaseAccess.ExecuteStoreProcedure("spACCCashFund_Edit", inputParam, outputParam);
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
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        if (this.SaveMain())
        {
          if (this.SaveDetail())
          {
            success = this.SaveDetailCash();
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
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
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
      lblCompanyName.Text = "Tên Công Ty";
      lblShortName.Text = "Tên Rút Gọn";
      lblAddress.Text = "Địa Chỉ";
      lblTel.Text = "Điện Thoại";
      lblFax.Text = "Số Fax";
      lblTaxCode.Text = "Mã Số Thuế";
      uegCompanyInfo.Text = "Thông Tin Công Ty";
      gpbBankInfo.Text = "Danh Sách Ngân Hàng";
      grbCashFund.Text = "Danh Sách Quỹ Tiền Mặt";

      this.SetBlankForTextOfButton(this);
    }
    #endregion Function

    #region Event
    private void ugrdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      ugrdData.SyncWithCurrencyManager = false;
      ugrdData.StyleSetName = "Excel2013";

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        }
      }
      for (int i = 0; i < e.Layout.Rows.Count; i++)
      {
        e.Layout.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

      // Set Column Style
      e.Layout.Bands[0].Columns["Default"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      ugrdData.DisplayLayout.Bands[0].Columns["Default"].DefaultCellValue = 0;
      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Set caption column      
      //e.Layout.Bands[0].Columns["CurrencyPid"].Header.Caption = "Currency";
      //e.Layout.Bands[0].Columns["BankCode"].Header.Caption = "Bank Code";
      //e.Layout.Bands[0].Columns["BankName"].Header.Caption = "Bank Name";
      //e.Layout.Bands[0].Columns["BankAccount"].Header.Caption = "Account No";
      //e.Layout.Bands[0].Columns["AccountPid"].Header.Caption = "Account Code";
      //e.Layout.Bands[0].Columns["SwiftCode"].Header.Caption = "Swift Code";      
      e.Layout.Bands[0].Columns["CurrencyPid"].Header.Caption = "Tiền Tệ";
      e.Layout.Bands[0].Columns["BankCode"].Header.Caption = "Mã Ngân Hàng";
      e.Layout.Bands[0].Columns["BankName"].Header.Caption = "Tên Ngân Hàng";
      e.Layout.Bands[0].Columns["Address"].Header.Caption = "Địa Chỉ";
      e.Layout.Bands[0].Columns["BankAccount"].Header.Caption = "Số Tài Khoản";
      e.Layout.Bands[0].Columns["AccountPid"].Header.Caption = "Tài Khoản";
      e.Layout.Bands[0].Columns["SwiftCode"].Header.Caption = "Mã Swift";
      e.Layout.Bands[0].Columns["Default"].Header.Caption = "Mặc Định";

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["AccountPid"].ValueList = ucboAccountCode;
      e.Layout.Bands[0].Columns["CurrencyPid"].ValueList = ucboCurrency;

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Set Width
      e.Layout.Bands[0].Columns["CurrencyPid"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["BankCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["BankAccount"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["Default"].MaxWidth = 80;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["CurrencyPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CurrencyPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["AccountPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["AccountPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

    }

    private void ugrdData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      if (columnName.Equals("default"))
      {
        try
        {
          int isDefault = DBConvert.ParseInt(e.Cell.Row.Cells["Default"].Value.ToString());
          if (isDefault == 1)
          {
            for (int i = 0; i <= ugrdData.Rows.Count; i++)
            {
              if (i != index)
              {
                ugrdData.Rows[i].Cells["Default"].Value = 0;
              }
            }
          }
        }
        catch { }
      }
      if (columnName.Equals("bankcode") || columnName.Equals("bankaccount"))
      {
        try
        {
          this.CheckProcessDuplicateBank();
        }
        catch { }

      }
      this.SetNeedToSave();
    }

    private void ugrdData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string colName = e.Cell.Column.ToString();
      //string value = e.NewValue.ToString();
      //switch (colName)
      //{
      //  case "Default":
      //   if (value == "1" )
      //    {
      //      //for (int i=0; i<= ugrdData.Rows.Count;i++)
      //      //{
      //      // ugrdData.Rows[i].Cells["Default"].Value= 0;
      //      //}
      //    }
      //    break;
      //  default:
      //    break;
      //}
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ugrdData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
      Utility.ExportToExcelWithDefaultPath(ugrdData, "Data");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugdData);
      }
    }

    private void ugrdData_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ugrdData.Selected.Rows.Count > 0 || ugrdData.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ugrdData, new Point(e.X, e.Y));
        }
      }
    }




    private void ugrCashFund_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      ugrCashFund.SyncWithCurrencyManager = false;
      ugrCashFund.StyleSetName = "Excel2013";

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        }
      }
      for (int i = 0; i < e.Layout.Rows.Count; i++)
      {
        e.Layout.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

      // Set Column Style
      e.Layout.Bands[0].Columns["DefaultCash"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      ugrCashFund.DisplayLayout.Bands[0].Columns["DefaultCash"].DefaultCellValue = 0;
      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Set caption column
      //e.Layout.Bands[0].Columns["AccountCashPid"].Header.Caption = "Account Code";
      //e.Layout.Bands[0].Columns["CurrencyCashPid"].Header.Caption = "Currency";
      //e.Layout.Bands[0].Columns["CashFundCode"].Header.Caption = "Cash Fund Code";
      //e.Layout.Bands[0].Columns["CashFundName"].Header.Caption = "Cas Fund Name";
      //e.Layout.Bands[0].Columns["DefaultCash"].Header.Caption = "Default";
      e.Layout.Bands[0].Columns["AccountCashPid"].Header.Caption = "Tài Khoản";
      e.Layout.Bands[0].Columns["CurrencyCashPid"].Header.Caption = "Tiền Tệ";
      e.Layout.Bands[0].Columns["CashFundCode"].Header.Caption = "Mã Quỹ TM";
      e.Layout.Bands[0].Columns["CashFundName"].Header.Caption = "Tên Quỹ TM";
      e.Layout.Bands[0].Columns["DefaultCash"].Header.Caption = "Mặc Định";

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["AccountCashPid"].ValueList = ucboAccountCash;
      e.Layout.Bands[0].Columns["CurrencyCashPid"].ValueList = ucboCurrencyCash;
      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Read only

      // ugrCashFund.DisplayLayout.Bands[0].Columns["CreateByCash"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      // ugrCashFund.DisplayLayout.Bands[0].Columns["CreateDateCash"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;


      // Set Width
      e.Layout.Bands[0].Columns["CurrencyCashPid"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["CashFundCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["CashFundName"].MinWidth = 130;
      e.Layout.Bands[0].Columns["DefaultCash"].MaxWidth = 80;
      //e.Layout.Bands[0].Columns["CreateDateCash"].MaxWidth = 110;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["AccountCashPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["AccountCashPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["CurrencyCashPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CurrencyCashPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
    }

    private void ugrCashFund_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
          this.listCashDeletedPid.Add(pid);
        }
      }
    }


    private void ucboAccountCode_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["AccountCode"].MaxWidth = 100;
    }


    private void ucboAccountCash_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["AccountCode"].MaxWidth = 100;
    }

    private void ugrCashFund_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      if (columnName.Equals("defaultcash"))
      {
        try
        {
          int isDefault = DBConvert.ParseInt(e.Cell.Row.Cells["DefaultCash"].Value.ToString());
          if (isDefault == 1)
          {
            for (int i = 0; i <= ugrdData.Rows.Count; i++)
            {
              if (i != index)
              {
                ugrdData.Rows[i].Cells["DefaultCash"].Value = 0;
              }
            }
          }
        }
        catch { }
      }
      if (columnName.Equals("cashfundcode"))
      {
        try
        {
          this.CheckProcessDuplicate();
        }
        catch { }

      }

      this.SetNeedToSave();
    }
    #endregion Event
  }
}
