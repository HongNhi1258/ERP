/*
  Author      : 
  Date        : 03/07/2013
  Description : List PR Online
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
  public partial class viewPUR_21_005 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewPUR_21_005()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_21_005_Load(object sender, EventArgs e)
    {
      drpDateFrom.Value = DateTime.Today.AddDays(-7);
      // Load Init
      this.LoadInit();
    }

    /// <summary>
    /// Load Init
    /// </summary>
    private void LoadInit()
    {
      // Load UltraCombo Status
      this.LoadComboStatus();
      // Load UltraCombo Create By
      this.LoadComboCreateBy();
      // Load UltraCombo Department
      this.LoadComboDepartment();
      // Load WO
      this.LoadWO();
      // Load Group
      this.LoadUltraComboGroup();
    }

    /// <summary>
    /// Load UltraCombo Create By
    /// </summary>
    private void LoadComboCreateBy()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_NhanVien, CONVERT(VARCHAR, ID_NhanVien) + ' - ' + EmpName";
      commandText += " FROM VHRNhanVien ";
      commandText += " WHERE Resigned = 0";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      Utility.LoadUltraCombo(ultCBRequestBy, dtSource, "ID_NhanVien", "Name", false, "ID_NhanVien");
    }

    /// <summary>
    /// Load UltraCombo Department
    /// </summary>
    private void LoadComboDepartment()
    {
      string commandText = string.Empty;
      commandText += "SELECT Department, Department +' - ' + DeparmentName AS DeparmentName FROM VHRDDepartment WHERE Department IS NOT NULL";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      Utility.LoadUltraCombo(ultCBDepartment, dtSource, "Department", "DeparmentName", false, "Department");
    }

    /// <summary>
    /// Load UltraCombo Status (0: New / 1: Confirm / 2: Finished)
    /// </summary>
    private void LoadComboStatus()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 0 ID, 'New' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Request Confirmed' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Head Department Confirmed' Name";
      commandText += " UNION";
      commandText += " SELECT 3 ID, 'Waiting For Receiving' Name";
      commandText += " UNION";
      commandText += " SELECT 4 ID, 'Finished' Name";
      commandText += " UNION";
      commandText += " SELECT 5 ID, 'Cancel' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      Utility.LoadUltraCombo(ultCBStatus, dtSource, "ID", "Name", false, "ID");
      ultCBStatus.Value = 2;
    }

    /// <summary>
    /// Load WO
    /// </summary>
    private void LoadWO()
    {
      string commandText = "SELECT DISTINCT WorkOrderPid AS WO FROM TblPLNWorkOrderConfirmedDetails";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt == null)
      {
        return;
      }
      ultCBWO.DataSource = dt;
      ultCBWO.DisplayMember = "WO";
      ultCBWO.ValueMember = "WO";
      ultCBWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBWO.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load Carcass
    /// </summary>
    /// <param name="wo"></param>
    private void LoadCarcass(int wo)
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT CarcassCode FROM TblPLNWorkOrderConfirmedDetails";
      commandText += " WHERE WorkOrderPid = " + wo;
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBCarcass.DataSource = dtSource;
      ultCBCarcass.DisplayMember = "CarcassCode";
      ultCBCarcass.ValueMember = "CarcassCode";
      ultCBCarcass.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBCarcass.DisplayLayout.AutoFitColumns = true;
    }

    #endregion Init

    #region Function
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;

      DBParameter[] input = new DBParameter[11];
      // Request From
      if (txtPROnlineFrom.Text.Length > 0)
      {
        input[0] = new DBParameter("@PROnlineFrom", DbType.String, txtPROnlineFrom.Text.Trim());
      }
      // Request To
      if (txtPROnlineTo.Text.Length > 0)
      {
        input[1] = new DBParameter("@PROnlineTo", DbType.String, txtPROnlineTo.Text.Trim());
      }
      // DateFrom
      if (drpDateFrom.Value != null)
      {
        DateTime prDateFrom = DateTime.MinValue;
        prDateFrom = (DateTime)drpDateFrom.Value;
        input[2] = new DBParameter("@DateFrom", DbType.DateTime, prDateFrom);
      }
      // DateTo
      if (drpDateTo.Value != null)
      {
        DateTime prDateTo = DateTime.MinValue;
        prDateTo = (DateTime)drpDateTo.Value;
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        input[3] = new DBParameter("@DateTo", DbType.DateTime, prDateTo);
      }
      // CreateBy
      if (ultCBRequestBy.Value != null)
      {
        input[4] = new DBParameter("@Requester", DbType.Int32, DBConvert.ParseInt(ultCBRequestBy.Value.ToString()));
      }
      // Status
      if (ultCBStatus.Value != null && DBConvert.ParseInt(ultCBStatus.Value.ToString()) != int.MinValue)
      {
        input[5] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ultCBStatus.Value.ToString()));
      }
      // WO
      if (ultCBWO.Value != null)
      {
        input[6] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(ultCBWO.Value.ToString()));
      }
      // Carcas
      if (ultCBCarcass.Value != null)
      {
        input[7] = new DBParameter("@Carcass", DbType.String, ultCBCarcass.Value.ToString());
      }
      // Material
      if (txtMaterial.Text.Trim().Length > 0)
      {
        input[8] = new DBParameter("@Material", DbType.String, txtMaterial.Text.Trim());
      }
      // Department Request
      if (ultCBDepartment.Value != null)
      {
        input[9] = new DBParameter("@Department", DbType.String, ultCBDepartment.Value.ToString());
      }

      // Group
      if (ultComboGroupInCharge.Value != null)
      {
        input[10] = new DBParameter("@Group", DbType.Int32, DBConvert.ParseInt(ultComboGroupInCharge.Value));
      }
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURPROnlineListPRInformation_Select", input);
      if (ds != null)
      {
        DataSet dsSource = this.CreateDataSet();
        dsSource.Tables["dtParent"].Merge(ds.Tables[0]);
        dsSource.Tables["dtChild"].Merge(ds.Tables[1]);
        ultData.DataSource = dsSource;

        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Status"].Value.ToString()) == 0)
          {
            row.Appearance.BackColor = Color.White;
          }
          else if (DBConvert.ParseInt(row.Cells["Status"].Value.ToString()) == 1)
          {
            row.Appearance.BackColor = Color.LightGreen;
          }
          else if (DBConvert.ParseInt(row.Cells["Status"].Value.ToString()) == 2)
          {
            row.Appearance.BackColor = Color.Pink;
          }
          else if (DBConvert.ParseInt(row.Cells["Status"].Value.ToString()) == 3)
          {
            row.Appearance.BackColor = Color.Yellow;
          }
          else if (DBConvert.ParseInt(row.Cells["Status"].Value.ToString()) == 4)
          {
            row.Appearance.BackColor = Color.LightSkyBlue;
          }
          else if (DBConvert.ParseInt(row.Cells["Status"].Value.ToString()) == 5)
          {
            row.Appearance.BackColor = Color.Gray;
          }
        }
      }
      // Enable button search
      btnSearch.Enabled = true;
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
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// Create DataSet
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("PID", typeof(System.Int64));
      taParent.Columns.Add("PROnlineNo", typeof(System.String));
      taParent.Columns.Add("Department", typeof(System.String));
      taParent.Columns.Add("Requester", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("PurposeOfRequisition", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.Int32));
      taParent.Columns.Add("StatusName", typeof(System.String));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("PROnlinePid", typeof(System.Int64));
      taChild.Columns.Add("WO", typeof(System.Int32));
      taChild.Columns.Add("CarcassCode", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("LastPOSupplier", typeof(System.String));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("MaterialName", typeof(System.String));
      taChild.Columns.Add("Unit", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("RequestDate", typeof(System.String));
      taChild.Columns.Add("StatusDetail", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["PROnlinePid"], false));
      return ds;
    }
    #endregion Function

    #region Event

    /// <summary>
    /// Clear
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      txtPROnlineFrom.Text = string.Empty;
      txtPROnlineTo.Text = string.Empty;
      ultCBStatus.Text = string.Empty;
      ultCBRequestBy.Text = string.Empty;
      ultCBWO.Text = string.Empty;
      ultCBCarcass.Text = string.Empty;
      ultCBDepartment.Text = string.Empty;
      txtMaterial.Text = string.Empty;
    }

    /// <summary>
    /// Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      Utility.SetPropertiesUltraGrid(ultData);

      e.Layout.Bands[0].Columns["PID"].Hidden = true;
      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[1].Columns["PROnlinePid"].Hidden = true;
      e.Layout.Bands[0].Columns["StatusName"].Header.Caption = "Status";
      e.Layout.Bands[1].Columns["LastPOSupplier"].Header.Caption = "Last PO Supplier";
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;

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
      bool selected = false;
      try
      {
        selected = ultData.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (ultData.Selected.Rows[0].ParentRow == null) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ParentRow;
      long pid = DBConvert.ParseLong(row.Cells["PID"].Value.ToString());

      viewPUR_21_006 uc = new viewPUR_21_006();
      uc.PRPid = pid;
      WindowUtinity.ShowView(uc, "APPROVED PR ONLINE", false, ViewState.MainWindow);
    }

    /// <summary>
    /// WO Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCBWO_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBWO.Value != null && ultCBWO.Text.ToString().Length > 0)
      {
        ultCBCarcass.Value = null;
        this.LoadCarcass(DBConvert.ParseInt(ultCBWO.Value.ToString()));
      }
      else
      {
        ultCBCarcass.Value = null;
      }
    }


    /// <summary>
    /// New Requisition Special
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {

    }

    private void LoadUltraComboGroup()
    {
      string commandText = "SELECT Pid, GroupName FROM TblPURStaffGroup WHERE DeleteFlg = 0";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);
      ultComboGroupInCharge.DataSource = dtSource;
      ultComboGroupInCharge.DisplayMember = "GroupName";
      ultComboGroupInCharge.ValueMember = "Pid";
      ultComboGroupInCharge.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultComboGroupInCharge.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultComboGroupInCharge.DisplayLayout.Bands[0].Columns["GroupName"].Width = 300;

      // Load Group In Charge(User Login)
      string commmadText = string.Format(@" SELECT SG.Pid
                                                FROM TblPURStaffGroup SG
	                                                LEFT JOIN TblPURStaffGroupDetail SGD ON SG.Pid = SGD.[Group]
                                                WHERE SG.DeleteFlg = 0 AND SG.LeaderGroup = {0} OR SGD.Employee = {1}", SharedObject.UserInfo.UserPid, SharedObject.UserInfo.UserPid);
      DataTable dtGroupInCharge = DataBaseAccess.SearchCommandTextDataTable(commmadText);
      if (dtGroupInCharge != null && dtGroupInCharge.Rows.Count > 0)
      {
        ultComboGroupInCharge.Value = DBConvert.ParseInt(dtGroupInCharge.Rows[0]["Pid"].ToString());
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
    #endregion Event
  }
}
