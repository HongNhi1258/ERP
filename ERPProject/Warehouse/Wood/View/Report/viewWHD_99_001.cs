/*
 * Author       : Duong Minh
 * CreateDate   : 14/06/2012
 * Description  : Report 
 */
using DaiCo.Application;
using DaiCo.ERPProject.DataSetSource.Woods;
using DaiCo.ERPProject.Reports.Woods;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using System;
using System.Data;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_99_001 : MainUserControl
  {
    #region Field
    public int ncategory = int.MinValue;
    public long issuingNotePid = long.MinValue;
    public long receivingPid = long.MinValue;
    #endregion Field

    #region Init

    public viewWHD_99_001()
    {
      InitializeComponent();
    }

    private void ViewVEN_99_001_Load(object sender, EventArgs e)
    {
      if (ncategory == 1)
      {
        this.IssuingReport();
      }
      else if (ncategory == 2)
      {
        this.ReceivingReport();
      }
      else if (ncategory == 3)
      {
        this.IssuingRequestOnlineReport();
      }
    }
    #endregion Init

    #region LoadReport
    private void ReceivingReport()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@RecivingNotePid", DbType.Int64, this.receivingPid);

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHDRPTReceivingNoteWoods_Select", inputParam);

      dsWHDWoodsIssuingNote dsSource = new dsWHDWoodsIssuingNote();
      dsSource.Tables["TblWHDWoodsIssuingNoteDetail"].Merge(ds.Tables[1]);

      double qty = 0;
      double pcs = 0;
      double qtyexi = 0;
      foreach (DataRow row in dsSource.Tables["TblWHDWoodsIssuingNoteDetail"].Rows)
      {
        qty += DBConvert.ParseDouble(row["Qty"].ToString());
        pcs += DBConvert.ParseDouble(row["Pcs"].ToString());
        qtyexi += DBConvert.ParseDouble(row["QtyEXI"].ToString());
      }
      qty = Math.Round(qty, 4);
      qtyexi = Math.Round(qtyexi, 4);
      cptWHDReceivingNoteWoods rpt = new cptWHDReceivingNoteWoods();
      rpt.SetDataSource(dsSource);

      string source = string.Empty;
      int type = DBConvert.ParseInt(ds.Tables[0].Rows[0]["Type"].ToString());
      if (type == 1)
      {
        source = "Supplier :" + ds.Tables[0].Rows[0]["Source"].ToString();
      }
      else if (type == 2)
      {
        source = "Department Requested :" + ds.Tables[0].Rows[0]["Source"].ToString();
      }
      else if (type == 3)
      {
        source = "";
      }

      string title = ds.Tables[0].Rows[0]["Title"].ToString();
      string trNo = "Tr#no: " + ds.Tables[0].Rows[0]["ReceivingCode"].ToString();
      string date = "Date: " + ds.Tables[0].Rows[0]["CreateDate"].ToString();
      string createBy = ds.Tables[0].Rows[0]["CreateBy"].ToString();
      string invoiceNo = ds.Tables[0].Rows[0]["InvoiceNo"].ToString();
      string customNo = ds.Tables[0].Rows[0]["CustomNo"].ToString();
      string customDate = "Custom Date: " + ds.Tables[0].Rows[0]["CustomDate"].ToString();

      rpt.SetParameterValue("Title", title);
      rpt.SetParameterValue("TrNo", trNo);
      rpt.SetParameterValue("Date", date);
      rpt.SetParameterValue("Source", source);
      rpt.SetParameterValue("Total", qty);
      rpt.SetParameterValue("TotalEXI", qtyexi);
      rpt.SetParameterValue("CreateBy", createBy);
      rpt.SetParameterValue("TotalPcs", pcs);
      rpt.SetParameterValue("InvoiceNo", invoiceNo);
      rpt.SetParameterValue("CustomNo", customNo);
      rpt.SetParameterValue("CustomDate", customDate);
      cptItemMaterialViewer.ReportSource = rpt;
    }

    private void IssuingReport()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@IssuingNotePid", DbType.Int64, this.issuingNotePid);

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHDRPTIssuingNoteWoods_Select", inputParam);

      dsWHDWoodsIssuingNote dsSource = new dsWHDWoodsIssuingNote();
      dsSource.Tables["TblWHDWoodsIssuingNoteDetail"].Merge(ds.Tables[1]);

      double qty = 0;
      double pcs = 0;
      double qtyexi = 0;
      foreach (DataRow row in dsSource.Tables["TblWHDWoodsIssuingNoteDetail"].Rows)
      {
        qty += DBConvert.ParseDouble(row["Qty"].ToString());
        pcs += DBConvert.ParseDouble(row["Pcs"].ToString());
        qtyexi += DBConvert.ParseDouble(row["QtyEXI"].ToString());
      }

      qty = Math.Round(qty, 4);
      qtyexi = Math.Round(qtyexi, 4);
      cptWHDIssuingNoteWoods rpt = new cptWHDIssuingNoteWoods();
      rpt.SetDataSource(dsSource);

      string remark = "WOD: " + ds.Tables[0].Rows[0]["Remark"].ToString();
      string source = string.Empty;
      int type = DBConvert.ParseInt(ds.Tables[0].Rows[0]["Type"].ToString());
      if (type == 1)
      {
        source = "Department Requested :" + ds.Tables[0].Rows[0]["Source"].ToString();
      }
      else if (type == 2)
      {
        source = "Supplier :" + ds.Tables[0].Rows[0]["Source"].ToString();
      }
      else if (type == 3)
      {
        source = "";
      }
      else if (type == 4)
      {
        source = "Supplier :" + ds.Tables[0].Rows[0]["Source"].ToString();
      }

      string title = ds.Tables[0].Rows[0]["Title"].ToString();
      string trNo = "Tr#no: " + ds.Tables[0].Rows[0]["IssuingCode"].ToString();
      string date = "Date: " + ds.Tables[0].Rows[0]["CreateDate"].ToString();

      rpt.SetParameterValue("WOD", remark);
      rpt.SetParameterValue("Title", title);
      rpt.SetParameterValue("TrNo", trNo);
      rpt.SetParameterValue("Date", date);
      rpt.SetParameterValue("Source", source);
      rpt.SetParameterValue("Total", qty);
      rpt.SetParameterValue("TotalEXI", qtyexi);
      rpt.SetParameterValue("TotalPcs", pcs);
      cptItemMaterialViewer.ReportSource = rpt;
    }


    private void IssuingRequestOnlineReport()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@IssuingPid", DbType.Int64, this.issuingNotePid);

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHDRPTWoodsIssuingRequestOnline_Select", inputParam);
      if (ds.Tables.Count > 0)
      {
        dsWHDIssuingRequestOnline dsSource = new dsWHDIssuingRequestOnline();
        dsSource.Tables["dtInfo"].Merge(ds.Tables[0]);
        dsSource.Tables["dtRequest"].Merge(ds.Tables[1]);
        dsSource.Tables["dtDetail"].Merge(ds.Tables[2]);

        double totalQty = 0;
        double totalQtyEXI = 0;
        double totalPcs = 0;
        for (int i = 0; i < dsSource.Tables["dtDetail"].Rows.Count; i++)
        {
          DataRow row = dsSource.Tables["dtDetail"].Rows[i];
          if (DBConvert.ParseDouble(row["Qty"].ToString()) != double.MinValue)
          {
            totalQty = totalQty + DBConvert.ParseDouble(row["Qty"].ToString());
          }
          if (DBConvert.ParseDouble(row["Pcs"].ToString()) != double.MinValue)
          {
            totalPcs = totalPcs + DBConvert.ParseDouble(row["Pcs"].ToString());
          }
          if (DBConvert.ParseDouble(row["QtyEXI"].ToString()) != double.MinValue)
          {
            totalQtyEXI = totalQtyEXI + DBConvert.ParseDouble(row["QtyEXI"].ToString());
          }
        }

        double totalRequired = 0;
        for (int i = 0; i < dsSource.Tables["dtRequest"].Rows.Count; i++)
        {
          DataRow row = dsSource.Tables["dtRequest"].Rows[i];
          if (DBConvert.ParseDouble(row["QtyRequest"].ToString()) != double.MinValue)
          {
            totalRequired = totalRequired + DBConvert.ParseDouble(row["QtyRequest"].ToString());
          }
        }

        cptWHDIssuingRequestOnline rpt = new cptWHDIssuingRequestOnline();
        rpt.SetDataSource(dsSource);
        rpt.SetParameterValue("totalQty", totalQty);
        rpt.SetParameterValue("totalPcs", totalPcs);
        rpt.SetParameterValue("totalRequired", totalRequired);
        rpt.SetParameterValue("totalQtyEXI", totalQtyEXI);
        cptItemMaterialViewer.ReportSource = rpt;
      }
    }
    #endregion LoadReport

    #region Function
    #endregion Function
  }
}