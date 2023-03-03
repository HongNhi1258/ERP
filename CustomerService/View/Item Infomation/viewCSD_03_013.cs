/*
  Author      : 
  Date        : 06-11-2013
  Description : Search Hold SO
  Standard Form: view_GNR_90_002
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using DaiCo.CustomerService;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_013 : MainUserControl
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
      this.LoadUltraComboCustomer();
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
      commandText += " SELECT 1 ID, 'CS Confirmed' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'PL Confirmed' Name";
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
    /// Load Customer
    /// </summary>
    private void LoadUltraComboCustomer()
    {
      string commandText = string.Empty;
      commandText = " SELECT Pid, CustomerCode, Name FROM TblCSDCustomerInfo";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBCustomer.DataSource = dtSource;
      ultCBCustomer.DisplayMember = "CustomerCode";
      ultCBCustomer.ValueMember = "Pid";
      ultCBCustomer.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultCBCustomer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCBCustomer.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load SO
    /// </summary>
    /// <param name="customerPid"></param>
    private void LoadUltraDropSO(long customerPid)
    {
      string commandText = string.Empty;
      commandText = " SELECT Pid SOPid, SaleNo, CustomerPONo PONo FROM TblPLNSaleOrder WHERE CustomerPid = " + customerPid;

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBSO.DataSource = dtSource;
      ultCBSO.DisplayMember = "SaleNo";
      ultCBSO.ValueMember = "SOPid";
      ultCBSO.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultCBSO.DisplayLayout.Bands[0].Columns["SOPid"].Hidden = true;
      ultCBSO.DisplayLayout.Bands[0].Columns["SaleNo"].MaxWidth = 100;
      ultCBSO.DisplayLayout.Bands[0].Columns["SaleNo"].MinWidth = 100;
      ultCBSO.DisplayLayout.Bands[0].Columns["PONo"].MaxWidth = 200;
      ultCBSO.DisplayLayout.Bands[0].Columns["PONo"].MinWidth = 200;
      ultCBSO.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;

      DBParameter[] param = new DBParameter[9];
      if (txtHoldNo.Text.Trim().Length > 0)
      {
        param[0] = new DBParameter("@HoldNo", DbType.String, txtHoldNo.Text);
      }

      if(ultCBCustomer.Text.Length > 0 && ultCBCustomer.Value != null)
      {
        param[1] = new DBParameter("@CustomerPid", DbType.Int64, DBConvert.ParseLong(ultCBCustomer.Value.ToString()));
      }

      if (ultCBSO.Text.Length > 0 && ultCBSO.Value != null)
      {
        param[2] = new DBParameter("@SOPid", DbType.Int64, DBConvert.ParseLong(ultCBSO.Value.ToString()));
      }

      if (txtItemCode.Text.Trim().Length > 0)
      {
        param[3] = new DBParameter("@ItemCode", DbType.String, txtItemCode.Text);
      }

      if (ultCBCreateBy.Text.Length > 0 && ultCBCreateBy.Value != null)
      {
        param[4] = new DBParameter("@CreateBy", DbType.Int32, DBConvert.ParseInt(ultCBCreateBy.Value.ToString()));
      }

      if (ultCBStatus.Text.Length > 0 && ultCBStatus.Value != null)
      {
        param[5] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ultCBStatus.Value.ToString()));
      }

      // DateFrom
      if (ultDateFrom.Value != null)
      {
        DateTime prDateFrom = DateTime.MinValue;
        prDateFrom = (DateTime)ultDateFrom.Value;
        param[6] = new DBParameter("@DateFrom", DbType.DateTime, prDateFrom);
      }
      // DateTo
      if (ultDateTo.Value != null)
      {
        DateTime prDateTo = DateTime.MinValue;
        prDateTo = (DateTime)ultDateTo.Value;
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        param[7] = new DBParameter("@DateTo", DbType.DateTime, prDateTo);
      }

      // SaleCode
      if (txtSaleCode.Text.Trim().Length > 0)
      {
        param[8] = new DBParameter("@SaleCode", DbType.String, txtSaleCode.Text);
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spCSDSOGetInfoHold_Select", param);
      if (ds != null)
      {
        DataSet dsSource = this.CreateDataSet();
        dsSource.Tables["dtParent"].Merge(ds.Tables[0]);
        dsSource.Tables["dtChild"].Merge(ds.Tables[1]);
        ultraGridInformation.DataSource = dsSource;

        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          UltraGridRow row = ultraGridInformation.Rows[i];
          if(String.Compare(row.Cells["Status"].Value.ToString(), "New", true) == 0)
          {
            row.Appearance.BackColor = Color.White;
          }
          else if (String.Compare(row.Cells["Status"].Value.ToString(), "CS Confirmed", true) == 0)
          {
            row.Appearance.BackColor = Color.Yellow;
          }
          else if (String.Compare(row.Cells["Status"].Value.ToString(), "PLN Confirmed", true) == 0)
          {
            row.Appearance.BackColor = Color.LightGreen;
          }

          if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 0)
          {
            row.Cells["Remark"].Appearance.ForeColor = Color.Blue;
            row.Cells["Remark"].Appearance.FontData.Bold = DefaultableBoolean.True;
          }
          else
          {
            row.Cells["Remark"].Appearance.ForeColor = Color.Red;
            row.Cells["Remark"].Appearance.FontData.Bold = DefaultableBoolean.True;
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
      txtHoldNo.Text = string.Empty;
      txtItemCode.Text = string.Empty;
      ultCBCustomer.Value = DBNull.Value;
      ultCBSO.Value = DBNull.Value;
      ultDateFrom.Value = DBNull.Value;
      ultDateTo.Value = DBNull.Value;
      ultCBCreateBy.Value = DBNull.Value;
      ultCBStatus.Value = DBNull.Value;
      txtHoldNo.Focus();
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
      taParent.Columns.Add("HoldNo", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("CustomerCode", typeof(System.String));
      taParent.Columns.Add("Name", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("SaleNo", typeof(System.String));
      taChild.Columns.Add("PONo", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("SaleCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("HoldQty", typeof(System.Int32));
      taChild.Columns.Add("PLNApproved", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["Pid"], false));
      return ds;
    }
    #endregion function

    #region event
    public viewCSD_03_013()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_03_013_Load(object sender, EventArgs e)
    {
      txtHoldNo.Focus();
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
      ControlUtility.SetPropertiesUltraGrid(ultraGridInformation);

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Tab1"].Hidden = true;
      e.Layout.Bands[0].Columns["Tab2"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;
      e.Layout.Bands[1].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["HoldQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["PLNApproved"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;


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
      UltraGridRow row = ultraGridInformation.Selected.Rows[0];
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());

      viewCSD_03_012 uc = new viewCSD_03_012();
      uc.holdPid = pid;
      WindowUtinity.ShowView(uc, "UPDATE HOLD INFO", false, ViewState.MainWindow);
    }

    private void ultCBCustomer_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBCustomer.Value != null)
      {
        this.LoadUltraDropSO(DBConvert.ParseLong(ultCBCustomer.Value.ToString()));
      }
      else
      {
        this.LoadUltraDropSO(-1);
      }
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewCSD_03_012 view = new viewCSD_03_012();
      WindowUtinity.ShowView(view, "UPDATE HOLD INFO", true, DaiCo.Shared.Utility.ViewState.MainWindow);
    }
    #endregion event
  }
}
