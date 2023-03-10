using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
///*
//  Author      : 
//  Date        : 24-12-2013
//  Description : List Information PR Change Request Date
//  Standard Form: view_GNR_90_002
//*/
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_21_010 : MainUserControl
  {
    #region field
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadComboCreateBy();
      this.LoadComboStatus();
      this.LoadComboDepartment();
    }

    /// <summary>
    /// Enter Search
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.SearchData();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// Load UltraCombo Create By
    /// </summary>
    private void LoadComboCreateBy()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_NhanVien, CONVERT(VARCHAR, ID_NhanVien) + ' - ' + HoNV + ' ' + TenNV Name";
      commandText += " FROM VHRNhanVien ";
      commandText += " WHERE Resigned = 0";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBCreateBy.DataSource = dtSource;
      ultCBCreateBy.DisplayMember = "Name";
      ultCBCreateBy.ValueMember = "ID_NhanVien";
      ultCBCreateBy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBCreateBy.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
      ultCBCreateBy.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load UltraCombo Status (0: New / 1: Confirm)
    /// </summary>
    private void LoadComboStatus()
    {
      string commandText = string.Empty;
      commandText += " SELECT 0 ID, 'New' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Confirmed' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Finished' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBStatus.DataSource = dtSource;
      ultCBStatus.DisplayMember = "Name";
      ultCBStatus.ValueMember = "ID";
      ultCBStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBStatus.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      ultCBStatus.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load Department
    /// </summary>
    private void LoadComboDepartment()
    {
      string commandText = string.Empty;
      commandText = "  SELECT Department, Department + ' - ' + DeparmentName AS Name";
      commandText += " FROM VHRDDepartment";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBDepartment.DataSource = dtSource;
      ultCBDepartment.DisplayMember = "Name";
      ultCBDepartment.ValueMember = "Department";
      ultCBDepartment.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultCBDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
      ultCBDepartment.DisplayLayout.AutoFitColumns = true;

      // Value Default
      ultCBDepartment.Value = SharedObject.UserInfo.Department;
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;

      DBParameter[] param = new DBParameter[9];
      if (txtCancelWOFrom.Text.Trim().Length > 0)
      {
        param[0] = new DBParameter("@NoFrom", DbType.String, txtCancelWOFrom.Text);
      }

      if (txtCancelWoTo.Text.Trim().Length > 0)
      {
        param[1] = new DBParameter("@NoTo", DbType.String, txtCancelWoTo.Text);
      }

      if (ultCBCreateBy.Text.Length > 0 && ultCBCreateBy.Value != null)
      {
        param[2] = new DBParameter("@RequestBy", DbType.Int32, DBConvert.ParseInt(ultCBCreateBy.Value.ToString()));
      }

      if (ultCBDepartment.Text.Length > 0 && ultCBDepartment.Value != null)
      {
        param[3] = new DBParameter("@Department", DbType.String, ultCBDepartment.Value.ToString());
      }

      if (ultCBStatus.Text.Length > 0 && ultCBStatus.Value != null)
      {
        param[4] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ultCBStatus.Value.ToString()));
      }

      // DateFrom
      if (ultDateFrom.Value != null)
      {
        DateTime prDateFrom = DateTime.MinValue;
        prDateFrom = (DateTime)ultDateFrom.Value;
        param[5] = new DBParameter("@DateFrom", DbType.DateTime, prDateFrom);
      }
      // DateTo
      if (ultDateTo.Value != null)
      {
        DateTime prDateTo = DateTime.MinValue;
        prDateTo = (DateTime)ultDateTo.Value;
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        param[6] = new DBParameter("@DateTo", DbType.DateTime, prDateTo);
      }

      if (txtMaterial.Text.Length > 0)
      {
        param[7] = new DBParameter("@Material", DbType.String, txtMaterial.Text);
      }

      if (txtPRNo.Text.Length > 0)
      {
        param[8] = new DBParameter("@PRNo", DbType.String, txtPRNo.Text);
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURPRGetListPRChangeRequestDate_Select", param);
      if (ds != null)
      {
        DataSet dsSource = this.CreateDataSet();
        dsSource.Tables["dtParent"].Merge(ds.Tables[0]);
        dsSource.Tables["dtChild"].Merge(ds.Tables[1]);
        dsSource.Tables["dtChildDetail"].Merge(ds.Tables[2]);
        ultraGridInformation.DataSource = dsSource;
      }

      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow row = ultraGridInformation.Rows[i];
        if (string.Compare(row.Cells["Status"].Value.ToString(), "New", true) == 0)
        {
          row.Appearance.BackColor = Color.White;
        }
        else if (string.Compare(row.Cells["Status"].Value.ToString(), "Confirmed", true) == 0)
        {
          row.Appearance.BackColor = Color.LightGreen;
        }
        else if (string.Compare(row.Cells["Status"].Value.ToString(), "Finished", true) == 0)
        {
          row.Appearance.BackColor = Color.LightSkyBlue;
        }
      }

      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtCancelWOFrom.Text = string.Empty;
      txtCancelWoTo.Text = string.Empty;
      ultDateFrom.Value = DBNull.Value;
      ultDateTo.Value = DBNull.Value;
      ultCBCreateBy.Value = DBNull.Value;
      ultCBStatus.Value = DBNull.Value;
      ultCBDepartment.Value = DBNull.Value;
      txtMaterial.Text = string.Empty;
      txtPRNo.Text = string.Empty;
    }

    /// <summary>
    /// Create DataSet
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("ChangeDateNo", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("RequestBy", typeof(System.String));
      taParent.Columns.Add("Department", typeof(System.String));
      taParent.Columns.Add("RemarkInfo", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("PRDetailPid", typeof(System.Int64));
      taChild.Columns.Add("PROnlineNo", typeof(System.String));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("NameEN", typeof(System.String));
      taChild.Columns.Add("NameVN", typeof(System.String));
      taChild.Columns.Add("Unit", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("RequestDate", typeof(System.String));
      ds.Tables.Add(taChild);

      DataTable taChildDetail = new DataTable("dtChildDetail");
      taChildDetail.Columns.Add("Pid", typeof(System.Int64));
      taChildDetail.Columns.Add("PRDetailPid", typeof(System.Int64));
      taChildDetail.Columns.Add("QtySeparate", typeof(System.Double));
      taChildDetail.Columns.Add("RequestDateSeparete", typeof(System.String));
      ds.Tables.Add(taChildDetail);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["Pid"], false));
      ds.Relations.Add(new DataRelation("dtChild_dtChildDetail", new DataColumn[] { taChild.Columns["Pid"], taChild.Columns["PRDetailPid"] }, new DataColumn[] { taChildDetail.Columns["Pid"], taChildDetail.Columns["PRDetailPid"] }, false));
      return ds;
    }
    #endregion function

    #region event
    public viewPUR_21_010()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_21_010_Load(object sender, EventArgs e)
    {
      txtCancelWOFrom.Focus();
      ultDateFrom.Value = DBNull.Value;
      ultDateTo.Value = DBNull.Value;
      this.InitData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCondition();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultraGridInformation, "Data");
    }
    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      ControlUtility.SetPropertiesUltraGrid(ultraGridInformation);

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["PRDetailPid"].Hidden = true;
      e.Layout.Bands[2].Columns["Pid"].Hidden = true;
      e.Layout.Bands[2].Columns["PRDetailPid"].Hidden = true;

      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[2].Columns["QtySeparate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[2].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[2].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[2].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultraGridInformation_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultraGridInformation.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultraGridInformation.Selected.Rows[0];
      if (row.ParentRow == null)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        string status = row.Cells["Status"].Value.ToString();
        string department = row.Cells["Department"].Value.ToString();
        if (string.Compare(status, "New", true) == 0 &&
            string.Compare(department, SharedObject.UserInfo.Department, true) != 0)
        {
          return;
        }
        viewPUR_21_011 uc = new viewPUR_21_011();
        uc.pid = pid;
        WindowUtinity.ShowView(uc, "UPDATE PR CHANGE REQUEST DATE", false, ViewState.MainWindow);
      }
    }

    private void btnNewChange_Click(object sender, EventArgs e)
    {
      viewPUR_21_011 view = new viewPUR_21_011();
      WindowUtinity.ShowView(view, "UPDATE PR CHANGE REQUEST DATE", true, DaiCo.Shared.Utility.ViewState.MainWindow);
    }
    #endregion event


  }
}
