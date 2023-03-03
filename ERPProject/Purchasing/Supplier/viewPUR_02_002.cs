/*
  Author      : Nguyen Thanh Binh
  Date        : 19-02-2023
  Description : Supplier information.
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
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPUR_02_002 : MainUserControl
  {
    #region Field
    public long supplierPid = long.MinValue;
    private bool confirm = false;
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    private bool isManager = false;
    private int status = 0;
    #endregion Field

    #region Init

    public viewPUR_02_002()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_02_002_Load(object sender, EventArgs e)
    {
      // Check User Is Manager ?
      this.isManager = this.CheckPurchaseManager(SharedObject.UserInfo.UserPid);

      // Load Debit
      LoadComboDebit();
      // Load Introduce Person
      LoadComboIntroducePerson();
      // Load Person In Charge
      this.LoadComboPersonInCharge();
      // Load Sex
      this.LoadSex();
      // Load TradeCommodity
      this.LoadComboTradeCommodity();
      this.LoadComboCountry();
      this.LoadComboCurrency();
      // Load Data
      this.LoadData();
    }
    #endregion Init

    #region Event
    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSupplierContact_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ContactName"].Header.Caption = "Tên người liên hệ";
      e.Layout.Bands[0].Columns["ContactPostion"].Header.Caption = "Vị trí";
      e.Layout.Bands[0].Columns["ContactMobile"].Header.Caption = "Số điện thoại";
      e.Layout.Bands[0].Columns["ContactEmail"].Header.Caption = "Email";
      e.Layout.Bands[0].Columns["Sex"].Header.Caption = "Giới tính";
      e.Layout.Bands[0].Columns["Sex"].ValueList = ultraDDSex;
      if (!this.confirm)
      {
        e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      }
      else
      {
        e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    ///  Save Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      // Check Invalid
      bool check = ValidationInput();
      if (check)
      {
        // Save Data
        bool resultSave = this.SaveData();
        if (!resultSave)
        {
          WindowUtinity.ShowMessageError("ERR0005");
        }
        else
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          btnDelete.Enabled = true;
        }
        // Load Lai Data
        this.LoadData();
      }
    }

    /// <summary>
    /// Delete Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      long outputValue = long.MinValue;
      DialogResult confirm;
      confirm = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0007", "Supplier Information");
      if (confirm == DialogResult.No)
      {
        return;
      }
      if (this.supplierPid != long.MinValue)
      {
        DBParameter[] inputParamDelete = new DBParameter[1];
        inputParamDelete[0] = new DBParameter("@Pid", DbType.Int64, this.supplierPid);

        DBParameter[] OutputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };
        DataBaseAccess.ExecuteStoreProcedure("spPURSupplierInfo_Delete", inputParamDelete, OutputParamDelete);
        outputValue = DBConvert.ParseLong(OutputParamDelete[0].Value.ToString());
        if (outputValue <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0004");
        }
        else
        {
          WindowUtinity.ShowMessageSuccess("MSG0002");
          this.CloseTab();
        }
      }
    }

    /// <summary>
    /// Before Row Deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSupplierContact_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletingPid.Add(pid);
        }
      }
    }

    /// <summary>
    /// After Row Deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSupplierContact_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
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
    #endregion Event

    #region Function
    /// <summary>
    /// Check Purchase Manager
    /// </summary>
    /// <param name="userPid"></param>
    /// <returns></returns>
    private bool CheckPurchaseManager(int userPid)
    {
      string commandText = string.Format("SELECT Manager FROM VHRDDepartmentInfo WHERE Code = 'PUR' AND Manager = {0}", userPid);
      DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCheck != null && dtCheck.Rows.Count > 0)
      {
        return true;
      }
      return false;
    }
    /// <summary>
    /// Load Sex
    /// </summary>
    private void LoadSex()
    {
      DataTable dtSex = new DataTable();
      dtSex.Columns.Add("Value", typeof(System.Int32));
      dtSex.Columns.Add("Text", typeof(System.String));
      DataRow row = dtSex.NewRow();
      row[0] = 0;
      row[1] = "Male";
      dtSex.Rows.Add(row);
      row = dtSex.NewRow();
      row[0] = 1;
      row[1] = "Female";
      dtSex.Rows.Add(row);
      ultraDDSex.DataSource = dtSex;
      ultraDDSex.ValueMember = "Value";
      ultraDDSex.DisplayMember = "Text";
      ultraDDSex.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDSex.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
    }
    /// <summary>
    /// Load UltraCombo Trade Type
    /// </summary>
    private void LoadComboTradeType()
    {
      string commandText = string.Format(@"SELECT Code, Value + ISNULL(' - ' + Description, '') Name FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", ConstantClass.GROUP_PUR_TRADETYPE);
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ucbCurrency.DataSource = dtSource;
      ucbCurrency.DisplayMember = "Name";
      ucbCurrency.ValueMember = "Code";
      ucbCurrency.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ucbCurrency.DisplayLayout.Bands[0].Columns["Name"].Width = 450;
      ucbCurrency.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Trade Commodity
    /// </summary>
    private void LoadComboTradeCommodity()
    {
      string commandText = string.Empty;
      commandText += " SELECT Code, Value ";
      commandText += " FROM TblBOMCodeMaster ";
      commandText += " WHERE [Group] = 7014 ";
      commandText += " 	AND DeleteFlag = 0 ";
      commandText += " ORDER BY Sort ";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbTradeCommodity, dtSource, "Code", "Value", false, "Code");
    }

    /// <summary>
    /// Load UltraCombo Country
    /// </summary>
    private void LoadComboCountry()
    {
      string commandText = string.Format(@"SELECT Pid Code, NationVN [Value], CountryCode
                                      FROM TblCSDNation
                                      ORDER BY NationVN");
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbCountry, dtSource, "Code", "Value", false, new string[] { "Code", "CountryCode" });
    }

    /// <summary>
    /// Load UltraCombo Currency
    /// </summary>
    private void LoadComboCurrency()
    {
      string commandText = string.Format(@"SELECT Pid Code, Code [Value] 
                                    FROM TblPURCurrencyInfo");
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbCurrency, dtSource, "Code", "Value", false, "Code");
    }

    /// <summary>
    /// Load UltraCombo Person In Charge
    /// </summary>
    private void LoadComboPersonInCharge()
    {
      string commandText = "SELECT Pid, CONVERT(varchar, Pid) +' - '+ EmpName AS EmpName  FROM VHRMEmployee WHERE Department = 'PUR'";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultPersonInCharge.DataSource = dtSource;
      ultPersonInCharge.DisplayMember = "EmpName";
      ultPersonInCharge.ValueMember = "Pid";
      ultPersonInCharge.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultPersonInCharge.DisplayLayout.Bands[0].Columns["EmpName"].Width = 450;
      ultPersonInCharge.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Introduce Person
    /// </summary>
    private void LoadComboIntroducePerson()
    {
      string commandText = "SELECT Pid, CONVERT(varchar, Pid) +' - '+ EmpName AS EmpName  FROM VHRMEmployee";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultIntroducePerson.DataSource = dtSource;
      ultIntroducePerson.DisplayMember = "EmpName";
      ultIntroducePerson.ValueMember = "Pid";
      ultIntroducePerson.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultIntroducePerson.DisplayLayout.Bands[0].Columns["EmpName"].Width = 450;
      ultIntroducePerson.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Debit
    /// </summary>
    private void LoadComboDebit()
    {
      string commandText = string.Format("SELECT Pid, TermCode, TermName, (TermCode + ' | ' + TermName) DisplayText FROM VGRNPaymentTermForSup ORDER BY TermCode");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbPaymentTerm, dtSource, "Pid", "DisplayText", false, new string[] { "Pid", "DisplayText" });
      ucbPaymentTerm.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      if (this.status == 0)
      {
        txtSupplierCode.ReadOnly = false;
        txtEnglishName.ReadOnly = false;
        txtVietnameseName.ReadOnly = false;
        btnSave.Enabled = true;
      }
      else if (this.status == 1)
      {
        if (this.isManager)
        {
          txtSupplierCode.ReadOnly = false;
          txtEnglishName.ReadOnly = false;
          txtVietnameseName.ReadOnly = false;
          btnSave.Enabled = true;
          btnDelete.Enabled = false;
          chkConfirm.Enabled = false;
        }
        else
        {
          txtSupplierCode.ReadOnly = true;
          txtEnglishName.ReadOnly = true;
          txtVietnameseName.ReadOnly = true;
          btnSave.Enabled = true;
          chkConfirm.Enabled = false;
          btnDelete.Enabled = false;
        }
      }
      else if (this.status == 2)
      {
        txtSupplierCode.ReadOnly = true;
        txtEnglishName.ReadOnly = true;
        txtVietnameseName.ReadOnly = true;
        btnSave.Enabled = true;
        btnDelete.Enabled = false;
        chkConfirm.Enabled = false;
      }
      else if (this.status == 3)
      {
        txtSupplierCode.ReadOnly = true;
        txtEnglishName.ReadOnly = true;
        txtVietnameseName.ReadOnly = true;
        btnSave.Enabled = false;
        btnDelete.Enabled = false;
        chkConfirm.Enabled = false;
      }
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.supplierPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPURSupplierInformation_Select", inputParam);
      DataTable dtSupplier = dsSource.Tables[0];
      if (dtSupplier.Rows.Count > 0)
      {
        DataRow row = dtSupplier.Rows[0];

        txtSupplierCode.Text = row["SupplierCode"].ToString();
        txtEnglishName.Text = row["EnglishName"].ToString();
        txtVietnameseName.Text = row["VietnameseName"].ToString();
        txtBankName.Text = row["BankName"].ToString();
        txtBankAccount.Text = row["BankAccount"].ToString();
        txtTaxNo.Text = row["TaxNo"].ToString();
        txtAddress.Text = row["SupAddress"].ToString();
        txtWebsite.Text = row["Website"].ToString();
        txtPhoneNo.Text = row["PhoneNo"].ToString();
        txtFaxNo.Text = row["FaxNo"].ToString();
        if (DBConvert.ParseInt(row["CountryPid"].ToString()) > 0)
        {
          ucbCountry.Value = DBConvert.ParseInt(row["CountryPid"].ToString());
        }
        if (DBConvert.ParseInt(row["CurrencyPid"].ToString()) > 0)
        {
          ucbCurrency.Value = DBConvert.ParseInt(row["CurrencyPid"].ToString());
        }
        if (DBConvert.ParseInt(row["IntroducePerson"].ToString()) > 0)
        {
          ultIntroducePerson.Value = DBConvert.ParseInt(row["IntroducePerson"].ToString());
        }
        if (DBConvert.ParseInt(row["PersonInCharge"].ToString()) > 0)
        {
          ultPersonInCharge.Value = DBConvert.ParseInt(row["PersonInCharge"].ToString());
        }
        if (DBConvert.ParseInt(row["Debit"].ToString()) > 0)
        {
          ucbPaymentTerm.Value = DBConvert.ParseInt(row["Debit"].ToString());
        }

        if (DBConvert.ParseInt(row["TradeCommodity"].ToString()) > 0)
        {
          this.ucbTradeCommodity.Value = DBConvert.ParseInt(row["TradeCommodity"].ToString());
        }

        this.status = DBConvert.ParseInt(row["Confirm"].ToString());
        if (this.status == 1 || this.status == 2)
        {
          chkConfirm.Checked = true;
        }
        else
        {
          chkConfirm.Checked = false;
        }
      }
      // Load Detail
      ultSupplierContact.DataSource = dsSource.Tables[1];
      // Set Status Control
      this.SetStatusControl();
    }

    /// <summary>
    /// Save Info
    /// </summary>
    /// <returns></returns>
    private bool SaveInfo()
    {
      DBParameter[] inputParam = new DBParameter[20];
      string text = string.Empty;
      string storeName = string.Empty;

      if (this.supplierPid == long.MinValue)
      {
        storeName = "spPURSupplierInformation_Insert";
        inputParam[15] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      }
      else
      {
        storeName = "spPURSupplierInformation_Update";
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.supplierPid);
        inputParam[15] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      }

      // 1. English Name
      text = txtEnglishName.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[1] = new DBParameter("@EnglishName", DbType.AnsiString, 256, text);
      }
      // 2. Vietnamese Name
      text = txtVietnameseName.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[2] = new DBParameter("@VietnameseName", DbType.String, 256, text);
      }
      if (ucbCountry.Value != null)
      {
        inputParam[3] = new DBParameter("@CountryPid", DbType.Int32, DBConvert.ParseInt(ucbCountry.Value.ToString()));
      }
      text = txtAddress.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[4] = new DBParameter("@Address", DbType.String, 512, text);
      }
      if (ucbCurrency.Value != null)
      {
        inputParam[5] = new DBParameter("@CurrencyPid", DbType.Int32, DBConvert.ParseInt(ucbCurrency.Value.ToString()));
      }
      text = txtWebsite.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[6] = new DBParameter("@Website", DbType.AnsiString, 128, text);
      }
      text = txtTaxNo.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[7] = new DBParameter("@TaxNo", DbType.AnsiString, 32, text);
      }
      text = txtBankName.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[8] = new DBParameter("@BankName", DbType.String, 256, text);
      }
      text = txtBankAccount.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[9] = new DBParameter("@BankAccount", DbType.AnsiString, 32, text);
      }

      if (ultPersonInCharge.Value != null)
      {
        inputParam[10] = new DBParameter("@PersonInChange", DbType.Int32, DBConvert.ParseInt(ultPersonInCharge.Value.ToString()));

      }

      if (ultIntroducePerson.Value != null)
      {
        inputParam[11] = new DBParameter("@IntroducePerson", DbType.Int32, DBConvert.ParseInt(ultIntroducePerson.Value.ToString()));
      }
      if (ucbPaymentTerm.Value != null)
      {
        inputParam[12] = new DBParameter("@Debit", DbType.Int32, DBConvert.ParseInt(ucbPaymentTerm.Value.ToString()));
      }
      if (chkConfirm.Checked)
      {
        inputParam[13] = new DBParameter("@Confirm", DbType.Int32, 2);
      }
      else
      {
        inputParam[13] = new DBParameter("@Confirm", DbType.Int32, 0);
      }
      if (txtSupplierCode.Text.Trim().Length > 0)
      {
        inputParam[14] = new DBParameter("@SupplierCode", DbType.AnsiString, 16, txtSupplierCode.Text);
      }
      if (txtPhoneNo.Text.Trim().Length > 0)
      {
        inputParam[16] = new DBParameter("@PhoneNo", DbType.AnsiString, 256, txtPhoneNo.Text);
      }
      if (txtFaxNo.Text.Trim().Length > 0)
      {
        inputParam[17] = new DBParameter("@FaxNo", DbType.AnsiString, 256, txtFaxNo.Text);
      }
      inputParam[19] = new DBParameter("@TradeCommodity", DbType.Int32, DBConvert.ParseInt(this.ucbTradeCommodity.Value.ToString()));

      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@SupplierPid", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      this.supplierPid = result;
      if (result <= 0)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Save Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      // 1.1 Delete
      foreach (long pid in this.listDeletedPid)
      {
        DBParameter[] inputParamDelete = new DBParameter[1];
        inputParamDelete[0] = new DBParameter("@Pid", DbType.Int64, pid);

        DBParameter[] OutputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };
        DataBaseAccess.ExecuteStoreProcedure("spPURSupplierContactPerson_Delete", inputParamDelete, OutputParamDelete);
        long outputValueDelete = DBConvert.ParseLong(OutputParamDelete[0].Value.ToString());
        if (outputValueDelete <= 0)
        {
          return false;
        }
      }
      //1.2 Save Detail
      string storeName = string.Empty;
      for (int i = 0; i < ultSupplierContact.Rows.Count; i++)
      {
        UltraGridRow row = ultSupplierContact.Rows[i];
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        DBParameter[] inputParam = new DBParameter[6];
        if (pid != long.MinValue)
        {
          storeName = "spPURSupplierContactPerson_Update";
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
        }
        else
        {
          storeName = "spPURSupplierContactPerson_Insert";
          inputParam[0] = new DBParameter("@SupplierPid", DbType.Int64, this.supplierPid);
        }

        // 1. Contact Name
        inputParam[1] = new DBParameter("@Name", DbType.String, row.Cells["ContactName"].Value.ToString().Trim());
        // 2. Contact Postion
        string text = row.Cells["ContactPostion"].Value.ToString().Trim();
        if (text.Length > 0)
        {
          inputParam[2] = new DBParameter("@Position", DbType.String, 128, text);
        }
        // 3. Contact Mobile
        text = row.Cells["ContactMobile"].Value.ToString().Trim();
        if (text.Length > 0)
        {
          inputParam[3] = new DBParameter("@Mobile", DbType.AnsiString, 128, text);
        }
        // 4. Contact Email
        text = row.Cells["ContactEmail"].Value.ToString().Trim();
        if (text.Length > 0)
        {
          inputParam[4] = new DBParameter("@Email", DbType.AnsiString, 256, text);
        }
        // 5. Sex
        int sex = DBConvert.ParseInt(row.Cells["Sex"].Value.ToString());
        if (sex != int.MinValue)
        {
          inputParam[5] = new DBParameter("@Sex", DbType.Int32, sex);
        }

        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
        long outputvalue = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (outputvalue <= 0)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool success = this.SaveInfo();
      if (success)
      {
        success = this.SaveDetail();
      }
      else
      {
        success = false;
      }
      return success;
    }

    /// <summary>
    ///  Check not null
    /// </summary>
    private bool ValidationInput()
    {
      string text = string.Empty;
      text = txtSupplierCode.Text;
      if (text.Length == 0 || text.Trim().Length > 16)
      {
        WindowUtinity.ShowMessageError("WRN0013", "Mã NCC");
        return false;
      }
      else
      {
        string commandText = string.Empty;
        if (this.supplierPid == long.MinValue)
        {
          commandText = " SELECT * FROM TblPURSupplierInfo WHERE DeleteFlg = 0 AND SupplierCode = '" + this.txtSupplierCode.Text.Trim() + "'";
        }
        else
        {
          commandText = " SELECT * FROM TblPURSupplierInfo WHERE DeleteFlg = 0 AND SupplierCode = '" + this.txtSupplierCode.Text.Trim() + "' AND Pid != " + this.supplierPid;
        }
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck.Rows.Count > 0)
        {
          WindowUtinity.ShowMessageErrorFromText("Mã NCC đã tồn tại trong hệ thống.");
          return false;
        }
      }

      text = txtEnglishName.Text.Trim();
      if (text.Length == 0)
      {
        WindowUtinity.ShowMessageError("WRN0013", "Tên tiếng anh");
        return false;
      }

      text = txtVietnameseName.Text.Trim();
      if (text.Length == 0)
      {
        WindowUtinity.ShowMessageError("WRN0013", "Tên tiếng việt");
        return false;
      }

      text = txtTaxNo.Text.Trim();
      if (text.Length == 0)
      {
        WindowUtinity.ShowMessageError("WRN0013", "Mã số thuế");
        return false;
      }
      else
      {
        string commandText = string.Empty;
        if (this.supplierPid == long.MinValue)
        {
          commandText = " SELECT * FROM TblPURSupplierInfo WHERE DeleteFlg = 0 AND TaxNo = '" + this.txtTaxNo.Text.Trim() + "'";
        }
        else
        {
          commandText = " SELECT * FROM TblPURSupplierInfo WHERE DeleteFlg = 0 AND TaxNo = '" + this.txtTaxNo.Text.Trim() + "' AND Pid != " + this.supplierPid;
        }
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck.Rows.Count > 0)
        {
          WindowUtinity.ShowMessageErrorFromText("Mã số thuế đã tồn tại trong hệ thóng.");
          return false;
        }
      }

      //if (ultPersonInCharge.Text.Length > 0)
      //{
      //  if (ultPersonInCharge.Value == null)
      //  {
      //    WindowUtinity.ShowMessageError("WRN0013", "Person In Charge");
      //    return false;
      //  }
      //}

      if (ucbCountry.Text.Length == 0)
      {
        WindowUtinity.ShowMessageError("WRN0013", "Quốc gia");
        return false;
      }

      if (ucbCurrency.Text.Length == 0)
      {
        WindowUtinity.ShowMessageError("WRN0013", "Tiền tệ");
        return false;
      }
      

      if (this.ucbTradeCommodity.Value == null
          || DBConvert.ParseInt(this.ucbTradeCommodity.Value.ToString()) == int.MinValue)
      {
        WindowUtinity.ShowMessageError("WRN0013", "Ngành hàng");
        return false;
      }

      if (this.ucbPaymentTerm.Value == null
          || DBConvert.ParseInt(this.ucbPaymentTerm.Value.ToString()) == int.MinValue)
      {
        WindowUtinity.ShowMessageError("WRN0013", "Điều khoản thanh toán");
        return false;
      }

      // Check Detail
      for (int i = 0; i < ultSupplierContact.Rows.Count; i++)
      {
        UltraGridRow row = ultSupplierContact.Rows[i];
        if (row.Cells["ContactName"].Text.Length == 0)
        {
          WindowUtinity.ShowMessageError("WRN0013", "Tên người liên hệ");
          return false;
        }
      }
      return true;
    }
    #endregion Function

    private void ucbCountry_ValueChanged(object sender, EventArgs e)
    {
      if(ucbCountry.SelectedRow != null)
      {
        if(ucbCountry.SelectedRow.Cells["CountryCode"].Value.ToString() == "VN")
        {
          ucbCurrency.Value = 1;
          ucbCurrency.ReadOnly = true;
        }
        else
        {
          ucbCurrency.ReadOnly = false;
        }  
      }  
    }
  }
}
