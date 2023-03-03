/*
 * Author       : Duong Minh
 * CreateDate   : 20/2/2012
 * Description  : Report
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using System.IO;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared;
using DaiCo.FinishGoodWarehouse.Reports;

namespace DaiCo.FinishGoodWarehouse
{
  public partial class ViewFGH_99_001 : MainUserControl
  {
    #region Field
    public int ncategory = int.MinValue;
    public long locationPid = long.MinValue;
    public long inStorePid = long.MinValue;
    public string inStoreCode = string.Empty;
    public long outStorePid = long.MinValue;
    public string outStoreCode = string.Empty;
    #endregion Field

    #region Init

    public ViewFGH_99_001()
    {
      InitializeComponent();
    }

    private void ViewPLN_99_001_Load(object sender, EventArgs e)
    {
      if (ncategory == 1)
      {
        this.TransferLocationBoxCodeReport();
      }
      else if (ncategory == 2)
      {
        this.TransferLocationBoxCodeDetailReport();
      }
      else if (ncategory == 3)
      {
        this.ReceivingBoxCodeReport();
      }
      else if (ncategory == 4)
      {
        this.ReceivingBoxCodeDetailReport();
      }
      else if (ncategory == 5)
      {
        this.AdjusmentInBoxCodeReport();
      }
      else if (ncategory == 6)
      {
        this.AdjusmentInBoxCodeDetailReport();
      }
      else if (ncategory == 7)
      {
        this.ReturnFromCustomerBoxCodeReport();
      }
      else if (ncategory == 8)
      {
        this.ReturnFromCustomerBoxCodeDetailReport();
      }
      else if (ncategory == 9)
      {
        this.IssuingBoxCodeReport();
      }
      else if (ncategory == 10)
      {
        this.IssuingBoxCodeDetailReport();
      }
      else if (ncategory == 11)
      {
        this.AdjustmentOutBoxCodeReport();
      }
      else if (ncategory == 12)
      {
        this.AdjustmentOutBoxCodeDetailReport();
      }
      else if (ncategory == 13)
      {
        this.IssueContainerBoxCodeReport();
      }
      else if (ncategory == 14)
      {
        this.IssueContainerBoxCodeDetailReport();
      }

    }

    #endregion Init

    #region LoadReport

    private void IssueContainerBoxCodeDetailReport()
    {
      DBParameter[] arrGetReport = new DBParameter[1];
      arrGetReport[0] = new DBParameter("@SPICode", DbType.AnsiString, 24, this.outStoreCode);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHFRPTFinishGoodsSoldToCustomerSpecialBoxCodeDetail", arrGetReport);

      FinishGoodWarehouse.DataSetFile.dsSoldToCustomerSpecialBoxCodeDetail dsSource = new FinishGoodWarehouse.DataSetFile.dsSoldToCustomerSpecialBoxCodeDetail();

      dsSource.Tables["dtSoldToCustomerSpecialBoxCodeDetail"].Merge(dtSource);

      SoldToCustomerSpecialBoxCodeDetail rptDoc = new SoldToCustomerSpecialBoxCodeDetail();
      rptDoc.SetDataSource(dsSource);

      string commandText = string.Empty;

      commandText += " SELECT Department, OutStoreCode, CONVERT(VARCHAR, DateOutStore, 103) DateOutStore, Note ";
      commandText += " FROM TblWHFOutStore ";
      commandText += " WHERE OutStoreCode  = '" + this.outStoreCode + "'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        if (dt.Rows[0]["Department"].ToString().Length > 0)
        {
          rptDoc.SetParameterValue("Department", "Department Requested: " + dt.Rows[0]["Department"].ToString());
        }
        else
        {
          rptDoc.SetParameterValue("Department", "Department Requested: " + " ");
        }
        rptDoc.SetParameterValue("TrNo", "TR#: " + dt.Rows[0]["OutStoreCode"].ToString());
        rptDoc.SetParameterValue("Date", "Date: " + dt.Rows[0]["DateOutStore"].ToString());
        rptDoc.SetParameterValue("Title", dt.Rows[0]["Note"].ToString());
      }
      int totalQtyItems = 0;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        totalQtyItems += DBConvert.ParseInt(dtSource.Rows[i]["QtyItems"].ToString());
      }
      rptDoc.SetParameterValue("TotalQtyItems", totalQtyItems);

      cptItemMaterialViewer.ReportSource = rptDoc;
    }

    private void IssueContainerBoxCodeReport()
    {
      DBParameter[] arrGetReport = new DBParameter[1];
      arrGetReport[0] = new DBParameter("@SPICode", DbType.AnsiString, 24, this.outStoreCode);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHFRPTFinishGoodsSoldToCustomerSpecialBoxCode", arrGetReport);

      FinishGoodWarehouse.DataSetFile.dsSoldToCustomerSpecialBoxCode dsSource = new FinishGoodWarehouse.DataSetFile.dsSoldToCustomerSpecialBoxCode();

      dsSource.Tables["dtSoldToCustomerSpecialBoxCode"].Merge(dtSource);

      SoldToCustomerSpecialBoxCode rptDoc = new SoldToCustomerSpecialBoxCode();
      rptDoc.SetDataSource(dsSource);

      string commandText = string.Empty;

      commandText += " SELECT Department, OutStoreCode, CONVERT(VARCHAR, DateOutStore, 103) DateOutStore, Note ";
      commandText += " FROM TblWHFOutStore ";
      commandText += " WHERE OutStoreCode  = '" + this.outStoreCode + "'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        if (dt.Rows[0]["Department"].ToString().Length > 0)
        {
          rptDoc.SetParameterValue("Department", "Department Requested: " + dt.Rows[0]["Department"].ToString());
        }
        else
        {
          rptDoc.SetParameterValue("Department", "Department Requested: " + " ");
        }

        rptDoc.SetParameterValue("TrNo", "TR#: " + dt.Rows[0]["OutStoreCode"].ToString());
        rptDoc.SetParameterValue("Date", "Date: " + dt.Rows[0]["DateOutStore"].ToString());
        rptDoc.SetParameterValue("Title", dt.Rows[0]["Note"].ToString());
      }
      int totalCTNs = 0;
      int totalQtyItems = 0;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        totalCTNs += DBConvert.ParseInt(dtSource.Rows[i]["CTNS"].ToString());
        totalQtyItems += DBConvert.ParseInt(dtSource.Rows[i]["QtyItems"].ToString());
      }
      rptDoc.SetParameterValue("TotalCTNs", totalCTNs);
      rptDoc.SetParameterValue("TotalQtyItems", totalQtyItems);

      cptItemMaterialViewer.ReportSource = rptDoc;
    }

    private void AdjustmentOutBoxCodeDetailReport()
    {
      DBParameter[] arrGetReport = new DBParameter[1];
      arrGetReport[0] = new DBParameter("@ADOPid", DbType.Int64, this.outStorePid);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHFRPTFinishGoodsAdjustmentOutBoxCodeDetail", arrGetReport);

      FinishGoodWarehouse.DataSetFile.dsAdjustmentOutBoxCodeDetail dsSource = new FinishGoodWarehouse.DataSetFile.dsAdjustmentOutBoxCodeDetail();

      dsSource.Tables["dtAdjustmentOutBoxCodeDetail"].Merge(dtSource);

      AdjustmentOutBoxCodeDetail rpt = new AdjustmentOutBoxCodeDetail();
      rpt.SetDataSource(dsSource);

      string commandText = string.Empty;

      commandText += " SELECT Department, OutStoreCode, CONVERT(VARCHAR, DateOutStore, 103) DateOutStore, Note ";
      commandText += " FROM TblWHFOutStore ";
      commandText += " WHERE PID  = " + this.outStorePid;
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        if (dt.Rows[0]["Department"].ToString().Length > 0)
        {
          rpt.SetParameterValue("Department", "Department Requested: " + dt.Rows[0]["Department"].ToString());
        }
        else
        {
          rpt.SetParameterValue("Department", "Department Requested: " + " ");
        }
        rpt.SetParameterValue("TrNo", "TR#: " + dt.Rows[0]["OutStoreCode"].ToString());
        rpt.SetParameterValue("Date", "Date: " + dt.Rows[0]["DateOutStore"].ToString());
        rpt.SetParameterValue("Title", dt.Rows[0]["Note"].ToString());
      }
      int totalQtyItems = 0;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        totalQtyItems += DBConvert.ParseInt(dtSource.Rows[i]["QtyItems"].ToString());
      }
      rpt.SetParameterValue("TotalQtyItems", totalQtyItems);

      cptItemMaterialViewer.ReportSource = rpt;
    }

    private void AdjustmentOutBoxCodeReport()
    {
      DBParameter[] arrGetReport = new DBParameter[1];
      arrGetReport[0] = new DBParameter("@ADOPid", DbType.Int64, this.outStorePid);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHFRPTFinishGoodsAdjustmentOutBoxCode", arrGetReport);

      FinishGoodWarehouse.DataSetFile.dsAdjustmentOutBoxCode dsSource = new FinishGoodWarehouse.DataSetFile.dsAdjustmentOutBoxCode();

      dsSource.Tables["dtAdjustmentOutBoxCode"].Merge(dtSource);

      AdjustmentOutBoxCode rpt = new AdjustmentOutBoxCode();
      rpt.SetDataSource(dsSource);

      string commandText = string.Empty;

      commandText += " SELECT Department, OutStoreCode, CONVERT(VARCHAR, DateOutStore, 103) DateOutStore, Note ";
      commandText += " FROM TblWHFOutStore ";
      commandText += " WHERE PID  = " + this.outStorePid;
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        if (dt.Rows[0]["Department"].ToString().Length > 0)
        {
          rpt.SetParameterValue("Department", "Department Requested: " + dt.Rows[0]["Department"].ToString());
        }
        else
        {
          rpt.SetParameterValue("Department", "Department Requested: " + " ");
        }
        rpt.SetParameterValue("TrNo", "TR#: " + dt.Rows[0]["OutStoreCode"].ToString());
        rpt.SetParameterValue("Date", "Date: " + dt.Rows[0]["DateOutStore"].ToString());
        rpt.SetParameterValue("Title", dt.Rows[0]["Note"].ToString());
      }
      int totalCTNs = 0;
      int totalQtyItems = 0;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        totalCTNs += DBConvert.ParseInt(dtSource.Rows[i]["CTNS"].ToString());
        totalQtyItems += DBConvert.ParseInt(dtSource.Rows[i]["QtyItems"].ToString());
      }
      rpt.SetParameterValue("TotalCTNs", totalCTNs);
      rpt.SetParameterValue("TotalQtyItems", totalQtyItems);

      cptItemMaterialViewer.ReportSource = rpt;
    }

    private void IssuingBoxCodeReport()
    {
      DBParameter[] arrGetReport = new DBParameter[1];
      arrGetReport[0] = new DBParameter("@IssCode", DbType.String, this.outStoreCode);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHFRPTFinishGoodsIssueBoxCode", arrGetReport);

      FinishGoodWarehouse.DataSetFile.dsIssueBoxCode dsSource = new FinishGoodWarehouse.DataSetFile.dsIssueBoxCode();

      dsSource.Tables["dtIssueBoxCode"].Merge(dtSource);

      IssueBoxCode rpt = new IssueBoxCode();
      rpt.SetDataSource(dsSource);

      string commandText = string.Empty;

      commandText += " SELECT Department, OutStoreCode, CONVERT(VARCHAR, DateOutStore, 103) DateOutStore, Note ";
      commandText += " FROM TblWHFOutStore ";
      commandText += " WHERE OutStoreCode  = '" + this.outStoreCode + "'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        rpt.SetParameterValue("Department", "Department Requested: " + dt.Rows[0]["Department"].ToString());
        rpt.SetParameterValue("TrNo", "TR#: " + dt.Rows[0]["OutStoreCode"].ToString());
        rpt.SetParameterValue("Date", "Date: " + dt.Rows[0]["DateOutStore"].ToString());
        rpt.SetParameterValue("Title", dt.Rows[0]["Note"].ToString());
      }
      int ctns = 0;
      int item = 0;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        ctns += DBConvert.ParseInt(dtSource.Rows[i]["CTNS"].ToString());
        item += DBConvert.ParseInt(dtSource.Rows[i]["QtyItems"].ToString());
      }
      rpt.SetParameterValue("CTNs", ctns);
      rpt.SetParameterValue("Item", item);

      cptItemMaterialViewer.ReportSource = rpt;
    }

    private void IssuingBoxCodeDetailReport()
    {
      DBParameter[] arrGetReport = new DBParameter[1];
      arrGetReport[0] = new DBParameter("@IssCode", DbType.String, this.outStoreCode);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHFRPTFinishGoodsIssueDetail", arrGetReport);

      FinishGoodWarehouse.DataSetFile.dsIssueBoxCodeDetail dsSource = new FinishGoodWarehouse.DataSetFile.dsIssueBoxCodeDetail();

      dsSource.Tables["dtIssueBoxCodeDetail"].Merge(dtSource);

      IssueBoxCodeDetail rpt = new IssueBoxCodeDetail();
      rpt.SetDataSource(dsSource);

      string commandText = string.Empty;

      commandText += " SELECT Department, OutStoreCode, CONVERT(VARCHAR, DateOutStore, 103) DateOutStore, Note ";
      commandText += " FROM TblWHFOutStore ";
      commandText += " WHERE OutStoreCode  = '" + this.outStoreCode + "'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        rpt.SetParameterValue("Department", "Department Requested: " + dt.Rows[0]["Department"].ToString());
        rpt.SetParameterValue("TrNo", "TR#: " + dt.Rows[0]["OutStoreCode"].ToString());
        rpt.SetParameterValue("Date", "Date: " + dt.Rows[0]["DateOutStore"].ToString());
        rpt.SetParameterValue("Title", dt.Rows[0]["Note"].ToString());
      }
      int totalQty = 0;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        totalQty += DBConvert.ParseInt(dtSource.Rows[i]["QuantityItem"].ToString());
      }
      rpt.SetParameterValue("TotalQty", totalQty);
      cptItemMaterialViewer.ReportSource = rpt;
    }

    private void TransferLocationBoxCodeDetailReport()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@TranNoPid", DbType.Int64, this.locationPid);

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHFRPTFinishGoodsTransferLocationDetail", inputParam);
      Shared.DataSetSource.FinishGoodWarehouse.dsFGHTransferLocationDetail dsSource = new DaiCo.Shared.DataSetSource.FinishGoodWarehouse.dsFGHTransferLocationDetail();

      dsSource.Tables["TblParent"].Merge(ds.Tables[0]);
      dsSource.Tables["TblChild"].Merge(ds.Tables[1]);

      cptWHFTransferLocationDetail rpt = new cptWHFTransferLocationDetail();
      int qtyItems = 0;
      foreach (DataRow row in ds.Tables[1].Rows)
      {
        qtyItems += DBConvert.ParseInt(row["QtyItems"].ToString());
      }

      rpt.SetDataSource(dsSource);
      rpt.SetParameterValue("QtyItems", qtyItems);
      cptItemMaterialViewer.ReportSource = rpt;
    }

    private void TransferLocationBoxCodeReport()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@TranNoPid", DbType.Int64, this.locationPid);

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHFRPTFinishGoodsTransferLocationBoxCode", inputParam);
      Shared.DataSetSource.FinishGoodWarehouse.dsFGHTransferLocationBoxCode dsSource = new DaiCo.Shared.DataSetSource.FinishGoodWarehouse.dsFGHTransferLocationBoxCode();

      dsSource.Tables["TblParent"].Merge(ds.Tables[0]);
      dsSource.Tables["TblChild"].Merge(ds.Tables[1]);

      cptWHFTransferLocationBoxCode rpt = new cptWHFTransferLocationBoxCode();
      int ctns = 0;
      int qtyItems = 0;
      foreach (DataRow row in ds.Tables[1].Rows)
      {
        ctns += DBConvert.ParseInt(row["CTNS"].ToString());
        qtyItems += DBConvert.ParseInt(row["QtyItems"].ToString());
      }

      rpt.SetDataSource(dsSource);
      rpt.SetParameterValue("CTNs", ctns);
      rpt.SetParameterValue("QtyItems", qtyItems);
      cptItemMaterialViewer.ReportSource = rpt;
    }

    private void ReceivingBoxCodeReport()
    {
      DBParameter[] arrGetReport = new DBParameter[1];
      arrGetReport[0] = new DBParameter("@RTWCode", DbType.String, this.inStoreCode);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHFRPTFinishGoodsReceivingBoxCode", arrGetReport);

      FinishGoodWarehouse.DataSetFile.dsReceivingBoxCode dsSource = new FinishGoodWarehouse.DataSetFile.dsReceivingBoxCode();

      dsSource.Tables["dtReceivingBoxCode"].Merge(dtSource);

      ReceivingBoxCode rpt = new ReceivingBoxCode();
      rpt.SetDataSource(dsSource);
      string commandText = string.Empty;

      commandText += " SELECT Department, InStoreCode, CONVERT(VARCHAR, DateInStore, 103) DateInStore, Note ";
      commandText += " FROM TblWHFInStore ";
      commandText += " WHERE InStoreCode  = '" + this.inStoreCode + "'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        rpt.SetParameterValue("Department", "Department Requested: " + dt.Rows[0]["Department"].ToString());
        rpt.SetParameterValue("TrNo", "TR#: " + dt.Rows[0]["InStoreCode"].ToString());
        rpt.SetParameterValue("Date", "Date: " + dt.Rows[0]["DateInStore"].ToString());
        rpt.SetParameterValue("Title", dt.Rows[0]["Note"].ToString());
      }
      int totalCTNs = 0;
      int totalQtyItems = 0;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        totalCTNs += DBConvert.ParseInt(dtSource.Rows[i]["CTNS"].ToString());
        totalQtyItems += DBConvert.ParseInt(dtSource.Rows[i]["QtyItems"].ToString());
      }
      rpt.SetParameterValue("TotalCTNs", totalCTNs);
      rpt.SetParameterValue("TotalQtyItems", totalQtyItems);

      cptItemMaterialViewer.ReportSource = rpt;
    }

    private void ReceivingBoxCodeDetailReport()
    {
      DBParameter[] arrGetReport = new DBParameter[1];
      arrGetReport[0] = new DBParameter("@RTWCode", DbType.String, this.inStoreCode);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHFRPTFinishGoodsReceivingBoxCodeDetail", arrGetReport);

      FinishGoodWarehouse.DataSetFile.dsReceivingBoxCodeDetail dsSource = new FinishGoodWarehouse.DataSetFile.dsReceivingBoxCodeDetail();

      dsSource.Tables["dtReceivingBoxCodeDetail"].Merge(dtSource);

      ReceivingBoxCodeDetail rpt = new ReceivingBoxCodeDetail();
      rpt.SetDataSource(dsSource);

      string commandText = string.Empty;

      commandText += " SELECT Department, InStoreCode, CONVERT(VARCHAR, DateInStore, 103) DateInStore, Note ";
      commandText += " FROM TblWHFInStore ";
      commandText += " WHERE InStoreCode  = '" + this.inStoreCode + "'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        rpt.SetParameterValue("Department", "Department Requested: " + dt.Rows[0]["Department"].ToString());
        rpt.SetParameterValue("TrNo", "TR#: " + dt.Rows[0]["InStoreCode"].ToString());
        rpt.SetParameterValue("Date", "Date: " + dt.Rows[0]["DateInStore"].ToString());
        rpt.SetParameterValue("Title", dt.Rows[0]["Note"].ToString());
      }
      int totalQtyItems = 0;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        totalQtyItems += DBConvert.ParseInt(dtSource.Rows[i]["QtyItems"].ToString());
      }
      rpt.SetParameterValue("TotalQtyItems", totalQtyItems);

      cptItemMaterialViewer.ReportSource = rpt;
    }

    private void AdjusmentInBoxCodeReport()
    {
      DBParameter[] arrGetReport = new DBParameter[1];
      arrGetReport[0] = new DBParameter("@ADIPid", DbType.Int64, this.inStorePid);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHFRPTFinishGoodsAdjustmentInBoxCode", arrGetReport);

      FinishGoodWarehouse.DataSetFile.dsAdjustmentInBoxCode dsSource = new FinishGoodWarehouse.DataSetFile.dsAdjustmentInBoxCode();

      dsSource.Tables["dtAdjustmentInBoxCode"].Merge(dtSource);

      AdjustmentInBoxCode rpt = new AdjustmentInBoxCode();
      rpt.SetDataSource(dsSource);

      string commandText = string.Empty;

      commandText += " SELECT Department, InStoreCode, CONVERT(VARCHAR, DateInStore, 103) DateInStore, Note ";
      commandText += " FROM TblWHFInStore ";
      commandText += " WHERE PID  = " + this.inStorePid;
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        if (dt.Rows[0]["Department"].ToString().Length > 0)
        {
          rpt.SetParameterValue("Department", "Department Requested: " + dt.Rows[0]["Department"].ToString());
        }
        else
        {
          rpt.SetParameterValue("Department", "Department Requested: " + " ");
        }
        rpt.SetParameterValue("TrNo", "TR#: " + dt.Rows[0]["InStoreCode"].ToString());
        rpt.SetParameterValue("Date", "Date: " + dt.Rows[0]["DateInStore"].ToString());
        rpt.SetParameterValue("Title", dt.Rows[0]["Note"].ToString());
      }
      int totalCTNs = 0;
      int totalQtyItems = 0;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        totalCTNs += DBConvert.ParseInt(dtSource.Rows[i]["CTNS"].ToString());
        totalQtyItems += DBConvert.ParseInt(dtSource.Rows[i]["QtyItems"].ToString());
      }
      rpt.SetParameterValue("TotalCTNs", totalCTNs);
      rpt.SetParameterValue("TotalQtyItems", totalQtyItems);

      cptItemMaterialViewer.ReportSource = rpt;
    }

    private void AdjusmentInBoxCodeDetailReport()
    {
      DBParameter[] arrGetReport = new DBParameter[1];
      arrGetReport[0] = new DBParameter("@ADIPid", DbType.Int64, this.inStorePid);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHFRPTFinishGoodsAdjustmentInBoxCodeDetail", arrGetReport);

      FinishGoodWarehouse.DataSetFile.dsAdjustmentInBoxCodeDetail dsSource = new FinishGoodWarehouse.DataSetFile.dsAdjustmentInBoxCodeDetail();

      dsSource.Tables["dtAdjustmentInBoxCodeDetail"].Merge(dtSource);

      AdjustmentInBoxCodeDetail rpt = new AdjustmentInBoxCodeDetail();
      rpt.SetDataSource(dsSource);

      string commandText = string.Empty;

      commandText += " SELECT Department, InStoreCode, CONVERT(VARCHAR, DateInStore, 103) DateInStore, Note ";
      commandText += " FROM TblWHFInStore ";
      commandText += " WHERE PID  = " + this.inStorePid;
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        if (dt.Rows[0]["Department"].ToString().Length > 0)
        {
          rpt.SetParameterValue("Department", "Department Requested: " + dt.Rows[0]["Department"].ToString());
        }
        else
        {
          rpt.SetParameterValue("Department", "Department Requested: " + " ");
        }
        rpt.SetParameterValue("TrNo", "TR#: " + dt.Rows[0]["InStoreCode"].ToString());
        rpt.SetParameterValue("Date", "Date: " + dt.Rows[0]["DateInStore"].ToString());
        rpt.SetParameterValue("Title", dt.Rows[0]["Note"].ToString());
      }
      int totalQtyItems = 0;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        totalQtyItems += DBConvert.ParseInt(dtSource.Rows[i]["QtyItems"].ToString());
      }
      rpt.SetParameterValue("TotalQtyItems", totalQtyItems);

      cptItemMaterialViewer.ReportSource = rpt;
    }

    private void ReturnFromCustomerBoxCodeReport()
    {
      DBParameter[] arrGetReport = new DBParameter[1];
      arrGetReport[0] = new DBParameter("@RFCCode", DbType.AnsiString, 24, this.inStoreCode);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHFRPTFinishGoodsReturnedFromCustomerBoxCode", arrGetReport);

      FinishGoodWarehouse.DataSetFile.dsReturnedFromCustomerBoxCode dsSource = new FinishGoodWarehouse.DataSetFile.dsReturnedFromCustomerBoxCode();

      dsSource.Tables["dtReturnedFromCustomerBoxCode"].Merge(dtSource);

      ReturnedFromCustomerBoxCode rpt = new ReturnedFromCustomerBoxCode();
      rpt.SetDataSource(dsSource);

      string commandText = string.Empty;

      commandText += " SELECT Department, InStoreCode, CONVERT(VARCHAR, DateInStore, 103) DateInStore, Note ";
      commandText += " FROM TblWHFInStore ";
      commandText += " WHERE InStoreCode  = '" + this.inStoreCode + "'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        if (dt.Rows[0]["Department"].ToString().Length > 0)
        {
          rpt.SetParameterValue("Department", "Department Requested: " + dt.Rows[0]["Department"].ToString());
        }
        else
        {
          rpt.SetParameterValue("Department", "Department Requested: " + " ");
        }
        rpt.SetParameterValue("TrNo", "TR#: " + dt.Rows[0]["InStoreCode"].ToString());
        rpt.SetParameterValue("Date", "Date: " + dt.Rows[0]["DateInStore"].ToString());
        rpt.SetParameterValue("Title", dt.Rows[0]["Note"].ToString());
      }
      int totalCTNs = 0;
      int totalQtyItems = 0;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        totalCTNs += DBConvert.ParseInt(dtSource.Rows[i]["CTNS"].ToString());
        totalQtyItems += DBConvert.ParseInt(dtSource.Rows[i]["QtyItems"].ToString());
      }
      rpt.SetParameterValue("TotalCTNs", totalCTNs);
      rpt.SetParameterValue("TotalQtyItems", totalQtyItems);

      cptItemMaterialViewer.ReportSource = rpt;
    }

    private void ReturnFromCustomerBoxCodeDetailReport()
    {
      DBParameter[] arrGetReport = new DBParameter[1];
      arrGetReport[0] = new DBParameter("@RFCCode", DbType.AnsiString, 24, this.inStoreCode);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHFRPTFinishGoodsReturnedFromCustomerBoxCodeDetail", arrGetReport);

      FinishGoodWarehouse.DataSetFile.dsReturnedFromCustomerBoxCodeDetail dsSource = new FinishGoodWarehouse.DataSetFile.dsReturnedFromCustomerBoxCodeDetail();

      dsSource.Tables["dtReturnedFromCustomerBoxCodeDetail"].Merge(dtSource);

      ReturnedFromCustomerBoxCodeDetail rpt = new ReturnedFromCustomerBoxCodeDetail();
      rpt.SetDataSource(dsSource);

      string commandText = string.Empty;

      commandText += " SELECT Department, InStoreCode, CONVERT(VARCHAR, DateInStore, 103) DateInStore, Note ";
      commandText += " FROM TblWHFInStore ";
      commandText += " WHERE InStoreCode  = '" + this.inStoreCode + "'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        if (dt.Rows[0]["Department"].ToString().Length > 0)
        {
          rpt.SetParameterValue("Department", "Department Requested: " + dt.Rows[0]["Department"].ToString());
        }
        else
        {
          rpt.SetParameterValue("Department", "Department Requested: " + " ");
        }
        rpt.SetParameterValue("TrNo", "TR#: " + dt.Rows[0]["InStoreCode"].ToString());
        rpt.SetParameterValue("Date", "Date: " + dt.Rows[0]["DateInStore"].ToString());
        rpt.SetParameterValue("Title", dt.Rows[0]["Note"].ToString());
      }
      int totalQtyItems = 0;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        totalQtyItems += DBConvert.ParseInt(dtSource.Rows[i]["QtyItems"].ToString());
      }
      rpt.SetParameterValue("TotalQtyItems", totalQtyItems);
      cptItemMaterialViewer.ReportSource = rpt;
    }

    #endregion LoadReport

    #region more function

    #endregion more function
  }
}