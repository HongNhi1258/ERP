/*
  Author      : Ha Anh
  Date        : 30/05/2014
  Description : Adjust material by import excel
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
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_07_105 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    private int status = int.MinValue;
    private IList listDeletedPid = new ArrayList();

    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;
    #endregion Field

    #region Init

    public viewPLN_07_105()
    {
      InitializeComponent();
    }

    private void viewPLN_07_105_Load(object sender, EventArgs e)
    {
      this.LoadInit();
    }

    #endregion Init

    #region Function

    private void LoadInit()
    {
      string commandText = "SELECT	'' Department, '' Material, CONVERT(float, 0) Remain, CONVERT(float, 0) AdjustQty, '' Remark, 0 Error WHERE 0 = 1";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultData.DataSource = dtSource;

      commandText = "SELECT Department, Department + ' | ' +  DeparmentName Name FROM VHRDDepartment WHERE DelFlag = 0";
      dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDDDepartment.DataSource = dtSource;
      if (dtSource != null)
      {
        ultDDDepartment.ValueMember = "Department";
        ultDDDepartment.DisplayMember = "Name";
      }

      // Material Code
      commandText = "SELECT MaterialCode, MaterialCode + ' | '+ MaterialNameEn  Name FROM VBOMMaterials WHERE IsControl = 1";
      dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDDMaterial.DataSource = dtSource;
      if (dtSource != null)
      {
        ultDDMaterial.ValueMember = "MaterialCode";
        ultDDMaterial.DisplayMember = "Name";
      }
    }

    private bool CheckVaild(out string message)
    {
      message = string.Empty;
      if (ultData.DataSource == null || ultData.Rows.Count == 0)
      {
        message = "Data in Grid";
        return false;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 1)
        {
          message = "Data in Grid";
          return false;
        }
      }
      return true;
    }

    private bool SaveData()
    {
      // Save master info
      bool success = this.SaveInfo();
      if (success)
      {
        success = true;
      }
      else
      {
        success = false;
      }
      return success;
    }

    private bool SaveInfo()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string storeName = "spPLNAllocateForDepartment_Insert";
        DBParameter[] inputParam = new DBParameter[5];
        inputParam[0] = new DBParameter("@Department", DbType.AnsiString, 50, ultData.Rows[i].Cells["Department"].Value.ToString());
        inputParam[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, ultData.Rows[i].Cells["Material"].Value.ToString());
        inputParam[2] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(DBConvert.ParseDouble(ultData.Rows[i].Cells["AdjustQty"].Value.ToString())));
        inputParam[3] = new DBParameter("@Remark", DbType.AnsiString, 4000, ultData.Rows[i].Cells["Remark"].Value.ToString());
        inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);

        if (DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0)
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

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      //Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;

      // Hide column
      e.Layout.Bands[0].Columns["Error"].Hidden = true;

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["Department"].ValueList = ultDDDepartment;
      e.Layout.Bands[0].Columns["Material"].ValueList = ultDDMaterial;

      // Read only
      e.Layout.Bands[0].Columns["Department"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Material"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remain"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      //string columnName = e.Cell.Column.ToString().ToLower();
      //switch (columnName)
      //{
      //  case "location":
      //    if (DBConvert.ParseLong(e.Cell.Row.Cells["Location"].Value.ToString()) <= 0)
      //    {
      //      WindowUtinity.ShowMessageError("ERR0001", "Location");
      //      return;
      //    }
      //    break;
      //  default:
      //    break;
      //}
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string columnName = e.Cell.Column.ToString();
      //string text = e.Cell.Text.Trim();
      //switch (columnName.ToLower())
      //{
      //  case "qty":
      //    if (text.Length > 0)
      //    {
      //      if (DBConvert.ParseDouble(text) <= 0)
      //      {
      //        WindowUtinity.ShowMessageError("ERR0001", "Qty");
      //        e.Cancel = true;
      //      }
      //    }
      //    break;
      //  default:
      //    break;
      //}
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

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      // Check Valid
      bool success = this.CheckVaild(out message);
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
        this.btnSave.Enabled = false;
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }

      //Load Data
      //this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Brown The Link
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
    /// Get Template
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "AdjustMaterialQty";
      string sheetName = "Sheet1";
      string outFileName = "AdjustMaterialQty";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    /// <summary>
    /// Import Data
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
        DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B4:E505]").Tables[0];
        if (dtSource == null)
        {
          return;
        }
        foreach (DataRow drSource in dtSource.Rows)
        {
          if (drSource["Department"].ToString().Trim().Length > 0)
          {
            if (DBConvert.ParseDouble(drSource["AdjustQty"].ToString()) == Double.MinValue)
            {
              drSource["AdjustQty"] = 0;
            }
          }
        }
        // 1: Create Table Import
        DataTable dtNew = this.CreateDataTable();
        foreach (DataRow drMain in dtSource.Rows)
        {
          if (drMain["Department"].ToString().Trim().Length > 0)
          {
            DataRow row = dtNew.NewRow();
            row["Department"] = drMain["Department"].ToString();
            row["Material"] = drMain["Material"].ToString();
            row["AdjustQty"] = DBConvert.ParseDouble(drMain["AdjustQty"].ToString());
            row["Remark"] = drMain["Remark"].ToString();
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
        foreach (DataRow dr in dtResult.Rows)
        {
          DataRow row = dtMain.NewRow();
          // 3: Check Trung(WO, Carcass, Component)
          string select = string.Format(@"Department = '{0}' AND Material = '{1}'", dr["Department"].ToString(), dr["Material"].ToString());
          DataRow[] foundRows = dtMain.Select(select);
          if (foundRows.Length >= 1)
          {
            row["Error"] = 1;
          }
          else
          {
            row["Error"] = DBConvert.ParseInt(dr["Error"].ToString());
          }
          row["Department"] = dr["Department"].ToString();
          row["Material"] = dr["Material"].ToString();
          row["Remain"] = DBConvert.ParseDouble(dr["Remain"].ToString());
          row["AdjustQty"] = DBConvert.ParseDouble(dr["AdjustQty"].ToString());
          row["Remark"] = dr["Remark"].ToString();
          // Add Row
          dtMain.Rows.Add(row);

          // 4: Gan Lai Luoi
          ultData.DataSource = dtMain;
          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            UltraGridRow row1 = ultData.Rows[i];
            if (DBConvert.ParseInt(row1.Cells["Error"].Value.ToString()) == 1)
            {
              row1.Appearance.BackColor = Color.YellowGreen;
            }
            else
            {
              row1.Appearance.BackColor = Color.White;
            }
          }
        }
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0105");
        return;
      }
    }

    private DataTable GetDataTableImport(DataTable dtImport)
    {
      SqlDBParameter[] input = new SqlDBParameter[1];
      input[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtImport);
      DataTable dtMain = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNMaterialAdjustmentQty_Select", input);
      return dtMain;
    }

    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Department", typeof(System.String));
      dt.Columns.Add("Material", typeof(System.String));
      dt.Columns.Add("AdjustQty", typeof(System.Double));
      dt.Columns.Add("Remark", typeof(System.String));
      return dt;
    }
    #endregion Event
  }
}
