/*
  Author      : Do Minh Tam
  Date        : 16/03/2012
  Description : Issue to container list
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
using System.IO;
using DaiCo.Shared.DataSetSource.FinishGoodWarehouse;

namespace DaiCo.FinishGoodWarehouse
{
  public partial class viewFGH_03_004 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewFGH_03_004()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewFGH_03_004_Load(object sender, EventArgs e)
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
      DBParameter[] param = new DBParameter[11];

      // Issuing No
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
      DateTime prDateSet = DateTime.MinValue;
      if (drpDateSet.Value != null)
      {
        prDateSet = (DateTime)drpDateSet.Value;
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

      if (prDateSet != DateTime.MinValue)
      {
        param[5] = new DBParameter("@CreateDateSet", DbType.DateTime, prDateSet);
      }

      //Container
      long value = long.MinValue;
      if (this.ultcbContainer.Value != null)
      {
        value = DBConvert.ParseLong(this.ultcbContainer.Value.ToString());
        if (value != int.MinValue)
        {
          param[6] = new DBParameter("@ContainerPid", DbType.Int64, value);
        }
      }

      if (ultDepartment.Value != null)
      {
        param[7] = new DBParameter("@Department", DbType.String, this.ultDepartment.Value.ToString());
      }

      if (ultStatus.Value != null)
      {
        param[8] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(this.ultStatus.Value.ToString()));
      }

      if (ultCreateBy.Value != null)
      {
        param[9] = new DBParameter("@CreateBy", DbType.Int32, DBConvert.ParseInt(this.ultCreateBy.Value.ToString()));
      }

      if (ultType.Value != null)
      {
        string type = string.Empty;
        //type Rec
        if (DBConvert.ParseInt(this.ultType.Value.ToString()) == 1)
        {
          type = "09ISS";
        }
        else if (DBConvert.ParseInt(this.ultType.Value.ToString()) == 2)
        {
          type = "09ADO";
        }
        else if (DBConvert.ParseInt(this.ultType.Value.ToString()) == 3)
        {
          type = "09STC";
        }
        else if (DBConvert.ParseInt(this.ultType.Value.ToString()) == 3)
        {
          type = "09SPI";
        }
        param[10] = new DBParameter("@Type", DbType.String, type);
      }

      string storeName = "spWHFListOutStoreContainer_Select";

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, param);

      ultData.DataSource = dtSource;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Posting"].Value.ToString()) == 0)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.White;
        }
        else
        {
          string type = ultData.Rows[i].Cells["Issue No"].Value.ToString().Substring(0, 5);
          if (type.CompareTo("09ISS") == 0)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
          }
          else if (type.CompareTo("09ADO") == 0)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.Pink;
          }
          else if (type.CompareTo("09STC") == 0)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
          }
          else if (type.CompareTo("09SPI") == 0)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.LightBlue;
          }

          if (DBConvert.ParseInt(ultData.Rows[i].Cells["Posting"].Value.ToString()) == 0)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.White;
          }
        }
      }
    }

    /// <summary>
    /// Load All Data For Search Information
    /// </summary>
    private void LoadData()
    {
      // Load UltraCombo Container List
      this.LoadComboContainer();

      // Load Status
      this.LoadStatus();

      // Load Department
      this.LoadDepartment();

      // Load CreateBy
      this.LoadCreateBy();

      // Load Type
      this.LoadType();

      // Set Focus
      this.txtNoFrom.Focus();
      this.drpDateFrom.Value = DBNull.Value;
      this.drpDateTo.Value = DBNull.Value;
      this.drpDateSet.Value = DBNull.Value;
 
    }

    // Load Status
    private void LoadStatus()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 0 ID, 'New' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Confirmed' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultStatus.DataSource = dtSource;
      ultStatus.DisplayMember = "Name";
      ultStatus.ValueMember = "ID";
      ultStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultStatus.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultStatus.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
    }

    // Load Type
    private void LoadType()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, '' Name";
      commandText += " UNION ";
      commandText += " SELECT 1 ID, 'ISS-Issuing Note' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'ADO-Ajustment Out' Name";
      commandText += " UNION";
      commandText += " SELECT 3 ID, 'STC-Sale To Customer' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultType.DataSource = dtSource;
      ultType.DisplayMember = "Name";
      ultType.ValueMember = "ID";
      ultType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultType.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultType.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;

      //this.ultType.Value = 3;
    }

    // Load Department
    private void LoadDepartment()
    {
      string commandText = "SELECT Department, Department + ' - ' + DeparmentName AS Name FROM VHRDDepartment";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDepartment.DataSource = dtSource;
      ultDepartment.DisplayMember = "Name";
      ultDepartment.ValueMember = "Department";
      ultDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDepartment.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      ultDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
    }

    // Load CreateBy
    private void LoadCreateBy()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_NhanVien, CAST(ID_NhanVien AS VARCHAR) + ' ' + HoNV + TenNV Name";
      commandText += " FROM VHRNhanVien ";
      commandText += " WHERE Resigned = 0 AND Department = 'WHD' ";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCreateBy.DataSource = dtSource;
      ultCreateBy.DisplayMember = "Name";
      ultCreateBy.ValueMember = "ID_NhanVien";
      ultCreateBy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCreateBy.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      ultCreateBy.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
    }

    private void LoadComboContainer()
    {
      string commandText = "Select ContainerNo, ShipDate, Pid from TblPLNSHPContainer ORDER BY Pid DESC";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultcbContainer.DataSource = dtSource;
      ultcbContainer.DisplayMember = "ContainerNo";
      ultcbContainer.ValueMember = "Pid";
      ultcbContainer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbContainer.DisplayLayout.Bands[0].Columns["ContainerNo"].Width = 350;
      ultcbContainer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
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
      this.drpDateFrom.Value = DBNull.Value;
      this.drpDateTo.Value = DBNull.Value;
      this.drpDateSet.Value = DateTime.Today;

      this.ultcbContainer.Text = string.Empty;


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
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["PID"].Hidden = true;
      e.Layout.Bands[0].Columns["Posting"].Hidden = true;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Open Screen when Double
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
      UltraGridRow row = ultData.Selected.Rows[0];
      long pid = DBConvert.ParseLong(row.Cells["PID"].Value.ToString());
      string type = row.Cells["Issue No"].Value.ToString().Substring(0, 5);

      if (type.CompareTo("09STC") == 0)
      {
        viewFGH_03_010 view = new viewFGH_03_010();
        view.outStorePid = pid;
        DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "ISSUE NOTE - ISSUE FOR CONTAINER", false, DaiCo.Shared.Utility.ViewState.MainWindow);
      }
      else if (type.CompareTo("09ISS") == 0)
      {
        viewFGH_03_005 view = new viewFGH_03_005();
        view.outStorePid = pid;
        DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "ISSUE NOTE - ISSUE FOR PRODUCTION", false, DaiCo.Shared.Utility.ViewState.MainWindow);
      }
      else if (type.CompareTo("09ADO") == 0)
      {
        viewFGH_03_006 view = new viewFGH_03_006();
        view.outStorePid = pid;
        DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "ISSUE NOTE - ADJUSTMENT OUT", false, DaiCo.Shared.Utility.ViewState.MainWindow);
      }
      else if (type.CompareTo("09SPI") == 0)
      {
        viewFGH_03_009 view = new viewFGH_03_009();
        view.outStorePid = pid;
        DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "ISSUE NOTE - ISSUE FOR CONTAINER(SPECIAL)", false, DaiCo.Shared.Utility.ViewState.MainWindow);
      }
      // Search Grid Again 
      this.Search();
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewFGH_03_003 view = new viewFGH_03_003();
      DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "ISSUE NOTE", false, DaiCo.Shared.Utility.ViewState.MainWindow);
    }

    #endregion Event
  }
}
