/*
  Author  : Võ Hoa Lư
  Email   : lu_it@daico-furniture.com
  Date    : 21-03-2011
*/

using DaiCo.Application;
using DaiCo.ERPProject.Warehouse.DataSetSource;
using DaiCo.ERPProject.Warehouse.Material.DataSetSource;
using DaiCo.ERPProject.Warehouse.Material.Reports;
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
  public partial class viewWHD_02_001 : MainUserControl
  {
    #region fields
    public string issueNo = string.Empty;
    private int writeOff = 0;
    private bool canUpdate = false;
    private long currentWo = long.MinValue;
    #endregion fields

    #region Init Data
    public viewWHD_02_001()
    {
      InitializeComponent();
    }

    private void viewWHD_02_001_Load(object sender, EventArgs e)
    {
      // Check Summary MonthLy Duong Minh 10/10/2011 START
      if (this.issueNo.Trim().Length == 0)
      {
        bool result = this.CheckSummary();
        if (result == false)
        {
          this.CloseTab();
          return;
        }
      }
      // Check Summary MonthLy Duong Minh 10/10/2011 END

      this.LoadDropdownList();
      this.LoadData();
    }
    #endregion Init Data

    #region Load Data
    private void LoadDropdownList()
    {
      // 1. Create By
      Utility.LoadUltraEmployeeByDeparment(drpCreateBy, string.Empty, true);
      // 2. Department
      Utility.LoadUltraDepartment(drpDepartment, true);
      // 3. Material Code
      Utility.LoadUltraMaterialCode(drpMaterials, 1);
      // 4. Sup No
      this.LoadSupplementNo();
    }

    private void LoadSupplementNo()
    {
      string commandText = string.Format(@"SELECT DISTINCT SUP.Pid, SUP.SupplementNo, SUP.Description 
                                           FROM TblPLNSupplementForWorkOrder SUP
	                                              INNER JOIN TblPLNSupplementForWorkOrderDetail DT ON (SUP.Pid = DT.SupplementPid)
                                          WHERE (SUP.Status <> 0) AND (DT.Supplement > DT.Issued) AND (DT.IsCloseWork = 0)
                                          ORDER BY Pid DESC");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText, 120);
      drpSuppNo.DataSource = dtSource;
      drpSuppNo.ValueMember = "Pid";
      drpSuppNo.DisplayMember = "SupplementNo";
      drpSuppNo.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      drpSuppNo.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private string GetIssueNo()
    {
      int writeOff = (chkWriteOff.Checked) ? 1 : 0;
      string commandText = string.Format("SELECT dbo.FWHDGetNewIssueNo(1, {0}, {1})", ConstantClass.ISSUE_TO_PRODUCTION, writeOff);
      object result = DataBaseAccess.ExecuteScalarCommandText(commandText);
      return (result != null) ? result.ToString() : string.Empty;
    }

    private void MainData()
    {
      txtTitle.ReadOnly = true;
      drpCreateBy.Enabled = false;
      txtEmployeePid.ReadOnly = true;
      drpDepartment.Enabled = false;
      drpReceiver.Enabled = false;
      radProduction.Enabled = false;
      radOtherDepartment.Enabled = false;
    }

    private void LoadData()
    {
      lbRemark.Text = string.Empty;
      this.SetStatusControl();
      if (issueNo.Length == 0)
      {
        grpSearch.Visible = false;
        this.canUpdate = (btnSave.Visible && btnSave.Enabled);
        txtIssueNo.Text = this.GetIssueNo();
        txtIssueDate.Text = DBConvert.ParseString(DateTime.Today, "dd/MM/yyyy");
        drpCreateBy.Value = SharedObject.UserInfo.UserPid;
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
        grpSearch.Visible = true;
        // 1. Load Base Data
        DataRow row = dtBaseData.Rows[0];
        txtIssueNo.Text = row["IssueNo"].ToString();
        txtTitle.Text = row["Title"].ToString();
        drpCreateBy.Value = row["CreateBy"];
        drpDepartment.Value = row["Department"];
        this.writeOff = DBConvert.ParseInt(row["WriteOff"].ToString());
        this.writeOff = (this.writeOff != 1) ? 0 : 1;
        chkWriteOff.Checked = (this.writeOff == 1);
        txtIssueDate.Text = row["IssueDate"].ToString();
        drpReceiver.Value = row["Recever"];
        int isProduction = DBConvert.ParseInt(row["IsProduction"].ToString());
        if (isProduction == 1)
        {
          radProduction.Checked = true;
        }
        else
        {
          radOtherDepartment.Checked = true;
        }
        int confirm = DBConvert.ParseInt(row["Confirm"].ToString());
        this.canUpdate = ((confirm != 1) && (btnSave.Visible) && (btnSave.Enabled));
        chkConfirm.Checked = (confirm == 1);
        if (confirm == 1)
        {
          this.MainData();
          grpSearch.Visible = false;
          btnAdd.Enabled = false;
          chkConfirm.Enabled = false;
          btnSave.Enabled = false;
          btnPrint.Enabled = true;
          ultIssueDetail.DisplayLayout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
        }
        radProduction.Enabled = false;
        radOtherDepartment.Enabled = false;
        drpDepartment.Enabled = false;
        txtEmployeePid.ReadOnly = true;
        chkWriteOff.Enabled = false;
        // 2. Load Grid
        ultIssueDetail.DataSource = dsData.Tables[1];
      }
    }

    private void SetStatusControl()
    {
      if (this.issueNo.Length > 0)
      {
        drpMaterials.Enabled = true;
        if (radProduction.Checked)
        {
          drpSuppNo.Enabled = true;
          txtWo.Enabled = true;
          drpItemCode.Enabled = true;
        }
        else
        {
          drpSuppNo.Enabled = false;
          txtWo.Enabled = false;
          drpItemCode.Enabled = false;
        }
      }
    }

    private void Search()
    {
      DBParameter[] inputParams = new DBParameter[5];
      inputParams[4] = new DBParameter("@Recovery", DbType.Int16, this.writeOff);
      string storeName = string.Empty;
      long wo = long.MinValue;
      string itemCode = string.Empty;
      int revision = int.MinValue;
      string materialCode = (drpMaterials.SelectedRow != null) ? drpMaterials.SelectedRow.Cells["Code"].Value.ToString() : string.Empty;
      /* 1. ISSUE SUPPLEMENT */
      if (drpSuppNo.SelectedRow != null)
      {
        storeName = "spWHDDataCanIssueForSupplement_Materials";
        long pid = DBConvert.ParseLong(drpSuppNo.SelectedRow.Cells["Pid"].Value.ToString());
        inputParams[0] = new DBParameter("@SupplementPid", DbType.Int64, pid);
        if (materialCode.Length > 0)
        {
          inputParams[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 50, materialCode);
        }
      }
      else
      {
        /* 2. ISSUE FOR PRODUCTION */
        if (radProduction.Checked)
        {
          // Check Material Code Or Work Order
          wo = DBConvert.ParseLong(txtWo.Text);
          if (materialCode.Length == 0 && wo == long.MinValue)
          {
            WindowUtinity.ShowMessageError("MSG0005", new string[] { "Material Code or Work Order" });
            drpMaterials.Focus();
            return;
          }
          // 2.2 Set Params
          if (wo != long.MinValue)
          {
            inputParams[0] = new DBParameter("@Wo", DbType.Int64, wo);
          }
          if (drpItemCode.SelectedRow != null)
          {
            itemCode = drpItemCode.SelectedRow.Cells["ItemCode"].Value.ToString();
            revision = DBConvert.ParseInt(drpItemCode.SelectedRow.Cells["Revision"].Value.ToString());
          }
          if (itemCode.Length > 0)
          {
            inputParams[1] = new DBParameter("@ItemCode", DbType.AnsiString, 50, itemCode);
          }
          if (revision != int.MinValue)
          {
            inputParams[2] = new DBParameter("@Revision", DbType.Int32, revision);
          }
          if (materialCode.Length > 0)
          {
            inputParams[3] = new DBParameter("@MaterialCode", DbType.AnsiString, 50, materialCode);
          }
          storeName = "spWHDDataCanIssueForProduction_Materials";
        }
        /* 3. ISSUE FOR DEPARTMENT */
        else
        {
          string department = drpDepartment.SelectedRow.Cells["Code"].Value.ToString();
          inputParams[0] = new DBParameter("@Department", DbType.AnsiString, 50, department);
          if (materialCode.Length > 0)
          {
            inputParams[3] = new DBParameter("@MaterialCode", DbType.AnsiString, 50, materialCode);
          }
          storeName = "spWHDDataCanIssueForDepartment_Materials";
        }
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, 240, inputParams);
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

    /// <summary>
    /// Check Summary PreMonth && PreYear
    /// </summary>
    /// <returns></returns>
    private bool CheckSummary()
    {
      DateTime firstDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
      int result = DateTime.Compare(firstDate, DateTime.Today);

      if (result <= 0)
      {
        int preMonth = 0;
        int preYear = 0;
        if (DateTime.Today.Month == 1)
        {
          preMonth = 12;
          preYear = DateTime.Today.Year - 1;
        }
        else
        {
          preMonth = DateTime.Today.Month - 1;
          preYear = DateTime.Today.Year;
        }

        string commandText = "SELECT COUNT(*) FROM TblWHDMonthlySummary_Materials WHERE Month = " + preMonth + " AND Year = " + preYear;
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if ((dtCheck == null) || (dtCheck != null && DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0))
        {
          WindowUtinity.ShowMessageError("ERR0303", preMonth.ToString(), preYear.ToString());
          return false;
        }
      }
      return true;
    }

    private bool CheckInvalid()
    {
      if (drpCreateBy.SelectedRow == null)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Create by" });
        drpCreateBy.Focus();
        return false;
      }
      string selectedValue = (drpDepartment.Value != null) ? drpDepartment.Value.ToString() : string.Empty;
      if (drpDepartment.SelectedRow == null || selectedValue.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Department" });
        drpDepartment.Focus();
        return false;
      }
      selectedValue = (drpReceiver.Value != null) ? drpReceiver.Value.ToString() : string.Empty;
      if (drpReceiver.SelectedRow == null || selectedValue.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Receiver" });
        drpReceiver.Focus();
        return false;
      }
      return true;
    }

    private void SaveData()
    {
      //DataTable dtDetail = (DataTable)this.ultIssueDetail.DataSource;
      //if (dtDetail != null && dtDetail.Rows.Count > 0)
      //{
      //  string codeCheck = ",sheet,pc,pcs,pair,set,roll,";
      //  foreach (DataRow row in dtDetail.Rows)
      //  {
      //    if (row["Unit"].ToString().ToLower().IndexOf(codeCheck) == -1)
      //    {
      //      if (DBConvert.ParseInt(row["Qty"].ToString()) == int.MinValue)
      //      {
      //        WindowUtinity.ShowMessageError("ERRO116", new string[] { row["MaterialCode"].ToString() });
      //        return;
      //      }
      //    }
      //  }
      //}

      // 1. Parent table
      string storeName = string.Empty;
      DBParameter[] inputParam = new DBParameter[11];
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 50, string.Empty) };
      int value = int.MinValue;
      if (this.issueNo.Length > 0)
      {
        inputParam[0] = new DBParameter("@IssueNo", DbType.AnsiString, 50, this.issueNo);
        value = (chkConfirm.Checked) ? 1 : 0;
        inputParam[9] = new DBParameter("@Confirm", DbType.Int32, value);
        value = (drpReceiver.SelectedRow != null) ? DBConvert.ParseInt(drpReceiver.Value.ToString()) : int.MinValue;
        inputParam[10] = new DBParameter("@CreateBy", DbType.Int32, value);
        storeName = "spWHDIssueNote_Materials_Update";
      }
      else
      {
        inputParam[0] = new DBParameter("@Warehouse", DbType.Int32, ConstantClass.MATERIALS_WAREHOUSE);
        inputParam[9] = new DBParameter("@Type", DbType.Int32, ConstantClass.ISSUE_TO_PRODUCTION);
        storeName = "spWHDIssueNote_Materials_Insert";
      }
      string text = txtTitle.Text.Trim();
      inputParam[1] = new DBParameter("@Tittle", DbType.String, 4000, text);
      value = (chkWriteOff.Checked) ? 1 : 0;
      inputParam[2] = new DBParameter("@WriteOff", DbType.Int32, value);
      inputParam[3] = new DBParameter("@AdjsutBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      text = (drpDepartment.SelectedRow != null) ? drpDepartment.Value.ToString() : string.Empty;
      inputParam[4] = new DBParameter("@Department", DbType.AnsiString, 50, text);
      value = (drpReceiver.SelectedRow != null) ? DBConvert.ParseInt(drpReceiver.Value.ToString()) : int.MinValue;
      if (value != int.MinValue)
      {
        inputParam[5] = new DBParameter("@Receiver", DbType.Int32, value);
      }
      if (0 == 1)
      {
        inputParam[6] = new DBParameter("@TransToWH", DbType.String, 50, "WIP");
        inputParam[7] = new DBParameter("@TransferStatus", DbType.Int32, 0);
      }
      value = (radProduction.Checked) ? 1 : 0;
      inputParam[8] = new DBParameter("@IsProduction", DbType.Int32, value);
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

    private void drpDepartment_TextChanged(object sender, EventArgs e)
    {
      string department = string.Empty;
      try
      {
        department = drpDepartment.Value.ToString();
      }
      catch { }
      Utility.LoadUltraEmployeeByDeparment(drpReceiver, department, true);
    }

    private void txtEmployeePid_Leave(object sender, EventArgs e)
    {
      int empPid = DBConvert.ParseInt(txtEmployeePid.Text);
      if (empPid != int.MinValue)
      {
        string commandText = string.Format("SELECT Department FROM VHRMEmployee WHERE Pid = {0}", empPid);
        object result = DataBaseAccess.ExecuteScalarCommandText(commandText);
        if (result == null)
        {
          drpDepartment.Value = string.Empty;
          Utility.LoadUltraEmployeeByDeparment(drpReceiver, "-1", true);
          drpReceiver.Value = string.Empty;
          return;
        }
        drpDepartment.Value = result.ToString();
        drpReceiver.Value = empPid;
      }
    }

    private void txtWo_Leave(object sender, EventArgs e)
    {
      long wo = DBConvert.ParseLong(txtWo.Text);
      if (txtWo.Enabled && this.currentWo != wo)
      {
        string commandText = string.Format(@"SELECT ItemCode, Revision, ItemCode + ' - Revision : ' + CONVERT(varchar, Revision) Descption
                                           FROM dbo.TblPLNWorkOrderConfirmedDetails
                                           WHERE WorkOrderPid = {0}", wo);
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        drpItemCode.DataSource = dtSource;
        drpItemCode.ValueMember = "ItemCode";
        drpItemCode.DisplayMember = "Descption";
        drpItemCode.DisplayLayout.Bands[0].Columns["ItemCode"].Hidden = true;
        drpItemCode.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
        drpItemCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
        this.currentWo = wo;
      }
    }

    private void radProduction_CheckedChanged(object sender, EventArgs e)
    {
      this.SetStatusControl();
    }

    private void drpMaterials_ValueChanged(object sender, EventArgs e)
    {
      this.SetStatusControl();
      int isControl = (drpMaterials.SelectedRow == null) ? -1 : DBConvert.ParseInt(drpMaterials.SelectedRow.Cells["IsControl"].Value.ToString());
      if (isControl == -1)
      {
        lbRemark.Text = string.Empty;
      }
      else if (isControl == 0 || this.writeOff == 1)
      {
        lbRemark.Text = "Condition : Issue qty <= stock balance (don't check allocate).";
      }
      else
      {
        lbRemark.Text = "Condition : Issue qty <= stock balance and issue qty <= allocate.";
      }
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
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
          DBParameter[] inputParam = new DBParameter[12];
          inputParam[0] = new DBParameter("@IssueNo", DbType.AnsiString, 50, this.issueNo);
          string materialCode = row.Cells["MaterialCode"].Value.ToString();
          inputParam[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 50, materialCode);
          inputParam[2] = new DBParameter("@Qty", DbType.Double, qty);
          int location = DBConvert.ParseInt(row.Cells["LocationPid"].Value.ToString());
          inputParam[3] = new DBParameter("@Location", DbType.Int32, location);
          inputParam[4] = new DBParameter("@WriteOff", DbType.Int32, this.writeOff);
          inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          long wo = DBConvert.ParseLong(row.Cells["Wo"].Value.ToString());
          if (wo != long.MinValue)
          {
            inputParam[6] = new DBParameter("@Wo", DbType.Int64, wo);
          }
          string itemCode = row.Cells["Item"].Value.ToString();
          if (itemCode.Length > 0)
          {
            inputParam[7] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          }
          int revision = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
          if (revision != int.MinValue)
          {
            inputParam[8] = new DBParameter("@Revison", DbType.Int32, revision);
          }
          long SupplementDetailPid = DBConvert.ParseLong(row.Cells["SupplementDetailPid"].Value.ToString());
          if (SupplementDetailPid != long.MinValue)
          {
            inputParam[9] = new DBParameter("@SupplementDetailPid", DbType.Int64, SupplementDetailPid);
          }
          int isProduction = (radProduction.Checked) ? 1 : 0;
          inputParam[10] = new DBParameter("@IsProduction", DbType.Int32, isProduction);
          inputParam[11] = new DBParameter("@Department", DbType.AnsiString, 50, drpDepartment.Value.ToString());
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spWHDIssueToProductionMaterialsDetail_Insert_PMISDB", inputParam, outputParam);
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
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDIssuingNotePrint_Materials", input);
      dsMaterialsIssuing dsIssue = new dsMaterialsIssuing();
      dsIssue.Tables["dtIssuingInfo"].Merge(dsSource.Tables[0]);
      dsIssue.Tables["dtIssuingDetail"].Merge(dsSource.Tables[1]);


      DaiCo.Shared.View_Report report = null;

      cptMaterialsIssueToProduction cpt = new cptMaterialsIssueToProduction();
      cpt.SetDataSource(dsIssue);
      report = new DaiCo.Shared.View_Report(cpt);
      report.IsShowGroupTree = false;
      report.ShowReport(Shared.Utility.ViewState.ModalWindow);
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultIssueDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Wo"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Wo"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Wo"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 80;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 90;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].Format = "#,###.##";
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Location"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Location"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["EnglishName"].Header.Caption = "English Name";
      e.Layout.Bands[0].Columns["VietnameseName"].Header.Caption = "Vietnamese Name";
      e.Layout.Bands[0].Override.AllowDelete = (this.canUpdate) ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
    }

    private void ultStockData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Wo"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Wo"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Wo"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Item"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["Item"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Item"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 90;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Location"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Location"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Balance"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Balance"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Balance"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Allocate"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Allocate"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Allocate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Issue"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Issue"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Issue"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LocationPid"].Hidden = true;
      e.Layout.Bands[0].Columns["IsControl"].Hidden = true;
      e.Layout.Bands[0].Columns["SupplementDetailPid"].Hidden = true;
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
        int isControl = DBConvert.ParseInt(e.Cell.Row.Cells["IsControl"].Value.ToString());
        if (this.writeOff != 1 && isControl == 1)
        {
          double allocate = DBConvert.ParseDouble(e.Cell.Row.Cells["Allocate"].Value.ToString());
          if (issue > allocate)
          {
            WindowUtinity.ShowMessageError("ERR0010", new string[] { "Issue", "Allocate" });
            e.Cancel = true;
            return;
          }
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
        DataBaseAccess.ExecuteStoreProcedure("spWHDIssueToProductionMaterialsDetail_Delete_PMISDB", inputParam, outputParam);
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

    private void btnPrintSummary_Click(object sender, EventArgs e)
    {
      DBParameter[] input = new DBParameter[] { new DBParameter("@IssueNo", DbType.AnsiString, 50, txtIssueNo.Text.Trim()) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDIssuingNotePrintSummary_Materials", input);
      dsMaterialsIssuingSummary dsIssue = new dsMaterialsIssuingSummary();
      dsIssue.Tables["dtIssuingInfo"].Merge(dsSource.Tables[0]);
      dsIssue.Tables["dtIssuingDetail"].Merge(dsSource.Tables[1]);

      double sumqty = 0;
      foreach (DataRow row in dsIssue.Tables["dtIssuingDetail"].Rows)
      {
        if (DBConvert.ParseDouble(row["Qty"].ToString()) != double.MinValue)
        {
          sumqty = sumqty + DBConvert.ParseDouble(row["Qty"].ToString());
        }
      }

      // ReportClass cpt = null;
      DaiCo.Shared.View_Report report = null;

      cptMaterialsIssueToProductionSummary cpt = new cptMaterialsIssueToProductionSummary();
      cpt.SetDataSource(dsIssue);
      cpt.SetParameterValue("Sumqty", sumqty);
      report = new DaiCo.Shared.View_Report(cpt);
      report.IsShowGroupTree = false;
      report.ShowReport(Shared.Utility.ViewState.ModalWindow);
    }
    #endregion Events    
  }
}
