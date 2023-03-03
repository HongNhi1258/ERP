/*
  Author      : 
  Date        : 
  Description : 
  Standard Code: view_MultiTab.cs
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win.UltraWinGrid;

namespace DaiCo.General
{
  public partial class view_MultiTab : MainUserControl
  {
    #region Feild
    public int iIndex = 0;
    private bool loaded = false;
    #endregion Feild

    #region Init

    public view_MultiTab()
    {
      InitializeComponent();
    }

    private void view_MultiTab_Load(object sender, EventArgs e)
    {
      this.InitTabData();
    }

    #endregion Init

    #region Function

    private void LoadData(int index)
    {
      switch (index)
      {
        case 1:

          break;
        case 2:

          break;
        default:
          break;
      }
    }

    private void InitTabData()
    {
      this.loaded = false;
      switch (iIndex)
      {
        case 1:
          this.LoadData(1);
          break;
        case 2:
          this.LoadData(2);
          break;
        default:
          break;
      }
      this.loaded = true;
    }

    private bool CheckInvalid(int index)
    {
      return true;
    }

    private void SaveData(int index)
    {
      btnSave1.Enabled = false;
      btnSave2.Enabled = false;
      if (this.CheckInvalid(index))
      {
        // 1. Delete data in grid

        // 2. Edit data

        // 3. Load data after edit data
        this.LoadData(index);
      }
      btnSave1.Enabled = true;
      btnSave2.Enabled = true;
    }

    #endregion Function

    #region Event

    private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
    {
      switch (tabControl1.SelectedIndex)
      {
        case 0:
          iIndex = 1;
          break;
        default:
          iIndex = tabControl1.SelectedIndex;
          break;
      }
      this.InitTabData();
    }

    private void ultGrid1_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      /*
      // Hidden column
      e.Layout.Bands[0].Columns["Code"].Hidden = true;
      // Edit caption column
      e.Layout.Bands[0].Columns["Value"].Header.Caption = "Function";
      // Set dropdown in grid
      e.Layout.Bands[0].Columns["Kind"].ValueList = this.ultKind;
      // Set text align value
      e.Layout.Bands[0].Columns["TimeDoIt"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      // Set max width column
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;
      // Set column style checkbox
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      // Set defaulf value
      e.Layout.Bands[0].Columns["Selected"].DefaultCellValue = 0;
      */

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultGrid2_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      /*
      // Hidden column
      e.Layout.Bands[0].Columns["Code"].Hidden = true;
      // Edit caption column
      e.Layout.Bands[0].Columns["Value"].Header.Caption = "Function";
      // Set dropdown in grid
      e.Layout.Bands[0].Columns["Kind"].ValueList = this.ultKind;
      // Set text align value
      e.Layout.Bands[0].Columns["TimeDoIt"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      // Set max width column
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;
      // Set column style checkbox
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      // Set defaulf value
      e.Layout.Bands[0].Columns["Selected"].DefaultCellValue = 0;
      */

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnSave1_Click(object sender, EventArgs e)
    {
      this.SaveData(1);
    }

    private void btnSave2_Click(object sender, EventArgs e)
    {
      this.SaveData(2);
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    #endregion Event
  }
}
