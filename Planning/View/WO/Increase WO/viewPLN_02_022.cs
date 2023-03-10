/*
  Author      : Do Tam
  Date        : 21/01/2013
  Description : Change Note Confirm ShipDate List
 */
using DaiCo.Application;
using DaiCo.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
namespace DaiCo.Planning
{
  public partial class viewPLN_02_022 : MainUserControl
  {
    #region Init
    public viewPLN_02_022()
    {
      InitializeComponent();
    }
    #endregion Init

    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

    #endregion Field

    #region Load Data

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      // Customer
      // Type
      Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbType, Shared.Utility.ConstantClass.GROUP_SALEORDERTYPE);
      // Create By
      Shared.Utility.ControlUtility.LoadEmployee(cmbCreateBy, "PLA");
    }

    #endregion Load Data

    #region Search

    /// <summary>
    /// Edit layout
    /// </summary>
    private void Editlayout()
    {
      ultData.DisplayLayout.Bands["dtParent_dtChild"].Columns["ParentPid"].Hidden = true;
      ultData.DisplayLayout.Bands["dtParent"].Columns["Pid"].Hidden = true;
      ultData.DisplayLayout.Bands["dtParent"].Columns["No"].Header.Caption = "Sale No";
      ultData.DisplayLayout.Bands["dtParent"].Columns["CusNo"].Header.Caption = "Customer Po No";
      ultData.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;
      ultData.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[11];

      if (txtWoFrom.Text.Trim().Length > 0)
      {
        long WoFrom = DBConvert.ParseLong(txtWoFrom.Text.Trim());
        if (WoFrom != long.MinValue)
        {
          param[0] = new DBParameter("@WoFrom", DbType.Int64, WoFrom);
        }
      }
      if (txtWoTo.Text.Trim().Length > 0)
      {
        long WoTo = DBConvert.ParseLong(txtWoTo.Text.Trim());
        if (WoTo != long.MinValue)
        {
          param[1] = new DBParameter("@WoTo", DbType.Int64, WoTo);
        }
      }
      if (txtWo.Text.Trim().Length > 0)
      {
        long Wo = DBConvert.ParseLong(txtWo.Text.Trim());
        if (Wo != long.MinValue)
        {
          param[2] = new DBParameter("@WoPid", DbType.Int64, Wo);
        }
      }
      //Create Date
      if (dt_POFrom.Value != null)
      {
        DateTime orderDate = DBConvert.ParseDateTime(dt_POFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        param[3] = new DBParameter("@CreateFrom", DbType.DateTime, orderDate);
      }
      if (dt_POTo.Value != null)
      {
        DateTime orderDate = DBConvert.ParseDateTime(dt_POTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        param[4] = new DBParameter("@CreateTo", DbType.DateTime, orderDate);
      }
      // Type
      int value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbType));
      if (value != int.MinValue)
      {
        param[5] = new DBParameter("@Type", DbType.Int32, value);
      }
      // Status
      try
      {
        value = cmbStatus.SelectedIndex;
        value = (value == -1 || value == 0) ? -1 : value - 1;
      }
      catch
      {
        value = int.MinValue;
      }
      if (value >= 0)
      {
        param[6] = new DBParameter("@Status", DbType.Int32, value);
      }
      // ItemCode
      string text = txtItemCode.Text.Trim();
      if (text.Length > 0)
      {
        param[7] = new DBParameter("@ItemCode", DbType.AnsiString, 18, "%" + text.Replace("'", "''") + "%");
      }
      // Create By
      value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCreateBy));
      if (value != int.MinValue)
      {
        param[8] = new DBParameter("@CreateBy", DbType.Int32, value);
      }
      // SaleCode
      text = txtSaleCode.Text.Trim();
      if (text.Length > 0)
      {
        param[9] = new DBParameter("@SaleCode", DbType.AnsiString, 18, "%" + text.Replace("'", "''") + "%");
      }
      // OldCode
      text = txtOldCode.Text.Trim();
      if (text.Length > 0)
      {
        param[10] = new DBParameter("@OldCode", DbType.AnsiString, 18, "%" + text.Replace("'", "''") + "%");
      }
      string storeName = "spPLNChangeWoQuantityList_Select";
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
      dsSource.Relations.Add(new DataRelation("TblParent_TblChild", new DataColumn[] { dsSource.Tables[0].Columns["Pid"] },
                                                new DataColumn[] { dsSource.Tables[1].Columns["Pid"] }, false));



      ultData.DataSource = dsSource;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string status = ultData.Rows[i].Cells["ST"].Value.ToString().Trim();
        if (status == "1")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        else if (status == "2")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Pink;
        }
      }
    }

    #endregion Search

    #region Event

    /// <summary>
    /// Load Date
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_01_003_Load(object sender, EventArgs e)
    {
      dt_POFrom.Value = DateTime.Today.AddDays(-3);
      dt_POTo.Value = DateTime.Today.AddDays(3);
      this.LoadData();
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
    /// Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Enabled = false;
      // Search Data
      this.Search();
      this.Enabled = true;
    }

    /// <summary>
    /// Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.AutoFitColumns = true;

      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        e.Layout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      DataTable dtNew = ((DataSet)ultData.DataSource).Tables[1];
      for (int i = 0; i < dtNew.Columns.Count; i++)
      {
        if (dtNew.Columns[i].DataType == typeof(Int32) || dtNew.Columns[i].DataType == typeof(float) || dtNew.Columns[i].DataType == typeof(Double))
        {
          ultData.DisplayLayout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (dtNew.Columns[i].DataType == typeof(DateTime))
        {
          ultData.DisplayLayout.Bands[1].Columns[i].Format = "dd-MMM-yyyy";
        }
      }
      dtNew = ((DataSet)ultData.DataSource).Tables[0];
      for (int i = 0; i < dtNew.Columns.Count; i++)
      {
        if (dtNew.Columns[i].DataType == typeof(Int32) || dtNew.Columns[i].DataType == typeof(float) || dtNew.Columns[i].DataType == typeof(Double))
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (dtNew.Columns[i].DataType == typeof(DateTime))
        {
          ultData.DisplayLayout.Bands[0].Columns[i].Format = "dd-MMM-yyyy";
        }
      }
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["STT"].Header.Caption = "Status";
      e.Layout.Bands[0].Columns["TransactionCode"].Header.Caption = "Transaction Code";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Creater";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[1].Columns["WorkOrderPid"].Header.Caption = "Work Order";
      e.Layout.Bands[1].Columns["NewQty"].Header.Caption = "New Qty";
      e.Layout.Bands[1].Columns["OldQty"].Header.Caption = "Old Qty";

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["ErrorPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ST"].Hidden = true;

      e.Layout.Bands[1].Columns["TransactionDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["FlagAccept"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["FlagAccept"].Header.Caption = "Accepted";

      e.Layout.Bands[1].Columns["FlagAccept"].CellAppearance.BackColor = Color.Yellow;

      // Set header height
      e.Layout.Bands[1].ColHeaderLines = 2;

      e.Layout.Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Double click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      //open SO form
      try
      {
        UltraGridRow row = ultData.Selected.Rows[0];
        viewPLN_02_020 uc = new viewPLN_02_020();
        long pid = long.MinValue;
        try
        {
          pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        }
        catch
        {
        }
        if (pid > 0)
        {
          uc.transactionPid = pid;
          Shared.Utility.WindowUtinity.ShowView(uc, "WORK ORDER QUANTITY ADJUSTMENT", false, Shared.Utility.ViewState.MainWindow);
        }
      }
      catch
      { }
    }

    /// <summary>
    /// Clear
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      foreach (Control o in tableLayoutPanel1.Controls)
      {
        if (o.GetType() != typeof(Label))
        {
          o.Text = "";

        }
      }
      txtSaleCode.Text = "";
      dt_POFrom.Value = null;
      dt_POTo.Value = null;

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
    private void btnNew_Click(object sender, EventArgs e)
    {
      try
      {
        viewPLN_02_020 uc = new viewPLN_02_020();
        Shared.Utility.WindowUtinity.ShowView(uc, "ADJUSTMENT WORKORDER QUANTITY", false, Shared.Utility.ViewState.MainWindow);
      }
      catch
      { }
    }
    #endregion Event



  }
}
