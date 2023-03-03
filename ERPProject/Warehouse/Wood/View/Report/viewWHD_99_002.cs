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
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.IO;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_99_002 : MainUserControl
  {
    #region Field
    public int ncategory = int.MinValue;
    public long issuingNotePid = long.MinValue;
    public long receivingPid = long.MinValue;
    #endregion Field

    #region Init

    public viewWHD_99_002()
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
      foreach (DataRow row in dsSource.Tables["TblWHDWoodsIssuingNoteDetail"].Rows)
      {
        qty += DBConvert.ParseDouble(row["Qty"].ToString());
      }

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

      rpt.SetParameterValue("Title", title);
      rpt.SetParameterValue("TrNo", trNo);
      rpt.SetParameterValue("Date", date);
      rpt.SetParameterValue("Source", source);
      rpt.SetParameterValue("Total", qty);
      rpt.SetParameterValue("CreateBy", createBy);
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
      foreach (DataRow row in dsSource.Tables["TblWHDWoodsIssuingNoteDetail"].Rows)
      {
        qty += DBConvert.ParseDouble(row["Qty"].ToString());
      }

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

      string path = "F:\\Reports\\1.pdf";
      string pathNew = "F:\\Reports\\1_encrypted.pdf";
      rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path);
      using (Stream input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
      using (Stream output = new FileStream(pathNew, FileMode.Create, FileAccess.Write, FileShare.None))
      {
        PdfReader reader = new PdfReader(input);
        PdfEncryptor.Encrypt(reader, output, iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, null, "it1daico", PdfWriter.ALLOW_PRINTING);
      }
    }
    #endregion LoadReport

    #region Function
    #endregion Function
  }
}