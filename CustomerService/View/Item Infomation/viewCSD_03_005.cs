using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Application;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_005 : MainUserControl
  {
    public viewCSD_03_005()
    {
      InitializeComponent();
    }

    #region function
    private void Search()
    {
      string itemCode = txtItemCode.Text.Trim();
      string saleCode = txtSaleCode.Text.Trim();
      string name = txtName.Text.Trim();
      string shortName = txtShortName.Text.Trim();
      int fromYear = DBConvert.ParseInt(txtFromYear.Text);
      int toYear = DBConvert.ParseInt(txtToYear.Text);
      int fromMonth = DBConvert.ParseInt(txtFromMonth.Text);
      int toMonth = DBConvert.ParseInt(txtToMonth.Text);
      DBParameter[] inputParam = new DBParameter[8];
      if(itemCode.Length > 0)
      {
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, "%" + itemCode + "%");
      }
      if (saleCode.Length > 0)
      {
        inputParam[1] = new DBParameter("saleCode", DbType.AnsiString, 16, "%" + saleCode + "%");
      }
      if (name.Length > 0)
      {
        inputParam[2] = new DBParameter("@Name", DbType.AnsiString, 256, "%" + name + "%");
      }
      if (shortName.Length > 0)
      {
        inputParam[3] = new DBParameter("@ShortName", DbType.AnsiString, 128, "%" + shortName + "%");
      }
      if (fromYear != int.MinValue)
      {
        inputParam[4] = new DBParameter("@FromYear", DbType.Int32, fromYear);
      }
      if (toYear != int.MinValue)
      {
        inputParam[5] = new DBParameter("@ToYear", DbType.Int32, toYear);
      }
      if (fromMonth != int.MinValue)
      {
        inputParam[6] = new DBParameter("@FromMonth", DbType.Int32, fromMonth);
      }
      if (toMonth != int.MinValue)
      {
        inputParam[7] = new DBParameter("@ToMonth", DbType.Int32, toMonth);
      }
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemSaleHistoryList", inputParam);
      ultraGridInformation.DataSource = dtSource;
    }
    #endregion function

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridInformation);
      e.Layout.Bands[0].Columns["Year"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Year"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Month"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Month"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["SaleCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["SaleCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ShortName"].Header.Caption = "Short Name";
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
    }
  }
}
