/*
 * Author       :  
 * CreateDate   : 08/04/2011
 * Description  : Insert / Update PR Detail
 */
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_03_002 : DaiCo.Shared.MainUserControl
  {
    #region Field
    public long prDetailPid = long.MinValue;
    public string prNo = string.Empty;
    public DataTable dtPrDetailInfo = new DataTable();
    private Boolean flag = false;
    private bool isPurchaseManager = false;
    private bool isUpdate = false;
    private bool loadData = false;
    private double amountExcept = double.MinValue;
    #endregion Field

    #region Init
    public viewPUR_03_002()
    {
      InitializeComponent();
      drpRequestDate.FormatString = ConstantClass.FORMAT_DATETIME;
    }

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_03_002_Load(object sender, EventArgs e)
    {
      //this.Text = this.Text.ToString() + " | " + SharedObject.UserInfo.UserName + " | " + SharedObject.UserInfo.LoginDate;

      // Load Materials
      this.LoadComboMaterials();

      // Load Currency 
      this.LoadComboCurrency();

      // Load Local/Import
      this.LoadComboLocalImport();

      // Load UrgentLevel
      this.LoadComboUrgentLevel();

      // Load ProjectCode
      this.LoadComboProjectCode();

      // Load Group In Charge
      this.LoadComboStaffGroup();

      // Check User
      this.CheckUser();

      // Load Data
      this.LoadData();
      this.loadData = true;
    }

    /// <summary>
    /// Check User
    /// </summary>
    private void CheckUser()
    {
      string commandText = "SELECT Manager FROM VHRDDepartmentInfo WHERE CODE = 'PUR'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dt != null && dt.Rows.Count > 0)
      {
        if (SharedObject.UserInfo.UserPid == DBConvert.ParseInt(dt.Rows[0]["Manager"].ToString()))
        {
          this.isPurchaseManager = true;
        }
      }
    }

    /// <summary>
    /// Update PR Schedule Total Money
    /// </summary>
    private bool UpdateTotalMoney(string pr, double amount)
    {
      // Input
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@PRNo", DbType.AnsiString, 16, pr);
      input[1] = new DBParameter("@Amount", DbType.Double, amount);
      // Output
      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, 0);
      // Exec
      DataBaseAccess.ExecuteStoreProcedure("spPURUpdatePRTotalAmount", input);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// LoadData
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@PRDetailPid", DbType.Int64, this.prDetailPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPURPRInfomationByPRDetailPid_Select", inputParam);
      dtPrDetailInfo = dsSource.Tables[0];

      if (dtPrDetailInfo.Rows.Count > 0)
      {
        DataRow row = dtPrDetailInfo.Rows[0];
        this.txtPRNo.Text = row["PRNo"].ToString().Trim();
        this.prNo = this.txtPRNo.Text;
        this.ultMaterials.Value = row["MaterialCode"].ToString().Trim();
        this.txtQty.Text = row["Quantity"].ToString().Trim();

        double number = DBConvert.ParseDouble(row["Price"].ToString());
        string numberRead = this.NumericFormat(number, 2);
        this.txtPrice.Text = numberRead;

        this.ultCurrency.Value = row["CurrencyPid"].ToString().Trim();
        this.txtExchangeRate.Text = row["ExchangeRate"].ToString().Trim();

        this.drpRequestDate.Value = DBConvert.ParseDateTime(row["RequestDate"].ToString().Trim(), DaiCo.Shared.Utility.ConstantClass.FORMAT_DATETIME);

        this.ultUrgentLevel.Value = row["Urgent"].ToString().Trim();
        this.txtExpectedBrand.Text = row["ExpectedBrand"].ToString().Trim();
        this.txtVat.Text = row["VAT"].ToString().Trim();
        this.ultLocalImport.Value = row["Imported"].ToString().Trim();
        this.ultProjectCode.Value = row["ProjectPid"].ToString().Trim();
        this.ultGroupInCharge.Value = row["GroupInCharge"].ToString().Trim();
        this.txtRemark.Text = row["Remark"].ToString().Trim();

        if (this.isPurchaseManager == true)
        {
          this.ultGroupInCharge.Enabled = true;
        }
        else
        {
          this.ultGroupInCharge.Enabled = false;
        }

        this.ultMaterials.Enabled = false;
        this.btnSaveContinue.Enabled = false;
        double price = (txtPrice.Text.Trim().Length == 0) ? 0 : DBConvert.ParseDouble(txtPrice.Text);
        double amount = price * DBConvert.ParseDouble(txtQty.Text) * DBConvert.ParseDouble(txtExchangeRate.Text) * -1;
        this.amountExcept = -amount;
        this.UpdateTotalMoney(txtPRNo.Text.Trim(), amount);
      }
      else
      {
        this.txtPRNo.Text = this.prNo;
        this.ultMaterials.Value = string.Empty;
        this.txtQty.Text = string.Empty;
        this.txtPrice.Text = string.Empty;

        this.ultCurrency.Text = "VND - 1";
        this.drpRequestDate.Value = DateTime.Today;
        this.ultUrgentLevel.Value = string.Empty;
        this.txtExpectedBrand.Text = string.Empty;
        this.txtVat.Text = string.Empty;
        this.ultLocalImport.Value = string.Empty;
        this.ultProjectCode.Value = string.Empty;
        this.ultGroupInCharge.Value = string.Empty;
        this.txtRemark.Text = string.Empty;
      }
    }

    /// <summary>
    /// Load UltraCombo Staff Group
    /// </summary>
    private void LoadComboStaffGroup()
    {
      string commandText = "SELECT Pid, GroupName FROM TblPURStaffGroup WHERE DeleteFlg = 0 ORDER BY Pid";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultGroupInCharge.DataSource = dtSource;
      ultGroupInCharge.DisplayMember = "GroupName";
      ultGroupInCharge.ValueMember = "Pid";
      ultGroupInCharge.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultGroupInCharge.DisplayLayout.Bands[0].Columns["GroupName"].Width = 250;
      ultGroupInCharge.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo ProjectCode
    /// </summary>
    private void LoadComboProjectCode()
    {
      string commandText = string.Format(@" SELECT  Pid Code, ProjectCode Name 
                                            FROM    TblPURPRProjectCode 
                                            WHERE   ISNULL(Finished, 0) = 0 AND ISNULL([Status], 0) = 1");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultProjectCode.DataSource = dtSource;
      ultProjectCode.DisplayMember = "Name";
      ultProjectCode.ValueMember = "Code";
      ultProjectCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultProjectCode.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultProjectCode.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Urgent Level
    /// </summary>
    private void LoadComboUrgentLevel()
    {
      string commandText = "SELECT [Code], [Value] Name FROM TblBOMCodeMaster WHERE [Group] = 7007 AND DeleteFlag = 0 ORDER BY Sort";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultUrgentLevel.DataSource = dtSource;
      ultUrgentLevel.DisplayMember = "Name";
      ultUrgentLevel.ValueMember = "Code";
      ultUrgentLevel.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultUrgentLevel.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultUrgentLevel.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Local / Import
    /// </summary>
    private void LoadComboLocalImport()
    {
      string commandText = "SELECT [Code], [Value] Name FROM TblBOMCodeMaster WHERE [Group] = 7006 AND DeleteFlag = 0 ORDER BY Sort";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultLocalImport.DataSource = dtSource;
      ultLocalImport.DisplayMember = "Name";
      ultLocalImport.ValueMember = "Code";
      ultLocalImport.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultLocalImport.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultLocalImport.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Currency
    /// </summary>
    private void LoadComboCurrency()
    {
      string commandText = "SELECT Pid, [Code] + ' - ' + CAST(CurrentExchangeRate AS VARCHAR) Name FROM TblPURCurrencyInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCurrency.DataSource = dtSource;
      ultCurrency.DisplayMember = "Name";
      ultCurrency.ValueMember = "Pid";
      ultCurrency.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCurrency.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultCurrency.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Materials
    /// </summary>
    private void LoadComboMaterials()
    {
      string commandText = "SELECT MaterialCode, MaterialCode + ' - ' + NameEN Name FROM TblGNRMaterialInformation WHERE [Status] >= 1 AND [Status] != 3";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultMaterials.DataSource = dtSource;
      ultMaterials.DisplayMember = "Name";
      ultMaterials.ValueMember = "MaterialCode";
      ultMaterials.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultMaterials.DisplayLayout.Bands[0].Columns["Name"].Width = 750;
      ultMaterials.DisplayLayout.Bands[0].Columns["MaterialCode"].Hidden = true;
    }
    #endregion Init

    #region Event

    /// <summary>
    /// TextBox Qty Text Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtQty_TextChanged(object sender, EventArgs e)
    {
      if (this.loadData)
      {
        this.isUpdate = true;
      }
    }

    /// <summary>
    /// TextBox Price Text Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtPrice_TextChanged(object sender, EventArgs e)
    {
      if (this.loadData)
      {
        this.isUpdate = true;
      }
    }

    /// <summary>
    /// Currency Value Changed ==> Get Exchange Rate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCurrency_ValueChanged(object sender, EventArgs e)
    {
      if (ultCurrency.SelectedRow != null)
      {
        if (this.loadData)
        {
          this.isUpdate = true;
        }
        long currency = DBConvert.ParseLong(ultCurrency.SelectedRow.Cells["Pid"].Value.ToString().Trim());
        string commandText = "SELECT CurrentExchangeRate FROM TblPURCurrencyInfo WHERE Pid = " + currency;
        DataTable dt = DataBaseAccess.SearchCommandText(commandText).Tables[0];
        if (dt.Rows.Count > 0)
        {
          this.txtExchangeRate.Text = dt.Rows[0]["CurrentExchangeRate"].ToString();
        }
        else
        {
          this.txtExchangeRate.Text = string.Empty;
        }
      }
      else
      {
        this.txtExchangeRate.Text = string.Empty;
      }
    }

    /// <summary>
    /// Material Value Change ==> Group In Charge Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultMaterials_ValueChanged(object sender, EventArgs e)
    {
      if (ultMaterials.SelectedRow != null)
      {
        string material = ultMaterials.SelectedRow.Cells["MaterialCode"].Value.ToString().Trim();
        string commandText = "SELECT GroupIncharge FROM TblGNRMaterialInformation WHERE MaterialCode = '" + material + "'";
        DataTable dt = DataBaseAccess.SearchCommandText(commandText).Tables[0];
        if (dt.Rows.Count > 0)
        {
          this.ultGroupInCharge.Value = dt.Rows[0]["GroupIncharge"].ToString();
        }
        else
        {
          this.ultGroupInCharge.Value = string.Empty;
        }
      }
      else
      {
        this.ultGroupInCharge.Value = string.Empty;
      }
    }

    /// <summary>
    /// Save & Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveClose_Click(object sender, EventArgs e)
    {
      // Save
      this.MainSave();

      if (flag == false)
      {
        // Close
        this.CloseTab();
      }

      this.flag = false;
    }

    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.UpdateTotalMoney(this.prNo, this.amountExcept);
      this.CloseTab();
    }

    /// <summary>
    /// Save & Continue
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveContinue_Click(object sender, EventArgs e)
    {
      // Save
      this.MainSave();

      if (flag == false)
      {
        this.prDetailPid = long.MinValue;

        // Load Data
        this.LoadData();
      }

      this.flag = false;
    }

    /// <summary>
    /// Format Price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtPrice_Leave(object sender, EventArgs e)
    {
      double number = DBConvert.ParseDouble(txtPrice.Text);
      string numberRead = this.NumericFormat(number, 2);
      txtPrice.Text = numberRead;
    }
    #endregion Event

    #region LoadData
    /// <summary>
    /// Format Numeric
    /// </summary>
    /// <param name="number"></param>
    /// <param name="phanLe"></param>
    /// <returns></returns>
    private string NumericFormat(double number, int phanLe)
    {
      if (number == double.MinValue)
      {
        return string.Empty;
      }
      if (phanLe < 0)
      {
        return number.ToString();
      }
      System.Globalization.NumberFormatInfo formatInfo = new System.Globalization.NumberFormatInfo();
      double t = Math.Truncate(number);
      formatInfo.NumberDecimalDigits = phanLe;
      return number.ToString("N", formatInfo);
    }

    /// <summary>
    /// Main Save Data
    /// </summary>
    /// <returns></returns>
    private double SaveData()
    {
      double amount = double.MinValue;
      int pid = this.SavePrDetailInfo(out amount);
      if (pid == int.MinValue)
      {
        return double.MinValue;
      }

      return amount;
    }

    /// <summary>
    /// Save PR Detail Information
    /// </summary>
    /// <returns></returns>
    private int SavePrDetailInfo(out double amount)
    {
      int result = int.MinValue;
      string storeName = string.Empty;
      // Insert
      if (this.prDetailPid == long.MinValue)
      {
        storeName = "spPURPRDetail_Insert";
        DBParameter[] inputParamInsert = new DBParameter[14];

        inputParamInsert[0] = new DBParameter("@PrNo", DbType.AnsiString, 16, this.prNo);
        inputParamInsert[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, ControlUtility.GetSelectedValueUltraCombobox(ultMaterials));
        inputParamInsert[2] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(this.txtQty.Text));
        if (DBConvert.ParseDouble(this.txtPrice.Text) != double.MinValue)
        {
          inputParamInsert[3] = new DBParameter("@Price", DbType.Double, DBConvert.ParseDouble(this.txtPrice.Text));
        }
        inputParamInsert[4] = new DBParameter("@CurrencyPid", DbType.Int64, DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultCurrency)));
        inputParamInsert[5] = new DBParameter("@ExchangeRate", DbType.Double, DBConvert.ParseDouble(this.txtExchangeRate.Text));
        inputParamInsert[6] = new DBParameter("@Urgent", DbType.Int32, DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultUrgentLevel)));
        DateTime date = new DateTime(this.drpRequestDate.DateTime.Year, this.drpRequestDate.DateTime.Month, this.drpRequestDate.DateTime.Day);
        inputParamInsert[7] = new DBParameter("@RequestDate", DbType.DateTime, date);

        if (this.txtExpectedBrand.Text.Length > 0)
        {
          inputParamInsert[8] = new DBParameter("@ExpectedBrand", DbType.AnsiString, 512, this.txtExpectedBrand.Text);
        }

        if (DBConvert.ParseDouble(this.txtVat.Text) != double.MinValue)
        {
          inputParamInsert[9] = new DBParameter("@VAT", DbType.Double, DBConvert.ParseDouble(this.txtVat.Text));
        }

        inputParamInsert[10] = new DBParameter("@Imported", DbType.Int32, DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultLocalImport)));

        if (DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultProjectCode)) != int.MinValue)
        {
          inputParamInsert[11] = new DBParameter("@ProjectPid", DbType.Int32, DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultProjectCode)));
        }

        inputParamInsert[12] = new DBParameter("@GroupPid", DbType.Int32, DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultGroupInCharge)));
        inputParamInsert[13] = new DBParameter("@Remark", DbType.AnsiString, 512, this.txtRemark.Text);

        DBParameter[] outputParamInsert = new DBParameter[] { new DBParameter("@Result", DbType.Int32, int.MinValue) };

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamInsert, outputParamInsert);
        result = DBConvert.ParseInt(outputParamInsert[0].Value.ToString());
      }
      // Update
      else
      {
        storeName = "spPURPRDetail_Update";
        DBParameter[] inputParamUpdate = new DBParameter[14];

        inputParamUpdate[0] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, ControlUtility.GetSelectedValueUltraCombobox(ultMaterials));
        inputParamUpdate[1] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(this.txtQty.Text));
        if (DBConvert.ParseDouble(this.txtPrice.Text) != double.MinValue)
        {
          inputParamUpdate[2] = new DBParameter("@Price", DbType.Double, DBConvert.ParseDouble(this.txtPrice.Text));
        }
        inputParamUpdate[3] = new DBParameter("@CurrencyPid", DbType.Int64, DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultCurrency)));
        inputParamUpdate[4] = new DBParameter("@ExchangeRate", DbType.Double, DBConvert.ParseDouble(this.txtExchangeRate.Text));
        inputParamUpdate[5] = new DBParameter("@Urgent", DbType.Int32, DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultUrgentLevel)));
        inputParamUpdate[6] = new DBParameter("@RequestDate", DbType.DateTime, this.drpRequestDate.DateTime.Date);

        if (this.txtExpectedBrand.Text.Length > 0)
        {
          inputParamUpdate[7] = new DBParameter("@ExpectedBrand", DbType.AnsiString, 512, this.txtExpectedBrand.Text);
        }

        if (DBConvert.ParseDouble(this.txtVat.Text) != double.MinValue)
        {
          inputParamUpdate[8] = new DBParameter("@VAT", DbType.Double, DBConvert.ParseDouble(this.txtVat.Text));
        }

        inputParamUpdate[9] = new DBParameter("@Imported", DbType.Int32, DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultLocalImport)));

        if (DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultProjectCode)) != int.MinValue)
        {
          inputParamUpdate[10] = new DBParameter("@ProjectPid", DbType.Int32, DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultProjectCode)));
        }

        inputParamUpdate[11] = new DBParameter("@GroupPid", DbType.Int32, DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultGroupInCharge)));
        inputParamUpdate[12] = new DBParameter("@Remark", DbType.AnsiString, 512, this.txtRemark.Text);
        inputParamUpdate[13] = new DBParameter("@PRDetailPid", DbType.Int64, this.prDetailPid);

        DBParameter[] outputParamUpdate = new DBParameter[] { new DBParameter("@Result", DbType.Int32, int.MinValue) };

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamUpdate, outputParamUpdate);
        result = DBConvert.ParseInt(outputParamUpdate[0].Value.ToString());
      }
      double price = DBConvert.ParseDouble(this.txtPrice.Text);
      if (price == double.MinValue)
      {
        price = 0;
      }
      amount = price * DBConvert.ParseDouble(this.txtExchangeRate.Text) * DBConvert.ParseDouble(this.txtQty.Text);
      return result;
    }

    /// <summary>
    /// Main Save
    /// </summary>
    private void MainSave()
    {
      string message = string.Empty;

      bool success = this.CheckValidPrDetailInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        this.flag = true;
        return;
      }

      double amount = double.MinValue;
      amount = this.SaveData();
      if (amount != double.MinValue)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        if (this.isUpdate)
        {
          this.UpdateTotalMoney(this.prNo, amount);
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
    }

    /// <summary>
    /// Check PR Detail Insert / Update
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidPrDetailInfo(out string message)
    {
      message = string.Empty;

      //// Check Valid PRNo
      //string text = string.Empty;
      //text = txtPRNo.Text;
      //if (text.Length == 0 || text.Trim().Length > 16)
      //{
      //  WindowUtinity.ShowMessageError("WRN0013", "PRNo");
      //  return false;
      //}
      //else
      //{
      //  string commandText = string.Empty;
      //  if (this.prNo.Length == 0)
      //  {
      //    commandText = "SELECT PRNo FROM TblPURPRInformation WHERE PRNo = '" + this.txtPRNo.Text.Trim() + "'";
      //  }
      //  else
      //  {
      //    commandText = " SELECT PRNo FROM TblPURPRInformation WHERE PRNo = '" + this.txtPRNo.Text.Trim() + "' AND PRNo != " + this.prNo;
      //  }
      //  DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
      //  if (dtCheck.Rows.Count > 0)
      //  {
      //    WindowUtinity.ShowMessageErrorFromText("PRNo is existed on system");
      //    return false;
      //  }
      //}

      // Materials
      string materials = ControlUtility.GetSelectedValueUltraCombobox(ultMaterials);

      if (materials.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Materials");
        return false;
      }

      // Currency
      int currency = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultCurrency));
      if (currency == int.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Currency");
        return false;
      }

      // Quantity
      double qty = DBConvert.ParseDouble(this.txtQty.Text);
      if (qty <= 0)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Quantity");
        return false;
      }

      // Local/Import
      int type = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultLocalImport));
      if (type == int.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Local/Import");
        return false;
      }

      // Urgent Level
      int urgentLevel = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultUrgentLevel));
      if (urgentLevel == int.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Urgent Level");
        return false;
      }

      // Group In Charge
      int groupInCharge = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultGroupInCharge));
      if (groupInCharge == int.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Group In Charge");
        return false;
      }

      // Request Date
      DateTime requestDate = DBConvert.ParseDateTime(this.drpRequestDate.Value);
      if (requestDate == DateTime.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Request Date");
        return false;
      }

      // Project Code
      if (ultProjectCode.Text.Length > 0)
      {
        int projectCode = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultProjectCode));
        if (projectCode == int.MinValue)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Project Code");
          return false;
        }
      }

      // VAT
      if (this.txtVat.Text.Length > 0)
      {
        double vat = DBConvert.ParseDouble(this.txtVat.Text);
        if (vat == double.MinValue)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The VAT");
          return false;
        }
      }

      // Price
      if (this.txtPrice.Text.Length > 0)
      {
        double price = DBConvert.ParseDouble(this.txtPrice.Text);
        if (price == double.MinValue)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Price");
          return false;
        }
      }
      return true;
    }
    #endregion LoadData
  }
}

