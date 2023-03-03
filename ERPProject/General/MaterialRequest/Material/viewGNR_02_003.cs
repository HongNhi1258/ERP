/*
  Author      : Ha Anh
  Date        : 02/06/2014
  Description : Request material by import excel
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

namespace DaiCo.ERPProject
{
  public partial class viewGNR_02_003 : MainUserControl
  {
    #region Field
    public long requestPid = long.MinValue;
    public int typeRequest = int.MinValue;
    public int recovery = int.MinValue;

    private IList listDeletedPid = new ArrayList();

    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;
    #endregion Field

    #region Init

    public viewGNR_02_003()
    {
      InitializeComponent();
    }

    private void viewGNR_02_003_Load(object sender, EventArgs e)
    {
      this.LoadInit();
    }

    #endregion Init

    #region Function

    private void LoadInit()
    {
      string commandText = "SELECT	'' Material, 0 Wo, '' ItemCode, 0 Revision, CAST(0 as float) Remain, CAST(0 as float) Qty, 0 Kind, 0 Error WHERE 0 = 1";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultData.DataSource = dtSource;
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
        UltraGridRow row = ultData.Rows[i];
        DBParameter[] inputParam = new DBParameter[12];

        inputParam[0] = new DBParameter("@MRNPid", DbType.Int64, this.requestPid);

        if (DBConvert.ParseLong(row.Cells["Wo"].Value.ToString()) != long.MinValue)
        {
          inputParam[1] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(row.Cells["Wo"].Value.ToString()));
        }
        if (row.Cells["ItemCode"].Text.Length > 0)
        {
          inputParam[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, row.Cells["ItemCode"].Value.ToString());
        }
        if (row.Cells["Revision"].Text.Length > 0)
        {
          inputParam[3] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
        }

        inputParam[5] = new DBParameter("MaterialCode", DbType.AnsiString, 16, row.Cells["Material"].Value.ToString());

        if (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) != long.MinValue)
        {
          inputParam[6] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()));
        }

        inputParam[7] = new DBParameter("@Type", DbType.Int32, DBConvert.ParseInt(row.Cells["Kind"].Value.ToString()));

        inputParam[8] = new DBParameter("@Recovery", DbType.Int32, recovery);

        inputParam[9] = new DBParameter("@DepartmentRequest", DbType.AnsiString, 8, SharedObject.UserInfo.Department);

        inputParam[10] = new DBParameter("@Warehouse", DbType.Int32, ConstantClass.MATERIALS_WAREHOUSE);

        //if (DBConvert.ParseLong(row.Cells["SupplementPid"].Value.ToString()) != long.MinValue)
        //{
        //  inputParam[11] = new DBParameter("@SupplementDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["SupplementPid"].Value.ToString()));
        //}

        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialRequisitionNoteDetail_Edit", inputParam, outputParam);
        long success = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (success <= 0)
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
      //e.Layout.Reset();
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
      //e.Layout.Bands[0].Columns["SupplementPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Kind"].Hidden = true;

      // Set dropdownlist for column
      //e.Layout.Bands[0].Columns["Department"].ValueList = ultDDDepartment;
      //e.Layout.Bands[0].Columns["Material"].ValueList = ultDDMaterial;

      // Read only
      //e.Layout.Bands[0].Columns["Supplement"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Material"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remain"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Wo"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "qty":
          e.Cell.Row.Cells["Error"].Value = 0;

          e.Cell.Row.Appearance.BackColor = Color.White;

          break;
        default:
          break;
      }
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      switch (columnName.ToLower())
      {
        case "qty":

          if (DBConvert.ParseDouble(text) <= 0 || DBConvert.ParseDouble(text) > DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Text))
          {
            WindowUtinity.ShowMessageError("ERR0001", "Qty");
            e.Cancel = true;
          }

          break;
        default:
          break;
      }
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
      string templateName = string.Empty;
      string sheetName = string.Empty;
      string outFileName = string.Empty;

      if (this.typeRequest == 0)
      {
        templateName = "RPT_GNR_02_003_01";
        sheetName = "Sheet1";
        outFileName = "RPT_GNR_02_003_01";
      }
      else
      {
        templateName = "RPT_GNR_02_003_02";
        sheetName = "Sheet1";
        outFileName = "RPT_GNR_02_003_02";
      }

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
        DataTable dtSource;
        if (this.typeRequest == 0)
        {
          dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B4:E505]").Tables[0];
        }
        else
        {
          dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B4:C505]").Tables[0];
        }
        if (dtSource == null)
        {
          return;
        }
        foreach (DataRow drSource in dtSource.Rows)
        {
          if (drSource["Material"].ToString().Trim().Length > 0)
          {
            if (DBConvert.ParseDouble(drSource["Qty"].ToString()) == Double.MinValue)
            {
              drSource["Qty"] = 0;
            }
          }
        }

        DataTable dtMain = (DataTable)this.ultData.DataSource;

        foreach (DataRow drMain in dtSource.Rows)
        {
          if (drMain["Material"].ToString().Trim().Length > 0)
          {
            if (this.typeRequest == 0)
            {
              DBParameter[] inputParam = new DBParameter[8];
              inputParam[0] = new DBParameter("@Material", DbType.AnsiString, 16, drMain["Material"].ToString());
              if (DBConvert.ParseLong(drMain["Wo"].ToString()) > 0)
              {
                inputParam[1] = new DBParameter("@Wo", DbType.Int64, DBConvert.ParseLong(drMain["Wo"].ToString()));
              }
              if (drMain["ItemCode"].ToString().Length > 0)
              {
                inputParam[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, drMain["ItemCode"].ToString());
              }
              //if (drMain["Supplement"].ToString().Length > 0)
              //{
              //  inputParam[3] = new DBParameter("@Supplement", DbType.AnsiString, 32, drMain["Supplement"].ToString());
              //}
              inputParam[4] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(drMain["Qty"].ToString()));
              inputParam[5] = new DBParameter("@Recovery", DbType.Int32, recovery);
              inputParam[6] = new DBParameter("@Warehouse", DbType.Int32, ConstantClass.MATERIALS_WAREHOUSE);
              inputParam[7] = new DBParameter("@Type", DbType.Int32, 0);

              DataTable dtSearch = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMRNImportData_Search", inputParam);

              for (int i = 0; i < dtSearch.Rows.Count; i++)
              {
                DataRow row = dtMain.NewRow();

                row["Material"] = dtSearch.Rows[i]["Material"].ToString();
                row["Remain"] = DBConvert.ParseDouble(dtSearch.Rows[i]["Remain"].ToString());
                row["Qty"] = DBConvert.ParseDouble(dtSearch.Rows[i]["Qty"].ToString());
                row["Error"] = DBConvert.ParseInt(dtSearch.Rows[i]["Error"].ToString());
                if (DBConvert.ParseLong(dtSearch.Rows[i]["Wo"].ToString()) > 0)
                {
                  row["Wo"] = DBConvert.ParseLong(dtSearch.Rows[i]["Wo"].ToString());
                }
                if (dtSearch.Rows[i]["ItemCode"].ToString().Length > 0)
                {
                  row["ItemCode"] = dtSearch.Rows[i]["ItemCode"].ToString();
                }
                if (DBConvert.ParseInt(dtSearch.Rows[i]["Revision"].ToString()) > 0)
                {
                  row["Revision"] = DBConvert.ParseInt(dtSearch.Rows[i]["Revision"].ToString());
                }
                row["Kind"] = DBConvert.ParseInt(dtSearch.Rows[i]["Kind"].ToString());
                //row["Supplement"] = dtSearch.Rows[i]["Supplement"].ToString();
                //if (DBConvert.ParseLong(dtSearch.Rows[i]["SupplementPid"].ToString()) > 0)
                //{
                //  row["SupplementPid"] = DBConvert.ParseLong(dtSearch.Rows[i]["SupplementPid"].ToString());
                //}

                string select = string.Format(@"Material = '{0}' AND Remain = '{1}' AND Qty = '{2}' AND Wo = '{3}' AND
                                            ItemCode = '{4}' AND Revision = '{5}' ",
                                              row["Material"].ToString(), row["Remain"].ToString(), row["Qty"].ToString(), row["Wo"].ToString(),
                                              row["ItemCode"].ToString(), row["Revision"].ToString());
                DataRow[] foundRows = dtMain.Select(select);
                if (foundRows.Length >= 1)
                {
                  row["Error"] = 1;
                }
                else
                {
                  row["Error"] = DBConvert.ParseInt(row["Error"].ToString());
                }

                dtMain.Rows.Add(row);
              }

            }
            else
            {
              DBParameter[] inputParam = new DBParameter[6];
              inputParam[0] = new DBParameter("@Material", DbType.AnsiString, 16, drMain["Material"].ToString());
              inputParam[1] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(drMain["Qty"].ToString()));
              inputParam[2] = new DBParameter("@Recovery", DbType.Int32, recovery);
              inputParam[3] = new DBParameter("@Warehouse", DbType.Int32, ConstantClass.MATERIALS_WAREHOUSE);
              inputParam[4] = new DBParameter("@Type", DbType.Int32, 1);
              inputParam[5] = new DBParameter("@Department", DbType.AnsiString, 32, SharedObject.UserInfo.Department);

              DataTable dtSearch = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMRNImportData_Search", inputParam);

              for (int i = 0; i < dtSearch.Rows.Count; i++)
              {
                DataRow row = dtMain.NewRow();
                row["Material"] = dtSearch.Rows[i]["Material"].ToString();
                row["Remain"] = DBConvert.ParseDouble(dtSearch.Rows[i]["Remain"].ToString());
                row["Qty"] = DBConvert.ParseDouble(dtSearch.Rows[i]["Qty"].ToString());
                row["Kind"] = DBConvert.ParseInt(dtSearch.Rows[i]["Kind"].ToString());
                row["Error"] = DBConvert.ParseInt(dtSearch.Rows[i]["Error"].ToString());

                string select = string.Format(@"Material = '{0}' AND Remain = '{1}' AND Qty = '{2}'", row["Material"].ToString(), row["Remain"].ToString(), row["Qty"].ToString());
                DataRow[] foundRows = dtMain.Select(select);
                if (foundRows.Length >= 1)
                {
                  row["Error"] = 1;
                }
                else
                {
                  row["Error"] = DBConvert.ParseInt(row["Error"].ToString());
                }

                dtMain.Rows.Add(row);
              }
            }
          }
        }

        //foreach (DataRow dr in dtMain)
        //{
        //  DataRow row = dtMain.NewRow();
        //  // 3: Check Trung(WO, Carcass, Component)
        //  string select = string.Format(@"Department = '{0}' AND Material = '{1}'", dr["Department"].ToString(), dr["Material"].ToString());
        //  DataRow[] foundRows = dtMain.Select(select);
        //  if (foundRows.Length >= 1)
        //  {
        //    row["Error"] = 1;
        //  }
        //  else
        //  {
        //    row["Error"] = DBConvert.ParseInt(dr["Error"].ToString());
        //  }
        //  row["Department"] = dr["Department"].ToString();
        //  row["Material"] = dr["Material"].ToString();
        //  row["Remain"] = DBConvert.ParseDouble(dr["Remain"].ToString());
        //  row["AdjustQty"] = DBConvert.ParseDouble(dr["AdjustQty"].ToString());
        //  row["Remark"] = dr["Remark"].ToString();
        //  // Add Row
        //  dtMain.Rows.Add(row);
        //}

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
      dt.Columns.Add("Material", typeof(System.String));
      dt.Columns.Add("Wo", typeof(System.Int64));
      dt.Columns.Add("ItemCode", typeof(System.String));
      //dt.Columns.Add("Supplement", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Double));
      return dt;
    }
    #endregion Event
  }
}
