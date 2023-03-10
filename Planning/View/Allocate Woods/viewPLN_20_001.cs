/*
  Author      : 
  Date        : 03-04-2013
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
  public partial class viewPLN_20_001 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    public bool flagWHD = false;
    private IList listDeletedPid = new ArrayList();
    private int status = int.MinValue;
    #endregion Field

    #region Init

    public viewPLN_20_001()
    {
      InitializeComponent();
    }
    private void viewPLN_20_001_Load(object sender, EventArgs e)
    {
      if (this.flagWHD)
      {
        this.chkSelectedAll.Visible = true;
      }
      else
      {
        this.chkSelectedAll.Visible = false;
      }
      // Load Data
      this.LoadData();
    }
    #endregion Init

    #region Function

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.pid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNWoodsRequestSpecialID_Select", inputParam);
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
          DataTable dtCode = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPLNWoodsGetNewRequestSpecialIDNo('30RES') NewRequestNo");
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
      dt.Columns.Add("Carcass", typeof(System.String));
      dt.Columns.Add("LotNoId", typeof(System.String));
      dt.Columns.Add("MainCode", typeof(System.String));
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
      DataTable dtMain = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNWoodsGetDataRequestSpecialID_Select", input);
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

      DataBaseAccess.ExecuteStoreProcedure("spPLNWoodsRequestSpecialID_Edit", inputParam, ouputParam);
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

        DataBaseAccess.ExecuteStoreProcedure("spPLNWoodsRequestSpecialIDDetail_Delete", inputDelete, outputDelete);
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
          DBParameter[] inputParam = new DBParameter[15];
          if (DBConvert.ParseLong(row["Pid"].ToString()) != long.MinValue)
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, long.Parse(row["Pid"].ToString()));
          }
          inputParam[1] = new DBParameter("@RequestPid", DbType.Int64, this.pid);
          if (DBConvert.ParseInt(row["WO"].ToString()) != int.MinValue)
          {
            inputParam[2] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(row["WO"].ToString()));
          }
          if (row["Carcass"].ToString().Trim().Length > 0)
          {
            inputParam[3] = new DBParameter("@Carcass", DbType.String, row["Carcass"].ToString());
          }
          if (DBConvert.ParseInt(row["WONew"].ToString()) != int.MinValue)
          {
            inputParam[4] = new DBParameter("@WONew", DbType.Int32, DBConvert.ParseInt(row["WONew"].ToString()));
          }
          if (row["CarcassNew"].ToString().Trim().Length > 0)
          {
            inputParam[5] = new DBParameter("@CarcassNew", DbType.String, row["CarcassNew"].ToString());
          }
          inputParam[6] = new DBParameter("@LotNoId", DbType.String, row["LotNoId"].ToString());
          inputParam[7] = new DBParameter("@MaterialCode", DbType.String, row["MaterialCode"].ToString());
          inputParam[8] = new DBParameter("@DimensionPid", DbType.Int64, DBConvert.ParseLong(row["DimensionPid"].ToString()));
          inputParam[9] = new DBParameter("@LocationPid", DbType.Int64, DBConvert.ParseLong(row["LocationPid"].ToString()));
          inputParam[10] = new DBParameter("@PackagePid", DbType.Int64, DBConvert.ParseLong(row["PackagePid"].ToString()));
          inputParam[11] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          inputParam[12] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          if (this.flagWHD == false)
          {
            inputParam[13] = new DBParameter("@Type", DbType.Int32, 0);
          }
          else
          {
            if (DBConvert.ParseInt(row["Select"].ToString()) == 1)
            {
              inputParam[13] = new DBParameter("@Type", DbType.Int32, 1);
            }
            else
            {
              inputParam[13] = new DBParameter("@Type", DbType.Int32, 0);
            }
          }

          inputParam[14] = new DBParameter("@MainCode", DbType.String, row["MainCode"].ToString());

          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spPLNWoodsRequestSpecialIDDetail_Edit", inputParam, outputParam);
          if (DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0)
          {
            return false;
          }
        }
      }
      if (this.flagWHD == true)
      {
        // 3: Update WO, Carcass(TblWHDStockBalance)
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@RequestPid", DbType.Int64, this.pid);

        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spPLNWoodsStockBalanceWOCarcass_Update", input, output);
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
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["Select"].Header.Caption = "WH Confirm";
      e.Layout.Bands[0].Columns["Error"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationPid"].Hidden = true;
      e.Layout.Bands[0].Columns["PackagePid"].Hidden = true;
      e.Layout.Bands[0].Columns["DimensionPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Dimension"].Header.Caption = "Dimension(LxWxT)";
      e.Layout.Bands[0].Columns["TotalCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Select"].DefaultCellValue = 0;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalCBM"],
              SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.0000}";
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }


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

    private void btnImport_Click(object sender, EventArgs e)
    {
      // Import Data
      if (this.txtImportExcel.Text.Trim().Length == 0)
      {
        return;
      }
      try
      {
        DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B4:E331]").Tables[0];
        if (dtSource == null)
        {
          return;
        }
        // 1: Create DataTable Import
        DataTable dtNew = this.CreateDataTable();
        foreach (DataRow drMain in dtSource.Rows)
        {
          if (drMain["IDWoods"].ToString().Trim().Length > 0)
          {
            DataRow row = dtNew.NewRow();
            row["WO"] = DBConvert.ParseInt(drMain["WO"].ToString());
            row["Carcass"] = drMain["Carcass"].ToString();
            row["LotNoId"] = drMain["IDWoods"].ToString();
            row["MainCode"] = drMain["MainCode"].ToString();
            dtNew.Rows.Add(row);
          }
        }
        // 2: Result Return
        DataTable dtResult = GetDataTableImport(dtNew);
        if (dtResult == null)
        {
          return;
        }

        DataTable dtMain = (DataTable)this.ultData.DataSource;
        // 3: Check Trung(BarCode)
        foreach (DataRow dr in dtResult.Rows)
        {
          DataRow row = dtMain.NewRow();
          string select = string.Empty;
          select = string.Format(@"LotNoId = '{0}'", dr["LotNoId"].ToString());
          DataRow[] foundRows = dtMain.Select(select);
          if (foundRows.Length >= 1)
          {
            row["Error"] = 1;
          }
          else
          {
            row["Error"] = DBConvert.ParseInt(dr["Error"].ToString());
          }

          if (DBConvert.ParseInt(dr["WO"].ToString()) != int.MinValue)
          {
            row["WO"] = DBConvert.ParseInt(dr["WO"].ToString());
          }

          if (dr["Carcass"].ToString().Trim().Length > 0)
          {
            row["Carcass"] = dr["Carcass"].ToString();
          }

          row["LotNoId"] = dr["LotNoId"].ToString();

          row["MaterialCode"] = dr["MaterialCode"].ToString();

          row["MaterialNameVn"] = dr["MaterialNameVn"].ToString();

          if (DBConvert.ParseDouble(dr["TotalCBM"].ToString()) != double.MinValue)
          {
            row["TotalCBM"] = DBConvert.ParseDouble(dr["TotalCBM"].ToString());
          }

          if (DBConvert.ParseLong(dr["DimensionPid"].ToString()) != long.MinValue)
          {
            row["DimensionPid"] = DBConvert.ParseLong(dr["DimensionPid"].ToString());
          }

          row["Dimension"] = dr["Dimension"].ToString();

          if (DBConvert.ParseLong(dr["LocationPid"].ToString()) != long.MinValue)
          {
            row["LocationPid"] = DBConvert.ParseLong(dr["LocationPid"].ToString());
          }

          row["Location"] = dr["Location"].ToString();

          if (DBConvert.ParseLong(dr["PackagePid"].ToString()) != long.MinValue)
          {
            row["PackagePid"] = DBConvert.ParseLong(dr["PackagePid"].ToString());
          }

          row["Package"] = dr["Package"].ToString();

          if (DBConvert.ParseInt(dr["WONew"].ToString()) != int.MinValue)
          {
            row["WONew"] = DBConvert.ParseInt(dr["WONew"].ToString());
          }

          if (dr["CarcassNew"].ToString().Trim().Length > 0)
          {
            row["CarcassNew"] = dr["CarcassNew"].ToString();
          }

          if (dr["MainCode"].ToString().Trim().Length > 0)
          {
            row["MainCode"] = dr["MainCode"].ToString();
          }

          row["Select"] = 0;

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
          else if (DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 2)
          {
            row1.Appearance.BackColor = Color.YellowGreen;
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

    private void btnGetTemPlate_Click(object sender, EventArgs e)
    {
      string templateName = "WoodsRequestSpecialID";
      string sheetName = "Sheet1";
      string outFileName = "Request Special";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      txtImportExcel.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void chkSelectedAll_CheckedChanged(object sender, EventArgs e)
    {
      int selected = 0;
      if (chkSelectedAll.Checked)
      {
        selected = 1;
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        ultData.Rows[i].Cells["Select"].Value = selected;
      }
    }
    #endregion Event
  }
}
