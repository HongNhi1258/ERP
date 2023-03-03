/*
  Author  : Vo Van Duy Qui
  Email   : qui_it@daico-furniture.com
  Date    : 26-11-2010
  Update  : 16/03/2012 by Nguyen Van Tron
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using VBReport;
using System.Diagnostics;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_003 : MainUserControl
  {
    #region field
    private bool isImportFromExcel = false;
    #endregion field
    #region Init
    public viewCSD_03_003()
    {
      InitializeComponent();
      ultDetail.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(ultDetail_AfterCellUpdate);
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_03_003_Load(object sender, EventArgs e)
    {
      this.LoadInitData();
    }

    /// <summary>
    /// Load Init Data
    /// </summary>
    private void LoadInitData()
    {
      //1. Load UltraCBCategory
      string command = "SELECT Pid, Category FROM TblCSDCategory ORDER BY Category ASC";
      DataTable dtCategory = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(command);
      Shared.Utility.ControlUtility.LoadUltraCombo(ultraCBCategory, dtCategory, "Pid", "Category", false, "Pid");

      //2. Load UltraCBCollection
      command = @"SELECT Code, Value + ISNULL(' - ' + Description, '') Value FROM TblBOMCodeMaster WHERE [Group] = 2 And DeleteFlag = 0 ORDER BY Value";
      DataTable dtCollection = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(command);
      Shared.Utility.ControlUtility.LoadUltraCombo(ultraCBCollection, dtCollection, "Code", "Value", false, "Code");

      //3. Load Customer (distribute)      
      ControlUtility.LoadUltraCBCustomer(ultraCBCustomer);
      ultraCBCustomer.Value = 27;

      //4. Exhibition
      ControlUtility.LoadUltraCBExhibition(ultraCBExhibition);
    }
    #endregion Init

    #region function
    #region SaveData
    /// <summary>
    /// Save Data
    /// </summary>
    private void SaveData()
    {
      btnSave.Enabled = false;
      bool success = true;
      DataTable dtSource = (DataTable)ultDetail.DataSource;
      foreach (DataRow row in dtSource.Rows)
      {
        if ((row.RowState == DataRowState.Modified) || this.isImportFromExcel)
        {
          string itemCode = row["ItemCode"].ToString().Trim();

          //US
          int usActive = DBConvert.ParseInt(row["USActive"].ToString());
          int usQuickShip = DBConvert.ParseInt(row["USQuickShip"].ToString());
          int usSpecial = DBConvert.ParseInt(row["USSpecial"].ToString());
          int usDiscontinue = DBConvert.ParseInt(row["USDiscontinue"].ToString());
          int usOnHold = DBConvert.ParseInt(row["USOnHold"].ToString());
          int usSP_D = DBConvert.ParseInt(row["USSP_D"].ToString());
          int usST_D = DBConvert.ParseInt(row["USST_D"].ToString());
          int usST_SP = DBConvert.ParseInt(row["USST_SP"].ToString());
          int usST_R = DBConvert.ParseInt(row["USST_R"].ToString());
          int usST_R_D = DBConvert.ParseInt(row["USST_R_D"].ToString());
          int usST_R_SP = DBConvert.ParseInt(row["USST_R_SP"].ToString());

          //UK
          int ukActive = DBConvert.ParseInt(row["UKActive"].ToString());
          int ukQuickShip = DBConvert.ParseInt(row["UKQuickShip"].ToString());
          int ukSpecial = DBConvert.ParseInt(row["UKSpecial"].ToString());
          int ukDiscontinue = DBConvert.ParseInt(row["UKDiscontinue"].ToString());
          int ukOnHold = DBConvert.ParseInt(row["UKOnHold"].ToString());
          int ukSP_D = DBConvert.ParseInt(row["UKSP_D"].ToString());
          int ukST_D = DBConvert.ParseInt(row["UKST_D"].ToString());
          int ukST_SP = DBConvert.ParseInt(row["UKST_SP"].ToString());

          //IT
          int ITActive = DBConvert.ParseInt(row["ITActive"].ToString());
          int ITQuickShip = DBConvert.ParseInt(row["ITQuickShip"].ToString());
          int ITSpecial = DBConvert.ParseInt(row["ITSpecial"].ToString());
          int ITDiscontinue = DBConvert.ParseInt(row["ITDiscontinue"].ToString());
          int ITOnHold = DBConvert.ParseInt(row["ITOnHold"].ToString());
          int ITSP_D = DBConvert.ParseInt(row["ITSP_D"].ToString());
          int ITST_D = DBConvert.ParseInt(row["ITST_D"].ToString());
          int ITST_SP = DBConvert.ParseInt(row["ITST_SP"].ToString());

          //ME
          int MEActive = DBConvert.ParseInt(row["MEActive"].ToString());
          int MEQuickShip = DBConvert.ParseInt(row["MEQuickShip"].ToString());
          int MESpecial = DBConvert.ParseInt(row["MESpecial"].ToString());
          int MEDiscontinue = DBConvert.ParseInt(row["MEDiscontinue"].ToString());
          int MEOnHold = DBConvert.ParseInt(row["MEOnHold"].ToString());
          int MESP_D = DBConvert.ParseInt(row["MESP_D"].ToString());
          int MEST_D = DBConvert.ParseInt(row["MEST_D"].ToString());
          int MEST_SP = DBConvert.ParseInt(row["MEST_SP"].ToString());

          //RU
          int RUActive = DBConvert.ParseInt(row["RUActive"].ToString());
          int RUQuickShip = DBConvert.ParseInt(row["RUQuickShip"].ToString());
          int RUSpecial = DBConvert.ParseInt(row["RUSpecial"].ToString());
          int RUDiscontinue = DBConvert.ParseInt(row["RUDiscontinue"].ToString());
          int RUOnHold = DBConvert.ParseInt(row["RUOnHold"].ToString());
          int RUSP_D = DBConvert.ParseInt(row["RUSP_D"].ToString());
          int RUST_D = DBConvert.ParseInt(row["RUST_D"].ToString());
          int RUST_SP = DBConvert.ParseInt(row["RUST_SP"].ToString());

          //AU
          int AUActive = DBConvert.ParseInt(row["AUActive"].ToString());
          int AUQuickShip = DBConvert.ParseInt(row["AUQuickShip"].ToString());
          int AUSpecial = DBConvert.ParseInt(row["AUSpecial"].ToString());
          int AUDiscontinue = DBConvert.ParseInt(row["AUDiscontinue"].ToString());
          int AUOnHold = DBConvert.ParseInt(row["AUOnHold"].ToString());
          int AUSP_D = DBConvert.ParseInt(row["AUSP_D"].ToString());
          int AUST_D = DBConvert.ParseInt(row["AUST_D"].ToString());
          int AUST_SP = DBConvert.ParseInt(row["AUST_SP"].ToString());

          //CN
          int CNActive = DBConvert.ParseInt(row["CNActive"].ToString());
          int CNQuickShip = DBConvert.ParseInt(row["CNQuickShip"].ToString());
          int CNSpecial = DBConvert.ParseInt(row["CNSpecial"].ToString());
          int CNDiscontinue = DBConvert.ParseInt(row["CNDiscontinue"].ToString());
          int CNOnHold = DBConvert.ParseInt(row["CNOnHold"].ToString());
          int CNSP_D = DBConvert.ParseInt(row["CNSP_D"].ToString());
          int CNST_D = DBConvert.ParseInt(row["CNST_D"].ToString());
          int CNST_SP = DBConvert.ParseInt(row["CNST_SP"].ToString());

          //EU
          int EUActive = DBConvert.ParseInt(row["EUActive"].ToString());
          int EUQuickShip = DBConvert.ParseInt(row["EUQuickShip"].ToString());
          int EUSpecial = DBConvert.ParseInt(row["EUSpecial"].ToString());
          int EUDiscontinue = DBConvert.ParseInt(row["EUDiscontinue"].ToString());
          int EUOnHold = DBConvert.ParseInt(row["EUOnHold"].ToString());
          int EUSP_D = DBConvert.ParseInt(row["EUSP_D"].ToString());
          int EUST_D = DBConvert.ParseInt(row["EUST_D"].ToString());
          int EUST_SP = DBConvert.ParseInt(row["EUST_SP"].ToString());

          //Other
          int otherActive = DBConvert.ParseInt(row["OtherActive"].ToString());
          int otherDisContinue = DBConvert.ParseInt(row["OtherDisContinue"].ToString());
          int otherCustom = DBConvert.ParseInt(row["OtherCustom"].ToString());
          int otherOnHold = DBConvert.ParseInt(row["OtherOnHold"].ToString());

          DBParameter[] inputParams = new DBParameter[73];
          inputParams[0] = new DBParameter("@ItemCode", DbType.AnsiString, 18, itemCode);

          //US
          inputParams[1] = new DBParameter("@USActive", DbType.Int32, usActive);
          inputParams[2] = new DBParameter("@USQuickShip", DbType.Int32, usQuickShip);
          inputParams[3] = new DBParameter("@USSpecial", DbType.Int32, usSpecial);
          inputParams[4] = new DBParameter("@USDiscontinue", DbType.Int32, usDiscontinue);
          inputParams[5] = new DBParameter("@USOnHold", DbType.Int32, usOnHold);
          inputParams[6] = new DBParameter("@USSP_D", DbType.Int32, usSP_D);
          inputParams[7] = new DBParameter("@USST_D", DbType.Int32, usST_D);
          inputParams[8] = new DBParameter("@USST_SP", DbType.Int32, usST_SP);
          inputParams[70] = new DBParameter("@USST_R", DbType.Int32, usST_R);
          inputParams[71] = new DBParameter("@USST_R_D", DbType.Int32, usST_R_D);
          inputParams[72] = new DBParameter("@USST_R_SP", DbType.Int32, usST_R_SP);

          //UK
          inputParams[9] = new DBParameter("@UKActive", DbType.Int32, ukActive);
          inputParams[10] = new DBParameter("@UKQuickShip", DbType.Int32, ukQuickShip);
          inputParams[11] = new DBParameter("@UKSpecial", DbType.Int32, ukSpecial);
          inputParams[12] = new DBParameter("@UKDiscontinue", DbType.Int32, ukDiscontinue);
          inputParams[13] = new DBParameter("@UKOnHold", DbType.Int32, ukOnHold);
          inputParams[14] = new DBParameter("@UKSP_D", DbType.Int32, ukSP_D);
          inputParams[15] = new DBParameter("@UKST_D", DbType.Int32, ukST_D);
          inputParams[16] = new DBParameter("@UKST_SP", DbType.Int32, ukST_SP);

          //IT
          inputParams[17] = new DBParameter("@ITActive", DbType.Int32, ITActive);
          inputParams[18] = new DBParameter("@ITQuickShip", DbType.Int32, ITQuickShip);
          inputParams[19] = new DBParameter("@ITSpecial", DbType.Int32, ITSpecial);
          inputParams[20] = new DBParameter("@ITDiscontinue", DbType.Int32, ITDiscontinue);
          inputParams[21] = new DBParameter("@ITOnHold", DbType.Int32, ITOnHold);
          inputParams[22] = new DBParameter("@ITSP_D", DbType.Int32, ITSP_D);
          inputParams[23] = new DBParameter("@ITST_D", DbType.Int32, ITST_D);
          inputParams[24] = new DBParameter("@ITST_SP", DbType.Int32, ITST_SP);

          //ME
          inputParams[25] = new DBParameter("@MEActive", DbType.Int32, MEActive);
          inputParams[26] = new DBParameter("@MEQuickShip", DbType.Int32, MEQuickShip);
          inputParams[27] = new DBParameter("@MESpecial", DbType.Int32, MESpecial);
          inputParams[28] = new DBParameter("@MEDiscontinue", DbType.Int32, MEDiscontinue);
          inputParams[29] = new DBParameter("@MEOnHold", DbType.Int32, MEOnHold);
          inputParams[30] = new DBParameter("@MESP_D", DbType.Int32, MESP_D);
          inputParams[31] = new DBParameter("@MEST_D", DbType.Int32, MEST_D);
          inputParams[32] = new DBParameter("@MEST_SP", DbType.Int32, MEST_SP);

          //RU
          inputParams[33] = new DBParameter("@RUActive", DbType.Int32, RUActive);
          inputParams[34] = new DBParameter("@RUQuickShip", DbType.Int32, RUQuickShip);
          inputParams[35] = new DBParameter("@RUSpecial", DbType.Int32, RUSpecial);
          inputParams[36] = new DBParameter("@RUDiscontinue", DbType.Int32, RUDiscontinue);
          inputParams[37] = new DBParameter("@RUOnHold", DbType.Int32, RUOnHold);
          inputParams[38] = new DBParameter("@RUSP_D", DbType.Int32, RUSP_D);
          inputParams[39] = new DBParameter("@RUST_D", DbType.Int32, RUST_D);
          inputParams[40] = new DBParameter("@RUST_SP", DbType.Int32, RUST_SP);

          //AU
          inputParams[41] = new DBParameter("@AUActive", DbType.Int32, AUActive);
          inputParams[42] = new DBParameter("@AUQuickShip", DbType.Int32, AUQuickShip);
          inputParams[43] = new DBParameter("@AUSpecial", DbType.Int32, AUSpecial);
          inputParams[44] = new DBParameter("@AUDiscontinue", DbType.Int32, AUDiscontinue);
          inputParams[45] = new DBParameter("@AUOnHold", DbType.Int32, AUOnHold);
          inputParams[46] = new DBParameter("@AUSP_D", DbType.Int32, AUSP_D);
          inputParams[47] = new DBParameter("@AUST_D", DbType.Int32, AUST_D);
          inputParams[48] = new DBParameter("@AUST_SP", DbType.Int32, AUST_SP);

          //CN
          inputParams[49] = new DBParameter("@CNActive", DbType.Int32, CNActive);
          inputParams[50] = new DBParameter("@CNQuickShip", DbType.Int32, CNQuickShip);
          inputParams[51] = new DBParameter("@CNSpecial", DbType.Int32, CNSpecial);
          inputParams[52] = new DBParameter("@CNDiscontinue", DbType.Int32, CNDiscontinue);
          inputParams[53] = new DBParameter("@CNOnHold", DbType.Int32, CNOnHold);
          inputParams[54] = new DBParameter("@CNSP_D", DbType.Int32, CNSP_D);
          inputParams[55] = new DBParameter("@CNST_D", DbType.Int32, CNST_D);
          inputParams[56] = new DBParameter("@CNST_SP", DbType.Int32, CNST_SP);

          //EU
          inputParams[57] = new DBParameter("@EUActive", DbType.Int32, EUActive);
          inputParams[58] = new DBParameter("@EUQuickShip", DbType.Int32, EUQuickShip);
          inputParams[59] = new DBParameter("@EUSpecial", DbType.Int32, EUSpecial);
          inputParams[60] = new DBParameter("@EUDiscontinue", DbType.Int32, EUDiscontinue);
          inputParams[61] = new DBParameter("@EUOnHold", DbType.Int32, EUOnHold);
          inputParams[62] = new DBParameter("@EUSP_D", DbType.Int32, EUSP_D);
          inputParams[63] = new DBParameter("@EUST_D", DbType.Int32, EUST_D);
          inputParams[64] = new DBParameter("@EUST_SP", DbType.Int32, EUST_SP);

          //Other
          inputParams[65] = new DBParameter("@OtherActive", DbType.Int32, otherActive);
          inputParams[66] = new DBParameter("@OtherDisContinue", DbType.Int32, otherDisContinue);
          inputParams[67] = new DBParameter("@OtherCustom", DbType.Int32, otherCustom);
          inputParams[68] = new DBParameter("@OtherOnHold", DbType.Int32, otherOnHold);

          inputParams[69] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
         

          DBParameter[] outputParam = new DBParameter[]{new DBParameter("@Result", DbType.Int64, 0)};
          DataBaseAccess.ExecuteStoreProcedure("spCSDItemMasterController_Edit", inputParams, outputParam);
          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result == 0)
          {
            success = false;            
          }
        }
      }
      if (success)
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0001");
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("WRN0004");
      }
      this.NeedToSave = false;      
      this.Search();
      btnSave.Enabled = true;
    }

    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;
      this.isImportFromExcel = false;
      string itemCode = txtItemCode.Text.Trim();
      string name = txtName.Text.Trim();
      string saleCode = txtSaleCode.Text.Trim();      

      DBParameter[] inputParams = new DBParameter[7];
      if (itemCode.Length > 0)
      {
        inputParams[0] = new DBParameter("@ItemCode", DbType.AnsiString, 18, "%" + itemCode + "%");
      }
      if (name.Length > 0)
      {
        inputParams[1] = new DBParameter("@Name", DbType.AnsiString, 18, "%" + name + "%");
      }
      if (ultraCBCollection.Value != null)
      {
        inputParams[2] = new DBParameter("@Collection", DbType.Int32, ultraCBCollection.Value);
      }
      if (ultraCBCategory.Value != null)
      {
        inputParams[3] = new DBParameter("@Category", DbType.Int64, ultraCBCategory.Value);
      }
      if (saleCode.Length > 0)
      {
        inputParams[4] = new DBParameter("@SaleCode", DbType.AnsiString, 16, "%" + saleCode + "%");
      }
      if (ultraCBCustomer.Value != null)
      {
        inputParams[5] = new DBParameter("@CustomerPid", DbType.Int64, ultraCBCustomer.Value);
      }
      if (ultraCBExhibition.Value != null)
      {
        inputParams[6] = new DBParameter("@Exhibition", DbType.Int32, ultraCBExhibition.Value);
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemMasterController_Select", 600,inputParams);
      ultDetail.DataSource = dtSource;
      chkUS.Checked = true;
      chkUK.Checked = true;
      chkIT.Checked = true;
      chkME.Checked = true;
      chkRU.Checked = true;
      chkAU.Checked = true;
      chkCN.Checked = true;
      chkEU.Checked = true;
      chkOther.Checked = true;
      btnSearch.Enabled = true;
    }
    #endregion SaveData

    private void ShowOrHideGroupCustomer(string prefixCustomerCode)
    {
      try
      {
        bool isCheck;
        switch (prefixCustomerCode)
        {
          case "US":
            isCheck = chkUS.Checked;
            break;
          case "UK":
            isCheck = chkUK.Checked;
            break;
          case "IT":
            isCheck = chkIT.Checked;
            break;
          case "ME":
            isCheck = chkME.Checked;
            break;
          case "RU":
            isCheck = chkRU.Checked;
            break;
          case "AU":
            isCheck = chkAU.Checked;
            break;
          case "CN":
            isCheck = chkCN.Checked;
            break;
          case "EU":
            isCheck = chkEU.Checked;
            break;
          default:
            isCheck = chkOther.Checked;
            break;
        }
        for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          string colName = ultDetail.DisplayLayout.Bands[0].Columns[i].ToString();
          if (colName.IndexOf(prefixCustomerCode) == 0)
          {
            ultDetail.DisplayLayout.Bands[0].Columns[i].Hidden = !isCheck;
          }
        }
        ultDetail.DisplayLayout.Bands[0].Columns["USSP_D"].Hidden = true;
        ultDetail.DisplayLayout.Bands[0].Columns["UKSP_D"].Hidden = true;
        ultDetail.DisplayLayout.Bands[0].Columns["ITSP_D"].Hidden = true;
        ultDetail.DisplayLayout.Bands[0].Columns["MESP_D"].Hidden = true;
        ultDetail.DisplayLayout.Bands[0].Columns["RUSP_D"].Hidden = true;
        ultDetail.DisplayLayout.Bands[0].Columns["AUSP_D"].Hidden = true;
        ultDetail.DisplayLayout.Bands[0].Columns["CNSP_D"].Hidden = true;
        ultDetail.DisplayLayout.Bands[0].Columns["EUSP_D"].Hidden = true;
      }
      catch { }
    }    
    #endregion function

    #region Event
    /// <summary>
    /// Button Search Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      if (this.NeedToSave)
      {
        string messageConfirm = Shared.Utility.FunctionUtility.GetMessage("MSG0008");
        DialogResult dlgr = MessageBox.Show(messageConfirm, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (dlgr == DialogResult.Yes)
        {
          SaveAndClose();
        }
        else if (dlgr == DialogResult.No)
        { 
          this.NeedToSave = false;
        }
      }
      this.Search();
    }

    /// <summary>
    /// Button Save Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    /// <summary>
    /// Button Close Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Override Function SaveAndClose
    /// </summary>
    public override void SaveAndClose()
    {
      this.btnSave_Click(null, null);
    }

    /// <summary>
    /// Format UltraGridView
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultDetail);
      //e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Columns["Revision"].Hidden = true;
      e.Layout.Bands[0].Columns["USSP_D"].Hidden = true;
      e.Layout.Bands[0].Columns["UKSP_D"].Hidden = true;
      e.Layout.Bands[0].Columns["ITSP_D"].Hidden = true;
      e.Layout.Bands[0].Columns["MESP_D"].Hidden = true;
      e.Layout.Bands[0].Columns["RUSP_D"].Hidden = true;
      e.Layout.Bands[0].Columns["AUSP_D"].Hidden = true;
      e.Layout.Bands[0].Columns["CNSP_D"].Hidden = true;
      e.Layout.Bands[0].Columns["EUSP_D"].Hidden = true;

      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SaleCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;      
      e.Layout.Bands[0].Columns["Name"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      //US
      e.Layout.Bands[0].Columns["USActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["USQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["USQuickShip"].Header.Caption = "US QS";
      e.Layout.Bands[0].Columns["USSpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["USSpecial"].Header.Caption = "US SP";
      e.Layout.Bands[0].Columns["USDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["USDiscontinue"].Header.Caption = "US D";
      e.Layout.Bands[0].Columns["USOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["USOnHold"].Header.Caption = "USOnHold";
      e.Layout.Bands[0].Columns["USSP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["USSP_D"].Header.Caption = "US SP-D";
      e.Layout.Bands[0].Columns["USST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["USST_D"].Header.Caption = "US ST-D";
      e.Layout.Bands[0].Columns["USST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["USST_SP"].Header.Caption = "US ST-SP";
      e.Layout.Bands[0].Columns["USST_R"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["USST_R"].Header.Caption = "US ST-R";

      e.Layout.Bands[0].Columns["USST_R_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["USST_R_D"].Header.Caption = "US ST*-D";
      e.Layout.Bands[0].Columns["USST_R_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["USST_R_SP"].Header.Caption = "US ST*-SP";

      //UK
      e.Layout.Bands[0].Columns["UKActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["UKQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["UKQuickShip"].Header.Caption = "UKQS";
      e.Layout.Bands[0].Columns["UKSpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["UKSpecial"].Header.Caption = "UK SP";
      e.Layout.Bands[0].Columns["UKDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["UKDiscontinue"].Header.Caption = "UK D";
      e.Layout.Bands[0].Columns["UKOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["UKOnHold"].Header.Caption = "UKOnHold";
      e.Layout.Bands[0].Columns["UKSP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["UKSP_D"].Header.Caption = "UK SP-D";
      e.Layout.Bands[0].Columns["UKST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["UKST_D"].Header.Caption = "UK ST-D";
      e.Layout.Bands[0].Columns["UKST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["UKST_SP"].Header.Caption = "UK ST-SP";

      //IT
      e.Layout.Bands[0].Columns["ITActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ITQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ITQuickShip"].Header.Caption = "ITQS";
      e.Layout.Bands[0].Columns["ITSpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ITSpecial"].Header.Caption = "IT SP";
      e.Layout.Bands[0].Columns["ITDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ITDiscontinue"].Header.Caption = "IT D";
      e.Layout.Bands[0].Columns["ITOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ITOnHold"].Header.Caption = "ITOnHold";
      e.Layout.Bands[0].Columns["ITSP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ITSP_D"].Header.Caption = "IT SP-D";
      e.Layout.Bands[0].Columns["ITST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ITST_D"].Header.Caption = "IT ST-D";
      e.Layout.Bands[0].Columns["ITST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ITST_SP"].Header.Caption = "IT ST-SP";

      //ME
      e.Layout.Bands[0].Columns["MEActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["MEQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["MEQuickShip"].Header.Caption = "MEQS";
      e.Layout.Bands[0].Columns["MESpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["MESpecial"].Header.Caption = "ME SP";
      e.Layout.Bands[0].Columns["MEDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["MEDiscontinue"].Header.Caption = "ME D";
      e.Layout.Bands[0].Columns["MEOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["MEOnHold"].Header.Caption = "MEOnHold";
      e.Layout.Bands[0].Columns["MESP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["MESP_D"].Header.Caption = "ME SP-D";
      e.Layout.Bands[0].Columns["MEST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["MEST_D"].Header.Caption = "ME ST-D";
      e.Layout.Bands[0].Columns["MEST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["MEST_SP"].Header.Caption = "ME ST-SP";

      //RU
      e.Layout.Bands[0].Columns["RUActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["RUQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["RUQuickShip"].Header.Caption = "RUQS";
      e.Layout.Bands[0].Columns["RUSpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["RUSpecial"].Header.Caption = "RU SP";
      e.Layout.Bands[0].Columns["RUDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["RUDiscontinue"].Header.Caption = "RU D";
      e.Layout.Bands[0].Columns["RUOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["RUOnHold"].Header.Caption = "RUOnHold";
      e.Layout.Bands[0].Columns["RUSP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["RUSP_D"].Header.Caption = "RU SP-D";
      e.Layout.Bands[0].Columns["RUST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["RUST_D"].Header.Caption = "RU ST-D";
      e.Layout.Bands[0].Columns["RUST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["RUST_SP"].Header.Caption = "RU ST-SP";

      //AU
      e.Layout.Bands[0].Columns["AUActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["AUQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["AUQuickShip"].Header.Caption = "AUQS";
      e.Layout.Bands[0].Columns["AUSpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["AUSpecial"].Header.Caption = "AU SP";
      e.Layout.Bands[0].Columns["AUDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["AUDiscontinue"].Header.Caption = "AU D";
      e.Layout.Bands[0].Columns["AUOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["AUOnHold"].Header.Caption = "AUOnHold";
      e.Layout.Bands[0].Columns["AUSP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["AUSP_D"].Header.Caption = "AU SP-D";
      e.Layout.Bands[0].Columns["AUST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["AUST_D"].Header.Caption = "AU ST-D";
      e.Layout.Bands[0].Columns["AUST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["AUST_SP"].Header.Caption = "AU ST-SP";

      //CN
      e.Layout.Bands[0].Columns["CNActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CNQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CNQuickShip"].Header.Caption = "CNQS";
      e.Layout.Bands[0].Columns["CNSpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CNSpecial"].Header.Caption = "CN SP";
      e.Layout.Bands[0].Columns["CNDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CNDiscontinue"].Header.Caption = "CN D";
      e.Layout.Bands[0].Columns["CNOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CNOnHold"].Header.Caption = "CNOnHold";
      e.Layout.Bands[0].Columns["CNSP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CNSP_D"].Header.Caption = "CN SP-D";
      e.Layout.Bands[0].Columns["CNST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CNST_D"].Header.Caption = "CN ST-D";
      e.Layout.Bands[0].Columns["CNST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CNST_SP"].Header.Caption = "CN ST-SP";

      //EU
      e.Layout.Bands[0].Columns["EUActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["EUQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["EUQuickShip"].Header.Caption = "EUQS";
      e.Layout.Bands[0].Columns["EUSpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["EUSpecial"].Header.Caption = "EU SP";
      e.Layout.Bands[0].Columns["EUDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["EUDiscontinue"].Header.Caption = "EU D";
      e.Layout.Bands[0].Columns["EUOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["EUOnHold"].Header.Caption = "EUOnHold";
      e.Layout.Bands[0].Columns["EUSP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["EUSP_D"].Header.Caption = "EU SP-D";
      e.Layout.Bands[0].Columns["EUST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["EUST_D"].Header.Caption = "EU ST-D";
      e.Layout.Bands[0].Columns["EUST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["EUST_SP"].Header.Caption = "EU ST-SP";

      e.Layout.Bands[0].Columns["OtherActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["OtherActive"].Header.Caption = "Active";
      e.Layout.Bands[0].Columns["OtherDisContinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["OtherDisContinue"].Header.Caption = "D";
      e.Layout.Bands[0].Columns["OtherCustom"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["OtherCustom"].Header.Caption = "Custom";
      e.Layout.Bands[0].Columns["OtherOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["OtherOnHold"].Header.Caption = "OnHold";

      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 80;
      e.Layout.Bands[0].Columns["SaleCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["SaleCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["USActive"].MinWidth = 60;
      e.Layout.Bands[0].Columns["USActive"].MaxWidth = 60;      
      e.Layout.Bands[0].Columns["USQuickShip"].MinWidth = 45;
      e.Layout.Bands[0].Columns["USQuickShip"].MaxWidth = 45;
      e.Layout.Bands[0].Columns["USSpecial"].MinWidth = 45;
      e.Layout.Bands[0].Columns["USSpecial"].MaxWidth = 45;
      e.Layout.Bands[0].Columns["USDiscontinue"].MinWidth = 40;
      e.Layout.Bands[0].Columns["USDiscontinue"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["USOnHold"].MinWidth = 60;
      e.Layout.Bands[0].Columns["USOnHold"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["USSP_D"].MinWidth = 55;
      e.Layout.Bands[0].Columns["USSP_D"].MaxWidth = 55;
      e.Layout.Bands[0].Columns["USST_D"].MinWidth = 55;
      e.Layout.Bands[0].Columns["USST_D"].MaxWidth = 55;
      e.Layout.Bands[0].Columns["USST_SP"].MinWidth = 60;
      e.Layout.Bands[0].Columns["USST_SP"].MaxWidth = 60;

      e.Layout.Bands[0].Columns["UKActive"].MinWidth = 60;
      e.Layout.Bands[0].Columns["UKActive"].MaxWidth = 60;      
      e.Layout.Bands[0].Columns["UKQuickShip"].MinWidth = 45;
      e.Layout.Bands[0].Columns["UKQuickShip"].MaxWidth = 45;
      e.Layout.Bands[0].Columns["UKSpecial"].MinWidth = 45;
      e.Layout.Bands[0].Columns["UKSpecial"].MaxWidth = 45;
      e.Layout.Bands[0].Columns["UKDiscontinue"].MinWidth = 40;
      e.Layout.Bands[0].Columns["UKDiscontinue"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["UKOnHold"].MinWidth = 60;
      e.Layout.Bands[0].Columns["UKOnHold"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["UKSP_D"].MinWidth = 55;
      e.Layout.Bands[0].Columns["UKSP_D"].MaxWidth = 55;
      e.Layout.Bands[0].Columns["UKST_D"].MinWidth = 55;
      e.Layout.Bands[0].Columns["UKST_D"].MaxWidth = 55;
      e.Layout.Bands[0].Columns["UKST_SP"].MinWidth = 60;
      e.Layout.Bands[0].Columns["UKST_SP"].MaxWidth = 60;

      e.Layout.Bands[0].Columns["OtherActive"].MinWidth = 50;
      e.Layout.Bands[0].Columns["OtherActive"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["OtherDisContinue"].MinWidth = 35;
      e.Layout.Bands[0].Columns["OtherDisContinue"].MaxWidth = 35;
      e.Layout.Bands[0].Columns["OtherCustom"].MinWidth = 50;
      e.Layout.Bands[0].Columns["OtherCustom"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["OtherOnHold"].MinWidth = 60;
      e.Layout.Bands[0].Columns["OtherOnHold"].MaxWidth = 60;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        string columnName = e.Layout.Bands[0].Columns[i].ToString();
        string prefix = columnName.Substring(0, 2);
        switch(prefix)
        {
          case "US":
            e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
            break;
          case "UK":
            e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.Yellow;
            break;
          case "IT":
            e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightCyan;
            break;
          case "ME":
            e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGreen;
            break;
          case "RU":
            e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightPink;
            break;
          case "AU":
            e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightSeaGreen;
            break;
          case "CN":
            e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightCoral;
            break;
          case "EU":
            e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightSalmon;
            break;
          default:
            break;
        }
      }      
    }

    /// <summary>
    /// After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      this.NeedToSave = true;
    }

    private void chkShowItemPicture_CheckedChanged(object sender, EventArgs e)
    {
      Shared.Utility.ControlUtility.BOMShowItemImage(ultDetail, groupItemPicture, pictureItem, chkShowItemPicture.Checked);
    }

    private void ultDetail_MouseClick(object sender, MouseEventArgs e)
    {
      Shared.Utility.ControlUtility.BOMShowItemImage(ultDetail, groupItemPicture, pictureItem, chkShowItemPicture.Checked);
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtItemCode.Text = string.Empty;
      txtName.Text = string.Empty;
      txtSaleCode.Text = string.Empty;
      //ultraCBCustomer.Value = null;
      ultraCBCategory.Value = null;
      ultraCBCollection.Value = null;
    }

    private void ultraCBCustomer_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Display"].Hidden = true;
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Code"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Code"].MaxWidth = 100;
    }

    private void btnExportToExcel_Click(object sender, EventArgs e)
    {      
      string strOutFileName = "Master Controller Item";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string pathOutputFile = string.Format(@"{0}\{1}_{2}_{3}.xls", strPathOutputFile, strOutFileName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);

      ControlUtility.ExportToExcel(ultDetail, pathOutputFile);
    }

    private void chkUS_CheckedChanged(object sender, EventArgs e)
    {
      this.ShowOrHideGroupCustomer("US");      
    }

    private void chkUK_CheckedChanged(object sender, EventArgs e)
    {
      this.ShowOrHideGroupCustomer("UK");
    }

    private void chkIT_CheckedChanged(object sender, EventArgs e)
    {
      this.ShowOrHideGroupCustomer("IT");
    }

    private void chkME_CheckedChanged(object sender, EventArgs e)
    {
      this.ShowOrHideGroupCustomer("ME");
    }

    private void chkRU_CheckedChanged(object sender, EventArgs e)
    {
      this.ShowOrHideGroupCustomer("RU");
    }

    private void chkAU_CheckedChanged(object sender, EventArgs e)
    {
      this.ShowOrHideGroupCustomer("AU");
    }

    private void chkCN_CheckedChanged(object sender, EventArgs e)
    {
      this.ShowOrHideGroupCustomer("CN");
    }

    private void chkEU_CheckedChanged(object sender, EventArgs e)
    {
      this.ShowOrHideGroupCustomer("EU");
    }

    private void chkOther_CheckedChanged(object sender, EventArgs e)
    {
      this.ShowOrHideGroupCustomer("Other");      
    }

    private void ultDetail_AfterCellActivate(object sender, EventArgs e)
    {
      if (ultDetail.ActiveCell != null && DBConvert.ParseInt(ultDetail.ActiveCell.Text.ToString()) == 0)
      {
        string activeColName = ultDetail.ActiveCell.Column.ToString();
        if (activeColName.StartsWith("US") && !activeColName.StartsWith("USActive"))
        {
          for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
          {
            string colName = ultDetail.DisplayLayout.Bands[0].Columns[i].ToString();
            if (colName.StartsWith("US") && string.Compare(activeColName, colName, true) != 0 && string.Compare(colName, "USActive", true) != 0)
            {
              ultDetail.ActiveCell.Row.Cells[i].Value = 0;
            }
          }
        }
        else if (activeColName.StartsWith("UK") && !activeColName.StartsWith("UKActive"))
        {
          for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
          {
            string colName = ultDetail.DisplayLayout.Bands[0].Columns[i].ToString();
            if (colName.StartsWith("UK") && string.Compare(activeColName, colName, true) != 0 && string.Compare(colName, "UKActive", true) != 0)
            {
              ultDetail.ActiveCell.Row.Cells[i].Value = 0;
            }
          }
        }
        else if (activeColName.StartsWith("Other") && !activeColName.StartsWith("OtherActive"))
        {
          for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
          {
            string colName = ultDetail.DisplayLayout.Bands[0].Columns[i].ToString();
            if (colName.StartsWith("Other") && string.Compare(activeColName, colName, true) != 0 && string.Compare(colName, "OtherActive", true) != 0)
            {
              ultDetail.ActiveCell.Row.Cells[i].Value = 0;
            }
          }
        }
        else if (activeColName.StartsWith("IT") && !activeColName.StartsWith("ITActive"))
        {
          for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
          {
            string colName = ultDetail.DisplayLayout.Bands[0].Columns[i].ToString();
            if (colName.StartsWith("IT") && string.Compare(activeColName, colName, true) != 0 && string.Compare(colName, "ITActive", true) != 0)
            {
              ultDetail.ActiveCell.Row.Cells[i].Value = 0;
            }
          }
        }
        else if (activeColName.StartsWith("ME") && !activeColName.StartsWith("MEActive"))
        {
          for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
          {
            string colName = ultDetail.DisplayLayout.Bands[0].Columns[i].ToString();
            if (colName.StartsWith("ME") && string.Compare(activeColName, colName, true) != 0 && string.Compare(colName, "MEActive", true) != 0)
            {
              ultDetail.ActiveCell.Row.Cells[i].Value = 0;
            }
          }
        }
        else if (activeColName.StartsWith("RU") && !activeColName.StartsWith("RUActive"))
        {
          for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
          {
            string colName = ultDetail.DisplayLayout.Bands[0].Columns[i].ToString();
            if (colName.StartsWith("RU") && string.Compare(activeColName, colName, true) != 0 && string.Compare(colName, "RUActive", true) != 0)
            {
              ultDetail.ActiveCell.Row.Cells[i].Value = 0;
            }
          }
        }
        else if (activeColName.StartsWith("AU") && !activeColName.StartsWith("AUActive"))
        {
          for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
          {
            string colName = ultDetail.DisplayLayout.Bands[0].Columns[i].ToString();
            if (colName.StartsWith("AU") && string.Compare(activeColName, colName, true) != 0 && string.Compare(colName, "AUActive", true) != 0)
            {
              ultDetail.ActiveCell.Row.Cells[i].Value = 0;
            }
          }
        }
        else if (activeColName.StartsWith("CN") && !activeColName.StartsWith("CNActive"))
        {
          for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
          {
            string colName = ultDetail.DisplayLayout.Bands[0].Columns[i].ToString();
            if (colName.StartsWith("CN") && string.Compare(activeColName, colName, true) != 0 && string.Compare(colName, "CNActive", true) != 0)
            {
              ultDetail.ActiveCell.Row.Cells[i].Value = 0;
            }
          }
        }
        else if (activeColName.StartsWith("EU") && !activeColName.StartsWith("EUActive"))
        {
          for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
          {
            string colName = ultDetail.DisplayLayout.Bands[0].Columns[i].ToString();
            if (colName.StartsWith("EU") && string.Compare(activeColName, colName, true) != 0 && string.Compare(colName, "EUActive", true) != 0)
            {
              ultDetail.ActiveCell.Row.Cells[i].Value = 0;
            }
          }
        }
      }
    }

    private void chkImportFromExcelFile_CheckedChanged(object sender, EventArgs e)
    {
      tableImportFromExcel.Visible = chkImportFromExcelFile.Checked;
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      txtExcelFilePath.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      
      btnImportFromExcel.Enabled = (txtExcelFilePath.Text.Length > 0);      
    }

    private void btnImportFromExcel_Click(object sender, EventArgs e)
    {
      int countItem = (int) DataBaseAccess.ExecuteScalarCommandText("SELECT COUNT(*) FROM TblBOMItemBasic WHERE CustomerPid = 27");
      string commandText = string.Format(string.Format(@"SELECT * FROM [Sheet1$A1:BN{0}]", countItem));
      DataSet ds = FunctionUtility.GetExcelToDataSet(txtExcelFilePath.Text, commandText);
      if (ds != null && ds.Tables.Count > 0)
      {
        DataTable dtDataFromExcel = ds.Tables[0];        
        this.Search();
        DataTable dtSource = (DataTable)ultDetail.DataSource;

        // Change colum name of dtDataFromExcel
        for (int i = 0; i < dtDataFromExcel.Columns.Count; i++)
        {
          string colExcelName = dtDataFromExcel.Columns[i].ColumnName;
          
          for (int k = 0; k < ultDetail.DisplayLayout.Bands[0].Columns.Count; k++)
          {
            string colHeaderName = ultDetail.DisplayLayout.Bands[0].Columns[k].Header.Caption;
            if (string.Compare(colExcelName, colHeaderName, true) == 0)
            {
              dtDataFromExcel.Columns[i].ColumnName = ultDetail.DisplayLayout.Bands[0].Columns[k].ToString();
            }
          }
        }
        // Import data into Grid
        dtSource.Rows.Clear();
        try
        {
          foreach (DataRow rowExcel in dtDataFromExcel.Rows)
          {
            DataRow row = dtSource.NewRow();
            for (int i = 0; i < dtSource.Columns.Count; i++)
            {
              string colName = dtSource.Columns[i].ColumnName;
              if (string.Compare(colName, "Revision", true) != 0 && string.Compare(colName, "USSP_D", true) != 0 && string.Compare(colName, "UKSP_D", true) != 0
                && string.Compare(colName, "ITSP_D", true) != 0 && string.Compare(colName, "MESP_D", true) != 0
                && string.Compare(colName, "RUSP_D", true) != 0 && string.Compare(colName, "AUSP_D", true) != 0
                && string.Compare(colName, "CNSP_D", true) != 0 && string.Compare(colName, "EUSP_D", true) != 0)
              {
                row[colName] = rowExcel[colName];
              }              
            }                     
            dtSource.Rows.Add(row);
          }
        }
        catch(Exception ex)
        {
          dtSource.Rows.Clear();
          WindowUtinity.ShowMessageError("ERR0007");
        }
        this.isImportFromExcel = true;
      }
    }

    private void ultraCBExhibition_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Code"].Hidden = true;
    }
    #endregion Event
  }
}
