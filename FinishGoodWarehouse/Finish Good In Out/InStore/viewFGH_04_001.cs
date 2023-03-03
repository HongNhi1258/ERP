/*
  Author      : Duong Minh
  Description : List of Receving Note
  Date        : 18/04/2012
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared;
using Infragistics.Win;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;
using DaiCo.Shared.DataSetSource.General;

namespace DaiCo.FinishGoodWarehouse
{
  public partial class viewFGH_04_001 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init

    public viewFGH_04_001()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load ViewGNR_02_001
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_02_001_Load(object sender, EventArgs e)
    {
      this.ultDateFrom.Value = DBNull.Value;
      this.ultDateTo.Value = DBNull.Value;
      // Load Status
      this.LoadStatus();

      // Load Department
      this.LoadDepartment();

      // Load CreateBy
      this.LoadCreateBy();

      // Load Customer
      this.LoadCustomer();

      // Load Type
      this.LoadType();
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
      commandText += " SELECT 1 ID, 'RTW-Return To Warehouse' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'ADI-Ajustment In' Name";
      commandText += " UNION";
      commandText += " SELECT 3 ID, 'RFC-Return From Customer' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultType.DataSource = dtSource;
      ultType.DisplayMember = "Name";
      ultType.ValueMember = "ID";
      ultType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultType.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultType.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;

      this.ultType.Value = 1;
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

    // Load Customer
    private void LoadCustomer()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, CustomerCode + ' - ' + Name Name";
      commandText += " FROM TblCSDCustomerInfo ";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCustomer.DataSource = dtSource;
      ultCustomer.DisplayMember = "Name";
      ultCustomer.ValueMember = "Pid";
      ultCustomer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCustomer.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      ultCustomer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      string department = string.Empty;
      long wo = long.MinValue;
      string boxId = string.Empty;
      int status = int.MinValue;
      string noFrom = txtReceivingNoteFrom.Text.Trim().Replace("'", "''");
      string noTo = txtReceivingNoteTo.Text.Trim().Replace("'", "''");
      string noSet = txtReceivingNoteSet.Text.Trim();
      int createBy = int.MinValue;
      long customer = long.MinValue;
      string type = string.Empty;

      string[] listNo = noSet.Split(',');
      foreach (string no in listNo)
      {
        if (no.Trim().Length > 0)
        {
          noSet += string.Format(",'{0}'", no.Replace("'", "").Trim());
        }
      }

      if (ultDepartment.Value != null)
      {
        department = this.ultDepartment.Value.ToString();
      }
      
      if (ultStatus.Value != null)
      {
        status = DBConvert.ParseInt(this.ultStatus.Value.ToString());
      }

      if (ultCustomer.Value != null)
      {
        customer = DBConvert.ParseInt(this.ultCustomer.Value.ToString());
      }

      if (ultCreateBy.Value != null)
      {
        createBy = DBConvert.ParseInt(this.ultCreateBy.Value.ToString());
      }

      if (ultType.Value != null)
      {
        //type Rec
        if (DBConvert.ParseInt(this.ultType.Value.ToString()) == 1)
        {
          type = "09RTW";
        }
        else if (DBConvert.ParseInt(this.ultType.Value.ToString()) == 2)
        {
          type = "09ADI";
        }
        else if (DBConvert.ParseInt(this.ultType.Value.ToString()) == 3)
        {
          type = "09RFC";
        }
      }

      DateTime createDateFrom = DateTime.MinValue;
      if (ultDateFrom.Value != null)
      {
        createDateFrom = (DateTime)ultDateFrom.Value;
      }

      DateTime createDateTo = DateTime.MinValue;
      if (ultDateTo.Value != null)
      {
        createDateTo = (DateTime)ultDateTo.Value;
      }

      if (this.txtWO.Text.Trim().Length > 0)
      {
        wo = DBConvert.ParseLong(this.txtWO.Text.Trim());
      }

      if (this.txtBoxId.Text.Trim().Length > 0)
      {
        boxId = this.txtBoxId.Text.Trim();
      }

      DBParameter[] intputParam = new DBParameter[12];
      if (noFrom.Length > 0)
      {
        intputParam[0] = new DBParameter("@ID_PhieuNhapFrom", DbType.AnsiString, 16, noFrom);
      }

      if (noTo.Length > 0)
      {
        intputParam[1] = new DBParameter("@ID_PhieuNhapTo", DbType.AnsiString, 16, noTo);
      }

      if (noSet.Length > 0)
      {
        noSet = string.Format("{0}", noSet.Remove(0, 1));
        intputParam[2] = new DBParameter("@ID_PhieuNhap", DbType.AnsiString, 1024, noSet);
      }

      if (ultDateFrom.Value != null)
      {
        intputParam[3] = new DBParameter("@NgayNhapFrom", DbType.DateTime, createDateFrom);
      }

      if (ultDateTo.Value != null)
      {
        createDateTo = createDateTo != (DateTime.MaxValue) ? createDateTo.AddDays(1) : createDateTo;
        intputParam[4] = new DBParameter("@NgayNhapTo", DbType.DateTime, createDateTo);
      }

      if (createBy != int.MinValue)
      {
        intputParam[5] = new DBParameter("@UserWh", DbType.Int32, createBy);
      }

      if (department != string.Empty)
      {
        intputParam[6] = new DBParameter("@Department", DbType.AnsiString, 24, department);
      }

      if (wo != long.MinValue)
      {
        intputParam[7] = new DBParameter("@WorkOrder", DbType.Int32, wo);
      }

      if (boxId.Length > 0)
      {
        intputParam[8] = new DBParameter("@SeriBox", DbType.String, boxId);
      }

      if (customer != long.MinValue)
      {
        intputParam[9] = new DBParameter("@Customer", DbType.Int64, customer);
      }

      if (type.Length > 0)
      {
        intputParam[10] = new DBParameter("@TypeRec", DbType.String, type);
      }

      if (status != int.MinValue)
      {
        intputParam[11] = new DBParameter("@Status", DbType.Int32, status);
      }

      string storeName = "SpWHFListReceivingNoteNew";

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, intputParam);

      DataSet dsData = this.ListReceivingNote();
      dsData.Tables["TblReceivingNote"].Merge(dsSource.Tables[0]);
      dsData.Tables["TblReceivingDetailNote"].Merge(dsSource.Tables[1]);

      ultDetail.DataSource = dsData;

      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultDetail.Rows[i].Cells["Posting"].Value.ToString()) == 0)
        {
          ultDetail.Rows[i].CellAppearance.BackColor = Color.White;
        }
        else
        {
          type = ultDetail.Rows[i].Cells["InStoreCode"].Value.ToString().Substring(0, 5);
          if (type.CompareTo("09RTW") == 0)
          {
            ultDetail.Rows[i].CellAppearance.BackColor = Color.LightGreen;
          }
          else if (type.CompareTo("09ADI") == 0)
          {
            ultDetail.Rows[i].CellAppearance.BackColor = Color.Pink;
          }
          else if (type.CompareTo("09RFC") == 0)
          {
            ultDetail.Rows[i].CellAppearance.BackColor = Color.Yellow;
          }

          if (DBConvert.ParseInt(ultDetail.Rows[i].Cells["Posting"].Value.ToString()) == 0)
          {
            ultDetail.Rows[i].CellAppearance.BackColor = Color.White;
          }
        }
      }
    }

    /// <summary>
    /// List Receiving Veneer
    /// </summary>
    /// <returns></returns>
    private DataSet ListReceivingNote()
    {
      DataSet ds = new DataSet();

      // Parent
      DataTable taParent = new DataTable("TblReceivingNote");
      taParent.Columns.Add("PID", typeof(System.Int64));
      taParent.Columns.Add("InStoreCode", typeof(System.String));
      taParent.Columns.Add("Department", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("Customer", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      ds.Tables.Add(taParent);

      // Child
      DataTable taChild = new DataTable("TblReceivingDetailNote");
      taChild.Columns.Add("PID", typeof(System.Int64));
      taChild.Columns.Add("SeriBoxNo", typeof(System.String));
      taChild.Columns.Add("BoxTypeCode", typeof(System.String));
      taChild.Columns.Add("BoxTypeName", typeof(System.String));
      taChild.Columns.Add("WorkOrder", typeof(System.Int64));
      taChild.Columns.Add("Length", typeof(System.Double));
      taChild.Columns.Add("Width", typeof(System.Double));
      taChild.Columns.Add("Height", typeof(System.Double));
      taChild.Columns.Add("Weight", typeof(System.Double));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("TblReceivingNote_TblReceivingDetailNote", taParent.Columns["PID"], taChild.Columns["PID"], false));
      return ds;
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Search Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
      chkExpandAll.Checked = false;
    }
    /// <summary>
    /// Clear Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      this.txtReceivingNoteTo.Text = string.Empty;
      this.txtReceivingNoteFrom.Text = string.Empty;
      this.txtReceivingNoteSet.Text = string.Empty;
      this.ultDateFrom.Value = DBNull.Value;
      this.ultDateTo.Value = DBNull.Value;
      this.ultDepartment.Text = string.Empty;
      this.ultStatus.Text = string.Empty;
      this.ultType.Text = string.Empty;
      this.txtWO.Text = string.Empty;
      this.txtBoxId.Text = string.Empty;
      this.ultCustomer.Text = string.Empty;
      this.ultStatus.Text = string.Empty;
      this.ultCreateBy.Text = string.Empty;
    }

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultDetail);

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        e.Layout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["PID"].Hidden = true;
      e.Layout.Bands[0].Columns["Posting"].Hidden = true;
      e.Layout.Bands[1].Columns["PID"].Hidden = true;
      
      e.Layout.Bands[1].Columns["WorkOrder"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Height"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Weight"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Expand All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultDetail.Rows.ExpandAll(true);
      }
      else
      {
        ultDetail.Rows.CollapseAll(true);
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

    private void ultDetail_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultDetail.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (ultDetail.Selected.Rows[0].ParentRow == null) ? ultDetail.Selected.Rows[0] : ultDetail.Selected.Rows[0].ParentRow;

      long pid = DBConvert.ParseLong(row.Cells["PID"].Value.ToString());
      string type = row.Cells["InStoreCode"].Value.ToString().Substring(0, 5);

      if (type.CompareTo("09RTW") == 0)
      {
        // Return From Production
        viewFGH_04_002 uc = new viewFGH_04_002();
        uc.inStorePid = pid;
        WindowUtinity.ShowView(uc, "UPDATE FG RECEIVING NOTE - RETURN FROM PRODUCTION", false, ViewState.MainWindow);
      }
      else if (type.CompareTo("09ADI") == 0)
      {
        // Adjustment In
        viewFGH_04_003 uc = new viewFGH_04_003();
        uc.inStorePid = pid;
        WindowUtinity.ShowView(uc, "UPDATE FG RECEIVING NOTE - ADJUSTMENT IN", false, ViewState.MainWindow);
      }
      else if (type.CompareTo("09RFC") == 0)
      {
        // Return From Customer
        viewFGH_04_006 uc = new viewFGH_04_006();
        uc.inStorePid = pid;
        WindowUtinity.ShowView(uc, "UPDATE FG RECEIVING NOTE - RETURN FROM CUSTOMER", false, ViewState.MainWindow);
      }

      // Search Grid Again 
      this.Search();
    }
    #endregion Event
  }
}
