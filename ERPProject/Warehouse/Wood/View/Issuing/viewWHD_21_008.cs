/*
  Author      : Xuân Trường
  Date        : 18/04/2013
  Description : Issue Request Online
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
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_21_008 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    private bool isConfirm = false;
    private int whPid = 3;
    #endregion Field

    #region Init

    public viewWHD_21_008()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_21_008_Load(object sender, EventArgs e)
    {      
      if (this.pid == long.MinValue)
      {
        // Check WH Summary of preMonth
        bool result = Utility.CheckWHPreMonthSummary(this.whPid);
        if (result == false)
        {
          this.CloseTab();
          return;
        }
      }

      this.LoadInit();
      this.LoadData();
      this.LoadComboRequestNote();
    }

    /// <summary>
    /// Load Init
    /// </summary>
    private void LoadInit()
    {
      this.LoadComboDepartment();
    }

    /// <summary>
    /// Load Request Note
    /// </summary>
    private void LoadComboRequestNote()
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT RES.Pid, RES.Code";
      commandText += " FROM TblGNRWoodsRequisitionNote RES";
      commandText += " INNER JOIN TblGNRWoodsRequisitionNoteDetail RED ON RES.Pid = RED.MRNPid";
      commandText += " WHERE 1 = 1 ";
      if (this.isConfirm == false)
      {
        commandText += " AND RES.[Status] = 1 ";
      }
      commandText += " ORDER BY RES.Pid DESC";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBRequestOnline.DataSource = dtSource;
      ultCBRequestOnline.DisplayMember = "Code";
      ultCBRequestOnline.ValueMember = "Pid";
      ultCBRequestOnline.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBRequestOnline.DisplayLayout.Bands[0].Columns["Code"].Width = 150;
      ultCBRequestOnline.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Create By
    /// </summary>
    private void LoadComboReceiver(string department)
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_NhanVien, CONVERT(VARCHAR, ID_NhanVien) + ' - ' + HoNV + ' ' + TenNV Name";
      commandText += " FROM VHRNhanVien ";
      commandText += " WHERE Resigned = 0 AND Department = '" + department + "'";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBReceiver.DataSource = dtSource;
      ultCBReceiver.DisplayMember = "Name";
      ultCBReceiver.ValueMember = "ID_NhanVien";
      ultCBReceiver.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBReceiver.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultCBReceiver.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Department
    /// </summary>
    private void LoadComboDepartment()
    {
      string commandText = string.Empty;
      commandText += " SELECT Department, Department + ' - ' + DeparmentName AS DeparmentName FROM VHRDDepartment";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBDepartment.DataSource = dtSource;
      ultCBDepartment.DisplayMember = "DeparmentName";
      ultCBDepartment.ValueMember = "Department";
      ultCBDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBDepartment.DisplayLayout.Bands[0].Columns["DeparmentName"].Width = 150;
      ultCBDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.pid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDWoodsIssueRequestOnline_Select", inputParam);
      if (dsSource != null)
      {
        // 1: Load Master
        DataTable dtInfo = dsSource.Tables[0];
        if (dtInfo.Rows.Count > 0)
        {
          DataRow row = dtInfo.Rows[0];
          ultCBRequestOnline.Value = DBConvert.ParseLong(row["RequestPid"].ToString());
          txtIssuingNo.Text = row["IssuingCode"].ToString();
          txtCreateBy.Text = row["CreateBy"].ToString();
          txtCreateDate.Text = row["CreateDate"].ToString();
          ultCBDepartment.Value = row["DepartmentRequest"].ToString();
          ultCBReceiver.Value = DBConvert.ParseInt(row["Receiver"].ToString());
          txtRemark.Text = row["Title"].ToString();
          this.isConfirm = (DBConvert.ParseInt(row["Status"].ToString()) == 1) ? true : false;
        }
        else
        {
          DataTable dtCode = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHDGetNewIssuingNoWoods('05ISS') NewRequestNo");
          if ((dtCode != null) && (dtCode.Rows.Count == 1))
          {
            this.txtIssuingNo.Text = dtCode.Rows[0]["NewRequestNo"].ToString();
            this.txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
            this.txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          }
        }

        // 2: Load Detail
        // 2.1 Master
        DataSet ds = this.CreateDataSet();
        ds.Tables["dtParent"].Merge(dsSource.Tables[1]);
        ds.Tables["dtChild"].Merge(dsSource.Tables[2]);
        ultInfo.DataSource = ds;

        // 2.2 Detail
        ultDetail.DataSource = dsSource.Tables[3];

        // 3: Set Status Control
        this.SetStatusControl();
      }
    }

    #endregion Init

    #region Function

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      if (this.isConfirm)
      {
        ultCBRequestOnline.ReadOnly = true;
        btnLoadData.Enabled = false;
        ultCBReceiver.ReadOnly = true;
        txtRemark.ReadOnly = true;
        btnImport.Enabled = false;
        btnAddMultiDetail.Enabled = false;
        chkConfirm.Enabled = false;
        btnSave.Enabled = false;
        btnDelete.Enabled = false;
        chkConfirm.Checked = true;
        chkExpand.Checked = true;
        grpImport.Visible = false;
      }
    }

    /// <summary>
    /// Creata Dataset
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("MaterialNameEn", typeof(System.String));
      taParent.Columns.Add("MaterialNameVn", typeof(System.String));
      taParent.Columns.Add("Unit", typeof(System.String));
      taParent.Columns.Add("QtyRequest", typeof(System.Double));
      taParent.Columns.Add("Issue", typeof(System.Double));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("WO", typeof(System.Int64));
      taChild.Columns.Add("CarcassCode", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Double));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("taParent_taChild", taParent.Columns["MaterialCode"], taChild.Columns["MaterialCode"], false));
      return ds;
    }

    /// <summary>
    /// Get Data Import
    /// </summary>
    /// <param name="dtSource"></param>
    private DataTable GetDataImport(DataTable dtSource)
    {
      SqlDBParameter[] input = new SqlDBParameter[2];
      input[0] = new SqlDBParameter("@WRNPid", SqlDbType.BigInt, DBConvert.ParseLong(ultCBRequestOnline.Value.ToString()));
      input[1] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtSource);
      DataTable dtMain = SqlDataBaseAccess.SearchStoreProcedureDataTable("spWHDWoodsGetDataIssuingRequestOnline_Select", input);
      return dtMain;
    }

    /// <summary>
    /// Check Data
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      //1: Check Requisition Note Issued
      string commandText = string.Empty;
      commandText = "SELECT * ";
      commandText += "FROM TblWHDIssuing ";
      commandText += "WHERE WRNPid = " + DBConvert.ParseLong(ultCBRequestOnline.Value.ToString()) + "";
      if (this.pid != long.MinValue)
      {
        commandText += " AND PID <> " + this.pid + "";
      }

      DataTable dtRequest = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtRequest != null && dtRequest.Rows.Count > 0)
      {
        message = "Requisition Note Was Issued";
        return false;
      }

      // 2: Check Receiver
      if (ultCBReceiver.Value == null)
      {
        message = "Receiver";
        return false;
      }

      // 3: Issue <= RequestQty
      for (int i = 0; i < ultInfo.Rows.Count; i++)
      {
        UltraGridRow row = ultInfo.Rows[i];
        if (DBConvert.ParseDouble(row.Cells["QtyRequest"].Value.ToString()) < DBConvert.ParseDouble(row.Cells["Issue"].Value.ToString()))
        {
          row.Cells["Issue"].Appearance.BackColor = Color.Yellow;
          message = "Issue <= RequestQty";
          return false;
        }
        else
        {
          row.Cells["Issue"].Appearance.BackColor = Color.White;
        }
      }

      // 4: Check Detail
      for (int j = 0; j < ultDetail.Rows.Count; j++)
      {
        UltraGridRow row = ultDetail.Rows[j];
        if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) != 0)
        {
          message = "LotNoId";
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Master
    /// </summary>
    /// <returns></returns>
    private bool SaveMaster()
    {
      DBParameter[] inputParam = new DBParameter[10];
      if (this.pid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      }
      inputParam[1] = new DBParameter("@PrefixCode", DbType.AnsiString, 8, "05ISS");
      if (chkConfirm.Checked)
      {
        inputParam[2] = new DBParameter("@Status", DbType.Int32, 1);
      }
      else
      {
        inputParam[2] = new DBParameter("@Status", DbType.Int32, 0);
      }
      if (ultCBDepartment.Value != null)
      {
        inputParam[3] = new DBParameter("@DepartmentRequest", DbType.AnsiString, 8, ultCBDepartment.Value.ToString());
      }
      if (txtRemark.Text.Trim().Length > 0)
      {
        inputParam[4] = new DBParameter("@Title", DbType.String, 1024, txtRemark.Text);
      }
      inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[6] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[7] = new DBParameter("@WRNPid", DbType.Int64, DBConvert.ParseLong(ultCBRequestOnline.Value.ToString()));
      inputParam[8] = new DBParameter("@Receiver", DbType.Int64, DBConvert.ParseLong(ultCBReceiver.Value.ToString()));
      inputParam[9] = new DBParameter("@Type", DbType.Int32, 6);

      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spWHDWoodsIssuingInfo_Edit", inputParam, outputParam);
      long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      // Gan Lai Pid
      this.pid = result;

      if (result <= 0)
      {
        return false;
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
      dt.Columns.Add("WidthEXI", typeof(System.Double));
      dt.Columns.Add("LengthEXI", typeof(System.Double));
      dt.Columns.Add("ThicknessEXI", typeof(System.Double));
      dt.Columns.Add("Package", typeof(System.String));
      dt.Columns.Add("Location", typeof(System.String));
      return dt;
    }

    ///// <summary>
    ///// Save Issuing Woods Detail 
    ///// </summary>
    ///// <returns></returns>
    //private bool SaveIssuingDetailWoods()
    //{
    //  // Create dt Before Save
    //  DataTable dtSource = this.CreateDataTableBeforeSave();

    //  // Get Main Data Issuing Detail
    //  DataTable dtMain = (DataTable)this.ultDetail.DataSource;

    //  foreach (DataRow drMain in dtMain.Rows)
    //  {
    //    // Get Row is not Delete and no Errors
    //    if (drMain.RowState != DataRowState.Deleted && DBConvert.ParseDouble(drMain["Error"].ToString()) == 0)
    //    {
    //      DataRow row = dtSource.NewRow();
    //      row["IDVeneer"] = drMain["LotNoId"].ToString();
    //      row["Code"] = drMain["MaterialCode"].ToString();
    //      row["Pcs"] = 1;
    //      row["Width"] = 0;
    //      row["Length"] = 0;
    //      row["Thickness"] = 0;
    //      row["Location"] = drMain["Package"].ToString(); ;
    //      dtSource.Rows.Add(row);
    //    }
    //  }

    //  SqlCommand cm = new SqlCommand("spWHDIssuingDetailWoods_Insert");
    //  cm.Connection = new SqlConnection(DBConvert.DecodeConnectiontring(ConfigurationSettings.AppSettings["connectionString"]));
    //  cm.CommandType = CommandType.StoredProcedure;
    //  cm.CommandTimeout = 500;

    //  // Data Table 
    //  SqlParameter para = cm.CreateParameter();
    //  para.ParameterName = "@ImportData";
    //  para.SqlDbType = SqlDbType.Structured;
    //  para.Value = dtSource;

    //  cm.Parameters.Add(para);

    //  // Issuing Pid
    //  para = cm.CreateParameter();
    //  para.ParameterName = "@IssuingPid";
    //  para.DbType = DbType.Int64;
    //  para.Value = this.pid;

    //  cm.Parameters.Add(para);

    //  try
    //  {
    //    if (cm.Connection.State != ConnectionState.Open)
    //    {
    //      cm.Connection.Open();
    //    }
    //    cm.ExecuteNonQuery();
    //  }
    //  catch (Exception ex)
    //  {
    //    string a = ex.Message;
    //    return false;
    //  }
    //  finally
    //  {
    //    if (cm.Connection.State != ConnectionState.Closed)
    //    {
    //      cm.Connection.Close();
    //    }
    //    cm.Connection.Dispose();
    //    cm.Dispose();
    //  }

    //  return true;
    //}

    private bool SaveIssuingDetailWoods()
    {
      // Create dt Before Save
      DataTable dtSource = this.CreateDataTableBeforeSave();

      // Get Main Data Issuing Detail
      DataTable dtMain = (DataTable)this.ultDetail.DataSource;

      foreach (DataRow drMain in dtMain.Rows)
      {
        // Get Row is not Delete and no Errors
        if (drMain.RowState != DataRowState.Deleted && DBConvert.ParseDouble(drMain["Error"].ToString()) == 0)
        {
          DataRow row = dtSource.NewRow();
          row["IDVeneer"] = drMain["LotNoId"].ToString();
          row["Code"] = drMain["MaterialCode"].ToString();
          row["Pcs"] = 1;
          row["Width"] = 0;
          row["Length"] = 0;
          row["Thickness"] = 0;
          row["WidthEXI"] = 0;
          row["LengthEXI"] = 0;
          row["ThicknessEXI"] = 0;
          row["Package"] = drMain["Package"].ToString();
          row["Location"] = drMain["Location"].ToString();
          dtSource.Rows.Add(row);
        }
      }

      SqlCommand cm = new SqlCommand("spWHDIssuingDetailWoods_Insert");
      cm.Connection = new SqlConnection(DBConvert.DecodeConnectiontring(ConfigurationSettings.AppSettings["connectionString"]));
      cm.CommandType = CommandType.StoredProcedure;
      cm.CommandTimeout = 6000;

      // Data Table 
      SqlParameter para = cm.CreateParameter();
      para.ParameterName = "@ImportData";
      para.SqlDbType = SqlDbType.Structured;
      para.Value = dtSource;

      cm.Parameters.Add(para);

      // Issuing Pid
      para = cm.CreateParameter();
      para.ParameterName = "@IssuingPid";
      para.DbType = DbType.Int64;
      para.Value = this.pid;

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

      //SqlDBParameter[] input = new SqlDBParameter[2];
      //input[0] = new SqlDBParameter("@IssuingPid", SqlDbType.BigInt, this.pid);
      //input[1] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtSource);

      //SqlDBParameter[] output = new SqlDBParameter[1];
      //output[0] = new SqlDBParameter("@Result", SqlDbType.Int, 0);
      //SqlDataBaseAccess.ExecuteStoreProcedure("spWHDIssuingDetailWoods_Insert", input, output);
      //int result = DBConvert.ParseInt(output[0].Value.ToString());
      //if (result <= 0)
      //{
      //  return false;
      //}
      return true;
    }

    /// <summary>
    /// SaveData
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool success = this.SaveMaster();
      if (success)
      {
        success = this.SaveIssuingDetailWoods();
        if (success)
        {
          success = this.UpdateValueAfterIssue();
        }
      }
      return success;
    }

    /// <summary>
    /// Update QtyIssue Request Online Detail
    /// Update Status Request Online Info
    /// Update QtyIssue Supplement Detail
    /// Update Status Supplement Info
    /// Update QtyIssue Allocate WO Summary
    /// </summary>
    /// <returns></returns>
    private bool UpdateValueAfterIssue()
    {
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@IssuingPid", DbType.Int64, this.pid);
      input[1] = new DBParameter("@MRNPid", DbType.Int64, DBConvert.ParseLong(ultCBRequestOnline.Value.ToString()));

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spWHDWoodsIssuingRequestOnline_Update", 6000, input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    #endregion Function

    #region Event

    /// <summary>
    /// Load Request Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLoadData_Click(object sender, EventArgs e)
    {
      if (ultCBRequestOnline.Value == null)
      {
        return;
      }

      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@WRNPid", DbType.Int64, DBConvert.ParseLong(ultCBRequestOnline.Value.ToString()));

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHDWoodsGetInfoRequestOnline_Select", input);
      if (ds == null)
      {
        return;
      }
      DataTable dtInfo = ds.Tables[0];
      DataTable dtDetailParent = ds.Tables[1];
      DataTable dtDetailChild = ds.Tables[2];

      // Gan Gia Tri
      // 1: Info
      ultCBDepartment.Value = dtInfo.Rows[0]["DepartmentRequest"].ToString();
      ultCBReceiver.Value = DBConvert.ParseInt(dtInfo.Rows[0]["CreateBy"].ToString());
      // 2: Detail
      DataSet dsMain = this.CreateDataSet();
      dsMain.Tables["dtParent"].Merge(dtDetailParent);
      dsMain.Tables["dtChild"].Merge(dtDetailChild);
      ultInfo.DataSource = dsMain;

      // 3: ReadOnly Combo
      ultCBRequestOnline.ReadOnly = true;
    }

    /// <summary>
    /// Get Template
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "WHDWoodsImportBarcode";
      string sheetName = "Sheet1";
      string outFileName = "List Barcode Issue";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    /// <summary>
    /// Import Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnImport_Click(object sender, EventArgs e)
    {
      // Import Data
      if (this.txtImportData.Text.Trim().Length == 0)
      {
        return;
      }
      try
      {
        DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImportData.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B2:B200]").Tables[0];
        if (dtSource == null)
        {
          return;
        }
        DataTable dtNew = new DataTable();
        dtNew.Columns.Add("LotNoId", typeof(System.String));
        for (int i = 0; i < dtSource.Rows.Count; i++)
        {
          if (dtSource.Rows[i]["ID Woods"].ToString().Trim().Length > 0)
          {
            DataRow row = dtNew.NewRow();
            row["LotNoId"] = dtSource.Rows[i]["ID Woods"].ToString();
            dtNew.Rows.Add(row);
          }
        }
        // Result Import Data
        DataTable dtResult = this.GetDataImport(dtNew);
        if (dtResult == null)
        {
          return;
        }
        DataTable dtDetail = (DataTable)ultDetail.DataSource;

        // 3: Check Trung(BarCode)
        foreach (DataRow dr in dtResult.Rows)
        {
          DataRow row = dtDetail.NewRow();
          string select = string.Empty;
          select = string.Format(@"LotNoId = '{0}'", dr["LotNoId"].ToString());
          DataRow[] foundRows = dtDetail.Select(select);
          if (foundRows.Length >= 1)
          {
            row["Error"] = 1;
          }
          else
          {
            row["Error"] = DBConvert.ParseInt(dr["Error"].ToString());
          }

          row["LotNoId"] = dr["LotNoId"].ToString();
          row["MaterialCode"] = dr["MaterialCode"].ToString();

          if (DBConvert.ParseLong(dr["DimensionPid"].ToString()) != long.MinValue)
          {
            row["DimensionPid"] = DBConvert.ParseLong(dr["DimensionPid"].ToString());
          }

          if (DBConvert.ParseDouble(dr["Length"].ToString()) != double.MinValue)
          {
            row["Length"] = DBConvert.ParseDouble(dr["Length"].ToString());
          }

          if (DBConvert.ParseDouble(dr["Width"].ToString()) != double.MinValue)
          {
            row["Width"] = DBConvert.ParseDouble(dr["Width"].ToString());
          }

          if (DBConvert.ParseDouble(dr["Thickness"].ToString()) != double.MinValue)
          {
            row["Thickness"] = DBConvert.ParseDouble(dr["Thickness"].ToString());
          }

          if (DBConvert.ParseLong(dr["PackagePid"].ToString()) != long.MinValue)
          {
            row["PackagePid"] = DBConvert.ParseLong(dr["PackagePid"].ToString());
          }

          if (dr["Package"].ToString().Trim().Length > 0)
          {
            row["Package"] = dr["Package"].ToString();
          }
          if (DBConvert.ParseLong(dr["LocationPid"].ToString()) != long.MinValue)
          {
            row["LocationPid"] = DBConvert.ParseLong(dr["LocationPid"].ToString());
          }

          if (dr["Location"].ToString().Trim().Length > 0)
          {
            row["Location"] = dr["Location"].ToString();
          }

          if (DBConvert.ParseDouble(dr["CBM"].ToString()) != double.MinValue)
          {
            row["CBM"] = DBConvert.ParseDouble(dr["CBM"].ToString());
          }
          dtDetail.Rows.Add(row);
        }

        // Add Detail
        for (int i = 0; i < ultDetail.Rows.Count; i++)
        {
          UltraGridRow row = ultDetail.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 1)
          {
            row.Appearance.BackColor = Color.Yellow;
          }
          else if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 2)
          {
            row.Appearance.BackColor = Color.Yellow;
          }
        }

        // Tinh IssueQty
        for (int i = 0; i < ultInfo.Rows.Count; i++)
        {
          UltraGridRow rowInfo = ultInfo.Rows[i];
          double issueQty = 0;
          for (int j = 0; j < ultDetail.Rows.Count; j++)
          {
            UltraGridRow rowDetail = ultDetail.Rows[j];
            if (string.Compare(rowInfo.Cells["MaterialCode"].Value.ToString(), rowDetail.Cells["MaterialCode"].Value.ToString(), true) == 0)
            {
              if (DBConvert.ParseDouble(rowDetail.Cells["CBM"].Value.ToString()) != double.MinValue)
              {
                issueQty = issueQty + DBConvert.ParseDouble(rowDetail.Cells["CBM"].Value.ToString());
              }
            }
          }
          rowInfo.Cells["Issue"].Value = issueQty;
        }
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0105");
        return;
      }
    }

    /// <summary>
    /// Add Multi Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddMultiDetail_Click(object sender, EventArgs e)
    {
      viewWHD_21_006 uc = new viewWHD_21_006();
      Shared.Utility.WindowUtinity.ShowView(uc, "ADD MULTI DETAIL", true, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
      if (uc.dtDetail.Rows.Count > 0)
      {
        DataTable dtSource = new DataTable();
        dtSource.Columns.Add("LotNoId", typeof(System.String));

        if (uc.dtDetail != null && uc.dtDetail.Rows.Count > 0)
        {
          for (int i = 0; i < uc.dtDetail.Rows.Count; i++)
          {
            DataRow row = dtSource.NewRow();
            row["LotNoId"] = uc.dtDetail.Rows[i]["IDWood"].ToString();
            dtSource.Rows.Add(row);
          }
        }

        // Result Import Data
        DataTable dtResult = this.GetDataImport(dtSource);
        if (dtResult == null)
        {
          return;
        }
        DataTable dtDetail = (DataTable)ultDetail.DataSource;

        // 3: Check Trung(BarCode)
        foreach (DataRow dr in dtResult.Rows)
        {
          DataRow row = dtDetail.NewRow();
          string select = string.Empty;
          select = string.Format(@"LotNoId = '{0}'", dr["LotNoId"].ToString());
          DataRow[] foundRows = dtDetail.Select(select);
          if (foundRows.Length >= 1)
          {
            row["Error"] = 1;
          }
          else
          {
            row["Error"] = DBConvert.ParseInt(dr["Error"].ToString());
          }

          row["LotNoId"] = dr["LotNoId"].ToString();
          row["MaterialCode"] = dr["MaterialCode"].ToString();

          if (DBConvert.ParseLong(dr["DimensionPid"].ToString()) != long.MinValue)
          {
            row["DimensionPid"] = DBConvert.ParseLong(dr["DimensionPid"].ToString());
          }

          if (DBConvert.ParseDouble(dr["Length"].ToString()) != double.MinValue)
          {
            row["Length"] = DBConvert.ParseDouble(dr["Length"].ToString());
          }

          if (DBConvert.ParseDouble(dr["Width"].ToString()) != double.MinValue)
          {
            row["Width"] = DBConvert.ParseDouble(dr["Width"].ToString());
          }

          if (DBConvert.ParseDouble(dr["Thickness"].ToString()) != double.MinValue)
          {
            row["Thickness"] = DBConvert.ParseDouble(dr["Thickness"].ToString());
          }

          if (DBConvert.ParseLong(dr["PackagePid"].ToString()) != long.MinValue)
          {
            row["PackagePid"] = DBConvert.ParseLong(dr["PackagePid"].ToString());
          }

          if (dr["Package"].ToString().Trim().Length > 0)
          {
            row["Package"] = dr["Package"].ToString();
          }
          if (DBConvert.ParseLong(dr["LocationPid"].ToString()) != long.MinValue)
          {
            row["LocationPid"] = DBConvert.ParseLong(dr["LocationPid"].ToString());
          }

          if (dr["Location"].ToString().Trim().Length > 0)
          {
            row["Location"] = dr["Location"].ToString();
          }

          if (DBConvert.ParseDouble(dr["CBM"].ToString()) != double.MinValue)
          {
            row["CBM"] = DBConvert.ParseDouble(dr["CBM"].ToString());
          }
          dtDetail.Rows.Add(row);
        }

        // Add Detail
        for (int i = 0; i < ultDetail.Rows.Count; i++)
        {
          UltraGridRow row = ultDetail.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 1)
          {
            row.Appearance.BackColor = Color.Yellow;
          }
          else if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 2)
          {
            row.Appearance.BackColor = Color.YellowGreen;
          }
        }

        // Tinh IssueQty
        for (int i = 0; i < ultInfo.Rows.Count; i++)
        {
          UltraGridRow rowInfo = ultInfo.Rows[i];
          double issueQty = 0;
          for (int j = 0; j < ultDetail.Rows.Count; j++)
          {
            UltraGridRow rowDetail = ultDetail.Rows[j];
            if (string.Compare(rowInfo.Cells["MaterialCode"].Value.ToString(), rowDetail.Cells["MaterialCode"].Value.ToString(), true) == 0)
            {
              if (DBConvert.ParseDouble(rowDetail.Cells["CBM"].Value.ToString()) != double.MinValue)
              {
                issueQty = issueQty + DBConvert.ParseDouble(rowDetail.Cells["CBM"].Value.ToString());
              }
            }
          }
          rowInfo.Cells["Issue"].Value = Math.Round(issueQty, 3);

          if (DBConvert.ParseDouble(rowInfo.Cells["Issue"].Value.ToString()) > DBConvert.ParseDouble(rowInfo.Cells["QtyRequest"].Value.ToString()))
          {
            rowInfo.Cells["Issue"].Appearance.BackColor = Color.Yellow;
          }
          else
          {
            rowInfo.Cells["Issue"].Appearance.BackColor = Color.White;
          }
        }
      }
    }

    /// <summary>
    /// Save
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // 1: Check Valid
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      // 4: Load Data
      this.LoadData();
    }

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (this.pid == long.MinValue)
      {
        return;
      }
      if (WindowUtinity.ShowMessageConfirm("MSG0007", "Issue Request Online").ToString() == "No")
      {
        return;
      }
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spWHDWoodsIssuingRequestOnline_Delete", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        WindowUtinity.ShowMessageError("ERR0093", "Issue Request Online");
        return;
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0002");
      }
      this.btnDelete.Visible = false;
      this.CloseTab();
    }

    /// <summary>
    /// Brown
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      txtImportData.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    /// <summary>
    /// Load File Thong Tin(LotNoId)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLoad_Click(object sender, EventArgs e)
    {
      // Get path from Folder
      string path = @"\PhanmemDENSOBHT8000";
      path = Path.GetFullPath(path);

      // Create DataTable
      DataTable dtSource = new DataTable();
      dtSource.Columns.Add("LotNoId", typeof(System.String));

      string curFile = path + @"\THONGTIN.txt";

      if (!File.Exists(curFile))
      {
        string message = string.Format(DaiCo.Shared.Utility.FunctionUtility.GetMessage("ERR0011"), "THONGTIN.txt");
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      string[] a = File.ReadAllLines(curFile);

      if (a.Length == 0)
      {
        return;
      }

      int index = int.MinValue;
      if (a[0].ToString().Length > 0)
      {
        index = a[0].IndexOf("*");
      }

      for (int i = 0; i < a.Length; i++)
      {
        if (a[i].Trim().ToString().Length > 0 && index != -1)
        {
          DataRow row = dtSource.NewRow();
          index = a[i].IndexOf("*");
          a[i] = a[i].Substring(0, index).Trim().ToString();
          row["LotNoId"] = a[i].ToString().Trim();
          dtSource.Rows.Add(row);
        }
        else if (a[i].Trim().ToString().Length > 0)
        {
          DataRow row = dtSource.NewRow();
          row["LotNoId"] = a[i].ToString().Trim();
          dtSource.Rows.Add(row);
        }
      }

      // Result Import Data
      DataTable dtResult = this.GetDataImport(dtSource);
      if (dtResult == null)
      {
        return;
      }
      DataTable dtDetail = (DataTable)ultDetail.DataSource;

      // 3: Check Trung(BarCode)
      foreach (DataRow dr in dtResult.Rows)
      {
        DataRow row = dtDetail.NewRow();
        string select = string.Empty;
        select = string.Format(@"LotNoId = '{0}'", dr["LotNoId"].ToString());
        DataRow[] foundRows = dtDetail.Select(select);
        if (foundRows.Length >= 1)
        {
          row["Error"] = 1;
        }
        else
        {
          row["Error"] = DBConvert.ParseInt(dr["Error"].ToString());
        }

        row["LotNoId"] = dr["LotNoId"].ToString();
        row["MaterialCode"] = dr["MaterialCode"].ToString();

        if (DBConvert.ParseLong(dr["DimensionPid"].ToString()) != long.MinValue)
        {
          row["DimensionPid"] = DBConvert.ParseLong(dr["DimensionPid"].ToString());
        }

        if (DBConvert.ParseDouble(dr["Length"].ToString()) != double.MinValue)
        {
          row["Length"] = DBConvert.ParseDouble(dr["Length"].ToString());
        }

        if (DBConvert.ParseDouble(dr["Width"].ToString()) != double.MinValue)
        {
          row["Width"] = DBConvert.ParseDouble(dr["Width"].ToString());
        }

        if (DBConvert.ParseDouble(dr["Thickness"].ToString()) != double.MinValue)
        {
          row["Thickness"] = DBConvert.ParseDouble(dr["Thickness"].ToString());
        }

        if (DBConvert.ParseDouble(dr["LengthEXI"].ToString()) != double.MinValue)
        {
          row["LengthEXI"] = DBConvert.ParseDouble(dr["LengthEXI"].ToString());
        }

        if (DBConvert.ParseDouble(dr["WidthEXI"].ToString()) != double.MinValue)
        {
          row["WidthEXI"] = DBConvert.ParseDouble(dr["WidthEXI"].ToString());
        }

        if (DBConvert.ParseDouble(dr["ThicknessEXI"].ToString()) != double.MinValue)
        {
          row["ThicknessEXI"] = DBConvert.ParseDouble(dr["ThicknessEXI"].ToString());
        }

        if (DBConvert.ParseLong(dr["PackagePid"].ToString()) != long.MinValue)
        {
          row["PackagePid"] = DBConvert.ParseLong(dr["PackagePid"].ToString());
        }

        if (dr["Package"].ToString().Trim().Length > 0)
        {
          row["Package"] = dr["Package"].ToString();
        }

        if (DBConvert.ParseLong(dr["LocationPid"].ToString()) != long.MinValue)
        {
          row["LocationPid"] = DBConvert.ParseLong(dr["LocationPid"].ToString());
        }

        if (dr["Location"].ToString().Trim().Length > 0)
        {
          row["Location"] = dr["Location"].ToString();
        }

        if (DBConvert.ParseDouble(dr["CBM"].ToString()) != double.MinValue)
        {
          row["CBM"] = DBConvert.ParseDouble(dr["CBM"].ToString());
        }

        if (DBConvert.ParseDouble(dr["CBMEXI"].ToString()) != double.MinValue)
        {
          row["TotalCBMEXI"] = DBConvert.ParseDouble(dr["CBMEXI"].ToString());
        }

        dtDetail.Rows.Add(row);
      }

      // Add Detail
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultDetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 1)
        {
          row.Appearance.BackColor = Color.Yellow;
        }
        else if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 2)
        {
          row.Appearance.BackColor = Color.YellowGreen;
        }
      }

      // Tinh IssueQty
      for (int i = 0; i < ultInfo.Rows.Count; i++)
      {
        UltraGridRow rowInfo = ultInfo.Rows[i];
        double issueQty = 0;
        for (int j = 0; j < ultDetail.Rows.Count; j++)
        {
          UltraGridRow rowDetail = ultDetail.Rows[j];
          if (string.Compare(rowInfo.Cells["MaterialCode"].Value.ToString(), rowDetail.Cells["MaterialCode"].Value.ToString(), true) == 0)
          {
            if (DBConvert.ParseDouble(rowDetail.Cells["CBM"].Value.ToString()) != double.MinValue)
            {
              issueQty = issueQty + DBConvert.ParseDouble(rowDetail.Cells["CBM"].Value.ToString());
            }
          }
        }
        rowInfo.Cells["Issue"].Value = issueQty;
      }
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
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["QtyRequest"].Header.Caption = "QtyRequest(CBM)";
      e.Layout.Bands[0].Columns["Issue"].Header.Caption = "QtyIssue(CBM)";
      e.Layout.Bands[1].Columns["Qty"].Header.Caption = "Qty(CBM)";

      e.Layout.Bands[1].Columns["MaterialCode"].Hidden = true;
      e.Layout.Bands[0].Columns["QtyRequest"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Issue"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Change Department
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCBDepartment_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBDepartment.Value != null && ultCBDepartment.Text.Length > 0)
      {
        this.LoadComboReceiver(ultCBDepartment.Value.ToString());
      }
    }

    /// <summary>
    /// Init layout Excess
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultExcess_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.Bands[0].ColHeaderLines = 2;

      e.Layout.Bands[0].Columns["PID"].Hidden = true;
      e.Layout.Bands[0].Columns["Error"].Hidden = true;
      e.Layout.Bands[0].Columns["DimensionPid"].Hidden = true;
      e.Layout.Bands[0].Columns["PackagePid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationPid"].Hidden = true;
      e.Layout.Bands[0].Columns["DimensionPidEXI"].Hidden = true;

      e.Layout.Bands[0].Columns["Length"].Header.Caption = "Length \nUsed";
      e.Layout.Bands[0].Columns["Width"].Header.Caption = "Width \nUsed";
      e.Layout.Bands[0].Columns["Thickness"].Header.Caption = "Thickness \nUsed";

      e.Layout.Bands[0].Columns["LengthEXI"].Header.Caption = "Length \nPhysical";
      e.Layout.Bands[0].Columns["WidthEXI"].Header.Caption = "Width \nPhysical";
      e.Layout.Bands[0].Columns["ThicknessEXI"].Header.Caption = "Thickness \nPhysical";

      e.Layout.Bands[0].Columns["CBM"].Header.Caption = "Qty \nUsed (CBM)";
      e.Layout.Bands[0].Columns["CBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalCBMEXI"].Header.Caption = "Qty \nPhysical (CBM)";
      e.Layout.Bands[0].Columns["TotalCBMEXI"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LotNoId"].CellAppearance.BackColor = Color.LightBlue;

      for (int i = 3; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      if (this.isConfirm)
      {
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      }
      else
      {
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
        e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Expand
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkExpand_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpand.Checked)
      {
        ultInfo.Rows.ExpandAll(true);
      }
      else
      {
        ultInfo.Rows.CollapseAll(true);
      }
    }

    /// <summary>
    /// After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;

      switch (columnName)
      {
        case "LotNoId":
          // Check Data & Get Info LotNoId
          DBParameter[] input = new DBParameter[2];
          input[0] = new DBParameter("@WRNPid", DbType.Int64, DBConvert.ParseLong(ultCBRequestOnline.Value.ToString()));
          input[1] = new DBParameter("@LotNoId", DbType.String, row.Cells["LotNoId"].Value.ToString());
          DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHDWoodsGetDataFromLotNoId_Select", input);
          if (dtSource != null && dtSource.Rows.Count == 1)
          {
            if (DBConvert.ParseInt(dtSource.Rows[0]["Error"].ToString()) != int.MinValue)
            {
              row.Cells["Error"].Value = DBConvert.ParseInt(dtSource.Rows[0]["Error"].ToString());
              if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 0)
              {
                row.Appearance.BackColor = Color.White;
              }
              else if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 1)
              {
                row.Appearance.BackColor = Color.Yellow;
              }
              else if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 2)
              {
                row.Appearance.BackColor = Color.Yellow;
              }
            }
            row.Cells["MaterialCode"].Value = dtSource.Rows[0]["MaterialCode"].ToString();
            if (DBConvert.ParseLong(dtSource.Rows[0]["DimensionPid"].ToString()) != long.MinValue)
            {
              row.Cells["DimensionPid"].Value = DBConvert.ParseLong(dtSource.Rows[0]["DimensionPid"].ToString());
            }
            if (DBConvert.ParseDouble(dtSource.Rows[0]["Length"].ToString()) != double.MinValue)
            {
              row.Cells["Length"].Value = DBConvert.ParseDouble(dtSource.Rows[0]["Length"].ToString());
            }
            if (DBConvert.ParseDouble(dtSource.Rows[0]["Width"].ToString()) != double.MinValue)
            {
              row.Cells["Width"].Value = DBConvert.ParseDouble(dtSource.Rows[0]["Width"].ToString());
            }
            if (DBConvert.ParseDouble(dtSource.Rows[0]["Thickness"].ToString()) != double.MinValue)
            {
              row.Cells["Thickness"].Value = DBConvert.ParseDouble(dtSource.Rows[0]["Thickness"].ToString());
            }
            if (DBConvert.ParseLong(dtSource.Rows[0]["Package"].ToString()) != long.MinValue)
            {
              row.Cells["PackagePid"].Value = DBConvert.ParseLong(dtSource.Rows[0]["Package"].ToString());
            }
            row.Cells["Package"].Value = dtSource.Rows[0]["Package"].ToString();
            if (DBConvert.ParseLong(dtSource.Rows[0]["LocationPid"].ToString()) != long.MinValue)
            {
              row.Cells["LocationPid"].Value = DBConvert.ParseLong(dtSource.Rows[0]["LocationPid"].ToString());
            }
            row.Cells["Location"].Value = dtSource.Rows[0]["Location"].ToString();
            if (DBConvert.ParseDouble(dtSource.Rows[0]["CBM"].ToString()) != double.MinValue)
            {
              row.Cells["CBM"].Value = DBConvert.ParseDouble(dtSource.Rows[0]["CBM"].ToString());
            }

            // Tinh Total Issue
            for (int i = 0; i < ultInfo.Rows.Count; i++)
            {
              UltraGridRow rowInfo = ultInfo.Rows[i];
              if (string.Compare(rowInfo.Cells["MaterialCode"].Value.ToString(), row.Cells["MaterialCode"].Value.ToString(), true) == 0)
              {
                rowInfo.Cells["Issue"].Value = DBConvert.ParseDouble(rowInfo.Cells["Issue"].Value.ToString()) +
                                           DBConvert.ParseDouble(row.Cells["CBM"].Value.ToString());
              }
            }
          }
          else
          {
            row.Cells["Error"].Value = 1;
            row.Appearance.BackColor = Color.Yellow;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// After Row Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_AfterRowsDeleted(object sender, EventArgs e)
    {
      // Tinh IssueQty
      for (int i = 0; i < ultInfo.Rows.Count; i++)
      {
        UltraGridRow rowInfo = ultInfo.Rows[i];
        double issueQty = 0;
        for (int j = 0; j < ultDetail.Rows.Count; j++)
        {
          UltraGridRow rowDetail = ultDetail.Rows[j];
          if (string.Compare(rowInfo.Cells["MaterialCode"].Value.ToString(), rowDetail.Cells["MaterialCode"].Value.ToString(), true) == 0)
          {
            if (DBConvert.ParseDouble(rowDetail.Cells["CBM"].Value.ToString()) != double.MinValue)
            {
              issueQty = issueQty + DBConvert.ParseDouble(rowDetail.Cells["CBM"].Value.ToString());
            }
          }
        }
        rowInfo.Cells["Issue"].Value = issueQty;

        if (DBConvert.ParseDouble(rowInfo.Cells["Issue"].Value.ToString()) > DBConvert.ParseDouble(rowInfo.Cells["QtyRequest"].Value.ToString()))
        {
          rowInfo.Cells["Issue"].Appearance.BackColor = Color.Yellow;
        }
        else
        {
          rowInfo.Cells["Issue"].Appearance.BackColor = Color.White;
        }
      }
    }

    /// <summary>
    /// Before Row Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();

      switch (columnName)
      {
        case "LotNoId":
          // Check Trung LotNoId
          string select = string.Format(@"LotNoId = '{0}'", e.Cell.Row.Cells["LotNoId"].Text);
          DataTable dt = (DataTable)ultDetail.DataSource;
          DataRow[] foundRow = dt.Select(select);
          if (foundRow.Length >= 1)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0013"), "LotNoId");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
          }
          break;
      }
    }


    /// <summary>
    /// Delete Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      if (this.pid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseLong(row.Cells["PID"].Value.ToString()) > 0
                        && DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == 0)
          {
            DBParameter[] inputParams = new DBParameter[2];
            inputParams[0] = new DBParameter("@IssuingPid", DbType.Int64, this.pid);
            inputParams[1] = new DBParameter("@LotNoId", DbType.String, row.Cells["LotNoId"].Value.ToString());

            DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

            string storeName = string.Empty;
            storeName = "spWHDIssuingDetailWoods_Delete";

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
    }

    private void btnPreview_Click(object sender, EventArgs e)
    {
      viewWHD_99_001 view = new viewWHD_99_001();
      view.ncategory = 3;
      view.issuingNotePid = this.pid;
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

        DBParameter[] inputParam = new DBParameter[1];
        inputParam[0] = new DBParameter("@IssuingPid", DbType.Int64, this.pid);

        DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHDRPTWoodsIssuingRequestOnline_Select", inputParam);
        if (ds.Tables.Count > 0)
        {
          dsWHDIssuingRequestOnline dsSource = new dsWHDIssuingRequestOnline();
          dsSource.Tables["dtInfo"].Merge(ds.Tables[0]);
          dsSource.Tables["dtRequest"].Merge(ds.Tables[1]);
          dsSource.Tables["dtDetail"].Merge(ds.Tables[2]);

          double totalQty = 0;
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

          string printerName = FunctionUtility.GetDefaultPrinter();
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
    }

    #endregion Event
  }
}
