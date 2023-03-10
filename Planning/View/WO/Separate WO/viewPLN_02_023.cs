/*
  Author      : 
  Date        : 26-11-2013
  Description : Separate WO
*/
using CrystalDecisions.CrystalReports.Engine;
using DaiCo.Application;
using DaiCo.Planning.DataSetFile;
using DaiCo.Planning.Reports;
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

namespace DaiCo.Planning
{
  public partial class viewPLN_02_023 : MainUserControl
  {
    #region Field
    public long cancelWOPid = long.MinValue;
    private int status = int.MinValue;
    private int tabLock1 = int.MinValue;
    private int tabLock2 = int.MinValue;
    private int tabLock3 = int.MinValue;
    private DataSet dsRequestITW = new DataSet();
    #endregion Field

    #region Init
    public viewPLN_02_023()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load View
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_023_Load(object sender, EventArgs e)
    {
      //this.cancelWOPid = 30;
      // Load WO
      this.LoadWorkOrder();
      // Load Item (WO - SO)
      this.LoadItemCodeWOSO();
      // Load SO
      //this.LoadDropDownSODetail();
      // Load Tab 1
      this.LoadTab1(this.cancelWOPid);
    }

    /// <summary>
    /// Load WO
    /// </summary>
    private void LoadWorkOrder()
    {
      string commandText = string.Empty;
      commandText = string.Format(@"SELECT DISTINCT WorkOrderPid WO
                                          FROM TblPLNWorkOrderConfirmedDetails
                                          ORDER BY WO ");

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
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
    /// Load CarcassCode
    /// </summary>
    /// <param name="wo"></param>
    private void LoadCarcassCode(int wo)
    {
      string commandText = string.Empty;
      commandText = string.Format(@" SELECT DISTINCT CarcassCode
                                     FROM TblPLNWorkOrderConfirmedDetails
                                     WHERE WorkOrderPid = {0}", wo);

      //commandText = " SELECT DISTINCT CarcassCode";
      //commandText += " FROM TblPLNWorkOrderConfirmedDetails";
      //commandText += " WHERE WorkOrderPid = "+ wo;

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
    /// Load ItemCode
    /// </summary>
    /// <param name="wo"></param>
    /// <param name="carcassCode"></param>
    private void LoadItemCode(int wo, string carcassCode)
    {
      string commandText = string.Empty;
      commandText = string.Format(@"SELECT DISTINCT ItemCode, Revision, Qty
                                    FROM TblPLNWorkOrderConfirmedDetails
                                    WHERE WorkOrderPid = {0} AND CarcassCode = '{1}'", wo, carcassCode);

      //commandText = " SELECT DISTINCT ItemCode, Revision, Qty";
      //commandText += " FROM TblPLNWorkOrderConfirmedDetails";
      //commandText += " WHERE WorkOrderPid = " + wo + " AND CarcassCode  = '"+ carcassCode +"'";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultDropItemCode.DataSource = dtSource;
      ultDropItemCode.DisplayMember = "ItemCode";
      ultDropItemCode.ValueMember = "ItemCode";
      ultDropItemCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDropItemCode.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      ultDropItemCode.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      ultDropItemCode.DisplayLayout.Bands[0].Columns["Revision"].MaxWidth = 70;
      ultDropItemCode.DisplayLayout.Bands[0].Columns["Revision"].MinWidth = 70;
      ultDropItemCode.DisplayLayout.Bands[0].Columns["Qty"].MaxWidth = 70;
      ultDropItemCode.DisplayLayout.Bands[0].Columns["Qty"].MinWidth = 70;
      ultDropItemCode.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load ItemCode WO - SO
    /// </summary>
    /// <param name="wo"></param>
    /// <param name="carcassCode"></param>
    private void LoadItemCodeWOSO()
    {
      string commandText = string.Empty;
      commandText = " SELECT ItemCode, Revision FROM TblBOMItemInfo";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultDropItemCodeWOSO.DataSource = dtSource;
      ultDropItemCodeWOSO.DisplayMember = "ItemCode";
      ultDropItemCodeWOSO.ValueMember = "ItemCode";
      ultDropItemCodeWOSO.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDropItemCodeWOSO.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      ultDropItemCodeWOSO.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      ultDropItemCodeWOSO.DisplayLayout.Bands[0].Columns["Revision"].MaxWidth = 70;
      ultDropItemCodeWOSO.DisplayLayout.Bands[0].Columns["Revision"].MinWidth = 70;
      ultDropItemCodeWOSO.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load ItemCode Belong CarcassCode
    /// </summary>
    /// <param name="wo"></param>
    /// <param name="carcassCode"></param>
    private void LoadItemCodeBelongCarcass(int wo, string carcassCode)
    {
      string commandText = string.Empty;
      commandText = string.Format(@"  SELECT ITEM.ItemCode, ITEM.Revision, ITEM.ItemCode As Name , ISNULL(WO.QtyShipped, 0) QtyShipped
                                      FROM TblBOMItemInfo ITEM
                                      LEFT JOIN 
                                      (
                                        SELECT ItemCode, Revision, SUM(ISNULL(QtyShipped, 0)) QtyShipped
                                        FROM TblPLNWOInfoDetailGeneral
                                        WHERE WoInfoPID = {0}
                                        GROUP BY ItemCode, Revision
                                      )WO ON WO.ItemCode = ITEM.ItemCode
                                        AND WO.Revision = ITEM.Revision
                                      WHERE CarcassCode = '{1}'", wo, carcassCode);

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultDropItemCodeSeparate.DataSource = dtSource;
      ultDropItemCodeSeparate.DisplayMember = "Name";
      ultDropItemCodeSeparate.ValueMember = "ItemCode";
      ultDropItemCodeSeparate.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDropItemCodeSeparate.DisplayLayout.Bands[0].Columns["Name"].Hidden = true;
      ultDropItemCodeSeparate.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDropItemCodeSeparate.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      ultDropItemCodeSeparate.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      ultDropItemCodeSeparate.DisplayLayout.Bands[0].Columns["Revision"].MaxWidth = 70;
      ultDropItemCodeSeparate.DisplayLayout.Bands[0].Columns["Revision"].MinWidth = 70;
      ultDropItemCodeSeparate.DisplayLayout.Bands[0].Columns["QtyShipped"].MaxWidth = 70;
      ultDropItemCodeSeparate.DisplayLayout.Bands[0].Columns["QtyShipped"].MinWidth = 70;
      ultDropItemCodeSeparate.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Show User Guide
    /// </summary>
    private void ShowUserGuide()
    {
      // Location
      string location = string.Empty;
      string commandText = string.Empty;
      commandText = String.Format(@"SELECT Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND Code = {1}", Shared.Utility.ConstantClass.GROUP_GNR_PATHFILEUPLOAD, 5);
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
      //File.Copy(location, locationNew);
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
      commandText = String.Format(@"SELECT Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND Code = {1}", Shared.Utility.ConstantClass.GROUP_GNR_PATHFILEUPLOAD, 5);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null)
      {
        location = dt.Rows[0]["Value"].ToString();
      }
      // End

      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@SeparateWOPid", DbType.Int64, this.cancelWOPid);

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNRPTSeparateWOInformation_Select", input);
      dsPLNSeparateWOReport dsSource = new dsPLNSeparateWOReport();
      if (ds != null)
      {
        // Cancel Wo Info
        dsSource.Tables["dtSeparateInfo"].Merge(ds.Tables[0]);
        // Cancel Wo Detail
        dsSource.Tables["dtSeparateDetail"].Merge(ds.Tables[1]);
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


      if (dsRequestITW == null || dsRequestITW.Tables.Count < 2)
      {
        dsSource.Tables["dtSeparateInfo"].Rows[0]["WIPITEM"] = "Not Affect";
      }
      else if (dsRequestITW.Tables[1].Rows.Count == 0)
      {
        dsSource.Tables["dtSeparateInfo"].Rows[0]["WIPITEM"] = "Not Affect";
      }
      else
      {
        dsSource.Tables["dtSeparateInfo"].Rows[0]["WIPITEM"] = "Affect";
      }
      // End Affect

      ReportClass cpt = null;
      cpt = new cptSeparateWOInformation();
      cpt.SetDataSource(dsSource);
      //cpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, location + txtCancelWONo.Text + ".pdf");
    }

    /// <summary>
    /// Load Drop SO Belong ItemCode & Revision
    /// </summary>
    /// <param name="wo"></param>
    /// <param name="carcassCode"></param>
    private void LoadDropDownSODetailItemRev(string itemCode, int revision, UltraGridRow row)
    {
      string commandText = string.Empty;
      commandText = "  SELECT SaleOrderDetailPid SODetailPid, SaleNo, PONo, ScheduleDelivery, ItemCode, Revision, Remain Qty, 0 QtyShipped";
      commandText += " FROM VPLNMasterPlan ";
      commandText += " WHERE Remain > 0 AND ItemCode  = '" + itemCode + "' AND Revision = " + revision + "";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultDropSODetailItemRev.DataSource = dtSource;
      ultDropSODetailItemRev.DisplayMember = "SaleNo";
      ultDropSODetailItemRev.ValueMember = "SaleNo";
      ultDropSODetailItemRev.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDropSODetailItemRev.DisplayLayout.Bands[0].Columns["SaleNo"].MinWidth = 100;
      ultDropSODetailItemRev.DisplayLayout.Bands[0].Columns["SaleNo"].MaxWidth = 100;

      ultDropSODetailItemRev.DisplayLayout.Bands[0].Columns["PONo"].MinWidth = 100;
      ultDropSODetailItemRev.DisplayLayout.Bands[0].Columns["PONo"].MaxWidth = 100;

      ultDropSODetailItemRev.DisplayLayout.Bands[0].Columns["ScheduleDelivery"].MinWidth = 100;
      ultDropSODetailItemRev.DisplayLayout.Bands[0].Columns["ScheduleDelivery"].MaxWidth = 100;

      ultDropSODetailItemRev.DisplayLayout.Bands[0].Columns["QtyShipped"].MinWidth = 100;
      ultDropSODetailItemRev.DisplayLayout.Bands[0].Columns["QtyShipped"].MaxWidth = 100;

      ultDropSODetailItemRev.DisplayLayout.Bands[0].Columns["SODetailPid"].Hidden = true;
      ultDropSODetailItemRev.DisplayLayout.Bands[0].Columns["ItemCode"].Hidden = true;
      ultDropSODetailItemRev.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;

      row.Cells["SaleNo"].ValueList = ultDropSODetailItemRev;
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
      }
      else
      {
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPLNCancelWOGetNewCode('SWO') CancelWONo");
        if ((dt != null) && (dt.Rows.Count > 0))
        {
          txtCancelWONo.Text = dt.Rows[0]["CancelWONo"].ToString();
          txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME); ;
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
      }
      if (this.tabLock2 == 1)
      {
        btnLockData2.Enabled = false;
      }
      if (this.tabLock3 == 1)
      {
        btnLockData3.Enabled = false;
      }

      if (this.status == 1)
      {
        txtReason.ReadOnly = true;
        txtRemark.ReadOnly = true;
        chkConfirm.Checked = true;
        chkConfirm.Enabled = false;
        btnSave.Enabled = false;
        ultItemStatus.Visible = false;
        btnDelete.Enabled = false;
      }
      else if (this.status == 0)
      {
        chkConfirm.Checked = false;
      }

      // Show User Guide
      string location = string.Empty;
      string commandText = string.Empty;
      commandText = String.Format(@"SELECT Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND Code = {1}", Shared.Utility.ConstantClass.GROUP_GNR_PATHFILEUPLOAD, 5);
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
    /// Information Separate Information
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSetSeparateInfo()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("WO", typeof(System.Int32));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("Qty", typeof(System.Int32));
      taParent.Columns.Add("WIPCAR", typeof(System.String));
      taParent.Columns.Add("SUB", typeof(System.String));
      taParent.Columns.Add("ITW", typeof(System.String));
      taParent.Columns.Add("WIPITEM", typeof(System.String));
      ds.Tables.Add(taParent);

      // Child
      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("DetailPid", typeof(System.Int64));
      taChild.Columns.Add("WO", typeof(System.Int32));
      //taChild.Columns.Add("CarcassCode", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("QtyShipped", typeof(System.Int32));
      taChild.Columns.Add("SeparateQty", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", new DataColumn[] { taParent.Columns["Pid"], taParent.Columns["WO"] }, new DataColumn[] { taChild.Columns["DetailPid"], taChild.Columns["WO"] }, false));
      return ds;
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
      taChildMaterial.Columns.Add("QtyCancel", typeof(System.Int32));
      taChildMaterial.Columns.Add("ReAllocateQty", typeof(System.Double));
      taChildMaterial.Columns.Add("Type", typeof(System.Int32));
      ds.Tables.Add(taChildMaterial);

      ds.Relations.Add(new DataRelation("dtParent_dtChildMaterial", new DataColumn[] { taParent.Columns["WO"], taParent.Columns["ItemCode"], taParent.Columns["Revision"] }, new DataColumn[] { taChildMaterial.Columns["WO"], taChildMaterial.Columns["ItemCode"], taChildMaterial.Columns["Revision"] }, false));
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
    /// Create Data Set For Import Data
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSetForImportData()
    {
      //Master
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("WO", typeof(System.Int32));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("Qty", typeof(System.Int32));
      ds.Tables.Add(taParent);

      //Detail
      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("DetailPid", typeof(System.Int64));
      taChild.Columns.Add("WO", typeof(System.Int32));
      taChild.Columns.Add("CarcassCode", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("QtyShipped", typeof(System.Int32));
      taChild.Columns.Add("SeparateQty", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", new DataColumn[] { taParent.Columns["Pid"], taParent.Columns["WO"], taParent.Columns["CarcassCode"] }, new DataColumn[] { taChild.Columns["DetailPid"], taChild.Columns["WO"], taChild.Columns["CarcassCode"] }, false));

      return ds;
    }
    /// <summary>
    /// Get Status Item
    /// </summary>
    private void GetStatusItem()
    {
      string stringdata = string.Empty;
      for (int i = 0; i < ultSeparateWO.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultSeparateWO.Rows[i];
        string itemParent = rowParent.Cells["ItemCode"].Value.ToString() + "-" + rowParent.Cells["Revision"].Value.ToString();
        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = rowParent.ChildBands[0].Rows[j];
          string itemChild = rowChild.Cells["ItemCode"].Value.ToString() + "-" + rowChild.Cells["Revision"].Value.ToString();
          if (itemParent != itemChild)
          {
            stringdata = stringdata + rowChild.Cells["ItemCode"].Value.ToString() + "^" + rowChild.Cells["Revision"].Value.ToString() + "~";
          }
        }
      }

      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@StringData", DbType.String, stringdata);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNSeparateWOGetStatusNewItem_Select", input);
      if (dtSource != null)
      {
        ultItemStatus.DataSource = dtSource;
      }
    }

    /// <summary>
    ///  Get Informatino WO - SO
    /// </summary>
    private void GetInfoWOSO()
    {
      string data = string.Empty;
      for (int i = 0; i < ultSeparateWO.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultSeparateWO.Rows[i];
        data = data + rowParent.Cells["WO"].Value.ToString() + "^" + rowParent.Cells["ItemCode"].Value.ToString() + "^" +
                      rowParent.Cells["Revision"].Value.ToString() + "~";
      }

      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@StringData", DbType.String, data);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNSeparateWOGetInfoAffectWOSO_Select", input);
      if (dtSource != null)
      {
        ultWOSO.DataSource = dtSource;
        for (int i = 0; i < ultWOSO.Rows.Count; i++)
        {
          UltraGridRow row = ultWOSO.Rows[i];
          row.Cells["WO"].Activation = Activation.ActivateOnly;
          row.Cells["ItemCode"].Activation = Activation.ActivateOnly;
          row.Cells["SaleNo"].Activation = Activation.ActivateOnly;
        }
      }
    }

    /// <summary>
    /// Check Status Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckStatusValid(out string message)
    {
      message = string.Empty;
      string command = string.Empty;
      DataTable dtCheck = new DataTable();
      // Check ItemCode Separate && Total SeparateQty  = Qty
      for (int i = 0; i < ultSeparateWO.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultSeparateWO.Rows[i];
        //        // Check Close WO
        //        long wo = DBConvert.ParseLong(rowParent.Cells["WO"].Value.ToString());
        //        command = string.Format(@"SELECT DISTINCT WorkOrderPid
        //                                     FROM TblPLNWorkOrderConfirmedDetails
        //                                     WHERE WorkOrderPid = {0} AND (ISNULL(CloseCOM1, 0) = 1 OR ISNULL(CloseCAR, 0) = 1 
        //                                                              OR ISNULL(CloseAll, 0) = 1)", wo);

        //        dtCheck = DataBaseAccess.SearchCommandTextDataTable(command);
        //        if (dtCheck != null && dtCheck.Rows.Count > 0)
        //        {
        //          ultSeparateWO.Rows[i].Appearance.BackColor = Color.Yellow;
        //          message = " WO is Close";
        //          return false;
        //        }
        // Total
        int qty = DBConvert.ParseInt(rowParent.Cells["Qty"].Value.ToString());
        int cc = 0;
        // Separate Qty
        int totalSeparateQty = 0;
        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = rowParent.ChildBands[0].Rows[j];
          // 1: Check ItemCode Separate
          if (rowChild.Cells["ItemCode"].Value.ToString().Length == 0)
          {
            message = "ItemCode";
            return false;
          }
          // Total SeparateQty
          if (rowChild.Cells["SeparateQty"].Value.ToString().Length == 0)
          {
            message = "SeparateQty";
            return false;
          }
          else
          {
            totalSeparateQty = totalSeparateQty + DBConvert.ParseInt(rowChild.Cells["SeparateQty"].Value.ToString());
            if (rowParent.Cells["ItemCode"].Value.ToString() == rowChild.Cells["ItemCode"].Value.ToString() &&
              DBConvert.ParseInt(rowParent.Cells["Revision"].Value.ToString()) == DBConvert.ParseInt(rowChild.Cells["Revision"].Value.ToString()))
            {
              cc = cc + 1;
            }
          }
        }

        if (cc != 1)
        {
          message = "ItemCode: " + rowParent.Cells["ItemCode"].Value.ToString() + " - " + rowParent.Cells["Revision"].Value.ToString();
          return false;
        }
        // 2: So sanh Qty = Total SeparateQty
        if (qty != totalSeparateQty)
        {
          message = "Total SeparateQty";
          return false;
        }
      }
      return true;
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
      string command = string.Empty;
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

      if (tab == 1)
      {
        // Check ItemConfirm, ProcessConfirm
        for (int i = 0; i < ultItemStatus.Rows.Count; i++)
        {
          UltraGridRow row = ultItemStatus.Rows[i];
          if (DBConvert.ParseInt(row.Cells["ItemConfirm"].Value.ToString()) != 1)
          {
            message = "Item Confirm";
            return false;
          }

          if (DBConvert.ParseInt(row.Cells["ProcessConfirm"].Value.ToString()) != 1)
          {
            message = "Process Confirm";
            return false;
          }
        }
        // End Check ItemConfirm, ProcessConfirm

        //// Test
        //string stringSeparate = string.Empty;
        //string stringWOSO = string.Empty;
        //// 1: Separate Information
        //for (int i = 0; i < ultSeparateWO.Rows.Count; i++)
        //{
        //  UltraGridRow row = ultSeparateWO.Rows[i];
        //  for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
        //  {
        //    UltraGridRow rowChild = row.ChildBands[0].Rows[j];
        //    stringSeparate += rowChild.Cells["WO"].Value.ToString() + '^'
        //                    + rowChild.Cells["ItemCode"].Value.ToString() + '^'
        //                    + rowChild.Cells["Revision"].Value.ToString() + '^'
        //                    + rowChild.Cells["SeparateQty"].Value.ToString() + '~';
        //  }
        //}

        //// 2: WOSO Information
        //for (int i = 0; i < ultWOSO.Rows.Count; i++)
        //{
        //  UltraGridRow row = ultWOSO.Rows[i]; 
        //  stringWOSO += row.Cells["WO"].Value.ToString() + '^'
        //                  + row.Cells["ItemCode"].Value.ToString() + '^'
        //                  + row.Cells["Revision"].Value.ToString() + '^'
        //                  + row.Cells["SODetailPid"].Value.ToString() + '^'
        //                  + row.Cells["SeparateQty"].Value.ToString() + '~';
        //}

        //// 3: Check Qty Separate <> Qty WOSO
        //DBParameter[] input = new DBParameter[2];
        //input[0] = new DBParameter("@StringSeparate", DbType.String, stringSeparate);
        //input[1] = new DBParameter("@StringWOSO", DbType.String, stringWOSO);
        //DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNCheckQtySeparateWo", input);
        //if (dsSource != null && dsSource.Tables.Count > 0)
        //{
        // if(dsSource.Tables[0].Rows.Count > 0)
        // {
        //   message = " Total Qty <> Total SeparateQty ";
        //   return false;
        // }
        // if (dsSource.Tables[1].Rows.Count > 0)
        // {
        //   message = " Total Qty SO > Qty Remain";
        //   return false;
        // }
        //}
        //// Test

        for (int i = 0; i < ultSeparateWO.Rows.Count; i++)
        {
          UltraGridRow row = ultSeparateWO.Rows[i];
          //          long wo = DBConvert.ParseLong(row.Cells["WO"].Value.ToString());
          //          command = string.Format(@"SELECT DISTINCT WorkOrderPid
          //                                     FROM TblPLNWorkOrderConfirmedDetails
          //                                     WHERE WorkOrderPid = {0} AND (ISNULL(CloseCOM1, 0) = 1 
          //                                                              OR ISNULL(CloseCAR, 0) = 1 
          //                                                              OR ISNULL(CloseAll, 0) = 1)", wo);
          //          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(command);
          //          dt = DataBaseAccess.SearchCommandTextDataTable(command);
          //if (dt != null && dt.Rows.Count > 0)
          //{
          //  ultSeparateWO.Rows[i].Appearance.BackColor = Color.Yellow;
          //  message = " WO is Close";
          //  return false;
          //}
          for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow rowChild = row.ChildBands[0].Rows[j];
            int totalQtyWOSO = 0;
            for (int k = 0; k < ultWOSO.Rows.Count; k++)
            {
              UltraGridRow rowWOSO = ultWOSO.Rows[k];
              // Check WO
              DataTable dtCheck = new DataTable();
              string commandText = string.Empty;

              // Check SODetail
              commandText = " SELECT SaleNo";
              commandText += " FROM VPLNMasterPlan ";
              commandText += " WHERE SaleOrderDetailPid = " + DBConvert.ParseLong(rowWOSO.Cells["SODetailPid"].Value.ToString());
              dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dtCheck == null || dtCheck.Rows.Count == 0)
              {
                message = "SODetail";
                return false;
              }

              // Check SeparateQty
              if (rowWOSO.Cells["SeparateQty"].Value.ToString().Length == 0)
              {
                message = "SeparateQty";
                return false;
              }

              if (DBConvert.ParseInt(rowWOSO.Cells["SeparateQty"].Value.ToString()) > DBConvert.ParseInt(rowWOSO.Cells["Qty"].Value.ToString()) &&
                  DBConvert.ParseInt(rowWOSO.Cells["SeparateQty"].Value.ToString()) < DBConvert.ParseInt(rowWOSO.Cells["QtyShipped"].Value.ToString()))
              {
                message = "SeparateQty";
                return false;
              }

              // Check Separate QtyWOSO
              if (DBConvert.ParseInt(rowChild.Cells["WO"].Value.ToString()) == DBConvert.ParseInt(rowWOSO.Cells["WO"].Value.ToString()) &&
                    rowChild.Cells["ItemCode"].Value.ToString() == rowWOSO.Cells["ItemCode"].Value.ToString() &&
                    DBConvert.ParseInt(rowChild.Cells["Revision"].Value.ToString()) == DBConvert.ParseInt(rowWOSO.Cells["Revision"].Value.ToString()))
              {
                totalQtyWOSO = totalQtyWOSO + DBConvert.ParseInt(rowWOSO.Cells["SeparateQty"].Value.ToString());
              }
            }

            // So Sanh
            if (DBConvert.ParseInt(rowChild.Cells["SeparateQty"].Value.ToString()) != totalQtyWOSO)
            {
              message = "Total SeparateQty WOSO";
              return false;
            }
          }

        }
      }
      else if (tab == 2) // Allocation Material
      {
        for (int i = 0; i < ultAllocateMaterial.Rows.Count; i++)
        {
          UltraGridRow rowParent = ultAllocateMaterial.Rows[i];
          for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow row = rowParent.ChildBands[0].Rows[j];
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
      return true;
    }

    private bool CheckDataWhenConfirm(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      commandText = string.Format(@"
                                      SELECT DT.WO
                                      FROM TblPLNSeparateWONoteDetail DT
	                                      INNER JOIN TblPLNWorkOrderConfirmedDetails WOC ON DT.WO = WOC.WorkOrderPid
													                                      AND DT.ItemCode = WOC.ItemCode
													                                      AND DT.Revision = WOC.Revision
                                      WHERE DT.Qty > WOC.Qty
	                                      AND DT.SeparateNoPid = {0}
                                  ", this.cancelWOPid);

      DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCheck != null && dtCheck.Rows.Count > 0)
      {
        message = "Separate Qty <= WO Qty";
        return false;
      }

      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@SeparatePid", DbType.Int64, this.cancelWOPid);
      dtCheck = DataBaseAccess.SearchStoreProcedureDataTable("spPLNSeparateWOCheckWOSO_Select", input);
      if (dtCheck != null && dtCheck.Rows.Count > 0)
      {
        message = "WOSO Qty <= WOSO Remain";
        return false;
      }

      // Check close Wo
      DataTable dt = new DataTable();
      //      for (int i = 0; i < ultWOSO.Rows.Count; i++)
      //      {
      //        UltraGridRow rowSeparate = ultWOSO.Rows[i];
      //        long wo = DBConvert.ParseLong(rowSeparate.Cells["WO"].Value.ToString());
      //        commandText = string.Format(@"SELECT DISTINCT WorkOrderPid
      //                                     FROM TblPLNWorkOrderConfirmedDetails
      //                                     WHERE WorkOrderPid = {0} AND (ISNULL(CloseCOM1, 0) = 1 
      //                                                              OR ISNULL(CloseCAR, 0) = 1 
      //                                                              OR ISNULL(CloseAll, 0) = 1)", wo);

      //        dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      //        if (dt != null && dt.Rows.Count > 0)
      //        {
      //          ultWOSO.Rows[i].Appearance.BackColor = Color.Yellow;
      //          message = " WO is Close";
      //          return false;
      //        }
      //      }

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
        input[0] = new DBParameter("@SeparateWOPid", DbType.Int64, this.cancelWOPid);
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spPLNSeparateWOData_Update", 300, input, output);
        if (DBConvert.ParseLong(output[0].Value.ToString()) <= 0)
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
          success = this.SaveTab1SeparateWODetail();
          if (success)
          {
            success = this.SaveTab1SeparateWOSO();
          }
        }
        else if (tab == 2)
        {
          success = this.SaveTab2();
        }
        else if (tab == 3)
        {
          this.SaveTab3();
        }
      }
      return success;
    }

    /// <summary>
    /// Lock Master Information
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
      inputParam[1] = new DBParameter("@Prefix", DbType.String, "SWO");
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
    /// Save Information
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
      inputParam[1] = new DBParameter("@Prefix", DbType.String, "SWO");
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
    /// Save Information Separate Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveTab1SeparateWODetail()
    {
      bool success = true;
      for (int i = 0; i < ultSeparateWO.Rows.Count; i++)
      {
        UltraGridRow row = ultSeparateWO.Rows[i];
        DBParameter[] inputParam = new DBParameter[6];
        inputParam[0] = new DBParameter("@SeparateNoPid", DbType.Int64, this.cancelWOPid);
        inputParam[1] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(row.Cells["WO"].Value.ToString()));
        inputParam[2] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CarcassCode"].Value.ToString());
        inputParam[3] = new DBParameter("@ItemCode", DbType.String, row.Cells["ItemCode"].Value.ToString());
        inputParam[4] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
        inputParam[5] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(row.Cells["Qty"].Value.ToString()));

        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spPLNSeparateWODetail_Insert", inputParam, outputParam);
        long separateDetailPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (separateDetailPid <= 0)
        {
          success = false;
        }
        else
        {
          success = this.SaveTab1SeparateWOSubDetail(row, separateDetailPid);
        }
      }
      return success;
    }

    /// <summary>
    /// Save Information Separate Sub Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveTab1SeparateWOSubDetail(UltraGridRow rowParent, long SeparateDetailPid)
    {
      for (int i = 0; i < rowParent.ChildBands[0].Rows.Count; i++)
      {
        UltraGridRow row = rowParent.ChildBands[0].Rows[i];
        DBParameter[] inputParam = new DBParameter[4];
        inputParam[0] = new DBParameter("@SeparateDetailNoPid", DbType.Int64, SeparateDetailPid);
        inputParam[1] = new DBParameter("@ItemCode", DbType.String, row.Cells["ItemCode"].Value.ToString());
        inputParam[2] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
        inputParam[3] = new DBParameter("@SeparateQty", DbType.Int32, DBConvert.ParseInt(row.Cells["SeparateQty"].Value.ToString()));

        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spPLNSeparateWOSubDetail_Insert", inputParam, outputParam);
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
    private bool SaveTab1SeparateWOSO()
    {
      for (int i = 0; i < ultWOSO.Rows.Count; i++)
      {
        UltraGridRow row = ultWOSO.Rows[i];
        DBParameter[] inputParam = new DBParameter[7];
        inputParam[0] = new DBParameter("@SeparateNoPid", DbType.Int64, this.cancelWOPid);
        inputParam[1] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(row.Cells["WO"].Value.ToString()));
        inputParam[2] = new DBParameter("@SODetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["SODetailPid"].Value.ToString()));
        inputParam[3] = new DBParameter("@ItemCode", DbType.String, row.Cells["ItemCode"].Value.ToString());
        inputParam[4] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
        inputParam[5] = new DBParameter("@QtySeparate", DbType.Int32, DBConvert.ParseInt(row.Cells["SeparateQty"].Value.ToString()));
        inputParam[6] = new DBParameter("@Remark", DbType.String, row.Cells["Remark"].Value.ToString());

        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spPLNSeparateWONoteDetailWithWOSO_Insert", inputParam, outputParam);
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
      return success;
    }

    /// <summary>
    /// Save Allocate Material
    /// </summary>
    /// <returns></returns>
    private bool SaveAllocateMaterial()
    {
      for (int i = 0; i < ultAllocateMaterial.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultAllocateMaterial.Rows[i];
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
    /// Save Request Request Component Store
    /// </summary>
    /// <returns></returns>
    private bool SaveTab3()
    {
      dsRequestITW = (DataSet)ultWIPCAR.DataSource;
      return true;
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
      inputParam[0] = new DBParameter("@SeparateWOPid", DbType.Int64, this.cancelWOPid);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNSeparateWOGetInfoTab1_Select", inputParam);
      if (dsSource != null)
      {
        DataSet dsSeparateInfo = this.CreateDataSetSeparateInfo();
        dsSeparateInfo.Tables["dtParent"].Merge(dsSource.Tables[0]);
        dsSeparateInfo.Tables["dtChild"].Merge(dsSource.Tables[1]);
        // Information Separate WO
        ultSeparateWO.DataSource = dsSeparateInfo;
        // Information WO-SO
        ultWOSO.DataSource = dsSource.Tables[2];
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
      commandText = string.Format(@"SELECT CAST(DT.WO AS bigint) WO,  DT.CarcassCode, DT.ItemCode, DT.Revision, DT.Qty - SUP.SeparateQty AS QtyCancel
                                    FROM  TblPLNSeparateWONoteDetail DT
	                                    INNER JOIN TblPLNSeparateWONoteSubDetail SUP ON DT.Pid = SUP.SeparateDetailPid
		                                    AND DT.ItemCode = SUP.ItemCode
		                                    AND DT.Revision = SUP.Revision
                                    WHERE DT.SeparateNoPid = {0}", this.cancelWOPid);
      DataTable dtParent = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtParent != null)
      {
        dsSource.Tables["dtParent"].Merge(dtParent);
      }
      // Load Material
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@SeparatePid", DbType.Int64, cancelWOPid);

      DataTable dtChildMaterial = DataBaseAccess.SearchStoreProcedureDataTable("spPLNSeparateWOAllocateMaterial_Select", inputParam);
      if (dtChildMaterial != null)
      {
        dsSource.Tables["dtChildMaterial"].Merge(dtChildMaterial);
      }

      ultAllocateMaterial.DataSource = dsSource;
    }

    /// <summary>
    /// Load tab3
    /// </summary>
    /// <param name="cancelWOPid"></param>
    private void LoadTab3(long cancelWOPid)
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
        inputParam[0] = new DBParameter("@SeparateWOPid", DbType.Int64, this.cancelWOPid);

        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNSeparateWOGetInfoRequestITW_Select", inputParam);
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

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["DetailPid"].Hidden = true;
      //e.Layout.Bands[1].Columns["SaleCode"].Hidden = true;

      e.Layout.Bands[0].Columns["WIPCAR"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SUB"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ITW"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["WIPITEM"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[1].Columns["QtyShipped"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["WO"].Hidden = true;
      e.Layout.Bands[1].Columns["CarcassCode"].Hidden = true;

      e.Layout.Bands[0].Columns["WO"].ValueList = ultDropWO;
      e.Layout.Bands[0].Columns["CarcassCode"].ValueList = ultDropCarcassCode;
      e.Layout.Bands[0].Columns["ItemCode"].ValueList = ultDropItemCode;
      e.Layout.Bands[1].Columns["ItemCode"].ValueList = ultDropItemCodeSeparate;

      e.Layout.Bands[0].Columns["WIPCAR"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["SUB"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["ITW"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["WIPITEM"].CellAppearance.ForeColor = Color.Blue;

      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["QtyShipped"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["SeparateQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["WO"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["CarcassCode"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["ItemCode"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Bands[1].Columns["ItemCode"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[1].Columns["SeparateQty"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[1].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      if (this.tabLock1 == 1)
      {
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

        e.Layout.Bands[1].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
        e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      }

      if (this.status == 1)
      {
        e.Layout.Bands[0].Columns["WIPCAR"].Hidden = true;
        e.Layout.Bands[0].Columns["SUB"].Hidden = true;
        e.Layout.Bands[0].Columns["ITW"].Hidden = true;
        e.Layout.Bands[0].Columns["WIPITEM"].Hidden = true;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init Layout Status Item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultItemStatus_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["ItemConfirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ProcessConfirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Alter Cell Update WO - SO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultWOSOCancel_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "ItemCode":
          row.Cells["SODetailPid"].Value = DBNull.Value;
          row.Cells["SaleNo"].Value = DBNull.Value;
          row.Cells["PONo"].Value = DBNull.Value;
          row.Cells["ScheduleDelivery"].Value = DBNull.Value;
          row.Cells["Qty"].Value = DBNull.Value;
          row.Cells["QtyShipped"].Value = DBNull.Value;

          if (row.Cells["ItemCode"].Value.ToString().Length > 0)
          {
            string commandText = string.Empty;
            commandText = "SELECT ItemCode FROM TblBOMItemInfo WHERE ItemCode = '" + row.Cells["ItemCode"].Value.ToString() + "'";
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt != null && dt.Rows.Count > 0)
            {
              row.Cells["Revision"].Value = DBConvert.ParseInt(ultDropItemCodeWOSO.SelectedRow.Cells["Revision"].Value.ToString());
            }
            else
            {
              row.Cells["Revision"].Value = DBNull.Value;
            }
          }
          else
          {
            row.Cells["Revision"].Value = DBNull.Value;
          }
          break;
        case "SaleNo":
          if (row.Cells["SaleNo"].Text.Length > 0)
          {
            // Check SaleNo
            string saleNo = row.Cells["SaleNo"].Text;

            string commandText = string.Empty;
            commandText = " SELECT SaleNo";
            commandText += " FROM VPLNMasterPlan ";
            commandText += " WHERE SaleNo = '" + saleNo + "'";
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt != null && dt.Rows.Count > 0)
            {
              row.Cells["SODetailPid"].Value = DBConvert.ParseLong(ultDropSODetailItemRev.SelectedRow.Cells["SODetailPid"].Value.ToString());
              row.Cells["PONo"].Value = ultDropSODetailItemRev.SelectedRow.Cells["PONo"].Value.ToString();
              row.Cells["ScheduleDelivery"].Value = ultDropSODetailItemRev.SelectedRow.Cells["ScheduleDelivery"].Value.ToString();
              row.Cells["Qty"].Value = DBConvert.ParseInt(ultDropSODetailItemRev.SelectedRow.Cells["Qty"].Value.ToString());
              row.Cells["QtyShipped"].Value = DBConvert.ParseInt(ultDropSODetailItemRev.SelectedRow.Cells["QtyShipped"].Value.ToString());
            }
            else
            {
              row.Cells["SODetailPid"].Value = DBNull.Value;
              row.Cells["SaleNo"].Value = DBNull.Value;
              row.Cells["PONo"].Value = DBNull.Value;
              row.Cells["ScheduleDelivery"].Value = DBNull.Value;
              row.Cells["Qty"].Value = DBNull.Value;
              row.Cells["QtyShipped"].Value = DBNull.Value;
              row.Cells["SeparateQty"].Value = DBNull.Value;
            }
          }
          else
          {
            row.Cells["SODetailPid"].Value = DBNull.Value;
            row.Cells["PONo"].Value = DBNull.Value;
            row.Cells["ScheduleDelivery"].Value = DBNull.Value;
            row.Cells["Qty"].Value = DBNull.Value;
            row.Cells["QtyShipped"].Value = DBNull.Value;
            row.Cells["SeparateQty"].Value = DBNull.Value;
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
    private void ultSeparateWO_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "WO":
          row.Cells["Pid"].Value = ultSeparateWO.Rows.Count;
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
          if (row.ParentRow == null)
          {
            // Check Trung
            if (row.Cells["ItemCode"].Text.Length > 0)
            {
              for (int i = 0; i < ultSeparateWO.Rows.Count; i++)
              {
                UltraGridRow rowGird = ultSeparateWO.Rows[i];
                if (rowGird != row &&
                  DBConvert.ParseInt(rowGird.Cells["WO"].Value.ToString()) == DBConvert.ParseInt(row.Cells["WO"].Value.ToString()) &&
                  rowGird.Cells["ItemCode"].Value.ToString() == ultDropItemCode.SelectedRow.Cells["ItemCode"].Value.ToString() &&
                  DBConvert.ParseInt(rowGird.Cells["Revision"].Value.ToString()) == DBConvert.ParseInt(ultDropItemCode.SelectedRow.Cells["Revision"].Value.ToString()))
                {
                  WindowUtinity.ShowMessageError("ERR0029", "WO - Carcass - ItemCode - Revision");
                  row.Cells["ItemCode"].Value = DBNull.Value;
                  return;
                }
              }
              // End
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
            }
            else
            {
              row.Cells["Revision"].Value = DBNull.Value;
              row.Cells["Qty"].Value = DBNull.Value;
            }
          }
          else
          {
            if (row.Cells["ItemCode"].Text.Length > 0)
            {
              row.Cells["DetailPid"].Value = DBConvert.ParseLong(row.ParentRow.Cells["Pid"].Value.ToString());
              // Check Trung ItemCode
              UltraGridRow rowParent = row.ParentRow;
              if (e.Cell.Row.Cells["ItemCode"].Text.Length > 0)
              {
                for (int i = 0; i < rowParent.ChildBands[0].Rows.Count; i++)
                {
                  UltraGridRow rowChild = rowParent.ChildBands[0].Rows[i];
                  if (rowChild != row &&
                      rowChild.Cells["ItemCode"].Value.ToString() == ultDropItemCodeSeparate.SelectedRow.Cells["ItemCode"].Text &&
                      DBConvert.ParseInt(rowChild.Cells["Revision"].Value.ToString()) == DBConvert.ParseInt(ultDropItemCodeSeparate.SelectedRow.Cells["Revision"].Text))
                  {
                    WindowUtinity.ShowMessageError("ERR0029", "ItemCode");
                    row.Cells["ItemCode"].Value = DBNull.Value;
                    return;
                  }
                }
              }
              // End

              string commandText = string.Empty;
              commandText = " SELECT DISTINCT ItemCode";
              commandText += " FROM TblBOMItemInfo";
              commandText += " WHERE CarcassCode  = '" + row.ParentRow.Cells["CarcassCode"].Value.ToString() + "'";
              System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dtSource != null && dtSource.Rows.Count > 0)
              {
                row.Cells["Revision"].Value = DBConvert.ParseInt(ultDropItemCodeSeparate.SelectedRow.Cells["Revision"].Value.ToString());
                row.Cells["QtyShipped"].Value = DBConvert.ParseInt(ultDropItemCodeSeparate.SelectedRow.Cells["QtyShipped"].Value.ToString());
              }
              else
              {
                row.Cells["Revision"].Value = DBNull.Value;
                row.Cells["QtyShipped"].Value = DBNull.Value;
              }
            }
            else
            {
              row.Cells["Revision"].Value = DBNull.Value;
              row.Cells["QtyShipped"].Value = DBNull.Value;
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
    private void ultWOSO_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "ItemCode":
          row.Cells["IsDelete"].Value = 1;
          row.Cells["SODetailPid"].Value = DBNull.Value;
          row.Cells["SaleNo"].Value = DBNull.Value;
          row.Cells["PONo"].Value = DBNull.Value;
          row.Cells["ScheduleDelivery"].Value = DBNull.Value;
          row.Cells["Qty"].Value = DBNull.Value;
          row.Cells["QtyShipped"].Value = DBNull.Value;
          if (row.Cells["ItemCode"].Value.ToString().Length > 0)
          {
            string commandText = string.Empty;
            commandText = "SELECT ItemCode FROM TblBOMItemInfo WHERE ItemCode = '" + row.Cells["ItemCode"].Text + "'";
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt != null && dt.Rows.Count > 0)
            {
              row.Cells["Revision"].Value = DBConvert.ParseInt(ultDropItemCodeWOSO.SelectedRow.Cells["Revision"].Value.ToString());
            }
            else
            {
              row.Cells["Revision"].Value = DBNull.Value;
            }
          }
          else
          {
            row.Cells["Revision"].Value = DBNull.Value;
          }
          break;
        case "SaleNo":
          if (row.Cells["SaleNo"].Text.Length > 0)
          {
            // Check SaleNo
            string saleNo = row.Cells["SaleNo"].Text;

            string commandText = string.Empty;
            commandText = " SELECT SaleNo";
            commandText += " FROM VPLNMasterPlan ";
            commandText += " WHERE SaleNo = '" + saleNo + "'";
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt != null && dt.Rows.Count > 0)
            {
              row.Cells["SODetailPid"].Value = DBConvert.ParseLong(ultDropSODetailItemRev.SelectedRow.Cells["SODetailPid"].Value.ToString());

              // Check Trung
              for (int i = 0; i < ultWOSO.Rows.Count; i++)
              {
                UltraGridRow rowGird = ultWOSO.Rows[i];
                if (rowGird != row &&
                  DBConvert.ParseLong(rowGird.Cells["SODetailPid"].Value.ToString()) == DBConvert.ParseLong(row.Cells["SODetailPid"].Value.ToString()))
                {
                  WindowUtinity.ShowMessageError("ERR0029", "SaleNo");
                  //row.Cells["SaleNo"].Value = DBNull.Value;
                  row.Cells["SODetailPid"].Value = DBNull.Value;
                  row.Cells["PONo"].Value = DBNull.Value;
                  row.Cells["ScheduleDelivery"].Value = DBNull.Value;
                  row.Cells["Qty"].Value = DBNull.Value;
                  row.Cells["QtyShipped"].Value = DBNull.Value;
                  row.Cells["SeparateQty"].Value = DBNull.Value;
                  return;
                }
              }
              // End

              row.Cells["PONo"].Value = ultDropSODetailItemRev.SelectedRow.Cells["PONo"].Value.ToString();
              row.Cells["ScheduleDelivery"].Value = ultDropSODetailItemRev.SelectedRow.Cells["ScheduleDelivery"].Value.ToString();
              row.Cells["Qty"].Value = DBConvert.ParseInt(ultDropSODetailItemRev.SelectedRow.Cells["Qty"].Value.ToString());
              row.Cells["QtyShipped"].Value = DBConvert.ParseInt(ultDropSODetailItemRev.SelectedRow.Cells["QtyShipped"].Value.ToString());
            }
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
    private void ultWOSOCancel_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "SaleNo":
          if (row.Cells["ItemCode"].Value.ToString().Length > 0 &&
              DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()) != int.MinValue)
          {
            string itemCode = row.Cells["ItemCode"].Value.ToString();
            int revision = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
            this.LoadDropDownSODetailItemRev(itemCode, revision, row);
          }
          break;
        default:
          break;
      }
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

      e.Layout.Bands[0].Columns["SODetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["isDelete"].Hidden = true;

      e.Layout.Bands[0].Columns["WO"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["ItemCode"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["SaleNo"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["SeparateQty"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Bands[0].Columns["PONo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ScheduleDelivery"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyShipped"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["WO"].ValueList = ultDropWO;
      e.Layout.Bands[0].Columns["ItemCode"].ValueList = ultDropItemCodeWOSO;
      e.Layout.Bands[0].Columns["SaleNo"].ValueList = ultDropSODetail;

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyShipped"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["SeparateQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      if (this.tabLock1 == 1)
      {
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      }

      if (this.status == 1)
      {
        e.Layout.Bands[0].Columns["Qty"].Hidden = true;
        e.Layout.Bands[0].Columns["QtyShipped"].Hidden = true;
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

      if (this.tabLock3 == 1)
      {
        e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      }

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
          if (row.ParentRow == null)
          {
            if (DBConvert.ParseInt(row.Cells["WO"].Value.ToString()) != Int32.MinValue &&
               row.Cells["CarcassCode"].Value.ToString().Length > 0)
            {
              this.LoadItemCode(DBConvert.ParseInt(row.Cells["WO"].Value.ToString()), row.Cells["CarcassCode"].Value.ToString());
            }
            else
            {
              this.LoadItemCode(-1, "");
            }
          }
          else
          {
            if (row.ParentRow.Cells["CarcassCode"].Value.ToString().Length > 0 &&
                 row.ParentRow.Cells["ItemCode"].Value.ToString().Length > 0)
            {
              this.LoadItemCodeBelongCarcass(DBConvert.ParseInt(row.ParentRow.Cells["WO"].Value.ToString()), row.ParentRow.Cells["CarcassCode"].Value.ToString());
            }
            else
            {
              this.LoadItemCodeBelongCarcass(-1, "");
            }
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
          if (row.ParentRow == null)
          {
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
          }
          else
          {
            if (row.Cells["ItemCode"].Value.ToString().Length > 0)
            {
              string commandText = string.Empty;
              commandText = " SELECT DISTINCT ItemCode";
              commandText += " FROM TblBOMItemInfo";
              commandText += " WHERE CarcassCode  = '" + row.ParentRow.Cells["CarcassCode"].Value.ToString() + "'";
              System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dtSource != null && dtSource.Rows.Count > 0)
              {
                row.Cells["Revision"].Value = DBConvert.ParseInt(ultDropItemCodeSeparate.SelectedRow.Cells["Revision"].Value.ToString());
                row.Cells["QtyShipped"].Value = DBConvert.ParseInt(ultDropItemCodeSeparate.SelectedRow.Cells["QtyShipped"].Value.ToString());
              }
              else
              {
                row.Cells["Revision"].Value = DBNull.Value;
                row.Cells["QtyShipped"].Value = DBNull.Value;
              }
            }
            else
            {
              row.Cells["Revision"].Value = DBNull.Value;
              row.Cells["QtyShipped"].Value = DBNull.Value;
            }
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
        case "ItemCode":
          if (row.Cells["ItemCode"].Text.Length > 0)
          {
            string commandText = string.Empty;
            commandText = "SELECT ItemCode FROM TblBOMItemInfo WHERE ItemCode = '" + row.Cells["ItemCode"].Text + "'";
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt == null || dt.Rows.Count == 0)
            {
              e.Cancel = true;
              return;
            }
          }
          break;
        case "SaleNo":
          if (row.Cells["SaleNo"].Text.Length > 0)
          {
            string commandText = string.Empty;
            commandText = " SELECT SaleNo";
            commandText += " FROM VPLNMasterPlan ";
            commandText += " WHERE SaleNo = '" + row.Cells["SaleNo"].Text + "'";
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt == null || dt.Rows.Count == 0)
            {
              e.Cancel = true;
            }
          }
          break;
        case "SeparateQty":
          if (row.Cells["SeparateQty"].Text.Length > 0)
          {
            if (DBConvert.ParseInt(row.Cells["SeparateQty"].Text) < 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "SeparateQty >= 0");
              e.Cancel = true;
            }
            else if (DBConvert.ParseInt(row.Cells["SeparateQty"].Text) > DBConvert.ParseInt(row.Cells["Qty"].Value.ToString()) ||
                    DBConvert.ParseInt(row.Cells["SeparateQty"].Text) < DBConvert.ParseInt(row.Cells["QtyShipped"].Value.ToString()))
            {
              WindowUtinity.ShowMessageError("ERR0001", "SeparateQty");
              e.Cancel = true;
            }
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
    /// Before Row Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultWOSO_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      // Check IsDelete = 1 Khong Cho Delete
      foreach (UltraGridRow row in e.Rows)
      {
        if (DBConvert.ParseInt(row.Cells["IsDelete"].Value.ToString()) == 0)
        {
          WindowUtinity.ShowMessageError("ERR0093", "Row");
          e.Cancel = true;
          return;
        }
      }

      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
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

      else if (tabControl1.SelectedTab == tabControl1.TabPages["tabWIPCAR"])
      {
        if (this.cancelWOPid != long.MinValue)
        {
          this.LoadTab3(this.cancelWOPid);
        }
      }
    }

    /// <summary>
    /// Check Change Status Cancel WO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCheckStatus_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckStatusValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      // End Check Valid

      // 1: Status Item
      for (int i = 0; i < ultSeparateWO.Rows.Count; i++)
      {
        UltraGridRow row = ultSeparateWO.Rows[i];
        DBParameter[] input = new DBParameter[4];
        input[0] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(row.Cells["WO"].Value.ToString()));
        input[1] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CarcassCode"].Value.ToString());

        DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPLNSeparateWOGetInfoLocationAffect_Select", input);
        if (dt != null && dt.Rows.Count > 0)
        {
          row.Cells["WIPCAR"].Value = dt.Rows[0]["WIPCAR"].ToString();
          row.Cells["SUB"].Value = dt.Rows[0]["SUB"].ToString();
          row.Cells["ITW"].Value = dt.Rows[0]["ITW"].ToString();
          row.Cells["WIPITEM"].Value = dt.Rows[0]["WIPITEM"].ToString();
        }
      }
      this.GetStatusItem();

      // 3: WO - SO
      this.GetInfoWOSO();

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
    /// Lock tab5
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
    /// Save Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckDataWhenConfirm(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

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

      // Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      //// Load User Guide
      //this.CreateUserGuide();
      //// Show User Guide
      //this.ShowUserGuide();
      // Load Main Data
      this.LoadMainData();

      this.LoadTab1(this.cancelWOPid);
      this.LoadTab2(this.cancelWOPid);
      this.LoadTab3(this.cancelWOPid);
    }

    private void chkHide_CheckedChanged(object sender, EventArgs e)
    {
      if (chkHide.Checked)
      {
        ultItemStatus.Visible = false;
      }
      else
      {
        ultItemStatus.Visible = true;
      }
    }

    /// <summary>
    /// Click User Guide
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void linkLabUserGuide_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      this.ShowUserGuide();
    }

    /// <summary>
    /// Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSeparateWO_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      string commandText = string.Empty;
      DataTable dtCheck = new DataTable();
      switch (columnName)
      {
        case "WO":
          if (row.Cells["WO"].Text.Length > 0)
          {
            commandText = "SELECT WorkOrderPid FROM TblPLNWorkOrderConfirmedDetails WHERE WorkOrderPid  = " + DBConvert.ParseInt(row.Cells["WO"].Text);
            dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCheck == null || dtCheck.Rows.Count == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "WO");
              e.Cancel = true;
            }
          }
          break;
        case "CarcassCode":
          if (row.Cells["CarcassCode"].Text.Length > 0)
          {
            commandText = string.Format(@"SELECT DISTINCT CarcassCode 
                                          FROM TblPLNWorkOrderConfirmedDetails 
                                          WHERE WorkOrderPid  = {0} AND CarcassCode = '{1}'", DBConvert.ParseInt(row.Cells["WO"].Value.ToString()), row.Cells["CarcassCode"].Text);
            dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCheck == null || dtCheck.Rows.Count == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "CarcassCode");
              e.Cancel = true;
            }
          }
          break;
        case "ItemCode":
          if (row.ParentRow == null)
          {
            if (DBConvert.ParseInt(row.Cells["WO"].Value.ToString()) != int.MinValue &&
                row.Cells["CarcassCode"].Value.ToString().Length > 0 &&
                row.Cells["ItemCode"].Text.Length > 0)
            {
              commandText = string.Format(@"SELECT DISTINCT CarcassCode 
                                          FROM TblPLNWorkOrderConfirmedDetails 
                                          WHERE WorkOrderPid  = {0} AND CarcassCode = '{1}' AND ItemCode = '{2}'", DBConvert.ParseInt(row.Cells["WO"].Value.ToString()), row.Cells["CarcassCode"].Value.ToString(), row.Cells["ItemCode"].Text);
              dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dtCheck == null || dtCheck.Rows.Count == 0)
              {
                WindowUtinity.ShowMessageError("ERR0001", "ItemCode");
                e.Cancel = true;
              }
            }
          }
          break;
        default:
          break;
      }
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
        string storeName = "spPLNSeparateWOInformation_Delete";
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@SeparateWOPid", DbType.Int64, this.cancelWOPid);

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
    /// Get Parth
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnBrowseItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcel.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }
    /// <summary>
    /// Import Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtImportExcel.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }
      // Get Items Count

      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [WOSeparate (1)$L3:L4]");
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
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), string.Format("SELECT * FROM [WOSeparate (1)$B5:K{0}]", itemCount + 5));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      // Input Data
      DataTable data = dsItemList.Tables[0];
      DataTable dtInput = this.dtWOSeparateResult();
      for (int k = 0; k < data.Rows.Count; k++)
      {
        long wo1 = DBConvert.ParseLong(data.Rows[k]["WO"].ToString());
        string fromitemCode = data.Rows[k]["FromItemCode"].ToString();
        int fromrevision = DBConvert.ParseInt(data.Rows[k]["FromRevision"].ToString());
        int separateQty = DBConvert.ParseInt(data.Rows[k]["SeparateQty"].ToString());
        string toitemCode = data.Rows[k]["ToItemCode"].ToString();
        string fromsaleNo = data.Rows[k]["FromSaleNo"].ToString();
        string tosaleNo = data.Rows[k]["ToSaleNo"].ToString();
        int toRevision = DBConvert.ParseInt(data.Rows[k]["ToRevision"].ToString());
        string fromRemark = data.Rows[k]["FromRemark"].ToString();
        string toRemark = data.Rows[k]["ToRemark"].ToString();
        DataRow[] rowCheckExcel = data.Select(string.Format("WO = {0} and FromItemCode = '{1}' and FromRevision = {2} ", wo1, fromitemCode, fromrevision));

        if (rowCheckExcel.Length == 0)
        {
          WindowUtinity.ShowMessageError("Data Error :" + "WO: " + wo1.ToString() + " " + " FromItemCode: " + fromitemCode + " " + " FromRevision: " + fromrevision.ToString());
          return;
        }
        string cmtext = string.Format(@"SELECT WorkOrderPid Wo, ItemCode, CarcassCode, Revision, Qty
                                        FROM TblPLNWorkOrderConfirmedDetails 
                                        WHERE WorkOrderPid = {0} 
                                        AND ItemCode = '{1}' 
                                        AND Revision = {2} ", wo1, fromitemCode, fromrevision);
        DataTable dtWOInfo = DataBaseAccess.SearchCommandTextDataTable(cmtext);
        long wo = long.MinValue;
        string itemCode = string.Empty;
        int revision = int.MinValue;
        int qty = int.MinValue;
        string carcassCode;
        if (dtWOInfo != null && dtWOInfo.Rows.Count > 0)
        {
          wo = DBConvert.ParseLong(dtWOInfo.Rows[0]["Wo"].ToString());
          itemCode = dtWOInfo.Rows[0]["ItemCode"].ToString();
          carcassCode = dtWOInfo.Rows[0]["CarcassCode"].ToString();
          qty = DBConvert.ParseInt(dtWOInfo.Rows[0]["Qty"].ToString());
          revision = DBConvert.ParseInt(dtWOInfo.Rows[0]["Revision"].ToString());
          if (separateQty < 0 || separateQty > qty)
          {
            WindowUtinity.ShowMessageError("ERR0150", "Separate Qty", string.Format(@"Wo: {0}, ItemCode: {1}, Revision: {2}", wo.ToString(), itemCode, revision.ToString()));
            return;
          }

          // Check the same carcass
          string command = string.Format(@"SELECT CarcassCode
                                          FROM TblBOMItemInfo 
                                          WHERE ItemCode = '{0}' 
	                                        AND Revision = {1} ", toitemCode, toRevision);
          DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(command);
          if (dtCarcass != null && dtCarcass.Rows.Count > 0)
          {
            string separateCarcass = dtCarcass.Rows[0]["CarcassCode"].ToString();
            if (string.Compare(carcassCode, separateCarcass, true) != 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", string.Format(@"Separate item: '{0}' (Rev {1}) is not the same carcass with item '{2}' (Rev {3}) in WO #{4})", toitemCode, toRevision, fromitemCode, fromrevision, wo1));
              return;
            }
          }
          else
          {
            WindowUtinity.ShowMessageError("ERR0001", string.Format(@" Separate ItemCode: {0}, Separate Revision: {1}", toitemCode.ToString(), toRevision.ToString()));
            return;
          }
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0001", string.Format(@"Wo: {0}, ItemCode: {1}, Revision: {2}", wo1.ToString(), fromitemCode, fromrevision.ToString()));
          return;
        }
        // Add Data
        DataRow rowadd = dtInput.NewRow();
        rowadd["WO"] = wo1;
        rowadd["FromItemCode"] = fromitemCode;
        rowadd["FromSaleNo"] = fromsaleNo;
        rowadd["FromRevision"] = fromrevision;
        rowadd["FromRemark"] = fromRemark;
        rowadd["ToItemCode"] = toitemCode;
        rowadd["ToSaleNo"] = tosaleNo;
        rowadd["ToRevision"] = toRevision;
        rowadd["SeparateQty"] = separateQty;
        rowadd["ToRemark"] = toRemark;
        dtInput.Rows.Add(rowadd);
      }
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtInput);
      DataSet ds = Shared.DataBaseUtility.SqlDataBaseAccess.SearchStoreProcedure("spPLNSeparateWo", inputParam);
      ds.Relations.Add(new DataRelation("dtParent_dtChild", ds.Tables[0].Columns["Pid"], ds.Tables[1].Columns["DetailPid"], false));
      ultSeparateWO.DataSource = ds;
      ultWOSO.DataSource = ds.Tables[2];
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckStatusValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      // End Check Valid
      this.GetStatusItem();
      btnCheckStatus.Enabled = false;
    }
    /// <summary>
    /// Create Table
    /// </summary>
    /// <returns></returns>
    private DataTable dtWOSeparateResult()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("WO", typeof(System.Int64));
      dt.Columns.Add("FromItemCode", typeof(System.String));
      dt.Columns.Add("FromSaleNo", typeof(System.String));
      dt.Columns.Add("FromRevision", typeof(System.Int32));
      dt.Columns.Add("FromRemark", typeof(System.String));
      dt.Columns.Add("ToItemCode", typeof(System.String));
      dt.Columns.Add("ToSaleNo", typeof(System.String));
      dt.Columns.Add("ToRevision", typeof(System.Int32));
      dt.Columns.Add("SeparateQty", typeof(System.Int32));
      dt.Columns.Add("ToRemark", typeof(System.String));
      return dt;
    }
    /// <summary>
    /// Get Tamplate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "PLN_02_023";
      string sheetName = "WOSeparate";
      string outFileName = "WOSeparate";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
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

  }
}
