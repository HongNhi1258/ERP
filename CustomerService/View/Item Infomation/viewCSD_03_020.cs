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
  public partial class viewCSD_03_020 : MainUserControl
  {
    #region field
    private bool isImportFromExcel = false;
    #endregion field
    #region Init
    public viewCSD_03_020()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_03_020_Load(object sender, EventArgs e)
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

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spCSDUpdateItemKind_Search", inputParams);
      ds.Relations.Add(new DataRelation("Parent_Child", ds.Tables[0].Columns["TransactionPid"], ds.Tables[1].Columns["TransactionPid"], false));
      ultDetail.DataSource = ds;
      btnSearch.Enabled = true;
    }
    #endregion SaveData

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
    /// Button Close Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
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
      e.Layout.Bands[1].Columns["Revision"].Hidden = true;
      e.Layout.Bands[1].Columns["USSP_D"].Hidden = true;
      e.Layout.Bands[1].Columns["UKSP_D"].Hidden = true;
      e.Layout.Bands[1].Columns["ITSP_D"].Hidden = true;
      e.Layout.Bands[1].Columns["MESP_D"].Hidden = true;
      e.Layout.Bands[1].Columns["RUSP_D"].Hidden = true;
      e.Layout.Bands[1].Columns["AUSP_D"].Hidden = true;
      e.Layout.Bands[1].Columns["CNSP_D"].Hidden = true;
      e.Layout.Bands[1].Columns["EUSP_D"].Hidden = true;
      e.Layout.Bands[0].Columns["TransactionPid"].Hidden = true;
      e.Layout.Bands[1].Columns["TransactionPid"].Hidden = true;

      e.Layout.Bands[1].Columns["ItemCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["SaleCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;      
      e.Layout.Bands[1].Columns["Name"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      //US
      e.Layout.Bands[1].Columns["USActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["USQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["USQuickShip"].Header.Caption = "US QS";
      e.Layout.Bands[1].Columns["USSpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["USSpecial"].Header.Caption = "US SP";
      e.Layout.Bands[1].Columns["USDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["USDiscontinue"].Header.Caption = "US D";
      e.Layout.Bands[1].Columns["USOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["USOnHold"].Header.Caption = "USOnHold";
      e.Layout.Bands[1].Columns["USSP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["USSP_D"].Header.Caption = "US SP-D";
      e.Layout.Bands[1].Columns["USST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["USST_D"].Header.Caption = "US ST-D";
      e.Layout.Bands[1].Columns["USST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["USST_SP"].Header.Caption = "US ST-SP";
      e.Layout.Bands[1].Columns["USST_R"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["USST_R"].Header.Caption = "US ST-R";

      e.Layout.Bands[1].Columns["USST_R_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["USST_R_D"].Header.Caption = "US ST*-D";
      e.Layout.Bands[1].Columns["USST_R_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["USST_R_SP"].Header.Caption = "US ST*-SP";

      //UK
      e.Layout.Bands[1].Columns["UKActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["UKQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["UKQuickShip"].Header.Caption = "UKQS";
      e.Layout.Bands[1].Columns["UKSpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["UKSpecial"].Header.Caption = "UK SP";
      e.Layout.Bands[1].Columns["UKDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["UKDiscontinue"].Header.Caption = "UK D";
      e.Layout.Bands[1].Columns["UKOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["UKOnHold"].Header.Caption = "UKOnHold";
      e.Layout.Bands[1].Columns["UKSP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["UKSP_D"].Header.Caption = "UK SP-D";
      e.Layout.Bands[1].Columns["UKST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["UKST_D"].Header.Caption = "UK ST-D";
      e.Layout.Bands[1].Columns["UKST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["UKST_SP"].Header.Caption = "UK ST-SP";

      //IT
      e.Layout.Bands[1].Columns["ITActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["ITQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["ITQuickShip"].Header.Caption = "ITQS";
      e.Layout.Bands[1].Columns["ITSpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["ITSpecial"].Header.Caption = "IT SP";
      e.Layout.Bands[1].Columns["ITDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["ITDiscontinue"].Header.Caption = "IT D";
      e.Layout.Bands[1].Columns["ITOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["ITOnHold"].Header.Caption = "ITOnHold";
      e.Layout.Bands[1].Columns["ITSP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["ITSP_D"].Header.Caption = "IT SP-D";
      e.Layout.Bands[1].Columns["ITST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["ITST_D"].Header.Caption = "IT ST-D";
      e.Layout.Bands[1].Columns["ITST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["ITST_SP"].Header.Caption = "IT ST-SP";

      //ME
      e.Layout.Bands[1].Columns["MEActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["MEQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["MEQuickShip"].Header.Caption = "MEQS";
      e.Layout.Bands[1].Columns["MESpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["MESpecial"].Header.Caption = "ME SP";
      e.Layout.Bands[1].Columns["MEDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["MEDiscontinue"].Header.Caption = "ME D";
      e.Layout.Bands[1].Columns["MEOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["MEOnHold"].Header.Caption = "MEOnHold";
      e.Layout.Bands[1].Columns["MESP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["MESP_D"].Header.Caption = "ME SP-D";
      e.Layout.Bands[1].Columns["MEST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["MEST_D"].Header.Caption = "ME ST-D";
      e.Layout.Bands[1].Columns["MEST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["MEST_SP"].Header.Caption = "ME ST-SP";

      //RU
      e.Layout.Bands[1].Columns["RUActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["RUQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["RUQuickShip"].Header.Caption = "RUQS";
      e.Layout.Bands[1].Columns["RUSpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["RUSpecial"].Header.Caption = "RU SP";
      e.Layout.Bands[1].Columns["RUDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["RUDiscontinue"].Header.Caption = "RU D";
      e.Layout.Bands[1].Columns["RUOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["RUOnHold"].Header.Caption = "RUOnHold";
      e.Layout.Bands[1].Columns["RUSP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["RUSP_D"].Header.Caption = "RU SP-D";
      e.Layout.Bands[1].Columns["RUST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["RUST_D"].Header.Caption = "RU ST-D";
      e.Layout.Bands[1].Columns["RUST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["RUST_SP"].Header.Caption = "RU ST-SP";

      //AU
      e.Layout.Bands[1].Columns["AUActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["AUQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["AUQuickShip"].Header.Caption = "AUQS";
      e.Layout.Bands[1].Columns["AUSpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["AUSpecial"].Header.Caption = "AU SP";
      e.Layout.Bands[1].Columns["AUDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["AUDiscontinue"].Header.Caption = "AU D";
      e.Layout.Bands[1].Columns["AUOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["AUOnHold"].Header.Caption = "AUOnHold";
      e.Layout.Bands[1].Columns["AUSP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["AUSP_D"].Header.Caption = "AU SP-D";
      e.Layout.Bands[1].Columns["AUST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["AUST_D"].Header.Caption = "AU ST-D";
      e.Layout.Bands[1].Columns["AUST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["AUST_SP"].Header.Caption = "AU ST-SP";

      //CN
      e.Layout.Bands[1].Columns["CNActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["CNQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["CNQuickShip"].Header.Caption = "CNQS";
      e.Layout.Bands[1].Columns["CNSpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["CNSpecial"].Header.Caption = "CN SP";
      e.Layout.Bands[1].Columns["CNDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["CNDiscontinue"].Header.Caption = "CN D";
      e.Layout.Bands[1].Columns["CNOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["CNOnHold"].Header.Caption = "CNOnHold";
      e.Layout.Bands[1].Columns["CNSP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["CNSP_D"].Header.Caption = "CN SP-D";
      e.Layout.Bands[1].Columns["CNST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["CNST_D"].Header.Caption = "CN ST-D";
      e.Layout.Bands[1].Columns["CNST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["CNST_SP"].Header.Caption = "CN ST-SP";

      //EU
      e.Layout.Bands[1].Columns["EUActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["EUQuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["EUQuickShip"].Header.Caption = "EUQS";
      e.Layout.Bands[1].Columns["EUSpecial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["EUSpecial"].Header.Caption = "EU SP";
      e.Layout.Bands[1].Columns["EUDiscontinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["EUDiscontinue"].Header.Caption = "EU D";
      e.Layout.Bands[1].Columns["EUOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["EUOnHold"].Header.Caption = "EUOnHold";
      e.Layout.Bands[1].Columns["EUSP_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["EUSP_D"].Header.Caption = "EU SP-D";
      e.Layout.Bands[1].Columns["EUST_D"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["EUST_D"].Header.Caption = "EU ST-D";
      e.Layout.Bands[1].Columns["EUST_SP"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["EUST_SP"].Header.Caption = "EU ST-SP";

      e.Layout.Bands[1].Columns["OtherActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["OtherActive"].Header.Caption = "Active";
      e.Layout.Bands[1].Columns["OtherDisContinue"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["OtherDisContinue"].Header.Caption = "D";
      e.Layout.Bands[1].Columns["OtherCustom"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["OtherCustom"].Header.Caption = "Custom";
      e.Layout.Bands[1].Columns["OtherOnHold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["OtherOnHold"].Header.Caption = "OnHold";

      e.Layout.Bands[1].Columns["ItemCode"].MaxWidth = 80;
      e.Layout.Bands[1].Columns["ItemCode"].MinWidth = 80;
      e.Layout.Bands[1].Columns["SaleCode"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["SaleCode"].MinWidth = 100;
      e.Layout.Bands[1].Columns["USActive"].MinWidth = 60;
      e.Layout.Bands[1].Columns["USActive"].MaxWidth = 60;
      e.Layout.Bands[1].Columns["USQuickShip"].MinWidth = 45;
      e.Layout.Bands[1].Columns["USQuickShip"].MaxWidth = 45;
      e.Layout.Bands[1].Columns["USSpecial"].MinWidth = 45;
      e.Layout.Bands[1].Columns["USSpecial"].MaxWidth = 45;
      e.Layout.Bands[1].Columns["USDiscontinue"].MinWidth = 40;
      e.Layout.Bands[1].Columns["USDiscontinue"].MaxWidth = 40;
      e.Layout.Bands[1].Columns["USOnHold"].MinWidth = 60;
      e.Layout.Bands[1].Columns["USOnHold"].MaxWidth = 60;
      e.Layout.Bands[1].Columns["USSP_D"].MinWidth = 55;
      e.Layout.Bands[1].Columns["USSP_D"].MaxWidth = 55;
      e.Layout.Bands[1].Columns["USST_D"].MinWidth = 55;
      e.Layout.Bands[1].Columns["USST_D"].MaxWidth = 55;
      e.Layout.Bands[1].Columns["USST_SP"].MinWidth = 60;
      e.Layout.Bands[1].Columns["USST_SP"].MaxWidth = 60;

      e.Layout.Bands[1].Columns["UKActive"].MinWidth = 60;
      e.Layout.Bands[1].Columns["UKActive"].MaxWidth = 60;
      e.Layout.Bands[1].Columns["UKQuickShip"].MinWidth = 45;
      e.Layout.Bands[1].Columns["UKQuickShip"].MaxWidth = 45;
      e.Layout.Bands[1].Columns["UKSpecial"].MinWidth = 45;
      e.Layout.Bands[1].Columns["UKSpecial"].MaxWidth = 45;
      e.Layout.Bands[1].Columns["UKDiscontinue"].MinWidth = 40;
      e.Layout.Bands[1].Columns["UKDiscontinue"].MaxWidth = 40;
      e.Layout.Bands[1].Columns["UKOnHold"].MinWidth = 60;
      e.Layout.Bands[1].Columns["UKOnHold"].MaxWidth = 60;
      e.Layout.Bands[1].Columns["UKSP_D"].MinWidth = 55;
      e.Layout.Bands[1].Columns["UKSP_D"].MaxWidth = 55;
      e.Layout.Bands[1].Columns["UKST_D"].MinWidth = 55;
      e.Layout.Bands[1].Columns["UKST_D"].MaxWidth = 55;
      e.Layout.Bands[1].Columns["UKST_SP"].MinWidth = 60;
      e.Layout.Bands[1].Columns["UKST_SP"].MaxWidth = 60;

      e.Layout.Bands[1].Columns["OtherActive"].MinWidth = 50;
      e.Layout.Bands[1].Columns["OtherActive"].MaxWidth = 50;
      e.Layout.Bands[1].Columns["OtherDisContinue"].MinWidth = 35;
      e.Layout.Bands[1].Columns["OtherDisContinue"].MaxWidth = 35;
      e.Layout.Bands[1].Columns["OtherCustom"].MinWidth = 50;
      e.Layout.Bands[1].Columns["OtherCustom"].MaxWidth = 50;
      e.Layout.Bands[1].Columns["OtherOnHold"].MinWidth = 60;
      e.Layout.Bands[1].Columns["OtherOnHold"].MaxWidth = 60;

      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        string columnName = e.Layout.Bands[1].Columns[i].ToString();
        string prefix = columnName.Substring(0, 2);
        switch(prefix)
        {
          case "US":
            e.Layout.Bands[1].Columns[i].CellAppearance.BackColor = Color.LightGray;
            break;
          case "UK":
            e.Layout.Bands[1].Columns[i].CellAppearance.BackColor = Color.Yellow;
            break;
          case "IT":
            e.Layout.Bands[1].Columns[i].CellAppearance.BackColor = Color.LightCyan;
            break;
          case "ME":
            e.Layout.Bands[1].Columns[i].CellAppearance.BackColor = Color.LightGreen;
            break;
          case "RU":
            e.Layout.Bands[1].Columns[i].CellAppearance.BackColor = Color.LightPink;
            break;
          case "AU":
            e.Layout.Bands[1].Columns[i].CellAppearance.BackColor = Color.LightSeaGreen;
            break;
          case "CN":
            e.Layout.Bands[1].Columns[i].CellAppearance.BackColor = Color.LightCoral;
            break;
          case "EU":
            e.Layout.Bands[1].Columns[i].CellAppearance.BackColor = Color.LightSalmon;
            break;
          default:
            break;
        }
        e.Layout.Bands[1].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
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

    private void ultraCBExhibition_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Code"].Hidden = true;
    }
    
    private void ultDetail_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultDetail.Selected.Rows.Count > 0 && ultDetail.Selected.Rows[0].Band.ParentBand == null)
      {
        long pid = DBConvert.ParseLong(ultDetail.Selected.Rows[0].Cells["TransactionPid"].Value.ToString());
        viewCSD_03_019 view = new viewCSD_03_019();
        view.viewTransactionPid = pid;
        WindowUtinity.ShowView(view, "Update Itemkind Information", true, ViewState.MainWindow);
      }
    } 
    
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewCSD_03_019 view = new viewCSD_03_019();
      Shared.Utility.WindowUtinity.ShowView(view, "Update Itemkind Infomation", false, Shared.Utility.ViewState.MainWindow);
    }
    #endregion Event

   

    
  }
}
