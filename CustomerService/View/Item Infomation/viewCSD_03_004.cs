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

namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_004 : MainUserControl
  {
    public viewCSD_03_004()
    {
      InitializeComponent();
    }

    private void viewCSD_03_004_Load(object sender, EventArgs e)
    {
      this.LoadDataCombobox();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void ultraGridStockBalance_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridStockBalance);
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["ShortName"].Header.Caption = "Short Name";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["SaleCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["SaleCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 70;
    }

    #region function    
    /// <summary>
    /// Search data information
    /// </summary>
    private void Search()
    {
      string itemCode = txtItemCode.Text.Trim();
      string saleCode = txtSaleCode.Text.Trim();
      string name = txtName.Text.Trim();
      string shortName = txtShortName.Text.Trim();
      long category = int.MinValue;
      if (cmbCategory.SelectedIndex > 0)
      {
        category = DBConvert.ParseInt(cmbCategory.SelectedValue.ToString());
      }
      long collection = int.MinValue;
      if (cmbCollection.SelectedIndex > 0)
      {
        collection = DBConvert.ParseInt(cmbCollection.SelectedValue.ToString());
      }

      DBParameter[] inputParam = new DBParameter[6];
      if (itemCode.Length > 0)
      {
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, "%" + itemCode + "%");
      }
      if (saleCode.Length > 0)
      {
        inputParam[1] = new DBParameter("@SaleCode", DbType.AnsiString, 16, "%" + saleCode + "%");
      }
      if (name.Length > 0)
      {
        inputParam[2] = new DBParameter("@Name", DbType.AnsiString, 256, "%" + name + "%");
      }
      if (shortName.Length > 0)
      {
        inputParam[3] = new DBParameter("@ShortName", DbType.AnsiString, 128, "%" + shortName + "%");
      }
      if (category != int.MinValue)
      {
        inputParam[4] = new DBParameter("@Category", DbType.Int64, category);
      }
      if (collection != int.MinValue)
      {
        inputParam[5] = new DBParameter("@Collection", DbType.Int64, collection);
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemStockBalanceList", inputParam);
      ultraGridStockBalance.DataSource = dtSource;
    }

    /// <summary>
    /// Load category and collection
    /// </summary>
    private void LoadDataCombobox()
    {
      // Load data category
      Shared.Utility.ControlUtility.LoadComboboxCategory(cmbCategory);
      // Load data collection
      Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbCollection, 2);
    }
    #endregion function
  }
}
