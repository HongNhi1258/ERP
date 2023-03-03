/*
  Author        : Vo Van Duy Qui
  Create date   : 06/09/2012
  Decription    : Item spare part list
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_011 : MainUserControl
  {
    #region Init
    /// <summary>
    /// Init form
    /// </summary>
    public viewCSD_03_011()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_03_011_Load(object sender, EventArgs e)
    {
      ControlUtility.LoadUltraCBCategory(ultCBCategory);
      ControlUtility.LoadUltraCBCollection(ultCBCollection);
      this.LoadItemKind();
    }
    #endregion Init

    #region Function

    private void LoadItemKind()
    {
      string commandText = string.Format("SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND DeleteFlag = 0 ORDER BY Sort", ConstantClass.GROUP_ITEMKIND);
      DataTable dtItemKind = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtItemKind != null && dtItemKind.Rows.Count > 0)
      {
        ControlUtility.LoadUltraCombo(ultCBType, dtItemKind, "Code", "Value", "Code");
        ultCBType.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultCBType.SelectedRow = ultCBType.Rows[0];
      }
    }
    /// <summary>
    /// Check input data for search
    /// </summary>
    /// <returns></returns>
    private bool CheckInputSearch()
    {
      if (ultCBCategory.SelectedRow == null && ultCBCategory.Text.Trim().Length > 0)
      {
        WindowUtinity.ShowMessageError("ERR0011", "Category");
        return false;
      }
      if (ultCBCollection.SelectedRow == null && ultCBCollection.Text.Trim().Length > 0)
      {
        WindowUtinity.ShowMessageError("ERR0011", "Collection");
        return false;
      }
      if (ultCBType.SelectedRow == null && ultCBType.Text.Trim().Length > 0)
      {
        WindowUtinity.ShowMessageError("ERR0011", "Item Kind");
        return false;
      }
      return true;
    }

    /// <summary>
    /// Search item pare part
    /// </summary>
    private void Search()
    {
      if (this.CheckInputSearch())
      {
        string itemCode = txtItemCode.Text.Trim();
        string saleCode = txtSaleCode.Text.Trim();
        int category = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultCBCategory));
        int collection = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultCBCollection));
        int itemKind = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultCBType));

        DBParameter[] inputSearch = new DBParameter[5];
        if (itemCode.Length > 0)
        {
          inputSearch[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, "%" + itemCode + "%");
        }
        if (saleCode.Length > 0)
        {
          inputSearch[1] = new DBParameter("@SaleCode", DbType.AnsiString, 16, "%" + saleCode + "%");
        }
        if (collection != int.MinValue)
        {
          inputSearch[2] = new DBParameter("@Collection", DbType.Int32, collection);
        }
        if (category != int.MinValue)
        {
          inputSearch[3] = new DBParameter("@Category", DbType.Int32, category);
        }
        if (itemKind != int.MinValue)
        {
          inputSearch[4] = new DBParameter("@ItemKind", DbType.Int32, itemKind);
        }

        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemSparepart_Select", inputSearch);
        ultData.DataSource = dtSource;
      }
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Evnet double click in grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      if (ultData.Selected.Rows.Count > 0)
      { 
        string itemCode = ultData.Selected.Rows[0].Cells["ItemCode"].Value.ToString().Trim();
        viewCSD_03_010 view = new viewCSD_03_010();
        view.itemCode = itemCode;
        WindowUtinity.ShowView(view, "ITEM SPARE PART INFO", true, ViewState.MainWindow);
      }
    }

    /// <summary>
    /// Initialize layout of ultragrid ultData
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["ItemKind"].Hidden = true;
      e.Layout.Bands[0].Columns["Confirm"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        int confirm = DBConvert.ParseInt(ultData.Rows[i].Cells["Confirm"].Value.ToString());
        if (confirm == 1)
        {
          ultData.Rows[i].Appearance.BackColor = Color.LightGray;
        }
      }
    }

    /// <summary>
    /// Event button 'Search' click
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
    /// Event button 'New' click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewCSD_03_010 view = new viewCSD_03_010();
      WindowUtinity.ShowView(view, "NEW ITEM SPARE PART", true, ViewState.MainWindow);
    }

    /// <summary>
    /// Event key 'Enter' down
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Object_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.Search();
        btnSearch.Enabled = true;
      }
    }

    /// <summary>
    /// Event button 'Close' click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event
  }
}
