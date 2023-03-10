/*
  Author      : Xuan Truong
  Date        : 16/11/2011
  Description : List Transfer Location Box
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Shared.DataSetSource.FinishGoodWarehouse;
using System.IO;
using DaiCo.FinishGoodWarehouse;

namespace DaiCo.FinishGoodWarehouse
{
  public partial class viewFGH_01_002 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewFGH_01_002()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewVEN_02_001_Load(object sender, EventArgs e)
    {
      // Load All Data For Search Information
      this.LoadData();

      // Set Focus
      this.txtNoFrom.Focus();
    }
    #endregion Init

    #region LoadData
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[7];

      // Tran No
      string text = txtNoFrom.Text.Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[0] = new DBParameter("@NoFrom", DbType.AnsiString, 24, text);
      }

      text = txtNoTo.Text.Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[1] = new DBParameter("@NoTo", DbType.AnsiString, 24, text);
      }

      text = txtNoSet.Text.Trim();
      string[] listNo = text.Split(',');
      text = string.Empty;
      foreach (string no in listNo)
      {
        if (no.Trim().Length > 0)
        {
          text += string.Format(",'{0}'", no.Replace("'", "").Trim());
        }
      }
      if (text.Length > 0)
      {
        text = string.Format("{0}", text.Remove(0, 1));
        param[2] = new DBParameter("@NoSet", DbType.AnsiString, 1024, text);
      }

      //Create Date
      DateTime prDateFrom = DateTime.MinValue;
      if (drpDateFrom.Value != null)
      {
        prDateFrom = (DateTime)drpDateFrom.Value;
      }
      DateTime prDateTo = DateTime.MinValue;
      if (drpDateTo.Value != null)
      {
        prDateTo = (DateTime)drpDateTo.Value;
      }

      if (prDateFrom != DateTime.MinValue)
      {
        param[3] = new DBParameter("@CreateDateFrom", DbType.DateTime, prDateFrom);
      }

      if (prDateTo != DateTime.MinValue)
      {
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        param[4] = new DBParameter("@CreateDateTo", DbType.DateTime, prDateTo);
      }

      // Create By
      int value = int.MinValue;
      if (this.ultCreateBy.Value != null)
      {
        value = DBConvert.ParseInt(this.ultCreateBy.Value.ToString());
        if (value != int.MinValue)
        {
          param[5] = new DBParameter("@CreateBy", DbType.Int32, value);
        }
      }

      // Box
      text = txtBox.Text.Trim();
     
      if (text.Length > 0)
      {
        param[6] = new DBParameter("@BoxNo", DbType.AnsiString, 256, text.Replace("'", "''"));
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHFListTransferLocation_Select", param);
      dsFGHListTransferLocation dslist = new dsFGHListTransferLocation();
      if (ds != null)
      {
        dslist.Tables["TransferLocation"].Merge(ds.Tables[0]);
      }
      ultData.DataSource = dslist;
    }
    /// <summary>
    /// Load All Data For Search Information
    /// </summary>
    private void LoadData()
    {
      // Load UltraCombo Create By
      this.LoadComboCreateBy();

      // Set Focus
      this.txtNoFrom.Focus();
    }
    /// <summary>
    /// Load UltraCombo Create By
    /// </summary>
    private void LoadComboCreateBy()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_NhanVien, CONVERT(VARCHAR, ID_NhanVien) + ' - ' + HoNV + ' ' + TenNV Name";
      commandText += " FROM VHRNhanVien ";
      commandText += " WHERE Department = 'WHD' AND Resigned = 0";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCreateBy.DataSource = dtSource;
      ultCreateBy.DisplayMember = "Name";
      ultCreateBy.ValueMember = "ID_NhanVien";
      ultCreateBy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCreateBy.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultCreateBy.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
    }
    #endregion LoadData

    #region Event
    /// <summary>
    /// Clear Information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      //Format
      this.txtNoFrom.Text = string.Empty;
      this.txtNoTo.Text = string.Empty;
      this.txtNoSet.Text = string.Empty;
      this.drpDateFrom.Value = DateTime.Today;
      this.drpDateTo.Value = DateTime.Today;
      this.ultCreateBy.Text = string.Empty;
      this.txtBox.Text = string.Empty;

      // Load All Data For Search Information
      this.LoadData();
    }
    /// <summary>
    /// Search Information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      // Search Information
      this.Search();
    }
    /// <summary>
    /// Format Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);

      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["PID"].Hidden = true;
      e.Layout.Bands[0].Columns["TranNo"].Header.Caption = "Tran No";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Open Sceen when Double
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

      viewFGH_01_001 uc = new viewFGH_01_001();
      uc.locationPid = pid;
      WindowUtinity.ShowView(uc, "UPDATE TRAN LOCATION NOTE", false, ViewState.MainWindow);

      // Search Grid Again 
      this.Search();
    }
    /// <summary>
    /// Keydown
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
      return;
    }
    /// <summary>
    /// New Transfer Location
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewFGH_01_001 view = new viewFGH_01_001();
      DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "UPDATE TRAN LOCATION NOTE", false, DaiCo.Shared.Utility.ViewState.MainWindow);
    }
    #endregion Event
  }
}
