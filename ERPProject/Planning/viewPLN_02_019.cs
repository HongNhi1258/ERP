/*
  Author      : 
  Date        : 13-11-2013
  Description : Cancel WO
*/
using CrystalDecisions.CrystalReports.Engine;
using DaiCo.Application;
using DaiCo.ERPProject.Share.DataSetSource;
using DaiCo.ERPProject.Share.ReportTemplate;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_02_019 : MainUserControl
  {
    #region Field
    public long cancelWOPid = long.MinValue;
    private int status = int.MinValue;
    private int tabLock1 = int.MinValue;
    private int tabLock2 = int.MinValue;
    private int tabLock3 = int.MinValue;
    private int tabLock4 = int.MinValue;
    private int tabLock5 = int.MinValue;
    private DataTable dtRequestCST = new DataTable();
    private DataSet dsRequestITW = new DataSet();
    private string requestOnlineCST = string.Empty;
    #endregion Field

    #region Init
    public viewPLN_02_019()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load View
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_019_Load(object sender, EventArgs e)
    {
      this.LoadWorkOrder();
      this.LoadTab1(this.cancelWOPid);
    }

    /// <summary>
    /// Load WO
    /// </summary>
    private void LoadWorkOrder()
    {
      string cmdText = string.Empty;
      cmdText = string.Format(@"SELECT DISTINCT WorkOrderPid WO
                              FROM TblPLNWorkOrderConfirmedDetails
                              ORDER BY WO ");

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(cmdText);
      if (dtSource == null)
      {
        return;
      }
      ultDropWO.DataSource = dtSource;
      ultDropWO.DisplayMember = "WO";
      ultDropWO.ValueMember = "WO";
      ultDropWO.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDropWO.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load CarcassCode/Chan Close Wo
    /// </summary>
    /// <param name="wo"></param>
    private void LoadCarcassCode(int wo)
    {
      string commandText = string.Empty;
      commandText = string.Format(@" SELECT DISTINCT CarcassCode
                                     FROM TblPLNWorkOrderConfirmedDetails
                                     WHERE WorkOrderPid = {0} AND ISNULL(CloseCOM1, 0) <> 1 AND ISNULL(CloseCAR, 0) <> 1 AND ISNULL(CloseAll, 0) <> 1", wo);

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultDropCarcassCode.DataSource = dtSource;
      ultDropCarcassCode.DisplayMember = "CarcassCode";
      ultDropCarcassCode.ValueMember = "CarcassCode";
      ultDropCarcassCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDropCarcassCode.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load ItemCode/Chan Close Wo
    /// </summary>
    /// <param name="wo"></param>
    /// <param name="carcassCode"></param>
    private void LoadItemCode(int wo, string carcassCode)
    {
      string commandText = string.Empty;
      //      commandText = string.Format(@"SELECT DISTINCT ItemCode, Revision, Qty
      //                                     FROM TblPLNWorkOrderConfirmedDetails
      //                                     WHERE WorkOrderPid = {0} AND CarcassCode = '{1}' AND ISNULL(CloseCOM1, 0) <> 1 AND ISNULL(CloseCAR, 0) <> 1 
      //                                                              AND ISNULL(CloseAll, 0) <> 1", wo, carcassCode);

      commandText = " SELECT DISTINCT ItemCode, Revision, Qty";
      commandText += " FROM TblPLNWorkOrderConfirmedDetails";
      commandText += " WHERE WorkOrderPid = " + wo + " AND CarcassCode  = '" + carcassCode + "'";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultDropItemCode.DataSource = dtSource;
      ultDropItemCode.DisplayMember = "ItemCode";
      ultDropItemCode.ValueMember = "ItemCode";
      ultDropItemCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDropItemCode.DisplayLayout.AutoFitColumns = true;
    }
    #endregion Init

    #region Function

    /// <summary>
    /// Load Main Data
    /// </summary>
    private void LoadMainData()
    {
      DBParameter[] inputParam = new DBParameter[1] { new DBParameter("@CancelWOPid", DbType.Int64, this.cancelWOPid) };
      DataTable dtMain = DataBaseAccess.SearchStoreProcedureDataTable("spPLNWOCancelInformation_Select", inputParam);
      if (dtMain != null && dtMain.Rows.Count > 0)
      {
        txtCancelWONo.Text = dtMain.Rows[0]["CancelWONo"].ToString();
        txtCreateBy.Text = dtMain.Rows[0]["CreateBy"].ToString();
        txtCreateDate.Text = dtMain.Rows[0]["CreateDate"].ToString();
        txtReason.Text = dtMain.Rows[0]["Reason"].ToString();
        txtRemark.Text = dtMain.Rows[0]["Remark"].ToString();
        this.status = DBConvert.ParseInt(dtMain.Rows[0]["Status"].ToString());
        this.tabLock1 = DBConvert.ParseInt(dtMain.Rows[0]["Tab1"].ToString());
        this.tabLock2 = DBConvert.ParseInt(dtMain.Rows[0]["Tab2"].ToString());
        this.tabLock3 = DBConvert.ParseInt(dtMain.Rows[0]["Tab3"].ToString());
        this.tabLock4 = DBConvert.ParseInt(dtMain.Rows[0]["Tab4"].ToString());
        this.tabLock5 = DBConvert.ParseInt(dtMain.Rows[0]["Tab5"].ToString());
      }
      else
      {
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPLNCancelWOGetNewCode('CWO') CancelWONo");
        if ((dt != null) && (dt.Rows.Count > 0))
        {
          txtCancelWONo.Text = dt.Rows[0]["CancelWONo"].ToString();
          txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
          ;
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
      if (this.tabLock1 == 1)
      {
        btnCheckStatus.Enabled = false;
        btnLockData1.Enabled = false;
        btnImport.Enabled = false;
      }
      if (this.tabLock2 == 1)
      {
        btnLockData2.Enabled = false;
      }
      if (this.tabLock3 == 1)
      {
        btnLockData3.Enabled = false;
      }
      if (this.tabLock4 == 1)
      {
        btnLockData4.Enabled = false;
      }
      if (this.tabLock5 == 1)
      {
        btnLockData5.Enabled = false;
      }

      if (this.status == 1)
      {
        txtReason.ReadOnly = true;
        txtRemark.ReadOnly = true;
        chkConfirm.Checked = true;
        chkConfirm.Enabled = false;
        btnSave.Enabled = false;
        btnDelete.Enabled = false;
      }

      // Show User Guide
      string location = string.Empty;
      string commandText = string.Empty;
      commandText = String.Format(@"SELECT Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND Code = {1}", Shared.Utility.ConstantClass.GROUP_GNR_PATHFILEUPLOAD, 4);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null)
      {
        location = dt.Rows[0]["Value"].ToString() + txtCancelWONo.Text + ".pdf";
        if (File.Exists(location))
        {
          linkLabUserGuide.Visible = true;
        }
        else
        {
          linkLabUserGuide.Visible = false;
        }
      }
    }

    /// <summary>
    /// Table Check Status
    /// </summary>
    /// <returns></returns>
    private DataTable CreateTableCheckStatus()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("WO", typeof(System.Int32));
      dt.Columns.Add("CarcassCode", typeof(System.String));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("Qty", typeof(System.Int32));
      dt.Columns.Add("QtyCancel", typeof(System.Int32));
      dt.Columns.Add("Remark", typeof(System.String));
      return dt;
    }

    /// <summary>
    /// Table Request Component Store
    /// </summary>
    /// <returns></returns>
    private DataTable CreateTableRequestCST()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Wo", typeof(System.Int32));
      dt.Columns.Add("Carcass", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Int32));
      return dt;
    }

    /// <summary>
    /// Create Data Set Allocate Material
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("WO", typeof(System.Int64));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("QtyCancel", typeof(System.Int32));
      ds.Tables.Add(taParent);

      // Material
      DataTable taChildMaterial = new DataTable("dtChildMaterial");
      taChildMaterial.Columns.Add("WO", typeof(System.Int64));
      taChildMaterial.Columns.Add("ItemCode", typeof(System.String));
      taChildMaterial.Columns.Add("Revision", typeof(System.Int32));
      taChildMaterial.Columns.Add("WOQty", typeof(System.Int32));
      taChildMaterial.Columns.Add("MaterialCode", typeof(System.String));
      taChildMaterial.Columns.Add("MaterialName", typeof(System.String));
      taChildMaterial.Columns.Add("Unit", typeof(System.String));
      taChildMaterial.Columns.Add("MatQtyPerItem", typeof(System.Double));
      taChildMaterial.Columns.Add("TotalMaterial", typeof(System.Double));
      taChildMaterial.Columns.Add("Allocated", typeof(System.Double));
      taChildMaterial.Columns.Add("Supplement", typeof(System.Double));
      taChildMaterial.Columns.Add("Required", typeof(System.Double));
      taChildMaterial.Columns.Add("Issued", typeof(System.Double));
      taChildMaterial.Columns.Add("NonIssue", typeof(System.Double));
      taChildMaterial.Columns.Add("Remain", typeof(System.Double));
      taChildMaterial.Columns.Add("NonAllocate", typeof(System.Double));
      taChildMaterial.Columns.Add("Stock", typeof(System.Double));
      taChildMaterial.Columns.Add("QtyCancel", typeof(System.Double));
      taChildMaterial.Columns.Add("ReAllocateQty", typeof(System.Double));
      taChildMaterial.Columns.Add("Type", typeof(System.Int32));
      ds.Tables.Add(taChildMaterial);

      ds.Relations.Add(new DataRelation("dtParent_dtChildMaterial", new DataColumn[] { taParent.Columns["WO"], taParent.Columns["ItemCode"], taParent.Columns["Revision"] }, new DataColumn[] { taChildMaterial.Columns["WO"], taChildMaterial.Columns["ItemCode"], taChildMaterial.Columns["Revision"] }, false));
      return ds;
    }

    /// <summary>
    /// Create Data Set Allocate Material
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSetAllocateWoodVeneer()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("WO", typeof(System.Int64));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("QtyCancel", typeof(System.Int32));
      ds.Tables.Add(taParent);

      // Veneer
      DataTable taChildVeneer = new DataTable("dtChildVeneer");
      taChildVeneer.Columns.Add("WO", typeof(System.Int64));
      taChildVeneer.Columns.Add("CarcassCode", typeof(System.String));
      taChildVeneer.Columns.Add("ItemCode", typeof(System.String));
      taChildVeneer.Columns.Add("CarcassQty", typeof(System.Int32));
      taChildVeneer.Columns.Add("MainCode", typeof(System.String));
      taChildVeneer.Columns.Add("MainName", typeof(System.String));
      taChildVeneer.Columns.Add("AltCode", typeof(System.String));
      taChildVeneer.Columns.Add("AltName", typeof(System.String));
      taChildVeneer.Columns.Add("QtyUnit", typeof(System.Double));
      taChildVeneer.Columns.Add("TotalQty", typeof(System.Double));
      taChildVeneer.Columns.Add("Allocated", typeof(System.Double));
      taChildVeneer.Columns.Add("AllocatedSpecial", typeof(System.Double));
      taChildVeneer.Columns.Add("Supplement", typeof(System.Double));
      taChildVeneer.Columns.Add("Required", typeof(System.Double));
      taChildVeneer.Columns.Add("Issued", typeof(System.Double));
      taChildVeneer.Columns.Add("NonIssue", typeof(System.Double));
      taChildVeneer.Columns.Add("Remain", typeof(System.Double));
      taChildVeneer.Columns.Add("NonAllocate", typeof(System.Double));
      taChildVeneer.Columns.Add("Stock", typeof(System.Double));
      taChildVeneer.Columns.Add("QtyCancel", typeof(System.Int32));
      taChildVeneer.Columns.Add("ReAllocateQty", typeof(System.Double));
      taChildVeneer.Columns.Add("Type", typeof(System.Int32));
      ds.Tables.Add(taChildVeneer);

      ds.Relations.Add(new DataRelation("dtParent_dtChildVeneer", new DataColumn[] { taParent.Columns["WO"], taParent.Columns["CarcassCode"] }, new DataColumn[] { taChildVeneer.Columns["WO"], taChildVeneer.Columns["CarcassCode"] }, false));

      // Woods
      DataTable taChildWood = new DataTable("dtChildWood");
      taChildWood.Columns.Add("WO", typeof(System.Int64));
      taChildWood.Columns.Add("CarcassCode", typeof(System.String));
      taChildWood.Columns.Add("ItemCode", typeof(System.String));
      taChildWood.Columns.Add("CarcassQty", typeof(System.Int32));
      taChildWood.Columns.Add("MainGroup", typeof(System.String));
      taChildWood.Columns.Add("MainCategory", typeof(System.String));
      taChildWood.Columns.Add("MainName", typeof(System.String));
      taChildWood.Columns.Add("AltGroup", typeof(System.String));
      taChildWood.Columns.Add("AltCategory", typeof(System.String));
      taChildWood.Columns.Add("AltName", typeof(System.String));
      taChildWood.Columns.Add("QtyUnit", typeof(System.Double));
      taChildWood.Columns.Add("TotalQty", typeof(System.Double));
      taChildWood.Columns.Add("Allocated", typeof(System.Double));
      taChildWood.Columns.Add("Supplement", typeof(System.Double));
      taChildWood.Columns.Add("Required", typeof(System.Double));
      taChildWood.Columns.Add("Issued", typeof(System.Double));
      taChildWood.Columns.Add("NonIssue", typeof(System.Double));
      taChildWood.Columns.Add("Remain", typeof(System.Double));
      taChildWood.Columns.Add("NonAllocate", typeof(System.Double));
      taChildWood.Columns.Add("Stock", typeof(System.Double));
      taChildWood.Columns.Add("QtyCancel", typeof(System.Int32));
      taChildWood.Columns.Add("ReAllocateQty", typeof(System.Double));
      taChildWood.Columns.Add("Type", typeof(System.Int32));
      ds.Tables.Add(taChildWood);

      ds.Relations.Add(new DataRelation("dtParent_dtChildWood", new DataColumn[] { taParent.Columns["WO"], taParent.Columns["CarcassCode"] }, new DataColumn[] { taChildWood.Columns["WO"], taChildWood.Columns["CarcassCode"] }, false));
      return ds;
    }

    /// <summary>
    /// Create Data Set For Request ITW
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSetForRequestITW()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("WO", typeof(System.Int32));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("QtyCancel", typeof(System.Int32));
      ds.Tables.Add(taParent);

      // Material
      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("WO", typeof(System.Int32));
      taChild.Columns.Add("CarcassCode", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("Partcode", typeof(System.String));
      taChild.Columns.Add("WorkAreaPid", typeof(System.Int64));
      taChild.Columns.Add("StandByEN", typeof(System.String));
      taChild.Columns.Add("Select", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", new DataColumn[] { taParent.Columns["WO"], taParent.Columns["CarcassCode"], taParent.Columns["ItemCode"], taParent.Columns["Revision"] }, new DataColumn[] { taChild.Columns["WO"], taChild.Columns["CarcassCode"], taChild.Columns["ItemCode"], taChild.Columns["Revision"] }, false));
      return ds;
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <param name="tab"></param>
    /// <returns></returns>
    private bool CheckValid(out string message, int tab)
    {
      message = string.Empty;
      // Check Lock Tab
      if (tab == 2)
      {
        if (this.tabLock1 != 1)
        {
          message = "Lock Tab1";
          return false;
        }
      }
      else if (tab == 3)
      {
        if (this.tabLock1 != 1)
        {
          message = "Lock Tab1";
          return false;
        }

        if (this.tabLock2 != 1)
        {
          message = "Lock Tab2";
          return false;
        }
      }
      else if (tab == 4)
      {
        if (this.tabLock1 != 1)
        {
          message = "Lock Tab1";
          return false;
        }

        if (this.tabLock2 != 1)
        {
          message = "Lock Tab2";
          return false;
        }

        if (this.tabLock3 != 1)
        {
          message = "Lock Tab3";
          return false;
        }
      }
      else if (tab == 5)
      {
        if (this.tabLock1 != 1)
        {
          message = "Lock Tab1";
          return false;
        }

        if (this.tabLock2 != 1)
        {
          message = "Lock Tab2";
          return false;
        }

        if (this.tabLock3 != 1)
        {
          message = "Lock Tab3";
          return false;
        }

        if (this.tabLock4 != 1)
        {
          message = "Lock Tab4";
          return false;
        }
      }

      if (tab == 1)
      {
        string commandText = string.Empty;
        DataTable dtCheck = new DataTable();
        for (int i = 0; i < ultCancellationInformation.Rows.Count; i++)
        {
          UltraGridRow rowCancel = ultCancellationInformation.Rows[i];
          // Check WO
          commandText = "  SELECT WorkOrderPid";
          commandText += " FROM TblPLNWorkOrderConfirmedDetails";
          commandText += " WHERE WorkOrderPid = " + DBConvert.ParseInt(rowCancel.Cells["WO"].Value.ToString());

          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck == null || dtCheck.Rows.Count == 0)
          {
            message = "WO";
            return false;
          }

          // Check CarcassCode
          commandText = "  SELECT CarcassCode";
          commandText += " FROM TblPLNWorkOrderConfirmedDetails";
          commandText += " WHERE WorkOrderPid = " + DBConvert.ParseInt(rowCancel.Cells["WO"].Value.ToString());
          commandText += " AND CarcassCode = '" + rowCancel.Cells["CarcassCode"].Value.ToString() + "'";

          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck == null || dtCheck.Rows.Count == 0)
          {
            message = "CarcassCode";
            return false;
          }

          // Check ItemCode
          commandText = "  SELECT ItemCode";
          commandText += " FROM TblPLNWorkOrderConfirmedDetails";
          commandText += " WHERE WorkOrderPid = " + DBConvert.ParseInt(rowCancel.Cells["WO"].Value.ToString());
          commandText += " AND CarcassCode = '" + rowCancel.Cells["CarcassCode"].Value.ToString() + "'";
          commandText += " AND ItemCode = '" + rowCancel.Cells["ItemCode"].Value.ToString() + "'";
          commandText += " AND Revision = " + DBConvert.ParseInt(rowCancel.Cells["Revision"].Value.ToString());

          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck == null || dtCheck.Rows.Count == 0)
          {
            message = "ItemCode - Revision";
            return false;
          }
          // Check Close Wo
          //          long wo = DBConvert.ParseInt(rowCancel.Cells["WO"].Value.ToString());
          //          string cacass = rowCancel.Cells["CarcassCode"].Value.ToString();
          //          string item = rowCancel.Cells["ItemCode"].Value.ToString();
          //          int revision = DBConvert.ParseInt(rowCancel.Cells["Revision"].Value.ToString());
          //          commandText = string.Format(@"SELECT WorkOrderPid, CarcassCode, ItemCode, Revision
          //                                     FROM TblPLNWorkOrderConfirmedDetails
          //                                     WHERE WorkOrderPid = {0} AND CarcassCode = '{1}' AND ItemCode = '{2}' AND Revision = {3}
          //                                                              AND (ISNULL(CloseCOM1, 0) = 1 OR ISNULL(CloseCAR, 0) = 1 
          //                                                              OR ISNULL(CloseAll, 0) = 1)", wo, cacass, item, revision);

          //          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          //          if (dtCheck != null && dtCheck.Rows.Count > 0)
          //          {
          //            ultCancellationInformation.Rows[i].Appearance.BackColor = Color.Yellow;
          //            message = " WO is Close";
          //            return false;
          //          }
          // Check Cancel Qty
          if (DBConvert.ParseInt(rowCancel.Cells["QtyCancel"].Value.ToString()) <= 0)
          {
            message = "QtyCancel > 0";
            return false;
          }
          else if (DBConvert.ParseInt(rowCancel.Cells["QtyCancel"].Value.ToString()) > DBConvert.ParseInt(rowCancel.Cells["Qty"].Value.ToString()))
          {
            message = "QtyCancel <= Qty";
            return false;
          }

          // Check Total Qty WO - SO
          int totalWOSO = 0;
          for (int j = 0; j < ultWOSOCancel.Rows.Count; j++)
          {
            UltraGridRow rowWOSO = ultWOSOCancel.Rows[j];
            // Check > 0
            if (DBConvert.ParseInt(rowWOSO.Cells["QtyCancel"].Value.ToString()) <= 0)
            {
              message = "QtyCancel WO-SO > 0";
              return false;
            }
            // Check QtyCancel <= Qty - ShippedQty
            if (DBConvert.ParseInt(rowWOSO.Cells["QtyCancel"].Value.ToString()) >
              DBConvert.ParseInt(rowWOSO.Cells["Qty"].Value.ToString()) - DBConvert.ParseInt(rowWOSO.Cells["QtyShipped"].Value.ToString()))
            {
              message = "QtyCancel <= Qty - QtyShipped";
              return false;
            }
            // Check Total QtyCancel WO-SO = QtyCancel
            if (DBConvert.ParseInt(rowCancel.Cells["WO"].Value.ToString()) == DBConvert.ParseInt(rowWOSO.Cells["WO"].Value.ToString()) &&
              rowCancel.Cells["ItemCode"].Value.ToString() == rowWOSO.Cells["ItemCode"].Value.ToString() &&
              DBConvert.ParseInt(rowCancel.Cells["Revision"].Value.ToString()) == DBConvert.ParseInt(rowWOSO.Cells["Revision"].Value.ToString()))
            {
              totalWOSO = totalWOSO + DBConvert.ParseInt(rowWOSO.Cells["QtyCancel"].Value.ToString());
            }
          }
          // Check
          if (DBConvert.ParseInt(rowCancel.Cells["QtyCancel"].Value.ToString()) != totalWOSO)
          {
            message = "Total QtyCancel WOSO";
            return false;
          }
        }
      }
      else if (tab == 2) // Allocation Material
      {
        for (int i = 0; i < ultPLNAllocateMaterial.Rows.Count; i++)
        {
          UltraGridRow rowParent = ultPLNAllocateMaterial.Rows[i];
          for (int band = 0; band < rowParent.ChildBands.Count; band++)
          {
            for (int j = 0; j < rowParent.ChildBands[band].Rows.Count; j++)
            {
              UltraGridRow row = rowParent.ChildBands[band].Rows[j];
              if (DBConvert.ParseDouble(row.Cells["ReAllocateQty"].Value.ToString()) < 0)
              {
                message = "ReAllocateQty";
                return false;
              }
              else if (DBConvert.ParseDouble(row.Cells["ReAllocateQty"].Value.ToString()) > DBConvert.ParseDouble(row.Cells["NonIssue"].Value.ToString()))
              {
                message = "ReAllocateQty <= NonIssue";
                return false;
              }
            }
          }
        }
      }
      return true;
    }
    /// <summary>
    /// Check Close Wo
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckCloseWo(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      DataTable dtCheck = new DataTable();
      for (int i = 0; i < ultCancellationInformation.Rows.Count; i++)
      {
        UltraGridRow rowCancel = ultCancellationInformation.Rows[i];
        long wo = DBConvert.ParseLong(rowCancel.Cells["WO"].Value.ToString());
        string cacass = rowCancel.Cells["CarcassCode"].Value.ToString();
        string item = rowCancel.Cells["ItemCode"].Value.ToString();
        int revision = DBConvert.ParseInt(rowCancel.Cells["Revision"].Value.ToString());
        commandText = string.Format(@"SELECT WorkOrderPid, CarcassCode, ItemCode, Revision
                                     FROM TblPLNWorkOrderConfirmedDetails
                                     WHERE WorkOrderPid = {0} AND CarcassCode = '{1}' AND ItemCode = '{2}' AND Revision = {3}
                                                              AND (ISNULL(CloseCOM1, 0) = 1 OR ISNULL(CloseCAR, 0) = 1 
                                                              OR ISNULL(CloseAll, 0) = 1)", wo, cacass, item, revision);

        dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          ultCancellationInformation.Rows[i].Appearance.BackColor = Color.Yellow;
          message = " WO is Close";
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
      bool success = this.SaveInfo();
      if (success)
      {
        success = this.SaveDetail();
        if (success)
        {
          success = this.SaveRequestCST();
        }
      }
      return success;
    }

    /// <summary>
    /// Update Qty Work Confirmed Detai
    /// Update Qty WO - SO 
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      if (chkConfirm.Checked)
      {
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@CancelWOPid", DbType.Int64, this.cancelWOPid);
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spPLNCancelWOData_Update", input, output);
        if (DBConvert.ParseLong(output[0].Value.ToString()) <= 0)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save master request cst
    /// </summary>
    /// <returns></returns>
    private bool SaveRequestCST()
    {
      bool success = true;
      if (chkConfirm.Checked && dtRequestCST.Rows.Count > 0)
      {
        DBParameter[] inputParam = new DBParameter[3];
        inputParam[0] = new DBParameter("@Prefix", DbType.String, "CRN");
        inputParam[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        if (txtRemarkRequestCST.Text.Trim().Length > 0)
        {
          inputParam[2] = new DBParameter("@Remark", DbType.String, txtRemarkRequestCST.Text);
        }
        DBParameter[] ouputParam = new DBParameter[2];
        ouputParam[0] = new DBParameter("@RequestOnlineNo", DbType.String, 32, string.Empty);
        ouputParam[1] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure("spPLNCancelWORequestCSTInfo_Insert", inputParam, ouputParam);
        // Gan Lai Pid
        this.requestOnlineCST = ouputParam[0].Value.ToString();
        long requestCSTPid = DBConvert.ParseLong(ouputParam[1].Value.ToString());
        if (requestCSTPid <= 0)
        {
          success = false;
        }
        else
        {
          // Save Detail Request CST
          success = this.SaveDetailRequestCST(requestCSTPid);
        }
      }
      return success;
    }

    /// <summary>
    /// Save Detail Request CST
    /// </summary>
    /// <param name="requestCSTPid"></param>
    /// <returns></returns>
    private bool SaveDetailRequestCST(long requestCSTPid)
    {
      this.dtRequestCST = this.GetComponentOver();
      for (int i = 0; i < dtRequestCST.Rows.Count; i++)
      {
        DataRow row = dtRequestCST.Rows[i];
        DBParameter[] inputParam = new DBParameter[5];
        inputParam[0] = new DBParameter("@RequestPid", DbType.Int64, requestCSTPid);
        inputParam[1] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(row["WO"].ToString()));
        inputParam[2] = new DBParameter("@Carcass", DbType.String, row["CarcassCode"].ToString());
        inputParam[3] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(row["Qty"].ToString()));
        inputParam[4] = new DBParameter("@Component", DbType.String, row["ComponentCode"].ToString());

        DBParameter[] outPutParam = new DBParameter[1];
        outPutParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure("spPLNCancelWORequestCSTDetail_Insert", inputParam, outPutParam);
        if (DBConvert.ParseLong(outPutParam[0].Value.ToString()) < 0)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="tab"></param>
    /// <returns></returns>
    private bool LockData(int tab)
    {
      bool success = this.SaveMaster(tab);
      if (success)
      {
        if (tab == 1)
        {
          success = this.SaveTab1Cancellation();
          if (success)
          {
            success = this.SaveTab1WOSO();
          }
        }
        else if (tab == 2)
        {
          success = this.SaveTab2();
        }
        else if (tab == 4)
        {
          success = this.SaveTab4();
        }
        else if (tab == 5)
        {
          success = this.SaveTab5();
        }
      }
      return success;
    }

    /// <summary>
    /// Save Master Information
    /// </summary>
    /// <param name="tab"></param>
    /// <returns></returns>
    private bool SaveMaster(int tab)
    {
      // Input
      DBParameter[] inputParam = new DBParameter[8];
      if (this.cancelWOPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.cancelWOPid);
      }
      // Code
      inputParam[1] = new DBParameter("@Prefix", DbType.String, "CWO");
      // Reason
      if (txtReason.Text.Trim().Length > 0)
      {
        inputParam[2] = new DBParameter("@Reason", DbType.String, txtReason.Text);
      }
      // Remark
      if (txtRemark.Text.Trim().Length > 0)
      {
        inputParam[3] = new DBParameter("@Remark", DbType.String, txtRemark.Text);
      }

      inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[5] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      inputParam[6] = new DBParameter("@Status", DbType.Int32, 0);

      if (tab == 1)
      {
        inputParam[7] = new DBParameter("@Tab1", DbType.Int32, 1);
      }
      else if (tab == 2)
      {
        inputParam[7] = new DBParameter("@Tab2", DbType.Int32, 1);
      }
      else if (tab == 3)
      {
        inputParam[7] = new DBParameter("@Tab3", DbType.Int32, 1);
      }
      else if (tab == 4)
      {
        inputParam[7] = new DBParameter("@Tab4", DbType.Int32, 1);
      }
      else if (tab == 5)
      {
        inputParam[7] = new DBParameter("@Tab5", DbType.Int32, 1);
      }

      // Output
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPLNCancelWOInformation_Edit", inputParam, outputParam);
      if (DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0)
      {
        return false;
      }
      this.cancelWOPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
      return true;
    }

    /// <summary>
    /// Save Master Information
    /// </summary>
    /// <param name="tab"></param>
    /// <returns></returns>
    private bool SaveInfo()
    {
      // Input
      DBParameter[] inputParam = new DBParameter[7];
      if (this.cancelWOPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.cancelWOPid);
      }
      // Code
      inputParam[1] = new DBParameter("@Prefix", DbType.String, "CWO");
      // Reason
      if (txtReason.Text.Trim().Length > 0)
      {
        inputParam[2] = new DBParameter("@Reason", DbType.String, txtReason.Text);
      }
      // Remark
      if (txtRemark.Text.Trim().Length > 0)
      {
        inputParam[3] = new DBParameter("@Remark", DbType.String, txtRemark.Text);
      }

      inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[5] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      inputParam[6] = new DBParameter("@Status", DbType.Int32, 1);

      if (chkConfirm.Checked)
      {
        inputParam[6] = new DBParameter("@Status", DbType.Int32, 1);
      }
      else
      {
        inputParam[6] = new DBParameter("@Status", DbType.Int32, 0);
      }

      // Output
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPLNCancelWOInformation_Edit", inputParam, outputParam);
      if (DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0)
      {
        return false;
      }
      this.cancelWOPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
      return true;
    }

    /// <summary>
    /// Save Information Cancel WO
    /// </summary>
    /// <returns></returns>
    private bool SaveTab1Cancellation()
    {
      for (int i = 0; i < ultCancellationInformation.Rows.Count; i++)
      {
        UltraGridRow row = ultCancellationInformation.Rows[i];
        DBParameter[] inputParam = new DBParameter[6];
        inputParam[0] = new DBParameter("@CancelNoPid", DbType.Int64, this.cancelWOPid);
        inputParam[1] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(row.Cells["WO"].Value.ToString()));
        inputParam[2] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CarcassCode"].Value.ToString());
        inputParam[3] = new DBParameter("@ItemCode", DbType.String, row.Cells["ItemCode"].Value.ToString());
        inputParam[4] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
        inputParam[5] = new DBParameter("@QtyCancel", DbType.Int32, DBConvert.ParseInt(row.Cells["QtyCancel"].Value.ToString()));

        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spPLNCancelWODetail_Insert", inputParam, outputParam);
        long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save WO - So Cancel
    /// </summary>
    /// <returns></returns>
    private bool SaveTab1WOSO()
    {
      for (int i = 0; i < ultWOSOCancel.Rows.Count; i++)
      {
        UltraGridRow row = ultWOSOCancel.Rows[i];
        DBParameter[] inputParam = new DBParameter[8];
        inputParam[0] = new DBParameter("@CancelNoPid", DbType.Int64, this.cancelWOPid);
        inputParam[1] = new DBParameter("@WOSODetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["WOSODetailPid"].Value.ToString()));
        inputParam[2] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseInt(row.Cells["WO"].Value.ToString()));
        inputParam[3] = new DBParameter("@SODetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["SaleOrderDetailPid"].Value.ToString()));
        inputParam[4] = new DBParameter("@ItemCode", DbType.String, row.Cells["ItemCode"].Value.ToString());
        inputParam[5] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
        inputParam[6] = new DBParameter("@QtyCancel", DbType.Int32, DBConvert.ParseInt(row.Cells["QtyCancel"].Value.ToString()));
        inputParam[7] = new DBParameter("@Remark", DbType.String, row.Cells["Remark"].Value.ToString());
        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spPLNCancelWONoteDetailWithWOSO_Insert", inputParam, outputParam);
        long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Allocation Material
    /// </summary>
    /// <returns></returns> 
    private bool SaveTab2()
    {
      bool success = this.SaveAllocateMaterial();
      if (success)
      {
        success = this.SaveAllocateWoodsVeneer();
      }
      return success;
    }

    /// <summary>
    /// Save Allocate Material
    /// </summary>
    /// <returns></returns>
    private bool SaveAllocateMaterial()
    {
      for (int i = 0; i < ultPLNAllocateMaterial.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultPLNAllocateMaterial.Rows[i];
        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow row = rowParent.ChildBands[0].Rows[j];
          if (DBConvert.ParseDouble(row.Cells["ReAllocateQty"].Value.ToString()) > 0)
          {
            DBParameter[] inputParam = new DBParameter[7];
            inputParam[0] = new DBParameter("@CancelNoPid", DbType.Int64, this.cancelWOPid);
            inputParam[1] = new DBParameter("@Type", DbType.Int32, DBConvert.ParseInt(row.Cells["Type"].Value.ToString()));
            inputParam[2] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(row.Cells["WO"].Value.ToString()));
            inputParam[3] = new DBParameter("@ItemCode", DbType.String, row.Cells["ItemCode"].Value.ToString());
            inputParam[4] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
            inputParam[5] = new DBParameter("@MaterialCode", DbType.String, row.Cells["MaterialCode"].Value.ToString());
            inputParam[6] = new DBParameter("@QtyReAllocate", DbType.Double, DBConvert.ParseDouble(row.Cells["ReAllocateQty"].Value.ToString()));

            DBParameter[] outputParam = new DBParameter[1];
            outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

            DataBaseAccess.ExecuteStoreProcedure("spPLNCancelWONoteDetailWithAllocateMaterial_Insert", inputParam, outputParam);
            long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
            if (result <= 0)
            {
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Save Allocate Woods Veneer
    /// </summary>
    /// <returns></returns>
    private bool SaveAllocateWoodsVeneer()
    {
      for (int i = 0; i < ultAllocateWoodVeneer.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultAllocateWoodVeneer.Rows[i];
        for (int band = 0; band < rowParent.ChildBands.Count; band++)
        {
          for (int j = 0; j < rowParent.ChildBands[band].Rows.Count; j++)
          {
            UltraGridRow row = rowParent.ChildBands[band].Rows[j];
            if (DBConvert.ParseDouble(row.Cells["ReAllocateQty"].Value.ToString()) > 0)
            {
              DBParameter[] inputParam = new DBParameter[9];
              inputParam[0] = new DBParameter("@CancelNoPid", DbType.Int64, this.cancelWOPid);
              inputParam[1] = new DBParameter("@Type", DbType.Int32, DBConvert.ParseInt(row.Cells["Type"].Value.ToString()));
              inputParam[2] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(row.Cells["WO"].Value.ToString()));
              if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 1)
              {
                inputParam[3] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CarcassCode"].Value.ToString());
                inputParam[4] = new DBParameter("@MaterialCode", DbType.String, row.Cells["MainCode"].Value.ToString());
                if (row.Cells["AltCode"].Value.ToString().Length > 0)
                {
                  inputParam[5] = new DBParameter("@AltCode", DbType.String, row.Cells["AltCode"].Value.ToString());
                }
              }
              else if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 2)
              {
                inputParam[3] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CarcassCode"].Value.ToString());
                inputParam[6] = new DBParameter("@MainGroup", DbType.String, row.Cells["MainGroup"].Value.ToString() + "-" + row.Cells["MainCategory"].Value.ToString());
                if (row.Cells["AltGroup"].Value.ToString().Length > 0)
                {
                  inputParam[7] = new DBParameter("@AltGroup", DbType.String, row.Cells["AltGroup"].Value.ToString() + "-" + row.Cells["AltCategory"].Value.ToString());
                }
              }
              inputParam[8] = new DBParameter("@QtyReAllocate", DbType.Double, DBConvert.ParseDouble(row.Cells["ReAllocateQty"].Value.ToString()));

              DBParameter[] outputParam = new DBParameter[1];
              outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

              DataBaseAccess.ExecuteStoreProcedure("spPLNCancelWONoteDetailWithAllocateMaterial_Insert", inputParam, outputParam);
              long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
              if (result <= 0)
              {
                return false;
              }
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Save Request Request Component Store
    /// </summary>
    /// <returns></returns>
    private bool SaveTab4()
    {
      dtRequestCST = this.GetComponentOver();
      return true;
    }

    /// <summary>
    /// Save Request Request Component Store
    /// </summary>
    /// <returns></returns>
    private bool SaveTab5()
    {
      dsRequestITW = (DataSet)ultWIPCAR.DataSource;
      return true;
    }

    /// <summary>
    /// Show User Guide
    /// </summary>
    private void ShowUserGuide()
    {
      // Location
      string location = string.Empty;
      string commandText = string.Empty;
      commandText = String.Format(@"SELECT Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND Code = {1}", Shared.Utility.ConstantClass.GROUP_GNR_PATHFILEUPLOAD, 4);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null)
      {
        location = dt.Rows[0]["Value"].ToString();
      }
      // End

      Process prc = new Process();
      string cancelWONo = txtCancelWONo.Text;
      string locationNew = string.Empty;
      // Location
      location = location + cancelWONo + ".pdf";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string folder = string.Format(@"{0}\Temporary", startupPath);
      if (!Directory.Exists(folder))
      {
        Directory.CreateDirectory(folder);
      }
      locationNew = string.Format(@"{0}\{1}", folder, System.IO.Path.GetFileName(location));

      if (File.Exists(locationNew))
      {
        try
        {
          File.Delete(locationNew);
        }
        catch
        {
          WindowUtinity.ShowMessageWarningFromText("File Is Opening!");
          return;
        }
      }
      File.Copy(location, locationNew);
      prc.StartInfo = new ProcessStartInfo(locationNew);
      try
      {
        prc.Start();
      }
      catch
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0046");
      }
    }

    /// <summary>
    /// Create User Guide
    /// </summary>
    private void CreateUserGuide()
    {
      // Location
      string location = string.Empty;
      string commandText = string.Empty;
      commandText = String.Format(@"SELECT Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND Code = {1}", Utility.GROUP_GNR_PATHFILEUPLOAD, 4);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null)
      {
        location = dt.Rows[0]["Value"].ToString();
      }
      // End

      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@CancelWOPid", DbType.Int64, this.cancelWOPid);

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNRPTCancelWOInformation_Select", input);
      dsPLNCancelWOReport dsSource = new dsPLNCancelWOReport();
      if (ds != null)
      {
        // Cancel Wo Info
        dsSource.Tables["dtCancelInfo"].Merge(ds.Tables[0]);
        // Cancel Wo Detail
        dsSource.Tables["dtCancelDetail"].Merge(ds.Tables[1]);
        // Affect COM1
        dsSource.Tables["dtCOM1"].Merge(ds.Tables[2]);
      }
      if (this.requestOnlineCST.Length > 0)
      {
        dsSource.Tables["dtCancelInfo"].Rows[0]["RequestCSTCode"] = this.requestOnlineCST;
      }

      if (dsRequestITW != null && dsRequestITW.Tables.Count > 0)
      {
        // Affect WIPITEM
        DataRow[] row = dsRequestITW.Tables[1].Select("Select = 1");
        if (row.Length > 0)
        {
          foreach (DataRow row1 in row)
          {
            DataRow row2 = dsSource.Tables["dtWIPCARInfo"].NewRow();
            row2["WO"] = DBConvert.ParseInt(row1["WO"].ToString());
            row2["CarcassCode"] = row1["CarcassCode"].ToString();
            row2["ItemCode"] = row1["ItemCode"].ToString();
            row2["Revision"] = DBConvert.ParseInt(row1["Revision"].ToString());
            row2["PartCode"] = row1["PartCode"].ToString();
            row2["StandByEN"] = row1["StandByEN"].ToString();
            dsSource.Tables["dtWIPCARInfo"].Rows.Add(row2);
          }
        }
        else
        {
          DataRow row2 = dsSource.Tables["dtWIPCARInfo"].NewRow();
          row2["WO"] = 1;
          row2["CarcassCode"] = "";
          row2["ItemCode"] = "";
          row2["Revision"] = 1;
          row2["PartCode"] = "";
          row2["StandByEN"] = "";
          dsSource.Tables["dtWIPCARInfo"].Rows.Add(row2);
        }
      }
      else
      {
        DataRow row2 = dsSource.Tables["dtWIPCARInfo"].NewRow();
        row2["WO"] = 1;
        row2["CarcassCode"] = "";
        row2["ItemCode"] = "";
        row2["Revision"] = 1;
        row2["PartCode"] = "";
        row2["StandByEN"] = "";
        dsSource.Tables["dtWIPCARInfo"].Rows.Add(row2);
      }

      // Affect 
      if (dsSource.Tables["dtCOM1"].Rows.Count > 0)
      {
        dsSource.Tables["dtCancelInfo"].Rows[0]["COM1"] = "Affect";
      }
      else
      {
        dsSource.Tables["dtCancelInfo"].Rows[0]["COM1"] = "Not Affect";
      }

      if (this.requestOnlineCST.Length > 0)
      {
        dsSource.Tables["dtCancelInfo"].Rows[0]["CST"] = "Affect";
      }
      else
      {
        dsSource.Tables["dtCancelInfo"].Rows[0]["CST"] = "Not Affect";
      }

      if (dsRequestITW == null || dsRequestITW.Tables.Count < 2)
      {
        dsSource.Tables["dtCancelInfo"].Rows[0]["WIPITEM"] = "Not Affect";
      }
      else if (dsRequestITW.Tables[1].Rows.Count == 0)
      {
        dsSource.Tables["dtCancelInfo"].Rows[0]["WIPITEM"] = "Not Affect";
      }
      else
      {
        dsSource.Tables["dtCancelInfo"].Rows[0]["WIPITEM"] = "Affect";
      }
      // End Affect

      ReportClass cpt = null;
      cpt = new cptPLNCancelWOInformation();
      cpt.SetDataSource(dsSource);
      cpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, location + txtCancelWONo.Text + ".pdf");
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Init layout Information Cancellation 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCancellationInformation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Columns["Remark"].Hidden = true;

      e.Layout.Bands[0].Columns["COM1"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CST"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["WIPCAR"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SUB"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ITW"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["WIPITEM"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["WO"].ValueList = ultDropWO;
      e.Layout.Bands[0].Columns["CarcassCode"].ValueList = ultDropCarcassCode;
      e.Layout.Bands[0].Columns["ItemCode"].ValueList = ultDropItemCode;

      e.Layout.Bands[0].Columns["COM1"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["CST"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["WIPCAR"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["SUB"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["ITW"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["WIPITEM"].CellAppearance.ForeColor = Color.Blue;

      e.Layout.Bands[0].Columns["QtyCancel"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["QtyCancel"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      if (this.tabLock1 == 1)
      {
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init layout WO - SO Cancel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultWOSOCancel_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["WOSODetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleOrderDetailPid"].Hidden = true;

      e.Layout.Bands[0].Columns["WO"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SaleNo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PONo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyShipped"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["QtyCancel"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["QtyCancel"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyShipped"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      if (this.tabLock1 == 1)
      {
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init layout Allocation Material
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultPLNAllocateMaterial_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[1].Columns["Type"].Hidden = true;
      e.Layout.Bands[1].Columns["ReAllocateQty"].CellAppearance.BackColor = Color.Aqua;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
      }

      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[1].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          if (i < e.Layout.Bands[1].Columns.Count - 2)
          {
            e.Layout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
          }
        }
      }

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      if (this.tabLock2 == 1)
      {
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
        e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init Layout Allocate Woods, Veneer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultAllocateWoodVeneer_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[1].Columns["Type"].Hidden = true;
      e.Layout.Bands[2].Columns["Type"].Hidden = true;

      e.Layout.Bands[1].Columns["ReAllocateQty"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[2].Columns["ReAllocateQty"].CellAppearance.BackColor = Color.Aqua;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
      }

      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[1].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          if (i < e.Layout.Bands[1].Columns.Count - 2)
          {
            e.Layout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
          }
        }
      }

      for (int i = 0; i < e.Layout.Bands[2].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[2].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[2].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          if (i < e.Layout.Bands[2].Columns.Count - 2)
          {
            e.Layout.Bands[2].Columns[i].CellActivation = Activation.ActivateOnly;
          }
        }
      }

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      if (this.tabLock2 == 1)
      {
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
        e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
        e.Layout.Bands[2].Override.AllowUpdate = DefaultableBoolean.False;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init layout COM1
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCOM1_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["QtyCancel"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init layout COM1 Stock Balance
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComponentStock_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["WO"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CarcassCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Stock"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyCancel"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Stock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyCancel"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init layout Request CST
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultRequestCST_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init layout WIPCAR
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultWIPCAR_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["QtyCancel"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[1].Columns["WO"].Hidden = true;
      e.Layout.Bands[1].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[1].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[1].Columns["Revision"].Hidden = true;
      e.Layout.Bands[1].Columns["WorkAreaPid"].Hidden = true;

      e.Layout.Bands[1].Columns["Partcode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["StandByEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[1].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init layout Request ITW
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultRequestITW_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;


      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Active Cell Cancellation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCancellationInformation_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "CarcassCode":
          if (DBConvert.ParseInt(row.Cells["WO"].Value.ToString()) != Int32.MinValue)
          {
            this.LoadCarcassCode(DBConvert.ParseInt(row.Cells["WO"].Value.ToString()));
          }
          else
          {
            this.LoadCarcassCode(-1);
          }
          break;
        case "ItemCode":
          if (DBConvert.ParseInt(row.Cells["WO"].Value.ToString()) != Int32.MinValue &&
             row.Cells["CarcassCode"].Value.ToString().Length > 0)
          {
            this.LoadItemCode(DBConvert.ParseInt(row.Cells["WO"].Value.ToString()), row.Cells["CarcassCode"].Value.ToString());
          }
          else
          {
            this.LoadItemCode(-1, "");
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// After cell update Cancellation 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCancellationInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "WO":
          row.Cells["CarcassCode"].Value = DBNull.Value;
          row.Cells["ItemCode"].Value = DBNull.Value;
          row.Cells["Revision"].Value = DBNull.Value;
          row.Cells["Qty"].Value = DBNull.Value;
          break;
        case "CarcassCode":
          row.Cells["ItemCode"].Value = DBNull.Value;
          row.Cells["Revision"].Value = DBNull.Value;
          row.Cells["Qty"].Value = DBNull.Value;
          break;
        case "ItemCode":
          string commandText = string.Empty;
          commandText = " SELECT DISTINCT ItemCode, Revision";
          commandText += " FROM TblPLNWorkOrderConfirmedDetails";
          commandText += " WHERE WorkOrderPid = " + DBConvert.ParseInt(row.Cells["WO"].Value.ToString());
          commandText += " AND CarcassCode  = '" + row.Cells["CarcassCode"].Value.ToString() + "'";
          commandText += " AND ItemCode  = '" + row.Cells["ItemCode"].Value.ToString() + "'";
          System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtSource != null && dtSource.Rows.Count > 0)
          {
            row.Cells["Revision"].Value = DBConvert.ParseInt(ultDropItemCode.SelectedRow.Cells["Revision"].Value.ToString());
            row.Cells["Qty"].Value = DBConvert.ParseInt(ultDropItemCode.SelectedRow.Cells["Qty"].Value.ToString());
          }
          else
          {
            row.Cells["Revision"].Value = DBNull.Value;
            row.Cells["Qty"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Before Cell Update WO - SO cancel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultWOSOCancel_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "QtyCancel":
          if (DBConvert.ParseInt(row.Cells["QtyCancel"].Text) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Qty Cancel > 0");
            e.Cancel = true;
          }
          else if (DBConvert.ParseInt(row.Cells["QtyCancel"].Text) >
                  DBConvert.ParseInt(row.Cells["Qty"].Value.ToString()) - DBConvert.ParseInt(row.Cells["QtyShipped"].Value.ToString()))
          {
            WindowUtinity.ShowMessageError("ERR0001", "Qty Cancel <= " + (DBConvert.ParseInt(row.Cells["Qty"].Value.ToString()) - DBConvert.ParseInt(row.Cells["QtyShipped"].Value.ToString())) + "");
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    private void ultPLNAllocateMaterial_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "ReAllocateQty":
          if (DBConvert.ParseDouble(row.Cells["ReAllocateQty"].Text) < 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "ReAllocateQty");
            e.Cancel = true;
          }
          else if (DBConvert.ParseDouble(row.Cells["ReAllocateQty"].Text) > DBConvert.ParseDouble(row.Cells["NonIssue"].Value.ToString()))
          {
            WindowUtinity.ShowMessageError("ERR0001", "ReAllocateQty <= " + row.Cells["NonIssue"].Value.ToString() + "");
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Before Cell Update Cancel WO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCancellationInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string value = e.NewValue.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "ItemCode":
          for (int i = 0; i < ultCancellationInformation.Rows.Count; i++)
          {
            UltraGridRow rowGird = ultCancellationInformation.Rows[i];
            if (rowGird != row &&
              DBConvert.ParseInt(rowGird.Cells["WO"].Value.ToString()) == DBConvert.ParseInt(row.Cells["WO"].Value.ToString()) &&
              rowGird.Cells["CarcassCode"].Value.ToString() == row.Cells["CarcassCode"].Value.ToString() &&
              rowGird.Cells["ItemCode"].Value.ToString() == row.Cells["ItemCode"].Text.ToString() &&
              DBConvert.ParseInt(rowGird.Cells["Revision"].Value.ToString()) == DBConvert.ParseInt(ultDropItemCode.SelectedRow.Cells["Revision"].Value.ToString()))
            {
              WindowUtinity.ShowMessageError("ERR0029", "WO - CarcassCode - ItemCode");
              e.Cancel = true;
              return;
            }
          }
          break;
        default:
          break;
      }
    }
    /// <summary>
    /// Change Tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (tabControl1.SelectedTab == tabControl1.TabPages["tabPLNWOData"])
      {
        this.LoadTab1(this.cancelWOPid);
      }

      else if (tabControl1.SelectedTab == tabControl1.TabPages["tabPLNAllocateMaterial"])
      {
        if (this.cancelWOPid != long.MinValue)
        {
          this.LoadTab2(this.cancelWOPid);
        }
      }

      else if (tabControl1.SelectedTab == tabControl1.TabPages["tabCOM1"])
      {
        if (this.cancelWOPid != long.MinValue)
        {
          this.LoadTab3(this.cancelWOPid);
        }
      }

      else if (tabControl1.SelectedTab == tabControl1.TabPages["tabCST"])
      {
        if (this.cancelWOPid != long.MinValue)
        {
          this.LoadTab4(this.cancelWOPid);
        }
      }

      else if (tabControl1.SelectedTab == tabControl1.TabPages["tabWIPCAR"])
      {
        if (this.cancelWOPid != long.MinValue)
        {
          this.LoadTab5(this.cancelWOPid);
        }
      }
    }

    /// <summary>
    /// Load Tab1
    /// </summary>
    /// <param name="cancelWOPid"></param>
    private void LoadTab1(long cancelWOPid)
    {
      // Load Main
      this.LoadMainData();

      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@CancelWOPid", DbType.Int64, this.cancelWOPid);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNWOCancelGetInfoTab1_Select", 3600, inputParam);
      if (dsSource != null)
      {
        ultCancellationInformation.DataSource = dsSource.Tables[0];
        ultWOSOCancel.DataSource = dsSource.Tables[1];
      }

      // Set Status Control
      this.SetStatusControl();
    }

    /// <summary>
    /// Load tab2
    /// </summary>
    /// <param name="cancelWOPid"></param>
    private void LoadTab2(long cancelWOPid)
    {
      // Load Main
      this.LoadMainData();

      // Load Allocate Material
      this.LoadAllocateMaterial();

      // Load Allocate Woods, Veneer
      this.LoadAllocateWoodVeneer();

      // Set Status Control
      this.SetStatusControl();
    }

    /// <summary>
    /// Load Allocate Material
    /// </summary>
    private void LoadAllocateMaterial()
    {
      DataSet dsSource = this.CreateDataSet();
      // Load Parent
      string commandText = string.Empty;
      commandText = " SELECT CAST(WO AS bigint) WO, CarcassCode, ItemCode, Revision, QtyCancel";
      commandText += " FROM TblPLNCancelWONoteDetail";
      commandText += " WHERE CancelNoPid = " + cancelWOPid;
      DataTable dtParent = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtParent != null)
      {
        dsSource.Tables["dtParent"].Merge(dtParent);
      }
      // Load Material
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@CancelPid", DbType.Int64, cancelWOPid);

      DataTable dtChildMaterial = DataBaseAccess.SearchStoreProcedureDataTable("spPLNCancelWOAllocateMaterial_Select", 3600, inputParam);
      if (dtChildMaterial != null)
      {
        dsSource.Tables["dtChildMaterial"].Merge(dtChildMaterial);
      }

      ultPLNAllocateMaterial.DataSource = dsSource;
    }

    /// <summary>
    /// Load Allocate Woods, Veneer
    /// </summary>
    private void LoadAllocateWoodVeneer()
    {
      DataSet dsSource = this.CreateDataSetAllocateWoodVeneer();
      // Load Parent
      string commandText = string.Empty;
      commandText = "  SELECT CAST(WO AS bigint) WO, CarcassCode, SUM(QtyCancel) QtyCancel";
      commandText += " FROM TblPLNCancelWONoteDetail";
      commandText += " WHERE CancelNoPid = " + cancelWOPid;
      commandText += " GROUP BY WO, CarcassCode";
      DataTable dtParent = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtParent != null)
      {
        dsSource.Tables["dtParent"].Merge(dtParent);
      }
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@CancelPid", DbType.Int64, cancelWOPid);

      // Load Veneer
      DataTable dtChildVeneer = DataBaseAccess.SearchStoreProcedureDataTable("spPLNCancelWOAllocateVeneer_Select", 3600, inputParam);
      if (dtChildVeneer != null)
      {
        dsSource.Tables["dtChildVeneer"].Merge(dtChildVeneer);
      }
      // Load Wood
      DataTable dtChildWood = DataBaseAccess.SearchStoreProcedureDataTable("spPLNCancelWOAllocateWoods_Select", 3600, inputParam);
      if (dtChildWood != null)
      {
        dsSource.Tables["dtChildWood"].Merge(dtChildWood);
      }

      ultAllocateWoodVeneer.DataSource = dsSource;
      // To Mau
      for (int i = 0; i < ultAllocateWoodVeneer.Rows.Count; i++)
      {
        UltraGridRow row = ultAllocateWoodVeneer.Rows[i];
        for (int bank = 0; bank < row.ChildBands.Count; bank++)
        {
          for (int j = 0; j < row.ChildBands[bank].Rows.Count; j++)
          {
            UltraGridRow rowChild = row.ChildBands[bank].Rows[j];
            if (DBConvert.ParseInt(rowChild.Cells["Type"].Value.ToString()) == 1)
            {
              rowChild.Appearance.BackColor = Color.Yellow;
            }
          }
        }
      }
    }
    /// <summary>
    /// Load tab3
    /// </summary>
    /// <param name="cancelWOPid"></param>
    private void LoadTab3(long cancelWOPid)
    {
      // Load Main
      this.LoadMainData();

      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@CancelWOPid", DbType.Int64, this.cancelWOPid);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNWOCancelGetInfoTab2_Select", 3600, inputParam);
      if (dtSource != null)
      {
        ultCOM1.DataSource = dtSource;
      }

      // Set Status Control
      this.SetStatusControl();
    }

    /// <summary>
    /// Load tab4
    /// </summary>
    /// <param name="cancelWOPid"></param>
    private void LoadTab4(long cancelWOPid)
    {
      // Load Main
      this.LoadMainData();

      // Load Info Master Request Online CST
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHDGetNewRequestCodeCarcass('CRN') Code");
      if ((dt != null) && (dt.Rows.Count > 0))
      {
        txtRequestNoCST.Text = dt.Rows[0]["Code"].ToString();
        txtRequestByCST.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
        txtRequestDateCST.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
      }

      string commandText = string.Empty;
      // Load Information Stock Balance Component
      commandText = string.Format(@"SELECT CWO.WO, CWO.CarcassCode, CWO.QtyCancel, ISNULL(TOC.StockQty, 0) Stock
                                    FROM 
                                    (
	                                    SELECT WO, CarcassCode, SUM(QtyCancel) QtyCancel
	                                    FROM TblPLNCancelWONoteDetail
	                                    WHERE CancelNoPid = {0}
	                                    GROUP BY WO, CarcassCode
                                    )CWO 
                                    LEFT JOIN VWIPCSTStockBalance TOC ON TOC.Wo = CWO.WO AND TOC.CarcassCode = CWO.CarcassCode", cancelWOPid);
      DataTable dtStock = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtStock != null)
      {
        ultComponentStock.DataSource = dtStock;
      }

      DataTable dtMain = this.GetComponentOver();
      ultRequestCST.DataSource = dtMain;

      // Set Status Control
      this.SetStatusControl();
    }

    /// <summary>
    /// Load tab5
    /// </summary>
    /// <param name="cancelWOPid"></param>
    private void LoadTab5(long cancelWOPid)
    {
      // Load Main
      this.LoadMainData();
      if (dsRequestITW != null && dsRequestITW.Tables.Count > 0)
      {
        ultWIPCAR.DataSource = dsRequestITW;
      }
      else
      {
        // Load Info Furniture
        DBParameter[] inputParam = new DBParameter[1];
        inputParam[0] = new DBParameter("@CancelWOPid", DbType.Int64, this.cancelWOPid);

        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNWOGetInfoFurnitureForRequestITW_Select", 3600, inputParam);
        if (dsSource != null)
        {
          DataSet ds = this.CreateDataSetForRequestITW();
          ds.Tables["dtParent"].Merge(dsSource.Tables[0]);
          ds.Tables["dtChild"].Merge(dsSource.Tables[1]);
          ultWIPCAR.DataSource = ds;
        }
      }
      // Set Status Control
      this.SetStatusControl();
    }

    /// <summary>
    /// Get Component Over
    /// </summary>
    /// <returns></returns>
    private DataTable GetComponentOver()
    {
      DataTable dtRequest = this.CreateTableRequestCST();
      string commandText = string.Format(@"SELECT CC.WO Wo, CC.CarcassCode Carcass, WOC.Qty - CC.Qty AS Qty
                                      FROM (
                                        SELECT CWO.WO, CWO.CarcassCode, SUM(CWO.QtyCancel) Qty
                                        FROM TblPLNCancelWONoteDetail CWO
                                        WHERE CWO.CancelNoPid = {0}
                                        GROUP BY CWO.WO, CWO.CarcassCode
                                      )CC
                                      INNER JOIN (
                                        SELECT WorkOrderPid, CarcassCode, SUM(Qty) Qty
                                        FROM TblPLNWorkOrderConfirmedDetails
                                        GROUP BY WorkOrderPid, CarcassCode
                                      )WOC ON WOC.WorkOrderPid = CC.WO AND WOC.CarcassCode = CC.CarcassCode", cancelWOPid);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataTable dtSource = new DataTable();
      if (dt != null)
      {
        dtRequest.Merge(dt);
        SqlDBParameter[] inputParam = new SqlDBParameter[1];
        inputParam[0] = new SqlDBParameter("@TableImport", SqlDbType.Structured, dtRequest);
        dtSource = Shared.DataBaseUtility.SqlDataBaseAccess.SearchStoreProcedureDataTable("spWIPOverComponentFromCST_Select", inputParam);
      }
      return dtSource;
    }
    /// <summary>
    /// Check Change Status Cancel WO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCheckStatus_Click(object sender, EventArgs e)
    {
      // Check Qty Cancel
      for (int i = 0; i < ultCancellationInformation.Rows.Count; i++)
      {
        UltraGridRow row = ultCancellationInformation.Rows[i];
        if (row.Cells["QtyCancel"].Text.Length == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "QtyCanel");
          return;
        }
        else if (DBConvert.ParseInt(row.Cells["QtyCancel"].Value.ToString()) <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "QtyCancel > 0");
          return;
        }
        else if (DBConvert.ParseInt(row.Cells["QtyCancel"].Value.ToString()) > DBConvert.ParseInt(row.Cells["Qty"].Value.ToString()))
        {
          WindowUtinity.ShowMessageError("ERR0001", "QtyCancel <= Qty");
          return;
        }
      }
      // End Check

      DataTable dt = (DataTable)ultCancellationInformation.DataSource;
      DataTable dtCheck = this.CreateTableCheckStatus();
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow row = dt.Rows[i];
        if (row.RowState != DataRowState.Deleted)
        {
          if (DBConvert.ParseInt(row["WO"].ToString()) != int.MinValue &&
                      row["CarcassCode"].ToString().Length > 0 &&
                      row["ItemCode"].ToString().Length > 0 &&
                      DBConvert.ParseInt(row["Revision"].ToString()) != int.MinValue &&
                      DBConvert.ParseInt(row["QtyCancel"].ToString()) != int.MinValue)
          {
            DataRow rowCheck = dtCheck.NewRow();
            rowCheck["WO"] = DBConvert.ParseInt(row["WO"].ToString());
            rowCheck["CarcassCode"] = row["CarcassCode"].ToString();
            rowCheck["ItemCode"] = row["ItemCode"].ToString();
            rowCheck["Revision"] = DBConvert.ParseInt(row["Revision"].ToString());
            rowCheck["Qty"] = DBConvert.ParseInt(row["Qty"].ToString());
            rowCheck["QtyCancel"] = DBConvert.ParseInt(row["QtyCancel"].ToString());
            rowCheck["Remark"] = row["Remark"].ToString();
            dtCheck.Rows.Add(rowCheck);
          }
        }
      }

      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@TableImport", SqlDbType.Structured, dtCheck);
      DataSet dsSource = Shared.DataBaseUtility.SqlDataBaseAccess.SearchStoreProcedure("spPLNCancelWOInfluenceData_Select", 3600, inputParam);
      if (dsSource != null)
      {
        // Status WIP
        ultCancellationInformation.DataSource = dsSource.Tables[0];
        //WO - SO
        ultWOSOCancel.DataSource = dsSource.Tables[1];
      }
    }

    /// <summary>
    /// Load tab1
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLockData1_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckValid(out message, 1);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = this.LockData(1);
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      this.LoadTab1(this.cancelWOPid);
    }

    /// <summary>
    /// Lock Tab2
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLockData2_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckValid(out message, 2);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = this.LockData(2);
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      this.LoadTab2(this.cancelWOPid);
    }

    /// <summary>
    /// Load tab3
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLockData3_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckValid(out message, 3);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = this.LockData(3);
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      this.LoadTab3(this.cancelWOPid);
    }

    /// <summary>
    /// Load tab4
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLockData4_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckValid(out message, 4);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = this.LockData(4);
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      this.LoadTab4(this.cancelWOPid);
    }

    /// <summary>
    /// Lock tab5
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLockData5_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckValid(out message, 5);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = this.LockData(5);
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      this.LoadTab5(this.cancelWOPid);
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      // Check Valid
      if (this.tabLock1 != 1)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Lock Tab1");
        return;
      }
      else if (this.tabLock2 != 1)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Lock Tab2");
        return;
      }
      else if (this.tabLock3 != 1)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Lock Tab3");
        return;
      }
      else if (this.tabLock4 != 1)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Lock Tab4");
        return;
      }
      else if (this.tabLock5 != 1)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Lock Tab5");
        return;
      }
      // Check close WO
      //string message = string.Empty;
      //bool success = this.CheckCloseWo(out message);
      //if (!success)
      //{
      //  WindowUtinity.ShowMessageError("ERR0001", message);
      //  return;
      //}
      // Save Data
      bool success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      // Create User Guide
      this.CreateUserGuide();
      // Show User Guide
      this.ShowUserGuide();
      // Load Main Data
      this.LoadMainData();

    }

    /// <summary>
    /// User Guide Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void linkLabUserGuide_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      this.ShowUserGuide();
    }

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (this.cancelWOPid == long.MinValue)
      {
        return;
      }

      if (WindowUtinity.ShowMessageConfirm("MSG0007", txtCancelWONo.Text).ToString() == "No")
      {
        return;
      }
      if (this.cancelWOPid != long.MinValue)
      {
        string storeName = "spPLNCancelWOInformation_Delete";
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@CancelWOPid", DbType.Int64, this.cancelWOPid);

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
          WindowUtinity.ShowMessageError("ERR0093", txtCancelWONo.Text);
          return;
        }
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
    }
    #endregion Event

    private void groupBox1_Enter(object sender, EventArgs e)
    {

    }

    private void btnBrowseItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcel.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "PLN_02_019";
      string sheetName = "WOCancel";
      string outFileName = "WOCancel";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtImportExcel.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }
      // Get Items Count

      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [WOCancel (1)$I3:I4]");
      int itemCount = 0;
      if (dsCount == null || dsCount.Tables.Count == 0 || dsCount.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Items Count");
        return;
      }
      else
      {
        itemCount = DBConvert.ParseInt(dsCount.Tables[0].Rows[0][0]);
        if (itemCount <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Items Count");
          return;
        }
      }

      // Get data for items list
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), string.Format("SELECT * FROM [WOCancel (1)$B5:H{0}]", itemCount + 5));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      //input data
      DataSet ds1 = FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), string.Format("SELECT * FROM [WOCancel (1)$B5:H{0}]", itemCount + 5));
      if (ds1 == null || ds1.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      DataTable data = ds1.Tables[0];
      DataTable dtInput = this.dtWOCancelResult();
      for (int k = 0; k < data.Rows.Count; k++)
      {
        long wo1 = DBConvert.ParseLong(data.Rows[k]["WO"].ToString());
        string itemCode1 = data.Rows[k]["ItemCode"].ToString();
        int revision1 = DBConvert.ParseInt(data.Rows[k]["Revision"].ToString());
        string carcassCode1 = data.Rows[k]["CarcassCode"].ToString();
        int qty1 = DBConvert.ParseInt(data.Rows[k]["Qty"].ToString());
        int qtyCancel = DBConvert.ParseInt(data.Rows[k]["QtyCancel"].ToString());
        string remark = data.Rows[k]["Remark"].ToString();
        if (itemCode1.Trim().Length > 0)
        {
          DataRow[] rowCheckExcel = data.Select(string.Format("Wo = {0} and ItemCode = '{1}' and Revision = {2} and CarcassCode = '{3}'", wo1, itemCode1, revision1, carcassCode1));
          if (rowCheckExcel.Length > 1)
          {
            WindowUtinity.ShowMessageError("ERR0013", string.Format("Wo = {0} and ItemCode = '{1}' and Revision = {2} and CarcassCode = '{3}'", wo1, itemCode1, revision1, carcassCode1));
            return;
          }
          string cmtext = string.Format(@"SELECT WorkOrderPid Wo, ItemCode, Revision, CarcassCode, Qty 
                                        FROM TblPLNWorkOrderConfirmedDetails
                                        WHERE WorkOrderPid = {0} 
	                                        AND ItemCode = '{1}' 
	                                        AND Revision = {2} 
	                                        AND CarcassCode = '{3}'
	                                        AND Qty = {4}", wo1, itemCode1, revision1, carcassCode1, qty1);
          DataTable dtWOInfo = DataBaseAccess.SearchCommandTextDataTable(cmtext);
          long wo = long.MinValue;
          string carcassCode = string.Empty;
          string itemCode = string.Empty;
          int revision = int.MinValue;
          int qty = int.MinValue;
          if (dtWOInfo != null && dtWOInfo.Rows.Count > 0)
          {
            wo = DBConvert.ParseLong(dtWOInfo.Rows[0]["Wo"].ToString());
            carcassCode = dtWOInfo.Rows[0]["CarcassCode"].ToString();
            itemCode = dtWOInfo.Rows[0]["ItemCode"].ToString();
            revision = DBConvert.ParseInt(dtWOInfo.Rows[0]["Revision"].ToString());
            qty = DBConvert.ParseInt(dtWOInfo.Rows[0]["Qty"].ToString());
            if (qtyCancel < 0 || qtyCancel > qty || qtyCancel > qty1)
            {
              WindowUtinity.ShowMessageError("ERR0150", "Cancel Qty", string.Format(@"Wo: {0}, CarcassCode: {1}, ItemCode: {2}, Revision: {3}", wo.ToString(), carcassCode, itemCode, revision.ToString()));
              return;
            }
            if (qty < 0 || qty1 < 0)
            {
              WindowUtinity.ShowMessageError("ERR0150", "Qty", string.Format(@"Wo: {0}, CarcassCode: {1}, ItemCode: {2}, Revision: {3}", wo.ToString(), carcassCode, itemCode, revision.ToString()));
              return;
            }
          }
          else
          {
            WindowUtinity.ShowMessageError("ERR0001", string.Format(@"Wo: {0}, CarcassCode: {1}, ItemCode: {2}, Revision: {3}", wo1.ToString(), carcassCode1, itemCode1, revision1.ToString()));
            return;
          }
          // Add Data
          DataRow rowadd = dtInput.NewRow();
          rowadd["WO"] = wo;
          rowadd["CarcassCode"] = carcassCode;
          rowadd["ItemCode"] = itemCode;
          rowadd["Revision"] = revision;
          rowadd["Qty"] = qty;
          rowadd["QtyCancel"] = qtyCancel;
          rowadd["Remark"] = remark;
          dtInput.Rows.Add(rowadd);
        }
      }
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@TableImport", SqlDbType.Structured, dtInput);
      DataSet dsSource = Shared.DataBaseUtility.SqlDataBaseAccess.SearchStoreProcedure("spPLNCancelWOImport_Select", 3600, inputParam);
      if (dsSource != null)
      {
        // Status WIP
        ultCancellationInformation.DataSource = dsSource.Tables[0];
        //WO - SO
        ultWOSOCancel.DataSource = dsSource.Tables[1];
      }

    }

    private DataTable dtWOCancelResult()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("WO", typeof(System.Int64));
      dt.Columns.Add("CarcassCode", typeof(System.String));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("Qty", typeof(System.Int32));
      dt.Columns.Add("QtyCancel", typeof(System.Int32));
      dt.Columns.Add("Remark", typeof(System.String));
      return dt;
    }
  }
}
