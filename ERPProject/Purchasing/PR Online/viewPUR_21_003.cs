/*
  Author      : 
  Date        : 4/7/2013
  Description : List Of PR Cancel
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPUR_21_003 : MainUserControl
  {
    #region Field
    bool flagUpdate = false;
    #endregion Field

    #region Init Form

    public viewPUR_21_003()
    {
      InitializeComponent();
      this.drpDateFrom.FormatString = ConstantClass.FORMAT_DATETIME;
      this.drpDateTo.FormatString = ConstantClass.FORMAT_DATETIME;
    }

    /// <summary>
    /// frmBOMListPackage_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_21_098_Load(object sender, EventArgs e)
    {
      if (this.btnPur.Visible)
      {
        this.flagUpdate = true;
      }
      else
      {
        this.flagUpdate = false;
      }
      this.btnPur.Visible = false;

      // Load Status
      this.LoadStatus();

      // Load Department
      this.LoadComboDepartment();

      // Load Create By
      this.LoadCreateBy();
    }

    /// <summary>
    /// Load UltraCombo Department
    /// </summary>
    private void LoadComboDepartment()
    {
      string commandText = @"SELECT Department, Department + ' - ' + DeparmentName AS Name 
	                         FROM VHRDDepartment 
	                         WHERE Department IS NOT NULL 
	                         ORDER BY Department";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDepartment.DataSource = dtSource;
      ultDepartment.DisplayMember = "Name";
      ultDepartment.ValueMember = "Department";
      ultDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDepartment.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
    }

    /// <summary>
    /// Load Status
    /// </summary>
    private void LoadStatus()
    {
      DataTable dtStatus = new DataTable();
      dtStatus.Columns.Add("Id", typeof(System.Int32));
      dtStatus.Columns.Add("Status", typeof(System.String));

      DataRow row = dtStatus.NewRow();
      row["Id"] = DBNull.Value;
      row["Status"] = "ALL";
      dtStatus.Rows.Add(row);

      row = dtStatus.NewRow();
      row["Id"] = 0;
      row["Status"] = "New";
      dtStatus.Rows.Add(row);

      row = dtStatus.NewRow();
      row["Id"] = 1;
      row["Status"] = "Confirmed";
      dtStatus.Rows.Add(row);

      row = dtStatus.NewRow();
      row["Id"] = 2;
      row["Status"] = "Finished";
      dtStatus.Rows.Add(row);

      ultStatus.DataSource = dtStatus;
      ultStatus.ValueMember = "Id";
      ultStatus.DisplayMember = "Status";
      ultStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultStatus.DisplayLayout.Bands[0].Columns["Id"].Hidden = true;
    }

    /// <summary>
    /// Load Create By
    /// </summary>
    private void LoadCreateBy()
    {
      string commnadText = @"SELECT Pid ID_NhanVien , CAST(Pid AS VARCHAR) + ' - ' + EmpName [Name]
                             FROM VHRMEmployee
                             WHERE Pid IS NOT NULL";

      DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commnadText);

      ultCreateBy.DataSource = dtCheck;
      ultCreateBy.ValueMember = "ID_NhanVien";
      ultCreateBy.DisplayMember = "Name";
      ultCreateBy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCreateBy.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
      ultCreateBy.DisplayLayout.Bands[0].Columns["Name"].Width = 400;
    }
    #endregion Init Form

    #region Process
    private void Search()
    {
      DateTime dateFrom = (drpDateFrom.Value != null ? (DateTime)drpDateFrom.Value : DateTime.MinValue);
      DateTime dateTo = (drpDateTo.Value != null ? (DateTime)drpDateTo.Value : DateTime.MinValue);
      if (dateTo != DateTime.MinValue)
      {
        dateTo = dateTo.AddDays(1);
      }

      DBParameter[] inputParam = new DBParameter[11];

      string text = txtPRCancelNo.Text.Trim();
      // PR Cancel No
      if (text.Length > 0)
      {
        inputParam[0] = new DBParameter("@PRCancelNo", DbType.String, text);
      }

      // Create By
      if (this.ultCreateBy.Value != null)
      {
        inputParam[1] = new DBParameter("@RequestBy", DbType.Int32, DBConvert.ParseInt(this.ultCreateBy.Value.ToString()));
      }

      // Department
      if (this.ultDepartment.Value != null)
      {
        inputParam[2] = new DBParameter("@Department", DbType.String, this.ultDepartment.Value.ToString());
      }

      // Status
      if (this.ultStatus.Value != null && DBConvert.ParseInt(this.ultStatus.Value.ToString()) != int.MinValue)
      {
        inputParam[3] = new DBParameter("@Status", DbType.Int32, this.ultStatus.Value.ToString());
      }

      // Date From 
      if (dateFrom != DateTime.MinValue)
      {
        inputParam[4] = new DBParameter("@DateFrom", DbType.DateTime, dateFrom);
      }

      // Date To
      if (dateTo != DateTime.MinValue)
      {
        inputParam[5] = new DBParameter("@DateTo", DbType.DateTime, dateTo);
      }

      // MaterialCode
      text = txtMaterial.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[6] = new DBParameter("@MaterialCode", DbType.String, text);
      }

      // WO
      text = txtWO.Text.Trim();
      if (text.Length > 0 && DBConvert.ParseInt(text) != int.MinValue)
      {
        inputParam[7] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(text));
      }

      // Carcass Code
      text = txtCarcassCode.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[8] = new DBParameter("@CarcassCode", DbType.String, text);
      }

      // Item Code
      text = txtItemCode.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[9] = new DBParameter("@ItemCode", DbType.String, text);
      }

      text = txtPRNo.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[10] = new DBParameter("@PRNo", DbType.String, text);
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURPRCancelInformationList_Select", inputParam);
      DataSet dsList = this.ListGridPRCancel();
      // Master
      dsList.Tables["TblMaster"].Merge(ds.Tables[0]);
      // Detail
      dsList.Tables["TblDetail"].Merge(ds.Tables[1]);

      ultData.DataSource = dsList;
    }
    #endregion Process

    #region Function
    /// <summary>
    /// Dataset For Grid
    /// </summary>
    /// <returns></returns>
    private DataSet ListGridPRCancel()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("TblMaster");
      taParent.Columns.Add("PID", typeof(System.Int64));
      taParent.Columns.Add("PRCancelNo", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("Department", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));

      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("TblDetail");
      taChild.Columns.Add("PID", typeof(System.Int64));
      taChild.Columns.Add("PRNo", typeof(System.String));
      taChild.Columns.Add("WO", typeof(System.Int32));
      taChild.Columns.Add("CarcassCode", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("NameEN", typeof(System.String));
      taChild.Columns.Add("NameVN", typeof(System.String));
      taChild.Columns.Add("Unit", typeof(System.String));
      taChild.Columns.Add("QtyCancel", typeof(System.Double));
      taChild.Columns.Add("QtyCancelReal", typeof(System.Double));
      taChild.Columns.Add("Status", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("TblMaster_TblDetail", taParent.Columns["PID"], taChild.Columns["PID"], false));
      return ds;
    }
    #endregion Function

    #region Event
    /// <summary>
    /// btnSearch_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.btnSearch.Enabled = false;

      // Search
      this.Search();

      this.btnSearch.Enabled = true;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// btnNew_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPUR_21_004 view = new viewPUR_21_004();
      view.flagPur = this.flagUpdate;
      Shared.Utility.WindowUtinity.ShowView(view, "PR CANCEL INFORMATION", false, Shared.Utility.ViewState.MainWindow);
    }

    /// <summary>
    /// Close tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// InitializeLayout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["PRCancelNo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Status"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Department"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Status"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[1].Columns["Status"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["PRNo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["WO"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["CarcassCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["NameVN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["QtyCancel"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["QtyCancelReal"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Status"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[1].Columns["WO"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["QtyCancel"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["QtyCancelReal"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[1].Columns["QtyCancelReal"].Header.Caption = "Pur CancelQty";

      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      if (ultData.Selected.Rows.Count > 0 && ultData.Selected.Rows[0].Band.ParentBand == null)
      {
        long pid = DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["Pid"].Value.ToString());
        string status = ultData.Selected.Rows[0].Cells["Status"].Value.ToString();
        if (!this.flagUpdate || string.Compare(status, "New", true) == 0)
        {
          viewPUR_21_004 view = new viewPUR_21_004();
          view.prCancelPid = pid;
          if (this.flagUpdate)
          {
            view.flagPur = true;
          }
          WindowUtinity.ShowView(view, "PR CANCEL INFORMATION", true, ViewState.MainWindow);
        }
        else
        {
          viewPUR_21_007 view = new viewPUR_21_007();
          view.prCancelPid = pid;
          WindowUtinity.ShowView(view, "PR CANCEL INFORMATION", true, ViewState.MainWindow);
        }
      }
    }
    #endregion Event
  }
}
