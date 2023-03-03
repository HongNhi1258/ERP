/*
  Author      : 
  Description : Veneer Requisition Info
  Date        : 11/06/2013
  Kind        : 1 (Veneer Allocation for WO & Carcass)
              : 2 (Veneer Allocation for Department)
              : 3 (Veneer Allocation for Supplement)
              : 4 (Veneer No Allocation)
              : 5 (Veneer Allocate Special)
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using System.Collections;
using DaiCo.Shared.UserControls;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.Utility;
using VBReport;
using System.Diagnostics;
using Infragistics.Win;

namespace DaiCo.General
{
  public partial class viewGNR_06_002 : MainUserControl
  {
    #region Field
    public long veneerRequisitionPid = long.MinValue;
    private int status = int.MinValue;
    private DateTime deliveryDate = DateTime.MinValue;
    private int deliveryHour = int.MinValue;
    private int deliveryMinute = int.MinValue;
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init

    public viewGNR_06_002()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load viewGNR_02_002
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_06_002_Load(object sender, EventArgs e)
    {
      // Load Init
      this.LoadInit();
      // Load Data
      this.LoadData();
      // Set Status Control
      this.SetStatusControl();

    }

    /// <summary>
    /// Load Init
    /// </summary>
    private void LoadInit()
    {
      // Load Department User Login
      this.LoadDepartmentUserLogin();
      // Load Urgent Level
      this.LoadUrgentLevel();
      // Load Day 
      this.LoadDiliveryDate();
      // Load Hour
      this.LoadHour();
      // Load Minute
      this.LoadMinute();
    }

    /// <summary>
    /// Load UrgentLevel
    /// </summary>
    private void LoadUrgentLevel()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_URGENTLEVEL;
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        this.ultcbUrgent.Value = dtSource.Rows[2][0].ToString();
      }
      ultcbUrgent.DataSource = dtSource;
      ultcbUrgent.DisplayMember = "Value";
      ultcbUrgent.ValueMember = "Code";
      ultcbUrgent.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbUrgent.DisplayLayout.Bands[0].Columns["Value"].Width = 120;
      ultcbUrgent.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Set Default Department
    /// </summary>
    private void LoadDepartmentUserLogin()
    {
      string commandText = "SELECT Department FROM VHRNhanVien WHERE ID_NhanVien = " + SharedObject.UserInfo.UserPid;
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        this.ultcbDepartmentRequest.Value = dtSource.Rows[0][0].ToString();
      }
    }

    /// <summary>
    /// Load Hour
    /// </summary>
    private void LoadHour()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Hour", typeof(Int32));
      for (int i = 0; i < 24; i++)
      {
        DataRow dr = dt.NewRow();
        dr["Hour"] = i;
        dt.Rows.Add(dr);
      }
      ultcbHour.DataSource = dt;
      ultcbHour.DisplayMember = "Hour";
      ultcbHour.ValueMember = "Hour";
      ultcbHour.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Minute
    /// </summary>
    private void LoadMinute()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Minute", typeof(Int32));
      for (int i = 0; i < 60; i++)
      {
        DataRow dr = dt.NewRow();
        dr["Minute"] = i;
        dt.Rows.Add(dr);
      }
      ultcbMinute.DataSource = dt;
      ultcbMinute.DisplayMember = "Minute";
      ultcbMinute.ValueMember = "Minute";
      ultcbMinute.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    ///  Load Delivery Date
    /// </summary>
    private void LoadDiliveryDate()
    {
      string commandText = "SELECT CONVERT(varchar, GETDATE(), 103) [Day], DATEPART(HH, GETDATE()) [Hour], DATEPART(MI, GETDATE()) [Minute]";
      DataTable dtHour = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DateTime day = DBConvert.ParseDateTime(dtHour.Rows[0]["Day"].ToString(), Shared.Utility.ConstantClass.FORMAT_DATETIME);
      ultDeliveryDate.Value = day;
    }

    /// <summary>
    /// Set Delivery Time
    /// </summary>
    private void SetDeliveryTime()
    {
      try
      {
        string commandText = "SELECT CONVERT(varchar, GETDATE(), 103) [Day], DATEPART(HH, GETDATE()) [Hour], DATEPART(MI, GETDATE()) [Minute]";
        DataTable dtHour = DataBaseAccess.SearchCommandTextDataTable(commandText);
        DateTime day = DBConvert.ParseDateTime(dtHour.Rows[0]["Day"].ToString(), Shared.Utility.ConstantClass.FORMAT_DATETIME);
        int hour = DBConvert.ParseInt(dtHour.Rows[0]["Hour"].ToString());
        int minute = DBConvert.ParseInt(dtHour.Rows[0]["Minute"].ToString());
        int temp = int.MinValue;
        if (ultcbUrgent.Value != null)
        {
          if (chkDeliveryTime.Checked == false)
          {
            if (hour + 4 > 24)
            {
              deliveryDate = day.AddDays(1);
              temp = 24 - hour;
              deliveryHour = 4 - temp;
              deliveryMinute = minute;
            }
            else
            {
              deliveryDate = day;
              deliveryHour = hour + 4;
              deliveryMinute = minute;
            }
          }
          else
          {
            deliveryDate = DBConvert.ParseDateTime(ultDeliveryDate.Value.ToString(), Shared.Utility.ConstantClass.FORMAT_DATETIME);
            deliveryHour = DBConvert.ParseInt(ultcbHour.Value.ToString());
            if (ultcbMinute.Value != null)
            {
              deliveryMinute = DBConvert.ParseInt(ultcbMinute.Value.ToString());
            }
            else
            {
              deliveryMinute = 0;
            }
          }
        }
        else
        {
          DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Urgent");
          return;
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      string storeName = "spGNRVeneerRequisitionByMRNPid_Select";
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@Pid", DbType.Int64, this.veneerRequisitionPid);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, input);

      // 1: Info
      DataTable dtMaterialRequisition = dsSource.Tables[0];
      if (dtMaterialRequisition != null && dtMaterialRequisition.Rows.Count > 0)
      {
        DataRow row = dtMaterialRequisition.Rows[0];
        txtMRNCode.Text = row["Code"].ToString();
        txtRequestBy.Text = row["CreateBy"].ToString();
        ultcbDepartmentRequest.Value = row["DepartmentRequest"].ToString();
        ultcbUrgent.Value = row["Urgent"].ToString();
        txtRemark.Text = row["remark"].ToString();
        status = DBConvert.ParseInt(row["Status"].ToString());
        ultcbHour.Value = row["DeliveryHour"].ToString();
        ultcbMinute.Value = row["DeliveryMinute"].ToString();
        ultDeliveryDate.Value = DBConvert.ParseDateTime(row["DeliveryDate"].ToString(), Shared.Utility.ConstantClass.FORMAT_DATETIME);
        if (DBConvert.ParseInt(row["IsProduction"].ToString()) == 1)
        {
          radProduction.Checked = true;
        }
        else
        {
          radOrderDepartment.Checked = true;
        }
      }
      else
      {
        // Get Code
        DataTable dtMaterialRequisitionNote = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FGNRGetNewVeneerRequisitionNo('RNV') NewWoodsRequisitionNo");
        if ((dtMaterialRequisitionNote != null) && (dtMaterialRequisitionNote.Rows.Count > 0))
        {
          txtMRNCode.Text = dtMaterialRequisitionNote.Rows[0]["NewWoodsRequisitionNo"].ToString();
          this.txtRequestBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          radProduction.Checked = true;
        }

        // Set DeliveryTime
        this.SetDeliveryTime();
        ultDeliveryDate.Value = deliveryDate;
        ultcbHour.Value = deliveryHour;
        ultcbMinute.Value = deliveryMinute;
      }

      // 2: Detail
      ultAfter.DataSource = null;
      DataSet ds = this.DataSetRequestOnline();
      ds.Tables["dtParent"].Merge(dsSource.Tables[1]);
      ds.Tables["dtChild"].Merge(dsSource.Tables[2]);
      ultAfter.DataSource = ds;

      for (int i = 0; i < ultAfter.Rows.Count; i++)
      {
        UltraGridRow row = ultAfter.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 2)
        {
          row.CellAppearance.BackColor = Color.LightSalmon;
        }
        else if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 1)
        {
          row.CellAppearance.BackColor = Color.GreenYellow;
        }
      }

      ultDetail.DataSource = dsSource.Tables[3];
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        if(DBConvert.ParseInt(ultDetail.Rows[i].Cells["Type"].Value.ToString()) == 2)
        {
          ultDetail.Rows[i].Appearance.BackColor = Color.LightSalmon;
        }
        else if (DBConvert.ParseInt(ultDetail.Rows[i].Cells["Type"].Value.ToString()) == 1)
        {
          ultDetail.Rows[i].Appearance.BackColor = Color.GreenYellow;
        }
      }

      // Set Status Control
      this.SetStatusControl();
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      if(status == 1 || status == 2)
      {
        ultcbUrgent.ReadOnly = true;
        ultcbHour.ReadOnly = true;
        ultcbMinute.ReadOnly = true;
        txtRemark.ReadOnly = true;
        btnAddSpecial.Enabled = false;
        btnAddAllocate.Enabled = false;
        chkConfirm.Enabled = false;
        btnSave.Enabled = false;
        btnDelete.Enabled = false;
        chkDeliveryTime.Enabled = false;
        chkConfirm.Checked = true;
      }
      if (ultAfter.Rows.Count > 0 || this.veneerRequisitionPid != long.MinValue)
      {
        radProduction.Enabled = false;
        radOrderDepartment.Enabled = false;
      }
      else
      {
        radProduction.Enabled = true;
        radOrderDepartment.Enabled = true;
      }
    }

    #endregion Init

    #region Function
    /// <summary>
    /// Data Set When Load
    /// </summary>
    /// <returns></returns>
    private DataSet DataSetRequestOnline()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("MaterialNameVn", typeof(System.String));
      taParent.Columns.Add("Unit", typeof(System.String));
      taParent.Columns.Add("TotalRequire", typeof(System.Double));
      taParent.Columns.Add("SpecialQty", typeof(System.Double));
      taParent.Columns.Add("Type", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("WO", typeof(System.Int32));
      taChild.Columns.Add("CarcassCode", typeof(System.String));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("Require", typeof(System.Double));
      taChild.Columns.Add("Type", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", new DataColumn[] { taParent.Columns["MaterialCode"], taParent.Columns["Type"] }, new DataColumn[] { taChild.Columns["MaterialCode"], taChild.Columns["Type"] }));
      return ds;
    }

    /// <summary>
    /// Check Valid Info
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckData(out string message)
    {
      message = string.Empty;
      if (ultcbUrgent.Value == null)
      {
        message = "Urgent";
        return false;
      }

      // Delivery Date
      if (ultDeliveryDate.Value == null)
      {
        message = "DeliveryDate >= Today";
        return false;
      }
      if (DBConvert.ParseDateTime(ultDeliveryDate.Value) < DateTime.Today)
      {
        message = "DeliveryDate >= Today";
        return false;
      }

      // Delivery Time
      string commandText = "SELECT DATEPART(HH, GETDATE()) [hour],  DATEPART(MI, GETDATE()) [minute]";
      DataTable dtHour = DataBaseAccess.SearchCommandTextDataTable(commandText);
      int hour = DBConvert.ParseInt(dtHour.Rows[0]["hour"].ToString());
      int minute = DBConvert.ParseInt(dtHour.Rows[0]["minute"].ToString());
      // Hour
      if (ultcbHour.Value != null)
      {
        if (ultcbMinute.Value != null)
        {
          if (DBConvert.ParseDateTime(ultDeliveryDate.Value) == DateTime.Today)
          {
            if (DBConvert.ParseInt(ultcbHour.Value.ToString()) < hour)
            {
              message = "Delivery Time >=" + hour + "h" + minute;
              return false;
            }
            else if (DBConvert.ParseInt(ultcbHour.Value.ToString()) == hour)
            {
              if (DBConvert.ParseInt(ultcbMinute.Value.ToString()) < minute)
              {
                message = "Delivery Time >=" + hour + "h" + minute;
                return false;
              }
            }
          }
        }
        else
        {
          message = "Minute";
          return false;
        }
      }
      else
      {
        message = "Hour";
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save Master Info
    /// </summary>
    /// <returns></returns>
    private bool SaveMaster(int status)
    {
      DBParameter[] inputParam = new DBParameter[12];
      if (this.veneerRequisitionPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.veneerRequisitionPid);
      }
      inputParam[1] = new DBParameter("@Code", DbType.AnsiString, 8, "RNV");

      inputParam[2] = new DBParameter("@Status", DbType.Int32, status);

      inputParam[3] = new DBParameter("@DepartmentRequest", DbType.AnsiString, 8, ultcbDepartmentRequest.Value.ToString());
      if (txtRemark.Text.Length > 0)
      {
        inputParam[4] = new DBParameter("@Remark", DbType.String, 1024, txtRemark.Text);
      }
      inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[6] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[7] = new DBParameter("@Urgent", DbType.Int32, DBConvert.ParseInt(ultcbUrgent.Value.ToString()));
      if (ultDeliveryDate.Value != null)
      {
        inputParam[8] = new DBParameter("@DeliveryDate", DbType.DateTime, ultDeliveryDate.Value);
      }
      if (ultcbHour.Value != null)
      {
        inputParam[9] = new DBParameter("@DeliveryHour", DbType.Int32, DBConvert.ParseInt(ultcbHour.Value.ToString()));
      }
      if (ultcbMinute.Value != null)
      {
        inputParam[10] = new DBParameter("@DeliveryMinute", DbType.Int32, DBConvert.ParseInt(ultcbMinute.Value.ToString()));
      }
      if (radProduction.Checked)
      {
        inputParam[11] = new DBParameter("@IsProduction", DbType.Int32, 1);
      }
      else if (radOrderDepartment.Checked)
      {
        inputParam[11] = new DBParameter("@IsProduction", DbType.Int32, 0);
      }

      DBParameter[] outputParam = new DBParameter[2];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      outputParam[1] = new DBParameter("@MaterialRequisitionPid", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spGNRVeneerRequisitionNote_Edit", inputParam, outputParam);
      long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      this.veneerRequisitionPid = DBConvert.ParseLong(outputParam[1].Value.ToString());

      if(result == 0)
      {
        return false;
      }
      return true;
    }

    #endregion Function

    #region Event
    /// <summary>
    /// Init Layout After
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultAfter_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultAfter);

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;
      e.Layout.Bands[1].Columns["Type"].Hidden = true;
      e.Layout.Bands[0].Columns["TotalRequire"].Header.Caption = "TotalRequire(M2)";
      e.Layout.Bands[1].Columns["Require"].Header.Caption = "Require(M2)";
      e.Layout.Bands[0].Columns["TotalRequire"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["SpecialQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      
      e.Layout.Bands[1].Columns["MaterialCode"].Hidden = true;
      e.Layout.Bands[1].Columns["Require"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      if (status == 1 || status == 2)
      {
        e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      }

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultDetail);

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Sheet";
      e.Layout.Bands[0].Columns["QtyM2"].Header.Caption = "Qty(M2)";
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyM2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      if (status == 1 || status == 2)
      {
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      }
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["QtyM2"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.00}";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Before Row Delete (Detail)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    /// <summary>
    /// Before Row Deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultAfter_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      // Check Luoi Detail Xem Co Material Do Khong
      DataTable dtDetail = (DataTable)ultDetail.DataSource;
      foreach (UltraGridRow row in e.Rows)
      {
        string select = string.Format(@"MaterialCode = '{0}'", row.Cells["MaterialCode"].Value.ToString());
        DataRow[] foundRow = dtDetail.Select(select);
        if (foundRow.Length >= 1)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0093"), row.Cells["MaterialCode"].Value.ToString());
          WindowUtinity.ShowMessageErrorFromText(message);

          e.Cancel = true;
          return;
        }
      }
      // Confirmed Save
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      // Delete
      if (this.veneerRequisitionPid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) > 0)
          {
            DBParameter[] inputParams = new DBParameter[1];
            inputParams[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

            DataBaseAccess.ExecuteStoreProcedure("spGNRVeneerRequisitionNoteDetail_Delete", inputParams, outputParams);
            if (DBConvert.ParseInt(outputParams[0].Value.ToString()) != 1)
            {
              WindowUtinity.ShowMessageError("ERR0004");
              this.LoadData();
              return;
            }
          }
        }
      }
      // Load Data
      this.LoadData();
    }


    /// <summary>
    /// Change Urgent
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultcbUrgent_ValueChanged(object sender, EventArgs e)
    {
      this.SetDeliveryTime();
    }

    /// <summary>
    /// Check DeliveryTime
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkDeliveryTime_CheckedChanged(object sender, EventArgs e)
    {
      if (chkDeliveryTime.Checked)
      {
        ultDeliveryDate.ReadOnly = false;
        ultcbHour.ReadOnly = false;
        ultcbMinute.ReadOnly = false;
      }
      else
      {
        ultDeliveryDate.ReadOnly = true;
        ultcbHour.ReadOnly = true;
        ultcbMinute.ReadOnly = true;
      }
    }

    /// <summary>
    /// Add (Allocate Special)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      radOrderDepartment.Enabled = false;
      radProduction.Enabled = false;

      // Check Data
      string message = string.Empty;
      bool success = this.CheckData(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Master Info
      this.status = 0;
      this.SaveMaster(this.status);

      // Load viewGNR_06_004
      viewGNR_06_006 uc = new viewGNR_06_006();
      uc.pid = this.veneerRequisitionPid;
      WindowUtinity.ShowView(uc, "SEARCH INFO ALLOCATE SPECIAL", false, ViewState.ModalWindow, FormWindowState.Maximized);

      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Add (Allocate WO & Carcass)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddAllocate_Click(object sender, EventArgs e)
    {
      radOrderDepartment.Enabled = false;
      radProduction.Enabled = false;

      // Check Data
      string message = string.Empty;
      bool success = this.CheckData(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Master Info
      this.status = 0;
      this.SaveMaster(this.status);

      // Load viewGNR_06_003
      viewGNR_06_003 uc = new viewGNR_06_003();
      uc.pid = this.veneerRequisitionPid;
      uc.department = ultcbDepartmentRequest.Value.ToString();
      if (radOrderDepartment.Checked)
      {
        uc.type = 2;
      }
      else
      {
        uc.type = 1;
      }
      WindowUtinity.ShowView(uc, "SEARCH INFO ALLOCATE WO & CARCASS", false, ViewState.ModalWindow, FormWindowState.Maximized);

      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Save Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      // Set Status
      if (chkConfirm.Checked)
      {
        this.status = 1;
      }
      else
      {
        this.status = 0;
      }
      // Check Data
      bool success = this.CheckData(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      // Save Master Info
      success = this.SaveMaster(this.status);
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      // Load Data
      this.LoadData();
    }
   
    /// <summary>
    /// Delete Note
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (this.veneerRequisitionPid == long.MinValue)
      {
        return;
      }
      if (WindowUtinity.ShowMessageConfirm("MSG0007", txtMRNCode.Text).ToString() == "No")
      {
        return;
      }
      if (this.veneerRequisitionPid != long.MinValue)
      {
        string storeName = "spGNRVeneerRequisitionNote_Delete";
        DBParameter[] input = new DBParameter[2];
        input[0] = new DBParameter("@pid", DbType.Int64, this.veneerRequisitionPid);
        input[1] = new DBParameter("@userPid", DbType.Int64, SharedObject.UserInfo.UserPid);
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure(storeName, input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result == 1)
        {
          WindowUtinity.ShowMessageSuccess("MSG0002");
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0093", txtMRNCode.Text);
          return;
        }
      }
      this.btnDelete.Visible = false;
      this.CloseTab();
    }

    /// <summary>
    /// Close Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultAfter_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      try
      {
        UltraGridRow rowSelect = ultAfter.Selected.Rows[0];
        if (this.veneerRequisitionPid != long.MinValue
            && DBConvert.ParseInt(rowSelect.Cells["Type"].Value.ToString()) == 2)
        {
          string commandText = string.Empty;
          commandText += " SELECT MaterialCode, ROUND(SUM((VED.Qty * DIM.[Length] * DIM.Width / 1000000)), 2) QtyM2 ";
          commandText += " FROM TblGNRVeneerRequisitionNote VE ";
          commandText += " 	INNER JOIN TblGNRVeneerRequisitionNoteDetailByLotnoID VED ON VE.Pid = VED.VRNPid ";
          commandText += " 	LEFT JOIN TblWHDDimensionVeneer DIM ON VED.Dimension = DIM.Pid ";
          commandText += " WHERE VE.Pid = " + this.veneerRequisitionPid;
          commandText += "  AND VED.MaterialCode = '" + rowSelect.Cells["MaterialCode"].Value.ToString() + "'";
          commandText += "  AND VED.[Type] != 1 ";
          commandText += " GROUP BY MaterialCode";
          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dt == null)
          {
            return;
          }

          viewGNR_06_005 uc = new viewGNR_06_005();
          uc.pid = this.veneerRequisitionPid;
          uc.materialCode = rowSelect.Cells["MaterialCode"].Value.ToString();
          uc.materialName = rowSelect.Cells["MaterialNameVn"].Value.ToString();
          if (dt.Rows.Count > 0)
          {
            uc.qtysuggest = DBConvert.ParseDouble(rowSelect.Cells["TotalRequire"].Value.ToString()) -
                                    DBConvert.ParseDouble(dt.Rows[0]["QtyM2"].ToString());
          }
          else
          {
            uc.qtysuggest = DBConvert.ParseDouble(rowSelect.Cells["TotalRequire"].Value.ToString());
          }

          WindowUtinity.ShowView(uc, "REQUEST ONLINE DETAIL (MATERIAL CODE)", false, ViewState.ModalWindow, FormWindowState.Maximized);
          // Load Data
          this.LoadData();
        }
      }
      catch
      { }
    }

    private void ultDetail_AfterRowsDeleted(object sender, EventArgs e)
    {
      if (this.veneerRequisitionPid != long.MinValue)
      {
        // 1: Delete Row
        foreach (long pidDelete in this.listDeletedPid)
        {
          DBParameter[] inputParams = new DBParameter[2];
          inputParams[0] = new DBParameter("@Pid", DbType.Int64, pidDelete);
          inputParams[1] = new DBParameter("@VRNPid", DbType.Int64, this.veneerRequisitionPid);
          DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          DataBaseAccess.ExecuteStoreProcedure("spGNRVeneerRequisitionNoteDetailByLotNoID_Delete", inputParams, outputParams);
          if (DBConvert.ParseLong(outputParams[0].Value.ToString()) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0004");
            this.LoadData();
            return;
          }
        }

        this.LoadData();
      }
    }

    private void radOrderDepartment_CheckedChanged(object sender, EventArgs e)
    {
      if (radOrderDepartment.Checked)
      {
        btnAddSpecial.Enabled = false;
      }
      else
      {
        btnAddSpecial.Enabled = true;
      }
    }
    #endregion Event   
  }
}