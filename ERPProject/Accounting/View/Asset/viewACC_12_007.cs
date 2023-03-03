/*
  Author      : Nguyen Thanh Binh
  Date        : 11/04/2021
  Description : asset transfer
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
  public partial class viewACC_12_007 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private int status = 0;
    private DataTable dtObject = new DataTable();
    private int docTypePid = ConstantClass.Asset_Transfer;
    public int actionCode = 1;
    private int currency = int.MinValue;
    public int objectTye = int.MinValue;
    public int creditPid = int.MinValue;
    private bool isLoadedDetail = false;
    private bool isLoadedPostTransaction = false;
    #endregion Field

    #region Init
    public viewACC_12_007()
    {
      InitializeComponent();
    }

    private void viewACC_12_007_Load(object sender, EventArgs e)
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
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCAssetTransfer_Init");

      //Init dropdown
      Utility.LoadUltraCombo(ucbddFromDepartment, dsInit.Tables[0], "DepartmentPid", "DeparmentName", false, "DepartmentPid");
      Utility.LoadUltraCombo(ucbddToDeparment, dsInit.Tables[0], "DepartmentPid", "DeparmentName", false, "DepartmentPid");
      Utility.LoadUltraCombo(ucbddEmployee, dsInit.Tables[1], "EmployeePid", "EmployeeName", false, "EmployeePid");
      Utility.LoadUltraCombo(ucbddAsset, dsInit.Tables[2], "Pid", "AssetCode", true, new string[] { "Pid", "DepartmentPid", "UsedEmployeePid", "CostCenterPid", "SegmentPid", "OriginalAmount", "AccumulatedDepAmount", "RemainedAmount", "InitQty" });
      ucbddAsset.DisplayLayout.Bands[0].Columns["AssetCode"].Header.Caption = "Mã Tài Sản";
      ucbddAsset.DisplayLayout.Bands[0].Columns["AssetName"].Header.Caption = "Tên Tài Sản";
      // Set Language
      //this.SetLanguage();
    }

    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        txtAssetTransferCode.ReadOnly = true;
        udtAssetTransferDate.ReadOnly = true;
        txtAssetTransferDesc.ReadOnly = true;
        btnSave.Enabled = false;
        chkConfirm.Enabled = false;
      }
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtAssetTransferCode.Text = dtMain.Rows[0]["TransferCode"].ToString();
        txtAssetTransferDesc.Text = dtMain.Rows[0]["TransferDesc"].ToString();
        udtAssetTransferDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["TransferDate"]);
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
          txtAssetTransferCode.Text = outputParam[0].Value.ToString();
          udtAssetTransferDate.Value = DateTime.Now;
        }
      }
    }

    private void LoadTransationData()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[2];
      inputParam[0] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      inputParam[1] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      DataSet dsSource = SqlDataBaseAccess.SearchStoreProcedure("spACCTransaction_Load", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        ugdTransaction.DataSource = dsSource.Tables[0];
      }
    }

    private void LoadAssetTransferDetail()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      DataSet dsSource = SqlDataBaseAccess.SearchStoreProcedure("spACCAssetTransferDetail_Load", inputParam);
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

        case "utpcAssetTransferDetail":
          if (!isLoadedDetail)
          {
            this.LoadAssetTransferDetail();
            this.isLoadedDetail = true;
          }
          break;
        case "utpcPostAssetTransfer":
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
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCAssetTransfer_Load", inputParam);
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
      if (udtAssetTransferDate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày chứng từ không được để trống!!!");
        udtAssetTransferDate.Focus();
        return false;
      }

      //check detail

      //for (int i = 0; i < ugdData.Rows.Count; i++)
      //{
      //  UltraGridRow row = ugdData.Rows[i];
      //  row.Selected = false;
      //  if (row.Cells["MaterialCode"].Value.ToString().Length <= 0)
      //  {
      //    row.Cells["MaterialCode"].Appearance.BackColor = Color.Yellow;
      //    WindowUtinity.ShowMessageErrorFromText("Mã SP không được để trống.");
      //    row.Selected = true;
      //    ugdData.ActiveRowScrollRegion.FirstRow = row;
      //    return false;
      //  }

      //  if (DBConvert.ParseDouble(row.Cells["OriginalAmount"].Value) <= 0)
      //  {
      //    row.Cells["OriginalAmount"].Appearance.BackColor = Color.Yellow;
      //    WindowUtinity.ShowMessageErrorFromText("Nguyên giá không được để trống và phải lớn hơn 0.");
      //    row.Selected = true;
      //    ugdData.ActiveRowScrollRegion.FirstRow = row;
      //    return false;
      //  }
      //  if (DBConvert.ParseInt(row.Cells["DepreciationMonth"].Value) < 0)
      //  {
      //    row.Cells["DepreciationMonth"].Appearance.BackColor = Color.Yellow;
      //    WindowUtinity.ShowMessageErrorFromText("Số tháng khấu hao không được để trống.");
      //    row.Selected = true;
      //    ugdData.ActiveRowScrollRegion.FirstRow = row;
      //    return false;
      //  }
      //  if (DBConvert.ParseDouble(row.Cells["DepreciationAmount"].Value) < 0)
      //  {
      //    row.Cells["DepreciationAmount"].Appearance.BackColor = Color.Yellow;
      //    WindowUtinity.ShowMessageErrorFromText("Giá trị khấu hao không được để trống.");
      //    row.Selected = true;
      //    ugdData.ActiveRowScrollRegion.FirstRow = row;
      //    return false;
      //  }

      //}
      return true;
    }

    private bool SaveMain()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[6];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      }
      if (txtAssetTransferDesc.Text.Trim().Length > 0)
      {
        inputParam[1] = new SqlDBParameter("@TransferDesc", SqlDbType.NVarChar, txtAssetTransferDesc.Text.Trim().ToString());
      }
      inputParam[2] = new SqlDBParameter("@TransferDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtAssetTransferDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      inputParam[3] = new SqlDBParameter("@Status", SqlDbType.Bit, chkConfirm.Checked ? 1 : 0);
      inputParam[4] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParam[5] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCAssetTransferMaster_Save", inputParam, outputParam);
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
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCAssetTransfer_Delete", deleteParam, outputParam);
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
          SqlDBParameter[] inputParam = new SqlDBParameter[9];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
          }
          inputParam[1] = new SqlDBParameter("@AssetTranserPid", SqlDbType.BigInt, this.viewPid);
          if (DBConvert.ParseLong(row["AssetPid"].ToString()) > 0)
          {
            inputParam[2] = new SqlDBParameter("@AssetPid", SqlDbType.BigInt, DBConvert.ParseLong(row["AssetPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["FromDepartmentPid"].ToString()) > 0)
          {
            inputParam[3] = new SqlDBParameter("@FromDepartmentPid", SqlDbType.Int, DBConvert.ParseInt(row["FromDepartmentPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["ToDepartmentPid"].ToString()) > 0)
          {
            inputParam[4] = new SqlDBParameter("@ToDepartmentPid", SqlDbType.Int, DBConvert.ParseInt(row["ToDepartmentPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["UsedEmployeePid"].ToString()) > 0)
          {
            inputParam[5] = new SqlDBParameter("@UsedEmployeePid", SqlDbType.Int, DBConvert.ParseInt(row["UsedEmployeePid"].ToString()));
          }
          inputParam[6] = new SqlDBParameter("@RemainedAmount", SqlDbType.Float, DBConvert.ParseDouble(row["RemainedAmount"].ToString()));

          if (DBConvert.ParseInt(row["Qty"].ToString()) >= 0)
          {
            inputParam[7] = new SqlDBParameter("@Qty", SqlDbType.Int, DBConvert.ParseInt(row["Qty"].ToString()));
          }
          inputParam[8] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCAssetTransferDetail_Save", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }


    private bool SavePostAssetTransfer()
    {
      bool success = true;
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDBParameter[] inputParam = new SqlDBParameter[4];
      inputParam[0] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      inputParam[1] = new SqlDBParameter("@AssetTransferPid", SqlDbType.BigInt, this.viewPid);
      inputParam[2] = new SqlDBParameter("@IsPosted", SqlDbType.Bit, chkConfirm.Checked ? 1 : 0);
      inputParam[3] = new SqlDBParameter("@CreateBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCPostAssetTransfer", inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) == 0)
      {
        success = false;
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
              success = this.SavePostAssetTransfer();
            }
            else
            {
              success = true;
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
      e.Layout.Bands[0].Columns["FromDepartmentPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["ToDepartmentPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["UsedEmployeePid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["AssetPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Set caption column
      e.Layout.Bands[0].Columns["FromDepartmentPid"].Header.Caption = "Từ bộ phận";
      e.Layout.Bands[0].Columns["ToDepartmentPid"].Header.Caption = "Đến bộ phận";
      e.Layout.Bands[0].Columns["UsedEmployeePid"].Header.Caption = "Người sử dụng";
      e.Layout.Bands[0].Columns["AssetPid"].Header.Caption = "Mã tài sản";
      e.Layout.Bands[0].Columns["AssetName"].Header.Caption = "Tên tài sản";
      e.Layout.Bands[0].Columns["OriginalAmount"].Header.Caption = "Nguyên giá";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Số lượng";
      e.Layout.Bands[0].Columns["AccumulatedDepAmount"].Header.Caption = "Hao mòn lũy kế";
      e.Layout.Bands[0].Columns["RemainedAmount"].Header.Caption = "Giá trị còn lại";


      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["FromDepartmentPid"].ValueList = ucbddFromDepartment;
      e.Layout.Bands[0].Columns["UsedEmployeePid"].ValueList = ucbddEmployee;
      e.Layout.Bands[0].Columns["AssetPid"].ValueList = ucbddAsset;
      e.Layout.Bands[0].Columns["ToDepartmentPid"].ValueList = ucbddToDeparment;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["FromDepartmentPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["FromDepartmentPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["UsedEmployeePid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["UsedEmployeePid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["AssetPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["AssetPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["ToDepartmentPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ToDepartmentPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;


      //e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalAmount"], SummaryPosition.UseSummaryPositionColumn);
      //e.Layout.Bands[0].Summaries[0].DisplayFormat = "Tổng = {0:#,##0.##}";
      //e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalAmountExchange"], SummaryPosition.UseSummaryPositionColumn);
      //e.Layout.Bands[0].Summaries[1].DisplayFormat = "Tổng = {0:#,##0.##}";
      //e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";

      e.Layout.Bands[0].Columns["AssetName"].CellActivation = Activation.ActivateOnly;
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
            e.Cell.Row.Cells["FromDepartmentPid"].Value = DBConvert.ParseInt(ucbddAsset.SelectedRow.Cells["DepartmentPid"].Value);
            e.Cell.Row.Cells["UsedEmployeePid"].Value = DBConvert.ParseInt(ucbddAsset.SelectedRow.Cells["UsedEmployeePid"].Value);
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