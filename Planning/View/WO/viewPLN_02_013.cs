using DaiCo.Application;
using DaiCo.Planning.Reports;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.Planning;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_02_013 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewPLN_02_013()
    {
      InitializeComponent();
    }

    private void viewPLN_02_013_Load(object sender, EventArgs e)
    {
      this.ultDateOfWeek.Value = DateTime.Today;
      this.LoadKindReport();
    }
    #endregion Init

    #region Function
    private void LoadKindReport()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Text", typeof(string));
      dt.Columns.Add("Value", typeof(Int32));

      DataRow row = dt.NewRow();
      row["Text"] = "Product Weekly Report";
      row["Value"] = 1;
      dt.Rows.InsertAt(row, 0);

      //Truong Add
      DataRow row1 = dt.NewRow();
      row1["Text"] = "Out put from foundry that has no wood components";
      row1["Value"] = 2;
      dt.Rows.InsertAt(row1, 1);
      // End

      ultCBKindReport.DataSource = dt;
      ultCBKindReport.DisplayLayout.AutoFitColumns = true;
      ultCBKindReport.DisplayMember = "Text";
      ultCBKindReport.ValueMember = "Value";
      ultCBKindReport.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
      ultCBKindReport.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadWorkArea()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 3004";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBArea.DataSource = dt;
      ultCBArea.DisplayLayout.AutoFitColumns = true;
      ultCBArea.DisplayMember = "Value";
      ultCBArea.ValueMember = "Code";
      ultCBArea.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultCBArea.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadWorkOder()
    {
      string commandText = "SELECT Pid WO FROM TblPLNWorkOrder";
      DataTable dtMain = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtMain != null)
      {
        ultcbWOFrom.DataSource = dtMain;
        ultcbWOFrom.DisplayMember = "WO";
        ultcbWOFrom.ValueMember = "WO";

        ultcbWOTo.DataSource = dtMain;
        ultcbWOTo.DisplayMember = "WO";
        ultcbWOTo.ValueMember = "WO";
      }
    }

    private void ReportWoodComponent()
    {
      string strTemplateName = "PLNWoodComponent";
      string strSheetName = "Sheet1";
      string strOutFileName = "Report List WO Not Wood Detail";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate\Planning";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      long woFrom = DBConvert.ParseLong(ultcbWOFrom.Value.ToString());
      long woTo = DBConvert.ParseLong(ultcbWOTo.Value.ToString());
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@WOFrom", DbType.Int64, woFrom);
      inputParam[1] = new DBParameter("@WOTo", DbType.Int64, woTo);

      DataTable dtMain = DataBaseAccess.SearchStoreProcedureDataTable("spPLNRPTListWorkOrderNotWoodDetail", inputParam);
      if (dtMain != null && dtMain.Rows.Count > 0)
      {
        oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
        for (int i = 0; i < dtMain.Rows.Count; i++)
        {
          DataRow dtRow = dtMain.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B7:J7").Copy();
            oXlsReport.RowInsert(6 + i);
            oXlsReport.Cell("B7:J7", 0, i).Paste();
          }
          oXlsReport.Cell("**WO", 0, i).Value = dtRow["WO"];
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"];
          oXlsReport.Cell("**Rev", 0, i).Value = dtRow["Revision"];
          oXlsReport.Cell("**OldCode", 0, i).Value = dtRow["OldCode"];
          oXlsReport.Cell("**SaleCode", 0, i).Value = dtRow["SaleCode"];
          oXlsReport.Cell("**ItemCBM", 0, i).Value = dtRow["CBM"];
          oXlsReport.Cell("**Qty", 0, i).Value = dtRow["Qty"];
          oXlsReport.Cell("**TotalCBM", 0, i).Value = dtRow["TotalCBM"];
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }
    #endregion Function

    #region Event
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      if (ultCBKindReport.Value != null)
      {
        if (DBConvert.ParseInt(ultCBKindReport.Value.ToString()) == 1)
        {
          if (ultCBArea.Value == null || ultDateOfWeek.Value == null)
          {
            return;
          }
          else
          {
            string storeName = "spPLNProductWeeklyReport_AREA";
            DBParameter[] inputParam = new DBParameter[2];
            inputParam[0] = new DBParameter("@Area", DbType.Int32, DBConvert.ParseInt(ultCBArea.Value.ToString()));
            inputParam[1] = new DBParameter("@Date", DbType.DateTime, (DateTime)ultDateOfWeek.Value);

            DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);

            if (dtSource != null && dtSource.Rows.Count > 0)
            {
              int sumQty = 0;
              double sumCBM = 0;
              dtSource.Columns.Add("Picture", typeof(System.Byte[]));
              foreach (DataRow row in dtSource.Rows)
              {
                try
                {
                  string imgPath = FunctionUtility.RDDGetItemImage(row["ItemCode"].ToString().Trim());
                  sumQty += DBConvert.ParseInt(row["QtyItems"].ToString());
                  sumCBM += DBConvert.ParseDouble(row["TotalCBM"].ToString());
                  FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
                  BinaryReader br = new BinaryReader(fs);
                  byte[] imgbyte = new byte[fs.Length + 1];
                  imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
                  row["Picture"] = imgbyte;
                  br.Close();
                  fs.Close();
                }
                catch { }
              }

              dsPLNProductWeeklyReport dsSource = new dsPLNProductWeeklyReport();
              dsSource.Tables[0].Merge(dtSource);

              DaiCo.Shared.View_Report report = null;
              cptPLNProductWeeklyReport cpt = new cptPLNProductWeeklyReport();

              cpt.SetDataSource(dsSource);
              cpt.SetParameterValue("Printer", SharedObject.UserInfo.EmpName);
              cpt.SetParameterValue("SumQty", sumQty);
              cpt.SetParameterValue("SumCBM", sumCBM);
              if (DBConvert.ParseInt(ultCBArea.Value.ToString()) == 1)
              {
                cpt.SetParameterValue("Tittle", "PAC out put in wk" + dtSource.Rows[0]["Week"].ToString());
              }
              if (DBConvert.ParseInt(ultCBArea.Value.ToString()) == 2)
              {
                cpt.SetParameterValue("Tittle", "ASS out put in wk" + dtSource.Rows[0]["Week"].ToString());
              }
              if (DBConvert.ParseInt(ultCBArea.Value.ToString()) == 3)
              {
                cpt.SetParameterValue("Tittle", "MCH out put in wk" + dtSource.Rows[0]["Week"].ToString());
              }

              report = new DaiCo.Shared.View_Report(cpt);
              report.IsShowGroupTree = false;
              report.ShowReport(Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
            }
          }
        }
        else
        {
          if (ultcbWOFrom.Value != null && ultcbWOTo.Value != null)
          {
            this.ReportWoodComponent();
          }
          else
          {
            WindowUtinity.ShowMessageError("ERR0115", "WorkOrder");
            return;
          }
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0115", "Report");
        return;
      }
    }

    private void ultCBKindReport_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBKindReport.Value != null)
      {
        if (DBConvert.ParseInt(ultCBKindReport.Value.ToString()) == 1)
        {
          tableLayoutWoodComponent.Visible = false;
          tableLayoutKindReport.Visible = true;
          if (ultCBKindReport.Value == null)
          {
            ultCBArea.Text = string.Empty;
            ultCBArea.DataSource = new DataTable();
          }
          else if (DBConvert.ParseInt(ultCBKindReport.Value.ToString()) == 1)
          {
            this.LoadWorkArea();
          }
        }
        else if (DBConvert.ParseInt(ultCBKindReport.Value.ToString()) == 2)
        {
          this.LoadWorkOder();
          tableLayoutWoodComponent.Visible = true;
          tableLayoutKindReport.Visible = false;
        }
      }
    }
    #endregion Event
  }
}
