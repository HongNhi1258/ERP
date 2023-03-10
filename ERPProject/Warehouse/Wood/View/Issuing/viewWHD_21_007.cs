/*
  Author      : Xuân Trường
  Date        : 09/04/2013
  Description : Issue Requisition Special ID
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
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_21_007 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    private bool isConfirm = false;
    private int whPid = 3;
    #endregion Field

    #region Init

    public viewWHD_21_007()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_21_007_Load(object sender, EventArgs e)
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
      //this.LoadComboRequestNote();
      this.LoadComboDepartment();
    }

    /// <summary>
    /// Load Request Note
    /// </summary>
    private void LoadComboRequestNote()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, Code";
      commandText += " FROM TblGNRWoodsRequisitionSpecialID";
      commandText += " WHERE 1 = 1";
      if (this.isConfirm == false)
      {
        commandText += "AND [Status] = 1";
      }
      commandText += " ORDER BY Pid DESC";

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
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDWoodsIssueSpecialID_Select", inputParam);
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
        DataSet ds = this.CreateDataSet();
        ds.Tables["dtParent"].Merge(dsSource.Tables[1]);
        ds.Tables["dtChild"].Merge(dsSource.Tables[2]);

        ultData.DataSource = ds;
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];
          row.Appearance.BackColor = Color.Wheat;
          int select = 0;
          for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow rowChild = row.ChildBands[0].Rows[j];
            if (DBConvert.ParseInt(rowChild.Cells["Select"].Value.ToString()) == 1)
            {
              select = select + 1;
            }
            if (rowChild.Cells["IDWood"].Value.ToString().Length == 0)
            {
              rowChild.Appearance.BackColor = Color.Yellow;
            }
          }
          row.Cells["Issue"].Value = select;
        }

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
        ultExcess.Visible = false;
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
      taParent.Columns.Add("WO", typeof(System.Int32));
      taParent.Columns.Add("Carcass", typeof(System.String));
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("MaterialNameEn", typeof(System.String));
      taParent.Columns.Add("MaterialNameVn", typeof(System.String));
      taParent.Columns.Add("Unit", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.Int32));
      taParent.Columns.Add("Issue", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("RowIndex", typeof(System.Int64));
      taChild.Columns.Add("WO", typeof(System.Int32));
      taChild.Columns.Add("Carcass", typeof(System.String));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("IDWood", typeof(System.String));
      taChild.Columns.Add("LotNoId", typeof(System.String));
      taChild.Columns.Add("DimensionPid", typeof(System.Int64));
      taChild.Columns.Add("Length", typeof(System.Double));
      taChild.Columns.Add("Width", typeof(System.Double));
      taChild.Columns.Add("Thickness", typeof(System.Double));
      taChild.Columns.Add("TotalCBM", typeof(System.Double));
      taChild.Columns.Add("DimensionPidEXI", typeof(System.Int64));
      taChild.Columns.Add("LengthEXI", typeof(System.Double));
      taChild.Columns.Add("WidthEXI", typeof(System.Double));
      taChild.Columns.Add("ThicknessEXI", typeof(System.Double));
      taChild.Columns.Add("TotalCBMEXI", typeof(System.Double));
      taChild.Columns.Add("PackagePid", typeof(System.Int64));
      taChild.Columns.Add("Package", typeof(System.String));
      taChild.Columns.Add("LocationPid", typeof(System.Int64));
      taChild.Columns.Add("Location", typeof(System.String));
      taChild.Columns.Add("Select", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("taParent_taChild", new DataColumn[] { taParent.Columns["WO"], taParent.Columns["Carcass"], taParent.Columns["MaterialCode"] }, new DataColumn[] { taChild.Columns["WO"], taChild.Columns["Carcass"], taChild.Columns["MaterialCode"] }));
      return ds;
    }

    /// <summary>
    /// Load IDWoods
    /// </summary>
    /// <param name="dtSource"></param>
    private void LoadIDWoods(DataTable dtSource)
    {
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        DataTable dtExcess = new DataTable();
        dtExcess = (DataTable)ultExcess.DataSource;
        if (dtExcess == null)
        {
          dtExcess = new DataTable();
          dtExcess.Columns.Add("IDWood", typeof(System.String));
          dtExcess.Columns.Add("Length", typeof(System.Double));
          dtExcess.Columns.Add("Width", typeof(System.Double));
          dtExcess.Columns.Add("Thickness", typeof(System.Double));
          dtExcess.Columns.Add("LengthEXI", typeof(System.Double));
          dtExcess.Columns.Add("WidthEXI", typeof(System.Double));
          dtExcess.Columns.Add("ThicknessEXI", typeof(System.Double));
          dtExcess.Columns.Add("MaterialCode", typeof(System.String));
          dtExcess.Columns.Add("Location", typeof(System.String));
          dtExcess.Columns.Add("Package", typeof(System.String));
        }

        DataSet ds = (DataSet)ultData.DataSource;
        for (int i = 0; i < dtSource.Rows.Count; i++)
        {
          DataRow row = dtSource.Rows[i];
          if (row.ToString().Trim().Length > 0)
          {
            string select = string.Format(@"LotNoId = '{0}'", row["ID Woods"].ToString());
            DataRow[] foundRow = ds.Tables["dtChild"].Select(select);
            if (foundRow.Length == 1)
            {
              int rowIndex = DBConvert.ParseInt(foundRow[0]["RowIndex"].ToString());
              ds.Tables["dtChild"].Rows[rowIndex - 1]["IDWood"] = row["ID Woods"].ToString();
            }
            else
            {
              //Check Exist
              string select1 = string.Format(@"IDWood = '{0}'", row["ID Woods"].ToString());
              DataRow[] foundRow1 = dtExcess.Select(select1);
              if (foundRow1.Length == 0)
              {
                string commandText = string.Empty;
                commandText += " SELECT LotNoId, DIM.[Length], DIM.[Width], DIM.[Thickness], DIMEXI.[Length] [LengthEXI], DIMEXI.[Width] [WidthEXI], DIMEXI.[Thickness] [ThicknessEXI], TOC.MaterialCode, PAK.Package, PAK.Location ";
                commandText += " FROM TblWHDStockBalance TOC";
                commandText += "   LEFT JOIN TblWHDDimensionWoods DIM ON DIM.Pid = TOC.DimensionPid";
                commandText += "   LEFT JOIN TblWHDDimensionWoods DIMEXI ON DIMEXI.Pid = TOC.DimensionPidEXI";
                commandText += "   LEFT JOIN ";
                commandText += "   (";
                commandText += "     SELECT PAK.Pid, PAK.Name Package, LOC.Name Location";
                commandText += "     FROM TblWHDPackage PAK";
                commandText += "       INNER JOIN TblWHDPosition LOC ON LOC.Pid = PAK.PositionPid";
                commandText += "   )PAK ON PAK.Pid = TOC.LocationPid ";
                commandText += " WHERE LotNoId = '" + row["ID Woods"].ToString() + "'";

                DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
                if (dt == null)
                {
                  return;
                }
                DataRow rowExcess = dtExcess.NewRow();
                rowExcess["IDWood"] = row["ID Woods"].ToString();

                if (dt.Rows.Count > 0)
                {
                  if (DBConvert.ParseDouble(dt.Rows[0]["Length"].ToString()) != double.MinValue)
                  {
                    rowExcess["Length"] = DBConvert.ParseDouble(dt.Rows[0]["Length"].ToString());
                  }
                  if (DBConvert.ParseDouble(dt.Rows[0]["Width"].ToString()) != double.MinValue)
                  {
                    rowExcess["Width"] = DBConvert.ParseDouble(dt.Rows[0]["Width"].ToString());
                  }

                  if (DBConvert.ParseDouble(dt.Rows[0]["Thickness"].ToString()) != double.MinValue)
                  {
                    rowExcess["Thickness"] = DBConvert.ParseDouble(dt.Rows[0]["Thickness"].ToString());
                  }
                  if (DBConvert.ParseDouble(dt.Rows[0]["LengthEXI"].ToString()) != double.MinValue)
                  {
                    rowExcess["LengthEXI"] = DBConvert.ParseDouble(dt.Rows[0]["LengthEXI"].ToString());
                  }
                  if (DBConvert.ParseDouble(dt.Rows[0]["WidthEXI"].ToString()) != double.MinValue)
                  {
                    rowExcess["WidthEXI"] = DBConvert.ParseDouble(dt.Rows[0]["WidthEXI"].ToString());
                  }

                  if (DBConvert.ParseDouble(dt.Rows[0]["ThicknessEXI"].ToString()) != double.MinValue)
                  {
                    rowExcess["ThicknessEXI"] = DBConvert.ParseDouble(dt.Rows[0]["ThicknessEXI"].ToString());
                  }
                  rowExcess["MaterialCode"] = dt.Rows[0]["MaterialCode"].ToString();
                  rowExcess["Location"] = dt.Rows[0]["Location"].ToString();
                  rowExcess["Package"] = dt.Rows[0]["Package"].ToString();
                }
                dtExcess.Rows.Add(rowExcess);
              }
            }
          }
        }
        ultData.DataSource = ds;
        ultExcess.DataSource = dtExcess;

        // To Mau
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          int issue = 0;
          UltraGridRow rowParent = ultData.Rows[i];
          for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow rowChild = rowParent.ChildBands[0].Rows[j];
            if (rowChild.Cells["IDWood"].Value.ToString().Length == 0)
            {
              rowChild.Appearance.BackColor = Color.Yellow;
            }
            else
            {
              rowChild.Appearance.BackColor = Color.White;
              issue = issue + 1;
            }
          }
          rowParent.Cells["Issue"].Value = issue;
        }
      }
    }

    /// <summary>
    /// Check Data
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      // Check Requisition Note Issued
      string commandText = string.Empty;
      commandText = "SELECT * ";
      commandText += "FROM TblWHDIssuing ";
      commandText += "WHERE WRNPid = " + DBConvert.ParseLong(ultCBRequestOnline.Value.ToString()) + "";
      if (this.pid != long.MinValue)
      {
        commandText += " AND PID <> " + this.pid + "";
      }
      commandText += " AND Type = 5 ";
      DataTable dtRequest = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtRequest != null && dtRequest.Rows.Count > 0)
      {
        message = "Requisition Note Was Issued";
        return false;
      }

      // Check Receiver
      if (ultCBReceiver.Value == null)
      {
        message = "Receiver";
        return false;
      }

      // Check Detail
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowInfo = ultData.Rows[i];
        for (int j = 0; j < rowInfo.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = rowInfo.ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(rowChild.Cells["Select"].Value.ToString()) == 1
            && rowChild.Cells["IDWood"].Value.ToString().Length == 0)
          {
            message = "IDWood";
            return false;
          }
        }
      }

      // Check Excess
      if (ultExcess.Rows.Count > 0)
      {
        message = "IDWood Excess";
        return false;
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
      inputParam[9] = new DBParameter("@Type", DbType.Int32, 5);

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
    /// Save Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = row.ChildBands[0].Rows[j];
          DBParameter[] input = new DBParameter[10];
          if (DBConvert.ParseLong(rowChild.Cells["Pid"].Value.ToString()) != long.MinValue)
          {
            input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(rowChild.Cells["Pid"].Value.ToString()));
          }
          input[1] = new DBParameter("@IssuingNotePid", DbType.Int64, this.pid);
          input[2] = new DBParameter("@LotNoId", DbType.String, rowChild.Cells["LotNoId"].Value.ToString());
          input[3] = new DBParameter("@MaterialCode", DbType.String, rowChild.Cells["MaterialCode"].Value.ToString());
          input[4] = new DBParameter("@DimensionPid", DbType.Double, DBConvert.ParseDouble(rowChild.Cells["DimensionPid"].Value.ToString()));
          input[5] = new DBParameter("@DimensionPidEXI", DbType.Int64, DBConvert.ParseDouble(rowChild.Cells["DimensionPidEXI"].Value.ToString()));
          input[6] = new DBParameter("@LocationPid", DbType.Int64, DBConvert.ParseLong(rowChild.Cells["LocationPid"].Value.ToString()));
          input[7] = new DBParameter("@PackagePid", DbType.Int64, DBConvert.ParseLong(rowChild.Cells["PackagePid"].Value.ToString()));
          input[8] = new DBParameter("@TotalCBM", DbType.Double, DBConvert.ParseDouble(rowChild.Cells["TotalCBM"].Value.ToString()));
          input[9] = new DBParameter("@Select", DbType.Int32, DBConvert.ParseInt(rowChild.Cells["Select"].Value.ToString()));

          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure("spWHDWoodsIssuingDetail_Edit", input, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result <= 0)
          {
            return false;
          }
        }
      }

      // 1: Update Status Requisition Special ID
      DBParameter[] inputUpdate = new DBParameter[1];
      inputUpdate[0] = new DBParameter("@Pid", DbType.Int64, this.pid);

      DBParameter[] outputUpdate = new DBParameter[1];
      outputUpdate[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure("spWHDWoodsRequisitionSpecialIDStatus_Update", 500, inputUpdate, outputUpdate);
      if (DBConvert.ParseLong(outputUpdate[0].Value.ToString()) <= 0)
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

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHDWoodsGetInfoRequisitionSpecialID_Select", input);
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

      // 2: Detail
      DataSet dsMain = this.CreateDataSet();
      dsMain.Tables["dtParent"].Merge(dtDetailParent);
      dsMain.Tables["dtChild"].Merge(dtDetailChild);
      ultData.DataSource = dsMain;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        row.Appearance.BackColor = Color.Wheat;
        for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = row.ChildBands[0].Rows[j];
          rowChild.Cells["Select"].Value = 1;
        }
      }
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

        // Load IDWoods
        this.LoadIDWoods(dtSource);
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
      dtSource.Columns.Add("ID Woods", typeof(System.String));

      if (uc.dtDetail != null && uc.dtDetail.Rows.Count > 0)
      {
        for (int i = 0; i < uc.dtDetail.Rows.Count; i++)
        {
          DataRow row = dtSource.NewRow();
          row["ID Woods"] = uc.dtDetail.Rows[i]["IDWood"].ToString();
          dtSource.Rows.Add(row);
        }
      }

      // Load IDWoods
      this.LoadIDWoods(dtSource);
    }

    /// <summary>
    /// Save
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.ultCBRequestOnline.Value == null
          || DBConvert.ParseLong(this.ultCBRequestOnline.Value.ToString()) == long.MinValue)
      {
        return;
      }

      string message = string.Empty;
      // 1: Check Valid
      // 1: Check Valid
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      // 2: Save Master
      success = this.SaveMaster();
      if (success)
      {
        // Save Detail
        success = this.SaveDetail();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0037", "Data");
        }
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
      if (WindowUtinity.ShowMessageConfirm("MSG0007", "Issue Special ID").ToString() == "No")
      {
        return;
      }
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spWHDWoodsIssuingSpecialID_Delete", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        WindowUtinity.ShowMessageError("ERR0093", "Issue Special ID");
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

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Issue"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["RowIndex"].Hidden = true;
      e.Layout.Bands[1].Columns["WO"].Hidden = true;
      e.Layout.Bands[1].Columns["Carcass"].Hidden = true;
      e.Layout.Bands[1].Columns["MaterialCode"].Hidden = true;
      e.Layout.Bands[1].Columns["DimensionPid"].Hidden = true;
      e.Layout.Bands[1].Columns["DimensionPidEXI"].Hidden = true;
      e.Layout.Bands[1].Columns["LocationPid"].Hidden = true;
      e.Layout.Bands[1].Columns["PackagePid"].Hidden = true;

      e.Layout.Bands[1].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["TotalCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["LengthEXI"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["WidthEXI"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["ThicknessEXI"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["TotalCBMEXI"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[1].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["Select"].DefaultCellValue = 1;

      for (int i = 0; i < e.Layout.Bands[1].Columns.Count - 1; i++)
      {
        e.Layout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Bands[1].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
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

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;

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
        ultData.Rows.ExpandAll(true);
      }
      else
      {
        ultData.Rows.CollapseAll(true);
      }
    }

    /// <summary>
    /// Load File ThongTin.txt
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
      dtSource.Columns.Add("ID Woods", typeof(System.String));

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
          row["ID Woods"] = a[i].ToString().Trim();
          dtSource.Rows.Add(row);
        }
        else if (a[i].Trim().ToString().Length > 0)
        {
          DataRow row = dtSource.NewRow();
          row["ID Woods"] = a[i].ToString().Trim();
          dtSource.Rows.Add(row);
        }
      }

      // Load IDWoods
      this.LoadIDWoods(dtSource);
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

    private void btnPrintPreview_Click(object sender, EventArgs e)
    {
      viewWHD_99_001 view = new viewWHD_99_001();
      view.ncategory = 3;
      view.issuingNotePid = this.pid;
      Shared.Utility.WindowUtinity.ShowView(view, "Report", false, Shared.Utility.ViewState.ModalWindow);
    }

    #endregion Event
  }
}
