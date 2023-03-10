/*
  Author      : 
  Date        : 17-03-2014
  Description : Import Data For Manhour Allocate For Component
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_10_018 : MainUserControl
  {
    #region Field
    public string team = string.Empty;
    public DateTime wDate = DateTime.MinValue;
    public int kind = int.MinValue;
    public int nonWIP = int.MinValue;

    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;
    #endregion Field

    #region Init
    public viewPLN_10_018()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_10_018_Load(object sender, EventArgs e)
    {

    }

    #endregion Init

    #region Function

    // Create Data Table
    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("EID", typeof(System.Int32));
      dt.Columns.Add("WO", typeof(System.Int64));
      dt.Columns.Add("Carcass", typeof(System.String));
      dt.Columns.Add("Component", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Int32));
      dt.Columns.Add("Processing", typeof(System.Double));
      dt.Columns.Add("SetUp", typeof(System.Double));
      dt.Columns.Add("WCP", typeof(System.String));
      dt.Columns.Add("NonOutPut", typeof(System.Int32));
      dt.Columns.Add("Remark", typeof(System.String));
      return dt;
    }

    // Create Data Table Save
    private DataTable CreateDataTableSave()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Team", typeof(System.String));
      dt.Columns.Add("WDate", typeof(System.DateTime));
      dt.Columns.Add("Kind", typeof(System.Int32));
      dt.Columns.Add("EID", typeof(System.Int32));

      dt.Columns.Add("WO", typeof(System.Int64));
      dt.Columns.Add("Carcass", typeof(System.String));
      dt.Columns.Add("Component", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Int32));
      dt.Columns.Add("ProcessPid", typeof(System.Int64));
      dt.Columns.Add("Processing", typeof(System.Double));
      dt.Columns.Add("SetUp", typeof(System.Double));
      dt.Columns.Add("NonOutPut", typeof(System.Int32));
      dt.Columns.Add("Remark", typeof(System.String));
      return dt;
    }

    /// <summary>
    /// Import Data From File Excel
    /// </summary>
    private void ImportData()
    {
      if (this.txtLocation.Text.Trim().Length == 0)
      {
        return;
      }
      // Get Data Table From Excel
      DataTable dtSource = new DataTable();
      dtSource = FunctionUtility.GetExcelToDataSetVersion2(txtLocation.Text.Trim(), "SELECT * FROM [Data (1)$B6:K507]").Tables[0];
      if (dtSource == null)
      {
        return;
      }

      DataTable dt = this.CreateDataTable();
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow rowData = dtSource.Rows[i];
        if (rowData["EID"].ToString().Length > 0)
        {
          DataRow row = dt.NewRow();
          if (DBConvert.ParseInt(rowData["EID"].ToString()) > 0)
          {
            row["EID"] = DBConvert.ParseInt(rowData["EID"].ToString());
          }

          if (DBConvert.ParseLong(rowData["WO"].ToString()) != long.MinValue)
          {
            row["WO"] = DBConvert.ParseLong(rowData["WO"].ToString());
          }

          if (rowData["Carcass"].ToString().Trim().Length > 0)
          {
            row["Carcass"] = rowData["Carcass"];
          }

          if (rowData["Component"].ToString().Trim().Length > 0)
          {
            row["Component"] = rowData["Component"];
          }

          if (DBConvert.ParseInt(rowData["Qty"].ToString()) > 0)
          {
            row["Qty"] = DBConvert.ParseInt(rowData["Qty"].ToString());
          }

          if (DBConvert.ParseDouble(rowData["Processing"].ToString()) > 0)
          {
            row["Processing"] = DBConvert.ParseDouble(rowData["Processing"].ToString());
          }

          if (DBConvert.ParseDouble(rowData["Setup"].ToString()) > 0)
          {
            row["Setup"] = DBConvert.ParseDouble(rowData["Setup"].ToString());
          }

          if (rowData["WCP"].ToString().Trim().Length > 0)
          {
            row["WCP"] = rowData["WCP"];
          }

          if (DBConvert.ParseInt(rowData["NonOutPut"].ToString()) >= 0)
          {
            row["NonOutPut"] = DBConvert.ParseInt(rowData["NonOutPut"].ToString());
          }

          if (rowData["Remark"].ToString().Trim().Length > 0)
          {
            row["Remark"] = rowData["Remark"];
          }
          dt.Rows.Add(row);
        }
      }

      SqlDBParameter[] inputParam = new SqlDBParameter[5];
      inputParam[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dt);
      inputParam[1] = new SqlDBParameter("@Team", SqlDbType.Text, this.team);
      inputParam[2] = new SqlDBParameter("@WDate", SqlDbType.DateTime, this.wDate);
      inputParam[3] = new SqlDBParameter("@Kind", SqlDbType.Int, this.kind);
      inputParam[4] = new SqlDBParameter("@NonWIP", SqlDbType.Int, this.nonWIP);
      DataTable dtCheck = Shared.DataBaseUtility.SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNCheckDataAllocateForComponent_Select", 900, inputParam);

      if (dtCheck != null && dtCheck.Rows.Count > 0)
      {
        ultData.DataSource = dtCheck;

        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 1) // EID
          {
            row.Appearance.BackColor = Color.Yellow;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 2) // WO
          {
            row.Appearance.BackColor = Color.CornflowerBlue;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 3) // Carcass
          {
            row.Appearance.BackColor = Color.Lime;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 4)  // Component
          {
            row.Appearance.BackColor = Color.LightCoral;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 5) // Processing
          {
            row.Appearance.BackColor = Color.Violet;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 6) // SetUp
          {
            row.Appearance.BackColor = Color.Pink;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 7) // WCP
          {
            row.Appearance.BackColor = Color.SkyBlue;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 8) // Over
          {
            row.Appearance.BackColor = Color.LightGreen;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 9) // NonOutPut
          {
            row.Appearance.BackColor = Color.Gray;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 10) // Qty
          {
            row.Appearance.BackColor = Color.Tan;
          }
        }
      }
    }

    /// <summary>
    /// Check Vaild
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckVaild(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) != 0)
        {
          message = "Data";
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      DataTable dt = this.CreateDataTableSave();
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = ultData.Rows[i];
        DataRow row = dt.NewRow();
        row["Team"] = this.team;
        row["WDate"] = this.wDate;
        row["Kind"] = this.kind;
        row["EID"] = DBConvert.ParseInt(rowGrid.Cells["EID"].Value.ToString());
        if (DBConvert.ParseLong(rowGrid.Cells["WO"].Value.ToString()) > 0)
        {
          row["WO"] = DBConvert.ParseLong(rowGrid.Cells["WO"].Value.ToString());
        }
        row["Carcass"] = rowGrid.Cells["Carcass"].Value.ToString();
        row["Component"] = rowGrid.Cells["Component"].Value.ToString();
        if (DBConvert.ParseInt(rowGrid.Cells["Qty"].Value.ToString()) > 0)
        {
          row["Qty"] = DBConvert.ParseInt(rowGrid.Cells["Qty"].Value.ToString());
        }
        if (DBConvert.ParseDouble(rowGrid.Cells["Processing"].Value.ToString()) > 0)
        {
          row["Processing"] = DBConvert.ParseDouble(rowGrid.Cells["Processing"].Value.ToString());
        }
        row["Setup"] = DBConvert.ParseDouble(rowGrid.Cells["Setup"].Value.ToString());
        if (DBConvert.ParseLong(rowGrid.Cells["ProcessPid"].Value.ToString()) > 0)
        {
          row["ProcessPid"] = DBConvert.ParseLong(rowGrid.Cells["ProcessPid"].Value.ToString());
        }
        row["NonOutPut"] = DBConvert.ParseInt(rowGrid.Cells["NonOutPut"].Value.ToString());
        if (rowGrid.Cells["Remark"].Value.ToString().Trim().Length > 0)
        {
          row["Remark"] = rowGrid.Cells["Remark"].Value.ToString();
        }
        dt.Rows.Add(row);
      }

      SqlDBParameter[] inputParam = new SqlDBParameter[2];
      inputParam[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dt);
      inputParam[1] = new SqlDBParameter("@CreateBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      SqlDBParameter[] outputParam = new SqlDBParameter[1];
      outputParam[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, 0);

      Shared.DataBaseUtility.SqlDataBaseAccess.ExecuteStoreProcedure("spPLNManhourAllocationForComFromImportExcel_Insert", inputParam, outputParam);
      long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    private void SaveData()
    {
      string message = string.Empty;
      bool success = this.CheckVaild(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      success = this.SaveDetail();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.CloseTab();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
    }

    #endregion Function

    #region Event

    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtLocation.Text.Trim().Length > 0);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      btnImport.Enabled = false;
      this.ImportData();
      btnImport.Enabled = true;
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "RPT_PLN_10_018";
      string sheetName = "Data";
      string outFileName = "TEMPLATE IMPORT DATA MANHOUR ALLOCATE FOR COMPONENT ";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["Errors"].Hidden = true;
      e.Layout.Bands[0].Columns["ProcessPid"].Hidden = true;
      e.Layout.Bands[0].Columns["NonOutPut"].Hidden = true;

      e.Layout.Bands[0].Columns["Processing"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Setup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    #endregion Event
  }
}
