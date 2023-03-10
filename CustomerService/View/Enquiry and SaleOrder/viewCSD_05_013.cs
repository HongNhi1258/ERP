/*
 * Created By   : Đặng 
 * Created Date : 17/10/2014
 * Description  : Upgrade Revision
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using DaiCo.Application;
using DaiCo.Objects;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared;
using Infragistics.Win;
using VBReport;
using DaiCo.Shared.DataBaseUtility;
using System.Diagnostics;
using DaiCo.Shared.Utility;
using DaiCo.Shared.DataSetSource.CustomerService;
using System.IO;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_05_013 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    private string itemCode = string.Empty;
    private int revision = int.MinValue;
    #endregion Field

    #region Init
    public viewCSD_05_013()
    {
      InitializeComponent();
    }

    private void viewCSD_05_013_Load(object sender, EventArgs e)
    {
      // Load Data
      this.LoadData();
      // Load Revision Need Upgrade
      this.LoadRevisionUpgrade();
    }

    /// <summary>
    /// Load Revision Need Upgrade
    /// </summary>
    private void LoadRevisionUpgrade()
    {
      string commandText = string.Format(@"SELECT Revision FROM TblBOMItemInfo WHERE ItemCode = '{0}' AND Revision <> {1}", this.itemCode, this.revision);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        ultCBUpgradeRevision.DataSource = dtSource;
        ultCBUpgradeRevision.DisplayMember = "Revision";
        ultCBUpgradeRevision.ValueMember = "Revision";
        ultCBUpgradeRevision.DisplayLayout.Bands[0].ColHeadersVisible = false;
      }
    }
    #endregion Init

    #region Event
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      // 1: Check Valid
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // 2: Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }

      // 3:Close Tab
      this.CloseTab();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["SaleOrderPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].Hidden = true;

      e.Layout.Bands[0].ColHeaderLines = 3;
      e.Layout.Bands[0].Columns["Cancel"].Header.Caption = "Cancel SO";
      e.Layout.Bands[0].Columns["ChangeConfirmShipdate"].Header.Caption = "Changed\nShipdate";
      e.Layout.Bands[0].Columns["ChangeDeadline"].Header.Caption = "WorkOrder\nDeadline";
      e.Layout.Bands[0].Columns["WorkConfirmed"].Header.Caption = "Confirmed WO";

      e.Layout.Bands[0].Columns["Cancel"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ChangeConfirmShipdate"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Hold"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Shipped"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ChangeDeadline"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["WorkConfirmed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Cancel"].Width = 150;
      e.Layout.Bands[0].Columns["ChangeConfirmShipdate"].Width = 150;
      e.Layout.Bands[0].Columns["Hold"].Width = 150;
      e.Layout.Bands[0].Columns["Shipped"].Width = 150;
      e.Layout.Bands[0].Columns["ChangeDeadline"].Width = 150;
      e.Layout.Bands[0].Columns["WorkConfirmed"].Width = 150;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    #endregion Event

    #region Function
    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, this.pid);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNSaleOrderUpgradeRevision_Select", input);
      if(dsSource != null)
      {
        // Master
        string saleOrder = dsSource.Tables[0].Rows[0]["SaleNo"].ToString();
        this.itemCode = dsSource.Tables[0].Rows[0]["ItemCode"].ToString();
        this.revision = DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["Revision"].ToString());
        txtSaleOrder.Text = saleOrder;
        txtItemCode.Text=  this.itemCode;
        txtRevision.Text = this.revision.ToString();

        // Detail
        ultData.DataSource = dsSource.Tables[1];

        // Set Status Control
        this.SetStatusControl();
      }
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      bool success = true;
      for (int i = 3; i < ultData.Rows[0].Cells.Count; i++)
      {
        if(DBConvert.ParseInt(ultData.Rows[0].Cells[i].Value) == 1)
        {
          ultData.Rows[0].Cells[i].Appearance.BackColor = Color.Yellow;
          success = false;
        }
      }
      if(success == false)
      {
        btnSave.Enabled = false;
      }
    }

    /// <summary>
    /// Check Data
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      // Check Revision Upgrade
      if(ultCBUpgradeRevision.Value == null ||
          DBConvert.ParseInt(ultCBUpgradeRevision.Text) == int.MinValue ||
          DBConvert.ParseInt(ultCBUpgradeRevision.Text) == DBConvert.ParseInt(txtRevision.Text))
      {
        message = "Revision Upgrade";
        return false;
      }
      // End

      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, this.pid);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNSaleOrderUpgradeRevision_Select", input);
      int value = 0;
      for (int col = 3; col < dsSource.Tables[1].Columns.Count; col++)
      {
        value = DBConvert.ParseInt(dsSource.Tables[1].Rows[0][col]);
        if(value == 1)
        {
          message = "Upgrade Revision Affect";
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, this.pid);
      input[1] = new DBParameter("@UpgradeRevision", DbType.Int32, DBConvert.ParseInt(ultCBUpgradeRevision.Value));
      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure("spPLNSaleOrderUpgradeRevision_Update", input, output);
      long result = DBConvert.ParseLong(output[0].Value);
      if(result <= 0)
      {
        return false;
      }
      return true;
    }
    #endregion Function
  }
}
