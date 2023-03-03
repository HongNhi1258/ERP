using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Application;
using System.Globalization;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_008 : MainUserControl
  {
    #region Init
    public viewCSD_04_008()
    {
      InitializeComponent();
    }

    private void viewCSD_04_008_Load(object sender, EventArgs e)
    {
      Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbCollection, 2);
      this.LoadComboBoxCategory();
    }

    /// <summary>
    /// Load ComboBox Category
    /// </summary>
    private void LoadComboBoxCategory()
    {
      string commandText = "SELECT Pid, Category FROM TblCSDCategory";
      DataTable dtSourceCategory = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow newRow = dtSourceCategory.NewRow();
      dtSourceCategory.Rows.InsertAt(newRow, 0);
      cmbCategory.DataSource = dtSourceCategory;
      cmbCategory.DisplayMember = "Category";
      cmbCategory.ValueMember = "Pid";
    }

    private void Search()
    {
      string itemCode = txtItemCode.Text.Trim();
      string name = txtName.Text.Trim();
      string saleCode = txtSaleCode.Text.Trim();
      int collection = int.MinValue;
      if (cmbCollection.SelectedIndex > 0)
      {
        collection = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCollection));
      }
      long category = long.MinValue;
      {
        category = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCategory));
      }

      DBParameter[] input = new DBParameter[5];
      if (itemCode.Length > 0)
      {
        input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 18, "%" + itemCode + "%");
      }
      if (saleCode.Length > 0)
      {
        input[1] = new DBParameter("@SaleCode", DbType.AnsiString, 18, "%" + saleCode + "%");
      }
      if (name.Length > 0)
      {
        input[2] = new DBParameter("@Name", DbType.AnsiString, 130, "%" + name + "%");
      }
      if (collection != int.MinValue)
      {
        input[3] = new DBParameter("@Collection", DbType.Int64, collection);
      }
      if (category != long.MinValue)
      {
        input[4] = new DBParameter("@Category", DbType.Int64, category);
      }

      DBParameter[] output = new DBParameter[2];
      output[0] = new DBParameter("@USD", DbType.Double, 0);
      output[1] = new DBParameter("@GBP", DbType.Double, 0);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDCurrentItemPrice_Select", input, output);
      ultDetail.DataSource = dtSource;
      txtUSD.Text = output[0].Value.ToString().Trim();
      txtGBP.Text = output[1].Value.ToString().Trim();
    }
    #endregion Init

    #region Event
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewCSD_04_002 view = new viewCSD_04_002();
      WindowUtinity.ShowView(view, "Item Price Information", false, ViewState.MainWindow);
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void ultDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultDetail);
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["FactoryCost"].Header.Caption = "Factory Cost";
      e.Layout.Bands[0].Columns["FactoryCost"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DaiCoSale"].Header.Caption = "DaiCo Sale";
      e.Layout.Bands[0].Columns["DaiCoSale"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DaiCoMakup"].Header.Caption = "DaiCo Makup (%)";
      e.Layout.Bands[0].Columns["DaiCoMakup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["USSale"].Header.Caption = "US Sale";
      e.Layout.Bands[0].Columns["USSale"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["USMakup"].Header.Caption = "US Makup (%)";
      e.Layout.Bands[0].Columns["USMakup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UKSale"].Header.Caption = "UK Sale";
      e.Layout.Bands[0].Columns["UKSale"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UKMakup"].Header.Caption = "UK Makup (%)";
      e.Layout.Bands[0].Columns["UKMakup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
    }
    #endregion Event
  }
}
