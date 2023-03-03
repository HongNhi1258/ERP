/*
  Author      : Huynh Thi Bang
  Date        : 26/10/2016
  Description : Import Outstanding
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;


namespace DaiCo.ERPProject
{
  public partial class viewPUR_02_003 : MainUserControl
  {
    #region Field
    public long transactionPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init
    public viewPUR_02_003()
    {
      InitializeComponent();
    }

    private void viewPUR_02_003_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion Init

    #region Function

    private void LoadData()
    {
      //if (this.transactionPid == long.MinValue)
      //{
      //  txtTransaction.Text = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPLNGetNewOutputCodeForChangeNoteWoQuantity('WCQ')", null).Rows[0][0].ToString();
      //  txtCreateBy.Text = SharedObject.UserInfo.EmpName.ToString();
      //  txtCreateDate.Text = DateTime.Today.ToString("dd/MM/yyyy");

      //}
      //else
      //{
      //  DBParameter[] param = new DBParameter[1];
      //  param[0] = new DBParameter("@transactionPid", DbType.Int64, this.transactionPid);
      //  string storeName = "spPLNChangeWoQuantityList_Select";
      //  DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
      //  txtTransaction.Text = dsSource.Tables[0].Rows[0]["TransactionCode"].ToString();
      //  txtCreateBy.Text = dsSource.Tables[0].Rows[0]["CreateBy"].ToString();
      //  txtRemark.Text = dsSource.Tables[0].Rows[0]["Remark"].ToString();
      //  txtReason.Text = dsSource.Tables[0].Rows[0]["Reason"].ToString();
      //  txtCreateDate.Text = dsSource.Tables[0].Rows[0]["CreateDate"].ToString();
      //ultData.DataSource = dsSource.Tables[1];
      //}
    }

    //private void btnExportExcel_Click(object sender, EventArgs e)
    //{
    //  this.ExportExcel();
    //}

    private void btnBrowseItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcelFile.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "RPT_PUR_02_003";
      string sheetName = "Data";
      string outFileName = "Data";
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
      if (!File.Exists(txtImportExcelFile.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }

      // Get data for items list
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), string.Format("SELECT * FROM [Data (1)$B5:C{0}]", 500));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      //input data
      DataTable dtSource = new DataTable();
      dtSource = dsItemList.Tables[0];

      SqlDBParameter[] sqlinput = new SqlDBParameter[1];
      DataTable dtInput = this.dtResult();
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        DataRow rowadd = dtInput.NewRow();
        if (DBConvert.ParseString(dtSource.Rows[i][0].ToString()) != "")
        {
          rowadd["SupplierCode"] = row["SupplierCode"];
          rowadd["Outstanding"] = row["Outstanding"];
          dtInput.Rows.Add(rowadd);
        }
      }
      sqlinput[0] = new SqlDBParameter("@Data", SqlDbType.Structured, dtInput);
      DataTable dtResult = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPURSupplierOutstanding", sqlinput);

      ultData.DataSource = dtResult;
      for (int j = 0; j < ultData.Rows.Count; j++)
      {
        if (ultData.Rows[j].Cells["StatusText"].Value.ToString().Trim().Length > 0)
        {
          ultData.Rows[j].Appearance.BackColor = Color.Yellow;
        }
      }

    }
    private DataTable dtResult()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("SupplierCode", typeof(System.String));
      dt.Columns.Add("Outstanding", typeof(System.Double));
      return dt;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      //Check error
      DataTable dt = (DataTable)ultData.DataSource;
      if (dt != null)
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (ultData.Rows[i].Cells["StatusText"].Value.ToString().Length > 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
            return;
          }
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0001");
        return;
      }
      // Add Data      
      if (this.SaveData())
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
      }
    }
    /// <summary>
    /// Save data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      DataTable dtSource = (DataTable)ultData.DataSource;
      DataTable dt = new DataTable();
      dt.Columns.Add("SupplierCode", typeof(System.String));
      dt.Columns.Add("Outstanding", typeof(System.Double));
      //foreach (DataRow row in dtSource.Rows)
      //{
      //  string supplier = row["SupplierCode"].ToString().Trim();
      //  double outstanding = DBConvert.ParseDouble(row["Outstanding"]);
      //  DataRow row1 = dt.NewRow();
      //  row1["SupplierCode"] = supplier;
      //  row1["Outstanding"] = outstanding;
      //  dt.Rows.Add(row1);

      //}
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string supplier = ultData.Rows[i].Cells["SupplierCode"].Value.ToString().Trim();
        double outstanding = DBConvert.ParseDouble(ultData.Rows[i].Cells["Outstanding"].Value.ToString());
        DataRow row1 = dt.NewRow();
        row1["SupplierCode"] = supplier;
        row1["Outstanding"] = outstanding;
        dt.Rows.Add(row1);
      }
      SqlDBParameter[] input = new SqlDBParameter[1];
      input[0] = new SqlDBParameter("@DataSource", SqlDbType.Structured, dt);
      SqlDBParameter[] output = new SqlDBParameter[1];
      output[0] = new SqlDBParameter("@Result", SqlDbType.Int, 0);
      SqlDataBaseAccess.ExecuteStoreProcedure("spPUROutstandingSupplier_Edit", input, output);
      long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      if (resultSave == 0)
      {
        return false;
      }

      return true;
    }

    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }
      e.Layout.Bands[0].Columns["StatusText"].Header.Caption = "Status Text";
      e.Layout.Bands[0].Columns["SupplierCode"].Header.Caption = "Supplier Code";
      e.Layout.Bands[0].Columns["EnglishName"].Header.Caption = "English Name";
      e.Layout.Bands[0].Columns["VietnameseName"].Header.Caption = "Vietnamese Name";

      e.Layout.Bands[0].Columns["StatusText"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SupplierCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["EnglishName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["VietnameseName"].CellActivation = Activation.ActivateOnly;

    }
    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
    #endregion Event

  }
}
