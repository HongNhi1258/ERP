/*
  Author      : Nguyen Huynh Quoc Tuan  
  Date        : 4/8/2015
  Description : Swap Carcass WorkOrder
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
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
  public partial class viewPLN_29_007 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init

    public viewPLN_29_007()
    {
      InitializeComponent();
    }

    #endregion Init

    #region Function

    /// <summary>
    /// Check Vaild
    /// </summary>
    /// <returns></returns>
    private bool CheckVaild()
    {
      DataTable dtSource = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow dr = dtSource.Rows[i];
        if (DBConvert.ParseInt(dr["Error"].ToString()) > 0)
        {
          {
            WindowUtinity.ShowMessageError("ERR0050", "Error");
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Swap CarcassWork
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool result = true;
      DataTable dtSource = (DataTable)ultData.DataSource;
      DataTable dtCARWOUpdate = this.dtFurniture();

      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        DataRow rowadd = dtCARWOUpdate.NewRow();
        if (DBConvert.ParseInt(row["Error"].ToString()) == 0)
        {
          // Furniture Old
          if (row["FurnitureOld"].ToString().Trim().Length > 0)
          {
            rowadd["FurnitureOld"] = row["FurnitureOld"];
          }

          // Furniture New
          if (row["FurnitureNew"].ToString().Trim().Length > 0)
          {
            rowadd["FurnitureNew"] = row["FurnitureNew"];
          }

          //Add row datatable
          dtCARWOUpdate.Rows.Add(rowadd);
        }
      }
      if (dtCARWOUpdate.Rows.Count > 0)
      {
        //Input
        SqlDBParameter[] sqlinput = new SqlDBParameter[1];
        sqlinput[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtCARWOUpdate);

        // Output ------
        SqlDBParameter[] sqloutput = new SqlDBParameter[1];
        sqloutput[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue);

        // Result ------
        SqlDataBaseAccess.ExecuteStoreProcedure("spPLNSwapCarcassWork_Update", 500, sqlinput, sqloutput);
        long resultOutput = DBConvert.ParseLong(sqloutput[0].Value.ToString());
        if (resultOutput <= 0)
        {
          result = false;
        }
      }
      return result;
    }

    /// <summary>
    /// List Furniture Old, New
    /// </summary>
    /// <returns></returns>
    private DataTable dtFurniture()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("FurnitureOld", typeof(System.String));
      dt.Columns.Add("FurnitureNew", typeof(System.String));
      return dt;
    }

    /// <summary>
    /// Import Data
    /// </summary>
    private void ImportData()
    {
      if (this.txtPath.Text.Trim().Length == 0)
      {
        return;
      }
      // Get Data Table From Excel
      DataTable dtSource = new DataTable();
      dtSource = FunctionUtility.GetExcelToDataSetVersion2(txtPath.Text.Trim(), "SELECT * FROM [Sheet1 (1)$A1:B400]").Tables[0];
      if (dtSource == null)
      {
        return;
      }

      // Input ------- 
      SqlDBParameter[] sqlinput = new SqlDBParameter[1];
      DataTable dtInput = this.dtFurniture();

      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        DataRow rowadd = dtInput.NewRow();
        if (row["FurnitureOld"].ToString().Length > 0 && row["FurnitureNew"].ToString().Length > 0)
        {
          // Furniture Old
          if (row["FurnitureOld"].ToString().Trim().Length > 0)
          {
            rowadd["FurnitureOld"] = row["FurnitureOld"];
          }

          // Furniture New
          if (row["FurnitureNew"].ToString().Trim().Length > 0)
          {
            rowadd["FurnitureNew"] = row["FurnitureNew"];
          }

          //Add row datatable
          dtInput.Rows.Add(rowadd);
        }
      }
      sqlinput[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtInput);
      DataTable dtResultDeadline = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNSwapCarcassWork", sqlinput);
      ultData.DataSource = dtResultDeadline;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) != 0)
        {
          ultData.Rows[i].Appearance.BackColor = Color.Yellow;
        }
      }
    }

    private void GetTemplate()
    {
      string templateName = "viewPLN_29_007_SwapCarcassWork";
      string sheetName = "Sheet1";
      string outFileName = "Swap Carcass Workorder";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      //// Hidden column
      //e.Layout.Bands[0].Columns["WorkAreaPidOld"].Hidden = true;
      //e.Layout.Bands[0].Columns["TransactionPidOld"].Hidden = true;
      //e.Layout.Bands[0].Columns["PidOld"].Hidden = true;
      //e.Layout.Bands[0].Columns["CARDeadlineOld"].Hidden = true;
      //e.Layout.Bands[0].Columns["COM1DeadlineOld"].Hidden = true;
      //e.Layout.Bands[0].Columns["SUBCONDeadlineOld"].Hidden = true;
      //e.Layout.Bands[0].Columns["ASSYDeadlineOld"].Hidden = true;
      //e.Layout.Bands[0].Columns["ASSHWDeadlineOld"].Hidden = true;
      //e.Layout.Bands[0].Columns["FFHWDeadlineOld"].Hidden = true;
      //e.Layout.Bands[0].Columns["MATDeadlineOld"].Hidden = true;

      //e.Layout.Bands[0].Columns["WorkAreaPidNew"].Hidden = true;
      //e.Layout.Bands[0].Columns["TransactionPidNew"].Hidden = true;
      //e.Layout.Bands[0].Columns["PidNew"].Hidden = true;
      //e.Layout.Bands[0].Columns["CARDeadlineNew"].Hidden = true;
      //e.Layout.Bands[0].Columns["COM1DeadlineNew"].Hidden = true;
      //e.Layout.Bands[0].Columns["SUBCONDeadlineNew"].Hidden = true;
      //e.Layout.Bands[0].Columns["ASSYDeadlineNew"].Hidden = true;
      //e.Layout.Bands[0].Columns["ASSHWDeadlineNew"].Hidden = true;
      //e.Layout.Bands[0].Columns["FFHWDeadlineNew"].Hidden = true;
      //e.Layout.Bands[0].Columns["MATDeadlineNew"].Hidden = true;

      e.Layout.Bands[0].Columns["Error"].Hidden = true;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (CheckVaild())
      {
        btnSave.Enabled = false;
        bool success = this.SaveData();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          btnSave.Enabled = true;
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnPath_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      txtPath.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      this.ImportData();
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      this.GetTemplate();
    }
    #endregion Event
  }
}
