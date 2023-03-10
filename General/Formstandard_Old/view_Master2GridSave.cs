/*
  Author      : TRAN HUNG
  Date        : 21/02/2013
  Description : 
  Standard From : view_Master2GridSave.cs
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

namespace DaiCo.Planning
{
  public partial class viewGNR_05_003 : MainUserControl
  {
    #region Field

    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

    #endregion Field

    #region Init

    /// <summary>
    /// viewGNR_05_003
    /// </summary>
    public viewGNR_05_003()
    {
      InitializeComponent();
    }

    /// <summary>
    /// viewWHD_05_002_Load
    /// </summary>
    private void viewWHD_05_002_Load(object sender, EventArgs e)
    {
     
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

    }

    /// <summary>
    /// Set Status Column When Search
    /// </summary>
    private void SetStatusColumn()
    {
      for (int colIndex = 1; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
        UltraGridCell cell = ultShowColumn.Rows[0].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
        }
      }
    }

    /// <summary>
    /// LoadColumnName
    /// </summary>
    private void LoadColumnName()
    {
      DataTable dtNew = new DataTable();
      DataTable dtColumn = (DataTable)ultData.DataSource;
      dtNew.Columns.Add("All", typeof(Int32));
      dtNew.Columns["All"].DefaultValue = 0;
      foreach (DataColumn column in dtColumn.Columns)
      {
        dtNew.Columns.Add(column.ColumnName, typeof(Int32));
        dtNew.Columns[column.ColumnName].DefaultValue = 0;

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
      }
      DataRow row = dtNew.NewRow();
      dtNew.Rows.Add(row);
      ultShowColumn.DataSource = dtNew;
      ultShowColumn.Rows[0].Appearance.BackColor = Color.LightBlue;
    }

    /// <summary>
    /// Hiden/Show ultragrid columns 
    /// </summary>
    private void ChkAll_CheckedChange()
    {
      for (int colIndex = 1; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
        UltraGridCell cell = ultShowColumn.Rows[0].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
        }
      }
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

    }

    /// <summary>
    /// Search 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
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

    /// <summary>
    /// ucItemCode_ValueChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ucItemCode_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtItemCode.Text = this.ucItemCode.SelectedValue;
    }

    /// <summary>
    /// ucCustomer_ValueChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ucCustomer_ValueChanged(object source, ValueChangedEventArgs args)
    {
     
    }

    /// <summary>
    /// chkItemCode_CheckedChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkItemCode_CheckedChanged(object sender, EventArgs e)
    {
      ucItemCode.Visible = chkItemCode.Checked;
    }

    /// <summary>
    /// chkCustomer_CheckedChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkCustomer_CheckedChanged(object sender, EventArgs e)
    {
     
    }

    /// <summary>
    /// ucItemCode_ValueChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultShowColumn_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      DataTable dtColumn = (DataTable)ultShowColumn.DataSource;
      int count = dtColumn.Columns.Count;
      for (int i = 0; i < count; i++)
      {
        e.Layout.Bands[0].Columns[i].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      }
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    /// <summary>
    /// ultShowColumn_CellChange
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultShowColumn_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      UltraGridRow row = e.Cell.Row;
      if (!columnName.Equals("ALL", StringComparison.OrdinalIgnoreCase))
      {
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[columnName].Hidden = e.Cell.Text.Equals("0");
        }
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false && e.Cell.Text == string.Empty)
        {
          ultData.DisplayLayout.Bands[0].Columns[columnName].Hidden = true;
        }
      }
      else
      {
        for (int i = 1; i < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          row.Cells[i].Value = e.Cell.Text;
        }
        this.ChkAll_CheckedChange();
      }
    }

    #endregion Event
  }
}
