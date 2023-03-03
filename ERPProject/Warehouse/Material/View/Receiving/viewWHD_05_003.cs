/*
  Author      : Dang Xuan Truong
  Date        : 04/06/2012
  Description : Receiving Return From Production For Material (03RTW)
*/
using CrystalDecisions.CrystalReports.Engine;
using DaiCo.Application;
using DaiCo.ERPProject.Warehouse.Material.DataSetSource;
using DaiCo.ERPProject.Warehouse.Material.Reports;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_05_003 : MainUserControl
  {
    #region Field
    private int whPid = 0;
    //RTW
    public string RTWNo = string.Empty;

    private string strPathOutputFile = string.Empty;
    private string strPathTemplate = string.Empty;
    private string pathExport = string.Empty;
    // Store
    string SP_WHDImportDataFromExcel = "spWHDGetDataImportReceivingSpecialMaterial_Select";
    string SP_WHDRTWInfomation_Insert = "spWHDRTWInfomation_Insert";
    string SP_WHDRTWInfomation_Update = "spWHDRTWInfomation_Update";
    string SP_WHDRTWDetail_Edit = "spWHDRTWDetail_Edit";
    string SP_WHDRTWInfomation_Select = "spWHDRTWInfomation_Select";
    string SP_WHDRTWInfomation_Delete = "spWHDRTWInfomation_Delete";
    string SP_WHDRTWDetail_Delete = "spWHDRTWDetail_Delete";
    string SP_WHDRTWStockBalance_Update = "spWHDRTWStockBalance_Update";
    // Status
    int status = int.MinValue;
    // Delete
    private IList listDeletingDetailPid = new ArrayList();
    private IList listDeletedDetailPid = new ArrayList();

    #endregion Field

    #region Init
    public viewWHD_05_003()
    {
      InitializeComponent();
    }

    private void viewWHD_05_003_Load(object sender, EventArgs e)
    {
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      this.strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      this.strPathTemplate = strStartupPath + @"\ExcelTemplate"; ;

      Utility.LoadUltraComboDepartment(ultCBDepartment);
      Utility.LoadUltraCBMaterialWHListByUser(ucbWarehouse);
      this.LoadData();
    }
    #endregion Init

    #region Function
    /// <summary>
    ///  Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@RTWNo", DbType.AnsiString, 50, this.RTWNo) };
      DataSet dsMain = DataBaseAccess.SearchStoreProcedure(SP_WHDRTWInfomation_Select, inputParam);
      DataTable dtRTWInfo = dsMain.Tables[0];
      if (dtRTWInfo.Rows.Count > 0)
      {
        DataRow row = dtRTWInfo.Rows[0];
        txtReceivingNote.Text = this.RTWNo;
        this.whPid = DBConvert.ParseInt(dtRTWInfo.Rows[0]["WHPid"]);
        ucbWarehouse.Value = this.whPid;
        txtTitle.Text = dtRTWInfo.Rows[0]["Title"].ToString();
        txtCreateBy.Text = dtRTWInfo.Rows[0]["CreateBy"].ToString();
        txtCreateDate.Text = dtRTWInfo.Rows[0]["CreateDate"].ToString();
        ultCBDepartment.Value = dtRTWInfo.Rows[0]["Department"].ToString();
        txtReason.Text = dtRTWInfo.Rows[0]["Reason"].ToString();
        this.status = DBConvert.ParseInt(dtRTWInfo.Rows[0]["Posting"].ToString());
      }
      else
      {
        DataTable dtRTW = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHDGetNewReceivingNoForMaterial('03RTW') NewRTWCode");
        if ((dtRTW != null) && (dtRTW.Rows.Count > 0))
        {
          this.txtReceivingNote.Text = dtRTW.Rows[0]["NewRTWCode"].ToString();
          this.txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
          this.txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          this.status = 0;
        }

        // Load default WH
        if (ucbWarehouse.Rows.Count > 0)
        {
          ucbWarehouse.Rows[0].Selected = true;
        }
      }
      // Load Detail
      ultData.DataSource = dsMain.Tables[1];

      bool isReadOnly = (ultData.Rows.Count > 0 ? true : false);
      ucbWarehouse.ReadOnly = isReadOnly;

      // Set Status control
      this.SetStatusControl();
    }

    /// <summary>
    /// Set Control
    /// </summary>
    private void SetStatusControl()
    {
      if (this.status == 1)
      {
        txtTitle.ReadOnly = true;
        ultCBDepartment.ReadOnly = true;
        txtReason.ReadOnly = true;
        btnImport.Enabled = false;
        chkConfirmed.Checked = true;
        chkConfirmed.Enabled = false;
        btnDelete.Enabled = false;
        btnSave.Enabled = false;
        ucbWarehouse.ReadOnly = true;
      }
    }

    /// <summary>
    /// Import Data From Excel
    /// </summary>
    private void ImportDataFromExcel()
    {
      if (this.txtImportExcel.Text.Trim().Length == 0 || this.whPid <= 0)
      {
        return;
      }
      try
      {
        DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B4:D55]").Tables[0];
        if (dtSource == null)
        {
          return;
        }
        foreach (DataRow drSource in dtSource.Rows)
        {
          if (drSource["MaterialCode"].ToString().Trim().Length > 0)
          {
            if (DBConvert.ParseDouble(drSource["Qty"].ToString()) == double.MinValue)
            {
              drSource["Qty"] = 0;
            }
          }
        }
        // Create DataTable
        DataTable dtNew = this.CreateDataTableBeforeSave();
        foreach (DataRow drMain in dtSource.Rows)
        {
          if (drMain["MaterialCode"].ToString().Trim().Length > 0)
          {
            DataRow row = dtNew.NewRow();
            row["MaterialCode"] = drMain["MaterialCode"].ToString();
            row["Location"] = drMain["Location"].ToString();
            row["Qty"] = DBConvert.ParseDouble(drMain["Qty"].ToString());
            row["WHPid"] = this.whPid;
            dtNew.Rows.Add(row);
          }
        }
        SqlDBParameter[] inputParam = new SqlDBParameter[1];
        inputParam[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtNew);
        DataSet dsResult = SqlDataBaseAccess.SearchStoreProcedure(SP_WHDImportDataFromExcel, inputParam);
        // Get DataTable Import
        DataTable result = dsResult.Tables[0];
        if (result == null)
        {
          return;
        }
        DataTable dtMain = (DataTable)this.ultData.DataSource;
        foreach (DataRow dr in result.Rows)
        {
          DataRow row = dtMain.NewRow();
          row["Errors"] = DBConvert.ParseInt(dr["Errors"].ToString());
          row["MaterialCode"] = dr["MaterialCode"].ToString();
          row["NameEN"] = dr["NameEN"].ToString();
          row["NameVN"] = dr["NameVN"].ToString();
          row["Unit"] = dr["Unit"].ToString();
          if (dr["Location"].ToString().Length > 0)
          {
            row["Location"] = dr["Location"].ToString();
          }
          row["Qty"] = DBConvert.ParseDouble(dr["Qty"].ToString());
          row["Flag"] = 1;
          dtMain.Rows.Add(row);
        }
        this.ultData.DataSource = dtMain;
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 1)
          {
            row.CellAppearance.BackColor = Color.Yellow;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 2)
          {
            row.CellAppearance.BackColor = Color.Lime;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 3)
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
    /// Create DataTable Before Save
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTableBeforeSave()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("MaterialCode", typeof(System.String));
      dt.Columns.Add("Location", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Double));
      dt.Columns.Add("WHPid", typeof(System.Int32));
      return dt;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      // Save Info
      bool success = this.SaveRTWInfo();
      if (!success)
      {
        return false;
      }
      // Save Detail
      success = this.SaveRTWDetail();
      if (!success)
      {
        return false;
      }
      // Update StockBalance
      success = this.UpdateStockBalance();
      if (!success)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckInvalid(out string message)
    {
      message = string.Empty;
      if (ucbWarehouse.SelectedRow == null)
      {
        message = "Warehouse";
        return false;
      }
      if (ultCBDepartment.Value == null)
      {
        message = "Department";
        return false;
      }
      // Create new receiving note
      if (this.RTWNo.Length == 0)
      {
        // Check WH Summary of preMonth
        if (!Utility.CheckWHPreMonthSummary(this.whPid))
        {
          return false;
        }
      }
      // Check Detail
      DataTable dtMaterial = (DataTable)ucbMaterialList.DataSource;
      DataTable dtLocation = (DataTable)ucbLocation.DataSource;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) != 0)
        {
          message = "Data Input";
          return false;
        }
        //Material
        string materialCode = row.Cells["MaterialCode"].Value.ToString();
        DataRow[] materialRows = dtMaterial.Select(string.Format("MaterialCode = '{0}'", materialCode));
        if (materialRows.Length == 0)
        {
          message = "Material";
          row.Cells["Errors"].Value = 1;
          row.CellAppearance.BackColor = Color.Yellow;
          return false;
        }
        //Qty
        if (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) <= 0)
        {
          message = "Qty";
          row.Cells["Errors"].Value = 3;
          row.CellAppearance.BackColor = Color.Magenta;
          return false;
        }
        //Location
        int iLocation = DBConvert.ParseInt(row.Cells["Location"].Value);
        DataRow[] locationRows = dtLocation.Select(string.Format("Pid = {0}", iLocation));
        if (locationRows.Length == 0)
        {
          message = "Location";
          row.Cells["Errors"].Value = 2;
          row.CellAppearance.BackColor = Color.Lime;
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Info 
    /// </summary>
    /// <returns></returns>
    private bool SaveRTWInfo()
    {
      if (this.RTWNo == string.Empty)
      {
        DBParameter[] inputParam = new DBParameter[8];
        inputParam[0] = new DBParameter("@RTWNo", DbType.AnsiString, 50, "03RTW");
        inputParam[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

        if (txtTitle.Text.Length > 0)
        {
          inputParam[2] = new DBParameter("@Title", DbType.String, 4000, txtTitle.Text);
        }
        if (chkConfirmed.Checked)
        {
          inputParam[3] = new DBParameter("@Posting", DbType.Int32, 1);
        }
        else
        {
          inputParam[3] = new DBParameter("@Posting", DbType.Int32, 0);
        }
        inputParam[4] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);
        inputParam[5] = new DBParameter("@Type", DbType.Int32, 2);
        inputParam[6] = new DBParameter("@Department", DbType.String, 50, ultCBDepartment.Value.ToString());
        inputParam[7] = new DBParameter("@Reason", DbType.String, 255, txtReason.Text);

        DBParameter[] outputParam = new DBParameter[2];
        outputParam[0] = new DBParameter("@ResultRTWNo", DbType.AnsiString, 50, string.Empty);
        outputParam[1] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(SP_WHDRTWInfomation_Insert, inputParam, outputParam);
        this.RTWNo = outputParam[0].Value.ToString();
        long result = DBConvert.ParseLong(outputParam[1].Value.ToString());
        if (result == 0)
        {
          return false;
        }
        return true;
      }
      else
      {
        DBParameter[] inputParam = new DBParameter[7];
        inputParam[0] = new DBParameter("@RTWNo", DbType.AnsiString, 50, this.RTWNo);
        inputParam[1] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

        if (txtTitle.Text.Length > 0)
        {
          inputParam[2] = new DBParameter("@Title", DbType.String, 4000, txtTitle.Text);
        }
        if (chkConfirmed.Checked)
        {
          inputParam[3] = new DBParameter("@Posting", DbType.Int32, 1);
        }
        else
        {
          inputParam[3] = new DBParameter("@Posting", DbType.Int32, 0);
        }
        inputParam[4] = new DBParameter("@Department", DbType.String, 50, ultCBDepartment.Value.ToString());
        inputParam[5] = new DBParameter("@Reason", DbType.String, 255, txtReason.Text);
        inputParam[6] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);

        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(SP_WHDRTWInfomation_Update, inputParam, outputParam);
        long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result == 0)
        {
          return false;
        }
        return true;
      }
    }

    /// <summary>
    /// Save Info Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveRTWDetail()
    {
      // Delete Row
      foreach (long rtwDetailPid in this.listDeletedDetailPid)
      {
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@RTWDetailPid", DbType.Int64, rtwDetailPid);
        DBParameter[] output = new DBParameter[1];

        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure(SP_WHDRTWDetail_Delete, input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result == 0)
        {
          return false;
        }
      }
      this.listDeletedDetailPid.Clear();
      // Insert & Update Row
      if (ultData.Rows.Count > 0)
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow rowInfo = ultData.Rows[i];
          if (DBConvert.ParseInt(rowInfo.Cells["Flag"].Value.ToString()) == 1)
          {
            DBParameter[] inputParam = new DBParameter[7];

            if (DBConvert.ParseLong(rowInfo.Cells["RTWDetailPid"].Value.ToString()) != long.MinValue)
            {
              inputParam[0] = new DBParameter("@RTWDetailPid", DbType.Int64, DBConvert.ParseLong(rowInfo.Cells["RTWDetailPid"].Value.ToString()));
            }
            inputParam[1] = new DBParameter("@RTWNo", DbType.AnsiString, 50, this.RTWNo);
            inputParam[2] = new DBParameter("@MaterialCode", DbType.AnsiString, 255, rowInfo.Cells["MaterialCode"].Value.ToString());
            inputParam[3] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(rowInfo.Cells["Qty"].Value.ToString()));
            inputParam[4] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);
            inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            inputParam[6] = new DBParameter("@Location", DbType.Int32, DBConvert.ParseInt(rowInfo.Cells["Location"].Value.ToString()));

            DBParameter[] outputParam = new DBParameter[1];
            outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

            DataBaseAccess.ExecuteStoreProcedure(SP_WHDRTWDetail_Edit, inputParam, outputParam);
            long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
            if (result == 0)
            {
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Update Stock Balance
    /// </summary>
    /// <returns></returns>
    private bool UpdateStockBalance()
    {
      if (chkConfirmed.Checked)
      {
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@IDPhieuNhap", DbType.AnsiString, 50, this.RTWNo);
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(SP_WHDRTWStockBalance_Update, input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result == 0)
        {
          return false;
        }
      }
      return true;
    }
    /// <summary>
    /// Check File Is Exist. If Exist Then Delete And Create File Else Create File
    /// Create by: Qui_VVD (18/10/2010 16:10)
    /// </summary>
    private void InitializeOutputDirectory()
    {
      if (Directory.Exists(this.strPathOutputFile))
      {
        string[] files = Directory.GetFiles(this.strPathOutputFile);
        foreach (string file in files)
        {
          try
          {
            File.Delete(file);
          }
          catch { }
        }
      }
      else
      {
        Directory.CreateDirectory(this.strPathOutputFile);
      }
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      e.Cell.Row.Cells["Flag"].Value = 1;
      switch (columnName)
      {
        case "materialcode":
          if (e.Cell.Row.Cells["MaterialCode"].Value.ToString().Length > 0)
          {
            string materialCode = e.Cell.Row.Cells["MaterialCode"].Value.ToString();
            string commadText = string.Format(@"SELECT MAT.NameEN, MAT.NameVN, UNIT.Symbol Unit
                                                FROM TblGNRMaterialInformation  MAT
	                                              INNER JOIN TblGNRMaterialUnit UNIT ON MAT.Unit = UNIT.Pid 
                                                                                  AND MAT.MaterialCode = '{0}'", materialCode);
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commadText);
            if (dt != null && dt.Rows.Count > 0)
            {
              e.Cell.Row.Cells["NameEN"].Value = dt.Rows[0]["NameEN"].ToString();
              e.Cell.Row.Cells["NameVN"].Value = dt.Rows[0]["NameVN"].ToString();
              e.Cell.Row.Cells["Unit"].Value = dt.Rows[0]["Unit"].ToString();
              e.Cell.Row.Cells["Errors"].Value = 0;
              e.Cell.Row.CellAppearance.BackColor = Color.White;
            }
            else
            {
              e.Cell.Row.Cells["NameEN"].Value = "";
              e.Cell.Row.Cells["NameVN"].Value = "";
              e.Cell.Row.Cells["Unit"].Value = "";
              e.Cell.Row.Cells["Errors"].Value = 1;
              e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
            }
          }
          break;
        case "qty":
          if (DBConvert.ParseInt(e.Cell.Row.Cells["Errors"].Value.ToString()) == 3)
          {
            e.Cell.Row.Cells["Errors"].Value = 0;
            e.Cell.Row.CellAppearance.BackColor = Color.White;
          }
          break;
        case "location":
          if (DBConvert.ParseInt(e.Cell.Row.Cells["Errors"].Value.ToString()) == 2)
          {
            e.Cell.Row.Cells["Errors"].Value = 0;
            e.Cell.Row.CellAppearance.BackColor = Color.White;
          }
          break;
        default:
          break;
      }

      // Read Only warehouse list
      bool isReadOnly = (ultData.Rows.Count > 0 ? true : false);
      ucbWarehouse.ReadOnly = isReadOnly;
    }

    /// <summary>
    /// Format Export File
    /// </summary>
    /// <param name="strTemplateName"></param>
    /// <param name="strSheetName"></param>
    /// <param name="strPreOutFileName"></param>
    /// <param name="strOutFileName"></param>
    /// <returns></returns>
    private XlsReport InitializeXlsReport(string strTemplateName, string strSheetName, string strPreOutFileName, out string strOutFileName)
    {
      IContainer components = new Container();
      XlsReport oXlsReport = new XlsReport(components);
      this.InitializeOutputDirectory();
      strTemplateName = string.Format(@"{0}\{1}.xls", this.strPathTemplate, strTemplateName);
      oXlsReport.FileName = strTemplateName;
      oXlsReport.Start.File();
      oXlsReport.Page.Begin(strSheetName, "1");
      string time = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second;
      strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xls", this.strPathOutputFile, strPreOutFileName, DateTime.Now.ToString("yyyyMMdd"), time);
      return oXlsReport;
    }
    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;
      e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["RTWDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Flag"].Hidden = true;
      e.Layout.Bands[0].Columns["Errors"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = ucbMaterialList;
      e.Layout.Bands[0].Columns["MaterialCode"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["MaterialCode"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material";
      e.Layout.Bands[0].Columns["Location"].ValueList = ucbLocation;
      e.Layout.Bands[0].Columns["Location"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["Location"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "English Name";
      e.Layout.Bands[0].Columns["NameVn"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameVn"].Hidden = true;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Location"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["Location"].MinWidth = 150;

      if (this.status == 1)
      {
        e.Layout.Override.AllowAddNew = AllowAddNew.No;
        e.Layout.Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Override.AllowUpdate = DefaultableBoolean.False;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      DBParameter[] input = new DBParameter[] { new DBParameter("@ReceivingNote", DbType.AnsiString, 48, txtReceivingNote.Text.Trim()) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDRPTReceivingNotePrint_Materials", input);
      dsWHDRPTMaterialsReceivingNote dsReceiving = new dsWHDRPTMaterialsReceivingNote();
      dsReceiving.Tables["dtReceivingInfo"].Merge(dsSource.Tables[0]);
      dsReceiving.Tables["dtReceivingDetail"].Merge(dsSource.Tables[1]);
      double totalQty = 0;
      for (int i = 0; i < dsReceiving.Tables["dtReceivingDetail"].Rows.Count; i++)
      {
        if (DBConvert.ParseDouble(dsReceiving.Tables["dtReceivingDetail"].Rows[i]["Qty"].ToString()) != double.MinValue)
        {
          totalQty = totalQty + DBConvert.ParseDouble(dsReceiving.Tables["dtReceivingDetail"].Rows[i]["Qty"].ToString());
        }
      }

      ReportClass cpt = null;
      DaiCo.Shared.View_Report report = null;

      cpt = new cptMaterialsReceivingNote();
      cpt.SetDataSource(dsReceiving);
      cpt.SetParameterValue("TotalQty", totalQty);
      cpt.SetParameterValue("Title", "MATERIALS RECEIVING NOTE");
      cpt.SetParameterValue("Receivedby", "Received by: ");
      report = new DaiCo.Shared.View_Report(cpt);
      report.IsShowGroupTree = false;
      report.ShowReport(Shared.Utility.ViewState.ModalWindow);
    }

    /// <summary>
    /// Delete Receving
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@RTWNo", DbType.AnsiString, 50, this.RTWNo);
      DBParameter[] output = new DBParameter[1];

      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure(SP_WHDRTWInfomation_Delete, input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result == 1)
      {
        WindowUtinity.ShowMessageSuccess("MSG0002");
      }
      else if (result == 0)
      {
        WindowUtinity.ShowMessageError("ERR0004");
        return;
      }
      this.CloseTab();
    }

    /// <summary>
    /// Save Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckInvalid(out message);
      if (!success)
      {
        if (message.Length > 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", message);
        }
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
      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Import Data From Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnImport_Click(object sender, EventArgs e)
    {
      this.ImportDataFromExcel();
    }

    /// <summary>
    /// Get template
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetemplate_Click(object sender, EventArgs e)
    {
      //Init report
      string strTemplateName = "WHDListmaterialsToReceiving";
      string strSheetName = "Sheet1";
      string strOutFileName = "Template Add Materials Return From Production";
      XlsReport oXlsReport = this.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, out strOutFileName);

      //Export
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// OpenDailog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnOpenDaiLog_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtImportExcel.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtImportExcel.Text.Trim().Length > 0);
    }

    /// <summary>
    /// Before Delete Row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingDetailPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long rtwDetailPid = long.MinValue;
        try
        {
          rtwDetailPid = DBConvert.ParseLong(row.Cells["RTWDetailPid"].Value.ToString());
        }
        catch { }
        if (rtwDetailPid != long.MinValue)
        {
          this.listDeletingDetailPid.Add(rtwDetailPid);
        }
      }
    }

    /// <summary>
    /// After Delete Row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long rtwDetailpid in this.listDeletingDetailPid)
      {
        this.listDeletedDetailPid.Add(rtwDetailpid);
      }
      bool isReadOnly = (ultData.Rows.Count > 0 ? true : false);
      ucbWarehouse.ReadOnly = isReadOnly;
    }

    /// <summary>
    /// Before Cell Update Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      switch (columnName.ToLower())
      {
        case "qty":
          if (DBConvert.ParseDouble(text) == double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Qty");
            e.Cancel = true;
          }
          else if (DBConvert.ParseDouble(text) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Qty");
            e.Cancel = true;
          }
          break;
        default:
          break;
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

    private void ucbWarehouse_ValueChanged(object sender, EventArgs e)
    {
      if (ucbWarehouse.SelectedRow != null)
      {
        this.whPid = DBConvert.ParseInt(ucbWarehouse.Value);
      }
      else
      {
        this.whPid = 0;
      }
      Utility.LoadUltraCBMaterialListByWHPid(ucbMaterialList, this.whPid);
      Utility.LoadUltraCBLocationListByWHPid(ucbLocation, this.whPid);
    }
    #endregion Event   
  }
}
