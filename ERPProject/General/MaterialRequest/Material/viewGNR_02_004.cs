/*
  Author      : Nguyen Ngoc Tien
  Description : Material Requisition Info
  Date        : 13/07/2015
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
  public partial class viewGNR_02_004 : MainUserControl
  {
    #region Field
    private int whPid = 0;
    public long MaterialRequisitionPid = long.MinValue;
    private int status = int.MinValue;

    private DateTime deliveryDate = DateTime.MinValue;
    private int deliveryHour = int.MinValue;
    private int deliveryMinute = int.MinValue;

    #endregion Field

    #region Init

    public viewGNR_02_004()
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

      //
      this.LoadLocation();
      // Set Status Control
      this.SetStatusControl();
      this.SetAutoSearchWhenPressEnter(groupBox3);
    }

    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new KeyEventHandler(ctr_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
    }

    void ctr_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.Search();
        btnSearch.Enabled = true;
        if (ultBefore.Rows.Count > 0)
        {
          btnAdd.Enabled = true;
        }
        else
        {
          btnAdd.Enabled = false;
        }
      }
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      string storeName = "spGNRMaterialRequisitionByMRNPid_SelectForPart";
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

      }
      else
      {
        DataTable dtMaterialRequisitionNote = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FGNRGetNewMaterialRequisitionNo('RNM') NewMaterialRequisitionNo");
        if ((dtMaterialRequisitionNote != null) && (dtMaterialRequisitionNote.Rows.Count > 0))
        {
          txtMRNCode.Text = dtMaterialRequisitionNote.Rows[0]["NewMaterialRequisitionNo"].ToString();
          this.txtRequestBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
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

      DataSet ds = new DataSet();
      ds.Tables.Add("Detail");
      ds.Tables.Add("Part");
      ds.Tables["Detail"].Merge(dsSource.Tables[1]);
      ds.Tables["Part"].Merge(dsSource.Tables[2]);
      ds.Relations.Add(new DataRelation("dsDetail_dsDescrip", new DataColumn[] { ds.Tables[0].Columns["MRNPid"] }, new DataColumn[] { ds.Tables[1].Columns["MRNDetailPid"] }, false));

      ultAfter.DataSource = null;

      ultAfter.DataSource = ds;

      for (int i = 0; i < ultAfter.Rows.Count; i++)
      {
        for (int j = 0; j < ultAfter.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow row = ultAfter.Rows[i].ChildBands[0].Rows[j];
          if (DBConvert.ParseLong(row.Cells["SupplementPid"].Value.ToString()) < 0)
          {
            row.CellAppearance.BackColor = Color.LightSalmon;
          }
          else
          {
            row.CellAppearance.BackColor = Color.LightGreen;
          }
        }
      }
      bool isReadOnly = (ultAfter.Rows.Count > 0 ? true : false);
      ucbWarehouse.ReadOnly = isReadOnly;
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
        chkConfirm.Enabled = false;
        btnSave.Enabled = false;
        btnDelete.Enabled = false;
        chkDeliveryTime.Enabled = false;
        chkExpandAll.Checked = true;
        chkConfirm.Checked = true;
        ucbWarehouse.ReadOnly = true;
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
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultcbWO, dtSource, "Pid", "Pid", false);
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
    /// Load Location
    /// </summary>
    private void LoadLocation()
    {
      string cmText = string.Format(@"SELECT Pid Value, StandByEN + ' - ' + WorkAreaNameVN Display
		                                  FROM TblWIPWorkArea
		                                  WHERE ISNULL(IsDeleted, 0) = 0
			                                  AND DevisionCode = 'CAR'
		                                  ORDER BY OrderBy");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmText);
      Utility.LoadUltraCombo(ultLocation, dt, "Value", "Display", false, "Value");
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
      Utility.LoadUltraCombo(ultcbSupplement, dtSource, "Pid", "SupplementNo", false, "Pid");
    }

    #endregion Init

    #region Function
    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      chkAutoAll.Checked = false;
      DataSet dsBefore = new DataSet();
      if (ultcbMaterialCode.Value != null || ultLocation.Value != null || ultcbWO.Value != null || ultcbItem.Value != null || ultcbSupplement.Value != null || txtMaterialName.Text.Trim().Length > 0 || txtPartCode.Text.Trim().Length > 0)
      {
        DBParameter[] inputParam = new DBParameter[10];
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
          inputParam[3] = new DBParameter("@ItemCode", DbType.AnsiString, 16, ultcbItem.Value.ToString());
        }
        // WO
        if (ultcbWO.Value != null)
        {
          inputParam[4] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ultcbWO.Value.ToString()));
        }
        // part code
        if (txtPartCode.Text.Trim().Length > 0)
        {
          inputParam[5] = new DBParameter("@PartCode", DbType.String, txtPartCode.Text.Trim());
        }
        if (ultLocation.SelectedRow != null)
        {
          inputParam[6] = new DBParameter("@LocationPid", DbType.Int64, DBConvert.ParseLong(ultLocation.Value.ToString()));
        }
        inputParam[7] = new DBParameter("@RequestBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        if (chkRemain.Checked == true)
        {
          inputParam[8] = new DBParameter("@Remain", DbType.Int32, 1);
        }
        // Warehouse
        inputParam[9] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);
        dsBefore = DataBaseAccess.SearchStoreProcedure("spGNRGetRequiredMaterialsForRequestPart", 600, inputParam);
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0013", "At Least One Condition");
        return;
      }
      DataSet ds = new DataSet();
      ds.Tables.Add("Mas");
      ds.Tables.Add("Detail");
      if (dsBefore != null)
      {
        ds.Tables["Mas"].Merge(dsBefore.Tables[0]);
        ds.Tables["Detail"].Merge(dsBefore.Tables[1]);
      }

      ds.Relations.Add(new DataRelation("dsDetail_dsDescrip", new DataColumn[] {  ds.Tables[0].Columns["SupplementPid"],
                                                                                  ds.Tables[0].Columns["WorkOrder"],
                                                                                  ds.Tables[0].Columns["ItemCode"],
                                                                                  ds.Tables[0].Columns["Revision"] ,
                                                                                  ds.Tables[0].Columns["MaterialCode"]},
                                                              new DataColumn[] { ds.Tables[1].Columns["SupplementPid"],
                                                                                  ds.Tables[1].Columns["WorkOrder"],
                                                                                  ds.Tables[1].Columns["ItemCode"],
                                                                                  ds.Tables[1].Columns["Revision"] ,
                                                                                  ds.Tables[1].Columns["MaterialCode"] }, false));

      if (ds != null)
      {
        ultBefore.DataSource = ds;
        for (int i = 0; i < ultBefore.Rows.Count; i++)
        {
          UltraGridRow row = ultBefore.Rows[i];
          if (DBConvert.ParseLong(row.Cells["SupplementPid"].Value.ToString()) < 0)
          {
            row.CellAppearance.BackColor = Color.LightSalmon;
          }
          else
          {
            row.CellAppearance.BackColor = Color.LightGreen;
          }
          for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow rowchild = row.ChildBands[0].Rows[j];
            if (DBConvert.ParseDouble(rowchild.Cells["Remain"].Value.ToString()) <= 0)
            {
              rowchild.Cells["Auto"].Activation = Activation.ActivateOnly;
            }
          }
        }
      }
    }

    /// <summary>
    /// Add Material Requisition
    /// </summary>
    private bool AddMaterialRequisition()
    {
      bool success = true;
      status = 0;
      success = this.AddMaterialRequisitionInfo(status);
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
      DBParameter[] inputParam = new DBParameter[12];
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
      inputParam[11] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);
      DBParameter[] outputParam = new DBParameter[2];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      outputParam[1] = new DBParameter("@MaterialRequisitionPid", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialRequisitionNote_EditForPart", inputParam, outputParam);
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
      DataSet ds = (DataSet)ultBefore.DataSource;
      DataTable dt = ds.Tables[1];
      DataTable dtSource = new DataTable();
      dtSource.Columns.Add("SupplementPid", typeof(System.Int64));
      dtSource.Columns.Add("PartPid", typeof(System.Int64));
      dtSource.Columns.Add("MaterialCode", typeof(System.String));
      dtSource.Columns.Add("Qty", typeof(System.Double));
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow row = dt.Rows[i];
        if (DBConvert.ParseInt(row["Auto"].ToString()) == 1 && DBConvert.ParseDouble(row["Requesting"].ToString()) > 0)
        {
          DataRow rowinsert = dtSource.NewRow();
          if (DBConvert.ParseLong(row["SupplementPid"].ToString()) != long.MinValue)
          {
            rowinsert["SupplementPid"] = DBConvert.ParseLong(row["SupplementPid"].ToString());
          }
          if (DBConvert.ParseLong(row["PartPid"].ToString()) != long.MinValue)
          {
            rowinsert["PartPid"] = DBConvert.ParseLong(row["PartPid"].ToString());
          }
          if (row["MaterialCode"].ToString().Length > 0)
          {
            rowinsert["MaterialCode"] = row["MaterialCode"].ToString();
          }
          if (DBConvert.ParseDouble(row["Requesting"].ToString()) > 0)
          {
            rowinsert["Qty"] = DBConvert.ParseDouble(row["Requesting"].ToString());
          }
          dtSource.Rows.Add(rowinsert);
        }
      }
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        SqlDBParameter[] input = new SqlDBParameter[2];
        input[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.MaterialRequisitionPid);
        input[1] = new SqlDBParameter("@DataSource", SqlDbType.Structured, dtSource);
        SqlDBParameter[] output = new SqlDBParameter[1];
        output[0] = new SqlDBParameter("@Result", SqlDbType.Int, int.MinValue);
        SqlDataBaseAccess.ExecuteStoreProcedure("spCARRequestMaterial_AddOnERP", input, output);
        long success = DBConvert.ParseLong(output[0].Value.ToString());
        if (success <= 0)
        {
          return false;
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
      for (int i = 0; i < ultBefore.Rows.Count; i++)
      {
        UltraGridRow row = ultBefore.Rows[i];
        row.Cells["TotalRequesting"].Appearance.BackColor = Color.LightSalmon;
        long supp = DBConvert.ParseLong(row.Cells["SupplementPid"].Value.ToString());
        double totalrequest = DBConvert.ParseDouble(row.Cells["TotalRequesting"].Value.ToString());
        if (totalrequest > 0)
        {
          double totalremain = DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString());
          if (supp > 0)
          {
            if (totalrequest > totalremain)
            {
              message = "Total Request <= Total Remain";
              row.Cells["TotalRequesting"].Appearance.BackColor = Color.Yellow;
              return false;
            }
          }
          else
          {
            double remainalllo = DBConvert.ParseDouble(row.Cells["RemainAllocate"].Value.ToString());
            if (remainalllo >= 0)
            {
              double remain = double.MinValue;
              if (totalremain <= remainalllo)
              {
                remain = totalremain;
              }
              else
              {
                remain = remainalllo;
              }
              if (totalrequest > remain)
              {
                message = "Total Request <= Total Remain";
                row.Cells["TotalRequesting"].Appearance.BackColor = Color.Yellow;
                return false;
              }
            }
          }
        }
        for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowchild = row.ChildBands[0].Rows[j];
          rowchild.Cells["Requesting"].Appearance.BackColor = Color.LightGray;
          if (DBConvert.ParseInt(rowchild.Cells["Auto"].Value.ToString()) == 1)
          {
            if (DBConvert.ParseDouble(rowchild.Cells["Requesting"].Value.ToString()) > DBConvert.ParseDouble(rowchild.Cells["Remain"].Value.ToString()))
            {
              rowchild.Cells["Requesting"].Appearance.BackColor = Color.Yellow;
              message = "Requesting <= Remain";
              return false;
            }
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
      //Shared.Utility.Utility.SetPropertiesUltraGrid(ultAfter);
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialNameVn"].Header.Caption = "MaterialName(vn)";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "MaterialName(En)";
      e.Layout.Bands[0].Columns["MRNPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 200;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 100;

      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[1].Columns["SupplementNo"].Header.Caption = "Supplement No";
      e.Layout.Bands[1].Columns["Qty"].Header.Caption = "Require";
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["MRNDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["SupplementPid"].Hidden = true;
      e.Layout.Bands[1].Columns["PartPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      if (status == 1 || status == 2)
      {
        e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      }
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.AllowRowSummaries = AllowRowSummaries.True;
      e.Layout.Override.SummaryDisplayArea |= SummaryDisplayAreas.InGroupByRows;
      this.ultAfter.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;
      e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;
    }
    /// <summary>
    /// Init Layout Before
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultBefore_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      //Shared.Utility.Utility.SetPropertiesUltraGrid(ultBefore);

      e.Layout.Bands[0].Columns["SupplementPid"].Hidden = true;
      e.Layout.Bands[1].Columns["SupplementPid"].Hidden = true;
      e.Layout.Bands[1].Columns["PartPid"].Hidden = true;
      e.Layout.Bands[1].Columns["WorkOrder"].Hidden = true;
      e.Layout.Bands[1].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[1].Columns["Revision"].Hidden = true;
      e.Layout.Bands[1].Columns["QtyRequested"].Header.Caption = "Requested";
      e.Layout.Bands[1].Columns["QtyIssued"].Header.Caption = "Issued";
      e.Layout.Bands[0].Columns["Require"].Header.Caption = "Total Require";
      e.Layout.Bands[0].Columns["QtyRequested"].Header.Caption = "Total Requested";
      e.Layout.Bands[0].Columns["QtyIssued"].Header.Caption = "Total Issued";

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Auto"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["Auto"].CellAppearance.BackColor = Color.LightGray;
      }

      for (int j = 0; j < e.Layout.Bands[1].Columns.Count; j++)
      {
        e.Layout.Bands[1].Columns[j].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[1].Columns["Requesting"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[1].Columns["Requesting"].CellAppearance.BackColor = Color.LightGray;
        e.Layout.Bands[1].Columns["Auto"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[1].Columns["Auto"].CellAppearance.BackColor = Color.LightGray;
      }

      e.Layout.Bands[0].Columns["WorkOrder"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["WorkOrder"].MinWidth = 70;

      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;

      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 30;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 30;

      e.Layout.Bands[1].Columns["MaterialCode"].MaxWidth = 150;
      e.Layout.Bands[1].Columns["MaterialCode"].MinWidth = 150;

      e.Layout.Bands[0].Columns["MaterialNameEn"].MaxWidth = 180;
      e.Layout.Bands[0].Columns["MaterialNameEn"].MinWidth = 180;

      e.Layout.Bands[0].Columns["MaterialNameVn"].MaxWidth = 180;
      e.Layout.Bands[0].Columns["MaterialNameVn"].MinWidth = 180;

      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 50;

      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Remain"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[1].Columns["Require"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["QtyRequested"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["QtyIssued"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Stock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["Auto"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["Auto"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
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
    /// Search Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      this.SetStatusControl();
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Add Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      if (ultBefore.Rows.Count > 0)
      {
        DataSet ds = (DataSet)ultBefore.DataSource;
        DataTable dt1 = ds.Tables[0];
        DataTable dt2 = ds.Tables[1];
        DataRow[] found1 = dt1.Select(string.Format("Auto = {0}", 1));
        DataRow[] found2 = dt2.Select(string.Format("Auto = {0}", 1));
        if (found1.Length > 0 || found2.Length > 0)
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
        else
        {
          WindowUtinity.ShowMessageError("ERR0115", "Part code");
          return;
        }
      }
      else
      {
        MessageBox.Show("No data on gird");
        return;
      }
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
      //this.SendEmail();
    }

    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("WorkOrder", typeof(System.Int64));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("MaterialCode", typeof(System.String));
      dt.Columns.Add("Remain", typeof(System.Double));
      dt.Columns.Add("Requesting", typeof(System.Double));
      return dt;
    }


    private void ultBefore_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      DataTable dtDetail = ((DataSet)ultBefore.DataSource).Tables[1];
      switch (columnName)
      {
        case "Requesting":
          DataTable dt = this.CreateDataTable();
          double total = 0;
          string item = e.Cell.Row.Cells["ItemCode"].Value.ToString();
          int revision = DBConvert.ParseInt(e.Cell.Row.Cells["Revision"].Value.ToString());
          long WO = DBConvert.ParseLong(e.Cell.Row.Cells["WorkOrder"].Value.ToString());
          string material = e.Cell.Row.Cells["MaterialCode"].Value.ToString();
          DataRow[] foundRows = dtDetail.Select(string.Format("WorkOrder = {0} AND ItemCode = '{1}' AND Revision = {2} AND MaterialCode = '{3}'", WO, item, revision, material));
          if (foundRows.Length > 0)
          {
            for (int i = 0; i < foundRows.Length; i++)
            {
              DataRow row = dt.NewRow();
              row[0] = foundRows[i].ItemArray[3];
              row[1] = foundRows[i].ItemArray[4].ToString();
              row[2] = foundRows[i].ItemArray[5];
              row[3] = foundRows[i].ItemArray[6].ToString();
              row[4] = foundRows[i].ItemArray[11];
              row[5] = foundRows[i].ItemArray[12];
              dt.Rows.Add(row);
            }
          }
          for (int i = 0; i < dt.Rows.Count; i++)
          {
            double rq = 0;
            if (DBConvert.ParseDouble(dt.Rows[i]["Requesting"].ToString()) <= 0)
            {
              rq = 0;
            }
            else
            {
              rq = DBConvert.ParseDouble(dt.Rows[i]["Requesting"].ToString());

            }
            total += rq;
          }
          e.Cell.Row.ParentRow.Cells["TotalRequesting"].Value = total;
          if (DBConvert.ParseDouble(e.Cell.Row.Cells["Requesting"].Value.ToString()) > 0)
          {
            e.Cell.Row.Cells["Auto"].Value = 1;
          }
          break;
        default:
          break;
      }
    }

    private void ultBefore_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string value = e.NewValue.ToString();
      string text = e.Cell.Text.Trim();

      switch (columnName)
      {
        case "Requesting":
          if (e.Cell.Row.ParentRow != null)
          {
            double remain3 = DBConvert.ParseDouble(e.Cell.Row.ParentRow.Cells["Remain"].Value.ToString());
            double total1 = DBConvert.ParseDouble(e.Cell.Row.ParentRow.Cells["TotalRequesting"].Value.ToString());
            double remainallocate1 = (DBConvert.ParseDouble(e.Cell.Row.ParentRow.Cells["RemainAllocate"].Value.ToString()) < 0 ? 0 : DBConvert.ParseDouble(e.Cell.Row.ParentRow.Cells["RemainAllocate"].Value.ToString()));
            double remain4 = double.MinValue;
            double request = DBConvert.ParseDouble(e.Cell.Row.Cells["Requesting"].Value.ToString());
            double remain = DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString());
            if (DBConvert.ParseLong(e.Cell.Row.Cells["SupplementPid"].Value.ToString()) == long.MinValue)
            {
              if (remain3 > remainallocate1)
              {
                remain4 = remainallocate1;
              }
              else
              {
                remain4 = remain3;
              }
            }
            else
            {
              remain4 = remain3;
            }
            e.Cell.Row.Cells["Requesting"].Appearance.BackColor = Color.LightGray;
            if (DBConvert.ParseDouble(value) > remain)
            {
              e.Cell.Row.Cells["Requesting"].Appearance.BackColor = Color.Yellow;
              WindowUtinity.ShowMessageError("WRN0003", "Requesting", "Remain");
              e.Cancel = true;
              break;
            }
            if (Math.Round((total1 + DBConvert.ParseDouble(value)), 3) > Math.Round(remain4, 3))
            {
              //e.Cell.Row.Cells["Requesting"].Value = newvalue;
              e.Cell.Row.Cells["Requesting"].Appearance.BackColor = Color.Yellow;
              WindowUtinity.ShowMessageError("WRN0003", "Requesting", "Remain");
              e.Cancel = true;
              break;
            }
          }
          break;
        default:
          break;
      }
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
      int pa = int.MinValue;
      double remain = double.MinValue;
      double totalremain = double.MinValue;
      double totalallocate = double.MinValue;
      double totalrequesting = 0;
      try
      {
        pa = DBConvert.ParseInt(e.Cell.Row.ParentRow.Index.ToString());
      }
      catch
      {

      }
      switch (strColName)
      {

        case "auto":
          if (DBConvert.ParseInt(e.Cell.Row.Cells["Auto"].Text) == 1)
          {
            if (pa != int.MinValue)
            {
              totalremain = DBConvert.ParseDouble(e.Cell.Row.ParentRow.Cells["Remain"].Value.ToString());
              totalallocate = DBConvert.ParseDouble(e.Cell.Row.ParentRow.Cells["RemainAllocate"].Value.ToString());
              totalrequesting = DBConvert.ParseDouble(e.Cell.Row.ParentRow.Cells["TotalRequesting"].Value.ToString());
              if (DBConvert.ParseLong(e.Cell.Row.Cells["SupplementPid"].Value.ToString()) == long.MinValue)
              {
                if (totalremain > totalallocate)
                {
                  remain = totalallocate;
                }
                else
                {
                  remain = totalremain;
                }
              }
              else
              {
                remain = totalremain;
              }
              if (totalrequesting < remain)
              {
                if (DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString()) > 0)
                {
                  //e.Cell.Row.Cells["Require"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString());
                  totalrequesting = totalrequesting + DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString());
                  if (totalrequesting > remain)
                  {
                    e.Cell.Row.Cells["Requesting"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString()) - (totalrequesting - remain);
                    e.Cell.Row.ParentRow.Cells["TotalRequesting"].Value = totalrequesting - (totalrequesting - remain);
                  }
                  else
                  {
                    e.Cell.Row.Cells["Requesting"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString());
                    e.Cell.Row.ParentRow.Cells["TotalRequesting"].Value = totalrequesting;
                  }

                }
              }
              else
              {
                break;
              }

            }
            else
            {
              totalremain = DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString());
              totalallocate = DBConvert.ParseDouble(e.Cell.Row.Cells["RemainAllocate"].Value.ToString());
              totalrequesting = DBConvert.ParseDouble(e.Cell.Row.Cells["TotalRequesting"].Value.ToString());
              if (DBConvert.ParseLong(e.Cell.Row.Cells["SupplementPid"].Value.ToString()) == long.MinValue)
              {
                if (totalremain > totalallocate)
                {
                  remain = totalallocate;
                }
                else
                {
                  remain = totalremain;
                }
              }
              else
              {
                remain = totalremain;
              }
              for (int i = 0; i < e.Cell.Row.ChildBands[0].Rows.Count; i++)
              {

                if (totalrequesting < remain)
                {
                  if (DBConvert.ParseDouble(e.Cell.Row.ChildBands[0].Rows[i].Cells["Remain"].Value.ToString()) > 0)
                  {
                    totalrequesting = totalrequesting + DBConvert.ParseDouble(e.Cell.Row.ChildBands[0].Rows[i].Cells["Remain"].Value.ToString());
                    if (totalrequesting > remain)
                    {
                      e.Cell.Row.ChildBands[0].Rows[i].Cells["Requesting"].Value = DBConvert.ParseDouble(e.Cell.Row.ChildBands[0].Rows[i].Cells["Remain"].Value.ToString()) - (totalrequesting - remain);
                      e.Cell.Row.Cells["TotalRequesting"].Value = totalrequesting - (totalrequesting - remain);
                    }
                    else
                    {
                      e.Cell.Row.ChildBands[0].Rows[i].Cells["Requesting"].Value = DBConvert.ParseDouble(e.Cell.Row.ChildBands[0].Rows[i].Cells["Remain"].Value.ToString());
                      e.Cell.Row.Cells["TotalRequesting"].Value = totalrequesting;
                    }

                    if (DBConvert.ParseDouble(e.Cell.Row.Cells["TotalRequesting"].Value.ToString()) > remain)
                    {
                      break;
                    }
                    e.Cell.Row.ChildBands[0].Rows[i].Cells["Auto"].Value = 1;
                  }
                }
              }

            }
          }
          else
          {
            if (pa != int.MinValue)
            {
              if (DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString()) > 0)
              {
                e.Cell.Row.ParentRow.Cells["TotalRequesting"].Value = DBConvert.ParseDouble(e.Cell.Row.ParentRow.Cells["TotalRequesting"].Value.ToString()) - DBConvert.ParseDouble(e.Cell.Row.Cells["Requesting"].Value.ToString());
                e.Cell.Row.Cells["Requesting"].Value = 0;
                e.Cell.Row.ParentRow.Cells["Auto"].Value = 0;
              }

            }
            else
            {
              for (int i = 0; i < e.Cell.Row.ChildBands[0].Rows.Count; i++)
              {
                e.Cell.Row.ChildBands[0].Rows[i].Cells["Requesting"].Value = 0;
                e.Cell.Row.ChildBands[0].Rows[i].Cells["Auto"].Value = 0;
              }
              e.Cell.Row.Cells["TotalRequesting"].Value = 0;
            }
          }
          break;

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
      ultDeliveryDate.Value = deliveryDate;
      ultcbHour.Value = deliveryHour;
      ultcbMinute.Value = deliveryMinute;
      if (ultcbUrgent.Value != null)
      {
        if (DBConvert.ParseInt(ultcbUrgent.Value.ToString()) == 1)
        {
          ultcbHour.ReadOnly = false;
          ultcbMinute.ReadOnly = false;
          ultDeliveryDate.ReadOnly = false;
        }
        else
        {
          ultcbHour.ReadOnly = true;
          ultcbMinute.ReadOnly = true;
          ultDeliveryDate.ReadOnly = true;
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
          long pidParent = long.MinValue;
          long pidChild = long.MinValue;
          try
          {
            pidParent = DBConvert.ParseLong(row.Cells["MRNPid"].Value.ToString());
          }
          catch
          {
            pidChild = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          }
          if (pidParent > 0 || pidChild > 0)
          {
            DBParameter[] inputParams = new DBParameter[2];
            if (pidParent > 0)
            {
              inputParams[0] = new DBParameter("@PidParent", DbType.Int64, pidParent);
            }
            if (pidChild > 0)
            {
              inputParams[1] = new DBParameter("@PidChild", DbType.Int64, pidChild);
            }
            DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialRequisitionNoteDetail_DeleteForPart", inputParams, outputParams);
            if (DBConvert.ParseInt(outputParams[0].Value.ToString()) != 1)
            {
              WindowUtinity.ShowMessageError("ERR0004");
              this.LoadData();
              return;
            }
          }
        }
      }
      this.LoadData();
      this.Search();
      //chkExpandAll.Checked = false;
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
      if (selected == 1)
      {
        for (int i = 0; i < ultBefore.Rows.Count; i++)
        {
          double remain = double.MinValue;
          if (DBConvert.ParseLong(ultBefore.Rows[i].Cells["SupplementPid"].Value.ToString()) == long.MinValue)
          {
            if (DBConvert.ParseDouble(ultBefore.Rows[i].Cells["Remain"].Value.ToString()) > DBConvert.ParseDouble(ultBefore.Rows[i].Cells["RemainAllocate"].Value.ToString()))
            {
              remain = DBConvert.ParseDouble(ultBefore.Rows[i].Cells["RemainAllocate"].Value.ToString());
            }
            else
            {
              remain = DBConvert.ParseDouble(ultBefore.Rows[i].Cells["Remain"].Value.ToString());
            }
          }
          else
          {
            remain = DBConvert.ParseDouble(ultBefore.Rows[i].Cells["Remain"].Value.ToString());
          }
          if (remain > 0)
          {
            double totalrequesting = DBConvert.ParseDouble(ultBefore.Rows[i].Cells["TotalRequesting"].Value.ToString());
            ///
            for (int j = 0; j < ultBefore.Rows[i].ChildBands[0].Rows.Count; j++)
            {

              if (totalrequesting < remain)
              {
                if (DBConvert.ParseDouble(ultBefore.Rows[i].ChildBands[0].Rows[j].Cells["Remain"].Value.ToString()) > 0)
                {
                  totalrequesting = totalrequesting + DBConvert.ParseDouble(ultBefore.Rows[i].ChildBands[0].Rows[j].Cells["Remain"].Value.ToString());
                  if (totalrequesting > remain)
                  {
                    ultBefore.Rows[i].ChildBands[0].Rows[j].Cells["Requesting"].Value = DBConvert.ParseDouble(ultBefore.Rows[i].ChildBands[0].Rows[j].Cells["Remain"].Value.ToString()) - (totalrequesting - remain);
                    ultBefore.Rows[i].Cells["TotalRequesting"].Value = totalrequesting - (totalrequesting - remain);
                  }
                  else
                  {
                    ultBefore.Rows[i].ChildBands[0].Rows[j].Cells["Requesting"].Value = DBConvert.ParseDouble(ultBefore.Rows[i].ChildBands[0].Rows[j].Cells["Remain"].Value.ToString());
                    ultBefore.Rows[i].Cells["TotalRequesting"].Value = totalrequesting;
                  }

                  if (DBConvert.ParseDouble(ultBefore.Rows[i].Cells["TotalRequesting"].Value.ToString()) > remain)
                  {
                    break;
                  }
                  ultBefore.Rows[i].ChildBands[0].Rows[j].Cells["Auto"].Value = 1;
                }
              }
            }
            ultBefore.Rows[i].Cells["Auto"].Value = selected;
          }
        }
      }
      else
      {
        for (int i = 0; i < ultBefore.Rows.Count; i++)
        {
          for (int j = 0; j < ultBefore.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            ultBefore.Rows[i].ChildBands[0].Rows[j].Cells["Requesting"].Value = 0;
            ultBefore.Rows[i].ChildBands[0].Rows[j].Cells["Auto"].Value = 0;
          }
          ultBefore.Rows[i].Cells["TotalRequesting"].Value = 0;
          ultBefore.Rows[i].Cells["Auto"].Value = 0;

        }
      }
      //else
      //{
      //  for (int i = 0; i < ultBefore.Rows.Count; i++)
      //  {
      //    for (int j = 0; j < ultBefore.Rows[i].ChildBands[0].Rows.Count; j++)
      //    {
      //      ultBefore.Rows[i].ChildBands[0].Rows[j].Cells["Requesting"].Value = DBConvert.ParseDouble(ultBefore.Rows[i].ChildBands[0].Rows[j].Cells["Remain"].Value.ToString());
      //    }
      //  }
      //}
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

    private void chkBeforeExpand_CheckedChanged(object sender, EventArgs e)
    {
      if (chkBeforeExpand.Checked)
      {
        ultBefore.Rows.ExpandAll(true);
      }
      else
      {
        ultBefore.Rows.CollapseAll(true);
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultAfter, "Data");
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
    #endregion Event
  }
}