/*
  Author      : XUÂN TRỪƠNG
  Date        : 22/08/2013
  Description : Coefficient For User IT
  Standard Form : viewGNR_90_008 
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.Utility;

namespace DaiCo.General
{
  public partial class viewGNR_50_001 : MainUserControl
  {
    #region Field
    //private IList listDeleting = new ArrayList();
    //private IList listDeleted = new ArrayList();
    #endregion Field

    #region Init

    /// <summary>
    /// Itit Form
    /// </summary>
    public viewGNR_50_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_50_001_Load(object sender, EventArgs e)
    {
      // Load User IT
      this.LoadComboUserID();
      // Load Grid
      this.LoadGrid();
    }
    #endregion Init

    #region Function

    /// <summary>
    /// Load UltraCombo Supplier
    /// </summary>
    private void LoadComboUserID()
    {
      string commandText = string.Empty;
      commandText += "SELECT EM.Pid UserID, CONVERT(VARCHAR, EM.Pid) + ' - ' + EM.EmpName UserName ";
      commandText += "FROM VHRMEmployee EM ";
      commandText += "  INNER JOIN TblBOMCodeMaster CM ON CM.[Group] = 9200 ";
      commandText += "                      AND EM.Department = CM.Value ";
     
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultdrpUserIT.DataSource = dtSource;
        ultdrpUserIT.DisplayMember = "UserName";
        ultdrpUserIT.ValueMember = "UserID";
        // Format Grid
        ultdrpUserIT.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultdrpUserIT.DisplayLayout.Bands[0].Columns["UserName"].Width = 250;
        ultdrpUserIT.DisplayLayout.Bands[0].Columns["UserID"].Hidden = true;
      }
    }

    /// <summary>
    /// Load UltraGrid 
    /// </summary>
    private void LoadGrid()
    {
      //Load Data On Grid
      string commandText = string.Empty;
      commandText += " SELECT COE.Pid, COE.UserPid, COE.Coefficient, 0 Selected";
      commandText += " FROM TblGNRProjectCoefficient COE";
      commandText += " 	  INNER JOIN VHRMEmployee EM ON COE.UserPid = EM.Pid";
      commandText += "    INNER JOIN TblBOMCodeMaster CM ON CM.[Group] = 9200 ";
      commandText += "                      AND EM.Department = CM.Value ";
      commandText += " ORDER BY COE.Pid";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultData.DataSource = dtSource;
    }

    /// <summary>
    /// Check Input
    /// </summary>
    /// <param name="warehouse"></param>
    /// <param name="dtSource"></param>
    /// <returns></returns>
    private bool CheckVaild(out string message)
    {
      message = string.Empty;
      for(int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        // Check UserID
        if(row.Cells["UserPid"].Text.Trim().Length == 0 || row.Cells["UserPid"].Value == null)
        {
          message = "Staff";
          return false;
        }

        // Check Coefficient
        if(row.Cells["Coefficient"].Text.Trim().Length == 0 || 
                DBConvert.ParseDouble(row.Cells["Coefficient"].Value.ToString()) <= 0)
        {
          message = "Coefficient";
          return false;
        }

        // Check Double
        if (row.Cells["UserPid"].Value.ToString().Length > 0 && DBConvert.ParseInt(row.Cells["UserPid"].Value.ToString()) != int.MinValue)
        {
          string select = string.Format(@"UserPid = {0}", DBConvert.ParseInt(row.Cells["UserPid"].Value.ToString()));
          DataTable dt = (DataTable)ultData.DataSource;
          DataRow[] foundRow = dt.Select(select);
          if (foundRow.Length > 1)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0013"), "Staff");
            return false;
          }
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
      bool success = true;
      DataTable dtSource = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        if(row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          // Input
          DBParameter[] inputParam = new DBParameter[3];
          if (DBConvert.ParseLong(row["Pid"].ToString()) != long.MinValue)
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));
          }
          inputParam[1] = new DBParameter("@UserPid", DbType.Int32, DBConvert.ParseInt(row["UserPid"].ToString()));
          inputParam[2] = new DBParameter("@Coefficient", DbType.Double, DBConvert.ParseDouble(row["Coefficient"].ToString()));
          // Output
          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          // Execute
          DataBaseAccess.ExecuteStoreProcedure("spGNRProjectCoefficient_Edit", inputParam, outputParam);
          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result <= 0)
          {
            success = false;
          }
          else
          {
            success = true;
          }
        }
      }
      return success;
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Set caption column
      e.Layout.Bands[0].Columns["UserPid"].Header.Caption = "Staff";
      
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["UserPid"].ValueList = ultdrpUserIT;

      // Set Column Style
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].DefaultCellValue = 0;

      // Set Width
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 100;

      // Back Color
      e.Layout.Bands[0].Columns["UserPid"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Coefficient"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Selected"].CellAppearance.BackColor = Color.LightBlue;

      // Set Align
      e.Layout.Bands[0].Columns["Coefficient"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// btnSave Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }

      // Load Grid
      this.LoadGrid();
    }

    /// <summary>
    /// btnClose Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    /// <summary>
    /// Delete Group
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      // Delete
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if(DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1 &&
            DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()) > 0)
        {
          // Input
          DBParameter[] inputParam = new DBParameter[1];
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
          // Output
          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          // Execute
          DataBaseAccess.ExecuteStoreProcedure("spGNRProjectCoefficient_Delete", inputParam, outputParam);
          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result <= 0)
          {
            // Error
            WindowUtinity.ShowMessageError("ERR0004");
            return;
          }
          else 
          {
            // Success
            WindowUtinity.ShowMessageSuccess("MSG0002");
          }
        }
      }
      // Load Data
      this.LoadGrid();
    }
    #endregion Event

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      switch (columnName)
      {
        case "userpid":
          if (value.Length > 0 && DBConvert.ParseInt(value) != int.MinValue)
          {
            string select = string.Format(@"UserPid = {0}", DBConvert.ParseInt(value));
            DataTable dt = (DataTable)ultData.DataSource;
            DataRow[] foundRow = dt.Select(select);
            if (foundRow.Length >= 1)
            {
              string message = string.Format(FunctionUtility.GetMessage("ERR0013"), "Staff");
              WindowUtinity.ShowMessageErrorFromText(message);
            }
          }
          break;
        default:
          break;
      }
    }
  }
}
