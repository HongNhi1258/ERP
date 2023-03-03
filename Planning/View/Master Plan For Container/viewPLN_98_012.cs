using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;

namespace DaiCo.Planning
{
  public partial class viewPLN_98_012 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewPLN_98_012()
    {
      InitializeComponent();
    }


    private void ultWIP_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultWOLinkSOBeforeShipped_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;
      e.Layout.Bands[0].Columns["WoInfoPID"].Header.Caption = "WO";
      e.Layout.Bands[0].Columns["WoInfoPID"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultShipped_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ShippedQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultWOLinkSOAfterShipped_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }


    private void ultCancellation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["CancellationQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultResult_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    #endregion Init

    #region Function
    #endregion Function

    #region Event
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;

      // Check Customer PONo
      if (txtCustomerPONo.Text.Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "CustomerPONo");
        btnSearch.Enabled = true;
        return;
      }
      else if (txtItemCode.Text.Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "ItemCode");
        btnSearch.Enabled = true;
        return;
      }

      DBParameter[] input = new DBParameter[2];
      if (txtCustomerPONo.Text.Length > 0)
      {
        input[0] = new DBParameter("@CustomerPONo", DbType.String, txtCustomerPONo.Text);
      }

      if (txtItemCode.Text.Length > 0)
      {
        input[1] = new DBParameter("@ItemCode", DbType.String, txtItemCode.Text);
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNCheckWOLinkSO_Select", input);
      if (dsSource != null)
      {
        ultWIP.DataSource = dsSource.Tables[0];

        for (int i = 0; i < this.ultWIP.Rows.Count; i++)
        {
          UltraGridRow row = this.ultWIP.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 1)
          {
            ultWIP.Rows[i].CellAppearance.BackColor = Color.LightGreen;
          }
        }

        ultCancellation.DataSource = dsSource.Tables[1];

        ultWOLinkSOBeforeShipped.DataSource = dsSource.Tables[2];
        for (int i = 0; i < this.ultWOLinkSOBeforeShipped.Rows.Count; i++)
        {
          UltraGridRow row = this.ultWOLinkSOBeforeShipped.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 1)
          {
            ultWOLinkSOBeforeShipped.Rows[i].CellAppearance.BackColor = Color.LightGreen;
          }
        }

        ultShipped.DataSource = dsSource.Tables[3];
        ultWOLinkSOAfterShipped.DataSource = dsSource.Tables[4];
        for (int i = 0; i < this.ultWOLinkSOAfterShipped.Rows.Count; i++)
        {
          UltraGridRow row = this.ultWOLinkSOAfterShipped.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 1)
          {
            ultWOLinkSOAfterShipped.Rows[i].CellAppearance.BackColor = Color.LightGreen;
          }
        }

        ultResult.DataSource = dsSource.Tables[5];
        for (int i = 0; i < this.ultResult.Rows.Count; i++)
        {
          UltraGridRow row = this.ultResult.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 1)
          {
            ultResult.Rows[i].CellAppearance.BackColor = Color.LightGreen;
          }
        }
      }

      btnSearch.Enabled = true;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event
  }
}
