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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewPUR_21_001 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewPUR_21_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_21_001_Load(object sender, EventArgs e)
    {
      drpDateFrom.Value = DBNull.Value;
      drpDateTo.Value = DBNull.Value;

      drpDateFrom.FormatProvider = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER);
      drpDateFrom.FormatString = ConstantClass.FORMAT_DATETIME;
      drpDateTo.FormatProvider = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER);
      drpDateTo.FormatString = ConstantClass.FORMAT_DATETIME;

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
    }

    /// <summary>
    /// Load UltraCombo Create By
    /// </summary>
    private void LoadComboCreateBy()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_NhanVien, (CONVERT(VARCHAR, ID_NhanVien) + ' - ' + EmpName) Name";
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
      commandText += " SELECT Department, Department + ' - ' + DeparmentName AS DeparmentName FROM VHRDDepartment";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      Utility.LoadUltraCombo(ultCBDepartment, dtSource, "Department", "DeparmentName", false, "Department");

      // Load Defaul Department User Login
      ultCBDepartment.Value = SharedObject.UserInfo.Department;
    }

    /// <summary>
    /// Load UltraCombo Status (0: New / 1: Confirm / 2: Finished)
    /// </summary>
    private void LoadComboStatus()
    {
      string commandText = "SELECT ID, [Name] FROM VPURPROnlineStatus";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      Utility.LoadUltraCombo(ultCBStatus, dtSource, "ID", "Name", false, "ID");
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
      Utility.LoadUltraCombo(ultCBWO, dt, "WO", "WO", false);
    }
    
    #endregion Init

    #region Function
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;

      DBParameter[] input = new DBParameter[10];
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
          // Child Band
          for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow rowChild = row.ChildBands[0].Rows[j];
            rowChild.Cells["RequestDate"].Appearance.ForeColor = Color.Blue;
            rowChild.Cells["Note"].Appearance.ForeColor = Color.Blue;
            rowChild.Cells["QtyRequest"].Appearance.ForeColor = Color.Blue;
            rowChild.Cells["Qty"].Appearance.ForeColor = Color.Blue;
            rowChild.Cells["QtyCancel"].Appearance.ForeColor = Color.Blue;
            rowChild.Cells["ReceiptedQty"].Appearance.ForeColor = Color.Blue;
            rowChild.Cells["RequestDate"].Appearance.FontData.Bold = DefaultableBoolean.True;
            rowChild.Cells["Note"].Appearance.FontData.Bold = DefaultableBoolean.True;
            rowChild.Cells["QtyRequest"].Appearance.FontData.Bold = DefaultableBoolean.True;
            rowChild.Cells["Qty"].Appearance.FontData.Bold = DefaultableBoolean.True;
            rowChild.Cells["QtyCancel"].Appearance.FontData.Bold = DefaultableBoolean.True;
            rowChild.Cells["ReceiptedQty"].Appearance.FontData.Bold = DefaultableBoolean.True;
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
      taChild.Columns.Add("Note", typeof(System.String));
      taChild.Columns.Add("RequestDate", typeof(System.String));
      taChild.Columns.Add("WO", typeof(System.Int32));
      taChild.Columns.Add("CarcassCode", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("MaterialName", typeof(System.String));
      taChild.Columns.Add("Unit", typeof(System.String));
      taChild.Columns.Add("QtyRequest", typeof(System.Double));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("QtyCancel", typeof(System.Double));
      taChild.Columns.Add("ReceiptedQty", typeof(System.Double));
      taChild.Columns.Add("ProjectCode", typeof(System.String));
      taChild.Columns.Add("StatusDetail", typeof(System.String));
      taChild.Columns.Add("Purpose", typeof(System.String));
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
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["PID"].Hidden = true;
      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[1].Columns["PROnlinePid"].Hidden = true;
      //Caption
      e.Layout.Bands[0].Columns["StatusName"].Header.Caption = "Trạng thái";
      e.Layout.Bands[0].Columns["PROnlineNo"].Header.Caption = "Phiếu ĐNMH";
      e.Layout.Bands[0].Columns["Department"].Header.Caption = "Bộ phận";
      e.Layout.Bands[0].Columns["Requester"].Header.Caption = "Người yêu cầu";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Ngày tạo";
      e.Layout.Bands[0].Columns["PurposeOfRequisition"].Header.Caption = "Mục đích";

      e.Layout.Bands[1].Columns["Qty"].Header.Caption = "Số lượng\nmua hàng";
      e.Layout.Bands[1].Columns["LastPOSupplier"].Header.Caption = "Nhà cung cấp\ngần nhất";
      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Mã sản phẩm";
      e.Layout.Bands[1].Columns["MaterialNameEn"].Header.Caption = "Tên SP (EN)";
      e.Layout.Bands[1].Columns["MaterialNameVn"].Header.Caption = "Tên SP (VN)";
      e.Layout.Bands[1].Columns["Unit"].Header.Caption = "Đơn vị";
      e.Layout.Bands[1].Columns["QtyCancel"].Header.Caption = "Số lượng\nhủy";
      e.Layout.Bands[1].Columns["ReceiptedQty"].Header.Caption = "Số lượng\nđã nhận";
      e.Layout.Bands[1].Columns["StatusDetail"].Header.Caption = "Trạng thái";
      e.Layout.Bands[1].Columns["QtyRequest"].Header.Caption = "Số lượng\nyêu cầu ban đầu";
      e.Layout.Bands[1].Columns["RequestDate"].Header.Caption = "Ngày yêu cầu\nhàng về";
      e.Layout.Bands[1].Columns["Purpose"].Header.Caption = "Mục đích";
      // Set header height
      e.Layout.Bands[1].ColHeaderLines = 2;

      e.Layout.Bands[1].Columns["QtyRequest"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["QtyCancel"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["ReceiptedQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
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

      string departmentRequest = row.Cells["Department"].Value.ToString();
      string departmentLogin = SharedObject.UserInfo.Department;
      int userLogin = SharedObject.UserInfo.UserPid;

      // Get Head Department
      string commandText = "SELECT Manager FROM VHRDDepartmentInfoForApprove WHERE Code = '" + departmentRequest + "'" + " AND Manager = " + userLogin;
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      int headDepartment = int.MinValue;
      if (dt != null && dt.Rows.Count > 0)
      {
        headDepartment = DBConvert.ParseInt(dt.Rows[0]["Manager"].ToString());
      }
      // Status
      int status = DBConvert.ParseInt(row.Cells["Status"].Value.ToString());

      if (string.Compare(departmentRequest, departmentLogin, true) == 0 ||
          userLogin == headDepartment || status >= 2)
      {
        viewPUR_21_002 uc = new viewPUR_21_002();
        uc.PRPid = pid;
        WindowUtinity.ShowView(uc, "UPDATE PR ONLINE", false, ViewState.MainWindow);
      }
    }

    /// <summary>
    /// WO Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCBWO_ValueChanged(object sender, EventArgs e)
    {
      
    }


    /// <summary>
    /// New Requisition Special
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPUR_21_002 view = new viewPUR_21_002();
      WindowUtinity.ShowView(view, "UPDATE PR ONLINE ", true, DaiCo.Shared.Utility.ViewState.MainWindow);
    }

    /// <summary>
    /// Export Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultData, "Data");
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
