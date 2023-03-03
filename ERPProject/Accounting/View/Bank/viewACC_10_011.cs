/*
  Author      : Nguyen Binh An
  Date        : 1/05/2021
  Description : Loan Agreement detail
  Standard Form: view_SaveMasterDetail
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_10_011 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    #endregion Field

    #region Init
    public viewACC_10_011()
    {
      InitializeComponent();
    }

    private void viewACC_10_011_Load(object sender, EventArgs e)
    {
      Utility.Format_UltraNumericEditor(tlpForm);
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
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCLoanAgreement_Init");
      Utility.LoadUltraCombo(ucbCompanyBank, dsInit.Tables[0], "Pid", "BankName", false, "Pid");
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[1], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate" });

      // Set Language
      //this.SetLanguage();
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtContractCode.Text = dtMain.Rows[0]["ContractCode"].ToString();
        udtEffectDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["EffectDate"]);
        udtEndDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["EndDate"]);
        ucbCompanyBank.Value = DBConvert.ParseInt(dtMain.Rows[0]["CompanyBankPid"]);
        ucbCurrency.Value = DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"]);

        txtContractDesc.Text = dtMain.Rows[0]["ContractDesc"].ToString();
        uneTotalAmount.Value = dtMain.Rows[0]["TotalAmount"];
        uneReceiptAmount.Value = dtMain.Rows[0]["ReceiptAmount"];
        uneRemainAmount.Value = dtMain.Rows[0]["RemainAmount"];

      }

    }

    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCLoanAgreement_Load", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        this.LoadMainData(dsSource.Tables[0]);
      }

      this.NeedToSave = false;
    }

    private bool CheckValid()
    {
      //check master
      if (txtContractCode.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Số hợp đồng không được để trống!!!");
        txtContractCode.Focus();
        return false;
      }
      if (ucbCompanyBank.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Tài khoản ngân hàng không được để trống!!!");
        ucbCompanyBank.Focus();
        return false;
      }
      if (ucbCurrency.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Loại tiền tệ không được để trống!!!");
        ucbCurrency.Focus();
        return false;
      }
      return true;
    }

    private bool SaveMain()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[11];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      }

      if (txtContractCode.Text.Trim().Length > 0)
      {
        inputParam[1] = new SqlDBParameter("@ContractCode", SqlDbType.VarChar, txtContractCode.Text.Trim().ToString());
      }

      if (txtContractDesc.Text.Trim().Length > 0)
      {
        inputParam[2] = new SqlDBParameter("@ContractDesc", SqlDbType.NVarChar, txtContractDesc.Text.Trim().ToString());
      }

      inputParam[3] = new SqlDBParameter("@EffectDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtEffectDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));

      inputParam[4] = new SqlDBParameter("@EndDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtEndDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));

      if (ucbCompanyBank.SelectedRow != null)
      {
        inputParam[5] = new SqlDBParameter("@CompanyBankPid", SqlDbType.Int, DBConvert.ParseInt(ucbCompanyBank.Value));
      }

      inputParam[6] = new SqlDBParameter("@TotalAmount", SqlDbType.Float, uneTotalAmount.Value);

      inputParam[7] = new SqlDBParameter("@ReceiptAmount", SqlDbType.Float, uneReceiptAmount.Value);

      inputParam[8] = new SqlDBParameter("@RemainAmount", SqlDbType.Float, uneRemainAmount.Value);

      inputParam[9] = new SqlDBParameter("@CurrencyPid", SqlDbType.Int, DBConvert.ParseInt(ucbCurrency.Value));

      inputParam[10] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);

      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };

      SqlDataBaseAccess.ExecuteStoreProcedure("spACCLoanAgreement_Save", inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        this.viewPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
        return true;
      }
      return false;
    }

    private void SaveData()
    {
      if (this.CheckValid())
      {
        bool success = true;
        success = this.SaveMain();
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
      btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);

      this.SetBlankForTextOfButton(this);
    }


    #endregion Function

    #region Event


    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseForm();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
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
      if (ucbCompanyBank.SelectedRow != null)
      {
        //uneExchangeRate.Value = ucbCompanyBank.SelectedRow.Cells["ExchangeRate"].Value;
      }
    }
    #endregion Event
  }
}
