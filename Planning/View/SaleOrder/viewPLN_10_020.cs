/*
  Author        : 
  Create date   : 04/11/2013
  Decription    : Hold SO
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

namespace DaiCo.Planning
{
  public partial class viewPLN_10_020 : MainUserControl
  {
    #region Field
    public long holdPid = long.MinValue;

    private int status = int.MinValue;
    private int statusTab1 = int.MinValue;
    private int statusTab2 = int.MinValue;
    private int statusTab3 = int.MinValue;
    private bool typeHold = true;

    #endregion Field

    #region Init
    public viewPLN_10_020()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load View
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_10_020_Load(object sender, EventArgs e)
    {
      this.LoadInit();
      this.LoadMain();
      this.LoadTab1();
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
      commandText = " SELECT Pid, CustomerCode, Name, CustomerCode + ' - ' + Name As [Description] FROM TblCSDCustomerInfo";

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
    /// Load Main
    /// </summary>
    private void LoadMain()
    {
      DBParameter[] inputParam = new DBParameter[1] { new DBParameter("@Pid", DbType.Int64, this.holdPid) };
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spCSDSOHoldInfomation_Select", inputParam);
      if (ds != null)
      {
        DataTable dtMain = ds.Tables[0];
        ultCBCustomer.Value = DBConvert.ParseLong(dtMain.Rows[0]["CustomerPid"].ToString());
        txtHoldPONote.Text = dtMain.Rows[0]["HoldNo"].ToString();
        txtReason.Text = dtMain.Rows[0]["Reason"].ToString();
        txtRemark.Text = dtMain.Rows[0]["Remark"].ToString();
        txtCreateBy.Text = dtMain.Rows[0]["CreateBy"].ToString();
        txtCreateDate.Text = dtMain.Rows[0]["CreateDate"].ToString();
        this.status = DBConvert.ParseInt(dtMain.Rows[0]["Status"].ToString());
        this.statusTab1 = DBConvert.ParseInt(dtMain.Rows[0]["Tab1"].ToString());
        this.statusTab2 = DBConvert.ParseInt(dtMain.Rows[0]["Tab2"].ToString());
        this.statusTab3 = DBConvert.ParseInt(dtMain.Rows[0]["Tab3"].ToString());
        if (DBConvert.ParseInt(dtMain.Rows[0]["Type"].ToString()) == 1)
        {
          this.typeHold = true;
        }
        else
        {
          this.typeHold = false;
        }
      }
    }

    /// <summary>
    /// Load Tab 1
    /// </summary>
    private void LoadTab1()
    {
      //Load Main Data
      this.LoadMain();

      // Load Detail Tab 1
      DBParameter[] inputParam = new DBParameter[1] { new DBParameter("@Pid", DbType.Int64, this.holdPid) };
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spCSDSOHoldInfomation_Select", inputParam);
      if (ds != null)
      {
        ultData.DataSource = ds.Tables[1];

        // Load Data Container
        if (this.status == 1 && this.typeHold == true)
        {
          this.GetDataContainer();
        }

        // Check Select All
        if (this.statusTab1 == 0)
        {
          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            ultData.Rows[i].Cells["PLNApproved"].Value = 1;
          }
          chkSelectAll.Checked = true;
        }

        this.SetStatusControl();
      }
    }

    /// <summary>
    /// Load Tab 2
    /// </summary>
    private void LoadTab2()
    {
      //Load Main Data
      this.LoadMain();

      // Load Detail Tab 2
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@HoldPid", DbType.Int64, this.holdPid);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDHoldSOGetInfoWOSO_Select", input);
      if (dsSource != null)
      {
        DataSet dsMain = this.DataSetWOSO();
        dsMain.Tables["dtParent"].Merge(dsSource.Tables[0]);
        dsMain.Tables["dtChild"].Merge(dsSource.Tables[1]);
        ultWOSO.DataSource = dsMain;
      }
      this.SetStatusControl();
    }


    /// <summary>
    /// Load Tab 3
    /// </summary>
    private void LoadTab3()
    {
      //Load Main Data
      this.LoadMain();

      // Load Detail Tab 3
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@HoldPid", DbType.Int64, this.holdPid);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDHoldSOGetInfoWOSOUnHold_Select", input);
      if (dsSource != null)
      {
        DataSet dsMain = this.DataSetWOSOUnHold();
        dsMain.Tables["dtParent"].Merge(dsSource.Tables[0]);
        dsMain.Tables["dtChild"].Merge(dsSource.Tables[1]);
        ultUnHold.DataSource = dsMain;
      }
      this.SetStatusControl();
    }

    /// <summary>
    /// Lock Tab 1
    /// </summary>
    /// <returns></returns>
    private bool LockTab1()
    {
      bool success = this.SaveMaster(1);
      if (success)
      {
        success = this.SaveDetail();
        if (success)
        {
          this.UpdateDataWhenHoldSO();
        }
      }
      return success;
    }

    /// <summary>
    /// Lock Tab 2
    /// </summary>
    /// <returns></returns>
    private bool LockTab2()
    {
      bool success = this.SaveMaster(2);
      return success;
    }

    /// <summary>
    /// Lock Tab 3
    /// </summary>
    /// <returns></returns>
    private bool LockTab3()
    {
      bool success = this.SaveMaster(3);
      return success;
    }

    /// <summary>
    /// Update SO Detail
    /// Update Qty Container
    /// </summary>
    /// <returns></returns>
    private bool UpdateDataWhenHoldSO()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@HoldPid", DbType.Int64, this.holdPid);

      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spCSDSOHoldWhenPlanningConfirm_Update", inputParam, outputParam);
      if (DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Update WO - SO When Hold SO
    /// </summary>
    /// <returns></returns>
    private bool UpdateWOSOWhenHoldSO()
    {
      for (int i = 0; i < ultWOSO.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultWOSO.Rows[i];
        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowWO = rowParent.ChildBands[0].Rows[j];
          for (int k = 0; k < rowWO.ChildBands[0].Rows.Count; k++)
          {
            UltraGridRow rowChild = rowWO.ChildBands[0].Rows[k];
            if (DBConvert.ParseInt(rowChild.Cells["UnRelease"].Value.ToString()) > 0)
            {
              DBParameter[] input = new DBParameter[4];
              input[0] = new DBParameter("@WOSOPid", DbType.Int64, DBConvert.ParseLong(rowWO.Cells["Pid"].Value.ToString()));
              input[1] = new DBParameter("@SODetailPid", DbType.Int64, DBConvert.ParseLong(rowChild.Cells["SODetailPid"].Value.ToString()));
              input[2] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(rowChild.Cells["UnRelease"].Value.ToString()));
              input[3] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);

              DBParameter[] output = new DBParameter[1];
              output[0] = new DBParameter("@Result", DbType.Int64, Int64.MinValue);
              DataBaseAccess.ExecuteStoreProcedure("spPLNDecreaseQtyWOSOWhenHoldSO_Update", input, output);
              long result = DBConvert.ParseLong(output[0].Value.ToString());
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
    /// Update WO - SO When UnHold SO
    /// </summary>
    /// <returns></returns>
    private bool UpdateWOSOWhenUnHoldSO()
    {
      for (int i = 0; i < ultUnHold.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultUnHold.Rows[i];
        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = rowParent.ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(rowChild.Cells["QtySwap"].Value.ToString()) > 0)
          {
            DBParameter[] input = new DBParameter[9];
            input[0] = new DBParameter("@SODetailPid", DbType.Int64, DBConvert.ParseLong(rowParent.Cells["SODetailPid"].Value.ToString()));
            input[1] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(rowChild.Cells["WO"].Value.ToString()));
            input[2] = new DBParameter("@ItemCode", DbType.String, rowChild.Cells["ItemCode"].Value.ToString());
            input[3] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(rowChild.Cells["Revision"].Value.ToString()));
            input[4] = new DBParameter("@WOSOPid", DbType.Int64, DBConvert.ParseLong(rowChild.Cells["WOSOPid"].Value.ToString()));
            input[5] = new DBParameter("@QtySwap", DbType.Int32, DBConvert.ParseInt(rowChild.Cells["QtySwap"].Value.ToString()));
            input[6] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
            if (rowParent.Cells["NewScheduleDelivery"].Value.ToString().Length > 0)
            {
              input[7] = new DBParameter("@NewScheduleDelivery", DbType.DateTime, DBConvert.ParseDateTime(rowParent.Cells["NewScheduleDelivery"].Value.ToString(), ConstantClass.FORMAT_DATETIME));
            }
            if (rowParent.Cells["RemarkContainer"].Value.ToString().Length > 0)
            {
              input[8] = new DBParameter("@RemarkContainer", DbType.String, rowParent.Cells["RemarkContainer"].Value.ToString());
            }

            DBParameter[] output = new DBParameter[1];
            output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
            DataBaseAccess.ExecuteStoreProcedure("spPLNIncreaseQtyWOSOWhenUnHoldSO_Update", input, output);
            long result = DBConvert.ParseLong(output[0].Value.ToString());
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
    /// Check Valid WOSO When HoldSO 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidWOSOWhenHoldSO(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultWOSO.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultWOSO.Rows[i];
        int total = 0;
        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowWO = rowParent.ChildBands[0].Rows[j];
          int total1 = 0;
          for (int k = 0; k < rowWO.ChildBands[0].Rows.Count; k++)
          {
            UltraGridRow rowChild = rowWO.ChildBands[0].Rows[k];
            if (rowChild.Cells["SaleNo"].Value.ToString().Length > 0)
            {
              if (DBConvert.ParseLong(rowChild.Cells["SODetailPid"].Value.ToString()) <= 0)
              {
                message = "SaleOrder";
                return false;
              }

              // Check UnRelease
              if (DBConvert.ParseInt(rowChild.Cells["UnRelease"].Value.ToString()) < 0)
              {
                message = "UnRelease > 0";
                return false;
              }

              if (DBConvert.ParseInt(rowChild.Cells["UnRelease"].Value.ToString()) >
                  DBConvert.ParseInt(rowChild.Cells["UnReleaseOld"].Value.ToString()))
              {
                message = "UnRelease <= " + rowChild.Cells["UnReleaseOld"].Value.ToString();
                return false;
              }
              total1 = total1 + DBConvert.ParseInt(rowChild.Cells["UnRelease"].Value.ToString());
              total = total + DBConvert.ParseInt(rowChild.Cells["UnRelease"].Value.ToString());
            }
          }
          // Check Total UnRelease <= Qty
          if (total1 > DBConvert.ParseInt(rowWO.Cells["Qty"].Value.ToString()))
          {
            message = "Total UnRelease <= Qty";
            return false;
          }
        }
        // Check Total UnRelease <= Open WO
        if (total > DBConvert.ParseInt(rowParent.Cells["OpenWO"].Value.ToString()))
        {
          message = "Total UnRelease <= Open WO";
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Check Valid WOSO When UnHoldSO 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidWOSOWhenUnHoldSO(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultUnHold.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultUnHold.Rows[i];
        int unRelease = DBConvert.ParseInt(rowParent.Cells["UnRelease"].Value.ToString());
        int totalQtySwap = 0;
        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = rowParent.ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(rowChild.Cells["QtySwap"].Value.ToString()) > DBConvert.ParseInt(rowChild.Cells["Qty"].Value.ToString()))
          {
            message = "QtySwap <= Qty";
            return false;
          }
          totalQtySwap = totalQtySwap + DBConvert.ParseInt(rowChild.Cells["QtySwap"].Value.ToString());
        }
        if (totalQtySwap > unRelease)
        {
          message = "Total QtySwap <= UnRelease";
          return false;
        }
      }

      // Check Trung ItemCode & Revision luoi Detail
      for (int i = 0; i < ultUnHold.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultUnHold.Rows[i];
        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = rowParent.ChildBands[0].Rows[j];
          int count = 0;
          // 
          for (int n = 0; n < ultUnHold.Rows.Count; n++)
          {
            UltraGridRow rowParent1 = ultUnHold.Rows[n];
            for (int m = 0; m < rowParent1.ChildBands[0].Rows.Count; m++)
            {
              UltraGridRow rowChild1 = rowParent.ChildBands[0].Rows[m];
              if (DBConvert.ParseLong(rowChild.Cells["WOSOPid"].Value.ToString()) == DBConvert.ParseLong(rowChild1.Cells["WOSOPid"].Value.ToString()))
              {
                count = count + 1;
              }
            }
          }
          if (count > 1)
          {
            message = "Dupplicated WO SO";
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Load ItemCode
    /// </summary>
    /// <param name="soPid"></param>
    private void LoadUltraDropItemCode(long soPid)
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@SOPid", DbType.Int64, soPid);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDLoadInfoItemNeedHold_Select", input);
      if (dtSource == null)
      {
        return;
      }
      ultDDItemCode.DataSource = dtSource;
      ultDDItemCode.DisplayMember = "ItemCode";
      ultDDItemCode.ValueMember = "ItemCode";
      ultDDItemCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 80;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["Revision"].MaxWidth = 50;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["Revision"].MinWidth = 50;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["OriginalQty"].MaxWidth = 80;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["OriginalQty"].MinWidth = 80;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["Cancelled"].MaxWidth = 80;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["Cancelled"].MinWidth = 80;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["OpenWO"].MaxWidth = 80;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["OpenWO"].MinWidth = 80;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["ShipQty"].MaxWidth = 80;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["ShipQty"].MinWidth = 80;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["ScheduleOnContainer"].MaxWidth = 120;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["ScheduleOnContainer"].MinWidth = 120;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["Balance"].MaxWidth = 80;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["Balance"].MinWidth = 80;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["Holded"].MaxWidth = 80;
      ultDDItemCode.DisplayLayout.Bands[0].Columns["Holded"].MinWidth = 80;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      if (this.typeHold)  // Hold
      {
        radHold.Checked = true;
        tabControl1.TabPages.Remove(tabSwapWOSO);
      }
      else // UnHold
      {
        radUnHold.Checked = true;
        tabControl1.TabPages.Remove(tabDeCreaseWOSO);
        groupContainer.Visible = false;
      }

      if (this.status == 1)
      {
        if (this.typeHold == true)
        {
          groupContainer.Visible = true;
          btnLockTab3.Enabled = false;
          btnSaveTab3.Enabled = false;
        }
        else
        {
          groupContainer.Visible = false;
          btnLockTab1.Enabled = false;
          btnLockTab2.Enabled = false;
          btnSaveTab2.Enabled = false;
        }

        if (this.statusTab1 == 1)
        {
          btnReturn.Enabled = false;
          btnLockTab1.Enabled = false;
          groupContainer.Visible = false;
          chkSelectAll.Enabled = false;
        }

        if (this.statusTab2 == 1)
        {
          btnReturn.Enabled = false;
          btnLockTab2.Enabled = false;
          btnSaveTab2.Enabled = false;
        }

        if (this.statusTab3 == 1)
        {
          btnReturn.Enabled = false;
          btnLockTab3.Enabled = false;
          btnSaveTab3.Enabled = false;
        }
      }
      else if (this.status == 2)
      {
        groupContainer.Visible = false;
        chkConfirmed.Checked = true;
        chkConfirmed.Enabled = false;
        btnSave.Enabled = false;
        btnLockTab1.Enabled = false;
        btnLockTab2.Enabled = false;
        btnLockTab3.Enabled = false;
        btnSaveTab2.Enabled = false;
        btnSaveTab3.Enabled = false;
        btnReturn.Enabled = false;
      }
    }

    /// <summary>
    /// Save Master
    /// </summary>
    /// <returns></returns>
    private bool SaveMaster(int tabLock)
    {
      // Input
      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.holdPid);
      inputParam[1] = new DBParameter("@PLNUpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      if (tabLock == 1)
      {
        inputParam[2] = new DBParameter("@Tab1", DbType.Int32, 1);
      }
      else if (tabLock == 2)
      {
        inputParam[2] = new DBParameter("@Tab2", DbType.Int32, 1);
      }
      else if (tabLock == 3)
      {
        inputParam[2] = new DBParameter("@Tab3", DbType.Int32, 1);
      }

      // Output
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spCSDSOHoldInfoPLN_Update", inputParam, outputParam);
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
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        DBParameter[] inputParam = new DBParameter[3];
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
        if (DBConvert.ParseInt(row.Cells["PLNApproved"].Value.ToString()) == 1)
        {
          inputParam[1] = new DBParameter("@PLNApproved", DbType.Int32, 1);
        }
        if (row.Cells["PLNRemark"].Value.ToString().Length > 0)
        {
          inputParam[2] = new DBParameter("@PLNRemark", DbType.String, row.Cells["PLNRemark"].Value.ToString());
        }

        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spCSDSOHoldIDetailPLN_Update", inputParam, outputParam);
        long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
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
    /// Get Data Container
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
      inputParam[1] = new SqlDBParameter("@Flag", SqlDbType.Int, 1);
      DataTable dtSource = Shared.DataBaseUtility.SqlDataBaseAccess.SearchStoreProcedureDataTable("spCSDSOHoldInfluenceContainer_Select", inputParam);
      if (dtSource != null)
      {
        ultDataContainer.DataSource = dtSource;
      }
    }

    /// <summary>
    /// Create Data Set WO - SO
    /// </summary>
    /// <returns></returns>
    private DataSet DataSetWOSO()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("CustomerCode", typeof(System.String));
      taParent.Columns.Add("SOPid", typeof(System.Int64));
      taParent.Columns.Add("SaleNo", typeof(System.String));
      taParent.Columns.Add("PONo", typeof(System.String));
      taParent.Columns.Add("SaleCode", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("HoldQty", typeof(System.Int32));
      taParent.Columns.Add("OpenWO", typeof(System.Int32));
      ds.Tables.Add(taParent);

      // Child
      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("SaleOrderPid", typeof(System.Int64));
      taChild.Columns.Add("WO", typeof(System.Int64));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("Qty", typeof(System.Int32));
      ds.Tables.Add(taChild);

      // Sub Child
      DataTable taChildDetail = new DataTable("dtChildDetail");
      taChildDetail.Columns.Add("WO", typeof(System.Int64));
      taChildDetail.Columns.Add("ItemCode", typeof(System.String));
      taChildDetail.Columns.Add("Revision", typeof(System.Int32));
      taChildDetail.Columns.Add("SODetailPid", typeof(System.Int64));
      taChildDetail.Columns.Add("Customer", typeof(System.String));
      taChildDetail.Columns.Add("SaleNo", typeof(System.String));
      taChildDetail.Columns.Add("PONo", typeof(System.String));
      taChildDetail.Columns.Add("ScheduleDelivery", typeof(System.String));
      taChildDetail.Columns.Add("UnReleaseOld", typeof(System.Int32));
      taChildDetail.Columns.Add("UnRelease", typeof(System.Int32));
      ds.Tables.Add(taChildDetail);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", new DataColumn[] { taParent.Columns["SOPid"], taParent.Columns["ItemCode"], taParent.Columns["Revision"] }, new DataColumn[] { taChild.Columns["SaleOrderPid"], taChild.Columns["ItemCode"], taChild.Columns["Revision"] }, false));
      ds.Relations.Add(new DataRelation("dtChild_dtChildDetail", new DataColumn[] { taChild.Columns["WO"], taChild.Columns["ItemCode"], taChild.Columns["Revision"] }, new DataColumn[] { taChildDetail.Columns["WO"], taChildDetail.Columns["ItemCode"], taChildDetail.Columns["Revision"] }, false));
      return ds;
    }

    /// <summary>
    /// Create Data Set WO - SO
    /// </summary>
    /// <returns></returns>
    private DataSet DataSetWOSOUnHold()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("SODetailPid", typeof(System.Int64));
      taParent.Columns.Add("CustomerCode", typeof(System.String));
      taParent.Columns.Add("SaleNo", typeof(System.String));
      taParent.Columns.Add("PONo", typeof(System.String));
      taParent.Columns.Add("SaleCode", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("HoldQty", typeof(System.Int32));
      taParent.Columns.Add("UnRelease", typeof(System.Int32));
      taParent.Columns.Add("ScheduleDelivery", typeof(System.String));
      taParent.Columns.Add("NewScheduleDelivery", typeof(System.DateTime));
      taParent.Columns.Add("RemarkContainer", typeof(System.String));
      ds.Tables.Add(taParent);

      // Child
      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("SODetailPid", typeof(System.Int64));
      taChild.Columns.Add("WOSOPid", typeof(System.Int64));
      taChild.Columns.Add("WO", typeof(System.Int64));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("Qty", typeof(System.Int32));
      taChild.Columns.Add("QtySwap", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", new DataColumn[] { taParent.Columns["SODetailPid"], taParent.Columns["ItemCode"], taParent.Columns["Revision"] }, new DataColumn[] { taChild.Columns["SODetailPid"], taChild.Columns["ItemCode"], taChild.Columns["Revision"] }, false));
      return ds;
    }

    /// <summary>
    /// Load List SO Detail
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="revision"></param>
    private void LoadDropSODetail(long soPid, string itemCode, int revision)
    {
      string commandText = string.Empty;
      commandText = "  SELECT MAS.SaleOrderDetailPid SODetailPid, MAS.SaleNo, CUS.CustomerCode Customer, MAS.PONo,";
      commandText += "    CONVERT(varchar, MAS.ScheduleDelivery, 103) ScheduleDelivery, MAS.Remain UnRelease, MAS.Remain UnReleaseOld";
      commandText += " FROM VPLNMasterPlan MAS";
      commandText += " LEFT JOIN TblCSDCustomerInfo CUS ON CUS.Pid = MAS.CustomerPid";
      commandText += " WHERE MAS.Remain > 0 AND MAS.ItemCode  = '" + itemCode + "' AND MAS.Revision = " + revision + "";
      commandText += " AND MAS.SaleOrderPid != " + soPid;

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultDropSODetail.DataSource = dtSource;
      ultDropSODetail.DisplayMember = "SaleNo";
      ultDropSODetail.ValueMember = "SaleNo";
      ultDropSODetail.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDropSODetail.DisplayLayout.Bands[0].Columns["SODetailPid"].Hidden = true;
      ultDropSODetail.DisplayLayout.Bands[0].Columns["UnReleaseOld"].Hidden = true;
      ultDropSODetail.DisplayLayout.Bands[0].Columns["Customer"].MinWidth = 150;
      ultDropSODetail.DisplayLayout.Bands[0].Columns["Customer"].MaxWidth = 150;
      ultDropSODetail.DisplayLayout.Bands[0].Columns["SaleNo"].MinWidth = 150;
      ultDropSODetail.DisplayLayout.Bands[0].Columns["SaleNo"].MaxWidth = 150;
      ultDropSODetail.DisplayLayout.Bands[0].Columns["PONo"].MinWidth = 150;
      ultDropSODetail.DisplayLayout.Bands[0].Columns["PONo"].MaxWidth = 150;
      ultDropSODetail.DisplayLayout.Bands[0].Columns["ScheduleDelivery"].MinWidth = 100;
      ultDropSODetail.DisplayLayout.Bands[0].Columns["ScheduleDelivery"].MaxWidth = 100;
      ultDropSODetail.DisplayLayout.Bands[0].Columns["UnRelease"].MinWidth = 100;
      ultDropSODetail.DisplayLayout.Bands[0].Columns["UnRelease"].MaxWidth = 100;
      ultDropSODetail.DisplayLayout.Bands[0].Columns["UnRelease"].CellAppearance.TextHAlign = HAlign.Right;
    }
    #endregion Function

    #region Event
    private void btnSave_Click(object sender, EventArgs e)
    {
      // Check Lock Tab
      if (this.typeHold)
      {
        // Check Lock tab1
        if (this.statusTab1 != 1)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Lock Tab");
          return;
        }

        // Check Lock Tab2
        if (this.statusTab2 != 1)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Lock Tab");
          return;
        }
      }
      else // UnHold
      {
        // Check Lock Tab3
        if (this.statusTab3 != 1)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Lock Tab");
          return;
        }
      }
      // Input
      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.holdPid);
      inputParam[1] = new DBParameter("@PLNUpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      if (chkConfirmed.Checked)
      {
        inputParam[2] = new DBParameter("@Status", DbType.Int32, 2);
      }
      else
      {
        inputParam[2] = new DBParameter("@Status", DbType.Int32, 1);
      }
      // Output
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spCSDSOHoldInfoPLN_Update", inputParam, outputParam);
      if (DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0)
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }

      this.LoadMain();
      this.LoadTab1();
      this.LoadTab2();
      this.LoadTab3();
    }

    /// <summary>
    /// Lock Tab 1
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLockTab1_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.SaveMaster(1);
      if (success)
      {
        if (success)
        {
          success = this.SaveDetail();
          if (success)
          {
            success = this.UpdateDataWhenHoldSO();
            if (success)
            {
              WindowUtinity.ShowMessageSuccess("MSG0004");
            }
            else
            {
              WindowUtinity.ShowMessageError("WRN0004");
            }
          }
        }
      }

      // Load Data
      this.LoadMain();
      this.LoadTab1();
    }

    /// <summary>
    /// Lock Tab 2
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLockTab2_Click(object sender, EventArgs e)
    {
      // Check Lock Tab1
      if (this.statusTab1 == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Lock Tab1");
        return;
      }

      bool success = this.LockTab2();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      // Load Tab 2
      this.LoadMain();
      this.LoadTab2();
    }

    /// <summary>
    /// Lock Tab 3
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLockTab3_Click(object sender, EventArgs e)
    {
      bool success = this.LockTab3();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      // Load Data
      this.LoadMain();
      this.LoadTab3();
    }

    /// <summary>
    /// Save WO - SO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveTab2_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckValidWOSOWhenHoldSO(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Updata WO - SO
      success = this.UpdateWOSOWhenHoldSO();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      // Load Tab 2
      this.LoadTab2();
    }

    /// <summary>
    /// Save Tab 3
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveTab3_Click(object sender, EventArgs e)
    {
      // Check Data
      string message = string.Empty;
      bool success = this.CheckValidWOSOWhenUnHoldSO(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Update WO - SO
      success = this.UpdateWOSOWhenUnHoldSO();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      // Load Tab3
      this.LoadTab3();
    }

    /// <summary>
    /// Return
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnReturn_Click(object sender, EventArgs e)
    {
      // Check Lock Tab
      if (this.statusTab1 == 1 || this.statusTab2 == 1 || this.statusTab3 == 1)
      {
        WindowUtinity.ShowMessageError("ERRO149", txtHoldPONote.Text, "Locked");
        return;
      }
      // End

      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@HoldPid", DbType.Int64, this.holdPid);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spCSDReturnHoldSO_Update", 300, input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result > 0)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004", "Return");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", "Return");
        return;
      }

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

    /// <summary>
    /// Init Layout Data
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
      e.Layout.Bands[0].Columns["SOPid"].Header.Caption = "SO";
      e.Layout.Bands[0].Columns["OriginalQty"].Header.Caption = "Original \n Qty";
      e.Layout.Bands[0].Columns["ShipQty"].Header.Caption = "ShipQty";
      e.Layout.Bands[0].Columns["OpenWO"].Header.Caption = "OpenWO";
      e.Layout.Bands[0].Columns["ScheduleOnContainer"].Header.Caption = "Schedule On \n Container";
      e.Layout.Bands[0].Columns["PLNRemark"].Header.Caption = "PLN \n Remark";
      e.Layout.Bands[0].Columns["CSDRemark"].Header.Caption = "CSD \n Remark";
      e.Layout.Bands[0].Columns["PLNApproved"].Header.Caption = "PLN \n Approved";

      e.Layout.Bands[0].Columns["PONo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["OriginalQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Cancelled"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["OpenWO"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ShipQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ScheduleOnContainer"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Balance"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Holded"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CSDRemark"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["HoldQty"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["PLNRemark"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["PLNApproved"].CellAppearance.BackColor = Color.Aqua;

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

      e.Layout.Bands[0].Columns["PLNApproved"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["PLNApproved"].DefaultCellValue = 0;
      e.Layout.Bands[0].Columns["SOPid"].ValueList = ultDDSO;
      e.Layout.Bands[0].Columns["ItemCode"].ValueList = ultDDItemCode;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;

      if (this.status == 2)
      {
        e.Layout.AutoFitColumns = true;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

        e.Layout.Bands[0].Columns["OriginalQty"].Hidden = true;
        e.Layout.Bands[0].Columns["Cancelled"].Hidden = true;
        e.Layout.Bands[0].Columns["ShipQty"].Hidden = true;
        e.Layout.Bands[0].Columns["OpenWO"].Hidden = true;
        e.Layout.Bands[0].Columns["ScheduleOnContainer"].Hidden = true;
        e.Layout.Bands[0].Columns["Balance"].Hidden = true;
        e.Layout.Bands[0].Columns["Holded"].Hidden = true;
      }

      if (this.statusTab1 == 1)
      {
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      }

      if (this.typeHold)
      {
        e.Layout.Bands[0].Columns["HoldQty"].Header.Caption = "HoldQty";
      }
      else
      {
        e.Layout.Bands[0].Columns["HoldQty"].Header.Caption = "UnHoldQty";
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init Layout Container
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

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init Layout WO - SO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultWOSO_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[2].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[1].Columns["SaleOrderPid"].Hidden = true;
      e.Layout.Bands[1].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[1].Columns["Revision"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;

      e.Layout.Bands[2].Columns["UnReleaseOld"].Hidden = true;
      e.Layout.Bands[2].Columns["WO"].Hidden = true;
      e.Layout.Bands[2].Columns["SODetailPid"].Hidden = true;
      e.Layout.Bands[2].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[2].Columns["Revision"].Hidden = true;

      e.Layout.Bands[2].Columns["SaleNo"].ValueList = ultDropSODetail;

      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["HoldQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["OpenWO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[2].Columns["UnRelease"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[2].Columns["SaleNo"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[2].Columns["UnRelease"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Bands[1].Columns["WO"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Qty"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[2].Columns["Customer"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[2].Columns["PONo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[2].Columns["ScheduleDelivery"].CellActivation = Activation.ActivateOnly;

      if (this.statusTab2 == 1)
      {
        e.Layout.Bands[2].Override.AllowAddNew = AllowAddNew.No;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init Layout UnHold SO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultUnHold_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["SODetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["HoldQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UnRelease"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CustomerCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PONo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SaleCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ScheduleDelivery"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NewScheduleDelivery"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["RemarkContainer"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Bands[1].Columns["SODetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["WOSOPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["QtySwap"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["WO"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["QtySwap"].CellAppearance.BackColor = Color.Aqua;

      if (this.status == 2 || this.statusTab3 == 1)
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
    /// Choose Select All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      int selected = (chkSelectAll.Checked ? 1 : 0);
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        ultData.Rows[i].Cells["PLNApproved"].Value = selected;
      }
    }

    /// <summary>
    /// Change Tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (tabControl1.SelectedTab == tabControl1.TabPages["tabHoldSO"])
      {
        this.LoadTab1();
      }
      else if (tabControl1.SelectedTab == tabControl1.TabPages["tabDeCreaseWOSO"])
      {
        this.LoadTab2();
      }
      else if (tabControl1.SelectedTab == tabControl1.TabPages["tabSwapWOSO"])
      {
        this.LoadTab3();
      }
    }

    /// <summary>
    /// After Cell Update WO - SO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultWOSO_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "SaleNo":
          row.Cells["SODetailPid"].Value = DBNull.Value;
          row.Cells["UnRelease"].Value = DBNull.Value;
          row.Cells["UnReleaseOld"].Value = DBNull.Value;
          row.Cells["PONo"].Value = DBNull.Value;
          row.Cells["ScheduleDelivery"].Value = DBNull.Value;
          row.Cells["Customer"].Value = DBNull.Value;
          if (ultDropSODetail.SelectedRow != null)
          {
            row.Cells["SODetailPid"].Value = DBConvert.ParseLong(ultDropSODetail.SelectedRow.Cells["SODetailPid"].Value.ToString());
            row.Cells["UnReleaseOld"].Value = DBConvert.ParseInt(ultDropSODetail.SelectedRow.Cells["UnReleaseOld"].Value.ToString());
            row.Cells["UnRelease"].Value = DBConvert.ParseInt(ultDropSODetail.SelectedRow.Cells["UnRelease"].Value.ToString());
            row.Cells["PONo"].Value = ultDropSODetail.SelectedRow.Cells["PONo"].Value.ToString();
            row.Cells["ScheduleDelivery"].Value = ultDropSODetail.SelectedRow.Cells["ScheduleDelivery"].Value.ToString();
            row.Cells["Customer"].Value = ultDropSODetail.SelectedRow.Cells["Customer"].Value.ToString();
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Before Cell Active WO - SO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultWOSO_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "SaleNo":
          if (row.ParentRow != null)
          {
            if (row.ParentRow.Cells["ItemCode"].Value.ToString().Length > 0 &&
                DBConvert.ParseInt(row.ParentRow.Cells["Revision"].Value.ToString()) != int.MinValue)
            {
              long soPid = DBConvert.ParseLong(row.ParentRow.Cells["SaleOrderPid"].Value.ToString());
              string itemCode = row.ParentRow.Cells["ItemCode"].Value.ToString();
              int revision = DBConvert.ParseInt(row.ParentRow.Cells["Revision"].Value.ToString());
              this.LoadDropSODetail(soPid, itemCode, revision);
            }
            else
            {
              this.LoadDropSODetail(-1, "", -1);
            }
          }
          break;
        default:
          break;
      }
    }


    /// <summary>
    /// Before Cell Update UnHold SO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultUnHold_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      string value = e.NewValue.ToString();
      switch (columnName)
      {
        case "QtySwap":
          if (value.Length > 0)
          {
            if (DBConvert.ParseInt(value) < 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "QtySwap");
              e.Cancel = true;
            }
            else if (DBConvert.ParseInt(value) > DBConvert.ParseInt(row.Cells["Qty"].Value.ToString()))
            {
              WindowUtinity.ShowMessageError("ERR0001", "QtySwap <= Qty");
              e.Cancel = true;
            }
          }
          break;
        default:
          break;
      }
    }

    private void ultWOSO_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      string value = e.NewValue.ToString();
      switch (columnName)
      {
        case "SaleNo":
          if (value.Length > 0)
          {
            DataTable dt = (DataTable)ultDropSODetail.DataSource;
            DataRow[] foundRow = dt.Select(string.Format(@"SaleNo = '{0}'", value));
            if (foundRow.Length == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "SO Detail");
              e.Cancel = true;
              return;
            }

            // Check Trung
            UltraGridRow rowParent = row.ParentRow;
            for (int i = 0; i < rowParent.ChildBands[0].Rows.Count; i++)
            {
              UltraGridRow rowChild = rowParent.ChildBands[0].Rows[i];
              if (row != rowChild &&
                  DBConvert.ParseLong(ultDropSODetail.SelectedRow.Cells["SODetailPid"].Value.ToString()) == DBConvert.ParseLong(rowChild.Cells["SODetailPid"].Value.ToString()))
              {
                WindowUtinity.ShowMessageError("ERR0013", "SO Detail");
                e.Cancel = true;
                return;
              }
            }
          }
          break;
        case "UnRelease":
          if (value.Length > 0)
          {
            if (DBConvert.ParseInt(value) < 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "UnRelease > 0");
              e.Cancel = true;
              return;
            }

            if (DBConvert.ParseInt(value) > DBConvert.ParseInt(row.Cells["UnReleaseOld"].Value.ToString()))
            {
              WindowUtinity.ShowMessageError("ERR0001", "UnRelease < " + row.Cells["UnReleaseOld"].Value.ToString());
              e.Cancel = true;
              return;
            }
          }
          break;
        default:
          break;
      }
    }
    #endregion Event
  }
}
