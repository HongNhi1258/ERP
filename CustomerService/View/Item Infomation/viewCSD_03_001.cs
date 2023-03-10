/*
  Author        : Lâm Quang Hà
  Create date   : 013/10/2010
  Decription    : Search and display Item infomation from Itemcode, DiscontinueFlag, Description, Collection And Category
  Checked by    : Võ Hoa Lư
  Checked date  : 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared.UserControls;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_001 : MainUserControl
  {
    #region Init Data

    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_03_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_03_001_Load(object sender, EventArgs e)
    {
      this.SetAutoSearchWhenPressEnter(groupBox1);
      this.LoadDropDownList();
    }
    #endregion Init Data

    #region Load Data

    /// <summary>
    /// Load dropdownlist
    /// </summary>
    private void LoadDropDownList()
    {
      //Load Collection
      this.LoadCollection();

      //Load Category
      this.LoadCategory();

      ControlUtility.LoadUltraCBExhibition(ultraCBExhibition);
    }

    /// <summary>
    /// Load Collection
    /// </summary>
    private void LoadCollection()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 2 AND DeleteFlag = 0 ORDER BY Value";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DaiCo.Shared.Utility.ControlUtility.LoadMultiCombobox(multiCBCollection, dtSource, "Code", "value");
      multiCBCollection.ColumnWidths = "0, 150";

    }

    /// <summary>
    /// Load Categoty
    /// </summary>
    private void LoadCategory()
    {
      string commandText = "SELECT Pid, Category FROM TblCSDCategory Order By Category ASC";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DaiCo.Shared.Utility.ControlUtility.LoadMultiCombobox(multiCBCategory, dt, "Pid", "Category");
      multiCBCategory.ColumnWidths = "0, 150";
    }

    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
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
        this.Search();
      }
    }

    #endregion Load Data

    #region Search

    /// <summary>
    /// Search Item infomation from ItemCode, DiscontinueFlag, Description, Collection And Category condition
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[8];
      string itemcode = txtItemCode.Text.Trim();
      if (itemcode.Length > 0)
      {
        param[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, "%" + itemcode + "%");
      }
      string saleCode = txtSaleCode.Text.Trim();
      if (saleCode.Length > 0)
      {
        param[1] = new DBParameter("@SaleCode", DbType.AnsiString, 16, "%" + saleCode + "%");
      }
      string itemName = txtItemName.Text.Trim();
      if (itemName.Length > 0)
      {
        param[2] = new DBParameter("@ItemName", DbType.String, 256, "%" + itemName + "%");
      }
      string description = txtDescription.Text.Trim();
      if (description.Length > 0)
      {
        param[3] = new DBParameter("@Description", DbType.String, 4000, "%" + description + "%");
      }
      if (multiCBCollection.SelectedIndex > 0)
      {
        long collection = DBConvert.ParseLong(multiCBCollection.SelectedValue.ToString());
        param[4] = new DBParameter("@Collection", DbType.Int64, collection);
      }
      if (multiCBCategory.SelectedIndex > 0)
      {
        long category = DBConvert.ParseInt(multiCBCategory.SelectedValue.ToString());
        param[5] = new DBParameter("@Category", DbType.Int64, category);
      }

      int flag = DBConvert.ParseInt(cmbStatus.SelectedIndex.ToString());
      if (flag > 0)
      {
        param[6] = new DBParameter("@DiscontinueFlag", DbType.Int32, (flag == 1)? 0 : 1);
      }
      if (ultraCBExhibition.Value != null)
      {
        param[7] = new DBParameter("@Exhibition", DbType.Int32, ultraCBExhibition.Value);
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemInfo_Select", param);
      ultraGridInformation.DataSource = dtSource;
      lbCount.Text = string.Format("Count: {0}", ultraGridInformation.Rows.FilteredInRowCount);
    }

    #endregion Search

    #region Event

    /// <summary>
    /// Close screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Search Item infomation from ItemCode, DiscontinueFlag, Description, Collection And Category
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// View Item Information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridInformation_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      try
      {
        string itemCode = ultraGridInformation.Selected.Rows[0].Cells["ItemCode"].Value.ToString();
        viewCSD_03_002 view = new viewCSD_03_002();
        view.itemCode = itemCode;
        WindowUtinity.ShowView(view, "ITEM INFORMATION", true, ViewState.MainWindow);
      }
      catch { }
    }

    /// <summary>
    /// Init layout for ultragrid view Item Infomation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridInformation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridInformation);
      e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 80;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";      
      e.Layout.Bands[0].Columns["SaleCode"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["SaleCode"].MinWidth = 80;
      e.Layout.Bands[0].Columns["MakertingDescription"].Header.Caption = "Markerting Note";
      e.Layout.Bands[0].Columns["PageNo"].Header.Caption = "Page";
      e.Layout.Bands[0].Columns["PageNo"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["PageNo"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["PageNo"].MinWidth = 40;
      e.Layout.Bands[0].Columns["Exhibition"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["Exhibition"].MinWidth = 80;

      e.Layout.Bands[0].Columns["Status"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Status"].MinWidth = 70;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowUpdate = DefaultableBoolean.False;
    }

    private void ultraCBExhibition_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Code"].Hidden = true;
    }

    private void ultraGridInformation_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
    {
      lbCount.Text = string.Format("Count: {0}", ultraGridInformation.Rows.FilteredInRowCount);
    }
    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultraGridInformation, "ItemInfo");
    }
    #endregion Event       
  }
}
