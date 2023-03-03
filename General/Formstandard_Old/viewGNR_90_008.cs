/*
  Author      : 
  Date        : 18/01/2013
  Description : Register on Grid
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
  public partial class viewGNR_90_008 : MainUserControl
  {
    #region Field
    private IList listDeleting = new ArrayList();
    private IList listDeleted = new ArrayList();
    #endregion Field

    #region Init

    /// <summary>
    /// Itit Form
    /// </summary>
    public viewGNR_90_008()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_90_008_Load(object sender, EventArgs e)
    {
      // Load Department
      this.LoadGrid();
    }
    #endregion Init

    #region Function

    ///// <summary>
    ///// Load UltraCombo Supplier
    ///// </summary>
    //private void LoadCombo()
    //{
    //  string commandText = string.Empty;
    //  commandText += " ....";
    //  DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
    //  if (dtSource != null)
    //  {
    //    ultControl.DataSource = dtSource;
    //    ultControl.DisplayMember = "...";
    //    ultControl.ValueMember = "...";
    //    // Format Grid
    //    ultControl.DisplayLayout.Bands[0].ColHeadersVisible = false;
    //    ultControl.DisplayLayout.Bands[0].Columns["..."].Width = 250;
    //    ultControl.DisplayLayout.Bands[0].Columns["..."].Hidden = true;
    //  }
    //}

    /// <summary>
    /// Load UltraGrid 
    /// </summary>
    private void LoadGrid()
    {
      //this.listDeleting = new ArrayList();
      //this.listDeleted = new ArrayList();

      // Load Data On Grid

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

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      /*
      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      
      // Hide column
      e.Layout.Bands[0].Columns[""].Hidden = true;
      
      // Set caption column
      e.Layout.Bands[0].Columns[""].Header.Caption = "\n";
      
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;
      
      // Set Align
      e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
      
      // Set Width
      e.Layout.Bands[0].Columns[""].MaxWidth = 100;
      e.Layout.Bands[0].Columns[""].MinWidth = 100;
      
      // Set Column Style
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      
      // Set color
      ultraGridInformation.Rows[0].Appearance.BackColor = Color.Yellow;
      ultraGridInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;
      
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      
      // Read only
      e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      
      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      */

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
      // Delete
    }
    #endregion Event   
  }
}
