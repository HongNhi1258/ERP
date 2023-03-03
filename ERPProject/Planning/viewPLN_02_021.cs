/*
  Author      : 
  Date        : 18-11-2013
  Description : Search Information Cancel WO
  Standard Form: view_GNR_90_002
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
  public partial class viewPLN_02_021 : MainUserControl
  {
    #region field
    private string stringType = string.Empty;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadComboCreateBy();
      this.LoadComboStatus();
      this.LoadWorkOrder();
      this.LoadComboType();
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
    /// Load UltraCombo Type
    /// </summary>
    private void LoadComboType()
    {
      string commandText = string.Empty;
      commandText += " SELECT 0 ID, 'Cancel WO' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Separate WO' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBType.DataSource = dtSource;
      ultCBType.DisplayMember = "Name";
      ultCBType.ValueMember = "ID";
      ultCBType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBType.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      ultCBType.DisplayLayout.AutoFitColumns = true;

      if (string.Compare("CANCEL", this.stringType, true) == 0)
      {
        ultCBType.Value = 0;
      }
      else
      {
        ultCBType.Value = 1;
      }
    }

    private void LoadWorkOrder()
    {
      string commandText = string.Empty;
      commandText = " SELECT DISTINCT WorkOrderPid WO";
      commandText += " FROM TblPLNWorkOrderConfirmedDetails";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBWO.DataSource = dtSource;
      ultCBWO.DisplayMember = "WO";
      ultCBWO.ValueMember = "WO";
      ultCBWO.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultCBWO.DisplayLayout.AutoFitColumns = true;
    }

    private void LoadCarcassCode(int wo)
    {
      string commandText = string.Empty;
      commandText = " SELECT DISTINCT CarcassCode";
      commandText += " FROM TblPLNWorkOrderConfirmedDetails";
      commandText += " WHERE WorkOrderPid = " + wo;

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBCarcassCode.DataSource = dtSource;
      ultCBCarcassCode.DisplayMember = "CarcassCode";
      ultCBCarcassCode.ValueMember = "CarcassCode";
      ultCBCarcassCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultCBCarcassCode.DisplayLayout.AutoFitColumns = true;
    }

    private void LoadItemCode(int wo)
    {
      string commandText = string.Empty;
      commandText = " SELECT DISTINCT ItemCode, Revision";
      commandText += " FROM TblPLNWorkOrderConfirmedDetails";
      commandText += " WHERE WorkOrderPid = " + wo;

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBItemCode.DataSource = dtSource;
      ultCBItemCode.DisplayMember = "ItemCode";
      ultCBItemCode.ValueMember = "ItemCode";
      ultCBItemCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultCBItemCode.DisplayLayout.AutoFitColumns = true;
    }
    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;

      DBParameter[] param = new DBParameter[10];
      if (txtCancelWOFrom.Text.Trim().Length > 0)
      {
        param[0] = new DBParameter("@CancelWONoFrom", DbType.String, txtCancelWOFrom.Text);
      }

      if (txtCancelWoTo.Text.Trim().Length > 0)
      {
        param[1] = new DBParameter("@CancelWONoTo", DbType.String, txtCancelWoTo.Text);
      }

      if (ultCBWO.Text.Length > 0 && ultCBWO.Value != null)
      {
        param[2] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(ultCBWO.Value.ToString()));
      }

      if (ultCBCarcassCode.Text.Length > 0 && ultCBCarcassCode.Value != null)
      {
        param[3] = new DBParameter("@CarcassCode", DbType.String, ultCBCarcassCode.Value.ToString());
      }

      if (ultCBItemCode.Text.Length > 0 && ultCBItemCode.Value != null)
      {
        param[4] = new DBParameter("@ItemCode", DbType.String, ultCBItemCode.Value.ToString());
      }

      if (ultCBCreateBy.Text.Length > 0 && ultCBCreateBy.Value != null)
      {
        param[5] = new DBParameter("@CreateBy", DbType.Int32, DBConvert.ParseInt(ultCBCreateBy.Value.ToString()));
      }

      if (ultCBStatus.Text.Length > 0 && ultCBStatus.Value != null)
      {
        param[6] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ultCBStatus.Value.ToString()));
      }

      // DateFrom
      if (ultDateFrom.Value != null)
      {
        DateTime prDateFrom = DateTime.MinValue;
        prDateFrom = (DateTime)ultDateFrom.Value;
        param[7] = new DBParameter("@DateFrom", DbType.DateTime, prDateFrom);
      }
      // DateTo
      if (ultDateTo.Value != null)
      {
        DateTime prDateTo = DateTime.MinValue;
        prDateTo = (DateTime)ultDateTo.Value;
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        param[8] = new DBParameter("@DateTo", DbType.DateTime, prDateTo);
      }

      if (string.Compare("CANCEL", this.stringType, true) == 0)
      {
        param[9] = new DBParameter("@Type", DbType.Int32, DBConvert.ParseInt(ultCBType.Value.ToString()));
      }
      if (ultCBType.Value != null && ultCBType.Value.ToString().Length > 0)
      {
        param[9] = new DBParameter("@Type", DbType.Int32, DBConvert.ParseInt(ultCBType.Value.ToString()));
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNCancelWOGetList_Select", param);
      if (ds != null)
      {
        DataSet dsSource = this.CreateDataSet();
        dsSource.Tables["dtParent"].Merge(ds.Tables[0]);
        dsSource.Tables["dtChild"].Merge(ds.Tables[1]);
        ultraGridInformation.DataSource = dsSource;

        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          UltraGridRow row = ultraGridInformation.Rows[i];
          if (String.Compare(row.Cells["Status"].Value.ToString(), "New", true) == 0)
          {
            row.Appearance.BackColor = Color.White;
          }
          else if (String.Compare(row.Cells["Status"].Value.ToString(), "Confirmed", true) == 0)
          {
            row.Appearance.BackColor = Color.Yellow;
          }
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
      ultCBWO.Value = DBNull.Value;
      ultCBCarcassCode.Value = DBNull.Value;
      ultCBItemCode.Value = DBNull.Value;
      txtCancelWOFrom.Focus();
      ultCBType.Value = DBNull.Value;
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
      taParent.Columns.Add("CancelWONo", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("Reason", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("Type", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("WO", typeof(System.Int32));
      taChild.Columns.Add("CarcassCode", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("QtyCancel", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["Pid"], false));
      return ds;
    }
    #endregion function

    #region event
    public viewPLN_02_021()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_021_Load(object sender, EventArgs e)
    {
      this.stringType = this.ViewParam;

      txtCancelWOFrom.Focus();
      ultDateFrom.Value = DBNull.Value;
      ultDateTo.Value = DBNull.Value;
      //Init Data
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

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      Utility.SetPropertiesUltraGrid(ultraGridInformation);

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;

      if (string.Compare("CANCEL", this.stringType, true) == 0)
      {
        e.Layout.Bands[0].Columns["Tab4"].Hidden = false;
        e.Layout.Bands[0].Columns["Tab5"].Hidden = false;

        e.Layout.Bands[0].Columns["Tab1"].Header.Caption = "PLN WO Data";
        e.Layout.Bands[0].Columns["Tab2"].Header.Caption = "PLN Allocate Material";
        e.Layout.Bands[0].Columns["Tab3"].Header.Caption = "COM1";
        e.Layout.Bands[0].Columns["Tab4"].Header.Caption = "CST";
        e.Layout.Bands[0].Columns["Tab5"].Header.Caption = "WIPCAR";
      }
      else
      {
        e.Layout.Bands[0].Columns["Tab1"].Header.Caption = "PLN WO Data";
        e.Layout.Bands[0].Columns["Tab2"].Header.Caption = "PLN Allocate Material";
        e.Layout.Bands[0].Columns["Tab3"].Header.Caption = "WIPCAR";

        e.Layout.Bands[0].Columns["Tab4"].Hidden = true;
        e.Layout.Bands[0].Columns["Tab5"].Hidden = true;
      }

      e.Layout.Bands[0].Columns["Tab1"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Tab2"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Tab3"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Tab4"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Tab5"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[1].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["QtyCancel"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

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

      UltraGridRow row = (ultraGridInformation.Selected.Rows[0].ParentRow == null) ? ultraGridInformation.Selected.Rows[0] : ultraGridInformation.Selected.Rows[0].ParentRow;
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      int type = DBConvert.ParseInt(row.Cells["Type"].Value.ToString());

      if (type == 0)
      {
        viewPLN_02_019 uc = new viewPLN_02_019();
        uc.cancelWOPid = pid;
        WindowUtinity.ShowView(uc, "UPDATE CANCEL WO", false, ViewState.MainWindow);
      }
      else
      {
        viewPLN_02_023 uc = new viewPLN_02_023();
        uc.cancelWOPid = pid;
        WindowUtinity.ShowView(uc, "UPDATE SEPARATE WO", false, ViewState.MainWindow);
      }
    }

    private void ultCBWO_ValueChanged(object sender, EventArgs e)
    {
      ultCBCarcassCode.Value = DBNull.Value;
      ultCBItemCode.Value = DBNull.Value;
      if (ultCBWO.Value != null)
      {
        this.LoadCarcassCode(DBConvert.ParseInt(ultCBWO.Value.ToString()));
        this.LoadItemCode(DBConvert.ParseInt(ultCBWO.Value.ToString()));
      }
      else
      {
        this.LoadCarcassCode(-1);
        this.LoadItemCode(-1);
      }
    }
    #endregion event
  }
}
