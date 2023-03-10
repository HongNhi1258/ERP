/*
  Author        : 
  Create date   : 04/11/2013
  Decription    : Hold SaleOrder
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win.UltraWinGrid;
using System.Diagnostics;
using System.IO;
using System.Data.OleDb;
using Infragistics.Win;
using System.Collections;
using VBReport;
using DaiCo.CustomerService.DataSetSource;
using CrystalDecisions.CrystalReports.Engine;
using DaiCo.CustomerService.ReportTemplate;
using DaiCo.Application.Web.Mail;
using iTextSharp.text.pdf;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_012 : MainUserControl
  {
    #region Field
    private int status = int.MinValue;
    public long holdPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private string pathTemplate = string.Empty;
    private string pathExport = string.Empty;
    bool typeHold = true;
    #endregion Field

    #region Init
    public viewCSD_03_012()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load View
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_03_012_Load(object sender, EventArgs e)
    {
      this.LoadInit();
      this.LoadData();
    }

    /// <summary>
    /// Load Init
    /// </summary>
    private void LoadInit()
    {
      this.LoadUltraComboCustomer();
    }

    /// <summary>
    /// Load Customer
    /// </summary>
    private void LoadUltraComboCustomer()
    {
      string commandText = string.Empty;
      commandText = " SELECT Pid, CustomerCode, Name, CustomerCode + ' - ' + Name As [Description]";
      commandText += " FROM TblCSDCustomerInfo";
      commandText += " ORDER BY CustomerCode ASC";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBCustomer.DataSource = dtSource;
      ultCBCustomer.DisplayMember = "Description";
      ultCBCustomer.ValueMember = "Pid";
      ultCBCustomer.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultCBCustomer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCBCustomer.DisplayLayout.Bands[0].Columns["Description"].Hidden = true;
      ultCBCustomer.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[1] { new DBParameter("@Pid", DbType.Int64, this.holdPid) };
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spCSDSOHoldInfomation_Select", inputParam);
      if (ds != null && ds.Tables.Count > 1)
      {
        // LoadMain
        this.LoadMainData(ds.Tables[0]);

        // Load Detail
        ultData.DataSource = ds.Tables[1];

        // Check Hold
        if (this.status == int.MinValue)
        {
          radHold.Checked = true;
        }


        // Load Data Container
        if(this.status != 2 && radHold.Checked == true)
        {
          this.GetDataContainer();
        }
      }
      // Set Status Control
      this.SetStatusControl();
    }

    /// <summary>
    /// Load MainData
    /// </summary>
    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        ultCBCustomer.Value = DBConvert.ParseLong(dtMain.Rows[0]["CustomerPid"].ToString());
        txtHoldPONote.Text = dtMain.Rows[0]["HoldNo"].ToString();
        txtReason.Text = dtMain.Rows[0]["Reason"].ToString();
        txtRemark.Text = dtMain.Rows[0]["Remark"].ToString();
        txtCreateBy.Text = dtMain.Rows[0]["CreateBy"].ToString();
        txtCreateDate.Text = dtMain.Rows[0]["CreateDate"].ToString();
        this.status = DBConvert.ParseInt(dtMain.Rows[0]["Status"].ToString());
        if (DBConvert.ParseInt(dtMain.Rows[0]["Type"].ToString()) == 1)
        {
          this.typeHold = true;
        }
        else
        {
          this.typeHold = false;
        }
      }
      else
      {
        DataTable dtCode = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FCSDSOHoldGetNewHoldNo('HPO') NewHoldNo");
        if ((dtCode != null) && (dtCode.Rows.Count == 1))
        {
          txtHoldPONote.Text = dtCode.Rows[0]["NewHoldNo"].ToString();
          txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
          txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
        }
      }
    }

    /// <summary>
    /// Load PONo
    /// </summary>
    /// <param name="customerPid"></param>
    private void LoadUltraDropCustomerPONo(long customerPid)
    {
      string commandText = string.Empty;
      commandText = "  SELECT DISTINCT PONo, SaleOrderPid SOPid, SaleNo";
      commandText += " FROM VPLNMasterPlan";
      commandText += " WHERE Balance > 0 AND CustomerPid = " + customerPid;
      commandText += " ORDER BY PONo ASC";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultDropCustomerPONo.DataSource = dtSource;
      ultDropCustomerPONo.DisplayMember = "PONo";
      ultDropCustomerPONo.ValueMember = "PONo";
      ultDropCustomerPONo.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDropCustomerPONo.DisplayLayout.Bands[0].Columns["SOPid"].Hidden = true;
      ultDropCustomerPONo.DisplayLayout.Bands[0].Columns["PONo"].MaxWidth = 200;
      ultDropCustomerPONo.DisplayLayout.Bands[0].Columns["PONo"].MinWidth = 200;
      ultDropCustomerPONo.DisplayLayout.Bands[0].Columns["SaleNo"].MaxWidth = 150;
      ultDropCustomerPONo.DisplayLayout.Bands[0].Columns["SaleNo"].MinWidth = 150;
      ultDropCustomerPONo.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load ItemCode
    /// </summary>
    /// <param name="soPid"></param>
    private void LoadUltraDropSaleCode(long soPid)
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@SOPid", DbType.Int64, soPid);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDLoadInfoItemNeedHold_Select", input);
      if (dtSource == null)
      {
        return;
      }
      ultDropSaleCode.DataSource = dtSource;
      ultDropSaleCode.DisplayMember = "SaleCode";
      ultDropSaleCode.ValueMember = "SaleCode";
      ultDropSaleCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 80;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["Revision"].MaxWidth = 50;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["Revision"].MinWidth = 50;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["OriginalQty"].MaxWidth = 80;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["OriginalQty"].MinWidth = 80;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["Cancelled"].MaxWidth = 80;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["Cancelled"].MinWidth = 80;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["OpenWO"].MaxWidth = 80;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["OpenWO"].MinWidth = 80;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["ShipQty"].MaxWidth = 80;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["ShipQty"].MinWidth = 80;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["ScheduleOnContainer"].MaxWidth = 120;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["ScheduleOnContainer"].MinWidth = 120;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["Balance"].MaxWidth = 80;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["Balance"].MinWidth = 80;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["Holded"].MaxWidth = 80;
      ultDropSaleCode.DisplayLayout.Bands[0].Columns["Holded"].MinWidth = 80;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      btnSave.Enabled = true;
      if(this.status >= 1)
      {
        ultCBCustomer.ReadOnly = true;
        txtReason.ReadOnly = true;
        txtRemark.ReadOnly = true;
        btnDelete.Enabled = false;
        btnSave.Enabled = false;
        chkConfirm.Checked = true;
        chkConfirm.Enabled = false;
        groupContainer.Visible = false;
        radHold.Enabled = false;
        radUnHold.Enabled = false;
      }

      if (this.typeHold)
      {
        radHold.Checked = true;
      }
      else
      {
        radUnHold.Checked = true;
      }
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      DataTable dtCheck = new DataTable();
      // Customer
      if(ultCBCustomer.Text.Length == 0 || ultCBCustomer.Value == null)
      {
        message = "Customer";
        return false;
      }
      // Detail
      DataTable dtMain = (DataTable)ultData.DataSource;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];

        // Check SO
        if (row.Cells["SOPid"].Text.Length == 0)
        {
          row.Cells["SOPid"].Appearance.BackColor = Color.Yellow;
          message = "SO";
          return false;
        }
        else
        {
          commandText = "SELECT SaleOrderPid FROM VPLNMasterPlan";
          commandText += " WHERE CustomerPid = " + DBConvert.ParseLong(ultCBCustomer.Value.ToString());
          commandText += " AND SaleOrderPid = " + DBConvert.ParseLong(row.Cells["SOPid"].Value.ToString()) + "";
          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if(dtCheck == null || dtCheck.Rows.Count == 0)
          {
            row.Cells["SOPid"].Appearance.BackColor = Color.Yellow;
            message = "SO";
            return false;
          }
        }

        // Check ItemCode
        if (row.Cells["ItemCode"].Text.Length == 0)
        {
          row.Cells["ItemCode"].Appearance.BackColor = Color.Yellow;
          message = "ItemCode";
          return false;
        }
        else
        {
          commandText = "SELECT ItemCode FROM VPLNMasterPlan ";
          commandText += " WHERE CustomerPid = " + DBConvert.ParseLong(ultCBCustomer.Value.ToString());
          commandText += " AND SaleOrderPid = " + DBConvert.ParseLong(row.Cells["SOPid"].Value.ToString());
          commandText += " AND ItemCode = '" + row.Cells["ItemCode"].Value.ToString() + "'";
          commandText += " AND Revision = " + DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()) + "";
          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck == null || dtCheck.Rows.Count == 0)
          {
            row.Cells["ItemCode"].Appearance.BackColor = Color.Yellow;
            message = "ItemCode";
            return false;
          }
        }

        //Check HoldQty
        if (row.Cells["HoldQty"].Value.ToString().Length == 0 ||
            DBConvert.ParseInt(row.Cells["HoldQty"].Value.ToString()) <= 0)
        {
          row.Cells["HoldQty"].Appearance.BackColor = Color.Yellow;
          message = "HoldQty";
          return false;
        }

        if(radHold.Checked)
        {
          if (DBConvert.ParseInt(row.Cells["HoldQty"].Value.ToString()) >
            DBConvert.ParseInt(row.Cells["Balance"].Value.ToString()) - DBConvert.ParseInt(row.Cells["Holded"].Value.ToString()))
          {
            message = "HoldQty < Balance - Holded";
            return false;
          }
        }

        if (radUnHold.Checked)
        {
          if (DBConvert.ParseInt(row.Cells["HoldQty"].Value.ToString()) > DBConvert.ParseInt(row.Cells["Holded"].Value.ToString()))
          {
            message = "HoldQty <= Holded";
            return false;
          }
        }

        // Check Trung(SO, ItemCode, Revision)
        int count = (int)dtMain.Compute("COUNT(SOPid)", string.Format("SOPid = {0} AND ItemCode = '{1}' AND Revision = {2}", row.Cells["SOPid"].Value, row.Cells["ItemCode"].Value, row.Cells["Revision"].Value));
        if (count > 1)
        {
          string text  = row.Cells["SaleNo"].Text + ", " + row.Cells["ItemCode"].Text + ", " + row.Cells["Revision"].Text;
          message = "Dupplicated in grid! Please check "+ text;
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
      bool success = this.SaveMaster();
      if (success)
      {
        success = this.SaveDetail();
        if (success)
        {
          success = this.UpdateHoldQtySaleOrder();
        }
      }
      return success;
    }

    /// <summary>
    /// Save Master
    /// </summary>
    /// <returns></returns>
    private bool SaveMaster()
    {
      // Input
      DBParameter[] inputParam = new DBParameter[9];
      // HoldPid
      if (this.holdPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.holdPid);
      }
      // Customer
      inputParam[1] = new DBParameter("@CustomerPid", DbType.Int64, DBConvert.ParseLong(ultCBCustomer.Value.ToString()));
      // HoldNo
      inputParam[2] = new DBParameter("@HoldNo ", DbType.String, "HPO");
      // Status
      if (chkConfirm.Checked)
      {
        inputParam[3] = new DBParameter("@Status", DbType.Int32, 1);
      }
      else
      {
        inputParam[3] = new DBParameter("@Status", DbType.Int32, 0);
      }
      // Reason
      if(txtReason.Text.Trim().Length > 0)
      {
        inputParam[4] = new DBParameter("@Reason", DbType.String, txtReason.Text);
      }
      // Remark
      if (txtRemark.Text.Trim().Length > 0)
      {
        inputParam[5] = new DBParameter("@Remark", DbType.String, txtRemark.Text);
      }
      inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[7] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      if (radHold.Checked)
      {
        inputParam[8] = new DBParameter("@Type", DbType.Int32, 1);
      }
      else
      {
        inputParam[8] = new DBParameter("@Type", DbType.Int32, 0);
      }
     
      // Output
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spCSDSOHoldInfoCS_Edit", inputParam, outputParam);
      if (DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0)
      {
        return false;
      }
      this.holdPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
      return true;
    }

    /// <summary>
    /// Save Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      // 1. Delete      
      for (int i = 0; i < listDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spCSDSOHoldDetail_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0))
        {
          return false;
        }
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        DBParameter[] inputParam = new DBParameter[7];
        if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) != long.MinValue)
        {
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
        }
        inputParam[1] = new DBParameter("@HoldPid", DbType.Int64, this.holdPid);
        inputParam[2] = new DBParameter("@SOPid", DbType.Int64, DBConvert.ParseLong(row.Cells["SOPid"].Value.ToString()));
        inputParam[3] = new DBParameter("@ItemCode", DbType.String, row.Cells["ItemCode"].Value.ToString());
        inputParam[4] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
        inputParam[5] = new DBParameter("@HoldQty", DbType.Int32, DBConvert.ParseInt(row.Cells["HoldQty"].Value.ToString()));
        if (row.Cells["CSDRemark"].Value.ToString().Length > 0)
        {
          inputParam[6] = new DBParameter("@CSDRemark", DbType.String, row.Cells["CSDRemark"].Value.ToString());
        }

        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spCSDSOHoldIDetailCS_Edit", inputParam, outputParam);
        long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Update Qty Hold SO
    /// </summary>
    /// <returns></returns>
    private bool UpdateHoldQtySaleOrder()
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@HoldPid", DbType.Int64, this.holdPid);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure("spCSDHoldQtySOWhenConfirmed_Update", 300, input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if(result <= 0)
      {
        return false;
      }
      return true;
    }


    /// <summary>
    /// Create Data Table
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("CustomerPid", typeof(System.Int64));
      dt.Columns.Add("SOPid", typeof(System.Int64));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("HoldQty", typeof(System.Int32));
      return dt;
    }

    /// <summary>
    /// Import Excel
    /// </summary>
    private void ImportExcel()
    {
      if (this.txtLocation.Text.Trim().Length == 0)
      {
        return;
      }
      try
      { 
        DataTable dtSource = FunctionUtility.GetExcelToDataSetVersion2(txtLocation.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B3:D110]").Tables[0];
        if (dtSource != null)
        {
          string stringdata = string.Empty;
          for (int i = 0; i < dtSource.Rows.Count; i++)
          {
            DataRow rowExcel = dtSource.Rows[i];
            if (ultCBCustomer.Value != null &&
                rowExcel["CustomerPONo"].ToString().Length > 0)
            {
              stringdata = stringdata + ultCBCustomer.Value.ToString() + "^" +rowExcel["CustomerPONo"].ToString() +"^" + rowExcel["SaleCode"].ToString() + "^" + rowExcel["HoldQty"].ToString() + "~";
            }
          }

          DBParameter[] inputParam = new DBParameter[1];
          inputParam[0] = new DBParameter("@StringData", DbType.String, stringdata);
          DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDSOHoldInfomationFromImportExcelNew_Select", inputParam);
          if(dsSource != null)
          {
            DataTable dtAfter = dsSource.Tables[0];
            DataTable dtBefore = (DataTable)ultData.DataSource;
            for (int i = 0; i < dtAfter.Rows.Count; i++)
            {
              DataRow rowAfter = dtAfter.Rows[i];
              DataRow rowBefore = dtBefore.NewRow();
              rowBefore["PONo"] = rowAfter["PONo"].ToString();
              rowBefore["SOPid"] = DBConvert.ParseLong(rowAfter["SOPid"].ToString());
              rowBefore["SaleNo"] = rowAfter["SaleNo"].ToString();
              rowBefore["SaleCode"] = rowAfter["SaleCode"].ToString();
              rowBefore["ItemCode"] = rowAfter["ItemCode"].ToString();
              rowBefore["Revision"] = rowAfter["Revision"].ToString();
              if (DBConvert.ParseInt(rowAfter["OriginalQty"].ToString()) != int.MinValue)
              {
                rowBefore["OriginalQty"] = DBConvert.ParseInt(rowAfter["OriginalQty"].ToString());
              }
              if (DBConvert.ParseInt(rowAfter["Cancelled"].ToString()) != int.MinValue)
              {
                rowBefore["Cancelled"] = DBConvert.ParseInt(rowAfter["Cancelled"].ToString());
              }
              if (DBConvert.ParseInt(rowAfter["ShipQty"].ToString()) != int.MinValue)
              {
                rowBefore["ShipQty"] = DBConvert.ParseInt(rowAfter["ShipQty"].ToString());
              }
              if (DBConvert.ParseInt(rowAfter["OpenWO"].ToString()) != int.MinValue)
              {
                rowBefore["OpenWO"] = DBConvert.ParseInt(rowAfter["OpenWO"].ToString());
              }
              if (DBConvert.ParseInt(rowAfter["Holded"].ToString()) != int.MinValue)
              {
                rowBefore["Holded"] = DBConvert.ParseInt(rowAfter["Holded"].ToString());
              }
              if (DBConvert.ParseInt(rowAfter["ScheduleOnContainer"].ToString()) != int.MinValue)
              {
                rowBefore["ScheduleOnContainer"] = DBConvert.ParseInt(rowAfter["ScheduleOnContainer"].ToString());
              }
              if (DBConvert.ParseInt(rowAfter["Balance"].ToString()) != int.MinValue)
              {
                rowBefore["Balance"] = DBConvert.ParseInt(rowAfter["Balance"].ToString());
              }
              rowBefore["HoldQty"] = DBConvert.ParseInt(rowAfter["HoldQty"].ToString());
              rowBefore["PLNApproved"] = 0;
              // Add Row
              dtBefore.Rows.Add(rowBefore);
            }
            // Gan Data Source
            ultData.DataSource = dtBefore;
            this.GetDataContainer();

            // Data Error
            string listErrors = string.Empty;
            txtError.Text = string.Empty;
            for (int i = 0; i < dsSource.Tables[1].Rows.Count; i++)
            {
              listErrors = listErrors + "("+dsSource.Tables[1].Rows[i]["PONo"].ToString() + " - " + dsSource.Tables[1].Rows[i]["SaleCode"].ToString() + " - " + dsSource.Tables[1].Rows[i]["HoldQty"].ToString() + ")";
            }
            if(listErrors.Length > 0)
            {
              txtError.Text = listErrors;
            }
          }
        }
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0105");
        return;
      }
    }

    /// <summary>
    /// Get data container
    /// </summary>
    private void GetDataContainer()
    {
      DataTable dt = this.CreateDataTable();
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = ultData.Rows[i];
        if (DBConvert.ParseInt(rowGrid.Cells["HoldQty"].Value.ToString()) > 0)
        {
          DataRow row = dt.NewRow();
          row["CustomerPid"] = DBConvert.ParseLong(ultCBCustomer.Value.ToString());
          row["SOPid"] = DBConvert.ParseLong(rowGrid.Cells["SOPid"].Value.ToString());
          row["ItemCode"] = rowGrid.Cells["ItemCode"].Value.ToString();
          row["Revision"] = DBConvert.ParseInt(rowGrid.Cells["Revision"].Value.ToString());
          row["HoldQty"] = DBConvert.ParseInt(rowGrid.Cells["HoldQty"].Value.ToString());
          dt.Rows.Add(row);
        }
      }

      SqlDBParameter[] inputParam = new SqlDBParameter[2];
      inputParam[0] = new SqlDBParameter("@TableImport", SqlDbType.Structured, dt);
      inputParam[1] = new SqlDBParameter("@Flag", SqlDbType.Int, 0);
      DataTable dtSource = Shared.DataBaseUtility.SqlDataBaseAccess.SearchStoreProcedureDataTable("spCSDSOHoldInfluenceContainer_Select", inputParam);
      if (dtSource != null)
      {
        ultDataContainer.DataSource = dtSource;
      }
    }

    /// <summary>
    /// Send Email
    /// </summary>
    private void SendEmail()
    {
      if (chkConfirm.Checked && this.typeHold == true)
      {
        DBParameter[] inputParam = new DBParameter[1];
        inputParam[0] = new DBParameter("@HoldPid", DbType.Int64, this.holdPid);
        DataSet ds = DataBaseAccess.SearchStoreProcedure("spCSDRPTSOHoldInfomation_Select", inputParam);
        if (ds.Tables.Count > 0)
        {
          dsCSDHoldSaleOrder dsSource = new dsCSDHoldSaleOrder();
          dsSource.Tables["dtInfo"].Merge(ds.Tables[0]);
          dsSource.Tables["dtDetail"].Merge(ds.Tables[1]);

          ReportClass cpt = null;
          cpt = new cptCSDHoldSaleOrder();
          cpt.SetDataSource(dsSource);

          string time = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
          // Check Exist Folder
          if (!Directory.Exists(@"\\fsdc01\public\\Reports\HoldSO"))
          {
            Directory.CreateDirectory(@"\\fsdc01\public\\Reports\HoldSO");
          }
          string path = string.Format(@"\\fsdc01\public\\Reports\HoldSO\\HOLD SALE ORDER_({0})_{1}_encrypted.pdf", txtHoldPONote.Text, DBConvert.ParseString(DateTime.Now.ToString("yyyyMMdd")) + time);
          string pathNew = string.Format(@"\\fsdc01\public\\Reports\HoldSO\\HOLD SALE ORDER_({0})_{1}.pdf", txtHoldPONote.Text, DBConvert.ParseString(DateTime.Now.ToString("yyyyMMdd")) + time);
          cpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path);
          using (Stream input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
          using (Stream output = new FileStream(pathNew, FileMode.Create, FileAccess.Write, FileShare.None))
          {
            PdfReader reader = new PdfReader(input);
            PdfEncryptor.Encrypt(reader, output, iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, null, "it1daico", PdfWriter.ALLOW_PRINTING);
          }
          MailMessage mailMessage = new MailMessage();
          string body = string.Empty;
          body += "<p><i><font color='6699CC'>";
          body += "Automatic Email From ERP System.";
          body += "</font></i></p> ";

          string mailTo = string.Empty;
          string commandText = string.Empty;
          commandText = "  SELECT NV.email";
          commandText += " FROM TblBOMCodeMaster AA";
          commandText += " LEFT JOIN VHRNhanVien NV ON NV.ID_NhanVien = CONVERT(INT, AA.Value)";
          commandText += " WHERE AA.[Group] = 7051";

          DataTable dtEmail = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtEmail != null)
          {
            for (int i = 0; i < dtEmail.Rows.Count; i++)
            {
              mailTo = dtEmail.Rows[i]["email"].ToString() + ";" + mailTo;
            }
          }

          mailMessage.ServerName = "10.0.0.5";
          mailMessage.Username = "dc@daico-furniture.com";
          mailMessage.Password = "dc123456";
          mailMessage.From = "dc@daico-furniture.com";
          mailMessage.To = mailTo;
          mailMessage.Subject = "HOLD SALE ORDER";
          mailMessage.Body = body;

          IList attachments = new ArrayList();
          attachments.Add(pathNew);
          mailMessage.Attachfile = attachments;
          mailMessage.SendMail(true);
        }
      }
    }
    #endregion Function

    #region Event

    /// <summary>
    /// Save Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      btnSave.Enabled = false;
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        btnSave.Enabled = true;
        return;
      }

      // Save Data
      success = this.SaveData();
      if (success)
      {
        // Send Email
        this.SendEmail();

        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Delete Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (this.holdPid != long.MinValue)
      {
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@Pid", DbType.Int64, this.holdPid);

        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, 0);

        DataBaseAccess.ExecuteStoreProcedure("spCSDSOHoldInfomation_Delete", input, output);
        if (DBConvert.ParseLong(output[0].Value.ToString()) <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0093", txtHoldPONote.Text);
          return;
        }
        else
        {
          WindowUtinity.ShowMessageSuccess("MSG0002");
        }
        this.CloseTab();
      }
    }

    /// <summary>
    /// Get Template Import Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "CSD_03_012";
      string sheetName = "Sheet1";
      string outFileName = "Template Import Hold SO";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    /// <summary>
    /// Brown
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtLocation.Text.Trim().Length > 0);
    }

    /// <summary>
    /// Import
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check Customer
      if(ultCBCustomer.Text.Length == 0 || ultCBCustomer.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Customer");
        return;
      }
      this.ImportExcel();
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

    /// <summary>
    /// Change Customer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCBCustomer_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBCustomer.Value != null)
      {
        this.LoadUltraDropCustomerPONo(DBConvert.ParseLong(ultCBCustomer.Value.ToString()));
      }
      else
      {
        this.LoadUltraDropCustomerPONo(-1);
      }

      // Read Only Customer
      ultCBCustomer.ReadOnly = true;
    }

    /// <summary>
    /// Init Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      ControlUtility.SetPropertiesUltraGrid(ultData);

      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[0].Columns["OriginalQty"].Header.Caption = "Original \n Qty";
      e.Layout.Bands[0].Columns["ShipQty"].Header.Caption = "ShipQty";
      e.Layout.Bands[0].Columns["OpenWO"].Header.Caption = "OpenWO";
      e.Layout.Bands[0].Columns["ScheduleOnContainer"].Header.Caption = "Schedule On \n Container";
      e.Layout.Bands[0].Columns["PLNRemark"].Header.Caption = "PLN \n Remark";
      e.Layout.Bands[0].Columns["CSDRemark"].Header.Caption = "CSD \n Remark";
      e.Layout.Bands[0].Columns["PLNApproved"].Header.Caption = "PLN \n Approved";

      e.Layout.Bands[0].Columns["PONo"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["SaleCode"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["HoldQty"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["CSDRemark"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["OriginalQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Cancelled"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ShipQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["OpenWO"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ScheduleOnContainer"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Balance"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Holded"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PLNRemark"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PLNApproved"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["PLNApproved"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["PLNApproved"].DefaultCellValue = 0;

      e.Layout.Bands[0].Columns["PONo"].ValueList = ultDropCustomerPONo;
      e.Layout.Bands[0].Columns["SaleCode"].ValueList = ultDropSaleCode;

      if(this.status >= 1)
      {
        e.Layout.AutoFitColumns = true;
        e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

        e.Layout.Bands[0].Columns["OriginalQty"].Hidden = true;
        e.Layout.Bands[0].Columns["Cancelled"].Hidden = true;
        e.Layout.Bands[0].Columns["ShipQty"].Hidden = true;
        e.Layout.Bands[0].Columns["OpenWO"].Hidden = true;
        e.Layout.Bands[0].Columns["ScheduleOnContainer"].Hidden = true;
        e.Layout.Bands[0].Columns["Balance"].Hidden = true;
        e.Layout.Bands[0].Columns["Holded"].Hidden = true;
      }

      if (this.typeHold)
      {
        e.Layout.Bands[0].Columns["HoldQty"].Header.Caption = "HoldQty";
      }
      else
      {
        e.Layout.Bands[0].Columns["HoldQty"].Header.Caption = "UnHoldQty";
      }

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["HoldQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init data container
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDataContainer_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      ControlUtility.SetPropertiesUltraGrid(ultDataContainer);

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].Columns["ShipDate"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["ShipDate"].CellAppearance.ForeColor = Color.Red;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      if(radUnHold.Checked)
      {
        e.Layout.Bands[0].Columns["HoldQty"].Header.Caption = "HoldQty";
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// After cell update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "PONo":
          if (row.Cells["PONo"].Value.ToString().Length > 0)
          {
            if (ultDropCustomerPONo.SelectedRow != null && ultDropCustomerPONo.SelectedRow.Cells["PONo"].Value != null)
            {
              row.Cells["SOPid"].Value = DBConvert.ParseLong(ultDropCustomerPONo.SelectedRow.Cells["SOPid"].Value.ToString());
              row.Cells["SaleNo"].Value = ultDropCustomerPONo.SelectedRow.Cells["SaleNo"].Value.ToString();
            }
            else
            {
              row.Cells["SOPid"].Value = DBNull.Value;
              row.Cells["SaleNo"].Value = DBNull.Value;
            }
          }
          else
          {
            row.Cells["SOPid"].Value = DBNull.Value;
            row.Cells["SaleNo"].Value = DBNull.Value;
          }
          // Update SaleCode
          row.Cells["SaleCode"].Value = DBNull.Value;
          break;
        case "SOPid":
          row.Cells["ItemCode"].Value = DBNull.Value;
          break;
        case "SaleCode":
          row.Cells["ItemCode"].Value = DBNull.Value;
          row.Cells["Revision"].Value = DBNull.Value;
          row.Cells["OriginalQty"].Value = DBNull.Value;
          row.Cells["Cancelled"].Value = DBNull.Value;
          row.Cells["ShipQty"].Value = DBNull.Value;
          row.Cells["OpenWO"].Value = DBNull.Value;
          row.Cells["ScheduleOnContainer"].Value = DBNull.Value;
          row.Cells["Balance"].Value = DBNull.Value;
          row.Cells["Holded"].Value = DBNull.Value;

          if(ultDropSaleCode.SelectedRow != null)
          {
            row.Cells["ItemCode"].Value = ultDropSaleCode.SelectedRow.Cells["ItemCode"].Value.ToString();
            row.Cells["Revision"].Value = DBConvert.ParseInt(ultDropSaleCode.SelectedRow.Cells["Revision"].Value.ToString());
            row.Cells["OriginalQty"].Value = DBConvert.ParseInt(ultDropSaleCode.SelectedRow.Cells["OriginalQty"].Value.ToString());
            row.Cells["Cancelled"].Value = DBConvert.ParseInt(ultDropSaleCode.SelectedRow.Cells["Cancelled"].Value.ToString());
            row.Cells["ShipQty"].Value = DBConvert.ParseInt(ultDropSaleCode.SelectedRow.Cells["ShipQty"].Value.ToString());
            row.Cells["OpenWO"].Value = DBConvert.ParseInt(ultDropSaleCode.SelectedRow.Cells["OpenWO"].Value.ToString());
            row.Cells["ScheduleOnContainer"].Value = DBConvert.ParseInt(ultDropSaleCode.SelectedRow.Cells["ScheduleOnContainer"].Value.ToString());
            row.Cells["Balance"].Value = DBConvert.ParseInt(ultDropSaleCode.SelectedRow.Cells["Balance"].Value.ToString());
            row.Cells["Holded"].Value = DBConvert.ParseInt(ultDropSaleCode.SelectedRow.Cells["Holded"].Value.ToString());
          }
          break;
        case "HoldQty":
          if (DBConvert.ParseInt(row.Cells["HoldQty"].Value.ToString()) > 0 &&
             (DBConvert.ParseInt(row.Cells["HoldQty"].Value.ToString()) <=
             DBConvert.ParseInt(row.Cells["Balance"].Value.ToString()) - 
             DBConvert.ParseInt(row.Cells["Holded"].Value.ToString())))
          {
            row.Cells["HoldQty"].Appearance.BackColor = Color.Aqua;
          }

          this.GetDataContainer();
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Before cell active
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "SaleCode":
          if (DBConvert.ParseLong(row.Cells["SOPid"].Value.ToString()) != long.MinValue)
          {
            this.LoadUltraDropSaleCode(DBConvert.ParseLong(row.Cells["SOPid"].Value.ToString()));
          }
          else
          {
            this.LoadUltraDropSaleCode(-1);
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Before cell update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
     string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      string value = e.NewValue.ToString();
      switch (columnName)
      {
        case "HoldQty":
          if (DBConvert.ParseInt(value) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "HoldQty");
            e.Cancel = true;
            return;
          }

          if(radHold.Checked)
          {
            if (DBConvert.ParseInt(value) >
             DBConvert.ParseInt(row.Cells["Balance"].Value.ToString()) - DBConvert.ParseInt(row.Cells["Holded"].Value.ToString()))
            {
              WindowUtinity.ShowMessageError("ERR0001", "HoldQty <= BalanceQty - Holded");
              e.Cancel = true;
              return;
            }
          }

          if (radUnHold.Checked)
          {
            if (DBConvert.ParseInt(value) > DBConvert.ParseInt(row.Cells["Holded"].Value.ToString()))
            {
              WindowUtinity.ShowMessageError("ERR0001", "HoldQty <= " + DBConvert.ParseInt(row.Cells["Holded"].Value.ToString()));
              e.Cancel = true;
            }
          }
          break;
        case "SaleCode":
          DataTable dt = (DataTable)ultDropSaleCode.DataSource;
          DataRow[] foundRow = dt.Select("SaleCode = '"+ row.Cells["SaleCode"].Text + "'");
          if(foundRow.Length == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "SaleCode");
            e.Cancel = true;
            return;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Before cell delete
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

      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }


    private void radHold_CheckedChanged(object sender, EventArgs e)
    {
      if (this.status <= 0)
      {
        if (radHold.Checked)
        {
          ultData.DisplayLayout.Bands[0].Columns["HoldQty"].Header.Caption = "HoldQty";
        }
        else
        {
          ultData.DisplayLayout.Bands[0].Columns["HoldQty"].Header.Caption = "UnHoldQty";
        }
      }
    }

    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.GetDataContainer();
    }

    private void radUnHold_CheckedChanged(object sender, EventArgs e)
    {
      if (radUnHold.Checked)
      {
        groupContainer.Visible = false;
      }
      else
      {
        groupContainer.Visible = true;
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      string strTemplateName = "RPT_CSD_03_012";
      string strSheetName = "Sheet1";
      string strOutFileName = "HOLD SALEORDER";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      int count = 0;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (count > 0)
        {
          oXlsReport.Cell("B8:P8").Copy();
          oXlsReport.RowInsert(7 + count);
          oXlsReport.Cell("B8:P8", 0, count).Paste();
        }
        oXlsReport.Cell("**No", 0, count).Value = count + 1;
        oXlsReport.Cell("**PONo", 0, count).Value = row.Cells["PONo"].Value.ToString();
        oXlsReport.Cell("**SaleNo", 0, count).Value = row.Cells["SaleNo"].Value.ToString();
        oXlsReport.Cell("**SaleCode", 0, count).Value = row.Cells["SaleCode"].Value.ToString();
        oXlsReport.Cell("**ItemCode", 0, count).Value = row.Cells["ItemCode"].Value.ToString();
        if (DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()) != int.MinValue)
        {
          oXlsReport.Cell("**Revision", 0, count).Value = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
        }
        else
        {
          oXlsReport.Cell("**Revision", 0, count).Value = DBNull.Value;
        }

        if (DBConvert.ParseInt(row.Cells["OriginalQty"].Value.ToString()) != int.MinValue)
        {
          oXlsReport.Cell("**OriginalQty", 0, count).Value = DBConvert.ParseInt(row.Cells["OriginalQty"].Value.ToString());
        }
        else
        {
          oXlsReport.Cell("**OriginalQty", 0, count).Value = DBNull.Value;
        }

        if (DBConvert.ParseInt(row.Cells["Cancelled"].Value.ToString()) != int.MinValue)
        {
          oXlsReport.Cell("**Cancelled", 0, count).Value = DBConvert.ParseInt(row.Cells["Cancelled"].Value.ToString());
        }
        else
        {
          oXlsReport.Cell("**Cancelled", 0, count).Value = DBNull.Value;
        }

        if (DBConvert.ParseInt(row.Cells["ShipQty"].Value.ToString()) != int.MinValue)
        {
          oXlsReport.Cell("**ShipQty", 0, count).Value = DBConvert.ParseInt(row.Cells["ShipQty"].Value.ToString());
        }
        else
        {
          oXlsReport.Cell("**ShipQty", 0, count).Value = DBNull.Value;
        }

        if (DBConvert.ParseInt(row.Cells["OpenWO"].Value.ToString()) != int.MinValue)
        {
          oXlsReport.Cell("**OpenWO", 0, count).Value = DBConvert.ParseInt(row.Cells["OpenWO"].Value.ToString());
        }
        else
        {
          oXlsReport.Cell("**OpenWO", 0, count).Value = DBNull.Value;
        }

        if (DBConvert.ParseInt(row.Cells["ScheduleOnContainer"].Value.ToString()) != int.MinValue)
        {
          oXlsReport.Cell("**ScheduleOnContainer", 0, count).Value = DBConvert.ParseInt(row.Cells["ScheduleOnContainer"].Value.ToString());
        }
        else
        {
          oXlsReport.Cell("**ScheduleOnContainer", 0, count).Value = DBNull.Value;
        }

        if (DBConvert.ParseInt(row.Cells["Balance"].Value.ToString()) != int.MinValue)
        {
          oXlsReport.Cell("**Balance", 0, count).Value = DBConvert.ParseInt(row.Cells["Balance"].Value.ToString());
        }
        else
        {
          oXlsReport.Cell("**Balance", 0, count).Value = DBNull.Value;
        }

        if (DBConvert.ParseInt(row.Cells["Holded"].Value.ToString()) != int.MinValue)
        {
          oXlsReport.Cell("**Holded", 0, count).Value = DBConvert.ParseInt(row.Cells["Holded"].Value.ToString());
        }
        else
        {
          oXlsReport.Cell("**Holded", 0, count).Value = DBNull.Value;
        }

        if (DBConvert.ParseInt(row.Cells["HoldQty"].Value.ToString()) != int.MinValue)
        {
          oXlsReport.Cell("**HoldQty", 0, count).Value = DBConvert.ParseInt(row.Cells["HoldQty"].Value.ToString());
        }
        else
        {
          oXlsReport.Cell("**HoldQty", 0, count).Value = DBNull.Value;
        }

        oXlsReport.Cell("**CSDRemark", 0, count).Value = row.Cells["CSDRemark"].Value.ToString();
        count = count + 1;
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }
    #endregion Event
  }
}
