/*
  Author      : Nguyen Huynh Quoc Tuan
  Date        : 25/5/2015
  Description : Capacity Draft and Confirm
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;
namespace DaiCo.ERPProject
{
  public partial class viewPLN_29_004 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init

    public viewPLN_29_004()
    {
      InitializeComponent();
    }

    private void viewPLN_29_004_Load(object sender, EventArgs e)
    {
      this.SetAutoSearchWhenPressEnter(groupBox1);
    }

    #endregion Init

    #region Function

    /// <summary>
    /// Search Capacity Draft & Confirmed
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;
      DBParameter[] input = new DBParameter[3];
      if (txtYear.Text.Trim().Length > 0)
      {
        input[0] = new DBParameter("@Year", DbType.Int32, DBConvert.ParseInt(txtYear.Text));
      }
      if (txtFrom.Text.Trim().Length > 0)
      {
        input[1] = new DBParameter("@WeekFrom", DbType.Int32, DBConvert.ParseInt(txtFrom.Text));
      }
      if (txtTo.Text.Trim().Length > 0)
      {
        input[2] = new DBParameter("@WeekTo", DbType.Int32, DBConvert.ParseInt(txtTo.Text));
      }
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNProductionPlanCapacity_Select", 600, input);
      if (ds != null)
      {
        ultDataDraft.DataSource = ds.Tables[0];
        ultDataConfirm.DataSource = ds.Tables[1];
        btnSearch.Enabled = true;
      }
    }

    private void ExportExcel()
    {
      if (ultDataDraft.Rows.Count > 0)
      {

        Utility.ExportToExcelWithDefaultPath(ultDataDraft, "Draft");


      }

      if (ultDataConfirm.Rows.Count > 0)
      {


        Utility.ExportToExcelWithDefaultPath(ultDataConfirm, "Confirm");


      }
    }

    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new KeyEventHandler(ctr_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
    }

    void ctr_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        if (txtYear.Text.Trim().Length == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Year");
        }
        else
        {
          this.Search();
        }
      }
    }
    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].Columns["TargetASSY"].Header.Caption = "Target ASSY+SAN";
      e.Layout.Bands[0].Columns["ASSY"].Header.Caption = "ASSY+SAN";

      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultDataDraft_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].Columns["TargetASSY"].Header.Caption = "Target ASSY+SAN";
      e.Layout.Bands[0].Columns["ASSY"].Header.Caption = "ASSY+SAN";

      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      if (txtYear.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Year");
      }
      else
      {
        this.Search();
      }
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }
    #endregion Event
  }
}
