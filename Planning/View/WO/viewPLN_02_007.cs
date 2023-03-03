/*
 * Author: Nguyen Van Tron
 * Date: 29/12/2011
 * Description: Print Hang Tag
 */

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.Planning;
using DaiCo.Shared.ReportTemplate.Planning;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_02_007 : MainUserControl
  {
    #region function
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      string message = string.Empty;
      bool success = this.CheckInvalid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0115", message);
        return;
      }

      long woFrom = DBConvert.ParseLong(txtWOFrom.Text);
      long woTo = DBConvert.ParseLong(txtWOTo.Text);
      DBParameter[] input = new DBParameter[3];
      input[0] = new DBParameter("@WOFrom", DbType.Int64, woFrom);
      input[1] = new DBParameter("@WOTo", DbType.Int64, woTo);
      if (txtItemCode.Text.Length > 0)
      {
        input[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, "%" + txtItemCode.Text.Replace("'", "''") + "%");
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNReportHangTagForWO", input);
      if (dtSource != null)
      {
        ultraGridWOInformation.DataSource = dtSource;

        for (int i = 0; i < ultraGridWOInformation.Rows.Count; i++)
        {
          if (DBConvert.ParseInt(ultraGridWOInformation.Rows[i].Cells["Status"].Value.ToString()) == 0)
          {
            ultraGridWOInformation.Rows[i].Appearance.BackColor = Color.Pink;
          }
        }
      }
    }
    /// <summary>
    /// Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckInvalid(out string message)
    {
      message = string.Empty;
      if (DBConvert.ParseLong(txtWOFrom.Text) == long.MinValue || DBConvert.ParseLong(txtWOTo.Text) == long.MinValue)
      {
        message = "WorkOrder";
        return false;
      }
      return true;
    }
    #endregion function

    #region event
    public viewPLN_02_007()
    {
      InitializeComponent();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Print
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrint_Click(object sender, EventArgs e)
    {
      DataSet dsMain = new dsPLNCreateHangTag();
      for (int i = 0; i < ultraGridWOInformation.Rows.Count; i++)
      {
        UltraGridRow row = ultraGridWOInformation.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(row.Cells["WoInfoPID"].Value.ToString()));
          inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, row.Cells["ItemCode"].Value.ToString());
          inputParam[2] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));

          DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNCreateHangTag", inputParam);
          if (dsSource != null && dsSource.Tables.Count > 1)
          {
            dsMain.Tables["dtCreateHangTag"].Merge(dsSource.Tables[0]);
            dsMain.Tables["dtItemFinishing"].Merge(dsSource.Tables[1]);
          }
        }
      }

      foreach (DataRow row in dsMain.Tables["dtCreateHangTag"].Rows)
      {
        string imgPath = FunctionUtility.BOMGetItemImage(row["ItemCode"].ToString(), DBConvert.ParseInt(row["Revision"].ToString()));
        row["ItemImage"] = FunctionUtility.ImageToByteArrayWithFormat(imgPath, 380, 1.68, "JPG");
      }

      cptPLNCreateHangTag cpt = new cptPLNCreateHangTag();
      cpt.SetDataSource(dsMain);
      ControlUtility.ViewCrystalReport(cpt);
    }

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridWOInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[0].Columns["WoInfoPID"].Header.Caption = "WO";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["Description"].Header.Caption = "Item Name";

      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["WoInfoPID"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["WoInfoPID"].MinWidth = 150;

      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 100;

      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 100;

      e.Layout.Bands[0].Columns["SaleCode"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["SaleCode"].MinWidth = 200;

      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;

      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 50;

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      int check = chkSelectAll.Checked ? 1 : 0;
      for (int i = 0; i < ultraGridWOInformation.Rows.Count; i++)
      {
        ultraGridWOInformation.Rows[i].Cells["Selected"].Value = check;
      }
    }
    #endregion event

  }
}
