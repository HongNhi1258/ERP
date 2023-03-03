/*
  Author      : Xuan Truong
  Description : Material Requisition Info
  Date        : 04/01/2012
  Kind        : 1 (Materials Allocation for WO, WO & Item)
              : 2 (Materials Allocation for Department)
              : 3 (Materials Allocation for Supplement)
              : 4 (Materials No Allocation)
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.General;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewGNR_02_002 : MainUserControl
  {
    #region Field
    private int whPid = 0;
    public long MaterialRequisitionPid = long.MinValue;
    private long currentWo = long.MinValue;
    private int status = int.MinValue;
    private int flag = 0;

    private DateTime deliveryDate = DateTime.MinValue;
    private int deliveryHour = int.MinValue;
    private int deliveryMinute = int.MinValue;

    #endregion Field

    #region Init

    public viewGNR_02_002()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load viewGNR_02_002
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_02_002_Load(object sender, EventArgs e)
    {
      // Load Department User Login
      this.LoadDepartmentUserLogin();

      // Load Urgent Level
      this.LoadUrgentLevel();

      // Warehouse
      Utility.LoadUltraCBMaterialWHList(ucbWarehouse);

      // Load WO
      this.LoadWO();

      // Load ItemCode
      this.LoadItemCode();

      // Load Supplement
      this.LoadSupplement();

      // Load Day 
      this.LoadDiliveryDate();

      // Load Hour
      this.LoadHour();

      // Load Minute
      this.LoadMinute();

      // Load Data
      this.LoadData();

      // Set Status Control
      this.SetStatusControl();
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      string storeName = "spGNRMaterialRequisitionByMRNPid_Select";
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@Pid", DbType.Int64, this.MaterialRequisitionPid);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, input);
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
        this.whPid = DBConvert.ParseInt(row["WarehousePid"]);
        ucbWarehouse.Value = this.whPid;

        ultcbHour.Value = row["DeliveryHour"].ToString();
        ultcbMinute.Value = row["DeliveryMinute"].ToString();
        ultDeliveryDate.Value = DBConvert.ParseDateTime(row["DeliveryDate"].ToString(), Shared.Utility.ConstantClass.FORMAT_DATETIME);

        //chkConfirm.Checked = true;

        //if (status == 1 || status == 2)
        //{
        //  ultcbHour.Value = row["DeliveryHour"].ToString();
        //  ultcbMinute.Value = row["DeliveryMinute"].ToString();
        //  ultDeliveryDate.Value = DBConvert.ParseDateTime(row["DeliveryDate"].ToString(), Shared.Utility.ConstantClass.FORMAT_DATETIME);

        //  chkConfirm.Checked = true;
        //}
        //else
        //{
        //  this.SetDeliveryTime();
        //  ultDeliveryDate.Value = deliveryDate;
        //  ultcbHour.Value = deliveryHour;
        //  ultcbMinute.Value = deliveryMinute;

        //  chkConfirm.Checked = false;
        //}

        if (DBConvert.ParseInt(row["Recovery"].ToString()) == 1)
        {
          chkRecovery.Checked = true;
        }
        else
        {
          chkRecovery.Checked = false;
        }

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
        DataTable dtMaterialRequisitionNote = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FGNRGetNewMaterialRequisitionNo('RNM') NewMaterialRequisitionNo");
        if ((dtMaterialRequisitionNote != null) && (dtMaterialRequisitionNote.Rows.Count > 0))
        {
          txtMRNCode.Text = dtMaterialRequisitionNote.Rows[0]["NewMaterialRequisitionNo"].ToString();
          this.txtRequestBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          radProduction.Checked = true;
          chkRecovery.Checked = false;
        }
        this.SetDeliveryTime();
        ultDeliveryDate.Value = deliveryDate;
        ultcbHour.Value = deliveryHour;
        ultcbMinute.Value = deliveryMinute;
        // Load default WH
        if (ucbWarehouse.Rows.Count > 0)
        {
          ucbWarehouse.Rows[0].Selected = true;
        }
      }

      dsGNRListMaterialRequisitionNote ds = new dsGNRListMaterialRequisitionNote();
      ds.Tables["MaterialRequisition"].Merge(dsSource.Tables[1]);
      ds.Tables["MaterialRequisitionDetail"].Merge(dsSource.Tables[2]);

      ultAfter.DataSource = null;

      ultAfter.DataSource = ds;

      for (int i = 0; i < ultAfter.Rows.Count; i++)
      {
        for (int j = 0; j < ultAfter.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow row = ultAfter.Rows[i].ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 1)
          {
            row.CellAppearance.BackColor = Color.LightSalmon;
          }
          else if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 2)
          {
            row.CellAppearance.BackColor = Color.GreenYellow;
          }
          else if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 3)
          {
            row.CellAppearance.BackColor = Color.Pink;
          }
          else if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 4)
          {
            row.CellAppearance.BackColor = Color.Aquamarine;
          }
        }
      }
      // Set Status Control
      this.SetStatusControl();
    }

    // Load Day
    private void LoadDiliveryDate()
    {
      string commandText = "SELECT CONVERT(varchar, GETDATE(), 103) [Day], DATEPART(HH, GETDATE()) [Hour], DATEPART(MI, GETDATE()) [Minute]";
      DataTable dtHour = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DateTime day = DBConvert.ParseDateTime(dtHour.Rows[0]["Day"].ToString(), Shared.Utility.ConstantClass.FORMAT_DATETIME);
      ultDeliveryDate.Value = day;
    }

    /// <summary>
    /// Load Month
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
    /// Load Year
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
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      if (status == 1 || status == 2)
      {
        ultcbUrgent.ReadOnly = true;
        ultcbHour.ReadOnly = true;
        ultcbMinute.ReadOnly = true;
        ultDeliveryDate.ReadOnly = true;
        txtRemark.ReadOnly = true;

        btnAdd.Enabled = false;
        btnImportExcel.Enabled = false;
        chkConfirm.Enabled = false;
        btnSave.Enabled = false;
        btnDelete.Enabled = false;
        chkDeliveryTime.Enabled = false;
        chkHide.Checked = true;
        chkExpandAll.Checked = true;
        chkConfirm.Checked = true;
        ucbWarehouse.ReadOnly = true;
      }
      else
      {
        ucbWarehouse.ReadOnly = false;
      }
      if (ultAfter.Rows.Count > 0)
      {
        radProduction.Enabled = false;
        radOrderDepartment.Enabled = false;
        chkRecovery.Enabled = false;
        ucbWarehouse.ReadOnly = true;
      }
      else
      {
        radProduction.Enabled = true;
        radOrderDepartment.Enabled = true;
        chkRecovery.Enabled = true;
      }
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
    /// Load WO
    /// </summary>
    private void LoadWO()
    {
      string commandText = "SELECT Pid FROM TblPLNWorkOrder ORDER BY Pid DESC";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultcbWO.DataSource = dtSource;
      ultcbWO.DisplayMember = "Pid";
      ultcbWO.ValueMember = "Pid";
      ultcbWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbWO.DisplayLayout.Bands[0].Columns["Pid"].Width = 200;
    }

    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadItemCode()
    {
      string commandText = string.Empty;
      if (ultcbWO.Value != null)
      {
        long wo = DBConvert.ParseLong(ultcbWO.Value.ToString());

        commandText = string.Format(@"
          SELECT DISTINCT ITEM.ItemCode, ITEM.Name ItemName, (ITEM.ItemCode + ' | ' + ITEM.Name) DisplayText
          FROM TblPLNWorkOrderConfirmedDetails WO
	          INNER JOIN TblBOMItemBasic ITEM ON WO.ItemCode = ITEM.ItemCode
          WHERE WO.WorkOrderPid = {0}
          ORDER BY ITEM.ItemCode DESC", wo);
      }
      else
      {
        commandText = string.Format(@"
          SELECT DISTINCT ITEM.ItemCode, ITEM.Name ItemName, (ITEM.ItemCode + ' | ' + ITEM.Name) DisplayText
          FROM TblPLNWorkOrderConfirmedDetails WO
	          INNER JOIN TblBOMItemBasic ITEM ON WO.ItemCode = ITEM.ItemCode
          ORDER BY ITEM.ItemCode DESC");
      }
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultcbItem, dtSource, "ItemCode", "DisplayText", false, "DisplayText");
      ultcbItem.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
    }

    /// <summary>
    /// Load Supplement
    /// </summary>
    private void LoadSupplement()
    {
      string commandText = string.Format(@"SELECT DISTINCT SUP.Pid, SUP.SupplementNo, SUP.Description 
                                           FROM TblPLNSupplementForWorkOrder SUP
	                                              INNER JOIN TblPLNSupplementForWorkOrderDetail DT ON (SUP.Pid = DT.SupplementPid)
                                          WHERE (SUP.Status = 1) AND (DT.IsCloseWork = 0)
                                          ORDER BY Pid DESC");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText, 120);
      ultcbSupplement.DataSource = dtSource;
      ultcbSupplement.ValueMember = "Pid";
      ultcbSupplement.DisplayMember = "SupplementNo";
      ultcbSupplement.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultcbSupplement.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    #endregion Init

    #region Function
    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      DataTable dtBefore = new DataTable();
      if (radProduction.Checked)
      {
        if (ultcbMaterialCode.Value != null || ultcbWO.Value != null || ultcbItem.Value != null || ultcbSupplement.Value != null || txtMaterialName.Text.Trim().Length > 0)
        {
          DBParameter[] inputParam = new DBParameter[8];
          // Supplement
          if (ultcbSupplement.Value != null)
          {
            inputParam[0] = new DBParameter("@SupplementPid", DbType.Int64, DBConvert.ParseLong(ultcbSupplement.Value.ToString()));
          }
          // Material Code
          if (ultcbMaterialCode.Value != null)
          {
            inputParam[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, ultcbMaterialCode.Value.ToString());
          }
          // Material Name
          if (txtMaterialName.Text.Trim().Length > 0)
          {
            inputParam[2] = new DBParameter("@MaterialName", DbType.String, 256, "%" + txtMaterialName.Text.Replace("'", "''") + "%");
          }
          // Item
          if (ultcbItem.Value != null)
          {
            inputParam[3] = new DBParameter("@ItemCode ", DbType.AnsiString, 16, ultcbItem.Value.ToString());
          }
          // WO
          if (ultcbWO.Value != null)
          {
            inputParam[4] = new DBParameter("@Wo", DbType.Int64, DBConvert.ParseLong(ultcbWO.Value.ToString()));
          }
          // Check Recovery
          int recovery = (chkRecovery.Checked) ? 1 : 0;
          inputParam[5] = new DBParameter("@Recovery", DbType.Int32, recovery);
          // Revision
          if (ultcbItem.Value != null)
          {
            int revision = DBConvert.ParseInt(ultcbItem.SelectedRow.Cells["Revision"].Value.ToString());
            if (revision != int.MinValue)
            {
              inputParam[6] = new DBParameter("@Revision", DbType.Int32, revision);
            }
          }
          // Warehouse
          inputParam[7] = new DBParameter("@Warehouse", DbType.Int32, this.whPid);
          dtBefore = DataBaseAccess.SearchStoreProcedureDataTable("spGNRGetRequiredMaterialsForProduction", 300, inputParam);
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0013", "At Least One Condition");
          return;
        }
      }
      else if (radOrderDepartment.Checked)
      {
        DBParameter[] input = new DBParameter[5];
        if (ultcbDepartmentRequest.Value != null)
        {
          input[0] = new DBParameter("@Department", DbType.AnsiString, 50, ultcbDepartmentRequest.Value.ToString());
        }
        // Material
        if (ultcbMaterialCode.Value != null)
        {
          input[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 50, ultcbMaterialCode.Value.ToString());
        }
        // Material Name
        if (txtMaterialName.Text.Trim().Length > 0)
        {
          input[2] = new DBParameter("@MaterialName", DbType.String, 256, "%" + txtMaterialName.Text.Replace("'", "''") + "%");
        }
        // Check Recovery
        int recovery = (chkRecovery.Checked) ? 1 : 0;
        input[3] = new DBParameter("@Recovery", DbType.Int32, recovery);
        // Warehouse
        input[4] = new DBParameter("@Warehouse", DbType.Int32, this.whPid);
        dtBefore = DataBaseAccess.SearchStoreProcedureDataTable("spGNRGetRequiredMaterialsForOtherDept", input);
      }

      ultBefore.DataSource = dtBefore;
      if (dtBefore != null)
      {
        for (int i = 0; i < ultBefore.Rows.Count; i++)
        {
          UltraGridRow row = ultBefore.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Kind"].Value.ToString()) == 1)
          {
            row.CellAppearance.BackColor = Color.LightSalmon;
          }
          else if (DBConvert.ParseInt(row.Cells["Kind"].Value.ToString()) == 2)
          {
            row.CellAppearance.BackColor = Color.GreenYellow;
          }
          else if (DBConvert.ParseInt(row.Cells["Kind"].Value.ToString()) == 3)
          {
            row.CellAppearance.BackColor = Color.Pink;
          }
          else if (DBConvert.ParseInt(row.Cells["Kind"].Value.ToString()) == 4)
          {
            row.CellAppearance.BackColor = Color.Aquamarine;
          }
        }
      }
    }

    /// <summary>
    /// Add Material Requisition
    /// </summary>
    private bool AddMaterialRequisition()
    {
      status = 0;
      bool success = this.AddMaterialRequisitionInfo(status);
      if (!success)
      {
        return false;
      }
      success = this.AddMaterialRequisitionDetail();
      if (!success)
      {
        return false;
      }
      return success;
    }

    /// <summary>
    /// Add Material Requisition Info
    /// </summary>
    /// <returns></returns>
    private bool AddMaterialRequisitionInfo(int status)
    {
      //this.SetDeliveryTime();
      DBParameter[] inputParam = new DBParameter[14];
      if (MaterialRequisitionPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.MaterialRequisitionPid);
      }
      inputParam[1] = new DBParameter("@Code", DbType.AnsiString, 8, "RNM");

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
      // Check Recovery
      int recovery = (chkRecovery.Checked) ? 1 : 0;
      inputParam[12] = new DBParameter("@Recovery", DbType.Int32, recovery);
      inputParam[13] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);

      DBParameter[] outputParam = new DBParameter[2];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      outputParam[1] = new DBParameter("@MaterialRequisitionPid", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialRequisitionNote_Edit", inputParam, outputParam);
      long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      MaterialRequisitionPid = DBConvert.ParseLong(outputParam[1].Value.ToString());

      if (result == 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Add Material Requisition Detail
    /// </summary>
    /// <returns></returns>
    private bool AddMaterialRequisitionDetail()
    {
      for (int i = 0; i < ultBefore.Rows.Count; i++)
      {
        UltraGridRow row = ultBefore.Rows[i];
        if (row.Cells["Require"].Text.Trim().Length > 0)
        {
          DBParameter[] inputParam = new DBParameter[11];

          inputParam[0] = new DBParameter("@MRNPid", DbType.Int64, this.MaterialRequisitionPid);

          if (DBConvert.ParseLong(row.Cells["Wo"].Value.ToString()) != long.MinValue)
          {
            inputParam[1] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(row.Cells["Wo"].Value.ToString()));
          }
          if (row.Cells["ItemCode"].Text.Length > 0)
          {
            inputParam[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, row.Cells["ItemCode"].Value.ToString());
          }
          if (row.Cells["Revision"].Text.Length > 0)
          {
            inputParam[3] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
          }
          if (DBConvert.ParseLong(row.Cells["SupplementDetailPid"].Value.ToString()) != long.MinValue)
          {
            inputParam[4] = new DBParameter("@SupplementDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["SupplementDetailPid"].Value.ToString()));
          }

          inputParam[5] = new DBParameter("MaterialCode", DbType.AnsiString, 16, row.Cells["MaterialCode"].Value.ToString());

          if (DBConvert.ParseDouble(row.Cells["Require"].Value.ToString()) != long.MinValue)
          {
            inputParam[6] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row.Cells["Require"].Value.ToString()));
          }

          int type = DBConvert.ParseInt(row.Cells["Kind"].Value.ToString());

          inputParam[7] = new DBParameter("@Type", DbType.Int32, type);

          // Check Recovery
          int recovery = (chkRecovery.Checked) ? 1 : 0;
          inputParam[8] = new DBParameter("@Recovery", DbType.Int32, recovery);

          if (ultcbDepartmentRequest.Value != null)
          {
            inputParam[9] = new DBParameter("@DepartmentRequest", DbType.AnsiString, 8, ultcbDepartmentRequest.Value.ToString());
          }

          inputParam[10] = new DBParameter("@Warehouse", DbType.Int32, ConstantClass.MATERIALS_WAREHOUSE);

          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialRequisitionNoteDetail_Edit", inputParam, outputParam);
          long success = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (success <= 0)
          {
            return false;
          }
        }
      }
      return true;
    }


    /// <summary>
    /// Check Valid Detail
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidDetail(out string message)
    {
      message = string.Empty;
      string commandMaterial = string.Format("SELECT MaterialCode FROM VBOMMaterials WHERE ('|' + WHPids + '|') LIKE ('%|' + CAST({0} as varchar) + '|%')", this.whPid);
      DataTable dtMaterialList = DataBaseAccess.SearchCommandTextDataTable(commandMaterial);
      for (int i = 0; i < ultBefore.Rows.Count; i++)
      {
        UltraGridRow row = ultBefore.Rows[i];
        if (row.Cells["Require"].Text.Trim().Length > 0)
        {
          if (DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString()) < DBConvert.ParseDouble(row.Cells["Require"].Value.ToString()))
          {
            message = "Remain >= Require";
            return false;
          }
          if (DBConvert.ParseDouble(row.Cells["Require"].Value.ToString()) == 0)
          {
            message = "Require > 0";
            return false;
          }
          if (DBConvert.ParseDouble(row.Cells["Require"].Value.ToString()) == double.MinValue)
          {
            message = "Require > 0";
            return false;
          }

          //Check Material is belongs to WH
          string materialCode = row.Cells["MaterialCode"].Value.ToString();
          if (dtMaterialList.Select(string.Format("MaterialCode = '{0}'", materialCode)).Length == 0)
          {
            message = "Material";
            return false;
          }
        }
      }
      return true;
    }
    /// <summary>
    /// Check Valid Info
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidInfo(out string message)
    {
      message = string.Empty;
      if (ucbWarehouse.SelectedRow == null)
      {
        message = "Warehouse";
        ucbWarehouse.Focus();
        return false;
      }
      if (ultcbUrgent.Value == null)
      {
        message = "Urgent";
        ultcbUrgent.Focus();
        return false;
      }

      // Delivery Date
      if (ultDeliveryDate.Value == null)
      {
        message = "DeliveryDate >= Today";
        ultDeliveryDate.Focus();
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
    /// Send Mail
    /// </summary>
    private void SendEmail()
    {
      string commandText = string.Empty;
      DataTable dtCheck = new DataTable();
      int i = 0;
      string symbol = " </br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp ";
      String symbol1 = " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp ";
      // Send Email
      if (this.status == 1 && this.chkConfirm.Checked)
      {
        Email email = new Email();
        email.Key = email.KEY_MRN_001;
        ArrayList arrList = email.GetDataMain(email.Key);
        if (arrList.Count == 3)
        {
          string toFromSql = string.Empty;
          toFromSql = string.Format(arrList[0].ToString(), SharedObject.UserInfo.UserPid.ToString(), this.ultcbDepartmentRequest.Value.ToString());
          toFromSql = email.GetEmailToFromSql(toFromSql);

          string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
          string urgent = string.Empty;
          if (DBConvert.ParseInt(ultcbUrgent.Value.ToString()) == 1)
          {
            urgent = "<font color=\"red\"><b>" + ultcbUrgent.Text + "</b></font>";
          }
          else
          {
            urgent = ultcbUrgent.Text;
          }
          string getdeliveryTime = string.Empty;
          commandText = "SELECT CONVERT(varchar,DATEPART(HH, MAR.DeliveryTime))+ ' h ' + CONVERT(varchar,DATEPART(MI, MAR.DeliveryTime)) +' ('+ CONVERT(varchar, MAR.DeliveryTime, 103)+')' DeliveryTime ";
          commandText += "FROM TblGNRMaterialRequisitionNote MAR ";
          commandText += "WHERE MAR.Code = '" + txtMRNCode.Text + "'";
          DataTable dtDeliveryTime = DataBaseAccess.SearchCommandTextDataTable(commandText);
          getdeliveryTime = dtDeliveryTime.Rows[0]["DeliveryTime"].ToString();

          string subject = string.Format(arrList[1].ToString(), this.txtMRNCode.Text, userName, getdeliveryTime);

          string listMaterial = string.Empty;
          if (ultAfter.Rows.Count > 0)
          {
            for (i = 0; i < ultAfter.Rows.Count; i++)
            {
              listMaterial += this.ultAfter.Rows[i].Cells["MaterialCode"].Value.ToString() + symbol1 + this.ultAfter.Rows[i].Cells["Name"].Value.ToString() + symbol;
            }
            listMaterial = symbol + listMaterial.Substring(0, listMaterial.Length - symbol.Length);
          }

          string body = string.Format(arrList[2].ToString(), getdeliveryTime, urgent, listMaterial);
          email.InsertEmail(email.Key, toFromSql, subject, body);
          //email.InsertEmail(email.Key, arrList[0].ToString(), subject, body);
        }
      }
    }

    /// <summary>
    /// kiem tra duong dan file image
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    static bool IsValidImage(string filePath)
    {
      return File.Exists(filePath);
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
      e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
      //Shared.Utility.Utility.SetPropertiesUltraGrid(ultAfter);

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[1].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Mã vật tư";
      e.Layout.Bands[0].Columns["MaterialNameVn"].Header.Caption = "Tên vật tư";
      e.Layout.Bands[0].Columns["Unit"].Header.Caption = "Đơn vị";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "SL\nyêu cầu";
      e.Layout.Bands[1].Columns["WO"].Header.Caption = "LSX";
      e.Layout.Bands[1].Columns["ItemCode"].Header.Caption = "Mã SP";
      e.Layout.Bands[1].Columns["Revision"].Header.Caption = "PB";
      e.Layout.Bands[1].Columns["SupplementNo"].Header.Caption = "Phiếu bổ sung";
      e.Layout.Bands[1].Columns["Qty"].Header.Caption = "SL\nyêu cầu";

      e.Layout.Bands[0].Columns["MRNPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["MaterialCode"].Width = 200;
      e.Layout.Bands[0].Columns["Unit"].Width = 100;
      e.Layout.Bands[0].Columns["Qty"].Width = 100;

      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["MRNPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Type"].Hidden = true;
      e.Layout.Bands[1].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      if (status == 1 || status == 2)
      {
        e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      }
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    /// <summary>
    /// Init Layout Before
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultBefore_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitStyle = AutoFitStyle.None;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      //Shared.Utility.Utility.SetPropertiesUltraGrid(ultBefore);

      e.Layout.Bands[0].Columns["SupplementDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Kind"].Hidden = true;

      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["Wo"].Header.Caption = "LSX";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Mã SP";
      e.Layout.Bands[0].Columns["Revision"].Header.Caption = "PB";
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Mã vật tư";
      e.Layout.Bands[0].Columns["MaterialNameVn"].Header.Caption = "Tên vật tư";
      e.Layout.Bands[0].Columns["Unit"].Header.Caption = "Đơn vị";
      e.Layout.Bands[0].Columns["QtyBOM"].Header.Caption = "SL định\nmức";
      e.Layout.Bands[0].Columns["Balance"].Header.Caption = "SL tồn";
      e.Layout.Bands[0].Columns["TotalRequired"].Header.Caption = "Tổng SL\nyêu cầu"; 
      e.Layout.Bands[0].Columns["Allocated"].Header.Caption = "SL được\nphân bổ";
      e.Layout.Bands[0].Columns["Issued"].Header.Caption = "SL\nđã nhận";
      e.Layout.Bands[0].Columns["Required"].Header.Caption = "SL đã\nyêu cầu";
      e.Layout.Bands[0].Columns["Require"].Header.Caption = "SL \nyêu cầu";
      e.Layout.Bands[0].Columns["Remain"].Header.Caption = "SL \ncòn lại";

      e.Layout.Bands[0].Columns["Wo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remain"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Balance"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Required"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Allocated"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Issued"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalRequired"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyBOM"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Wo"].Width = 50;
      e.Layout.Bands[0].Columns["ItemCode"].Width = 70;
      e.Layout.Bands[0].Columns["Revision"].Width = 30;
      e.Layout.Bands[0].Columns["MaterialCode"].Width = 90;
      e.Layout.Bands[0].Columns["MaterialNameVn"].Width = 180;
      e.Layout.Bands[0].Columns["Unit"].Width = 50;

      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Required"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Issued"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Require"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Allocated"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Balance"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalRequired"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyBOM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["Auto"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
    }
    /// <summary>
    /// After cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultBefore_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string strColName = e.Cell.Column.ToString().ToLower();
      int index = e.Cell.Row.Index;
      int recovery = int.MinValue;

      if (chkRecovery.Checked)
      {
        recovery = 1;
      }
      else
      {
        recovery = 0;
      }

      switch (strColName)
      {
        case "Require":
          {
            if (e.Cell.Row.Cells["Require"].Text.Trim().Length > 0)
            {
              if (DBConvert.ParseDouble(e.Cell.Row.Cells["Require"].Value.ToString()) == double.MinValue || DBConvert.ParseDouble(e.Cell.Row.Cells["Require"].Value.ToString()) < 0)
              {
                DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Require");
                break;
              }
            }
            break;
          }
        case "auto":
          {
            int select = DBConvert.ParseInt(e.Cell.Value.ToString());
            if (select == 1)
            {
              double qty = DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString());
              if (qty > 0)
              {
                e.Cell.Row.Cells["Require"].Value = qty;
              }
            }
            else if (select == 0)
            {
              e.Cell.Row.Cells["Require"].Value = DBNull.Value;
            }
            break;
          }
      }
    }

    /// <summary>
    /// Change Wo
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultcbWO_ValueChanged(object sender, EventArgs e)
    {
      ultcbItem.Value = null;
      this.LoadItemCode();

    }

    /// <summary>
    /// Check Hide
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkHide_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkHide.Checked)
      {
        groupBox2.Visible = false;
        groupBox3.Visible = false;
        btnAdd.Visible = false;
        //btnImportExcel.Visible = false;
      }
      else
      {
        groupBox2.Visible = true;
        groupBox3.Visible = true;
        btnAdd.Visible = true;
        //btnImportExcel.Visible = true;
      }
    }
    /// <summary>
    /// Search Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
      this.SetStatusControl();
    }
    /// <summary>
    /// Add Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckValidInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      success = this.CheckValidDetail(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      success = this.AddMaterialRequisition();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      this.LoadData();
      this.Search();
      chkExpandAll.Checked = false;
    }
    /// <summary>
    /// Save Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      if (chkConfirm.Checked)
      {
        status = 1;
      }
      else
      {
        status = 0;
      }
      bool success = this.CheckValidInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      success = this.AddMaterialRequisitionInfo(status);
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      this.LoadData();
      this.SendEmail();
    }

    /// <summary>
    /// Cell Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultBefore_CellChange(object sender, CellEventArgs e)
    {

      string strColName = e.Cell.Column.ToString().ToLower();
      int index = e.Cell.Row.Index;
      switch (strColName)
      {
        case "auto":
          {
            flag = 1;
            if (DBConvert.ParseInt(e.Cell.Row.Cells["Auto"].Text) == 1)
            {
              if (DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString()) > 0)
              {
                e.Cell.Row.Cells["Require"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString());
              }
            }
            else
            {
              e.Cell.Row.Cells["Require"].Value = DBNull.Value;
            }
            break;
          }
      }
    }
    /// <summary>
    /// Change Urgent
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultcbUrgent_ValueChanged(object sender, EventArgs e)
    {
      this.SetDeliveryTime();
      //ultDeliveryDate.Value = deliveryDate;
      //ultcbHour.Value = deliveryHour;
      //ultcbMinute.Value = deliveryMinute;
      //if (ultcbUrgent.Value != null)
      //{
      //  if (DBConvert.ParseInt(ultcbUrgent.Value.ToString()) == 1)
      //  {
      //    ultcbHour.ReadOnly = false;
      //    ultcbMinute.ReadOnly = false;
      //    ultDeliveryDate.ReadOnly = false;
      //  }
      //  else
      //  {
      //    ultcbHour.ReadOnly = true;
      //    ultcbMinute.ReadOnly = true;
      //    ultDeliveryDate.ReadOnly = true;
      //  }
      //}
    }
    /// <summary>
    /// Before Row Deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultAfter_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      if (this.MaterialRequisitionPid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) > 0)
          {
            DBParameter[] inputParams = new DBParameter[1];
            inputParams[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

            DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialRequisitionNoteDetail_Delete", inputParams, outputParams);
            if (DBConvert.ParseInt(outputParams[0].Value.ToString()) != 1)
            {
              WindowUtinity.ShowMessageError("ERR0004");
              this.LoadData();
              return;
            }
          }
        }
      }
    }

    /// <summary>
    /// Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultBefore_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (flag == 0)
      {
        string strColName = e.Cell.Column.ToString();

        if (string.Compare(strColName, "Require") == 0)
        {
          if (e.Cell.Row.Cells["Require"].Text.Trim().Length > 0)
          {
            if (DBConvert.ParseDouble(e.Cell.Row.Cells["Require"].Text) == double.MinValue || DBConvert.ParseDouble(e.Cell.Row.Cells["Require"].Text) < 0)
            {
              string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Require");
              WindowUtinity.ShowMessageErrorFromText(message);
              e.Cancel = true;
            }
            else if (DBConvert.ParseDouble(e.Cell.Row.Cells["Require"].Text) > DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Text))
            {
              string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Require <= Remain");
              WindowUtinity.ShowMessageErrorFromText(message);
              e.Cancel = true;
            }
          }
        }
      }
      flag = 0;
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
        ultAfter.Rows.ExpandAll(true);
      }
      else
      {
        ultAfter.Rows.CollapseAll(true);
      }
    }
    /// <summary>
    /// Delete Note
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (this.MaterialRequisitionPid == long.MinValue)
      {
        return;
      }
      if (WindowUtinity.ShowMessageConfirm("MSG0007", txtMRNCode.Text).ToString() == "No")
      {
        return;
      }
      if (MaterialRequisitionPid != long.MinValue)
      {
        string storeName = "spGNRMaterialRequisitionNote_Delete";
        DBParameter[] input = new DBParameter[2];
        input[0] = new DBParameter("@pid", DbType.Int64, MaterialRequisitionPid);
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
    /// Auto All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkAutoAll_CheckedChanged(object sender, EventArgs e)
    {
      int selected = (chkAutoAll.Checked ? 1 : 0);
      for (int i = 0; i < ultBefore.Rows.Count; i++)
      {
        ultBefore.Rows[i].Cells["Auto"].Value = selected;
      }
    }

    private void radOrderDepartment_CheckedChanged(object sender, EventArgs e)
    {
      if (radOrderDepartment.Checked)
      {
        ultcbWO.Text = string.Empty;
        ultcbItem.Text = string.Empty;
        ultcbSupplement.Text = string.Empty;
        ultcbWO.ReadOnly = true;
        ultcbItem.ReadOnly = true;
        ultcbSupplement.ReadOnly = true;
        ultBefore.DataSource = null;
      }
    }

    private void radProduction_CheckedChanged(object sender, EventArgs e)
    {
      if (radProduction.Checked)
      {
        ultcbWO.ReadOnly = false;
        ultcbItem.ReadOnly = false;
        ultcbSupplement.ReadOnly = false;
        ultBefore.DataSource = null;
      }
    }

    /// <summary>
    /// Before_DoubleClick
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultBefore_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultBefore.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultBefore.Selected.Rows[0];

      string materialCode = row.Cells["MaterialCode"].Value.ToString();

      try
      {
        string locationImage = @FunctionUtility.GNRGetMaterialImage(materialCode);

        Process p = new Process();
        p.StartInfo.FileName = "rundll32.exe";
        if (IsValidImage(locationImage))
        {
          p.StartInfo.Arguments = @"C:\WINDOWS\System32\shimgvw.dll,ImageView_Fullscreen " + locationImage;
        }
        else
        {
          p.StartInfo.Arguments = @"C:\WINDOWS\System32\shimgvw.dll,ImageView_Fullscreen ";
        }
        p.Start();
      }
      catch
      {

      }
    }

    /// <summary>
    /// After_DoubleClick
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultAfter_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultAfter.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultAfter.Selected.Rows[0];

      string materialCode = row.Cells["MaterialCode"].Value.ToString();

      try
      {
        string locationImage = @FunctionUtility.GNRGetMaterialImage(materialCode);

        Process p = new Process();
        p.StartInfo.FileName = "rundll32.exe";
        if (IsValidImage(locationImage))
        {
          p.StartInfo.Arguments = @"C:\WINDOWS\System32\shimgvw.dll,ImageView_Fullscreen " + locationImage;
        }
        else
        {
          p.StartInfo.Arguments = @"C:\WINDOWS\System32\shimgvw.dll,ImageView_Fullscreen ";
        }
        p.Start();
      }
      catch
      {

      }
    }

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
    /// Close Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnImportExcel_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      if (chkConfirm.Checked)
      {
        status = 1;
      }
      else
      {
        status = 0;
      }
      bool success = this.CheckValidInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      success = this.AddMaterialRequisitionInfo(status);

      if (this.MaterialRequisitionPid != long.MinValue)
      {
        viewGNR_02_003 view = new viewGNR_02_003();
        view.requestPid = this.MaterialRequisitionPid;
        view.typeRequest = radProduction.Checked ? 0 : 1;
        view.recovery = (chkRecovery.Checked) ? 1 : 0;
        WindowUtinity.ShowView(view, "Import Material", true, ViewState.ModalWindow);
        this.LoadData();
        this.Search();
        this.SendEmail();
      }
      else
      {
        WindowUtinity.ShowMessageErrorFromText("Please save master information first!");
      }
    }

    private void ucbWarehouse_ValueChanged(object sender, EventArgs e)
    {
      if (ucbWarehouse.SelectedRow != null)
      {
        this.whPid = DBConvert.ParseInt(ucbWarehouse.Value);
      }
      else
      {
        this.whPid = 0;
      }
      Utility.LoadUltraCBMaterialListByWHPid(ultcbMaterialCode, this.whPid);
    }

    private void ultAfter_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.LoadData();
      this.Search();
      chkExpandAll.Checked = false;
    }
    #endregion Event
  }
}