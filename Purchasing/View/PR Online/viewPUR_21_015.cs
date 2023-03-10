/*
  Author      : 
  Date        : 02/03/2015
  Description : Material Follow Up
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;

namespace Purchasing
{
  public partial class viewPUR_21_015 : MainUserControl
  {
    #region Field
    public long PROnlinePid = long.MinValue;
    public bool flagHeadDepartment = false;
    #endregion Field

    #region Init
    #endregion Init

    #region Function
    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@PROnlinePid", DbType.Int64, this.PROnlinePid);
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNMaterialFollowUp_Select", input);
      if (ds != null)
      {
        labPRNo.Text = ds.Tables[0].Rows[0]["PROnlineNo"].ToString();
        int status = DBConvert.ParseInt(ds.Tables[0].Rows[0]["Status"].ToString());
        // Detail
        ultData.DataSource = ds.Tables[1];

        // To Mau
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];
          if (DBConvert.ParseDouble(row.Cells["Demand"].Value.ToString()) > 0
              && (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) > DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString())
              || DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString()) < 0))
          {
            row.Appearance.BackColor = Color.Yellow;
          }
        }

        // Set Status
        if (status >= 2 || this.flagHeadDepartment == false)
        {
          btnAdjust.Enabled = false;
        }
        else
        {
          btnAdjust.Enabled = true;
        }
      }
    }

    private bool AdjustData()
    {
      DataTable dtMain = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtMain.Rows.Count; i++)
      {
        DataRow row = dtMain.Rows[i];
        if (row.RowState == DataRowState.Modified)
        {
          DBParameter[] input = new DBParameter[3];
          input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));
          input[1] = new DBParameter("@Demand", DbType.Int64, DBConvert.ParseDouble(row["Demand"].ToString()));
          input[2] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, 0);
          DataBaseAccess.ExecuteStoreProcedure("spPLNMaterialFollowUp_Update", input, output);
          int result = DBConvert.ParseInt(output[0].Value.ToString());
          if (result <= 0)
          {
            return false;
          }
        }
      }
      return true;
    }
    #endregion Function

    #region Event
    public viewPUR_21_015()
    {
      InitializeComponent();
    }

    private void viewPUR_21_015_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialNameEn"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialNameVn"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Issue"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Stock"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PR"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PO"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remain"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Demand"].CellAppearance.BackColor = Color.LightGray;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnAdjust_Click(object sender, EventArgs e)
    {
      bool success = this.AdjustData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event 
  }
}
