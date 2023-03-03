/*
  Author      : Dang Xuan Truong
  Date        : 08/08/2012
  Description : Receiving Return From Supplier (03REC)
*/
using CrystalDecisions.CrystalReports.Engine;
using DaiCo.Application;
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

namespace DaiCo.ERPProject
{
  public partial class viewWHD_05_006 : MainUserControl
  {
    #region Field
    private int whPid = 0;
    //RTW
    public string receivingCode = string.Empty;

    private string strPathOutputFile = string.Empty;
    private string strPathTemplate = string.Empty;
    private string pathExport = string.Empty;
    // Status
    int status = int.MinValue;
    // Delete
    private IList listDeletingDetailPid = new ArrayList();
    private IList listDeletedDetailPid = new ArrayList();

    #endregion Field

    #region Init
    public viewWHD_05_006()
    {
      InitializeComponent();
    }

    private void viewWHD_05_006_Load(object sender, EventArgs e)
    {
      // Load Warehouse List
      Utility.LoadUltraCBMaterialWHListByUser(ucbWarehouse);
      this.LoadComboSupplier();
      this.LoadData();
    }
    #endregion Init

    #region Function
    /// <summary>
    ///  Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ReceivingCode", DbType.AnsiString, 16, this.receivingCode) };
      DataSet dsMain = DataBaseAccess.SearchStoreProcedure("spWHDReceivingReturnFromSupplier_Select", inputParam);
      DataTable dtReceivingInfo = dsMain.Tables[0];
      if (dtReceivingInfo.Rows.Count > 0)
      {
        DataRow row = dtReceivingInfo.Rows[0];
        txtReceivingNote.Text = this.receivingCode;
        txtTitle.Text = dtReceivingInfo.Rows[0]["Title"].ToString();
        txtCreateBy.Text = dtReceivingInfo.Rows[0]["CreateBy"].ToString();
        txtCreateDate.Text = dtReceivingInfo.Rows[0]["CreateDate"].ToString();
        ultCBSupplier.Value = dtReceivingInfo.Rows[0]["Supplier"].ToString();
        txtSupplierNote.Text = dtReceivingInfo.Rows[0]["SupplierNote"].ToString();
        this.whPid = DBConvert.ParseInt(dtReceivingInfo.Rows[0]["WHPid"]);
        ucbWarehouse.Value = this.whPid;
        this.status = DBConvert.ParseInt(dtReceivingInfo.Rows[0]["Posting"].ToString());
      }
      else
      {
        DataTable dtREC = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHDGetNewReceivingNoForMaterial('03REC') NewRECCode");
        if ((dtREC != null) && (dtREC.Rows.Count > 0))
        {
          this.txtReceivingNote.Text = dtREC.Rows[0]["NewRECCode"].ToString();
          this.txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
          this.txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          this.status = 0;
        }
        // Load default WH
        if (ucbWarehouse.Rows.Count > 0)
        {
          ucbWarehouse.Rows[0].Selected = true;
        }
      }
      // Load Detail
      ultData.DataSource = dsMain.Tables[1];

      bool isReadOnly = (ultData.Rows.Count > 0 ? true : false);
      ucbWarehouse.ReadOnly = isReadOnly;

      // Set Status control
      this.SetStatusControl();
    }

    /// <summary>
    /// Set Control
    /// </summary>
    private void SetStatusControl()
    {
      if (this.status == 1)
      {
        txtTitle.ReadOnly = true;
        ultCBSupplier.ReadOnly = true;
        txtSupplierNote.ReadOnly = true;
        chkConfirmed.Checked = true;
        chkConfirmed.Enabled = false;
        btnDelete.Enabled = false;
        btnSave.Enabled = false;
        btnAdd.Enabled = false;
        ucbWarehouse.ReadOnly = true;
      }
    }

    /// <summary>
    /// Load UltraCombo Supplier
    /// </summary>
    private void LoadComboSupplier()
    {
      string commandText = " SELECT SupplierCode, (EnglishName + ' | ' + SupplierCode) EnglishName FROM TblPURSupplierInfo";
      commandText += " WHERE Confirm = 2 AND DeleteFlg = 0 AND LEN(EnglishName) > 2 ORDER BY EnglishName";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultCBSupplier, dtSource, "SupplierCode", "EnglishName", false, "SupplierCode");
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      // Save Info
      bool success = this.SaveRTWInfo();
      if (!success)
      {
        return false;
      }
      // Save Detail
      success = this.SaveRTWDetail();
      if (!success)
      {
        return false;
      }
      // Update StockBalance
      success = this.UpdateStockBalance();
      if (!success)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckInvalid(out string message)
    {
      message = string.Empty;
      if (ucbWarehouse.SelectedRow == null)
      {
        message = "Warehouse";
        return false;
      }
      if (ultCBSupplier.Value == null)
      {
        message = "Supplier";
        return false;
      }
      // Create new receiving note
      if (this.receivingCode.Length == 0)
      {
        // Check WH Summary of preMonth
        if (!Utility.CheckWHPreMonthSummary(this.whPid))
        {
          return false;
        }
      }

      // Check Detail
      DataTable dtMaterial = (DataTable)ucbMaterialList.DataSource;
      DataTable dtLocation = (DataTable)ucbLocation.DataSource;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        //Material
        string materialCode = row.Cells["MaterialCode"].Value.ToString();
        DataRow[] materialRows = dtMaterial.Select(string.Format("MaterialCode = '{0}'", materialCode));
        if (materialRows.Length == 0)
        {
          message = "Material";
          return false;
        }
        //Qty
        if (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) <= 0)
        {
          message = "Qty";
          return false;
        }
        //Location
        int iLocation = DBConvert.ParseInt(row.Cells["LocationPid"].Value);
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
    /// Save Info 
    /// </summary>
    /// <returns></returns>
    private bool SaveRTWInfo()
    {
      if (this.receivingCode == string.Empty)
      {
        DBParameter[] inputParam = new DBParameter[8];

        inputParam[0] = new DBParameter("@RECNo", DbType.AnsiString, 8, "03REC");
        inputParam[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        if (txtTitle.Text.Length > 0)
        {
          inputParam[2] = new DBParameter("@Title", DbType.String, 4000, txtTitle.Text);
        }
        if (chkConfirmed.Checked)
        {
          inputParam[3] = new DBParameter("@Posting", DbType.Int32, 1);
        }
        else
        {
          inputParam[3] = new DBParameter("@Posting", DbType.Int32, 0);
        }
        inputParam[4] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);
        inputParam[5] = new DBParameter("@Type", DbType.Int32, 5);
        inputParam[6] = new DBParameter("@Supplier", DbType.String, ultCBSupplier.Value.ToString());
        inputParam[7] = new DBParameter("@SupplierNote", DbType.String, txtSupplierNote.Text);

        DBParameter[] outputParam = new DBParameter[2];
        outputParam[0] = new DBParameter("@ResultRECNo", DbType.AnsiString, 48, string.Empty);
        outputParam[1] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spWHDReceivingReturnFromSupplier_Insert", inputParam, outputParam);
        this.receivingCode = outputParam[0].Value.ToString();
        long result = DBConvert.ParseLong(outputParam[1].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
        return true;
      }
      else
      {
        DBParameter[] inputParam = new DBParameter[7];
        inputParam[0] = new DBParameter("@ReceivingCode", DbType.String, this.receivingCode);
        inputParam[1] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        if (txtTitle.Text.Length > 0)
        {
          inputParam[2] = new DBParameter("@Title", DbType.String, 4000, txtTitle.Text);
        }
        if (chkConfirmed.Checked)
        {
          inputParam[3] = new DBParameter("@Posting", DbType.Int32, 1);
        }
        else
        {
          inputParam[3] = new DBParameter("@Posting", DbType.Int32, 0);
        }
        inputParam[4] = new DBParameter("@Supplier", DbType.String, 48, ultCBSupplier.Value.ToString());
        inputParam[5] = new DBParameter("@SupplierNote", DbType.String, 255, txtSupplierNote.Text);
        inputParam[6] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);
        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure("spWHDReceivingReturnFromSupplier_Update", inputParam, outputParam);
        long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
        return true;
      }
    }

    /// <summary>
    /// Save Info Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveRTWDetail()
    {
      // Delete Row
      foreach (long rtwDetailPid in this.listDeletedDetailPid)
      {
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@RTWDetailPid", DbType.Int64, rtwDetailPid);
        DBParameter[] output = new DBParameter[1];

        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure("spWHDReceivingDetailReturnFromSupplier_Delete", input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result == 0)
        {
          return false;
        }
      }
      this.listDeletedDetailPid.Clear();

      // Insert & Update Row
      if (ultData.Rows.Count > 0)
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow rowInfo = ultData.Rows[i];
          DBParameter[] inputParam = new DBParameter[8];
          if (DBConvert.ParseLong(rowInfo.Cells["ReceivingDetailPid"].Value.ToString()) != long.MinValue)
          {
            inputParam[0] = new DBParameter("@ReceivingDetailPid", DbType.Int64, DBConvert.ParseLong(rowInfo.Cells["ReceivingDetailPid"].Value.ToString()));
          }
          inputParam[1] = new DBParameter("@ReceivingCode", DbType.AnsiString, 50, this.receivingCode);
          inputParam[2] = new DBParameter("@MaterialCode", DbType.AnsiString, 255, rowInfo.Cells["MaterialCode"].Value.ToString());
          inputParam[3] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(rowInfo.Cells["Qty"].Value.ToString()));
          inputParam[4] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);
          inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          inputParam[6] = new DBParameter("@Location", DbType.Int32, DBConvert.ParseInt(rowInfo.Cells["LocationPid"].Value.ToString()));
          if (rowInfo.Cells["IDReturnToSupplier"].Value.ToString().Length > 0)
          {
            inputParam[7] = new DBParameter("@IDReturnToSupplier", DbType.AnsiString, 50, rowInfo.Cells["IDReturnToSupplier"].Value.ToString());
          }

          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spWHDReceivingDetailReturnFromSupplier_Edit", inputParam, outputParam);
          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result == 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Update Stock Balance
    /// </summary>
    /// <returns></returns>
    private bool UpdateStockBalance()
    {
      if (chkConfirmed.Checked)
      {
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@IDPhieuNhap", DbType.AnsiString, 48, this.receivingCode);
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spWHDReceivingReturnFromSupplierStockBalance_Update", input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result == 0)
        {
          return false;
        }
      }
      return true;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "materialcode":
          if (e.Cell.Row.Cells["MaterialCode"].Value.ToString().Length > 0)
          {
            string materialCode = e.Cell.Row.Cells["MaterialCode"].Value.ToString();
            string commadText = string.Format(@"SELECT MAT.NameEN, MAT.NameVN, UNIT.Symbol Unit
                                                FROM TblGNRMaterialInformation  MAT
	                                              INNER JOIN TblGNRMaterialUnit UNIT ON MAT.Unit = UNIT.Pid 
                                                                                  AND MAT.MaterialCode = '{0}'", materialCode);
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commadText);
            if (dt != null && dt.Rows.Count > 0)
            {
              e.Cell.Row.Cells["NameEN"].Value = dt.Rows[0]["NameEN"].ToString();
              e.Cell.Row.Cells["NameVN"].Value = dt.Rows[0]["NameVN"].ToString();
              e.Cell.Row.Cells["Unit"].Value = dt.Rows[0]["Unit"].ToString();
            }
            else
            {
              e.Cell.Row.Cells["NameEN"].Value = "";
              e.Cell.Row.Cells["NameVN"].Value = "";
              e.Cell.Row.Cells["Unit"].Value = "";
            }
          }
          break;
        case "qty":
          if (DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Qty");
            return;
          }
          break;
        case "location":
          if (DBConvert.ParseInt(e.Cell.Row.Cells["Location"].Value.ToString()) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Location");
            return;
          }
          break;
        default:
          break;
      }

      // Read Only warehouse list
      bool isReadOnly = (ultData.Rows.Count > 0 ? true : false);
      ucbWarehouse.ReadOnly = isReadOnly;
    }

    /// <summary>
    /// Value Change Supplier
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCBSupplier_ValueChanged(object sender, EventArgs e)
    {
      this.LoadComboListIssuingMaterial();
    }

    /// <summary>
    /// Load UltraCombo List IssuingNo
    /// </summary>
    private void LoadComboListIssuingMaterial()
    {
      ultCBIssuingNo.DataSource = null;
      ultCBIssuingNo.Text = string.Empty;
      if (ultCBSupplier.Value != null && this.whPid > 0)
      {
        DBParameter[] input = new DBParameter[2];
        input[0] = new DBParameter("@SupplierCode", DbType.String, ultCBSupplier.Value.ToString());
        input[1] = new DBParameter("@WHPid", DbType.Int32, this.whPid);
        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHDGetListMaterialReturnFromSupplier", input);

        Utility.LoadUltraCombo(ultCBIssuingNo, dtSource, "IssuingDetailPid", "DisplayText", new string[] { "IssuingDetailPid", "DisplayText" });
      }
    }

    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;
      e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["IDReturnToSupplier"].Header.Caption = "Issue No";
      e.Layout.Bands[0].Columns["ReceivingDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationPid"].Header.Caption = "Location";
      e.Layout.Bands[0].Columns["LocationPid"].ValueList = ucbLocation;
      e.Layout.Bands[0].Columns["LocationPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["LocationPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material";
      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = ucbMaterialList;
      e.Layout.Bands[0].Columns["MaterialCode"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["MaterialCode"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["IDReturnToSupplier"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "English Name";
      e.Layout.Bands[0].Columns["NameVn"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameVn"].Hidden = true;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["IDReturnToSupplier"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["IDReturnToSupplier"].MinWidth = 150;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 100;
      e.Layout.Bands[0].Columns["LocationPid"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["LocationPid"].MinWidth = 150;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (row.Cells["IDReturnToSupplier"].Value.ToString().Length > 0)
        {
          row.Cells["MaterialCode"].Activation = Activation.ActivateOnly;
        }
        else
        {
          row.Cells["MaterialCode"].Activation = Activation.AllowEdit;
        }
      }

      if (this.status == 1)
      {
        e.Layout.Override.AllowAddNew = AllowAddNew.No;
        e.Layout.Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Override.AllowUpdate = DefaultableBoolean.False;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      DBParameter[] input = new DBParameter[] { new DBParameter("@ReceivingNote", DbType.AnsiString, 48, this.receivingCode) };
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

    /// <summary>
    /// Delete Receving
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@RTWNo", DbType.AnsiString, 50, this.receivingCode);
      DBParameter[] output = new DBParameter[1];

      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure("spWHDReceivingReturnFromSupplier_Delete", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result == 1)
      {
        WindowUtinity.ShowMessageSuccess("MSG0002");
      }
      else if (result == 0)
      {
        WindowUtinity.ShowMessageError("ERR0004");
        return;
      }
      this.CloseTab();
    }

    /// <summary>
    /// Save Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckInvalid(out message);
      if (!success)
      {
        if (message.Length > 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", message);
        }
        return;
      }
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Before Delete Row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingDetailPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long rtwDetailPid = long.MinValue;
        try
        {
          rtwDetailPid = DBConvert.ParseLong(row.Cells["ReceivingDetailPid"].Value.ToString());
        }
        catch { }
        if (rtwDetailPid != long.MinValue)
        {
          this.listDeletingDetailPid.Add(rtwDetailPid);
        }
      }
    }

    /// <summary>
    /// After Delete Row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long rtwDetailpid in this.listDeletingDetailPid)
      {
        this.listDeletedDetailPid.Add(rtwDetailpid);
      }
      bool isReadOnly = (ultData.Rows.Count > 0 ? true : false);
      ucbWarehouse.ReadOnly = isReadOnly;
    }

    /// <summary>
    /// Before Cell Update Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      switch (columnName.ToLower())
      {
        case "qty":
          if (DBConvert.ParseDouble(text) == double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Qty");
            e.Cancel = true;
          }
          else if (DBConvert.ParseDouble(text) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Qty");
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Add Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      if (ultCBIssuingNo.Value != null)
      {
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@IssuingDetailPid", DbType.Int64, DBConvert.ParseLong(ultCBIssuingNo.Value.ToString())) };
        DataSet dsMain = DataBaseAccess.SearchStoreProcedure("spWHDGetListMaterialsMakeByIssuingDetailPid", inputParam);
        if (dsMain != null)
        {
          DataTable dtSource = (DataTable)ultData.DataSource;
          foreach (DataRow drMain in dsMain.Tables[0].Rows)
          {
            DataRow row = dtSource.NewRow();
            row["IDReturnToSupplier"] = drMain["IssuingCode"].ToString();
            row["MaterialCode"] = drMain["MaterialCode"].ToString();
            row["NameEN"] = drMain["NameEN"].ToString();
            row["NameVN"] = drMain["NameVN"].ToString();
            row["Unit"] = drMain["Unit"].ToString();
            row["Qty"] = DBConvert.ParseDouble(drMain["Qty"].ToString());
            row["LocationPid"] = DBConvert.ParseDouble(drMain["LocationPid"].ToString());
            dtSource.Rows.Add(row);
          }

          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            UltraGridRow row = ultData.Rows[i];
            if (row.Cells["IDReturnToSupplier"].Value.ToString().Length > 0)
            {
              row.Cells["MaterialCode"].Activation = Activation.ActivateOnly;
            }
            else
            {
              row.Cells["MaterialCode"].Activation = Activation.AllowEdit;
            }
          }
          //ultData.DataSource = dtSource;
        }
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
      this.LoadComboListIssuingMaterial();
      Utility.LoadUltraCBMaterialListByWHPid(ucbMaterialList, this.whPid);
      Utility.LoadUltraCBLocationListByWHPid(ucbLocation, this.whPid);
    }

    private void ultCBIssuingNo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["IssuingNote"].Header.Caption = "Issuing Note";
      e.Layout.Bands[0].Columns["IssuingNote"].MinWidth = 120;
      e.Layout.Bands[0].Columns["IssuingNote"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["IssuingDate"].Header.Caption = "Issuing Date";
      e.Layout.Bands[0].Columns["IssuingDate"].MinWidth = 100;
      e.Layout.Bands[0].Columns["IssuingDate"].MaxWidth = 100;
    }
    #endregion Event       
  }
}
