/*
  Author      : 
  Date        : 19/8/2013
  Description : Search Data And Before Save
  Standard From : viewGNR_90_003
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System.Collections;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared;
using System.Diagnostics;
using CrystalDecisions.CrystalReports.Engine;
using VBReport;

namespace DaiCo.General
{
  public partial class viewGNR_90_003 : MainUserControl
  {
    #region Field

    //string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

    #endregion Field

    #region Init

    /// <summary>
    /// viewGNR_90_003
    /// </summary>
    public viewGNR_90_003()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load
    /// </summary>
    private void viewGNR_90_003_Load(object sender, EventArgs e)
    {
      // Init Data Control
    }

    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadItemCode()
    {
      //string commandText = string.Empty;
      //commandText += " SELECT DISTINCT ItemCode ";
      //commandText += " FROM TblPLNMasterPlanContainerMainData ";
      //commandText += " ORDER BY ItemCode ";

      //DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      //ucItemCode.DataSource = dtItem;
      //ucItemCode.ColumnWidths = "200";
      //ucItemCode.DataBind();
      //ucItemCode.ValueMember = "ItemCode";
    }

    #endregion Init

    #region Function

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      this.btnSearch.Enabled = false;
      string storeName = string.Empty;

      //DBParameter[] param = new DBParameter[11];
      //param[5] = new DBParameter("@OldCode", DbType.String, text);

      //DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      //ultraGridInformation.DataSource = dtSource;

      //lbCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
      btnSearch.Enabled = true;
    }

    /// <summary>
    ///  Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      //for (int i = 0; i < ultDetail.Rows.Count; i++)
      //{
      //  UltraGridRow row = ultDetail.Rows[i];
      //  if (DBConvert.ParseInt(row.Cells["Location"].Value.ToString()) == int.MinValue)
      //  {
      //    message = "Location";
      //    return false;
      //  }
      //}
      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      if (ultData.Rows.Count > 0)
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow rowInfo = ultData.Rows[i];
          if (DBConvert.ParseInt(rowInfo.Cells["Selected"].Value.ToString()) == 1)
          {
            if (DBConvert.ParseDouble(rowInfo.Cells["Qty"].Value.ToString()) != 0)
            {
              DBParameter[] inputParam = new DBParameter[6];

              //inputParam[0] = new DBParameter("@ReceivingNo", DbType.AnsiString, 50, this.receivingNo);
              //inputParam[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 255, rowInfo.Cells["MaterialCode"].Value.ToString());
              //inputParam[2] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(rowInfo.Cells["Qty"].Value.ToString()));
              //inputParam[3] = new DBParameter("@WarehousePid", DbType.Int32, 1);
              //inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              //inputParam[5] = new DBParameter("@PONo", DbType.AnsiString, 16, rowInfo.Cells["PONo"].Value.ToString());

              DBParameter[] outputParam = new DBParameter[1];
              //outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

              DataBaseAccess.ExecuteStoreProcedure("StoreName", inputParam, outputParam);
              long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
              if (result == 0)
              {
                return false;
              }
            }
          }
        }
      }
      return true;
    }
    #endregion Function

    #region Event

    /// <summary>
    /// Init Layout 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

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
    /// Search 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
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

    /// <summary>
    /// Save
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckValid(out message);
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
        WindowUtinity.ShowMessageError("WRN0004");
      }

      // Search Data
      this.Search();
    }

    /// <summary>
    /// ProcessCmdKey
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    #endregion Event
  }
}
