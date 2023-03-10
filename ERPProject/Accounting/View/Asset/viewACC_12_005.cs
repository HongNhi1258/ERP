/*
  Author      : Nguyen Thanh Binh
  Date        : 11/04/2021
  Description : asset shipment
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
  public partial class viewACC_12_005 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private int status = 0;
    private DataTable dtObject = new DataTable();
    private int docTypePid = ConstantClass.Asset_Shipment;
    public int actionCode = 1;
    private int currency = int.MinValue;
    public int objectTye = int.MinValue;
    public int creditPid = int.MinValue;
    private bool isLoadedDetail = false;
    private bool isLoadedPostTransaction = false;
    #endregion Field

    #region Init
    public viewACC_12_005()
    {
      InitializeComponent();
    }

    private void viewACC_12_005_Load(object sender, EventArgs e)
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
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCAssetShipment_Init");

      //Init dropdown
      Utility.LoadUltraCombo(ucbddDepartment, dsInit.Tables[0], "DepartmentPid", "DeparmentName", false, "DepartmentPid");
      Utility.LoadUltraCombo(ucbddEmployee, dsInit.Tables[1], "EmployeePid", "EmployeeName", false, "EmployeePid");
      Utility.LoadUltraCombo(ucbddAsset, dsInit.Tables[2], "Pid", "AssetCode", true, new string[] { "Pid", "DepartmentPid", "UsedEmployeePid", "CostCenterPid", "SegmentPid", "OriginalAmount", "AccumulatedDepAmount", "RemainedAmount", "InitQty" });
      ucbddAsset.DisplayLayout.Bands[0].Columns["AssetCode"].Header.Caption = "Mã Tài Sản";
      ucbddAsset.DisplayLayout.Bands[0].Columns["AssetName"].Header.Caption = "Tên Tài Sản";
      Utility.LoadUltraCombo(ucbddSegment, dsInit.Tables[3], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbddCostCenter, dsInit.Tables[4], "Value", "Display", false, "Value");
      // Set Language
      //this.SetLanguage();
    }

    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        txtAssetShipmentCode.ReadOnly = true;
        udtAssetShipmentDate.ReadOnly = true;
        txtAssetShipmentDesc.ReadOnly = true;
        btnSave.Enabled = false;
        chkConfirm.Enabled = false;
      }
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtAssetShipmentCode.Text = dtMain.Rows[0]["ShipmentCode"].ToString();
        txtAssetShipmentDesc.Text = dtMain.Rows[0]["ShipmentDesc"].ToString();
        udtAssetShipmentDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["ShipmentDate"]);
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
          txtAssetShipmentCode.Text = outputParam[0].Value.ToString();
          udtAssetShipmentDate.Value = DateTime.Now;
        }
      }
    }

    private void LoadTransationData()
    {
      ugdTransaction.SetDataSource(this.docTypePid, this.viewPid);
    }

    private void LoadAssetShipmentDetail()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      DataSet dsSource = SqlDataBaseAccess.SearchStoreProcedure("spACCAssetShipmentDetail_Load", inputParam);
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

        case "utpcAssetShipmentDetail":
          if (!isLoadedDetail)
          {
            this.LoadAssetShipmentDetail();
            this.isLoadedDetail = true;
          }
          break;
        case "utpcPostAssetShipment":
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
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCAssetShipment_Load", inputParam);
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
      if (udtAssetShipmentDate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày chứng từ không được để trống!!!");
        udtAssetShipmentDate.Focus();
        return false;
      }

      //check detail

      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        UltraGridRow row = ugdData.Rows[i];
        row.Selected = false;
        if (row.Cells["AssetPid"].Value.ToString().Length > 0)
        {
          DateTime depreciationDate = DateTime.MinValue;
          string cmd = string.Format(@"SELECT DepreciationDate, Status
                        FROM TblACCAsset
                        WHERE Pid = {0} ", DBConvert.ParseInt(row.Cells["AssetPid"].Value.ToString()));
          DataTable dtAsset = DataBaseAccess.SearchCommandTextDataTable(cmd);
          depreciationDate = DBConvert.ParseDateTime(dtAsset.Rows[0]["DepreciationDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          if (DBConvert.ParseDateTime(udtAssetShipmentDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) < depreciationDate)
          {
            WindowUtinity.ShowMessageErrorFromText(string.Format("Ngày chứng từ không được nhỏ hơn ngày bắt khấu hao TSCĐ ( {0} < {1})", DBConvert.ParseDateTime(udtAssetShipmentDate.Value), depreciationDate));
            row.Selected = true;
            ugdData.ActiveRowScrollRegion.FirstRow = row;
            return false;
          }
          if (DBConvert.ParseInt(dtAsset.Rows[0]["Status"].ToString()) == 1 || DBConvert.ParseInt(dtAsset.Rows[0]["Status"].ToString()) == 4)
          {
            WindowUtinity.ShowMessageErrorFromText("Trạng thái tài sản cổ định không hợp lệ (Đã mua, Đã thanh lý).");
            row.Selected = true;
            ugdData.ActiveRowScrollRegion.FirstRow = row;
            return false;
          }
                    
        }
      }
      return true;
    }

    private bool SaveMain()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[6];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      }
      if (txtAssetShipmentDesc.Text.Trim().Length > 0)
      {
        inputParam[1] = new SqlDBParameter("@ShipmentDesc", SqlDbType.NVarChar, txtAssetShipmentDesc.Text.Trim().ToString());
      }
      inputParam[2] = new SqlDBParameter("@ShipmentDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtAssetShipmentDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      inputParam[3] = new SqlDBParameter("@Status", SqlDbType.Bit, chkConfirm.Checked ? 1 : 0);
      inputParam[4] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParam[5] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCAssetShipmentMaster_Save", inputParam, outputParam);
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
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCAssetShipment_Delete", deleteParam, outputParam);
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
          SqlDBParameter[] inputParam = new SqlDBParameter[12];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
          }
          inputParam[1] = new SqlDBParameter("@AssetShipmentPid", SqlDbType.BigInt, this.viewPid);
          if (DBConvert.ParseLong(row["AssetPid"].ToString()) > 0)
          {
            inputParam[2] = new SqlDBParameter("@AssetPid", SqlDbType.BigInt, DBConvert.ParseLong(row["AssetPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["DepartmentPid"].ToString()) > 0)
          {
            inputParam[3] = new SqlDBParameter("@DepartmentPid", SqlDbType.Int, DBConvert.ParseInt(row["DepartmentPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["EmployeeUsePid"].ToString()) > 0)
          {
            inputParam[4] = new SqlDBParameter("@UsedEmployeePid", SqlDbType.Int, DBConvert.ParseInt(row["EmployeeUsePid"].ToString()));
          }
          if (DBConvert.ParseInt(row["CostCenterPid"].ToString()) > 0)
          {
            inputParam[5] = new SqlDBParameter("@CostCenterPid", SqlDbType.Int, DBConvert.ParseInt(row["CostCenterPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["SegmentPid"].ToString()) > 0)
          {
            inputParam[6] = new SqlDBParameter("@SegmentPid", SqlDbType.Int, DBConvert.ParseInt(row["SegmentPid"].ToString()));
          }
          if (DBConvert.ParseDouble(row["OriginalAmount"].ToString()) >= 0)
          {
            inputParam[7] = new SqlDBParameter("@OriginalAmount", SqlDbType.Float, DBConvert.ParseDouble(row["OriginalAmount"].ToString()));
          }
          if (DBConvert.ParseDouble(row["AccumulatedDepAmount"].ToString()) >= 0)
          {
            inputParam[8] = new SqlDBParameter("@AccumulatedDepAmount", SqlDbType.Float, DBConvert.ParseDouble(row["AccumulatedDepAmount"].ToString()));
          }
          if (DBConvert.ParseInt(row["Qty"].ToString()) >= 0)
          {
            inputParam[9] = new SqlDBParameter("@Qty", SqlDbType.Int, DBConvert.ParseInt(row["Qty"].ToString()));
          }
          inputParam[10] = new SqlDBParameter("@RemainedAmount", SqlDbType.Float, DBConvert.ParseDouble(row["RemainedAmount"].ToString()));


          inputParam[11] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCAssetShipmentDetail_Save", inputParam, outputParam);
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
              bool isPosted = chkConfirm.Checked;
              success = Utility.ACCPostTransaction(this.docTypePid, viewPid, isPosted, SharedObject.UserInfo.UserPid);
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
      e.Layout.AutoFitColumns = false;
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
      e.Layout.Bands[0].Columns["EmployeeUsePid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["AssetPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["CostCenterPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["SegmentPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Set caption column
      e.Layout.Bands[0].Columns["DepartmentPid"].Header.Caption = "Phòng ban";
      e.Layout.Bands[0].Columns["EmployeeUsePid"].Header.Caption = "Người sử dụng";
      e.Layout.Bands[0].Columns["AssetPid"].Header.Caption = "Mã tài sản";
      e.Layout.Bands[0].Columns["AssetName"].Header.Caption = "Tên tài sản";
      e.Layout.Bands[0].Columns["OriginalAmount"].Header.Caption = "Nguyên giá";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Số lượng";
      e.Layout.Bands[0].Columns["AccumulatedDepAmount"].Header.Caption = "Hao mòn lũy kế";
      e.Layout.Bands[0].Columns["RemainedAmount"].Header.Caption = "Giá trị còn lại";
      e.Layout.Bands[0].Columns["SegmentPid"].Header.Caption = "Khoản mục chị phí";
      e.Layout.Bands[0].Columns["CostCenterPid"].Header.Caption = "Trung tâm chi phí";

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["DepartmentPid"].ValueList = ucbddDepartment;
      e.Layout.Bands[0].Columns["EmployeeUsePid"].ValueList = ucbddEmployee;
      e.Layout.Bands[0].Columns["AssetPid"].ValueList = ucbddAsset;
      e.Layout.Bands[0].Columns["CostCenterPid"].ValueList = ucbddCostCenter;
      e.Layout.Bands[0].Columns["SegmentPid"].ValueList = ucbddSegment;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["DepartmentPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["DepartmentPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["EmployeeUsePid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["EmployeeUsePid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["AssetPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["AssetPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["CostCenterPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CostCenterPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["SegmentPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["SegmentPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;


      //e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalAmount"], SummaryPosition.UseSummaryPositionColumn);
      //e.Layout.Bands[0].Summaries[0].DisplayFormat = "Tổng = {0:#,##0.##}";
      //e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalAmountExchange"], SummaryPosition.UseSummaryPositionColumn);
      //e.Layout.Bands[0].Summaries[1].DisplayFormat = "Tổng = {0:#,##0.##}";
      //e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";

      e.Layout.Bands[0].Columns["DepartmentPid"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["AssetName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["EmployeeUsePid"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["OriginalAmount"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["AccumulatedDepAmount"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["RemainedAmount"].CellActivation = Activation.ActivateOnly;
    }

    private void ugdData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      this.NeedToSave = true;
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      string commandText = string.Empty;
      switch (columnName)
      {
        case "assetpid":
          try
          {
            e.Cell.Row.Cells["AssetName"].Value = ucbddAsset.SelectedRow.Cells["AssetName"].Value.ToString();
            e.Cell.Row.Cells["DepartmentPid"].Value = DBConvert.ParseInt(ucbddAsset.SelectedRow.Cells["DepartmentPid"].Value);
            e.Cell.Row.Cells["EmployeeUsePid"].Value = DBConvert.ParseInt(ucbddAsset.SelectedRow.Cells["UsedEmployeePid"].Value);
            e.Cell.Row.Cells["OriginalAmount"].Value = DBConvert.ParseDouble(ucbddAsset.SelectedRow.Cells["OriginalAmount"].Value);
            if (DBConvert.ParseDouble(ucbddAsset.SelectedRow.Cells["AccumulatedDepAmount"].Value) != double.MinValue)
            {
              e.Cell.Row.Cells["AccumulatedDepAmount"].Value = DBConvert.ParseDouble(ucbddAsset.SelectedRow.Cells["AccumulatedDepAmount"].Value);
            }
            else
            {
              e.Cell.Row.Cells["AccumulatedDepAmount"].Value = 0;
            }
            if (DBConvert.ParseDouble(ucbddAsset.SelectedRow.Cells["RemainedAmount"].Value) != double.MinValue)
            {
              e.Cell.Row.Cells["RemainedAmount"].Value = DBConvert.ParseDouble(ucbddAsset.SelectedRow.Cells["RemainedAmount"].Value);
            }
            else
            {
              e.Cell.Row.Cells["RemainedAmount"].Value = 0;
            }
            if (DBConvert.ParseInt(ucbddAsset.SelectedRow.Cells["SegmentPid"].Value) != int.MinValue)
            {
              e.Cell.Row.Cells["SegmentPid"].Value = DBConvert.ParseInt(ucbddAsset.SelectedRow.Cells["SegmentPid"].Value);
            }
            if (DBConvert.ParseInt(ucbddAsset.SelectedRow.Cells["CostCenterPid"].Value) != int.MinValue)
            {
              e.Cell.Row.Cells["CostCenterPid"].Value = DBConvert.ParseInt(ucbddAsset.SelectedRow.Cells["CostCenterPid"].Value);
            }
          }
          catch
          {

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
    private void ugdData_AfterRowInsert(object sender, RowEventArgs e)
    {
      //read only
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




    #endregion Event

  }
}