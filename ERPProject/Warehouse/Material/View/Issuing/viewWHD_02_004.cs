
/*
  Author  : Võ Hoa Lư
  Email   : lu_it@daico-furniture.com
  Date    : 21-03-2011
*/

using DaiCo.Application;
using DaiCo.ERPProject.Warehouse.DataSetSource;
using DaiCo.ERPProject.Warehouse.Reports;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;
namespace DaiCo.ERPProject
{
  public partial class viewWHD_02_004 : MainUserControl
  {
    private int whPid = 0;
    public string issueNo = string.Empty;
    private int writeOff = 0;
    private bool canUpdate = false;
    private int confirm = 0;

    #region Init Data
    public viewWHD_02_004()
    {
      InitializeComponent();
    }

    private void viewWHD_02_004_Load(object sender, EventArgs e)
    {
      this.BindData();
      this.LoadData();
      ultMaterialCode.HideButonAlls();
    }
    #endregion Init Data

    #region Load Data
    private void BindData()
    {
      // 1. Create By
      Utility.LoadUltraEmployeeByDeparment(drpCreateBy, string.Empty, true);
      // 2. Approved By
      this.LoadComboApprovedBy();
      // 3. Warehouse
      Utility.LoadUltraCBMaterialWHListByUser(ucbWarehouse);
    }

    /// <summary>
    /// Load UltraCombo Approved By
    /// </summary>
    private void LoadComboApprovedBy()
    {
      string commandText = string.Empty;
      commandText += "  SELECT DEP.Manager, CONVERT(varchar, DEP.Manager) + ' - ' + NV.HoNV + ' ' + NV.TenNV Name";
      commandText += "  FROM VHRDDepartmentInfo DEP";
      commandText += "  LEFT JOIN VHRNhanVien NV ON DEP.Manager = NV.ID_NhanVien";
      commandText += "  WHERE DEP.Code = 'WHD'";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      drpApproved.DataSource = dtSource;
      drpApproved.DisplayMember = "Name";
      drpApproved.ValueMember = "Manager";
      drpApproved.DisplayLayout.Bands[0].ColHeadersVisible = false;
      drpApproved.DisplayLayout.Bands[0].Columns["Name"].Width = 350;
      drpApproved.DisplayLayout.Bands[0].Columns["Manager"].Hidden = true;
    }

    private string GetIssueNo()
    {
      int writeOff = (chkWriteOff.Checked) ? 1 : 0;
      string commandText = string.Format("SELECT dbo.FWHDGetNewIssueNo({0}, {1}, {2})", this.whPid, ConstantClass.ADJUSTMENT_OUT, writeOff);
      object result = DataBaseAccess.ExecuteScalarCommandText(commandText);
      return (result != null) ? result.ToString() : string.Empty;
    }

    private void ReadOnlyMainData()
    {
      txtTitle.ReadOnly = true;
      drpApproved.Enabled = false;
      txtRemark.ReadOnly = true;
      ucbWarehouse.ReadOnly = true;
    }

    private void LoadData()
    {
      btnPrint.Enabled = true;
      if (issueNo.Length == 0)
      {
        this.canUpdate = (btnSave.Visible && btnSave.Enabled);
        txtIssueNo.Text = this.GetIssueNo();
        txtIssueDate.Text = DBConvert.ParseString(DateTime.Today, "dd/MM/yyyy");
        drpCreateBy.Value = SharedObject.UserInfo.UserPid;
        grpSearch.Visible = false;
        // Load default WH
        if (ucbWarehouse.Rows.Count > 0)
        {
          ucbWarehouse.Rows[0].Selected = true;
        }
      }
      else
      {
        DBParameter[] param = new DBParameter[] { new DBParameter("@IssueNo", DbType.AnsiString, 50, this.issueNo) };
        DataSet dsData = DataBaseAccess.SearchStoreProcedure("spWHDIssuingNoteInfomation_Materials", param);
        DataTable dtBaseData = dsData.Tables[0];
        if (dtBaseData.Rows.Count == 0)
        {
          WindowUtinity.ShowMessageError("ERR0007");
          this.CloseTab();
          return;
        }
        // 1. Load Base Data
        DataRow row = dtBaseData.Rows[0];
        txtIssueNo.Text = row["IssueNo"].ToString();
        txtTitle.Text = row["Title"].ToString();
        drpCreateBy.Value = row["CreateBy"];
        drpApproved.Value = row["Approved"];
        this.writeOff = DBConvert.ParseInt(row["WriteOff"].ToString());
        this.writeOff = (this.writeOff != 1) ? 0 : 1;
        chkWriteOff.Checked = (this.writeOff == 1);
        txtIssueDate.Text = row["IssueDate"].ToString();
        txtRemark.Text = row["Remark"].ToString();
        this.whPid = DBConvert.ParseInt(row["WHPid"]);
        ucbWarehouse.Value = this.whPid;

        this.confirm = DBConvert.ParseInt(row["Confirm"].ToString());
        this.canUpdate = ((confirm != 1) && (btnSave.Visible) && (btnSave.Enabled));
        grpSearch.Visible = (confirm == 0);
        chkConfirm.Checked = (confirm == 1);
        if (this.confirm == 1)
        {
          this.ReadOnlyMainData();
          chkConfirm.Enabled = false;
          btnSave.Enabled = false;

          ultIssueDetail.DisplayLayout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
        }
        chkWriteOff.Enabled = false;
        // 2. Load Grid
        ultIssueDetail.DataSource = dsData.Tables[1];
        bool isReadOnly = (ultIssueDetail.Rows.Count > 0 ? true : false);
        ucbWarehouse.ReadOnly = isReadOnly;
      }
    }

    private void Search()
    {
      DBParameter[] inputParams = new DBParameter[3];
      string selectedCode = ultMaterialCode.SelectedValue;
      selectedCode = (selectedCode.Length > 0) ? string.Format(@";{0};", selectedCode) : string.Empty;
      if (selectedCode.Length > 0)
      {
        inputParams[0] = new DBParameter("@MaterialCodes", DbType.AnsiString, 8000, selectedCode.Replace(" ", ""));
      }
      inputParams[1] = new DBParameter("@Recovery", DbType.Int32, this.writeOff);
      inputParams[2] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHDMaterialStockBalance", 240, inputParams);
      ultStockData.DataSource = dtSource;
      int count = dtSource.Columns.Count;
      for (int i = 0; i < count; i++)
      {
        string name = dtSource.Columns[i].ColumnName;
        ultStockData.DisplayLayout.Bands[0].Columns[i].CellActivation = (string.Compare(name, "Issue", true) == 0) ? Infragistics.Win.UltraWinGrid.Activation.AllowEdit : Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
    }
    #endregion Load Data

    #region Check & Save Data

    private bool CheckInvalid()
    {
      if (ucbWarehouse.SelectedRow == null)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Warehouse" });
        ucbWarehouse.Focus();
        return false;
      }
      if (drpCreateBy.SelectedRow == null)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Create by" });
        drpCreateBy.Focus();
        return false;
      }
      string selectedValue = (drpApproved.Value != null) ? drpApproved.Value.ToString() : string.Empty;
      if (drpApproved.SelectedRow == null || selectedValue.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Approved" });
        drpApproved.Focus();
        return false;
      }
      // Create new issuing note
      if (this.issueNo.Length == 0)
      {
        // Check WH Summary of preMonth
        if (!Utility.CheckWHPreMonthSummary(this.whPid))
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Issue Note
    /// </summary>
    private void SaveData()
    {
      string storeName = string.Empty;
      DBParameter[] inputParam = new DBParameter[9];
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 50, string.Empty) };
      int value = int.MinValue;
      if (this.issueNo.Length > 0)
      {
        inputParam[0] = new DBParameter("@IssueNo", DbType.AnsiString, 50, this.issueNo);
        value = (chkConfirm.Checked) ? 1 : 0;
        inputParam[7] = new DBParameter("@Warehouse", DbType.Int32, this.whPid);
        inputParam[8] = new DBParameter("@Confirm", DbType.Int32, value);
        storeName = "spWHDIssueNote_Materials_Update";
      }
      else
      {
        inputParam[0] = new DBParameter("@Warehouse", DbType.Int32, this.whPid);
        inputParam[7] = new DBParameter("@Type", DbType.Int32, ConstantClass.ADJUSTMENT_OUT);
        value = (chkConfirm.Checked) ? 1 : 0;
        inputParam[8] = new DBParameter("@Posting", DbType.Int32, value);
        storeName = "spWHDIssueNote_Materials_Insert";
      }
      string text = txtTitle.Text.Trim();
      inputParam[1] = new DBParameter("@Tittle", DbType.String, 4000, text);
      value = (chkWriteOff.Checked) ? 1 : 0;
      inputParam[2] = new DBParameter("@WriteOff", DbType.Int32, value);
      inputParam[3] = new DBParameter("@AdjsutBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      value = (drpApproved.SelectedRow != null) ? DBConvert.ParseInt(drpApproved.Value.ToString()) : int.MinValue;
      inputParam[4] = new DBParameter("@ApprovedBy", DbType.Int32, value);
      text = txtRemark.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[5] = new DBParameter("@Remark", DbType.AnsiString, 100, text);
      }
      inputParam[6] = new DBParameter("@IsProduction", DbType.Int32, 0);
      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      string outputValue = outputParam[0].Value.ToString().Trim();
      if (outputValue.Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0005");
        return;
      }
      this.issueNo = outputValue;
      WindowUtinity.ShowMessageSuccess("MSG0004");
    }
    #endregion Check & Save Data

    #region Events
    private void chkWriteOff_CheckedChanged(object sender, EventArgs e)
    {
      if (this.issueNo.Length == 0)
      {
        txtIssueNo.Text = this.GetIssueNo();
      }
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
      Shared.Utility.ControlUtility.LoadListMaterials(ultMaterialCode, this.whPid, this.writeOff);
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      string selectedMaterialCode = ultMaterialCode.SelectedValue;
      if (selectedMaterialCode.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Material Code" });
        return;
      }
      this.Search();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      bool success = true;
      for (int i = 0; i < ultStockData.Rows.Count; i++)
      {
        UltraGridRow row = ultStockData.Rows[i];
        double qty = DBConvert.ParseDouble(row.Cells["Issue"].Value.ToString());
        if (qty > 0)
        {
          DBParameter[] inputParam = new DBParameter[8];
          inputParam[0] = new DBParameter("@IssueNo", DbType.AnsiString, 50, this.issueNo);
          string materialCode = row.Cells["MaterialCode"].Value.ToString();
          inputParam[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 50, materialCode);
          inputParam[2] = new DBParameter("@Qty", DbType.Double, qty);
          int location = DBConvert.ParseInt(row.Cells["LocationPid"].Value.ToString());
          inputParam[3] = new DBParameter("@Location", DbType.Int32, location);
          inputParam[4] = new DBParameter("@WriteOff", DbType.Int32, this.writeOff);
          inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          if (row.Cells["PONo"].Value.ToString().Length > 0)
          {
            inputParam[6] = new DBParameter("@PONo", DbType.AnsiString, 50, row.Cells["PONo"].Value.ToString());
          }
          inputParam[7] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spWHDIssueNonProductionMaterialsDetail_Insert_PMISDB", inputParam, outputParam);
          long outputValue = DBConvert.ParseLong(outputParam[0].Value.ToString().Trim());
          int index = i + 1;
          switch (outputValue)
          {
            case 0:
              WindowUtinity.ShowMessageError("ERR0031", new string[] { index.ToString() });
              success = false;
              break;
            case -1:
              WindowUtinity.ShowMessageError("ERR0067", new string[] { index.ToString() });
              success = false;
              break;
            case -2:
              WindowUtinity.ShowMessageError("ERR0068", new string[] { index.ToString() });
              success = false;
              break;
            default:
              break;
          }
        }
      }
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      this.LoadData();
      this.Search();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        this.SaveData();
        this.LoadData();
      }
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      DBParameter[] input = new DBParameter[] { new DBParameter("@IssueNo", DbType.AnsiString, 50, txtIssueNo.Text.Trim()) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDRPTMaterialIssuingPrint_Materials", input);
      dsWHDMaterialIssuingNonStock dsIssue = new dsWHDMaterialIssuingNonStock();
      dsIssue.Tables["dtIssuingInfo"].Merge(dsSource.Tables[0]);
      dsIssue.Tables["dtIssuingDetail"].Merge(dsSource.Tables[1]);

      double totalQty = 0;
      if (dsIssue.Tables["dtIssuingDetail"] != null && dsIssue.Tables["dtIssuingDetail"].Rows.Count > 0)
      {
        for (int i = 0; i < dsIssue.Tables["dtIssuingDetail"].Rows.Count; i++)
        {
          if (DBConvert.ParseDouble(dsIssue.Tables["dtIssuingDetail"].Rows[i]["Qty"].ToString()) != double.MinValue)
          {
            totalQty = totalQty + DBConvert.ParseDouble(dsIssue.Tables["dtIssuingDetail"].Rows[i]["Qty"].ToString());
          }
        }
      }

      //ReportClass cpt = null;
      DaiCo.Shared.View_Report report = null;
      CrystalDecisions.CrystalReports.Engine.ReportClass cpt = null;

      if (this.confirm > 0)
      {
        cpt = new cptMaterialsAdjustmentOut();
      }
      else
      {
        cpt = new cptMaterialsAdjustmentOutDraff();
      }
      cpt.SetDataSource(dsIssue);

      cpt.SetParameterValue("TotalQty", totalQty);
      report = new DaiCo.Shared.View_Report(cpt);
      //---------------Export ------------------------
      //report = new DaiCo.Shared.View_Report(cpt);
      // report.IsShowGroupTree = false;
      // -------------------------------------------------
      report.ShowReport(Shared.Utility.ViewState.ModalWindow);
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultIssueDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Wo"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].Hidden = true;
      e.Layout.Bands[0].Columns["Reason"].Hidden = true;
      e.Layout.Bands[0].Columns["PONo"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 90;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].Format = "#,###.##";
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 70;
      //e.Layout.Bands[0].Columns["Location"].MinWidth = 80;
      //e.Layout.Bands[0].Columns["Location"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["EnglishName"].Header.Caption = "English Name";
      e.Layout.Bands[0].Columns["VietnameseName"].Header.Caption = "Vietnamese Name";
      e.Layout.Bands[0].Override.AllowDelete = (this.canUpdate) ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
    }

    private void ultStockData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 90;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Location"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Location"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["Balance"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Balance"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Balance"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Balance"].Format = "#,###.##";
      e.Layout.Bands[0].Columns["Issue"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Issue"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Issue"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LocationPid"].Hidden = true;
      e.Layout.Bands[0].Columns["PONo"].Hidden = true;
    }

    private void ultStockData_BeforeCellUpdate(object sender, Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      if (string.Compare(columnName, "Issue", true) == 0)
      {
        string text = e.Cell.Text.Trim();
        if (text.Length == 0)
        {
          return;
        }
        double issue = DBConvert.ParseDouble(text);
        if (issue < 0)
        {
          WindowUtinity.ShowMessageError("ERR0009");
          e.Cancel = true;
          return;
        }
        double balance = DBConvert.ParseDouble(e.Cell.Row.Cells["Balance"].Value.ToString());
        if (issue > balance)
        {
          WindowUtinity.ShowMessageError("ERR0010", new string[] { "Issue", "Balance" });
          e.Cancel = true;
          return;
        }
      }

    }

    private void ultIssueDetail_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      bool success = true;
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spWHDIssueNonProductionMaterialsDetail_Delete_PMISDB", inputParam, outputParam);
        long outputValue = DBConvert.ParseLong(outputParam[0].Value.ToString().Trim());
        if (outputValue == 0)
        {
          success = false;
        }
      }
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0002");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0004");
      }
      this.Search();
    }

    private void ultIssueDetail_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void ultStockData_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode != Keys.Down && e.KeyCode != Keys.Up)
      {
        return;
      }
      int rowIndex = (e.KeyCode == Keys.Down) ? ultStockData.ActiveCell.Row.Index + 1 : ultStockData.ActiveCell.Row.Index - 1;
      int cellIndex = ultStockData.ActiveCell.Column.Index;
      try
      {
        ultStockData.Rows[rowIndex].Cells[cellIndex].Activate();
        ultStockData.PerformAction(UltraGridAction.EnterEditMode, false, false);
      }
      catch { }
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
      Shared.Utility.ControlUtility.LoadListMaterials(ultMaterialCode, this.whPid, this.writeOff);
    }
    #endregion Events    
  }
}
