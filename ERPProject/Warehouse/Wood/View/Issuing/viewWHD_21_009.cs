/*
  Author      : Xuân Trường
  Date        : 18/04/2013
  Description : Issue By Paper
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
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_21_009 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    private bool isConfirm = false;
    private IList listDeletedPid = new ArrayList();
    private IList listDeletedPidByWOCarcass = new ArrayList();
    private int flag = 0;
    private int whPid = 3;
    #endregion Field

    #region Init

    public viewWHD_21_009()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_21_009_Load(object sender, EventArgs e)
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
    }

    /// <summary>
    /// Load Init
    /// </summary>
    private void LoadInit()
    {
      this.LoadWO();
      this.LoadComboDepartment();
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
    /// Load WO
    /// </summary>
    private void LoadWO()
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT WO ";
      commandText += " FROM TblPLNWoodsAllocateWorkOrderSummary  ";
      commandText += " WHERE IsCloseWork = 0  ";
      commandText += " ORDER BY WO   ";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDWO.DataSource = dtSource;
      ultDDWO.DisplayMember = "WO";
      ultDDWO.ValueMember = "WO";
      ultDDWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDWO.DisplayLayout.Bands[0].Columns["WO"].Width = 200;
    }

    /// <summary>
    /// Load Carcass
    /// </summary>
    private void LoadCarcass(long Wo)
    {
      string commandText = string.Empty;

      commandText += " SELECT DISTINCT CarcassCode ";
      commandText += " FROM TblPLNWoodsAllocateWorkOrderSummary  ";
      commandText += " WHERE IsCloseWork = 0 ";
      commandText += "    AND WO =" + Wo;
      commandText += " ORDER BY CarcassCode ";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultDDCarcass.DataSource = dtSource;
        ultDDCarcass.ValueMember = "CarcassCode";
        ultDDCarcass.DisplayMember = "CarcassCode";
        ultDDCarcass.DisplayLayout.Bands[0].Columns["CarcassCode"].Width = 200;
        ultDDCarcass.DisplayLayout.Bands[0].ColHeadersVisible = false;
      }
    }

    /// <summary>
    /// Load MaterialCode Theo (Allocation WO - Carcass)
    /// </summary>
    private void LoadMaterialCode(long wo, string carcass)
    {
      string commandText = string.Empty;
      commandText += " SELECT MAT.MaterialCode, MAT.MaterialNameEn NameVN, ROUND(ISNULL(SB.Qty, 0) - ISNULL(TOT.[Required], 0), 3) Qty ";
      commandText += " FROM TblPLNWoodsAllocateWorkOrderSummary SU ";
      commandText += " 	INNER JOIN VBOMMaterials MAT ON SU.[MainGroup] = SUBSTRING(MAT.MaterialCode, 1, 3) ";
      commandText += " 									AND SU.MainCategory = SUBSTRING(MAT.MaterialCode, 5, 2) ";
      commandText += " 	INNER JOIN VWHDWoodsStockBalance SB ON SB.MaterialCode = MAT.MaterialCode ";
      commandText += "  LEFT JOIN ";
      commandText += "  ( ";
      commandText += "      SELECT RED.MaterialCode, SUM(RED.Qty) [Required]  ";
      commandText += "      FROM TblGNRWoodsRequisitionNote RES  ";
      commandText += "        	INNER JOIN TblGNRWoodsRequisitionNoteDetail RED ON RES.Pid = RED.MRNPid ";
      commandText += "      WHERE (RES.[Status] <> 2) ";
      commandText += "      GROUP BY RED.MaterialCode ";
      commandText += "  ) TOT ON SB.MaterialCode = TOT.MaterialCode  ";
      commandText += " WHERE WO = " + wo;
      commandText += "  	AND CarcassCode = '" + carcass + "'";
      commandText += " 	  AND IsCloseWork = 0 ";
      commandText += " 	  AND SU.AltGroup IS NULL ";
      commandText += " ORDER BY MAT.MaterialCode ";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDMaterialCode.DataSource = dtSource;
      ultDDMaterialCode.DisplayMember = "MaterialCode";
      ultDDMaterialCode.ValueMember = "MaterialCode";
      ultDDMaterialCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDMaterialCode.DisplayLayout.Bands[0].Columns["MaterialCode"].Width = 200;
    }
    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.pid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDWoodsIssuePaper_Select", inputParam);
      if (dsSource != null)
      {
        // 1: Load Master
        DataTable dtInfo = dsSource.Tables[0];
        if (dtInfo.Rows.Count > 0)
        {
          DataRow row = dtInfo.Rows[0];
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
        ultInfo.DataSource = dsSource.Tables[1];
        for (int i = 0; i < ultInfo.Rows.Count; i++)
        {
          UltraGridRow row = ultInfo.Rows[i];
          if (row.Cells["Issue"].Value.ToString().Length > 0
              && DBConvert.ParseDouble(row.Cells["Issue"].Value.ToString()) >
                 DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()))
          {
            row.Cells["Issue"].Appearance.BackColor = Color.Yellow;
          }
        }

        ultDetail.DataSource = dsSource.Tables[2];

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
        ultCBReceiver.ReadOnly = true;
        txtRemark.ReadOnly = true;
        btnImport.Enabled = false;
        btnAddMultiDetail.Enabled = false;
        chkConfirm.Enabled = false;
        btnSave.Enabled = false;
        btnDelete.Enabled = false;
        chkConfirm.Checked = true;
      }
    }

    /// <summary>
    /// Get Data Import
    /// </summary>
    /// <param name="dtSource"></param>
    private DataTable GetDataImport(DataTable dtSource)
    {
      SqlDBParameter[] input = new SqlDBParameter[1];
      input[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtSource);
      DataTable dtMain = SqlDataBaseAccess.SearchStoreProcedureDataTable("spWHDWoodsGetDataIssuingPaper_Select", input);
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

      // 1: Check Receiver
      if (ultCBReceiver.Value == null)
      {
        message = "Receiver";
        return false;
      }

      // 2: Check Department Request
      if (ultCBDepartment.Value == null)
      {
        message = "Department Request";
        return false;
      }

      // 3: Check Qty <= Remain
      for (int i = 0; i < ultInfo.Rows.Count; i++)
      {
        UltraGridRow row = ultInfo.Rows[i];
        if (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) <= 0)
        {
          message = "Qty";
          return false;
        }
        if (DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString()) <= 0)
        {
          message = "Remain";
          return false;
        }
        if (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) > DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString()))
        {
          message = "Qty <= Remain";
          return false;
        }
      }

      // 4: Check Detail
      for (int j = 0; j < ultDetail.Rows.Count; j++)
      {
        UltraGridRow row = ultDetail.Rows[j];
        if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) != 0)
        {
          message = "Data";
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
      DBParameter[] inputParam = new DBParameter[9];
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
      inputParam[7] = new DBParameter("@Receiver", DbType.Int64, DBConvert.ParseLong(ultCBReceiver.Value.ToString()));
      inputParam[8] = new DBParameter("@Type", DbType.Int32, 7);

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
    /// Save Issuing Woods Detail 
    /// </summary>
    /// <returns></returns>
    private bool SaveIssuingDetailWoods()
    {
      // Delete Row
      foreach (long pidDelete in this.listDeletedPid)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pidDelete) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };

        DataBaseAccess.ExecuteStoreProcedure("spWHDWoodsIssuingDetail_Delete", inputDelete, outputDelete);
        long resultDelete = DBConvert.ParseLong(outputDelete[0].Value.ToString());
        if (resultDelete <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0004");
          return false;
        }
      }
      // End

      DataTable dtDetail = (DataTable)ultDetail.DataSource;
      DataTable dt = new DataTable();
      dt.Columns.Add("LotNoId", typeof(string));
      for (int i = 0; i < dtDetail.Rows.Count; i++)
      {
        DataRow rowDetail = dtDetail.Rows[i];
        if (rowDetail.RowState != DataRowState.Deleted
            && DBConvert.ParseLong(rowDetail["PID"].ToString()) == long.MinValue)
        {
          DataRow row = dt.NewRow();
          row["LotNoId"] = rowDetail["LotNoId"].ToString();
          dt.Rows.Add(row);
        }
      }

      SqlDBParameter[] input = new SqlDBParameter[2];
      input[0] = new SqlDBParameter("@IssuingPid", SqlDbType.BigInt, this.pid);
      input[1] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dt);

      SqlDBParameter[] output = new SqlDBParameter[1];
      output[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue);

      SqlDataBaseAccess.ExecuteStoreProcedure("spWHDWoodsIssuingPaperDetail_Insert", 500, input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// SaveData
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool success = true;

      // 1: Save Issuing
      success = this.SaveIssuing();
      if (!success)
      {
        return success;
      }

      // 2: Check Data
      string message = string.Empty;
      success = this.CheckData(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return success;
      }

      // 3: Update Data When Confirm
      this.UpdateDataWhenConfim();

      return success;
    }

    /// <summary>
    /// Save Issuing
    /// </summary>
    /// <returns></returns>
    private bool SaveIssuing()
    {
      bool success = this.SaveMaster();
      if (success)
      {
        success = this.SaveIssuingDetailWoods();
        if (success)
        {
          success = this.SaveIssuingDetailByWOCarcass();
        }
      }
      return success;
    }

    /// <summary>
    /// Delete Stock Balance When Confirmed
    /// Update IssueQty In Allocation WO - Carcass
    /// </summary>
    private void UpdateDataWhenConfim()
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@IssuingPid", DbType.Int64, this.pid);
      DataBaseAccess.ExecuteStoreProcedure("spWHDWoodsIssuingWhenConfirmed_Update", 500, input);
    }

    /// <summary>
    /// Check Data
    /// </summary>
    /// <returns></returns>
    private bool CheckData(out string message)
    {
      message = string.Empty;
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@IssuingPid", DbType.Int64, this.pid);
      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spWHDWoodsCheckIssuingDetailByWOCarcass", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result == 3)
      {
        message = "Sum(Qty) < Remain";
        return false;
      }
      else if (result == 2)
      {
        message = "Issue < Qty";
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save IssuingDetailByWOCarcass
    /// </summary>
    /// <returns></returns>
    private bool SaveIssuingDetailByWOCarcass()
    {
      // Delete Row
      foreach (long pidDelete in this.listDeletedPidByWOCarcass)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pidDelete) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };

        DataBaseAccess.ExecuteStoreProcedure("spWHDWoodsIssuingDetailByWOCarcass_Delete", inputDelete, outputDelete);
        long resultDelete = DBConvert.ParseLong(outputDelete[0].Value.ToString());
        if (resultDelete <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0004");
          return false;
        }
      }
      // End

      long result = long.MinValue;
      DataTable dtMain = (DataTable)ultInfo.DataSource;
      for (int i = 0; i < dtMain.Rows.Count; i++)
      {
        DataRow row = dtMain.Rows[i];
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] input = new DBParameter[8];
          if (DBConvert.ParseLong(row["Pid"].ToString()) != long.MinValue)
          {
            input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));
          }
          input[1] = new DBParameter("@IssuingPid", DbType.Int64, this.pid);
          input[2] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(row["WO"].ToString()));
          input[3] = new DBParameter("@Carcass", DbType.String, row["CarcassCode"].ToString());
          input[4] = new DBParameter("@Group", DbType.String, row["Group"].ToString());
          input[5] = new DBParameter("@Category", DbType.String, row["Category"].ToString());
          input[6] = new DBParameter("@MaterialCode", DbType.String, row["MaterialCode"].ToString());
          input[7] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row["Qty"].ToString()));

          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure("spWHDWoodsIssuingDetailByWOCarcass_Edit", input, output);

          result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result <= 0)
          {
            return false;
          }
        }
      }

      // Do Trang Issue
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@IssuingPid", DbType.Int64, this.pid);

      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spWHDWoodsIssuingDetailByWOCarcass_Update", 500, inputParam, outputParam);
      result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Import List IDWoods
    /// </summary>
    private void ImportData(DataTable dtSource)
    {
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        // Result Import Data
        DataTable dtResult = this.GetDataImport(dtSource);
        if (dtResult == null)
        {
          return;
        }
        DataTable dtDetail = (DataTable)ultDetail.DataSource;

        // Check Trung(LotNoId)
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
          else
          {
            // Kiem tra LotNoId co trong ds Material(DetailByWOCarcass)
            DataTable dtInfo = (DataTable)ultInfo.DataSource;
            string select = string.Empty;
            select = string.Format(@"MaterialCode = '{0}'", row.Cells["MaterialCode"].Value.ToString());
            DataRow[] foundRows = dtInfo.Select(select);
            if (foundRows.Length == 0)
            {
              row.Cells["Error"].Value = 1;
              row.Appearance.BackColor = Color.Yellow;
            }
          }
        }
      }
    }

    #endregion Function

    #region Event
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

        // Import Data
        this.ImportData(dtNew);
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
      Shared.Utility.WindowUtinity.ShowView(uc, "ADMULTI DETAIL", true, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);

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

      // Import Data
      this.ImportData(dtSource);
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
      if (WindowUtinity.ShowMessageConfirm("MSG0007", "Issue").ToString() == "No")
      {
        return;
      }
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spWHDWoodsIssuingByPaper_Delete", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        WindowUtinity.ShowMessageError("ERR0093", "Issue");
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

      // Import Data
      this.ImportData(dtSource);
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
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      Utility.SetPropertiesUltraGrid(ultInfo);

      e.Layout.Bands[0].Columns["WO"].ValueList = this.ultDDWO;
      e.Layout.Bands[0].Columns["CarcassCode"].ValueList = this.ultDDCarcass;
      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = this.ultDDMaterialCode;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Group"].Hidden = true;
      e.Layout.Bands[0].Columns["Category"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialNameEn"].Hidden = true;
      e.Layout.Bands[0].Columns["Unit"].Hidden = true;

      e.Layout.Bands[0].Columns["Balance"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["AllocatedQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Required"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["IssuedQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Issue"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["MaterialNameVn"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Balance"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["AllocatedQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Required"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remain"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["IssuedQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Issue"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["WO"].CellAppearance.BackColor = Color.Gainsboro;
      e.Layout.Bands[0].Columns["CarcassCode"].CellAppearance.BackColor = Color.Gainsboro;
      e.Layout.Bands[0].Columns["MaterialCode"].CellAppearance.BackColor = Color.Gainsboro;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.Gainsboro;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;

      if (this.isConfirm)
      {
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      }

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Issue"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.0000}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.0000}";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init layout Excess
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultExcess_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      Utility.SetPropertiesUltraGrid(ultDetail);

      e.Layout.Bands[0].Columns["Error"].Hidden = true;
      e.Layout.Bands[0].Columns["PID"].Hidden = true;
      e.Layout.Bands[0].Columns["DimensionPid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationPid"].Hidden = true;
      e.Layout.Bands[0].Columns["PackagePid"].Hidden = true;
      e.Layout.Bands[0].Columns["CBM"].Header.Caption = "Issue";

      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      if (this.isConfirm)
      {
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      }

      //Sum Qty, Total CBM And IssueQty
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["CBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.0000}";

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
        ultCBReceiver.Value = DBNull.Value;
        this.LoadComboReceiver(ultCBDepartment.Value.ToString());
      }
      else
      {
        ultCBReceiver.Value = DBNull.Value;
        this.LoadComboReceiver("");
      }
    }

    /// <summary>
    /// Delete Row
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

      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["PID"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    /// <summary>
    /// Before Cell Active
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultInfo_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string commandText = string.Empty;
      long wo = DBConvert.ParseLong(e.Cell.Row.Cells["WO"].Value.ToString());
      string carcass = e.Cell.Row.Cells["CarcassCode"].Value.ToString();

      if (e.Cell.Row.Cells.Exists("WO"))
      {
        // Load WO
        this.LoadCarcass(wo);
      }
      if (e.Cell.Row.Cells.Exists("WO") && e.Cell.Row.Cells.Exists("CarcassCode"))
      {
        // Load Material(Allocation WO - Carcass)
        this.LoadMaterialCode(wo, carcass);
      }
    }

    /// <summary>
    /// Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultInfo_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;
      string select = string.Empty;

      switch (columnName)
      {
        // Check Trung WO - CarcassCode - MaterialCode
        case "materialcode":
          select += "WO = " + DBConvert.ParseLong(row.Cells["WO"].Value.ToString()) + "";
          select += "AND CarcassCode = '" + row.Cells["CarcassCode"].Value.ToString() + "'";
          select += "AND MaterialCode = '" + row.Cells["MaterialCode"].Text + "'";
          DataTable dt = (DataTable)ultInfo.DataSource;
          DataRow[] foundRow = dt.Select(select);
          if (foundRow.Length >= 1)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0013"), "WO, CarcassCode, MaterialCode");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
          }
          break;
        case "qty":
          if (flag == 0)
          {
            if (DBConvert.ParseDouble(row.Cells["Qty"].Text) <= 0 ||
              (DBConvert.ParseDouble(row.Cells["Qty"].Text) > DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString())))
            {
              string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Qty");
              WindowUtinity.ShowMessageErrorFromText(message);
              e.Cancel = true;
            }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultInfo_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;
      string commandText = string.Empty;

      switch (columnName)
      {
        // WO
        case "wo":
          row.Cells["CarcassCode"].Value = DBNull.Value;
          row.Cells["MaterialCode"].Value = DBNull.Value;
          break;

        // Carcass
        case "carcasscode":
          row.Cells["MaterialCode"].Value = DBNull.Value;
          break;

        // Material
        case "materialcode":
          // 1: Lay Thong Tin Remain Allocation
          DBParameter[] input = new DBParameter[3];
          input[0] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(row.Cells["WO"].Value.ToString()));
          input[1] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CarcassCode"].Value.ToString());
          input[2] = new DBParameter("@MaterialCode", DbType.String, row.Cells["MaterialCode"].Value.ToString());

          DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDWoodsGetInfoAllocationWOCarcass_Select", input);
          if (dtData != null && dtData.Rows.Count > 0)
          {
            row.Cells["Group"].Value = dtData.Rows[0]["Group"].ToString();
            row.Cells["Category"].Value = dtData.Rows[0]["Category"].ToString();
            row.Cells["MaterialNameEn"].Value = dtData.Rows[0]["MaterialNameEn"].ToString();
            row.Cells["MaterialNameVn"].Value = dtData.Rows[0]["MaterialNameVn"].ToString();
            row.Cells["Unit"].Value = dtData.Rows[0]["Unit"].ToString();
            row.Cells["Balance"].Value = DBConvert.ParseDouble(dtData.Rows[0]["Balance"].ToString());
            row.Cells["AllocatedQty"].Value = DBConvert.ParseDouble(dtData.Rows[0]["AllocatedQty"].ToString());
            row.Cells["IssuedQty"].Value = DBConvert.ParseDouble(dtData.Rows[0]["IssuedQty"].ToString());
            row.Cells["Required"].Value = DBConvert.ParseDouble(dtData.Rows[0]["Required"].ToString());
            row.Cells["Remain"].Value = DBConvert.ParseDouble(dtData.Rows[0]["Remain"].ToString());
            this.flag = 1;
            row.Cells["Qty"].Value = DBNull.Value;
            this.flag = 0;
            row.Cells["Issue"].Value = DBNull.Value;
          }
          else
          {
            row.Cells["Group"].Value = DBNull.Value;
            row.Cells["Category"].Value = DBNull.Value;
            row.Cells["MaterialNameEn"].Value = DBNull.Value;
            row.Cells["MaterialNameVn"].Value = DBNull.Value;
            row.Cells["Unit"].Value = DBNull.Value;
            row.Cells["Balance"].Value = DBNull.Value;
            row.Cells["AllocatedQty"].Value = DBNull.Value;
            row.Cells["IssuedQty"].Value = DBNull.Value;
            row.Cells["Required"].Value = DBNull.Value;
            row.Cells["Remain"].Value = DBNull.Value;
            row.Cells["Qty"].Value = DBNull.Value;
            row.Cells["Issue"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
    }

    private void ultInfo_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["PID"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPidByWOCarcass.Add(pid);
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
