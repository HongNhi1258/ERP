/*
  Author      : 
  Date        : 18/01/2013
  Description : Register Department
  Standard Form : view_Master_OneGridSave.cs  
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
  public partial class view_Master_OneGridSave : MainUserControl
  {
    #region Field
    private IList listDeleting = new ArrayList();
    private IList listDeleted = new ArrayList();
    #endregion Field

    #region Init

    /// <summary>
    /// Itit Form
    /// </summary>
    public view_Master_OneGridSave()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_01_001_Load(object sender, EventArgs e)
    {
      // Load Department
      this.LoadGrid();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Load UltraGrid 
    /// </summary>
    private void LoadGrid()
    {
      //this.listDeleting = new ArrayList();
      //this.listDeleted = new ArrayList();

      //string commandText = string.Empty;
      //commandText += " SELECT Code, Value, [Description], 0 [Selected] ";
      //commandText += " FROM TblBOMCodeMaster ";
      //commandText += " WHERE [Group] = 99001 ";
      //commandText += "  	AND DeleteFlag = 0 ";

      //DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      //ultData.DataSource = dtSource;
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

      // Check Info

      // Check Detail

      return true;
    }

    /// <summary>
    /// Check Delete
    /// </summary>
    /// <returns></returns>
    private bool ValidateDelete()
    {
      bool result = true;
      //for (int i = 0; i < ultData.Rows.Count; i++)
      //{
      //  int selected = DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString());
      //  if (selected == 1)
      //  {
      //    string commandText = string.Empty;
      //    commandText += " SELECT COUNT(*) ";
      //    commandText += " FROM TblBOMCodeMaster ";
      //    commandText += " WHERE [Group] = 99002 ";
      //    commandText += "     AND Kind =" + DBConvert.ParseInt(ultData.Rows[i].Cells["Code"].Value.ToString());
      //    DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
      //    if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) > 0)
      //    {
      //      WindowUtinity.ShowMessageError("ERR0054", ultData.Rows[i].Cells["Value"].Value.ToString());
      //      result = false;
      //      break;
      //    }
      //  }
      //}
      return result;
    }

    private bool SaveData()
    {
      bool success = false;
      //// Save master info
      //bool success = this.SaveInfo();
      //if (success)
      //{
      //  // Save detail
      //  success = this.SaveDetail();
      //}
      //else
      //{
      //  success = false;
      //}

      //foreach (DataRow row in dtSource.Rows)
      //{
      //  string storeName = string.Empty;
      //  DBParameter[] input = new DBParameter[3];
      //  DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };

      //  if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
      //  {
      //    string value = row["Value"].ToString().Trim();
      //    string remark = row["Description"].ToString().Trim();

      //    input[0] = new DBParameter("@Value", DbType.String, 128, value);
      //    input[1] = new DBParameter("@Remark", DbType.String, 128, remark);

      //    if (row.RowState == DataRowState.Added)
      //    {
      //      input[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      //      storeName = "spGNRITLogDepartment_Insert";
      //    }
      //    else if (row.RowState == DataRowState.Modified)
      //    {
      //      storeName = "spGNRITLogDepartment_Update";
      //    }

      //    if (storeName.Length > 0)
      //    {
      //      DataBaseAccess.ExecuteStoreProcedure(storeName, input);
      //    }
      //  }
      //}
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
      //e.Layout.Reset();
      //e.Layout.AutoFitColumns = true;
      //e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      //e.Layout.ScrollStyle = ScrollStyle.Immediate;

      //e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      //e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;
      //e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      //e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      //e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      //e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      //e.Layout.Bands[0].Columns["Selected"].DefaultCellValue = 0;
      //for (int i = 0 ; i < ultData.Rows.Count; i++)
      //{
      //  string value = ultData.Rows[i].Cells["Value"].Value.ToString();
      //  if (value.Length > 0)
      //  {
      //    ultData.Rows[i].Cells["Value"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      //  }    
      //}

      //e.Layout.Bands[0].Columns["Value"].Header.Caption = "Module";
      //e.Layout.Bands[0].Columns["Code"].Hidden = true;

      //e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      //e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      //e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      //e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
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
    /// ultData Mouse Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      //if (ultData.Rows.Count > 0 && ultData.Selected.Rows.Count > 0)
      //{
      //  string value = ultData.Selected.Rows[0].Cells["Value"].Value.ToString().Trim();
      //  int code = DBConvert.ParseInt(ultData.Selected.Rows[0].Cells["Code"].Value.ToString().Trim());
      //  viewGNR_99_002 uc = new viewGNR_99_002();
      //  uc.kind = code;
      //  uc.department = value;
      //  Shared.Utility.WindowUtinity.ShowView(uc, "FUNCTION", false, DaiCo.Shared.Utility.ViewState.MainWindow);
      //}
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
      //if (ultData.Rows.Count > 0)
      //{
      //  int countCheck = 0;
      //  for (int i = 0; i < ultData.Rows.Count; i++)
      //  {
      //    int selected = DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString());
      //    if (selected == 1)
      //    {
      //      countCheck++;
      //    }
      //  }

      //  if (countCheck == 0)
      //  {
      //    return;
      //  }

      //  DialogResult result = WindowUtinity.ShowMessageConfirm("MSG0015");
      //  if (result == DialogResult.Yes)
      //  {
      //    bool check = ValidateDelete();
      //    if (check)
      //    {
      //      for (int i = 0; i < ultData.Rows.Count; i++)
      //      {
      //        int selected = DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString());
      //        if (selected == 1)
      //        {
      //          int code = DBConvert.ParseInt(ultData.Rows[i].Cells["Code"].Value.ToString());
      //          DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Code", DbType.Int32, 
      //                  DBConvert.ParseInt(ultData.Rows[i].Cells["Code"].Value.ToString())) };
      //          DataBaseAccess.ExecuteStoreProcedure("spGNRITLogDepartment_Delete", inputParam);
      //        }
      //      }
      //      WindowUtinity.ShowMessageSuccess("MSG0002");
      //    }          
      //    // Load Grid
      //    this.LoadGrid();
      //  }
      //}
    }
    #endregion Event   
  }
}
