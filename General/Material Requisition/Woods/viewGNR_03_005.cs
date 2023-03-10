/*
  Author      : 
  Date        : 06-04-2013
  Description : Request Online Special ID
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
  public partial class viewGNR_03_005 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private DateTime deliveryDate = DateTime.MinValue;
    private int deliveryHour = int.MinValue;
    private int deliveryMinute = int.MinValue;
    #endregion Field

    #region Init

    public viewGNR_03_005()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_03_005_Load(object sender, EventArgs e)
    {
      this.LoadInit();
      this.LoadData();
    }

    /// <summary>
    /// Load Init
    /// </summary>
    private void LoadInit()
    {
      this.LoadUrgentLevel();
      this.LoadDiliveryDate();
      this.LoadHour();
      this.LoadMinute();
      this.LoadWO();
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
        this.ultCBUrgent.Value = dtSource.Rows[2][0].ToString();
      }
      else
      {
        return;
      }
      ultCBUrgent.DataSource = dtSource;
      ultCBUrgent.DisplayMember = "Value";
      ultCBUrgent.ValueMember = "Code";
      ultCBUrgent.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBUrgent.DisplayLayout.Bands[0].Columns["Value"].Width = 120;
      ultCBUrgent.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load Day
    /// </summary>
    private void LoadDiliveryDate()
    {
      string commandText = "SELECT CONVERT(varchar, GETDATE(), 103) [Day], DATEPART(HH, GETDATE()) [Hour], DATEPART(MI, GETDATE()) [Minute]";
      DataTable dtHour = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DateTime day = DBConvert.ParseDateTime(dtHour.Rows[0]["Day"].ToString(), Shared.Utility.ConstantClass.FORMAT_DATETIME);
      ultDTDeliveryDate.Value = day;
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
      ultCBHour.DataSource = dt;
      ultCBHour.DisplayMember = "Hour";
      ultCBHour.ValueMember = "Hour";
      ultCBHour.DisplayLayout.Bands[0].ColHeadersVisible = false;
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
      ultCBMinute.DataSource = dt;
      ultCBMinute.DisplayMember = "Minute";
      ultCBMinute.ValueMember = "Minute";
      ultCBMinute.DisplayLayout.Bands[0].ColHeadersVisible = false;
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
        if (ultCBUrgent.Value != null)
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
            deliveryDate = DBConvert.ParseDateTime(ultDTDeliveryDate.Value.ToString(), Shared.Utility.ConstantClass.FORMAT_DATETIME);
            deliveryHour = DBConvert.ParseInt(ultCBHour.Value.ToString());
            if (ultCBMinute.Value != null)
            {
              deliveryMinute = DBConvert.ParseInt(ultCBMinute.Value.ToString());
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
    /// Load WO
    /// </summary>
    private void LoadWO()
    {
      string commandText = "SELECT Pid FROM TblPLNWorkOrder WHERE Confirm = 1 and Status = 0 ORDER BY Pid DESC";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if(dt == null)
      {
        return;
      }
      ultCBWO.DataSource = dt;
      ultCBWO.DisplayMember = "Pid";
      ultCBWO.ValueMember = "Pid";
      ultCBWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Carcass
    /// </summary>
    /// <param name="wo"></param>
    private void LoadCarcass(int wo)
    {
      string commandText = string.Format(@"SELECT Distinct CarcassCode
                           FROM VPLNWorkOrderCarcassList  info
                           WHERE info.WoPid = {0}", wo);
      DataTable dt = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt == null)
      {
        return;
      }

      ultCBCarcass.DataSource = dt;
      ultCBCarcass.ValueMember = "CarcassCode";
      ultCBCarcass.DisplayMember = "CarcassCode";
      ultCBCarcass.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    #endregion Init

    #region Function

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.pid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spGNRWoodsRequisitionSpecialID_Select", inputParam);

      // 1: Info
      DataTable dtInfo = dsSource.Tables[0];
      if (dtInfo != null && dtInfo.Rows.Count > 0)
      {
        DataRow row = dtInfo.Rows[0];
        txtWRNCode.Text = row["Code"].ToString();
        txtRequestBy.Text = row["CreateBy"].ToString();
        txtDepartment.Text = row["DepartmentRequest"].ToString();
        ultCBUrgent.Value = row["Urgent"].ToString();
        txtRemark.Text = row["remark"].ToString();
        ultCBHour.Value = row["DeliveryHour"].ToString();
        ultCBMinute.Value = row["DeliveryMinute"].ToString();
        ultDTDeliveryDate.Value = DBConvert.ParseDateTime(row["DeliveryDate"].ToString(), Shared.Utility.ConstantClass.FORMAT_DATETIME);
        if (DBConvert.ParseInt(row["Status"].ToString()) >= 1)
        {
          chkConfirm.Checked = true;
        }
        else
        {
          chkConfirm.Checked = false;
        }
      }
      else
      {
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FGNRWoodsGetNewRequisitionSpecialNo('RWP') NewCode");
        if ((dt != null) && (dt.Rows.Count > 0))
        {
          txtWRNCode.Text = dt.Rows[0]["NewCode"].ToString();
          txtRequestBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          txtDepartment.Text = SharedObject.UserInfo.Department;
        }
        this.SetDeliveryTime();
        ultDTDeliveryDate.Value = deliveryDate;
        ultCBHour.Value = deliveryHour;
        ultCBMinute.Value = deliveryMinute;
      }

      // 2: Load Detail
      DataSet ds = this.CreateDataSetData();
      ds.Tables["dtParent"].Merge(dsSource.Tables[1]);
      ds.Tables["dtChild"].Merge(dsSource.Tables[2]);

      this.ultData.DataSource = null;

      ultData.DataSource = ds;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        row.Appearance.BackColor = Color.Wheat;
      }

      // 3: Set Status Control
      this.SetStatusControl();
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      if(chkConfirm.Checked)
      {
        ultCBUrgent.ReadOnly = true;
        chkDeliveryTime.Enabled = false;
        ultDTDeliveryDate.ReadOnly = true;
        ultCBHour.ReadOnly = true;
        ultCBMinute.ReadOnly = true;
        txtRemark.ReadOnly = true;
        chkExpand.Checked = true;
        btnAdd.Enabled = false;
        chkHide.Checked = true;
        chkConfirm.Enabled = false;
        btnSave.Enabled = false;
        btnDelete.Enabled = false;
        // Detail
        ultData.Rows.Band.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      }
    }

    /// <summary>
    /// Creata Dataset
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSetSearch()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("WO", typeof(System.Int32));
      taParent.Columns.Add("Carcass", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("MaterialNameEn", typeof(System.String));
      taParent.Columns.Add("MaterialNameVn", typeof(System.String));
      taParent.Columns.Add("Unit", typeof(System.String));
      taParent.Columns.Add("Balance", typeof(System.Int32));
      taParent.Columns.Add("Required", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("WO", typeof(System.Int32));
      taChild.Columns.Add("Carcass", typeof(System.String));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("LotNoId", typeof(System.String));
      taChild.Columns.Add("DimensionPid", typeof(System.Int64));
      taChild.Columns.Add("Length", typeof(System.Double));
      taChild.Columns.Add("Width", typeof(System.Double));
      taChild.Columns.Add("Thickness", typeof(System.Double));
      taChild.Columns.Add("Package", typeof(System.String));
      taChild.Columns.Add("Locaion", typeof(System.String));
     
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("taParent_taChild", new DataColumn[] { taParent.Columns["WO"], taParent.Columns["Carcass"], taParent.Columns["MaterialCode"] }, new DataColumn[] { taChild.Columns["WO"], taChild.Columns["Carcass"], taChild.Columns["MaterialCode"] }));
      return ds;
    }

    /// <summary>
    /// Creata Dataset Data
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSetData()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("WO", typeof(System.Int32));
      taParent.Columns.Add("Carcass", typeof(System.String));
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("MaterialNameEn", typeof(System.String));
      taParent.Columns.Add("MaterialNameVn", typeof(System.String));
      taParent.Columns.Add("Unit", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("WO", typeof(System.Int32));
      taChild.Columns.Add("Carcass", typeof(System.String));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("LotNoId", typeof(System.String));
      taChild.Columns.Add("Length", typeof(System.Double));
      taChild.Columns.Add("Width", typeof(System.Double));
      taChild.Columns.Add("Thickness", typeof(System.Double));
      taChild.Columns.Add("Package", typeof(System.String));
      taChild.Columns.Add("Location", typeof(System.String));
      taChild.Columns.Add("Issue", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("taParent_taChild", new DataColumn[] { taParent.Columns["WO"], taParent.Columns["Carcass"], taParent.Columns["MaterialCode"] }, new DataColumn[] { taChild.Columns["WO"], taChild.Columns["Carcass"], taChild.Columns["MaterialCode"] }));
      return ds;
    }

    /// <summary>
    /// Search Info
    /// </summary>
    private void Search()
    {
      if (this.txtMaterial.Text.Trim().Length > 0 || ultCBWO.Value != null || ultCBCarcass.Value != null)
      {
        DBParameter[] input = new DBParameter[3];
        if (txtMaterial.Text.Trim().Length > 0)
        {
          input[0] = new DBParameter("@Material", DbType.String, txtMaterial.Text);
        }
        if (ultCBWO.Value != null)
        {
          input[1] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(ultCBWO.Value.ToString()));
        }
        if (ultCBCarcass.Value != null)
        {
          input[2] = new DBParameter("@Carcass", DbType.String, ultCBCarcass.Value.ToString());
        }

        DataSet ds = DataBaseAccess.SearchStoreProcedure("spGNRWoodsLoadInfoIDWoodsSpecial_Select", input);
        if (ds != null)
        {
          DataSet dsSource = this.CreateDataSetSearch();
          dsSource.Tables["dtParent"].Merge(ds.Tables[0]);
          dsSource.Tables["dtChild"].Merge(ds.Tables[1]);
          ultSearch.DataSource = dsSource;
          for (int i = 0; i < ultSearch.Rows.Count; i++)
          {
            UltraGridRow row = ultSearch.Rows[i];
            row.Appearance.BackColor = Color.Wheat;
          }
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0013", "At Least One Condition");
        return;
      }
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
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// Check Data
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      if (ultCBUrgent.Value == null)
      {
        message = "Urgent";
        return false;
      }

      // Delivery Date
      if (ultDTDeliveryDate.Value == null)
      {
        message = "DeliveryDate >= Today";
        return false;
      }
      if (DBConvert.ParseDateTime(ultDTDeliveryDate.Value) < DateTime.Today)
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
      if (ultCBHour.Value == null)
      {
        message = "Hour";
        return false;
      }
      // Minute
      if(ultCBMinute.Value == null)
      {
        message = "Minute";
        return false;
      }
      // Delivery Date
      if (DBConvert.ParseDateTime(ultDTDeliveryDate.Value) < DateTime.Today)
      {
        message = "Delivery Date >= Today";
        return false;
      }
      else
      {
        if (DBConvert.ParseInt(ultCBHour.Value.ToString()) < hour)
        {
          message = "Delivery Time >=" + hour + "h" + minute;
          return false;
        }
        else if (DBConvert.ParseInt(ultCBHour.Value.ToString()) == hour)
        {
          if (DBConvert.ParseInt(ultCBMinute.Value.ToString()) < minute)
          {
            message = "Delivery Time >=" + hour + "h" + minute;
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Save Master
    /// </summary>
    /// <returns></returns>
    private bool SaveMaster()
    {
      DBParameter[] inputParam = new DBParameter[11];
      if (this.pid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      }
      inputParam[1] = new DBParameter("@Code", DbType.AnsiString, 8, "RWP");
      if (chkConfirm.Checked)
      {
        inputParam[2] = new DBParameter("@Status", DbType.Int32, 1);
      }
      else
      {
        inputParam[2] = new DBParameter("@Status", DbType.Int32, 0);
      }
      inputParam[3] = new DBParameter("@DepartmentRequest", DbType.AnsiString, 8, SharedObject.UserInfo.Department);
      if (txtRemark.Text.Trim().Length > 0)
      {
        inputParam[4] = new DBParameter("@Remark", DbType.String, 1024, txtRemark.Text);
      }
      inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[6] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[7] = new DBParameter("@Urgent", DbType.Int32, DBConvert.ParseInt(ultCBUrgent.Value.ToString()));
      if (ultDTDeliveryDate.Value != null)
      {
        inputParam[8] = new DBParameter("@DeliveryDate", DbType.DateTime, ultDTDeliveryDate.Value);
      }
      if (ultCBHour.Value != null)
      {
        inputParam[9] = new DBParameter("@DeliveryHour", DbType.Int32, DBConvert.ParseInt(ultCBHour.Value.ToString()));
      }
      if (ultCBMinute.Value != null)
      {
        inputParam[10] = new DBParameter("@DeliveryMinute", DbType.Int32, DBConvert.ParseInt(ultCBMinute.Value.ToString()));
      }

      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spGNRWoodsRequisitionSpecialID_Edit", inputParam, outputParam);
      long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      // Gan Lai Pid
      this.pid = result;

      if (result <= 0)
      {
        return false;
      }    
      return true;
    }

    /// <summary>
    /// Save Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      for (int i = 0; i < ultSearch.Rows.Count; i++)
      {
        UltraGridRow row = ultSearch.Rows[i];
        for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = row.ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(rowChild.Cells["Select"].Value.ToString()) == 1)
          {
            DBParameter[] input = new DBParameter[9];
            input[0] = new DBParameter("@WRNPid", DbType.Int64, this.pid);
            input[1] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(rowChild.Cells["WO"].Value.ToString()));
            input[2] = new DBParameter("@Carcass", DbType.String, rowChild.Cells["Carcass"].Value.ToString());
            input[3] = new DBParameter("@LotNoId", DbType.String, rowChild.Cells["LotNoId"].Value.ToString());
            input[4] = new DBParameter("@MaterialCode", DbType.String, rowChild.Cells["MaterialCode"].Value.ToString());
            input[5] = new DBParameter("@DimesionPid", DbType.Int64, DBConvert.ParseLong(rowChild.Cells["DimensionPid"].Value.ToString()));
            input[6] = new DBParameter("@LocationPid", DbType.Int64, DBConvert.ParseLong(rowChild.Cells["LocationPid"].Value.ToString()));
            input[7] = new DBParameter("@PackagePid", DbType.Int64, DBConvert.ParseLong(rowChild.Cells["PackagePid"].Value.ToString()));
            input[8] = new DBParameter("@Issue", DbType.Int32, 0);

            DBParameter[] output = new DBParameter[1];
            output[0] = new   DBParameter("@Result", DbType.Int64, long.MinValue);

            DataBaseAccess.ExecuteStoreProcedure("spGNRWoodsRequisitionSpecialIDDetail_Insert", input, output);
          }
        }     
      }
      return true;
    }

    #endregion Function

    #region Event

    /// <summary>
    /// WO Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCBWO_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBWO.Value != null && ultCBWO.Text.ToString().Length > 0)
      {
        ultCBCarcass.Value = null;
        this.LoadCarcass(DBConvert.ParseInt(ultCBWO.Value.ToString()));
      }
      else
      {
        ultCBCarcass.Value = null;
      }
    }

    /// <summary>
    /// Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.btnSearch.Enabled = false;
      this.Search();
      this.btnSearch.Enabled = true;
    }

    /// <summary>
    /// Add Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // 1: Check Valid
      bool success = this.CheckValid(out message);
      if (success)
      {
        // 2: Save Master
        success = this.SaveMaster();
        if (success)
        {
          // 3: Save Detail
          success = this.SaveDetail();
          if (success)
          {
            WindowUtinity.ShowMessageSuccess("MSG0004");
          }
          else
          {
            WindowUtinity.ShowMessageError("ERR0037", "Data");
          }
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0037", "Data");
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // 4: Search Data
      this.Search();

      // 5: Load Data
      this.LoadData();
    }

    /// <summary>
    /// Save
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
      success =  this.SaveMaster();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      // 4: Load Data
      this.LoadData();
    }

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (this.pid == long.MinValue)
      {
        return;
      }
      if (WindowUtinity.ShowMessageConfirm("MSG0007", "Requisition Special ID").ToString() == "No")
      {
        return;
      }
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spGNRWoodsRequisitionSpecialID_Delete", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        WindowUtinity.ShowMessageError("ERR0093", "Requisition Special ID");
        return;
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0002");
      }
      this.btnDelete.Visible = false;
      this.CloseTab();
    }

    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
      this.Visible = false;
    }

    /// <summary>
    /// Check Delivery Time
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkDeliveryTime_CheckedChanged(object sender, EventArgs e)
    {
      if (chkDeliveryTime.Checked)
      {
        ultDTDeliveryDate.ReadOnly = false;
        ultCBHour.ReadOnly = false;
        ultCBMinute.ReadOnly = false;
      }
      else
      {
        ultDTDeliveryDate.ReadOnly = true;
        ultCBHour.ReadOnly = true;
        ultCBMinute.ReadOnly = true;
      }
    }

    /// <summary>
    /// Init Layout Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSearch_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["Balance"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Required"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Auto"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[1].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["DimensionPid"].Hidden = true;
      e.Layout.Bands[1].Columns["LocationPid"].Hidden = true;
      e.Layout.Bands[1].Columns["PackagePid"].Hidden = true;

      e.Layout.Bands[1].Columns["WO"].Hidden = true;
      e.Layout.Bands[1].Columns["Carcass"].Hidden = true;
      e.Layout.Bands[1].Columns["MaterialCode"].Hidden = true;
      e.Layout.Bands[1].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["Select"].DefaultCellValue = 0;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 2; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      for (int i = 0; i < e.Layout.Bands[1].Columns.Count - 1; i++)
      {
        e.Layout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
      }
     
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Bands[1].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init Layout Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["WO"].Hidden = true;
      e.Layout.Bands[1].Columns["Carcass"].Hidden = true;
      e.Layout.Bands[1].Columns["MaterialCode"].Hidden = true;
      e.Layout.Bands[1].Columns["Issue"].Header.Caption = "Status(Issue)";
      e.Layout.Bands[1].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[1].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Before Row Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      if (this.pid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) != long.MinValue)
          {
            // 1: Delete Row
            DBParameter[] inputParam = new DBParameter[1];
            inputParam[0] = new DBParameter("@DetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));

            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spGNRWoodsRequisitionSpecialIDDetail_Delete", inputParam, outputParam);
            long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
            if(result <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0004");
              this.LoadData();
              return;
            }
          }
        }
      }
      // 2: Search Data
      this.Search();
      // 3: Load Data
      this.LoadData();
    }


    /// <summary>
    /// Hide Check
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkHide_CheckedChanged(object sender, EventArgs e)
    {
      if (chkHide.Checked)
      {
        grpSearch.Visible = false;
        btnAdd.Visible = false;
      }
      else
      {
        grpSearch.Visible = true;
        btnAdd.Visible = true;
      }
    }

    /// <summary>
    /// Expand All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkExpand_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpand.Checked)
      {
        ultData.Rows.ExpandAll(true);
      }
      else
      {
        ultData.Rows.CollapseAll(true);
      }
    }

    /// <summary>
    /// Cell Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSearch_CellChange(object sender, CellEventArgs e)
    {
      string strColName = e.Cell.Column.ToString().ToLower();
      int index = e.Cell.Row.Index;
      switch (strColName)
      {
        case "auto":
          {
            if (DBConvert.ParseInt(e.Cell.Row.Cells["Auto"].Text.ToString()) == 1)
            {
              for (int i = 0; i < e.Cell.Row.ChildBands[0].Rows.Count; i++)
              {
                UltraGridRow row = e.Cell.Row.ChildBands[0].Rows[i];

                row.Cells["Select"].Value = 1;
              }
            }
            else
            {
              for (int i = 0; i < e.Cell.Row.ChildBands[0].Rows.Count; i++)
              {
                UltraGridRow row = e.Cell.Row.ChildBands[0].Rows[i];

                row.Cells["Select"].Value = 0;
              }
            }

            break;
          }
      }
    }
    #endregion Event  
  }
}
