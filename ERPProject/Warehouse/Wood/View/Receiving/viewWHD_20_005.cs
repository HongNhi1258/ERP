/*
  Author      : Duong Minh
  Date        : 14/06/2012
  Description : Insert/Update Receiving From Supplier
*/
using DaiCo.Application;
using DaiCo.ERPProject.DataSetSource.Woods;
using DaiCo.ERPProject.Reports.Woods;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_20_005 : MainUserControl
  {
    #region Field
    // Pid Receiving
    public long receivingPid = long.MinValue;

    // Status
    private int status = 0;

    // Flag Update
    private bool canUpdate = false;

    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;
    private string printerName;
    private int whPid = 3;
    #endregion Field

    #region Init
    public viewWHD_20_005()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_20_005_Load(object sender, EventArgs e)
    {      
      if (this.receivingPid == long.MinValue)
      {
        // Check WH Summary of preMonth
        bool result = Utility.CheckWHPreMonthSummary(this.whPid);
        if (result == false)
        {
          this.CloseTab();
          return;
        }
      }      

      string startupPath = System.Windows.Forms.Application.StartupPath;
      this.pathTemplate = startupPath + @"\ExcelTemplate";
      this.pathExport = startupPath + @"\Report";

      // Load UltraCombo Supplier
      this.LoadComboSupplier();

      // Load Data
      this.LoadData();
    }        

    /// <summary>
    /// Load UltraCombo Supplier
    /// </summary>
    private void LoadComboSupplier()
    {
      string commandText = "SELECT ID_NhaCC, ID_NhaCC + ' - ' + TenNhaCCEN AS Name FROM VWHDSupplier";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultSupplier.DataSource = dtSource;
      ultSupplier.DisplayMember = "Name";
      ultSupplier.ValueMember = "ID_NhaCC";
      ultSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultSupplier.DisplayLayout.Bands[0].Columns["Name"].Width = 450;
      ultSupplier.DisplayLayout.Bands[0].Columns["ID_NhaCC"].Hidden = true;
    }

    /// <summary>
    /// LoadData
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ReceivingPid", DbType.Int64, this.receivingPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDReceivingInformationByReveivingPidWoods_Select", inputParam);
      DataTable dtReceivingInfo = dsSource.Tables[0];

      if (dtReceivingInfo.Rows.Count > 0)
      {
        DataRow row = dtReceivingInfo.Rows[0];

        this.txtReceivingNote.Text = row["ReceivingCode"].ToString();
        this.txtTitle.Text = row["Title"].ToString();

        this.status = DBConvert.ParseInt(row["Status"].ToString());

        this.ultSupplier.Value = row["ImportSource"].ToString();
        this.txtRemark.Text = row["Remark"].ToString();
        this.txtCreateBy.Text = row["CreateBy"].ToString();
        this.txtDate.Text = row["CreateDate"].ToString();

        if (this.status > 0)
        {
          this.chkComfirm.Checked = true;
        }
      }
      else
      {
        DataTable dtReceiving = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHDGetNewReceivingNoWoods('05SPR') NewReceivingNo");
        if ((dtReceiving != null) && (dtReceiving.Rows.Count > 0))
        {
          this.txtReceivingNote.Text = dtReceiving.Rows[0]["NewReceivingNo"].ToString();
          this.txtDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
          this.txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
        }
      }
      this.SetStatusControl();

      // Load Data Detail Receiving
      this.LoadDataReceivingDetail(dsSource);

      // Set Focus
      //this.txtTitle.Focus();
    }

    /// <summary>
    /// SetStatusControl
    /// </summary>
    private void SetStatusControl()
    {
      this.canUpdate = (btnSave.Visible && this.status == 0);

      this.ultSupplier.Enabled = this.canUpdate;
      this.txtTitle.Enabled = this.canUpdate;
      this.txtRemark.Enabled = this.canUpdate;
      this.txtImportExcel.Enabled = this.canUpdate;
      this.btnBrown.Enabled = this.canUpdate;
      this.btnImport.Enabled = this.canUpdate;
      this.btnGetTemplate.Enabled = this.canUpdate;

      this.chkComfirm.Enabled = this.canUpdate;
      this.btnSave.Enabled = this.canUpdate;
      this.btnPrintPreview.Enabled = !this.canUpdate;
      this.btnPrint.Enabled = !this.canUpdate;
    }

    /// <summary>
    /// Load Data Detail Receiving
    /// </summary>
    /// <param name="dsSource"></param>
    private void LoadDataReceivingDetail(DataSet dsSource)
    {
      this.ultReceipt.DataSource = dsSource.Tables[1];

      for (int i = 0; i < ultReceipt.Rows.Count; i++)
      {
        UltraGridRow row = ultReceipt.Rows[i];
        row.Activation = Activation.ActivateOnly;
      }
    }
    #endregion Init

    #region Event
    /// <summary>
    ///  Open Dialog Get File
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtImportExcel.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtImportExcel.Text.Trim().Length > 0);
    }

    /// <summary>
    /// Import File From Template
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnImport_Click(object sender, EventArgs e)
    {
      if (this.txtImportExcel.Text.Trim().Length == 0)
      {
        return;
      }

      try
      {
        DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [Data (1)$B5:G3007]").Tables[0];

        if (dtSource == null)
        {
          return;
        }
        foreach (DataRow drSource in dtSource.Rows)
        {
          if (drSource["IDWood"].ToString().Trim().Length > 0)
          {
            if (DBConvert.ParseDouble(drSource["Width"].ToString()) == double.MinValue)
            {
              drSource["Width"] = 0;
            }

            if (DBConvert.ParseDouble(drSource["Length"].ToString()) == double.MinValue)
            {
              drSource["Length"] = 0;
            }

            if (DBConvert.ParseDouble(drSource["Thickness"].ToString()) == double.MinValue)
            {
              drSource["Thickness"] = 0;
            }
          }
        }

        // Create dt Before Save
        DataTable dtNew = this.CreateDataTableBeforeSave();

        foreach (DataRow drMain in dtSource.Rows)
        {
          if (drMain["IDWood"].ToString().Trim().Length > 0)
          {
            DataRow row = dtNew.NewRow();
            row["IDVeneer"] = drMain["IDWood"].ToString();
            row["Code"] = drMain["Code"].ToString();
            row["Pcs"] = 1;
            row["Width"] = DBConvert.ParseDouble(drMain["Width"].ToString());
            row["Length"] = DBConvert.ParseDouble(drMain["Length"].ToString());
            row["Thickness"] = DBConvert.ParseDouble(drMain["Thickness"].ToString());
            row["Location"] = drMain["Package"].ToString();
            dtNew.Rows.Add(row);
          }
        }

        // Get DataTable Import
        DataTable result = this.GetDataTableImport(dtNew);
        if (result == null)
        {
          return;
        }

        DataTable dtMain = (DataTable)this.ultReceipt.DataSource;
        // Loop for datatable get from SQL
        foreach (DataRow dr in result.Rows)
        {
          DataRow row = dtMain.NewRow();
          DataRow[] foundRows = dtMain.Select("LotNoId = '" + dr["LotNoId"].ToString() + "'");
          if (foundRows.Length > 0)
          {
            row["Errors"] = 1;
          }
          else
          {
            row["Errors"] = DBConvert.ParseInt(dr["Errors"].ToString());
          }

          row["LotNoId"] = dr["LotNoId"].ToString();
          row["MaterialCode"] = dr["MaterialCode"].ToString();
          row["NameEN"] = dr["NameEN"].ToString();
          row["NameVN"] = dr["NameVN"].ToString();
          row["Qty"] = 1;
          row["Width"] = DBConvert.ParseDouble(dr["Width"].ToString());
          row["Length"] = DBConvert.ParseDouble(dr["Length"].ToString());
          row["Thickness"] = DBConvert.ParseDouble(dr["Thickness"].ToString());
          row["TotalCBM"] = DBConvert.ParseDouble(dr["TotalCBM"].ToString());
          row["Location"] = dr["Location"].ToString();
          dtMain.Rows.Add(row);
        }

        this.ultReceipt.DataSource = dtMain;
        for (int i = 0; i < ultReceipt.Rows.Count; i++)
        {
          UltraGridRow row = ultReceipt.Rows[i];
          row.Activation = Activation.ActivateOnly;

          if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 1)
          {
            row.CellAppearance.BackColor = Color.Yellow;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 2)
          {
            row.CellAppearance.BackColor = Color.CornflowerBlue;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 3)
          {
            row.CellAppearance.BackColor = Color.Lime;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 4)
          {
            row.CellAppearance.BackColor = Color.Red;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 5)
          {
            row.CellAppearance.BackColor = Color.Magenta;
          }
        }

        btnImport.Enabled = false;
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0105");
        return;
      }
    }

    /// <summary>
    /// Get Template From Server
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "Template Woods Import Data";
      string sheetName = "Data";
      string outFileName = "Template Woods Import Data";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    /// <summary>
    /// Save (Insert/Update) Receiving Note
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
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
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }

      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Format Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultReceipt_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Errors"].Hidden = true;

      e.Layout.Bands[0].Columns["LotNoId"].Header.Caption = "ID Wood";
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["TotalCBM"].Header.Caption = "CBM";
      e.Layout.Bands[0].Columns["Location"].Header.Caption = "Package";

      e.Layout.Bands[0].Columns["LotNoId"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["LotNoId"].MinWidth = 70;

      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 90;

      e.Layout.Bands[0].Columns["Location"].MaxWidth = 110;
      e.Layout.Bands[0].Columns["Location"].MinWidth = 110;

      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 60;

      e.Layout.Bands[0].Columns["Length"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Length"].MinWidth = 70;

      e.Layout.Bands[0].Columns["Width"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Width"].MinWidth = 70;

      e.Layout.Bands[0].Columns["Thickness"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Thickness"].MinWidth = 70;

      e.Layout.Bands[0].Columns["TotalCBM"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["TotalCBM"].MinWidth = 120;

      e.Layout.Bands[0].Columns["TotalCBM"].CellAppearance.ForeColor = Color.Blue;

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      //Sum Qty And Total CBM
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalCBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.0000}";

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
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
    /// Delete Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultReceipt_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      if (this.receivingPid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) != 0)
          {
            continue;
          }

          string lotNoId = row.Cells["LotNoId"].Value.ToString().Substring(1, row.Cells["LotNoId"].Value.ToString().Length - 2);

          DBParameter[] inputParams = new DBParameter[2];
          inputParams[0] = new DBParameter("@ReceivingPid", DbType.Int64, this.receivingPid);
          inputParams[1] = new DBParameter("@LotNoId", DbType.String, lotNoId);

          DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          string storeName = string.Empty;
          storeName = "spWHDReceivingDetailWoods_Delete";

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParams, outputParams);
          if (DBConvert.ParseInt(outputParams[0].Value.ToString()) != 1)
          {
            WindowUtinity.ShowMessageError("ERR0004");
            this.LoadData();
            return;
          }
        }
      }
    }

    /// <summary>
    /// Print Issuing Note
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrintPreview_Click(object sender, EventArgs e)
    {
      viewWHD_99_001 view = new viewWHD_99_001();
      view.ncategory = 2;
      view.receivingPid = receivingPid;
      Shared.Utility.WindowUtinity.ShowView(view, "Report", false, Shared.Utility.ViewState.ModalWindow);
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      PrintDialog prt = new PrintDialog();
      DialogResult result = prt.ShowDialog();

      if (result == DialogResult.OK)
      {
        int nCopy = prt.PrinterSettings.Copies;
        int sPage = prt.PrinterSettings.FromPage;
        int ePage = prt.PrinterSettings.ToPage;

        // Report
        DBParameter[] inputParam = new DBParameter[1];
        inputParam[0] = new DBParameter("@RecivingNotePid", DbType.Int64, this.receivingPid);

        DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHDRPTReceivingNoteWoods_Select", inputParam);

        dsWHDWoodsIssuingNote dsSource = new dsWHDWoodsIssuingNote();
        dsSource.Tables["TblWHDWoodsIssuingNoteDetail"].Merge(ds.Tables[1]);

        double qty = 0;
        double pcs = 0;
        foreach (DataRow row in dsSource.Tables["TblWHDWoodsIssuingNoteDetail"].Rows)
        {
          qty += DBConvert.ParseDouble(row["Qty"].ToString());
          pcs += DBConvert.ParseDouble(row["Pcs"].ToString());
        }
        qty = Math.Round(qty, 4);
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
        rpt.SetParameterValue("TotalPcs", pcs);

        // End Report

        printerName = DaiCo.Shared.Utility.FunctionUtility.GetDefaultPrinter();
        try
        {
          rpt.PrintOptions.PrinterName = printerName;
          rpt.PrintToPrinter(nCopy, false, sPage, ePage);
          WindowUtinity.ShowMessageSuccessFromText("Print Successfully");
        }
        catch (Exception err)
        {
          MessageBox.Show(err.ToString());
        }
      }
    }
    #endregion Event

    #region Process
    /// <summary>
    /// Get DataTable When Import Data
    /// </summary>
    /// <param name="dtSource"></param>
    /// <returns></returns>
    private DataTable GetDataTableImport(DataTable dtSource)
    {
      DataTable dt = new DataTable();
      SqlCommand cm = new SqlCommand("spWHDGetDataImportReceivingWoods_Select");
      cm.Connection = new SqlConnection(DBConvert.DecodeConnectiontring(ConfigurationSettings.AppSettings["connectionString"]));
      cm.CommandType = CommandType.StoredProcedure;

      // Data Table 
      SqlParameter para = cm.CreateParameter();
      para.ParameterName = "@ImportData";
      para.SqlDbType = SqlDbType.Structured;
      para.Value = dtSource;

      cm.Parameters.Add(para);

      SqlDataAdapter adp = new SqlDataAdapter();
      adp.SelectCommand = cm;
      DataSet result = new DataSet();
      try
      {
        if (cm.Connection.State != ConnectionState.Open)
        {
          cm.Connection.Open();
        }
        adp.Fill(result);
      }
      catch (Exception ex)
      {
        result = null;
        return null;
      }

      dt = result.Tables[0];
      return dt;
    }
    /// <summary>
    /// Main Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool result = true;

      //Master Information Receiving Woods
      result = this.SaveReceivingWoods();

      if (!result)
      {
        return false;
      }

      // Save Receiving Woods Detail 
      result = this.SaveReceivingDetailWoods();

      return result;
    }

    /// <summary>
    /// Save Receiving Woods Detail 
    /// </summary>
    /// <returns></returns>
    private bool SaveReceivingDetailWoods()
    {
      // Create dt Before Save
      DataTable dtSource = this.CreateDataTableBeforeSave();

      // Get Main Data Receiving Detail
      DataTable dtMain = (DataTable)this.ultReceipt.DataSource;

      foreach (DataRow drMain in dtMain.Rows)
      {
        // Get Row is Added and no Errors
        if (drMain.RowState == DataRowState.Added && DBConvert.ParseDouble(drMain["Errors"].ToString()) == 0)
        {
          DataRow row = dtSource.NewRow();
          row["IDVeneer"] = drMain["LotNoId"].ToString();
          row["Code"] = drMain["MaterialCode"].ToString();
          row["Pcs"] = DBConvert.ParseDouble(drMain["Qty"].ToString());
          row["Width"] = DBConvert.ParseDouble(drMain["Width"].ToString());
          row["Length"] = DBConvert.ParseDouble(drMain["Length"].ToString());
          row["Thickness"] = DBConvert.ParseDouble(drMain["Thickness"].ToString());
          row["Location"] = drMain["Location"].ToString();
          dtSource.Rows.Add(row);
        }
      }

      SqlCommand cm = new SqlCommand("spWHDReceivingDetailWoods_Insert");
      cm.Connection = new SqlConnection(DBConvert.DecodeConnectiontring(ConfigurationSettings.AppSettings["connectionString"]));
      cm.CommandType = CommandType.StoredProcedure;

      // Data Table 
      SqlParameter para = cm.CreateParameter();
      para.ParameterName = "@ImportData";
      para.SqlDbType = SqlDbType.Structured;
      para.Value = dtSource;

      cm.Parameters.Add(para);

      // Receiving Pid  
      para = cm.CreateParameter();
      para.ParameterName = "@ReceivingPid";
      para.DbType = DbType.Int64;
      para.Value = this.receivingPid;

      cm.Parameters.Add(para);

      try
      {
        if (cm.Connection.State != ConnectionState.Open)
        {
          cm.Connection.Open();
        }
        cm.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        string a = ex.Message;
        return false;
      }
      finally
      {
        if (cm.Connection.State != ConnectionState.Closed)
        {
          cm.Connection.Close();
        }
        cm.Connection.Dispose();
        cm.Dispose();
      }

      // Send To PUR
      if (this.chkComfirm.Checked)
      {
        Email email = new Email();
        email.Key = email.KEY_WHD_001;
        ArrayList arrList = email.GetDataMain(email.Key);
        if (arrList.Count == 3)
        {
          string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
          string subject = string.Format(arrList[1].ToString(), this.txtReceivingNote.Text, userName);
          string body = string.Format(arrList[2].ToString(), this.txtReceivingNote.Text, userName);
          email.InsertEmail(email.Key, arrList[0].ToString(), subject, body);
        }
      }
      return true;
    }

    /// <summary>
    /// Create DataTable Before Save
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTableBeforeSave()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("IDVeneer", typeof(System.String));
      dt.Columns.Add("Code", typeof(System.String));
      dt.Columns.Add("Pcs", typeof(System.Double));
      dt.Columns.Add("Width", typeof(System.Double));
      dt.Columns.Add("Length", typeof(System.Double));
      dt.Columns.Add("Thickness", typeof(System.Double));
      dt.Columns.Add("Location", typeof(System.String));
      return dt;
    }

    /// <summary>
    /// Save Master Information Receiving Woods
    /// </summary>
    /// <returns></returns>
    private bool SaveReceivingWoods()
    {
      string storeName = string.Empty;


      // Update
      if (this.receivingPid != long.MinValue)
      {
        storeName = "spWHDReceivingWoods_Update";
        DBParameter[] inputParamUpdate = new DBParameter[7];

        // Pid
        inputParamUpdate[0] = new DBParameter("@PID", DbType.Int64, this.receivingPid);

        // Title
        if (this.txtTitle.Text.Length > 0)
        {
          inputParamUpdate[1] = new DBParameter("@Title", DbType.AnsiString, 4000, this.txtTitle.Text);
        }

        // Status
        if (this.chkComfirm.Checked)
        {
          inputParamUpdate[2] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          inputParamUpdate[2] = new DBParameter("@Status", DbType.Int32, 0);
        }

        // Import Source
        inputParamUpdate[3] = new DBParameter("@ImportSource", DbType.String, this.ultSupplier.Value.ToString());

        // Type
        inputParamUpdate[4] = new DBParameter("@Type", DbType.Int32, 4);

        // Remark
        inputParamUpdate[5] = new DBParameter("@Remark", DbType.AnsiString, 4000, this.txtRemark.Text);

        // UpdateBy
        inputParamUpdate[6] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

        DBParameter[] outputParamUpdate = new DBParameter[1];
        outputParamUpdate[0] = new DBParameter("@ResultPid", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamUpdate, outputParamUpdate);

        long resultPid = DBConvert.ParseLong(outputParamUpdate[0].Value.ToString());
        this.receivingPid = resultPid;
        if (resultPid == long.MinValue)
        {
          return false;
        }
      }
      // Insert
      else
      {
        storeName = "spWHDReceivingWoods_Insert";
        DBParameter[] inputParamInsert = new DBParameter[7];

        // Title
        if (this.txtTitle.Text.Length > 0)
        {
          inputParamInsert[0] = new DBParameter("@Title", DbType.AnsiString, 4000, this.txtTitle.Text);
        }

        // Status
        if (this.chkComfirm.Checked)
        {
          inputParamInsert[1] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          inputParamInsert[1] = new DBParameter("@Status", DbType.Int32, 0);
        }

        // Import Source
        inputParamInsert[2] = new DBParameter("@ImportSource", DbType.String, this.ultSupplier.Value.ToString());

        // Type
        inputParamInsert[3] = new DBParameter("@Type", DbType.Int32, 4);

        // Remark
        inputParamInsert[4] = new DBParameter("@Remark", DbType.AnsiString, 4000, this.txtRemark.Text);

        // CreateBy
        inputParamInsert[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

        // ReceivingNo
        inputParamInsert[6] = new DBParameter("@ReceivingNo", DbType.String, "05SPR");

        DBParameter[] outputParamInsert = new DBParameter[1];
        outputParamInsert[0] = new DBParameter("@ResultPid", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamInsert, outputParamInsert);

        long resultPid = DBConvert.ParseLong(outputParamInsert[0].Value.ToString());
        this.receivingPid = resultPid;
        if (resultPid == long.MinValue)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Check valid before save
    /// </summary>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      // Master Information
      if (this.ultSupplier.Value != null && this.ultSupplier.Value.ToString().Length > 0)
      {
        string commandText = string.Empty;
        commandText = "SELECT COUNT(*) FROM VWHDSupplier WHERE ID_NhaCC = '" + this.ultSupplier.Value.ToString() + "'";
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
          {
            message = "Supplier";
            return false;
          }
        }
        else
        {
          message = "Supplier";
          return false;
        }
      }
      else
      {
        message = "Supplier";
        return false;
      }

      // Detail
      DataTable dtMain = (DataTable)this.ultReceipt.DataSource;

      foreach (DataRow drMain in dtMain.Rows)
      {
        if (drMain.RowState != DataRowState.Deleted)
        {
          //Errors
          if (DBConvert.ParseInt(drMain["Errors"].ToString()) != 0)
          {
            message = "Data Input";
            return false;
          }
        }
      }

      return true;
    }
    #endregion Process
  }
}
