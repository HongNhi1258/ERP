/*
  Author      : Ha Anh
  Description : List Consignee
  Date        : 23-09-2011
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using DaiCo.Shared.DataBaseUtility;
using System.Diagnostics;
using VBReport;
using System.IO;

namespace DaiCo.ERPProject
{
  public partial class viewCSD_02_004 : MainUserControl
  {
    #region Field
    /// <summary>
    /// init
    /// </summary>
    public viewCSD_02_004()
    {
      InitializeComponent();
    }
    #endregion Field

    #region Load Data
    /// <summary>
    /// Load view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_02_004_Load(object sender, EventArgs e)
    {
      this.LoadcbCountry();
      this.search();
    }

    /// <summary>
    /// load combobox country
    /// </summary>
    private void LoadcbCountry()
    {
      string commandText = "SELECT Pid, NationEN FROM TblCSDNation ORDER BY OrderBy ASC";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dt.NewRow();
      dt.Rows.InsertAt(row, 0);
      ultcbCountry.DataSource = dt;
      ultcbCountry.DisplayMember = "NationEN";
      ultcbCountry.ValueMember = "Pid";

      ultcbCountry.DisplayLayout.AutoFitColumns = true;
      ultcbCountry.DisplayLayout.Bands[0].HeaderVisible = false;
      ultcbCountry.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbCountry.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }
    #endregion Load Data

    #region Event
    /// <summary>
    /// event search click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.search();
    }

    /// <summary>
    /// search infomation when search click button
    /// </summary>
    private void search() 
    {
      string consignee = txtConsigneeCode.Text.Trim();
      string name = txtName.Text.Trim();
      string contact = txtContact.Text.Trim();
      string city = txtCity.Text.Trim();
      long country = long.MinValue;

      if (ultcbCountry.Text.Trim().Length > 0 && ultcbCountry.Value != null)
      {
        country = DBConvert.ParseLong(ultcbCountry.Value.ToString());
      }
      else if (ultcbCountry.Text.Trim().Length > 0 && ultcbCountry.Value == null)
      {
        country = 0;
      }

      DBParameter[] inputParam = new DBParameter[6];

      if (consignee != string.Empty)
      {
        inputParam[1] = new DBParameter("@Consignee", DbType.AnsiString, 8, "%" + consignee + "%");
      }
      if (name != string.Empty)
      {
        inputParam[2] = new DBParameter("@Name", DbType.AnsiString, 128, "%" + name + "%");
      }
      if (contact != string.Empty)
      {
        inputParam[3] = new DBParameter("@Contact", DbType.AnsiString, 128, "%" + contact + "%");
      }
      if (city != string.Empty)
      {
        inputParam[4] = new DBParameter("@City", DbType.String, 128, "%" + city + "%");
      }
      if (country != long.MinValue)
      {
        inputParam[5] = new DBParameter("@Country", DbType.Int64, country);
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spCSDConsignee_Search", inputParam);

      ultGrid.DataSource = ds;
    }

    /// <summary>
    /// close tab click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    
    /// <summary>
    /// new consignee click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewCSD_02_003 view = new viewCSD_02_003();
      WindowUtinity.ShowView(view, "Consignee Infomation", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Normal);
      this.search();
    }

    /// <summary>
    /// init grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Utility.SetPropertiesUltraGrid(ultGrid);
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Columns["ConsigneeCode"].Header.Caption = "Consignee Code";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Consignee Name";
      e.Layout.Bands[0].Columns["NationEN"].Header.Caption = "Country";
      e.Layout.Bands[0].Columns["PostalCode"].Header.Caption = "Postal Code";
      e.Layout.Bands[0].Columns["StreetAdress"].Header.Caption = "Street Adress";

      e.Layout.Bands[0].Columns["PostalCode"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// event double click row grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultGrid_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultGrid.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultGrid.Selected.Rows[0];
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());

      viewCSD_02_003 view = new viewCSD_02_003();
      view.consigneePid = pid;
      WindowUtinity.ShowView(view, "Consignee Infomation", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Normal);
      this.search();
    }
    #endregion Event
  }
}
