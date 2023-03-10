/*
  Author      : 
  Date        : 31-05-2013
  Description : Request Special ID
              : FlagWHD = true is Warehouse
              : FlagWHD = false is Planing
*/
using DaiCo.Application;
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
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_21_001 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    public bool flagWHD = false;
    private IList listDeletedPid = new ArrayList();
    private int status = int.MinValue;
    #endregion Field

    #region Init

    public viewPLN_21_001()
    {
      InitializeComponent();
    }

    private void viewPLN_21_001_Load(object sender, EventArgs e)
    {
      if (this.flagWHD)
      {
        this.chkAuto.Visible = true;
      }
      else
      {
        this.chkAuto.Visible = false;
      }
      // Load Data
      this.LoadData();

      if (this.flagWHD)
      {
        this.chkAuto.Checked = false;
        this.chkAuto.Checked = true;
      }
    }
    #endregion Init

    #region Function

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.pid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNVeneerRequestSpecialID_Select", inputParam);
      if (dsSource != null)
      {
        // 1: Load Master
        DataTable dtInfo = dsSource.Tables[0];
        if (dtInfo.Rows.Count > 0)
        {
          DataRow row = dtInfo.Rows[0];
          txtRequestNo.Text = row["RequestNo"].ToString();
          txtCreateBy.Text = row["CreateBy"].ToString();
          txtCreateDate.Text = row["CreateDate"].ToString();
          txtDescription.Text = row["Description"].ToString();
          this.status = DBConvert.ParseInt(row["Status"].ToString());
        }
        else
        {
          DataTable dtCode = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPLNVeneerGetNewRequestSpecialIDNo('30RES') NewRequestNo");
          if ((dtCode != null) && (dtCode.Rows.Count == 1))
          {
            this.txtRequestNo.Text = dtCode.Rows[0]["NewRequestNo"].ToString();
            this.txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
            this.txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          }
        }
        // 2: Load Detail
        ultData.DataSource = dsSource.Tables[1];
        // 3: Set Status Control
        this.SetStatusControl();
      }
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      if (this.flagWHD == true)
      {
        btnImport.Enabled = false;
        tblLayoutImport.Visible = false;
        ultData.Rows.Band.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        if (this.status == 1)
        {
          btnSave.Enabled = true;
        }
      }
      else
      {
        ultData.Rows.Band.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
        if (this.status == 1)
        {
          chkConfirm.Checked = true;
          chkConfirm.Enabled = false;
          btnSave.Enabled = false;
          ultData.Rows.Band.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        }
      }

      if (this.status == 2)
      {
        txtDescription.ReadOnly = true;
        chkConfirm.Checked = true;
        chkConfirm.Enabled = false;
        btnSave.Enabled = false;
        ultData.Rows.Band.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
        ultData.Rows.Band.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      }
    }

    /// <summary>
    /// Create Table Import
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("WO", typeof(System.Int32));
      dt.Columns.Add("CarcassCode", typeof(System.String));
      dt.Columns.Add("NewWO", typeof(System.Int32));
      dt.Columns.Add("NewCarcassCode", typeof(System.String));
      dt.Columns.Add("LotNoId", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Double));
      dt.Columns.Add("MainMaterialCode", typeof(System.String));
      return dt;
    }

    /// <summary>
    /// Get Data Import Excel
    /// </summary>
    /// <param name="dtImport"></param>
    /// <returns></returns>
    private DataTable GetDataTableImport(DataTable dtImport)
    {
      SqlDBParameter[] input = new SqlDBParameter[1];
      input[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtImport);
      DataTable dtMain = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNVeneerGetDataRequestSpecialID_Select", input);
      return dtMain;
    }

    /// <summary>
    /// Check Vaild
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckVaild(out string message)
    {
      message = string.Empty;
      // 2: Detail
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) > 0)
        {
          message = "Data Input";
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Check Vaild
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckOver(out string message)
    {
      message = string.Empty;
      // 2: Detail
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Error"].Value.ToString()) == -1)
        {
          message = "Data Input";
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      // 1: Save Master
      bool success = this.SaveInfo();
      if (success)
      {
        // 2: Save Detail
        success = this.SaveDetail();
      }
      else
      {
        success = false;
      }
      return success;
    }

    /// <summary>
    /// Save Master
    /// </summary>
    /// <returns></returns>
    private bool SaveInfo()
    {
      DBParameter[] inputParam = new DBParameter[6];
      if (this.pid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      }
      inputParam[1] = new DBParameter("@PrefixCode", DbType.AnsiString, 5, "30RES");
      inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[3] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      if (txtDescription.Text.Trim().Length > 0)
      {
        inputParam[4] = new DBParameter("@Description", DbType.String, txtDescription.Text);
      }
      if (this.flagWHD == false)
      {
        if (chkConfirm.Checked)
        {
          inputParam[5] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          inputParam[5] = new DBParameter("@Status", DbType.Int32, 0);
        }
      }
      else
      {
        if (chkConfirm.Checked)
        {
          inputParam[5] = new DBParameter("@Status", DbType.Int32, 2);
        }
        else
        {
          inputParam[5] = new DBParameter("@Status", DbType.Int32, 1);
        }
      }

      DBParameter[] ouputParam = new DBParameter[1];
      ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPLNVeneerRequestSpecialID_Edit", inputParam, ouputParam);
      long result = DBConvert.ParseLong(ouputParam[0].Value.ToString());
      // Gan Lai ReceivingPid
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
      // 1: Delete Row
      foreach (long pidDelete in this.listDeletedPid)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pidDelete) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };

        DataBaseAccess.ExecuteStoreProcedure("spPLNVeneerRequestSpecialIDDetail_Delete", inputDelete, outputDelete);
        long resultDelete = DBConvert.ParseLong(outputDelete[0].Value.ToString());
        if (resultDelete <= 0)
        {
          return false;
        }
      }

      // 2: Save Detail
      DataTable dtMain = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtMain.Rows.Count; i++)
      {
        DataRow row = dtMain.Rows[i];
        if (row.RowState != DataRowState.Deleted)
        {
          DBParameter[] inputParam = new DBParameter[13];
          if (DBConvert.ParseLong(row["Pid"].ToString()) != long.MinValue)
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, long.Parse(row["Pid"].ToString()));
          }
          inputParam[1] = new DBParameter("@RequestPid", DbType.Int64, this.pid);
          if (DBConvert.ParseInt(row["WO"].ToString()) != int.MinValue)
          {
            inputParam[2] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(row["WO"].ToString()));
          }
          if (row["CarcassCode"].ToString().Trim().Length > 0)
          {
            inputParam[3] = new DBParameter("@CarcassCode", DbType.String, row["CarcassCode"].ToString());
          }
          if (DBConvert.ParseInt(row["NewWO"].ToString()) != int.MinValue)
          {
            inputParam[4] = new DBParameter("@NewWO", DbType.Int32, DBConvert.ParseInt(row["NewWO"].ToString()));
          }
          if (row["NewCarcass"].ToString().Trim().Length > 0)
          {
            inputParam[5] = new DBParameter("@NewCarcassCode", DbType.String, row["NewCarcass"].ToString());
          }
          inputParam[6] = new DBParameter("@LotNoId", DbType.String, row["LotNoId"].ToString());
          inputParam[7] = new DBParameter("@MaterialCode", DbType.String, row["MaterialCode"].ToString());
          inputParam[8] = new DBParameter("@MainMaterialCode", DbType.String, row["MainMaterialCode"].ToString());
          inputParam[9] = new DBParameter("@DimensionPid", DbType.Int64, DBConvert.ParseLong(row["DimensionPid"].ToString()));
          inputParam[10] = new DBParameter("@LocationPid", DbType.Int64, DBConvert.ParseLong(row["LocationPid"].ToString()));
          inputParam[11] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row["Qty"].ToString()));
          if (this.flagWHD == false)
          {
            inputParam[12] = new DBParameter("@Type", DbType.Int32, 0);
          }
          else
          {
            if (DBConvert.ParseInt(row["Select"].ToString()) == 1)
            {
              inputParam[12] = new DBParameter("@Type", DbType.Int32, 1);
            }
            else
            {
              inputParam[12] = new DBParameter("@Type", DbType.Int32, 0);
            }
          }

          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure("spPLNVeneerRequestSpecialIDDetail_Edit", inputParam, outputParam);
          if (DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0)
          {
            return false;
          }
        }
      }
      if (this.flagWHD == true)
      {
        // 3: Update WO, Carcass(TblWHDStockBalance_Veneer)
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@RequestPid", DbType.Int64, this.pid);

        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spPLNVeneerStockBalanceWOCarcass_Update", input, output);
        if (DBConvert.ParseLong(output[0].Value.ToString()) <= 0)
        {
          return false;
        }
      }

      return true;
    }
    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["Select"].Header.Caption = "WH Confirm";
      e.Layout.Bands[0].Columns["Error"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationPid"].Hidden = true;
      e.Layout.Bands[0].Columns["DimensionPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Select"].DefaultCellValue = 0;
      e.Layout.Bands[0].Columns["Allocated"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyM2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["NewWO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["LotNoId"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["LotNoId"].MinWidth = 70;

      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 100;

      e.Layout.Bands[0].Columns["MainMaterialCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["MainMaterialCode"].MinWidth = 100;

      e.Layout.Bands[0].Columns["NewCarcass"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["NewCarcass"].MinWidth = 100;

      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 100;

      e.Layout.Bands[0].Columns["WO"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["WO"].MinWidth = 70;

      e.Layout.Bands[0].Columns["NewWO"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["NewWO"].MinWidth = 70;

      e.Layout.Bands[0].Columns["Length"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Length"].MinWidth = 50;

      e.Layout.Bands[0].Columns["Width"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Width"].MinWidth = 50;

      e.Layout.Bands[0].Columns["Location"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Location"].MinWidth = 50;

      e.Layout.Bands[0].Columns["Allocated"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Allocated"].MinWidth = 60;

      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 40;

      e.Layout.Bands[0].Columns["QtyM2"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["QtyM2"].MinWidth = 40;

      e.Layout.Bands[0].Columns["Select"].MaxWidth = 30;
      e.Layout.Bands[0].Columns["Select"].MinWidth = 30;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Allocated"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["QtyM2"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["QtyM2"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.00}";

      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Bofore Row Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    /// <summary>
    /// Select All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkSelectedAll_CheckedChanged(object sender, EventArgs e)
    {
      int selected = 0;
      if (chkAuto.Checked)
      {
        selected = 1;
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        ultData.Rows[i].Cells["Select"].Value = selected;
      }
    }

    /// <summary>
    /// Select Confirm WH
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "auto":
          if (DBConvert.ParseInt(e.Cell.Row.Cells["Auto"].Text) == 1)
          {
            e.Cell.Row.Cells["QtyAllocated"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value.ToString());
          }
          else
          {
            e.Cell.Row.Cells["QtyAllocated"].Value = 0;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Get Template Import Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetTemPlate_Click(object sender, EventArgs e)
    {
      string templateName = "PLN_21_001";
      string sheetName = "Sheet1";
      string outFileName = "Request Special";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
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
      txtImportExcel.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    /// <summary>
    /// Import Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnImport_Click(object sender, EventArgs e)
    {
      // Import Data
      if (this.txtImportExcel.Text.Trim().Length == 0)
      {
        return;
      }
      try
      {
        DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B4:H1000]").Tables[0];
        if (dtSource == null)
        {
          return;
        }
        // 1: Create DataTable Import
        DataTable dtNew = this.CreateDataTable();
        // Excel File
        foreach (DataRow drMain in dtSource.Rows)
        {
          if (drMain["IDVeneer"].ToString().Trim().Length > 0)
          {
            DataRow row = dtNew.NewRow();
            if (DBConvert.ParseInt(drMain["WO"].ToString()) > 0)
            {
              row["WO"] = DBConvert.ParseInt(drMain["WO"].ToString());
            }
            else
            {
              row["WO"] = DBNull.Value;
            }
            row["CarcassCode"] = drMain["Carcass"].ToString();
            if (DBConvert.ParseInt(drMain["New WO"].ToString()) > 0)
            {
              row["NewWO"] = DBConvert.ParseInt(drMain["New WO"].ToString());
            }
            else
            {
              row["NewWO"] = DBNull.Value;
            }
            row["NewCarcassCode"] = drMain["New Carcass"].ToString();
            row["LotNoId"] = drMain["IDVeneer"].ToString();
            if (DBConvert.ParseInt(drMain["Qty"].ToString()) > 0)
            {
              row["Qty"] = DBConvert.ParseInt(drMain["Qty"].ToString());
            }
            else
            {
              row["Qty"] = 0;
            }
            row["MainMaterialCode"] = drMain["MainMaterialCode"].ToString();
            dtNew.Rows.Add(row);
          }
        }

        // On Grid
        for (int i = 0; i < this.ultData.Rows.Count; i++)
        {
          UltraGridRow rowGrid = this.ultData.Rows[i];
          DataRow row = dtNew.NewRow();
          if (DBConvert.ParseInt(rowGrid.Cells["WO"].Value.ToString()) > 0)
          {
            row["WO"] = DBConvert.ParseInt(rowGrid.Cells["WO"].Value.ToString());
          }
          else
          {
            row["WO"] = DBNull.Value;
          }
          row["CarcassCode"] = rowGrid.Cells["CarcassCode"].Value.ToString();
          if (DBConvert.ParseInt(rowGrid.Cells["NewWO"].Value.ToString()) > 0)
          {
            row["NewWO"] = DBConvert.ParseInt(rowGrid.Cells["NewWO"].Value.ToString());
          }
          else
          {
            row["NewWO"] = DBNull.Value;
          }
          row["NewCarcassCode"] = rowGrid.Cells["NewCarcass"].Value.ToString();
          row["LotNoId"] = rowGrid.Cells["LotNoId"].Value.ToString();
          if (DBConvert.ParseInt(rowGrid.Cells["Qty"].Value.ToString()) > 0)
          {
            row["Qty"] = DBConvert.ParseInt(rowGrid.Cells["Qty"].Value.ToString());
          }
          else
          {
            row["Qty"] = 0;
          }
          row["MainMaterialCode"] = rowGrid.Cells["MainMaterialCode"].Value.ToString();
          dtNew.Rows.Add(row);
        }

        // 2: Result Return
        DataTable dtResult = GetDataTableImport(dtNew);
        if (dtResult == null)
        {
          return;
        }
        // Gan DataSource
        DataTable dtMain = (DataTable)this.ultData.DataSource;
        dtMain.Clear();

        foreach (DataRow dr in dtResult.Rows)
        {
          DataRow row = dtMain.NewRow();
          row["Error"] = DBConvert.ParseInt(dr["Error"].ToString());

          if (DBConvert.ParseInt(dr["WO"].ToString()) != int.MinValue)
          {
            row["WO"] = DBConvert.ParseInt(dr["WO"].ToString());
          }
          if (dr["CarcassCode"].ToString().Trim().Length > 0)
          {
            row["CarcassCode"] = dr["CarcassCode"].ToString();
          }
          row["LotNoId"] = dr["LotNoId"].ToString();
          row["MaterialCode"] = dr["MaterialCode"].ToString();
          row["MaterialNameVn"] = dr["MaterialNameVn"].ToString();
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
          if (DBConvert.ParseLong(dr["LocationPid"].ToString()) != long.MinValue)
          {
            row["LocationPid"] = DBConvert.ParseLong(dr["LocationPid"].ToString());
          }
          row["Location"] = dr["Location"].ToString();
          if (DBConvert.ParseInt(dr["NewWO"].ToString()) != int.MinValue)
          {
            row["NewWO"] = DBConvert.ParseInt(dr["NewWO"].ToString());
          }
          if (dr["NewCarcass"].ToString().Trim().Length > 0)
          {
            row["NewCarcass"] = dr["NewCarcass"].ToString();
          }
          if (DBConvert.ParseInt(dr["Qty"].ToString()) != int.MinValue)
          {
            row["Qty"] = DBConvert.ParseInt(dr["Qty"].ToString());
          }
          if (DBConvert.ParseDouble(dr["QtyM2"].ToString()) != Double.MinValue)
          {
            row["QtyM2"] = DBConvert.ParseDouble(dr["QtyM2"].ToString());
          }
          if (DBConvert.ParseInt(dr["Allocated"].ToString()) != int.MinValue)
          {
            row["Allocated"] = DBConvert.ParseInt(dr["Allocated"].ToString());
          }
          row["MainMaterialCode"] = dr["MainMaterialCode"].ToString();
          row["Select"] = 0;
          // Add Row
          dtMain.Rows.Add(row);
        }

        // 4: Gan Lai Luoi
        ultData.DataSource = dtMain;
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row1 = ultData.Rows[i];
          if (DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 1)
          {
            row1.Appearance.BackColor = Color.Yellow;
          }
          else if (DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 2 ||
                    DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 3 ||
                    DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 4 ||
                    DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 5 ||
                    DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 10)
          {
            row1.Appearance.BackColor = Color.YellowGreen;
          }
          else if (DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 6 ||
                    DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 7 ||
                    DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 8 ||
                    DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 9)
          {
            row1.Appearance.BackColor = Color.Aqua;
          }
          else if (DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == -1)
          {
            row1.Appearance.BackColor = Color.Orange;
          }
        }
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0105");
        return;
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
      bool success = this.CheckVaild(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      //2: Check Over
      success = this.CheckOver(out message);
      if (!success)
      {
        DialogResult result = WindowUtinity.ShowMessageConfirm("ERR0026", message);
        if (result == DialogResult.No)
        {
          return;
        }
      }

      // 2: Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }

      // 3: Load Data
      this.LoadData();
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

    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {

      // 1: Create DataTable Import
      DataTable dtNew = this.CreateDataTable();
      // On Grid
      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = this.ultData.Rows[i];
        DataRow row = dtNew.NewRow();
        if (DBConvert.ParseInt(rowGrid.Cells["WO"].Value.ToString()) > 0)
        {
          row["WO"] = DBConvert.ParseInt(rowGrid.Cells["WO"].Value.ToString());
        }
        else
        {
          row["WO"] = DBNull.Value;
        }
        row["CarcassCode"] = rowGrid.Cells["CarcassCode"].Value.ToString();
        if (DBConvert.ParseInt(rowGrid.Cells["NewWO"].Value.ToString()) > 0)
        {
          row["NewWO"] = DBConvert.ParseInt(rowGrid.Cells["NewWO"].Value.ToString());
        }
        else
        {
          row["NewWO"] = DBNull.Value;
        }
        row["NewCarcassCode"] = rowGrid.Cells["NewCarcass"].Value.ToString();
        row["LotNoId"] = rowGrid.Cells["LotNoId"].Value.ToString();
        if (DBConvert.ParseInt(rowGrid.Cells["Qty"].Value.ToString()) > 0)
        {
          row["Qty"] = DBConvert.ParseInt(rowGrid.Cells["Qty"].Value.ToString());
        }
        else
        {
          row["Qty"] = 0;
        }
        row["MainMaterialCode"] = rowGrid.Cells["MainMaterialCode"].Value.ToString();
        dtNew.Rows.Add(row);
      }

      // 2: Result Return
      DataTable dtResult = GetDataTableImport(dtNew);
      if (dtResult == null)
      {
        return;
      }
      // Gan DataSource
      DataTable dtMain = (DataTable)this.ultData.DataSource;
      dtMain.Clear();

      foreach (DataRow dr in dtResult.Rows)
      {
        DataRow row = dtMain.NewRow();
        row["Error"] = DBConvert.ParseInt(dr["Error"].ToString());

        if (DBConvert.ParseInt(dr["WO"].ToString()) != int.MinValue)
        {
          row["WO"] = DBConvert.ParseInt(dr["WO"].ToString());
        }
        if (dr["CarcassCode"].ToString().Trim().Length > 0)
        {
          row["CarcassCode"] = dr["CarcassCode"].ToString();
        }
        row["LotNoId"] = dr["LotNoId"].ToString();
        row["MaterialCode"] = dr["MaterialCode"].ToString();
        row["MaterialNameVn"] = dr["MaterialNameVn"].ToString();
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
        if (DBConvert.ParseLong(dr["LocationPid"].ToString()) != long.MinValue)
        {
          row["LocationPid"] = DBConvert.ParseLong(dr["LocationPid"].ToString());
        }
        row["Location"] = dr["Location"].ToString();
        if (DBConvert.ParseInt(dr["NewWO"].ToString()) != int.MinValue)
        {
          row["NewWO"] = DBConvert.ParseInt(dr["NewWO"].ToString());
        }
        if (dr["NewCarcass"].ToString().Trim().Length > 0)
        {
          row["NewCarcass"] = dr["NewCarcass"].ToString();
        }
        if (DBConvert.ParseInt(dr["Qty"].ToString()) != int.MinValue)
        {
          row["Qty"] = DBConvert.ParseInt(dr["Qty"].ToString());
        }
        if (DBConvert.ParseDouble(dr["QtyM2"].ToString()) != Double.MinValue)
        {
          row["QtyM2"] = DBConvert.ParseDouble(dr["QtyM2"].ToString());
        }
        if (DBConvert.ParseInt(dr["Allocated"].ToString()) != int.MinValue)
        {
          row["Allocated"] = DBConvert.ParseInt(dr["Allocated"].ToString());
        }
        row["MainMaterialCode"] = dr["MainMaterialCode"].ToString();
        row["Select"] = 0;
        // Add Row
        dtMain.Rows.Add(row);
      }

      // 4: Gan Lai Luoi
      ultData.DataSource = dtMain;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row1 = ultData.Rows[i];
        if (DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 1)
        {
          row1.Appearance.BackColor = Color.Yellow;
        }
        else if (DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 2 ||
                  DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 3 ||
                  DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 4 ||
                  DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 5 ||
                  DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 10)
        {
          row1.Appearance.BackColor = Color.YellowGreen;
        }
        else if (DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 6 ||
                  DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 7 ||
                  DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 8 ||
                  DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 9)
        {
          row1.Appearance.BackColor = Color.Aqua;
        }
        else if (DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == -1)
        {
          row1.Appearance.BackColor = Color.Orange;
        }
      }
    }

    /// <summary>
    /// Add Multi Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddMulti_Click(object sender, EventArgs e)
    {
      viewPLN_21_015 uc = new viewPLN_21_015();
      WindowUtinity.ShowView(uc, "ADD MULTI DETAIL", true, ViewState.ModalWindow, FormWindowState.Maximized);
      DataTable dtDetail = uc.dtDetail;

      // Create Data Table Import
      DataTable dtSource = this.CreateDataTable();

      // Add Row From UltraGrid
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = ultData.Rows[i];
        DataRow row = dtSource.NewRow();
        if (DBConvert.ParseInt(rowGrid.Cells["WO"].Value.ToString()) > 0)
        {
          row["WO"] = DBConvert.ParseInt(rowGrid.Cells["WO"].Value.ToString());
        }
        if (rowGrid.Cells["CarcassCode"].Value.ToString().Length > 0)
        {
          row["CarcassCode"] = rowGrid.Cells["CarcassCode"].Value.ToString();
        }
        if (DBConvert.ParseInt(rowGrid.Cells["NewWO"].Value.ToString()) > 0)
        {
          row["NewWO"] = DBConvert.ParseInt(rowGrid.Cells["NewWO"].Value.ToString());
        }
        if (rowGrid.Cells["NewCarcass"].Value.ToString().Length > 0)
        {
          row["NewCarcassCode"] = rowGrid.Cells["NewCarcass"].Value.ToString();
        }
        row["LotNoId"] = rowGrid.Cells["LotNoId"].Value.ToString();
        if (DBConvert.ParseDouble(rowGrid.Cells["Qty"].Value.ToString()) > 0)
        {
          row["Qty"] = DBConvert.ParseDouble(rowGrid.Cells["Qty"].Value.ToString());
        }
        if (rowGrid.Cells["MainMaterialCode"].Value.ToString().Length > 0)
        {
          row["MainMaterialCode"] = rowGrid.Cells["MainMaterialCode"].Value.ToString();
        }
        dtSource.Rows.Add(row);
      }

      // Add Row From Add Multi Detail
      for (int j = 0; j < dtDetail.Rows.Count; j++)
      {
        DataRow rowDetail = dtDetail.Rows[j];
        DataRow row = dtSource.NewRow();
        if (DBConvert.ParseInt(rowDetail["WO"].ToString()) > 0)
        {
          row["WO"] = DBConvert.ParseInt(rowDetail["WO"].ToString());
        }
        if (rowDetail["CarcassCode"].ToString().Length > 0)
        {
          row["CarcassCode"] = rowDetail["CarcassCode"].ToString();
        }
        if (DBConvert.ParseInt(rowDetail["NewWO"].ToString()) > 0)
        {
          row["NewWO"] = DBConvert.ParseInt(rowDetail["NewWO"].ToString());
        }
        if (rowDetail["NewCarcassCode"].ToString().Length > 0)
        {
          row["NewCarcassCode"] = rowDetail["NewCarcassCode"].ToString();
        }
        row["LotNoId"] = rowDetail["LotNoId"].ToString();
        row["Qty"] = DBConvert.ParseDouble(rowDetail["Qty"].ToString());
        if (rowDetail["MainMaterialCode"].ToString().Length > 0)
        {
          row["MainMaterialCode"] = rowDetail["MainMaterialCode"].ToString();
        }
        dtSource.Rows.Add(row);
      }

      // 2: Result Return
      DataTable dtResult = GetDataTableImport(dtSource);
      if (dtResult == null)
      {
        return;
      }

      // Gan DataSource
      DataTable dtMain = (DataTable)this.ultData.DataSource;
      dtMain.Clear();

      foreach (DataRow dr in dtResult.Rows)
      {
        DataRow row = dtMain.NewRow();
        row["Error"] = DBConvert.ParseInt(dr["Error"].ToString());

        if (DBConvert.ParseInt(dr["WO"].ToString()) != int.MinValue)
        {
          row["WO"] = DBConvert.ParseInt(dr["WO"].ToString());
        }
        if (dr["CarcassCode"].ToString().Trim().Length > 0)
        {
          row["CarcassCode"] = dr["CarcassCode"].ToString();
        }
        row["LotNoId"] = dr["LotNoId"].ToString();
        row["MaterialCode"] = dr["MaterialCode"].ToString();
        row["MaterialNameVn"] = dr["MaterialNameVn"].ToString();
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
        if (DBConvert.ParseLong(dr["LocationPid"].ToString()) != long.MinValue)
        {
          row["LocationPid"] = DBConvert.ParseLong(dr["LocationPid"].ToString());
        }
        row["Location"] = dr["Location"].ToString();
        if (DBConvert.ParseInt(dr["NewWO"].ToString()) != int.MinValue)
        {
          row["NewWO"] = DBConvert.ParseInt(dr["NewWO"].ToString());
        }
        if (dr["NewCarcass"].ToString().Trim().Length > 0)
        {
          row["NewCarcass"] = dr["NewCarcass"].ToString();
        }
        if (DBConvert.ParseInt(dr["Qty"].ToString()) != int.MinValue)
        {
          row["Qty"] = DBConvert.ParseInt(dr["Qty"].ToString());
        }
        if (DBConvert.ParseDouble(dr["QtyM2"].ToString()) != Double.MinValue)
        {
          row["QtyM2"] = DBConvert.ParseDouble(dr["QtyM2"].ToString());
        }
        if (DBConvert.ParseInt(dr["Allocated"].ToString()) != int.MinValue)
        {
          row["Allocated"] = DBConvert.ParseInt(dr["Allocated"].ToString());
        }
        row["MainMaterialCode"] = dr["MainMaterialCode"].ToString();
        row["Select"] = 0;
        // Add Row
        dtMain.Rows.Add(row);
      }

      // 4: Gan Lai Luoi
      ultData.DataSource = dtMain;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row1 = ultData.Rows[i];
        if (DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 1)
        {
          row1.Appearance.BackColor = Color.Yellow;
        }
        else if (DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 2 ||
                  DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 3 ||
                  DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 4 ||
                  DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 5 ||
                  DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 10)
        {
          row1.Appearance.BackColor = Color.YellowGreen;
        }
        else if (DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 6 ||
                  DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 7 ||
                  DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 8 ||
                  DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 9)
        {
          row1.Appearance.BackColor = Color.Aqua;
        }
        else if (DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == -1)
        {
          row1.Appearance.BackColor = Color.Orange;
        }
      }
    }
    #endregion Event

  }
}
