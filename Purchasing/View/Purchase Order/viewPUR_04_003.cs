/*
  Author      : 
  Date        : 26/04/2011
  Description : Insert Update PO
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Purchasing;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_04_003 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    // List Delete Pid
    private IList listDeletingDetailPid = new ArrayList();
    private IList listDeletedDetailPid = new ArrayList();
    private IList listDeletingSchedulePid = new ArrayList();
    private IList listDeletedSchedulePid = new ArrayList();
    private IList listDeletedPRDetailPid = new ArrayList();
    string listDeletedAddition = string.Empty;
    // PO
    public string poNo = string.Empty;
    // Status Control
    private bool nonConfirm = true;
    private bool canUpdate = false;
    private int status = 0;
    private bool isManager = false;
    private bool isLeader = false;
    // Data UltData
    private DataSet dsMain = new DataSet();
    // Store Name
    private string SP_PUR_PODETAIL_DELETE = "spPURPODetail_Delete";
    private string SP_PUR_PODETAILSCHEDULE_DELETE = "spPURPODetailSchedule_Delete";
    private string SP_PUR_POINFO_EDIT = "spPURPOInformation_Edit";
    private string SP_PUR_PODETAIL_EDIT = "spPURPODetail_Edit";
    private string SP_PUR_PODETAILSCHEDULE_EDIT = "spPURPODetailSchedule_Edit";

    #endregion Field

    #region Init Data
    /// <summary>
    /// New Update PO
    /// </summary>
    public viewPUR_04_003()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load New Update PO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_04_003_Load(object sender, EventArgs e)
    {
      // Check User Is Manager ?
      this.isManager = this.CheckPurchaseManager(SharedObject.UserInfo.UserPid);

      // Load Group In Charge
      this.LoadComboStaffGroup();

      // Load Suplier
      this.LoadComboSupplier();

      // Load PaymentForSupplier
      this.LoadComboPaymentForPurchaser();

      // Load Currency
      this.LoadComboCurrencyDetail();

      // Load ETD
      this.LoadComboETD();

      // Load Currency Addition
      this.LoadComboCurrencyAddition();

      // Load Currency
      this.LoadComboCurrency();

      // Load Data
      this.LoadData();
    }
    #endregion Init Data

    #region LoadData

    /// <summary>
    /// Load Payment
    /// </summary>
    private void LoadComboPaymentForPurchaser()
    {
      string commandText = string.Empty;
      commandText += " SELECT 0 ID, 'Supplier' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Purchaser' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Credit Card' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBPaymentForPurchaser.DataSource = dtSource;
      ultCBPaymentForPurchaser.DisplayMember = "Name";
      ultCBPaymentForPurchaser.ValueMember = "ID";
      ultCBPaymentForPurchaser.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBPaymentForPurchaser.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultCBPaymentForPurchaser.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      // Default Supplier
      //ultCBPaymentForPurchaser.Value = 0;
    }

    /// <summary>
    /// Load List Purchaser
    /// </summary>
    private void LoadComboPurchaser()
    {
      string commandText = "SELECT Pid, EmpName Name FROM VHRMEmployee WHERE Department = 'PUR'";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBPurchaser.DataSource = dtSource;
      ultCBPurchaser.DisplayMember = "Name";
      ultCBPurchaser.ValueMember = "Pid";
      ultCBPurchaser.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBPurchaser.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultCBPurchaser.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }
    /// <summary>
    /// Load UltraCombo Currency Addition
    /// </summary>
    private void LoadComboCurrencyAddition()
    {
      string commandText = "SELECT Pid, [Code] Name, CurrentExchangeRate ExchangeRate FROM TblPURCurrencyInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpAddCurrency.DataSource = dtSource;
      udrpAddCurrency.DisplayMember = "Name";
      udrpAddCurrency.ValueMember = "Pid";
      udrpAddCurrency.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpAddCurrency.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      udrpAddCurrency.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      udrpAddCurrency.DisplayLayout.Bands[0].Columns["ExchangeRate"].Hidden = true;
    }

    //<summary>
    //Load Multi Currency 
    //</summary>
    private void LoadComboCurrencyDetail()
    {
      string commandText = "SELECT Pid, Code + ' - ' + CAST(CurrentExchangeRate AS VARCHAR) Name FROM TblPURCurrencyInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpCurrency.DataSource = dtSource;
      udrpCurrency.ValueMember = "Pid";
      udrpCurrency.DisplayMember = "Name";
      udrpCurrency.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpCurrency.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      udrpCurrency.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
    }

    /// <summary>
    /// Load Multi ETD
    /// </summary>
    private void LoadComboETD()
    {
      string commandText = "SELECT Code, Value Name FROM TblBOMCodeMaster WHERE [Group] = 7013 AND DeleteFlag = 0";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpETD.DataSource = dtSource;
      udrpETD.ValueMember = "Code";
      udrpETD.DisplayMember = "Name";
      udrpETD.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpETD.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      udrpETD.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
    }

    /// <summary>
    /// LoadData
    /// </summary>
    private void LoadData()
    {
      // Load List PR
      this.LoadComboListPR();

      this.listDeletedDetailPid = new ArrayList();
      this.listDeletedSchedulePid = new ArrayList();
      this.listDeletedPRDetailPid = new ArrayList();
      if (this.poNo.Length > 0)
      {
        grpPRInfo.Visible = false;
        this.ultGroupInCharge.Enabled = false;
      }
      else
      {
        grpPRInfo.Visible = true;
        this.ultGroupInCharge.Enabled = true;
      }
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@PONo", DbType.String, this.poNo) };
      dsMain = DataBaseAccess.SearchStoreProcedure("spPURPOInformationByPONo", inputParam);
      DataTable dtPO = dsMain.Tables[0];

      if (dtPO.Rows.Count > 0)
      {
        DataRow row = dtPO.Rows[0];
        this.txtPoNo.Text = this.poNo;
        this.ultGroupInCharge.Value = DBConvert.ParseLong(row["GroupInCharge"].ToString());
        this.ultSupplier.Value = DBConvert.ParseLong(row["SupplierPid"].ToString());
        if (DBConvert.ParseLong(row["ContactPerson"].ToString()) != long.MinValue
            && DBConvert.ParseLong(row["ContactPerson"].ToString()) != 0)
        {
          this.ultContactPerson.Value = DBConvert.ParseLong(row["ContactPerson"].ToString());
        }
        this.txtOrderNo.Text = row["OrderNo"].ToString();
        this.txtFinishDate.Text = row["FinishedDate"].ToString();
        this.txtCreateBy.Text = row["CreateBy"].ToString();
        this.txtCreateDate.Text = row["CreateDate"].ToString();
        this.txtApprovedBy.Text = row["ApprovedBy"].ToString();
        this.txtRemark.Text = row["Remark"].ToString();
        if (DBConvert.ParseLong(row["CurrencyPid"].ToString()) > 0)
        {
          this.ultCBCurrency.Value = DBConvert.ParseLong(row["CurrencyPid"].ToString());
        }
        if (DBConvert.ParseInt(row["PaymentForPurchaser"].ToString()) >= 0)
        {
          this.ultCBPaymentForPurchaser.Value = DBConvert.ParseInt(row["PaymentForPurchaser"].ToString());
        }
        if (DBConvert.ParseInt(row["Purchaser"].ToString()) >= 0)
        {
          this.ultCBPurchaser.Value = DBConvert.ParseInt(row["Purchaser"].ToString());
        }
        if (!this.isManager)
        {
          this.nonConfirm = (DBConvert.ParseInt(row["POStatus"].ToString()) == 0);
        }
        else
        {
          this.nonConfirm = (DBConvert.ParseInt(row["POStatus"].ToString()) < 3);
        }

        this.status = DBConvert.ParseInt(row["POStatus"].ToString());
        if (!this.isManager && this.status > 0)
        {
          this.chkLock.Checked = true;
        }
        else if (this.isManager && this.status >= 3)
        {
          this.chkLock.Checked = true;
        }

        this.lblStatus.Text = row["Status"].ToString();
        this.isLeader = this.CheckGroupLeader(DBConvert.ParseLong(row["GroupInCharge"].ToString()), SharedObject.UserInfo.UserPid);
      }
      else
      {
        DataTable dtPR = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPURGetNewPOCode() NewPOCode");
        if ((dtPR != null) && (dtPR.Rows.Count > 0))
        {
          this.txtPoNo.Text = dtPR.Rows[0]["NewPOCode"].ToString();
          this.txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
          this.lblStatus.Text = "New";
          if (!isManager)
          {
            // Load Group In Charge(User Login)
            string commmadText = string.Format(@" SELECT SG.Pid
                                                  FROM TblPURStaffGroup SG
	                                                LEFT JOIN TblPURStaffGroupDetail SGD ON SG.Pid = SGD.[Group]
                                                  WHERE SG.DeleteFlg = 0 AND( SG.LeaderGroup = {0} OR SGD.Employee = {1})", SharedObject.UserInfo.UserPid, SharedObject.UserInfo.UserPid);
            DataTable dtGroupInCharge = DataBaseAccess.SearchCommandTextDataTable(commmadText);
            if (dtGroupInCharge != null && dtGroupInCharge.Rows.Count > 0)
            {
              this.ultGroupInCharge.Value = DBConvert.ParseInt(dtGroupInCharge.Rows[0]["Pid"].ToString());
            }
          }
          else
          {
            ultGroupInCharge.ReadOnly = false;
          }
        }
      }
      this.canUpdate = (btnSave.Enabled && this.nonConfirm);
      this.LoadDataPODetail(dsMain);
      // Set Status control
      this.SetStatusControl();
      this.ultPRNo_ValueChanged(null, null);
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      this.ultSupplier.Enabled = this.canUpdate;
      this.ultContactPerson.Enabled = this.canUpdate;
      this.txtOrderNo.Enabled = this.canUpdate;
      this.txtRemark.Enabled = this.canUpdate;
      this.btnMakePO.Enabled = this.canUpdate;
      this.chkLock.Enabled = this.canUpdate;
      if (this.status >= 5)
      {
        btnSave.Enabled = false;
      }
      this.grpPRInfo.Visible = this.canUpdate;
      this.ultCBPaymentForPurchaser.Enabled = this.canUpdate;
      this.ultCBPurchaser.Enabled = this.canUpdate;
      if ((this.status <= 1 && this.isLeader) || (this.status <= 2 && this.isManager))
      {
        this.chkLock.Checked = !this.canUpdate;
      }
      if (this.poNo == string.Empty)
      {
        btnOrderRemark.Enabled = false;
        btnRemarkDetail.Enabled = false;
        btnPrint.Enabled = false;
      }
      else
      {
        btnOrderRemark.Enabled = true;
        btnRemarkDetail.Enabled = true;
        btnPrint.Enabled = true;
      }
      if (this.status >= 3)
      {
        btnOrderRemark.Enabled = true;
      }
      this.ultCBCurrency.Enabled = this.canUpdate;
      this.txtExchangeRate.Enabled = this.canUpdate;
    }

    /// <summary>
    /// Load Data PO Detail
    /// </summary>
    /// <param name="dsSource"></param>
    private void LoadDataPODetail(DataSet dsSource)
    {
      DataSet dsData = this.ListPO();
      dsData.Tables.Add(dsSource.Tables[1].Clone());
      dsData.Tables.Add(dsSource.Tables[2].Clone());
      try
      {
        dsData.Tables["TblPO"].Merge(dsSource.Tables[1]);
      }
      catch
      {
      }
      try
      {
        dsData.Tables["TblPODetail"].Merge(dsSource.Tables[2]);
      }
      catch
      {
      }
      try
      {
        dsData.Tables["TblPOMapping"].Merge(dsSource.Tables[3]);
      }
      catch
      {
      }
      this.ultData.DataSource = dsData;

      this.canUpdate = (btnSave.Visible && this.nonConfirm);
      this.ultAddition.DataSource = dsSource.Tables[4];
      if (dsSource.Tables[4].Rows.Count > 0)
      {
        this.grpAdditionPrice.Visible = true;
      }
      else
      {
        this.grpAdditionPrice.Visible = false;
      }

      for (int i = 0; i < ultAddition.Rows.Count; i++)
      {
        UltraGridRow row = ultAddition.Rows[i];

        if (this.canUpdate)
        {
          row.Activation = Activation.AllowEdit;
        }
        else
        {
          row.Activation = Activation.ActivateOnly;
        }
      }

      // Lay GroupInCharge Khi User Login
      int groupPurchasingLogin = int.MinValue;
      string commadText = string.Format(@"SELECT DISTINCT SG.Pid
                                          FROM TblPURStaffGroup SG
                                          LEFT JOIN TblPURStaffGroupDetail SGD ON SG.Pid = SGD.[Group]
                                          WHERE DeleteFlg = 0 AND SG.LeaderGroup = {0} OR SGD.Employee = {1}", SharedObject.UserInfo.UserPid, SharedObject.UserInfo.UserPid);
      DataTable dtGroupPurchasingLogin = DataBaseAccess.SearchCommandTextDataTable(commadText);
      if (dtGroupPurchasingLogin != null && dtGroupPurchasingLogin.Rows.Count > 0)
      {
        groupPurchasingLogin = DBConvert.ParseInt(dtGroupPurchasingLogin.Rows[0]["Pid"].ToString());
      }
      // Lay GroupInCharge Cua POInfo
      int groupPurchasingPO = int.MinValue;
      if (this.poNo.Length > 0)
      {
        groupPurchasingPO = DBConvert.ParseInt(ultGroupInCharge.Value.ToString());
      }

      // Status  = 3 Received
      //         = 2 Waiting A Part
      //         = 1 Waiting For Receiving
      //         = 0 New
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        // Cho Phep Chinh Sua Over Qty
        if (DBConvert.ParseInt(row.Cells["StatusPODetail"].Value.ToString()) < 3)
        {
          row.Cells["Over"].Activation = Activation.AllowEdit;
        }
        else
        {
          row.Cells["Over"].Activation = Activation.ActivateOnly;
        }
        // Chinh sua qty, Price(dua vao status PO Detail va Group Purchasing)
        //if (DBConvert.ParseInt(row.Cells["StatusPODetail"].Value.ToString()) <= 2 && groupPurchasingLogin == groupPurchasingPO)
        if (DBConvert.ParseInt(row.Cells["StatusPODetail"].Value.ToString()) <= 2)
        {
          row.Cells["Quantity"].Activation = Activation.AllowEdit;
          if (DBConvert.ParseInt(row.Cells["StatusPODetail"].Value.ToString()) < 1)
          {
            row.Cells["Price"].Activation = Activation.AllowEdit;
          }
          row.Cells["VAT"].Activation = Activation.AllowEdit;
          row.Cells["Remark"].Activation = Activation.AllowEdit;

          //Child Bank
          for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow rowChild = row.ChildBands[0].Rows[j];
            rowChild.Cells["Qty"].Activation = Activation.AllowEdit;
            rowChild.Cells["ExpectDate"].Activation = Activation.AllowEdit;
            rowChild.Cells["ContractNo"].Activation = Activation.AllowEdit;
            rowChild.Cells["NameOfGoods"].Activation = Activation.AllowEdit;
            rowChild.Cells["InvoiceNo"].Activation = Activation.AllowEdit;
            rowChild.Cells["ETD1"].Activation = Activation.AllowEdit;
            rowChild.Cells["ConfirmExpectDate"].Activation = Activation.AllowEdit;
            rowChild.Cells["TimeOfReceivingOriginal"].Activation = Activation.AllowEdit;
            rowChild.Cells["TimeOfReceivingDoc"].Activation = Activation.AllowEdit;
            rowChild.Cells["ArrivalTimeToPort"].Activation = Activation.AllowEdit;
            rowChild.Cells["BLDate"].Activation = Activation.AllowEdit;
            rowChild.Cells["DocumentAt"].Activation = Activation.AllowEdit;
            rowChild.Cells["ContractNo"].Activation = Activation.AllowEdit;
          }
        }
        else
        {
          row.Cells["Quantity"].Activation = Activation.ActivateOnly;
          row.Cells["Price"].Activation = Activation.ActivateOnly;
          row.Cells["VAT"].Activation = Activation.ActivateOnly;
          row.Cells["Remark"].Activation = Activation.ActivateOnly;

          //Child Bank
          for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow rowChild = row.ChildBands[0].Rows[j];
            rowChild.Cells["Qty"].Activation = Activation.ActivateOnly;
            if (DBConvert.ParseInt(row.Cells["StatusPODetail"].Value.ToString()) == 3 ||
                DBConvert.ParseInt(row.Cells["StatusPODetail"].Value.ToString()) == 4)
            //groupPurchasingLogin != groupPurchasingPO)
            {
              rowChild.Cells["ExpectDate"].Activation = Activation.ActivateOnly;
            }
            rowChild.Cells["ContractNo"].Activation = Activation.ActivateOnly;
            rowChild.Cells["NameOfGoods"].Activation = Activation.ActivateOnly;
            rowChild.Cells["InvoiceNo"].Activation = Activation.ActivateOnly;
            rowChild.Cells["ETD1"].Activation = Activation.ActivateOnly;
            rowChild.Cells["ConfirmExpectDate"].Activation = Activation.ActivateOnly;
            rowChild.Cells["TimeOfReceivingOriginal"].Activation = Activation.ActivateOnly;
            rowChild.Cells["TimeOfReceivingDoc"].Activation = Activation.ActivateOnly;
            rowChild.Cells["ArrivalTimeToPort"].Activation = Activation.ActivateOnly;
            rowChild.Cells["BLDate"].Activation = Activation.ActivateOnly;
            rowChild.Cells["DocumentAt"].Activation = Activation.ActivateOnly;
            rowChild.Cells["ContractNo"].Activation = Activation.ActivateOnly;
          }
        }
        row.CellAppearance.BackColor = Color.PaleTurquoise;
      }

      ultData.Rows.ExpandAll(true);
    }

    private DataSet ListPO()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("TblPO");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("PRDTPid", typeof(System.Int64));
      taParent.Columns.Add("PRNo", typeof(System.String));
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("NameEN", typeof(System.String));
      taParent.Columns.Add("Unit", typeof(System.String));
      taParent.Columns.Add("GroupName", typeof(System.String));
      taParent.Columns.Add("RequestDate", typeof(System.String));
      taParent.Columns.Add("CommittedDate", typeof(System.String));
      taParent.Columns.Add("MaxExpectDate", typeof(System.String));
      taParent.Columns.Add("Quantity", typeof(System.Double));
      taParent.Columns.Add("Price", typeof(System.Double));
      taParent.Columns.Add("Currency", typeof(System.Int64));
      taParent.Columns.Add("VAT", typeof(System.Double));
      taParent.Columns.Add("Over", typeof(System.Double));
      taParent.Columns.Add("Remark", typeof(System.String));
      taParent.Columns.Add("SicentificName", typeof(System.String));
      taParent.Columns.Add("HavestedForest", typeof(System.String));
      taParent.Columns.Add("HavestedCountry", typeof(System.String));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("TblPODetail");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("PODetatlPid", typeof(System.Int64));
      taChild.Columns.Add("PRDTPid", typeof(System.Int64));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("ReceiptedQty", typeof(System.Double));
      taChild.Columns.Add("QtyCancel", typeof(System.Double));
      taChild.Columns.Add("ExpectDate", typeof(System.DateTime));
      taChild.Columns.Add("ConfirmExpectDate", typeof(System.DateTime));
      taChild.Columns.Add("LatestDeliveryDate", typeof(System.DateTime));
      taChild.Columns.Add("ContractNo", typeof(System.String));
      taChild.Columns.Add("NameOfGoods", typeof(System.String));
      taChild.Columns.Add("InvoiceNo", typeof(System.String));
      taChild.Columns.Add("ETD1", typeof(System.Int32));
      taChild.Columns.Add("ETD2", typeof(System.DateTime));
      taChild.Columns.Add("ETA", typeof(System.DateTime));
      taChild.Columns.Add("TimeOfReceivingDoc", typeof(System.DateTime));
      taChild.Columns.Add("TimeOfReceivingOriginal", typeof(System.DateTime));
      taChild.Columns.Add("ArrivalTimeToPort", typeof(System.DateTime));
      taChild.Columns.Add("BLDate", typeof(System.DateTime));
      taChild.Columns.Add("DocumentAt", typeof(System.String));
      ds.Tables.Add(taChild);

      DataTable taSchedule = new DataTable("TblPOMapping");
      taSchedule.Columns.Add("Pid", typeof(System.Int64));
      taSchedule.Columns.Add("ReceivingNote", typeof(System.String));
      taSchedule.Columns.Add("Qty", typeof(System.Double));
      taSchedule.Columns.Add("CreateDate", typeof(System.String));

      ds.Tables.Add(taSchedule);

      ds.Relations.Add(new DataRelation("TblPO_TblPODetail", new DataColumn[] { taParent.Columns["Pid"], taParent.Columns["PRDTPid"] }, new DataColumn[] { taChild.Columns["PODetatlPid"], taChild.Columns["PRDTPid"] }, false));
      ds.Relations.Add(new DataRelation("TblPODetail_TblPOMapping", new DataColumn[] { taChild.Columns["Pid"] }, new DataColumn[] { taSchedule.Columns["Pid"] }, false));
      return ds;
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
    /// Load UltraCombo Supplier
    /// </summary>
    private void LoadComboSupplier()
    {
      string commandText = "SELECT Pid , SupplierCode  + ' - ' + VietnameseName Supplier FROM TblPURSupplierInfo WHERE  DeleteFlg = 0  ORDER BY VietnameseName";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultSupplier.DataSource = dtSource;
      ultSupplier.DisplayMember = "Supplier";
      ultSupplier.ValueMember = "Pid";
      ultSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultSupplier.DisplayLayout.Bands[0].Columns["Supplier"].Width = 250;
      ultSupplier.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Currency
    /// </summary>
    private void LoadComboCurrency()
    {
      string commandText = " SELECT Pid, Code, CurrentExchangeRate FROM TblPURCurrencyInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBCurrency.DataSource = dtSource;
      ultCBCurrency.DisplayMember = "Code";
      ultCBCurrency.ValueMember = "Pid";
      ultCBCurrency.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBCurrency.DisplayLayout.Bands[0].Columns["Code"].Width = 250;
      ultCBCurrency.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCBCurrency.DisplayLayout.Bands[0].Columns["CurrentExchangeRate"].Hidden = true;

      this.ultCBCurrency.Value = 1;
    }

    /// <summary>
    /// Load UltraCombo List PR
    /// </summary>
    private void LoadComboListPR()
    {
      string commandText = string.Empty;
      commandText = string.Format(@"
                                      SELECT PR.PRNo, PR.PRNo + '_' + CONVERT(VARCHAR, PR.CreateDate, 103) Name
                                      FROM 
                                      (
	                                      SELECT PR.PRNo, COUNT(*) Times
	                                      FROM TblPURPRInformation PR
		                                      INNER JOIN TblPURPRDetail DT ON PR.PRNo = DT.PRNo
	                                      WHERE PR.[Status] IN (6, 7, 8, 9, 10)
			                                      AND DT.[Status] IN (8, 9)
	                                      GROUP BY PR.PRNo
	                                      HAVING COUNT(*) > 0
                                      ) AA 
                                      INNER JOIN TblPURPRInformation PR ON AA.PRNo = PR.PRNo
                                      ORDER BY PR.PRNo DESC
                                  ");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultPRNo.DataSource = dtSource;
      ultPRNo.DisplayMember = "Name";
      ultPRNo.ValueMember = "PRNo";
      ultPRNo.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultPRNo.DisplayLayout.Bands[0].Columns["Name"].Width = 550;
      ultPRNo.DisplayLayout.Bands[0].Columns["PRNo"].Hidden = true;
    }

    /// <summary>
    /// Check Valid PO Information Info
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidPOInformationInfo(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      // Purchaser Group
      long groupPurchase = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultGroupInCharge));
      if (groupPurchase == long.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Group In Charge");
        return false;
      }
      // Supplier
      long supplierPid = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultSupplier));
      if (supplierPid == long.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Supplier");
        return false;
      }
      if (ultCBPaymentForPurchaser.Value == null)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Payment");
        return false;
      }
      int paymentForSuplier = DBConvert.ParseInt(ultCBPaymentForPurchaser.Value.ToString());
      if (paymentForSuplier == 1)
      {
        if (ultCBPurchaser.Value == null)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Purchaser");
          return false;
        }
      }
      // Currency
      if (ultCBCurrency.Value == null)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Currency");
        return false;
      }

      return true;
    }

    /// <summary>
    /// Check Valid PO Detail Information
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidPODetailInformation(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      long groupPurchase = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultGroupInCharge));
      DataTable dtCheck = new DataTable();
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];

        // PO Detail Schedule
        double qty = DBConvert.ParseDouble(row.Cells["Quantity"].Value.ToString());
        double prQty = this.GetPRDetailQty(DBConvert.ParseLong(row.Cells["PRDTPid"].Value.ToString()));
        if (qty > prQty)
        {
          message = FunctionUtility.GetMessage("ERR0201");
          return false;
        }

        // Check So Luong Nhap Vao PO
        long poDetailPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        double qtyPO = 0;
        if (poDetailPid != int.MinValue && poDetailPid > 0)
        {
          commandText = string.Format(@"SELECT Quantity FROM TblPURPODetail WHERE Pid = {0}", poDetailPid);
          DataTable dtQtyPO = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtQtyPO != null)
          {
            qtyPO = DBConvert.ParseDouble(dtQtyPO.Rows[0]["Quantity"].ToString());
          }
        }
        long prDetailPid = DBConvert.ParseLong(row.Cells["PRDTPid"].Value.ToString());
        commandText = string.Format(@"SELECT PRDetailPid, Quantity, SUM(ISNULL(QT.QtyHaveDonePO, 0)) QtyHaveDonePO
                                      FROM TblPURPRDetail PRDT
                                      LEFT JOIN 
                                          (
                                            SELECT PODT.PRDetailPid, PODT.Quantity [QtyHaveDonePO]
                                            FROM TblPURPODetail PODT
                                            WHERE PODT.[Status] != 4
                                          ) QT ON PRDT.Pid = QT.PRDetailPid
                                      WHERE PRDT.Pid = {0} 
                                      GROUP BY PRDetailPid, Quantity", prDetailPid);

        DataTable dtCheckQty = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheckQty != null)
        {
          double qtyRemain = Math.Round(DBConvert.ParseDouble(dtCheckQty.Rows[0]["Quantity"].ToString()) - DBConvert.ParseDouble(dtCheckQty.Rows[0]["QtyHaveDonePO"].ToString()) + qtyPO, 4);
          if (qty > qtyRemain)
          {
            message = FunctionUtility.GetMessage("ERR0201");
            return false;
          }
        }
        double qtyDetail = 0;
        if (row.HasChild())
        {
          int count = row.ChildBands[0].Rows.Count;
          for (int j = 0; j < count; j++)
          {
            UltraGridRow rowDetail = row.ChildBands[0].Rows[j];
            if (DBConvert.ParseDouble(rowDetail.Cells["Qty"].Value.ToString()) != double.MinValue && DBConvert.ParseDouble(rowDetail.Cells["Qty"].Value.ToString()) > 0)
            {
              qtyDetail += DBConvert.ParseDouble(rowDetail.Cells["Qty"].Value.ToString());
            }
            else
            {
              message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The PO Detail Quantity");
              return false;
            }

            if (rowDetail.Cells["ExpectDate"].Value.ToString().Length == 0
                || (DateTime)rowDetail.Cells["ExpectDate"].Value == DateTime.MinValue)
            {
              message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Expect Date");
              return false;
            }
          }
          if (qtyDetail != qty)
          {
            message = FunctionUtility.GetMessage("ERR0201");
            return false;
          }
        }

        // Check Currency
        if (DBConvert.ParseLong(row.Cells["Currency"].Value.ToString()) != DBConvert.ParseLong(ultCBCurrency.Value.ToString()))
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Currency Detail");
          return false;
        }

        //Check Over
        if (row.Cells["Over"].Value.ToString().Trim().Length > 0)
        {
          if (DBConvert.ParseDouble(row.Cells["Over"].Value.ToString()) < 0)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Over");
            return false;
          }
        }
        // Kiem Tra GroupPurchase Voi GroupInCharge Trong MaterialCode(Tru nhung hang service)
        long groupInchargeMat;
        if (this.lblStatus.Text == "New")
        {
          groupInchargeMat = groupPurchase;
        }
        else
        {
          groupInchargeMat = DBConvert.ParseLong(row.Cells["GroupIncharge"].Value.ToString());
        }
        commandText = string.Format(@"  SELECT *
                                        FROM
                                        (
                                          (
                                            SELECT MaterialCode
                                            FROM TblGNRMaterialInformation
                                            WHERE MaterialCode = '{0}' AND GroupIncharge = {1}
                                          )
                                          UNION
                                          (
                                            SELECT MaterialCode
                                            FROM TblGNRMaterialInformation MAT 
                                            INNER JOIN TblGNRMaterialGroup MAG ON MAT.[Group] = MAG.[Group]
                                            WHERE MAG.Warehouse = 4 AND MaterialCode = '{2}'
                                          )
                                        )AA", row.Cells["MaterialCode"].Value.ToString(), groupInchargeMat, row.Cells["MaterialCode"].Value.ToString());
        DataTable dtMain = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtMain == null || dtMain.Rows.Count == 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Group In Charge Code: " + row.Cells["MaterialCode"].Value.ToString() + "");
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Get PR Detail Qty
    /// </summary>
    /// <param name="prDetailPid"></param>
    /// <returns></returns>
    private double GetPRDetailQty(long prDetailPid)
    {
      double qty = 0;
      string commandText = string.Format("SELECT Quantity FROM TblPURPRDetail WHERE Pid = {0}", prDetailPid);
      object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
      if (obj != null)
      {
        qty = (double)obj;
      }
      return qty;
    }

    /// <summary>
    /// Get Current Exchange Rate
    /// </summary>
    /// <param name="currencyPid"></param>
    /// <returns></returns>
    private double GetExchangeRate(long currencyPid)
    {
      double exchangeRate = double.MinValue;
      string commandText = string.Format("SELECT CurrentExchangeRate FROM TblPURCurrencyInfo WHERE Pid = {0}", currencyPid);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        exchangeRate = DBConvert.ParseDouble(dt.Rows[0][0].ToString());
      }
      return exchangeRate;
    }
    #endregion LoadData

    #region CheckValid & SaveData

    /// <summary>
    /// Check PONo Exist
    /// </summary>
    /// <param name="poNo"></param>
    /// <returns></returns>
    private bool CheckPONoExist(string poNo)
    {
      string commandText = string.Format("SELECT PONo FROM TblPURPOInformation WHERE PONo = '{0}'", poNo);
      DataTable dtCheckExist = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCheckExist != null && dtCheckExist.Rows.Count > 0)
      {
        return true;
      }
      return false;
    }

    /// <summary>
    /// Check Group Leader
    /// </summary>
    /// <param name="userPid"></param>
    /// <returns></returns>
    private bool CheckGroupLeader(long group, int userPid)
    {
      string commandText = string.Format("SELECT LeaderGroup FROM TblPURStaffGroup WHERE DeleteFlg = 0 AND Pid = {0}", group);
      DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCheck != null && dtCheck.Rows.Count > 0)
      {
        int leader = DBConvert.ParseInt(dtCheck.Rows[0][0].ToString());
        if (leader != userPid)
        {
          return false;
        }
      }
      return true;
    }

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
    /// Get Qty Done PO
    /// </summary>
    /// <param name="poDTPid"></param>
    /// <returns></returns>
    private double GetQtyDonePO(long prDetailPid)
    {
      double qty = 0;
      string commandText = string.Format("SELECT SUM(Quantity) FROM TblPURPODetail WHERE [Status] != 4 AND PRDetailPid = {0}", prDetailPid);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        qty = DBConvert.ParseDouble(dt.Rows[0][0].ToString());
      }
      qty = (qty == double.MinValue) ? 0 : qty;
      return qty;
    }

    /// <summary>
    /// Update PR And PR Detail Status
    /// </summary>
    private bool UpdatePRStatus()
    {
      // Input
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@PONo", DbType.AnsiString, 16, this.poNo);
      // Output
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, 0);
      // Exec
      DataBaseAccess.ExecuteStoreProcedure("spPURUpdateStatusWhenCreatePO_Update", inputParam, outputParam);
      long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Update MaxExpectDate PODetail
    /// </summary>
    private bool UpdateMaxExpectDatePODetail()
    {
      if (chkLock.Checked)
      {
        // Input
        DBParameter[] inputParam = new DBParameter[1];
        inputParam[0] = new DBParameter("@PONo", DbType.AnsiString, 16, this.poNo);
        // Output
        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, 0);
        // Exec
        DataBaseAccess.ExecuteStoreProcedure("spPURPODetailMaxExpectDate_Update", inputParam, outputParam);
        long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Order Remark
    /// </summary>
    /// <returns></returns>
    private bool SaveOrderRemark()
    {
      string orderRemark = string.Empty;
      DateTime date = DateTime.MinValue;
      DateTime dateHTML = DateTime.MinValue;
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@PONo", DbType.AnsiString, 16, this.poNo);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPURPOInfomationOrderRemark_Select", inputParam);
      date = DBConvert.ParseDateTime(dtSource.Rows[0]["CreateDate"].ToString(), USER_COMPUTER_FORMAT_DATETIME);
      string commandText = "SELECT Value FROM TblBOMCodeMaster WHERE [Group] = 9009";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null)
      {
        dateHTML = DBConvert.ParseDateTime(dt.Rows[0]["Value"].ToString(), USER_COMPUTER_FORMAT_DATETIME);
      }
      if (dtSource != null)
      {
        if (date > dateHTML)
        {
          if (dtSource.Rows[0]["OrderRemark"].ToString().Length > 0)
          {
            orderRemark = dtSource.Rows[0]["OrderRemark"].ToString();
          }
        }
        else
        {
          if (dtSource.Rows[0]["OrderRemark"].ToString().Length > 0)
          {
            orderRemark = this.StripHTML(dtSource.Rows[0]["OrderRemark"].ToString());
          }
        }
      }
      if (orderRemark.Length > 0)
      {
        DBParameter[] input = new DBParameter[2];
        input[0] = new DBParameter("@PONo", DbType.AnsiString, this.poNo);

        input[1] = new DBParameter("@OrderRemark", DbType.String, orderRemark);
        //inputParam[2] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
        DataBaseAccess.ExecuteStoreProcedure("spPURPOInformationOrderRemark_Update", input, outputParam);
        int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save PO Information
    /// </summary>
    /// <param name="poNo"></param>
    private string SavePOInfo()
    {
      //string poNo = txtPoNo.Text.Trim();
      long groupInCharge = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultGroupInCharge));
      long supplierPid = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultSupplier));
      long contactPerson = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultContactPerson));
      string orderNo = txtOrderNo.Text.Trim();
      string remark = txtRemark.Text.Trim();
      int userPid = SharedObject.UserInfo.UserPid;
      if (this.chkLock.Checked)
      {
        this.status = 3;
      }
      DBParameter[] input = new DBParameter[15];
      // Check PO Exist
      bool checkPO = this.CheckPONoExist(poNo);
      if (checkPO)
      {
        input[0] = new DBParameter("@PONo", DbType.AnsiString, 16, poNo);
        input[10] = new DBParameter("@UpdateBy", DbType.Int32, userPid);
      }
      input[1] = new DBParameter("@Status", DbType.Int32, this.status);
      input[2] = new DBParameter("@GroupInCharge", DbType.Int64, groupInCharge);
      if (chkLock.Checked)
      {
        input[3] = new DBParameter("@ApprovedBy", DbType.Int32, userPid);
        input[4] = new DBParameter("@ApprovedDate", DbType.DateTime, DateTime.Today);
      }
      input[5] = new DBParameter("@SupplierPid", DbType.Int64, supplierPid);
      if (contactPerson != long.MinValue)
      {
        input[6] = new DBParameter("@ContactPerson", DbType.Int64, contactPerson);
      }
      if (orderNo.Length > 0)
      {
        input[7] = new DBParameter("@OrderNo", DbType.AnsiString, 32, orderNo);
      }
      if (remark.Length > 0)
      {
        input[8] = new DBParameter("@Remark", DbType.String, 512, remark);
      }
      input[9] = new DBParameter("@CreateBy", DbType.Int32, userPid);
      if (ultCBPaymentForPurchaser.Value != null)
      {
        input[11] = new DBParameter("@PaymentForPurchaser", DbType.Int32, DBConvert.ParseInt(ultCBPaymentForPurchaser.Value.ToString()));
      }
      if (ultCBPurchaser.Value != null)
      {
        input[12] = new DBParameter("@Purchaser", DbType.Int32, DBConvert.ParseInt(ultCBPurchaser.Value.ToString()));
      }

      // Total Price
      double totalPrice = 0;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        double qty = DBConvert.ParseDouble(row.Cells["Quantity"].Value.ToString());
        double price = DBConvert.ParseDouble(row.Cells["Price"].Value.ToString());
        long currencyPid = DBConvert.ParseLong(row.Cells["Currency"].Value.ToString());
        double exchangeRate = double.MinValue;
        if (currencyPid != long.MinValue)
        {
          exchangeRate = this.GetExchangeRate(currencyPid);
        }
        double totalVAT = 0;
        if (DBConvert.ParseDouble(row.Cells["VAT"].Value.ToString()) != double.MinValue)
        {
          totalVAT = (DBConvert.ParseDouble(row.Cells["VAT"].Value.ToString()) * qty * price * exchangeRate) / 100;
        }
        totalPrice += (qty * price * exchangeRate) + totalVAT;
      }
      // Cong Them AdditionPrice
      for (int j = 0; j < ultAddition.Rows.Count; j++)
      {
        UltraGridRow row = ultAddition.Rows[j];
        double qtyAddition = DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString());
        double priceAddition = DBConvert.ParseDouble(row.Cells["Price"].Value.ToString());
        long currencyPidAddition = DBConvert.ParseLong(row.Cells["CurrencyPid"].Value.ToString());
        double exchangeRateAddition = double.MinValue;
        if (currencyPidAddition != long.MinValue)
        {
          exchangeRateAddition = this.GetExchangeRate(currencyPidAddition);
        }
        totalPrice += (qtyAddition * priceAddition * exchangeRateAddition);
      }
      input[13] = new DBParameter("@TotalPrice", DbType.Double, totalPrice);
      if (ultCBCurrency.Value != null)
      {
        input[14] = new DBParameter("@CurrencyPid", DbType.Int64, DBConvert.ParseLong(ultCBCurrency.Value.ToString()));
      }

      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 16, string.Empty) };
      DataBaseAccess.ExecuteStoreProcedure(SP_PUR_POINFO_EDIT, input, output);
      return output[0].Value.ToString().Trim();
    }

    /// <summary>
    /// Save PO Detail
    /// </summary>
    /// <param name="poNo"></param>
    /// <param name="isManager"></param>
    private bool SavePODetail(string poNo, bool isManager)
    {
      foreach (long detailPid in this.listDeletedDetailPid)
      {
        long prDTPid = long.MinValue;
        string commandText = string.Format("SELECT PRDetailPid FROM TblPURPODetail WHERE Pid = {0}", detailPid);
        object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
        if (obj != null)
        {
          prDTPid = (long)obj;
        }
        if (prDTPid != long.MinValue)
        {
          this.listDeletedPRDetailPid.Add(prDTPid);
        }

        // Delete PO Detail
        DBParameter[] inputDeleteDetail = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, detailPid) };
        DBParameter[] outputDeleteDetail = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

        DataBaseAccess.ExecuteStoreProcedure(SP_PUR_PODETAIL_DELETE, inputDeleteDetail, outputDeleteDetail);
        long resultDelete = DBConvert.ParseLong(outputDeleteDetail[0].Value.ToString());
        if (resultDelete == 0)
        {
          return false;
        }
      }
      if (ultData.Rows.Count > 0)
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];
          long prDetail = DBConvert.ParseLong(row.Cells["PRDTPid"].Value.ToString());
          int statusDetail = 0;
          if (chkLock.Checked)
          {
            statusDetail = 1;
          }
          long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          double qty = DBConvert.ParseDouble(row.Cells["Quantity"].Value.ToString());
          double price = DBConvert.ParseDouble(row.Cells["Price"].Value.ToString());
          long currencyPid = DBConvert.ParseLong(row.Cells["Currency"].Value.ToString());
          double exchangeRate = double.MinValue;
          if (currencyPid != long.MinValue)
          {
            exchangeRate = this.GetExchangeRate(currencyPid);
          }
          double vat = DBConvert.ParseDouble(row.Cells["VAT"].Value.ToString());
          double over = DBConvert.ParseDouble(row.Cells["Over"].Value.ToString());
          string remark = row.Cells["Remark"].Value.ToString().Trim();
          string sicentificName = row.Cells["SicentificName"].Value.ToString().Trim();
          string havestedForest = row.Cells["HavestedForest"].Value.ToString().Trim();
          string havestedCountry = row.Cells["HavestedCountry"].Value.ToString().Trim();

          DBParameter[] input = new DBParameter[14];
          if (pid != 0)
          {
            input[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          input[1] = new DBParameter("@PONo", DbType.AnsiString, 16, poNo);
          input[2] = new DBParameter("@PRDetailPid", DbType.Int64, prDetail);
          input[3] = new DBParameter("@Status", DbType.Int32, statusDetail);
          input[4] = new DBParameter("@Quantity", DbType.Double, qty);
          if (price != double.MinValue)
          {
            input[5] = new DBParameter("@Price", DbType.Double, price);
          }
          if (currencyPid != long.MinValue)
          {
            input[6] = new DBParameter("@CurrencyPid", DbType.Int64, currencyPid);
          }
          if (exchangeRate != double.MinValue)
          {
            input[7] = new DBParameter("@ExchangeRate", DbType.Double, exchangeRate);
          }
          if (vat != double.MinValue)
          {
            input[8] = new DBParameter("@VAT", DbType.Double, vat);
          }
          if (remark.Length > 0)
          {
            input[9] = new DBParameter("@Remark", DbType.String, remark);
          }
          if (over != double.MinValue)
          {
            input[10] = new DBParameter("Over", DbType.Double, over);
          }
          if (sicentificName.Length > 0)
          {
            input[11] = new DBParameter("@SicentificName", DbType.String, sicentificName);
          }
          if (havestedForest.Length > 0)
          {
            input[12] = new DBParameter("@HavestedForest", DbType.String, havestedForest);
          }
          if (havestedCountry.Length > 0)
          {
            input[13] = new DBParameter("@HavestedCountry", DbType.String, havestedCountry);
          }

          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(SP_PUR_PODETAIL_EDIT, input, output);
          long poDetailPid = DBConvert.ParseLong(output[0].Value.ToString());
          if (poDetailPid == 0)
          {
            return false;
          }
          else
          {
            foreach (long schedulePid in this.listDeletedSchedulePid)
            {
              DBParameter[] inputDeleteSchedule = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, schedulePid) };
              DBParameter[] outputDeleteSchedule = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

              DataBaseAccess.ExecuteStoreProcedure(SP_PUR_PODETAILSCHEDULE_DELETE, inputDeleteSchedule, outputDeleteSchedule);
              long resultDelete = DBConvert.ParseLong(outputDeleteSchedule[0].Value.ToString());
              if (resultDelete == 0)
              {
                return false;
              }
            }

            int countChild = row.ChildBands[0].Rows.Count;
            if (countChild > 0)
            {
              string arrayPODetailSchedulePid = string.Empty;
              for (int j = 0; j < countChild; j++)
              {
                UltraGridRow rowChild = row.ChildBands[0].Rows[j];
                long result = this.SavePODetailSchedule(rowChild, poDetailPid);
                if (result <= 0)
                {
                  return false;
                }
                else
                {
                  arrayPODetailSchedulePid = arrayPODetailSchedulePid + "~" + result.ToString();
                }
              }

              // Delete PODetailSchedulePid
              bool a = this.DeletePODetailSchedulePidOver(DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()), arrayPODetailSchedulePid);
              if (a == false)
              {
                return false;
              }
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Save PO Detail Schedule
    /// </summary>
    /// <param name="rowChild"></param>
    /// <param name="poDetailPid"></param>
    private long SavePODetailSchedule(UltraGridRow rowChild, long poDetailPid)
    {
      if (rowChild != null)
      {
        long pid = DBConvert.ParseLong(rowChild.Cells["Pid"].Value.ToString());
        double qtySCH = DBConvert.ParseDouble(rowChild.Cells["Qty"].Value.ToString());
        double receiptedQty = DBConvert.ParseDouble(rowChild.Cells["ReceiptedQty"].Value.ToString());
        DateTime expectDate;
        if (rowChild.Cells["ExpectDate"].Value.ToString().Trim().Length == 0)
        {
          expectDate = DateTime.MinValue;
        }
        else
        {
          expectDate = (DateTime)rowChild.Cells["ExpectDate"].Value;
        }

        DateTime confirmExpectDate;
        if (rowChild.Cells["ConfirmExpectDate"].Value.ToString().Trim().Length == 0)
        {
          confirmExpectDate = DateTime.MinValue;
        }
        else
        {
          confirmExpectDate = (DateTime)rowChild.Cells["ConfirmExpectDate"].Value;
        }

        DateTime latestDeliveryDate;
        if (rowChild.Cells["LatestDeliveryDate"].Value.ToString().Trim().Length == 0)
        {
          latestDeliveryDate = DateTime.MinValue;
        }
        else
        {
          latestDeliveryDate = (DateTime)rowChild.Cells["LatestDeliveryDate"].Value;
        }

        string contactNo = rowChild.Cells["ContractNo"].Value.ToString().Trim();
        string nameOfGoods = rowChild.Cells["NameOfGoods"].Value.ToString().Trim();
        string invoiceNo = rowChild.Cells["InvoiceNo"].Value.ToString().Trim();
        int etd1 = DBConvert.ParseInt(rowChild.Cells["ETD1"].Value.ToString());
        DateTime etd2;
        if (rowChild.Cells["ETD2"].Value.ToString().Trim().Length == 0)
        {
          etd2 = DateTime.MinValue;
        }
        else
        {
          etd2 = (DateTime)rowChild.Cells["ETD2"].Value;
        }

        DateTime eta;
        if (rowChild.Cells["ETA"].Value.ToString().Trim().Length == 0)
        {
          eta = DateTime.MinValue;
        }
        else
        {
          eta = (DateTime)rowChild.Cells["ETA"].Value;
        }

        DateTime timeOfReceivingDoc;
        if (rowChild.Cells["TimeOfReceivingDoc"].Value.ToString().Trim().Length == 0)
        {
          timeOfReceivingDoc = DateTime.MinValue;
        }
        else
        {
          timeOfReceivingDoc = (DateTime)rowChild.Cells["TimeOfReceivingDoc"].Value;
        }

        DateTime timeOfReceivingOriginal;
        if (rowChild.Cells["TimeOfReceivingOriginal"].Value.ToString().Trim().Length == 0)
        {
          timeOfReceivingOriginal = DateTime.MinValue;
        }
        else
        {
          timeOfReceivingOriginal = (DateTime)rowChild.Cells["TimeOfReceivingOriginal"].Value;
        }

        DateTime arrivalTimeToPort;
        if (rowChild.Cells["arrivalTimeToPort"].Value.ToString().Trim().Length == 0)
        {
          arrivalTimeToPort = DateTime.MinValue;
        }
        else
        {
          arrivalTimeToPort = (DateTime)rowChild.Cells["arrivalTimeToPort"].Value;
        }

        DateTime blDate;
        if (rowChild.Cells["BLDate"].Value.ToString().Trim().Length == 0)
        {
          blDate = DateTime.MinValue;
        }
        else
        {
          blDate = (DateTime)rowChild.Cells["BLDate"].Value;
        }

        string documentAt = rowChild.Cells["DocumentAt"].Value.ToString().Trim();

        DBParameter[] inputSCH = new DBParameter[18];
        if (pid != 0 && pid != long.MinValue)
        {
          inputSCH[0] = new DBParameter("@Pid", DbType.Int64, pid);
        }
        inputSCH[1] = new DBParameter("@PODetailPid", DbType.Int64, poDetailPid);
        inputSCH[2] = new DBParameter("@Quantity", DbType.Double, qtySCH);
        if (receiptedQty != double.MinValue)
        {
          inputSCH[3] = new DBParameter("@ReceiptedQty", DbType.Double, receiptedQty);
        }
        inputSCH[4] = new DBParameter("@ExpectDate", DbType.DateTime, expectDate);
        if (confirmExpectDate != DateTime.MinValue)
        {
          inputSCH[5] = new DBParameter("@ConfirmExpectDate", DbType.DateTime, confirmExpectDate);
        }
        if (latestDeliveryDate != DateTime.MinValue)
        {
          inputSCH[6] = new DBParameter("@LatestDeliveryDate", DbType.DateTime, latestDeliveryDate);
        }
        if (contactNo.Length > 0)
        {
          inputSCH[7] = new DBParameter("@ContractNo", DbType.AnsiString, 32, contactNo);
        }
        if (nameOfGoods.Length > 0)
        {
          inputSCH[8] = new DBParameter("@NameOfGoods", DbType.AnsiString, 200, nameOfGoods);
        }
        if (invoiceNo.Length > 0)
        {
          inputSCH[9] = new DBParameter("@InvoiceNo", DbType.AnsiString, 32, invoiceNo);
        }
        if (etd1 != int.MinValue)
        {
          inputSCH[10] = new DBParameter("@ETD1", DbType.Int32, etd1);
        }
        if (etd2 != DateTime.MinValue)
        {
          inputSCH[11] = new DBParameter("@ETD2", DbType.DateTime, etd2);
        }
        if (eta != DateTime.MinValue)
        {
          inputSCH[12] = new DBParameter("@ETA", DbType.DateTime, eta);
        }
        if (timeOfReceivingDoc != DateTime.MinValue)
        {
          inputSCH[13] = new DBParameter("@TimeOfReceivingDoc", DbType.DateTime, timeOfReceivingDoc);
        }
        if (timeOfReceivingOriginal != DateTime.MinValue)
        {
          inputSCH[14] = new DBParameter("@TimeOfReceivingOriginal", DbType.DateTime, timeOfReceivingOriginal);
        }
        if (arrivalTimeToPort != DateTime.MinValue)
        {
          inputSCH[15] = new DBParameter("@ArrivalTimeToPort", DbType.DateTime, arrivalTimeToPort);
        }
        if (blDate != DateTime.MinValue)
        {
          inputSCH[16] = new DBParameter("@BLDate", DbType.DateTime, blDate);
        }
        if (documentAt.Length > 0)
        {
          inputSCH[17] = new DBParameter("@DocumentAt", DbType.AnsiString, 24, documentAt);
        }

        DBParameter[] outputSCH = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure(SP_PUR_PODETAILSCHEDULE_EDIT, inputSCH, outputSCH);
        long result = DBConvert.ParseLong(outputSCH[0].Value.ToString());
        //if (result == 0)
        //{
        //  return false;
        //}
        return result;
      }
      return 0;
    }

    /// <summary>
    /// Delete PODetailSchedulePid Over
    /// </summary>
    /// <param name="poDetailPid"></param>
    /// <param name="array"></param>
    /// <returns></returns>
    private bool DeletePODetailSchedulePidOver(long poDetailPid, string stringData)
    {
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@PODetailPid", DbType.Int64, poDetailPid);
      input[1] = new DBParameter("@StringData", DbType.String, stringData);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int32, 0);
      DataBaseAccess.ExecuteStoreProcedure("spPURPRChangeRequestDatePODetailScheduleOver_Delete", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    private bool SaveData()
    {
      bool result = true;
      this.poNo = this.SavePOInfo();
      if (this.poNo.Length == 0)
      {
        return false;
      }

      // Save Order Remark
      result = this.SaveOrderRemark();
      if (result == false)
      {
        return false;
      }

      // Save PO Detail
      result = this.SavePODetail(this.poNo, this.isManager);
      if (result == false)
      {
        return false;
      }

      // Update Addition
      result = this.SaveUpdatePrAddition();
      if (result == false)
      {
        return false;
      }

      // Update Addition
      result = this.UpdateMaxExpectDatePODetail();
      if (result == false)
      {
        return false;
      }

      // Update Status PR
      result = this.UpdatePRStatus();
      if (result == false)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Save Update Pr Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveUpdatePrAddition()
    {
      // Insert Addition Price PO From PR
      if (this.poNo.Length > 0)
      {
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@PONo", DbType.AnsiString, 16, this.poNo);
        if (this.listDeletedAddition.Length > 0)
        {
          inputParam[1] = new DBParameter("@ListPRDeleted", DbType.AnsiString, 1024, this.listDeletedAddition);
        }
        DataBaseAccess.ExecuteStoreProcedure("spPURAdditionPricePOFromPR_Insert", inputParam);
      }

      string result = string.Empty;
      string commandText = string.Empty;
      DataTable dt = new DataTable();
      for (int i = 0; i < ultAddition.Rows.Count; i++)
      {
        UltraGridRow row = ultAddition.Rows[i];
        string storeName = "spPURPURAdditionPrice_Edit";
        DBParameter[] inputParam = new DBParameter[8];
        // Update
        if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) != long.MinValue)
        {
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
          inputParam[2] = new DBParameter("@Name", DbType.String, row.Cells["Name"].Value.ToString());
          inputParam[3] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()));
          inputParam[4] = new DBParameter("@Price", DbType.Double, DBConvert.ParseDouble(row.Cells["Price"].Value.ToString()));
          if (row.Cells["Remark"].ToString().Length > 0)
          {
            inputParam[5] = new DBParameter("@Remark", DbType.String, row.Cells["Remark"].Value.ToString());
          }
          inputParam[6] = new DBParameter("@CurrencyPid", DbType.Int32, DBConvert.ParseInt(row.Cells["CurrencyPid"].Value.ToString()));
          inputParam[7] = new DBParameter("@ExchangeRate", DbType.Double, DBConvert.ParseDouble(row.Cells["ExchangeRate"].Value.ToString()));
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);

          if (DBConvert.ParseInt(outputParam[0].Value.ToString()) == 0)
          {
            return false;
          }
        }
        // Insert
        else
        {
          inputParam[1] = new DBParameter("@PRPONo", DbType.String, this.poNo);
          inputParam[2] = new DBParameter("@Name", DbType.String, row.Cells["Name"].Value.ToString());
          inputParam[3] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()));
          inputParam[4] = new DBParameter("@Price", DbType.Double, DBConvert.ParseDouble(row.Cells["Price"].Value.ToString()));
          if (row.Cells["Remark"].ToString().Length > 0)
          {
            inputParam[5] = new DBParameter("@Remark", DbType.String, row.Cells["Remark"].Value.ToString());
          }
          inputParam[6] = new DBParameter("@CurrencyPid", DbType.Int32, DBConvert.ParseInt(row.Cells["CurrencyPid"].Value.ToString()));
          inputParam[7] = new DBParameter("@ExchangeRate", DbType.Double, DBConvert.ParseDouble(row.Cells["ExchangeRate"].Value.ToString()));
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          if (DBConvert.ParseInt(outputParam[0].Value.ToString()) == 0)
          {
            return false;
          }
        }
      }
      return true;
    }
    #endregion CheckValid & SaveData

    #region Event
    /// <summary>
    /// Save Main Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.ultData.Rows.Count == 0 && this.poNo.Length == 0)
      {
        return;
      }

      bool success;
      string message = string.Empty;

      // Check PO Info
      success = this.CheckValidPOInformationInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      // Check PO Detail
      success = this.CheckValidPODetailInformation(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      // Check Addition Info
      success = this.CheckValidPRDetailAdditionInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      // Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
        return;
      }
      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Check Valid PR Detail Info
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidPRDetailAdditionInfo(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      DataTable dtCheck = new DataTable();
      for (int i = 0; i < ultAddition.Rows.Count; i++)
      {
        UltraGridRow row = ultAddition.Rows[i];
        if (row.Cells["Name"].Value.ToString().Length == 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Addition/Name");
          return false;
        }

        if (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) == double.MinValue)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Addition/Qty");
          return false;
        }

        if (DBConvert.ParseDouble(row.Cells["Price"].Value.ToString()) == double.MinValue)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Addition/Price");
          return false;
        }

        if (DBConvert.ParseInt(row.Cells["CurrencyPid"].Value.ToString()) == int.MinValue)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Addition/Currency");
          return false;
        }

        if (DBConvert.ParseDouble(row.Cells["ExchangeRate"].Value.ToString()) == double.MinValue)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Addition/ExchangeRate");
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Check Select All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      bool flagSelect = false;
      for (int i = 0; i < ultPR.Rows.Count; i++)
      {
        UltraGridRow row = ultPR.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 0)
        {
          flagSelect = true;
          break;
        }
      }

      for (int i = 0; i < ultPR.Rows.Count; i++)
      {
        UltraGridRow row = ultPR.Rows[i];
        if (flagSelect == true)
        {
          row.Cells["Selected"].Value = 1;
        }
        else
        {
          row.Cells["Selected"].Value = 0;
        }
      }
    }

    /// <summary>
    /// Make PO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnMakePO_Click(object sender, EventArgs e)
    {
      bool flag = false;
      for (int i = 0; i < ultPR.Rows.Count; i++)
      {
        UltraGridRow row = ultPR.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          flag = true;
        }
      }

      if (flag == false)
      {
        return;
      }

      bool success;
      string message = string.Empty;

      success = this.CheckValidMakePOInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      string commandText = string.Empty;
      for (int i = 0; i < ultPR.Rows.Count; i++)
      {
        UltraGridRow row = ultPR.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          long prdtPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          double qtyPOBought = (DBConvert.ParseDouble(row.Cells["QtyHaveDonePO"].Value.ToString()) == double.MinValue) ? 0 : DBConvert.ParseDouble(row.Cells["QtyHaveDonePO"].Value.ToString());
          commandText = string.Empty;
          commandText = string.Format(@"SELECT PR.Pid, PR.PRNo, PR.MaterialCode, PR.NameEN, PR.Unit,
                                            PR.Quantity, PR.Currency, PR.Price, PR.VAT, PR.Remark,
                                            CASE WHEN PR.ChangeDate IS NULL THEN PR.RequestDate
                                            ELSE PR.ChangeDate END RequestDate, PR.CommittedDate, PR.DefaultDeliveryDate
                                        FROM 
                                        (
                                            SELECT PRDT.Pid, PRDT.PRNo, PRDT.MaterialCode, MAT.NameEN, UNI.Symbol Unit,
                                                PRDT.Quantity - ISNULL(PRDT.QtyCancel, 0)	Quantity,
                                                CONVERT(VARCHAR, PRDT.RequestDate, 103) RequestDate, CONVERT(VARCHAR, PRDT.ConfirmedDateDelivery, 103) CommittedDate,
                                            CONVERT(VARCHAR, DATEADD(DAY, -2, PRDT.ConfirmedDateDelivery), 103) DefaultDeliveryDate,
                                                PRDT.CurrencyPid Currency, PRDT.Price, PRDT.VAT, PRDT.Remark,
                                                STUFF((SELECT ', ' + CONVERT(varchar, PROL.Qty) + ' : '+ CONVERT(varchar, PROL.RequestDate, 103), ''
                                                                 FROM TblPURPRDetail DT
                                                                    LEFT JOIN
                                                                    (
                                                                        SELECT PR.PROnlineNo, PRDT.MaterialCode, TOC.Qty, TOC.RequestDate
                                                                        FROM TblPURPRDetailStockBalance TOC
                                                                            INNER JOIN TblPURPROnlineDetailInformation PRDT ON TOC.PRDetailPid = PRDT.PID
                                                                            INNER JOIN TblPURPROnlineInformation PR ON PR.PID  = PRDT.PROnlinePid
                                                                    )PROL ON PROL.PROnlineNo  = DT.PRNo
                                                                            AND PROL.MaterialCode = DT.MaterialCode
                                                     WHERE DT.PRNo = PRDT.PRNo
                                                        AND DT.MaterialCode = PRDT.MaterialCode
                                                                   FOR XML
                                                                       PATH ('')), 1, 2, '') AS ChangeDate	
                                            FROM TblPURPRDetail PRDT 
                                                LEFT JOIN TblGNRMaterialInformation MAT ON PRDT.MaterialCode = MAT.MaterialCode 
                                                LEFT JOIN TblPURCurrencyInfo CUR ON PRDT.CurrencyPid = CUR.Pid 
                                                LEFT JOIN TblGNRMaterialUnit UNI ON MAT.Unit = UNI.Pid 
	                                        WHERE PRDT.Pid = {0}
                                        )PR", prdtPid);

          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dt != null && dt.Rows.Count > 0)
          {
            dsMain = (DataSet)this.ultData.DataSource;
            DataRow drRow = dsMain.Tables[0].NewRow();
            drRow["Pid"] = 0;
            drRow["PRDTPid"] = DBConvert.ParseLong(dt.Rows[0]["Pid"].ToString());
            drRow["PRNo"] = dt.Rows[0]["PRNo"].ToString();
            drRow["MaterialCode"] = dt.Rows[0]["MaterialCode"].ToString();
            drRow["NameEN"] = dt.Rows[0]["NameEN"].ToString();
            drRow["Unit"] = dt.Rows[0]["Unit"].ToString();
            drRow["RequestDate"] = dt.Rows[0]["RequestDate"].ToString();
            drRow["CommittedDate"] = dt.Rows[0]["CommittedDate"].ToString();
            drRow["Quantity"] = DBConvert.ParseDouble(dt.Rows[0]["Quantity"].ToString()) - qtyPOBought;
            drRow["Currency"] = DBConvert.ParseLong(dt.Rows[0]["Currency"].ToString());
            drRow["Price"] = DBConvert.ParseDouble(dt.Rows[0]["Price"].ToString());
            if (DBConvert.ParseDouble(dt.Rows[0]["VAT"].ToString()) != double.MinValue)
            {
              drRow["VAT"] = DBConvert.ParseDouble(dt.Rows[0]["VAT"].ToString());
            }
            drRow["Remark"] = dt.Rows[0]["Remark"].ToString();
            dsMain.Tables[0].Rows.Add(drRow);

            DataRow drRowChild = dsMain.Tables[1].NewRow();
            drRowChild["Pid"] = 0;
            drRowChild["PODetatlPid"] = 0;
            drRowChild["PRDTPid"] = DBConvert.ParseLong(dt.Rows[0]["Pid"].ToString());
            drRowChild["PRDTPid"] = DBConvert.ParseLong(dt.Rows[0]["Pid"].ToString());
            drRowChild["Qty"] = DBConvert.ParseDouble(dt.Rows[0]["Quantity"].ToString());
            if (DBConvert.ParseDateTime(dt.Rows[0]["DefaultDeliveryDate"].ToString(), DaiCo.Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
            {
              drRowChild["ExpectDate"] = DBConvert.ParseDateTime(dt.Rows[0]["DefaultDeliveryDate"].ToString(), DaiCo.Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }

            dsMain.Tables[1].Rows.Add(drRowChild);
            this.ultData.DataSource = dsMain;
          }
        }
      }
    }

    /// <summary>
    /// Check Valid Make PO
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidMakePOInfo(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      DataSet ds = (DataSet)this.ultData.DataSource;
      if (ds != null)
      {
        DataTable dtCheck = ds.Tables[0];
        if (dtCheck.Rows.Count > 0)
        {
          for (int i = 0; i < ultPR.Rows.Count; i++)
          {
            UltraGridRow row = ultPR.Rows[i];
            if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
            {
              DataRow[] foundRow = dtCheck.Select("PRDTPid =" + DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()) + " AND MaterialCode = '" + row.Cells["MaterialCode"].Value.ToString().Trim() + "'");
              if (foundRow.Length > 0)
              {
                message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Gird Data PO");
                return false;
              }
            }
          }
        }
      }

      // PR Detail
      DataTable dtPR = (DataTable)ultPR.DataSource;
      if (dtPR != null)
      {
        foreach (DataRow row in dtPR.Rows)
        {
          if (DBConvert.ParseInt(row["Selected"].ToString()) == 1)
          {
            double qty = DBConvert.ParseDouble(row["Quantity"].ToString());
            double qtyPOBought = (DBConvert.ParseDouble(row["QtyHaveDonePO"].ToString()) == double.MinValue) ? 0 : DBConvert.ParseDouble(row["QtyHaveDonePO"].ToString());
            if (qty - qtyPOBought == 0)
            {
              message = string.Format(FunctionUtility.GetMessage("ERR0203"));
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Format Gird Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["PRDTPid"].Hidden = true;
      e.Layout.Bands[0].Columns["StatusPODetail"].Hidden = true;
      e.Layout.Bands[0].Columns["GroupIncharge"].Hidden = true;

      e.Layout.Bands[0].Columns["PRNo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PRNo"].Header.Caption = "PR No";
      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["RequestDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Currency"].ValueList = udrpCurrency;
      e.Layout.Bands[1].Columns["ETD1"].ValueList = udrpETD;
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "Material Name";
      e.Layout.Bands[0].Columns["RequestDate"].Header.Caption = "Request Date";
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Quantity"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["VAT"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["VAT"].Header.Caption = "VAT(%)";
      e.Layout.Bands[0].Columns["Over"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Over"].Header.Caption = "Over(%)";
      e.Layout.Bands[0].Columns["Price"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["MaxExpectDate"].Header.Caption = "Original ExpectDate";
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["PODetatlPid"].Hidden = true;
      e.Layout.Bands[1].Columns["PRDTPid"].Hidden = true;
      e.Layout.Bands[1].Columns["ReceiptedQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["ReceiptedQty"].Header.Caption = "Receipted Qty";
      e.Layout.Bands[1].Columns["ReceiptedQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["QtyCancel"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["QtyCancel"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[1].Columns["ExpectDate"].Header.Caption = "Expected Date";
      e.Layout.Bands[1].Columns["ExpectDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["ExpectDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;

      e.Layout.Bands[1].Columns["ConfirmExpectDate"].Header.Caption = "Confirm Expected Date";
      e.Layout.Bands[1].Columns["ConfirmExpectDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["ConfirmExpectDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;

      e.Layout.Bands[1].Columns["LatestDeliveryDate"].Header.Caption = "Lastest Delivery Date";
      e.Layout.Bands[1].Columns["LatestDeliveryDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["LatestDeliveryDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["LatestDeliveryDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;

      e.Layout.Bands[1].Columns["ContractNo"].Header.Caption = "Contract No";
      e.Layout.Bands[1].Columns["NameOfGoods"].Header.Caption = "Name Of Goods";
      e.Layout.Bands[1].Columns["InvoiceNo"].Header.Caption = "Invoice No";
      e.Layout.Bands[1].Columns["ETD1"].Header.Caption = "Type Of ETD";
      e.Layout.Bands[1].Columns["ETD2"].Header.Caption = "ETD";
      e.Layout.Bands[1].Columns["ETD2"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["ETD2"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["ETD2"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[1].Columns["Qty"].Header.Caption = "Quantity";
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[1].Columns["ETA"].Header.Caption = "ETA";
      e.Layout.Bands[1].Columns["ETA"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["ETA"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;

      e.Layout.Bands[1].Columns["TimeOfReceivingDoc"].Header.Caption = "Time Of Receiving Doc";
      e.Layout.Bands[1].Columns["TimeOfReceivingDoc"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["TimeOfReceivingDoc"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;

      e.Layout.Bands[1].Columns["TimeOfReceivingOriginal"].Header.Caption = "Time Of Receiving Original";
      e.Layout.Bands[1].Columns["TimeOfReceivingOriginal"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["TimeOfReceivingOriginal"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;

      e.Layout.Bands[1].Columns["ArrivalTimeToPort"].Header.Caption = "Arrival Time To Port";
      e.Layout.Bands[1].Columns["ArrivalTimeToPort"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["ArrivalTimeToPort"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;

      e.Layout.Bands[1].Columns["BLDate"].Header.Caption = "BL Date";
      e.Layout.Bands[1].Columns["BLDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["BLDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[1].Columns["DocumentAt"].Header.Caption = "Document At";
      e.Layout.Bands[2].Columns["Pid"].Hidden = true;
      e.Layout.Bands[2].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[2].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[2].Columns["ReceivingNote"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[2].Columns["ReceivingNote"].Header.Caption = "REC.Note";
      e.Layout.Bands[2].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;

      if ((this.status <= 1 && this.isLeader) || (this.status <= 2 && this.isManager))
      {
        this.canUpdate = true;
      }

      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      if (this.status == 5 || this.status == 6)
      {
        e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      }
      e.Layout.Bands[1].Override.AllowDelete = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Currency"].CellActivation = (this.canUpdate) ? Activation.AllowEdit : Activation.ActivateOnly;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Format Grid PR
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultPR_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "Material Name";
      e.Layout.Bands[0].Columns["Quantity"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Quantity"].Header.Caption = "Qty";
      e.Layout.Bands[0].Columns["QtyHaveDonePO"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyHaveDonePO"].Header.Caption = "Qty Have Done PO";
      e.Layout.Bands[0].Columns["Currency"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Urgent"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["RequestDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["RequestDate"].Header.Caption = "Request Date";
      e.Layout.Bands[0].Columns["GroupName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["GroupName"].Header.Caption = "Group Name";
      e.Layout.Bands[0].Columns["VAT"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Price"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Price"].Format = "###,###.##";

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["Quantity"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyHaveDonePO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["VAT"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// List Pr Value Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultPRNo_ValueChanged(object sender, EventArgs e)
    {
      string value = string.Empty;
      if (ultPRNo.Value != null)
      {
        value = ultPRNo.Value.ToString().Trim();
      }
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@PRNo", DbType.AnsiString, 16, value) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPURPRDetailMakePOByPRNo", inputParam);
      if (dsSource != null)
      {
        this.ultPR.DataSource = dsSource.Tables[0];
        for (int i = 0; i < ultPR.Rows.Count; i++)
        {
          ultPR.Rows[i].Cells["Source"].Appearance.BackColor = Color.Yellow;
        }
      }
    }

    /// <summary>
    /// Supplier Change ==> Contact Person 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSupplier_ValueChanged(object sender, EventArgs e)
    {
      if (ultSupplier.SelectedRow != null)
      {
        string supplierPid = ultSupplier.SelectedRow.Cells["Pid"].Value.ToString().Trim();
        string commandText = "SELECT Pid, Name + '  ' + ISNULL(Mobile, '') Name FROM TblPURSupplierContactPerson WHERE SupplierPid = " + supplierPid;
        DataTable dt = DataBaseAccess.SearchCommandText(commandText).Tables[0];
        ultContactPerson.DataSource = dt;
        ultContactPerson.DisplayMember = "Name";
        ultContactPerson.ValueMember = "Pid";
        ultContactPerson.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultContactPerson.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
        ultContactPerson.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
        this.ultContactPerson.Value = string.Empty;
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
    /// Before Cell Update ==> Check Data Type ETD valid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      string commandText = string.Empty;
      DataTable dt = new DataTable();
      switch (columnName.ToLower())
      {
        case "etd1":
          commandText += "SELECT Code FROM TblBOMCodeMaster WHERE [Group] = 7013 AND DeleteFlag = 0 AND Value = '" + text + "'";
          dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

          if (dt.Rows.Count == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Type Of ETD");
            e.Cancel = true;
          }
          break;
        case "currency":
          if (text.Trim().Length == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Currency");
            e.Cancel = true;
            break;
          }
          string value = text.Split('-')[0].Trim();
          double exchange = DBConvert.ParseDouble(text.Split('-')[1].Trim());
          commandText = "SELECT Code FROM TblPURCurrencyInfo WHERE Code = '" + value + "' AND CurrentExchangeRate = " + exchange;
          dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

          if ((dt == null) || (dt != null && dt.Rows.Count == 0))
          {
            WindowUtinity.ShowMessageError("ERR0001", "Currency");
            e.Cancel = true;
          }
          break;
        case "qty":
          if (text.Length > 0)
          {
            if (DBConvert.ParseDouble(text) <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Qty ExpectDate");
              e.Cancel = true;
            }
            else if (DBConvert.ParseDouble(text) > 0)
            {
              double receipted = (DBConvert.ParseDouble(e.Cell.Row.Cells["ReceiptedQty"].Value.ToString()) > 0) ? DBConvert.ParseDouble(e.Cell.Row.Cells["ReceiptedQty"].Value.ToString()) : 0;
              double canceled = (DBConvert.ParseDouble(e.Cell.Row.Cells["QtyCancel"].Value.ToString()) > 0) ? DBConvert.ParseDouble(e.Cell.Row.Cells["QtyCancel"].Value.ToString()) : 0;

              if (DBConvert.ParseDouble(text) < receipted + canceled)
              {
                WindowUtinity.ShowMessageError("ERR0001", "Qty < Receipted + Cancel");
                e.Cancel = true;
              }
            }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Edit Cell ETD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "etd1":
          if (e.Cell.Row.Cells["ETD1"].Text.ToString() == "OTHER")
          {
            e.Cell.Row.Cells["ETD2"].Band.Columns["ETD2"].CellActivation = Activation.AllowEdit;
          }
          else
          {
            e.Cell.Row.Cells["ETD2"].Value = DBNull.Value;
            e.Cell.Row.Cells["ETD2"].Band.Columns["ETD2"].CellActivation = Activation.ActivateOnly;
          }

          break;
        case "quantity":
          e.Cell.Row.ChildBands[0].Rows[0].Cells["Qty"].Value = e.Cell.Row.Cells["Quantity"].Value;
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// ultData After Rows Deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingDetailPid)
      {
        this.listDeletedDetailPid.Add(pid);
      }

      foreach (long posdPid in this.listDeletingSchedulePid)
      {
        this.listDeletedSchedulePid.Add(posdPid);
      }
    }

    /// <summary>
    /// ultData Before Rows Deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingDetailPid = new ArrayList();
      this.listDeletingSchedulePid = new ArrayList();

      foreach (UltraGridRow row in e.Rows)
      {
        if (row.ChildBands[0].Rows.Count > 0)
        {
          long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          if (pid != long.MinValue)
          {
            this.listDeletingDetailPid.Add(pid);
          }
        }
        else
        {
          foreach (UltraGridRow rowChild in e.Rows)
          {
            long posdPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
            if (posdPid != long.MinValue)
            {
              this.listDeletingSchedulePid.Add(posdPid);
            }
          }
        }
      }
    }

    /// <summary>
    /// chkHideListPR Checked Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkHideListPR_CheckedChanged(object sender, EventArgs e)
    {
      if (this.grpPRInfo.Visible)
      {
        this.grpPRInfo.Visible = false;
      }
      else
      {
        this.grpPRInfo.Visible = true;
      }
    }

    private void ultAddition_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      foreach (UltraGridRow row in e.Rows)
      {
        // Luu danh sach PR Deleted
        this.listDeletedAddition = ',' + row.Cells["PRRelation"].Value.ToString();

        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          DBParameter[] inputParams = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
          DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          string storeName = string.Empty;
          storeName = "spPURAdditionPrice_Delete";

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParams, outputParams);
          if (DBConvert.ParseInt(outputParams[0].Value.ToString()) == 0)
          {
            WindowUtinity.ShowMessageError("ERR0004");
            this.LoadData();
            return;
          }
        }
      }
    }

    private void ultAddition_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "currencypid":
          try
          {
            e.Cell.Row.Cells["ExchangeRate"].Value = udrpAddCurrency.SelectedRow.Cells["ExchangeRate"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["ExchangeRate"].Value = DBNull.Value;
          }
          break;
      }
    }

    private void ultAddition_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Horizontal;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["PRRelation"].Hidden = true;
      e.Layout.Bands[0].Columns["Price"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["ExchangeRate"].Format = "###,###.##";

      e.Layout.Bands[0].Columns["ExchangeRate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CurrencyPid"].Header.Caption = "Currency";

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ExchangeRate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      if (this.canUpdate)
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

      e.Layout.Bands[0].Columns["CurrencyPid"].ValueList = udrpAddCurrency;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void chkAdddition_CheckedChanged(object sender, EventArgs e)
    {
      if (this.grpAdditionPrice.Visible)
      {
        this.grpAdditionPrice.Visible = false;
      }
      else
      {
        this.grpAdditionPrice.Visible = true;
      }
    }

    private void ultCBPaymentForPurchaser_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBPaymentForPurchaser.Value != null)
      {
        if (DBConvert.ParseInt(ultCBPaymentForPurchaser.Value.ToString()) == 1)
        {
          lblPurchaser.Visible = true;
          lblAA.Visible = true;
          ultCBPurchaser.Visible = true;
          this.LoadComboPurchaser();
        }
        else
        {
          ultCBPurchaser.Text = string.Empty;
          lblPurchaser.Visible = false;
          lblAA.Visible = false;
          ultCBPurchaser.Visible = false;
        }
      }
    }

    private void btnOrderRemark_Click(object sender, EventArgs e)
    {
      viewPUR_04_004 view = new viewPUR_04_004();
      view.PONo = this.poNo;
      view.status = this.status;
      WindowUtinity.ShowView(view, "ORDER REMARK", false, ViewState.ModalWindow, FormWindowState.Normal);
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      if (this.poNo.Length > 0)
      {
        viewPUR_04_007 view = new viewPUR_04_007();
        view.poNO = poNo;
        if (chkWithAddition.Checked)
        {
          view.chkWithAddition = true;
        }
        if (chkRefCode.Checked)
        {
          view.chkRefCode = true;
        }
        else
        {
          view.chkRefCode = false;
        }
        if (chkNameVn.Checked)
        {
          view.chkNameVn = true;
        }
        else
        {
          view.chkNameVn = false;
        }
        DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "Print PO", false, DaiCo.Shared.Utility.ViewState.ModalWindow);
        //  DBParameter[] input = new DBParameter[2];
        //  input[0] = new DBParameter("@PONo", DbType.AnsiString, 16, this.poNo);
        //  input[1] = new DBParameter("@Current", DbType.AnsiString, 16,DBConvert.ParseInt(this.ultCBCurrency.Value.ToString()));
        //  DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURRPTPOInfomation_Select", input);
        //  if (ds != null)
        //  {
        //    dsPURPOInfomation dsSource = new dsPURPOInfomation();
        //    dsSource.Tables["dtPOInfo"].Merge(ds.Tables[0]);
        //    dsSource.Tables["dtPODetail"].Merge(ds.Tables[1]);
        //    if (chkWithAddition.Checked)
        //    {
        //      dsSource.Tables["dtPOAdditionPrice"].Merge(ds.Tables[2]);
        //    }
        //    dsSource.Tables["dtListPR"].Merge(ds.Tables[3]);

        //    DaiCo.Shared.View_Report report = null;
        //    cptPURPOInformation cpt = new cptPURPOInformation();
        //    double totalAmount = 0;
        //    double totalAdditionPrice = 0;
        //    double totalVAT = 0;
        //    double total = 0;
        //    string orderRemark = string.Empty;

        //    // Total Amount
        //    for (int i = 0; i < dsSource.Tables["dtPODetail"].Rows.Count; i++)
        //    {
        //      DataRow row = dsSource.Tables["dtPODetail"].Rows[i];
        //      totalAmount = totalAmount + DBConvert.ParseDouble(row["Amount"].ToString());
        //      if (row["VAT"].ToString().Length > 0)
        //      {
        //        totalVAT = totalVAT + ((DBConvert.ParseDouble(row["Amount"].ToString()) * DBConvert.ParseDouble(row["VAT"].ToString())) / 100);
        //      }
        //    }

        //    // Total Addition Price
        //    for (int j = 0; j < dsSource.Tables["dtPOAdditionPrice"].Rows.Count; j++)
        //    {
        //      DataRow row = dsSource.Tables["dtPOAdditionPrice"].Rows[j];
        //      totalAdditionPrice = totalAdditionPrice + DBConvert.ParseDouble(row["Amount"].ToString());
        //    }
        //    // Total
        //    total = totalAmount + totalAdditionPrice + totalVAT;
        //    // Number To English
        //    string numberToEnglish = Shared.Utility.NumberToEnglish.ChangeNumericToWords(total) + "("+dsSource.Tables["dtPOInfo"].Rows[0]["Currency"].ToString() +")";

        //    // Order Remark
        //    if (dsSource.Tables["dtPOInfo"].Rows[0]["OrderRemark"].ToString().Length > 0)
        //    {
        //      DateTime date = DateTime.MinValue;
        //      DateTime dateHtml = DateTime.MinValue;
        //      date = DBConvert.ParseDateTime(dsSource.Tables["dtPOInfo"].Rows[0]["UpdateDate"].ToString(), USER_COMPUTER_FORMAT_DATETIME);
        //      string commandText1 = "SELECT Value FROM TblBOMCodeMaster WHERE [Group] = 9009";
        //      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText1);
        //      if (dt != null)
        //      {
        //        dateHtml = DBConvert.ParseDateTime(dt.Rows[0]["Value"].ToString(), USER_COMPUTER_FORMAT_DATETIME);
        //      }
        //      if (date >= dateHtml)
        //      {
        //        orderRemark = dsSource.Tables["dtPOInfo"].Rows[0]["OrderRemark"].ToString();
        //      }
        //      else
        //      {
        //        orderRemark = this.StripHTML(dsSource.Tables["dtPOInfo"].Rows[0]["OrderRemark"].ToString());
        //      }
        //      cpt.ReportFooterSection3.SectionFormat.EnableSuppress = false;
        //    }
        //    else
        //    {
        //      cpt.ReportFooterSection3.SectionFormat.EnableSuppress = true;
        //    }
        //    // Remark Detail
        //    if (dsSource.Tables["dtPOInfo"].Rows[0]["RemarkDetailEN"].ToString().Length > 0 ||
        //        dsSource.Tables["dtPOInfo"].Rows[0]["RemarkDetailVN"].ToString().Length > 0)
        //    {
        //      cpt.ReportFooterSection6.SectionFormat.EnableSuppress = false;
        //    }
        //    else
        //    {
        //      cpt.ReportFooterSection6.SectionFormat.EnableSuppress = true;
        //    }

        //    string companyName = string.Empty;
        //    string email = string.Empty;
        //    string website = string.Empty;
        //    string telephone = string.Empty;
        //    string taxCode = string.Empty;
        //    string accountNo = string.Empty;
        //    string fax = string.Empty;
        //    string address = string.Empty;
        //    string purchaseManager = string.Empty;
        //    string PrintDate = string.Empty;

        //    string commandText = "SELECT Code, ISNULL([Description], '') [Description] FROM TblBOMCodeMaster WHERE [Group] = 9008";
        //    DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        //    if (dtSource != null)
        //    {
        //      address = dtSource.Rows[0]["Description"].ToString();
        //      telephone = dtSource.Rows[1]["Description"].ToString();
        //      fax = dtSource.Rows[2]["Description"].ToString();
        //      email = dtSource.Rows[3]["Description"].ToString();
        //      website = dtSource.Rows[4]["Description"].ToString();
        //      taxCode = dtSource.Rows[5]["Description"].ToString();
        //      accountNo = dtSource.Rows[6]["Description"].ToString();
        //      companyName = dtSource.Rows[7]["Description"].ToString();
        //    }
        //    // PurchasManager
        //    string commandTex = "SELECT ManagerName FROM VHRDDepartmentInfo WHERE CODE = 'PUR'";
        //    DataTable dtPurchaseManagerName = DataBaseAccess.SearchCommandTextDataTable(commandTex);
        //    if (dtPurchaseManagerName != null)
        //    {
        //      purchaseManager = dtPurchaseManagerName.Rows[0]["ManagerName"].ToString();
        //    }
        //    //PrintDate
        //    PrintDate = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME) + " By: " +SharedObject.UserInfo.EmpName;
        //    // Hide, Unhide Header, Detail(When Check RefCode)
        //    if (chkRefCode.Checked)
        //    {
        //      cpt.PageHeaderSection1.SectionFormat.EnableSuppress = true;
        //      cpt.DetailSection1.SectionFormat.EnableSuppress = true;
        //      cpt.PageHeaderSection2.SectionFormat.EnableSuppress = false;
        //      cpt.DetailSection2.SectionFormat.EnableSuppress = false;
        //      if (chkNameVn.Checked)
        //      {
        //        cpt.DetailSection3.SectionFormat.EnableSuppress = true;
        //        cpt.DetailSection4.SectionFormat.EnableSuppress = false;
        //      }
        //      else
        //      {
        //        cpt.DetailSection3.SectionFormat.EnableSuppress = true;
        //        cpt.DetailSection4.SectionFormat.EnableSuppress = true;
        //      }
        //    }
        //    else
        //    {
        //      cpt.PageHeaderSection1.SectionFormat.EnableSuppress = false;
        //      cpt.DetailSection1.SectionFormat.EnableSuppress = false;
        //      cpt.PageHeaderSection2.SectionFormat.EnableSuppress = true;
        //      cpt.DetailSection2.SectionFormat.EnableSuppress = true;
        //      if (chkNameVn.Checked)
        //      {
        //        cpt.DetailSection3.SectionFormat.EnableSuppress = false;
        //        cpt.DetailSection4.SectionFormat.EnableSuppress = true;
        //      }
        //      else
        //      {
        //        cpt.DetailSection3.SectionFormat.EnableSuppress = true;
        //        cpt.DetailSection4.SectionFormat.EnableSuppress = true;
        //      }
        //    }
        //    // Hide, unhide AdditionPrice
        //    if (dsSource.Tables["dtPOAdditionPrice"].Rows.Count > 0)
        //    {
        //      cpt.ReportFooterSection1.SectionFormat.EnableSuppress = false;
        //    }
        //    else
        //    {
        //      cpt.ReportFooterSection1.SectionFormat.EnableSuppress = true;
        //    }

        //    cpt.SetDataSource(dsSource);

        //    cpt.SetParameterValue("address", address);
        //    cpt.SetParameterValue("telephone", telephone);
        //    cpt.SetParameterValue("email", email);
        //    cpt.SetParameterValue("website", website);
        //    cpt.SetParameterValue("taxCode", taxCode);
        //    cpt.SetParameterValue("accountNo", accountNo);
        //    cpt.SetParameterValue("fax", fax);
        //    cpt.SetParameterValue("companyName", companyName);
        //    // PurchaseManager Name
        //    cpt.SetParameterValue("purchaseManager", purchaseManager);
        //    // Total
        //    cpt.SetParameterValue("totalAmount", totalAmount);
        //    cpt.SetParameterValue("totalVAT", totalVAT);
        //    cpt.SetParameterValue("total", total);

        //    // Order Remark
        //    cpt.SetParameterValue("orderRemark", orderRemark);

        //    // Number To English
        //    cpt.SetParameterValue("numberToEnglish", numberToEnglish);

        //    //PrintDate
        //    cpt.SetParameterValue("PrintDate", PrintDate);

        //    if (chkRefCode.Checked)
        //    {
        //      cpt.SetParameterValue("checkRefCode", 1);
        //    }
        //    else
        //    {
        //      cpt.SetParameterValue("checkRefCode", 0);
        //    }

        //    //ControlUtility.ViewCrystalReport(cpt);

        //    report = new DaiCo.Shared.View_Report(cpt);
        //    report.IsShowGroupTree = false;
        //    report.ShowReport(Shared.Utility.ViewState.MainWindow);
        //  }
      }
    }

    private string StripHTML(string source)
    {
      try
      {
        string result;

        // Remove HTML Development formatting
        // Replace line breaks with space
        // because browsers inserts space
        result = source.Replace("\r", " ");
        // Replace line breaks with space
        // because browsers inserts space
        result = result.Replace("\n", " ");
        // Remove step-formatting
        result = result.Replace("\t", string.Empty);
        // Remove repeating spaces because browsers ignore them
        result = System.Text.RegularExpressions.Regex.Replace(result,
                                                              @"( )+", " ");

        // Remove the header (prepare first by clearing attributes)
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*head([^>])*>", "<head>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"(<( )*(/)( )*head( )*>)", "</head>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(<head>).*(</head>)", string.Empty,
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // remove all scripts (prepare first by clearing attributes)
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*script([^>])*>", "<script>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"(<( )*(/)( )*script( )*>)", "</script>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        //result = System.Text.RegularExpressions.Regex.Replace(result,
        //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
        //         string.Empty,
        //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"(<script>).*(</script>)", string.Empty,
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // remove all styles (prepare first by clearing attributes)
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*style([^>])*>", "<style>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"(<( )*(/)( )*style( )*>)", "</style>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(<style>).*(</style>)", string.Empty,
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // insert tabs in spaces of <td> tags
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*td([^>])*>", "\t",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // insert line breaks in places of <BR> and <LI> tags
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*br( )*>", "\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*li( )*>", "\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // insert line paragraphs (double line breaks) in place
        // if <P>, <DIV> and <TR> tags
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*div([^>])*>", "\r\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*tr([^>])*>", "\r\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*p([^>])*>", "\r\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // Remove remaining tags like <a>, links, images,
        // comments etc - anything that's enclosed inside < >
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<[^>]*>", string.Empty,
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // replace special characters:
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @" ", " ",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&bull;", " * ",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&lsaquo;", "<",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&rsaquo;", ">",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&trade;", "(tm)",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&frasl;", "/",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&lt;", "<",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&gt;", ">",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&copy;", "(c)",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&reg;", "(r)",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Remove all others. More can be added, see
        // http://hotwired.lycos.com/webmonkey/reference/special_characters/
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&(.{2,6});", string.Empty,
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // for testing
        //System.Text.RegularExpressions.Regex.Replace(result,
        //       this.txtRegex.Text,string.Empty,
        //       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // make line breaking consistent
        result = result.Replace("\n", "\r");

        // Remove extra line breaks and tabs:
        // replace over 2 breaks with 2 and over 4 tabs with 4.
        // Prepare first to remove any whitespaces in between
        // the escaped characters and remove redundant tabs in between line breaks
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\r)( )+(\r)", "\r\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\t)( )+(\t)", "\t\t",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\t)( )+(\r)", "\t\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\r)( )+(\t)", "\r\t",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Remove redundant tabs
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\r)(\t)+(\r)", "\r\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Remove multiple tabs following a line break with just one tab
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\r)(\t)+", "\r\t",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Initial replacement target string for line breaks
        string breaks = "\r\r\r";
        // Initial replacement target string for tabs
        string tabs = "\t\t\t\t\t";
        for (int index = 0; index < result.Length; index++)
        {
          result = result.Replace(breaks, "\r\r");
          result = result.Replace(tabs, "\t\t\t\t");
          breaks = breaks + "\r";
          tabs = tabs + "\t";
        }

        // That's it.
        return result;
      }
      catch
      {
        MessageBox.Show("Error");
        return source;
      }
    }

    private void ultCBCurrency_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBCurrency.Value != null)
      {
        if (DBConvert.ParseLong(ultCBCurrency.Value.ToString()) > 0)
        {
          txtExchangeRate.Text = ultCBCurrency.SelectedRow.Cells["CurrentExchangeRate"].Text;
          this.txtExchangeRate.ReadOnly = true;
          this.txtExchangeRate.Enabled = false;
        }
        else
        {
          txtExchangeRate.Text = string.Empty;
        }
      }
      else
      {
        txtExchangeRate.Text = string.Empty;
      }
    }

    /// <summary>
    ///  Copy Expect Date
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCopy_Click(object sender, EventArgs e)
    {
      if (ultDDExpectDate.Value != null)
      {
        if (DBConvert.ParseDateTime(ultDDExpectDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
        {
          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            UltraGridRow row = ultData.Rows[i];
            for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
            {
              UltraGridRow rowChild = row.ChildBands[0].Rows[j];
              rowChild.Cells["ExpectDate"].Value = DBConvert.ParseDateTime(ultDDExpectDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
          }
          WindowUtinity.ShowMessageSuccess("MSG0022");
          return;
        }
      }
    }

    /// <summary>
    /// New PO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNewPO_Click(object sender, EventArgs e)
    {
      viewPUR_04_003 view = new viewPUR_04_003();
      Shared.Utility.WindowUtinity.ShowView(view, "NEW PO", true, DaiCo.Shared.Utility.ViewState.MainWindow);
    }

    private void btnRemarkDetail_Click(object sender, EventArgs e)
    {
      viewPUR_04_006 view = new viewPUR_04_006();
      view.poNo = this.poNo;
      view.status = this.status;
      WindowUtinity.ShowView(view, "REMARK DETAIL", false, ViewState.ModalWindow, FormWindowState.Normal);
    }

    #endregion Event
  }
}
