/*
 * Author       : 
 * CreateDate   : 09/10/2012
 * Description  : Reports Total Oustanding Of PO
 */
using DaiCo.Application;
using DaiCo.Planning.DataSetFile;
using DaiCo.Planning.Reports;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_10_009 : MainUserControl
  {
    #region Init

    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;

    public viewPLN_10_009()
    {
      InitializeComponent();
    }

    private void viewPLN_10_009_Load(object sender, EventArgs e)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      this.pathTemplate = startupPath + @"\ExcelTemplate";
      this.pathExport = startupPath + @"\Report";

      // Load Report
      this.LoadReport();

      // Load ItemCode
      this.LoadItemCode();

      // Load Customer
      this.LoadCustomer();
    }

    #endregion Init

    #region Event
    private void btnExport_Click(object sender, EventArgs e)
    {
      this.TotalOutstadingOfPOsForTest();
      //this.TotalOutstadingOfPOs();
    }

    private void chkItemCode_CheckedChanged(object sender, EventArgs e)
    {
      ucItemCode.Visible = chkItemCode.Checked;
    }

    private void chkCustomer_CheckedChanged(object sender, EventArgs e)
    {
      ucCustomer.Visible = chkCustomer.Checked;
    }

    private void ucItemCode_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtItemCode.Text = this.ucItemCode.SelectedValue;
    }

    private void ucCustomer_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtCustomer.Text = this.ucCustomer.SelectedValue;
    }

    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtItemImport.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnGettemplate_Click(object sender, EventArgs e)
    {
      string templateName = "RPT_PLN_10_009";
      string sheetName = "Sheet1";
      string outFileName = "TemplateImportItemCode";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event

    #region Function
    /// <summary>
    /// Load Type Report
    /// </summary>
    private void LoadReport()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 ID, 'Total Oustanding Of POs' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultReport.DataSource = dtSource;
      ultReport.DisplayMember = "Name";
      ultReport.ValueMember = "ID";
      ultReport.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultReport.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      ultReport.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      //Default
      ultReport.Value = 1;
    }

    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadItemCode()
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT ItemCode ";
      commandText += " FROM TblBOMItemBasic ";
      commandText += " ORDER BY ItemCode ";

      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucItemCode.DataSource = dtItem;
      ucItemCode.ColumnWidths = "200";
      ucItemCode.DataBind();
      ucItemCode.ValueMember = "ItemCode";
    }

    /// <summary>
    /// Load Customer
    /// </summary>
    private void LoadCustomer()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, CustomerCode + '-' + Name Name ";
      commandText += " FROM TblCSDCustomerInfo CSD ";
      commandText += " ORDER BY CustomerCode ";

      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucCustomer.DataSource = dtItem;
      ucCustomer.ColumnWidths = "0;200";
      ucCustomer.DataBind();
      ucCustomer.ValueMember = "Pid";
      ucCustomer.DisplayMember = "Name";
      ucCustomer.AutoSearchBy = "Name";
    }

    /// <summary>
    /// Total Outstanding Of POs
    /// </summary>
    private void TotalOutstadingOfPOs()
    {
      // Check Report & Customer
      if (ultReport.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Report");
        return;
      }

      SqlDBParameter[] input = new SqlDBParameter[5];
      // ItemCode
      string text = string.Empty;
      DataTable dtImportExcel = this.CreateDataTable();
      if (txtItemCode.Text.Length > 0)
      {
        text = this.txtItemCode.Text;
      }
      else
      {
        if (txtItemImport.Text.Length > 0)
        {
          DataTable dtSource = new DataTable();
          // Import Excel
          try
          {
            dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtItemImport.Text.Trim(), "SELECT * FROM [Sheet1 (1)$C4:D1504]").Tables[0];
          }
          catch
          {
            dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtItemImport.Text.Trim(), "SELECT * FROM [Sheet1$C4:D1504]").Tables[0];
          }
          if (dtSource != null)
          {
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
              if (dtSource.Rows[i]["ItemCode"].ToString().Trim().Length > 0)
              {
                text = text + ";" + dtSource.Rows[i]["ItemCode"].ToString();

                DataRow row = dtImportExcel.NewRow();
                // STT
                if (DBConvert.ParseInt(dtSource.Rows[i]["STT"]) > 0)
                {
                  row["STT"] = DBConvert.ParseInt(dtSource.Rows[i]["STT"]);
                }
                else
                {
                  row["STT"] = 1000;
                }
                // ItemCode
                if (dtSource.Rows[i]["ItemCode"].ToString().Trim().Length > 0)
                {
                  row["ItemCode"] = dtSource.Rows[i]["ItemCode"];
                }
                dtImportExcel.Rows.Add(row);
              }
            }
          }
        }
      }
      // ItemCode
      if (text.Length > 0)
      {
        input[0] = new SqlDBParameter("@ItemCode", SqlDbType.Text, text);
      }

      // Customer
      text = string.Empty;
      text = txtCustomer.Text;
      if (text.Length > 0)
      {
        input[1] = new SqlDBParameter("@Customer", SqlDbType.Text, text);
      }

      // WO
      if (txtWO.Text.Length > 0)
      {
        input[2] = new SqlDBParameter("@WO", SqlDbType.Text, txtWO.Text);
      }

      // Data Import
      input[3] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtImportExcel);
      input[4] = new SqlDBParameter("@IsGetEnquiry", SqlDbType.Int, (rdEnquirySaleOrder.Checked ? 1 : 0));

      DataSet ds = SqlDataBaseAccess.SearchStoreProcedure("spPLNMasterPlanTotalOutStandingPO_Select", 300, input);
      if (ds != null)
      {
        dsPLNTotalOustandingOfPOs dsSource = new dsPLNTotalOustandingOfPOs();
        dsSource.Tables["dtTotalOustandingInfo"].Merge(ds.Tables[0]);
        dsSource.Tables["dtTotalOutstandingItem"].Merge(ds.Tables[1]);
        //dsSource.Tables["dtTotalOutstandingSub"].Merge(ds.Tables[2]);
        foreach (DataRow row in dsSource.Tables["dtTotalOustandingInfo"].Rows)
        {
          try
          {
            string imgPath = FunctionUtility.BOMGetItemImage(row["ItemCode"].ToString(), DBConvert.ParseInt(row["RevisionActive"].ToString()));
            row["Picture"] = FunctionUtility.ImageToByteArrayWithFormat(imgPath, 380, 1.77, "JPG");
          }
          catch { }

          try
          {
            int status = DBConvert.ParseInt(row["ItemKind"].ToString());
            row["PicItemKind"] = FunctionUtility.GetLocalItemKindIcon(status, 12); // 12 = JCUSA
          }
          catch { }
        }

        DaiCo.Shared.View_Report report = null;
        cptPLNTotalOutStandingOfPOs cpt = new cptPLNTotalOutStandingOfPOs();
        cpt.SetDataSource(dsSource);
        //cptPLNTotalOutStandingOfPOsSubReport_1 cptSub = new cptPLNTotalOutStandingOfPOsSubReport_1();
        //cptSub.SetDataSource(dsSource.Tables["dtTotalOutstandingSub"]);
        ControlUtility.ViewCrystalReport(cpt);
      }
    }

    /// <summary>
    /// Total Outstanding Of POs
    /// </summary>
    private void TotalOutstadingOfPOsForTest()
    {
      // Check Report & Customer
      if (ultReport.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Report");
        return;
      }

      SqlDBParameter[] input = new SqlDBParameter[5];
      // ItemCode
      string text = string.Empty;
      DataTable dtImportExcel = this.CreateDataTable();
      if (txtItemCode.Text.Length > 0)
      {
        text = this.txtItemCode.Text;
      }
      else
      {
        if (txtItemImport.Text.Length > 0)
        {
          DataTable dtSource = new DataTable();
          // Import Excel
          try
          {
            dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtItemImport.Text.Trim(), "SELECT * FROM [Sheet1 (1)$C4:D1504]").Tables[0];
          }
          catch
          {
            dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtItemImport.Text.Trim(), "SELECT * FROM [Sheet1$C4:D1504]").Tables[0];
          }
          if (dtSource != null)
          {
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
              if (dtSource.Rows[i]["ItemCode"].ToString().Trim().Length > 0)
              {
                text = text + ";" + dtSource.Rows[i]["ItemCode"].ToString();

                DataRow row = dtImportExcel.NewRow();
                // STT
                if (DBConvert.ParseInt(dtSource.Rows[i]["STT"]) > 0)
                {
                  row["STT"] = DBConvert.ParseInt(dtSource.Rows[i]["STT"]);
                }
                else
                {
                  row["STT"] = 1000;
                }
                // ItemCode
                if (dtSource.Rows[i]["ItemCode"].ToString().Trim().Length > 0)
                {
                  row["ItemCode"] = dtSource.Rows[i]["ItemCode"];
                }
                dtImportExcel.Rows.Add(row);
              }
            }
          }
        }
      }
      // ItemCode
      if (text.Length > 0)
      {
        input[0] = new SqlDBParameter("@ItemCode", SqlDbType.Text, text);
      }

      // Customer
      text = string.Empty;
      text = txtCustomer.Text;
      if (text.Length > 0)
      {
        input[1] = new SqlDBParameter("@Customer", SqlDbType.Text, text);
      }

      // WO
      if (txtWO.Text.Length > 0)
      {
        input[2] = new SqlDBParameter("@WO", SqlDbType.Text, txtWO.Text);
      }

      // Data Import
      input[3] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtImportExcel);
      input[4] = new SqlDBParameter("@IsGetEnquiry", SqlDbType.Int, (rdEnquirySaleOrder.Checked ? 1 : 0));

      DataSet ds = SqlDataBaseAccess.SearchStoreProcedure("spPLNMasterPlanTotalOutStandingPO_Select_ForTest", 1000, input);
      if (ds != null)
      {
        dsPLNTotalOustandingOfPOs dsSource = new dsPLNTotalOustandingOfPOs();
        dsSource.Tables["dtTotalOustandingInfo"].Merge(ds.Tables[0]);
        dsSource.Tables["dtTotalOutstandingItem"].Merge(ds.Tables[1]);
        dsSource.Tables["dtTotalOutstandingSub"].Merge(ds.Tables[2]);
        foreach (DataRow row in dsSource.Tables["dtTotalOustandingInfo"].Rows)
        {
          try
          {
            string imgPath = FunctionUtility.BOMGetItemImage(row["ItemCode"].ToString(), DBConvert.ParseInt(row["RevisionActive"].ToString()));
            row["Picture"] = FunctionUtility.ImageToByteArrayWithFormat(imgPath, 380, 1.77, "JPG");
          }
          catch { }

          try
          {
            int status = DBConvert.ParseInt(row["ItemKind"].ToString());
            row["PicItemKind"] = FunctionUtility.GetLocalItemKindIcon(status, 12); // 12 = JCUSA
          }
          catch { }
        }

        DaiCo.Shared.View_Report report = null;
        cptPLNTotalOutStandingOfPOs_1 cpt = new cptPLNTotalOutStandingOfPOs_1();
        cpt.SetDataSource(dsSource);
        //cptPLNTotalOutStandingOfPOsSubReport_1 cptSub = new cptPLNTotalOutStandingOfPOsSubReport_1();
        //cptSub.SetDataSource(dsSource.Tables["dtTotalOutstandingSub"]);
        ControlUtility.ViewCrystalReport(cpt);
      }
    }

    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("STT", typeof(System.Int32));
      dt.Columns.Add("ItemCode", typeof(System.String));
      return dt;
    }

    private void btnExportForTest_Click(object sender, EventArgs e)
    {
      this.TotalOutstadingOfPOsForTest();
    }

    #endregion Function
  }
}
