/*
  Author      : Dang Xuan Truong
  Date        : 03/08/2012
  Description : Receiving Note For Service
*/
using DaiCo.Application;
using DaiCo.ERPProject.Warehouse.Reports;
using DaiCo.ERPProject.Warehouse.Service.DataSetSource;
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
  public partial class viewWHD_26_002 : MainUserControl
  {
    #region Field
    //REC
    public long receivingPid = long.MinValue;
    // Status
    int status = int.MinValue;
    // Delete
    private IList listDeletingDetailPid = new ArrayList();
    private IList listDeletedDetailPid = new ArrayList();
    private IList listMaterialDeletingPid = new ArrayList();
    private IList listMaterialDeletedPid = new ArrayList();
    // Store
    string SP_WHDReceivingInfomationByPONo = "spWHDReceivingNoteByReceivingPid_Select";
    string SP_WHDListMaterialMakeByPONo = "spWHDListMaterialMakeByPONoService";
    string SP_WHDReceivingInfomation_Insert = "spWHDReceivingInfomationService_Insert";
    string SP_WHDReceivingInfomation_Update = "spWHDReceivingInfomationService_Update";
    string SP_WHDReceivingDetail_Edit = "spWHDReceivingDetailService_Edit";
    string SP_WHDMapingPOReceiving_Edit = "spWHDMapingPOReceivingService_Edit";
    string SP_WHDReceivingDetail_Delete = "spWHDReceivingDetailService_Delete";
    string SP_WHDStatusPRPO_Update = "spWHDStatusPRPOService_Update";
    string SP_WHDListMaterialMakeByPONoMaterialCode_Check = "spWHDListMaterialMakeByPONoService_Check";

    //Data Set
    DataSet dsMain = new DataSet();
    private string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

    #endregion Field

    #region Init
    public viewWHD_26_002()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load ViewWHD_05_001
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_26_002_Load(object sender, EventArgs e)
    {
      // Load List Supplier
      this.LoadComboSupplier();

      this.LoadComboDepartment();
      // Load Data
      this.LoadData();
    }
    #endregion Init

    #region Function

    /// <summary>
    /// Auto Search Receiver
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtAutoSearch_Leave(object sender, EventArgs e)
    {
      if (DBConvert.ParseInt(this.txtAutoSearch.Text) != int.MinValue)
      {
        this.LoadComboReceiverByAutoSearch(DBConvert.ParseInt(this.txtAutoSearch.Text));
      }
      else
      {
        this.ultDepartment.Text = string.Empty;
      }
    }


    /// <summary>
    /// Load Ultra Combo Receiver By Auto Search
    /// </summary>
    private void LoadComboReceiverByAutoSearch(int idNhanVien)
    {
      string commandText = "SELECT Department FROM VHRNhanVien WHERE ID_NhanVien =" + idNhanVien;
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        ultDepartment.Value = dtSource.Rows[0]["Department"].ToString();
        ultReceiver.Value = idNhanVien;
      }
    }

    /// <summary>
    /// Update Auto Search 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultReceiver_ValueChanged(object sender, EventArgs e)
    {
      if (ultReceiver != null && ultReceiver.Value != null
                  && DBConvert.ParseInt(ultReceiver.Value.ToString()) != int.MinValue)
      {
        this.txtAutoSearch.Text = ultReceiver.Value.ToString();
      }
      else
      {
        this.txtAutoSearch.Text = string.Empty;
      }
    }

    /// <summary>
    /// Load UltraCombo Department
    /// </summary>
    private void LoadComboDepartment()
    {
      string commandText = "SELECT Department, Department + ' - ' + DeparmentName AS Name FROM VHRDDepartment";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDepartment.DataSource = dtSource;
      ultDepartment.DisplayMember = "Name";
      ultDepartment.ValueMember = "Department";
      ultDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDepartment.DisplayLayout.Bands[0].Columns["Name"].Width = 350;
      ultDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Receiver
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDepartment_TextChanged(object sender, EventArgs e)
    {
      // Load UltraCombo Receiver By Department
      this.LoadComboReceiverByDepartment();
    }

    /// <summary>
    /// Load UltraCombo Receiver By Department
    /// </summary>
    private void LoadComboReceiverByDepartment()
    {
      if (ultDepartment.Value != null && ultDepartment.Value.ToString().Length > 0)
      {
        string commandText = string.Empty;
        commandText += " SELECT ID_NhanVien, CAST(ID_NhanVien AS VARCHAR) + ' - ' + HoNV + ' ' + TenNV Name";
        commandText += " FROM VHRNhanVien";
        commandText += " WHERE Department = '" + ultDepartment.Value.ToString() + "'" + " AND Resigned = 0";

        System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

        ultReceiver.Text = string.Empty;
        ultReceiver.DataSource = dtSource;
        ultReceiver.DisplayMember = "Name";
        ultReceiver.ValueMember = "ID_NhanVien";
        ultReceiver.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultReceiver.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
        ultReceiver.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
      }
      else
      {
        ultReceiver.Text = string.Empty;
      }
    }

    /// <summary>
    /// Load UltraCombo Supplier
    /// </summary>
    private void LoadComboSupplier()
    {
      string commandText = string.Format(@" SELECT Pid, CAST(SupplierCode as nvarchar) + ' - ' + CAST(EnglishName as nvarchar) +  ' - ' + VietnameseName [EnglishName] FROM TblPURSupplierInfo
                                      WHERE Confirm = 2 AND DeleteFlg = 0 AND LEN(EnglishName) > 2 
                                      ORDER BY EnglishName");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        Utility.LoadUltraCombo(ultCBSupplier, dtSource, "Pid", "EnglishName", false, "Pid");
      }
    }

    /// <summary>
    /// Load UltraCombo List PO
    /// </summary>
    private void LoadComboListPO()
    {
      //ultCBPONo.Text = null;
      string commandText = string.Format(@" SELECT PO.PONo, PO.PONo + '_' + CONVERT(VARCHAR, PO.CreateDate, 103) Name
                                            FROM TblPURPOInformation PO
	                                            INNER JOIN TblPURSupplierInfo SUP ON SUP.Pid = PO.SupplierPid
                                            WHERE (PO.[Status] >= 2 AND PO.[Status] <= 4) AND SUP.Pid = {0} ORDER BY PO.PONo DESC
                                            ", ultCBSupplier.Value.ToString());
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        ultCBPONo.DataSource = dtSource;
        ultCBPONo.DisplayMember = "Name";
        ultCBPONo.ValueMember = "PONo";
        ultCBPONo.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultCBPONo.DisplayLayout.Bands[0].Columns["Name"].Width = 550;
        ultCBPONo.DisplayLayout.Bands[0].Columns["PONo"].Hidden = true;
      }
      else
      {
        ultCBPONo.Text = string.Empty;
      }
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ReceivingPid", DbType.Int64, this.receivingPid) };
      DataSet dsMain = DataBaseAccess.SearchStoreProcedure(SP_WHDReceivingInfomationByPONo, inputParam);
      DataTable dtReceivingInfo = dsMain.Tables[0];
      if (dtReceivingInfo.Rows.Count > 0)
      {
        DataRow row = dtReceivingInfo.Rows[0];
        txtReceivingNote.Text = dtReceivingInfo.Rows[0]["ReceivingCode"].ToString();
        txtTitle.Text = dtReceivingInfo.Rows[0]["Title"].ToString();
        txtCreateBy.Text = dtReceivingInfo.Rows[0]["CreateBy"].ToString();
        txtDate.Text = dtReceivingInfo.Rows[0]["CreateDate"].ToString();
        txtRemark.Text = dtReceivingInfo.Rows[0]["Remark"].ToString();
        txtTransportation.Text = dtReceivingInfo.Rows[0]["Transportation"].ToString();
        txtInvoiceNo.Text = dtReceivingInfo.Rows[0]["InvoiceNo"].ToString();
        txtAutoSearch.Text = dtReceivingInfo.Rows[0]["ReceivedBy"].ToString();
        ultDepartment.Value = dtReceivingInfo.Rows[0]["Department"].ToString();
        if (DBConvert.ParseInt(dtReceivingInfo.Rows[0]["ReceivedBy"].ToString()) != int.MinValue)
        {
          ultReceiver.Value = DBConvert.ParseInt(dtReceivingInfo.Rows[0]["ReceivedBy"].ToString());
        }
        ultInvoiceDate.Value = dtReceivingInfo.Rows[0]["InvoiceDate"].ToString();
        ultCBSupplier.Value = DBConvert.ParseLong(dtReceivingInfo.Rows[0]["Supplier"].ToString());
        this.status = DBConvert.ParseInt(dtReceivingInfo.Rows[0]["Status"].ToString());
      }
      else
      {
        DataTable dtREC = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHDGetNewReceivingNoForService('RC') NewRECCode");
        if ((dtREC != null) && (dtREC.Rows.Count > 0))
        {
          this.txtReceivingNote.Text = dtREC.Rows[0]["NewRECCode"].ToString();
          this.txtDate.Text = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
          this.txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          this.status = 0;
        }
      }
      // Load Detail
      dsWHDReceivingServiceDetail ds = new dsWHDReceivingServiceDetail();
      ds.Tables["dtReceivingDetailInfo"].Merge(dsMain.Tables[1]);
      ds.Tables["dtReceivingDetail"].Merge(dsMain.Tables[2]);
      ultRecInfomation.DataSource = ds;
      // Set Status control
      this.SetStatusControl();

      if (this.ultCBPONo.Value != null && this.ultCBPONo.Value.ToString().Length > 0)
      {
        this.ultCBPONo_ValueChanged(null, null);
      }
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      if (ultRecInfomation.Rows.Count > 0)
      {
        ultCBSupplier.ReadOnly = true;
      }
      else
      {
        ultCBSupplier.ReadOnly = false;
      }

      if (this.status == 0)
      {
        this.chkConfirm.Enabled = true;
        this.btnSave.Enabled = true;
      }
      else
      {
        txtTitle.ReadOnly = true;
        ultCBSupplier.ReadOnly = true;
        btnAdd.Enabled = false;
        chkConfirm.Checked = true;
        chkHide.Checked = true;
        txtRemark.ReadOnly = true;
        txtInvoiceNo.ReadOnly = true;
        ultDepartment.ReadOnly = true;
        txtTransportation.ReadOnly = true;
        ultInvoiceDate.ReadOnly = true;
        txtAutoSearch.ReadOnly = true;
        ultReceiver.ReadOnly = true;

        this.chkConfirm.Enabled = false;
        this.btnSave.Enabled = false;
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
      // Insert
      if (this.receivingPid == long.MinValue)
      {
        DBParameter[] inputParam = new DBParameter[10];
        // PrefixCode
        inputParam[0] = new DBParameter("@PrefixCode", DbType.AnsiString, 8, "RC");
        // Create By
        inputParam[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        // Title
        if (txtTitle.Text.Length > 0)
        {
          inputParam[2] = new DBParameter("@Title", DbType.String, txtTitle.Text);
        }
        // Status
        if (chkConfirm.Checked)
        {
          inputParam[3] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          inputParam[3] = new DBParameter("@Status", DbType.Int32, 0);
        }
        inputParam[4] = new DBParameter("@Supplier", DbType.Int64, DBConvert.ParseLong(ultCBSupplier.Value.ToString()));
        // Remark
        if (txtRemark.Text.Length > 0)
        {
          inputParam[5] = new DBParameter("@Remark", DbType.String, txtRemark.Text);
        }
        // InvoiceNo
        if (txtInvoiceNo.Text.Length > 0)
        {
          inputParam[6] = new DBParameter("@InvoiceNo", DbType.String, txtInvoiceNo.Text);
        }
        // Transportation
        if (txtTransportation.Text.Length > 0)
        {
          inputParam[7] = new DBParameter("@Transportation", DbType.String, txtTransportation.Text);
        }
        // InvoiceDate
        if (ultInvoiceDate.Value != null)
        {
          inputParam[8] = new DBParameter("@InvoiceDate", DbType.DateTime, DBConvert.ParseDateTime(ultInvoiceDate.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
        }
        // Receiver
        if (ultReceiver.Value != null)
        {
          inputParam[9] = new DBParameter("@ReceivedBy", DbType.Int32, DBConvert.ParseInt(ultReceiver.Value.ToString()));
        }

        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(SP_WHDReceivingInfomation_Insert, inputParam, outputParam);
        this.receivingPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
        long result = receivingPid;
        if (result <= 0)
        {
          return false;
        }
        return true;
      }
      else
      {
        DBParameter[] inputParam = new DBParameter[10];
        inputParam[0] = new DBParameter("@ReceivingPid", DbType.Int64, this.receivingPid);
        inputParam[1] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        if (txtTitle.Text.Length > 0)
        {
          inputParam[2] = new DBParameter("@Title", DbType.String, txtTitle.Text);
        }
        if (chkConfirm.Checked)
        {
          inputParam[3] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          inputParam[3] = new DBParameter("@Status", DbType.Int32, 0);
        }
        inputParam[4] = new DBParameter("@Supplier", DbType.Int64, DBConvert.ParseLong(ultCBSupplier.Value.ToString()));

        if (txtRemark.Text.Length > 0)
        {
          inputParam[5] = new DBParameter("@Remark", DbType.String, txtRemark.Text);
        }
        // InvoiceNo
        if (txtInvoiceNo.Text.Length > 0)
        {
          inputParam[6] = new DBParameter("@InvoiceNo", DbType.String, txtInvoiceNo.Text);
        }
        // Transportation
        if (txtTransportation.Text.Length > 0)
        {
          inputParam[7] = new DBParameter("@Transportation", DbType.String, txtTransportation.Text);
        }
        // InvoiceDate
        if (ultInvoiceDate.Value != null)
        {
          inputParam[8] = new DBParameter("@InvoiceDate", DbType.DateTime, DBConvert.ParseDateTime(ultInvoiceDate.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
        }
        // Receiver
        if (ultReceiver.Value != null)
        {
          inputParam[9] = new DBParameter("@ReceivedBy", DbType.Int32, DBConvert.ParseInt(ultReceiver.Value.ToString()));
        }
        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure(SP_WHDReceivingInfomation_Update, inputParam, outputParam);
        long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
        return true;
      }
    }

    /// <summary>
    /// Save Receiving Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveReceivingDetail()
    {
      if (ultPOInfomation.Rows.Count > 0)
      {
        for (int i = 0; i < ultPOInfomation.Rows.Count; i++)
        {
          UltraGridRow rowInfo = ultPOInfomation.Rows[i];
          if (DBConvert.ParseInt(rowInfo.Cells["Selected"].Value.ToString()) == 1)
          {
            if (DBConvert.ParseDouble(rowInfo.Cells["Qty"].Value.ToString()) != 0)
            {
              DBParameter[] inputParam = new DBParameter[4];
              inputParam[0] = new DBParameter("@ReceivingPid", DbType.Int64, this.receivingPid);
              inputParam[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, rowInfo.Cells["MaterialCode"].Value.ToString());
              inputParam[2] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(rowInfo.Cells["Qty"].Value.ToString()));
              inputParam[3] = new DBParameter("@PONo", DbType.AnsiString, 16, rowInfo.Cells["PONo"].Value.ToString());

              DBParameter[] outputParam = new DBParameter[1];
              outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

              DataBaseAccess.ExecuteStoreProcedure(SP_WHDReceivingDetail_Edit, inputParam, outputParam);
              long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
              if (result <= 0)
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
    /// Save Mapping PO
    /// </summary>
    /// <returns></returns>
    private bool SaveMappingPOReceiving()
    {
      // Delete Row
      foreach (string materialCode in this.listMaterialDeletedPid)
      {
        DBParameter[] input = new DBParameter[2];
        input[0] = new DBParameter("@MaterialCode", DbType.AnsiString, 255, materialCode);
        input[1] = new DBParameter("@ReceivingPid", DbType.Int64, this.receivingPid);
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure(SP_WHDReceivingDetail_Delete, input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result == 0)
        {
          return false;
        }
      }
      this.listMaterialDeletedPid.Clear();

      foreach (long recDetailPid in this.listDeletedDetailPid)
      {
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@ReceivingDetailPid", DbType.Int64, recDetailPid);
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure(SP_WHDReceivingDetail_Delete, input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result == 0)
        {
          return false;
        }
      }
      this.listDeletedDetailPid.Clear();
      // End Delete

      // Maping PRPO
      if (chkConfirm.Checked)
      {
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@ReceivingPid", DbType.Int64, this.receivingPid);
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure(SP_WHDMapingPOReceiving_Edit, input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
        else
        {
          // Update Status PR And PO
          DataBaseAccess.ExecuteStoreProcedure(SP_WHDStatusPRPO_Update, input, output);
          result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result <= 0)
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
    private bool CheckValidInfo(out string message)
    {
      message = string.Empty;
      // Check Supplier
      if (ultCBSupplier.Value == null)
      {
        message = "Supplier";
        return false;
      }
      // Check Received
      if (ultReceiver.Value == null)
      {
        message = "Received By";
        return false;
      }
      return true;
    }

    /// <summary>
    /// Check Valid Info PO
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidDetail(out string message)
    {
      message = string.Empty;

      for (int i = 0; i < ultPOInfomation.Rows.Count; i++)
      {
        UltraGridRow row = ultPOInfomation.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          if (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) != double.MinValue)
          {
            DBParameter[] inputParam = new DBParameter[2];
            inputParam[0] = new DBParameter("@PONo", DbType.String, row.Cells["PONo"].Value.ToString());
            inputParam[1] = new DBParameter("@MaterialCode", DbType.String, row.Cells["MaterialCode"].Value.ToString());
            DataTable dtCheck = DataBaseAccess.SearchStoreProcedureDataTable(this.SP_WHDListMaterialMakeByPONoMaterialCode_Check, inputParam);
            if (dtCheck != null && dtCheck.Rows.Count > 0)
            {
              if (DBConvert.ParseDouble(dtCheck.Rows[0]["Qty"].ToString())
                  < DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()))
              {
                message = "Qty <=  " + (DBConvert.ParseDouble(row.Cells["QtyPO"].Value.ToString()) -
                                        DBConvert.ParseDouble(row.Cells["QtyReceipted"].Value.ToString()) -
                                        DBConvert.ParseDouble(row.Cells["QtyCancel"].Value.ToString()));
                return false;
              }
            }
            else
            {
              message = "Data is invalid";
              return false;
            }
          }
          else
          {
            message = "Qty";
            return false;
          }
        }
      }
      return true;
    }

    #endregion Function

    #region Event
    /// <summary>
    /// Init PO Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultPOInfomation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;

      e.Layout.Bands[0].Columns["PONo"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialName"].Header.Caption = "NameEN";
      e.Layout.Bands[0].Columns["MaterialNameVn"].Header.Caption = "NameVN";
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["PONo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialNameVN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyPO"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyReceipted"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyCancel"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyReceipted"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyCancel"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyPO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    /// <summary>
    /// Init Receivinf Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultRecInfomation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;

      e.Layout.Bands[0].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameVN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["TotalQty"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["TotalQty"].MinWidth = 100;
      e.Layout.Bands[0].Columns["NameVN"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["NameVN"].MinWidth = 200;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 100;

      e.Layout.Bands[1].Columns["MaterialCode"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["ReceivingCodePid"].Hidden = true;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      if (this.status == 1)
      {
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
        e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    ///  Add
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckValidInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      success = this.CheckValidDetail(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        this.LoadData();
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
      this.ultCBPONo_ValueChanged(sender, e);
      this.LoadData();
    }
    /// <summary>
    /// Value Change PONo
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCBPONo_ValueChanged(object sender, EventArgs e)
    {
      string value = string.Empty;
      if (ultCBPONo.SelectedRow != null)
      {
        value = ultCBPONo.Value.ToString().Trim();
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@PONo", DbType.AnsiString, 16, value) };
        DataSet dsSource = DataBaseAccess.SearchStoreProcedure(SP_WHDListMaterialMakeByPONo, inputParam);
        if (dsSource != null)
        {
          this.ultPOInfomation.DataSource = dsSource.Tables[0];
        }
        // End button Add
        if (this.status == 0)
        {
          btnAdd.Enabled = true;
        }
        else
        {
          btnAdd.Enabled = false;
        }
      }
      else
      {
        ultPOInfomation.DataSource = null;
        btnAdd.Enabled = false;
        return;
      }
    }
    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckValidInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      // Save Info && Update Status Material When WH Received
      success = this.SaveReceivingInfo();
      if (success)
      {
        // Save Mapping When Confirm
        success = this.SaveMappingPOReceiving();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      this.ultCBPONo_ValueChanged(sender, e);
      // Load lai Data
      this.LoadData();
    }

    /// <summary>
    /// After Cell Update PO Infomation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultPOInfomation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "qty":
          if (DBConvert.ParseDouble(e.Cell.Row.Cells["QtyReceipted"].Value.ToString()) != double.MinValue)
          {
            if ((DBConvert.ParseDouble(e.Cell.Row.Cells["QtyPO"].Value.ToString()) - DBConvert.ParseDouble(e.Cell.Row.Cells["QtyReceipted"].Value.ToString())) < DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value.ToString()))
            {
              WindowUtinity.ShowMessageError("ERR0001", "Qty");
              return;
            }
          }
          else
          {
            if (DBConvert.ParseDouble(e.Cell.Row.Cells["QtyPO"].Value.ToString()) < DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value.ToString()))
            {
              WindowUtinity.ShowMessageError("ERR0001", "Qty");
              return;
            }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    ///  Before Cell Update PO Infomation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultPOInfomation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      switch (columnName.ToLower())
      {
        case "qty":
          if (text.Trim().Length > 0)
          {
            if (DBConvert.ParseDouble(text) == double.MinValue)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Qty");
              e.Cancel = true;
            }
            else if (DBConvert.ParseDouble(text) < 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Qty");
              e.Cancel = true;
            }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Check Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkSelect_CheckedChanged(object sender, EventArgs e)
    {
      bool flagSelect = false;
      for (int i = 0; i < ultPOInfomation.Rows.Count; i++)
      {
        UltraGridRow row = ultPOInfomation.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 0)
        {
          flagSelect = true;
          break;
        }
      }
      for (int i = 0; i < ultPOInfomation.Rows.Count; i++)
      {
        UltraGridRow row = ultPOInfomation.Rows[i];
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
    /// Check Change hide 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkHide_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkHide.Checked)
      {
        this.grpPOInfomation.Visible = false;
      }
      else
      {
        this.grpPOInfomation.Visible = true;
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
        case "location":
          if (DBConvert.ParseLong(e.Cell.Row.Cells["Location"].Value.ToString()) == long.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Location");
            return;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Change Info PO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultPOInfomation_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "qty":
          e.Cell.Row.Cells["Selected"].Value = 1;
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Choose Supplier
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCBSupplier_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBSupplier.Value != null)
      {
        ultCBPONo.DataSource = null;
        ultCBPONo.Text = string.Empty;
        this.LoadComboListPO();
      }
      else
      {
        ultCBPONo.DataSource = null;
        ultCBPONo.Text = string.Empty;
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
    /// Key up
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultPOInfomation_KeyUp(object sender, KeyEventArgs e)
    {
      try
      {
        if (e.KeyCode != Keys.Down && e.KeyCode != Keys.Up)
        {
          return;
        }
        int rowIndex = (e.KeyCode == Keys.Down) ? ultPOInfomation.ActiveCell.Row.Index + 1 : ultPOInfomation.ActiveCell.Row.Index - 1;
        int cellIndex = ultPOInfomation.ActiveCell.Column.Index;
        try
        {
          ultPOInfomation.Rows[rowIndex].Cells[cellIndex].Activate();
          ultPOInfomation.PerformAction(UltraGridAction.EnterEditMode, false, false);
        }
        catch
        {
        }
      }
      catch
      {
      }
    }

    /// <summary>
    /// Print
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrint_Click(object sender, EventArgs e)
    {
      // Report
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@ReceivingPid", DbType.Int64, this.receivingPid);

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHDRPTReceivingService_Select", inputParam);

      dsWHDReceivingServiceReport dsSource = new dsWHDReceivingServiceReport();
      if (dsSource != null)
      {
        dsSource.Tables["dtInfo"].Merge(ds.Tables[0]);
        dsSource.Tables["dtDetail"].Merge(ds.Tables[1]);
        DaiCo.Shared.View_Report report = null;
        cptReceivingService rpt = new cptReceivingService();
        rpt.SetDataSource(dsSource);

        double totalQty = 0;
        for (int i = 0; i < dsSource.Tables["dtDetail"].Rows.Count; i++)
        {
          if (DBConvert.ParseDouble(dsSource.Tables["dtDetail"].Rows[i]["Qty"].ToString()) != double.MinValue)
          {
            totalQty = totalQty + DBConvert.ParseDouble(dsSource.Tables["dtDetail"].Rows[i]["Qty"].ToString());
          }
        }
        string printDate = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
        string printBy = SharedObject.UserInfo.EmpName;
        rpt.SetParameterValue("TotalQty", totalQty);
        rpt.SetParameterValue("Printdate", printDate);
        rpt.SetParameterValue("Printby", printBy);
        report = new DaiCo.Shared.View_Report(rpt);

        report.IsShowGroupTree = false;
        report.ShowReport(Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
      }

    }
    #endregion Event 
  }
}
